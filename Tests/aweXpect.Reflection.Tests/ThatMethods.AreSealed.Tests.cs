using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class AreSealed
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenFilteringOnlySealedMethods_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = typeof(ClassWithSealedMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(m => m.IsFinal && m.IsVirtual);

				async Task Act()
				{
					await That(subject).AreSealed();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodsContainNonSealedMethods_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(AbstractClassWithMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreSealed();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             all are sealed,
					             but none of * were

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
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenFilteringOnlySealedMethods_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(ClassWithSealedMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(m => m.IsFinal && m.IsVirtual);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreSealed());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             not all are sealed,
					             but all * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenMethodsContainNonSealedMethods_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = typeof(AbstractClassWithMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreSealed());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
