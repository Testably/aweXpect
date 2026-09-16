namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public class HasLength
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenActualIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).HasLength(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has length equal to 0,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenExpectedLengthIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				string subject = "";

				async Task Act()
					=> await That(subject).HasLength(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The expected length must be greater than or equal to zero*")
					.AsWildcard().And
					.WithParamName("expected");
			}

			[Theory]
			[InlineData("", 1)]
			[InlineData("abc", 4)]
			[InlineData(" a b c ", 6)]
			public async Task WhenLengthDiffers_ShouldFail(string subject, int length)
			{
				async Task Act()
					=> await That(subject).HasLength(length);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has length equal to {length},
					              but it had length {subject.Length}
					              """);
			}

			[Theory]
			[InlineData("", 0)]
			[InlineData("abc", 3)]
			[InlineData(" a b c ", 7)]
			public async Task WhenLengthMatches_ShouldSucceed(string subject, int length)
			{
				async Task Act()
					=> await That(subject).HasLength(length);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class EqualToTests
		{
			[Fact]
			public async Task WhenActualIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).HasLength().EqualTo(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has length equal to 0,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenExpectedLengthIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				string subject = "";

				async Task Act()
					=> await That(subject).HasLength().EqualTo(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The expected length must be greater than or equal to zero*")
					.AsWildcard().And
					.WithParamName("expected");
			}

			[Theory]
			[InlineData("", 1)]
			[InlineData("abc", 4)]
			[InlineData(" a b c ", 6)]
			public async Task WhenLengthDiffers_ShouldFail(string subject, int length)
			{
				async Task Act()
					=> await That(subject).HasLength().EqualTo(length);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has length equal to {length},
					              but it had length {subject.Length}
					              """);
			}

			[Theory]
			[InlineData("", 0)]
			[InlineData("abc", 3)]
			[InlineData(" a b c ", 7)]
			public async Task WhenLengthMatches_ShouldSucceed(string subject, int length)
			{
				async Task Act()
					=> await That(subject).HasLength().EqualTo(length);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class GreaterThanOrEqualToTests
		{
			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				string subject = "foo";
				int? expected = null;

				async Task Act()
					=> await That(subject).HasLength().GreaterThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has length greater than or equal to <null>,
					             but it had length 3
					             """);
			}

			[Fact]
			public async Task WhenLengthOfSubjectIsGreaterThanExpected_ShouldSucceed()
			{
				string subject = "foo";
				int? expected = 2;

				async Task Act()
					=> await That(subject).HasLength().GreaterThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenLengthOfSubjectIsLessThanExpected_ShouldFail()
			{
				string subject = "foo";
				int? expected = 4;

				async Task Act()
					=> await That(subject).HasLength().GreaterThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has length greater than or equal to {Formatter.Format(expected)},
					              but it had length 3
					              """);
			}

			[Fact]
			public async Task WhenLengthOfSubjectIsTheSameAsExpected_ShouldSucceed()
			{
				string subject = "foo";
				int expected = 3;

				async Task Act()
					=> await That(subject).HasLength().GreaterThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class GreaterThanTests
		{
			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				string subject = "foo";
				int? expected = null;

				async Task Act()
					=> await That(subject).HasLength().GreaterThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has length greater than <null>,
					             but it had length 3
					             """);
			}

			[Fact]
			public async Task WhenLengthOfSubjectIsGreaterThanExpected_ShouldSucceed()
			{
				string subject = "foo";
				int? expected = 2;

				async Task Act()
					=> await That(subject).HasLength().GreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenLengthOfSubjectIsLessThanExpected_ShouldFail()
			{
				string subject = "foo";
				int? expected = 4;

				async Task Act()
					=> await That(subject).HasLength().GreaterThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has length greater than {Formatter.Format(expected)},
					              but it had length 3
					              """);
			}

			[Fact]
			public async Task WhenLengthOfSubjectIsTheSameAsExpected_ShouldFail()
			{
				string subject = "foo";
				int expected = 3;

				async Task Act()
					=> await That(subject).HasLength().GreaterThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has length greater than {Formatter.Format(expected)},
					              but it had length 3
					              """);
			}
		}

		public sealed class LessThanOrEqualToTests
		{
			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				string subject = "foo";
				int? expected = null;

				async Task Act()
					=> await That(subject).HasLength().LessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has length less than or equal to <null>,
					             but it had length 3
					             """);
			}

			[Fact]
			public async Task WhenLengthOfSubjectIsGreaterThanExpected_ShouldFail()
			{
				string subject = "foo";
				int? expected = 2;

				async Task Act()
					=> await That(subject).HasLength().LessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has length less than or equal to {Formatter.Format(expected)},
					              but it had length 3
					              """);
			}

			[Fact]
			public async Task WhenLengthOfSubjectIsLessThanExpected_ShouldSucceed()
			{
				string subject = "foo";
				int? expected = 4;

				async Task Act()
					=> await That(subject).HasLength().LessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenLengthOfSubjectIsTheSameAsExpected_ShouldSucceed()
			{
				string subject = "foo";
				int expected = 3;

				async Task Act()
					=> await That(subject).HasLength().LessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class LessThanTests
		{
			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				string subject = "foo";
				int? expected = null;

				async Task Act()
					=> await That(subject).HasLength().LessThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has length less than <null>,
					             but it had length 3
					             """);
			}

			[Fact]
			public async Task WhenLengthOfSubjectIsGreaterThanExpected_ShouldFail()
			{
				string subject = "foo";
				int? expected = 2;

				async Task Act()
					=> await That(subject).HasLength().LessThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has length less than {Formatter.Format(expected)},
					              but it had length 3
					              """);
			}

			[Fact]
			public async Task WhenLengthOfSubjectIsLessThanExpected_ShouldSucceed()
			{
				string subject = "foo";
				int? expected = 4;

				async Task Act()
					=> await That(subject).HasLength().LessThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenLengthOfSubjectIsTheSameAsExpected_ShouldFail()
			{
				string subject = "foo";
				int expected = 3;

				async Task Act()
					=> await That(subject).HasLength().LessThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has length less than {Formatter.Format(expected)},
					              but it had length 3
					              """);
			}
		}

		public sealed class NotEqualToTests
		{
			[Fact]
			public async Task WhenActualIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).HasLength().NotEqualTo(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has length not equal to 1,
					             but it was <null>
					             """);
			}

			[Theory]
			[InlineData("", 1)]
			[InlineData("abc", 4)]
			[InlineData(" a b c ", 6)]
			public async Task WhenLengthDiffers_ShouldSucceed(string subject, int length)
			{
				async Task Act()
					=> await That(subject).HasLength().NotEqualTo(length);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("", 0)]
			[InlineData("abc", 3)]
			[InlineData(" a b c ", 7)]
			public async Task WhenLengthMatches_ShouldFail(string subject, int length)
			{
				async Task Act()
					=> await That(subject).HasLength().NotEqualTo(length);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has length not equal to {length},
					              but it had length {length}
					              """);
			}

			[Fact]
			public async Task WhenUnexpectedLengthIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				string subject = "";

				async Task Act()
					=> await That(subject).HasLength().NotEqualTo(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The unexpected length must be greater than or equal to zero*")
					.AsWildcard().And
					.WithParamName("unexpected");
			}
		}
	}
}
