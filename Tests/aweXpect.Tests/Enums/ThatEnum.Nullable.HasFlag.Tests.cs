namespace aweXpect.Tests;

public sealed partial class ThatEnum
{
	public sealed partial class Nullable
	{
		public sealed class HasFlag
		{
			public sealed class Tests
			{
				[Test]
				public async Task WhenExpectedIsANamedArgument_ShouldSucceed()
				{
					MyColors? subject = MyColors.Yellow | MyColors.Red;

					async Task Act()
						=> await That(subject).HasFlag(expected: MyColors.Red);

					await That(Act).DoesNotThrow();
				}

				[Test]
				[Arguments(MyColors.Blue | MyColors.Red, MyColors.Green)]
				[Arguments(MyColors.Green | MyColors.Yellow, MyColors.Blue)]
				public async Task WhenSubjectDoesNotHaveFlag_ShouldFail(MyColors? subject,
					MyColors expected)
				{
					async Task Act()
						=> await That(subject).HasFlag(expected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has flag {Formatter.Format(expected)},
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Test]
				[Arguments(MyColors.Blue)]
				[Arguments(MyColors.Green)]
				public async Task WhenSubjectHasFlag_ShouldSucceed(MyColors expected)
				{
					MyColors? subject = MyColors.Yellow | MyColors.Red | expected;

					async Task Act()
						=> await That(subject).HasFlag(expected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					MyColors? subject = null;

					async Task Act()
						=> await That(subject).HasFlag(MyColors.Blue);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has flag Blue,
						             but it was <null>
						             """);
				}

				[Test]
				[Arguments(MyColors.Blue)]
				[Arguments(MyColors.Green)]
				public async Task WhenSubjectIsTheSame_ShouldSucceed(MyColors expected)
				{
					MyColors? subject = expected;

					async Task Act()
						=> await That(subject).HasFlag(expected);

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
