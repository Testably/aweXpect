namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public class HasLines
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenActualIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).HasLines(lines => lines.HasCount(0));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has lines that have exactly 0 items,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenExpectationsAreEmpty_ShouldThrowArgumentException()
			{
				string subject = "Starting up\nReady";

				async Task Act()
					=> await That(subject).HasLines(_ => { });

				await That(Act).Throws<ArgumentException>()
					.WithMessage("You must add at least one expectation in the expectations callback.*").AsWildcard()
					.And.WithParamName("expectations");
			}

			[Test]
			public async Task WhenExpectationsIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "Starting up\nReady";

				async Task Act()
					=> await That(subject).HasLines(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectations").And
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenLineIsIndented_ShouldSucceedWhenIndentationIsIgnored()
			{
				string subject = "Starting up\n    Connected to database\n        Ready";

				async Task Act()
					=> await That(subject).HasLines(lines => lines.Contains("Ready").IgnoringIndentation());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenLineIsMissing_ShouldFail()
			{
				string subject = "Starting up\nConnected to database\nReady";

				async Task Act()
					=> await That(subject).HasLines(lines => lines.Contains("Error"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has lines that contain "Error" at least once,
					             but it did not contain it

					             Collection:
					             [
					               "Starting up",
					               "Connected to database",
					               "Ready"
					             ]
					             """);
			}

			[Test]
			public async Task WhenLinesContainTheExpectedLine_ShouldSucceed()
			{
				string subject = "Starting up\nConnected to database\nReady";

				async Task Act()
					=> await That(subject).HasLines(lines => lines.Contains("Connected to database"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenLinesDoNotContainTheUnexpectedLine_ShouldSucceed()
			{
				string subject = "Starting up\nConnected to database\nReady";

				async Task Act()
					=> await That(subject).HasLines(lines => lines.DoesNotContain("Error"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenLinesSatisfyTheCondition_ShouldSucceed()
			{
				string subject = "one\ntwo\nthree";

				async Task Act()
					=> await That(subject).HasLines(lines => lines.All().Satisfy(l => l!.Length < 6));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNestedInAMemberExpectationOnAnItem_ShouldFail()
			{
				string subject = "a\nb";

				async Task Act()
					=> await That(subject).HasLines(lines => lines.HasItemThat(line => line
						.Whose(s => s.Trim(), t => t.HasLines(l => l.HasCount(3)))));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has lines that have an item that has Trim() that has lines that have exactly 3 items,
					             but it had no matching item

					             Collection:
					             [
					               "a",
					               "b"
					             ]
					             """);
			}

			[Test]
			public async Task WhenNestedInAnItemExpectationOnTheLines_ShouldFail()
			{
				string subject = "a\nb";

				async Task Act()
					=> await That(subject).HasLines(lines => lines.HasItemThat(line => line.HasLines(l => l.HasCount(3))));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has lines that have an item that has lines that have exactly 3 items,
					             but it had no matching item

					             Collection:
					             [
					               "a",
					               "b"
					             ]
					             """);
			}

			[Test]
			public async Task WhenSubjectIsEmpty_ShouldHaveNoLines()
			{
				string subject = "";

				async Task Act()
					=> await That(subject).HasLines(lines => lines.IsEmpty());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubstringMatchesButLineDoesNot_ShouldFail()
			{
				string subject = "Ready steady go";

				async Task Act()
					=> await That(subject).HasLines(lines => lines.Contains("Ready"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has lines that contain "Ready" at least once,
					             but it did not contain it

					             Collection:
					             [
					               "Ready steady go"
					             ]
					             """);
			}

			[Test]
			[Arguments("", 0)]
			[Arguments("a", 1)]
			[Arguments("a\nb", 2)]
			[Arguments("a\nb\n", 2)]
			[Arguments("a\nb\n\n", 3)]
			[Arguments("\n", 1)]
			[Arguments("a\r\n", 1)]
			[Arguments("a\r\nb\r\n", 2)]
			[Arguments("\r\n", 1)]
			[Arguments("\r", 1)]
			[Arguments("one\r\ntwo\nthree\rfour", 4)]
			public async Task WhenUsingDifferentNewlineStyles_ShouldSplitConsistently(string subject, int lineCount)
			{
				async Task Act()
					=> await That(subject).HasLines(lines => lines.HasCount(lineCount));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenUsingMixedNewlineStyles_ShouldReturnTheLines()
			{
				string subject = "one\r\ntwo\nthree\rfour";

				async Task Act()
					=> await That(subject).HasLines(lines => lines.IsEqualTo(["one", "two", "three", "four",]));

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenActualIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it
						.HasLines(lines => lines.Contains("Ready")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have lines that contain "Ready" at least once,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenLinesDoNotSatisfyTheExpectations_ShouldSucceed()
			{
				string subject = "Starting up\nReady";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it
						.HasLines(lines => lines.Contains("Error")));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenLinesSatisfyTheExpectations_ShouldFail()
			{
				string subject = "Starting up\nReady";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it
						.HasLines(lines => lines.Contains("Ready")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have lines that contain "Ready" at least once,
					             but it had lines [
					               "Starting up",
					               "Ready"
					             ]
					             """);
			}

			[Test]
			public async Task WhenNestedInAnItemExpectationOnTheLines_ShouldFail()
			{
				string subject = "a\nb";

				async Task Act()
					=> await That(subject).HasLines(lines => lines.HasItemThat(line => line
						.DoesNotComplyWith(it => it.HasLines(l => l.HasCount(1)))));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has lines that have an item that does not have lines that have exactly one item,
					             but it had no matching item

					             Collection:
					             [
					               "a",
					               "b"
					             ]
					             """);
			}

			[Test]
			public async Task WhenNestedLinesSatisfyTheExpectations_ShouldFail()
			{
				string subject = "a\nb";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it
						.HasLines(lines => lines.HasItemThat(line => line.HasLines(l => l.HasCount(1)))));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have lines that have an item that has lines that have exactly one item,
					             but it had lines [
					               "a",
					               "b"
					             ]
					             """);
			}
		}
	}
}
