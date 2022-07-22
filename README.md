# Random

### Background

See: https://csharpindepth.com/articles/Random

TLDR, Random in C# has 2 problems that developers have to code around: 

1. Random is not thread-safe. In some cases when the Random instance is accessed by multiple threads, it could yield numbers that will always be 0 and then that instance becomes useless. This is for net framework, net core, and net (`net5.0`).
2. Random instances new'ed up in close time intervals will result in same numbers. This is because Random is deterministic where instances with same seed will give same numbers. The default Random ctor internally uses `Environment.TickCount` as seed which doesn't change for 10-16ms on net framework. This is fixed on net core, and net (`net5.0`). 

### Random API

Random doesn't inherit from an interface so the implementations inherit from Random versus a static class. This makes it easier to swap in whichever implementation that suits the need. `RandomFactory` gives out a singleton instance of the implementations as a single Random instance per app domain is the recommendation. The ctors are not public and a specifc seed cannot be used. 

`LockRandom` simply makes pass-through calls to the base instance's methods with a lock, so using a specific seed technically is possible but not done. 

`ThreadStaticRandom` and `ThreadLocalRandom` use similar approaches where a global RNG is used to seed the threads' Random instances. A specifc seed is meaningless and the base instance is unused. The global RNG implementations are not exactly the same so as to keep them as the original authors intended. 

### Tests

- **Showcase_Issues**: Showcases different behaviors and problems with Random approaches. 
- **Verify_Correctness**: Showcases issue 1. with Random and how the other approaches don't have it. Particularly, `Verify_Correctness_Random` is the test that showcases the thread-safety issue (issue 1. above) once in a while on net framework and even net core, net (`net5.0`). Yikes! 
- **Verify_Perf**: Perf 10M iterations of `Next()` call in parallel for different approaches. 
- **Verify_Distribution**: Sample 100 iterations to manually check distribution (for patterns, repeats, etc). 

**Perf**

`Verify_Perf` for 10M `Next()` calls. 

| Test (net5.0)                  |   Time (ms) |
| :-                             |          -: |
| Verify_Perf_NewInstance        |        3198 |
| Verify_Perf_Random (BUG!)      |         252 |
| Verify_Perf_NewGuidAsSeed      |        3225 |
| Verify_Perf_LockRandom         |         663 |
| Verify_Perf_ThreadLocalRandom  |          80 |
| Verify_Perf_ThreadStaticRandom |          66 |

### Recommendations

- Use `LockRandom` if you want a balance of speed and to guard yourself against any issues, patterns, etc. 
- Use `ThreadStaticRandom`, `ThreadLocalRandom` if you want speed but are ok with the possibility of issues discovered in future (patterns, etc). They are the fastest of the bunch. 
- Implementations with new Random instances on every call, and using `Guid.NewGuid().GetHashCode()` as seed are the slowest (issue 2. on net framework only). `Guid.NewGuid().GetHashCode()` could come with its own problems as it depends on Guid's `GetHashCode()` implementation. 
- Implementation with global Random instance is riskiest (issue 1. above), so definitely avoid that. 
