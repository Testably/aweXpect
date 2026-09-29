#if NET8_0_OR_GREATER
using System.Collections.Generic;
using aweXpect.Customization;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class EndsWith
	{
		public sealed class Within
		{
			public sealed class DateOnlyTests
			{
				[Fact]
				public async Task WhenAnItemOfTheSuffixLiesOutsideTheTolerance_ShouldFail()
				{
					DateOnly[] values = [new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IAsyncEnumerable<DateOnly> subject = ToAsyncEnumerable(values);
					IEnumerable<DateOnly> expected = [new DateOnly(2024, 1, 13), new DateOnly(2024, 1, 21),];

					async Task Act()
						=> await That(subject).EndsWith(expected).Within(1.Days());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              ends with expected ± 1 day,
						              but it contained item {Formatter.Format(new DateOnly(2024, 1, 11))} at index 1 instead of {Formatter.Format(new DateOnly(2024, 1, 13))}

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task WhenTheSuffixLiesWithinTheTolerance_ShouldSucceed()
				{
					DateOnly[] values = [new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IAsyncEnumerable<DateOnly> subject = ToAsyncEnumerable(values);

					async Task Act()
						=> await That(subject).EndsWith(new DateOnly(2024, 1, 12), new DateOnly(2024, 1, 21)).Within(1.Days());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenToleranceIsNotAWholeNumberOfDays_ShouldThrowArgumentOutOfRangeException()
				{
					DateOnly[] values = [new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IAsyncEnumerable<DateOnly> subject = ToAsyncEnumerable(values);

					object Act()
						=> That(subject).EndsWith(new DateOnly(2024, 1, 12), new DateOnly(2024, 1, 21)).Within(1.Days() + 1.Hours());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("tolerance").And
						.WithMessage("The tolerance must be a whole number of days.").AsPrefix()
						.Because("a date has no time of day, so the remainder is rejected as soon as it is specified");
				}
			}

			public sealed class NullableDateOnlyTests
			{
				[Fact]
				public async Task WhenAnItemOfTheSuffixLiesOutsideTheTolerance_ShouldFail()
				{
					DateOnly?[] values = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IAsyncEnumerable<DateOnly?> subject = ToAsyncEnumerable(values);
					IEnumerable<DateOnly?> expected = [new DateOnly(2024, 1, 13), new DateOnly(2024, 1, 21),];

					async Task Act()
						=> await That(subject).EndsWith(expected).Within(1.Days());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              ends with expected ± 1 day,
						              but it contained item {Formatter.Format(new DateOnly(2024, 1, 11))} at index 2 instead of {Formatter.Format(new DateOnly(2024, 1, 13))}

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task WhenTheSuffixLiesWithinTheTolerance_ShouldSucceed()
				{
					DateOnly?[] values = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IAsyncEnumerable<DateOnly?> subject = ToAsyncEnumerable(values);

					async Task Act()
						=> await That(subject).EndsWith(new DateOnly(2024, 1, 12), new DateOnly(2024, 1, 21)).Within(1.Days());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenToleranceIsNotAWholeNumberOfDays_ShouldThrowArgumentOutOfRangeException()
				{
					DateOnly?[] values = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IAsyncEnumerable<DateOnly?> subject = ToAsyncEnumerable(values);

					object Act()
						=> That(subject).EndsWith(new DateOnly(2024, 1, 12), new DateOnly(2024, 1, 21)).Within(1.Days() + 1.Hours());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("tolerance").And
						.WithMessage("The tolerance must be a whole number of days.").AsPrefix()
						.Because("a date has no time of day, so the remainder is rejected as soon as it is specified");
				}
			}

			public sealed class DateTimeTests
			{
				[Fact]
				public async Task WhenAnItemOfTheSuffixLiesOutsideTheTolerance_ShouldFail()
				{
					DateTime[] values = [new DateTime(2024, 1, 1, 13, 0, 0), new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					IAsyncEnumerable<DateTime> subject = ToAsyncEnumerable(values);
					IEnumerable<DateTime> expected = [new DateTime(2024, 1, 1, 14, 2, 0), new DateTime(2024, 1, 1, 15, 0, 0),];

					async Task Act()
						=> await That(subject).EndsWith(expected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              ends with expected ± 1:00,
						              but it contained item {Formatter.Format(new DateTime(2024, 1, 1, 14, 0, 0))} at index 1 instead of {Formatter.Format(new DateTime(2024, 1, 1, 14, 2, 0))}

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task WhenTheDefaultToleranceIsSet_ShouldApplyIt()
				{
					DateTime[] values = [new DateTime(2024, 1, 1, 13, 0, 0), new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					IAsyncEnumerable<DateTime> subject = ToAsyncEnumerable(values);

					async Task Act()
					{
						using IDisposable __ =
							Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Minutes());
						await That(subject).EndsWith(new DateTime(2024, 1, 1, 14, 1, 0), new DateTime(2024, 1, 1, 15, 0, 0));
					}

					await That(Act).DoesNotThrow()
						.Because("the items fall back to the default tolerance, as a single value does");
				}

				[Fact]
				public async Task WhenTheDefaultToleranceIsSet_ShouldMentionIt()
				{
					DateTime[] values = [new DateTime(2024, 1, 1, 13, 0, 0), new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					IAsyncEnumerable<DateTime> subject = ToAsyncEnumerable(values);
					IEnumerable<DateTime> expected = [new DateTime(2024, 1, 1, 14, 2, 0), new DateTime(2024, 1, 1, 15, 0, 0),];

					async Task Act()
					{
						using IDisposable __ =
							Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Minutes());
						await That(subject).EndsWith(expected);
					}

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              ends with expected ± 1:00,
						              but it contained item {Formatter.Format(new DateTime(2024, 1, 1, 14, 0, 0))} at index 1 instead of {Formatter.Format(new DateTime(2024, 1, 1, 14, 2, 0))}

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """)
						.Because("the applied default tolerance is part of the expectation");
				}

				[Fact]
				public async Task WhenTheSuffixLiesWithinTheTolerance_ShouldSucceed()
				{
					DateTime[] values = [new DateTime(2024, 1, 1, 13, 0, 0), new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					IAsyncEnumerable<DateTime> subject = ToAsyncEnumerable(values);

					async Task Act()
						=> await That(subject).EndsWith(new DateTime(2024, 1, 1, 14, 1, 0), new DateTime(2024, 1, 1, 15, 0, 0)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableDateTimeTests
			{
				[Fact]
				public async Task WhenAnItemOfTheSuffixLiesOutsideTheTolerance_ShouldFail()
				{
					DateTime?[] values = [new DateTime(2024, 1, 1, 13, 0, 0), null, new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					IAsyncEnumerable<DateTime?> subject = ToAsyncEnumerable(values);
					IEnumerable<DateTime?> expected = [new DateTime(2024, 1, 1, 14, 2, 0), new DateTime(2024, 1, 1, 15, 0, 0),];

					async Task Act()
						=> await That(subject).EndsWith(expected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              ends with expected ± 1:00,
						              but it contained item {Formatter.Format(new DateTime(2024, 1, 1, 14, 0, 0))} at index 2 instead of {Formatter.Format(new DateTime(2024, 1, 1, 14, 2, 0))}

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task WhenTheSuffixLiesWithinTheTolerance_ShouldSucceed()
				{
					DateTime?[] values = [new DateTime(2024, 1, 1, 13, 0, 0), null, new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					IAsyncEnumerable<DateTime?> subject = ToAsyncEnumerable(values);

					async Task Act()
						=> await That(subject).EndsWith(new DateTime(2024, 1, 1, 14, 1, 0), new DateTime(2024, 1, 1, 15, 0, 0)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class DateTimeOffsetTests
			{
				[Fact]
				public async Task WhenAnItemOfTheSuffixLiesOutsideTheTolerance_ShouldFail()
				{
					DateTimeOffset[] values = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					IAsyncEnumerable<DateTimeOffset> subject = ToAsyncEnumerable(values);
					IEnumerable<DateTimeOffset> expected = [new DateTimeOffset(2024, 1, 1, 14, 2, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];

					async Task Act()
						=> await That(subject).EndsWith(expected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              ends with expected ± 1:00,
						              but it contained item {Formatter.Format(new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero))} at index 1 instead of {Formatter.Format(new DateTimeOffset(2024, 1, 1, 14, 2, 0, TimeSpan.Zero))}

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task WhenTheSuffixLiesWithinTheTolerance_ShouldSucceed()
				{
					DateTimeOffset[] values = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					IAsyncEnumerable<DateTimeOffset> subject = ToAsyncEnumerable(values);

					async Task Act()
						=> await That(subject).EndsWith(new DateTimeOffset(2024, 1, 1, 14, 1, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableDateTimeOffsetTests
			{
				[Fact]
				public async Task WhenAnItemOfTheSuffixLiesOutsideTheTolerance_ShouldFail()
				{
					DateTimeOffset?[] values = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), null, new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					IAsyncEnumerable<DateTimeOffset?> subject = ToAsyncEnumerable(values);
					IEnumerable<DateTimeOffset?> expected = [new DateTimeOffset(2024, 1, 1, 14, 2, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];

					async Task Act()
						=> await That(subject).EndsWith(expected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              ends with expected ± 1:00,
						              but it contained item {Formatter.Format(new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero))} at index 2 instead of {Formatter.Format(new DateTimeOffset(2024, 1, 1, 14, 2, 0, TimeSpan.Zero))}

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task WhenTheSuffixLiesWithinTheTolerance_ShouldSucceed()
				{
					DateTimeOffset?[] values = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), null, new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					IAsyncEnumerable<DateTimeOffset?> subject = ToAsyncEnumerable(values);

					async Task Act()
						=> await That(subject).EndsWith(new DateTimeOffset(2024, 1, 1, 14, 1, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class DecimalTests
			{
				[Fact]
				public async Task WhenAnItemOfTheSuffixLiesOutsideTheTolerance_ShouldFail()
				{
					decimal[] values = [1.0m, 2.0m, 3.0m,];
					IAsyncEnumerable<decimal> subject = ToAsyncEnumerable(values);
					IEnumerable<decimal> expected = [2.5m, 3.0m,];

					async Task Act()
						=> await That(subject).EndsWith(expected).Within(0.25m);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              ends with expected ± 0.25,
						              but it contained item {Formatter.Format(2.0m)} at index 1 instead of {Formatter.Format(2.5m)}

						              Collection:
						              {Formatter.Format(values)}
						              """);
				}

				[Fact]
				public async Task WhenTheSuffixLiesWithinTheTolerance_ShouldSucceed()
				{
					decimal[] values = [1.0m, 2.0m, 3.0m,];
					IAsyncEnumerable<decimal> subject = ToAsyncEnumerable(values);

					async Task Act()
						=> await That(subject).EndsWith(2.25m, 3.0m).Within(0.25m);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableDecimalTests
			{
				[Fact]
				public async Task WhenAnItemOfTheSuffixLiesOutsideTheTolerance_ShouldFail()
				{
					decimal?[] values = [1.0m, null, 2.0m, 3.0m,];
					IAsyncEnumerable<decimal?> subject = ToAsyncEnumerable(values);
					IEnumerable<decimal?> expected = [2.5m, 3.0m,];

					async Task Act()
						=> await That(subject).EndsWith(expected).Within(0.25m);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              ends with expected ± 0.25,
						              but it contained item {Formatter.Format(2.0m)} at index 2 instead of {Formatter.Format(2.5m)}

						              Collection:
						              {Formatter.Format(values)}
						              """);
				}

				[Fact]
				public async Task WhenTheSuffixLiesWithinTheTolerance_ShouldSucceed()
				{
					decimal?[] values = [1.0m, null, 2.0m, 3.0m,];
					IAsyncEnumerable<decimal?> subject = ToAsyncEnumerable(values);

					async Task Act()
						=> await That(subject).EndsWith(2.25m, 3.0m).Within(0.25m);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class DoubleTests
			{
				[Fact]
				public async Task WhenAnItemOfTheSuffixLiesOutsideTheTolerance_ShouldFail()
				{
					double[] values = [1.0, 2.0, 3.0,];
					IAsyncEnumerable<double> subject = ToAsyncEnumerable(values);
					IEnumerable<double> expected = [2.5, 3.0,];

					async Task Act()
						=> await That(subject).EndsWith(expected).Within(0.25);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              ends with expected ± 0.25,
						              but it contained item {Formatter.Format(2.0)} at index 1 instead of {Formatter.Format(2.5)}

						              Collection:
						              {Formatter.Format(values)}
						              """);
				}

				[Theory]
				[InlineData(false)]
				[InlineData(true)]
				public async Task WhenCombinedWithUsing_ShouldThrowInvalidOperationException(bool negated)
				{
					double[] values = [1.0, 2.0, 3.0,];
					IAsyncEnumerable<double> subject = ToAsyncEnumerable(values);

					async Task Act()
					{
						if (negated)
						{
							await That(subject).DoesNotEndWith(2.25, 3.0).Within(0.25).Using(new AllEqualComparer());
						}
						else
						{
							await That(subject).EndsWith(2.25, 3.0).Within(0.25).Using(new AllEqualComparer());
						}
					}

					await That(Act).Throws<InvalidOperationException>()
						.WithMessage("Using cannot be combined with Within.")
						.Because("the comparer would silently replace the tolerance");
				}

				[Fact]
				public async Task WhenTheSuffixLiesWithinTheTolerance_ShouldSucceed()
				{
					double[] values = [1.0, 2.0, 3.0,];
					IAsyncEnumerable<double> subject = ToAsyncEnumerable(values);

					async Task Act()
						=> await That(subject).EndsWith(2.25, 3.0).Within(0.25);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableDoubleTests
			{
				[Fact]
				public async Task WhenAnItemOfTheSuffixLiesOutsideTheTolerance_ShouldFail()
				{
					double?[] values = [1.0, null, 2.0, 3.0,];
					IAsyncEnumerable<double?> subject = ToAsyncEnumerable(values);
					IEnumerable<double?> expected = [2.5, 3.0,];

					async Task Act()
						=> await That(subject).EndsWith(expected).Within(0.25);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              ends with expected ± 0.25,
						              but it contained item {Formatter.Format(2.0)} at index 2 instead of {Formatter.Format(2.5)}

						              Collection:
						              {Formatter.Format(values)}
						              """);
				}

				[Fact]
				public async Task WhenTheSuffixLiesWithinTheTolerance_ShouldSucceed()
				{
					double?[] values = [1.0, null, 2.0, 3.0,];
					IAsyncEnumerable<double?> subject = ToAsyncEnumerable(values);

					async Task Act()
						=> await That(subject).EndsWith(2.25, 3.0).Within(0.25);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class FloatTests
			{
				[Fact]
				public async Task WhenAnItemOfTheSuffixLiesOutsideTheTolerance_ShouldFail()
				{
					float[] values = [1.0F, 2.0F, 3.0F,];
					IAsyncEnumerable<float> subject = ToAsyncEnumerable(values);
					IEnumerable<float> expected = [2.5F, 3.0F,];

					async Task Act()
						=> await That(subject).EndsWith(expected).Within(0.25F);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              ends with expected ± 0.25,
						              but it contained item {Formatter.Format(2.0F)} at index 1 instead of {Formatter.Format(2.5F)}

						              Collection:
						              {Formatter.Format(values)}
						              """);
				}

				[Fact]
				public async Task WhenTheSuffixLiesWithinTheTolerance_ShouldSucceed()
				{
					float[] values = [1.0F, 2.0F, 3.0F,];
					IAsyncEnumerable<float> subject = ToAsyncEnumerable(values);

					async Task Act()
						=> await That(subject).EndsWith(2.25F, 3.0F).Within(0.25F);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableFloatTests
			{
				[Fact]
				public async Task WhenAnItemOfTheSuffixLiesOutsideTheTolerance_ShouldFail()
				{
					float?[] values = [1.0F, null, 2.0F, 3.0F,];
					IAsyncEnumerable<float?> subject = ToAsyncEnumerable(values);
					IEnumerable<float?> expected = [2.5F, 3.0F,];

					async Task Act()
						=> await That(subject).EndsWith(expected).Within(0.25F);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              ends with expected ± 0.25,
						              but it contained item {Formatter.Format(2.0F)} at index 2 instead of {Formatter.Format(2.5F)}

						              Collection:
						              {Formatter.Format(values)}
						              """);
				}

				[Fact]
				public async Task WhenTheSuffixLiesWithinTheTolerance_ShouldSucceed()
				{
					float?[] values = [1.0F, null, 2.0F, 3.0F,];
					IAsyncEnumerable<float?> subject = ToAsyncEnumerable(values);

					async Task Act()
						=> await That(subject).EndsWith(2.25F, 3.0F).Within(0.25F);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class TimeOnlyTests
			{
				[Fact]
				public async Task WhenAnItemLiesAcrossMidnight_ShouldUseTheShorterDistance()
				{
					TimeOnly[] values = [new TimeOnly(22, 0), new TimeOnly(23, 59, 30), new TimeOnly(2, 0),];
					IAsyncEnumerable<TimeOnly> subject = ToAsyncEnumerable(values);

					async Task Act()
						=> await That(subject).EndsWith(new TimeOnly(0, 0, 30), new TimeOnly(2, 0)).Within(1.Minutes());

					await That(Act).DoesNotThrow()
						.Because("the times are compared on the clock face, where 23:59:30 and 00:00:30 are one minute apart");
				}

				[Fact]
				public async Task WhenAnItemOfTheSuffixLiesOutsideTheTolerance_ShouldFail()
				{
					TimeOnly[] values = [new TimeOnly(13, 0), new TimeOnly(14, 0), new TimeOnly(15, 0),];
					IAsyncEnumerable<TimeOnly> subject = ToAsyncEnumerable(values);
					IEnumerable<TimeOnly> expected = [new TimeOnly(14, 2), new TimeOnly(15, 0),];

					async Task Act()
						=> await That(subject).EndsWith(expected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              ends with expected ± 1:00,
						              but it contained item {Formatter.Format(new TimeOnly(14, 0))} at index 1 instead of {Formatter.Format(new TimeOnly(14, 2))}

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task WhenTheSuffixLiesWithinTheTolerance_ShouldSucceed()
				{
					TimeOnly[] values = [new TimeOnly(13, 0), new TimeOnly(14, 0), new TimeOnly(15, 0),];
					IAsyncEnumerable<TimeOnly> subject = ToAsyncEnumerable(values);

					async Task Act()
						=> await That(subject).EndsWith(new TimeOnly(14, 1), new TimeOnly(15, 0)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableTimeOnlyTests
			{
				[Fact]
				public async Task WhenAnItemLiesAcrossMidnight_ShouldUseTheShorterDistance()
				{
					TimeOnly?[] values = [new TimeOnly(22, 0), null, new TimeOnly(23, 59, 30), new TimeOnly(2, 0),];
					IAsyncEnumerable<TimeOnly?> subject = ToAsyncEnumerable(values);

					async Task Act()
						=> await That(subject).EndsWith(new TimeOnly(0, 0, 30), new TimeOnly(2, 0)).Within(1.Minutes());

					await That(Act).DoesNotThrow()
						.Because("the times are compared on the clock face, where 23:59:30 and 00:00:30 are one minute apart");
				}

				[Fact]
				public async Task WhenAnItemOfTheSuffixLiesOutsideTheTolerance_ShouldFail()
				{
					TimeOnly?[] values = [new TimeOnly(13, 0), null, new TimeOnly(14, 0), new TimeOnly(15, 0),];
					IAsyncEnumerable<TimeOnly?> subject = ToAsyncEnumerable(values);
					IEnumerable<TimeOnly?> expected = [new TimeOnly(14, 2), new TimeOnly(15, 0),];

					async Task Act()
						=> await That(subject).EndsWith(expected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              ends with expected ± 1:00,
						              but it contained item {Formatter.Format(new TimeOnly(14, 0))} at index 2 instead of {Formatter.Format(new TimeOnly(14, 2))}

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task WhenTheSuffixLiesWithinTheTolerance_ShouldSucceed()
				{
					TimeOnly?[] values = [new TimeOnly(13, 0), null, new TimeOnly(14, 0), new TimeOnly(15, 0),];
					IAsyncEnumerable<TimeOnly?> subject = ToAsyncEnumerable(values);

					async Task Act()
						=> await That(subject).EndsWith(new TimeOnly(14, 1), new TimeOnly(15, 0)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class TimeSpanTests
			{
				[Fact]
				public async Task WhenAnItemOfTheSuffixLiesOutsideTheTolerance_ShouldFail()
				{
					TimeSpan[] values = [new TimeSpan(1, 0, 0), new TimeSpan(2, 0, 0), new TimeSpan(3, 0, 0),];
					IAsyncEnumerable<TimeSpan> subject = ToAsyncEnumerable(values);
					IEnumerable<TimeSpan> expected = [new TimeSpan(2, 2, 0), new TimeSpan(3, 0, 0),];

					async Task Act()
						=> await That(subject).EndsWith(expected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              ends with expected ± 1:00,
						              but it contained item {Formatter.Format(new TimeSpan(2, 0, 0))} at index 1 instead of {Formatter.Format(new TimeSpan(2, 2, 0))}

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task WhenTheSuffixLiesWithinTheTolerance_ShouldSucceed()
				{
					TimeSpan[] values = [new TimeSpan(1, 0, 0), new TimeSpan(2, 0, 0), new TimeSpan(3, 0, 0),];
					IAsyncEnumerable<TimeSpan> subject = ToAsyncEnumerable(values);

					async Task Act()
						=> await That(subject).EndsWith(new TimeSpan(2, 1, 0), new TimeSpan(3, 0, 0)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableTimeSpanTests
			{
				[Fact]
				public async Task WhenAnItemOfTheSuffixLiesOutsideTheTolerance_ShouldFail()
				{
					TimeSpan?[] values = [new TimeSpan(1, 0, 0), null, new TimeSpan(2, 0, 0), new TimeSpan(3, 0, 0),];
					IAsyncEnumerable<TimeSpan?> subject = ToAsyncEnumerable(values);
					IEnumerable<TimeSpan?> expected = [new TimeSpan(2, 2, 0), new TimeSpan(3, 0, 0),];

					async Task Act()
						=> await That(subject).EndsWith(expected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              ends with expected ± 1:00,
						              but it contained item {Formatter.Format(new TimeSpan(2, 0, 0))} at index 2 instead of {Formatter.Format(new TimeSpan(2, 2, 0))}

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task WhenTheSuffixLiesWithinTheTolerance_ShouldSucceed()
				{
					TimeSpan?[] values = [new TimeSpan(1, 0, 0), null, new TimeSpan(2, 0, 0), new TimeSpan(3, 0, 0),];
					IAsyncEnumerable<TimeSpan?> subject = ToAsyncEnumerable(values);

					async Task Act()
						=> await That(subject).EndsWith(new TimeSpan(2, 1, 0), new TimeSpan(3, 0, 0)).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}
		}
	}

	public sealed partial class DoesNotEndWith
	{
		public sealed class Within
		{
			public sealed class DateOnlyTests
			{
				[Fact]
				public async Task WhenAnItemOfTheSuffixLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateOnly[] values = [new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IAsyncEnumerable<DateOnly> subject = ToAsyncEnumerable(values);
					IEnumerable<DateOnly> unexpected = [new DateOnly(2024, 1, 13), new DateOnly(2024, 1, 21),];

					async Task Act()
						=> await That(subject).DoesNotEndWith(unexpected).Within(1.Days());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenTheSuffixLiesWithinTheTolerance_ShouldFail()
				{
					DateOnly[] values = [new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IAsyncEnumerable<DateOnly> subject = ToAsyncEnumerable(values);
					DateOnly[] unexpected = [new DateOnly(2024, 1, 12), new DateOnly(2024, 1, 21),];

					async Task Act()
						=> await That(subject).DoesNotEndWith(unexpected).Within(1.Days());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not end with {Formatter.Format(unexpected)} ± 1 day,
						              but it did end with {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}
			}

			public sealed class NullableDateOnlyTests
			{
				[Fact]
				public async Task WhenAnItemOfTheSuffixLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateOnly?[] values = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IAsyncEnumerable<DateOnly?> subject = ToAsyncEnumerable(values);
					IEnumerable<DateOnly?> unexpected = [new DateOnly(2024, 1, 13), new DateOnly(2024, 1, 21),];

					async Task Act()
						=> await That(subject).DoesNotEndWith(unexpected).Within(1.Days());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenTheSuffixLiesWithinTheTolerance_ShouldFail()
				{
					DateOnly?[] values = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					IAsyncEnumerable<DateOnly?> subject = ToAsyncEnumerable(values);
					DateOnly?[] unexpected = [new DateOnly(2024, 1, 12), new DateOnly(2024, 1, 21),];

					async Task Act()
						=> await That(subject).DoesNotEndWith(unexpected).Within(1.Days());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not end with {Formatter.Format(unexpected)} ± 1 day,
						              but it did end with {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}
			}

			public sealed class DateTimeTests
			{
				[Fact]
				public async Task WhenAnItemOfTheSuffixLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateTime[] values = [new DateTime(2024, 1, 1, 13, 0, 0), new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					IAsyncEnumerable<DateTime> subject = ToAsyncEnumerable(values);
					IEnumerable<DateTime> unexpected = [new DateTime(2024, 1, 1, 14, 2, 0), new DateTime(2024, 1, 1, 15, 0, 0),];

					async Task Act()
						=> await That(subject).DoesNotEndWith(unexpected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenTheSuffixLiesWithinTheTolerance_ShouldFail()
				{
					DateTime[] values = [new DateTime(2024, 1, 1, 13, 0, 0), new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					IAsyncEnumerable<DateTime> subject = ToAsyncEnumerable(values);
					DateTime[] unexpected = [new DateTime(2024, 1, 1, 14, 1, 0), new DateTime(2024, 1, 1, 15, 0, 0),];

					async Task Act()
						=> await That(subject).DoesNotEndWith(unexpected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not end with {Formatter.Format(unexpected)} ± 1:00,
						              but it did end with {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}
			}

			public sealed class NullableDateTimeTests
			{
				[Fact]
				public async Task WhenAnItemOfTheSuffixLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateTime?[] values = [new DateTime(2024, 1, 1, 13, 0, 0), null, new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					IAsyncEnumerable<DateTime?> subject = ToAsyncEnumerable(values);
					IEnumerable<DateTime?> unexpected = [new DateTime(2024, 1, 1, 14, 2, 0), new DateTime(2024, 1, 1, 15, 0, 0),];

					async Task Act()
						=> await That(subject).DoesNotEndWith(unexpected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenTheSuffixLiesWithinTheTolerance_ShouldFail()
				{
					DateTime?[] values = [new DateTime(2024, 1, 1, 13, 0, 0), null, new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					IAsyncEnumerable<DateTime?> subject = ToAsyncEnumerable(values);
					DateTime?[] unexpected = [new DateTime(2024, 1, 1, 14, 1, 0), new DateTime(2024, 1, 1, 15, 0, 0),];

					async Task Act()
						=> await That(subject).DoesNotEndWith(unexpected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not end with {Formatter.Format(unexpected)} ± 1:00,
						              but it did end with {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}
			}

			public sealed class DateTimeOffsetTests
			{
				[Fact]
				public async Task WhenAnItemOfTheSuffixLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateTimeOffset[] values = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					IAsyncEnumerable<DateTimeOffset> subject = ToAsyncEnumerable(values);
					IEnumerable<DateTimeOffset> unexpected = [new DateTimeOffset(2024, 1, 1, 14, 2, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];

					async Task Act()
						=> await That(subject).DoesNotEndWith(unexpected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenTheSuffixLiesWithinTheTolerance_ShouldFail()
				{
					DateTimeOffset[] values = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					IAsyncEnumerable<DateTimeOffset> subject = ToAsyncEnumerable(values);
					DateTimeOffset[] unexpected = [new DateTimeOffset(2024, 1, 1, 14, 1, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];

					async Task Act()
						=> await That(subject).DoesNotEndWith(unexpected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not end with {Formatter.Format(unexpected)} ± 1:00,
						              but it did end with {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}
			}

			public sealed class NullableDateTimeOffsetTests
			{
				[Fact]
				public async Task WhenAnItemOfTheSuffixLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateTimeOffset?[] values = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), null, new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					IAsyncEnumerable<DateTimeOffset?> subject = ToAsyncEnumerable(values);
					IEnumerable<DateTimeOffset?> unexpected = [new DateTimeOffset(2024, 1, 1, 14, 2, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];

					async Task Act()
						=> await That(subject).DoesNotEndWith(unexpected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenTheSuffixLiesWithinTheTolerance_ShouldFail()
				{
					DateTimeOffset?[] values = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), null, new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					IAsyncEnumerable<DateTimeOffset?> subject = ToAsyncEnumerable(values);
					DateTimeOffset?[] unexpected = [new DateTimeOffset(2024, 1, 1, 14, 1, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];

					async Task Act()
						=> await That(subject).DoesNotEndWith(unexpected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not end with {Formatter.Format(unexpected)} ± 1:00,
						              but it did end with {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}
			}

			public sealed class DecimalTests
			{
				[Fact]
				public async Task WhenAnItemOfTheSuffixLiesOutsideTheTolerance_ShouldSucceed()
				{
					decimal[] values = [1.0m, 2.0m, 3.0m,];
					IAsyncEnumerable<decimal> subject = ToAsyncEnumerable(values);
					IEnumerable<decimal> unexpected = [2.5m, 3.0m,];

					async Task Act()
						=> await That(subject).DoesNotEndWith(unexpected).Within(0.25m);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenTheSuffixLiesWithinTheTolerance_ShouldFail()
				{
					decimal[] values = [1.0m, 2.0m, 3.0m,];
					IAsyncEnumerable<decimal> subject = ToAsyncEnumerable(values);
					decimal[] unexpected = [2.25m, 3.0m,];

					async Task Act()
						=> await That(subject).DoesNotEndWith(unexpected).Within(0.25m);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not end with {Formatter.Format(unexpected)} ± 0.25,
						              but it did end with {Formatter.Format(values, FormattingOptions.SingleLine)}
						              """);
				}
			}

			public sealed class NullableDecimalTests
			{
				[Fact]
				public async Task WhenAnItemOfTheSuffixLiesOutsideTheTolerance_ShouldSucceed()
				{
					decimal?[] values = [1.0m, null, 2.0m, 3.0m,];
					IAsyncEnumerable<decimal?> subject = ToAsyncEnumerable(values);
					IEnumerable<decimal?> unexpected = [2.5m, 3.0m,];

					async Task Act()
						=> await That(subject).DoesNotEndWith(unexpected).Within(0.25m);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenTheSuffixLiesWithinTheTolerance_ShouldFail()
				{
					decimal?[] values = [1.0m, null, 2.0m, 3.0m,];
					IAsyncEnumerable<decimal?> subject = ToAsyncEnumerable(values);
					decimal?[] unexpected = [2.25m, 3.0m,];

					async Task Act()
						=> await That(subject).DoesNotEndWith(unexpected).Within(0.25m);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not end with {Formatter.Format(unexpected)} ± 0.25,
						              but it did end with {Formatter.Format(values, FormattingOptions.SingleLine)}
						              """);
				}
			}

			public sealed class DoubleTests
			{
				[Fact]
				public async Task WhenAnItemOfTheSuffixLiesOutsideTheTolerance_ShouldSucceed()
				{
					double[] values = [1.0, 2.0, 3.0,];
					IAsyncEnumerable<double> subject = ToAsyncEnumerable(values);
					IEnumerable<double> unexpected = [2.5, 3.0,];

					async Task Act()
						=> await That(subject).DoesNotEndWith(unexpected).Within(0.25);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenTheSuffixLiesWithinTheTolerance_ShouldFail()
				{
					double[] values = [1.0, 2.0, 3.0,];
					IAsyncEnumerable<double> subject = ToAsyncEnumerable(values);
					double[] unexpected = [2.25, 3.0,];

					async Task Act()
						=> await That(subject).DoesNotEndWith(unexpected).Within(0.25);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not end with {Formatter.Format(unexpected)} ± 0.25,
						              but it did end with {Formatter.Format(values, FormattingOptions.SingleLine)}
						              """);
				}
			}

			public sealed class NullableDoubleTests
			{
				[Fact]
				public async Task WhenAnItemOfTheSuffixLiesOutsideTheTolerance_ShouldSucceed()
				{
					double?[] values = [1.0, null, 2.0, 3.0,];
					IAsyncEnumerable<double?> subject = ToAsyncEnumerable(values);
					IEnumerable<double?> unexpected = [2.5, 3.0,];

					async Task Act()
						=> await That(subject).DoesNotEndWith(unexpected).Within(0.25);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenTheSuffixLiesWithinTheTolerance_ShouldFail()
				{
					double?[] values = [1.0, null, 2.0, 3.0,];
					IAsyncEnumerable<double?> subject = ToAsyncEnumerable(values);
					double?[] unexpected = [2.25, 3.0,];

					async Task Act()
						=> await That(subject).DoesNotEndWith(unexpected).Within(0.25);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not end with {Formatter.Format(unexpected)} ± 0.25,
						              but it did end with {Formatter.Format(values, FormattingOptions.SingleLine)}
						              """);
				}
			}

			public sealed class FloatTests
			{
				[Fact]
				public async Task WhenAnItemOfTheSuffixLiesOutsideTheTolerance_ShouldSucceed()
				{
					float[] values = [1.0F, 2.0F, 3.0F,];
					IAsyncEnumerable<float> subject = ToAsyncEnumerable(values);
					IEnumerable<float> unexpected = [2.5F, 3.0F,];

					async Task Act()
						=> await That(subject).DoesNotEndWith(unexpected).Within(0.25F);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenTheSuffixLiesWithinTheTolerance_ShouldFail()
				{
					float[] values = [1.0F, 2.0F, 3.0F,];
					IAsyncEnumerable<float> subject = ToAsyncEnumerable(values);
					float[] unexpected = [2.25F, 3.0F,];

					async Task Act()
						=> await That(subject).DoesNotEndWith(unexpected).Within(0.25F);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not end with {Formatter.Format(unexpected)} ± 0.25,
						              but it did end with {Formatter.Format(values, FormattingOptions.SingleLine)}
						              """);
				}
			}

			public sealed class NullableFloatTests
			{
				[Fact]
				public async Task WhenAnItemOfTheSuffixLiesOutsideTheTolerance_ShouldSucceed()
				{
					float?[] values = [1.0F, null, 2.0F, 3.0F,];
					IAsyncEnumerable<float?> subject = ToAsyncEnumerable(values);
					IEnumerable<float?> unexpected = [2.5F, 3.0F,];

					async Task Act()
						=> await That(subject).DoesNotEndWith(unexpected).Within(0.25F);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenTheSuffixLiesWithinTheTolerance_ShouldFail()
				{
					float?[] values = [1.0F, null, 2.0F, 3.0F,];
					IAsyncEnumerable<float?> subject = ToAsyncEnumerable(values);
					float?[] unexpected = [2.25F, 3.0F,];

					async Task Act()
						=> await That(subject).DoesNotEndWith(unexpected).Within(0.25F);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not end with {Formatter.Format(unexpected)} ± 0.25,
						              but it did end with {Formatter.Format(values, FormattingOptions.SingleLine)}
						              """);
				}
			}

			public sealed class TimeOnlyTests
			{
				[Fact]
				public async Task WhenAnItemOfTheSuffixLiesOutsideTheTolerance_ShouldSucceed()
				{
					TimeOnly[] values = [new TimeOnly(13, 0), new TimeOnly(14, 0), new TimeOnly(15, 0),];
					IAsyncEnumerable<TimeOnly> subject = ToAsyncEnumerable(values);
					IEnumerable<TimeOnly> unexpected = [new TimeOnly(14, 2), new TimeOnly(15, 0),];

					async Task Act()
						=> await That(subject).DoesNotEndWith(unexpected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenTheSuffixLiesWithinTheTolerance_ShouldFail()
				{
					TimeOnly[] values = [new TimeOnly(13, 0), new TimeOnly(14, 0), new TimeOnly(15, 0),];
					IAsyncEnumerable<TimeOnly> subject = ToAsyncEnumerable(values);
					TimeOnly[] unexpected = [new TimeOnly(14, 1), new TimeOnly(15, 0),];

					async Task Act()
						=> await That(subject).DoesNotEndWith(unexpected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not end with {Formatter.Format(unexpected)} ± 1:00,
						              but it did end with {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}
			}

			public sealed class NullableTimeOnlyTests
			{
				[Fact]
				public async Task WhenAnItemOfTheSuffixLiesOutsideTheTolerance_ShouldSucceed()
				{
					TimeOnly?[] values = [new TimeOnly(13, 0), null, new TimeOnly(14, 0), new TimeOnly(15, 0),];
					IAsyncEnumerable<TimeOnly?> subject = ToAsyncEnumerable(values);
					IEnumerable<TimeOnly?> unexpected = [new TimeOnly(14, 2), new TimeOnly(15, 0),];

					async Task Act()
						=> await That(subject).DoesNotEndWith(unexpected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenTheSuffixLiesWithinTheTolerance_ShouldFail()
				{
					TimeOnly?[] values = [new TimeOnly(13, 0), null, new TimeOnly(14, 0), new TimeOnly(15, 0),];
					IAsyncEnumerable<TimeOnly?> subject = ToAsyncEnumerable(values);
					TimeOnly?[] unexpected = [new TimeOnly(14, 1), new TimeOnly(15, 0),];

					async Task Act()
						=> await That(subject).DoesNotEndWith(unexpected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not end with {Formatter.Format(unexpected)} ± 1:00,
						              but it did end with {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}
			}

			public sealed class TimeSpanTests
			{
				[Fact]
				public async Task WhenAnItemOfTheSuffixLiesOutsideTheTolerance_ShouldSucceed()
				{
					TimeSpan[] values = [new TimeSpan(1, 0, 0), new TimeSpan(2, 0, 0), new TimeSpan(3, 0, 0),];
					IAsyncEnumerable<TimeSpan> subject = ToAsyncEnumerable(values);
					IEnumerable<TimeSpan> unexpected = [new TimeSpan(2, 2, 0), new TimeSpan(3, 0, 0),];

					async Task Act()
						=> await That(subject).DoesNotEndWith(unexpected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenTheSuffixLiesWithinTheTolerance_ShouldFail()
				{
					TimeSpan[] values = [new TimeSpan(1, 0, 0), new TimeSpan(2, 0, 0), new TimeSpan(3, 0, 0),];
					IAsyncEnumerable<TimeSpan> subject = ToAsyncEnumerable(values);
					TimeSpan[] unexpected = [new TimeSpan(2, 1, 0), new TimeSpan(3, 0, 0),];

					async Task Act()
						=> await That(subject).DoesNotEndWith(unexpected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not end with {Formatter.Format(unexpected)} ± 1:00,
						              but it did end with {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}
			}

			public sealed class NullableTimeSpanTests
			{
				[Fact]
				public async Task WhenAnItemOfTheSuffixLiesOutsideTheTolerance_ShouldSucceed()
				{
					TimeSpan?[] values = [new TimeSpan(1, 0, 0), null, new TimeSpan(2, 0, 0), new TimeSpan(3, 0, 0),];
					IAsyncEnumerable<TimeSpan?> subject = ToAsyncEnumerable(values);
					IEnumerable<TimeSpan?> unexpected = [new TimeSpan(2, 2, 0), new TimeSpan(3, 0, 0),];

					async Task Act()
						=> await That(subject).DoesNotEndWith(unexpected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenTheSuffixLiesWithinTheTolerance_ShouldFail()
				{
					TimeSpan?[] values = [new TimeSpan(1, 0, 0), null, new TimeSpan(2, 0, 0), new TimeSpan(3, 0, 0),];
					IAsyncEnumerable<TimeSpan?> subject = ToAsyncEnumerable(values);
					TimeSpan?[] unexpected = [new TimeSpan(2, 1, 0), new TimeSpan(3, 0, 0),];

					async Task Act()
						=> await That(subject).DoesNotEndWith(unexpected).Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not end with {Formatter.Format(unexpected)} ± 1:00,
						              but it did end with {Formatter.Format(values, FormattingOptions.MultipleLines)}
						              """);
				}
			}
		}
	}
}
#endif
