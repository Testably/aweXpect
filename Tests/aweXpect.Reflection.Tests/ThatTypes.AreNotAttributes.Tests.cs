using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotAttributes
	{
		public sealed class TypeTests
		{
			[Test]
			public async Task WhenEnumerableContainsAttributeType_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass), typeof(PublicAttribute),
				};

				async Task Act()
				{
					await That(subject).AreNotAttributes();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none are attributes,
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
			public async Task WhenEnumerableContainsNoAttributeTypes_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass),
				};

				async Task Act()
				{
					await That(subject).AreNotAttributes();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class Tests
		{
			[Test]
			public async Task WhenAssembliesContainNoAttributeTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotAttributes>().Types()
					.WhichAreNotAttributes();

				async Task Act()
				{
					await That(subject).AreNotAttributes();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFilteringOnlyAttributes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotAttributes>().Types()
					.WhichAreAttributes();

				async Task Act()
				{
					await That(subject).AreNotAttributes();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that types which are attributes in assembly containing type ThatTypes.AreNotAttributes
					             none are attributes,
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
			public async Task WhenAssembliesContainNoAttributeTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotAttributes>().Types()
					.WhichAreNotAttributes();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotAttributes());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types which are not attributes in assembly containing type ThatTypes.AreNotAttributes
					             at least one is an attribute,
					             but none of * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenFilteringOnlyAttributes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotAttributes>().Types()
					.WhichAreAttributes();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotAttributes());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
