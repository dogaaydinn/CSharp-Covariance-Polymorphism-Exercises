# Release Process Documentation

## Overview

This document describes the complete release process for the Advanced C# Concepts project, from planning to post-release activities.

## Table of Contents

- [Release Types](#release-types)
- [Pre-Release Checklist](#pre-release-checklist)
- [Release Process](#release-process)
- [Post-Release Activities](#post-release-activities)
- [Hotfix Process](#hotfix-process)
- [Version Numbers](#version-numbers)
- [Release Channels](#release-channels)
- [Troubleshooting](#troubleshooting)

## Release Types

### Major Release (X.0.0)

**When to release:**
- Breaking API changes
- Major architectural changes
- Significant feature additions

**Example:** 1.0.0 → 2.0.0

**Process:**
1. Plan breaking changes
2. Write migration guide
3. Update major version
4. Extended testing period
5. Communicate widely

### Minor Release (0.X.0)

**When to release:**
- New features (backwards-compatible)
- Significant enhancements
- New APIs

**Example:** 1.0.0 → 1.1.0

**Process:**
1. Feature complete
2. Update documentation
3. Update minor version
4. Standard testing
5. Regular communication

### Patch Release (0.0.X)

**When to release:**
- Bug fixes
- Security patches
- Documentation updates
- Minor improvements

**Example:** 1.0.0 → 1.0.1

**Process:**
1. Fix verified
2. Quick testing
3. Update patch version
4. Fast-track release

### Pre-Release

**Types:**
- **Alpha** (1.0.0-alpha.1) - Early development
- **Beta** (1.0.0-beta.1) - Feature complete, testing
- **RC** (1.0.0-rc.1) - Release candidate

**Process:**
1. Tag with pre-release identifier
2. Publish to test channels
3. Gather feedback
4. Iterate

## Pre-Release Checklist

### Code Quality

- [ ] All tests passing (unit + integration)
- [ ] Code coverage >90%
- [ ] Mutation testing score >80%
- [ ] Static analysis passing
- [ ] No high/critical security vulnerabilities
- [ ] Performance benchmarks meet targets
- [ ] Memory leaks checked

### Documentation

- [ ] API documentation updated
- [ ] README.md updated
- [ ] CHANGELOG.md updated
- [ ] Migration guide written (if breaking changes)
- [ ] Code examples updated
- [ ] Architecture diagrams current

### Configuration

- [ ] Version bumped in all necessary files
- [ ] Package metadata updated
- [ ] Dependencies reviewed and updated
- [ ] License file current
- [ ] Copyright years updated

### Testing

- [ ] Smoke tests passed
- [ ] Integration tests passed
- [ ] Performance tests passed
- [ ] Security scan passed
- [ ] Cross-platform testing (if applicable)

### Legal & Compliance

- [ ] License compliance checked
- [ ] Third-party licenses reviewed
- [ ] GDPR compliance verified (if applicable)
- [ ] Export control reviewed (if applicable)

## Release Process

### Step 1: Prepare Release Branch

```bash
# Ensure develop is up to date
git checkout develop
git pull origin develop

# Create release branch
git checkout -b release/1.1.0

# Push release branch
git push -u origin release/1.1.0
```

### Step 2: Update Version

GitVersion will automatically calculate the version based on branch name and commits.

Verify the version:
```bash
dotnet gitversion
```

### Step 3: Update CHANGELOG

#### Automated (Recommended)

```bash
# Install git-cliff if not already installed
# See docs/guides/CHANGELOG_AUTOMATION.md

# Generate changelog
git cliff --tag v1.1.0 --prepend CHANGELOG.md

# Review and commit
git add CHANGELOG.md
git commit -m "docs: update CHANGELOG for v1.1.0"
git push
```

#### Manual

Edit CHANGELOG.md:
```markdown
## [1.1.0] - 2025-12-01

### Added
- New authentication system with OAuth2 support
- Real-time notifications using SignalR

### Changed
- Improved performance by 50%
- Updated UI design

### Fixed
- Fixed memory leak in background service (#123)
- Corrected validation logic (#456)

### Security
- Updated dependencies to patch vulnerabilities

[1.1.0]: https://github.com/dogaaydinn/repo/compare/v1.0.0...v1.1.0
```

### Step 4: Final Testing

```bash
# Run all tests
dotnet test --configuration Release

# Run benchmarks
dotnet run --project tests/AdvancedConcepts.Benchmarks --configuration Release

# Run security scan
dotnet list package --vulnerable

# Build for production
dotnet build --configuration Release
```

### Step 5: Create Pull Request

1. Create PR from `release/1.1.0` to `main`
2. Ensure all CI checks pass
3. Get required approvals
4. **Do NOT merge yet**

### Step 6: Merge to Main

```bash
# Switch to main
git checkout main
git pull origin main

# Merge release branch (no fast-forward)
git merge --no-ff release/1.1.0

# Create annotated tag
git tag -a v1.1.0 -m "Release version 1.1.0

Major Features:
- OAuth2 authentication
- Real-time notifications

Bug Fixes:
- Memory leak fix
- Validation corrections

See CHANGELOG.md for full details.
"

# Push main with tags
git push origin main
git push origin v1.1.0
```

### Step 7: Automated Release (GitHub Actions)

The `release.yml` workflow will automatically:

1. **Trigger** on tag push
2. **Build** the project
3. **Run tests**
4. **Generate** release notes
5. **Create** GitHub release
6. **Publish** Docker images (if configured)
7. **Deploy** documentation

Monitor the workflow at:
```
https://github.com/dogaaydinn/repo/actions
```

### Step 8: Verify Release

```bash
# Check GitHub release
# https://github.com/dogaaydinn/repo/releases/tag/v1.1.0

# Verify Docker image (if applicable)
# docker pull ghcr.io/dogaaydinn/advancedconcepts:1.1.0

# Check documentation deployment
# https://dogaaydinn.github.io/repo
```

### Step 9: Merge Back to Develop

```bash
# Switch to develop
git checkout develop
git pull origin develop

# Merge release branch
git merge --no-ff release/1.1.0

# Push develop
git push origin develop
```

### Step 10: Clean Up

```bash
# Delete release branch (optional)
git branch -d release/1.1.0
git push origin --delete release/1.1.0

# Update project boards
# Close milestone
# Archive completed issues
```

## Post-Release Activities

### Communication

**Announce Release:**

1. **GitHub Release** - Automatically created
2. **Social Media** (if applicable)
   ```
   🎉 Advanced C# Concepts v1.1.0 is now available!

   ✨ New: OAuth2 authentication
   ⚡ 50% performance improvement
   🐛 Critical bug fixes

   https://github.com/dogaaydinn/repo/releases/tag/v1.1.0
   ```

3. **Documentation Site** - Update homepage
4. **Email Newsletter** (if applicable)

### Monitoring

**Monitor for Issues:**

```bash
# Check error rates
# Monitor performance metrics
# Review user feedback
# Track download stats
```

**Set up alerts:**
- GitHub issue notifications
- Error tracking (if integrated)
- Performance monitoring

### Documentation

- [ ] Update API documentation site
- [ ] Update getting started guide
- [ ] Add release to version history
- [ ] Update samples/examples if needed

### Support

- [ ] Monitor GitHub issues
- [ ] Respond to questions
- [ ] Fix critical bugs immediately
- [ ] Plan next release

## Hotfix Process

### When to Create a Hotfix

- Critical bugs in production
- Security vulnerabilities
- Data loss risks
- Service outages

### Hotfix Steps

```bash
# 1. Create hotfix branch from main
git checkout main
git pull origin main
git checkout -b hotfix/1.1.1

# 2. Fix the issue
# ... make changes ...
git add .
git commit -m "fix: critical bug in authentication"

# 3. Test thoroughly
dotnet test --configuration Release

# 4. Update version and CHANGELOG
# (GitVersion handles version automatically)
git cliff --tag v1.1.1 --prepend CHANGELOG.md
git add CHANGELOG.md
git commit -m "docs: update CHANGELOG for v1.1.1"

# 5. Merge to main
git checkout main
git merge --no-ff hotfix/1.1.1
git tag -a v1.1.1 -m "Hotfix: Critical authentication bug"
git push origin main --tags

# 6. Merge back to develop
git checkout develop
git merge --no-ff hotfix/1.1.1
git push origin develop

# 7. Delete hotfix branch
git branch -d hotfix/1.1.1
git push origin --delete hotfix/1.1.1
```

### Hotfix Communication

**Immediate:**
- GitHub release with "HOTFIX" label
- Update security advisory (if security-related)
- Notify affected users

**Template:**
```markdown
# Hotfix Release: v1.1.1

## Critical Fix

This hotfix addresses a critical bug in the authentication system
that could lead to unauthorized access.

### What Changed
- Fixed authentication bypass vulnerability
- Added additional validation checks

### Who Should Upgrade
**All users running v1.1.0 should upgrade immediately.**

### How to Upgrade
```bash
# Update to latest version
dotnet add package AdvancedConcepts.Core --version 1.1.1
```

### More Information
- CVE-XXXX-XXXX (if applicable)
- Security advisory: [link]
```

## Version Numbers

### Semantic Versioning

```
MAJOR.MINOR.PATCH[-PRERELEASE][+BUILD]

Examples:
1.0.0         - Stable release
1.1.0-alpha.1 - Alpha pre-release
1.1.0-beta.2  - Beta pre-release
1.1.0-rc.1    - Release candidate
1.1.0+build.1 - Build metadata
```

### Version Bumping Rules

| Change Type | Version Bump | Example |
|-------------|--------------|---------|
| Breaking change | MAJOR | 1.0.0 → 2.0.0 |
| New feature | MINOR | 1.0.0 → 1.1.0 |
| Bug fix | PATCH | 1.0.0 → 1.0.1 |
| Pre-release | Identifier | 1.0.0 → 1.0.0-alpha.1 |

### GitVersion Calculation

GitVersion automatically calculates versions based on:
- Branch name
- Commit history
- Tags
- Conventional commit messages

```bash
# View calculated version
dotnet gitversion

# Get specific component
dotnet gitversion /showvariable SemVer
```

## Release Channels

### Production

- **Branch:** `main`
- **Versions:** Stable (e.g., 1.0.0)
- **Audience:** All users
- **Quality:** Highest
- **Frequency:** Monthly (typically)

### Beta

- **Branch:** `release/*`
- **Versions:** RC (e.g., 1.1.0-rc.1)
- **Audience:** Early adopters
- **Quality:** High
- **Frequency:** Weekly (during release cycle)

### Development

- **Branch:** `develop`
- **Versions:** Alpha (e.g., 1.1.0-alpha.5)
- **Audience:** Developers, testers
- **Quality:** Experimental
- **Frequency:** Continuous

### Feature

- **Branch:** `feature/*`
- **Versions:** Feature builds (e.g., 1.1.0-feature.3)
- **Audience:** Feature developers
- **Quality:** Unstable
- **Frequency:** Per commit

## Troubleshooting

### Issue: Tag Already Exists

```bash
# Error: tag 'v1.1.0' already exists

# Solution 1: Delete and recreate (if not pushed)
git tag -d v1.1.0
git tag -a v1.1.0 -m "Release 1.1.0"

# Solution 2: Use force push (DANGEROUS - only if tag wasn't pulled by others)
git push -f origin v1.1.0
```

### Issue: Release Build Failed

```bash
# Check GitHub Actions logs
# Fix issues
# Re-push tag to trigger workflow

git tag -d v1.1.0
git tag -a v1.1.0 -m "Release 1.1.0"
git push origin v1.1.0 --force
```

### Issue: Tests Fail After Merge

```bash
# Investigate failure
dotnet test --logger "console;verbosity=detailed"

# Fix issues
# Commit fix
git add .
git commit -m "fix: resolve test failures"

# Create patch release
git tag -a v1.1.1 -m "Patch: Fix test failures"
git push origin main --tags
```

### Issue: Version Number Incorrect

```bash
# Check GitVersion configuration
cat GitVersion.yml

# Verify branch name
git branch --show-current

# Recalculate version
dotnet gitversion /updateassemblyinfo
```

## Release Schedule

### Regular Releases

- **Major:** Annually (or as needed for breaking changes)
- **Minor:** Quarterly
- **Patch:** As needed (typically monthly)

### Release Timeline

```
Week 1-2: Feature development
Week 3:   Code freeze, create release branch
Week 4:   Testing and bug fixes
Week 5:   Release candidate (RC)
Week 6:   Final release
```

### Recommended Calendar

- **Q1 (Jan-Mar):** Minor release (X.1.0)
- **Q2 (Apr-Jun):** Minor release (X.2.0)
- **Q3 (Jul-Sep):** Minor release (X.3.0)
- **Q4 (Oct-Dec):** Major release planning / Minor release (X.4.0)

## Best Practices

### Before Release

- ✅ Plan releases in advance
- ✅ Communicate timeline
- ✅ Feature freeze 2 weeks before release
- ✅ Thorough testing period
- ✅ Documentation review

### During Release

- ✅ Follow checklist strictly
- ✅ Automate where possible
- ✅ Test release artifacts
- ✅ Coordinate with team
- ✅ Monitor CI/CD closely

### After Release

- ✅ Monitor for issues
- ✅ Respond quickly to problems
- ✅ Gather feedback
- ✅ Plan next release
- ✅ Update roadmap

### Emergency Releases

- ✅ Keep calm
- ✅ Assess severity
- ✅ Fix and test quickly
- ✅ Communicate clearly
- ✅ Post-mortem analysis

## Resources

### Documentation

- [Versioning Strategy](./guides/VERSIONING_STRATEGY.md)
- [NuGet Packaging](./guides/NUGET_PACKAGING.md)
- [Changelog Automation](./guides/CHANGELOG_AUTOMATION.md)

### Tools

- [GitVersion](https://gitversion.net/)
- [git-cliff](https://git-cliff.org/)
- [Semantic Versioning](https://semver.org/)
- [Keep a Changelog](https://keepachangelog.com/)

### Workflows

- [Release Workflow](.github/workflows/release.yml)
- [CI Workflow](.github/workflows/ci.yml)
- [CD Workflow](.github/workflows/cd.yml)

---

**Last Updated:** 2025-12-01
**Version:** 1.0
