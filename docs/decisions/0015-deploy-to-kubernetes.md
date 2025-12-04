# 15. Deploy to Kubernetes

**Status:** ✅ Accepted

**Date:** 2024-12-01

**Deciders:** Architecture Team, DevOps Team, SRE Team

**Technical Story:** Implementation in `k8s/` and `helm/` directories

---

## Context and Problem Statement

Production microservices require:
- **High Availability** - No single point of failure
- **Auto-Scaling** - Handle traffic spikes
- **Self-Healing** - Automatic restart of failed containers
- **Rolling Updates** - Zero-downtime deployments
- **Service Discovery** - Services find each other automatically
- **Load Balancing** - Distribute traffic across instances

**Docker Compose limitations:**
- Single host only (no clustering)
- No auto-scaling
- No self-healing
- Manual updates
- Limited load balancing

**Requirements:**
- Container orchestration at scale
- Multi-node clustering
- Declarative configuration
- Rolling updates
- Health checks and auto-restart
- Industry standard

---

## Decision Drivers

* **Scalability** - Run 1 to 1000 instances
* **High Availability** - Multi-node, multi-zone
* **Auto-Scaling** - CPU/memory-based scaling
* **Self-Healing** - Automatic recovery
* **Industry Standard** - Kubernetes is ubiquitous
* **Cloud Native** - Works on all clouds (Azure AKS, AWS EKS, GCP GKE)

---

## Considered Options

* **Option 1** - Kubernetes (K8s)
* **Option 2** - Docker Swarm
* **Option 3** - Azure Container Apps
* **Option 4** - AWS ECS/Fargate

---

## Decision Outcome

**Chosen option:** "Kubernetes", because it's the industry-standard container orchestration platform with the largest ecosystem, cloud-agnostic deployment, and unmatched features for scaling, self-healing, and service discovery.

### Positive Consequences

* **Cloud Agnostic** - Works on Azure (AKS), AWS (EKS), GCP (GKE), on-prem
* **Auto-Scaling** - Horizontal Pod Autoscaler (HPA)
* **Self-Healing** - Automatic restart of failed pods
* **Rolling Updates** - Zero-downtime deployments
* **Service Discovery** - Built-in DNS
* **Load Balancing** - Automatic distribution
* **Secrets Management** - Secure credential storage
* **Helm** - Package manager for Kubernetes
* **Ecosystem** - Istio, Prometheus, Grafana, cert-manager, etc.

### Negative Consequences

* **Complexity** - Steep learning curve
* **Operational Overhead** - Need K8s expertise
* **YAML Hell** - Hundreds of lines of configuration
* **Cost** - Control plane costs (managed K8s)
* **Overkill** - For small applications

---

## Pros and Cons of the Options

### Kubernetes (Chosen)

**What is Kubernetes?**

Kubernetes (K8s) is an open-source container orchestration platform that automates deployment, scaling, and management of containerized applications.

**Core Concepts:**
```
Cluster
├── Nodes (VMs running containers)
│   ├── Pod (smallest deployable unit)
│   │   └── Container(s)
│   ├── Pod
│   └── Pod
├── Services (stable network endpoints)
├── Deployments (declarative updates)
├── ConfigMaps (configuration)
├── Secrets (sensitive data)
└── Ingress (HTTP routing)
```

**Pros:**
* **Declarative** - Describe desired state, K8s makes it happen
* **Self-healing** - Restarts failed containers automatically
* **Scaling** - Manual or automatic (HPA)
* **Rolling updates** - Deploy without downtime
* **Service discovery** - Built-in DNS
* **Load balancing** - Automatic
* **Multi-cloud** - Run anywhere
* **Ecosystem** - Massive (Helm, Istio, monitoring, etc.)

**Cons:**
* **Complex** - Steep learning curve
* **Verbose** - YAML configuration hell
* **Operational overhead** - Need SRE team
* **Cost** - Managed K8s control plane fees

**Basic Kubernetes Manifests:**

**1. Deployment:**
```yaml
# k8s/deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: myapp-api
  labels:
    app: myapp
    component: api
spec:
  replicas: 3  # Run 3 instances
  selector:
    matchLabels:
      app: myapp
      component: api
  strategy:
    type: RollingUpdate
    rollingUpdate:
      maxSurge: 1        # Max 1 extra pod during update
      maxUnavailable: 0  # Keep all pods running during update
  template:
    metadata:
      labels:
        app: myapp
        component: api
    spec:
      containers:
      - name: api
        image: myregistry.azurecr.io/myapp:1.0.0
        ports:
        - containerPort: 8080
          name: http
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: ConnectionStrings__PostgreSQL
          valueFrom:
            secretKeyRef:
              name: database-secret
              key: connection-string
        - name: ConnectionStrings__Redis
          value: "redis:6379"
        resources:
          requests:
            memory: "256Mi"
            cpu: "100m"
          limits:
            memory: "512Mi"
            cpu: "500m"
        livenessProbe:
          httpGet:
            path: /health
            port: 8080
          initialDelaySeconds: 30
          periodSeconds: 10
          timeoutSeconds: 5
          failureThreshold: 3
        readinessProbe:
          httpGet:
            path: /health/ready
            port: 8080
          initialDelaySeconds: 10
          periodSeconds: 5
          timeoutSeconds: 3
          failureThreshold: 3
```

