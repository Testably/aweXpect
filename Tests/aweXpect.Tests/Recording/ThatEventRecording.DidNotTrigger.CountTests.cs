using System.ComponentModel;
using aweXpect.Core;
using aweXpect.Recording;

namespace aweXpect.Tests;

public sealed partial class ThatEventRecording
{
	public sealed partial class DidNotTrigger
	{
		public sealed class CountTests
		{
			[Theory]
			[InlineData(2, 1, true)]
			[InlineData(2, 2, false)]
			[InlineData(2, 8, false)]
			[InlineData(8, 2, true)]
			public async Task ShouldSupportAtLeast(int minimum, int count, bool expectSuccess)
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				sut.NotifyCustomEvents(count);

				async Task Act() =>
					await That(recording).DidNotTrigger(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.AtLeast(minimum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that recording
					              has recorded the CustomEvent event on sut fewer than {minimum.ToTimesString()},
					              but it was recorded {count.ToTimesString()} in *
					              """).AsWildcard();
			}

			[Theory]
			[InlineData(0, true)]
			[InlineData(1, false)]
			public async Task ShouldSupportAtLeastOnce(int count, bool expectSuccess)
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				sut.NotifyCustomEvents(count);

				async Task Act() =>
					await That(recording).DidNotTrigger(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.AtLeast(1.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that recording
					             has never recorded the CustomEvent event on sut,
					             but it was recorded once in *
					             """).AsWildcard();
			}

			[Theory]
			[InlineData(1, 1, false)]
			[InlineData(1, 2, true)]
			[InlineData(2, 2, false)]
			[InlineData(2, 8, true)]
			[InlineData(8, 2, false)]
			public async Task ShouldSupportAtMost(int maximum, int count, bool expectSuccess)
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				sut.NotifyCustomEvents(count);

				async Task Act() =>
					await That(recording).DidNotTrigger(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.AtMost(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that recording
					              has recorded the CustomEvent event on sut more than {maximum.ToTimesString()},
					              but it was recorded {count.ToTimesString()} in *
					              """).AsWildcard();
			}

			[Theory]
			[InlineData(0, 1, 0, false)]
			[InlineData(0, 1, 3, true)]
			[InlineData(6, 8, 5, true)]
			[InlineData(6, 8, 6, false)]
			[InlineData(6, 8, 8, false)]
			[InlineData(6, 8, 9, true)]
			public async Task ShouldSupportBetween(int minimum, int maximum, int count, bool expectSuccess)
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				sut.NotifyCustomEvents(count);

				async Task Act() =>
					await That(recording).DidNotTrigger(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.Between(minimum).And(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that recording
					              has recorded the CustomEvent event on sut not between {minimum} and {maximum} times,
					              but it was {(count == 0 ? "never recorded" : $"recorded {count} times")} in *
					              """).AsWildcard();
			}

			[Theory]
			[InlineData(1, 1, false)]
			[InlineData(2, 1, true)]
			[InlineData(1, 2, true)]
			[InlineData(2, 2, false)]
			[InlineData(8, 2, true)]
			public async Task ShouldSupportExactly(int expected, int count, bool expectSuccess)
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				sut.NotifyCustomEvents(count);

				async Task Act() =>
					await That(recording).DidNotTrigger(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.Exactly(expected.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that recording
					              has recorded the CustomEvent event on sut not exactly {expected.ToTimesString()},
					              but it was recorded {count.ToTimesString()} in *
					              """).AsWildcard();
			}

			[Theory]
			[InlineData(2, 1, false)]
			[InlineData(2, 2, true)]
			[InlineData(2, 8, true)]
			[InlineData(8, 2, false)]
			public async Task ShouldSupportLessThan(int maximum, int count, bool expectSuccess)
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				sut.NotifyCustomEvents(count);

				async Task Act() =>
					await That(recording).DidNotTrigger(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.LessThan(maximum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that recording
					              has recorded the CustomEvent event on sut at least {maximum.ToTimesString()},
					              but it was recorded {count.ToTimesString()} in *
					              """).AsWildcard();
			}

			[Theory]
			[InlineData(1, 1, true)]
			[InlineData(1, 2, false)]
			[InlineData(2, 2, true)]
			[InlineData(2, 8, false)]
			[InlineData(8, 2, true)]
			public async Task ShouldSupportMoreThan(int minimum, int count, bool expectSuccess)
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				sut.NotifyCustomEvents(count);

				async Task Act() =>
					await That(recording).DidNotTrigger(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.MoreThan(minimum.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that recording
					              has recorded the CustomEvent event on sut at most {minimum.ToTimesString()},
					              but it was recorded {count.ToTimesString()} in *
					              """).AsWildcard();
			}

			[Theory]
			[InlineData(0, false)]
			[InlineData(1, true)]
			public async Task ShouldSupportNever(int count, bool expectSuccess)
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				sut.NotifyCustomEvents(count);

				async Task Act() =>
					await That(recording).DidNotTrigger(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.Never();

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut at least once,
					             but it was never recorded in []
					             """);
			}

			[Theory]
			[InlineData(0, true)]
			[InlineData(1, false)]
			[InlineData(2, true)]
			public async Task ShouldSupportOnce(int count, bool expectSuccess)
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				sut.NotifyCustomEvents(count);

