using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class AreAsync
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFilteringOnlyAsyncMethods_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = typeof(ClassWithAsyncMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(m => m.IsReallyAsync());

				async Task Act()
				{
					await That(subject).AreAsync();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodsContainNonAsyncMethods_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(ClassWithAsyncMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreAsync();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all async,
					             but it contained non-async methods [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFilteringOnlyAsyncMethods_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(ClassWithAsyncMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(m => m.IsReallyAsync());

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreAsync());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all async,
					             but it only contained async methods [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenMethodsContainNonAsyncMethods_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = typeof(ClassWithAsyncMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreAsync());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
			public async Task WhenFilteringOnlyAsyncMethods_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo?> subject = typeof(ClassWithAsyncMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(m => m.IsReallyAsync())
					.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).AreAsync();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodsContainNonAsyncMethods_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = typeof(ClassWithAsyncMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).AreAsync();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all async,
					             but it contained non-async methods [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyAsyncMethods_Negated_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = typeof(ClassWithAsyncMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(m => m.IsReallyAsync())
					.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreAsync());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all async,
					             but it only contained async methods [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