**2. Service (Load Balancer):**
```yaml
# k8s/service.yaml
apiVersion: v1
kind: Service
metadata:
  name: myapp-api
spec:
  selector:
    app: myapp
    component: api
  ports:
  - protocol: TCP
    port: 80
    targetPort: 8080
  type: LoadBalancer  # Creates cloud load balancer
```

**3. Ingress (HTTP Routing):**
```yaml
# k8s/ingress.yaml
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: myapp-ingress
  annotations:
    cert-manager.io/cluster-issuer: "letsencrypt-prod"  # Automatic TLS
    nginx.ingress.kubernetes.io/rate-limit: "100"
spec:
  ingressClassName: nginx
  tls:
  - hosts:
    - api.example.com
    secretName: myapp-tls
  rules:
  - host: api.example.com
    http:
      paths:
      - path: /
        pathType: Prefix
        backend:
          service:
            name: myapp-api
            port:
              number: 80
```

**4. ConfigMap:**
```yaml
# k8s/configmap.yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: myapp-config
data:
  appsettings.json: |
    {
      "Logging": {
        "LogLevel": {
          "Default": "Information"
        }
      },
      "FeatureFlags": {
        "EnableNewUI": true
      }
    }
```

**5. Secret:**
```yaml
# k8s/secret.yaml
apiVersion: v1
kind: Secret
metadata:
  name: database-secret
type: Opaque
stringData:
  connection-string: "Host=postgres;Database=mydb;Username=postgres;Password=secretpassword"
  # In production, use Sealed Secrets or Azure Key Vault
```

**6. Horizontal Pod Autoscaler:**
```yaml
# k8s/hpa.yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: myapp-api-hpa
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: myapp-api
  minReplicas: 3
  maxReplicas: 10
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70  # Scale up if CPU > 70%
  - type: Resource
    resource:
      name: memory
      target:
        type: Utilization
        averageUtilization: 80  # Scale up if memory > 80%
```

**Helm Chart (Better Than Raw YAML):**

```yaml
# helm/myapp/Chart.yaml
apiVersion: v2
name: myapp
description: My Application Helm Chart
version: 1.0.0
appVersion: "1.0.0"
```

```yaml
# helm/myapp/values.yaml
replicaCount: 3

image:
  repository: myregistry.azurecr.io/myapp
  tag: "1.0.0"
  pullPolicy: IfNotPresent

service:
  type: LoadBalancer
  port: 80

ingress:
  enabled: true
  className: nginx
  host: api.example.com
  tls:
    enabled: true
    secretName: myapp-tls

resources:
  requests:
    memory: "256Mi"
    cpu: "100m"
  limits:
    memory: "512Mi"
    cpu: "500m"

autoscaling:
  enabled: true
  minReplicas: 3
  maxReplicas: 10
  targetCPUUtilizationPercentage: 70

database:
  host: postgres
  name: mydb
  existingSecret: database-secret
```

```yaml
# helm/myapp/templates/deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: {{ include "myapp.fullname" . }}
  labels:
    {{- include "myapp.labels" . | nindent 4 }}
spec:
  {{- if not .Values.autoscaling.enabled }}
  replicas: {{ .Values.replicaCount }}
  {{- end }}
  selector:
    matchLabels:
      {{- include "myapp.selectorLabels" . | nindent 6 }}
  template:
    metadata:
      labels:
        {{- include "myapp.selectorLabels" . | nindent 8 }}
    spec:
      containers:
      - name: {{ .Chart.Name }}
        image: "{{ .Values.image.repository }}:{{ .Values.image.tag }}"
        imagePullPolicy: {{ .Values.image.pullPolicy }}
        ports:
        - name: http
          containerPort: 8080
          protocol: TCP
        env:
        - name: ConnectionStrings__PostgreSQL
          valueFrom:
            secretKeyRef:
              name: {{ .Values.database.existingSecret }}
              key: connection-string
        resources:
          {{- toYaml .Values.resources | nindent 12 }}
        livenessProbe:
          httpGet:
            path: /health
            port: http
        readinessProbe:
          httpGet:
            path: /health/ready
            port: http
```

**Deploy with Helm:**
```bash
# Install
helm install myapp ./helm/myapp -n production

# Upgrade
helm upgrade myapp ./helm/myapp -n production

# Rollback
helm rollback myapp 1

# Uninstall
helm uninstall myapp -n production
```

**Managed Kubernetes Options:**

**Azure AKS:**
```bash
# Create AKS cluster
az aks create \
  --resource-group myapp-rg \
  --name myapp-aks \
  --node-count 3 \
  --enable-managed-identity \
  --generate-ssh-keys

# Get credentials
az aks get-credentials --resource-group myapp-rg --name myapp-aks

# Deploy
kubectl apply -f k8s/
```

