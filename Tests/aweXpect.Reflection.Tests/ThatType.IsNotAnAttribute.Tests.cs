using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotAnAttribute
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsAnAttribute_ShouldFail()
			{
				Type subject = typeof(PublicAttribute);

				async Task Act()
				{
					await That(subject).IsNotAnAttribute();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not an attribute,
					             but it was attribute PublicAttribute
					             """);
			}

			[Theory]
			[MemberData(nameof(NonAttributeTypes))]
			public async Task WhenTypeIsNotAnAttribute_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).IsNotAnAttribute();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsNotAnAttribute();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not an attribute,
					             but it was <null>
					             """);
			}

			public static TheoryData<Type> NonAttributeTypes() => new()
			{
				typeof(PublicClass),
				typeof(PublicStruct),
				typeof(IPublicInterface),
				typeof(PublicEnum),
			};
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenTypeIsAnAttribute_ShouldSucceed()
			{
				Type subject = typeof(PublicAttribute);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAnAttribute());
				}

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[MemberData(nameof(NonAttributeTypes))]
			public async Task WhenTypeIsNotAnAttribute_ShouldFail(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAnAttribute());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("*is an attribute*but it was*").AsWildcard();
			}

			public static TheoryData<Type> NonAttributeTypes() => new()
			{
				typeof(PublicClass),
				typeof(PublicStruct),
				typeof(IPublicInterface),
				typeof(PublicEnum),
			};
		}
	}
}
