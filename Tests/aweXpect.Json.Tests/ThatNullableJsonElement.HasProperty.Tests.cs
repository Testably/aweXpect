using System.Text.Json;

namespace aweXpect.Json.Tests;

public sealed partial class ThatNullableJsonElement
{
	public sealed class HasProperty
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenExpectationsAreNotMet_ShouldFail()
			{
				JsonElement? subject = FromString("{\"title\": \"Yesterday\"}");

				async Task Act()
					=> await That(subject).HasProperty("title", p => p.IsString(s => s.IsEqualTo("Let It Be")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has property "title" which is a string which is equal to "Let It Be",
					             but $.title was "Yesterday", which differs at index 0:
					                ↓ (actual)
					               "Yesterday"
					               "Let It Be"
					                ↑ (expected)
					             """);
			}

			[Test]
			public async Task WhenExpectationsAreNull_ShouldThrowArgumentNullException()
			{
				JsonElement? subject = FromString("{}");

				async Task Act()
					=> await That(subject).HasProperty("foo", null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenNameIsNull_ShouldThrowArgumentNullException()
			{
				JsonElement? subject = FromString("{}");

				async Task Act()
					=> await That(subject).HasProperty(null!, p => p.IsNumber());

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'name' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenPropertyDoesNotExist_ShouldFail()
			{
				JsonElement? subject = FromString("{}");

				async Task Act()
					=> await That(subject).HasProperty("foo");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has property "foo",
					             but property $.foo did not exist

					             Actual:
					             {}
					             """);
			}

			[Test]
			public async Task WhenPropertyExists_ShouldSucceed()
			{
				JsonElement? subject = FromString("{\"foo\": 1}");

				async Task Act()
					=> await That(subject).HasProperty("foo").And.HasProperty("foo", p => p.IsNumber<int>());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyIsNull_WhenTreatingNullAsAbsent_ShouldFail()
			{
				JsonElement? subject = FromString("{\"foo\": null}");

				async Task Act()
					=> await That(subject).HasProperty("foo", p => p.IsNull()).TreatingNullAsAbsent();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has property "foo" which is null,
					             but property $.foo did not exist

					             Actual:
					             {
					               "foo": null
					             }

					             JSON options:
					              - treat null as absent
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).HasProperty("foo", p => p.IsNumber());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has property "foo" which is a number,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_WithoutExpectations_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).HasProperty("foo");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has property "foo",
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasProperty("foo"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have property "foo",
					             but it was <null>
					             """);
			}
		}
	}

	public sealed class DoesNotHaveProperty
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenNameIsNull_ShouldThrowArgumentNullException()
			{
				JsonElement? subject = FromString("{}");

				async Task Act()
					=> await That(subject).DoesNotHaveProperty(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'name' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenPropertyDoesNotExist_ShouldSucceed()
			{
				JsonElement? subject = FromString("{\"bar\": 1}");

				async Task Act()
					=> await That(subject).DoesNotHaveProperty("foo");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyExists_ShouldFail()
			{
				JsonElement? subject = FromString("{\"foo\": {\"bar\": 1}}");

				async Task Act()
					=> await That(subject).DoesNotHaveProperty("foo");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have property "foo",
					             but it had property "foo" with {"bar":1}

					             Actual:
					             {
					               "foo": {
					                 "bar": 1
					               }
					             }
					             """);
			}

			[Test]
			public async Task WhenPropertyIsNull_WhenTreatingNullAsAbsent_ShouldSucceed()
			{
				JsonElement? subject = FromString("{\"foo\": null}");

				async Task Act()
					=> await That(subject).DoesNotHaveProperty("foo").TreatingNullAsAbsent();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveProperty("foo");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have property "foo",
					             but it was <null>
					             """);
			}
		}
	}
}
