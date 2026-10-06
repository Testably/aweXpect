namespace aweXpect.Tests;

public sealed partial class ThatChar
{
	public sealed partial class Nullable
	{
		public sealed class IsNull
		{
			public sealed class Tests
			{
				[Test]
				[Arguments('a')]
				[Arguments('X')]
				[Arguments('5')]
				[Arguments('\t')]
				public async Task WhenSubjectIsNotNull_ShouldFail(char? subject)
				{
					async Task Act()
						=> await That(subject).IsNull();

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is null,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldSucceed()
				{
					char? subject = null;

					async Task Act()
						=> await That(subject).IsNull();

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
