using System.Text.Json;

namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonElement
{
	public sealed class HasElementAt
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenElementDoesNotSatisfyTheExpectations_ShouldFailWithThePathOfTheElement()
			{
				JsonElement subject = FromString("[{\"title\": \"Yesterday\"}]");

				async Task Act()
					=> await That(subject).HasElementAt(0, t => t
						.HasProperty("title", p => p.IsString(s => s.StartsWith("Let"))));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has element at index 0 which has property "title" which is a string which starts with "Let",
					             but $[0].title was "Yesterday", which differs at index 0:
					                ↓ (actual)
					               "Yesterday"
					               "Let"
					                ↑ (expected prefix)
					             """);
			}

			[Test]
			public async Task WhenElementSatisfiesTheExpectations_ShouldSucceed()
			{
				JsonElement subject = FromString("[1, \"Let It Be\"]");

				async Task Act()
					=> await That(subject).HasElementAt(1, e => e.IsString(s => s.StartsWith("Let")));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectationsAreNull_ShouldThrowArgumentNullException()
			{
				JsonElement subject = FromString("[]");

				async Task Act()
					=> await That(subject).HasElementAt(0, null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenIndexIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				JsonElement subject = FromString("[]");

				async Task Act()
					=> await That(subject).HasElementAt(-1, e => e.IsNumber());

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("The 'index' must not be negative.").AsPrefix();
			}

			[Test]
			public async Task WhenIndexIsOutOfRange_ShouldNotVerifyTheExpectations()
			{
				JsonElement subject = FromString("[1, 2]");

				async Task Act()
					=> await That(subject).HasElementAt(2, e => e.IsNumber());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has element at index 2 which is a number,
					             but index $[2] did not exist
					             """);
			}

			[Test]
			[Arguments("{}", "an object")]
			[Arguments("1", "a number")]
			[Arguments("null", "null")]
			public async Task WhenJsonIsNoArray_ShouldFail(string json, string kindString)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).HasElementAt(0, e => e.IsNumber());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has element at index 0 which is a number,
					              but it was {kindString}
					              """);
			}

			[Test]
			public async Task WhenNestedInHasProperty_ShouldContinueThePath()
			{
				JsonElement subject = FromString("{\"tracks\": [\"Come Together\", 2]}");

				async Task Act()
					=> await That(subject).HasProperty("tracks", t => t.HasElementAt(1, e => e.IsString()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has property "tracks" which has element at index 1 which is a string,
					             but $.tracks[1] was a number
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenElementExists_ShouldFail()
			{
				JsonElement subject = FromString("[1, 2]");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasElementAt(1, e => e.IsNumber()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have element at index 1 or $[1] is not a number,
					             but $[1] was a number
					             """);
			}

			[Test]
			public async Task WhenIndexIsOutOfRange_ShouldSucceed()
			{
				JsonElement subject = FromString("[1, 2]");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasElementAt(2, e => e.IsNumber()));

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WithoutExpectationsTests
		{
			[Test]
			public async Task ShouldReturnTheSubject()
			{
				JsonElement subject = FromString("[1, 2]");

				JsonElement result = await That(subject).HasElementAt(1);

				await That(result.GetRawText()).IsEqualTo(subject.GetRawText());
			}

			[Test]
			public async Task WhenElementExists_ShouldSucceed()
			{
				JsonElement subject = FromString("[1, null]");

				async Task Act()
					=> await That(subject).HasElementAt(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenIndexIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				JsonElement subject = FromString("[]");

				async Task Act()
					=> await That(subject).HasElementAt(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("The 'index' must not be negative.").AsPrefix();
			}

			[Test]
			public async Task WhenIndexIsOutOfRange_ShouldFail()
			{
				JsonElement subject = FromString("[1, 2]");

				async Task Act()
					=> await That(subject).HasElementAt(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has element at index 2,
					             but index $[2] did not exist
					             """);
			}

			[Test]
			public async Task WhenJsonIsNoArray_ShouldFail()
			{
				JsonElement subject = FromString("{}");

				async Task Act()
					=> await That(subject).HasElementAt(0);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has element at index 0,
					             but it was an object
					             """);
			}

			[Test]
			public async Task WhenNegatedAndIndexIsOutOfRange_ShouldSucceed()
			{
				JsonElement subject = FromString("[1, 2]");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasElementAt(2));

				await That(Act).DoesNotThrow();
			}
		}
	}
}
