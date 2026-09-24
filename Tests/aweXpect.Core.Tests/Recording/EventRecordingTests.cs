using System.Runtime.CompilerServices;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Metadata;
using aweXpect.Recording;

namespace aweXpect.Core.Tests.Recording;

public sealed class EventRecordingTests
{
	[Fact]
	public async Task MissingEventName_ShouldDetachTheAlreadyAttachedEvents()
	{
		CustomEventClass sut = new();

		void Act()
			=> sut.Record().Events(nameof(CustomEventClass.CustomEvent), "someMissingEventName");

		await That(Act).Throws<NotSupportedException>()
			.WithMessage(
				"Event someMissingEventName is not supported on EventRecordingTests.CustomEventClass { }. When publishing with trimming or Native AOT enabled, ensure that the type is rooted, so that its events are preserved.");
		await That(sut.HasSubscribers()).IsFalse()
			.Because("a recording that never completed leaves nothing behind that could ever detach the handler");
	}

	[Fact]
	public async Task MissingEventName_ShouldThrowNotSupportedException()
	{
		CustomEventClass sut = new();

		void Act()
			=> sut.Record().Events("someMissingEventName");

		await That(Act).Throws<NotSupportedException>()
			.WithMessage(
				"Event someMissingEventName is not supported on EventRecordingTests.CustomEventClass { }. When publishing with trimming or Native AOT enabled, ensure that the type is rooted, so that its events are preserved.")
			.Because("reflection cannot tell a missing event from one the trimmer removed");
	}

	[Fact]
	public async Task WhenAnEventCannotBeAttached_ShouldRecordTheOtherEvents()
	{
		ManyParametersClass subject = new();

		IEventRecording<ManyParametersClass> recording = subject.Record().Events();
		subject.NotifyOtherEvent();
		IEventRecordingResult result = await recording.StopWhen(_ => false, TimeSpan.Zero);

		await That(result.GetEventCount(nameof(ManyParametersClass.OtherEvent))).IsEqualTo(1)
			.Because("an event the reflective fallback cannot bind a handler to must not cost the recording of all the other events");
	}

	[Fact]
	public async Task WhenAnEventCannotBeAttached_ShouldThrowNotSupportedExceptionWhenItIsAsserted()
	{
		ManyParametersClass sut = new();
		IEventRecording<ManyParametersClass> recording = sut.Record().Events();
		IEventRecordingResult result = await recording.StopWhen(_ => false, TimeSpan.Zero);

		void Act()
			=> result.GetEventCount(nameof(ManyParametersClass.CustomEvent));

		await That(Act).Throws<NotSupportedException>()
			.WithMessage("The CustomEvent event contains too many parameters (5): [int, int, int, int, int]")
			.Because("a skipped event has to name its reason instead of looking like an event that was never triggered");
	}

	[Fact]
	public async Task WhenAnEventCannotBeAttached_WhenAnotherEventIsRequestedByName_ShouldRecordIt()
	{
		ManyParametersClass subject = new();

		IEventRecording<ManyParametersClass> recording =
			subject.Record().Events(nameof(ManyParametersClass.OtherEvent));
		subject.NotifyOtherEvent();
		IEventRecordingResult result = await recording.StopWhen(_ => false, TimeSpan.Zero);

		await That(result.GetEventCount(nameof(ManyParametersClass.OtherEvent))).IsEqualTo(1)
			.Because("only the requested events are attached, so an unattachable event of the same type is never touched");
	}

	[Fact]
	public async Task WhenAnEventCannotBeAttached_WhenRequestedByName_ShouldDetachTheAlreadyAttachedEvents()
	{
		ManyParametersClass sut = new();

		void Act()
			=> sut.Record().Events(nameof(ManyParametersClass.OtherEvent), nameof(ManyParametersClass.CustomEvent));

		await That(Act).Throws<NotSupportedException>()
			.WithMessage("The CustomEvent event contains too many parameters (5): [int, int, int, int, int]");
		await That(sut.HasOtherEventSubscribers()).IsFalse()
			.Because("a recording that never completed leaves nothing behind that could ever detach the handler");
	}

