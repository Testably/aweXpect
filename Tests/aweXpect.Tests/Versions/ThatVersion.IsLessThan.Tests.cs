namespace aweXpect.Tests;

public sealed partial class ThatVersion
{
	public sealed class IsLessThan
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenExpectedIsNull_AndNegated_ShouldFail()
			{
				Version? subject = new(1, 2);
				Version? expected = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsLessThan(expected));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not less than <null>,
					             but it was 1.2
					             """)
					.Because("nothing can be ordered against null, so the negation fails as well");
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				Version? subject = new(1, 2);
				Version? expected = null;

				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is less than <null>,
					             but it was 1.2
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsGreater_ShouldFail()
			{
				Version? subject = new(2, 0);
				Version expected = new(1, 2);

				async Task Act()
					=> await That(subject).IsLessThan(expected)
						.Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is less than 1.2, because we want to test the failure,
					             but it was 2.0
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsLess_ShouldSucceed()
			{
				Version? subject = new(1, 2);
				Version expected = new(2, 0);

				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Version? subject = null;

				async Task Act()
					=> await That(subject).IsLessThan(new Version(1, 2));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is less than 1.2,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsTheSame_ShouldFail()
			{
				Version? subject = new(1, 2, 3, 4);
				Version expected = new(1, 2, 3, 4);

				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is less than 1.2.3.4,
					             but it was 1.2.3.4
					             """);
			}

			[Fact]
			public async Task WhenSubjectOmitsAComponentOfExpected_ShouldSucceed()
			{
				Version? subject = new(1, 2);
				Version expected = new(1, 2, 0);

				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).DoesNotThrow()
					.Because("an unspecified build compares as less than an explicit zero");
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenSubjectIsGreater_ShouldSucceed()
			{
				Version subject = new(1, 2, 3, 4);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsLessThan(new Version(1, 2)));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsLess_ShouldFail()
			{
				Version subject = new(1, 2, 3, 4);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsLessThan(new Version(2, 0)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not less than 2.0,
					             but it was 1.2.3.4
					             """);
			}
		}
	}
}
