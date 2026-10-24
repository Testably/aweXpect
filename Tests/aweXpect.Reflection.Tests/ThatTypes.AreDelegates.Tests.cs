using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreDelegates
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAssembliesContainNonDelegateTypes_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreDelegates>().Types();

				async Task Act()
				{
					await That(subject).AreDelegates();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that types in assembly containing type ThatTypes.AreDelegates
					             all are delegates,
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
			public async Task WhenFilteringOnlyDelegates_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreDelegates>().Types()
					.WhichAreDelegates();

				async Task Act()
				{
					await That(subject).AreDelegates();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class TypeTests
		{
			[Test]
			public async Task WhenEnumerableContainsNonDelegateType_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicDelegate), typeof(PublicClass),
				};

				async Task Act()
				{
					await That(subject).AreDelegates();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all are delegates,
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
			public async Task WhenEnumerableContainsOnlyDelegateTypes_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(PublicDelegate),
				};

				async Task Act()
				{
					await That(subject).AreDelegates();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAssembliesContainNonDelegateTypes_ShouldSucceed()
			{
				Filtered.Types subject = In.AssemblyContaining<AreDelegates>().Types();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreDelegates());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFilteringOnlyDelegates_ShouldFail()
			{
				Filtered.Types subject = In.AssemblyContaining<AreDelegates>().Types()
					.WhichAreDelegates();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreDelegates());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types which are delegates in assembly containing type ThatTypes.AreDelegates
					             not all are delegates,
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
