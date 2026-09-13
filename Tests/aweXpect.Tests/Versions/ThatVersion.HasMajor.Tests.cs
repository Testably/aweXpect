namespace aweXpect.Tests;

public sealed partial class ThatVersion
{
	public sealed class HasMajor
	{
		public sealed class EqualToTests
		{
			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasMajor().EqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has major equal to <null>,
					             but it had major 2010
					             """);
			}

			[Fact]
			public async Task WhenSubjectAndExpectedIsNull_ShouldFail()
			{
				Version? subject = null;
				int? expected = null;

				async Task Act()
					=> await That(subject).HasMajor().EqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has major equal to <null>,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Version? subject = null;
				int? expected = 1;

				async Task Act()
					=> await That(subject).HasMajor().EqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has major equal to 1,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenMajorOfSubjectIsDifferent_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = 2011;

				async Task Act()
					=> await That(subject).HasMajor().EqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has major equal to {Formatter.Format(expected)},
					              but it had major 2010
					              """);
			}

			[Fact]
			public async Task WhenMajorOfSubjectIsTheSame_ShouldSucceed()
			{
				Version? subject = new(2010, 11, 12, 13);
				int expected = 2010;

				async Task Act()
					=> await That(subject).HasMajor().EqualTo(expected);

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
					=> await That(subject).HasMajor().GreaterThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has major greater than or equal to <null>,
					             but it had major 2010
					             """);
			}

			[Fact]
			public async Task WhenMajorOfSubjectIsGreaterThanExpected_ShouldSucceed()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = 2009;

				async Task Act()
					=> await That(subject).HasMajor().GreaterThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMajorOfSubjectIsLessThanExpected_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = 2011;

				async Task Act()
					=> await That(subject).HasMajor().GreaterThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has major greater than or equal to {Formatter.Format(expected)},
					              but it had major 2010
					              """);
			}

			[Fact]
			public async Task WhenMajorOfSubjectIsTheSameAsExpected_ShouldSucceed()
			{
				Version? subject = new(2010, 11, 12, 13);
				int expected = 2010;

				async Task Act()
					=> await That(subject).HasMajor().GreaterThanOrEqualTo(expected);

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
					=> await That(subject).HasMajor().GreaterThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has major greater than <null>,
					             but it had major 2010
					             """);
			}

			[Fact]
			public async Task WhenMajorOfSubjectIsGreaterThanExpected_ShouldSucceed()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = 2009;

				async Task Act()
					=> await That(subject).HasMajor().GreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMajorOfSubjectIsLessThanExpected_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = 2011;

				async Task Act()
					=> await That(subject).HasMajor().GreaterThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has major greater than {Formatter.Format(expected)},
					              but it had major 2010
					              """);
			}

			[Fact]
			public async Task WhenMajorOfSubjectIsTheSameAsExpected_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int expected = 2010;

				async Task Act()
					=> await That(subject).HasMajor().GreaterThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has major greater than {Formatter.Format(expected)},
					              but it had major 2010
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
					=> await That(subject).HasMajor().LessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has major less than or equal to <null>,
					             but it had major 2010
					             """);
			}

			[Fact]
			public async Task WhenMajorOfSubjectIsGreaterThanExpected_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = 2009;

				async Task Act()
					=> await That(subject).HasMajor().LessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has major less than or equal to {Formatter.Format(expected)},
					              but it had major 2010
					              """);
			}

			[Fact]
			public async Task WhenMajorOfSubjectIsLessThanExpected_ShouldSucceed()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = 2011;

				async Task Act()
					=> await That(subject).HasMajor().LessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMajorOfSubjectIsTheSameAsExpected_ShouldSucceed()
			{
				Version? subject = new(2010, 11, 12, 13);
				int expected = 2010;

				async Task Act()
					=> await That(subject).HasMajor().LessThanOrEqualTo(expected);

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
					=> await That(subject).HasMajor().LessThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has major less than <null>,
					             but it had major 2010
					             """);
			}

			[Fact]
			public async Task WhenMajorOfSubjectIsGreaterThanExpected_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = 2009;

				async Task Act()
					=> await That(subject).HasMajor().LessThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has major less than {Formatter.Format(expected)},
					              but it had major 2010
					              """);
			}

			[Fact]
			public async Task WhenMajorOfSubjectIsLessThanExpected_ShouldSucceed()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = 2011;

				async Task Act()
					=> await That(subject).HasMajor().LessThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMajorOfSubjectIsTheSameAsExpected_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int expected = 2010;

				async Task Act()
					=> await That(subject).HasMajor().LessThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has major less than {Formatter.Format(expected)},
					              but it had major 2010
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
					=> await That(subject).HasMajor().NotEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has major not equal to <null>,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Version? subject = null;
				int? unexpected = 1;

				async Task Act()
					=> await That(subject).HasMajor().NotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has major not equal to {unexpected},
					              but it was <null>
					              """);
			}

			[Fact]
			public async Task WhenUnexpectedIsNull_ShouldSucceed()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? unexpected = null;

				async Task Act()
					=> await That(subject).HasMajor().NotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMajorOfSubjectIsDifferent_ShouldSucceed()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? unexpected = 2011;

				async Task Act()
					=> await That(subject).HasMajor().NotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMajorOfSubjectIsTheSame_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int unexpected = 2010;

				async Task Act()
					=> await That(subject).HasMajor().NotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has major not equal to {Formatter.Format(unexpected)},
					              but it had major 2010
					              """);
			}
		}
	}
}
