using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class AreNotAsync
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenFilteringOnlyNonAsyncMethods_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = typeof(ClassWithAsyncMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(m => !m.IsReallyAsync());

				async Task Act()
				{
					await That(subject).AreNotAsync();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodsContainAsyncMethods_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(ClassWithAsyncMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreNotAsync();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none are async,
					             but * of * were

					             Matching items:
					             [
					               *
					             ]

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenFilteringOnlyNonAsyncMethods_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(ClassWithAsyncMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(m => !m.IsReallyAsync());

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotAsync());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             at least one is async,
					             but none of * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenMethodsContainAsyncMethods_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = typeof(ClassWithAsyncMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotAsync());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenFilteringOnlyNonAsyncMethods_Negated_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = typeof(ClassWithAsyncMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(m => !m.IsReallyAsync())
					.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotAsync());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             at least one is async,
					             but none of * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenFilteringOnlyNonAsyncMethods_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo?> subject = typeof(ClassWithAsyncMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(m => !m.IsReallyAsync())
					.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).AreNotAsync();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodsContainAsyncMethods_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = typeof(ClassWithAsyncMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).AreNotAsync();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none are async,
					             but * of * were

					             Matching items:
					             [
					               *
					             ]

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
