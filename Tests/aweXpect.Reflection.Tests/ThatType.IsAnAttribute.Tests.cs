using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsAnAttribute
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenTypeIsAnAttribute_ShouldSucceed()
			{
				Type subject = typeof(PublicAttribute);

				async Task Act()
				{
					await That(subject).IsAnAttribute();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			[MethodDataSource(nameof(NonAttributeType))]
			public async Task WhenTypeIsNotAnAttribute_ShouldFail(Type? subject, string name)
			{
				async Task Act()
				{
					await That(subject).IsAnAttribute();
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that subject
					              is an attribute,
					              but it was {name}
					              """);
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsAnAttribute();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is an attribute,
					             but it was <null>
					             """);
			}

			public static IEnumerable<(Type?, string)> NonAttributeType() => new List<(Type?, string)>
			{
				(null, "<null>"),
				(typeof(PublicClass), "class PublicClass"),
				(typeof(PublicStruct), "struct PublicStruct"),
				(typeof(IPublicInterface), "interface IPublicInterface"),
				(typeof(PublicEnum), "enum PublicEnum"),
			};
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenTypeIsAnAttribute_ShouldFail()
			{
				Type subject = typeof(PublicAttribute);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsAnAttribute());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an attribute,
					             but it was attribute PublicAttribute
					             """);
			}

			[Test]
			[MethodDataSource(nameof(NonAttributeTypeForNegated))]
			public async Task WhenTypeIsNotAnAttribute_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsAnAttribute());
				}

				await That(Act).DoesNotThrow();
			}

			public static IEnumerable<Type> NonAttributeTypeForNegated() => new List<Type>
			{
				typeof(PublicClass),
				typeof(PublicStruct),
				typeof(IPublicInterface),
				typeof(PublicEnum),
			};
		}
	}
}
