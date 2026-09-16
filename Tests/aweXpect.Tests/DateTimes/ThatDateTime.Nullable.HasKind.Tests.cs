namespace aweXpect.Tests;

public sealed partial class ThatDateTime
{
	public sealed partial class Nullable
	{
		public sealed class HasKind
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenKindOfSubjectIsDifferent_ShouldFail()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167, DateTimeKind.Utc);
					DateTimeKind expected = DateTimeKind.Local;

					async Task Act()
						=> await That(subject).HasKind(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has kind equal to {Formatter.Format(expected)},
						              but it had kind Utc
						              """);
				}

				[Fact]
				public async Task WhenKindOfSubjectIsTheSame_ShouldSucceed()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167, DateTimeKind.Utc);
					DateTimeKind expected = DateTimeKind.Utc;

					async Task Act()
						=> await That(subject).HasKind(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					DateTime? subject = null;
					DateTimeKind expected = DateTimeKind.Utc;

					async Task Act()
						=> await That(subject).HasKind(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has kind equal to Utc,
						             but it was <null>
						             """);
				}
			}

			public sealed class EqualToTests
			{
				[Fact]
				public async Task WhenKindOfSubjectIsDifferent_ShouldFail()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167, DateTimeKind.Utc);
					DateTimeKind expected = DateTimeKind.Local;

					async Task Act()
						=> await That(subject).HasKind().EqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has kind equal to {Formatter.Format(expected)},
						              but it had kind Utc
						              """);
				}

				[Fact]
				public async Task WhenKindOfSubjectIsTheSame_ShouldSucceed()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167, DateTimeKind.Utc);
					DateTimeKind expected = DateTimeKind.Utc;

					async Task Act()
						=> await That(subject).HasKind().EqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					DateTime? subject = null;
					DateTimeKind expected = DateTimeKind.Utc;

					async Task Act()
						=> await That(subject).HasKind().EqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has kind equal to Utc,
						             but it was <null>
						             """);
				}
			}

			public sealed class NotEqualToTests
			{
				[Fact]
				public async Task WhenKindOfSubjectIsDifferent_ShouldSucceed()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167, DateTimeKind.Utc);
					DateTimeKind unexpected = DateTimeKind.Local;

					async Task Act()
						=> await That(subject).HasKind().NotEqualTo(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenKindOfSubjectIsTheSame_ShouldFail()
				{
					DateTime? subject = new(2010, 11, 12, 13, 14, 15, 167, DateTimeKind.Utc);
					DateTimeKind unexpected = DateTimeKind.Utc;

					async Task Act()
						=> await That(subject).HasKind().NotEqualTo(unexpected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has kind not equal to {Formatter.Format(unexpected)},
						              but it had kind Utc
						              """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					DateTime? subject = null;
					DateTimeKind unexpected = DateTimeKind.Utc;

					async Task Act()
						=> await That(subject).HasKind().NotEqualTo(unexpected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has kind not equal to {Formatter.Format(unexpected)},
						              but it was <null>
						              """);
				}
			}
		}
	}
}
