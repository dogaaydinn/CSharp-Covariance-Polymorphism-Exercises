# Legacy Code Refactoring

## Problem
10-year-old codebase with 500-line methods, no tests, tight coupling. Need to add new features safely.

## Solutions
1. **Basic**: Extract Method refactoring + unit tests
2. **Advanced**: Strategy Pattern + Dependency Injection
3. **Enterprise**: Strangler Fig Pattern (gradual migration)

## Techniques
- Extract Method/Class
- Introduce Parameter Object
- Replace Conditional with Polymorphism
- Dependency Injection
- Facade Pattern (wrap legacy code)
- Characterization Tests (test current behavior)

## Approach
- Add tests first (safety net)
- Refactor incrementally
- Never break existing functionality
- Improve code coverage gradually

See PROBLEM.md for real refactoring examples.
