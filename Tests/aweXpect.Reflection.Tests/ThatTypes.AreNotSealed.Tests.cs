using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotSealed
	{
		public sealed class TypeTests
		{
			[Test]
			public async Task WhenEnumerableContainsNoSealedTypes_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicAbstractClass),
				};

				async Task Act()
				{
					await That(subject).AreNotSealed();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableContainsSealedType_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicAbstractClass), typeof(PublicSealedClass),
				};

				async Task Act()
				{
					await That(subject).AreNotSealed();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none are sealed,
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

		public sealed class Tests
		{
			[Test]
			public async Task WhenAssembliesContainNonSealedTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotSealed>().Abstract.Types();

				async Task Act()
				{
					await That(subject).AreNotSealed();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFilteringOnlySealedTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotSealed>().Types()
					.Which(type => type is { IsAbstract: false, IsSealed: true, IsInterface: false, });

				async Task Act()
				{
					await That(subject).AreNotSealed();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that types matching type => type is { IsAbstract: false, IsSealed: true, IsInterface: false, } in assembly containing type ThatTypes.AreNotSealed
					             none are sealed,
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
			public async Task WhenAssembliesContainNonSealedTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotSealed>().Abstract.Types();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotSealed());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that abstract types in assembly containing type ThatTypes.AreNotSealed
					             at least one is sealed,
					             but none of * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenFilteringOnlySealedTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotSealed>().Types()
					.Which(type => type is { IsAbstract: false, IsSealed: true, IsInterface: false, });

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotSealed());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
