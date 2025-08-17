using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class AreAbstract
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFilteringOnlyAbstractMethods_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = typeof(AbstractClassWithMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(m => m.IsAbstract);

				async Task Act()
					=> await That(subject).AreAbstract();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodsContainNonAbstractMethods_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(AbstractClassWithMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).AreAbstract();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all abstract,
					             but it contained non-abstract methods [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFilteringOnlyAbstractMethods_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(AbstractClassWithMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(m => m.IsAbstract);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreAbstract());

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all abstract,
					             but it only contained abstract methods [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenMethodsContainNonAbstractMethods_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = typeof(AbstractClassWithMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.AreAbstract());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