	[Fact]
	public async Task WhenAnEventCannotBeAttached_WhenRequestedByName_ShouldThrowNotSupportedException()
	{
		ManyParametersClass sut = new();

		void Act()
			=> sut.Record().Events(nameof(ManyParametersClass.CustomEvent));

		await That(Act).Throws<NotSupportedException>()
			.WithMessage("The CustomEvent event contains too many parameters (5): [int, int, int, int, int]")
			.Because("an event that was asked for by name is what the recording is about, so it has to fail right away");
	}

	[Fact]
	public async Task WhenAnEventCannotBeAttached_WithUnrecordedEventName_ShouldNameTheSkippedEvent()
	{
		ManyParametersClass sut = new();
		IEventRecording<ManyParametersClass> recording = sut.Record().Events();
		IEventRecordingResult result = await recording.StopWhen(_ => false, TimeSpan.Zero);

		void Act()
			=> result.GetEventCount("Typo");

		await That(Act).Throws<NotSupportedException>()
			.WithMessage(
				"Event Typo was not recorded on sut, only [\"OtherEvent\"]. No handler could be attached to [\"CustomEvent\"]. When publishing with trimming or Native AOT enabled, ensure that the type is rooted, so that its events are preserved.")
			.Because("a skipped event is missing from the recorded ones for a reason that the message has to name");
	}

	[Fact]
	public async Task WhenChainedWithAnd_ShouldCheckEveryConstraint()
	{
		CustomEventClass sut = new();
		IEventRecording<CustomEventClass> recording = sut.Record().Events();
		sut.NotifyCustomEvent(1);
		sut.NotifyCustomEvent(2);

		async Task Act()
			=> await That(recording).Triggered(nameof(CustomEventClass.CustomEvent)).Twice()
				.And.Triggered(nameof(CustomEventClass.CustomEvent)).AtLeast().Once()
				.And.Triggered(nameof(CustomEventClass.CustomEvent)).Never();

		await That(Act).Throws<XunitException>()
			.WithMessage("*has never recorded the CustomEvent event*").AsWildcard()
			.Because("every constraint of one expectation checks the same recording, and only the third one fails");
	}

	[Fact]
	public async Task WhenChainedWithAnd_ShouldNotStopTheRecordingForTheNextConstraint()
	{
		CustomEventClass sut = new();
		IEventRecording<CustomEventClass> recording = sut.Record().Events();
		sut.NotifyCustomEvent(1);

		async Task Act()
			=> await That(recording).Triggered(nameof(CustomEventClass.CustomEvent)).Once()
				.And.Triggered(nameof(CustomEventClass.CustomEvent)).Once();

		await That(Act).DoesNotThrow()
			.Because("the constraints of one awaited expectation share the evaluation that stopped the recording");
	}

	[Fact]
	public async Task WhenChainedWithOr_ShouldCheckEveryConstraint()
	{
		CustomEventClass sut = new();
		IEventRecording<CustomEventClass> recording = sut.Record().Events();
		sut.NotifyCustomEvent(1);

		async Task Act()
			=> await That(recording).Triggered(nameof(CustomEventClass.CustomEvent)).Never()
				.Or.Triggered(nameof(CustomEventClass.CustomEvent)).Once();

		await That(Act).DoesNotThrow()
			.Because("the second constraint is only reached because the first one did not stop the expectation");
	}

	[Fact]
	public async Task WhenDisposed_ShouldStopListening()
	{
		CustomEventClass subject = new();
		IDisposableEventRecording<CustomEventClass> recording = subject.Record().Events().UntilDisposed();
		subject.NotifyCustomEvent(1);
		IEventRecordingResult result = await recording.StopWhen(_ => false, TimeSpan.Zero);

		recording.Dispose();
		subject.NotifyCustomEvent(2);

		await That(subject.HasSubscribers()).IsFalse()
			.Because("disposing detaches the handler from the subject");
		await That(result.GetEventCount(nameof(CustomEventClass.CustomEvent))).IsEqualTo(1)
			.Because("an event that is triggered after the disposal is not recorded any more");
	}

	[Fact]
	public async Task WhenDisposed_WithAFurtherExpectation_ShouldThrowInvalidOperationException()
	{
		CustomEventClass sut = new();
		IDisposableEventRecording<CustomEventClass> recording = sut.Record().Events().UntilDisposed();
		recording.Dispose();

		async Task Act()
			=> await That(recording).Triggered(nameof(CustomEventClass.CustomEvent));

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("The recording was already disposed.").AsSuffix()
			.Because("a disposed recording is detached and would answer from its frozen queue");
	}

