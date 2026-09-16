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
	public async Task WhenRegistered_ShouldRecordThroughTheRegistration()
	{
		RegisterCustomEvent();
		RegisteredClass subject = new();

		IEventRecording<RegisteredClass> recording = subject.Record().Events();
		subject.NotifyCustomEvent(1);
		subject.NotifyCustomEvent(2);
		IEventRecordingResult result = await recording.StopWhen(_ => false, TimeSpan.Zero);
		subject.NotifyCustomEvent(3);

		await That(result.GetEventCount(nameof(RegisteredClass.CustomEvent), p => p[0] is 2)).IsEqualTo(1)
			.Because("the registered handler boxes the value-type argument itself");
		await That(result.GetEventCount(nameof(RegisteredClass.CustomEvent))).IsEqualTo(2)
			.Because("stopping the recording removes the registered handler");
		await That(result.ToString(nameof(RegisteredClass.CustomEvent))).IsEqualTo("""
			[
			  CustomEvent(1),
			  CustomEvent(2)
			]
			""");
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

	private static void RegisterCustomEvent()
		=> TypeMetadataRegistry.RegisterEvent<RegisteredClass>(nameof(RegisteredClass.CustomEvent),
			record => new RegisteredClass.CustomEventDelegate(arg1 => record([arg1,])),
			(subject, handler) => subject.CustomEvent += (RegisteredClass.CustomEventDelegate)handler,
			(subject, handler) => subject.CustomEvent -= (RegisteredClass.CustomEventDelegate)handler);

	private sealed class CustomEventClass
	{
		public delegate void CustomEventDelegate(int arg1);

		public event CustomEventDelegate? CustomEvent;

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
