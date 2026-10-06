namespace aweXpect.Tests;

public sealed partial class ThatEnum
{
	public sealed partial class Nullable
	{
		public sealed class IsNull
		{
			public sealed class Tests
			{
				[Test]
				[Arguments(MyColors.Blue)]
				[Arguments((MyColors)42)]
				public async Task WhenSubjectIsNotNull_ShouldFail(MyColors? subject)
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
					MyColors? subject = null;

					async Task Act()
						=> await That(subject).IsNull();

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
