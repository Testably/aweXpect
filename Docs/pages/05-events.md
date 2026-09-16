# Events

Describes the possible expectations for verifying events.

## Recording

First, you have to start a recording of events. This can be done with the `.Record().Events()` extension method in the "
aweXpect.Recording" namespace.

```csharp
class ThresholdReachedEventArgs(int threshold = 0) : EventArgs
{
    public int Threshold { get; } = threshold;
}
class MyClass
{
  public event EventHandler? ThresholdReached;
  public void OnThresholdReached(ThresholdReachedEventArgs e)
    => ThresholdReached?.Invoke(this, e);
}
MyClass subject = new MyClass();

// ↓ Records all events
IEventRecording<MyClass> recording = subject.Record().Events();
IEventRecording<MyClass> recording = subject.Record().Events(nameof(MyClass.ThresholdReached));
// ↑ Records only the ThresholdReached event
```

## Triggering

You can verify that a recording recorded an event:

```csharp
// Start the recording
IEventRecording<MyClass> recording = subject.Record().Events();

// Perform some action on the subject under test
subject.OnThresholdReached(new ThresholdReachedEventArgs());

// Expect that the ThresholdReached event was triggered at least once
await Expect.That(recording).Triggered(nameof(MyClass.ThresholdReached));
```

You can also verify that a recording did not record an event:

```csharp
IEventRecording<MyClass> recording = subject.Record().Events();

// Perform some action on the subject under test that must stay below the threshold

// Expect that the ThresholdReached event was never triggered
await Expect.That(recording).DidNotTrigger(nameof(MyClass.ThresholdReached));
```

This is equivalent to `.Triggered(nameof(MyClass.ThresholdReached)).Never()`.

## Filtering

You can filter the recorded events based on their parameters.

```csharp
IEventRecording<MyClass> recording = subject.Record().Events();

subject.OnThresholdReached(new ThresholdReachedEventArgs(5));
subject.OnThresholdReached(new ThresholdReachedEventArgs(15));

await Expect.That(recording).Triggered(nameof(MyClass.ThresholdReached))
  .WithParameter<ThresholdReachedEventArgs>(e => e.Threshold > 10);
```

## Timeout

You can specify a timeout within the expected events should be triggered:

```csharp
IEventRecording<MyClass> recording = subject.Record().Events();

_ = Task.Delay(2.Seconds()).ContinueWith(_ => {
    // Trigger the events in the background
    subject.OnThresholdReached(new ThresholdReachedEventArgs(5));
    subject.OnThresholdReached(new ThresholdReachedEventArgs(15));
});

await Expect.That(recording).Triggered(nameof(MyClass.ThresholdReached))
  .WithParameter<ThresholdReachedEventArgs>(e => e.Threshold > 10)
  .Within(3.Seconds());
```

The `.Within(TimeSpan)` method will wait up to 3 seconds for the expected events and
finish successfully as soon as the events are triggered.

More precisely, it stops as soon as the outcome can no longer change, and otherwise waits for the
whole timeout. For an expectation with an upper bound (`DidNotTrigger`, `Never()`,
`AtMost(2.Times())`, `Exactly(1)`) that means the opposite: it waits out the timeout to be sure no
further event arrives, and returns early only when one event too many is recorded.

```csharp
IEventRecording<MyClass> recording = subject.Record().Events();

// Waits for 3 seconds and expects that no ThresholdReached event is triggered in that time
await Expect.That(recording).DidNotTrigger(nameof(MyClass.ThresholdReached))
  .Within(3.Seconds());
```

### Sender

