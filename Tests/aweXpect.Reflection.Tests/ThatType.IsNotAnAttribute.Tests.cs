using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotAnAttribute
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenTypeIsAnAttribute_ShouldFail()
			{
				Type subject = typeof(PublicAttribute);

				async Task Act()
				{
					await That(subject).IsNotAnAttribute();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not an attribute,
					             but it was attribute PublicAttribute
					             """);
			}

			[Test]
			[MethodDataSource(nameof(NonAttributeTypes))]
			public async Task WhenTypeIsNotAnAttribute_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).IsNotAnAttribute();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsNotAnAttribute();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not an attribute,
					             but it was <null>
					             """);
			}

			public static IEnumerable<Type> NonAttributeTypes() => new List<Type>
			{
				typeof(PublicClass),
				typeof(PublicStruct),
				typeof(IPublicInterface),
				typeof(PublicEnum),
			};
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenTypeIsAnAttribute_ShouldSucceed()
			{
				Type subject = typeof(PublicAttribute);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAnAttribute());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			[MethodDataSource(nameof(NonAttributeTypes))]
			public async Task WhenTypeIsNotAnAttribute_ShouldFail(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAnAttribute());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("*is an attribute*but it was*").AsWildcard();
			}

			public static IEnumerable<Type> NonAttributeTypes() => new List<Type>
			{
				typeof(PublicClass),
				typeof(PublicStruct),
				typeof(IPublicInterface),
				typeof(PublicEnum),
			};
		}
	}
}
