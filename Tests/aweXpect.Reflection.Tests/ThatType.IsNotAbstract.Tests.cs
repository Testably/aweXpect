using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotAbstract
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenTypeIsAbstract_ShouldFail()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
				{
					await That(subject).IsNotAbstract();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not abstract,
					             but it was abstract PublicAbstractClass
					             """);
			}

			[Test]
			[MethodDataSource(nameof(AbstractTypesForNegated))]
			public async Task WhenTypeIsAbstract_ShouldSucceedWithNegatedAssertion(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAbstract());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNotAbstract_ShouldFailWithNegatedAssertion()
			{
				Type subject = typeof(PublicClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAbstract());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is abstract,
					             but it was non-abstract PublicClass
					             """);
			}

			[Test]
			[MethodDataSource(nameof(NonAbstractTypes))]
			public async Task WhenTypeIsNotAbstract_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).IsNotAbstract();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsNotAbstract();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not abstract,
					             but it was <null>
					             """);
			}

			public static IEnumerable<Type> AbstractTypesForNegated() => new List<Type>
			{
				typeof(PublicAbstractClass),
			};

			public static IEnumerable<Type> NonAbstractTypes()
				=>
				[
					typeof(PublicStaticClass),
					typeof(PublicSealedClass),
					typeof(Container.PublicNestedClass),
					typeof(IPublicInterface),
				];
		}
	}
}
