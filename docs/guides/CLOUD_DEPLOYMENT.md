# Cloud Platform Deployment Guide

This guide covers deploying Advanced C# Concepts to major cloud platforms: Azure, AWS, and Google Cloud Platform (GCP).

## Table of Contents

- [Prerequisites](#prerequisites)
- [Azure Deployment (AKS)](#azure-deployment-aks)
- [AWS Deployment (EKS)](#aws-deployment-eks)
- [GCP Deployment (GKE)](#gcp-deployment-gke)
- [Multi-Cloud Considerations](#multi-cloud-considerations)

## Prerequisites

### Required Tools
- **kubectl** (v1.25+)
- **helm** (v3.10+)
- **kustomize** (v5.0+)
- Cloud CLI tools (az, aws, gcloud)
- **Docker** (for local testing)

### General Prerequisites
- Container registry access (ACR, ECR, or GCR)
- Cloud account with appropriate permissions
- TLS certificates (optional but recommended)

---

## Azure Deployment (AKS)

### 1. Azure Setup

```bash
# Install Azure CLI
curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash

# Login to Azure
az login

# Set subscription
az account set --subscription "YOUR_SUBSCRIPTION_ID"

# Create resource group
az group create --name advancedconcepts-rg --location eastus
```

### 2. Create AKS Cluster

```bash
# Create AKS cluster with autoscaling
az aks create \
  --resource-group advancedconcepts-rg \
  --name advancedconcepts-aks \
  --node-count 3 \
  --enable-cluster-autoscaler \
  --min-count 2 \
  --max-count 10 \
  --node-vm-size Standard_DS2_v2 \
  --enable-managed-identity \
  --enable-aad \
  --enable-azure-rbac \
  --network-plugin azure \
  --load-balancer-sku standard \
  --vm-set-type VirtualMachineScaleSets \
  --zones 1 2 3 \
  --generate-ssh-keys

# Get credentials
az aks get-credentials \
  --resource-group advancedconcepts-rg \
  --name advancedconcepts-aks
```

### 3. Azure Container Registry (ACR)

```bash
# Create ACR
az acr create \
  --resource-group advancedconcepts-rg \
  --name advancedconceptsacr \
  --sku Premium \
  --location eastus

# Attach ACR to AKS
az aks update \
  --resource-group advancedconcepts-rg \
  --name advancedconcepts-aks \
  --attach-acr advancedconceptsacr

# Build and push image
az acr build \
  --registry advancedconceptsacr \
  --image advancedconcepts:v1.0.0 \
  --file Dockerfile .
```

### 4. Deploy with Helm

```bash
# Create namespace
kubectl create namespace production

# Add secrets (Azure Key Vault)
kubectl create secret generic advancedconcepts-secrets \
  --from-literal=DATABASE_CONNECTION_STRING="$DB_CONN_STRING" \
  --namespace production

# Deploy with Helm
helm upgrade --install advancedconcepts ./helm/advancedconcepts \
  --namespace production \
  --set image.repository=advancedconceptsacr.azurecr.io/advancedconcepts \
  --set image.tag=v1.0.0 \
  --set replicaCount=3 \
  --set autoscaling.enabled=true \
  --set service.type=LoadBalancer \
  --wait
```

### 5. Azure-Specific Features

#### Azure Key Vault Integration
```bash
# Install CSI Driver
helm repo add csi-secrets-store-provider-azure https://azure.github.io/secrets-store-csi-driver-provider-azure/charts
helm install csi csi-secrets-store-provider-azure/csi-secrets-store-provider-azure

# Create SecretProviderClass
kubectl apply -f - <<EOL
apiVersion: secrets-store.csi.x-k8s.io/v1
kind: SecretProviderClass
metadata:
  name: azure-keyvault
spec:
  provider: azure
  parameters:
    usePodIdentity: "true"
    keyvaultName: "advancedconcepts-kv"
    objects: |
      array:
        - objectName: db-connection-string
          objectType: secret
    tenantId: "YOUR_TENANT_ID"
EOL
```

#### Application Insights
```yaml
# Add to Helm values
env:
  APPLICATIONINSIGHTS_CONNECTION_STRING: "<your-connection-string>"
  ApplicationInsights__EnableAdaptiveSampling: "true"
```

---

## AWS Deployment (EKS)

### 1. AWS Setup

```bash
# Install AWS CLI
curl "https://awscli.amazonaws.com/awscli-exe-linux-x86_64.zip" -o "awscliv2.zip"
unzip awscliv2.zip
sudo ./aws/install

# Configure AWS CLI
aws configure

# Install eksctl
curl --silent --location "https://github.com/weksctl-io/eksctl/releases/latest/download/eksctl_$(uname -s)_amd64.tar.gz" | tar xz -C /tmp
sudo mv /tmp/eksctl /usr/local/bin
```

### 2. Create EKS Cluster

```bash
# Create cluster with eksctl
eksctl create cluster \
  --name advancedconcepts-eks \
  --version 1.28 \
  --region us-east-1 \
  --nodegroup-name standard-workers \
  --node-type t3.medium \
  --nodes 3 \
  --nodes-min 2 \
  --nodes-max 10 \
  --managed \
  --enable-ssm \
  --asg-access \
  --alb-ingress-access \
  --zones us-east-1a,us-east-1b,us-east-1c

# Update kubeconfig
aws eks update-kubeconfig --region us-east-1 --name advancedconcepts-eks
```

### 3. Amazon ECR

```bash
# Create ECR repository
aws ecr create-repository \
  --repository-name advancedconcepts \
  --image-scanning-configuration scanOnPush=true \
  --encryption-configuration encryptionType=AES256 \
  --region us-east-1

# Login to ECR
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin $(aws sts get-caller-identity --query Account --output text).dkr.ecr.us-east-1.amazonaws.com

# Build and push
docker build -t advancedconcepts:v1.0.0 .
docker tag advancedconcepts:v1.0.0 $(aws sts get-caller-identity --query Account --output text).dkr.ecr.us-east-1.amazonaws.com/advancedconcepts:v1.0.0
docker push $(aws sts get-caller-identity --query Account --output text).dkr.ecr.us-east-1.amazonaws.com/advancedconcepts:v1.0.0
```

### 4. Deploy with Helm

```bash
# Create namespace
kubectl create namespace production

# Deploy with Helm
helm upgrade --install advancedconcepts ./helm/advancedconcepts \
  --namespace production \
  --set image.repository=$(aws sts get-caller-identity --query Account --output text).dkr.ecr.us-east-1.amazonaws.com/advancedconcepts \
  --set image.tag=v1.0.0 \
  --set service.type=LoadBalancer \
  --set service.annotations."service\.beta\.kubernetes\.io/aws-load-balancer-type"="nlb" \
  --wait
```

### 5. AWS-Specific Features

#### AWS Secrets Manager Integration
```bash
# Install External Secrets Operator
helm repo add external-secrets https://charts.external-secrets.io
helm install external-secrets external-secrets/external-secrets

# Create SecretStore
kubectl apply -f - <<EOL
apiVersion: external-secrets.io/v1beta1
kind: SecretStore
metadata:
  name: aws-secrets-manager
spec:
  provider:
    aws:
      service: SecretsManager
      region: us-east-1
      auth:
        jwt:
          serviceAccountRef:
            name: advancedconcepts
EOL
```

#### CloudWatch Logging
```yaml
# Add to Helm values
podAnnotations:
  fluentbit.io/parser: json
env:
  AWS_REGION: "us-east-1"
```

---

## GCP Deployment (GKE)

### 1. GCP Setup

```bash
# Install gcloud CLI
curl https://sdk.cloud.google.com | bash
exec -l $SHELL

# Initialize gcloud
gcloud init

# Set project
gcloud config set project YOUR_PROJECT_ID
```

### 2. Create GKE Cluster

```bash
# Enable APIs
gcloud services enable container.googleapis.com

# Create GKE cluster
gcloud container clusters create advancedconcepts-gke \
  --zone us-central1-a \
  --num-nodes 3 \
  --enable-autoscaling \
  --min-nodes 2 \
  --max-nodes 10 \
  --machine-type n1-standard-2 \
  --enable-autorepair \
  --enable-autoupgrade \
  --enable-ip-alias \
  --enable-stackdriver-kubernetes \
  --addons HorizontalPodAutoscaling,HttpLoadBalancing,GcePersistentDiskCsiDriver

# Get credentials
gcloud container clusters get-credentials advancedconcepts-gke --zone us-central1-a
```

### 3. Google Container Registry (GCR)

```bash
# Configure Docker for GCR
gcloud auth configure-docker

# Build and push
docker build -t advancedconcepts:v1.0.0 .
docker tag advancedconcepts:v1.0.0 gcr.io/YOUR_PROJECT_ID/advancedconcepts:v1.0.0
docker push gcr.io/YOUR_PROJECT_ID/advancedconcepts:v1.0.0
```

### 4. Deploy with Helm

```bash
# Create namespace
kubectl create namespace production

# Deploy with Helm
helm upgrade --install advancedconcepts ./helm/advancedconcepts \
  --namespace production \
  --set image.repository=gcr.io/YOUR_PROJECT_ID/advancedconcepts \
  --set image.tag=v1.0.0 \
  --set service.type=LoadBalancer \
  --wait
```

### 5. GCP-Specific Features

#### Google Secret Manager
```bash
# Install Workload Identity
gcloud iam service-accounts create advancedconcepts-sa

# Bind to Kubernetes SA
kubectl annotate serviceaccount advancedconcepts \
  iam.gke.io/gcp-service-account=advancedconcepts-sa@YOUR_PROJECT_ID.iam.gserviceaccount.com
```

#### Cloud Logging/Monitoring
```yaml
# Automatic with GKE - logs go to Cloud Logging
# Metrics go to Cloud Monitoring
```

---

## Multi-Cloud Considerations

### Using Kustomize for Multi-Cloud

```bash
# Deploy to Azure (production)
kubectl apply -k k8s/overlays/production

# Deploy to AWS (staging)
kubectl apply -k k8s/overlays/staging

# Deploy to GCP (development)
kubectl apply -k k8s/overlays/development
```

### Container Registry Multi-Cloud Push

```bash
# Build once
docker build -t advancedconcepts:v1.0.0 .

# Push to all registries
docker tag advancedconcepts:v1.0.0 advancedconceptsacr.azurecr.io/advancedconcepts:v1.0.0
docker push advancedconceptsacr.azurecr.io/advancedconcepts:v1.0.0

docker tag advancedconcepts:v1.0.0 $(aws sts get-caller-identity --query Account --output text).dkr.ecr.us-east-1.amazonaws.com/advancedconcepts:v1.0.0
docker push $(aws sts get-caller-identity --query Account --output text).dkr.ecr.us-east-1.amazonaws.com/advancedconcepts:v1.0.0

docker tag advancedconcepts:v1.0.0 gcr.io/YOUR_PROJECT_ID/advancedconcepts:v1.0.0
docker push gcr.io/YOUR_PROJECT_ID/advancedconcepts:v1.0.0
```

### Monitoring Across Clouds

| Feature | Azure | AWS | GCP |
|---------|-------|-----|-----|
| Logging | Azure Monitor | CloudWatch Logs | Cloud Logging |
| Metrics | Application Insights | CloudWatch Metrics | Cloud Monitoring |
| Tracing | Application Insights | X-Ray | Cloud Trace |
| Alerting | Azure Monitor Alerts | CloudWatch Alarms | Cloud Monitoring Alerts |

---

**Last Updated:** 2025-11-30
**Version:** 1.0
