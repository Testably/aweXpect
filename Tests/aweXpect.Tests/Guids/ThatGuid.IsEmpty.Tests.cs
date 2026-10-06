namespace aweXpect.Tests;

public sealed partial class ThatGuid
{
	public sealed class IsEmpty
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenSubjectIsEmpty_ShouldSucceed()
			{
				Guid subject = Guid.Empty;

				async Task Act()
					=> await That(subject).IsEmpty();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNotEmpty_ShouldFail()
			{
				Guid subject = OtherGuid();

				async Task Act()
					=> await That(subject).IsEmpty();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is empty,
					              but it was {Formatter.Format(subject)}
					              """);
			}
		}
	}
}