	[Fact]
	public async Task WhenEvaluatedEventually_ShouldThrowInvalidOperationExceptionOnTheRetry()
	{
		CustomEventClass sut = new();
		IEventRecording<CustomEventClass> recording = sut.Record().Events();

		async Task Act()
			=> await That(() => recording).Eventually()
				.Triggered(nameof(CustomEventClass.CustomEvent)).Once()
				.WithTimeout(TimeSpan.FromSeconds(30));

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage(
				"The recording was already stopped. Use .UntilDisposed() to keep recording across multiple expectations.")
			.AsSuffix()
			.Because("every retry is a further evaluation, and a retry of the stopped recording could only see the same frozen snapshot");
	}

	[Fact]
	public async Task WhenEventWasNotRecorded_ShouldThrowNotSupportedException()
	{
		CustomEventClass sut = new();
		IEventRecording<CustomEventClass> recording = sut.Record().Events();
		IEventRecordingResult result = await recording.StopWhen(_ => false, TimeSpan.Zero);

		void Act()
			=> result.GetEventCount("OtherEvent");

		await That(Act).Throws<NotSupportedException>()
			.WithMessage(
				"Event OtherEvent was not recorded on sut, only [\"CustomEvent\"]. When publishing with trimming or Native AOT enabled, ensure that the type is rooted, so that its events are preserved.")
			.Because("a recording of all events records nothing when the trimmer removed them, so the access has to fail loudly");
	}

	[Fact]
	public async Task WhenExpectationIsEvaluatedTwice_ShouldThrowInvalidOperationException()
	{
		CustomEventClass sut = new();
		IEventRecording<CustomEventClass> recording = sut.Record().Events();
		sut.NotifyCustomEvent(1);
		await That(recording).Triggered(nameof(CustomEventClass.CustomEvent)).Once();
		sut.NotifyCustomEvent(2);

		async Task Act()
			=> await That(recording).Triggered(nameof(CustomEventClass.CustomEvent)).Twice();

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage(
				"The recording was already stopped. Use .UntilDisposed() to keep recording across multiple expectations.")
			.AsSuffix()
			.Because("the stopped recording would otherwise answer the second expectation from stale data");
	}

	[Fact]
	public async Task WhenExpectationIsFollowedByAStop_ShouldThrowInvalidOperationException()
	{
		CustomEventClass sut = new();
		IEventRecording<CustomEventClass> recording = sut.Record().Events();
		sut.NotifyCustomEvent(1);
		await That(recording).Triggered(nameof(CustomEventClass.CustomEvent)).Once();

		async Task Act()
			=> await recording.StopWhen(_ => false, TimeSpan.Zero);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage(
				"The recording was already stopped. Use .UntilDisposed() to keep recording across multiple expectations.")
			.Because("the evaluation that stopped the recording does not reach beyond the expectation that opened it");
	}

	[Fact]
	public async Task WhenHandlerReturnsAValue_ShouldThrowNotSupportedException()
	{
		ReturningHandlerClass sut = new();

		void Act()
			=> sut.Record().Events(nameof(ReturningHandlerClass.CustomEvent));

		await That(Act).Throws<NotSupportedException>()
			.WithMessage("The CustomEvent event cannot be recorded, because its handler returns int")
			.Because("the recorder cannot supply a return value, so the reason has to be named instead of a binding error");
	}

	[Fact]
	public async Task WhenHandlerReturnsAValue_WhenRecordingAllEvents_ShouldSkipTheEvent()
	{
		ReturningHandlerClass sut = new();
		IEventRecording<ReturningHandlerClass> recording = sut.Record().Events();
		IEventRecordingResult result = await recording.StopWhen(_ => false, TimeSpan.Zero);

		void Act()
			=> result.GetEventCount(nameof(ReturningHandlerClass.CustomEvent));

		await That(Act).Throws<NotSupportedException>()
			.WithMessage("The CustomEvent event cannot be recorded, because its handler returns int")
			.Because("the reason is kept until the event is asked for, so that the other events can still be recorded");
	}

