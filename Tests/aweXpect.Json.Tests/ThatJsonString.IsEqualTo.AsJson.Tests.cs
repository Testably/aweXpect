using aweXpect.Core;
using aweXpect.Customization;

namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonString
{
	public sealed class IsEqualTo
	{
		public sealed class AsJson
		{
			public sealed class Tests
			{
				[Fact]
				public async Task ForCollections_ShouldSupportAsJson()
				{
					string[] subject = ["{ }", "{\"foo\":1}", "[]",];
					string expected = "{}";

					async Task Act()
						=> await That(subject).All().AreEqualTo(expected).AsJson();

					await That(Act).Throws<XunitException>()
						.WithMessage(""""
						             Expected that subject
						             is JSON equivalent to {} for all items,
						             but only 1 of 3 were

						             Not matching items:
						             [
						               "{\"foo\":1}",
						               "[]"
						             ]

						             Collection:
						             [
						               "{ }",
						               "{\"foo\":1}",
						               "[]"
						             ]
						             """");
				}

				[Theory]
				[InlineData(nameof(ThatString.Contains))]
				[InlineData(nameof(ThatString.DoesNotContain))]
				public async Task ForStringContains_ShouldNotBeAvailable(string methodName)
				{
					Type result = typeof(ThatString)
						.GetMethod(methodName, [typeof(IThat<string?>), typeof(string),])!.ReturnType;

					void Act()
						=> typeof(JsonExtensions).GetMethod(nameof(JsonExtensions.AsJson))!.MakeGenericMethod(result);

					await That(Act).Throws<ArgumentException>()
						.WithMessage("GenericArguments[0], ?aweXpect.Results.StringOccurrenceCountResult`2*?TResult?.").AsWildcard()
						.Because("comparing substrings of a string as JSON is not meaningful, so IsEqualTo(…).AsJson() or IsValidJsonMatching should be used instead");
				}

				[Fact]
				public async Task WhenCollectionContainsIncorrectJson_ShouldFail()
				{
					string[] subject = ["{}", "foo",];

					async Task Act()
						=> await That(subject).All().AreEqualTo("{}").AsJson();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is JSON equivalent to {} for all items,
						             but an item was "foo", which could not be parsed as JSON: 'foo' is an invalid JSON literal. Expected the literal 'false'. LineNumber: 0 | BytePositionInLine: 1.
						             """)
						.Because("an item that is no JSON cannot be compared, so the value names the item");
				}

				[Fact]
				public async Task WhenCollectionContainsIncorrectJsonWithContains_ShouldFail()
				{
					string[] subject = ["foo",];

					async Task Act()
						=> await That(subject).Contains("{}").AsJson();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             contains an item JSON equivalent to {} at least once,
						             but an item was "foo", which could not be parsed as JSON: 'foo' is an invalid JSON literal. Expected the literal 'false'. LineNumber: 0 | BytePositionInLine: 1.
						             """);
				}

				[Fact]
				public async Task WhenCollectionIsEmptyAndExpectedIsIncorrectJson_ShouldThrowArgumentException()
				{
					string[] subject = [];

					async Task Act()
						=> await That(subject).All().AreEqualTo("foo").AsJson();

					await That(Act).Throws<ArgumentException>()
						.WithMessage("The expected JSON is invalid: *").AsWildcard()
						.Because("an invalid expected JSON is rejected also when no item is compared with it");
				}

				[Fact]
				public async Task WhenCollectionIsEmptyAndExpectedIsIncorrectJsonWithContains_ShouldThrowArgumentException()
				{
					string[] subject = [];

					async Task Act()
						=> await That(subject).Contains("foo").AsJson();

					await That(Act).Throws<ArgumentException>()
						.WithMessage("The expected JSON is invalid: *").AsWildcard()
						.Because("an invalid expected JSON is rejected also when no item is compared with it");
				}

				[Fact]
				public async Task WhenCollectionIsEmptyAndExpectedIsNull_ShouldThrowArgumentNullException()
				{
					string[] subject = [];

					async Task Act()
						=> await That(subject).All().AreEqualTo(null).AsJson();

					await That(Act).Throws<ArgumentNullException>()
						.WithMessage("The expected JSON cannot be null.*").AsWildcard()
						.Because("a null expected value is rejected also when no item is compared with it");
				}

				[Fact]
				public async Task WhenContainsMoreThan10Differences_ShouldLimitListOfDifferences()
				{
					string subject = """
					                 {
					                   "foo1": null,
					                   "foo2": null,
					                   "foo3": null,
					                   "foo4": null,
					                   "foo5": null,
					                   "foo6": null,
					                   "foo7": null,
					                   "foo8": null,
					                   "foo9": null,
					                   "foo10": null,
					                   "foo11": null,
					                   "foo12": null,
					                 }
					                 """;
					string expected = "{}";

					async Task Act()
						=> await That(subject).IsEqualTo(expected).AsJson();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is JSON equivalent to {},
						             but it differed as
						               $.foo1 had unexpected <null> and
						               $.foo2 had unexpected <null> and
						               $.foo3 had unexpected <null> and
						               $.foo4 had unexpected <null> and
						               $.foo5 had unexpected <null> and
						               $.foo6 had unexpected <null> and
						               $.foo7 had unexpected <null> and
						               $.foo8 had unexpected <null> and
						               $.foo9 had unexpected <null> and
						               $.foo10 had unexpected <null> and
						                … (2 more)

						             Actual:
						             {
						               "foo1": null,
						               "foo2": null,
						               "foo3": null,
						               "foo4": null,
						               "foo5": null,
						               "foo6": null,
						               "foo7": null,
						               "foo8": null,
						               "foo9": null,
						               "foo10": null,
						               "foo11": null,
						               "foo12": null,
						             }

						             Expected:
						             {}
						             """);
				}

				[Fact]
				public async Task
					WhenContainsMoreThanMaximumNumberOfCollectionItemsDifferences_ShouldLimitListOfDifferences()
				{
					using IDisposable x = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Set(3);
					string subject = """
					                 {
					                   "foo1": null,
					                   "foo2": null,
					                   "foo3": null,
					                   "foo4": null,
					                   "foo5": null,
					                   "foo6": null,
					                   "foo7": null,
					                   "foo8": null,
					                   "foo9": null,
					                   "foo10": null,
					                   "foo11": null,
					                   "foo12": null,
					                 }
					                 """;
					string expected = "{}";

					async Task Act()
						=> await That(subject).IsEqualTo(expected).AsJson();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is JSON equivalent to {},
						             but it differed as
						               $.foo1 had unexpected <null> and
						               $.foo2 had unexpected <null> and
						               $.foo3 had unexpected <null> and
						                … (9 more)

						             Actual:
						             {
						               "foo1": null,
						               "foo2": null,
						               "foo3": null,
						               "foo4": null,
						               "foo5": null,
						               "foo6": null,
						               "foo7": null,
						               "foo8": null,
						               "foo9": null,
						               "foo10": null,
						               "foo11": null,
						               "foo12": null,
						             }

						             Expected:
						             {}
						             """);
				}

				[Theory]
				[InlineData("foo",
					"'foo' is an invalid JSON literal. Expected the literal 'false'. LineNumber: 0 | BytePositionInLine: 1.")]
				[InlineData("{\"foo\":{\"bar\":False}}",
					"'F' is an invalid start of a value. LineNumber: 0 | BytePositionInLine: 14.")]
				[InlineData("{\"foo\":{\"bar\":[1,2}}",
					"'}' is invalid without a matching open. LineNumber: 0 | BytePositionInLine: 18.")]
				public async Task WhenExpectedIsIncorrectJson_ShouldThrowArgumentException(string expected,
					string errorMessage)
				{
					string subject = "{}";

					async Task Act()
						=> await That(subject).IsEqualTo(expected).AsJson(o => o.IgnoringAdditionalProperties());

					await That(Act).Throws<ArgumentException>()
						.WithMessage($"The expected JSON is invalid: {errorMessage}");
				}

				[Fact]
				public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
				{
					string subject = "{}";

					async Task Act()
						=> await That(subject).IsEqualTo(null).AsJson();

					await That(Act).Throws<ArgumentNullException>()
						.WithMessage("The expected JSON cannot be null.*").AsWildcard()
						.Because("a null expected value is no JSON to compare with");
				}

				[Fact]
				public async Task WhenIgnoringCase_ShouldThrowInvalidOperationException()
				{
					string subject = "{}";

					async Task Act()
						=> await That(subject).IsEqualTo("{}").AsJson().IgnoringCase();

					await That(Act).Throws<InvalidOperationException>()
						.WithMessage("IgnoringCase cannot be combined with AsJson.")
						.Because("JSON is compared by its structure, so the casing option cannot be honoured");
				}

				[Fact]
				public async Task WhenUsingComparer_ShouldThrowInvalidOperationException()
				{
					string subject = "{}";

					async Task Act()
						=> await That(subject).IsEqualTo("{}").Using(StringComparer.Ordinal).AsJson();

					await That(Act).Throws<InvalidOperationException>()
						.WithMessage("Using cannot be combined with AsJson.")
						.Because("JSON is compared by its structure, so a custom comparer cannot be honoured");
				}

				[Fact]
				public async Task WhenCombinedWithAnotherMatchType_ShouldThrowInvalidOperationException()
				{
					string subject = "{}";

					async Task Act()
						=> await That(subject).IsEqualTo("{}").AsJson().AsWildcard();

					await That(Act).Throws<InvalidOperationException>()
						.WithMessage("AsWildcard cannot be combined with AsJson.");
				}

				[Fact]
				public async Task WhenSpecifiedTwice_ShouldThrowInvalidOperationException()
				{
					string subject = "{}";

					async Task Act()
						=> await That(subject).IsEqualTo("{}").AsJson().AsJson();

					await That(Act).Throws<InvalidOperationException>()
						.WithMessage("AsJson cannot be specified more than once.");
				}

				[Fact]
				public async Task WhenSubjectIsIncorrectJsonInWhose_ShouldFail()
				{
					JsonContainer subject = new("foo");

					async Task Act()
						=> await That(subject).Whose(x => x.Json, j => j.IsEqualTo("{}").AsJson());

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             whose Json is JSON equivalent to {},
						             but Json was "foo", which could not be parsed as JSON: 'foo' is an invalid JSON literal. Expected the literal 'false'. LineNumber: 0 | BytePositionInLine: 1.

						             Actual (Json):
						             foo

						             Expected (Json):
						             {}
						             """);
				}

				[Fact]
				public async Task WhenCollectionContainsNullItem_ShouldFail()
				{
					string?[] subject = ["{}", null,];

					async Task Act()
						=> await That(subject).All().AreEqualTo("{}").AsJson();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is JSON equivalent to {} for all items,
						             but only 1 of 2 were

						             Not matching items:
						             [
						               <null>
						             ]

						             Collection:
						             [
						               "{}",
						               <null>
						             ]
						             """)
						.Because("a null item is no JSON");
				}

				[Fact]
				public async Task WithContains_ShouldUseTheItemWording()
				{
					string[] subject = ["{\"a\":2}",];

					async Task Act()
						=> await That(subject).Contains("{\"a\":1}").AsJson();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             contains an item JSON equivalent to {"a":1} at least once,
						             but it did not contain it

						             Collection:
						             [
						               "{\"a\":2}"
						             ]
						             """);
				}

				[Fact]
				public async Task WithMessage_ShouldUseTheWithWording()
				{
					Action subject = () => throw new InvalidOperationException("{\"a\":2}");

					async Task Act()
						=> await That(subject).Throws<InvalidOperationException>().WithMessage("{\"a\":1}").AsJson();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             throws an InvalidOperationException with message JSON equivalent to {"a":1},
						             but message differed as $.a was 2 instead of 1

						             Message:
						             {"a":2}
						             """);
				}

				[Theory]
				[InlineData("{ \"foo\": 2 }", "{  \"foo\": \"2\"  }", "$.foo was number 2 instead of \"2\"")]
				[InlineData("{ \"foo\": 1.23 }", "{  \"foo\": \"1.23\"  }",
					"$.foo was number 1.23 instead of \"1.23\"")]
				[InlineData("{ \"foo\": \"2\" }", "{  \"foo\": 2  }", "$.foo was string \"2\" instead of 2")]
				[InlineData("{ \"foo\": true }", "{  \"foo\": \"\"  }", "$.foo was boolean True instead of \"\"")]
				[InlineData("{ \"foo\": 1 }", "{  \"foo\": true  }", "$.foo was number 1 instead of True")]
				[InlineData("{ \"foo\": {\"value\":false} }", "{  \"foo\": false  }",
					"$.foo was object {\"value\":false} instead of False")]
				[InlineData("{ \"foo\": null }", "{  \"foo\": \"\"  }", "$.foo was <null> instead of \"\"")]
				public async Task WhenPropertiesOfValuesHaveDifferentType_ShouldFail(string subject, string expected,
					string message)
				{
					async Task Act()
						=> await That(subject).IsEqualTo(expected).AsJson();

					await That(Act).Throws<XunitException>()
						.WithMessage($"*but it differed as {message}*").AsWildcard();
				}

				[Theory]
				[InlineData("{ \"foo\": 2 }", "{  \"foo\": 3  }", "$.foo was 2 instead of 3")]
				[InlineData("{ \"foo\": 1.23 }", "{  \"foo\": 2.23  }", "$.foo was 1.23 instead of 2.23")]
				[InlineData("{ \"foo\": \"bar\" }", "{  \"foo\": \"baz\"  }", "$.foo was \"bar\" instead of \"baz\"")]
				[InlineData("{ \"foo\": true }", "{  \"foo\": false  }", "$.foo was True instead of False")]
				[InlineData("{ \"foo\": false }", "{  \"foo\": true  }", "$.foo was False instead of True")]
				[InlineData("{ \"foo\": { \"bar\" : 1 } }", "{\"foo\":{\"bar\":2}}", "$.foo.bar was 1 instead of 2")]
				[InlineData("{ \"foo\": [1, 3, 3] }", "{\"foo\":[1,2,3]}", "$.foo[1] was 3 instead of 2")]
				[InlineData("{ \"foo\": 9007199254740993 }", "{  \"foo\": 9007199254740992  }",
					"$.foo was 9007199254740993 instead of 9007199254740992")]
				[InlineData("{ \"foo\": 1e-30 }", "{  \"foo\": 2e-30  }", "$.foo was 1e-30 instead of 2e-30")]
				[InlineData("{ \"foo\": 1e400 }", "{  \"foo\": 2e400  }", "$.foo was 1e400 instead of 2e400")]
				public async Task WhenPropertiesOfValuesHaveDifferentValue_ShouldFail(string subject, string expected,
					string message)
				{
					async Task Act()
						=> await That(subject).IsEqualTo(expected).AsJson();

					await That(Act).Throws<XunitException>()
						.WithMessage($"*but it differed as {message}*").AsWildcard();
				}

				[Fact]
				public async Task WhenStringsHaveMultipleDifferences_ShouldListAllDifferencesInFailureMessage()
				{
					string subject = """
					                 { "foo": 1.1, "bar": "baz", "something": "else" }
					                 """;
					string expected = """
					                  {
					                    "foo": 2.1,
					                    "bar": "bart",
					                    "baz": true
					                  }
					                  """;

					async Task Act()
						=> await That(subject).IsEqualTo(expected).AsJson();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is JSON equivalent to {
						               "foo": 2.1,
						               "bar": "bart",
						               "baz": true
						             },
						             but it differed as
						               $.foo was 1.1 instead of 2.1 and
						               $.bar was "baz" instead of "bart" and
						               $.baz was missing and
						               $.something had unexpected "else"

						             Actual:
						             { "foo": 1.1, "bar": "baz", "something": "else" }

						             Expected:
						             {
						               "foo": 2.1,
						               "bar": "bart",
						               "baz": true
						             }
						             """);
				}

				[Fact]
				public async Task WhenSubjectAndExpectedAreNull_ShouldThrowArgumentNullException()
				{
					string? subject = null;
					string? expected = null;

					async Task Act()
						=> await That(subject).IsEqualTo(expected).AsJson();

					await That(Act).Throws<ArgumentNullException>()
						.WithMessage("The expected JSON cannot be null.*").AsWildcard();
				}

				[Theory]
				[InlineData(false)]
				[InlineData(true)]
				public async Task WhenSubjectContainsAdditionalMembers_ShouldFailWhenCheckingForAdditionalMembers(
					bool ignoreAdditionalProperties)
				{
					string subject = """
					                 { "foo": 1, "bar" : "xyz" }
					                 """;
					string expected = """
					                  {
					                    "foo": 1
					                  }
					                  """;

					async Task Act()
						=> await That(subject).IsEqualTo(expected)
							.AsJson(o => o.IgnoringAdditionalProperties(ignoreAdditionalProperties));

					await That(Act).Throws<XunitException>().OnlyIf(!ignoreAdditionalProperties)
						.WithMessage("""
						             Expected that subject
						             is JSON equivalent to {
						               "foo": 1
						             },
						             but it differed as $.bar had unexpected "xyz"

						             Actual:
						             { "foo": 1, "bar" : "xyz" }

						             Expected:
						             {
						               "foo": 1
						             }
						             """);
				}

				[Theory]
				[InlineData(false)]
				[InlineData(true)]
				public async Task WhenSubjectContainsAdditionalNullProperty_ShouldFailUnlessTreatingNullAsAbsent(
					bool treatNullAsAbsent)
				{
					string subject = """
					                 {"title": "Let It Be", "album": null}
					                 """;
					string expected = """
					                  {"title": "Let It Be"}
					                  """;

					async Task Act()
						=> await That(subject).IsEqualTo(expected)
							.AsJson(o => o.TreatingNullAsAbsent(treatNullAsAbsent));

					await That(Act).Throws<XunitException>().OnlyIf(!treatNullAsAbsent)
						.WithMessage("""
						             Expected that subject
						             is JSON equivalent to {"title": "Let It Be"},
						             but it differed as $.album had unexpected <null>

						             Actual:
						             {"title": "Let It Be", "album": null}

						             Expected:
						             {"title": "Let It Be"}
						             """);
				}

				[Theory]
				[InlineData(false)]
				[InlineData(true)]
				public async Task WhenSubjectMissesExpectedNullProperty_ShouldFailUnlessTreatingNullAsAbsent(
					bool treatNullAsAbsent)
				{
					string subject = """
					                 {"title": "Let It Be"}
					                 """;
					string expected = """
					                  {"title": "Let It Be", "album": null}
					                  """;

					async Task Act()
						=> await That(subject).IsEqualTo(expected)
							.AsJson(o => o.TreatingNullAsAbsent(treatNullAsAbsent));

					await That(Act).Throws<XunitException>().OnlyIf(!treatNullAsAbsent)
						.WithMessage("""
						             Expected that subject
						             is JSON equivalent to {"title": "Let It Be", "album": null},
						             but it differed as $.album was missing

						             Actual:
						             {"title": "Let It Be"}

						             Expected:
						             {"title": "Let It Be", "album": null}
						             """);
				}

				[Fact]
				public async Task WhenNestedNullPropertiesDiffer_WhenTreatingNullAsAbsent_ShouldSucceed()
				{
					string subject = """
					                 {"foo": {"bar": null}, "items": [{"a": 1}, {"a": 2, "b": null}]}
					                 """;
					string expected = """
					                  {"foo": {"baz": null}, "items": [{"a": 1, "b": null}, {"a": 2}]}
					                  """;

					async Task Act()
						=> await That(subject).IsEqualTo(expected).AsJson(o => o.TreatingNullAsAbsent());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenNullArrayElementsDiffer_WhenTreatingNullAsAbsent_ShouldFail()
				{
					string subject = """
					                 {"foo": [1, null]}
					                 """;
					string expected = """
					                  {"foo": [1]}
					                  """;

					async Task Act()
						=> await That(subject).IsEqualTo(expected).AsJson(o => o.TreatingNullAsAbsent());

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is JSON equivalent to {"foo": [1]},
						             but it differed as $.foo[1] had unexpected <null>

						             Actual:
						             {"foo": [1, null]}

						             Expected:
						             {"foo": [1]}
						             """)
						.Because("null elements of an array are not affected by the option");
				}

				[Theory]
				[InlineData(false)]
				[InlineData(true)]
				public async Task WhenArrayElementsAreInDifferentOrder_ShouldFailUnlessIgnoringArrayOrder(
					bool ignoreArrayOrder)
				{
					string subject = """
					                 {"tags": ["rock", "pop"]}
					                 """;
					string expected = """
					                  {"tags": ["pop", "rock"]}
					                  """;

					async Task Act()
						=> await That(subject).IsEqualTo(expected)
							.AsJson(o => o.IgnoringArrayOrder(ignoreArrayOrder));

					await That(Act).Throws<XunitException>().OnlyIf(!ignoreArrayOrder)
						.WithMessage("""
						             Expected that subject
						             is JSON equivalent to {"tags": ["pop", "rock"]},
						             but it differed as
						               $.tags[0] was "rock" instead of "pop" and
						               $.tags[1] was "pop" instead of "rock"

						             Actual:
						             {"tags": ["rock", "pop"]}

						             Expected:
						             {"tags": ["pop", "rock"]}
						             """);
				}

				[Fact]
				public async Task WhenArrayElementsDiffer_WhenIgnoringArrayOrder_ShouldReportMissingAndUnexpectedElements()
				{
					string subject = """
					                 ["rock", "blues", "jazz", "metal"]
					                 """;
					string expected = """
					                  ["jazz", "rock", "pop", "soul"]
					                  """;

					async Task Act()
						=> await That(subject).IsEqualTo(expected).AsJson(o => o.IgnoringArrayOrder());

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is JSON equivalent to ["jazz", "rock", "pop", "soul"],
						             but it differed as
						               $ had missing "pop" and
						               $ had missing "soul" and
						               $[1] had unexpected "blues" and
						               $[3] had unexpected "metal"

						             Actual:
						             ["rock", "blues", "jazz", "metal"]

						             Expected:
						             ["jazz", "rock", "pop", "soul"]
						             """);
				}

				[Fact]
				public async Task WhenObjectsInArrayAreInDifferentOrder_WhenIgnoringArrayOrder_ShouldSucceed()
				{
					string subject = """
					                 [{"id": 1, "tags": ["a", "b"]}, {"id": 2, "tags": ["c"]}]
					                 """;
					string expected = """
					                  [{"id": 2, "tags": ["c"]}, {"id": 1, "tags": ["b", "a"]}]
					                  """;

					async Task Act()
						=> await That(subject).IsEqualTo(expected).AsJson(o => o.IgnoringArrayOrder());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectHasFewerArrayItems_ShouldFail()
				{
					string subject = """
					                 [1, 2]
					                 """;
					string expected = """
					                  [1,2,3,4]
					                  """;

					async Task Act()
						=> await That(subject).IsEqualTo(expected).AsJson(o => o.IgnoringAdditionalProperties());

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is JSON equivalent to [1,2,3,4],
						             but it differed as
						               $[2] had missing 3 and
						               $[3] had missing 4

						             Actual:
						             [1, 2]

						             Expected:
						             [1,2,3,4]
						             """);
				}

				[Fact]
				public async Task WhenSubjectHasMoreArrayItems_ShouldFail()
				{
					string subject = """
					                 {
					                   "foo": [1, 2, 3, 4]
					                 }
					                 """;
					string expected = """
					                  {"foo":[1,2]}
					                  """;

					async Task Act()
						=> await That(subject).IsEqualTo(expected).AsJson();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is JSON equivalent to {"foo":[1,2]},
						             but it differed as
						               $.foo[2] had unexpected 3 and
						               $.foo[3] had unexpected 4

						             Actual:
						             {
						               "foo": [1, 2, 3, 4]
						             }

						             Expected:
						             {"foo":[1,2]}
						             """);
				}

				[Theory]
				[InlineData("foo",
					"'foo' is an invalid JSON literal. Expected the literal 'false'. LineNumber: 0 | BytePositionInLine: 1.")]
				[InlineData("{\"foo\":{\"bar\":False}}",
					"'F' is an invalid start of a value. LineNumber: 0 | BytePositionInLine: 14.")]
				[InlineData("{\"foo\":{\"bar\":[1,2}}",
					"'}' is invalid without a matching open. LineNumber: 0 | BytePositionInLine: 18.")]
				public async Task WhenSubjectIsIncorrectJson_ShouldFail(string subject, string errorMessage)
				{
					string expected = "{}";

					async Task Act()
						=> await That(subject).IsEqualTo(expected).AsJson(o => o.IgnoringAdditionalProperties());

					await That(Act).Throws<XunitException>()
						.WithMessage($$"""
						               Expected that subject
						               is JSON equivalent to {},
						               but it was "{{subject.Replace("\"", "\\\"")}}", which could not be parsed as JSON: {{errorMessage}}

						               Actual:
						               {{subject}}

						               Expected:
						               {}
						               """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					string? subject = null;
					string expected = "{}";

					async Task Act()
						=> await That(subject).IsEqualTo(expected).AsJson();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is JSON equivalent to {},
						             but it was <null>
						             """);
				}


				[Theory]
				[InlineData("{ \"foo\": 2 }", "{  \"foo\": 2  }", "integer values are supported")]
				[InlineData("{ \"foo\": 1.23 }", "{  \"foo\": 1.23  }", "floating values are supported")]
				[InlineData("{ \"foo\": 9007199254740993 }", "{  \"foo\": 9007199254740993  }",
					"integers beyond the precision of a double are supported")]
				[InlineData("{ \"foo\": 1.0 }", "{  \"foo\": 1  }", "the notation of a number is ignored")]
				[InlineData("{ \"foo\": 1.5e2 }", "{  \"foo\": 150  }", "the exponent notation is supported")]
				[InlineData("{ \"foo\": 1e400 }", "{  \"foo\": 1e400  }",
					"numbers beyond the range of a double are supported")]
				[InlineData("{ \"foo\": \"bar\" }", "{  \"foo\": \"bar\"  }", "string values are supported")]
				[InlineData("{ \"foo\": true }", "{  \"foo\": true  }", "boolean true is supported")]
				[InlineData("{ \"foo\": false }", "{  \"foo\": false  }", "boolean false is supported")]
				[InlineData("{ \"foo\": null }", "{  \"foo\": null  }", "null is supported")]
				[InlineData("{ \"foo\": { \"bar\" : 1 } }", "{\"foo\":{\"bar\":1}}", "nested objects are supported")]
				[InlineData("{ \"foo\": [ 1, 2, 3 ] }", "{\"foo\":[1,2,3]}", "arrays are supported")]
				public async Task WhenValuesAreSameValidJson_ShouldSucceed(string subject, string expected,
					string because)
				{
					async Task Act()
						=> await That(subject).IsEqualTo(expected).AsJson();

					await That(Act).DoesNotThrow().Because(because);
				}

				private sealed record JsonContainer(string Json);
			}

			public sealed class NegatedTests
			{
				[Fact]
				public async Task WhenCollectionContainsIncorrectJson_ShouldFail()
				{
					string[] subject = ["{\"foo\":1}", "foo",];

					async Task Act()
						=> await That(subject).None().AreEqualTo("{}").AsJson();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is JSON equivalent to {} for no items,
						             but an item was "foo", which could not be parsed as JSON: 'foo' is an invalid JSON literal. Expected the literal 'false'. LineNumber: 0 | BytePositionInLine: 1.
						             """)
						.Because("an item that is no JSON is not different from the unexpected JSON either");
				}

				[Fact]
				public async Task WhenCollectionContainsIncorrectJsonWithDoesNotContain_ShouldFail()
				{
					string[] subject = ["{\"foo\":1}", "foo",];

					async Task Act()
						=> await That(subject).DoesNotContain("{}").AsJson();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not contain an item JSON equivalent to {},
						             but an item was "foo", which could not be parsed as JSON: 'foo' is an invalid JSON literal. Expected the literal 'false'. LineNumber: 0 | BytePositionInLine: 1.
						             """)
						.Because("an item that is no JSON is not different from the unexpected JSON either");
				}

				[Fact]
				public async Task WhenCollectionIsEmptyAndExpectedIsIncorrectJson_ShouldThrowArgumentException()
				{
					string[] subject = [];

					async Task Act()
						=> await That(subject).DoesNotContain("foo").AsJson();

					await That(Act).Throws<ArgumentException>()
						.WithMessage("The expected JSON is invalid: *").AsWildcard()
						.Because("an invalid expected JSON would let the negation pass for every collection");
				}

				[Fact]
				public async Task WhenExpectedIsIncorrectJson_ShouldThrowArgumentException()
				{
					string subject = "{}";

					async Task Act()
						=> await That(subject).IsNotEqualTo("foo").AsJson();

					await That(Act).Throws<ArgumentException>()
						.WithMessage("The expected JSON is invalid: *").AsWildcard()
						.Because("an invalid expected JSON would let the negation pass for every subject");
				}

				[Fact]
				public async Task WhenSubjectIsEquivalent_ShouldFail()
				{
					string subject = "{}";

					async Task Act()
						=> await That(subject).IsNotEqualTo("{ }").AsJson();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not JSON equivalent to { },
						             but it was "{}"

						             Expected:
						             { }
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsIncorrectJson_ShouldFail()
				{
					string subject = "foo";

					async Task Act()
						=> await That(subject).IsNotEqualTo("{}").AsJson();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not JSON equivalent to {},
						             but it was "foo", which could not be parsed as JSON: 'foo' is an invalid JSON literal. Expected the literal 'false'. LineNumber: 0 | BytePositionInLine: 1.

						             Actual:
						             foo

						             Expected:
						             {}
						             """)
						.Because("a subject that is no JSON is not different from the unexpected JSON either");
				}

				[Fact]
				public async Task WhenSubjectIsEquivalentWhenTreatingNullAsAbsent_ShouldFail()
				{
					string subject = "{\"foo\":null}";

					async Task Act()
						=> await That(subject).IsNotEqualTo("{}").AsJson(o => o.TreatingNullAsAbsent());

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not JSON equivalent to {},
						             but it was "{\"foo\":null}"

						             Expected:
						             {}
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsEquivalentWhenIgnoringArrayOrder_ShouldFail()
				{
					string subject = "[1,2]";

					async Task Act()
						=> await That(subject).IsNotEqualTo("[2,1]").AsJson(o => o.IgnoringArrayOrder());

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not JSON equivalent to [2,1],
						             but it was "[1,2]"

						             Expected:
						             [2,1]
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsNotEquivalent_ShouldSucceed()
				{
					string subject = "{\"foo\":1}";

					async Task Act()
						=> await That(subject).IsNotEqualTo("{}").AsJson();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					string? subject = null;

					async Task Act()
						=> await That(subject).IsNotEqualTo("{}").AsJson();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not JSON equivalent to {},
						             but it was <null>
						             """)
						.Because("a null subject has no JSON content to inspect");
				}
			}
		}
	}
}
