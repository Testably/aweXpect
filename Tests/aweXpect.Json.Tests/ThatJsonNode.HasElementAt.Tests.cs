using System.Text.Json;
using System.Text.Json.Nodes;
using aweXpect.Core;

namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonNode
{
	public sealed class HasElementAt
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenElementDoesNotSatisfyTheExpectations_ShouldFail()
			{
				JsonArray subject = new(1, "two");

				async Task Act()
					=> await That(subject).HasElementAt(1, e => e.IsNumber());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has element at index 1 which is a number,
					             but $[1] was a string
					             """);
			}

			[Test]
			public async Task WhenElementExists_ShouldPassTheNodeItself()
			{
				JsonObject track = new() { ["title"] = "Come Together", };
				JsonArray subject = new(track);

				async Task Act()
					=> await That(subject).HasElementAt(0, t => t.IsSameAs(track));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectationsAreNull_ShouldThrowArgumentNullException()
			{
				JsonArray subject = new();

				async Task Act()
					=> await That(subject).HasElementAt(0, null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenIndexIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				JsonArray subject = new();

				async Task Act()
					=> await That(subject).HasElementAt(-1, e => e.IsNumber());

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("The index must not be negative.").AsPrefix();
			}

			[Test]
			public async Task WhenIndexIsOutOfRange_ShouldFail()
			{
				JsonArray subject = new(1);

				async Task Act()
					=> await That(subject).HasElementAt(1, e => e.IsNumber());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has element at index 1 which is a number,
					             but index $[1] did not exist

					             Actual:
					             [
					               1
					             ]
					             """);
			}

			[Test]
			public async Task WhenNodeIsNoArray_ShouldFail()
			{
				JsonObject subject = new();

				async Task Act()
					=> await That(subject).HasElementAt(0, e => e.IsNumber());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has element at index 0 which is a number,
					             but it was an object

					             Actual:
					             {}
					             """);
			}

			[Test]
			public async Task WhenValueHoldsAnArray_ShouldPassTheElement()
			{
				JsonNode? subject = JsonValue.Create(new[] { "a", "b", });

				async Task Act()
					=> await That(subject).HasElementAt(1, e => e.IsString(s => s.IsEqualTo("b")));

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WithoutExpectationsTests
		{
			[Test]
			public async Task WhenElementExists_ShouldSucceed()
			{
				JsonArray subject = new(1, null);

				async Task Act()
					=> await That(subject).HasElementAt(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenIndexIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				JsonNode? subject = FromString("[]");

				async Task Act()
					=> await That(subject).HasElementAt(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("The index must not be negative.").AsPrefix();
			}

			[Test]
			public async Task WhenIndexIsOutOfRange_ShouldFail()
			{
				JsonArray subject = new(1);

				async Task Act()
					=> await That(subject).HasElementAt(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has element at index 1,
					             but index $[1] did not exist

					             Actual:
					             [
					               1
					             ]
					             """);
			}
		}
	}
}
