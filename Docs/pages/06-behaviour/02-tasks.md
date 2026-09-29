# Tasks

Describes the possible expectations for a `Task` or `ValueTask` that is passed to `Expect.That` directly.

| Expectation                                 | Negated        | Summary                                                         |
|---------------------------------------------|----------------|-----------------------------------------------------------------|
| [`Throws`](#running-tasks), …               | `DoesNotThrow` | the running `Task` or `ValueTask` faults, as for a delegate     |
| [`ExecutesIn`](#running-tasks)              |                | the rest of the running task completes within the expected time |
| [expectations on `T`](#tasks-with-a-result) |                | the result of a `Task<T>` or `ValueTask<T>` meets them          |
| [`Expect.That<Task>(…)`](#the-task-object)  |                | expectations on the task object itself                          |

## Running tasks

A `Task` or `ValueTask` is treated like a [delegate](./01-delegates.md): an asynchronous operation without return value
that is already running. All expectations for a delegate without return value are available:

```csharp
Task ImportAlbumAsync(string title) => Task.CompletedTask;

await Expect.That(ImportAlbumAsync("Abbey Road")).DoesNotThrow();
await Expect.That(Task.FromException(new CustomException("Yesterday"))).Throws<CustomException>();
```

The task is already running when the expectation receives it, so an expectation on the execution time only measures
the duration that remains, and a timeout cannot stop it: the expectation only stops waiting for it and fails with "did
not finish within …". Pass a delegate (`Expect.That(() => ImportAlbumAsync("Abbey Road"))`) to measure the whole
execution, see
[execution time](../03-how-it-works/06-time-and-cancellation.md#execution-time).

A `ValueTask` is consumed by `Expect.That`, so it must not be awaited anywhere else.

## Tasks with a result

A `Task<T>` or `ValueTask<T>` behaves differently: it is awaited and its **result** becomes the subject, so all
expectations for `T` are available. To check the exception or the execution time of such a task, wrap it in a lambda
(`() => task`), which turns it into a delegate:

```csharp
Task<int> playCount = Task.FromResult(42);

await Expect.That(playCount).IsEqualTo(42);
await Expect.That(() => playCount).DoesNotThrow();
```

## The task object

To make an expectation about the task object rather than about what it does, state the type explicitly:

```csharp
await Expect.That<Task>(task).IsNotNull();
```

## Faulted tasks

A task, like an asynchronous [delegate](./01-delegates.md), is awaited, so when it faults with several exceptions
(e.g. from `Task.WhenAll`), only the first one counts as thrown, just as with `await`. A failure message lists the
others under "Other exceptions".
