# Support

Thank you for using Advanced C# Concepts! This document provides information on how to get help and support.

## Table of Contents

- [Documentation](#documentation)
- [Getting Help](#getting-help)
- [Reporting Issues](#reporting-issues)
- [Security Vulnerabilities](#security-vulnerabilities)
- [Community](#community)
- [Commercial Support](#commercial-support)

## Documentation

Before seeking support, please check our comprehensive documentation:

### Official Documentation
- **[README.md](README.md)** - Project overview, quick start, and features
- **[ROADMAP.md](ROADMAP.md)** - Project roadmap and development phases
- **[CONTRIBUTING.md](CONTRIBUTING.md)** - Contribution guidelines
- **[SECURITY.md](SECURITY.md)** - Security policy and vulnerability reporting
- **[CHANGELOG.md](CHANGELOG.md)** - Version history and changes

### Architecture Documentation
- **[Architecture Decision Records](docs/architecture/01-architecture-decision-records/)** - Design decisions (ADR-001 through ADR-004)
- **[C4 Diagrams](docs/architecture/02-c4-diagrams/)** - System architecture diagrams
- **[Design Patterns](docs/architecture/03-design-patterns/)** - Pattern documentation
- **[Performance Guides](docs/architecture/04-performance/)** - Optimization guides

### API Documentation
- **[API Reference](https://dogaaydinn.github.io/CSharp-Covariance-Polymorphism-Exercises)** - Auto-generated API documentation (DocFX)

## Getting Help

### Self-Service Resources

#### Quick Start
See [README.md](README.md#getting-started) for:
- Installation instructions
- Prerequisites
- Running your first example
- Common commands

#### Examples
The project includes comprehensive examples:
- **Beginner:** Basic polymorphism and inheritance
- **Intermediate:** Boxing/unboxing, covariance/contravariance
- **Advanced:** Pattern matching, LINQ, async/await, DI
- **Enterprise:** SOLID, Polly, FluentValidation, Result pattern

Run examples:
```bash
dotnet run --project src/AdvancedConcepts.Core
```

#### Troubleshooting

##### Build Issues

**Problem:** Build fails with "SDK not found"
```
Solution:
1. Verify .NET 8 SDK is installed: dotnet --version
2. Install from: https://dotnet.microsoft.com/download/dotnet/8.0
3. Restart terminal/IDE after installation
```

**Problem:** NuGet restore fails
```
Solution:
1. Clear NuGet cache: dotnet nuget locals all --clear
2. Restore packages: dotnet restore
3. Check internet connection
```

**Problem:** Analyzer warnings/errors
```
Solution:
1. These are intentional code quality checks
2. Review warnings to improve code quality
3. Not all warnings need to be fixed immediately
4. See .editorconfig for configuration
```

##### Test Issues

**Problem:** Tests fail to run
```
Solution:
1. Ensure all dependencies restored: dotnet restore
2. Build solution: dotnet build
3. Run tests: dotnet test --verbosity normal
4. Check test output for specific failures
```

**Problem:** Flaky integration tests
```
Solution:
1. Re-run tests a few times
2. Check for timing issues or resource contention
3. Report persistent failures as bugs
```

##### Docker Issues

**Problem:** Docker Compose fails to start
```
Solution:
1. Ensure Docker is running
2. Check port availability (5341, 5342, 9090, 3000)
3. Pull latest images: docker-compose pull
4. Restart: docker-compose down && docker-compose up -d
```

### GitHub Discussions

For questions, ideas, and general discussions:

**[GitHub Discussions](https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises/discussions)**

Categories:
- **Q&A** - Ask questions about using the project
- **Ideas** - Suggest new features or improvements
- **Show and Tell** - Share your projects using Advanced C# Concepts
- **General** - General discussion

### Stack Overflow

Tag your questions with:
- `c#`
- `dotnet`
- `covariance`
- `contravariance`
- `advanced-csharp`

Search existing questions:
[Stack Overflow - Advanced C# Topics](https://stackoverflow.com/questions/tagged/c%23+covariance)

## Reporting Issues

### Bug Reports

Found a bug? Please report it!

1. **Search existing issues** to avoid duplicates
2. **Use the bug report template** when creating a new issue
3. **Include:**
   - Clear description of the bug
   - Steps to reproduce
   - Expected behavior
   - Actual behavior
   - Environment details (.NET version, OS, IDE)
   - Code samples or screenshots
   - Error messages and stack traces

**[Report a Bug](https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises/issues/new?template=bug_report.md)**

### Feature Requests

Have an idea for improvement?

1. **Search existing feature requests** to avoid duplicates
2. **Use the feature request template**
3. **Explain:**
   - The problem you're trying to solve
   - Your proposed solution
   - Alternative approaches considered
   - Examples of how it would work

**[Request a Feature](https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises/issues/new?template=feature_request.md)**

### Issue Response Times

- **Critical bugs:** 24-48 hours
- **Regular bugs:** 3-7 days
- **Feature requests:** 1-2 weeks
- **Questions:** 1-3 days

*Note: This is an open-source project maintained by volunteers. Response times may vary.*

## Security Vulnerabilities

**DO NOT** report security vulnerabilities through public GitHub issues.

Instead, please report them via email to **dogaaydinn@gmail.com** with subject line `[SECURITY]`.

You should receive a response within 48 hours. For more details, see our [Security Policy](SECURITY.md).

### What to Include

- Type of vulnerability
- Full paths of affected source files
- Location of the affected code (tag/branch/commit or URL)
- Steps to reproduce
- Proof-of-concept or exploit code (if possible)
- Impact assessment

## Community

### GitHub
- **Repository:** [dogaaydinn/CSharp-Covariance-Polymorphism-Exercises](https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises)
- **Discussions:** [GitHub Discussions](https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises/discussions)
- **Issues:** [Issue Tracker](https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises/issues)

### Email
- **General inquiries:** dogaaydinn@gmail.com
- **Security issues:** dogaaydinn@gmail.com (subject: [SECURITY])
- **Partnership/collaboration:** dogaaydinn@gmail.com (subject: [PARTNERSHIP])

### Social Media
- **GitHub:** [@dogaaydinn](https://github.com/dogaaydinn)

### Code of Conduct

Please note that this project is released with a [Contributor Code of Conduct](CODE_OF_CONDUCT.md). By participating in this project you agree to abide by its terms.

## Commercial Support

This is an open-source educational project maintained primarily by volunteers. Commercial support is not currently available.

However, if you're interested in:
- **Enterprise training** using this project
- **Consulting** on C# advanced concepts
- **Custom development** based on these patterns

Please contact: dogaaydinn@gmail.com with subject line `[COMMERCIAL]`

## Version Support

### Currently Supported Versions

| Version | .NET Version | Supported Until |
|---------|-------------|-----------------|
| 1.0.x   | .NET 8 LTS  | November 2026   |

### End of Life

Versions no longer supported:
- **< 1.0:** Not supported (pre-release versions)

## Additional Resources

### Learning Resources
- [Microsoft C# Documentation](https://docs.microsoft.com/en-us/dotnet/csharp/)
- [.NET Performance Tips](https://docs.microsoft.com/en-us/dotnet/core/whats-new/performance-improvements)
- [C# Language Specification](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/)

### Related Projects
- [BenchmarkDotNet](https://benchmarkdotnet.org/)
- [xUnit](https://xunit.net/)
- [Serilog](https://serilog.net/)
- [Polly](https://github.com/App-vNext/Polly)
- [FluentValidation](https://fluentvalidation.net/)

### Tools
- [Visual Studio](https://visualstudio.microsoft.com/)
- [Rider](https://www.jetbrains.com/rider/)
- [VS Code](https://code.visualstudio.com/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

## Contributing

Interested in contributing? Great! Please see our [Contributing Guide](CONTRIBUTING.md) for:
- Development setup
- Coding standards
- Testing guidelines
- Pull request process

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

**Need help that's not covered here?**

- **Create a discussion:** [GitHub Discussions](https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises/discussions)
- **Email us:** dogaaydinn@gmail.com
- **Report an issue:** [Issue Tracker](https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises/issues)

Thank you for using Advanced C# Concepts! 🚀

---

**Last Updated:** 2025-11-30
**Version:** 1.0
