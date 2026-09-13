namespace aweXpect.Tests;

public sealed partial class ThatVersion
{
	public sealed class HasMinor
	{
		public sealed class EqualToTests
		{
			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasMinor().EqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has minor equal to <null>,
					             but it had minor 11
					             """);
			}

			[Fact]
			public async Task WhenSubjectAndExpectedIsNull_ShouldFail()
			{
				Version? subject = null;
				int? expected = null;

				async Task Act()
					=> await That(subject).HasMinor().EqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has minor equal to <null>,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Version? subject = null;
				int? expected = 1;

				async Task Act()
					=> await That(subject).HasMinor().EqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has minor equal to 1,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenMinorOfSubjectIsDifferent_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = 12;

				async Task Act()
					=> await That(subject).HasMinor().EqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has minor equal to {Formatter.Format(expected)},
					              but it had minor 11
					              """);
			}

			[Fact]
			public async Task WhenMinorOfSubjectIsTheSame_ShouldSucceed()
			{
				Version? subject = new(2010, 11, 12, 13);
				int expected = 11;

				async Task Act()
					=> await That(subject).HasMinor().EqualTo(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class GreaterThanOrEqualToTests
		{
			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasMinor().GreaterThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has minor greater than or equal to <null>,
					             but it had minor 11
					             """);
			}

			[Fact]
			public async Task WhenMinorOfSubjectIsGreaterThanExpected_ShouldSucceed()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = 10;

				async Task Act()
					=> await That(subject).HasMinor().GreaterThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMinorOfSubjectIsLessThanExpected_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = 12;

				async Task Act()
					=> await That(subject).HasMinor().GreaterThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has minor greater than or equal to {Formatter.Format(expected)},
					              but it had minor 11
					              """);
			}

			[Fact]
			public async Task WhenMinorOfSubjectIsTheSameAsExpected_ShouldSucceed()
			{
				Version? subject = new(2010, 11, 12, 13);
				int expected = 11;

				async Task Act()
					=> await That(subject).HasMinor().GreaterThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class GreaterThanTests
		{
			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasMinor().GreaterThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has minor greater than <null>,
					             but it had minor 11
					             """);
			}

			[Fact]
			public async Task WhenMinorOfSubjectIsGreaterThanExpected_ShouldSucceed()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = 10;

				async Task Act()
					=> await That(subject).HasMinor().GreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMinorOfSubjectIsLessThanExpected_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = 12;

				async Task Act()
					=> await That(subject).HasMinor().GreaterThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has minor greater than {Formatter.Format(expected)},
					              but it had minor 11
					              """);
			}

			[Fact]
			public async Task WhenMinorOfSubjectIsTheSameAsExpected_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int expected = 11;

				async Task Act()
					=> await That(subject).HasMinor().GreaterThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has minor greater than {Formatter.Format(expected)},
					              but it had minor 11
					              """);
			}
		}

		public sealed class LessThanOrEqualToTests
		{
			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasMinor().LessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has minor less than or equal to <null>,
					             but it had minor 11
					             """);
			}

			[Fact]
			public async Task WhenMinorOfSubjectIsGreaterThanExpected_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = 10;

				async Task Act()
					=> await That(subject).HasMinor().LessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has minor less than or equal to {Formatter.Format(expected)},
					              but it had minor 11
					              """);
			}

			[Fact]
			public async Task WhenMinorOfSubjectIsLessThanExpected_ShouldSucceed()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = 12;

				async Task Act()
					=> await That(subject).HasMinor().LessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMinorOfSubjectIsTheSameAsExpected_ShouldSucceed()
			{
				Version? subject = new(2010, 11, 12, 13);
				int expected = 11;

				async Task Act()
					=> await That(subject).HasMinor().LessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class LessThanTests
		{
			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasMinor().LessThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has minor less than <null>,
					             but it had minor 11
					             """);
			}

			[Fact]
			public async Task WhenMinorOfSubjectIsGreaterThanExpected_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = 10;

				async Task Act()
					=> await That(subject).HasMinor().LessThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has minor less than {Formatter.Format(expected)},
					              but it had minor 11
					              """);
			}

			[Fact]
			public async Task WhenMinorOfSubjectIsLessThanExpected_ShouldSucceed()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = 12;

				async Task Act()
					=> await That(subject).HasMinor().LessThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMinorOfSubjectIsTheSameAsExpected_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int expected = 11;

				async Task Act()
					=> await That(subject).HasMinor().LessThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has minor less than {Formatter.Format(expected)},
					              but it had minor 11
					              """);
			}
		}

		public sealed class NotEqualToTests
		{
			[Fact]
			public async Task WhenSubjectAndUnexpectedIsNull_ShouldFail()
			{
				Version? subject = null;
				int? expected = null;

				async Task Act()
					=> await That(subject).HasMinor().NotEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has minor not equal to <null>,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Version? subject = null;
				int? unexpected = 1;

				async Task Act()
					=> await That(subject).HasMinor().NotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has minor not equal to {unexpected},
					              but it was <null>
					              """);
			}

			[Fact]
			public async Task WhenUnexpectedIsNull_ShouldSucceed()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? unexpected = null;

				async Task Act()
					=> await That(subject).HasMinor().NotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMinorOfSubjectIsDifferent_ShouldSucceed()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? unexpected = 12;

				async Task Act()
					=> await That(subject).HasMinor().NotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMinorOfSubjectIsTheSame_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int unexpected = 11;

				async Task Act()
					=> await That(subject).HasMinor().NotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has minor not equal to {Formatter.Format(unexpected)},
					              but it had minor 11
					              """);
			}
		}
	}
}
