using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreExtensionProperties
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenPropertiesContainNonExtensionProperties_Negated_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(ClassWithRequiredMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreExtensionProperties());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertiesContainNonExtensionProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(ClassWithRequiredMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreExtensionProperties();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all extension properties,
					             but it contained non-extension properties [
					               *
					             ]
					             """).AsWildcard();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
			public async Task WhenPropertiesContainNonExtensionProperties_ShouldFail()
			{
				IAsyncEnumerable<PropertyInfo?> subject = typeof(ClassWithRequiredMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).AreExtensionProperties();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all extension properties,
					             but it contained non-extension properties [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif

#if NET10_0_OR_GREATER
		public sealed class NewSyntaxTests
		{
			[Fact]
			public async Task WhenAllAreExtensionProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(StaticClassWithNewExtensionProperties).GetExtensionProperty("IsBlankText"),
					typeof(StaticClassWithNewExtensionProperties).GetExtensionProperty("DefaultValue"),
					typeof(GenericClassWithNewExtensionProperties).GetExtensionProperty("Capacity"),
				];

				async Task Act()
				{
					await That(subject).AreExtensionProperties();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllAreExtensionProperties_Negated_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(StaticClassWithNewExtensionProperties).GetExtensionProperty("IsBlankText"),
					typeof(StaticClassWithNewExtensionProperties).GetExtensionProperty("DefaultValue"),
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreExtensionProperties());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all extension properties,
					             but it only contained extension properties [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenContainingARegularProperty_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(StaticClassWithNewExtensionProperties).GetExtensionProperty("DefaultValue"),
					typeof(StaticClassWithNewExtensionProperties).GetProperty(
						nameof(StaticClassWithNewExtensionProperties.RegularProperty))!,
				];

				async Task Act()
				{
					await That(subject).AreExtensionProperties();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all extension properties,
					             but it contained non-extension properties [
					               *
					             ]
					             """).AsWildcard();
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task WhenAllAreExtensionProperties_AsyncEnumerable_ShouldSucceed()
			{
				IAsyncEnumerable<PropertyInfo?> subject = new[]
				{
					typeof(StaticClassWithNewExtensionProperties).GetExtensionProperty("IsBlankText"),
					typeof(StaticClassWithNewExtensionProperties).GetExtensionProperty("DefaultValue"),
				}.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).AreExtensionProperties();
				}

				await That(Act).DoesNotThrow();
			}
#endif
		}
#endif
	}
}
