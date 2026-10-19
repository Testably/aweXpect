using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreSealed
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAssembliesContainNonSealedTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreSealed>().Types();

				async Task Act()
				{
					await That(subject).AreSealed();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that types in assembly containing type ThatTypes.AreSealed
					             are all sealed,
					             but it contained non-sealed types

					             Not matching items:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenFilteringOnlySealedTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreSealed>().Types()
					.Which(type => type is { IsAbstract: false, IsSealed: true, IsInterface: false, });

				async Task Act()
				{
					await That(subject).AreSealed();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAssembliesContainNonSealedTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreSealed>().Types();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreSealed());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFilteringOnlySealedTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreSealed>().Types()
					.Which(type => type is { IsAbstract: false, IsSealed: true, IsInterface: false, });

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreSealed());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types matching type => type is { IsAbstract: false, IsSealed: true, IsInterface: false, } in assembly containing type ThatTypes.AreSealed
					             are not all sealed,
					             but it only contained sealed types

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
