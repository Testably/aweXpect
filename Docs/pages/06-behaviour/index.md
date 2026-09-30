# Behaviour

Describes the possible expectations for code that runs: delegates, tasks, events and callbacks.

| Page                           | Expectations                                                                                |
|--------------------------------|---------------------------------------------------------------------------------------------|
| [Delegates](./01-delegates.md) | `Throws`, `ThrowsExactly`, `DoesNotThrow`, the thrown exception, `ExecutesIn`, `Eventually` |
| [Tasks](./02-tasks.md)         | the delegate expectations for a running `Task` or `ValueTask`, the result of a `Task<T>`    |
| [Events](./03-events.md)       | `Triggered`, `DidNotTrigger`, `TriggeredPropertyChanged`, `TriggeredPropertyChangedFor`     |
| [Callbacks](./04-callbacks.md) | `Signaled`, `DidNotSignal` on a `Signaler`                                                  |

Many of these expectations wait: for a delegate or a task to complete, for callbacks to be signaled, or with
`Within(…)` for events to be triggered. [Time and cancellation](../03-how-it-works/06-time-and-cancellation.md)
describes how long they wait, how `WithTimeout(…)` and `WithCancellation(…)` limit it, and how a timeout or a
cancellation is reported.
