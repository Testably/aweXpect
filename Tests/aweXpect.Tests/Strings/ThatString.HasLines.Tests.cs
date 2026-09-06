namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public class HasLines
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenActualIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).HasLines(lines => lines.HasCount(0));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has lines which has exactly 0 items,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenLineIsMissing_ShouldFail()
			{
				string subject = "Starting up\nConnected to database\nReady";

				async Task Act()
					=> await That(subject).HasLines(lines => lines.Contains("Error"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has lines which contains "Error" at least once,
					             but it did not contain it

					             Collection:
					             [
					               "Starting up",
					               "Connected to database",
					               "Ready"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenLinesContainTheExpectedLine_ShouldSucceed()
			{
				string subject = "Starting up\nConnected to database\nReady";

				async Task Act()
					=> await That(subject).HasLines(lines => lines.Contains("Connected to database"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenLinesDoNotContainTheUnexpectedLine_ShouldSucceed()
			{
				string subject = "Starting up\nConnected to database\nReady";

				async Task Act()
					=> await That(subject).HasLines(lines => lines.DoesNotContain("Error"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenLinesSatisfyTheCondition_ShouldSucceed()
			{
				string subject = "one\ntwo\nthree";

				async Task Act()
					=> await That(subject).HasLines(lines => lines.All().Satisfy(l => l!.Length < 6));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubstringMatchesButLineDoesNot_ShouldFail()
			{
				string subject = "Ready steady go";

				async Task Act()
					=> await That(subject).HasLines(lines => lines.Contains("Ready"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has lines which contains "Ready" at least once,
					             but it did not contain it

					             Collection:
					             [
					               "Ready steady go"
					             ]
					             """);
			}

			[Theory]
			[InlineData("", 0)]
			[InlineData("a", 1)]
			[InlineData("a\nb", 2)]
			[InlineData("a\nb\n", 2)]
			[InlineData("a\nb\n\n", 3)]
			[InlineData("\n", 1)]
			[InlineData("one\r\ntwo\nthree\rfour", 4)]
			public async Task WhenUsingDifferentNewlineStyles_ShouldSplitConsistently(string subject, int lineCount)
			{
				async Task Act()
					=> await That(subject).HasLines(lines => lines.HasCount(lineCount));

				await That(Act).DoesNotThrow();
			}

			[Fact]
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
			[Fact]
			public async Task WhenLinesDoNotSatisfyTheExpectations_ShouldSucceed()
			{
				string subject = "Starting up\nReady";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it
						.HasLines(lines => lines.Contains("Error")));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenLinesSatisfyTheExpectations_ShouldFail()
			{
				string subject = "Starting up\nReady";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it
						.HasLines(lines => lines.Contains("Ready")));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have lines which contains "Ready" at least once,
					             but it had
					             """);
			}
		}
	}
}
