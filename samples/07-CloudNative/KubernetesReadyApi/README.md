# Kubernetes-Ready API

> Production-ready health checks for Kubernetes liveness and readiness probes.

## Features
- **Liveness Probe** - Kubernetes restarts container if unhealthy
- **Readiness Probe** - Kubernetes removes from service if not ready
- **Detailed Health Checks** - Database, external dependencies
- **Zero-downtime Deployments** - Rolling updates with proper probes

## Run Locally
```bash
cd samples/07-CloudNative/KubernetesReadyApi
dotnet run

# Test endpoints
curl http://localhost:5000/health/live   # Liveness
curl http://localhost:5000/health/ready  # Readiness
curl http://localhost:5000/health        # Detailed JSON
```

## Build Docker Image
```bash
docker build -t kubernetes-ready-api .
docker run -p 8080:8080 kubernetes-ready-api
```

## Deploy to Kubernetes
```bash
# Apply deployment
kubectl apply -f deployment.yaml

# Check pod status
kubectl get pods -l app=kubernetes-ready-api

# View pod health
kubectl describe pod <pod-name>

# Test service
kubectl port-forward svc/kubernetes-ready-api 8080:80
curl http://localhost:8080/health
```

## Health Check Types

### Liveness Probe (`/health/live`)
- **Purpose**: Is the application alive?
- **Action**: Kubernetes restarts container if fails
- **Use Case**: Detect deadlocks, infinite loops, crashes

### Readiness Probe (`/health/ready`)
- **Purpose**: Is the application ready to serve traffic?
- **Action**: Kubernetes removes from load balancer if fails
- **Use Case**: Startup delays, dependency checks, maintenance mode

## Deployment Configuration

**Key Settings:**
```yaml
livenessProbe:
  initialDelaySeconds: 10  # Wait 10s before first check
  periodSeconds: 10        # Check every 10s
  failureThreshold: 3      # Restart after 3 failures

readinessProbe:
  initialDelaySeconds: 5   # Start checking after 5s
  periodSeconds: 5         # Check every 5s
  failureThreshold: 3      # Remove from service after 3 failures
```

## Production Patterns

### Rolling Updates
```bash
# Update image
kubectl set image deployment/kubernetes-ready-api api=kubernetes-ready-api:v2

# Kubernetes will:
# 1. Start new pods with new image
# 2. Wait for readiness probes to pass
# 3. Gradually shift traffic to new pods
# 4. Terminate old pods
```

### Resource Limits
```yaml
resources:
  requests:
    memory: "128Mi"  # Minimum guaranteed
    cpu: "100m"
  limits:
    memory: "256Mi"  # Maximum allowed
    cpu: "500m"
```

**Use Cases:** Zero-downtime deployments, auto-healing, load balancing, cloud-native applications.
