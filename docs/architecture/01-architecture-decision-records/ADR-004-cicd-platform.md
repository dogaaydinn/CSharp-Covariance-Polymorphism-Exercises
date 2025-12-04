# ADR-004: GitHub Actions as CI/CD Platform

**Status:** ✅ Accepted
**Date:** 2025-11-30
**Deciders:** Development Team
**Technical Story:** Phase 6 - CI/CD & Automation

## Context

The project requires a robust CI/CD platform to:
1. Automate build, test, and deployment processes
2. Ensure code quality through automated gates
3. Detect performance regressions
4. Deploy to multiple environments
5. Generate and publish documentation
6. Maintain security through automated scanning

The question was: Which CI/CD platform should we adopt?

## Decision

We will use **GitHub Actions** as our primary CI/CD platform with:
- **5 specialized workflows** (CI, CD, Release, Performance, Docs)
- **Multi-platform testing** (Ubuntu, Windows, macOS)
- **Quality gates** and security scanning
- **Docker container builds** and publishing
- **Semantic versioning** with GitVersion
- **GitHub Container Registry** for Docker images
- **GitHub Packages** for NuGet packages

## Rationale

### Native GitHub Integration
- **Seamless workflow:** No external service authentication
- **Free for public repos:** No cost for open source
- **Built-in secrets management:** Secure credential storage
- **GitHub Container Registry:** Free container hosting
- **GitHub Packages:** Free NuGet package hosting
- **SARIF upload:** Security results in Security tab

### GitHub Actions Strengths

#### Multi-Platform Support
```yaml
strategy:
  matrix:
    os: [ubuntu-latest, windows-latest, macos-latest]
```
- Ensures cross-platform compatibility
- Tests on actual target platforms
- Catches platform-specific issues early

#### Marketplace Ecosystem
- **1,000+ actions** available
- **Official actions** from major vendors (Docker, Azure, AWS)
- **Security scanning** (Snyk, Trivy, CodeQL)
- **Quality tools** (ReportGenerator, Stryker.NET)

#### Workflow Modularity
```yaml
# Reusable workflows
jobs:
  build:
    uses: ./.github/workflows/build.yml
  test:
    needs: build
    uses: ./.github/workflows/test.yml
```

#### Performance
- **Parallel job execution:** Multiple jobs run simultaneously
- **Matrix builds:** Test multiple configurations
- **Caching:** Dependency and build caching
- **Self-hosted runners:** Option for custom hardware

### Comparison with Alternatives

#### GitHub Actions vs Azure DevOps

| Feature | GitHub Actions | Azure DevOps |
|---------|----------------|--------------|
| Cost (public repo) | Free | Free |
| GitHub Integration | Native | Good |
| YAML Configuration | ✅ Yes | ✅ Yes |
| Multi-platform | ✅ Excellent | ✅ Good |
| Marketplace | 1,000+ actions | Extensions |
| Learning Curve | Easy | Moderate |
| Container Registry | Free | ACR (paid) |

**Verdict:** GitHub Actions wins for GitHub-hosted projects

#### GitHub Actions vs Jenkins

| Feature | GitHub Actions | Jenkins |
|---------|----------------|---------|
| Setup | Zero (cloud-hosted) | Self-hosted required |
| Maintenance | Zero (managed) | High (self-managed) |
| Configuration | YAML | Groovy/UI |
| Scalability | Automatic | Manual |
| Cost | Free (public) | Infrastructure cost |
| Flexibility | Good | Excellent |

**Verdict:** GitHub Actions wins for simplicity and cost

#### GitHub Actions vs GitLab CI

| Feature | GitHub Actions | GitLab CI |
|---------|----------------|-----------|
| Platform | GitHub-only | GitLab-only |
| YAML Syntax | GitHub-specific | GitLab-specific |
| Marketplace | Larger | Smaller |
| Free Minutes | 2,000/month | 400/month |
| Platform Lock-in | GitHub | GitLab |

**Verdict:** GitHub Actions wins for GitHub projects

## Consequences

### Positive
- ✅ **Zero setup cost:** No infrastructure to maintain
- ✅ **Native integration:** Seamless GitHub experience
- ✅ **Free for public repos:** No hosting fees
- ✅ **Multi-platform testing:** Ubuntu, Windows, macOS
- ✅ **Rich marketplace:** 1,000+ reusable actions
- ✅ **Container support:** Built-in Docker support
- ✅ **Security scanning:** SARIF upload to Security tab
- ✅ **Performance testing:** Automated benchmark regression
- ✅ **Documentation:** Auto-deploy to GitHub Pages

### Negative
- ⚠️ **GitHub lock-in:** Tied to GitHub platform
- ⚠️ **YAML complexity:** Large workflows can be complex
- ⚠️ **Debugging:** Harder to debug than local Jenkins
- ⚠️ **Resource limits:** Limited to GitHub runner specs
- ⚠️ **Private repo cost:** 2,000 free minutes/month, then paid

### Operational Impact
- **Build time:** ~5 minutes for full CI pipeline
- **Monthly minutes:** ~150 minutes/week for public repo (free)
- **Storage:** Artifacts stored for 90 days
- **Concurrency:** Multiple workflows run in parallel

## Implementation

### Workflow Structure

```
.github/workflows/
├── ci.yml          # Continuous Integration
├── cd.yml          # Continuous Deployment
├── release.yml     # Release Management
├── performance.yml # Performance Testing
├── docs.yml        # Documentation
├── security.yml    # Security Scanning (Phase 7)
└── codeql.yml      # CodeQL Analysis
```

