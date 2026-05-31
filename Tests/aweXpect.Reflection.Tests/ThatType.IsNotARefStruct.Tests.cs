using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotARefStruct
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsARefStruct_ShouldFail()
			{
				Type subject = typeof(PublicRefStruct);

				async Task Act()
				{
					await That(subject).IsNotARefStruct();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not a ref struct,
					             but it was struct PublicRefStruct
					             """);
			}

			[Theory]
			[MemberData(nameof(RefStructTypesForNegated))]
			public async Task WhenTypeIsARefStruct_ShouldSucceedWithNegatedAssertion(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotARefStruct());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNotARefStruct_ShouldFailWithNegatedAssertion()
			{
				Type subject = typeof(PublicClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotARefStruct());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is a ref struct,
					             but it was class PublicClass
					             """);
			}

			[Theory]
			[MemberData(nameof(NonRefStructTypes))]
			public async Task WhenTypeIsNotARefStruct_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).IsNotARefStruct();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsNotARefStruct();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not a ref struct,
					             but it was <null>
					             """);
			}

			public static TheoryData<Type> RefStructTypesForNegated() => new()
			{
				typeof(PublicRefStruct),
			};

			public static TheoryData<Type> NonRefStructTypes() => new()
			{
				typeof(PublicStruct),
				typeof(PublicReadOnlyStruct),
				typeof(IPublicInterface),
				typeof(PublicEnum),
				typeof(PublicClass),
				typeof(PublicRecordStruct),
			};
		}
	}
}
