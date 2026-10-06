namespace aweXpect.Tests;

public sealed partial class ThatEnum
{
	public sealed class IsNotDefined
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenFlagsSubjectHasAnUndefinedBit_ShouldSucceed()
			{
				MyColors subject = (MyColors)(1 << 4 | 1);

				async Task Act()
					=> await That(subject).IsNotDefined();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(MyColors.Blue | MyColors.Green)]
			[Arguments(MyColors.Yellow | MyColors.Red)]
			public async Task WhenFlagsSubjectIsACombinationOfFlags_ShouldFail(MyColors subject)
			{
				async Task Act()
					=> await That(subject).IsNotDefined();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not defined,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenFlagsSubjectIsZeroWithoutAZeroMember_ShouldSucceed()
			{
				MyColors subject = 0;

				async Task Act()
					=> await That(subject).IsNotDefined();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(MyColors.Blue)]
			[Arguments(MyColors.Green)]
			public async Task WhenSubjectIsDefined_ShouldFail(MyColors subject)
			{
				async Task Act()
					=> await That(subject).IsNotDefined();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not defined,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenSubjectIsNotDefined_ShouldSucceed()
			{
				MyColors subject = (MyColors)42;

				async Task Act()
					=> await That(subject).IsNotDefined();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectWithoutFlagsAttributeConsistsOfBitsOfAMember_ShouldSucceed()
			{
				EnumByte subject = (EnumByte)1;

				async Task Act()
					=> await That(subject).IsNotDefined();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
