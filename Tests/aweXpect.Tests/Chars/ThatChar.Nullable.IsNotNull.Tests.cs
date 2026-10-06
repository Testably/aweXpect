namespace aweXpect.Tests;

public sealed partial class ThatChar
{
	public sealed partial class Nullable
	{
		public sealed class IsNotNull
		{
			public sealed class Tests
			{
				[Test]
				[Arguments('a')]
				[Arguments('X')]
				[Arguments('5')]
				[Arguments('\t')]
				public async Task WhenSubjectIsNotNull_ShouldSucceed(char? subject)
				{
					async Task Act()
						=> await That(subject).IsNotNull();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					char? subject = null;

					async Task Act()
						=> await That(subject).IsNotNull();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not null,
						             but it was <null>
						             """);
				}
			}
		}
	}
}
