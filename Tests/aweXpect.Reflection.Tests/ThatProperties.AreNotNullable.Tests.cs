using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreNotNullable
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllPropertiesAreNotNullable_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(ClassWithNonNullableMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreNotNullable();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertiesContainNullableProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(ClassWithMixedNullableMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreNotNullable();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not nullable,
					             but it contained nullable properties [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenPropertiesContainNull_ShouldFail()
			{
				IEnumerable<PropertyInfo?> subject = [null,];

				async Task Act()
				{
					await That(subject).AreNotNullable();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not nullable,
					             but it contained nullable properties [
					               <null>
					             ]
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllPropertiesAreNotNullable_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(ClassWithNonNullableMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotNullable());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             also contain a nullable property,
					             but it only contained non-nullable properties [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenPropertiesContainNullableProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(ClassWithMixedNullableMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

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
			public async Task WhenAllPropertiesAreNotNullable_ShouldSucceed()
			{
				IAsyncEnumerable<PropertyInfo?> subject = typeof(ClassWithNonNullableMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).AreNotNullable();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertiesContainNullableProperties_ShouldFail()
			{
				IAsyncEnumerable<PropertyInfo?> subject = typeof(ClassWithMixedNullableMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).AreNotNullable();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not nullable,
					             but it contained nullable properties [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
