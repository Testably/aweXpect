using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotStatic
	{
		public sealed class TypeTests
		{
			[Test]
			public async Task WhenEnumerableContainsNoStaticTypes_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicSealedClass),
				};

				async Task Act()
				{
					await That(subject).AreNotStatic();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableContainsStaticType_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicSealedClass), typeof(PublicStaticClass),
				};

				async Task Act()
				{
					await That(subject).AreNotStatic();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none are static,
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
			public async Task WhenAssembliesContainNonStaticTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotStatic>().Abstract.Types();

				async Task Act()
				{
					await That(subject).AreNotStatic();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFilteringOnlyStaticTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotStatic>().Types()
					.Which(type => type is { IsAbstract: true, IsSealed: true, IsInterface: false, });

				async Task Act()
				{
					await That(subject).AreNotStatic();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that types matching type => type is { IsAbstract: true, IsSealed: true, IsInterface: false, } in assembly containing type ThatTypes.AreNotStatic
					             none are static,
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
			public async Task WhenAssembliesContainNonStaticTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotStatic>().Abstract.Types();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotStatic());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that abstract types in assembly containing type ThatTypes.AreNotStatic
					             at least one is static,
					             but none of * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenFilteringOnlyStaticTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotStatic>().Types()
					.Which(type => type is { IsAbstract: true, IsSealed: true, IsInterface: false, });

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotStatic());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