### 1. CI Workflow (ci.yml)
**Purpose:** Build, test, and validate code quality

**Triggers:**
- Every push
- Every pull request

**Jobs:**
- Build on 3 platforms (Ubuntu, Windows, macOS)
- Run unit tests + integration tests
- Generate code coverage reports
- Run mutation testing
- Security vulnerability scanning
- Publish test results and artifacts

**Quality Gates:**
- ✅ Build must succeed
- ✅ All tests must pass
- ✅ No critical vulnerabilities

### 2. CD Workflow (cd.yml)
**Purpose:** Deploy to staging and production

**Triggers:**
- Push to main/master branch
- Manual dispatch

**Jobs:**
- Build Docker image
- Generate SBOM
- Deploy to staging
- Health checks
- Deploy to production (manual approval)
- Rollback capability

**Environments:**
- Staging (automatic)
- Production (manual approval + branch protection)

### 3. Release Workflow (release.yml)
**Purpose:** Create releases and publish packages

**Triggers:**
- Git tag push (v*.*.*)
- Manual dispatch

**Jobs:**
- Calculate semantic version (GitVersion)
- Generate changelog
- Build NuGet packages
- Build Docker images (multi-platform)
- Create GitHub Release
- Publish to NuGet.org
- Publish to GitHub Packages
- Update documentation

### 4. Performance Workflow (performance.yml)
**Purpose:** Detect performance regressions

**Triggers:**
- Pull requests
- Scheduled (weekly)

**Jobs:**
- Run BenchmarkDotNet suite
- Compare against baseline
- Detect regressions
- Publish results

### 5. Docs Workflow (docs.yml)
**Purpose:** Generate and deploy documentation

**Triggers:**
- Push to main/master
- Manual dispatch

**Jobs:**
- Build DocFX documentation
- Validate markdown links
- Deploy to GitHub Pages

## Quality Gates

### Build Quality
- ✅ Multi-platform build success
- ✅ Zero build errors
- ✅ All tests passing
- ✅ Code coverage maintained
- ✅ Mutation score maintained

### Security
- ✅ No critical/high vulnerabilities
- ✅ Dependency scanning passing
- ✅ Container scanning passing
- ✅ Secret scanning passing

### Performance
- ✅ No performance regressions >10%
- ✅ Memory allocations within budget
- ✅ Benchmark suite passing

## Example Workflow Configuration

```yaml
name: CI Pipeline

on:
  push:
    branches: [ master, main, develop ]
  pull_request:
    branches: [ master, main ]

env:
  DOTNET_VERSION: '8.0.x'

jobs:
  build-and-test:
    name: Build & Test (${{ matrix.os }})
    runs-on: ${{ matrix.os }}
    strategy:
      fail-fast: false
      matrix:
        os: [ubuntu-latest, windows-latest, macos-latest]
        configuration: [Debug, Release]

    steps:
    - name: Checkout code
      uses: actions/checkout@v4
      with:
        fetch-depth: 0

    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}

    - name: Restore dependencies
      run: dotnet restore

    - name: Build
      run: dotnet build --configuration ${{ matrix.configuration }} --no-restore

    - name: Test
      run: dotnet test --configuration ${{ matrix.configuration }} --no-build --verbosity normal

    - name: Upload test results
      if: always()
      uses: actions/upload-artifact@v4
      with:
        name: test-results-${{ matrix.os }}-${{ matrix.configuration }}
        path: TestResults/
```

## Verification

- ✅ 5 workflows implemented and operational
- ✅ Multi-platform testing (Ubuntu, Windows, macOS)
- ✅ Quality gates enforced
- ✅ Docker build and publish
- ✅ NuGet package publishing ready
- ✅ Documentation deployment
- ✅ Security scanning integrated
- ✅ Performance regression detection

## Monitoring & Observability

### Workflow Metrics
- **Success rate:** Track build success/failure
- **Duration:** Monitor workflow execution time
- **Cost:** Track GitHub Actions minutes consumed
- **Concurrency:** Monitor parallel job execution

### Alerting
- **Failed builds:** Notify team via GitHub notifications
- **Security issues:** SARIF upload triggers Security alerts
- **Performance regressions:** Comment on PR with results

## Migration Path

If we need to migrate away from GitHub Actions:
1. YAML workflows are relatively portable
2. Docker builds work anywhere
3. GitVersion works on any CI platform
4. Tests run with `dotnet test` (platform-agnostic)
5. Security scanning tools (Snyk, Trivy) support multiple CI platforms

## References

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [GitHub Actions Marketplace](https://github.com/marketplace?type=actions)
- [GitHub Container Registry](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-container-registry)
- [GitHub Packages](https://docs.github.com/en/packages)
- [Workflow Syntax](https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions)

## Related ADRs

- ADR-001: .NET 8 Upgrade (target platform for builds)
- ADR-002: Testing Strategy (integrated in CI pipeline)
- ADR-003: Logging Framework (used in deployment)

## Future Considerations

- [ ] **Self-hosted runners** for GPU workloads
- [ ] **Kubernetes deployments** via ArgoCD/Flux
- [ ] **Multi-cloud deployment** (Azure + AWS)
- [ ] **Canary deployments** for production
- [ ] **A/B testing** infrastructure

---

**Last Updated:** 2025-11-30
**Next Review:** 2026-03-01
