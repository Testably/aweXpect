#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Collections.Immutable;
using aweXpect.Customization;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsContainedIn
	{
		public sealed class ImmutableWithin
		{
			public sealed class DateOnlyTests
			{
				[Test]
				public async Task WhenAnItemLiesOutsideTheTolerance_ShouldFail()
				{
					DateOnly[] values = [new(2024, 1, 1), new(2024, 1, 11), new(2024, 1, 21),];
					ImmutableArray<DateOnly> subject = [.. values,];
					IEnumerable<DateOnly> expected = [new(2024, 1, 1), new(2024, 1, 13), new(2024, 1, 21),];

					async Task Act()
						=> await That(subject).IsContainedIn(expected).Within(1.Days());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is contained in collection expected ± 1 day in order and contiguous,
						             but it contained item 2024-01-11 at index 1 that was not expected

						             Collection:
						             [
						               2024-01-01,
						               2024-01-11,
						               2024-01-21
						             ]

						             Expected:
						             [
						               2024-01-01,
						               2024-01-13,
						               2024-01-21
						             ]
						             """);
				}

				[Test]
				public async Task WhenEachItemLiesWithinTheTolerance_ShouldSucceed()
				{
					DateOnly[] values = [new(2024, 1, 1), new(2024, 1, 11), new(2024, 1, 21),];
					ImmutableArray<DateOnly> subject = [.. values,];
					IEnumerable<DateOnly> expected = [new(2024, 1, 1), new(2024, 1, 12), new(2024, 1, 21),];

					async Task Act()
						=> await That(subject).IsContainedIn(expected).Within(1.Days());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenToleranceIsNotAWholeNumberOfDays_ShouldThrowArgumentOutOfRangeException()
				{
					DateOnly[] values = [new(2024, 1, 1), new(2024, 1, 11), new(2024, 1, 21),];
					ImmutableArray<DateOnly> subject = [.. values,];
					IEnumerable<DateOnly> expected = [new(2024, 1, 1), new(2024, 1, 12), new(2024, 1, 21),];

					object Act()
						=> That(subject).IsContainedIn(expected).Within(1.Days() + 1.Hours());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("tolerance").And
						.WithMessage("The tolerance must be a whole number of days.").AsPrefix()
						.Because("a date has no time of day, so the remainder is rejected as soon as it is specified");
				}
			}

			public sealed class NullableDateOnlyTests
			{
				[Test]
				public async Task WhenAnItemLiesOutsideTheTolerance_ShouldFail()
				{
					DateOnly?[] values = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					ImmutableArray<DateOnly?> subject = [.. values,];
					IEnumerable<DateOnly?> expected = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 13), new DateOnly(2024, 1, 21),];

					async Task Act()
						=> await That(subject).IsContainedIn(expected).Within(1.Days());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is contained in collection expected ± 1 day in order and contiguous,
						             but it contained item 2024-01-11 at index 2 that was not expected

						             Collection:
						             [
						               2024-01-01,
						               <null>,
						               2024-01-11,
						               2024-01-21
						             ]

						             Expected:
						             [
						               2024-01-01,
						               <null>,
						               2024-01-13,
						               2024-01-21
						             ]
						             """);
				}

				[Test]
				public async Task WhenEachItemLiesWithinTheTolerance_ShouldSucceed()
				{
					DateOnly?[] values = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					ImmutableArray<DateOnly?> subject = [.. values,];
					IEnumerable<DateOnly?> expected = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 12), new DateOnly(2024, 1, 21),];

					async Task Act()
						=> await That(subject).IsContainedIn(expected).Within(1.Days());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenToleranceIsNotAWholeNumberOfDays_ShouldThrowArgumentOutOfRangeException()
				{
					DateOnly?[] values = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					ImmutableArray<DateOnly?> subject = [.. values,];
					IEnumerable<DateOnly?> expected = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 12), new DateOnly(2024, 1, 21),];

					object Act()
						=> That(subject).IsContainedIn(expected).Within(1.Days() + 1.Hours());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("tolerance").And
						.WithMessage("The tolerance must be a whole number of days.").AsPrefix()
						.Because("a date has no time of day, so the remainder is rejected as soon as it is specified");
				}
			}

			public sealed class DateTimeTests
			{
				[Test]
				public async Task WhenAnItemLiesOutsideTheTolerance_ShouldFail()
				{
					DateTime[] values = [new(2024, 1, 1, 13, 0, 0), new(2024, 1, 1, 14, 0, 0), new(2024, 1, 1, 15, 0, 0),];
					ImmutableArray<DateTime> subject = [.. values,];
					IEnumerable<DateTime> expected = [new(2024, 1, 1, 13, 0, 0), new(2024, 1, 1, 14, 2, 0), new(2024, 1, 1, 15, 0, 0),];

					async Task Act()
						=> await That(subject).IsContainedIn(expected).Within(1.Minutes());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is contained in collection expected ± 1:00 in order and contiguous,
						             but it contained item 2024-01-01T14:00:00.0000000 at index 1 that was not expected

						             Collection:
						             [
						               2024-01-01T13:00:00.0000000,
						               2024-01-01T14:00:00.0000000,
						               2024-01-01T15:00:00.0000000
						             ]

						             Expected:
						             [
						               2024-01-01T13:00:00.0000000,
						               2024-01-01T14:02:00.0000000,
						               2024-01-01T15:00:00.0000000
						             ]
						             """);
				}

				[Test]
				public async Task WhenEachItemLiesWithinTheTolerance_ShouldSucceed()
				{
					DateTime[] values = [new(2024, 1, 1, 13, 0, 0), new(2024, 1, 1, 14, 0, 0), new(2024, 1, 1, 15, 0, 0),];
					ImmutableArray<DateTime> subject = [.. values,];
					IEnumerable<DateTime> expected = [new(2024, 1, 1, 13, 0, 0), new(2024, 1, 1, 14, 1, 0), new(2024, 1, 1, 15, 0, 0),];

					async Task Act()
						=> await That(subject).IsContainedIn(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTheDefaultToleranceIsSet_ShouldApplyIt()
				{
					DateTime[] values = [new(2024, 1, 1, 13, 0, 0), new(2024, 1, 1, 14, 0, 0), new(2024, 1, 1, 15, 0, 0),];
					ImmutableArray<DateTime> subject = [.. values,];
					IEnumerable<DateTime> expected = [new(2024, 1, 1, 13, 0, 0), new(2024, 1, 1, 14, 1, 0), new(2024, 1, 1, 15, 0, 0),];

					async Task Act()
					{
						using IDisposable __ =
							Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Minutes());
						await That(subject).IsContainedIn(expected);
					}

					await That(Act).DoesNotThrow()
						.Because("the items fall back to the default tolerance, as a single value does");
				}

				[Test]
				public async Task WhenTheDefaultToleranceIsSet_ShouldMentionIt()
				{
					DateTime[] values = [new(2024, 1, 1, 13, 0, 0), new(2024, 1, 1, 14, 0, 0), new(2024, 1, 1, 15, 0, 0),];
					ImmutableArray<DateTime> subject = [.. values,];
					IEnumerable<DateTime> expected = [new(2024, 1, 1, 13, 0, 0), new(2024, 1, 1, 14, 2, 0), new(2024, 1, 1, 15, 0, 0),];

					async Task Act()
					{
						using IDisposable __ =
							Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Minutes());
						await That(subject).IsContainedIn(expected);
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is contained in collection expected ± 1:00 in order and contiguous,
						             but it contained item 2024-01-01T14:00:00.0000000 at index 1 that was not expected

						             Collection:
						             [
						               2024-01-01T13:00:00.0000000,
						               2024-01-01T14:00:00.0000000,
						               2024-01-01T15:00:00.0000000
						             ]

						             Expected:
						             [
						               2024-01-01T13:00:00.0000000,
						               2024-01-01T14:02:00.0000000,
						               2024-01-01T15:00:00.0000000
						             ]
						             """)
						.Because("the applied default tolerance is part of the expectation");
				}
			}

			public sealed class NullableDateTimeTests
			{
				[Test]
				public async Task WhenAnItemLiesOutsideTheTolerance_ShouldFail()
				{
					DateTime?[] values = [new DateTime(2024, 1, 1, 13, 0, 0), null, new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					ImmutableArray<DateTime?> subject = [.. values,];
					IEnumerable<DateTime?> expected = [new DateTime(2024, 1, 1, 13, 0, 0), null, new DateTime(2024, 1, 1, 14, 2, 0), new DateTime(2024, 1, 1, 15, 0, 0),];

					async Task Act()
						=> await That(subject).IsContainedIn(expected).Within(1.Minutes());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is contained in collection expected ± 1:00 in order and contiguous,
						             but it contained item 2024-01-01T14:00:00.0000000 at index 2 that was not expected

						             Collection:
						             [
						               2024-01-01T13:00:00.0000000,
						               <null>,
						               2024-01-01T14:00:00.0000000,
						               2024-01-01T15:00:00.0000000
						             ]

						             Expected:
						             [
						               2024-01-01T13:00:00.0000000,
						               <null>,
						               2024-01-01T14:02:00.0000000,
						               2024-01-01T15:00:00.0000000
						             ]
						             """);
				}

				[Test]
				public async Task WhenEachItemLiesWithinTheTolerance_ShouldSucceed()
				{
					DateTime?[] values = [new DateTime(2024, 1, 1, 13, 0, 0), null, new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					ImmutableArray<DateTime?> subject = [.. values,];
					IEnumerable<DateTime?> expected = [new DateTime(2024, 1, 1, 13, 0, 0), null, new DateTime(2024, 1, 1, 14, 1, 0), new DateTime(2024, 1, 1, 15, 0, 0),];

					async Task Act()
						=> await That(subject).IsContainedIn(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class DateTimeOffsetTests
			{
				[Test]
				public async Task WhenAnItemLiesOutsideTheTolerance_ShouldFail()
				{
					DateTimeOffset[] values = [new(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), new(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					ImmutableArray<DateTimeOffset> subject = [.. values,];
					IEnumerable<DateTimeOffset> expected = [new(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), new(2024, 1, 1, 14, 2, 0, TimeSpan.Zero), new(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];

					async Task Act()
						=> await That(subject).IsContainedIn(expected).Within(1.Minutes());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is contained in collection expected ± 1:00 in order and contiguous,
						             but it contained item 2024-01-01T14:00:00.0000000+00:00 at index 1 that was not expected

						             Collection:
						             [
						               2024-01-01T13:00:00.0000000+00:00,
						               2024-01-01T14:00:00.0000000+00:00,
						               2024-01-01T15:00:00.0000000+00:00
						             ]

						             Expected:
						             [
						               2024-01-01T13:00:00.0000000+00:00,
						               2024-01-01T14:02:00.0000000+00:00,
						               2024-01-01T15:00:00.0000000+00:00
						             ]
						             """);
				}

				[Test]
				public async Task WhenEachItemLiesWithinTheTolerance_ShouldSucceed()
				{
					DateTimeOffset[] values = [new(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), new(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					ImmutableArray<DateTimeOffset> subject = [.. values,];
					IEnumerable<DateTimeOffset> expected = [new(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), new(2024, 1, 1, 14, 1, 0, TimeSpan.Zero), new(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];

					async Task Act()
						=> await That(subject).IsContainedIn(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableDateTimeOffsetTests
			{
				[Test]
				public async Task WhenAnItemLiesOutsideTheTolerance_ShouldFail()
				{
					DateTimeOffset?[] values = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), null, new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					ImmutableArray<DateTimeOffset?> subject = [.. values,];
					IEnumerable<DateTimeOffset?> expected = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), null, new DateTimeOffset(2024, 1, 1, 14, 2, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];

					async Task Act()
						=> await That(subject).IsContainedIn(expected).Within(1.Minutes());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is contained in collection expected ± 1:00 in order and contiguous,
						             but it contained item 2024-01-01T14:00:00.0000000+00:00 at index 2 that was not expected

						             Collection:
						             [
						               2024-01-01T13:00:00.0000000+00:00,
						               <null>,
						               2024-01-01T14:00:00.0000000+00:00,
						               2024-01-01T15:00:00.0000000+00:00
						             ]

						             Expected:
						             [
						               2024-01-01T13:00:00.0000000+00:00,
						               <null>,
						               2024-01-01T14:02:00.0000000+00:00,
						               2024-01-01T15:00:00.0000000+00:00
						             ]
						             """);
				}

				[Test]
				public async Task WhenEachItemLiesWithinTheTolerance_ShouldSucceed()
				{
					DateTimeOffset?[] values = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), null, new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					ImmutableArray<DateTimeOffset?> subject = [.. values,];
					IEnumerable<DateTimeOffset?> expected = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), null, new DateTimeOffset(2024, 1, 1, 14, 1, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];

					async Task Act()
						=> await That(subject).IsContainedIn(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class DecimalTests
			{
				[Test]
				public async Task WhenAnItemLiesOutsideTheTolerance_ShouldFail()
				{
					decimal[] values = [1.0m, 2.0m, 3.0m,];
					ImmutableArray<decimal> subject = [.. values,];
					IEnumerable<decimal> expected = [1.0m, 2.5m, 3.0m,];

					async Task Act()
						=> await That(subject).IsContainedIn(expected).Within(0.25m);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is contained in collection expected ± 0.25 in order and contiguous,
						             but it contained item 2.0 at index 1 that was not expected

						             Collection:
						             [1.0, 2.0, 3.0]

						             Expected:
						             [1.0, 2.5, 3.0]
						             """);
				}

				[Test]
				public async Task WhenEachItemLiesWithinTheTolerance_ShouldSucceed()
				{
					decimal[] values = [1.0m, 2.0m, 3.0m,];
					ImmutableArray<decimal> subject = [.. values,];
					IEnumerable<decimal> expected = [1.0m, 2.25m, 3.0m,];

					async Task Act()
						=> await That(subject).IsContainedIn(expected).Within(0.25m);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableDecimalTests
			{
				[Test]
				public async Task WhenAnItemLiesOutsideTheTolerance_ShouldFail()
				{
					decimal?[] values = [1.0m, null, 2.0m, 3.0m,];
					ImmutableArray<decimal?> subject = [.. values,];
					IEnumerable<decimal?> expected = [1.0m, null, 2.5m, 3.0m,];

					async Task Act()
						=> await That(subject).IsContainedIn(expected).Within(0.25m);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is contained in collection expected ± 0.25 in order and contiguous,
						             but it contained item 2.0 at index 2 that was not expected

						             Collection:
						             [1.0, <null>, 2.0, 3.0]

						             Expected:
						             [1.0, <null>, 2.5, 3.0]
						             """);
				}

				[Test]
				public async Task WhenEachItemLiesWithinTheTolerance_ShouldSucceed()
				{
					decimal?[] values = [1.0m, null, 2.0m, 3.0m,];
					ImmutableArray<decimal?> subject = [.. values,];
					IEnumerable<decimal?> expected = [1.0m, null, 2.25m, 3.0m,];

					async Task Act()
						=> await That(subject).IsContainedIn(expected).Within(0.25m);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class DoubleTests
			{
				[Test]
				public async Task WhenAnItemLiesOutsideTheTolerance_ShouldFail()
				{
					double[] values = [1.0, 2.0, 3.0,];
					ImmutableArray<double> subject = [.. values,];
					IEnumerable<double> expected = [1.0, 2.5, 3.0,];

					async Task Act()
						=> await That(subject).IsContainedIn(expected).Within(0.25);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is contained in collection expected ± 0.25 in order and contiguous,
						             but it contained item 2.0 at index 1 that was not expected

						             Collection:
						             [1.0, 2.0, 3.0]

						             Expected:
						             [1.0, 2.5, 3.0]
						             """);
				}

				[Test]
				[Arguments(false)]
				[Arguments(true)]
				public async Task WhenCombinedWithUsing_ShouldThrowInvalidOperationException(bool negated)
				{
					double[] values = [1.0, 2.0, 3.0,];
					ImmutableArray<double> subject = [.. values,];
					IEnumerable<double> expected = [1.0, 2.25, 3.0,];

					async Task Act()
					{
						if (negated)
						{
							await That(subject).IsNotContainedIn(expected).Within(0.25).Using(new AllEqualComparer());
						}
						else
						{
							await That(subject).IsContainedIn(expected).Within(0.25).Using(new AllEqualComparer());
						}
					}

					await That(Act).Throws<InvalidOperationException>()
						.WithMessage("Using cannot be combined with Within.")
						.Because("the comparer would silently replace the tolerance");
				}

				[Test]
				public async Task WhenEachItemLiesWithinTheTolerance_ShouldSucceed()
				{
					double[] values = [1.0, 2.0, 3.0,];
					ImmutableArray<double> subject = [.. values,];
					IEnumerable<double> expected = [1.0, 2.25, 3.0,];

					async Task Act()
						=> await That(subject).IsContainedIn(expected).Within(0.25);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableDoubleTests
			{
				[Test]
				public async Task WhenAnItemLiesOutsideTheTolerance_ShouldFail()
				{
					double?[] values = [1.0, null, 2.0, 3.0,];
					ImmutableArray<double?> subject = [.. values,];
					IEnumerable<double?> expected = [1.0, null, 2.5, 3.0,];

					async Task Act()
						=> await That(subject).IsContainedIn(expected).Within(0.25);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is contained in collection expected ± 0.25 in order and contiguous,
						             but it contained item 2.0 at index 2 that was not expected

						             Collection:
						             [1.0, <null>, 2.0, 3.0]

						             Expected:
						             [1.0, <null>, 2.5, 3.0]
						             """);
				}

				[Test]
				public async Task WhenEachItemLiesWithinTheTolerance_ShouldSucceed()
				{
					double?[] values = [1.0, null, 2.0, 3.0,];
					ImmutableArray<double?> subject = [.. values,];
					IEnumerable<double?> expected = [1.0, null, 2.25, 3.0,];

					async Task Act()
						=> await That(subject).IsContainedIn(expected).Within(0.25);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class FloatTests
			{
				[Test]
				public async Task WhenAnItemLiesOutsideTheTolerance_ShouldFail()
				{
					float[] values = [1.0F, 2.0F, 3.0F,];
					ImmutableArray<float> subject = [.. values,];
					IEnumerable<float> expected = [1.0F, 2.5F, 3.0F,];

					async Task Act()
						=> await That(subject).IsContainedIn(expected).Within(0.25F);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is contained in collection expected ± 0.25 in order and contiguous,
						             but it contained item 2.0 at index 1 that was not expected

						             Collection:
						             [1.0, 2.0, 3.0]

						             Expected:
						             [1.0, 2.5, 3.0]
						             """);
				}

				[Test]
				public async Task WhenEachItemLiesWithinTheTolerance_ShouldSucceed()
				{
					float[] values = [1.0F, 2.0F, 3.0F,];
					ImmutableArray<float> subject = [.. values,];
					IEnumerable<float> expected = [1.0F, 2.25F, 3.0F,];

					async Task Act()
						=> await That(subject).IsContainedIn(expected).Within(0.25F);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableFloatTests
			{
				[Test]
				public async Task WhenAnItemLiesOutsideTheTolerance_ShouldFail()
				{
					float?[] values = [1.0F, null, 2.0F, 3.0F,];
					ImmutableArray<float?> subject = [.. values,];
					IEnumerable<float?> expected = [1.0F, null, 2.5F, 3.0F,];

					async Task Act()
						=> await That(subject).IsContainedIn(expected).Within(0.25F);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is contained in collection expected ± 0.25 in order and contiguous,
						             but it contained item 2.0 at index 2 that was not expected

						             Collection:
						             [1.0, <null>, 2.0, 3.0]

						             Expected:
						             [1.0, <null>, 2.5, 3.0]
						             """);
				}

				[Test]
				public async Task WhenEachItemLiesWithinTheTolerance_ShouldSucceed()
				{
					float?[] values = [1.0F, null, 2.0F, 3.0F,];
					ImmutableArray<float?> subject = [.. values,];
					IEnumerable<float?> expected = [1.0F, null, 2.25F, 3.0F,];

					async Task Act()
						=> await That(subject).IsContainedIn(expected).Within(0.25F);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class TimeOnlyTests
			{
				[Test]
				public async Task WhenAnItemLiesAcrossMidnight_ShouldUseTheShorterDistance()
				{
					TimeOnly[] values = [new(22, 0), new(23, 59, 30), new(2, 0),];
					ImmutableArray<TimeOnly> subject = [.. values,];
					IEnumerable<TimeOnly> expected = [new(22, 0), new(0, 0, 30), new(2, 0),];

					async Task Act()
						=> await That(subject).IsContainedIn(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow()
						.Because("the times are compared on the clock face, where 23:59:30 and 00:00:30 are one minute apart");
				}

				[Test]
				public async Task WhenAnItemLiesOutsideTheTolerance_ShouldFail()
				{
					TimeOnly[] values = [new(13, 0), new(14, 0), new(15, 0),];
					ImmutableArray<TimeOnly> subject = [.. values,];
					IEnumerable<TimeOnly> expected = [new(13, 0), new(14, 2), new(15, 0),];

					async Task Act()
						=> await That(subject).IsContainedIn(expected).Within(1.Minutes());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is contained in collection expected ± 1:00 in order and contiguous,
						             but it contained item 14:00:00.0000000 at index 1 that was not expected

						             Collection:
						             [
						               13:00:00.0000000,
						               14:00:00.0000000,
						               15:00:00.0000000
						             ]

						             Expected:
						             [
						               13:00:00.0000000,
						               14:02:00.0000000,
						               15:00:00.0000000
						             ]
						             """);
				}

				[Test]
				public async Task WhenEachItemLiesWithinTheTolerance_ShouldSucceed()
				{
					TimeOnly[] values = [new(13, 0), new(14, 0), new(15, 0),];
					ImmutableArray<TimeOnly> subject = [.. values,];
					IEnumerable<TimeOnly> expected = [new(13, 0), new(14, 1), new(15, 0),];

					async Task Act()
						=> await That(subject).IsContainedIn(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableTimeOnlyTests
			{
				[Test]
				public async Task WhenAnItemLiesAcrossMidnight_ShouldUseTheShorterDistance()
				{
					TimeOnly?[] values = [new TimeOnly(22, 0), null, new TimeOnly(23, 59, 30), new TimeOnly(2, 0),];
					ImmutableArray<TimeOnly?> subject = [.. values,];
					IEnumerable<TimeOnly?> expected = [new TimeOnly(22, 0), null, new TimeOnly(0, 0, 30), new TimeOnly(2, 0),];

					async Task Act()
						=> await That(subject).IsContainedIn(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow()
						.Because("the times are compared on the clock face, where 23:59:30 and 00:00:30 are one minute apart");
				}

				[Test]
				public async Task WhenAnItemLiesOutsideTheTolerance_ShouldFail()
				{
					TimeOnly?[] values = [new TimeOnly(13, 0), null, new TimeOnly(14, 0), new TimeOnly(15, 0),];
					ImmutableArray<TimeOnly?> subject = [.. values,];
					IEnumerable<TimeOnly?> expected = [new TimeOnly(13, 0), null, new TimeOnly(14, 2), new TimeOnly(15, 0),];

					async Task Act()
						=> await That(subject).IsContainedIn(expected).Within(1.Minutes());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is contained in collection expected ± 1:00 in order and contiguous,
						             but it contained item 14:00:00.0000000 at index 2 that was not expected

						             Collection:
						             [
						               13:00:00.0000000,
						               <null>,
						               14:00:00.0000000,
						               15:00:00.0000000
						             ]

						             Expected:
						             [
						               13:00:00.0000000,
						               <null>,
						               14:02:00.0000000,
						               15:00:00.0000000
						             ]
						             """);
				}

				[Test]
				public async Task WhenEachItemLiesWithinTheTolerance_ShouldSucceed()
				{
					TimeOnly?[] values = [new TimeOnly(13, 0), null, new TimeOnly(14, 0), new TimeOnly(15, 0),];
					ImmutableArray<TimeOnly?> subject = [.. values,];
					IEnumerable<TimeOnly?> expected = [new TimeOnly(13, 0), null, new TimeOnly(14, 1), new TimeOnly(15, 0),];

					async Task Act()
						=> await That(subject).IsContainedIn(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class TimeSpanTests
			{
				[Test]
				public async Task WhenAnItemLiesOutsideTheTolerance_ShouldFail()
				{
					TimeSpan[] values = [new(1, 0, 0), new(2, 0, 0), new(3, 0, 0),];
					ImmutableArray<TimeSpan> subject = [.. values,];
					IEnumerable<TimeSpan> expected = [new(1, 0, 0), new(2, 2, 0), new(3, 0, 0),];

					async Task Act()
						=> await That(subject).IsContainedIn(expected).Within(1.Minutes());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is contained in collection expected ± 1:00 in order and contiguous,
						             but it contained item 2:00:00 at index 1 that was not expected

						             Collection:
						             [
						               1:00:00,
						               2:00:00,
						               3:00:00
						             ]

						             Expected:
						             [
						               1:00:00,
						               2:02:00,
						               3:00:00
						             ]
						             """);
				}

				[Test]
				public async Task WhenEachItemLiesWithinTheTolerance_ShouldSucceed()
				{
					TimeSpan[] values = [new(1, 0, 0), new(2, 0, 0), new(3, 0, 0),];
					ImmutableArray<TimeSpan> subject = [.. values,];
					IEnumerable<TimeSpan> expected = [new(1, 0, 0), new(2, 1, 0), new(3, 0, 0),];

					async Task Act()
						=> await That(subject).IsContainedIn(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NullableTimeSpanTests
			{
				[Test]
				public async Task WhenAnItemLiesOutsideTheTolerance_ShouldFail()
				{
					TimeSpan?[] values = [new TimeSpan(1, 0, 0), null, new TimeSpan(2, 0, 0), new TimeSpan(3, 0, 0),];
					ImmutableArray<TimeSpan?> subject = [.. values,];
					IEnumerable<TimeSpan?> expected = [new TimeSpan(1, 0, 0), null, new TimeSpan(2, 2, 0), new TimeSpan(3, 0, 0),];

					async Task Act()
						=> await That(subject).IsContainedIn(expected).Within(1.Minutes());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is contained in collection expected ± 1:00 in order and contiguous,
						             but it contained item 2:00:00 at index 2 that was not expected

						             Collection:
						             [
						               1:00:00,
						               <null>,
						               2:00:00,
						               3:00:00
						             ]

						             Expected:
						             [
						               1:00:00,
						               <null>,
						               2:02:00,
						               3:00:00
						             ]
						             """);
				}

				[Test]
				public async Task WhenEachItemLiesWithinTheTolerance_ShouldSucceed()
				{
					TimeSpan?[] values = [new TimeSpan(1, 0, 0), null, new TimeSpan(2, 0, 0), new TimeSpan(3, 0, 0),];
					ImmutableArray<TimeSpan?> subject = [.. values,];
					IEnumerable<TimeSpan?> expected = [new TimeSpan(1, 0, 0), null, new TimeSpan(2, 1, 0), new TimeSpan(3, 0, 0),];

					async Task Act()
						=> await That(subject).IsContainedIn(expected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}
			}
		}
	}

	public sealed partial class IsNotContainedIn
	{
		public sealed class ImmutableWithin
		{
			public sealed class DateOnlyTests
			{
				[Test]
				public async Task WhenAnItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateOnly[] values = [new(2024, 1, 1), new(2024, 1, 11), new(2024, 1, 21),];
					ImmutableArray<DateOnly> subject = [.. values,];
					IEnumerable<DateOnly> unexpected = [new(2024, 1, 1), new(2024, 1, 13), new(2024, 1, 21),];

					async Task Act()
						=> await That(subject).IsNotContainedIn(unexpected).Within(1.Days());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEachItemLiesWithinTheTolerance_ShouldFail()
				{
					DateOnly[] values = [new(2024, 1, 1), new(2024, 1, 11), new(2024, 1, 21),];
					ImmutableArray<DateOnly> subject = [.. values,];
					IEnumerable<DateOnly> unexpected = [new(2024, 1, 1), new(2024, 1, 12), new(2024, 1, 21),];

					async Task Act()
						=> await That(subject).IsNotContainedIn(unexpected).Within(1.Days());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not contained in collection unexpected ± 1 day in order and contiguous,
						              but it was

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(unexpected, FormattingOptions.MultipleLines)}
						              """);
				}
			}

			public sealed class NullableDateOnlyTests
			{
				[Test]
				public async Task WhenAnItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateOnly?[] values = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					ImmutableArray<DateOnly?> subject = [.. values,];
					IEnumerable<DateOnly?> unexpected = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 13), new DateOnly(2024, 1, 21),];

					async Task Act()
						=> await That(subject).IsNotContainedIn(unexpected).Within(1.Days());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEachItemLiesWithinTheTolerance_ShouldFail()
				{
					DateOnly?[] values = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 21),];
					ImmutableArray<DateOnly?> subject = [.. values,];
					IEnumerable<DateOnly?> unexpected = [new DateOnly(2024, 1, 1), null, new DateOnly(2024, 1, 12), new DateOnly(2024, 1, 21),];

					async Task Act()
						=> await That(subject).IsNotContainedIn(unexpected).Within(1.Days());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not contained in collection unexpected ± 1 day in order and contiguous,
						              but it was

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(unexpected, FormattingOptions.MultipleLines)}
						              """);
				}
			}

			public sealed class DateTimeTests
			{
				[Test]
				public async Task WhenAnItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateTime[] values = [new(2024, 1, 1, 13, 0, 0), new(2024, 1, 1, 14, 0, 0), new(2024, 1, 1, 15, 0, 0),];
					ImmutableArray<DateTime> subject = [.. values,];
					IEnumerable<DateTime> unexpected = [new(2024, 1, 1, 13, 0, 0), new(2024, 1, 1, 14, 2, 0), new(2024, 1, 1, 15, 0, 0),];

					async Task Act()
						=> await That(subject).IsNotContainedIn(unexpected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEachItemLiesWithinTheTolerance_ShouldFail()
				{
					DateTime[] values = [new(2024, 1, 1, 13, 0, 0), new(2024, 1, 1, 14, 0, 0), new(2024, 1, 1, 15, 0, 0),];
					ImmutableArray<DateTime> subject = [.. values,];
					IEnumerable<DateTime> unexpected = [new(2024, 1, 1, 13, 0, 0), new(2024, 1, 1, 14, 1, 0), new(2024, 1, 1, 15, 0, 0),];

					async Task Act()
						=> await That(subject).IsNotContainedIn(unexpected).Within(1.Minutes());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not contained in collection unexpected ± 1:00 in order and contiguous,
						              but it was

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(unexpected, FormattingOptions.MultipleLines)}
						              """);
				}
			}

			public sealed class NullableDateTimeTests
			{
				[Test]
				public async Task WhenAnItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateTime?[] values = [new DateTime(2024, 1, 1, 13, 0, 0), null, new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					ImmutableArray<DateTime?> subject = [.. values,];
					IEnumerable<DateTime?> unexpected = [new DateTime(2024, 1, 1, 13, 0, 0), null, new DateTime(2024, 1, 1, 14, 2, 0), new DateTime(2024, 1, 1, 15, 0, 0),];

					async Task Act()
						=> await That(subject).IsNotContainedIn(unexpected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEachItemLiesWithinTheTolerance_ShouldFail()
				{
					DateTime?[] values = [new DateTime(2024, 1, 1, 13, 0, 0), null, new DateTime(2024, 1, 1, 14, 0, 0), new DateTime(2024, 1, 1, 15, 0, 0),];
					ImmutableArray<DateTime?> subject = [.. values,];
					IEnumerable<DateTime?> unexpected = [new DateTime(2024, 1, 1, 13, 0, 0), null, new DateTime(2024, 1, 1, 14, 1, 0), new DateTime(2024, 1, 1, 15, 0, 0),];

					async Task Act()
						=> await That(subject).IsNotContainedIn(unexpected).Within(1.Minutes());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not contained in collection unexpected ± 1:00 in order and contiguous,
						              but it was

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(unexpected, FormattingOptions.MultipleLines)}
						              """);
				}
			}

			public sealed class DateTimeOffsetTests
			{
				[Test]
				public async Task WhenAnItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateTimeOffset[] values = [new(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), new(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					ImmutableArray<DateTimeOffset> subject = [.. values,];
					IEnumerable<DateTimeOffset> unexpected = [new(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), new(2024, 1, 1, 14, 2, 0, TimeSpan.Zero), new(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];

					async Task Act()
						=> await That(subject).IsNotContainedIn(unexpected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEachItemLiesWithinTheTolerance_ShouldFail()
				{
					DateTimeOffset[] values = [new(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), new(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					ImmutableArray<DateTimeOffset> subject = [.. values,];
					IEnumerable<DateTimeOffset> unexpected = [new(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), new(2024, 1, 1, 14, 1, 0, TimeSpan.Zero), new(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];

					async Task Act()
						=> await That(subject).IsNotContainedIn(unexpected).Within(1.Minutes());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not contained in collection unexpected ± 1:00 in order and contiguous,
						              but it was

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(unexpected, FormattingOptions.MultipleLines)}
						              """);
				}
			}

			public sealed class NullableDateTimeOffsetTests
			{
				[Test]
				public async Task WhenAnItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					DateTimeOffset?[] values = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), null, new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					ImmutableArray<DateTimeOffset?> subject = [.. values,];
					IEnumerable<DateTimeOffset?> unexpected = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), null, new DateTimeOffset(2024, 1, 1, 14, 2, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];

					async Task Act()
						=> await That(subject).IsNotContainedIn(unexpected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEachItemLiesWithinTheTolerance_ShouldFail()
				{
					DateTimeOffset?[] values = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), null, new DateTimeOffset(2024, 1, 1, 14, 0, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];
					ImmutableArray<DateTimeOffset?> subject = [.. values,];
					IEnumerable<DateTimeOffset?> unexpected = [new DateTimeOffset(2024, 1, 1, 13, 0, 0, TimeSpan.Zero), null, new DateTimeOffset(2024, 1, 1, 14, 1, 0, TimeSpan.Zero), new DateTimeOffset(2024, 1, 1, 15, 0, 0, TimeSpan.Zero),];

					async Task Act()
						=> await That(subject).IsNotContainedIn(unexpected).Within(1.Minutes());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not contained in collection unexpected ± 1:00 in order and contiguous,
						              but it was

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(unexpected, FormattingOptions.MultipleLines)}
						              """);
				}
			}

			public sealed class DecimalTests
			{
				[Test]
				public async Task WhenAnItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					decimal[] values = [1.0m, 2.0m, 3.0m,];
					ImmutableArray<decimal> subject = [.. values,];
					IEnumerable<decimal> unexpected = [1.0m, 2.5m, 3.0m,];

					async Task Act()
						=> await That(subject).IsNotContainedIn(unexpected).Within(0.25m);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEachItemLiesWithinTheTolerance_ShouldFail()
				{
					decimal[] values = [1.0m, 2.0m, 3.0m,];
					ImmutableArray<decimal> subject = [.. values,];
					IEnumerable<decimal> unexpected = [1.0m, 2.25m, 3.0m,];

					async Task Act()
						=> await That(subject).IsNotContainedIn(unexpected).Within(0.25m);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not contained in collection unexpected ± 0.25 in order and contiguous,
						              but it was

						              Collection:
						              {Formatter.Format(values)}

						              Expected:
						              {Formatter.Format(unexpected)}
						              """);
				}
			}

			public sealed class NullableDecimalTests
			{
				[Test]
				public async Task WhenAnItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					decimal?[] values = [1.0m, null, 2.0m, 3.0m,];
					ImmutableArray<decimal?> subject = [.. values,];
					IEnumerable<decimal?> unexpected = [1.0m, null, 2.5m, 3.0m,];

					async Task Act()
						=> await That(subject).IsNotContainedIn(unexpected).Within(0.25m);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEachItemLiesWithinTheTolerance_ShouldFail()
				{
					decimal?[] values = [1.0m, null, 2.0m, 3.0m,];
					ImmutableArray<decimal?> subject = [.. values,];
					IEnumerable<decimal?> unexpected = [1.0m, null, 2.25m, 3.0m,];

					async Task Act()
						=> await That(subject).IsNotContainedIn(unexpected).Within(0.25m);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not contained in collection unexpected ± 0.25 in order and contiguous,
						              but it was

						              Collection:
						              {Formatter.Format(values)}

						              Expected:
						              {Formatter.Format(unexpected)}
						              """);
				}
			}

			public sealed class DoubleTests
			{
				[Test]
				public async Task WhenAnItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					double[] values = [1.0, 2.0, 3.0,];
					ImmutableArray<double> subject = [.. values,];
					IEnumerable<double> unexpected = [1.0, 2.5, 3.0,];

					async Task Act()
						=> await That(subject).IsNotContainedIn(unexpected).Within(0.25);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEachItemLiesWithinTheTolerance_ShouldFail()
				{
					double[] values = [1.0, 2.0, 3.0,];
					ImmutableArray<double> subject = [.. values,];
					IEnumerable<double> unexpected = [1.0, 2.25, 3.0,];

					async Task Act()
						=> await That(subject).IsNotContainedIn(unexpected).Within(0.25);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not contained in collection unexpected ± 0.25 in order and contiguous,
						              but it was

						              Collection:
						              {Formatter.Format(values)}

						              Expected:
						              {Formatter.Format(unexpected)}
						              """);
				}
			}

			public sealed class NullableDoubleTests
			{
				[Test]
				public async Task WhenAnItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					double?[] values = [1.0, null, 2.0, 3.0,];
					ImmutableArray<double?> subject = [.. values,];
					IEnumerable<double?> unexpected = [1.0, null, 2.5, 3.0,];

					async Task Act()
						=> await That(subject).IsNotContainedIn(unexpected).Within(0.25);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEachItemLiesWithinTheTolerance_ShouldFail()
				{
					double?[] values = [1.0, null, 2.0, 3.0,];
					ImmutableArray<double?> subject = [.. values,];
					IEnumerable<double?> unexpected = [1.0, null, 2.25, 3.0,];

					async Task Act()
						=> await That(subject).IsNotContainedIn(unexpected).Within(0.25);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not contained in collection unexpected ± 0.25 in order and contiguous,
						              but it was

						              Collection:
						              {Formatter.Format(values)}

						              Expected:
						              {Formatter.Format(unexpected)}
						              """);
				}
			}

			public sealed class FloatTests
			{
				[Test]
				public async Task WhenAnItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					float[] values = [1.0F, 2.0F, 3.0F,];
					ImmutableArray<float> subject = [.. values,];
					IEnumerable<float> unexpected = [1.0F, 2.5F, 3.0F,];

					async Task Act()
						=> await That(subject).IsNotContainedIn(unexpected).Within(0.25F);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEachItemLiesWithinTheTolerance_ShouldFail()
				{
					float[] values = [1.0F, 2.0F, 3.0F,];
					ImmutableArray<float> subject = [.. values,];
					IEnumerable<float> unexpected = [1.0F, 2.25F, 3.0F,];

					async Task Act()
						=> await That(subject).IsNotContainedIn(unexpected).Within(0.25F);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not contained in collection unexpected ± 0.25 in order and contiguous,
						              but it was

						              Collection:
						              {Formatter.Format(values)}

						              Expected:
						              {Formatter.Format(unexpected)}
						              """);
				}
			}

			public sealed class NullableFloatTests
			{
				[Test]
				public async Task WhenAnItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					float?[] values = [1.0F, null, 2.0F, 3.0F,];
					ImmutableArray<float?> subject = [.. values,];
					IEnumerable<float?> unexpected = [1.0F, null, 2.5F, 3.0F,];

					async Task Act()
						=> await That(subject).IsNotContainedIn(unexpected).Within(0.25F);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEachItemLiesWithinTheTolerance_ShouldFail()
				{
					float?[] values = [1.0F, null, 2.0F, 3.0F,];
					ImmutableArray<float?> subject = [.. values,];
					IEnumerable<float?> unexpected = [1.0F, null, 2.25F, 3.0F,];

					async Task Act()
						=> await That(subject).IsNotContainedIn(unexpected).Within(0.25F);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not contained in collection unexpected ± 0.25 in order and contiguous,
						              but it was

						              Collection:
						              {Formatter.Format(values)}

						              Expected:
						              {Formatter.Format(unexpected)}
						              """);
				}
			}

			public sealed class TimeOnlyTests
			{
				[Test]
				public async Task WhenAnItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					TimeOnly[] values = [new(13, 0), new(14, 0), new(15, 0),];
					ImmutableArray<TimeOnly> subject = [.. values,];
					IEnumerable<TimeOnly> unexpected = [new(13, 0), new(14, 2), new(15, 0),];

					async Task Act()
						=> await That(subject).IsNotContainedIn(unexpected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEachItemLiesWithinTheTolerance_ShouldFail()
				{
					TimeOnly[] values = [new(13, 0), new(14, 0), new(15, 0),];
					ImmutableArray<TimeOnly> subject = [.. values,];
					IEnumerable<TimeOnly> unexpected = [new(13, 0), new(14, 1), new(15, 0),];

					async Task Act()
						=> await That(subject).IsNotContainedIn(unexpected).Within(1.Minutes());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not contained in collection unexpected ± 1:00 in order and contiguous,
						              but it was

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(unexpected, FormattingOptions.MultipleLines)}
						              """);
				}
			}

			public sealed class NullableTimeOnlyTests
			{
				[Test]
				public async Task WhenAnItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					TimeOnly?[] values = [new TimeOnly(13, 0), null, new TimeOnly(14, 0), new TimeOnly(15, 0),];
					ImmutableArray<TimeOnly?> subject = [.. values,];
					IEnumerable<TimeOnly?> unexpected = [new TimeOnly(13, 0), null, new TimeOnly(14, 2), new TimeOnly(15, 0),];

					async Task Act()
						=> await That(subject).IsNotContainedIn(unexpected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEachItemLiesWithinTheTolerance_ShouldFail()
				{
					TimeOnly?[] values = [new TimeOnly(13, 0), null, new TimeOnly(14, 0), new TimeOnly(15, 0),];
					ImmutableArray<TimeOnly?> subject = [.. values,];
					IEnumerable<TimeOnly?> unexpected = [new TimeOnly(13, 0), null, new TimeOnly(14, 1), new TimeOnly(15, 0),];

					async Task Act()
						=> await That(subject).IsNotContainedIn(unexpected).Within(1.Minutes());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not contained in collection unexpected ± 1:00 in order and contiguous,
						              but it was

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(unexpected, FormattingOptions.MultipleLines)}
						              """);
				}
			}

			public sealed class TimeSpanTests
			{
				[Test]
				public async Task WhenAnItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					TimeSpan[] values = [new(1, 0, 0), new(2, 0, 0), new(3, 0, 0),];
					ImmutableArray<TimeSpan> subject = [.. values,];
					IEnumerable<TimeSpan> unexpected = [new(1, 0, 0), new(2, 2, 0), new(3, 0, 0),];

					async Task Act()
						=> await That(subject).IsNotContainedIn(unexpected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEachItemLiesWithinTheTolerance_ShouldFail()
				{
					TimeSpan[] values = [new(1, 0, 0), new(2, 0, 0), new(3, 0, 0),];
					ImmutableArray<TimeSpan> subject = [.. values,];
					IEnumerable<TimeSpan> unexpected = [new(1, 0, 0), new(2, 1, 0), new(3, 0, 0),];

					async Task Act()
						=> await That(subject).IsNotContainedIn(unexpected).Within(1.Minutes());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not contained in collection unexpected ± 1:00 in order and contiguous,
						              but it was

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(unexpected, FormattingOptions.MultipleLines)}
						              """);
				}
			}

			public sealed class NullableTimeSpanTests
			{
				[Test]
				public async Task WhenAnItemLiesOutsideTheTolerance_ShouldSucceed()
				{
					TimeSpan?[] values = [new TimeSpan(1, 0, 0), null, new TimeSpan(2, 0, 0), new TimeSpan(3, 0, 0),];
					ImmutableArray<TimeSpan?> subject = [.. values,];
					IEnumerable<TimeSpan?> unexpected = [new TimeSpan(1, 0, 0), null, new TimeSpan(2, 2, 0), new TimeSpan(3, 0, 0),];

					async Task Act()
						=> await That(subject).IsNotContainedIn(unexpected).Within(1.Minutes());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEachItemLiesWithinTheTolerance_ShouldFail()
				{
					TimeSpan?[] values = [new TimeSpan(1, 0, 0), null, new TimeSpan(2, 0, 0), new TimeSpan(3, 0, 0),];
					ImmutableArray<TimeSpan?> subject = [.. values,];
					IEnumerable<TimeSpan?> unexpected = [new TimeSpan(1, 0, 0), null, new TimeSpan(2, 1, 0), new TimeSpan(3, 0, 0),];

					async Task Act()
						=> await That(subject).IsNotContainedIn(unexpected).Within(1.Minutes());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not contained in collection unexpected ± 1:00 in order and contiguous,
						              but it was

						              Collection:
						              {Formatter.Format(values, FormattingOptions.MultipleLines)}

						              Expected:
						              {Formatter.Format(unexpected, FormattingOptions.MultipleLines)}
						              """);
				}
			}
		}
	}
}
#endif
