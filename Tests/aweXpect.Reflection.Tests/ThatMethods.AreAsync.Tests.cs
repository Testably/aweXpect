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
			[Test]
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

			[Test]
			public async Task WhenMethodsContainNonAsyncMethods_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(ClassWithAsyncMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreAsync();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             are all async,
					             but it contained non-async methods

					             Not matching items:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenFilteringOnlyAsyncMethods_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(ClassWithAsyncMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(m => m.IsReallyAsync());

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreAsync());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             are not all async,
					             but it only contained async methods

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
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
			[Test]
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

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             are not all async,
					             but it only contained async methods

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
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

			[Test]
			public async Task WhenMethodsContainNonAsyncMethods_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = typeof(ClassWithAsyncMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).AreAsync();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             are all async,
					             but it contained non-async methods

					             Not matching items:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
