using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotNested
	{
		public sealed class TypeTests
		{
			[Test]
			public async Task WhenEnumerableContainsNestedType_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass), typeof(Container.PublicNestedClass),
				};

				async Task Act()
				{
					await That(subject).AreNotNested();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none are nested,
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
			public async Task WhenEnumerableContainsNoNestedTypes_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass),
				};

				async Task Act()
				{
					await That(subject).AreNotNested();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class Tests
		{
			[Test]
			public async Task WhenAssembliesContainNonNestedTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotNested>().Types()
					.Which(type => !type.IsNested);

				async Task Act()
				{
					await That(subject).AreNotNested();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFilteringOnlyNestedTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotNested>().Types()
					.Which(type => type.IsNested);

				async Task Act()
				{
					await That(subject).AreNotNested();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that types matching type => type.IsNested in assembly containing type ThatTypes.AreNotNested
					             none are nested,
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
			public async Task WhenAssembliesContainNonNestedTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotNested>().Types()
					.Which(type => !type.IsNested);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotNested());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types matching type => !type.IsNested in assembly containing type ThatTypes.AreNotNested
					             at least one is nested,
					             but none of * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenFilteringOnlyNestedTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotNested>().Types()
					.Which(type => type.IsNested);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotNested());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
