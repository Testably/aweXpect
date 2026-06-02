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
	public sealed class AreNotVirtual
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFilteringOnlyNonVirtualMethods_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = typeof(AbstractClassWithMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(m => !m.IsVirtual);

				async Task Act()
				{
					await That(subject).AreNotVirtual();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodsContainVirtualMethods_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(AbstractClassWithMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreNotVirtual();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not virtual,
					             but it contained virtual methods [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFilteringOnlyNonVirtualMethods_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(AbstractClassWithMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(m => !m.IsVirtual);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotVirtual());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             also contain a virtual method,
					             but it only contained non-virtual methods [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenMethodsContainVirtualMethods_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = typeof(AbstractClassWithMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotVirtual());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
			public async Task WhenFilteringOnlyNonVirtualMethods_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo?> subject = typeof(AbstractClassWithMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(m => !m.IsVirtual)
					.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).AreNotVirtual();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodsContainVirtualMethods_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = typeof(AbstractClassWithMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).AreNotVirtual();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not virtual,
					             but it contained virtual methods [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
