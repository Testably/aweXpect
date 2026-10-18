using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotDelegates
	{
		public sealed class TypeTests
		{
			[Test]
			public async Task WhenEnumerableContainsDelegateType_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass), typeof(PublicDelegate),
				};

				async Task Act()
				{
					await That(subject).AreNotDelegates();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             are all not delegates,
					             but it contained delegates

					             Not matching items:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenEnumerableContainsNoDelegateTypes_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicClass),
				};

				async Task Act()
				{
					await That(subject).AreNotDelegates();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class Tests
		{
			[Test]
			public async Task WhenAssembliesContainNoDelegateTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotDelegates>().Types()
					.WhichAreNotDelegates();

				async Task Act()
				{
					await That(subject).AreNotDelegates();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFilteringOnlyDelegates_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotDelegates>().Types()
					.WhichAreDelegates();

				async Task Act()
				{
					await That(subject).AreNotDelegates();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that types which are delegates in assembly containing type ThatTypes.AreNotDelegates
					             are all not delegates,
					             but it contained delegates

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
			public async Task WhenAssembliesContainNoDelegateTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotDelegates>().Types()
					.WhichAreNotDelegates();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotDelegates());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types which are not delegates in assembly containing type ThatTypes.AreNotDelegates
					             also contain a delegate,
					             but it only contained not delegates

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenFilteringOnlyDelegates_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreNotDelegates>().Types()
					.WhichAreDelegates();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotDelegates());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
