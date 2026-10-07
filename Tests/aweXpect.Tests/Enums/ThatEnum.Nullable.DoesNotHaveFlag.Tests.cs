namespace aweXpect.Tests;

public sealed partial class ThatEnum
{
	public sealed partial class Nullable
	{
		public sealed class DoesNotHaveFlag
		{
			public sealed class Tests
			{
				[Test]
				[Arguments(MyColors.Blue)]
				[Arguments(MyColors.Green)]
				public async Task WhenSubjectDoesNotHaveFlag_ShouldSucceed(MyColors unexpected)
				{
					MyColors? subject = MyColors.Yellow | (MyColors.Red & ~unexpected);

					async Task Act()
						=> await That(subject).DoesNotHaveFlag(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				[Arguments(MyColors.Blue | MyColors.Green, MyColors.Green)]
				[Arguments(MyColors.Blue | MyColors.Yellow, MyColors.Blue)]
				public async Task WhenSubjectHasFlag_ShouldFail(MyColors? subject, MyColors unexpected)
				{
					async Task Act()
						=> await That(subject).DoesNotHaveFlag(unexpected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              does not have flag {Formatter.Format(unexpected)},
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					MyColors? subject = null;

					async Task Act()
						=> await That(subject).DoesNotHaveFlag(MyColors.Blue);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             does not have flag Blue,
						             but it was <null>
						             """);
				}

				[Test]
				[Arguments(MyColors.Blue)]
				[Arguments(MyColors.Green)]
				public async Task WhenSubjectIsTheSame_ShouldFail(MyColors unexpected)
				{
					MyColors? subject = unexpected;

					async Task Act()
						=> await That(subject).DoesNotHaveFlag(unexpected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              does not have flag {Formatter.Format(unexpected)},
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Test]
				public async Task WhenUnexpectedIsANamedArgument_ShouldSucceed()
				{
					MyColors? subject = MyColors.Yellow | MyColors.Red;

					async Task Act()
						=> await That(subject).DoesNotHaveFlag(MyColors.Blue);

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
