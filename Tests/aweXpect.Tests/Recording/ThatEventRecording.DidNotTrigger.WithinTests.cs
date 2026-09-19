using System.Threading;
using aweXpect.Recording;

namespace aweXpect.Tests;

public sealed partial class ThatEventRecording
{
	public sealed partial class DidNotTrigger
	{
		public sealed class WithinTests
		{
			[Fact]
			public async Task WhenEventIsNotTriggeredWithinTimeout_ShouldSucceed()
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();
				using CancellationTokenSource cts = new();
				CancellationToken token = cts.Token;

				_ = Task.Delay(2000.Milliseconds(), token)
					.ContinueWith(_ => sut.NotifyCustomEvent(), token);

				async Task Act() =>
					await That(recording).DidNotTrigger(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.Within(10.Milliseconds());

				await That(Act).DoesNotThrow();
				cts.Cancel();
			}

			[Fact]
			public async Task WhenEventIsTriggeredWithinTimeout_ShouldFail()
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				_ = Task.Delay(20.Milliseconds())
					.ContinueWith(_ => sut.NotifyCustomEvent());

				async Task Act() =>
					await That(recording).DidNotTrigger(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.Within(5.Seconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that recording
					             has never recorded the CustomEvent event on sut within 0:05,
					             but it was recorded once in [
					               CustomEvent()
					             ] after 0:*
					             """).AsWildcard();
			}
		}
	}
}
