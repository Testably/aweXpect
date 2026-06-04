using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;
#if NET10_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsNotAnExtensionProperty
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenPropertyIsNotAnExtensionProperty_ShouldSucceed()
			{
				PropertyInfo subject =
					typeof(ClassWithRequiredMembers).GetProperty(nameof(ClassWithRequiredMembers.OptionalProperty))!;

				async Task Act()
				{
					await That(subject).IsNotAnExtensionProperty();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotAnExtensionProperty();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not an extension property,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenPropertyIsNotAnExtensionProperty_ShouldFail()
			{
				PropertyInfo subject =
					typeof(ClassWithRequiredMembers).GetProperty(nameof(ClassWithRequiredMembers.OptionalProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAnExtensionProperty());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is an extension property,
					              but it was not an extension property {Formatter.Format(subject)}
					              """);
			}
		}

#if NET10_0_OR_GREATER
		public sealed class NewSyntaxTests
		{
			[Fact]
			public async Task WhenPropertyIsAnInstanceExtensionProperty_ShouldFail()
			{
				PropertyInfo subject =
					typeof(StaticClassWithNewExtensionProperties).GetExtensionProperty("IsBlankText");

				async Task Act()
				{
					await That(subject).IsNotAnExtensionProperty();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is not an extension property,
					              but it was an extension property {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenPropertyIsAStaticExtensionProperty_ShouldFail()
			{
				PropertyInfo subject =
					typeof(StaticClassWithNewExtensionProperties).GetExtensionProperty("DefaultValue");

				async Task Act()
				{
					await That(subject).IsNotAnExtensionProperty();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is not an extension property,
					              but it was an extension property {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenPropertyIsARegularStaticPropertyInExtensionClass_ShouldSucceed()
			{
				PropertyInfo subject =
					typeof(StaticClassWithNewExtensionProperties).GetProperty(
						nameof(StaticClassWithNewExtensionProperties.RegularProperty))!;

				async Task Act()
				{
					await That(subject).IsNotAnExtensionProperty();
				}

				await That(Act).DoesNotThrow();
			}
		}
#endif
	}
}
