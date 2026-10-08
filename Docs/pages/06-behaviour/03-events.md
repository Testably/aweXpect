# Events

Describes the possible expectations for verifying events.

| Expectation                                      | Negated                           | Summary                                       |
|--------------------------------------------------|-----------------------------------|-----------------------------------------------|
| [`Triggered`](#triggering)                       | `DidNotTrigger`                   | the recording recorded the event              |
| [`TriggeredPropertyChanged`](#special-events)    | `DidNotTriggerPropertyChanged`    | `PropertyChanged` was raised for any property |
| [`TriggeredPropertyChangedFor`](#special-events) | `DidNotTriggerPropertyChangedFor` | `PropertyChanged` was raised for the property |

The events are recorded first. The samples on this page use the following recording:

```csharp
using aweXpect.Recording;

class TrackStartedEventArgs(string title = "") : EventArgs
{
  public string Title { get; } = title;
}
class Player
{
  public event EventHandler<TrackStartedEventArgs>? TrackStarted;
  public void Play(string title)
    => TrackStarted?.Invoke(this, new TrackStartedEventArgs(title));
}
Player player = new Player();

// ↓ Records all events
IEventRecording<Player> recording = player.Record().Events();
IEventRecording<Player> trackRecording = player.Record().Events(nameof(Player.TrackStarted));
// ↑ Records only the TrackStarted event
```

## Recording

`.Record().Events()` in the `aweXpect.Recording` namespace starts a recording of all events of the subject, or of the
events with the given names.

<details>
<summary>Events that cannot be recorded</summary>

Without a registration from the [source generator](../03-how-it-works/08-native-aot.md#events), the handler is bound
reflectively. Such a handler must take at most four parameters, must return nothing and must take no parameter by
reference. Recording all events skips an event whose handler does not fit, so that the other events of the subject are
still recorded, and an expectation on the skipped event throws a `NotSupportedException` with the reason; recording it
by name throws right away.

</details>

### Stopping

An expectation stops the recording: it detaches the handlers from the subject as soon as it is evaluated. Until then,
every constraint of that one expectation sees the recorded events, including the ones that arrive while it waits with
`Within(…)`, because `.And` and `.Or` combine into a single expectation. A further expectation on the same recording
throws an `InvalidOperationException`, so that it cannot silently answer from the events that were recorded until then:

```csharp
IEventRecording<Player> recording = player.Record().Events();

player.Play("Let It Be");
await Expect.That(recording).Triggered(nameof(Player.TrackStarted)).Once();

player.Play("Yesterday");
// ↓ throws, because the previous expectation already stopped the recording
await Expect.That(recording).Triggered(nameof(Player.TrackStarted)).Twice();
```

<details>
<summary>`Expect.ThatAll(…)` and `Expect.ThatAny(…)`</summary>

The expectations within one `Expect.ThatAll(…)` or `Expect.ThatAny(…)`, including nested ones, share the recording: it
is stopped when the whole combination was evaluated. They are evaluated one after the other, so the events that arrive
while one of them waits with `Within(…)` also count for the following ones, as with `.And`.

</details>

`.UntilDisposed()` keeps the recording running across multiple expectations and hands its lifetime to you:

```csharp
using IDisposableEventRecording<Player> recording = player.Record().Events().UntilDisposed();

player.Play("Let It Be");
await Expect.That(recording).Triggered(nameof(Player.TrackStarted)).Once();

player.Play("Yesterday");
await Expect.That(recording).Triggered(nameof(Player.TrackStarted)).Twice();
```

Disposing detaches the handlers, so an event that is triggered afterwards is not recorded any more and an
expectation on the disposed recording throws as well.

## Triggering

You can verify that a recording recorded an event:

```csharp
IEventRecording<Player> recording = player.Record().Events();

player.Play("Let It Be");

await Expect.That(recording).Triggered(nameof(Player.TrackStarted));
```

You can also verify that a recording did not record an event:

```csharp
IEventRecording<Player> recording = player.Record().Events();

// Perform an action on the player that must not start a track

await Expect.That(recording).DidNotTrigger(nameof(Player.TrackStarted));
```

`Triggered` expects the event at least once, and `DidNotTrigger` is equivalent to `Triggered(…).Never()`. A count
negates the expectation, so `DidNotTrigger(nameof(Player.TrackStarted)).AtLeast(2.Times())` expects the event to be
triggered less than twice.

Without `Within(…)`, only the events recorded so far count. To wait for events that are triggered in the background,
see [waiting for events](../03-how-it-works/06-time-and-cancellation.md#events).

### Counting

You can verify that an event was recorded a specific number of times:

```csharp
using aweXpect.Core; // for `Times()`

IEventRecording<Player> recording = player.Record().Events();

player.Play("Let It Be");
player.Play("Yesterday");

await Expect.That(recording).Triggered(nameof(Player.TrackStarted)).Between(1).And(2.Times());
```

The same occurrence constraints as for [`Contains`](../05-collections/01-equality.md#contained-items) are available:
`AtLeast(2.Times())`, `AtMost(3.Times())`, `Between(1).And(4.Times())`, `Exactly(0.Times())`, `MoreThan(1.Times())`,
`LessThan(3.Times())`, `Once()`, `Twice()` and `Never()`.

## Filtering

You can filter the recorded events based on their parameters:

```csharp
IEventRecording<Player> recording = player.Record().Events();

player.Play("Let It Be");
player.Play("Yesterday");

await Expect.That(recording).Triggered(nameof(Player.TrackStarted))
  .WithParameter<TrackStartedEventArgs>(e => e.Title == "Yesterday");
```

This matches an event when any of its parameters is of the given type and satisfies the predicate. To check the
parameter at a specific zero-based position instead, pass the position first:

```csharp
IEventRecording<Player> recording = player.Record().Events();

player.Play("Yesterday");

await Expect.That(recording).Triggered(nameof(Player.TrackStarted))
  .WithParameter<TrackStartedEventArgs>(1, e => e.Title == "Yesterday");
```

An event whose parameter at that position is missing or of another type does not match.

<details>
<summary>`null` parameters</summary>

A parameter that is `null` is passed to the predicate, unless the given type is a non-nullable value type; without a
position, `null` parameters are ignored. The predicates for the sender and for the `EventArgs` below also receive a
value that is `null`. A negative position throws an `ArgumentOutOfRangeException`.

</details>

When you follow
the [event best practices](https://learn.microsoft.com/en-us/dotnet/standard/asynchronous-programming-patterns/best-practices-for-implementing-the-event-based-asynchronous-pattern),
you can also filter the recorded events based on the sender (the first parameter) or on their `EventArgs` (the second
parameter):

```csharp
IEventRecording<Player> recording = player.Record().Events();

player.Play("Let It Be");

await Expect.That(recording).Triggered(nameof(Player.TrackStarted))
  .WithSender(s => s == player)
  .Because("the sender is the first parameter");
```

```csharp
IEventRecording<Player> recording = player.Record().Events();

player.Play("Let It Be");

await Expect.That(recording).Triggered(nameof(Player.TrackStarted))
  .With<TrackStartedEventArgs>(e => e.Title.StartsWith("Let"))
  .Because("the EventArgs are the second parameter");
```

## Special events

aweXpect includes overloads for the
[`INotifyPropertyChanged.PropertyChanged`](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.inotifypropertychanged.propertychanged)
event:

```csharp
AlbumViewModel album = // ...implements INotifyPropertyChanged
using IDisposableEventRecording<AlbumViewModel> recording = album.Record().Events().UntilDisposed();

album.Rename("Let It Be... Naked");

await Expect.That(recording).TriggeredPropertyChanged()
  .Because("it should trigger the PropertyChanged event for any property name");
await Expect.That(recording).TriggeredPropertyChangedFor(x => x.Title)
  .Because("it should trigger the PropertyChanged event for the 'Title' property name");
```

The expression has to access a property directly on the subject, as in `x => x.Title`. A nested access like
`x => x.Artist.Name`, a property of another object or a computed value like `x => !x.IsFavorite` throws an
`ArgumentException`. For any other name, use the overload with the property name, e.g.
`TriggeredPropertyChangedFor("Title")`.

The negated expectations verify that the event was not triggered:

```csharp
AlbumViewModel album = // ...implements INotifyPropertyChanged
using IDisposableEventRecording<AlbumViewModel> recording = album.Record().Events().UntilDisposed();

// do something that must not change the album

await Expect.That(recording).DidNotTriggerPropertyChanged()
  .Because("it should not trigger for any property name");
await Expect.That(recording).DidNotTriggerPropertyChangedFor(x => x.Title)
  .Because("it should not trigger for the 'Title' property name");
```

An event with a `null` or empty property name counts as a change of every property.

<details>
<summary>`null` or empty property names</summary>

As defined by the `INotifyPropertyChanged`
[contract](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.inotifypropertychanged.propertychanged#remarks),
an event that was triggered with a `null` or empty property name notifies that *all* properties changed: it
satisfies `TriggeredPropertyChangedFor` for every property name and lets `DidNotTriggerPropertyChangedFor`
fail for every property name. A whitespace-only name is a name like any other. Expecting the `null` or the empty
property name itself, e.g. `TriggeredPropertyChangedFor((string?)null)`, matches only the events that notify that all
properties changed, but no named one, and without distinguishing the two spellings, which the contract allows
interchangeably. The expectation then reads "for all properties".

</details>
