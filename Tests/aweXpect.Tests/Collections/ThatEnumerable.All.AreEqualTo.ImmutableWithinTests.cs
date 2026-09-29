#if NET8_0_OR_GREATER
using System.Collections.Immutable;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class All
	{
		public sealed partial class AreEqualTo
		{
			public sealed class ImmutableWithin
			{
				public sealed class DoubleTests
				{
					[Fact]
					public async Task WhenValuesAreNotWithinTolerance_ShouldFail()
					{
						ImmutableArray<double> subject = [1.0, 1.3, 0.9,];

						async Task Act()
							=> await That(subject).All().AreEqualTo(1.0).Within(0.2);

						await That(Act).Throws<XunitException>()
							.WithMessage("""
							             Expected that subject
							             is equal to 1.0 ± 0.2 for all items,
							             but only 2 of 3 were

							             Not matching items:
							             [1.3]

							             Collection:
							             [1.0, 1.3, 0.9]
							             """);
					}

					[Fact]
					public async Task WhenValuesAreWithinTolerance_ShouldSucceed()
					{
						ImmutableArray<double> subject = [1.0, 1.1, 0.9,];

						async Task Act()
							=> await That(subject).All().AreEqualTo(1.0).Within(0.2);

						await That(Act).DoesNotThrow();
					}
				}

				public sealed class NullableDoubleTests
				{
					[Fact]
					public async Task WhenValuesAreNotWithinTolerance_ShouldFail()
					{
						ImmutableArray<double?> subject = [1.0, 1.3, 0.9,];

						async Task Act()
							=> await That(subject).All().AreEqualTo(1.0).Within(0.2);

						await That(Act).Throws<XunitException>()
							.WithMessage("""
							             Expected that subject
							             is equal to 1.0 ± 0.2 for all items,
							             but only 2 of 3 were

							             Not matching items:
							             [1.3]

							             Collection:
							             [1.0, 1.3, 0.9]
							             """);
					}

					[Fact]
					public async Task WhenValuesAreWithinTolerance_ShouldSucceed()
					{
						ImmutableArray<double?> subject = [1.0, 1.1, 0.9,];

						async Task Act()
							=> await That(subject).All().AreEqualTo(1.0).Within(0.2);

						await That(Act).DoesNotThrow();
					}
				}

				public sealed class FloatTests
				{
					[Fact]
					public async Task WhenValuesAreNotWithinTolerance_ShouldFail()
					{
						ImmutableArray<float> subject = [1.0F, 1.3F, 0.9F,];

						async Task Act()
							=> await That(subject).All().AreEqualTo(1.0F).Within(0.2F);

						await That(Act).Throws<XunitException>()
							.WithMessage("""
							             Expected that subject
							             is equal to 1.0 ± 0.2 for all items,
							             but only 2 of 3 were

							             Not matching items:
							             [1.3]

							             Collection:
							             [1.0, 1.3, 0.9]
							             """);
					}

					[Fact]
					public async Task WhenValuesAreWithinTolerance_ShouldSucceed()
					{
						ImmutableArray<float> subject = [1.0F, 1.1F, 0.9F,];

						async Task Act()
							=> await That(subject).All().AreEqualTo(1.0F).Within(0.2F);

						await That(Act).DoesNotThrow();
					}
				}

				public sealed class NullableFloatTests
				{
					[Fact]
					public async Task WhenValuesAreNotWithinTolerance_ShouldFail()
					{
						ImmutableArray<float?> subject = [1.0F, 1.3F, 0.9F,];

						async Task Act()
							=> await That(subject).All().AreEqualTo(1.0F).Within(0.2F);

						await That(Act).Throws<XunitException>()
							.WithMessage("""
							             Expected that subject
							             is equal to 1.0 ± 0.2 for all items,
							             but only 2 of 3 were

							             Not matching items:
							             [1.3]

							             Collection:
							             [1.0, 1.3, 0.9]
							             """);
					}

					[Fact]
					public async Task WhenValuesAreWithinTolerance_ShouldSucceed()
					{
						ImmutableArray<float?> subject = [1.0F, 1.1F, 0.9F,];

						async Task Act()
							=> await That(subject).All().AreEqualTo(1.0F).Within(0.2F);

						await That(Act).DoesNotThrow();
					}
				}

				public sealed class DecimalTests
				{
					[Fact]
					public async Task WhenValuesAreNotWithinTolerance_ShouldFail()
					{
						ImmutableArray<decimal> subject = [1.0m, 1.3m, 0.9m,];

						async Task Act()
							=> await That(subject).All().AreEqualTo(1.0m).Within(0.2m);

						await That(Act).Throws<XunitException>()
							.WithMessage("""
							             Expected that subject
							             is equal to 1.0 ± 0.2 for all items,
							             but only 2 of 3 were

							             Not matching items:
							             [1.3]

							             Collection:
							             [1.0, 1.3, 0.9]
							             """);
					}

					[Fact]
					public async Task WhenValuesAreWithinTolerance_ShouldSucceed()
					{
						ImmutableArray<decimal> subject = [1.0m, 1.1m, 0.9m,];

						async Task Act()
							=> await That(subject).All().AreEqualTo(1.0m).Within(0.2m);

						await That(Act).DoesNotThrow();
					}
				}

				public sealed class NullableDecimalTests
				{
					[Fact]
					public async Task WhenValuesAreNotWithinTolerance_ShouldFail()
					{
						ImmutableArray<decimal?> subject = [1.0m, 1.3m, 0.9m,];

						async Task Act()
							=> await That(subject).All().AreEqualTo(1.0m).Within(0.2m);

						await That(Act).Throws<XunitException>()
							.WithMessage("""
							             Expected that subject
							             is equal to 1.0 ± 0.2 for all items,
							             but only 2 of 3 were

							             Not matching items:
							             [1.3]

							             Collection:
							             [1.0, 1.3, 0.9]
							             """);
					}

					[Fact]
					public async Task WhenValuesAreWithinTolerance_ShouldSucceed()
					{
						ImmutableArray<decimal?> subject = [1.0m, 1.1m, 0.9m,];

						async Task Act()
							=> await That(subject).All().AreEqualTo(1.0m).Within(0.2m);

						await That(Act).DoesNotThrow();
					}
				}

				public sealed class DateTimeTests
				{
					[Fact]
					public async Task WhenValuesAreNotWithinTolerance_ShouldFail()
					{
						DateTime now = DateTime.Now;
						ImmutableArray<DateTime> subject = [now.AddMinutes(1), now, now.AddMinutes(-2),];

						async Task Act()
							=> await That(subject).All().AreEqualTo(now).Within(1.Minutes());

						await That(Act).Throws<XunitException>()
							.WithMessage($"""
							              Expected that subject
							              is equal to {Formatter.Format(now)} ± 1:00 for all items,
							              but only 2 of 3 were

							              Not matching items:
							              [
							                {Formatter.Format(now.AddMinutes(-2))}
							              ]

							              Collection:
							              [
							                {Formatter.Format(now.AddMinutes(1))},
							                {Formatter.Format(now)},
							                {Formatter.Format(now.AddMinutes(-2))}
							              ]
							              """);
					}

					[Fact]
					public async Task WhenValuesAreWithinTolerance_ShouldSucceed()
					{
						DateTime now = DateTime.Now;
						ImmutableArray<DateTime> subject = [now.AddMinutes(1), now, now.AddMinutes(-1),];

						async Task Act()
							=> await That(subject).All().AreEqualTo(now).Within(1.Minutes());

						await That(Act).DoesNotThrow();
					}
				}

				public sealed class NullableDateTimeTests
				{
					[Fact]
					public async Task WhenValuesAreNotWithinTolerance_ShouldFail()
					{
						DateTime now = DateTime.Now;
						ImmutableArray<DateTime?> subject = [now.AddMinutes(1), now, null, now.AddMinutes(-2),];

						async Task Act()
							=> await That(subject).All().AreEqualTo(now).Within(1.Minutes());

						await That(Act).Throws<XunitException>()
							.WithMessage($"""
							              Expected that subject
							              is equal to {Formatter.Format(now)} ± 1:00 for all items,
							              but only 2 of 4 were

							              Not matching items:
							              [
							                <null>,
							                {Formatter.Format(now.AddMinutes(-2))}
							              ]

							              Collection:
							              [
							                {Formatter.Format(now.AddMinutes(1))},
							                {Formatter.Format(now)},
							                <null>,
							                {Formatter.Format(now.AddMinutes(-2))}
							              ]
							              """);
					}

					[Fact]
					public async Task WhenValuesAreWithinTolerance_ShouldSucceed()
					{
						DateTime now = DateTime.Now;
						ImmutableArray<DateTime?> subject = [now.AddMinutes(1), now, now.AddMinutes(-1),];

						async Task Act()
							=> await That(subject).All().AreEqualTo(now).Within(1.Minutes());

						await That(Act).DoesNotThrow();
					}
				}

				public sealed class DateTimeOffsetTests
				{
					[Fact]
					public async Task WhenValuesAreNotWithinTolerance_ShouldFail()
					{
						DateTimeOffset now = DateTimeOffset.Now;
						ImmutableArray<DateTimeOffset> subject = [now.AddMinutes(1), now, now.AddMinutes(-2),];

						async Task Act()
							=> await That(subject).All().AreEqualTo(now).Within(1.Minutes());

						await That(Act).Throws<XunitException>()
							.WithMessage($"""
							              Expected that subject
							              is equal to {Formatter.Format(now)} ± 1:00 for all items,
							              but only 2 of 3 were

							              Not matching items:
							              [
							                {Formatter.Format(now.AddMinutes(-2))}
							              ]

							              Collection:
							              [
							                {Formatter.Format(now.AddMinutes(1))},
							                {Formatter.Format(now)},
							                {Formatter.Format(now.AddMinutes(-2))}
							              ]
							              """);
					}

					[Fact]
					public async Task WhenValuesAreWithinTolerance_ShouldSucceed()
					{
						DateTimeOffset now = DateTimeOffset.Now;
						ImmutableArray<DateTimeOffset> subject = [now.AddMinutes(1), now, now.AddMinutes(-1),];

						async Task Act()
							=> await That(subject).All().AreEqualTo(now).Within(1.Minutes());

						await That(Act).DoesNotThrow();
					}
				}

				public sealed class NullableDateTimeOffsetTests
				{
					[Fact]
					public async Task WhenValuesAreNotWithinTolerance_ShouldFail()
					{
						DateTimeOffset now = DateTimeOffset.Now;
						ImmutableArray<DateTimeOffset?> subject = [now.AddMinutes(1), now, null, now.AddMinutes(-2),];

						async Task Act()
							=> await That(subject).All().AreEqualTo(now).Within(1.Minutes());

						await That(Act).Throws<XunitException>()
							.WithMessage($"""
							              Expected that subject
							              is equal to {Formatter.Format(now)} ± 1:00 for all items,
							              but only 2 of 4 were

							              Not matching items:
							              [
							                <null>,
							                {Formatter.Format(now.AddMinutes(-2))}
							              ]

							              Collection:
							              [
							                {Formatter.Format(now.AddMinutes(1))},
							                {Formatter.Format(now)},
							                <null>,
							                {Formatter.Format(now.AddMinutes(-2))}
							              ]
							              """);
					}

					[Fact]
					public async Task WhenValuesAreWithinTolerance_ShouldSucceed()
					{
						DateTimeOffset now = DateTimeOffset.Now;
						ImmutableArray<DateTimeOffset?> subject = [now.AddMinutes(1), now, now.AddMinutes(-1),];

						async Task Act()
							=> await That(subject).All().AreEqualTo(now).Within(1.Minutes());

						await That(Act).DoesNotThrow();
					}
				}

				public sealed class TimeSpanTests
				{
					[Fact]
					public async Task WhenValuesAreNotWithinTolerance_ShouldFail()
					{
						ImmutableArray<TimeSpan> subject = [61.Minutes(), 1.Hours(), 58.Minutes(),];

						async Task Act()
							=> await That(subject).All().AreEqualTo(1.Hours()).Within(1.Minutes());

						await That(Act).Throws<XunitException>()
							.WithMessage("""
							             Expected that subject
							             is equal to 1:00:00 ± 1:00 for all items,
							             but only 2 of 3 were

							             Not matching items:
							             [
							               58:00
							             ]

							             Collection:
							             [
							               1:01:00,
							               1:00:00,
							               58:00
							             ]
							             """);
					}

					[Fact]
					public async Task WhenValuesAreWithinTolerance_ShouldSucceed()
					{
						ImmutableArray<TimeSpan> subject = [61.Minutes(), 1.Hours(), 59.Minutes(),];

						async Task Act()
							=> await That(subject).All().AreEqualTo(1.Hours()).Within(1.Minutes());

						await That(Act).DoesNotThrow();
					}
				}

				public sealed class NullableTimeSpanTests
				{
					[Fact]
					public async Task WhenValuesAreNotWithinTolerance_ShouldFail()
					{
						ImmutableArray<TimeSpan?> subject = [61.Minutes(), 1.Hours(), null, 58.Minutes(),];

						async Task Act()
							=> await That(subject).All().AreEqualTo(1.Hours()).Within(1.Minutes());

						await That(Act).Throws<XunitException>()
							.WithMessage("""
							             Expected that subject
							             is equal to 1:00:00 ± 1:00 for all items,
							             but only 2 of 4 were

							             Not matching items:
							             [
							               <null>,
							               58:00
							             ]

							             Collection:
							             [
							               1:01:00,
							               1:00:00,
							               <null>,
							               58:00
							             ]
							             """);
					}

					[Fact]
					public async Task WhenValuesAreWithinTolerance_ShouldSucceed()
					{
						ImmutableArray<TimeSpan?> subject = [61.Minutes(), 1.Hours(), 59.Minutes(),];

						async Task Act()
							=> await That(subject).All().AreEqualTo(1.Hours()).Within(1.Minutes());

						await That(Act).DoesNotThrow();
					}
				}

				public sealed class DateOnlyTests
				{
					[Fact]
					public async Task WhenToleranceIsNotAWholeNumberOfDays_ShouldThrowArgumentOutOfRangeException()
					{
						DateOnly[] values = [new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 12),];
						ImmutableArray<DateOnly> subject = [.. values,];

						object Act()
							=> That(subject).All().AreEqualTo(new DateOnly(2024, 1, 11)).Within(1.Days() + 1.Hours());

						await That(Act).Throws<ArgumentOutOfRangeException>()
							.WithParamName("tolerance").And
							.WithMessage("The tolerance must be a whole number of days.").AsPrefix()
							.Because("a date has no time of day, so the remainder is rejected as soon as it is specified");
					}

					[Fact]
					public async Task WhenValuesAreNotWithinTolerance_ShouldFail()
					{
						DateOnly[] values = [new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 13),];
						ImmutableArray<DateOnly> subject = [.. values,];
						DateOnly expected = new DateOnly(2024, 1, 11);

						async Task Act()
							=> await That(subject).All().AreEqualTo(expected).Within(1.Days());

						await That(Act).Throws<XunitException>()
							.WithMessage($"""
							              Expected that subject
							              is equal to {Formatter.Format(expected)} ± 1 day for all items,
							              but only 1 of 2 were

							              Not matching items:
							              {Formatter.Format(new DateOnly[] { new DateOnly(2024, 1, 13), }, FormattingOptions.MultipleLines)}

							              Collection:
							              {Formatter.Format(values, FormattingOptions.MultipleLines)}
							              """);
					}

					[Fact]
					public async Task WhenValuesAreWithinTolerance_ShouldSucceed()
					{
						DateOnly[] values = [new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 12),];
						ImmutableArray<DateOnly> subject = [.. values,];

						async Task Act()
							=> await That(subject).All().AreEqualTo(new DateOnly(2024, 1, 11)).Within(1.Days());

						await That(Act).DoesNotThrow();
					}
				}

				public sealed class NullableDateOnlyTests
				{
					[Fact]
					public async Task WhenToleranceIsNotAWholeNumberOfDays_ShouldThrowArgumentOutOfRangeException()
					{
						DateOnly?[] values = [new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 12),];
						ImmutableArray<DateOnly?> subject = [.. values,];

						object Act()
							=> That(subject).All().AreEqualTo(new DateOnly(2024, 1, 11)).Within(1.Days() + 1.Hours());

						await That(Act).Throws<ArgumentOutOfRangeException>()
							.WithParamName("tolerance").And
							.WithMessage("The tolerance must be a whole number of days.").AsPrefix()
							.Because("a date has no time of day, so the remainder is rejected as soon as it is specified");
					}

					[Fact]
					public async Task WhenValuesAreNotWithinTolerance_ShouldFail()
					{
						DateOnly?[] values = [new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 13),];
						ImmutableArray<DateOnly?> subject = [.. values,];
						DateOnly? expected = new DateOnly(2024, 1, 11);

						async Task Act()
							=> await That(subject).All().AreEqualTo(expected).Within(1.Days());

						await That(Act).Throws<XunitException>()
							.WithMessage($"""
							              Expected that subject
							              is equal to {Formatter.Format(expected)} ± 1 day for all items,
							              but only 1 of 2 were

							              Not matching items:
							              {Formatter.Format(new DateOnly?[] { new DateOnly(2024, 1, 13), }, FormattingOptions.MultipleLines)}

							              Collection:
							              {Formatter.Format(values, FormattingOptions.MultipleLines)}
							              """);
					}

					[Fact]
					public async Task WhenValuesAreWithinTolerance_ShouldSucceed()
					{
						DateOnly?[] values = [new DateOnly(2024, 1, 11), new DateOnly(2024, 1, 12),];
						ImmutableArray<DateOnly?> subject = [.. values,];

						async Task Act()
							=> await That(subject).All().AreEqualTo(new DateOnly(2024, 1, 11)).Within(1.Days());

						await That(Act).DoesNotThrow();
					}
				}

				public sealed class TimeOnlyTests
				{
					[Fact]
					public async Task WhenAnItemLiesAcrossMidnight_ShouldUseTheShorterDistance()
					{
						TimeOnly[] values = [new TimeOnly(23, 59, 30), new TimeOnly(0, 0, 30),];
						ImmutableArray<TimeOnly> subject = [.. values,];

						async Task Act()
							=> await That(subject).All().AreEqualTo(new TimeOnly(23, 59, 30)).Within(1.Minutes());

						await That(Act).DoesNotThrow()
							.Because("the times are compared on the clock face, where 23:59:30 and 00:00:30 are one minute apart");
					}

					[Fact]
					public async Task WhenValuesAreNotWithinTolerance_ShouldFail()
					{
						TimeOnly[] values = [new TimeOnly(14, 0), new TimeOnly(14, 2),];
						ImmutableArray<TimeOnly> subject = [.. values,];
						TimeOnly expected = new TimeOnly(14, 0);

						async Task Act()
							=> await That(subject).All().AreEqualTo(expected).Within(1.Minutes());

						await That(Act).Throws<XunitException>()
							.WithMessage($"""
							              Expected that subject
							              is equal to {Formatter.Format(expected)} ± 1:00 for all items,
							              but only 1 of 2 were

							              Not matching items:
							              {Formatter.Format(new TimeOnly[] { new TimeOnly(14, 2), }, FormattingOptions.MultipleLines)}

							              Collection:
							              {Formatter.Format(values, FormattingOptions.MultipleLines)}
							              """);
					}

					[Fact]
					public async Task WhenValuesAreWithinTolerance_ShouldSucceed()
					{
						TimeOnly[] values = [new TimeOnly(14, 0), new TimeOnly(14, 1),];
						ImmutableArray<TimeOnly> subject = [.. values,];

						async Task Act()
							=> await That(subject).All().AreEqualTo(new TimeOnly(14, 0)).Within(1.Minutes());

						await That(Act).DoesNotThrow();
					}
				}

				public sealed class NullableTimeOnlyTests
				{
					[Fact]
					public async Task WhenAnItemLiesAcrossMidnight_ShouldUseTheShorterDistance()
					{
						TimeOnly?[] values = [new TimeOnly(23, 59, 30), new TimeOnly(0, 0, 30),];
						ImmutableArray<TimeOnly?> subject = [.. values,];

						async Task Act()
							=> await That(subject).All().AreEqualTo(new TimeOnly(23, 59, 30)).Within(1.Minutes());

						await That(Act).DoesNotThrow()
							.Because("the times are compared on the clock face, where 23:59:30 and 00:00:30 are one minute apart");
					}

					[Fact]
					public async Task WhenValuesAreNotWithinTolerance_ShouldFail()
					{
						TimeOnly?[] values = [new TimeOnly(14, 0), new TimeOnly(14, 2),];
						ImmutableArray<TimeOnly?> subject = [.. values,];
						TimeOnly? expected = new TimeOnly(14, 0);

						async Task Act()
							=> await That(subject).All().AreEqualTo(expected).Within(1.Minutes());

						await That(Act).Throws<XunitException>()
							.WithMessage($"""
							              Expected that subject
							              is equal to {Formatter.Format(expected)} ± 1:00 for all items,
							              but only 1 of 2 were

							              Not matching items:
							              {Formatter.Format(new TimeOnly?[] { new TimeOnly(14, 2), }, FormattingOptions.MultipleLines)}

							              Collection:
							              {Formatter.Format(values, FormattingOptions.MultipleLines)}
							              """);
					}

					[Fact]
					public async Task WhenValuesAreWithinTolerance_ShouldSucceed()
					{
						TimeOnly?[] values = [new TimeOnly(14, 0), new TimeOnly(14, 1),];
						ImmutableArray<TimeOnly?> subject = [.. values,];

						async Task Act()
							=> await That(subject).All().AreEqualTo(new TimeOnly(14, 0)).Within(1.Minutes());

						await That(Act).DoesNotThrow();
					}
				}
			}
		}
	}
}
#endif
