namespace aweXpect.Tests;

public sealed partial class ThatEnum
{
	public sealed partial class Nullable
	{
		public sealed class IsDefined
		{
			public sealed class Tests
			{
				[Test]
				public async Task WhenFlagsSubjectHasAnUndefinedBit_ShouldFail()
				{
					MyColors? subject = (MyColors)(1 << 4 | 1);

					async Task Act()
						=> await That(subject).IsDefined();

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is defined,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Test]
				[Arguments(MyColors.Blue | MyColors.Green)]
				[Arguments(MyColors.Yellow | MyColors.Red)]
				public async Task WhenFlagsSubjectIsACombinationOfFlags_ShouldSucceed(MyColors? subject)
				{
					async Task Act()
						=> await That(subject).IsDefined();

					await That(Act).DoesNotThrow();
				}

				[Test]
				[Arguments(MyColors.Blue)]
				[Arguments(MyColors.Green)]
				public async Task WhenSubjectIsDefined_ShouldSucceed(MyColors? subject)
				{
					async Task Act()
						=> await That(subject).IsDefined();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsNotDefined_ShouldFail()
				{
					MyColors? subject = (MyColors)42;

					async Task Act()
						=> await That(subject).IsDefined();

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is defined,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					MyColors? subject = null;

					async Task Act()
						=> await That(subject).IsDefined();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is defined,
						             but it was <null>
						             """);
				}
			}
		}
	}
}
