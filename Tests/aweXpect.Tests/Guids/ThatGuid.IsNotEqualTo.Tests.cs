namespace aweXpect.Tests;

public sealed partial class ThatGuid
{
	public sealed class IsNotEqualTo
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenSubjectIsDifferent_ShouldSucceed()
			{
				Guid subject = FixedGuid();
				Guid unexpected = OtherGuid();

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsTheSame_ShouldFail()
			{
				Guid subject = FixedGuid();
				Guid unexpected = subject;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenUnexpectedIsNull_ShouldSucceed()
			{
				Guid subject = FixedGuid();
				Guid? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
