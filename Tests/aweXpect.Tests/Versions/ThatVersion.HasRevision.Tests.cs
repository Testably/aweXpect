namespace aweXpect.Tests;

public sealed partial class ThatVersion
{
	public sealed class HasRevision
		{
			public sealed class EqualToTests
			{
				[Fact]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					Version? subject = new(2010, 11, 12, 13);
					int? expected = null;

					async Task Act()
						=> await That(subject).HasRevision().EqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has revision equal to <null>,
						             but it had revision 13
						             """);
				}

				[Fact]
				public async Task WhenSubjectAndExpectedIsNull_ShouldFail()
				{
					Version? subject = null;
					int? expected = null;

					async Task Act()
						=> await That(subject).HasRevision().EqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has revision equal to <null>,
						             but it was <null>
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					Version? subject = null;
					int? expected = 1;

					async Task Act()
						=> await That(subject).HasRevision().EqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has revision equal to 1,
						             but it was <null>
						             """);
				}

				[Fact]
				public async Task WhenRevisionOfSubjectIsDifferent_ShouldFail()
				{
					Version? subject = new(2010, 11, 12, 13);
					int? expected = 14;

					async Task Act()
						=> await That(subject).HasRevision().EqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has revision equal to {Formatter.Format(expected)},
						              but it had revision 13
						              """);
				}

				[Fact]
				public async Task WhenRevisionOfSubjectIsTheSame_ShouldSucceed()
				{
					Version? subject = new(2010, 11, 12, 13);
					int expected = 13;

					async Task Act()
						=> await That(subject).HasRevision().EqualTo(expected);

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
						=> await That(subject).HasRevision().GreaterThanOrEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has revision greater than or equal to <null>,
						             but it had revision 13
						             """);
				}

				[Fact]
				public async Task WhenRevisionOfSubjectIsGreaterThanExpected_ShouldSucceed()
				{
					Version? subject = new(2010, 11, 12, 13);
					int? expected = 12;

					async Task Act()
						=> await That(subject).HasRevision().GreaterThanOrEqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenRevisionOfSubjectIsLessThanExpected_ShouldFail()
				{
					Version? subject = new(2010, 11, 12, 13);
					int? expected = 14;

					async Task Act()
						=> await That(subject).HasRevision().GreaterThanOrEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has revision greater than or equal to {Formatter.Format(expected)},
						              but it had revision 13
						              """);
				}

				[Fact]
				public async Task WhenRevisionOfSubjectIsTheSameAsExpected_ShouldSucceed()
				{
					Version? subject = new(2010, 11, 12, 13);
					int expected = 13;

					async Task Act()
						=> await That(subject).HasRevision().GreaterThanOrEqualTo(expected);

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
						=> await That(subject).HasRevision().GreaterThan(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has revision greater than <null>,
						             but it had revision 13
						             """);
				}

				[Fact]
				public async Task WhenRevisionOfSubjectIsGreaterThanExpected_ShouldSucceed()
				{
					Version? subject = new(2010, 11, 12, 13);
					int? expected = 12;

					async Task Act()
						=> await That(subject).HasRevision().GreaterThan(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenRevisionOfSubjectIsLessThanExpected_ShouldFail()
				{
					Version? subject = new(2010, 11, 12, 13);
					int? expected = 14;

					async Task Act()
						=> await That(subject).HasRevision().GreaterThan(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has revision greater than {Formatter.Format(expected)},
						              but it had revision 13
						              """);
				}

				[Fact]
				public async Task WhenRevisionOfSubjectIsTheSameAsExpected_ShouldFail()
				{
					Version? subject = new(2010, 11, 12, 13);
					int expected = 13;

					async Task Act()
						=> await That(subject).HasRevision().GreaterThan(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has revision greater than {Formatter.Format(expected)},
						              but it had revision 13
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
						=> await That(subject).HasRevision().LessThanOrEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has revision less than or equal to <null>,
						             but it had revision 13
						             """);
				}

				[Fact]
				public async Task WhenRevisionOfSubjectIsGreaterThanExpected_ShouldFail()
				{
					Version? subject = new(2010, 11, 12, 13);
					int? expected = 12;

					async Task Act()
						=> await That(subject).HasRevision().LessThanOrEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has revision less than or equal to {Formatter.Format(expected)},
						              but it had revision 13
						              """);
				}

				[Fact]
				public async Task WhenRevisionOfSubjectIsLessThanExpected_ShouldSucceed()
				{
					Version? subject = new(2010, 11, 12, 13);
					int? expected = 14;

					async Task Act()
						=> await That(subject).HasRevision().LessThanOrEqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenRevisionOfSubjectIsTheSameAsExpected_ShouldSucceed()
				{
					Version? subject = new(2010, 11, 12, 13);
					int expected = 13;

					async Task Act()
						=> await That(subject).HasRevision().LessThanOrEqualTo(expected);

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
						=> await That(subject).HasRevision().LessThan(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has revision less than <null>,
						             but it had revision 13
						             """);
				}

				[Fact]
				public async Task WhenRevisionOfSubjectIsGreaterThanExpected_ShouldFail()
				{
					Version? subject = new(2010, 11, 12, 13);
					int? expected = 12;

					async Task Act()
						=> await That(subject).HasRevision().LessThan(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has revision less than {Formatter.Format(expected)},
						              but it had revision 13
						              """);
				}

				[Fact]
				public async Task WhenRevisionOfSubjectIsLessThanExpected_ShouldSucceed()
				{
					Version? subject = new(2010, 11, 12, 13);
					int? expected = 14;

					async Task Act()
						=> await That(subject).HasRevision().LessThan(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenRevisionOfSubjectIsTheSameAsExpected_ShouldFail()
				{
					Version? subject = new(2010, 11, 12, 13);
					int expected = 13;

					async Task Act()
						=> await That(subject).HasRevision().LessThan(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has revision less than {Formatter.Format(expected)},
						              but it had revision 13
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
						=> await That(subject).HasRevision().NotEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has revision not equal to <null>,
						             but it was <null>
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					Version? subject = null;
					int? unexpected = 1;

					async Task Act()
						=> await That(subject).HasRevision().NotEqualTo(unexpected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has revision not equal to {unexpected},
						              but it was <null>
						              """);
				}

				[Fact]
				public async Task WhenUnexpectedIsNull_ShouldSucceed()
				{
					Version? subject = new(2010, 11, 12, 13);
					int? unexpected = null;

					async Task Act()
						=> await That(subject).HasRevision().NotEqualTo(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenRevisionOfSubjectIsDifferent_ShouldSucceed()
				{
					Version? subject = new(2010, 11, 12, 13);
					int? unexpected = 14;

					async Task Act()
						=> await That(subject).HasRevision().NotEqualTo(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenRevisionOfSubjectIsTheSame_ShouldFail()
				{
					Version? subject = new(2010, 11, 12, 13);
					int unexpected = 13;

					async Task Act()
						=> await That(subject).HasRevision().NotEqualTo(unexpected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has revision not equal to {Formatter.Format(unexpected)},
						              but it had revision 13
						              """);
				}
			}
		}
	public sealed class RevisionUnspecifiedTests
	{
		[Fact]
		public async Task ShouldBeMinusOne()
		{
			Version subject = new(1, 2);

			async Task Act()
				=> await That(subject).HasRevision().EqualTo(-1);

			await That(Act).DoesNotThrow();
		}
	}
}
