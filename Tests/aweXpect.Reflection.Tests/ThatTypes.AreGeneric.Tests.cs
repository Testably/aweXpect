using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed partial class AreGeneric
	{
		public sealed class TypeTests
		{
			[Test]
			public async Task WhenEnumerableContainsNonGenericType_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicGenericClass<>), typeof(PublicClass),
				};

				async Task Act()
				{
					await That(subject).AreGeneric();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all are generic,
					             but only * of * were

					             Not matching items:
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
			public async Task WhenEnumerableContainsOnlyGenericTypes_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicGenericClass<>),
				};

				async Task Act()
				{
					await That(subject).AreGeneric();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<Type?> subject = [typeof(PublicGenericClass<>), null,];

				async Task Act()
				{
					await That(subject).AreGeneric();
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.AreGeneric());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all are generic,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               PublicGenericClass<>,
					               <null>
					             ]
					             """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             not all are generic,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               PublicGenericClass<>,
					               <null>
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}
		}

		public sealed class Tests
		{
			[Test]
			public async Task WhenAssembliesContainNonGenericTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreGeneric>().Types();

				async Task Act()
				{
					await That(subject).AreGeneric();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that types in assembly containing type ThatTypes.AreGeneric
					             all are generic,
					             but only * of * were

					             Not matching items:
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
			public async Task WhenFilteringOnlyGenericTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreGeneric>().Types()
					.Which(type => type.IsGenericType);

				async Task Act()
				{
					await That(subject).AreGeneric();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAssembliesContainNonGenericTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreGeneric>().Types();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreGeneric());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFilteringOnlyGenericTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreGeneric>().Types()
					.Which(type => type.IsGenericType);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreGeneric());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types matching type => type.IsGenericType in assembly containing type ThatTypes.AreGeneric
					             not all are generic,
					             but all * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
