using System.Text.Json;

namespace aweXpect.Json.Tests;

public sealed partial class ThatNullableJsonElement
{
	public sealed class HasValueAt
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenExpectationsAreNull_ShouldThrowArgumentNullException()
			{
				JsonElement? subject = FromString("{}");

				async Task Act()
					=> await That(subject).HasValueAt("foo", null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenIntermediatePropertyDoesNotExist_ShouldFail()
			{
				JsonElement? subject = FromString("{\"foo\": {}}");

				async Task Act()
					=> await That(subject).HasValueAt("foo.bar.baz", v => v.IsNumber());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has value at "foo.bar.baz" which is a number,
					             but property $.foo.bar did not exist
					             """);
			}

			[Test]
			public async Task WhenIntermediatePropertyIsNull_WhenTreatingNullAsAbsent_ShouldFail()
			{
				JsonElement? subject = FromString("{\"foo\": null}");

				async Task Act()
					=> await That(subject).HasValueAt("foo.bar", v => v.IsNumber()).TreatingNullAsAbsent();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has value at "foo.bar" which is a number,
					             but property $.foo did not exist

					             JSON options:
					              - treat null as absent
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).HasValueAt("foo.bar", v => v.IsNumber());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has value at "foo.bar" which is a number,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenValueSatisfiesTheExpectations_ShouldSucceed()
			{
				JsonElement? subject = FromString("{\"foo\": [{\"bar\": 2}]}");

				async Task Act()
					=> await That(subject).HasValueAt("foo[0].bar", v => v.IsNumber<int>(n => n.IsEqualTo(2)));

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WithoutExpectationsTests
		{
			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).HasValueAt("foo.bar");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has value at "foo.bar",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenValueDoesNotExist_ShouldFail()
			{
				JsonElement? subject = FromString("{\"foo\": {}}");

				async Task Act()
					=> await That(subject).HasValueAt("foo.bar");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has value at "foo.bar",
					             but property $.foo.bar did not exist
					             """);
			}

			[Test]
			public async Task WhenValueExists_ShouldSucceed()
			{
				JsonElement? subject = FromString("{\"foo\": [{\"bar\": 2}]}");

				async Task Act()
					=> await That(subject).HasValueAt("foo[0].bar");

				await That(Act).DoesNotThrow();
			}
		}
	}

	public sealed class DoesNotHaveValueAt
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveValueAt("foo.bar");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have value at "foo.bar",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenValueDoesNotExist_ShouldSucceed()
			{
				JsonElement? subject = FromString("{\"foo\": {}}");

				async Task Act()
					=> await That(subject).DoesNotHaveValueAt("foo.bar");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenValueExists_ShouldFail()
			{
				JsonElement? subject = FromString("{\"foo\": {\"bar\": 2}}");

				async Task Act()
					=> await That(subject).DoesNotHaveValueAt("foo.bar");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have value at "foo.bar",
					             but it had value at "foo.bar" with 2
					             """);
			}

			[Test]
			public async Task WhenValueIsNull_WhenTreatingNullAsAbsent_ShouldSucceed()
			{
				JsonElement? subject = FromString("{\"foo\": {\"bar\": null}}");

				async Task Act()
					=> await That(subject).DoesNotHaveValueAt("foo.bar").TreatingNullAsAbsent();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
