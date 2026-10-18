using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreClasses
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAssembliesContainNonClassTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreClasses>().Types();

				async Task Act()
				{
					await That(subject).AreClasses();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that types in assembly containing type ThatTypes.AreClasses
					             are all classes,
					             but it contained other types

					             Not matching items:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenFilteringOnlyClasses_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreClasses>().Types()
					.Which(type => type.IsClass && !type.IsRecordClass());

				async Task Act()
				{
					await That(subject).AreClasses();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAssembliesContainNonClassTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreClasses>().Types();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreClasses());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFilteringOnlyClasses_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreClasses>().Types()
					.Which(type => type.IsClass && !type.IsRecordClass());

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreClasses());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types matching type => type.IsClass && !type.IsRecordClass() in assembly containing type ThatTypes.AreClasses
					             are not all classes,
					             but it only contained classes

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
