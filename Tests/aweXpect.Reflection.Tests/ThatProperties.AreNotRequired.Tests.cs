using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreNotRequired
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFilteringOnlyNonRequiredProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(BaseClassWithMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreNotRequired();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertiesContainRequiredProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(ClassWithOnlyRequiredMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreNotRequired();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not required,
					             but it contained required properties [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFilteringOnlyNonRequiredProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(BaseClassWithMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotRequired());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             also contain a required property,
					             but it only contained non-required properties [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenPropertiesContainRequiredProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(ClassWithOnlyRequiredMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotRequired());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
			public async Task WhenFilteringOnlyNonRequiredProperties_ShouldSucceed()
			{
				IAsyncEnumerable<PropertyInfo?> subject = typeof(BaseClassWithMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).AreNotRequired();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertiesContainRequiredProperties_ShouldFail()
			{
				IAsyncEnumerable<PropertyInfo?> subject = typeof(ClassWithOnlyRequiredMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).AreNotRequired();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not required,
					             but it contained required properties [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
