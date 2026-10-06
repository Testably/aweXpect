namespace aweXpect.Tests;

public sealed partial class ThatBool
{
	public sealed partial class Nullable
	{
		public sealed class IsNotEqualTo
		{
			public sealed class Tests
			{
				[Test]
				public async Task WhenSubjectAndExpectedAreNull_ShouldFail()
				{
					bool? subject = null;
					bool? unexpected = null;

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not <null>,
						             but it was <null>
						             """);
				}

				[Test]
				[Arguments(true, false)]
				[Arguments(true, null)]
				[Arguments(false, true)]
				[Arguments(false, null)]
				[Arguments(null, true)]
				[Arguments(null, false)]
				public async Task WhenSubjectIsDifferent_ShouldSucceed(bool? subject, bool? unexpected)
				{
					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				[Arguments(true)]
				[Arguments(false)]
				public async Task WhenSubjectIsTheSame_ShouldFail(bool? subject)
				{
					bool? unexpected = subject;

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not {Formatter.Format(unexpected)},
						              but it was {Formatter.Format(subject)}
						              """);
				}
			}
		}
	}
}
