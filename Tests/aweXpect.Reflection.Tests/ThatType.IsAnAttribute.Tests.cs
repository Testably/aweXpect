using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsAnAttribute
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsAnAttribute_ShouldSucceed()
			{
				Type subject = typeof(PublicAttribute);

				async Task Act()
				{
					await That(subject).IsAnAttribute();
				}

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[MemberData(nameof(NonAttributeType))]
			public async Task WhenTypeIsNotAnAttribute_ShouldFail(Type? subject, string name)
			{
				async Task Act()
				{
					await That(subject).IsAnAttribute();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is an attribute,
					              but it was {name}
					              """);
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsAnAttribute();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is an attribute,
					             but it was <null>
					             """);
			}

			public static TheoryData<Type?, string> NonAttributeType() => new()
			{
				{
					null, "<null>"
				},
				{
					typeof(PublicClass), "class PublicClass"
				},
				{
					typeof(PublicStruct), "struct PublicStruct"
				},
				{
					typeof(IPublicInterface), "interface IPublicInterface"
				},
				{
					typeof(PublicEnum), "enum PublicEnum"
				},
			};
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenTypeIsAnAttribute_ShouldFail()
			{
				Type subject = typeof(PublicAttribute);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsAnAttribute());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not an attribute,
					             but it was attribute PublicAttribute
					             """);
			}

			[Theory]
			[MemberData(nameof(NonAttributeTypeForNegated))]
			public async Task WhenTypeIsNotAnAttribute_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsAnAttribute());
				}

				await That(Act).DoesNotThrow();
			}

			public static TheoryData<Type> NonAttributeTypeForNegated() => new()
			{
				typeof(PublicClass),
				typeof(PublicStruct),
				typeof(IPublicInterface),
				typeof(PublicEnum),
			};
		}
	}
}
