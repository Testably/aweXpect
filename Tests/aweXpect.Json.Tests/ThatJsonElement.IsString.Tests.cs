using System.Linq;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Json.Results;

namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonElement
{
	public sealed class IsString
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("\"\"")]
			[Arguments("\"foo\"")]
			public async Task WhenJsonIsAString_ShouldSucceed(string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsString();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("{}", "an object")]
			[Arguments("[]", "an array")]
			[Arguments("2", "a number")]
			[Arguments("true", "true")]
			[Arguments("false", "false")]
			[Arguments("null", "null")]
			public async Task WhenJsonIsNoString_ShouldFail(string json, string kindString)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsString();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is a string,
					              but it was {kindString}
					              """);
			}

			[Test]
			public async Task WhenItemsArePlural_ShouldUsePluralForm()
			{
				JsonElement[] items = [FromString("\"foo\""), FromString("2"),];

				async Task Act()
					=> await That(new { Items = items, })
						.Whose(x => x.Items, i => i.All().ComplyWith(item => item.IsString()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that new { Items = items, }
					             whose Items are a string for all items,
					             but only 1 of 2 were
					             *
					             """).AsWildcard()
					.Because("the items are the plural subject of the expectation");
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenJsonIsAString_ShouldFail()
			{
				JsonElement subject = FromString("\"foo\"");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsString());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not a string,
					             but it was a string
					             """);
			}

			[Test]
			[Arguments("{}")]
			[Arguments("[]")]
			[Arguments("2")]
			[Arguments("true")]
			[Arguments("null")]
			public async Task WhenJsonIsNoString_ShouldSucceed(string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsString());

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WithExpectationsTests
		{
			[Test]
			public async Task WhenExpectationsAreMet_ShouldSucceed()
			{
				JsonElement subject = FromString("\"Let It Be\"");

				async Task Act()
					=> await That(subject).IsString(s => s.StartsWith("let").IgnoringCase());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectationsAreNotMet_ShouldFail()
			{
				JsonElement subject = FromString("\"Yesterday\"");

				async Task Act()
					=> await That(subject).IsString(s => s.IsEqualTo("Let*").AsWildcard());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a string which matches "Let*",
					             but it did not match:
					               ↓ (actual)
					               "Yesterday"
					               "Let*"
					               ↑ (wildcard pattern)
					             """);
			}

			[Test]
			public async Task WhenExpectationsAreNull_ShouldThrowArgumentNullException()
			{
				JsonElement subject = FromString("\"foo\"");

				async Task Act()
					=> await That(subject).IsString((Action<IThatSubject<string>>)null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenJsonIsNoString_ShouldNotVerifyTheExpectations()
			{
				JsonElement subject = FromString("243");

				async Task Act()
					=> await That(subject).IsString(s => s.IsEqualTo("243"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a string which is equal to "243",
					             but it was a number

					             Actual:
					             243
					             """);
			}

			[Test]
			public async Task WhenNegatedAndExpectationsAreMet_ShouldFail()
			{
				JsonElement subject = FromString("\"Let It Be\"");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsString(s => s.StartsWith("Let")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not a string or its value does not start with "Let",
					             but it was "Let It Be"
					             """);
			}

			[Test]
			public async Task WhenNegatedAndExpectationsAreNotMet_ShouldSucceed()
			{
				JsonElement subject = FromString("\"Yesterday\"");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsString(s => s.StartsWith("Let")));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNegatedAndJsonIsNoString_ShouldSucceed()
			{
				JsonElement subject = FromString("1");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsString(s => s.StartsWith("Let")));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNegatedInHasProperty_ShouldNameEveryAlternative()
			{
				JsonElement subject = FromString("{\"title\": \"Let It Be\"}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it
						.HasProperty("title", t => t.IsString(s => s.StartsWith("Let"))));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have property "title" or $.title is not a string or its value does not start with "Let",
					             but $.title was "Let It Be"
					             """);
			}
		}

		public sealed class WithOperatorTests
		{
			[Test]
			public async Task WhenContaining_ShouldCountTheOccurrences()
			{
				JsonElement subject = FromString("\"Let It Be\"");

				async Task Act()
					=> await That(subject).IsString().Containing("e").AtLeast(3);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a string containing "e" at least 3 times,
					             but it contained "e" twice in "Let It Be"
					             """);
			}

			[Test]
			public async Task WhenEqualTo_AsWildcard_ShouldSucceed()
			{
				JsonElement subject = FromString("\"Let It Be\"");

				async Task Act()
					=> await That(subject).IsString().EqualTo("Let*").AsWildcard();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEqualTo_IgnoringCase_ShouldSucceed()
			{
				JsonElement subject = FromString("\"Let It Be\"");

				async Task Act()
					=> await That(subject).IsString().EqualTo("LET IT BE").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				JsonElement subject = FromString("\"Let It Be\"");

				async Task Act()
					=> await That(subject).IsString().EqualTo(null);

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'expected' cannot be null.").AsPrefix()
					.Because("a JSON string is never null");
			}

			[Test]
			[Arguments("42", "a number")]
			[Arguments("null", "null")]
			[Arguments("{}", "an object")]
			public async Task WhenJsonIsNoString_ShouldFail(string json, string kindString)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsString().StartingWith("4");

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is a string starting with "4",
					              but it was {kindString}
					              """);
			}

			[Test]
			public async Task WhenNestedInHasProperty_ShouldNameThePathOfTheProperty()
			{
				JsonElement subject = FromString("{\"title\": \"Yesterday\"}");

				async Task Act()
					=> await That(subject).HasProperty("title", t => t.IsString().StartingWith("Let"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has property "title" which is a string starting with "Let",
					             but $.title was "Yesterday", which differs at index 0:
					                ↓ (actual)
					               "Yesterday"
					               "Let"
					                ↑ (expected prefix)
					             """);
			}

			[Test]
			public async Task WhenNotStartingWith_AndStringStartsWithIt_ShouldFail()
			{
				JsonElement subject = FromString("\"Let It Be\"");

				async Task Act()
					=> await That(subject).IsString().NotStartingWith("Let");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a string not starting with "Let",
					             but it was "Let It Be"
					             """);
			}

			[Test]
			public async Task WhenOperatorIsCalledTwice_ShouldThrowInvalidOperationException()
			{
				JsonElement subject = FromString("\"Let It Be\"");

				async Task Act()
					=> await That(subject).CompliesWith(it =>
					{
						IsStringResult<JsonElement, IThat<JsonElement>> result = it.IsString();
						result.StartingWith("Let");
						result.EndingWith("Be");
					});

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("IsString() accepts only one comparison, so call only one operator on its result.");
			}

			[Test]
			[Arguments("\"Let It Be\"")]
			[Arguments("42")]
			[Arguments("null")]
			public async Task WhenPatternIsInvalid_ShouldThrowArgumentException(string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsString().EqualTo("[").AsRegex();

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' regex pattern is invalid: ").AsPrefix()
					.Because("an invalid pattern is rejected whichever value the subject has");
			}

			[Test]
			public async Task WhenStringDiffers_ShouldFailWithTheMessageOfTheValueOverload()
			{
				JsonElement subject = FromString("\"Let It Bee\"");

				async Task Act()
					=> await That(subject).IsString().EqualTo("Let It Be");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a string equal to "Let It Be",
					             but it was "Let It Bee" with a length of 10, which is longer than the expected length of 9 and has superfluous:
					               "e"
					             """);
			}

			[Test]
			public async Task WhenStringMatches_ShouldContinueOnTheElement()
			{
				JsonElement subject = FromString("\"Let It Be\"");

				JsonElement result = await That(subject).IsString().StartingWith("Let").And.IsString().EndingWith("Be");

				await That(result.GetString()).IsEqualTo("Let It Be");
			}
		}

		public sealed class WithOperatorNegatedTests
		{
			[Test]
			public async Task WhenJsonIsNoString_ShouldSucceed()
			{
				JsonElement subject = FromString("42");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsString().StartingWith("4"));

				await That(Act).DoesNotThrow()
					.Because("a value of another kind is no string starting with the prefix, like for IsString()");
			}

			[Test]
			public async Task WhenOperatorIsNegated_ShouldCombineBothNegations()
			{
				JsonElement subject = FromString("\"Yesterday\"");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsString().NotStartingWith("Let"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not a string not starting with "Let",
					             but it was "Yesterday", which differs at index 0:
					                ↓ (actual)
					               "Yesterday"
					               "Let"
					                ↑ (expected prefix)
					             """);
			}

			[Test]
			public async Task WhenStringDoesNotMatch_ShouldSucceed()
			{
				JsonElement subject = FromString("\"Yesterday\"");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsString().StartingWith("Let"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenStringMatches_ShouldFail()
			{
				JsonElement subject = FromString("\"Let It Be\"");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsString().Containing("e").AtLeast(2));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not a string containing "e" at least twice,
					             but it contained "e" at least twice in "Let It Be"
					             """);
			}
		}

		public sealed class WithValueTests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				JsonElement subject = FromString("\"foo\"");

				async Task Act()
					=> await That(subject).IsString((string)null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'expected' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenJsonIsNoString_ShouldFail()
			{
				JsonElement subject = FromString("42");

				async Task Act()
					=> await That(subject).IsString("42");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a string equal to "42",
					             but it was a number

					             Actual:
					             42
					             """);
			}

			[Test]
			public async Task WhenNestedInHasProperty_ShouldNameThePathOfTheProperty()
			{
				JsonElement subject = FromString("{\"title\": \"Yesterday\"}");

				async Task Act()
					=> await That(subject).HasProperty("title", t => t.IsString("Let It Be"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has property "title" which is a string equal to "Let It Be",
					             but $.title was "Yesterday", which differs at index 0:
					                ↓ (actual)
					               "Yesterday"
					               "Let It Be"
					                ↑ (expected)

					             Actual ($.title):
					             "Yesterday"
					             """);
			}

			[Test]
			public async Task WhenStringDiffers_ShouldFail()
			{
				JsonElement subject = FromString("\"Let It Bee\"");

				async Task Act()
					=> await That(subject).IsString("Let It Be");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a string equal to "Let It Be",
					             but it was "Let It Bee" with a length of 10, which is longer than the expected length of 9 and has superfluous:
					               "e"

					             Actual:
					             "Let It Bee"
					             """);
			}

			[Test]
			public async Task WhenStringDiffersOnlyInCase_IgnoringCase_ShouldSucceed()
			{
				JsonElement subject = FromString("\"Let It Be\"");

				async Task Act()
					=> await That(subject).IsString("LET IT BE").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenStringDoesNotStartWithTheExpectedPrefix_AsPrefix_ShouldFail()
			{
				JsonElement subject = FromString("\"Yesterday\"");

				async Task Act()
					=> await That(subject).IsString("Let").AsPrefix();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a string starting with "Let",
					             but it was "Yesterday", which differs at index 0:
					                ↓ (actual)
					               "Yesterday"
					               "Let"
					                ↑ (expected prefix)

					             Actual:
					             "Yesterday"
					             """);
			}

			[Test]
			public async Task WhenStringIsEqual_ShouldSucceed()
			{
				JsonElement subject = FromString("\"Let It Be\"");

				async Task Act()
					=> await That(subject).IsString("Let It Be");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenStringIsLongerThanTheMessageShows_ShouldShowItCompletelyAsContext()
			{
				string value = string.Join(",", Enumerable.Range(1, 60).Select(i => $"track-{i}"));
				JsonElement subject = FromString($"\"{value}\"");

				async Task Act()
					=> await That(subject).IsString("track-0");

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              *

					              Actual:
					              "{value}"
					              """).AsWildcard()
					.Because("the failure shortens the long string, while the context shows it completely");
			}

			[Test]
			public async Task WhenStringMatchesTheWildcardPattern_AsWildcard_ShouldSucceed()
			{
				JsonElement subject = FromString("\"Let It Be\"");

				async Task Act()
					=> await That(subject).IsString("Let*").AsWildcard();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenStringStartsWithTheExpectedPrefix_AsPrefix_ShouldSucceed()
			{
				JsonElement subject = FromString("\"Let It Be\"");

				async Task Act()
					=> await That(subject).IsString("Let").AsPrefix();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WithValueNegatedTests
		{
			[Test]
			public async Task WhenJsonIsNoString_ShouldSucceed()
			{
				JsonElement subject = FromString("42");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsString("42"));

				await That(Act).DoesNotThrow()
					.Because("a value of another kind is no string equal to the expected value, like for IsString()");
			}

			[Test]
			public async Task WhenNestedInHasPropertyAndValueIsNoString_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"rating\": 4.5}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasProperty("rating", r => r.IsString("4.5")));

				await That(Act).DoesNotThrow()
					.Because("the property exists, but is no string equal to the expected value");
			}

			[Test]
			public async Task WhenStringDiffers_ShouldSucceed()
			{
				JsonElement subject = FromString("\"Yesterday\"");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsString("Let It Be"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenStringIsEqual_ShouldFail()
			{
				JsonElement subject = FromString("\"Let It Be\"");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsString("Let It Be"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not a string equal to "Let It Be",
					             but it was "Let It Be"

					             Actual:
					             "Let It Be"
					             """);
			}

			[Test]
			public async Task WhenStringStartsWithTheUnexpectedPrefix_AsPrefix_ShouldFail()
			{
				JsonElement subject = FromString("\"Let It Be\"");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsString("Let").AsPrefix());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not a string starting with "Let",
					             but it was "Let It Be"

					             Actual:
					             "Let It Be"
					             """);
			}
		}
	}
}
