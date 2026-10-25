using System.Text.Json;

namespace aweXpect.Json.Tests;

public sealed partial class ThatNullableJsonElement
{
	public sealed class HasElementAt
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenElementSatisfiesTheExpectations_ShouldSucceed()
			{
				JsonElement? subject = FromString("[\"a\", \"b\"]");

				async Task Act()
					=> await That(subject).HasElementAt(1, e => e.IsString(s => s.IsEqualTo("b")));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectationsAreNull_ShouldThrowArgumentNullException()
			{
				JsonElement? subject = FromString("[]");

				async Task Act()
					=> await That(subject).HasElementAt(0, null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenIndexIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				JsonElement? subject = FromString("[]");

				async Task Act()
					=> await That(subject).HasElementAt(-1, e => e.IsNumber());

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("The 'index' must not be negative.").AsPrefix();
			}

			[Test]
			public async Task WhenIndexIsOutOfRange_ShouldFail()
			{
				JsonElement? subject = FromString("[]");

				async Task Act()
					=> await That(subject).HasElementAt(0, e => e.IsNumber());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has element at index 0 which is a number,
					             but index $[0] did not exist
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).HasElementAt(0, e => e.IsNumber());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has element at index 0 which is a number,
					             but it was <null>
					             """);
			}
		}

		public sealed class WithoutExpectationsTests
		{
			[Test]
			public async Task WhenElementExists_ShouldSucceed()
			{
				JsonElement? subject = FromString("[1]");

				async Task Act()
					=> await That(subject).HasElementAt(0);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenIndexIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				JsonElement? subject = FromString("[]");

				async Task Act()
					=> await That(subject).HasElementAt(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("The 'index' must not be negative.").AsPrefix();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).HasElementAt(0);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has element at index 0,
					             but it was <null>
					             """);
			}
		}
	}
}
