using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class DoNotOverride
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFilteringOnlyNonOverridingMethods_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = typeof(AbstractClassWithMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoNotOverride();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodsContainOverridingMethods_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(ClassWithSealedMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoNotOverride();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             all do not override a base method,
					             but it contained methods which override a base method [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFilteringOnlyNonOverridingMethods_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(AbstractClassWithMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotOverride());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             also contain a method which overrides a base method,
					             but it only contained methods which do not override a base method [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenMethodsContainOverridingMethods_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = typeof(ClassWithSealedMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotOverride());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
			public async Task WhenFilteringOnlyNonOverridingMethods_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo?> subject = typeof(AbstractClassWithMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).DoNotOverride();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodsContainOverridingMethods_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = typeof(ClassWithSealedMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).DoNotOverride();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             all do not override a base method,
					             but it contained methods which override a base method [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
