The `Has…` expectations whose value is a number or a `TimeSpan`, e.g. `HasLength()`, `HasCount()`, `HasYear()` or
`HasMajor()`, continue with one of the following comparisons, each with a negated counterpart:

| Comparison                | Negated                      | Succeeds when the value is                    |
|---------------------------|------------------------------|-----------------------------------------------|
| `EqualTo(x)`              | `NotEqualTo(x)`              | equal to `x`                                  |
| `GreaterThan(x)`          | `NotGreaterThan(x)`          | greater than `x`                              |
| `GreaterThanOrEqualTo(x)` | `NotGreaterThanOrEqualTo(x)` | greater than or equal to `x`                  |
| `LessThan(x)`             | `NotLessThan(x)`             | less than `x`                                 |
| `LessThanOrEqualTo(x)`    | `NotLessThanOrEqualTo(x)`    | less than or equal to `x`                     |
| `Between(min).And(max)`   | `NotBetween(min).And(max)`   | between `min` and `max`, both bounds included |

Passing the value directly, e.g. `HasLength(10)`, is a shorthand for `EqualTo`.

<details>
<summary>Comparing with `null` and invalid arguments</summary>

The expected value is nullable, e.g. to pass a value mapped from a property. As the actual value is never `null`,
`EqualTo(null)` fails and `NotEqualTo(null)` succeeds, while a comparison of order against `null`, such as
`GreaterThan(null)` or `Between(null).And(3)`, fails even when negated.

A maximum below the minimum in `Between` or `NotBetween` throws an `ArgumentOutOfRangeException`, as does a negative
expected length, line count, count, position or buffer size.

</details>
