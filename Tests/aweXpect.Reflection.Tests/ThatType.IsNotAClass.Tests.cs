using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotAClass
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsAClass_ShouldFail()
			{
				Type subject = typeof(PublicClass);

				async Task Act()
					=> await That(subject).IsNotAClass();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not a class,
					             but it was class PublicClass
					             """);
			}

			[Theory]
			[MemberData(nameof(NonClassTypes))]
			public async Task WhenTypeIsNotAClass_ShouldSucceed(Type subject)
			{
				async Task Act()
					=> await That(subject).IsNotAClass();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
					=> await That(subject).IsNotAClass();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not a class,
					             but it was <null>
					             """);
			}

			public static TheoryData<Type> NonClassTypes()
				=>
				[
					typeof(IPublicInterface),
					typeof(PublicEnum),
					typeof(PublicStruct),
					typeof(PublicRecord),
					typeof(PublicRecordStruct),
				];
		}
	}
}
