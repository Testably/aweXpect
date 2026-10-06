namespace aweXpect.Tests;

public sealed partial class ThatBool
{
	public sealed class IsNotEqualTo
	{
		public sealed class Tests
		{
			[Test]
			[Arguments(true)]
			[Arguments(false)]
			public async Task WhenSubjectIsDifferent_ShouldSucceed(bool subject)
			{
				bool unexpected = !subject;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(true)]
			[Arguments(false)]
			public async Task WhenSubjectIsTheSame_ShouldFail(bool subject)
			{
				bool unexpected = subject;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[AutoArguments(true)]
			[AutoArguments(false)]
			public async Task WhenSubjectIsTheSame_ShouldFailWithDescriptiveMessage(
				bool subject, string reason)
			{
				bool unexpected = subject;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Because(reason);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not {Formatter.Format(unexpected)}, because {reason},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments(true)]
			[Arguments(false)]
			public async Task WhenUnexpectedIsNull_ShouldSucceed(bool subject)
			{
				bool? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
