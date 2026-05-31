using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class AreStatic
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFieldsContainNonStaticFields_ShouldFail()
			{
				IEnumerable<FieldInfo> subject = typeof(TestClassWithStaticMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
					           BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreStatic();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all static,
					             but it contained non-static fields [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyStaticFields_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subject = typeof(TestClassWithStaticMembers)
					.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);

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
			public async Task WhenFieldsContainNonStaticFields_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subject = typeof(TestClassWithStaticMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
					           BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreStatic());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyStaticFields_ShouldFail()
			{
				IEnumerable<FieldInfo> subject = typeof(TestClassWithStaticMembers)
					.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreStatic());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all static,
					             but it only contained static fields [
					               *
					             ]
					             """).AsWildcard();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
			public async Task WhenFieldsContainNonStaticFields_ShouldFail()
			{
				IAsyncEnumerable<FieldInfo?> subject = typeof(TestClassWithStaticMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
					           BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).AreStatic();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all static,
					             but it contained non-static fields [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyStaticFields_ShouldSucceed()
			{
				IAsyncEnumerable<FieldInfo?> subject = typeof(TestClassWithStaticMembers)
					.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).AreStatic();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyStaticFields_Negated_ShouldFail()
			{
				IAsyncEnumerable<FieldInfo?> subject = typeof(TestClassWithStaticMembers)
					.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreStatic());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all static,
					             but it only contained static fields [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
