namespace aweXpect.Tests;

public sealed partial class ThatBool
{
	public sealed class IsNotEqualTo
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData(true)]
			[InlineData(false)]
			public async Task WhenSubjectIsDifferent_ShouldSucceed(bool subject)
			{
				bool unexpected = !subject;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(true)]
			[InlineData(false)]
			public async Task WhenSubjectIsTheSame_ShouldFail(bool subject)
			{
				bool unexpected = subject;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineAutoData(true)]
			[InlineAutoData(false)]
			public async Task WhenSubjectIsTheSame_ShouldFailWithDescriptiveMessage(
				bool subject, string reason)
			{
				bool unexpected = subject;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Because(reason);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not {Formatter.Format(unexpected)}, because {reason},
					              but it was {Formatter.Format(subject)}
					              """);
			}
		}
	}
}
