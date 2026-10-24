using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class AreNotStatic
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenFilteringOnlyNonStaticMethods_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = typeof(TestClassWithStaticMembers)
					.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreNotStatic();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodsContainStaticMethods_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(TestClassWithStaticMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
					            BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreNotStatic();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none are static,
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
			public async Task WhenFilteringOnlyNonStaticMethods_ShouldFail()
			{
				IEnumerable<MethodInfo> subject = typeof(TestClassWithStaticMembers)
					.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotStatic());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             at least one is static,
					             but none of * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenMethodsContainStaticMethods_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject = typeof(TestClassWithStaticMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
					            BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotStatic());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenFilteringOnlyNonStaticMethods_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo?> subject = typeof(TestClassWithStaticMembers)
					.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).AreNotStatic();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodsContainStaticMethods_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = typeof(TestClassWithStaticMembers)
					.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
					            BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).AreNotStatic();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none are static,
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
