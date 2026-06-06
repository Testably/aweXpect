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
			[Fact]
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

			[Fact]
			public async Task WhenFieldsContainNonNullableFields_ShouldFail()
			{
				IEnumerable<FieldInfo> subject = typeof(ClassWithMixedNullableMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreNullable();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all nullable,
					             but it contained non-nullable fields [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllFieldsAreNullable_ShouldFail()
			{
				IEnumerable<FieldInfo> subject = typeof(ClassWithNullableMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNullable());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all nullable,
					             but it only contained nullable fields [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
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
			[Fact]
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

			[Fact]
			public async Task WhenFieldsContainNonNullableFields_ShouldFail()
			{
				IAsyncEnumerable<FieldInfo?> subject = typeof(ClassWithMixedNullableMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).AreNullable();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all nullable,
					             but it contained non-nullable fields [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
