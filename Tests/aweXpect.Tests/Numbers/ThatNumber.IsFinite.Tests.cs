using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect.Tests;

public sealed partial class ThatNumber
{
	public sealed class IsFinite
	{
		public sealed class Tests
		{
			[Test]
			public async Task ForDouble_ShouldSupportChaining()
			{
				double subject = double.Epsilon;

				async Task Act()
					=> await That(subject).IsFinite()
						.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(double.PositiveInfinity)]
			[Arguments(double.NegativeInfinity)]
			[Arguments(double.NaN)]
			public async Task ForDouble_WhenSubjectIsInfinityOrNaN_ShouldFail(
				double subject)
			{
				async Task Act() => await That(subject).IsFinite();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is finite,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments(-1d)]
			[Arguments(0d)]
			[Arguments(1d)]
			[Arguments(double.MinValue)]
			[Arguments(double.MaxValue)]
			[Arguments(double.Epsilon)]
			public async Task ForDouble_WhenSubjectIsNormalValue_ShouldSucceed(double subject)
			{
				async Task Act()
					=> await That(subject).IsFinite();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForFloat_ShouldSupportChaining()
			{
				float subject = float.Epsilon;

				async Task Act() => await That(subject).IsFinite()
					.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(float.PositiveInfinity)]
			[Arguments(float.NegativeInfinity)]
			[Arguments(float.NaN)]
			public async Task ForFloat_WhenSubjectIsInfinityOrNaN_ShouldFail(
				float subject)
			{
				async Task Act()
					=> await That(subject).IsFinite();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is finite,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments(-1f)]
			[Arguments(0f)]
			[Arguments(1f)]
			[Arguments(float.MinValue)]
			[Arguments(float.MaxValue)]
			[Arguments(float.Epsilon)]
			public async Task ForFloat_WhenSubjectIsNormalValue_ShouldSucceed(float subject)
			{
				async Task Act()
					=> await That(subject).IsFinite();

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task ForHalf_ShouldSupportChaining()
			{
				Half subject = Half.Epsilon;

				async Task Act()
					=> await That(subject).IsFinite()
						.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			[MethodDataSource(nameof(GetNaNOrInfinityHalfValues))]
			public async Task ForHalf_WhenSubjectIsInfinityOrNaN_ShouldFail(Half subject)
			{
				async Task Act()
					=> await That(subject).IsFinite();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is finite,
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			[MethodDataSource(nameof(GetNormalHalfValues))]
			public async Task ForHalf_WhenSubjectIsNormalValue_ShouldSucceed(Half subject)
			{
				async Task Act()
					=> await That(subject).IsFinite();

				await That(Act).DoesNotThrow();
			}
#endif

			[Test]
			public async Task ForNullableDouble_ShouldReturnTheNotNullSubject()
			{
				double? subject = 1.0;

				AndOrResult<double, IThat<double?>> result = That(subject).IsFinite();

				await That(await result).IsEqualTo(subject)
					.Because("the nullable IsFinite narrows the subject on every target framework");
			}

			[Test]
			public async Task ForNullableDouble_ShouldSupportChaining()
			{
				double? subject = double.Epsilon;

				async Task Act()
					=> await That(subject).IsFinite()
						.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(double.PositiveInfinity)]
			[Arguments(double.NegativeInfinity)]
			[Arguments(double.NaN)]
			[Arguments(null)]
			public async Task ForNullableDouble_WhenSubjectIsInfinityOrNaNOrNull_ShouldFail(
				double? subject)
			{
				async Task Act() => await That(subject).IsFinite();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is finite,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments(-1d)]
			[Arguments(0d)]
			[Arguments(1d)]
			[Arguments(double.MinValue)]
			[Arguments(double.MaxValue)]
			[Arguments(double.Epsilon)]
			public async Task ForNullableDouble_WhenSubjectIsNormalValue_ShouldSucceed(double? subject)
			{
				async Task Act()
					=> await That(subject).IsFinite();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForNullableFloat_ShouldReturnTheNotNullSubject()
			{
				float? subject = 1.0f;

				AndOrResult<float, IThat<float?>> result = That(subject).IsFinite();

				await That(await result).IsEqualTo(subject)
					.Because("the nullable IsFinite narrows the subject on every target framework");
			}

			[Test]
			public async Task ForNullableFloat_ShouldSupportChaining()
			{
				float? subject = float.Epsilon;

				async Task Act() => await That(subject).IsFinite()
					.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(float.PositiveInfinity)]
			[Arguments(float.NegativeInfinity)]
			[Arguments(float.NaN)]
			[Arguments(null)]
			public async Task ForNullableFloat_WhenSubjectIsInfinityOrNaNOrNull_ShouldFail(
				float? subject)
			{
				async Task Act()
					=> await That(subject).IsFinite();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is finite,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments(-1f)]
			[Arguments(0f)]
			[Arguments(1f)]
			[Arguments(float.MinValue)]
			[Arguments(float.MaxValue)]
			[Arguments(float.Epsilon)]
			public async Task ForNullableFloat_WhenSubjectIsNormalValue_ShouldSucceed(float? subject)
			{
				async Task Act()
					=> await That(subject).IsFinite();

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task ForNullableHalf_ShouldSupportChaining()
			{
				Half? subject = Half.Epsilon;

				async Task Act()
					=> await That(subject).IsFinite()
						.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			[MethodDataSource(nameof(GetNaNOrInfinityHalfValues))]
			public async Task ForNullableHalf_WhenSubjectIsInfinityOrNaN_ShouldFail(
				Half subjectValue)
			{
				Half? subject = subjectValue;

				async Task Act() => await That(subject).IsFinite();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is finite,
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			[MethodDataSource(nameof(GetNormalHalfValues))]
			public async Task ForNullableHalf_WhenSubjectIsNormalValue_ShouldSucceed(
				Half subjectValue)
			{
				Half? subject = subjectValue;

				async Task Act()
					=> await That(subject).IsFinite();

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			public async Task ForNullableHalf_WhenSubjectIsNull_ShouldFail()
			{
				Half? subject = null;

				async Task Act()
					=> await That(subject).IsFinite();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is finite,
					             but it was <null>
					             """);
			}
#endif

#if NET8_0_OR_GREATER
			public static IEnumerable<Half> GetNormalHalfValues() =>
			[
				(Half)0.0,
				(Half)1.0,
				Half.MinValue,
				Half.MaxValue,
				Half.Epsilon,
			];

			public static IEnumerable<Half> GetNaNOrInfinityHalfValues() =>
			[
				Half.NaN,
				Half.NegativeInfinity,
				Half.PositiveInfinity,
			];
#endif
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments(double.PositiveInfinity)]
			[Arguments(double.NegativeInfinity)]
			[Arguments(double.NaN)]
			public async Task ForDouble_WhenSubjectIsInfinityOrNaN_ShouldSucceed(double subject)
			{
				async Task Act() => await That(subject).DoesNotComplyWith(it =>
					it.IsFinite());

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(-1d)]
			[Arguments(0d)]
			[Arguments(1d)]
			[Arguments(double.MinValue)]
			[Arguments(double.MaxValue)]
			[Arguments(double.Epsilon)]
			public async Task ForDouble_WhenSubjectIsNormalValue_ShouldFail(double subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it =>
						it.IsFinite());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not finite,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments(double.PositiveInfinity)]
			[Arguments(double.NegativeInfinity)]
			[Arguments(double.NaN)]
			public async Task ForNullableDouble_WhenSubjectIsInfinityNaN_ShouldSucceed(double? subject)
			{
				async Task Act() => await That(subject).DoesNotComplyWith(it =>
					it.IsFinite());

				await That(Act).DoesNotThrow();
			}


			[Test]
			[Arguments(-1d)]
			[Arguments(0d)]
			[Arguments(1d)]
			[Arguments(double.MinValue)]
			[Arguments(double.MaxValue)]
			[Arguments(double.Epsilon)]
			public async Task ForNullableDouble_WhenSubjectIsNormalValue_ShouldFail(
				double? subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it =>
						it.IsFinite());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not finite,
					              but it was {Formatter.Format(subject)}
					              """);
			}
			[Test]
			public async Task ForNullableDouble_WhenSubjectIsNull_ShouldFail()
			{
				double? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsFinite());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not finite,
					             but it was <null>
					             """);
			}
		}
	}
}
