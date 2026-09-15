#if NET8_0_OR_GREATER
using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class All
	{
		public sealed partial class AreEqualTo
		{
			public sealed class Within
			{
				public sealed class DoubleTests
				{
					[Fact]
					public async Task WhenValuesAreNotWithinTolerance_ShouldFail()
					{
						IAsyncEnumerable<double> subject = ToAsyncEnumerable(1.0, 1.3, 0.9);

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
						IAsyncEnumerable<double> subject = ToAsyncEnumerable(1.0, 1.1, 0.9);

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
						IAsyncEnumerable<double?> subject = ToAsyncEnumerable<double?>(1.0, 1.3, 0.9);

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
						IAsyncEnumerable<double?> subject = ToAsyncEnumerable<double?>(1.0, 1.1, 0.9);

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
						IAsyncEnumerable<float> subject = ToAsyncEnumerable(1.0F, 1.3F, 0.9F);

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
						IAsyncEnumerable<float> subject = ToAsyncEnumerable(1.0F, 1.1F, 0.9F);

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
						IAsyncEnumerable<float?> subject = ToAsyncEnumerable<float?>(1.0F, 1.3F, 0.9F);

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
						IAsyncEnumerable<float?> subject = ToAsyncEnumerable<float?>(1.0F, 1.1F, 0.9F);

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
						IAsyncEnumerable<decimal> subject = ToAsyncEnumerable(1.0m, 1.3m, 0.9m);

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
						IAsyncEnumerable<decimal> subject = ToAsyncEnumerable(1.0m, 1.1m, 0.9m);

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
						IAsyncEnumerable<decimal?> subject = ToAsyncEnumerable<decimal?>(1.0m, 1.3m, 0.9m);

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
						IAsyncEnumerable<decimal?> subject = ToAsyncEnumerable<decimal?>(1.0m, 1.1m, 0.9m);

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
						IAsyncEnumerable<DateTime> subject =
							ToAsyncEnumerable(now.AddMinutes(1), now, now.AddMinutes(-2));

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
						IAsyncEnumerable<DateTime> subject =
							ToAsyncEnumerable(now.AddMinutes(1), now, now.AddMinutes(-1));

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
						IAsyncEnumerable<DateTime?> subject =
							ToAsyncEnumerable<DateTime?>(now.AddMinutes(1), now, null, now.AddMinutes(-2));

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
						IAsyncEnumerable<DateTime?> subject =
							ToAsyncEnumerable<DateTime?>(now.AddMinutes(1), now, now.AddMinutes(-1));

						async Task Act()
							=> await That(subject).All().AreEqualTo(now).Within(1.Minutes());

						await That(Act).DoesNotThrow();
					}
				}
			}
		}
	}
}
#endif