				async Task Act() =>
					await That(recording).DidNotTrigger(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.Once();

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut not exactly once,
					             but it was recorded once in [
					               CustomEvent()
					             ]
					             """);
			}

			[Theory]
			[InlineData(1, true)]
			[InlineData(2, false)]
			[InlineData(3, true)]
			public async Task ShouldSupportTwice(int count, bool expectSuccess)
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				sut.NotifyCustomEvents(count);

				async Task Act() =>
					await That(recording).DidNotTrigger(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.Twice();

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut not exactly twice,
					             but it was recorded twice in [
					               CustomEvent(),
					               CustomEvent()
					             ]
					             """);
			}

			[Theory]
			[InlineData(1, true)]
			[InlineData(2, false)]
			public async Task WithEventArgs_ShouldOnlyCountMatchingEvents(int matchingCount, bool expectSuccess)
			{
				PropertyChangedClass sut = new()
				{
					MyValue = 2,
				};
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				for (int i = 0; i < matchingCount; i++)
				{
					sut.NotifyPropertyChanged(nameof(PropertyChangedClass.MyValue));
				}

				sut.NotifyPropertyChanged("SomethingElse");
				sut.NotifyPropertyChanged("SomethingElse");

				async Task Act() =>
					await That(recording).DidNotTrigger(nameof(INotifyPropertyChanged.PropertyChanged))
						.With<PropertyChangedEventArgs>(e => e.PropertyName == "MyValue")
						.AtLeast(2.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that recording
					             has recorded the PropertyChanged event on sut with PropertyChangedEventArgs e => e.PropertyName == "MyValue" fewer than twice,
					             but it was recorded twice in *
					             """).AsWildcard();
			}

			[Theory]
			[InlineData(1, true)]
			[InlineData(2, false)]
			public async Task WithParameter_ShouldOnlyCountMatchingEvents(int matchingCount, bool expectSuccess)
			{
				CustomEventWithParametersClass<string> sut = new();
				IEventRecording<CustomEventWithParametersClass<string>> recording = sut.Record().Events();

				for (int i = 0; i < matchingCount; i++)
				{
					sut.NotifyCustomEvent("foo");
				}

				sut.NotifyCustomEvent("bar");
				sut.NotifyCustomEvent("bar");

				async Task Act() =>
					await That(recording).DidNotTrigger(nameof(CustomEventWithParametersClass<string>.CustomEvent))
						.WithParameter<string>(s => s == "foo")
						.Twice();

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut with string parameter s => s == "foo" not exactly twice,
					             but it was recorded twice in [
					               CustomEvent("foo"),
					               CustomEvent("foo"),
					               CustomEvent("bar"),
					               CustomEvent("bar")
					             ]
					             """);
			}

			[Theory]
			[InlineData(1, false)]
			[InlineData(2, true)]
			public async Task WithSender_ShouldOnlyCountMatchingEvents(int matchingCount, bool expectSuccess)
			{
				PropertyChangedClass sender = new()
				{
					MyValue = 1,
				};
				PropertyChangedClass sut = new()
				{
					MyValue = 2,
				};
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				for (int i = 0; i < matchingCount; i++)
				{
					sut.NotifyPropertyChanged(sender, nameof(PropertyChangedClass.MyValue));
				}

				sut.NotifyPropertyChanged(sut, nameof(PropertyChangedClass.MyValue));

				async Task Act() =>
					await That(recording).DidNotTrigger(nameof(INotifyPropertyChanged.PropertyChanged))
						.WithSender(s => s == sender)
						.AtMost(1.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that recording
					             has recorded the PropertyChanged event on sut with sender s => s == sender more than once,
					             but it was recorded once in *
					             """).AsWildcard();
			}

			[Fact]
			public async Task Within_WhenCountIsReachedWithinTimeout_ShouldFail()
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				_ = Task.Delay(20.Milliseconds())
					.ContinueWith(_ => sut.NotifyCustomEvents(2));

				async Task Act() =>
					await That(recording).DidNotTrigger(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.AtLeast(2.Times())
						.Within(5.Seconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut fewer than twice within 0:05,
					             but it was recorded twice in [
					               CustomEvent(),
					               CustomEvent()
					             ] after 0:*
					             """).AsWildcard();
			}

			[Fact]
			public async Task Within_WhenCountIsNotReachedWithinTimeout_ShouldSucceed()
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				sut.NotifyCustomEvent();

				async Task Act() =>
					await That(recording).DidNotTrigger(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.AtLeast(2.Times())
						.Within(10.Milliseconds());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Within_WhenExactCountIsReachedWithinTimeout_ShouldFail()
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				sut.NotifyCustomEvent();

				async Task Act() =>
					await That(recording).DidNotTrigger(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.Once()
						.Within(10.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut not exactly once within 0:00.010,
					             but it was recorded once in [
					               CustomEvent()
					             ] within 0:*
					             """).AsWildcard();
			}
		}

		public sealed class NegatedCountTests
		{
			[Theory]
			[InlineData(1, false)]
			[InlineData(2, true)]
			public async Task ShouldSupportAtLeast(int count, bool expectSuccess)
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				sut.NotifyCustomEvents(count);

				async Task Act() =>
					await That(recording).DoesNotComplyWith(r => r
						.DidNotTrigger(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.AtLeast(2.Times()));

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut at least twice,
					             but it was recorded once in [
					               CustomEvent()
					             ]
					             """);
			}
		}
	}
}