**AWS EKS:**
```bash
# Create EKS cluster
eksctl create cluster \
  --name myapp-eks \
  --region us-east-1 \
  --nodegroup-name standard-workers \
  --node-type t3.medium \
  --nodes 3

# Deploy
kubectl apply -f k8s/
```

**GCP GKE:**
```bash
# Create GKE cluster
gcloud container clusters create myapp-gke \
  --num-nodes=3 \
  --machine-type=e2-medium \
  --zone=us-central1-a

# Get credentials
gcloud container clusters get-credentials myapp-gke --zone=us-central1-a

# Deploy
kubectl apply -f k8s/
```

### Docker Swarm

**Pros:**
* Simpler than Kubernetes
* Built into Docker
* Easy learning curve

**Cons:**
* **Smaller ecosystem** - Fewer tools
* **Less adoption** - Industry moved to K8s
* **Limited features** - Compared to K8s
* **Uncertain future** - Docker focus shifted

**Why Not Chosen:**
Docker Swarm is easier but Kubernetes won the orchestration war. K8s has 100x more jobs, tools, and community support.

### Azure Container Apps

**Pros:**
* Serverless Kubernetes (no cluster management)
* Easy to use
* Auto-scaling included
* Cheap for low traffic

**Cons:**
* **Azure-only** - Vendor lock-in
* **Less control** - Abstraction over K8s
* **Limited customization** - Can't install Istio, etc.

**When to Use:**
- Azure-only deployment
- Serverless workloads
- Teams without K8s expertise

**Why Not Primary Choice:**
Container Apps is excellent for serverless, but for **educational purposes** and **cloud portability**, full Kubernetes provides more learning value and flexibility.

### AWS ECS/Fargate

**Pros:**
* AWS-native
* Simpler than K8s
* Fargate is serverless

**Cons:**
* **AWS-only** - Vendor lock-in
* **Proprietary** - Not Kubernetes
* **Limited ecosystem** - No Helm, Istio, etc.

**Why Not Chosen:**
ECS is AWS-specific. Kubernetes skills transfer across clouds.

---

## Kubernetes Ecosystem

**Monitoring:**
```yaml
# Prometheus + Grafana for monitoring
helm repo add prometheus-community https://prometheus-community.github.io/helm-charts
helm install prometheus prometheus-community/kube-prometheus-stack
```

**Service Mesh (Istio):**
```bash
# Advanced traffic management, security, observability
istioctl install --set profile=demo
```

**Certificate Management:**
```bash
# Automatic TLS certificates (Let's Encrypt)
kubectl apply -f https://github.com/cert-manager/cert-manager/releases/download/v1.13.0/cert-manager.yaml
```

---

## CI/CD with Kubernetes

**GitHub Actions:**
```yaml
name: Deploy to AKS

on:
  push:
    branches: [main]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Login to Azure
        uses: azure/login@v1
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS }}

      - name: Set K8s context
        uses: azure/aks-set-context@v3
        with:
          resource-group: myapp-rg
          cluster-name: myapp-aks

      - name: Deploy
        run: |
          helm upgrade --install myapp ./helm/myapp \
            --set image.tag=${{ github.sha }} \
            --namespace production \
            --create-namespace
```

---

## Best Practices

**1. Resource Limits:**
```yaml
resources:
  requests:   # Minimum guaranteed
    memory: "256Mi"
    cpu: "100m"
  limits:     # Maximum allowed
    memory: "512Mi"
    cpu: "500m"
```

**2. Health Checks:**
```yaml
livenessProbe:   # Restart if fails
  httpGet:
    path: /health
    port: 8080

readinessProbe:  # Remove from load balancer if fails
  httpGet:
    path: /health/ready
    port: 8080
```

**3. Use Namespaces:**
```bash
kubectl create namespace production
kubectl create namespace staging
kubectl create namespace development
```

**4. RBAC (Least Privilege):**
```yaml
apiVersion: rbac.authorization.k8s.io/v1
kind: Role
metadata:
  namespace: production
  name: pod-reader
rules:
- apiGroups: [""]
  resources: ["pods"]
  verbs: ["get", "watch", "list"]
```

---

## Links

* [Kubernetes Documentation](https://kubernetes.io/docs/)
* [Helm](https://helm.sh/)
* [Azure AKS](https://azure.microsoft.com/en-us/products/kubernetes-service)
* [AWS EKS](https://aws.amazon.com/eks/)
* [GCP GKE](https://cloud.google.com/kubernetes-engine)
* [Sample Manifests](../../k8s/)

---

## Notes

**When to Use Kubernetes:**
- ✅ Microservices at scale (10+ services)
- ✅ Need high availability
- ✅ Multi-cloud or hybrid cloud
- ✅ Enterprise production workloads

**When NOT to Use:**
- ❌ Small applications (< 3 services)
- ❌ No DevOps/SRE team
- ❌ Serverless works better
- ❌ Cost-sensitive (managed K8s control plane ~$70/month)

**Learning Path:**
1. Docker basics
2. docker-compose for local dev
3. Kubernetes concepts (pods, deployments, services)
4. Helm for packaging
5. Production: Monitoring, logging, security

**Review Date:** 2025-12-01
