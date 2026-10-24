using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class AreNotStatic
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenFieldsContainStaticFields_ShouldFail()
			{
				IEnumerable<FieldInfo> subject = typeof(TestClassWithStaticMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
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

			[Test]
			public async Task WhenFilteringOnlyNonStaticFields_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subject = typeof(TestClassWithStaticMembers)
					.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

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
			public async Task WhenFieldsContainStaticFields_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subject = typeof(TestClassWithStaticMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
					           BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotStatic());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFilteringOnlyNonStaticFields_ShouldFail()
			{
				IEnumerable<FieldInfo> subject = typeof(TestClassWithStaticMembers)
					.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

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
			public async Task WhenFieldsContainStaticFields_ShouldFail()
			{
				IAsyncEnumerable<FieldInfo?> subject = typeof(TestClassWithStaticMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
					           BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<FieldInfo?>();

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
			public async Task WhenFilteringOnlyNonStaticFields_ShouldSucceed()
			{
				IAsyncEnumerable<FieldInfo?> subject = typeof(TestClassWithStaticMembers)
					.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<FieldInfo?>();

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