	[Fact]
	public async Task WhenHandlerTakesAParameterByReference_ShouldThrowNotSupportedException()
	{
		ByReferenceHandlerClass sut = new();

		void Act()
			=> sut.Record().Events(nameof(ByReferenceHandlerClass.CustomEvent));

		await That(Act).Throws<NotSupportedException>()
			.WithMessage("The CustomEvent event cannot be recorded, because its handler takes the parameter value by reference")
			.Because("a by-reference parameter cannot be boxed into the recorded arguments");
	}

	[Fact]
	public async Task WhenHandlerTakesAParameterByReference_WhenRecordingAllEvents_ShouldSkipTheEvent()
	{
		ByReferenceHandlerClass sut = new();
		IEventRecording<ByReferenceHandlerClass> recording = sut.Record().Events();
		IEventRecordingResult result = await recording.StopWhen(_ => false, TimeSpan.Zero);

		void Act()
			=> result.GetEventCount(nameof(ByReferenceHandlerClass.CustomEvent));

		await That(Act).Throws<NotSupportedException>()
			.WithMessage("The CustomEvent event cannot be recorded, because its handler takes the parameter value by reference")
			.Because("the reason is kept until the event is asked for, so that the other events can still be recorded");
	}

	[Fact]
	public async Task WhenNoEventCanBeAttached_WithUnrecordedEventName_ShouldNotClaimThatNoEventWasFound()
	{
		OnlyUnrecordableClass sut = new();
		IEventRecording<OnlyUnrecordableClass> recording = sut.Record().Events();
		IEventRecordingResult result = await recording.StopWhen(_ => false, TimeSpan.Zero);

		void Act()
			=> result.GetEventCount("Typo");

		await That(Act).Throws<NotSupportedException>()
			.WithMessage(
				"Event Typo was not recorded on sut, because no event was recorded. No handler could be attached to [\"CustomEvent\"]. When publishing with trimming or Native AOT enabled, ensure that the type is rooted, so that its events are preserved.")
			.Because("reflection did find an event, so blaming an empty recording on a removed event would mislead");
	}

	[Fact]
	public async Task WhenNoEventWasFound_ShouldThrowNotSupportedException()
	{
		WithoutEvents sut = new();
		IEventRecording<WithoutEvents> recording = sut.Record().Events();
		IEventRecordingResult result = await recording.StopWhen(_ => false, TimeSpan.Zero);

		void Act()
			=> result.GetEventCount("CustomEvent");

		await That(Act).Throws<NotSupportedException>()
			.WithMessage(
				"Event CustomEvent was not recorded on sut, because no event was found. When publishing with trimming or Native AOT enabled, ensure that the type is rooted, so that its events are preserved.")
			.Because("a recording that found no event at all is the symptom of a trimmed type, so the message must not read like a bug");
	}

	[Fact]
	public async Task WhenNoFilterIsApplied_ShouldCountAllRecordings()
	{
		CustomEventClass subject = new();

		IEventRecording<CustomEventClass> recording = subject.Record().Events();
		subject.NotifyCustomEvent(1);
		subject.NotifyCustomEvent(2);
		IEventRecordingResult result = await recording.StopWhen(_ => false, TimeSpan.Zero);
		subject.NotifyCustomEvent(3);

		await That(result.GetEventCount(nameof(CustomEventClass.CustomEvent))).IsEqualTo(2);
	}

	[Fact]
	public async Task WhenOnlyMembersAreRegistered_ShouldReflectOverTheEvents()
	{
		TypeMetadataRegistry.RegisterProperty<MemberRegisteredClass, int>(nameof(MemberRegisteredClass.Number),
			x => x.Number);
		MemberRegisteredClass subject = new();

		IEventRecording<MemberRegisteredClass> recording = subject.Record().Events();
		subject.NotifyCustomEvent(1);
		IEventRecordingResult result = await recording.StopWhen(_ => false, TimeSpan.Zero);

		await That(result.GetEventCount(nameof(MemberRegisteredClass.CustomEvent))).IsEqualTo(1)
			.Because("a member registration says nothing about the events, which still have to be reflected over");
	}

