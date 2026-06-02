using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class Override
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFilteringOnlyOverridingMethods_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = typeof(ClassWithSealedMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(m => m.GetBaseDefinition().DeclaringType != m.DeclaringType);

				async Task Act()
				{
					await That(subject).Override();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodsContainNonOverridingMethods_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(AbstractClassWithMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).Override();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             all override a base method,
					             but it contained methods which do not override a base method [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFilteringOnlyOverridingMethods_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(ClassWithSealedMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(m => m.GetBaseDefinition().DeclaringType != m.DeclaringType);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.Override());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             do not all override a base method,
					             but it only contained methods which override a base method [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenMethodsContainNonOverridingMethods_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = typeof(AbstractClassWithMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.Override());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
			public async Task WhenFilteringOnlyOverridingMethods_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo?> subject = typeof(ClassWithSealedMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(m => m.GetBaseDefinition().DeclaringType != m.DeclaringType)
					.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).Override();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodsContainNonOverridingMethods_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = typeof(AbstractClassWithMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).Override();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             all override a base method,
					             but it contained methods which do not override a base method [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
