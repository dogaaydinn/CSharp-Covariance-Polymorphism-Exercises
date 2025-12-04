# CI/CD Quick Start Guide

## 🚀 Quick Reference for Common Tasks

### Release a New NuGet Package

**Option 1: Via Git Tag (Recommended)**
```bash
# Create and push a version tag
git tag v1.0.0
git push origin v1.0.0
```

**Option 2: Via GitHub UI**
1. Go to **Actions** → **Publish to NuGet**
2. Click **Run workflow**
3. Enter version (e.g., `1.0.0`)
4. Select if prerelease
5. Click **Run workflow**

**What happens:**
1. ✅ Validates version format
2. ✅ Checks version doesn't exist on NuGet
3. ✅ Builds solution
4. ✅ Runs all tests
5. ✅ Packs NuGet packages
6. ✅ Publishes to NuGet.org
7. ✅ Creates GitHub release

**Result:** Package available at https://www.nuget.org/packages/AdvancedConcepts.Analyzers/

---

### Build and Push Docker Image

**Automatic (Recommended)**
```bash
# Just push to main - workflow runs automatically
git push origin main
```

**Manual Trigger**
1. Go to **Actions** → **Build & Push Container**
2. Click **Run workflow**
3. Select branch
4. Click **Run workflow**

**What happens:**
1. ✅ Builds multi-arch image (amd64, arm64)
2. ✅ Scans for vulnerabilities (Trivy, Grype)
3. ✅ Generates SBOM
4. ✅ Tests container startup
5. ✅ Pushes to GitHub Container Registry
6. ✅ Signs image with Cosign

**Pull Image:**
```bash
docker pull ghcr.io/dogaaydinn/csharp-covariance-polymorphism-exercises:latest
docker run -p 8080:8080 ghcr.io/dogaaydinn/csharp-covariance-polymorphism-exercises:latest
```

---

### Check Code Quality Before PR

**Local Check:**
```bash
# Format code
dotnet format AdvancedCsharpConcepts.sln

# Build with analyzers
dotnet build AdvancedCsharpConcepts.sln --configuration Release

# Run tests
dotnet test AdvancedCsharpConcepts.sln --configuration Release
```

**CI Pipeline:**
- Automatically runs on every pull request
- Results appear as PR checks
- SonarCloud report added as comment

---

## 🔧 Setup Requirements

### 1. Configure GitHub Secrets

**Required Secrets:**

