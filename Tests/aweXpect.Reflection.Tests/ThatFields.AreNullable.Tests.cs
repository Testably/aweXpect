using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class AreNullable
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllFieldsAreNullable_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subject = typeof(ClassWithNullableMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreNullable();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldsContainNonNullableFields_ShouldFail()
			{
				IEnumerable<FieldInfo> subject = typeof(ClassWithMixedNullableMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreNullable();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             all are nullable,
					             but only * of * were

					             Not matching items:
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
			public async Task WhenAllFieldsAreNullable_ShouldFail()
			{
				IEnumerable<FieldInfo> subject = typeof(ClassWithNullableMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNullable());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             not all are nullable,
					             but all * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenFieldsContainNonNullableFields_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subject = typeof(ClassWithMixedNullableMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNullable());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenAllFieldsAreNullable_ShouldSucceed()
			{
				IAsyncEnumerable<FieldInfo?> subject = typeof(ClassWithNullableMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).AreNullable();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldsContainNonNullableFields_ShouldFail()
			{
				IAsyncEnumerable<FieldInfo?> subject = typeof(ClassWithMixedNullableMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).AreNullable();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             all are nullable,
					             but only * of * were

					             Not matching items:
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
