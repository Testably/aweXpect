using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatField
{
	public sealed class HasName
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenExpectedValueIsOnlySubstring_ShouldFail()
			{
				FieldInfo? subject =
					typeof(ClassWithFields).GetField(nameof(ClassWithFields.PublicField));

				async Task Act()
				{
					await That(subject).HasName("Field");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has name equal to "Field",
					             but it had name "PublicField", which differs at index 0:
					                ↓ (actual)
					               "PublicField"
					               "Field"
					                ↑ (expected)
					             """);
			}

			[Test]
			public async Task WhenFieldInfoHasExpectedPrefix_ShouldSucceed()
			{
				FieldInfo? subject =
					typeof(ClassWithFields).GetField(nameof(ClassWithFields.PublicField));

				async Task Act()
				{
					await That(subject).HasName("Public").AsPrefix();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldInfoHasMatchingName_ShouldSucceed()
			{
				FieldInfo? subject =
					typeof(ClassWithFields).GetField(nameof(ClassWithFields.PublicField));

				async Task Act()
				{
					await That(subject).HasName("PublicField");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldInfoIsNull_ShouldFail()
			{
				FieldInfo? subject = null;

				async Task Act()
				{
					await That(subject).HasName("foo");
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             has name equal to "foo",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenFieldInfoMatchesIgnoringCase_ShouldSucceed()
			{
				FieldInfo? subject =
					typeof(ClassWithFields).GetField(nameof(ClassWithFields.PublicField));

				async Task Act()
				{
					await That(subject).HasName("pUBLICfIELD").IgnoringCase();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenFieldInfoDoesNotHaveName_ShouldSucceed()
			{
				FieldInfo? subject =
					typeof(ClassWithFields).GetField(nameof(ClassWithFields.PublicField));

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.HasName("NonExistentField"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldInfoHasName_ShouldFail()
			{
				FieldInfo? subject =
					typeof(ClassWithFields).GetField(nameof(ClassWithFields.PublicField));

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.HasName("PublicField"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has name not equal to "PublicField",
					             but it had name "PublicField"
					             """);
			}
		}
	}
}
