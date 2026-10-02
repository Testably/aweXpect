namespace aweXpect.Tests;

public sealed partial class ThatEnum
{
	public sealed class IsDefined
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData(1 << 4)]
			[InlineData(1 << 4 | 1)]
			public async Task WhenFlagsSubjectHasAnUndefinedBit_ShouldFail(int value)
			{
				MyColors subject = (MyColors)value;

				async Task Act()
					=> await That(subject).IsDefined();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is defined,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(MyColors.Blue | MyColors.Green)]
			[InlineData(MyColors.Yellow | MyColors.Red)]
			[InlineData(MyColors.Blue | MyColors.Green | MyColors.Yellow | MyColors.Red)]
			public async Task WhenFlagsSubjectIsACombinationOfFlags_ShouldSucceed(MyColors subject)
			{
				async Task Act()
					=> await That(subject).IsDefined();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFlagsSubjectIsZeroWithoutAZeroMember_ShouldFail()
			{
				MyColors subject = 0;

				async Task Act()
					=> await That(subject).IsDefined();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is defined,
					             but it was 0
					             """).Because("zero is no combination of flags unless the enum names it");
			}

			[Fact]
			public async Task WhenSignedFlagsSubjectCombinesTheSignBit_ShouldSucceed()
			{
				EnumFlagsSByte subject = EnumFlagsSByte.Sign | EnumFlagsSByte.Low;

				async Task Act()
					=> await That(subject).IsDefined();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSignedFlagsSubjectHasAnUndefinedBit_ShouldFail()
			{
				EnumFlagsSByte subject = EnumFlagsSByte.Sign | (EnumFlagsSByte)2;

				async Task Act()
					=> await That(subject).IsDefined();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is defined,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(MyColors.Blue)]
			[InlineData(MyColors.Green)]
			public async Task WhenSubjectIsDefined_ShouldSucceed(MyColors subject)
			{
				async Task Act()
					=> await That(subject).IsDefined();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNotDefined_ShouldFail()
			{
				MyColors subject = (MyColors)42;

				async Task Act()
					=> await That(subject).IsDefined();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is defined,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenSubjectWithoutFlagsAttributeConsistsOfBitsOfAMember_ShouldFail()
			{
				EnumByte subject = (EnumByte)1;

				async Task Act()
					=> await That(subject).IsDefined();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is defined,
					             but it was 1
					             """)
					.Because("without the [Flags] attribute only a named member is defined, even if its bits are set in another member");
			}

			[Fact]
			public async Task WhenUnsignedFlagsSubjectCombinesTheHighestBit_ShouldSucceed()
			{
				EnumFlagsULong subject = EnumFlagsULong.High | EnumFlagsULong.Low;

				async Task Act()
					=> await That(subject).IsDefined();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenUnsignedFlagsSubjectHasAnUndefinedBit_ShouldFail()
			{
				EnumFlagsULong subject = EnumFlagsULong.High | (EnumFlagsULong)2;

				async Task Act()
					=> await That(subject).IsDefined();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is defined,
					              but it was {Formatter.Format(subject)}
					              """);
			}
		}
	}
}
