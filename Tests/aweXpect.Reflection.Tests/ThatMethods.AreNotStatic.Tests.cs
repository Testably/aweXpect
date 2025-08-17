using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class AreNotStatic
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFilteringOnlyNonStaticMethods_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = typeof(TestClassWithStaticMembers)
					.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).AreNotStatic();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodsContainStaticMethods_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(TestClassWithStaticMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
					            BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).AreNotStatic();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not static,
					             but it contained static methods [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFilteringOnlyNonStaticMethods_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(TestClassWithStaticMembers)
					.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotStatic());

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             also contain a static method,
					             but it only contained non-static methods [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenMethodsContainStaticMethods_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = typeof(TestClassWithStaticMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
					            BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotStatic());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
