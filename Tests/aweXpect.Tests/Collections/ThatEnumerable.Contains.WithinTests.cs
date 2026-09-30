using System.Collections.Generic;
using aweXpect.Customization;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class Contains
	{
		public sealed class Within
		{
#if NET8_0_OR_GREATER
			public sealed class DateOnlyTests
			{
				[Fact]
				public async Task Collection_WhenAnItemLiesOutsideTheTolerance_ShouldFail()
				{
					DateOnly[] values = [new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IEnumerable<DateOnly> subject = values;
					IEnumerable<DateOnly> expected = [new DateOnly(2024, 1, 13), new DateOnly(2024, 1, 21),];

					async Task Act()
						=> await That(subject).Contains(expected).Within(1.Days());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains collection expected ± 1 day in order and contiguous,
						              but it lacked all 2 expected items

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(expected, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task Collection_WhenEachItemLiesWithinTheTolerance_ShouldSucceed()
				{
					DateOnly[] values = [new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IEnumerable<DateOnly> subject = values;
					IEnumerable<DateOnly> expected = [new DateOnly(2024, 1, 12), new DateOnly(2024, 1, 21),];

					async Task Act()
						=> await That(subject).Contains(expected).Within(1.Days());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Collection_WhenToleranceIsNotAWholeNumberOfDays_ShouldThrowArgumentOutOfRangeException()
				{
					DateOnly[] values = [new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IEnumerable<DateOnly> subject = values;
					IEnumerable<DateOnly> expected = [new DateOnly(2024, 1, 12), new DateOnly(2024, 1, 21),];

					object Act()
						=> That(subject).Contains(expected).Within(1.Days() + 1.Hours());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("tolerance").And
						.WithMessage("The tolerance must be a whole number of days.").AsPrefix()
						.Because("a date has no time of day, so the remainder is rejected as soon as it is specified");
				}

				[Fact]
				public async Task Item_WhenItLiesAtTheEdgeOfTheTolerance_ShouldSucceed()
				{
					DateOnly[] values = [new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IEnumerable<DateOnly> subject = values;

					async Task Act()
						=> await That(subject).Contains(new DateOnly(2024, 1, 12)).Within(1.Days());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Item_WhenItLiesOutsideTheTolerance_ShouldFail()
				{
					DateOnly[] values = [new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IEnumerable<DateOnly> subject = values;
					DateOnly expected = new DateOnly(2024, 1, 13);

					async Task Act()
						=> await That(subject).Contains(expected).Within(1.Days());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains an item equal to {Formatter.Format(expected)} ± 1 day at least once,
						              but it did not contain it

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task Item_WhenTheDefaultToleranceIsSet_ShouldApplyIt()
				{
					DateOnly[] values = [new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IEnumerable<DateOnly> subject = values;

					async Task Act()
					{
						using IDisposable __ =
							Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Days());
						await That(subject).Contains(new DateOnly(2024, 1, 12));
					}

					await That(Act).DoesNotThrow()
						.Because("the items fall back to the default tolerance, as a single value does");
				}

				[Fact]
				public async Task Item_WhenTheDefaultToleranceIsSet_ShouldMentionIt()
				{
					DateOnly[] values = [new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IEnumerable<DateOnly> subject = values;
					DateOnly expected = new DateOnly(2024, 1, 13);

					async Task Act()
					{
						using IDisposable __ =
							Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Days());
						await That(subject).Contains(expected);
					}

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains an item equal to {Formatter.Format(expected)} ± 1 day at least once,
						              but it did not contain it

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """)
						.Because("the applied default tolerance is part of the expectation");
				}

				[Fact]
				public async Task Item_WhenTheDefaultToleranceIsShorterThanADay_ShouldNeitherApplyNorMentionIt()
				{
					DateOnly[] values = [new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IEnumerable<DateOnly> subject = values;
					DateOnly expected = new DateOnly(2024, 1, 12);

					async Task Act()
					{
						using IDisposable __ =
							Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(23.Hours());
						await That(subject).Contains(expected);
					}

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains an item equal to {Formatter.Format(expected)} at least once,
						              but it did not contain it

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """)
						.Because("a date only honours the whole days of the default tolerance");
				}

				[Fact]
				public async Task Item_WhenToleranceIsNotAWholeNumberOfDays_ShouldThrowArgumentOutOfRangeException()
				{
					DateOnly[] values = [new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IEnumerable<DateOnly> subject = values;

					object Act()
						=> That(subject).Contains(new DateOnly(2024, 1, 12)).Within(1.Days() + 1.Hours());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("tolerance").And
						.WithMessage("The tolerance must be a whole number of days.").AsPrefix()
						.Because("a date has no time of day, so the remainder is rejected as soon as it is specified");
				}
			}
#endif

#if NET8_0_OR_GREATER
			public sealed class NullableDateOnlyTests
			{
				[Fact]
				public async Task Collection_WhenAnItemLiesOutsideTheTolerance_ShouldFail()
				{
					DateOnly?[] values = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IEnumerable<DateOnly?> subject = values;
					IEnumerable<DateOnly?> expected = [new DateOnly(2024, 1, 13), new DateOnly(2024, 1, 21),];

					async Task Act()
						=> await That(subject).Contains(expected).Within(1.Days());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains collection expected ± 1 day in order and contiguous,
						              but it lacked all 2 expected items

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(expected, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task Collection_WhenEachItemLiesWithinTheTolerance_ShouldSucceed()
				{
					DateOnly?[] values = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IEnumerable<DateOnly?> subject = values;
					IEnumerable<DateOnly?> expected = [new DateOnly(2024, 1, 12), new DateOnly(2024, 1, 21),];

					async Task Act()
						=> await That(subject).Contains(expected).Within(1.Days());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Collection_WhenToleranceIsNotAWholeNumberOfDays_ShouldThrowArgumentOutOfRangeException()
				{
					DateOnly?[] values = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IEnumerable<DateOnly?> subject = values;
					IEnumerable<DateOnly?> expected = [new DateOnly(2024, 1, 12), new DateOnly(2024, 1, 21),];

					object Act()
						=> That(subject).Contains(expected).Within(1.Days() + 1.Hours());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("tolerance").And
						.WithMessage("The tolerance must be a whole number of days.").AsPrefix()
						.Because("a date has no time of day, so the remainder is rejected as soon as it is specified");
				}

				[Fact]
				public async Task Item_WhenItLiesAtTheEdgeOfTheTolerance_ShouldSucceed()
				{
					DateOnly?[] values = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IEnumerable<DateOnly?> subject = values;

					async Task Act()
						=> await That(subject).Contains(new DateOnly(2024, 1, 12)).Within(1.Days());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Item_WhenItLiesOutsideTheTolerance_ShouldFail()
				{
					DateOnly?[] values = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IEnumerable<DateOnly?> subject = values;
					DateOnly? expected = new DateOnly(2024, 1, 13);

					async Task Act()
						=> await That(subject).Contains(expected).Within(1.Days());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains an item equal to {Formatter.Format(expected)} ± 1 day at least once,
						              but it did not contain it

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task Item_WhenToleranceIsNotAWholeNumberOfDays_ShouldThrowArgumentOutOfRangeException()
				{
					DateOnly?[] values = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IEnumerable<DateOnly?> subject = values;

					object Act()
						=> That(subject).Contains(new DateOnly(2024, 1, 12)).Within(1.Days() + 1.Hours());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("tolerance").And
						.WithMessage("The tolerance must be a whole number of days.").AsPrefix()
						.Because("a date has no time of day, so the remainder is rejected as soon as it is specified");
				}
			}
#endif

			public sealed class DateTimeTests
			{
				[Fact]
				public async Task Collection_WhenAnItemLiesOutsideTheTolerance_ShouldFail()
				{
					DateTime[] values = [new DateTime(2024, 1, 1, 13, 0, 0), new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					IEnumerable<DateTime> subject = values;
					IEnumerable<DateTime> expected = [new DateTime(2024, 1, 1, 14, 2, 0), new DateTime(2024, 1, 1, 15, 0, 0),];

					async Task Act()
						=> await That(subject).Contains(expected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains collection expected ± 1:00 in order and contiguous,
						              but it lacked all 2 expected items

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(expected, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task Collection_WhenEachItemLiesWithinTheTolerance_ShouldSucceed()
				{
					DateTime[] values = [new DateTime(2024, 1, 1, 13, 0, 0), new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					IEnumerable<DateTime> subject = values;
					IEnumerable<DateTime> expected = [new DateTime(2024, 1, 1, 14, 1, 0), new DateTime(2024, 1, 1, 15, 0, 0),];

					async Task Act()
						=> await That(subject).Contains(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Collection_WhenTheDefaultToleranceIsSet_ShouldApplyIt()
				{
					DateTime[] values = [new DateTime(2024, 1, 1, 13, 0, 0), new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					IEnumerable<DateTime> subject = values;
					IEnumerable<DateTime> expected = [new DateTime(2024, 1, 1, 14, 1, 0), new DateTime(2024, 1, 1, 15, 0, 0),];

					async Task Act()
					{
						using IDisposable __ =
							Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Minutes());
						await That(subject).Contains(expected);
					}

					await That(Act).DoesNotThrow()
						.Because("the items fall back to the default tolerance, as a single value does");
				}

				[Fact]
				public async Task Collection_WhenTheDefaultToleranceIsSet_ShouldMentionIt()
				{
					DateTime[] values = [new DateTime(2024, 1, 1, 13, 0, 0), new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					IEnumerable<DateTime> subject = values;
					IEnumerable<DateTime> expected = [new DateTime(2024, 1, 1, 14, 2, 0), new DateTime(2024, 1, 1, 15, 0, 0),];

					async Task Act()
					{
						using IDisposable __ =
							Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Minutes());
						await That(subject).Contains(expected);
					}

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains collection expected ± 1:00 in order and contiguous,
						              but it lacked all 2 expected items

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(expected, FormattingOptions.MultipleLines)}
						              """)
						.Because("the applied default tolerance is part of the expectation");
				}

				[Fact]
				public async Task Item_WhenItLiesAtTheEdgeOfTheTolerance_ShouldSucceed()
				{
					DateTime[] values = [new DateTime(2024, 1, 1, 13, 0, 0), new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					IEnumerable<DateTime> subject = values;

					async Task Act()
						=> await That(subject).Contains(new DateTime(2024, 1, 1, 14, 1, 0)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Item_WhenItLiesOutsideTheTolerance_ShouldFail()
				{
					DateTime[] values = [new DateTime(2024, 1, 1, 13, 0, 0), new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					IEnumerable<DateTime> subject = values;
					DateTime expected = new DateTime(2024, 1, 1, 14, 2, 0);

					async Task Act()
						=> await That(subject).Contains(expected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains an item equal to {Formatter.Format(expected)} ± 1:00 at least once,
						              but it did not contain it

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task Item_WhenTheDefaultToleranceIsSet_ShouldApplyIt()
				{
					DateTime[] values = [new DateTime(2024, 1, 1, 13, 0, 0), new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					IEnumerable<DateTime> subject = values;

					async Task Act()
					{
						using IDisposable __ =
							Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Minutes());
						await That(subject).Contains(new DateTime(2024, 1, 1, 14, 1, 0));
					}

					await That(Act).DoesNotThrow()
						.Because("the items fall back to the default tolerance, as a single value does");
				}

				[Fact]
				public async Task Item_WhenTheDefaultToleranceIsSet_ShouldMentionIt()
				{
					DateTime[] values = [new DateTime(2024, 1, 1, 13, 0, 0), new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					IEnumerable<DateTime> subject = values;
					DateTime expected = new DateTime(2024, 1, 1, 14, 2, 0);

					async Task Act()
					{
						using IDisposable __ =
							Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Minutes());
						await That(subject).Contains(expected);
					}

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains an item equal to {Formatter.Format(expected)} ± 1:00 at least once,
						              but it did not contain it

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """)
						.Because("the applied default tolerance is part of the expectation");
				}
			}

			public sealed class NullableDateTimeTests
			{
				[Fact]
				public async Task Collection_WhenAnItemLiesOutsideTheTolerance_ShouldFail()
				{
					DateTime?[] values = [new DateTime(2024, 1, 1, 13, 0, 0), null, new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					IEnumerable<DateTime?> subject = values;
					IEnumerable<DateTime?> expected = [new DateTime(2024, 1, 1, 14, 2, 0), new DateTime(2024, 1, 1, 15, 0, 0),];

					async Task Act()
						=> await That(subject).Contains(expected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains collection expected ± 1:00 in order and contiguous,
						              but it lacked all 2 expected items

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(expected, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task Collection_WhenEachItemLiesWithinTheTolerance_ShouldSucceed()
				{
					DateTime?[] values = [new DateTime(2024, 1, 1, 13, 0, 0), null, new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					IEnumerable<DateTime?> subject = values;
					IEnumerable<DateTime?> expected = [new DateTime(2024, 1, 1, 14, 1, 0), new DateTime(2024, 1, 1, 15, 0, 0),];

					async Task Act()
						=> await That(subject).Contains(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Item_WhenItLiesAtTheEdgeOfTheTolerance_ShouldSucceed()
				{
					DateTime?[] values = [new DateTime(2024, 1, 1, 13, 0, 0), null, new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					IEnumerable<DateTime?> subject = values;

					async Task Act()
						=> await That(subject).Contains(new DateTime(2024, 1, 1, 14, 1, 0)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Item_WhenItLiesOutsideTheTolerance_ShouldFail()
				{
					DateTime?[] values = [new DateTime(2024, 1, 1, 13, 0, 0), null, new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					IEnumerable<DateTime?> subject = values;
					DateTime? expected = new DateTime(2024, 1, 1, 14, 2, 0);

					async Task Act()
						=> await That(subject).Contains(expected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains an item equal to {Formatter.Format(expected)} ± 1:00 at least once,
						              but it did not contain it

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}
			}

			public sealed class DateTimeOffsetTests
			{
				[Fact]
				public async Task Collection_WhenAnItemLiesOutsideTheTolerance_ShouldFail()
				{
					DateTimeOffset[] values = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					IEnumerable<DateTimeOffset> subject = values;
					IEnumerable<DateTimeOffset> expected = [new DateTimeOffset(2024, 1, 1, 14, 2, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];

					async Task Act()
						=> await That(subject).Contains(expected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains collection expected ± 1:00 in order and contiguous,
						              but it lacked all 2 expected items

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(expected, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task Collection_WhenEachItemLiesWithinTheTolerance_ShouldSucceed()
				{
					DateTimeOffset[] values = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					IEnumerable<DateTimeOffset> subject = values;
					IEnumerable<DateTimeOffset> expected = [new DateTimeOffset(2024, 1, 1, 14, 1, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];

					async Task Act()
						=> await That(subject).Contains(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Item_WhenItLiesAtTheEdgeOfTheTolerance_ShouldSucceed()
				{
					DateTimeOffset[] values = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					IEnumerable<DateTimeOffset> subject = values;

					async Task Act()
						=> await That(subject).Contains(new DateTimeOffset(2024, 1, 1, 14, 1, 0, TimeSpan.Zero)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Item_WhenItLiesOutsideTheTolerance_ShouldFail()
				{
					DateTimeOffset[] values = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					IEnumerable<DateTimeOffset> subject = values;
					DateTimeOffset expected = new DateTimeOffset(2024, 1, 1, 14, 2, 0, TimeSpan.Zero);

					async Task Act()
						=> await That(subject).Contains(expected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains an item equal to {Formatter.Format(expected)} ± 1:00 at least once,
						              but it did not contain it

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task Item_WhenTheDefaultToleranceIsSet_ShouldApplyIt()
				{
					DateTimeOffset[] values = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					IEnumerable<DateTimeOffset> subject = values;

					async Task Act()
					{
						using IDisposable __ =
							Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Minutes());
						await That(subject).Contains(new DateTimeOffset(2024, 1, 1, 14, 1, 0, TimeSpan.Zero));
					}

					await That(Act).DoesNotThrow()
						.Because("the items fall back to the default tolerance, as a single value does");
				}

				[Fact]
				public async Task Item_WhenTheDefaultToleranceIsSet_ShouldMentionIt()
				{
					DateTimeOffset[] values = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					IEnumerable<DateTimeOffset> subject = values;
					DateTimeOffset expected = new DateTimeOffset(2024, 1, 1, 14, 2, 0, TimeSpan.Zero);

					async Task Act()
					{
						using IDisposable __ =
							Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Minutes());
						await That(subject).Contains(expected);
					}

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains an item equal to {Formatter.Format(expected)} ± 1:00 at least once,
						              but it did not contain it

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """)
						.Because("the applied default tolerance is part of the expectation");
				}
			}

			public sealed class NullableDateTimeOffsetTests
			{
				[Fact]
				public async Task Collection_WhenAnItemLiesOutsideTheTolerance_ShouldFail()
				{
					DateTimeOffset?[] values = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), null, new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					IEnumerable<DateTimeOffset?> subject = values;
					IEnumerable<DateTimeOffset?> expected = [new DateTimeOffset(2024, 1, 1, 14, 2, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];

					async Task Act()
						=> await That(subject).Contains(expected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains collection expected ± 1:00 in order and contiguous,
						              but it lacked all 2 expected items

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(expected, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task Collection_WhenEachItemLiesWithinTheTolerance_ShouldSucceed()
				{
					DateTimeOffset?[] values = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), null, new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					IEnumerable<DateTimeOffset?> subject = values;
					IEnumerable<DateTimeOffset?> expected = [new DateTimeOffset(2024, 1, 1, 14, 1, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];

					async Task Act()
						=> await That(subject).Contains(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Item_WhenItLiesAtTheEdgeOfTheTolerance_ShouldSucceed()
				{
					DateTimeOffset?[] values = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), null, new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					IEnumerable<DateTimeOffset?> subject = values;

					async Task Act()
						=> await That(subject).Contains(new DateTimeOffset(2024, 1, 1, 14, 1, 0, TimeSpan.Zero)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Item_WhenItLiesOutsideTheTolerance_ShouldFail()
				{
					DateTimeOffset?[] values = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), null, new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					IEnumerable<DateTimeOffset?> subject = values;
					DateTimeOffset? expected = new DateTimeOffset(2024, 1, 1, 14, 2, 0, TimeSpan.Zero);

					async Task Act()
						=> await That(subject).Contains(expected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains an item equal to {Formatter.Format(expected)} ± 1:00 at least once,
						              but it did not contain it

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}
			}

			public sealed class DecimalTests
			{
				[Fact]
				public async Task Collection_WhenAnItemLiesOutsideTheTolerance_ShouldFail()
				{
					decimal[] values = [1.0m, 2.0m, 3.0m,];
					IEnumerable<decimal> subject = values;
					IEnumerable<decimal> expected = [2.5m, 3.0m,];

					async Task Act()
						=> await That(subject).Contains(expected).Within(0.25m);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains collection expected ± 0.25 in order and contiguous,
						              but it lacked all 2 expected items

						              Collection:
						              {Formatter.Format(values)}

						              Expected:
						              {Formatter.Format(expected)}
						              """);
				}

				[Fact]
				public async Task Collection_WhenEachItemLiesWithinTheTolerance_ShouldSucceed()
				{
					decimal[] values = [1.0m, 2.0m, 3.0m,];
					IEnumerable<decimal> subject = values;
					IEnumerable<decimal> expected = [2.25m, 3.0m,];

					async Task Act()
						=> await That(subject).Contains(expected).Within(0.25m);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Item_WhenItLiesAtTheEdgeOfTheTolerance_ShouldSucceed()
				{
					decimal[] values = [1.0m, 2.0m, 3.0m,];
					IEnumerable<decimal> subject = values;

					async Task Act()
						=> await That(subject).Contains(2.25m).Within(0.25m);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Item_WhenItLiesOutsideTheTolerance_ShouldFail()
				{
					decimal[] values = [1.0m, 2.0m, 3.0m,];
					IEnumerable<decimal> subject = values;
					decimal expected = 2.5m;

					async Task Act()
						=> await That(subject).Contains(expected).Within(0.25m);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains an item equal to {Formatter.Format(expected)} ± 0.25 at least once,
						              but it did not contain it

						              Collection:
						              {Formatter.Format(values)}
						              """);
				}
			}

			public sealed class NullableDecimalTests
			{
				[Fact]
				public async Task Collection_WhenAnItemLiesOutsideTheTolerance_ShouldFail()
				{
					decimal?[] values = [1.0m, null, 2.0m, 3.0m,];
					IEnumerable<decimal?> subject = values;
					IEnumerable<decimal?> expected = [2.5m, 3.0m,];

					async Task Act()
						=> await That(subject).Contains(expected).Within(0.25m);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains collection expected ± 0.25 in order and contiguous,
						              but it lacked all 2 expected items

						              Collection:
						              {Formatter.Format(values)}

						              Expected:
						              {Formatter.Format(expected)}
						              """);
				}

				[Fact]
				public async Task Collection_WhenEachItemLiesWithinTheTolerance_ShouldSucceed()
				{
					decimal?[] values = [1.0m, null, 2.0m, 3.0m,];
					IEnumerable<decimal?> subject = values;
					IEnumerable<decimal?> expected = [2.25m, 3.0m,];

					async Task Act()
						=> await That(subject).Contains(expected).Within(0.25m);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Item_WhenItLiesAtTheEdgeOfTheTolerance_ShouldSucceed()
				{
					decimal?[] values = [1.0m, null, 2.0m, 3.0m,];
					IEnumerable<decimal?> subject = values;

					async Task Act()
						=> await That(subject).Contains(2.25m).Within(0.25m);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Item_WhenItLiesOutsideTheTolerance_ShouldFail()
				{
					decimal?[] values = [1.0m, null, 2.0m, 3.0m,];
					IEnumerable<decimal?> subject = values;
					decimal? expected = 2.5m;

					async Task Act()
						=> await That(subject).Contains(expected).Within(0.25m);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains an item equal to {Formatter.Format(expected)} ± 0.25 at least once,
						              but it did not contain it

						              Collection:
						              {Formatter.Format(values)}
						              """);
				}
			}

			public sealed class DoubleTests
			{
				[Fact]
				public async Task Collection_WhenAnItemLiesOutsideTheTolerance_ShouldFail()
				{
					double[] values = [1.0, 2.0, 3.0,];
					IEnumerable<double> subject = values;
					IEnumerable<double> expected = [2.5, 3.0,];

					async Task Act()
						=> await That(subject).Contains(expected).Within(0.25);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains collection expected ± 0.25 in order and contiguous,
						              but it lacked all 2 expected items

						              Collection:
						              {Formatter.Format(values)}

						              Expected:
						              {Formatter.Format(expected)}
						              """);
				}

				[Theory]
				[InlineData(false)]
				[InlineData(true)]
				public async Task Collection_WhenCombinedWithUsing_ShouldThrowInvalidOperationException(bool negated)
				{
					double[] values = [1.0, 2.0, 3.0,];
					IEnumerable<double> subject = values;
					IEnumerable<double> expected = [2.25, 3.0,];

					async Task Act()
					{
						if (negated)
						{
							await That(subject).DoesNotContain(expected).Within(0.25).Using(new AllEqualComparer());
						}
						else
						{
							await That(subject).Contains(expected).Within(0.25).Using(new AllEqualComparer());
						}
					}

					await That(Act).Throws<InvalidOperationException>()
						.WithMessage("Using cannot be combined with Within.")
						.Because("the comparer would silently replace the tolerance");
				}

				[Fact]
				public async Task Collection_WhenEachItemLiesWithinTheTolerance_ShouldSucceed()
				{
					double[] values = [1.0, 2.0, 3.0,];
					IEnumerable<double> subject = values;
					IEnumerable<double> expected = [2.25, 3.0,];

					async Task Act()
						=> await That(subject).Contains(expected).Within(0.25);

					await That(Act).DoesNotThrow();
				}

				[Theory]
				[InlineData(false)]
				[InlineData(true)]
				public async Task Item_WhenCombinedWithUsing_ShouldThrowInvalidOperationException(bool negated)
				{
					double[] values = [1.0, 2.0, 3.0,];
					IEnumerable<double> subject = values;

					async Task Act()
					{
						if (negated)
						{
							await That(subject).DoesNotContain(2.25).Within(0.25).Using(new AllEqualComparer());
						}
						else
						{
							await That(subject).Contains(2.25).Within(0.25).Using(new AllEqualComparer());
						}
					}

					await That(Act).Throws<InvalidOperationException>()
						.WithMessage("Using cannot be combined with Within.")
						.Because("the comparer would silently replace the tolerance");
				}

				[Fact]
				public async Task Item_WhenItLiesAtTheEdgeOfTheTolerance_ShouldSucceed()
				{
					double[] values = [1.0, 2.0, 3.0,];
					IEnumerable<double> subject = values;

					async Task Act()
						=> await That(subject).Contains(2.25).Within(0.25);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Item_WhenItLiesOutsideTheTolerance_ShouldFail()
				{
					double[] values = [1.0, 2.0, 3.0,];
					IEnumerable<double> subject = values;
					double expected = 2.5;

					async Task Act()
						=> await That(subject).Contains(expected).Within(0.25);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains an item equal to {Formatter.Format(expected)} ± 0.25 at least once,
						              but it did not contain it

						              Collection:
						              {Formatter.Format(values)}
						              """);
				}
			}

			public sealed class NullableDoubleTests
			{
				[Fact]
				public async Task Collection_WhenAnItemLiesOutsideTheTolerance_ShouldFail()
				{
					double?[] values = [1.0, null, 2.0, 3.0,];
					IEnumerable<double?> subject = values;
					IEnumerable<double?> expected = [2.5, 3.0,];

					async Task Act()
						=> await That(subject).Contains(expected).Within(0.25);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains collection expected ± 0.25 in order and contiguous,
						              but it lacked all 2 expected items

						              Collection:
						              {Formatter.Format(values)}

						              Expected:
						              {Formatter.Format(expected)}
						              """);
				}

				[Fact]
				public async Task Collection_WhenEachItemLiesWithinTheTolerance_ShouldSucceed()
				{
					double?[] values = [1.0, null, 2.0, 3.0,];
					IEnumerable<double?> subject = values;
					IEnumerable<double?> expected = [2.25, 3.0,];

					async Task Act()
						=> await That(subject).Contains(expected).Within(0.25);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Collection_WhenExpectedIsNull_ShouldThrowArgumentNullException()
				{
					IEnumerable<double?> subject = [1.0, null, 2.0,];
					IEnumerable<double>? expected = null;

					async Task Act()
						=> await That(subject).Contains(expected!).Within(0.25);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("expected").And
						.WithMessage("The 'expected' value cannot be null.").AsPrefix()
						.Because("a null collection of non-nullable items is rejected like any other null expected collection");
				}

				[Fact]
				public async Task Item_WhenItLiesAtTheEdgeOfTheTolerance_ShouldSucceed()
				{
					double?[] values = [1.0, null, 2.0, 3.0,];
					IEnumerable<double?> subject = values;

					async Task Act()
						=> await That(subject).Contains(2.25).Within(0.25);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Item_WhenItLiesOutsideTheTolerance_ShouldFail()
				{
					double?[] values = [1.0, null, 2.0, 3.0,];
					IEnumerable<double?> subject = values;
					double? expected = 2.5;

					async Task Act()
						=> await That(subject).Contains(expected).Within(0.25);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains an item equal to {Formatter.Format(expected)} ± 0.25 at least once,
						              but it did not contain it

						              Collection:
						              {Formatter.Format(values)}
						              """);
				}
			}

			public sealed class FloatTests
			{
				[Fact]
				public async Task Collection_WhenAnItemLiesOutsideTheTolerance_ShouldFail()
				{
					float[] values = [1.0F, 2.0F, 3.0F,];
					IEnumerable<float> subject = values;
					IEnumerable<float> expected = [2.5F, 3.0F,];

					async Task Act()
						=> await That(subject).Contains(expected).Within(0.25F);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains collection expected ± 0.25 in order and contiguous,
						              but it lacked all 2 expected items

						              Collection:
						              {Formatter.Format(values)}

						              Expected:
						              {Formatter.Format(expected)}
						              """);
				}

				[Fact]
				public async Task Collection_WhenEachItemLiesWithinTheTolerance_ShouldSucceed()
				{
					float[] values = [1.0F, 2.0F, 3.0F,];
					IEnumerable<float> subject = values;
					IEnumerable<float> expected = [2.25F, 3.0F,];

					async Task Act()
						=> await That(subject).Contains(expected).Within(0.25F);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Item_WhenItLiesAtTheEdgeOfTheTolerance_ShouldSucceed()
				{
					float[] values = [1.0F, 2.0F, 3.0F,];
					IEnumerable<float> subject = values;

					async Task Act()
						=> await That(subject).Contains(2.25F).Within(0.25F);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Item_WhenItLiesOutsideTheTolerance_ShouldFail()
				{
					float[] values = [1.0F, 2.0F, 3.0F,];
					IEnumerable<float> subject = values;
					float expected = 2.5F;

					async Task Act()
						=> await That(subject).Contains(expected).Within(0.25F);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains an item equal to {Formatter.Format(expected)} ± 0.25 at least once,
						              but it did not contain it

						              Collection:
						              {Formatter.Format(values)}
						              """);
				}
			}

			public sealed class NullableFloatTests
			{
				[Fact]
				public async Task Collection_WhenAnItemLiesOutsideTheTolerance_ShouldFail()
				{
					float?[] values = [1.0F, null, 2.0F, 3.0F,];
					IEnumerable<float?> subject = values;
					IEnumerable<float?> expected = [2.5F, 3.0F,];

					async Task Act()
						=> await That(subject).Contains(expected).Within(0.25F);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains collection expected ± 0.25 in order and contiguous,
						              but it lacked all 2 expected items

						              Collection:
						              {Formatter.Format(values)}

						              Expected:
						              {Formatter.Format(expected)}
						              """);
				}

				[Fact]
				public async Task Collection_WhenEachItemLiesWithinTheTolerance_ShouldSucceed()
				{
					float?[] values = [1.0F, null, 2.0F, 3.0F,];
					IEnumerable<float?> subject = values;
					IEnumerable<float?> expected = [2.25F, 3.0F,];

					async Task Act()
						=> await That(subject).Contains(expected).Within(0.25F);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Item_WhenItLiesAtTheEdgeOfTheTolerance_ShouldSucceed()
				{
					float?[] values = [1.0F, null, 2.0F, 3.0F,];
					IEnumerable<float?> subject = values;

					async Task Act()
						=> await That(subject).Contains(2.25F).Within(0.25F);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Item_WhenItLiesOutsideTheTolerance_ShouldFail()
				{
					float?[] values = [1.0F, null, 2.0F, 3.0F,];
					IEnumerable<float?> subject = values;
					float? expected = 2.5F;

					async Task Act()
						=> await That(subject).Contains(expected).Within(0.25F);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains an item equal to {Formatter.Format(expected)} ± 0.25 at least once,
						              but it did not contain it

						              Collection:
						              {Formatter.Format(values)}
						              """);
				}
			}

#if NET8_0_OR_GREATER
			public sealed class TimeOnlyTests
			{
				[Fact]
				public async Task Collection_WhenAnItemLiesAcrossMidnight_ShouldUseTheShorterDistance()
				{
					TimeOnly[] values = [new TimeOnly(22, 0), new TimeOnly(23, 59, 30), new TimeOnly(2, 0),];
					IEnumerable<TimeOnly> subject = values;
					IEnumerable<TimeOnly> expected = [new TimeOnly(0, 0, 30), new TimeOnly(2, 0),];

					async Task Act()
						=> await That(subject).Contains(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow()
						.Because("the times are compared on the clock face, where 23:59:30 and 00:00:30 are one minute apart");
				}

				[Fact]
				public async Task Collection_WhenAnItemLiesOutsideTheTolerance_ShouldFail()
				{
					TimeOnly[] values = [new TimeOnly(13, 0), new TimeOnly(14, 0), new TimeOnly(15, 0),];
					IEnumerable<TimeOnly> subject = values;
					IEnumerable<TimeOnly> expected = [new TimeOnly(14, 2), new TimeOnly(15, 0),];

					async Task Act()
						=> await That(subject).Contains(expected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains collection expected ± 1:00 in order and contiguous,
						              but it lacked all 2 expected items

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(expected, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task Collection_WhenEachItemLiesWithinTheTolerance_ShouldSucceed()
				{
					TimeOnly[] values = [new TimeOnly(13, 0), new TimeOnly(14, 0), new TimeOnly(15, 0),];
					IEnumerable<TimeOnly> subject = values;
					IEnumerable<TimeOnly> expected = [new TimeOnly(14, 1), new TimeOnly(15, 0),];

					async Task Act()
						=> await That(subject).Contains(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Item_WhenAnItemLiesAcrossMidnight_ShouldUseTheShorterDistance()
				{
					TimeOnly[] values = [new TimeOnly(22, 0), new TimeOnly(23, 59, 30), new TimeOnly(2, 0),];
					IEnumerable<TimeOnly> subject = values;

					async Task Act()
						=> await That(subject).Contains(new TimeOnly(0, 0, 30)).Within(1.Minutes());

					await That(Act).DoesNotThrow()
						.Because("the times are compared on the clock face, where 23:59:30 and 00:00:30 are one minute apart");
				}

				[Fact]
				public async Task Item_WhenItLiesAtTheEdgeOfTheTolerance_ShouldSucceed()
				{
					TimeOnly[] values = [new TimeOnly(13, 0), new TimeOnly(14, 0), new TimeOnly(15, 0),];
					IEnumerable<TimeOnly> subject = values;

					async Task Act()
						=> await That(subject).Contains(new TimeOnly(14, 1)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Item_WhenItLiesOutsideTheTolerance_ShouldFail()
				{
					TimeOnly[] values = [new TimeOnly(13, 0), new TimeOnly(14, 0), new TimeOnly(15, 0),];
					IEnumerable<TimeOnly> subject = values;
					TimeOnly expected = new TimeOnly(14, 2);

					async Task Act()
						=> await That(subject).Contains(expected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains an item equal to {Formatter.Format(expected)} ± 1:00 at least once,
						              but it did not contain it

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task Item_WhenTheDefaultToleranceIsSet_ShouldApplyIt()
				{
					TimeOnly[] values = [new TimeOnly(13, 0), new TimeOnly(14, 0), new TimeOnly(15, 0),];
					IEnumerable<TimeOnly> subject = values;

					async Task Act()
					{
						using IDisposable __ =
							Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Minutes());
						await That(subject).Contains(new TimeOnly(14, 1));
					}

					await That(Act).DoesNotThrow()
						.Because("the items fall back to the default tolerance, as a single value does");
				}

				[Fact]
				public async Task Item_WhenTheDefaultToleranceIsSet_ShouldMentionIt()
				{
					TimeOnly[] values = [new TimeOnly(13, 0), new TimeOnly(14, 0), new TimeOnly(15, 0),];
					IEnumerable<TimeOnly> subject = values;
					TimeOnly expected = new TimeOnly(14, 2);

					async Task Act()
					{
						using IDisposable __ =
							Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Minutes());
						await That(subject).Contains(expected);
					}

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains an item equal to {Formatter.Format(expected)} ± 1:00 at least once,
						              but it did not contain it

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """)
						.Because("the applied default tolerance is part of the expectation");
				}

				[Fact]
				public async Task Item_WhenTheToleranceIsTwelveHours_ShouldMatchTheOppositeTimeOnTheClockFace()
				{
					TimeOnly[] values = [new TimeOnly(13, 0), new TimeOnly(14, 0), new TimeOnly(15, 0),];
					IEnumerable<TimeOnly> subject = values;

					async Task Act()
						=> await That(subject).Contains(new TimeOnly(2, 0)).Within(12.Hours());

					await That(Act).DoesNotThrow()
						.Because("no two times are more than 12 hours apart on the clock face");
				}
			}
#endif

#if NET8_0_OR_GREATER
			public sealed class NullableTimeOnlyTests
			{
				[Fact]
				public async Task Collection_WhenAnItemLiesAcrossMidnight_ShouldUseTheShorterDistance()
				{
					TimeOnly?[] values = [new TimeOnly(22, 0), null, new TimeOnly(23, 59, 30), new TimeOnly(2, 0),];
					IEnumerable<TimeOnly?> subject = values;
					IEnumerable<TimeOnly?> expected = [new TimeOnly(0, 0, 30), new TimeOnly(2, 0),];

					async Task Act()
						=> await That(subject).Contains(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow()
						.Because("the times are compared on the clock face, where 23:59:30 and 00:00:30 are one minute apart");
				}

				[Fact]
				public async Task Collection_WhenAnItemLiesOutsideTheTolerance_ShouldFail()
				{
					TimeOnly?[] values = [new TimeOnly(13, 0), null, new TimeOnly(14, 0), new TimeOnly(15, 0),];
					IEnumerable<TimeOnly?> subject = values;
					IEnumerable<TimeOnly?> expected = [new TimeOnly(14, 2), new TimeOnly(15, 0),];

					async Task Act()
						=> await That(subject).Contains(expected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains collection expected ± 1:00 in order and contiguous,
						              but it lacked all 2 expected items

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(expected, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task Collection_WhenEachItemLiesWithinTheTolerance_ShouldSucceed()
				{
					TimeOnly?[] values = [new TimeOnly(13, 0), null, new TimeOnly(14, 0), new TimeOnly(15, 0),];
					IEnumerable<TimeOnly?> subject = values;
					IEnumerable<TimeOnly?> expected = [new TimeOnly(14, 1), new TimeOnly(15, 0),];

					async Task Act()
						=> await That(subject).Contains(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Item_WhenAnItemLiesAcrossMidnight_ShouldUseTheShorterDistance()
				{
					TimeOnly?[] values = [new TimeOnly(22, 0), null, new TimeOnly(23, 59, 30), new TimeOnly(2, 0),];
					IEnumerable<TimeOnly?> subject = values;

					async Task Act()
						=> await That(subject).Contains(new TimeOnly(0, 0, 30)).Within(1.Minutes());

					await That(Act).DoesNotThrow()
						.Because("the times are compared on the clock face, where 23:59:30 and 00:00:30 are one minute apart");
				}

				[Fact]
				public async Task Item_WhenItLiesAtTheEdgeOfTheTolerance_ShouldSucceed()
				{
					TimeOnly?[] values = [new TimeOnly(13, 0), null, new TimeOnly(14, 0), new TimeOnly(15, 0),];
					IEnumerable<TimeOnly?> subject = values;

					async Task Act()
						=> await That(subject).Contains(new TimeOnly(14, 1)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Item_WhenItLiesOutsideTheTolerance_ShouldFail()
				{
					TimeOnly?[] values = [new TimeOnly(13, 0), null, new TimeOnly(14, 0), new TimeOnly(15, 0),];
					IEnumerable<TimeOnly?> subject = values;
					TimeOnly? expected = new TimeOnly(14, 2);

					async Task Act()
						=> await That(subject).Contains(expected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains an item equal to {Formatter.Format(expected)} ± 1:00 at least once,
						              but it did not contain it

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}
			}
#endif

			public sealed class TimeSpanTests
			{
				[Fact]
				public async Task Collection_WhenAnItemLiesOutsideTheTolerance_ShouldFail()
				{
					TimeSpan[] values = [new TimeSpan(1, 0, 0), new TimeSpan(2, 0, 0), new TimeSpan(3, 0, 0),];
					IEnumerable<TimeSpan> subject = values;
					IEnumerable<TimeSpan> expected = [new TimeSpan(2, 2, 0), new TimeSpan(3, 0, 0),];

					async Task Act()
						=> await That(subject).Contains(expected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains collection expected ± 1:00 in order and contiguous,
						              but it lacked all 2 expected items

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(expected, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task Collection_WhenEachItemLiesWithinTheTolerance_ShouldSucceed()
				{
					TimeSpan[] values = [new TimeSpan(1, 0, 0), new TimeSpan(2, 0, 0), new TimeSpan(3, 0, 0),];
					IEnumerable<TimeSpan> subject = values;
					IEnumerable<TimeSpan> expected = [new TimeSpan(2, 1, 0), new TimeSpan(3, 0, 0),];

					async Task Act()
						=> await That(subject).Contains(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Item_WhenItLiesAtTheEdgeOfTheTolerance_ShouldSucceed()
				{
					TimeSpan[] values = [new TimeSpan(1, 0, 0), new TimeSpan(2, 0, 0), new TimeSpan(3, 0, 0),];
					IEnumerable<TimeSpan> subject = values;

					async Task Act()
						=> await That(subject).Contains(new TimeSpan(2, 1, 0)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Item_WhenItLiesOutsideTheTolerance_ShouldFail()
				{
					TimeSpan[] values = [new TimeSpan(1, 0, 0), new TimeSpan(2, 0, 0), new TimeSpan(3, 0, 0),];
					IEnumerable<TimeSpan> subject = values;
					TimeSpan expected = new TimeSpan(2, 2, 0);

					async Task Act()
						=> await That(subject).Contains(expected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains an item equal to {Formatter.Format(expected)} ± 1:00 at least once,
						              but it did not contain it

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task Item_WhenTheDefaultToleranceIsSet_ShouldApplyIt()
				{
					TimeSpan[] values = [new TimeSpan(1, 0, 0), new TimeSpan(2, 0, 0), new TimeSpan(3, 0, 0),];
					IEnumerable<TimeSpan> subject = values;

					async Task Act()
					{
						using IDisposable __ =
							Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Minutes());
						await That(subject).Contains(new TimeSpan(2, 1, 0));
					}

					await That(Act).DoesNotThrow()
						.Because("the items fall back to the default tolerance, as a single value does");
				}

				[Fact]
				public async Task Item_WhenTheDefaultToleranceIsSet_ShouldMentionIt()
				{
					TimeSpan[] values = [new TimeSpan(1, 0, 0), new TimeSpan(2, 0, 0), new TimeSpan(3, 0, 0),];
					IEnumerable<TimeSpan> subject = values;
					TimeSpan expected = new TimeSpan(2, 2, 0);

					async Task Act()
					{
						using IDisposable __ =
							Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Minutes());
						await That(subject).Contains(expected);
					}

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains an item equal to {Formatter.Format(expected)} ± 1:00 at least once,
						              but it did not contain it

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """)
						.Because("the applied default tolerance is part of the expectation");
				}
			}

			public sealed class NullableTimeSpanTests
			{
				[Fact]
				public async Task Collection_WhenAnItemLiesOutsideTheTolerance_ShouldFail()
				{
					TimeSpan?[] values = [new TimeSpan(1, 0, 0), null, new TimeSpan(2, 0, 0), new TimeSpan(3, 0, 0),];
					IEnumerable<TimeSpan?> subject = values;
					IEnumerable<TimeSpan?> expected = [new TimeSpan(2, 2, 0), new TimeSpan(3, 0, 0),];

					async Task Act()
						=> await That(subject).Contains(expected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains collection expected ± 1:00 in order and contiguous,
						              but it lacked all 2 expected items

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(expected, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task Collection_WhenEachItemLiesWithinTheTolerance_ShouldSucceed()
				{
					TimeSpan?[] values = [new TimeSpan(1, 0, 0), null, new TimeSpan(2, 0, 0), new TimeSpan(3, 0, 0),];
					IEnumerable<TimeSpan?> subject = values;
					IEnumerable<TimeSpan?> expected = [new TimeSpan(2, 1, 0), new TimeSpan(3, 0, 0),];

					async Task Act()
						=> await That(subject).Contains(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Item_WhenItLiesAtTheEdgeOfTheTolerance_ShouldSucceed()
				{
					TimeSpan?[] values = [new TimeSpan(1, 0, 0), null, new TimeSpan(2, 0, 0), new TimeSpan(3, 0, 0),];
					IEnumerable<TimeSpan?> subject = values;

					async Task Act()
						=> await That(subject).Contains(new TimeSpan(2, 1, 0)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Item_WhenItLiesOutsideTheTolerance_ShouldFail()
				{
					TimeSpan?[] values = [new TimeSpan(1, 0, 0), null, new TimeSpan(2, 0, 0), new TimeSpan(3, 0, 0),];
					IEnumerable<TimeSpan?> subject = values;
					TimeSpan? expected = new TimeSpan(2, 2, 0);

					async Task Act()
						=> await That(subject).Contains(expected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              contains an item equal to {Formatter.Format(expected)} ± 1:00 at least once,
						              but it did not contain it

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}
			}
		}
	}

	public sealed partial class DoesNotContain
	{
		public sealed class Within
		{
#if NET8_0_OR_GREATER
			public sealed class DateOnlyTests
			{
				[Fact]
				public async Task Collection_WhenAnItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateOnly[] values = [new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IEnumerable<DateOnly> subject = values;
					IEnumerable<DateOnly> unexpected = [new DateOnly(2024, 1, 13), new DateOnly(2024, 1, 21),];

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(1.Days());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Collection_WhenEachItemLiesWithinTheTolerance_ShouldFail()
				{
					DateOnly[] values = [new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IEnumerable<DateOnly> subject = values;
					IEnumerable<DateOnly> unexpected = [new DateOnly(2024, 1, 12), new DateOnly(2024, 1, 21),];

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(1.Days());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not contain collection unexpected ± 1 day in order and contiguous,
						              but it did

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(unexpected, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task Item_WhenItLiesAtTheEdgeOfTheTolerance_ShouldFail()
				{
					DateOnly[] values = [new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IEnumerable<DateOnly> subject = values;
					DateOnly unexpected = new DateOnly(2024, 1, 12);

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(1.Days());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not contain an item equal to {Formatter.Format(unexpected)} ± 1 day,
						              but it contained {Formatter.Format(values[1])} at least once

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task Item_WhenItLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateOnly[] values = [new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IEnumerable<DateOnly> subject = values;

					async Task Act()
						=> await That(subject).DoesNotContain(new DateOnly(2024, 1, 13)).Within(1.Days());

					await That(Act).DoesNotThrow();
				}
			}
#endif

#if NET8_0_OR_GREATER
			public sealed class NullableDateOnlyTests
			{
				[Fact]
				public async Task Collection_WhenAnItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateOnly?[] values = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IEnumerable<DateOnly?> subject = values;
					IEnumerable<DateOnly?> unexpected = [new DateOnly(2024, 1, 13), new DateOnly(2024, 1, 21),];

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(1.Days());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Collection_WhenEachItemLiesWithinTheTolerance_ShouldFail()
				{
					DateOnly?[] values = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IEnumerable<DateOnly?> subject = values;
					IEnumerable<DateOnly?> unexpected = [new DateOnly(2024, 1, 12), new DateOnly(2024, 1, 21),];

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(1.Days());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not contain collection unexpected ± 1 day in order and contiguous,
						              but it did

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(unexpected, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task Item_WhenItLiesAtTheEdgeOfTheTolerance_ShouldFail()
				{
					DateOnly?[] values = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IEnumerable<DateOnly?> subject = values;
					DateOnly? unexpected = new DateOnly(2024, 1, 12);

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(1.Days());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not contain an item equal to {Formatter.Format(unexpected)} ± 1 day,
						              but it contained {Formatter.Format(values[2])} at least once

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task Item_WhenItLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateOnly?[] values = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IEnumerable<DateOnly?> subject = values;

					async Task Act()
						=> await That(subject).DoesNotContain(new DateOnly(2024, 1, 13)).Within(1.Days());

					await That(Act).DoesNotThrow();
				}
			}
#endif

			public sealed class DateTimeTests
			{
				[Fact]
				public async Task Collection_WhenAnItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateTime[] values = [new DateTime(2024, 1, 1, 13, 0, 0), new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					IEnumerable<DateTime> subject = values;
					IEnumerable<DateTime> unexpected = [new DateTime(2024, 1, 1, 14, 2, 0), new DateTime(2024, 1, 1, 15, 0, 0),];

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Collection_WhenEachItemLiesWithinTheTolerance_ShouldFail()
				{
					DateTime[] values = [new DateTime(2024, 1, 1, 13, 0, 0), new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					IEnumerable<DateTime> subject = values;
					IEnumerable<DateTime> unexpected = [new DateTime(2024, 1, 1, 14, 1, 0), new DateTime(2024, 1, 1, 15, 0, 0),];

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not contain collection unexpected ± 1:00 in order and contiguous,
						              but it did

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(unexpected, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task Item_WhenItLiesAtTheEdgeOfTheTolerance_ShouldFail()
				{
					DateTime[] values = [new DateTime(2024, 1, 1, 13, 0, 0), new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					IEnumerable<DateTime> subject = values;
					DateTime unexpected = new DateTime(2024, 1, 1, 14, 1, 0);

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not contain an item equal to {Formatter.Format(unexpected)} ± 1:00,
						              but it contained {Formatter.Format(values[1])} at least once

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task Item_WhenItLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateTime[] values = [new DateTime(2024, 1, 1, 13, 0, 0), new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					IEnumerable<DateTime> subject = values;

					async Task Act()
						=> await That(subject).DoesNotContain(new DateTime(2024, 1, 1, 14, 2, 0)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableDateTimeTests
			{
				[Fact]
				public async Task Collection_WhenAnItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateTime?[] values = [new DateTime(2024, 1, 1, 13, 0, 0), null, new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					IEnumerable<DateTime?> subject = values;
					IEnumerable<DateTime?> unexpected = [new DateTime(2024, 1, 1, 14, 2, 0), new DateTime(2024, 1, 1, 15, 0, 0),];

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Collection_WhenEachItemLiesWithinTheTolerance_ShouldFail()
				{
					DateTime?[] values = [new DateTime(2024, 1, 1, 13, 0, 0), null, new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					IEnumerable<DateTime?> subject = values;
					IEnumerable<DateTime?> unexpected = [new DateTime(2024, 1, 1, 14, 1, 0), new DateTime(2024, 1, 1, 15, 0, 0),];

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not contain collection unexpected ± 1:00 in order and contiguous,
						              but it did

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(unexpected, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task Item_WhenItLiesAtTheEdgeOfTheTolerance_ShouldFail()
				{
					DateTime?[] values = [new DateTime(2024, 1, 1, 13, 0, 0), null, new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					IEnumerable<DateTime?> subject = values;
					DateTime? unexpected = new DateTime(2024, 1, 1, 14, 1, 0);

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not contain an item equal to {Formatter.Format(unexpected)} ± 1:00,
						              but it contained {Formatter.Format(values[2])} at least once

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task Item_WhenItLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateTime?[] values = [new DateTime(2024, 1, 1, 13, 0, 0), null, new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					IEnumerable<DateTime?> subject = values;

					async Task Act()
						=> await That(subject).DoesNotContain(new DateTime(2024, 1, 1, 14, 2, 0)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class DateTimeOffsetTests
			{
				[Fact]
				public async Task Collection_WhenAnItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateTimeOffset[] values = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					IEnumerable<DateTimeOffset> subject = values;
					IEnumerable<DateTimeOffset> unexpected = [new DateTimeOffset(2024, 1, 1, 14, 2, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Collection_WhenEachItemLiesWithinTheTolerance_ShouldFail()
				{
					DateTimeOffset[] values = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					IEnumerable<DateTimeOffset> subject = values;
					IEnumerable<DateTimeOffset> unexpected = [new DateTimeOffset(2024, 1, 1, 14, 1, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not contain collection unexpected ± 1:00 in order and contiguous,
						              but it did

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(unexpected, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task Item_WhenItLiesAtTheEdgeOfTheTolerance_ShouldFail()
				{
					DateTimeOffset[] values = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					IEnumerable<DateTimeOffset> subject = values;
					DateTimeOffset unexpected = new DateTimeOffset(2024, 1, 1, 14, 1, 0, TimeSpan.Zero);

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not contain an item equal to {Formatter.Format(unexpected)} ± 1:00,
						              but it contained {Formatter.Format(values[1])} at least once

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task Item_WhenItLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateTimeOffset[] values = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					IEnumerable<DateTimeOffset> subject = values;

					async Task Act()
						=> await That(subject).DoesNotContain(new DateTimeOffset(2024, 1, 1, 14, 2, 0, TimeSpan.Zero)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableDateTimeOffsetTests
			{
				[Fact]
				public async Task Collection_WhenAnItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateTimeOffset?[] values = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), null, new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					IEnumerable<DateTimeOffset?> subject = values;
					IEnumerable<DateTimeOffset?> unexpected = [new DateTimeOffset(2024, 1, 1, 14, 2, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Collection_WhenEachItemLiesWithinTheTolerance_ShouldFail()
				{
					DateTimeOffset?[] values = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), null, new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					IEnumerable<DateTimeOffset?> subject = values;
					IEnumerable<DateTimeOffset?> unexpected = [new DateTimeOffset(2024, 1, 1, 14, 1, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not contain collection unexpected ± 1:00 in order and contiguous,
						              but it did

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(unexpected, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task Item_WhenItLiesAtTheEdgeOfTheTolerance_ShouldFail()
				{
					DateTimeOffset?[] values = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), null, new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					IEnumerable<DateTimeOffset?> subject = values;
					DateTimeOffset? unexpected = new DateTimeOffset(2024, 1, 1, 14, 1, 0, TimeSpan.Zero);

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not contain an item equal to {Formatter.Format(unexpected)} ± 1:00,
						              but it contained {Formatter.Format(values[2])} at least once

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task Item_WhenItLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateTimeOffset?[] values = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), null, new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					IEnumerable<DateTimeOffset?> subject = values;

					async Task Act()
						=> await That(subject).DoesNotContain(new DateTimeOffset(2024, 1, 1, 14, 2, 0, TimeSpan.Zero)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class DecimalTests
			{
				[Fact]
				public async Task Collection_WhenAnItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					decimal[] values = [1.0m, 2.0m, 3.0m,];
					IEnumerable<decimal> subject = values;
					IEnumerable<decimal> unexpected = [2.5m, 3.0m,];

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(0.25m);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Collection_WhenEachItemLiesWithinTheTolerance_ShouldFail()
				{
					decimal[] values = [1.0m, 2.0m, 3.0m,];
					IEnumerable<decimal> subject = values;
					IEnumerable<decimal> unexpected = [2.25m, 3.0m,];

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(0.25m);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not contain collection unexpected ± 0.25 in order and contiguous,
						              but it did

						              Collection:
						              {Formatter.Format(values)}

						              Expected:
						              {Formatter.Format(unexpected)}
						              """);
				}

				[Fact]
				public async Task Item_WhenItLiesAtTheEdgeOfTheTolerance_ShouldFail()
				{
					decimal[] values = [1.0m, 2.0m, 3.0m,];
					IEnumerable<decimal> subject = values;
					decimal unexpected = 2.25m;

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(0.25m);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not contain an item equal to {Formatter.Format(unexpected)} ± 0.25,
						              but it contained {Formatter.Format(values[1])} at least once

						              Collection:
						              {Formatter.Format(values)}
						              """);
				}

				[Fact]
				public async Task Item_WhenItLiesOutsideTheTolerance_ShouldSucceed()
				{
					decimal[] values = [1.0m, 2.0m, 3.0m,];
					IEnumerable<decimal> subject = values;

					async Task Act()
						=> await That(subject).DoesNotContain(2.5m).Within(0.25m);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableDecimalTests
			{
				[Fact]
				public async Task Collection_WhenAnItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					decimal?[] values = [1.0m, null, 2.0m, 3.0m,];
					IEnumerable<decimal?> subject = values;
					IEnumerable<decimal?> unexpected = [2.5m, 3.0m,];

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(0.25m);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Collection_WhenEachItemLiesWithinTheTolerance_ShouldFail()
				{
					decimal?[] values = [1.0m, null, 2.0m, 3.0m,];
					IEnumerable<decimal?> subject = values;
					IEnumerable<decimal?> unexpected = [2.25m, 3.0m,];

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(0.25m);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not contain collection unexpected ± 0.25 in order and contiguous,
						              but it did

						              Collection:
						              {Formatter.Format(values)}

						              Expected:
						              {Formatter.Format(unexpected)}
						              """);
				}

				[Fact]
				public async Task Item_WhenItLiesAtTheEdgeOfTheTolerance_ShouldFail()
				{
					decimal?[] values = [1.0m, null, 2.0m, 3.0m,];
					IEnumerable<decimal?> subject = values;
					decimal? unexpected = 2.25m;

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(0.25m);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not contain an item equal to {Formatter.Format(unexpected)} ± 0.25,
						              but it contained {Formatter.Format(values[2])} at least once

						              Collection:
						              {Formatter.Format(values)}
						              """);
				}

				[Fact]
				public async Task Item_WhenItLiesOutsideTheTolerance_ShouldSucceed()
				{
					decimal?[] values = [1.0m, null, 2.0m, 3.0m,];
					IEnumerable<decimal?> subject = values;

					async Task Act()
						=> await That(subject).DoesNotContain(2.5m).Within(0.25m);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class DoubleTests
			{
				[Fact]
				public async Task Collection_WhenAnItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					double[] values = [1.0, 2.0, 3.0,];
					IEnumerable<double> subject = values;
					IEnumerable<double> unexpected = [2.5, 3.0,];

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(0.25);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Collection_WhenEachItemLiesWithinTheTolerance_ShouldFail()
				{
					double[] values = [1.0, 2.0, 3.0,];
					IEnumerable<double> subject = values;
					IEnumerable<double> unexpected = [2.25, 3.0,];

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(0.25);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not contain collection unexpected ± 0.25 in order and contiguous,
						              but it did

						              Collection:
						              {Formatter.Format(values)}

						              Expected:
						              {Formatter.Format(unexpected)}
						              """);
				}

				[Fact]
				public async Task Item_WhenItLiesAtTheEdgeOfTheTolerance_ShouldFail()
				{
					double[] values = [1.0, 2.0, 3.0,];
					IEnumerable<double> subject = values;
					double unexpected = 2.25;

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(0.25);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not contain an item equal to {Formatter.Format(unexpected)} ± 0.25,
						              but it contained {Formatter.Format(values[1])} at least once

						              Collection:
						              {Formatter.Format(values)}
						              """);
				}

				[Fact]
				public async Task Item_WhenItLiesOutsideTheTolerance_ShouldSucceed()
				{
					double[] values = [1.0, 2.0, 3.0,];
					IEnumerable<double> subject = values;

					async Task Act()
						=> await That(subject).DoesNotContain(2.5).Within(0.25);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableDoubleTests
			{
				[Fact]
				public async Task Collection_WhenAnItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					double?[] values = [1.0, null, 2.0, 3.0,];
					IEnumerable<double?> subject = values;
					IEnumerable<double?> unexpected = [2.5, 3.0,];

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(0.25);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Collection_WhenEachItemLiesWithinTheTolerance_ShouldFail()
				{
					double?[] values = [1.0, null, 2.0, 3.0,];
					IEnumerable<double?> subject = values;
					IEnumerable<double?> unexpected = [2.25, 3.0,];

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(0.25);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not contain collection unexpected ± 0.25 in order and contiguous,
						              but it did

						              Collection:
						              {Formatter.Format(values)}

						              Expected:
						              {Formatter.Format(unexpected)}
						              """);
				}

				[Fact]
				public async Task Item_WhenItLiesAtTheEdgeOfTheTolerance_ShouldFail()
				{
					double?[] values = [1.0, null, 2.0, 3.0,];
					IEnumerable<double?> subject = values;
					double? unexpected = 2.25;

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(0.25);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not contain an item equal to {Formatter.Format(unexpected)} ± 0.25,
						              but it contained {Formatter.Format(values[2])} at least once

						              Collection:
						              {Formatter.Format(values)}
						              """);
				}

				[Fact]
				public async Task Item_WhenItLiesOutsideTheTolerance_ShouldSucceed()
				{
					double?[] values = [1.0, null, 2.0, 3.0,];
					IEnumerable<double?> subject = values;

					async Task Act()
						=> await That(subject).DoesNotContain(2.5).Within(0.25);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class FloatTests
			{
				[Fact]
				public async Task Collection_WhenAnItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					float[] values = [1.0F, 2.0F, 3.0F,];
					IEnumerable<float> subject = values;
					IEnumerable<float> unexpected = [2.5F, 3.0F,];

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(0.25F);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Collection_WhenEachItemLiesWithinTheTolerance_ShouldFail()
				{
					float[] values = [1.0F, 2.0F, 3.0F,];
					IEnumerable<float> subject = values;
					IEnumerable<float> unexpected = [2.25F, 3.0F,];

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(0.25F);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not contain collection unexpected ± 0.25 in order and contiguous,
						              but it did

						              Collection:
						              {Formatter.Format(values)}

						              Expected:
						              {Formatter.Format(unexpected)}
						              """);
				}

				[Fact]
				public async Task Item_WhenItLiesAtTheEdgeOfTheTolerance_ShouldFail()
				{
					float[] values = [1.0F, 2.0F, 3.0F,];
					IEnumerable<float> subject = values;
					float unexpected = 2.25F;

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(0.25F);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not contain an item equal to {Formatter.Format(unexpected)} ± 0.25,
						              but it contained {Formatter.Format(values[1])} at least once

						              Collection:
						              {Formatter.Format(values)}
						              """);
				}

				[Fact]
				public async Task Item_WhenItLiesOutsideTheTolerance_ShouldSucceed()
				{
					float[] values = [1.0F, 2.0F, 3.0F,];
					IEnumerable<float> subject = values;

					async Task Act()
						=> await That(subject).DoesNotContain(2.5F).Within(0.25F);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableFloatTests
			{
				[Fact]
				public async Task Collection_WhenAnItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					float?[] values = [1.0F, null, 2.0F, 3.0F,];
					IEnumerable<float?> subject = values;
					IEnumerable<float?> unexpected = [2.5F, 3.0F,];

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(0.25F);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Collection_WhenEachItemLiesWithinTheTolerance_ShouldFail()
				{
					float?[] values = [1.0F, null, 2.0F, 3.0F,];
					IEnumerable<float?> subject = values;
					IEnumerable<float?> unexpected = [2.25F, 3.0F,];

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(0.25F);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not contain collection unexpected ± 0.25 in order and contiguous,
						              but it did

						              Collection:
						              {Formatter.Format(values)}

						              Expected:
						              {Formatter.Format(unexpected)}
						              """);
				}

				[Fact]
				public async Task Item_WhenItLiesAtTheEdgeOfTheTolerance_ShouldFail()
				{
					float?[] values = [1.0F, null, 2.0F, 3.0F,];
					IEnumerable<float?> subject = values;
					float? unexpected = 2.25F;

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(0.25F);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not contain an item equal to {Formatter.Format(unexpected)} ± 0.25,
						              but it contained {Formatter.Format(values[2])} at least once

						              Collection:
						              {Formatter.Format(values)}
						              """);
				}

				[Fact]
				public async Task Item_WhenItLiesOutsideTheTolerance_ShouldSucceed()
				{
					float?[] values = [1.0F, null, 2.0F, 3.0F,];
					IEnumerable<float?> subject = values;

					async Task Act()
						=> await That(subject).DoesNotContain(2.5F).Within(0.25F);

					await That(Act).DoesNotThrow();
				}
			}

#if NET8_0_OR_GREATER
			public sealed class TimeOnlyTests
			{
				[Fact]
				public async Task Collection_WhenAnItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					TimeOnly[] values = [new TimeOnly(13, 0), new TimeOnly(14, 0), new TimeOnly(15, 0),];
					IEnumerable<TimeOnly> subject = values;
					IEnumerable<TimeOnly> unexpected = [new TimeOnly(14, 2), new TimeOnly(15, 0),];

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Collection_WhenEachItemLiesWithinTheTolerance_ShouldFail()
				{
					TimeOnly[] values = [new TimeOnly(13, 0), new TimeOnly(14, 0), new TimeOnly(15, 0),];
					IEnumerable<TimeOnly> subject = values;
					IEnumerable<TimeOnly> unexpected = [new TimeOnly(14, 1), new TimeOnly(15, 0),];

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not contain collection unexpected ± 1:00 in order and contiguous,
						              but it did

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(unexpected, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task Item_WhenItLiesAtTheEdgeOfTheTolerance_ShouldFail()
				{
					TimeOnly[] values = [new TimeOnly(13, 0), new TimeOnly(14, 0), new TimeOnly(15, 0),];
					IEnumerable<TimeOnly> subject = values;
					TimeOnly unexpected = new TimeOnly(14, 1);

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not contain an item equal to {Formatter.Format(unexpected)} ± 1:00,
						              but it contained {Formatter.Format(values[1])} at least once

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task Item_WhenItLiesOutsideTheTolerance_ShouldSucceed()
				{
					TimeOnly[] values = [new TimeOnly(13, 0), new TimeOnly(14, 0), new TimeOnly(15, 0),];
					IEnumerable<TimeOnly> subject = values;

					async Task Act()
						=> await That(subject).DoesNotContain(new TimeOnly(14, 2)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}
#endif

#if NET8_0_OR_GREATER
			public sealed class NullableTimeOnlyTests
			{
				[Fact]
				public async Task Collection_WhenAnItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					TimeOnly?[] values = [new TimeOnly(13, 0), null, new TimeOnly(14, 0), new TimeOnly(15, 0),];
					IEnumerable<TimeOnly?> subject = values;
					IEnumerable<TimeOnly?> unexpected = [new TimeOnly(14, 2), new TimeOnly(15, 0),];

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Collection_WhenEachItemLiesWithinTheTolerance_ShouldFail()
				{
					TimeOnly?[] values = [new TimeOnly(13, 0), null, new TimeOnly(14, 0), new TimeOnly(15, 0),];
					IEnumerable<TimeOnly?> subject = values;
					IEnumerable<TimeOnly?> unexpected = [new TimeOnly(14, 1), new TimeOnly(15, 0),];

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not contain collection unexpected ± 1:00 in order and contiguous,
						              but it did

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(unexpected, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task Item_WhenItLiesAtTheEdgeOfTheTolerance_ShouldFail()
				{
					TimeOnly?[] values = [new TimeOnly(13, 0), null, new TimeOnly(14, 0), new TimeOnly(15, 0),];
					IEnumerable<TimeOnly?> subject = values;
					TimeOnly? unexpected = new TimeOnly(14, 1);

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not contain an item equal to {Formatter.Format(unexpected)} ± 1:00,
						              but it contained {Formatter.Format(values[2])} at least once

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task Item_WhenItLiesOutsideTheTolerance_ShouldSucceed()
				{
					TimeOnly?[] values = [new TimeOnly(13, 0), null, new TimeOnly(14, 0), new TimeOnly(15, 0),];
					IEnumerable<TimeOnly?> subject = values;

					async Task Act()
						=> await That(subject).DoesNotContain(new TimeOnly(14, 2)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}
#endif

			public sealed class TimeSpanTests
			{
				[Fact]
				public async Task Collection_WhenAnItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					TimeSpan[] values = [new TimeSpan(1, 0, 0), new TimeSpan(2, 0, 0), new TimeSpan(3, 0, 0),];
					IEnumerable<TimeSpan> subject = values;
					IEnumerable<TimeSpan> unexpected = [new TimeSpan(2, 2, 0), new TimeSpan(3, 0, 0),];

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Collection_WhenEachItemLiesWithinTheTolerance_ShouldFail()
				{
					TimeSpan[] values = [new TimeSpan(1, 0, 0), new TimeSpan(2, 0, 0), new TimeSpan(3, 0, 0),];
					IEnumerable<TimeSpan> subject = values;
					IEnumerable<TimeSpan> unexpected = [new TimeSpan(2, 1, 0), new TimeSpan(3, 0, 0),];

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not contain collection unexpected ± 1:00 in order and contiguous,
						              but it did

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(unexpected, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task Item_WhenItLiesAtTheEdgeOfTheTolerance_ShouldFail()
				{
					TimeSpan[] values = [new TimeSpan(1, 0, 0), new TimeSpan(2, 0, 0), new TimeSpan(3, 0, 0),];
					IEnumerable<TimeSpan> subject = values;
					TimeSpan unexpected = new TimeSpan(2, 1, 0);

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not contain an item equal to {Formatter.Format(unexpected)} ± 1:00,
						              but it contained {Formatter.Format(values[1])} at least once

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task Item_WhenItLiesOutsideTheTolerance_ShouldSucceed()
				{
					TimeSpan[] values = [new TimeSpan(1, 0, 0), new TimeSpan(2, 0, 0), new TimeSpan(3, 0, 0),];
					IEnumerable<TimeSpan> subject = values;

					async Task Act()
						=> await That(subject).DoesNotContain(new TimeSpan(2, 2, 0)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableTimeSpanTests
			{
				[Fact]
				public async Task Collection_WhenAnItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					TimeSpan?[] values = [new TimeSpan(1, 0, 0), null, new TimeSpan(2, 0, 0), new TimeSpan(3, 0, 0),];
					IEnumerable<TimeSpan?> subject = values;
					IEnumerable<TimeSpan?> unexpected = [new TimeSpan(2, 2, 0), new TimeSpan(3, 0, 0),];

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Collection_WhenEachItemLiesWithinTheTolerance_ShouldFail()
				{
					TimeSpan?[] values = [new TimeSpan(1, 0, 0), null, new TimeSpan(2, 0, 0), new TimeSpan(3, 0, 0),];
					IEnumerable<TimeSpan?> subject = values;
					IEnumerable<TimeSpan?> unexpected = [new TimeSpan(2, 1, 0), new TimeSpan(3, 0, 0),];

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not contain collection unexpected ± 1:00 in order and contiguous,
						              but it did

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(unexpected, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task Item_WhenItLiesAtTheEdgeOfTheTolerance_ShouldFail()
				{
					TimeSpan?[] values = [new TimeSpan(1, 0, 0), null, new TimeSpan(2, 0, 0), new TimeSpan(3, 0, 0),];
					IEnumerable<TimeSpan?> subject = values;
					TimeSpan? unexpected = new TimeSpan(2, 1, 0);

					async Task Act()
						=> await That(subject).DoesNotContain(unexpected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not contain an item equal to {Formatter.Format(unexpected)} ± 1:00,
						              but it contained {Formatter.Format(values[2])} at least once

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task Item_WhenItLiesOutsideTheTolerance_ShouldSucceed()
				{
					TimeSpan?[] values = [new TimeSpan(1, 0, 0), null, new TimeSpan(2, 0, 0), new TimeSpan(3, 0, 0),];
					IEnumerable<TimeSpan?> subject = values;

					async Task Act()
						=> await That(subject).DoesNotContain(new TimeSpan(2, 2, 0)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