	[Fact]
	public async Task WhenRegistered_ShouldRecordOnlyTheRegisteredEvents()
	{
		RegisterCustomEvent();
		RegisteredClass subject = new();

		void Act()
			=> subject.Record().Events();

		await That(Act).DoesNotThrow();
		await That(() => subject.Record().Events(nameof(RegisteredClass.OtherEvent)))
			.Throws<NotSupportedException>()
			.WithMessage("Event OtherEvent is not supported on EventRecordingTests.RegisteredClass { }")
			.Because("a registered type is served from the registry alone, so a missing event is missing for sure");
	}

	[Fact]
	public async Task WhenRegistered_ShouldRecordThroughTheRegistration()
	{
		RegisterCustomEvent();
		RegisteredClass subject = new();

		IEventRecording<RegisteredClass> recording = subject.Record().Events();
		subject.NotifyCustomEvent(1);
		subject.NotifyCustomEvent(2);
		IEventRecordingResult result = await recording.StopWhen(_ => false, TimeSpan.Zero);
		subject.NotifyCustomEvent(3);

		await That(result.GetEventCount(nameof(RegisteredClass.CustomEvent), p => p[0] is 20)).IsEqualTo(1)
			.Because("the registered factory produced the recorded arguments, which reflection could not have done");
		await That(result.GetEventCount(nameof(RegisteredClass.CustomEvent))).IsEqualTo(2)
			.Because("stopping the recording removes the registered handler");
		await That(result.ToString(nameof(RegisteredClass.CustomEvent))).IsEqualTo("""
			[
			  CustomEvent(10),
			  CustomEvent(20)
			]
			""");
	}

	[Fact]
	public async Task WhenRegistered_WithUnrecordedEventName_ShouldThrowWithoutTrimmingHint()
	{
		RegisterCustomEvent();
		RegisteredClass sut = new();
		IEventRecording<RegisteredClass> recording = sut.Record().Events(nameof(RegisteredClass.CustomEvent));
		IEventRecordingResult result = await recording.StopWhen(_ => false, TimeSpan.Zero);

		void Act()
			=> result.ToString(nameof(RegisteredClass.OtherEvent));

		await That(Act).Throws<NotSupportedException>()
			.WithMessage("Event OtherEvent was not recorded on sut, only [\"CustomEvent\"]")
			.Because("a registered type is served from the registry alone, so nothing could have been removed");
	}

	[Fact]
	public async Task WhenStopIsCalled_ShouldRemoveAStaticHandler()
	{
		IEventRecording<StaticEventClass> recording = RecordStaticEvent();
		GC.Collect();
		GC.WaitForPendingFinalizers();

		IEventRecordingResult result = await recording.StopWhen(_ => false, TimeSpan.Zero);
		StaticEventClass.NotifyStaticEvent();

		await That(StaticEventClass.HasSubscribers()).IsFalse()
			.Because("a static handler does not need the subject to be removed, so a collected subject must not keep it attached");
		await That(result.GetEventCount(nameof(StaticEventClass.StaticEvent))).IsEqualTo(0);
	}

	[Fact]
	public async Task WhenStopIsCalled_ShouldStopListening()
	{
		CustomEventClass subject = new();

		IEventRecording<CustomEventClass> recording = subject.Record().Events();
		subject.NotifyCustomEvent(1);
		subject.NotifyCustomEvent(2);

		IEventRecordingResult result = await recording.StopWhen(_ => false, TimeSpan.Zero);

		subject.NotifyCustomEvent(3);

		await That(result.GetEventCount(nameof(CustomEventClass.CustomEvent), _ => true)).IsEqualTo(2);
	}

	[Fact]
	public async Task WhenStopIsCalledTwice_ShouldThrowInvalidOperationException()
	{
		CustomEventClass sut = new();

		IEventRecording<CustomEventClass> recording = sut.Record().Events();
		await recording.StopWhen(_ => false, TimeSpan.Zero);

		async Task Act()
			=> await recording.StopWhen(_ => false, TimeSpan.Zero);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage(
				"The recording was already stopped. Use .UntilDisposed() to keep recording across multiple expectations.")
			.Because("the detached recording would evaluate the predicate against stale data");
	}

