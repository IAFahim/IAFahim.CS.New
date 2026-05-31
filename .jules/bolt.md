
## 2024-05-18 - Math Namespace Shadowing
**Learning:** The project contains its own `IAFahim.Math` namespace, which can conflict with the built-in C# `Math` class when the `using System;` directive is present. This causes `Math` to resolve to `IAFahim.Math` instead of `System.Math`, breaking compilation for methods like `Math.DivRem`.
**Action:** When calling standard math methods in this project (especially within the `IAFahim.Math.*` namespaces), always use the fully qualified `System.Math` (e.g., `System.Math.DivRem`) to avoid ambiguous reference errors.
