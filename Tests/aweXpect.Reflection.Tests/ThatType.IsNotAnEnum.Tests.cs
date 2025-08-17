using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotAnEnum
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsAnEnum_ShouldFail()
			{
				Type subject = typeof(PublicEnum);

				async Task Act()
					=> await That(subject).IsNotAnEnum();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not an enum,
					             but it was enum PublicEnum
					             """);
			}

			[Theory]
			[MemberData(nameof(EnumTypesForNegated))]
			public async Task WhenTypeIsAnEnum_ShouldSucceedWithNegatedAssertion(Type subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotAnEnum());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNotAnEnum_ShouldFailWithNegatedAssertion()
			{
				Type subject = typeof(PublicClass);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotAnEnum());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an enum,
					             but it was class PublicClass
					             """);
			}

			[Theory]
			[MemberData(nameof(NonEnumTypes))]
			public async Task WhenTypeIsNotAnEnum_ShouldSucceed(Type subject)
			{
				async Task Act()
					=> await That(subject).IsNotAnEnum();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
					=> await That(subject).IsNotAnEnum();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not an enum,
					             but it was <null>
					             """);
			}

			public static TheoryData<Type> EnumTypesForNegated() => new()
			{
				typeof(PublicEnum),
			};

			public static TheoryData<Type> NonEnumTypes()
				=>
				[
					typeof(IPublicInterface),
					typeof(PublicClass),
					typeof(PublicStruct),
					typeof(PublicRecord),
					typeof(PublicRecordStruct),
				];
		}
	}
}
