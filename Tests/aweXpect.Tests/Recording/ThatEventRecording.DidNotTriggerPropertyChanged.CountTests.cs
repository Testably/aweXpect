using aweXpect.Core;
using aweXpect.Recording;

namespace aweXpect.Tests;

public sealed partial class ThatEventRecording
{
	public sealed partial class DidNotTriggerPropertyChanged
	{
		public sealed class CountTests
		{
			[Theory]
			[InlineData(1, true)]
			[InlineData(2, false)]
			public async Task ShouldSupportAtLeast(int count, bool expectSuccess)
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				NotifyMyValueChanged(sut, count);

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChanged()
						.AtLeast(2.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that recording
					             has recorded the PropertyChanged event on sut fewer than twice,
					             but it was recorded twice in *
					             """).AsWildcard();
			}

			[Theory]
			[InlineData(1, false)]
			[InlineData(2, true)]
			public async Task ShouldSupportAtMost(int count, bool expectSuccess)
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				NotifyMyValueChanged(sut, count);

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChanged()
						.AtMost(1.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that recording
					             has recorded the PropertyChanged event on sut more than once,
					             but it was recorded once in *
					             """).AsWildcard();
			}

			[Theory]
			[InlineData(1, true)]
			[InlineData(2, false)]
			[InlineData(4, true)]
			public async Task ShouldSupportBetween(int count, bool expectSuccess)
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				NotifyMyValueChanged(sut, count);

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChanged()
						.Between(2).And(3.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that recording
					             has recorded the PropertyChanged event on sut not between 2 and 3 times,
					             but it was recorded twice in *
					             """).AsWildcard();
			}

			[Theory]
			[InlineData(2, true)]
			[InlineData(3, false)]
			[InlineData(4, true)]
			public async Task ShouldSupportExactly(int count, bool expectSuccess)
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				NotifyMyValueChanged(sut, count);

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChanged()
						.Exactly(3.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that recording
					             has recorded the PropertyChanged event on sut not exactly 3 times,
					             but it was recorded 3 times in *
					             """).AsWildcard();
			}

			[Theory]
			[InlineData(1, false)]
			[InlineData(2, true)]
			public async Task ShouldSupportLessThan(int count, bool expectSuccess)
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				NotifyMyValueChanged(sut, count);

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChanged()
						.LessThan(2.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that recording
					             has recorded the PropertyChanged event on sut at least twice,
					             but it was recorded once in *
					             """).AsWildcard();
			}

			[Theory]
			[InlineData(1, true)]
			[InlineData(2, false)]
			public async Task ShouldSupportMoreThan(int count, bool expectSuccess)
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				NotifyMyValueChanged(sut, count);

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChanged()
						.MoreThan(1.Times());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that recording
					             has recorded the PropertyChanged event on sut at most once,
					             but it was recorded twice in *
					             """).AsWildcard();
			}

			[Theory]
			[InlineData(0, false)]
			[InlineData(1, true)]
			public async Task ShouldSupportNever(int count, bool expectSuccess)
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				NotifyMyValueChanged(sut, count);

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChanged()
						.Never();

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that recording
					             has recorded the PropertyChanged event on sut at least once,
					             but it was never recorded in []
					             """);
			}

			[Theory]
			[InlineData(0, true)]
			[InlineData(1, false)]
			[InlineData(2, true)]
			public async Task ShouldSupportOnce(int count, bool expectSuccess)
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				NotifyMyValueChanged(sut, count);

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChanged()
						.Once();

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that recording
					             has recorded the PropertyChanged event on sut not exactly once,
					             but it was recorded once in *
					             """).AsWildcard();
			}

			[Theory]
			[InlineData(1, true)]
			[InlineData(2, false)]
			[InlineData(3, true)]
			public async Task ShouldSupportTwice(int count, bool expectSuccess)
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				NotifyMyValueChanged(sut, count);

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChanged()
						.Twice();

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that recording
					             has recorded the PropertyChanged event on sut not exactly twice,
					             but it was recorded twice in *
					             """).AsWildcard();
			}

			[Fact]
			public async Task Within_WhenCountIsReachedWithinTimeout_ShouldFail()
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				_ = Task.Delay(20.Milliseconds())
					.ContinueWith(_ => NotifyMyValueChanged(sut, 2));

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChanged()
						.AtLeast(2.Times())
						.Within(5.Seconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the PropertyChanged event on sut fewer than twice within 0:05,
					             but it was recorded twice in * after 0:*
					             """).AsWildcard();
			}

			[Fact]
			public async Task Within_WhenCountIsNotReachedWithinTimeout_ShouldSucceed()
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				NotifyMyValueChanged(sut, 1);

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChanged()
						.AtLeast(2.Times())
						.Within(10.Milliseconds());

				await That(Act).DoesNotThrow();
			}

			private static void NotifyMyValueChanged(PropertyChangedClass sut, int count)
			{
				for (int i = 0; i < count; i++)
				{
					sut.NotifyPropertyChanged(nameof(PropertyChangedClass.MyValue));
				}
			}
		}
	}
}
