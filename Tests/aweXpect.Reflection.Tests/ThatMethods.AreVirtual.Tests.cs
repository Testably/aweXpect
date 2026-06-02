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
	public sealed class AreVirtual
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFilteringOnlyVirtualMethods_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = typeof(AbstractClassWithMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(m => m.IsVirtual);

				async Task Act()
				{
					await That(subject).AreVirtual();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodsContainNonVirtualMethods_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(AbstractClassWithMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreVirtual();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all virtual,
					             but it contained non-virtual methods [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFilteringOnlyVirtualMethods_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(AbstractClassWithMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(m => m.IsVirtual);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreVirtual());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all virtual,
					             but it only contained virtual methods [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenMethodsContainNonVirtualMethods_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = typeof(AbstractClassWithMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreVirtual());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
			public async Task WhenFilteringOnlyVirtualMethods_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo?> subject = typeof(AbstractClassWithMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(m => m.IsVirtual)
					.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).AreVirtual();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodsContainNonVirtualMethods_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = typeof(AbstractClassWithMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).AreVirtual();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all virtual,
					             but it contained non-virtual methods [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
