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
					             are all not abstract,
					             but it contained abstract types

					             Not matching items:
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
					             are all not abstract,
					             but it contained abstract types

					             Not matching items:
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
					             also contain an abstract type,
					             but it only contained non-abstract types

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
