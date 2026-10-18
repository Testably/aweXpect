using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
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
			[Test]
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

			[Test]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotAnExtensionProperty();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not an extension property,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenPropertyIsNotAnExtensionProperty_ShouldFail()
			{
				PropertyInfo subject =
					typeof(ClassWithRequiredMembers).GetProperty(nameof(ClassWithRequiredMembers.OptionalProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAnExtensionProperty());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					              Expected that subject
					              is an extension property,
					              but it was not an extension property public string ClassWithRequiredMembers.OptionalProperty { get; set; }
					              """);
			}
		}

#if NET10_0_OR_GREATER
		public sealed class NewSyntaxTests
		{
			[Test]
			public async Task WhenPropertyIsAnInstanceExtensionProperty_ShouldFail()
			{
				PropertyInfo subject =
					typeof(StaticClassWithNewExtensionProperties).GetExtensionProperty("IsBlankText");

				async Task Act()
				{
					await That(subject).IsNotAnExtensionProperty();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is not an extension property,
					              but it was an extension property public bool StaticClassWithNewExtensionProperties.<G>$34505F560D9EACF86A87F3ED1F85E448.IsBlankText { get; }
					              """);
			}

			[Test]
			public async Task WhenPropertyIsAStaticExtensionProperty_ShouldFail()
			{
				PropertyInfo subject =
					typeof(StaticClassWithNewExtensionProperties).GetExtensionProperty("DefaultValue");

				async Task Act()
				{
					await That(subject).IsNotAnExtensionProperty();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is not an extension property,
					              but it was an extension property public string StaticClassWithNewExtensionProperties.<G>$34505F560D9EACF86A87F3ED1F85E448.DefaultValue { get; }
					              """);
			}

			[Test]
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