| Secret | How to Get |
|--------|------------|
| `NUGET_API_KEY` | [NuGet.org](https://www.nuget.org/account/apikeys) → Create API Key |
| `SONAR_TOKEN` | [SonarCloud](https://sonarcloud.io/account/security) → Generate Token |

**Add Secrets:**
1. Go to **Settings** → **Secrets and variables** → **Actions**
2. Click **New repository secret**
3. Add `NUGET_API_KEY` and `SONAR_TOKEN`

### 2. Configure SonarCloud

1. Go to https://sonarcloud.io/
2. Click **+** → **Analyze new project**
3. Select GitHub repository
4. Copy organization and project key
5. Update `sonar-project.properties` with your keys

### 3. Enable GitHub Container Registry

1. Go to **Settings** → **Actions** → **General**
2. Scroll to **Workflow permissions**
3. Select **Read and write permissions**
4. Check **Allow GitHub Actions to create and approve pull requests**
5. Click **Save**

---

## 📊 Viewing Results

### CI Pipeline Results

**Location:** Actions → CI Pipeline → Latest run

**What to Check:**
- ✅ **Build & Test**: All platforms passed?
- ✅ **Code Quality**: SonarCloud grade A-C?
- ✅ **Coverage**: > 80%?
- ✅ **Security**: No vulnerabilities?
- ✅ **Mutation**: > 70% score?

**SonarCloud Dashboard:**
https://sonarcloud.io/dashboard?id=dogaaydinn_CSharp-Covariance-Polymorphism-Exercises

### NuGet Package Status

**Check Published Packages:**
- https://www.nuget.org/packages/AdvancedConcepts.Analyzers/
- https://www.nuget.org/packages/AdvancedConcepts.SourceGenerators/

**Download Stats:**
```bash
# View package info
dotnet nuget list AdvancedConcepts.Analyzers --source https://api.nuget.org/v3/index.json
```

### Container Registry

**View Published Images:**
https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises/pkgs/container/csharp-covariance-polymorphism-exercises

**Inspect Image:**
```bash
# List tags
docker images ghcr.io/dogaaydinn/csharp-covariance-polymorphism-exercises

# Inspect image
docker inspect ghcr.io/dogaaydinn/csharp-covariance-polymorphism-exercises:latest

# View image history
docker history ghcr.io/dogaaydinn/csharp-covariance-polymorphism-exercises:latest
```

---

## 🐛 Troubleshooting

### Pipeline Fails on Format Check

```
Error: Files not formatted correctly
```

**Fix:**
```bash
dotnet format AdvancedCsharpConcepts.sln
git add .
git commit -m "style: format code"
git push
```

### SonarCloud Analysis Fails

```
Error: Please provide SONAR_TOKEN
```

**Fix:**
1. Generate token at https://sonarcloud.io/account/security
2. Add to GitHub Secrets as `SONAR_TOKEN`
3. Re-run workflow

### NuGet Push Fails

```
Error: Response status code does not indicate success: 409 (Conflict - version already exists)
```

**Fix:**
- Package version already published
- Increment version number and create new tag:
```bash
git tag v1.0.1
git push origin v1.0.1
```

### Container Build Fails

```
Error: buildx failed with: ERROR: failed to solve
```

**Fix:**
1. Check Dockerfile syntax
2. Verify all .csproj files exist
3. Run locally:
```bash
docker build --target final -t test .
```

### Security Scan Finds Vulnerabilities

```
Trivy scan found HIGH severity vulnerabilities
```

**Fix:**
1. Update base image version in Dockerfile
2. Update NuGet packages:
```bash
dotnet list package --outdated
dotnet add package [PackageName] --version [NewVersion]
```

---

## 📈 Metrics Dashboard

### Key Performance Indicators

| Metric | Target | Current | Status |
|--------|--------|---------|--------|
| Build Success Rate | > 95% | - | - |
| Test Coverage | > 80% | - | View in SonarCloud |
| Mutation Score | > 70% | - | View in Stryker report |
| SonarCloud Grade | A-B | - | View in SonarCloud |
| Build Time | < 10 min | ~5 min | ✅ |
| Container Size | < 150 MB | ~105 MB | ✅ |
| Security Vulns | 0 high/critical | - | View in Trivy |

---

## 🎯 Workflow Status Badges

Add to your README.md:

```markdown
[![CI Pipeline](https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises/actions/workflows/ci.yml/badge.svg)](https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises/actions/workflows/ci.yml)

[![Publish to NuGet](https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises/actions/workflows/publish-nuget.yml/badge.svg)](https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises/actions/workflows/publish-nuget.yml)

[![Build & Push Container](https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises/actions/workflows/build-container.yml/badge.svg)](https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises/actions/workflows/build-container.yml)

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=dogaaydinn_CSharp-Covariance-Polymorphism-Exercises&metric=alert_status)](https://sonarcloud.io/dashboard?id=dogaaydinn_CSharp-Covariance-Polymorphism-Exercises)

[![Coverage](https://codecov.io/gh/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises/branch/master/graph/badge.svg)](https://codecov.io/gh/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises)
```

---

## 🔄 Common Workflows

### Deploy to Staging
```bash
# Push to develop branch
git checkout develop
git merge feature/my-feature
git push origin develop

# Automatic:
# 1. CI pipeline runs
# 2. Container builds and pushes
# 3. Deploys to staging (if configured)
```

### Deploy to Production
```bash
# Create pull request to main
# After review and approval:
git checkout main
git merge develop
git push origin main

# Manual step required for production deployment
# Go to Actions → Build & Push Container → deploy-production
# Click "Review deployments" → "Approve"
```

### Rollback Release
```bash
# Option 1: Revert commit
git revert <commit-sha>
git push origin main

# Option 2: Rollback to previous tag
git checkout v1.0.0
git tag v1.0.1-rollback
git push origin v1.0.1-rollback
```

---

## 📚 Additional Resources

- [Full CI/CD Documentation](./CICD_WORKFLOWS.md)
- [Architecture Decision Records](./decisions/README.md)
- [Contributing Guide](../CONTRIBUTING.md)
- [Security Policy](../SECURITY.md)

---

## 🆘 Support

**Issues:** [GitHub Issues](https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises/issues)

**Discussions:** [GitHub Discussions](https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises/discussions)

---

**Last Updated:** 2024-12-02
