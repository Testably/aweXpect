The `Has…` expectations for a number or a `TimeSpan`, e.g. `HasLength()`, `HasCount()`, `HasYear()` or `HasMajor()`,
continue with one of the following comparisons, each with a negated counterpart:

| Comparison                   | Negated                         | Succeeds when the value is                        |
|------------------------------|---------------------------------|---------------------------------------------------|
| `EqualTo(x)`                 | `NotEqualTo(x)`                 | equal to `x`                                      |
| `GreaterThan(x)`             | `NotGreaterThan(x)`             | greater than `x`                                  |
| `GreaterThanOrEqualTo(x)`    | `NotGreaterThanOrEqualTo(x)`    | greater than or equal to `x`                      |
| `LessThan(x)`                | `NotLessThan(x)`                | less than `x`                                     |
| `LessThanOrEqualTo(x)`       | `NotLessThanOrEqualTo(x)`       | less than or equal to `x`                         |
| `Between(min).And(max)`      | `NotBetween(min).And(max)`      | between `min` and `max`, both bounds included     |

Passing the value directly, e.g. `HasLength(10)`, is a shorthand for `EqualTo`.
