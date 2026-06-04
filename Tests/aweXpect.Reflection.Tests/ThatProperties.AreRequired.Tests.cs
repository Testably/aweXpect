using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreRequired
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFilteringOnlyRequiredProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(ClassWithOnlyRequiredMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreRequired();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertiesContainNonRequiredProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(ClassWithRequiredMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreRequired();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all required,
					             but it contained non-required properties [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFilteringOnlyRequiredProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(ClassWithOnlyRequiredMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreRequired());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all required,
					             but it only contained required properties [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenPropertiesContainNonRequiredProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(ClassWithRequiredMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreRequired());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
			public async Task WhenFilteringOnlyRequiredProperties_ShouldSucceed()
			{
				IAsyncEnumerable<PropertyInfo?> subject = typeof(ClassWithOnlyRequiredMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).AreRequired();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertiesContainNonRequiredProperties_ShouldFail()
			{
				IAsyncEnumerable<PropertyInfo?> subject = typeof(ClassWithRequiredMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).AreRequired();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all required,
					             but it contained non-required properties [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
