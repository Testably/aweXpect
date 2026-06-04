using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreNotExtensionProperties
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllPropertiesAreNonExtensionProperties_Negated_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(ClassWithRequiredMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotExtensionProperties());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             also contain an extension property,
					             but it only contained non-extension properties [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenAllPropertiesAreNonExtensionProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(ClassWithRequiredMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				async Task Act()
				{
					await That(subject).AreNotExtensionProperties();
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
			public async Task WhenAllPropertiesAreNonExtensionProperties_ShouldSucceed()
			{
				IAsyncEnumerable<PropertyInfo?> subject = typeof(ClassWithRequiredMembers)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).AreNotExtensionProperties();
				}

				await That(Act).DoesNotThrow();
			}
		}
#endif

#if NET10_0_OR_GREATER
		public sealed class NewSyntaxTests
		{
			[Fact]
			public async Task WhenAllAreExtensionProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(StaticClassWithNewExtensionProperties).GetExtensionProperty("IsBlankText"),
					typeof(StaticClassWithNewExtensionProperties).GetExtensionProperty("DefaultValue"),
				];

				async Task Act()
				{
					await That(subject).AreNotExtensionProperties();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not extension properties,
					             but it contained extension properties [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenContainingARegularProperty_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(StaticClassWithNewExtensionProperties).GetProperty(
						nameof(StaticClassWithNewExtensionProperties.RegularProperty))!,
				];

				async Task Act()
				{
					await That(subject).AreNotExtensionProperties();
				}

				await That(Act).DoesNotThrow();
			}
		}
#endif
	}
}
