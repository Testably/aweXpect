using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class AreNotNullable
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllFieldsAreNotNullable_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subject = typeof(ClassWithNonNullableMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreNotNullable();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldsContainNull_ShouldFail()
			{
				IEnumerable<FieldInfo?> subject = [null,];

				async Task Act()
				{
					await That(subject).AreNotNullable();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not nullable,
					             but it contained nullable fields [
					               <null>
					             ]
					             """);
			}

			[Fact]
			public async Task WhenFieldsContainNullableFields_ShouldFail()
			{
				IEnumerable<FieldInfo> subject = typeof(ClassWithMixedNullableMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreNotNullable();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not nullable,
					             but it contained nullable fields [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllFieldsAreNotNullable_ShouldFail()
			{
				IEnumerable<FieldInfo> subject = typeof(ClassWithNonNullableMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotNullable());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             also contain a nullable field,
					             but it only contained non-nullable fields [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFieldsContainNullableFields_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subject = typeof(ClassWithMixedNullableMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotNullable());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
			public async Task WhenAllFieldsAreNotNullable_ShouldSucceed()
			{
				IAsyncEnumerable<FieldInfo?> subject = typeof(ClassWithNonNullableMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).AreNotNullable();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldsContainNullableFields_ShouldFail()
			{
				IAsyncEnumerable<FieldInfo?> subject = typeof(ClassWithMixedNullableMembers)
					.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).AreNotNullable();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not nullable,
					             but it contained nullable fields [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
