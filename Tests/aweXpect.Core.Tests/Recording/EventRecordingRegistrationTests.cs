#if NET8_0_OR_GREATER && DEBUG
using System.Linq;
using aweXpect.Core.Metadata;
using aweXpect.Recording;

namespace aweXpect.Core.Tests.Recording;

public sealed class EventRecordingRegistrationTests
{
	[Fact]
	public async Task GeneratedRegistration_ShouldRecordAnEventWithMoreThanFourValueTypeParameters()
	{
		Publisher subject = new();

		IEventRecording<Publisher> recording = subject.Record().Events(nameof(Publisher.Counted));
		subject.RaiseCounted(1, "foo", true, DateTime.MinValue, 2);
		IEventRecordingResult result = await recording.StopWhen(_ => false, TimeSpan.Zero);

		await That(result.GetEventCount(nameof(Publisher.Counted), p => p[0] is 1 && p[4] is 2)).IsEqualTo(1)
			.Because("the generated handler boxes every argument itself and has no parameter ceiling");
	}

	[Fact]
	public async Task GeneratedRegistration_ShouldRecordAStaticEvent()
	{
		Publisher subject = new();

		IEventRecording<Publisher> recording = subject.Record().Events(nameof(Publisher.StaticChanged));
		Publisher.RaiseStaticChanged();
		IEventRecordingResult result = await recording.StopWhen(_ => false, TimeSpan.Zero);
		Publisher.RaiseStaticChanged();

		await That(result.GetEventCount(nameof(Publisher.StaticChanged))).IsEqualTo(1)
			.Because("reflection returns the static events of the recorded type, so the registration has to as well");
	}

	[Fact]
	public async Task GeneratedRegistration_ShouldRegisterTheEventsOfTheRecordedType()
	{
		bool isRegistered = TypeMetadataRegistry.Instance.TryGet(typeof(Publisher),
			out TypeMetadataRegistry.TypeMetadata? metadata);

		await That(isRegistered).IsTrue()
			.Because("the generator emits a module initializer for the type recorded in this class");
		await That(metadata!.Events.Values.OrderBy(x => x.Order).Select(x => x.Name))
			.IsEqualTo(["Changed", "Counted", "StaticChanged",])
			.Because("the events are registered in the order reflection returns them");
	}

	[Fact]
	public async Task GeneratedRegistration_WithMissingEventName_ShouldThrowWithoutTrimmingHint()
	{
		Publisher subject = new();

		void Act()
			=> subject.Record().Events("Missing");

		await That(Act).Throws<NotSupportedException>()
			.WithMessage("Event Missing is not supported on EventRecordingRegistrationTests.Publisher { }")
			.Because("a registered type is served from the registry alone, so a missing event is missing for sure");
	}

	public sealed class Publisher
	{
		public delegate void CountedHandler(int count, string name, bool flag, DateTime at, int? optional);

		public event EventHandler? Changed;
		public event CountedHandler? Counted;
		public static event EventHandler? StaticChanged;

		public void RaiseChanged() => Changed?.Invoke(this, EventArgs.Empty);

		public void RaiseCounted(int count, string name, bool flag, DateTime at, int? optional)
			=> Counted?.Invoke(count, name, flag, at, optional);

		public static void RaiseStaticChanged() => StaticChanged?.Invoke(null, EventArgs.Empty);
	}
}
#endif
