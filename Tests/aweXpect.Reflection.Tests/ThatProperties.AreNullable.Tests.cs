using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreNullable
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllPropertiesAreNullable_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(ClassWithNullableMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreNullable();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertiesContainNonNullableProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(ClassWithMixedNullableMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

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
			public async Task WhenAllPropertiesAreNullable_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(ClassWithNullableMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

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
			public async Task WhenPropertiesContainNonNullableProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(ClassWithMixedNullableMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

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
			public async Task WhenAllPropertiesAreNullable_ShouldSucceed()
			{
				IAsyncEnumerable<PropertyInfo?> subject = typeof(ClassWithNullableMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).AreNullable();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertiesContainNonNullableProperties_ShouldFail()
			{
				IAsyncEnumerable<PropertyInfo?> subject = typeof(ClassWithMixedNullableMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<PropertyInfo?>();

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
