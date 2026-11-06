using aweXpect.Core.Extending;
using aweXpect.Recording;

namespace aweXpect.Core.Tests.Core.Extending;

public sealed class EventRecordingsTests
{
	[Test]
	public async Task StopWhen_OnAnotherImplementation_ShouldThrowNotSupportedException()
	{
		ForeignRecording recording = new();

		async Task Act()
			=> await EventRecordings.StopWhen(recording, _ => false, TimeSpan.Zero);

		await That(Act).Throws<NotSupportedException>()
			.WithMessage(
				"Only a recording created by .Record().Events() supports EventRecordings.StopWhen(), but it was EventRecordingsTests.ForeignRecording { }.")
			.Because("only the recording of this library knows the events it recorded");
	}

	[Test]
	public async Task StopWhen_ShouldReturnTheEventsRecordedUntilThen()
	{
		Counter subject = new();
		IEventRecording<Counter> recording = subject.Record().Events();
		subject.Increment();
		subject.Increment();

		IEventRecordingResult result = await EventRecordings.StopWhen(recording, _ => false, TimeSpan.Zero);

		await That(result.GetEventCount(nameof(Counter.Changed))).IsEqualTo(2);
	}

	[Test]
	public async Task StopWhen_WhenRecordingIsNull_ShouldThrowArgumentNullException()
	{
		async Task Act()
			=> await EventRecordings.StopWhen<Counter>(null!, _ => false, TimeSpan.Zero);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("recording").And
			.WithMessage("The 'recording' cannot be null.").AsPrefix();
	}

	private sealed class Counter
	{
		public event EventHandler? Changed;

		public void Increment() => Changed?.Invoke(this, EventArgs.Empty);
	}

	private sealed class ForeignRecording : IEventRecording<Counter>;
}
