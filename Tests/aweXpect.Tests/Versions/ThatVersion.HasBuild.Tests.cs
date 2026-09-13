namespace aweXpect.Tests;

public sealed partial class ThatVersion
{
	public sealed class HasBuild
	{
		public sealed class EqualToTests
		{
			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasBuild().EqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has build equal to <null>,
					             but it had build 12
					             """);
			}

			[Fact]
			public async Task WhenSubjectAndExpectedIsNull_ShouldFail()
			{
				Version? subject = null;
				int? expected = null;

				async Task Act()
					=> await That(subject).HasBuild().EqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has build equal to <null>,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Version? subject = null;
				int? expected = 1;

				async Task Act()
					=> await That(subject).HasBuild().EqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has build equal to 1,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenBuildOfSubjectIsDifferent_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = 13;

				async Task Act()
					=> await That(subject).HasBuild().EqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has build equal to {Formatter.Format(expected)},
					              but it had build 12
					              """);
			}

			[Fact]
			public async Task WhenBuildOfSubjectIsTheSame_ShouldSucceed()
			{
				Version? subject = new(2010, 11, 12, 13);
				int expected = 12;

				async Task Act()
					=> await That(subject).HasBuild().EqualTo(expected);

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
					=> await That(subject).HasBuild().GreaterThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has build greater than or equal to <null>,
					             but it had build 12
					             """);
			}

			[Fact]
			public async Task WhenBuildOfSubjectIsGreaterThanExpected_ShouldSucceed()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = 11;

				async Task Act()
					=> await That(subject).HasBuild().GreaterThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenBuildOfSubjectIsLessThanExpected_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = 13;

				async Task Act()
					=> await That(subject).HasBuild().GreaterThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has build greater than or equal to {Formatter.Format(expected)},
					              but it had build 12
					              """);
			}

			[Fact]
			public async Task WhenBuildOfSubjectIsTheSameAsExpected_ShouldSucceed()
			{
				Version? subject = new(2010, 11, 12, 13);
				int expected = 12;

				async Task Act()
					=> await That(subject).HasBuild().GreaterThanOrEqualTo(expected);

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
					=> await That(subject).HasBuild().GreaterThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has build greater than <null>,
					             but it had build 12
					             """);
			}

			[Fact]
			public async Task WhenBuildOfSubjectIsGreaterThanExpected_ShouldSucceed()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = 11;

				async Task Act()
					=> await That(subject).HasBuild().GreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenBuildOfSubjectIsLessThanExpected_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = 13;

				async Task Act()
					=> await That(subject).HasBuild().GreaterThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has build greater than {Formatter.Format(expected)},
					              but it had build 12
					              """);
			}

			[Fact]
			public async Task WhenBuildOfSubjectIsTheSameAsExpected_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int expected = 12;

				async Task Act()
					=> await That(subject).HasBuild().GreaterThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has build greater than {Formatter.Format(expected)},
					              but it had build 12
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
					=> await That(subject).HasBuild().LessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has build less than or equal to <null>,
					             but it had build 12
					             """);
			}

			[Fact]
			public async Task WhenBuildOfSubjectIsGreaterThanExpected_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = 11;

				async Task Act()
					=> await That(subject).HasBuild().LessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has build less than or equal to {Formatter.Format(expected)},
					              but it had build 12
					              """);
			}

			[Fact]
			public async Task WhenBuildOfSubjectIsLessThanExpected_ShouldSucceed()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = 13;

				async Task Act()
					=> await That(subject).HasBuild().LessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenBuildOfSubjectIsTheSameAsExpected_ShouldSucceed()
			{
				Version? subject = new(2010, 11, 12, 13);
				int expected = 12;

				async Task Act()
					=> await That(subject).HasBuild().LessThanOrEqualTo(expected);

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
					=> await That(subject).HasBuild().LessThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has build less than <null>,
					             but it had build 12
					             """);
			}

			[Fact]
			public async Task WhenBuildOfSubjectIsGreaterThanExpected_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = 11;

				async Task Act()
					=> await That(subject).HasBuild().LessThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has build less than {Formatter.Format(expected)},
					              but it had build 12
					              """);
			}

			[Fact]
			public async Task WhenBuildOfSubjectIsLessThanExpected_ShouldSucceed()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = 13;

				async Task Act()
					=> await That(subject).HasBuild().LessThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenBuildOfSubjectIsTheSameAsExpected_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int expected = 12;

				async Task Act()
					=> await That(subject).HasBuild().LessThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has build less than {Formatter.Format(expected)},
					              but it had build 12
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
					=> await That(subject).HasBuild().NotEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has build not equal to <null>,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Version? subject = null;
				int? unexpected = 1;

				async Task Act()
					=> await That(subject).HasBuild().NotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has build not equal to {unexpected},
					              but it was <null>
					              """);
			}

			[Fact]
			public async Task WhenUnexpectedIsNull_ShouldSucceed()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? unexpected = null;

				async Task Act()
					=> await That(subject).HasBuild().NotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenBuildOfSubjectIsDifferent_ShouldSucceed()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? unexpected = 13;

				async Task Act()
					=> await That(subject).HasBuild().NotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenBuildOfSubjectIsTheSame_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int unexpected = 12;

				async Task Act()
					=> await That(subject).HasBuild().NotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has build not equal to {Formatter.Format(unexpected)},
					              but it had build 12
					              """);
			}
		}

		[Fact]
		public async Task WhenBuildIsUnspecified_ShouldBeMinusOne()
		{
			Version subject = new(1, 2);

			async Task Act()
				=> await That(subject).HasBuild().EqualTo(-1);

			await That(Act).DoesNotThrow();
		}
	}
}
