namespace aweXpect.Tests;

public sealed partial class ThatVersion
{
	public sealed class HasRevision
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenRevisionOfSubjectIsDifferent_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int expected = 14;

				async Task Act()
					=> await That(subject).HasRevision(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has revision equal to {Formatter.Format(expected)},
					              but it had revision 13
					              """);
			}

			[Test]
			public async Task WhenRevisionOfSubjectIsTheSame_ShouldSucceed()
			{
				Version? subject = new(2010, 11, 12, 13);
				int expected = 13;

				async Task Act()
					=> await That(subject).HasRevision(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Version? subject = null;
				int expected = 1;

				async Task Act()
					=> await That(subject).HasRevision(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has revision equal to 1,
					             but it was <null>
					             """);
			}
		}

		public sealed class EqualToTests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasRevision().EqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has revision equal to <null>,
					             but it had revision 13
					             """);
			}

			[Test]
			public async Task WhenRevisionOfSubjectIsDifferent_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = 14;

				async Task Act()
					=> await That(subject).HasRevision().EqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has revision equal to {Formatter.Format(expected)},
					              but it had revision 13
					              """);
			}

			[Test]
			public async Task WhenRevisionOfSubjectIsTheSame_ShouldSucceed()
			{
				Version? subject = new(2010, 11, 12, 13);
				int expected = 13;

				async Task Act()
					=> await That(subject).HasRevision().EqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectAndExpectedIsNull_ShouldFail()
			{
				Version? subject = null;
				int? expected = null;

				async Task Act()
					=> await That(subject).HasRevision().EqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has revision equal to <null>,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Version? subject = null;
				int? expected = 1;

				async Task Act()
					=> await That(subject).HasRevision().EqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has revision equal to 1,
					             but it was <null>
					             """);
			}
		}

		public sealed class GreaterThanOrEqualToTests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasRevision().GreaterThanOrEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has revision greater than or equal to <null>,
					             but it had revision 13
					             """);
			}

			[Test]
			public async Task WhenRevisionOfSubjectIsGreaterThanExpected_ShouldSucceed()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = 12;

				async Task Act()
					=> await That(subject).HasRevision().GreaterThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenRevisionOfSubjectIsLessThanExpected_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = 14;

				async Task Act()
					=> await That(subject).HasRevision().GreaterThanOrEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has revision greater than or equal to {Formatter.Format(expected)},
					              but it had revision 13
					              """);
			}

			[Test]
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
			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasRevision().GreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has revision greater than <null>,
					             but it had revision 13
					             """);
			}

			[Test]
			public async Task WhenRevisionOfSubjectIsGreaterThanExpected_ShouldSucceed()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = 12;

				async Task Act()
					=> await That(subject).HasRevision().GreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenRevisionOfSubjectIsLessThanExpected_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = 14;

				async Task Act()
					=> await That(subject).HasRevision().GreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has revision greater than {Formatter.Format(expected)},
					              but it had revision 13
					              """);
			}

			[Test]
			public async Task WhenRevisionOfSubjectIsTheSameAsExpected_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int expected = 13;

				async Task Act()
					=> await That(subject).HasRevision().GreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has revision greater than {Formatter.Format(expected)},
					              but it had revision 13
					              """);
			}
		}

		public sealed class LessThanOrEqualToTests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasRevision().LessThanOrEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has revision less than or equal to <null>,
					             but it had revision 13
					             """);
			}

			[Test]
			public async Task WhenRevisionOfSubjectIsGreaterThanExpected_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = 12;

				async Task Act()
					=> await That(subject).HasRevision().LessThanOrEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has revision less than or equal to {Formatter.Format(expected)},
					              but it had revision 13
					              """);
			}

			[Test]
			public async Task WhenRevisionOfSubjectIsLessThanExpected_ShouldSucceed()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = 14;

				async Task Act()
					=> await That(subject).HasRevision().LessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
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
			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = null;

				async Task Act()
					=> await That(subject).HasRevision().LessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has revision less than <null>,
					             but it had revision 13
					             """);
			}

			[Test]
			public async Task WhenRevisionOfSubjectIsGreaterThanExpected_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = 12;

				async Task Act()
					=> await That(subject).HasRevision().LessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has revision less than {Formatter.Format(expected)},
					              but it had revision 13
					              """);
			}

			[Test]
			public async Task WhenRevisionOfSubjectIsLessThanExpected_ShouldSucceed()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? expected = 14;

				async Task Act()
					=> await That(subject).HasRevision().LessThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenRevisionOfSubjectIsTheSameAsExpected_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int expected = 13;

				async Task Act()
					=> await That(subject).HasRevision().LessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has revision less than {Formatter.Format(expected)},
					              but it had revision 13
					              """);
			}
		}

		public sealed class NotEqualToTests
		{
			[Test]
			public async Task WhenRevisionOfSubjectIsDifferent_ShouldSucceed()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? unexpected = 14;

				async Task Act()
					=> await That(subject).HasRevision().NotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenRevisionOfSubjectIsTheSame_ShouldFail()
			{
				Version? subject = new(2010, 11, 12, 13);
				int unexpected = 13;

				async Task Act()
					=> await That(subject).HasRevision().NotEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              does not have revision equal to {Formatter.Format(unexpected)},
					              but it had revision 13
					              """);
			}

			[Test]
			public async Task WhenSubjectAndUnexpectedIsNull_ShouldFail()
			{
				Version? subject = null;
				int? expected = null;

				async Task Act()
					=> await That(subject).HasRevision().NotEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have revision equal to <null>,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Version? subject = null;
				int? unexpected = 1;

				async Task Act()
					=> await That(subject).HasRevision().NotEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              does not have revision equal to {unexpected},
					              but it was <null>
					              """);
			}

			[Test]
			public async Task WhenUnexpectedIsNull_ShouldSucceed()
			{
				Version? subject = new(2010, 11, 12, 13);
				int? unexpected = null;

				async Task Act()
					=> await That(subject).HasRevision().NotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}
		}

		[Test]
		public async Task WhenRevisionIsUnspecified_ShouldBeMinusOne()
		{
			Version subject = new(1, 2);

			async Task Act()
				=> await That(subject).HasRevision().EqualTo(-1);

			await That(Act).DoesNotThrow();
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenRevisionDiffers_ShouldSucceed()
			{
				Version subject = new(1, 2, 3, 4);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasRevision(5));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenRevisionMatches_ShouldFail()
			{
				Version subject = new(1, 2, 3, 4);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasRevision(4));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have revision equal to 4,
					             but it had revision 4
					             """);
			}
		}
	}
}
