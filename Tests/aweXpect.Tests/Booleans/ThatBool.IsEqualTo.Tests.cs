namespace aweXpect.Tests;

public sealed partial class ThatBool
{
	public sealed class IsEqualTo
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData(true)]
			[InlineData(false)]
			public async Task WhenExpectedIsNull_ShouldFail(bool subject)
			{
				bool? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(true)]
			[InlineData(false)]
			public async Task WhenSubjectIsDifferent_ShouldFail(bool subject)
			{
				bool expected = !subject;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineAutoData(true)]
			[InlineAutoData(false)]
			public async Task WhenSubjectIsDifferent_ShouldFailWithDescriptiveMessage(
				bool subject, string reason)
			{
				bool expected = !subject;

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Because(reason);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is {Formatter.Format(expected)}, because {reason},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(true)]
			[InlineData(false)]
			public async Task WhenSubjectIsTheSame_ShouldSucceed(bool subject)
			{
				bool expected = subject;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