When you follow
the [event best practices](https://learn.microsoft.com/en-us/dotnet/standard/asynchronous-programming-patterns/best-practices-for-implementing-the-event-based-asynchronous-pattern),
you can filter the recorded events based on the sender (the first parameter):

```csharp
IEventRecording<MyClass> recording = subject.Record().Events();

subject.OnThresholdReached(new ThresholdReachedEventArgs(5));

await Expect.That(recording).Triggered(nameof(MyClass.ThresholdReached))
  .WithSender(s => s == subject);
```

### EventArgs

When you follow
the [event best practices](https://learn.microsoft.com/en-us/dotnet/standard/asynchronous-programming-patterns/best-practices-for-implementing-the-event-based-asynchronous-pattern),
you can filter the recorded events based on their `EventArgs` (the second parameter):

```csharp
IEventRecording<MyClass> recording = subject.Record().Events();

subject.OnThresholdReached(new ThresholdReachedEventArgs(5));

await Expect.That(recording).Triggered(nameof(MyClass.ThresholdReached))
  .With<ThresholdReachedEventArgs>(e => e < 10);
```

## Counting

You can verify that an event was recorded a specific number of times

```csharp
IEventRecording<MyClass> recording = subject.Record().Events();

subject.OnThresholdReached(new ThresholdReachedEventArgs(5));
subject.OnThresholdReached(new ThresholdReachedEventArgs(15));

await Expect.That(recording).Triggered(nameof(MyClass.ThresholdReached))
  .Between(1).And(2.Times();
```

You can use the same occurrence constraints as in the [contain](/docs/expectations/collections#contain) method:

- `AtLeast(2.Times())`
- `AtMost(3.Times())`
- `Between(1).And(4.Times())`
- `Exactly(0.Times())`

## Special events

For common events, you can create specific overloads.  
Included are some overloads for the [
`INotifyPropertyChanged.PropertyChanged`](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.inotifypropertychanged.propertychanged)
event:

```csharp
MyClass subject = // ...implements INotifyPropertyChanged
IEventRecording<MyClass> recording = subject.Record().Events();

// do something that triggers the PropertyChanged event
subject.Execute();

await Expect.That(recording).TriggeredPropertyChanged()
  .Because("it should trigger the PropertyChanged event for any property name");

await Expect.That(recording).TriggeredPropertyChangedFor(x => x.MyProperty)
  .Because("it should trigger the PropertyChanged event for the 'MyProperty' property name");

await Expect.That(recording).DidNotTriggerPropertyChanged()
  .Because("it should not trigger for any property name");

await Expect.That(recording).DidNotTriggerPropertyChangedFor(x => x.MyProperty)
  .Because("it should not trigger for the 'MyProperty' property name");
```

## Trimming and Native AOT

A recording has to know the events of its subject and attach a handler to each of them. Reflection provides both
under the JIT, but publishing with trimming or Native AOT enabled removes events that are only reached reflectively,
and the handler for an event with value-type parameters cannot be bound without runtime code generation.

The source generator that ships with the `aweXpect` package closes this gap: for every call site of `Record()`, it
registers the public events of the subject's static type together with a handler factory, so the recording neither
looks the events up nor binds a handler reflectively. The registration runs when your assembly is loaded and needs no
configuration. A subject whose runtime type has a registration is recorded through it, every other subject is
reflected over as before, and a registered handler takes any number of parameters.

The generator works from the declared type, so the same limits apply as for
[equivalency](/docs/expectations/equivalency#trimming-and-native-aot):

- a subject declared as an interface, an abstract class or a base type only reveals the declared type; the recording
  looks up the runtime type of the instance, which stays on reflection,
- a `private`, `protected` or `file`-local type cannot be referenced by generated code and is reflected over,
- a subject that reaches `Record()` through your own extension method is only registered if the extension's parameter
  or type parameter carries `[RequiresEventMetadata]`,
- an event whose handler returns a value or takes a parameter by reference cannot be recorded by the reflective path
  either, and keeps its type on reflection,
- a `struct` subject is never registered, because a handler added to a boxed copy never sees the caller's value.

A type the generator did not see can be named explicitly with `[assembly: GenerateMetadata(typeof(MyClass))]`, which
registers its members and its events.

Under trimming, a recording of a subject without registration cannot tell a missing event from one the trimmer
removed, so the error for an unknown event name, and for an event that a recording of all events did not find, asks
you to root the type.
