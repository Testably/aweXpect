using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreAttributes
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAssembliesContainNonAttributeTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreAttributes>().Types();

				async Task Act()
				{
					await That(subject).AreAttributes();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that types in assembly containing type ThatTypes.AreAttributes
					             all are attributes,
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
			public async Task WhenFilteringOnlyAttributes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreAttributes>().Types()
					.WhichAreAttributes();

				async Task Act()
				{
					await That(subject).AreAttributes();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class TypeTests
		{
			[Test]
			public async Task WhenEnumerableContainsNonAttributeType_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicAttribute), typeof(PublicClass),
				};

				async Task Act()
				{
					await That(subject).AreAttributes();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all are attributes,
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
			public async Task WhenEnumerableContainsOnlyAttributeTypes_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicAttribute),
				};

				async Task Act()
				{
					await That(subject).AreAttributes();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAssembliesContainNonAttributeTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreAttributes>().Types();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreAttributes());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFilteringOnlyAttributes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreAttributes>().Types()
					.WhichAreAttributes();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreAttributes());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types which are attributes in assembly containing type ThatTypes.AreAttributes
					             not all are attributes,
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
