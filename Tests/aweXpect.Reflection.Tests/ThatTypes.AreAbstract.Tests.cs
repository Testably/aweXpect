using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreAbstract
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAssembliesContainNonAbstractTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreAbstract>().Types();

				async Task Act()
				{
					await That(subject).AreAbstract();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that types in assembly containing type ThatTypes.AreAbstract
					             all are abstract,
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
			public async Task WhenFilteringOnlyAbstractTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreAbstract>().Types()
					.Which(type => type is { IsAbstract: true, IsSealed: false, IsInterface: false, });

				async Task Act()
				{
					await That(subject).AreAbstract();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAssembliesContainNonAbstractTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreAbstract>().Types();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreAbstract());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFilteringOnlyAbstractTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreAbstract>().Types()
					.Which(type => type is { IsAbstract: true, IsSealed: false, IsInterface: false, });

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreAbstract());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types matching type => type is { IsAbstract: true, IsSealed: false, IsInterface: false, } in assembly containing type ThatTypes.AreAbstract
					             not all are abstract,
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
