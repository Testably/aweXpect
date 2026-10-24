using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotAbstract
	{
		public sealed class TypeTests
		{
			[Test]
			public async Task WhenEnumerableContainsAbstractType_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass), typeof(PublicAbstractClass),
				};

				async Task Act()
				{
					await That(subject).AreNotAbstract();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none are abstract,
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
			public async Task WhenEnumerableContainsNoAbstractTypes_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicSealedClass),
				};

				async Task Act()
				{
					await That(subject).AreNotAbstract();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class Tests
		{
			[Test]
			public async Task WhenAssembliesContainNonAbstractTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotAbstract>().Sealed.Types();

				async Task Act()
				{
					await That(subject).AreNotAbstract();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFilteringOnlyAbstractTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotAbstract>().Types()
					.Which(type => type is { IsAbstract: true, IsSealed: false, IsInterface: false, });

				async Task Act()
				{
					await That(subject).AreNotAbstract();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that types matching type => type is { IsAbstract: true, IsSealed: false, IsInterface: false, } in assembly containing type ThatTypes.AreNotAbstract
					             none are abstract,
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
			public async Task WhenAssembliesContainNonAbstractTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotAbstract>().Sealed.Types();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotAbstract());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that sealed types in assembly containing type ThatTypes.AreNotAbstract
					             at least one is abstract,
					             but none of * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenFilteringOnlyAbstractTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotAbstract>().Types()
					.Which(type => type is { IsAbstract: true, IsSealed: false, IsInterface: false, });

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotAbstract());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
