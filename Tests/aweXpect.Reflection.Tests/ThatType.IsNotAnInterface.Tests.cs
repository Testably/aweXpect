using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotAnInterface
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsAnInterface_ShouldFail()
			{
				Type subject = typeof(IPublicInterface);

				async Task Act()
					=> await That(subject).IsNotAnInterface();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not an interface,
					             but it was interface IPublicInterface
					             """);
			}

			[Theory]
			[MemberData(nameof(NonInterfaceTypes))]
			public async Task WhenTypeIsNotAnInterface_ShouldSucceed(Type subject)
			{
				async Task Act()
					=> await That(subject).IsNotAnInterface();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
					=> await That(subject).IsNotAnInterface();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not an interface,
					             but it was <null>
					             """);
			}

			public static TheoryData<Type> NonInterfaceTypes()
				=>
				[
					typeof(PublicClass),
					typeof(PublicEnum),
					typeof(PublicStruct),
					typeof(PublicRecord),
					typeof(PublicRecordStruct),
				];
		}
	}
}
