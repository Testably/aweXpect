using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructors
{
	public sealed class AreStatic
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenConstructorsContainNonStaticConstructors_ShouldFail()
			{
				IEnumerable<ConstructorInfo> subject = typeof(TestClassWithStaticMembers)
					.GetConstructors(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
					                 BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreStatic();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all static,
					             but it contained non-static constructors [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyStaticConstructors_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> subject = typeof(TestClassWithStaticMembers)
					.GetConstructors(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreStatic();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenConstructorsContainNonStaticConstructors_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> subject = typeof(TestClassWithStaticMembers)
					.GetConstructors(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
					                 BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreStatic());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyStaticConstructors_ShouldFail()
			{
				IEnumerable<ConstructorInfo> subject = typeof(TestClassWithStaticMembers)
					.GetConstructors(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreStatic());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all static,
					             but it only contained static constructors [
					               *
					             ]
					             """).AsWildcard();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
			public async Task WhenConstructorsContainNonStaticConstructors_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo?> subject = typeof(TestClassWithStaticMembers)
					.GetConstructors(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
					                 BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<ConstructorInfo?>();

				async Task Act()
				{
					await That(subject).AreStatic();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all static,
					             but it contained non-static constructors [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyStaticConstructors_ShouldSucceed()
			{
				IAsyncEnumerable<ConstructorInfo?> subject = typeof(TestClassWithStaticMembers)
					.GetConstructors(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<ConstructorInfo?>();

				async Task Act()
				{
					await That(subject).AreStatic();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyStaticConstructors_Negated_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo?> subject = typeof(TestClassWithStaticMembers)
					.GetConstructors(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<ConstructorInfo?>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreStatic());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all static,
					             but it only contained static constructors [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
