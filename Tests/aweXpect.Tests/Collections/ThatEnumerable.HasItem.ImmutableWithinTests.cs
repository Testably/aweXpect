#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using aweXpect.Customization;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class HasItem
	{
		public sealed class ImmutableWithin
		{
			public sealed class DateOnlyTests
			{
				[Test]
				public async Task WhenTheItemAtTheIndexLiesWithinTheTolerance_ShouldSucceed()
				{
					DateOnly[] values = [new(2024, 1, 1), new(2024, 1, 11), new(2024, 1, 21),];
					ImmutableArray<DateOnly> subject = [.. values,];

					async Task Act()
						=> await That(subject).HasItem(new DateOnly(2024, 1, 12)).Within(1.Days()).AtIndex(1);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTheItemLiesAtTheEdgeOfTheTolerance_ShouldSucceed()
				{
					DateOnly[] values = [new(2024, 1, 1), new(2024, 1, 11), new(2024, 1, 21),];
					ImmutableArray<DateOnly> subject = [.. values,];

					async Task Act()
						=> await That(subject).HasItem(new DateOnly(2024, 1, 12)).Within(1.Days());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTheItemLiesOutsideTheTolerance_ShouldFail()
				{
					DateOnly[] values = [new(2024, 1, 1), new(2024, 1, 11), new(2024, 1, 21),];
					ImmutableArray<DateOnly> subject = [.. values,];
					DateOnly expected = new(2024, 1, 13);

					async Task Act()
						=> await That(subject).HasItem(expected).Within(1.Days());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has an item equal to {Formatter.Format(expected)} ± 1 day,
						              but it had no matching item

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Test]
				public async Task WhenToleranceIsNotAWholeNumberOfDays_ShouldThrowArgumentOutOfRangeException()
				{
					DateOnly[] values = [new(2024, 1, 1), new(2024, 1, 11), new(2024, 1, 21),];
					ImmutableArray<DateOnly> subject = [.. values,];

					object Act()
						=> That(subject).HasItem(new DateOnly(2024, 1, 12)).Within(1.Days() + 1.Hours());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("tolerance").And
						.WithMessage("The tolerance must be a whole number of days.").AsPrefix()
						.Because("a date has no time of day, so the remainder is rejected as soon as it is specified");
				}
			}

			public sealed class NullableDateOnlyTests
			{
				[Test]
				public async Task WhenTheItemAtTheIndexLiesWithinTheTolerance_ShouldSucceed()
				{
					DateOnly?[] values = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					ImmutableArray<DateOnly?> subject = [.. values,];

					async Task Act()
						=> await That(subject).HasItem(new DateOnly(2024, 1, 12)).Within(1.Days()).AtIndex(2);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTheItemLiesAtTheEdgeOfTheTolerance_ShouldSucceed()
				{
					DateOnly?[] values = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					ImmutableArray<DateOnly?> subject = [.. values,];

					async Task Act()
						=> await That(subject).HasItem(new DateOnly(2024, 1, 12)).Within(1.Days());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTheItemLiesOutsideTheTolerance_ShouldFail()
				{
					DateOnly?[] values = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					ImmutableArray<DateOnly?> subject = [.. values,];
					DateOnly? expected = new DateOnly(2024, 1, 13);

					async Task Act()
						=> await That(subject).HasItem(expected).Within(1.Days());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has an item equal to {Formatter.Format(expected)} ± 1 day,
						              but it had no matching item

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Test]
				public async Task WhenToleranceIsNotAWholeNumberOfDays_ShouldThrowArgumentOutOfRangeException()
				{
					DateOnly?[] values = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					ImmutableArray<DateOnly?> subject = [.. values,];

					object Act()
						=> That(subject).HasItem(new DateOnly(2024, 1, 12)).Within(1.Days() + 1.Hours());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("tolerance").And
						.WithMessage("The tolerance must be a whole number of days.").AsPrefix()
						.Because("a date has no time of day, so the remainder is rejected as soon as it is specified");
				}
			}

			public sealed class DateTimeTests
			{
				[Test]
				public async Task WhenTheDefaultToleranceIsSet_ShouldApplyIt()
				{
					DateTime[] values = [new(2024, 1, 1, 13, 0, 0), new(2024, 1, 1, 14, 0, 0), new(2024, 1, 1, 15, 0, 0),];
					ImmutableArray<DateTime> subject = [.. values,];

					async Task Act()
					{
						using IDisposable __ =
							Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Minutes());
						await That(subject).HasItem(new DateTime(2024, 1, 1, 14, 1, 0));
					}

					await That(Act).DoesNotThrow()
						.Because("the items fall back to the default tolerance, as a single value does");
				}

				[Test]
				public async Task WhenTheDefaultToleranceIsSet_ShouldMentionIt()
				{
					DateTime[] values = [new(2024, 1, 1, 13, 0, 0), new(2024, 1, 1, 14, 0, 0), new(2024, 1, 1, 15, 0, 0),];
					ImmutableArray<DateTime> subject = [.. values,];
					DateTime expected = new(2024, 1, 1, 14, 2, 0);

					async Task Act()
					{
						using IDisposable __ =
							Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Minutes());
						await That(subject).HasItem(expected);
					}

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has an item equal to {Formatter.Format(expected)} ± 1:00,
						              but it had no matching item

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """)
						.Because("the applied default tolerance is part of the expectation");
				}

				[Test]
				public async Task WhenTheItemAtTheIndexLiesWithinTheTolerance_ShouldSucceed()
				{
					DateTime[] values = [new(2024, 1, 1, 13, 0, 0), new(2024, 1, 1, 14, 0, 0), new(2024, 1, 1, 15, 0, 0),];
					ImmutableArray<DateTime> subject = [.. values,];

					async Task Act()
						=> await That(subject).HasItem(new DateTime(2024, 1, 1, 14, 1, 0)).Within(1.Minutes()).AtIndex(1);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTheItemLiesAtTheEdgeOfTheTolerance_ShouldSucceed()
				{
					DateTime[] values = [new(2024, 1, 1, 13, 0, 0), new(2024, 1, 1, 14, 0, 0), new(2024, 1, 1, 15, 0, 0),];
					ImmutableArray<DateTime> subject = [.. values,];

					async Task Act()
						=> await That(subject).HasItem(new DateTime(2024, 1, 1, 14, 1, 0)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTheItemLiesOutsideTheTolerance_ShouldFail()
				{
					DateTime[] values = [new(2024, 1, 1, 13, 0, 0), new(2024, 1, 1, 14, 0, 0), new(2024, 1, 1, 15, 0, 0),];
					ImmutableArray<DateTime> subject = [.. values,];
					DateTime expected = new(2024, 1, 1, 14, 2, 0);

					async Task Act()
						=> await That(subject).HasItem(expected).Within(1.Minutes());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has an item equal to {Formatter.Format(expected)} ± 1:00,
						              but it had no matching item

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}
			}

			public sealed class NullableDateTimeTests
			{
				[Test]
				public async Task WhenTheItemAtTheIndexLiesWithinTheTolerance_ShouldSucceed()
				{
					DateTime?[] values = [new DateTime(2024, 1, 1, 13, 0, 0), null, new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					ImmutableArray<DateTime?> subject = [.. values,];

					async Task Act()
						=> await That(subject).HasItem(new DateTime(2024, 1, 1, 14, 1, 0)).Within(1.Minutes()).AtIndex(2);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTheItemLiesAtTheEdgeOfTheTolerance_ShouldSucceed()
				{
					DateTime?[] values = [new DateTime(2024, 1, 1, 13, 0, 0), null, new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					ImmutableArray<DateTime?> subject = [.. values,];

					async Task Act()
						=> await That(subject).HasItem(new DateTime(2024, 1, 1, 14, 1, 0)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTheItemLiesOutsideTheTolerance_ShouldFail()
				{
					DateTime?[] values = [new DateTime(2024, 1, 1, 13, 0, 0), null, new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					ImmutableArray<DateTime?> subject = [.. values,];
					DateTime? expected = new DateTime(2024, 1, 1, 14, 2, 0);

					async Task Act()
						=> await That(subject).HasItem(expected).Within(1.Minutes());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has an item equal to {Formatter.Format(expected)} ± 1:00,
						              but it had no matching item

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}
			}

			public sealed class DateTimeOffsetTests
			{
				[Test]
				public async Task WhenTheItemAtTheIndexLiesWithinTheTolerance_ShouldSucceed()
				{
					DateTimeOffset[] values = [new(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), new(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					ImmutableArray<DateTimeOffset> subject = [.. values,];

					async Task Act()
						=> await That(subject).HasItem(new DateTimeOffset(2024, 1, 1, 14, 1, 0, TimeSpan.Zero)).Within(1.Minutes()).AtIndex(1);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTheItemLiesAtTheEdgeOfTheTolerance_ShouldSucceed()
				{
					DateTimeOffset[] values = [new(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), new(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					ImmutableArray<DateTimeOffset> subject = [.. values,];

					async Task Act()
						=> await That(subject).HasItem(new DateTimeOffset(2024, 1, 1, 14, 1, 0, TimeSpan.Zero)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTheItemLiesOutsideTheTolerance_ShouldFail()
				{
					DateTimeOffset[] values = [new(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), new(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					ImmutableArray<DateTimeOffset> subject = [.. values,];
					DateTimeOffset expected = new(2024, 1, 1, 14, 2, 0, TimeSpan.Zero);

					async Task Act()
						=> await That(subject).HasItem(expected).Within(1.Minutes());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has an item equal to {Formatter.Format(expected)} ± 1:00,
						              but it had no matching item

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}
			}

			public sealed class NullableDateTimeOffsetTests
			{
				[Test]
				public async Task WhenTheItemAtTheIndexLiesWithinTheTolerance_ShouldSucceed()
				{
					DateTimeOffset?[] values = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), null, new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					ImmutableArray<DateTimeOffset?> subject = [.. values,];

					async Task Act()
						=> await That(subject).HasItem(new DateTimeOffset(2024, 1, 1, 14, 1, 0, TimeSpan.Zero)).Within(1.Minutes()).AtIndex(2);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTheItemLiesAtTheEdgeOfTheTolerance_ShouldSucceed()
				{
					DateTimeOffset?[] values = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), null, new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					ImmutableArray<DateTimeOffset?> subject = [.. values,];

					async Task Act()
						=> await That(subject).HasItem(new DateTimeOffset(2024, 1, 1, 14, 1, 0, TimeSpan.Zero)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTheItemLiesOutsideTheTolerance_ShouldFail()
				{
					DateTimeOffset?[] values = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), null, new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					ImmutableArray<DateTimeOffset?> subject = [.. values,];
					DateTimeOffset? expected = new DateTimeOffset(2024, 1, 1, 14, 2, 0, TimeSpan.Zero);

					async Task Act()
						=> await That(subject).HasItem(expected).Within(1.Minutes());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has an item equal to {Formatter.Format(expected)} ± 1:00,
						              but it had no matching item

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}
			}

			public sealed class DecimalTests
			{
				[Test]
				public async Task WhenTheItemAtTheIndexLiesWithinTheTolerance_ShouldSucceed()
				{
					decimal[] values = [1.0m, 2.0m, 3.0m,];
					ImmutableArray<decimal> subject = [.. values,];

					async Task Act()
						=> await That(subject).HasItem(2.25m).Within(0.25m).AtIndex(1);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTheItemLiesAtTheEdgeOfTheTolerance_ShouldSucceed()
				{
					decimal[] values = [1.0m, 2.0m, 3.0m,];
					ImmutableArray<decimal> subject = [.. values,];

					async Task Act()
						=> await That(subject).HasItem(2.25m).Within(0.25m);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTheItemLiesOutsideTheTolerance_ShouldFail()
				{
					decimal[] values = [1.0m, 2.0m, 3.0m,];
					ImmutableArray<decimal> subject = [.. values,];
					decimal expected = 2.5m;

					async Task Act()
						=> await That(subject).HasItem(expected).Within(0.25m);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has an item equal to {Formatter.Format(expected)} ± 0.25,
						              but it had no matching item

						              Collection:
						              {Formatter.Format(values)}
						              """);
				}
			}

			public sealed class NullableDecimalTests
			{
				[Test]
				public async Task WhenTheItemAtTheIndexLiesWithinTheTolerance_ShouldSucceed()
				{
					decimal?[] values = [1.0m, null, 2.0m, 3.0m,];
					ImmutableArray<decimal?> subject = [.. values,];

					async Task Act()
						=> await That(subject).HasItem(2.25m).Within(0.25m).AtIndex(2);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTheItemLiesAtTheEdgeOfTheTolerance_ShouldSucceed()
				{
					decimal?[] values = [1.0m, null, 2.0m, 3.0m,];
					ImmutableArray<decimal?> subject = [.. values,];

					async Task Act()
						=> await That(subject).HasItem(2.25m).Within(0.25m);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTheItemLiesOutsideTheTolerance_ShouldFail()
				{
					decimal?[] values = [1.0m, null, 2.0m, 3.0m,];
					ImmutableArray<decimal?> subject = [.. values,];
					decimal? expected = 2.5m;

					async Task Act()
						=> await That(subject).HasItem(expected).Within(0.25m);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has an item equal to {Formatter.Format(expected)} ± 0.25,
						              but it had no matching item

						              Collection:
						              {Formatter.Format(values)}
						              """);
				}
			}

			public sealed class DoubleTests
			{
				[Test]
				[Arguments(false)]
				[Arguments(true)]
				public async Task WhenCombinedWithUsing_ShouldThrowInvalidOperationException(bool negated)
				{
					double[] values = [1.0, 2.0, 3.0,];
					ImmutableArray<double> subject = [.. values,];

					async Task Act()
					{
						if (negated)
						{
							await That(subject).DoesNotHaveItem(2.25).Within(0.25).Using(new AllEqualComparer());
						}
						else
						{
							await That(subject).HasItem(2.25).Within(0.25).Using(new AllEqualComparer());
						}
					}

					await That(Act).Throws<InvalidOperationException>()
						.WithMessage("Using cannot be combined with Within.")
						.Because("the comparer would silently replace the tolerance");
				}

				[Test]
				public async Task WhenTheItemAtTheIndexLiesWithinTheTolerance_ShouldSucceed()
				{
					double[] values = [1.0, 2.0, 3.0,];
					ImmutableArray<double> subject = [.. values,];

					async Task Act()
						=> await That(subject).HasItem(2.25).Within(0.25).AtIndex(1);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTheItemLiesAtTheEdgeOfTheTolerance_ShouldSucceed()
				{
					double[] values = [1.0, 2.0, 3.0,];
					ImmutableArray<double> subject = [.. values,];

					async Task Act()
						=> await That(subject).HasItem(2.25).Within(0.25);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTheItemLiesOutsideTheTolerance_ShouldFail()
				{
					double[] values = [1.0, 2.0, 3.0,];
					ImmutableArray<double> subject = [.. values,];
					double expected = 2.5;

					async Task Act()
						=> await That(subject).HasItem(expected).Within(0.25);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has an item equal to {Formatter.Format(expected)} ± 0.25,
						              but it had no matching item

						              Collection:
						              {Formatter.Format(values)}
						              """);
				}
			}

			public sealed class NullableDoubleTests
			{
				[Test]
				public async Task WhenTheItemAtTheIndexLiesWithinTheTolerance_ShouldSucceed()
				{
					double?[] values = [1.0, null, 2.0, 3.0,];
					ImmutableArray<double?> subject = [.. values,];

					async Task Act()
						=> await That(subject).HasItem(2.25).Within(0.25).AtIndex(2);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTheItemLiesAtTheEdgeOfTheTolerance_ShouldSucceed()
				{
					double?[] values = [1.0, null, 2.0, 3.0,];
					ImmutableArray<double?> subject = [.. values,];

					async Task Act()
						=> await That(subject).HasItem(2.25).Within(0.25);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTheItemLiesOutsideTheTolerance_ShouldFail()
				{
					double?[] values = [1.0, null, 2.0, 3.0,];
					ImmutableArray<double?> subject = [.. values,];
					double? expected = 2.5;

					async Task Act()
						=> await That(subject).HasItem(expected).Within(0.25);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has an item equal to {Formatter.Format(expected)} ± 0.25,
						              but it had no matching item

						              Collection:
						              {Formatter.Format(values)}
						              """);
				}
			}

			public sealed class FloatTests
			{
				[Test]
				public async Task WhenTheItemAtTheIndexLiesWithinTheTolerance_ShouldSucceed()
				{
					float[] values = [1.0F, 2.0F, 3.0F,];
					ImmutableArray<float> subject = [.. values,];

					async Task Act()
						=> await That(subject).HasItem(2.25F).Within(0.25F).AtIndex(1);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTheItemLiesAtTheEdgeOfTheTolerance_ShouldSucceed()
				{
					float[] values = [1.0F, 2.0F, 3.0F,];
					ImmutableArray<float> subject = [.. values,];

					async Task Act()
						=> await That(subject).HasItem(2.25F).Within(0.25F);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTheItemLiesOutsideTheTolerance_ShouldFail()
				{
					float[] values = [1.0F, 2.0F, 3.0F,];
					ImmutableArray<float> subject = [.. values,];
					float expected = 2.5F;

					async Task Act()
						=> await That(subject).HasItem(expected).Within(0.25F);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has an item equal to {Formatter.Format(expected)} ± 0.25,
						              but it had no matching item

						              Collection:
						              {Formatter.Format(values)}
						              """);
				}
			}

			public sealed class NullableFloatTests
			{
				[Test]
				public async Task WhenTheItemAtTheIndexLiesWithinTheTolerance_ShouldSucceed()
				{
					float?[] values = [1.0F, null, 2.0F, 3.0F,];
					ImmutableArray<float?> subject = [.. values,];

					async Task Act()
						=> await That(subject).HasItem(2.25F).Within(0.25F).AtIndex(2);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTheItemLiesAtTheEdgeOfTheTolerance_ShouldSucceed()
				{
					float?[] values = [1.0F, null, 2.0F, 3.0F,];
					ImmutableArray<float?> subject = [.. values,];

					async Task Act()
						=> await That(subject).HasItem(2.25F).Within(0.25F);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTheItemLiesOutsideTheTolerance_ShouldFail()
				{
					float?[] values = [1.0F, null, 2.0F, 3.0F,];
					ImmutableArray<float?> subject = [.. values,];
					float? expected = 2.5F;

					async Task Act()
						=> await That(subject).HasItem(expected).Within(0.25F);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has an item equal to {Formatter.Format(expected)} ± 0.25,
						              but it had no matching item

						              Collection:
						              {Formatter.Format(values)}
						              """);
				}
			}

			public sealed class TimeOnlyTests
			{
				[Test]
				public async Task WhenAnItemLiesAcrossMidnight_ShouldUseTheShorterDistance()
				{
					TimeOnly[] values = [new(22, 0), new(23, 59, 30), new(2, 0),];
					ImmutableArray<TimeOnly> subject = [.. values,];

					async Task Act()
						=> await That(subject).HasItem(new TimeOnly(0, 0, 30)).Within(1.Minutes());

					await That(Act).DoesNotThrow()
						.Because("the times are compared on the clock face, where 23:59:30 and 00:00:30 are one minute apart");
				}

				[Test]
				public async Task WhenTheItemAtTheIndexLiesWithinTheTolerance_ShouldSucceed()
				{
					TimeOnly[] values = [new(13, 0), new(14, 0), new(15, 0),];
					ImmutableArray<TimeOnly> subject = [.. values,];

					async Task Act()
						=> await That(subject).HasItem(new TimeOnly(14, 1)).Within(1.Minutes()).AtIndex(1);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTheItemLiesAtTheEdgeOfTheTolerance_ShouldSucceed()
				{
					TimeOnly[] values = [new(13, 0), new(14, 0), new(15, 0),];
					ImmutableArray<TimeOnly> subject = [.. values,];

					async Task Act()
						=> await That(subject).HasItem(new TimeOnly(14, 1)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTheItemLiesOutsideTheTolerance_ShouldFail()
				{
					TimeOnly[] values = [new(13, 0), new(14, 0), new(15, 0),];
					ImmutableArray<TimeOnly> subject = [.. values,];
					TimeOnly expected = new(14, 2);

					async Task Act()
						=> await That(subject).HasItem(expected).Within(1.Minutes());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has an item equal to {Formatter.Format(expected)} ± 1:00,
						              but it had no matching item

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}
			}

			public sealed class NullableTimeOnlyTests
			{
				[Test]
				public async Task WhenAnItemLiesAcrossMidnight_ShouldUseTheShorterDistance()
				{
					TimeOnly?[] values = [new TimeOnly(22, 0), null, new TimeOnly(23, 59, 30), new TimeOnly(2, 0),];
					ImmutableArray<TimeOnly?> subject = [.. values,];

					async Task Act()
						=> await That(subject).HasItem(new TimeOnly(0, 0, 30)).Within(1.Minutes());

					await That(Act).DoesNotThrow()
						.Because("the times are compared on the clock face, where 23:59:30 and 00:00:30 are one minute apart");
				}

				[Test]
				public async Task WhenTheItemAtTheIndexLiesWithinTheTolerance_ShouldSucceed()
				{
					TimeOnly?[] values = [new TimeOnly(13, 0), null, new TimeOnly(14, 0), new TimeOnly(15, 0),];
					ImmutableArray<TimeOnly?> subject = [.. values,];

					async Task Act()
						=> await That(subject).HasItem(new TimeOnly(14, 1)).Within(1.Minutes()).AtIndex(2);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTheItemLiesAtTheEdgeOfTheTolerance_ShouldSucceed()
				{
					TimeOnly?[] values = [new TimeOnly(13, 0), null, new TimeOnly(14, 0), new TimeOnly(15, 0),];
					ImmutableArray<TimeOnly?> subject = [.. values,];

					async Task Act()
						=> await That(subject).HasItem(new TimeOnly(14, 1)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTheItemLiesOutsideTheTolerance_ShouldFail()
				{
					TimeOnly?[] values = [new TimeOnly(13, 0), null, new TimeOnly(14, 0), new TimeOnly(15, 0),];
					ImmutableArray<TimeOnly?> subject = [.. values,];
					TimeOnly? expected = new TimeOnly(14, 2);

					async Task Act()
						=> await That(subject).HasItem(expected).Within(1.Minutes());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has an item equal to {Formatter.Format(expected)} ± 1:00,
						              but it had no matching item

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}
			}

			public sealed class TimeSpanTests
			{
				[Test]
				public async Task WhenTheItemAtTheIndexLiesWithinTheTolerance_ShouldSucceed()
				{
					TimeSpan[] values = [new(1, 0, 0), new(2, 0, 0), new(3, 0, 0),];
					ImmutableArray<TimeSpan> subject = [.. values,];

					async Task Act()
						=> await That(subject).HasItem(new TimeSpan(2, 1, 0)).Within(1.Minutes()).AtIndex(1);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTheItemLiesAtTheEdgeOfTheTolerance_ShouldSucceed()
				{
					TimeSpan[] values = [new(1, 0, 0), new(2, 0, 0), new(3, 0, 0),];
					ImmutableArray<TimeSpan> subject = [.. values,];

					async Task Act()
						=> await That(subject).HasItem(new TimeSpan(2, 1, 0)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTheItemLiesOutsideTheTolerance_ShouldFail()
				{
					TimeSpan[] values = [new(1, 0, 0), new(2, 0, 0), new(3, 0, 0),];
					ImmutableArray<TimeSpan> subject = [.. values,];
					TimeSpan expected = new(2, 2, 0);

					async Task Act()
						=> await That(subject).HasItem(expected).Within(1.Minutes());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has an item equal to {Formatter.Format(expected)} ± 1:00,
						              but it had no matching item

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}
			}

			public sealed class NullableTimeSpanTests
			{
				[Test]
				public async Task WhenTheItemAtTheIndexLiesWithinTheTolerance_ShouldSucceed()
				{
					TimeSpan?[] values = [new TimeSpan(1, 0, 0), null, new TimeSpan(2, 0, 0), new TimeSpan(3, 0, 0),];
					ImmutableArray<TimeSpan?> subject = [.. values,];

					async Task Act()
						=> await That(subject).HasItem(new TimeSpan(2, 1, 0)).Within(1.Minutes()).AtIndex(2);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTheItemLiesAtTheEdgeOfTheTolerance_ShouldSucceed()
				{
					TimeSpan?[] values = [new TimeSpan(1, 0, 0), null, new TimeSpan(2, 0, 0), new TimeSpan(3, 0, 0),];
					ImmutableArray<TimeSpan?> subject = [.. values,];

					async Task Act()
						=> await That(subject).HasItem(new TimeSpan(2, 1, 0)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTheItemLiesOutsideTheTolerance_ShouldFail()
				{
					TimeSpan?[] values = [new TimeSpan(1, 0, 0), null, new TimeSpan(2, 0, 0), new TimeSpan(3, 0, 0),];
					ImmutableArray<TimeSpan?> subject = [.. values,];
					TimeSpan? expected = new TimeSpan(2, 2, 0);

					async Task Act()
						=> await That(subject).HasItem(expected).Within(1.Minutes());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has an item equal to {Formatter.Format(expected)} ± 1:00,
						              but it had no matching item

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}
			}
		}
	}

	public sealed partial class DoesNotHaveItem
	{
		public sealed class ImmutableWithin
		{
			public sealed class DateOnlyTests
			{
				[Test]
				public async Task WhenTheItemLiesAtTheEdgeOfTheTolerance_ShouldFail()
				{
					DateOnly[] values = [new(2024, 1, 1), new(2024, 1, 11), new(2024, 1, 21),];
					ImmutableArray<DateOnly> subject = [.. values,];
					DateOnly unexpected = new(2024, 1, 12);

					async Task Act()
						=> await That(subject).DoesNotHaveItem(unexpected).Within(1.Days());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              does not have an item equal to {Formatter.Format(unexpected)} ± 1 day,
						              but it had item {Formatter.Format(new DateOnly(2024, 1, 11))}

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Test]
				public async Task WhenTheItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateOnly[] values = [new(2024, 1, 1), new(2024, 1, 11), new(2024, 1, 21),];
					ImmutableArray<DateOnly> subject = [.. values,];

					async Task Act()
						=> await That(subject).DoesNotHaveItem(new DateOnly(2024, 1, 13)).Within(1.Days());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableDateOnlyTests
			{
				[Test]
				public async Task WhenTheItemLiesAtTheEdgeOfTheTolerance_ShouldFail()
				{
					DateOnly?[] values = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					ImmutableArray<DateOnly?> subject = [.. values,];
					DateOnly? unexpected = new DateOnly(2024, 1, 12);

					async Task Act()
						=> await That(subject).DoesNotHaveItem(unexpected).Within(1.Days());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              does not have an item equal to {Formatter.Format(unexpected)} ± 1 day,
						              but it had item {Formatter.Format(new DateOnly(2024, 1, 11))}

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Test]
				public async Task WhenTheItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateOnly?[] values = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					ImmutableArray<DateOnly?> subject = [.. values,];

					async Task Act()
						=> await That(subject).DoesNotHaveItem(new DateOnly(2024, 1, 13)).Within(1.Days());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class DateTimeTests
			{
				[Test]
				public async Task WhenTheItemLiesAtTheEdgeOfTheTolerance_ShouldFail()
				{
					DateTime[] values = [new(2024, 1, 1, 13, 0, 0), new(2024, 1, 1, 14, 0, 0), new(2024, 1, 1, 15, 0, 0),];
					ImmutableArray<DateTime> subject = [.. values,];
					DateTime unexpected = new(2024, 1, 1, 14, 1, 0);

					async Task Act()
						=> await That(subject).DoesNotHaveItem(unexpected).Within(1.Minutes());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              does not have an item equal to {Formatter.Format(unexpected)} ± 1:00,
						              but it had item {Formatter.Format(new DateTime(2024, 1, 1, 14, 0, 0))}

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Test]
				public async Task WhenTheItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateTime[] values = [new(2024, 1, 1, 13, 0, 0), new(2024, 1, 1, 14, 0, 0), new(2024, 1, 1, 15, 0, 0),];
					ImmutableArray<DateTime> subject = [.. values,];

					async Task Act()
						=> await That(subject).DoesNotHaveItem(new DateTime(2024, 1, 1, 14, 2, 0)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableDateTimeTests
			{
				[Test]
				public async Task WhenTheItemLiesAtTheEdgeOfTheTolerance_ShouldFail()
				{
					DateTime?[] values = [new DateTime(2024, 1, 1, 13, 0, 0), null, new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					ImmutableArray<DateTime?> subject = [.. values,];
					DateTime? unexpected = new DateTime(2024, 1, 1, 14, 1, 0);

					async Task Act()
						=> await That(subject).DoesNotHaveItem(unexpected).Within(1.Minutes());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              does not have an item equal to {Formatter.Format(unexpected)} ± 1:00,
						              but it had item {Formatter.Format(new DateTime(2024, 1, 1, 14, 0, 0))}

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Test]
				public async Task WhenTheItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateTime?[] values = [new DateTime(2024, 1, 1, 13, 0, 0), null, new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					ImmutableArray<DateTime?> subject = [.. values,];

					async Task Act()
						=> await That(subject).DoesNotHaveItem(new DateTime(2024, 1, 1, 14, 2, 0)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class DateTimeOffsetTests
			{
				[Test]
				public async Task WhenTheItemLiesAtTheEdgeOfTheTolerance_ShouldFail()
				{
					DateTimeOffset[] values = [new(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), new(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					ImmutableArray<DateTimeOffset> subject = [.. values,];
					DateTimeOffset unexpected = new(2024, 1, 1, 14, 1, 0, TimeSpan.Zero);

					async Task Act()
						=> await That(subject).DoesNotHaveItem(unexpected).Within(1.Minutes());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              does not have an item equal to {Formatter.Format(unexpected)} ± 1:00,
						              but it had item {Formatter.Format(new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero))}

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Test]
				public async Task WhenTheItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateTimeOffset[] values = [new(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), new(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					ImmutableArray<DateTimeOffset> subject = [.. values,];

					async Task Act()
						=> await That(subject).DoesNotHaveItem(new DateTimeOffset(2024, 1, 1, 14, 2, 0, TimeSpan.Zero)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableDateTimeOffsetTests
			{
				[Test]
				public async Task WhenTheItemLiesAtTheEdgeOfTheTolerance_ShouldFail()
				{
					DateTimeOffset?[] values = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), null, new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					ImmutableArray<DateTimeOffset?> subject = [.. values,];
					DateTimeOffset? unexpected = new DateTimeOffset(2024, 1, 1, 14, 1, 0, TimeSpan.Zero);

					async Task Act()
						=> await That(subject).DoesNotHaveItem(unexpected).Within(1.Minutes());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              does not have an item equal to {Formatter.Format(unexpected)} ± 1:00,
						              but it had item {Formatter.Format(new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero))}

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Test]
				public async Task WhenTheItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateTimeOffset?[] values = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), null, new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					ImmutableArray<DateTimeOffset?> subject = [.. values,];

					async Task Act()
						=> await That(subject).DoesNotHaveItem(new DateTimeOffset(2024, 1, 1, 14, 2, 0, TimeSpan.Zero)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class DecimalTests
			{
				[Test]
				public async Task WhenTheItemLiesAtTheEdgeOfTheTolerance_ShouldFail()
				{
					decimal[] values = [1.0m, 2.0m, 3.0m,];
					ImmutableArray<decimal> subject = [.. values,];
					decimal unexpected = 2.25m;

					async Task Act()
						=> await That(subject).DoesNotHaveItem(unexpected).Within(0.25m);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              does not have an item equal to {Formatter.Format(unexpected)} ± 0.25,
						              but it had item {Formatter.Format(2.0m)}

						              Collection:
						              {Formatter.Format(values)}
						              """);
				}

				[Test]
				public async Task WhenTheItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					decimal[] values = [1.0m, 2.0m, 3.0m,];
					ImmutableArray<decimal> subject = [.. values,];

					async Task Act()
						=> await That(subject).DoesNotHaveItem(2.5m).Within(0.25m);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableDecimalTests
			{
				[Test]
				public async Task WhenTheItemLiesAtTheEdgeOfTheTolerance_ShouldFail()
				{
					decimal?[] values = [1.0m, null, 2.0m, 3.0m,];
					ImmutableArray<decimal?> subject = [.. values,];
					decimal? unexpected = 2.25m;

					async Task Act()
						=> await That(subject).DoesNotHaveItem(unexpected).Within(0.25m);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              does not have an item equal to {Formatter.Format(unexpected)} ± 0.25,
						              but it had item {Formatter.Format(2.0m)}

						              Collection:
						              {Formatter.Format(values)}
						              """);
				}

				[Test]
				public async Task WhenTheItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					decimal?[] values = [1.0m, null, 2.0m, 3.0m,];
					ImmutableArray<decimal?> subject = [.. values,];

					async Task Act()
						=> await That(subject).DoesNotHaveItem(2.5m).Within(0.25m);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class DoubleTests
			{
				[Test]
				public async Task WhenTheItemLiesAtTheEdgeOfTheTolerance_ShouldFail()
				{
					double[] values = [1.0, 2.0, 3.0,];
					ImmutableArray<double> subject = [.. values,];
					double unexpected = 2.25;

					async Task Act()
						=> await That(subject).DoesNotHaveItem(unexpected).Within(0.25);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              does not have an item equal to {Formatter.Format(unexpected)} ± 0.25,
						              but it had item {Formatter.Format(2.0)}

						              Collection:
						              {Formatter.Format(values)}
						              """);
				}

				[Test]
				public async Task WhenTheItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					double[] values = [1.0, 2.0, 3.0,];
					ImmutableArray<double> subject = [.. values,];

					async Task Act()
						=> await That(subject).DoesNotHaveItem(2.5).Within(0.25);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableDoubleTests
			{
				[Test]
				public async Task WhenTheItemLiesAtTheEdgeOfTheTolerance_ShouldFail()
				{
					double?[] values = [1.0, null, 2.0, 3.0,];
					ImmutableArray<double?> subject = [.. values,];
					double? unexpected = 2.25;

					async Task Act()
						=> await That(subject).DoesNotHaveItem(unexpected).Within(0.25);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              does not have an item equal to {Formatter.Format(unexpected)} ± 0.25,
						              but it had item {Formatter.Format(2.0)}

						              Collection:
						              {Formatter.Format(values)}
						              """);
				}

				[Test]
				public async Task WhenTheItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					double?[] values = [1.0, null, 2.0, 3.0,];
					ImmutableArray<double?> subject = [.. values,];

					async Task Act()
						=> await That(subject).DoesNotHaveItem(2.5).Within(0.25);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class FloatTests
			{
				[Test]
				public async Task WhenTheItemLiesAtTheEdgeOfTheTolerance_ShouldFail()
				{
					float[] values = [1.0F, 2.0F, 3.0F,];
					ImmutableArray<float> subject = [.. values,];
					float unexpected = 2.25F;

					async Task Act()
						=> await That(subject).DoesNotHaveItem(unexpected).Within(0.25F);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              does not have an item equal to {Formatter.Format(unexpected)} ± 0.25,
						              but it had item {Formatter.Format(2.0F)}

						              Collection:
						              {Formatter.Format(values)}
						              """);
				}

				[Test]
				public async Task WhenTheItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					float[] values = [1.0F, 2.0F, 3.0F,];
					ImmutableArray<float> subject = [.. values,];

					async Task Act()
						=> await That(subject).DoesNotHaveItem(2.5F).Within(0.25F);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableFloatTests
			{
				[Test]
				public async Task WhenTheItemLiesAtTheEdgeOfTheTolerance_ShouldFail()
				{
					float?[] values = [1.0F, null, 2.0F, 3.0F,];
					ImmutableArray<float?> subject = [.. values,];
					float? unexpected = 2.25F;

					async Task Act()
						=> await That(subject).DoesNotHaveItem(unexpected).Within(0.25F);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              does not have an item equal to {Formatter.Format(unexpected)} ± 0.25,
						              but it had item {Formatter.Format(2.0F)}

						              Collection:
						              {Formatter.Format(values)}
						              """);
				}

				[Test]
				public async Task WhenTheItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					float?[] values = [1.0F, null, 2.0F, 3.0F,];
					ImmutableArray<float?> subject = [.. values,];

					async Task Act()
						=> await That(subject).DoesNotHaveItem(2.5F).Within(0.25F);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class TimeOnlyTests
			{
				[Test]
				public async Task WhenTheItemLiesAtTheEdgeOfTheTolerance_ShouldFail()
				{
					TimeOnly[] values = [new(13, 0), new(14, 0), new(15, 0),];
					ImmutableArray<TimeOnly> subject = [.. values,];
					TimeOnly unexpected = new(14, 1);

					async Task Act()
						=> await That(subject).DoesNotHaveItem(unexpected).Within(1.Minutes());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              does not have an item equal to {Formatter.Format(unexpected)} ± 1:00,
						              but it had item {Formatter.Format(new TimeOnly(14, 0))}

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Test]
				public async Task WhenTheItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					TimeOnly[] values = [new(13, 0), new(14, 0), new(15, 0),];
					ImmutableArray<TimeOnly> subject = [.. values,];

					async Task Act()
						=> await That(subject).DoesNotHaveItem(new TimeOnly(14, 2)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableTimeOnlyTests
			{
				[Test]
				public async Task WhenTheItemLiesAtTheEdgeOfTheTolerance_ShouldFail()
				{
					TimeOnly?[] values = [new TimeOnly(13, 0), null, new TimeOnly(14, 0), new TimeOnly(15, 0),];
					ImmutableArray<TimeOnly?> subject = [.. values,];
					TimeOnly? unexpected = new TimeOnly(14, 1);

					async Task Act()
						=> await That(subject).DoesNotHaveItem(unexpected).Within(1.Minutes());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              does not have an item equal to {Formatter.Format(unexpected)} ± 1:00,
						              but it had item {Formatter.Format(new TimeOnly(14, 0))}

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Test]
				public async Task WhenTheItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					TimeOnly?[] values = [new TimeOnly(13, 0), null, new TimeOnly(14, 0), new TimeOnly(15, 0),];
					ImmutableArray<TimeOnly?> subject = [.. values,];

					async Task Act()
						=> await That(subject).DoesNotHaveItem(new TimeOnly(14, 2)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class TimeSpanTests
			{
				[Test]
				public async Task WhenTheItemLiesAtTheEdgeOfTheTolerance_ShouldFail()
				{
					TimeSpan[] values = [new(1, 0, 0), new(2, 0, 0), new(3, 0, 0),];
					ImmutableArray<TimeSpan> subject = [.. values,];
					TimeSpan unexpected = new(2, 1, 0);

					async Task Act()
						=> await That(subject).DoesNotHaveItem(unexpected).Within(1.Minutes());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              does not have an item equal to {Formatter.Format(unexpected)} ± 1:00,
						              but it had item {Formatter.Format(new TimeSpan(2, 0, 0))}

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Test]
				public async Task WhenTheItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					TimeSpan[] values = [new(1, 0, 0), new(2, 0, 0), new(3, 0, 0),];
					ImmutableArray<TimeSpan> subject = [.. values,];

					async Task Act()
						=> await That(subject).DoesNotHaveItem(new TimeSpan(2, 2, 0)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableTimeSpanTests
			{
				[Test]
				public async Task WhenTheItemLiesAtTheEdgeOfTheTolerance_ShouldFail()
				{
					TimeSpan?[] values = [new TimeSpan(1, 0, 0), null, new TimeSpan(2, 0, 0), new TimeSpan(3, 0, 0),];
					ImmutableArray<TimeSpan?> subject = [.. values,];
					TimeSpan? unexpected = new TimeSpan(2, 1, 0);

					async Task Act()
						=> await That(subject).DoesNotHaveItem(unexpected).Within(1.Minutes());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              does not have an item equal to {Formatter.Format(unexpected)} ± 1:00,
						              but it had item {Formatter.Format(new TimeSpan(2, 0, 0))}

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Test]
				public async Task WhenTheItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					TimeSpan?[] values = [new TimeSpan(1, 0, 0), null, new TimeSpan(2, 0, 0), new TimeSpan(3, 0, 0),];
					ImmutableArray<TimeSpan?> subject = [.. values,];

					async Task Act()
						=> await That(subject).DoesNotHaveItem(new TimeSpan(2, 2, 0)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
#endif
