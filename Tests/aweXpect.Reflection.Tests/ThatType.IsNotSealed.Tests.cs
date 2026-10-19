using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotSealed
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenTypeIsNotSealed_ShouldFailWithNegatedAssertion()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotSealed());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is sealed,
					             but it was non-sealed PublicAbstractClass
					             """);
			}

			[Test]
			[MethodDataSource(nameof(NonSealedTypes))]
			public async Task WhenTypeIsNotSealed_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).IsNotSealed();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsNotSealed();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not sealed,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenTypeIsSealed_ShouldFail()
			{
				Type subject = typeof(PublicSealedClass);

				async Task Act()
				{
					await That(subject).IsNotSealed();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not sealed,
					             but it was sealed PublicSealedClass
					             """);
			}

			[Test]
			[MethodDataSource(nameof(SealedTypesForNegated))]
			public async Task WhenTypeIsSealed_ShouldSucceedWithNegatedAssertion(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotSealed());
				}

				await That(Act).DoesNotThrow();
			}

			public static IEnumerable<Type> SealedTypesForNegated() => new List<Type>
			{
				typeof(PublicSealedClass),
			};

			public static IEnumerable<Type> NonSealedTypes()
				=>
				[
					typeof(PublicAbstractClass),
					typeof(PublicStaticClass),
					typeof(Container.PublicNestedClass),
					typeof(IPublicInterface),
				];
		}
	}
}
