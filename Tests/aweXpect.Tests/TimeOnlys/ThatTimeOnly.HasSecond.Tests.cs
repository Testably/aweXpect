#if NET8_0_OR_GREATER
namespace aweXpect.Tests;

public sealed partial class ThatTimeOnly
{
	public sealed class HasSecond
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenSecondOfSubjectIsDifferent_ShouldFail()
			{
				TimeOnly subject = new(13, 14, 15);
				int expected = 14;

				async Task Act()
					=> await That(subject).HasSecond(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has second equal to {Formatter.Format(expected)},
					              but it had second 15
					              """);
			}

			[Fact]
			public async Task WhenSecondOfSubjectIsTheSame_ShouldSucceed()
			{
				TimeOnly subject = new(13, 14, 15);
				int expected = 15;

				async Task Act()
					=> await That(subject).HasSecond(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class EqualToTests
		{
			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				TimeOnly subject = new(13, 14, 15);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasSecond().EqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has second equal to <null>,
					             but it had second 15
					             """);
			}

			[Fact]
			public async Task WhenSecondOfSubjectIsDifferent_ShouldFail()
			{
				TimeOnly subject = new(13, 14, 15);
				int? expected = 14;

				async Task Act()
					=> await That(subject).HasSecond().EqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has second equal to {Formatter.Format(expected)},
					              but it had second 15
					              """);
			}

			[Fact]
			public async Task WhenSecondOfSubjectIsTheSame_ShouldSucceed()
			{
				TimeOnly subject = new(13, 14, 15);
				int expected = 15;

				async Task Act()
					=> await That(subject).HasSecond().EqualTo(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class GreaterThanOrEqualToTests
		{
			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				TimeOnly subject = new(13, 14, 15);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasSecond().GreaterThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has second greater than or equal to <null>,
					             but it had second 15
					             """);
			}

			[Fact]
			public async Task WhenSecondOfSubjectIsGreaterThanExpected_ShouldSucceed()
			{
				TimeOnly subject = new(13, 14, 15);
				int? expected = 14;

				async Task Act()
					=> await That(subject).HasSecond().GreaterThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSecondOfSubjectIsLessThanExpected_ShouldFail()
			{
				TimeOnly subject = new(13, 14, 15);
				int? expected = 16;

				async Task Act()
					=> await That(subject).HasSecond().GreaterThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has second greater than or equal to {Formatter.Format(expected)},
					              but it had second 15
					              """);
			}

			[Fact]
			public async Task WhenSecondOfSubjectIsTheSameAsExpected_ShouldSucceed()
			{
				TimeOnly subject = new(13, 14, 15);
				int expected = 15;

				async Task Act()
					=> await That(subject).HasSecond().GreaterThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class GreaterThanTests
		{
			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				TimeOnly subject = new(13, 14, 15);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasSecond().GreaterThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has second greater than <null>,
					             but it had second 15
					             """);
			}

			[Fact]
			public async Task WhenSecondOfSubjectIsGreaterThanExpected_ShouldSucceed()
			{
				TimeOnly subject = new(13, 14, 15);
				int? expected = 14;

				async Task Act()
					=> await That(subject).HasSecond().GreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSecondOfSubjectIsLessThanExpected_ShouldFail()
			{
				TimeOnly subject = new(13, 14, 15);
				int? expected = 16;

				async Task Act()
					=> await That(subject).HasSecond().GreaterThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has second greater than {Formatter.Format(expected)},
					              but it had second 15
					              """);
			}

			[Fact]
			public async Task WhenSecondOfSubjectIsTheSameAsExpected_ShouldFail()
			{
				TimeOnly subject = new(13, 14, 15);
				int expected = 15;

				async Task Act()
					=> await That(subject).HasSecond().GreaterThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has second greater than {Formatter.Format(expected)},
					              but it had second 15
					              """);
			}
		}

		public sealed class LessThanOrEqualToTests
		{
			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				TimeOnly subject = new(13, 14, 15);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasSecond().LessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has second less than or equal to <null>,
					             but it had second 15
					             """);
			}

			[Fact]
			public async Task WhenSecondOfSubjectIsGreaterThanExpected_ShouldFail()
			{
				TimeOnly subject = new(13, 14, 15);
				int? expected = 14;

				async Task Act()
					=> await That(subject).HasSecond().LessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has second less than or equal to {Formatter.Format(expected)},
					              but it had second 15
					              """);
			}

			[Fact]
			public async Task WhenSecondOfSubjectIsLessThanExpected_ShouldSucceed()
			{
				TimeOnly subject = new(13, 14, 15);
				int? expected = 16;

				async Task Act()
					=> await That(subject).HasSecond().LessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSecondOfSubjectIsTheSameAsExpected_ShouldSucceed()
			{
				TimeOnly subject = new(13, 14, 15);
				int expected = 15;

				async Task Act()
					=> await That(subject).HasSecond().LessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class LessThanTests
		{
			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				TimeOnly subject = new(13, 14, 15);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasSecond().LessThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has second less than <null>,
					             but it had second 15
					             """);
			}

			[Fact]
			public async Task WhenSecondOfSubjectIsGreaterThanExpected_ShouldFail()
			{
				TimeOnly subject = new(13, 14, 15);
				int? expected = 14;

				async Task Act()
					=> await That(subject).HasSecond().LessThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has second less than {Formatter.Format(expected)},
					              but it had second 15
					              """);
			}

			[Fact]
			public async Task WhenSecondOfSubjectIsLessThanExpected_ShouldSucceed()
			{
				TimeOnly subject = new(13, 14, 15);
				int? expected = 16;

				async Task Act()
					=> await That(subject).HasSecond().LessThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSecondOfSubjectIsTheSameAsExpected_ShouldFail()
			{
				TimeOnly subject = new(13, 14, 15);
				int expected = 15;

				async Task Act()
					=> await That(subject).HasSecond().LessThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has second less than {Formatter.Format(expected)},
					              but it had second 15
					              """);
			}
		}

		public sealed class NotEqualToTests
		{
			[Fact]
			public async Task WhenSecondOfSubjectIsDifferent_ShouldSucceed()
			{
				TimeOnly subject = new(13, 14, 15);
				int? unexpected = 14;

				async Task Act()
					=> await That(subject).HasSecond().NotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSecondOfSubjectIsTheSame_ShouldFail()
			{
				TimeOnly subject = new(13, 14, 15);
				int unexpected = 15;

				async Task Act()
					=> await That(subject).HasSecond().NotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has second not equal to {Formatter.Format(unexpected)},
					              but it had second 15
					              """);
			}

			[Fact]
			public async Task WhenUnexpectedIsNull_ShouldSucceed()
			{
				TimeOnly subject = new(13, 14, 15);
				int? unexpected = null;

				async Task Act()
					=> await That(subject).HasSecond().NotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
#endif
