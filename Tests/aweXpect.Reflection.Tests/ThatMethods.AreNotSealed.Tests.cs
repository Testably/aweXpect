using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class AreNotSealed
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFilteringOnlyNonSealedMethods_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = typeof(AbstractClassWithMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).AreNotSealed();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodsContainSealedMethods_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(ClassWithSealedMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).AreNotSealed();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not sealed,
					             but it contained sealed methods [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFilteringOnlyNonSealedMethods_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(AbstractClassWithMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotSealed());

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             also contain a sealed method,
					             but it only contained non-sealed methods [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenMethodsContainSealedMethods_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = typeof(ClassWithSealedMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotSealed());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
