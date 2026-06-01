# OPTIMIZATION

## Overview

Profiling was conducted on a physical device (**Xiaomi Redmi Note 10S – 8GB RAM**) using Unity Profiler and Memory Profiler to ensure accurate runtime performance evaluation.

Overall, the project does not exhibit any critical performance bottlenecks during normal gameplay. The system is stable, with optimization efforts focused on scalability and future improvements rather than immediate fixes.

---

## CPU

* Target: 30 FPS (VSync enabled)
* Frame time distribution:

  * ~65%: WaitForTargetFPS
  * ~20%: Universal Render Pipeline (rendering)
  * ~4%: Script Update
  * ~2%: WaitForPresentOnGfxThread
  * Remaining: <1%

**Analysis:**

* The CPU is mostly idle, indicating the application is not CPU-bound.
* Rendering cost remains moderate and stable.
* Gameplay logic is lightweight, with minimal per-frame processing.

---

## Memory

* Runtime memory: ~295–296 MB
* Memory remains stable across multiple gameplay loops and scene transitions
* No increasing trend observed → no memory leak

**Analysis:**

* Memory usage is consistent and predictable even without explicit asset release.
* Scene loading does not introduce additional memory pressure.
* No abnormal growth detected during repeated play sessions.

---

## Garbage Collection

* No GC spikes during gameplay
* Allocations occur only during scene loading

**Analysis:**

* Gameplay loop is effectively allocation-free after initialization.
* GC impact is negligible and does not affect frame stability.

---

## Scene Loading

* Method: SceneManager.LoadSceneAsync
* Observed drop: ~15 FPS during loading phase

**Analysis:**

* FPS drop is caused by asset activation and initialization on the main thread.
* This behavior is expected for synchronous scene activation.

**Mitigation:**

* A dedicated Loading Scene is used, preventing impact on user experience.

---

## Update Usage

* Update is used in limited cases:

  * Input handling
  * Screen size change detection

**Analysis:**

* No unnecessary per-frame logic
* Systems are lightweight and controlled

---

## Rendering

* Rendering cost is stable (~20% frame time)

**Analysis:**

* No rendering bottleneck observed
* Suitable for current project scope

---

## Addressables

* Assets are currently **not explicitly released**

**Analysis:**

* Despite the lack of manual release, memory remains stable across multiple gameplay loops.
* This indicates no immediate memory pressure under current usage patterns.
* However, this may introduce scalability risks as asset count increases.

---

## Future Optimizations

While the current system performs well, several improvements can be made to enhance scalability and long-term efficiency:

* **Addressables Lifecycle**

  * Implement proper asset release strategy
  * Track and manage asset references explicitly

* **Scene Architecture**

  * Transition to a persistent scene setup (single root scene + dynamic view loading)
  * Reduce reliance on full scene loads

* **Scene Loading**

  * Use additive scene loading to minimize frame spikes
  * Preload critical assets before activation

* **Memory Management**

  * Introduce sprite atlas to optimize texture loading/unloading
  * Group assets by usage context (scene, popup, level)

* **Update Optimization**

  * Further reduce Update usage by shifting to event-driven patterns where applicable

---

## Conclusion

The project is not CPU-bound, GPU-bound, or memory-bound under normal conditions.
Memory remains stable even without explicit Addressables release, and no GC-related issues are present.

The only notable performance cost occurs during scene loading, which is expected and already mitigated through a dedicated loading flow.

Future work will focus on improving asset lifecycle management and overall architecture to ensure scalability as the project grows.
