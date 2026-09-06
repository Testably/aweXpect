namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public partial class Contains
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenActualIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).Contains("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains "foo" at least once,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				string subject = "some text";
				string expected = "";

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The 'expected' string cannot be empty.").AsPrefix().And
					.WithParamName("expected");
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				string subject = "some text";
				string? expected = null;

				async Task Act()
					=> await That(subject).Contains(expected!);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains <null> at least once,
					             but "some text" cannot be validated against <null>
					             """);
			}

			[Fact]
			public async Task WhenExpectedStringIsContained_ShouldSucceed()
			{
				string subject = "some text";
				string expected = "me";

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectedStringIsNotContained_ShouldFail()
			{
				string subject = "some text";
				string expected = "not";

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains "not" at least once,
					             but it did not contain "not" in "some text"
					             
					             Actual:
					             some text
					             
					             Expected:
					             not
					             """);
			}
		}

		public sealed class IgnoringCaseTests
		{
			[Fact]
			public async Task ShouldIncludeSettingInExpectationText()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string expected = "in";

				async Task Act()
					=> await That(subject).Contains(expected).AtLeast(7).IgnoringCase();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains "in" at least 7 times ignoring case,
					             but it contained "in" 5 times in "In this text in between the word an investigator should find the word 'IN' multiple times."
					             
					             Actual:
					             In this text in between the word an investigator should find the word 'IN' multiple times.
					             
					             Expected:
					             in
					             """);
			}

			[Fact]
			public async Task
				WhenExpectedStringOccursEnoughTimesCaseInsensitive_ShouldSucceed()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string expected = "in";

				async Task Act()
					=> await That(subject).Contains(expected).AtLeast(5).IgnoringCase();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class IgnoringIndentationTests
		{
			[Fact]
			public async Task ShouldIncludeSettingInExpectationText()
			{
				string subject = "foo";
				string expected = "bar";

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringIndentation();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains "bar" at least once ignoring indentation,
					             but it did not contain "bar" in "foo"

					             Actual:
					             foo

					             Expected:
					             bar
					             """);
			}

			[Fact]
			public async Task WhenCombinedWithIgnoringCase_ShouldIgnoreBoth()
			{
				string subject = "foo\n    BAR";
				string expected = "foo\nbar";

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringIndentation().IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSnippetIsIndentedDifferently_ShouldSucceed()
			{
				string subject = """
				                 public class Foo
				                 {
				                     public int Bar
				                     {
				                         get;
				                     }
				                 }
				                 """;
				string expected = """
				                  public int Bar
				                  {
				                      get;
				                  }
				                  """;

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringIndentation();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSnippetOccursWithDifferentIndentations_ShouldCountAllOccurrences()
			{
				string subject = "  a\n  b\nx\na\nb";
				string expected = "a\nb";

				async Task Act()
					=> await That(subject).Contains(expected).Twice().IgnoringIndentation();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectUsesADifferentNewlineStyle_ShouldSucceed()
			{
				string subject = "foo\r\n    bar";
				string expected = "foo\nbar";

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringIndentation();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class IgnoringNewlineStyleTests
		{
			[Fact]
			public async Task WhenSubjectUsesADifferentNewlineStyle_ShouldFindAllOccurrences()
			{
				string subject = "x\r\na\r\nb\r\ny\r\na\r\nb";
				string expected = "a\nb";

				async Task Act()
					=> await That(subject).Contains(expected).Twice().IgnoringNewlineStyle();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class UsingTests
		{
			[Fact]
			public async Task
				WhenExpectedStringOccursEnoughTimesForTheComparer_ShouldSucceed()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string expected = "in";

				async Task Act()
					=> await That(subject).Contains(expected).Exactly(4)
						.Using(new IgnoreCaseForVocalsComparer());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task
				WhenExpectedStringOccursIncorrectTimesForTheComparer_ShouldIncludeComparerInMessage()
			{
				string subject =
					"In this text in between the word an investigator should find the word 'IN' multiple times.";
				string expected = "in";

				async Task Act()
					=> await That(subject).Contains(expected).Exactly(5)
						.Using(new IgnoreCaseForVocalsComparer());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains "in" exactly 5 times using IgnoreCaseForVocalsComparer,
					             but it contained "in" 4 times in "In this text in between the word an investigator should find the word 'IN' multiple times."
					             
					             Actual:
					             In this text in between the word an investigator should find the word 'IN' multiple times.
					             
					             Expected:
					             in
					             """);
			}
		}
	}
}
