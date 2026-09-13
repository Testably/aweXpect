namespace aweXpect.Tests;

public sealed partial class ThatEnum
{
	public sealed partial class Nullable
	{
		public sealed class HasFlag
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					MyColors? subject = MyColors.Yellow;

					async Task Act()
						=> await That(subject).HasFlag(null);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has flag <null>,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task WhenSubjectAndExpectedIsNull_ShouldFail()
				{
					MyColors? subject = null;

					async Task Act()
						=> await That(subject).HasFlag(null);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has flag <null>,
						             but it was <null>
						             """);
				}

				[Theory]
				[InlineData(MyColors.Blue | MyColors.Red, MyColors.Green)]
				[InlineData(MyColors.Green | MyColors.Yellow, MyColors.Blue)]
				public async Task WhenSubjectDoesNotHaveFlag_ShouldFail(MyColors? subject,
					MyColors expected)
				{
					async Task Act()
						=> await That(subject).HasFlag(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has flag {Formatter.Format(expected)},
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Theory]
				[InlineData(MyColors.Blue)]
				[InlineData(MyColors.Green)]
				public async Task WhenSubjectHasFlag_ShouldSucceed(MyColors expected)
				{
					MyColors? subject = MyColors.Yellow | MyColors.Red | expected;

					async Task Act()
						=> await That(subject).HasFlag(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					MyColors? subject = null;

					async Task Act()
						=> await That(subject).HasFlag(MyColors.Blue);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has flag Blue,
						             but it was <null>
						             """);
				}

				[Theory]
				[InlineData(MyColors.Blue)]
				[InlineData(MyColors.Green)]
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
