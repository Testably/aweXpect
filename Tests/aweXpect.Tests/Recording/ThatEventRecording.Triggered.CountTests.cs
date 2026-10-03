using aweXpect.Core;
using aweXpect.Recording;

namespace aweXpect.Tests;

public sealed partial class ThatEventRecording
{
	public sealed partial class Triggered
	{
		public sealed class CountTests
		{
			[Theory]
			[InlineData(1, 1, true)]
			[InlineData(2, 1, false)]
			[InlineData(1, 2, true)]
			[InlineData(2, 2, true)]
			[InlineData(8, 2, false)]
			[InlineData(2, 8, true)]
			public async Task ShouldSupportAtLeast(int minimum, int count, bool expectSuccess)
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				sut.NotifyCustomEvents(count);

				async Task Act() =>
					await That(recording).Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.AtLeast(minimum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that recording
					              has recorded the CustomEvent event on sut at least {minimum.ToTimesString()},
					              but it was only recorded {count.ToTimesString()} in *
					              """).AsWildcard();
			}

			[Theory]
			[InlineData(1, 1, true)]
			[InlineData(2, 1, true)]
			[InlineData(1, 2, false)]
			[InlineData(2, 2, true)]
			[InlineData(8, 2, true)]
			[InlineData(2, 8, false)]
			public async Task ShouldSupportAtMost(int maximum, int count, bool expectSuccess)
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				sut.NotifyCustomEvents(count);

				async Task Act() =>
					await That(recording).Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.AtMost(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that recording
					              has recorded the CustomEvent event on sut at most {maximum.ToTimesString()},
					              but it was recorded {count.ToTimesString()} in *
					              """).AsWildcard();
			}

			[Theory]
			[InlineData(0, 1, 3, false)]
			[InlineData(6, 8, 4, false)]
			[InlineData(6, 8, 5, false)]
			[InlineData(6, 8, 6, true)]
			[InlineData(6, 8, 7, true)]
			[InlineData(6, 8, 8, true)]
			[InlineData(6, 8, 9, false)]
			[InlineData(6, 8, 10, false)]
			public async Task ShouldSupportBetween(int minimum, int maximum, int count, bool expectSuccess)
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				sut.NotifyCustomEvents(count);

				async Task Act() =>
					await That(recording).Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.Between(minimum).And(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that recording
					              has recorded the CustomEvent event on sut between {minimum} and {maximum} times,
					              but it was {(count < minimum ? "only " : "")}recorded {(count == 1 ? "once" : $"{count} times")} in *
					              """).AsWildcard();
			}

			[Theory]
			[InlineData(1, 1, true)]
			[InlineData(2, 1, false)]
			[InlineData(1, 2, false)]
			[InlineData(2, 2, true)]
			[InlineData(8, 2, false)]
			[InlineData(2, 8, false)]
			public async Task ShouldSupportExactly(int expected, int count, bool expectSuccess)
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				sut.NotifyCustomEvents(count);

				async Task Act() =>
					await That(recording).Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.Exactly(expected.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that recording
					              has recorded the CustomEvent event on sut exactly {expected.ToTimesString()},
					              but it was {(count < expected ? "only " : "")}recorded {count.ToTimesString()} in *
					              """).AsWildcard();
			}

			[Theory]
			[InlineData(0, true)]
			[InlineData(1, false)]
			public async Task ShouldSupportNever(int count, bool expectSuccess)
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				sut.NotifyCustomEvents(count);

				async Task Act() =>
					await That(recording).Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.Never();

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that recording
					             has never recorded the CustomEvent event on sut,
					             but it was recorded once in *
					             """).AsWildcard();
			}

			[Theory]
			[InlineData(0, false)]
			[InlineData(1, true)]
			[InlineData(2, false)]
			public async Task ShouldSupportOnce(int count, bool expectSuccess)
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				sut.NotifyCustomEvents(count);

				async Task Act() =>
					await That(recording).Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.Once();

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that recording
					              has recorded the CustomEvent event on sut exactly once,
					              but it was {(count == 0 ? "never recorded" : $"recorded {count.ToTimesString()} in *")}
					              """).AsWildcard();
			}
		}
	}
}