	[Fact]
	public async Task WhenStopPredicateThrows_ShouldStillStopListening()
	{
		CustomEventClass subject = new();
		IEventRecording<CustomEventClass> recording = subject.Record().Events();

		async Task Act()
			=> await recording.StopWhen(_ => throw new InvalidOperationException("boom"), TimeSpan.FromSeconds(1));

		await That(Act).Throws<InvalidOperationException>().WithMessage("boom");
		subject.NotifyCustomEvent(1);
		await That(subject.HasSubscribers()).IsFalse()
			.Because("a predicate that throws must not leave the handler attached to the subject");
	}

	[Fact]
	public async Task WhenStopTimeoutExceedsTheTimerRange_ShouldWaitForTheEvent()
	{
		CustomEventClass subject = new();
		IEventRecording<CustomEventClass> recording = subject.Record().Events();
		_ = Task.Run(async () =>
		{
			await Task.Delay(TimeSpan.FromMilliseconds(50));
			subject.NotifyCustomEvent(1);
		});

		IEventRecordingResult result = await recording.StopWhen(
			r => r.GetEventCount(nameof(CustomEventClass.CustomEvent)) > 0, TimeSpan.MaxValue);

		await That(result.GetEventCount(nameof(CustomEventClass.CustomEvent))).IsEqualTo(1)
			.Because("a timeout beyond the range of the timers must not throw");
	}

	[Fact]
	public async Task WhenUntilDisposed_OnAnotherImplementation_ShouldThrowNotSupportedException()
	{
		ForeignRecording sut = new();

		void Act()
			=> sut.UntilDisposed();

		await That(Act).Throws<NotSupportedException>()
			.WithMessage(
				"Only a recording created by .Record().Events() supports .UntilDisposed(), but was EventRecordingTests.ForeignRecording { }")
			.Because("only the recording of this library knows when it detaches its handlers");
	}

	[Fact]
	public async Task WhenUntilDisposed_ShouldAllowMultipleExpectations()
	{
		CustomEventClass sut = new();
		using IDisposableEventRecording<CustomEventClass> recording = sut.Record().Events().UntilDisposed();

		sut.NotifyCustomEvent(1);
		await That(recording).Triggered(nameof(CustomEventClass.CustomEvent)).Once();
		sut.NotifyCustomEvent(2);

		await That(recording).Triggered(nameof(CustomEventClass.CustomEvent)).Twice()
			.Because("the recording keeps counting until it is disposed");
	}

	[Fact]
	public async Task WhenUntilDisposed_WhenCalledTwice_ShouldReturnTheSameRecording()
	{
		CustomEventClass sut = new();
		IEventRecording<CustomEventClass> recording = sut.Record().Events();

		IDisposableEventRecording<CustomEventClass> result = recording.UntilDisposed().UntilDisposed();

		await That(result).IsSameAs(recording)
			.Because("the opt-in only decides when the recording stops, so repeating it changes nothing");
	}

	[Fact]
	public async Task WhenUntilDisposed_WhenStopPredicateThrows_ShouldKeepListeningUntilDisposed()
	{
		CustomEventClass subject = new();
		IDisposableEventRecording<CustomEventClass> recording = subject.Record().Events().UntilDisposed();

		async Task Act()
			=> await recording.StopWhen(_ => throw new InvalidOperationException("boom"), TimeSpan.FromSeconds(1));

		await That(Act).Throws<InvalidOperationException>().WithMessage("boom");
		await That(subject.HasSubscribers()).IsTrue()
			.Because("the caller owns the lifetime of the recording once it opted in");

		recording.Dispose();

		await That(subject.HasSubscribers()).IsFalse()
			.Because("a predicate that threw must not leave the handler attached beyond the disposal");
	}

	[Fact]
	public async Task WhenUntilDisposed_WhenTheRecordingWasStopped_ShouldThrowInvalidOperationException()
	{
		CustomEventClass sut = new();
		IEventRecording<CustomEventClass> recording = sut.Record().Events();
		await recording.StopWhen(_ => false, TimeSpan.Zero);

		void Act()
			=> recording.UntilDisposed();

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage(
				"The recording was already stopped and cannot be continued. Call .UntilDisposed() before the first expectation.")
			.Because("the handlers are already detached, so nothing could be recorded from then on");
	}

