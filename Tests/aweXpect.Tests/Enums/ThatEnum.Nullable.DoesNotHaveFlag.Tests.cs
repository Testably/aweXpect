namespace aweXpect.Tests;

public sealed partial class ThatEnum
{
	public sealed partial class Nullable
	{
		public sealed class DoesNotHaveFlag
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					MyColors? subject = null;

					async Task Act()
						=> await That(subject).DoesNotHaveFlag(MyColors.Blue);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not have flag Blue,
						             but it was <null>
						             """);
				}

				[Fact]
				public async Task WhenSubjectAndUnexpectedIsNull_ShouldFail()
				{
					MyColors? subject = null;

					async Task Act()
						=> await That(subject).DoesNotHaveFlag(null);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not have flag <null>,
						             but it was <null>
						             """);
				}

				[Theory]
				[InlineData(MyColors.Blue)]
				[InlineData(MyColors.Green)]
				public async Task WhenSubjectDoesNotHaveFlag_ShouldSucceed(MyColors unexpected)
				{
					MyColors? subject = MyColors.Yellow | (MyColors.Red & ~unexpected);

					async Task Act()
						=> await That(subject).DoesNotHaveFlag(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Theory]
				[InlineData(MyColors.Blue | MyColors.Green, MyColors.Green)]
				[InlineData(MyColors.Blue | MyColors.Yellow, MyColors.Blue)]
				public async Task WhenSubjectHasFlag_ShouldFail(MyColors? subject, MyColors unexpected)
				{
					async Task Act()
						=> await That(subject).DoesNotHaveFlag(unexpected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not have flag {Formatter.Format(unexpected)},
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Theory]
				[InlineData(MyColors.Blue)]
				[InlineData(MyColors.Green)]
				public async Task WhenSubjectIsTheSame_ShouldFail(MyColors unexpected)
				{
					MyColors? subject = unexpected;

					async Task Act()
						=> await That(subject).DoesNotHaveFlag(unexpected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not have flag {Formatter.Format(unexpected)},
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task WhenUnexpectedIsNull_ShouldSucceed()
				{
					MyColors? subject = MyColors.Yellow;

					async Task Act()
						=> await That(subject).DoesNotHaveFlag(null);

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
