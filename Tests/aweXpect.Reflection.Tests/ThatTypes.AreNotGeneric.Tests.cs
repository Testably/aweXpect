using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotGeneric
	{
		public sealed class TypeTests
		{
			[Test]
			public async Task WhenEnumerableContainsGenericType_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass), typeof(PublicGenericClass<>),
				};

				async Task Act()
				{
					await That(subject).AreNotGeneric();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none are generic,
					             but * of * were

					             Matching items:
					             [
					               *
					             ]

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenEnumerableContainsNoGenericTypes_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass),
				};

				async Task Act()
				{
					await That(subject).AreNotGeneric();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<Type?> subject = [typeof(PublicClass), null,];

				async Task Act()
				{
					await That(subject).AreNotGeneric();
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotGeneric());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none are generic,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               PublicClass,
					               <null>
					             ]
					             """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             at least one is generic,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               PublicClass,
					               <null>
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}
		}

		public sealed class Tests
		{
			[Test]
			public async Task WhenAssembliesContainNonGenericTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotGeneric>().Types()
					.Which(type => !type.IsGenericType);

				async Task Act()
				{
					await That(subject).AreNotGeneric();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFilteringOnlyGenericTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotGeneric>().Types()
					.Which(type => type.IsGenericType);

				async Task Act()
				{
					await That(subject).AreNotGeneric();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that types matching type => type.IsGenericType in assembly containing type ThatTypes.AreNotGeneric
					             none are generic,
					             but * of * were

					             Matching items:
					             [
					               *
					             ]

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAssembliesContainNonGenericTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotGeneric>().Types()
					.Which(type => !type.IsGenericType);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotGeneric());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types matching type => !type.IsGenericType in assembly containing type ThatTypes.AreNotGeneric
					             at least one is generic,
					             but none of * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenFilteringOnlyGenericTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotGeneric>().Types()
					.Which(type => type.IsGenericType);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotGeneric());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
