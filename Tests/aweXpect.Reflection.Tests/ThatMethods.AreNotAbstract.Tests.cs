using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class AreNotAbstract
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFilteringOnlyNonAbstractMethods_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = typeof(AbstractClassWithMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(m => !m.IsAbstract);

				async Task Act()
					=> await That(subject).AreNotAbstract();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodsContainAbstractMethods_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(AbstractClassWithMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).AreNotAbstract();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not abstract,
					             but it contained abstract methods [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFilteringOnlyNonAbstractMethods_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(AbstractClassWithMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(m => !m.IsAbstract);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotAbstract());

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             also contain an abstract method,
					             but it only contained non-abstract methods [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenMethodsContainAbstractMethods_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = typeof(AbstractClassWithMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreNotAbstract());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
