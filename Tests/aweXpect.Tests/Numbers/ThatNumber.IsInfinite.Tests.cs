namespace aweXpect.Tests;

public sealed partial class ThatNumber
{
	public sealed class IsInfinite
	{
		public sealed class Tests
		{
			[Test]
			public async Task ForDouble_ShouldSupportChaining()
			{
				double subject = double.PositiveInfinity;

				async Task Act()
					=> await That(subject).IsInfinite()
						.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(double.PositiveInfinity)]
			[Arguments(double.NegativeInfinity)]
			public async Task ForDouble_WhenSubjectIsInfinity_ShouldSucceed(double subject)
			{
				async Task Act() => await That(subject).IsInfinite();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(-1d)]
			[Arguments(0d)]
			[Arguments(1d)]
			[Arguments(double.MinValue)]
			[Arguments(double.MaxValue)]
			[Arguments(double.Epsilon)]
			[Arguments(double.NaN)]
			public async Task ForDouble_WhenSubjectIsNormalOrNaNValue_ShouldFail(double subject)
			{
				async Task Act()
					=> await That(subject).IsInfinite();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is infinite,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task ForFloat_ShouldSupportChaining()
			{
				float subject = float.PositiveInfinity;

				async Task Act() => await That(subject).IsInfinite()
					.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(float.PositiveInfinity)]
			[Arguments(float.NegativeInfinity)]
			public async Task ForFloat_WhenSubjectIsInfinity_ShouldSucceed(float subject)
			{
				async Task Act()
					=> await That(subject).IsInfinite();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(-1f)]
			[Arguments(0f)]
			[Arguments(1f)]
			[Arguments(float.MinValue)]
			[Arguments(float.MaxValue)]
			[Arguments(float.Epsilon)]
			[Arguments(float.NaN)]
			public async Task ForFloat_WhenSubjectIsNormalOrNaNValue_ShouldFail(float subject)
			{
				async Task Act()
					=> await That(subject).IsInfinite();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is infinite,
					              but it was {Formatter.Format(subject)}
					              """);
			}
#if NET8_0_OR_GREATER
			[Test]
			public async Task ForHalf_ShouldSupportChaining()
			{
				Half subject = Half.PositiveInfinity;

				async Task Act()
					=> await That(subject).IsInfinite()
						.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			[MethodDataSource(nameof(GetInfinityHalfValues))]
			public async Task ForHalf_WhenSubjectIsInfinity_ShouldSucceed(Half subject)
			{
				async Task Act()
					=> await That(subject).IsInfinite();

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			[MethodDataSource(nameof(GetNormalOrNaNHalfValues))]
			public async Task ForHalf_WhenSubjectIsNormalOrNaNValue_ShouldFail(Half subject)
			{
				async Task Act()
					=> await That(subject).IsInfinite();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is infinite,
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif

			[Test]
			public async Task ForNullableDouble_ShouldSupportChaining()
			{
				double? subject = double.PositiveInfinity;

				async Task Act()
					=> await That(subject).IsInfinite()
						.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(double.PositiveInfinity)]
			[Arguments(double.NegativeInfinity)]
			public async Task ForNullableDouble_WhenSubjectIsInfinity_ShouldSucceed(double? subject)
			{
				async Task Act() => await That(subject).IsInfinite();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(-1d)]
			[Arguments(0d)]
			[Arguments(1d)]
			[Arguments(double.MinValue)]
			[Arguments(double.MaxValue)]
			[Arguments(double.Epsilon)]
			[Arguments(double.NaN)]
			[Arguments(null)]
			public async Task ForNullableDouble_WhenSubjectIsNormalOrNaNValueOrNull_ShouldFail(
				double? subject)
			{
				async Task Act()
					=> await That(subject).IsInfinite();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is infinite,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task ForNullableFloat_ShouldSupportChaining()
			{
				float? subject = float.PositiveInfinity;

				async Task Act() => await That(subject).IsInfinite()
					.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(float.PositiveInfinity)]
			[Arguments(float.NegativeInfinity)]
			public async Task ForNullableFloat_WhenSubjectIsInfinity_ShouldSucceed(float? subject)
			{
				async Task Act()
					=> await That(subject).IsInfinite();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(-1f)]
			[Arguments(0f)]
			[Arguments(1f)]
			[Arguments(float.MinValue)]
			[Arguments(float.MaxValue)]
			[Arguments(float.Epsilon)]
			[Arguments(float.NaN)]
			[Arguments(null)]
			public async Task ForNullableFloat_WhenSubjectIsNormalOrNaNValueOrNull_ShouldFail(float? subject)
			{
				async Task Act()
					=> await That(subject).IsInfinite();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is infinite,
					              but it was {Formatter.Format(subject)}
					              """);
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task ForNullableHalf_ShouldSupportChaining()
			{
				Half? subject = Half.NegativeInfinity;

				async Task Act()
					=> await That(subject).IsInfinite()
						.And.IsEqualTo(subject);

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			[MethodDataSource(nameof(GetInfinityHalfValues))]
			public async Task ForNullableHalf_WhenSubjectIsInfinity_ShouldSucceed(
				Half subjectValue)
			{
				Half? subject = subjectValue;

				async Task Act() => await That(subject).IsInfinite();

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			[MethodDataSource(nameof(GetNormalOrNaNHalfValues))]
			public async Task ForNullableHalf_WhenSubjectIsNormalOrNaNValue_ShouldFail(
				Half subjectValue)
			{
				Half? subject = subjectValue;

				async Task Act()
					=> await That(subject).IsInfinite();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is infinite,
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			public async Task ForNullableHalf_WhenSubjectIsNull_ShouldFail()
			{
				Half? subject = null;

				async Task Act()
					=> await That(subject).IsInfinite();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is infinite,
					             but it was <null>
					             """);
			}
#endif

#if NET8_0_OR_GREATER
			public static TheoryData<Half> GetNormalOrNaNHalfValues() => new(
				(Half)0.0,
				(Half)1.0,
				Half.MinValue,
				Half.MaxValue,
				Half.Epsilon,
				Half.NaN);

			public static TheoryData<Half> GetInfinityHalfValues() => new(
				Half.NegativeInfinity,
				Half.PositiveInfinity);
#endif
		}
		
		public sealed class NegatedTests
		{
			[Test]
			[Arguments(double.PositiveInfinity)]
			[Arguments(double.NegativeInfinity)]
			public async Task ForDouble_WhenSubjectIsInfinity_ShouldFail(double subject)
			{
				async Task Act() => await That(subject).DoesNotComplyWith(it => 
					it.IsInfinite());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not infinite,
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
			[Arguments(double.NaN)]
			public async Task ForDouble_WhenSubjectIsNormalOrNaNValue_ShouldSucceed(double subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => 
						it.IsInfinite());

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(double.PositiveInfinity)]
			[Arguments(double.NegativeInfinity)]
			public async Task ForNullableDouble_WhenSubjectIsInfinity_ShouldFail(double? subject)
			{
				async Task Act() => await That(subject).DoesNotComplyWith(it => 
					it.IsInfinite());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not infinite,
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
			[Arguments(double.NaN)]
			public async Task ForNullableDouble_WhenSubjectIsNormalOrNaNValue_ShouldSucceed(
				double? subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => 
						it.IsInfinite());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForNullableDouble_WhenSubjectIsNull_ShouldFail()
			{
				double? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsInfinite());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not infinite,
					             but it was <null>
					             """);
			}
		}
	}
}