	[Fact]
	public async Task WhenUntilDisposed_WithEventName_ShouldAllowMultipleExpectations()
	{
		CustomEventClass sut = new();
		using IDisposableEventRecording<CustomEventClass> recording =
			sut.Record().Events(nameof(CustomEventClass.CustomEvent)).UntilDisposed();

		sut.NotifyCustomEvent(1);
		await That(recording).Triggered(nameof(CustomEventClass.CustomEvent)).Once();
		sut.NotifyCustomEvent(2);

		await That(recording).Triggered(nameof(CustomEventClass.CustomEvent)).Twice()
			.Because("a recording of a single event keeps counting until it is disposed as well");
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static IEventRecording<StaticEventClass> RecordStaticEvent()
		=> new StaticEventClass().Record().Events(nameof(StaticEventClass.StaticEvent));

	private static void RegisterCustomEvent()
		=> TypeMetadataRegistry.RegisterEvent<RegisteredClass>(nameof(RegisteredClass.CustomEvent),
			record => new RegisteredClass.CustomEventDelegate(arg1 => record([arg1 * 10,])),
			(subject, handler) => subject.CustomEvent += (RegisteredClass.CustomEventDelegate)handler,
			(subject, handler) => subject.CustomEvent -= (RegisteredClass.CustomEventDelegate)handler);

	private sealed class CustomEventClass
	{
		public delegate void CustomEventDelegate(int arg1);

		public event CustomEventDelegate? CustomEvent;

		public bool HasSubscribers() => CustomEvent is not null;

		public void NotifyCustomEvent(int arg1)
			=> CustomEvent?.Invoke(arg1);
	}

	private sealed class ForeignRecording : IEventRecording<CustomEventClass>
	{
		public Task<IEventRecordingResult> StopWhen(Func<IEventRecordingResult, bool> areFound, TimeSpan timeout,
			IEvaluationContext? context = null)
			=> throw new NotSupportedException();
	}

	private sealed class MemberRegisteredClass
	{
		public delegate void CustomEventDelegate(int arg1);

		public event CustomEventDelegate? CustomEvent;

		public int Number { get; set; }

		public void NotifyCustomEvent(int arg1)
			=> CustomEvent?.Invoke(arg1);
	}

	private sealed class WithoutEvents;

	private sealed class StaticEventClass
	{
		public static event EventHandler? StaticEvent;

		public static bool HasSubscribers() => StaticEvent is not null;

		public static void NotifyStaticEvent() => StaticEvent?.Invoke(null, EventArgs.Empty);
	}

	private sealed class ReturningHandlerClass
	{
		public delegate int CustomEventDelegate(int value);

#pragma warning disable CS0067 // Event is never used
		public event CustomEventDelegate? CustomEvent;
#pragma warning restore CS0067 // Event is never used
	}

	private sealed class ByReferenceHandlerClass
	{
		public delegate void CustomEventDelegate(ref int value);

#pragma warning disable CS0067 // Event is never used
		public event CustomEventDelegate? CustomEvent;
#pragma warning restore CS0067 // Event is never used
	}

	private sealed class ManyParametersClass
	{
		public delegate void CustomEventDelegate(int arg1, int arg2, int arg3, int arg4, int arg5);

#pragma warning disable CS0067 // Event is never used
		public event CustomEventDelegate? CustomEvent;
#pragma warning restore CS0067 // Event is never used

		public event EventHandler? OtherEvent;

		public bool HasOtherEventSubscribers() => OtherEvent is not null;

		public void NotifyOtherEvent() => OtherEvent?.Invoke(this, EventArgs.Empty);
	}

	private sealed class OnlyUnrecordableClass
	{
		public delegate void CustomEventDelegate(int arg1, int arg2, int arg3, int arg4, int arg5);

#pragma warning disable CS0067 // Event is never used
		public event CustomEventDelegate? CustomEvent;
#pragma warning restore CS0067 // Event is never used
	}

	private sealed class RegisteredClass
	{
		public delegate void CustomEventDelegate(int arg1);

		public event CustomEventDelegate? CustomEvent;

#pragma warning disable CS0067 // Event is never used
		public event EventHandler? OtherEvent;
#pragma warning restore CS0067 // Event is never used

		public void NotifyCustomEvent(int arg1)
			=> CustomEvent?.Invoke(arg1);
	}
}
