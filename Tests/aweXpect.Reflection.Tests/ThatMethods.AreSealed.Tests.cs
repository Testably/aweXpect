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
			[Fact]
			public async Task WhenFilteringOnlySealedMethods_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = typeof(ClassWithSealedMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(m => m.IsFinal && m.IsVirtual);

				async Task Act()
					=> await That(subject).AreSealed();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodsContainNonSealedMethods_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(AbstractClassWithMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).AreSealed();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all sealed,
					             but it contained non-sealed methods [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFilteringOnlySealedMethods_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(ClassWithSealedMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(m => m.IsFinal && m.IsVirtual);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreSealed());

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all sealed,
					             but it only contained sealed methods [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenMethodsContainNonSealedMethods_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = typeof(AbstractClassWithMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreSealed());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
