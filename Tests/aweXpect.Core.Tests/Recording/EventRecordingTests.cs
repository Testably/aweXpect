using System.Runtime.CompilerServices;
using aweXpect.Core.Metadata;
using aweXpect.Recording;

namespace aweXpect.Core.Tests.Recording;

public sealed class EventRecordingTests
{
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
	public async Task WhenStopIsCalledTwice_ShouldNotThrowAnyException()
	{
		CustomEventClass subject = new();

		IEventRecording<CustomEventClass> recording = subject.Record().Events();
		await recording.StopWhen(_ => false, TimeSpan.Zero);

		await That(() => recording.StopWhen(_ => false, TimeSpan.Zero)).DoesNotThrow();
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

		public void NotifyOtherEvent() => OtherEvent?.Invoke(this, EventArgs.Empty);
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
