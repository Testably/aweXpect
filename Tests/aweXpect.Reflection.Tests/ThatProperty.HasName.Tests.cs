using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class HasName
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenExpectedValueIsOnlySubstring_ShouldFail()
			{
				PropertyInfo? subject =
					typeof(ClassWithProperties).GetProperty(nameof(ClassWithProperties.PublicProperty));

				async Task Act()
					=> await That(subject).HasName("Property");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has name equal to "Property",
					             but it was "PublicProperty" which differs at index 1:
					                 ↓ (actual)
					               "PublicProperty"
					               "Property"
					                 ↑ (expected)
					             """);
			}

			[Fact]
			public async Task WhenPropertyInfoHasExpectedPrefix_ShouldSucceed()
			{
				PropertyInfo? subject =
					typeof(ClassWithProperties).GetProperty(nameof(ClassWithProperties.PublicProperty));

				async Task Act()
					=> await That(subject).HasName("Public").AsPrefix();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyInfoHasMatchingName_ShouldSucceed()
			{
				PropertyInfo? subject =
					typeof(ClassWithProperties).GetProperty(nameof(ClassWithProperties.PublicProperty));

				async Task Act()
					=> await That(subject).HasName("PublicProperty");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyInfoIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
					=> await That(subject).HasName("foo");

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             has name equal to "foo",
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenPropertyInfoMatchesIgnoringCase_ShouldSucceed()
			{
				PropertyInfo? subject =
					typeof(ClassWithProperties).GetProperty(nameof(ClassWithProperties.PublicProperty));

				async Task Act()
					=> await That(subject).HasName("pUBLICpROPERTY").IgnoringCase();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenPropertyInfoDoesNotHaveName_ShouldSucceed()
			{
				PropertyInfo? subject =
					typeof(ClassWithProperties).GetProperty(nameof(ClassWithProperties.PublicProperty));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasName("NonExistentProperty"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyInfoHasName_ShouldFail()
			{
				PropertyInfo? subject =
					typeof(ClassWithProperties).GetProperty(nameof(ClassWithProperties.PublicProperty));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasName("PublicProperty"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has name not equal to "PublicProperty",
					             but it was "PublicProperty"
					             """);
			}
		}
	}
}
