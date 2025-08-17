using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class AreStatic
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFilteringOnlyStaticMethods_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = typeof(TestClassWithStaticMembers)
					.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).AreStatic();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodsContainNonStaticMethods_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(TestClassWithStaticMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
					            BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).AreStatic();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all static,
					             but it contained non-static methods [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFilteringOnlyStaticMethods_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(TestClassWithStaticMembers)
					.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreStatic());

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all static,
					             but it only contained static methods [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenMethodsContainNonStaticMethods_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = typeof(TestClassWithStaticMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
					            BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreStatic());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
