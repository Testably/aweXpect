using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructors
{
	public sealed class AreNotStatic
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenConstructorsContainStaticConstructors_ShouldFail()
			{
				IEnumerable<ConstructorInfo> subject = typeof(TestClassWithStaticMembers)
					.GetConstructors(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
					                 BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

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

			[Test]
			public async Task WhenFilteringOnlyNonStaticConstructors_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> subject = typeof(TestClassWithStaticMembers)
					.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreNotStatic();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenConstructorsContainStaticConstructors_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> subject = typeof(TestClassWithStaticMembers)
					.GetConstructors(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
					                 BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotStatic());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFilteringOnlyNonStaticConstructors_ShouldFail()
			{
				IEnumerable<ConstructorInfo> subject = typeof(TestClassWithStaticMembers)
					.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

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
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenConstructorsContainStaticConstructors_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo?> subject = typeof(TestClassWithStaticMembers)
					.GetConstructors(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
					                 BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<ConstructorInfo?>();

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

			[Test]
			public async Task WhenFilteringOnlyNonStaticConstructors_ShouldSucceed()
			{
				IAsyncEnumerable<ConstructorInfo?> subject = typeof(TestClassWithStaticMembers)
					.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<ConstructorInfo?>();

				async Task Act()
				{
					await That(subject).AreNotStatic();
				}

				await That(Act).DoesNotThrow();
			}
		}
#endif
	}
}
