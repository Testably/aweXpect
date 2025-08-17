using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

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
			[MemberData(nameof(InterfaceTypesForNegated))]
			public async Task WhenTypeIsAnInterface_ShouldSucceedWithNegatedAssertion(Type subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotAnInterface());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNotAnInterface_ShouldFailWithNegatedAssertion()
			{
				Type subject = typeof(PublicClass);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotAnInterface());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an interface,
					             but it was class PublicClass
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

			public static TheoryData<Type> InterfaceTypesForNegated() => new()
			{
				typeof(IPublicInterface),
			};

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
