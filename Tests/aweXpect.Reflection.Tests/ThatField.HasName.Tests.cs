using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatField
{
	public sealed class HasName
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenExpectedValueIsOnlySubstring_ShouldFail()
			{
				FieldInfo? subject =
					typeof(ClassWithFields).GetField(nameof(ClassWithFields.PublicField));

				async Task Act()
					=> await That(subject).HasName("Field");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has name equal to "Field",
					             but it was "PublicField" which differs at index 0:
					                ↓ (actual)
					               "PublicField"
					               "Field"
					                ↑ (expected)
					             """);
			}

			[Fact]
			public async Task WhenFieldInfoHasExpectedPrefix_ShouldSucceed()
			{
				FieldInfo? subject =
					typeof(ClassWithFields).GetField(nameof(ClassWithFields.PublicField));

				async Task Act()
					=> await That(subject).HasName("Public").AsPrefix();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldInfoHasMatchingName_ShouldSucceed()
			{
				FieldInfo? subject =
					typeof(ClassWithFields).GetField(nameof(ClassWithFields.PublicField));

				async Task Act()
					=> await That(subject).HasName("PublicField");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldInfoIsNull_ShouldFail()
			{
				FieldInfo? subject = null;

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
			public async Task WhenFieldInfoMatchesIgnoringCase_ShouldSucceed()
			{
				FieldInfo? subject =
					typeof(ClassWithFields).GetField(nameof(ClassWithFields.PublicField));

				async Task Act()
					=> await That(subject).HasName("pUBLICfIELD").IgnoringCase();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
