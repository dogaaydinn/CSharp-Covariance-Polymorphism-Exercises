# Sample Projects

This directory contains practical examples demonstrating the concepts from the Advanced C# Concepts library.

## 📚 Learning Path

Follow these samples in order for the best learning experience:

### Level 1: Beginner
**Time:** 1-2 hours
**Prerequisites:** Basic C# knowledge

- [01-PolymorphismBasics](./01-Beginner/PolymorphismBasics/) - Understanding polymorphism
- [02-CastingExamples](./01-Beginner/CastingExamples/) - Upcasting and downcasting
- [03-OverrideVirtual](./01-Beginner/OverrideVirtual/) - Virtual methods and overrides

### Level 2: Intermediate
**Time:** 2-3 hours
**Prerequisites:** Completed beginner samples

- [04-CovarianceContravariance](./02-Intermediate/CovarianceContravariance/) - Generic variance
- [05-BoxingPerformance](./02-Intermediate/BoxingPerformance/) - Boxing/unboxing impact
- [06-GenericConstraints](./02-Intermediate/GenericConstraints/) - Advanced generics

### Level 3: Advanced
**Time:** 4-6 hours
**Prerequisites:** Completed intermediate samples

- [07-DesignPatterns](./03-Advanced/DesignPatterns/) - Factory, Builder, Strategy patterns
- [08-SOLIDPrinciples](./03-Advanced/SOLIDPrinciples/) - SOLID in practice
- [09-PerformanceOptimization](./03-Advanced/PerformanceOptimization/) - Span<T>, benchmarking
- [10-ResiliencePatterns](./03-Advanced/ResiliencePatterns/) - Polly retry, circuit breaker
- [11-ObservabilityPatterns](./03-Advanced/ObservabilityPatterns/) - Structured logging, tracing

### Level 4: Expert (Phase 10 - Innovation)
**Time:** 6-10 hours
**Prerequisites:** Solid understanding of C# and Roslyn

- [12-SourceGenerators](./04-Expert/SourceGenerators/) - Build custom source generators
- [13-RoslynAnalyzers](./04-Expert/RoslynAnalyzers/) - Create code analyzers
- [14-NativeAOT](./04-Expert/NativeAOT/) - AOT compilation and optimization
- [15-AdvancedPerformance](./04-Expert/AdvancedPerformance/) - SIMD, parallelism

### Level 5: Real-World Applications
**Time:** 8-12 hours
**Prerequisites:** Completed expert samples

- [16-MLNetIntegration](./05-RealWorld/MLNetIntegration/) - Machine learning integration
- [17-MicroserviceTemplate](./05-RealWorld/MicroserviceTemplate/) - Complete microservice
- [18-WebApiAdvanced](./05-RealWorld/WebApiAdvanced/) - Production-ready API

## 🎯 Quick Start

### Run a Sample

```bash
# Navigate to sample directory
cd samples/01-Beginner/PolymorphismBasics

# Run the sample
dotnet run
```

### Run All Samples

```bash
# From repository root
./scripts/run-all-samples.sh
```

## 📖 Sample Structure

Each sample follows this structure:

```
SampleName/
├── README.md           # Explains the concept
├── Program.cs          # Main demonstration
├── Examples/           # Code examples
├── Tests/              # Sample tests (optional)
└── SampleName.csproj   # Project file
```

## 🎓 Learning Objectives

### Beginner Samples
- ✅ Understand polymorphism fundamentals
- ✅ Master casting and type checking
- ✅ Learn virtual methods and overrides

### Intermediate Samples
- ✅ Apply covariance and contravariance
- ✅ Optimize boxing/unboxing operations
- ✅ Use generic constraints effectively

### Advanced Samples
- ✅ Implement design patterns correctly
- ✅ Apply SOLID principles
- ✅ Optimize performance with modern C#
- ✅ Build resilient applications
- ✅ Implement comprehensive observability

### Expert Samples
- ✅ Create custom source generators
- ✅ Build Roslyn analyzers
- ✅ Compile with Native AOT
- ✅ Apply advanced performance techniques

### Real-World Samples
- ✅ Integrate machine learning
- ✅ Build production-ready services
- ✅ Deploy to cloud platforms

## 💡 Tips

1. **Follow the order** - Samples build on each other
2. **Read the README** - Each sample has detailed explanations
3. **Experiment** - Modify the code and see what happens
4. **Run tests** - Learn from the test cases
5. **Ask questions** - Open issues if something is unclear

## 🔗 Resources

- [Main Documentation](../docs/)
- [Architecture Guides](../docs/architecture/)
- [API Reference](../docs/api/)
- [Contributing Guide](../CONTRIBUTING.md)

## 📊 Difficulty Levels

| Level | Time | Concepts | Prerequisites |
|-------|------|----------|---------------|
| Beginner | 1-2h | Basic OOP | C# basics |
| Intermediate | 2-3h | Generics, variance | Beginner complete |
| Advanced | 4-6h | Patterns, performance | Intermediate complete |
| Expert | 6-10h | Roslyn, AOT | Advanced complete |
| Real-World | 8-12h | ML, microservices | Expert complete |

## 🎯 What You'll Build

- **12 Tutorial Projects** - Step-by-step learning
- **6 Real Applications** - Production-ready examples
- **100+ Code Examples** - Practical demonstrations
- **200+ Tests** - Learn from test cases

---

**Start with:** [01-PolymorphismBasics](./01-Beginner/PolymorphismBasics/)

**Questions?** Open an [issue](https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises/issues)

Happy Learning! 🚀
