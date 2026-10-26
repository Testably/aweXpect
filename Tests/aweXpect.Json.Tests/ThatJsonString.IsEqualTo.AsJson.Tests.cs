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
				[Test]
				public async Task ForCollections_ShouldSupportAsJson()
				{
					string[] subject = ["{ }", "{\"foo\":1}", "[]",];
					string expected = "{}";

					async Task Act()
						=> await That(subject).All().AreEqualTo(expected).AsJson();

					await That(Act).Throws<FailException>()
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

				[Test]
				[Arguments(nameof(ThatString.Contains))]
				[Arguments(nameof(ThatString.DoesNotContain))]
				public async Task ForStringContains_ShouldNotBeAvailable(string methodName)
				{
					Type result = typeof(ThatString)
						.GetMethod(methodName, [typeof(IThat<string?>), typeof(string),])!.ReturnType;

					void Act()
						=> typeof(JsonExtensions).GetMethod(nameof(JsonExtensions.AsJson))!.MakeGenericMethod(result);

					await That(Act).Throws<ArgumentException>()
						.WithMessage("GenericArguments[0], ?aweXpect.Results.StringOccurrenceCountResult`2*?TSelf?.").AsWildcard()
						.Because("comparing substrings of a string as JSON is not meaningful, so IsEqualTo(…).AsJson() or IsValidJsonMatching should be used instead");
				}

				[Test]
				public async Task WhenCollectionContainsIncorrectJson_ShouldFail()
				{
					string[] subject = ["{}", "foo",];

					async Task Act()
						=> await That(subject).All().AreEqualTo("{}").AsJson();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is JSON equivalent to {} for all items,
						             but an item was "foo", which could not be parsed as JSON: 'foo' is an invalid JSON literal. Expected the literal 'false'. LineNumber: 0 | BytePositionInLine: 1.
						             """)
						.Because("an item that is no JSON cannot be compared, so the value names the item");
				}

				[Test]
				public async Task WhenCollectionContainsIncorrectJsonWithContains_ShouldFail()
				{
					string[] subject = ["foo",];

					async Task Act()
						=> await That(subject).Contains("{}").AsJson();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             contains an item JSON equivalent to {} at least once,
						             but an item was "foo", which could not be parsed as JSON: 'foo' is an invalid JSON literal. Expected the literal 'false'. LineNumber: 0 | BytePositionInLine: 1.
						             """);
				}

				[Test]
				public async Task WhenCollectionIsEmptyAndExpectedIsIncorrectJson_ShouldThrowArgumentException()
				{
					string[] subject = [];

					async Task Act()
						=> await That(subject).All().AreEqualTo("foo").AsJson();

					await That(Act).Throws<ArgumentException>()
						.WithMessage("The 'expected' JSON is invalid: *").AsWildcard()
						.Because("an invalid expected JSON is rejected also when no item is compared with it");
				}

				[Test]
				public async Task WhenCollectionIsEmptyAndExpectedIsIncorrectJsonWithContains_ShouldThrowArgumentException()
				{
					string[] subject = [];

					async Task Act()
						=> await That(subject).Contains("foo").AsJson();

					await That(Act).Throws<ArgumentException>()
						.WithMessage("The 'expected' JSON is invalid: *").AsWildcard()
						.Because("an invalid expected JSON is rejected also when no item is compared with it");
				}

				[Test]
				public async Task WhenCollectionIsEmptyAndExpectedIsNull_ShouldThrowArgumentNullException()
				{
					string[] subject = [];

					async Task Act()
						=> await That(subject).All().AreEqualTo(null).AsJson();

					await That(Act).Throws<ArgumentNullException>()
						.WithMessage("The 'expected' JSON cannot be null.*").AsWildcard()
						.Because("a null expected value is rejected also when no item is compared with it");
				}

				[Test]
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

					await That(Act).Throws<FailException>()
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
						               (… and 2 more)

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

				[Test]
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

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is JSON equivalent to {},
						             but it differed as
						               $.foo1 had unexpected <null> and
						               $.foo2 had unexpected <null> and
						               $.foo3 had unexpected <null> and
						               (… and 9 more)

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

				[Test]
				[Arguments("foo",
					"'foo' is an invalid JSON literal. Expected the literal 'false'. LineNumber: 0 | BytePositionInLine: 1.")]
				[Arguments("{\"foo\":{\"bar\":False}}",
					"'F' is an invalid start of a value. LineNumber: 0 | BytePositionInLine: 14.")]
				[Arguments("{\"foo\":{\"bar\":[1,2}}",
					"'}' is invalid without a matching open. LineNumber: 0 | BytePositionInLine: 18.")]
				public async Task WhenExpectedIsIncorrectJson_ShouldThrowArgumentException(string expected,
					string errorMessage)
				{
					string subject = "{}";

					async Task Act()
						=> await That(subject).IsEqualTo(expected).AsJson(o => o.IgnoringAdditionalProperties());

					await That(Act).Throws<ArgumentException>()
						.WithMessage($"The 'expected' JSON is invalid: {errorMessage}").AsPrefix();
				}

				[Test]
				public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
				{
					string subject = "{}";

					async Task Act()
						=> await That(subject).IsEqualTo(null).AsJson();

					await That(Act).Throws<ArgumentNullException>()
						.WithMessage("The 'expected' JSON cannot be null.*").AsWildcard()
						.Because("a null expected value is no JSON to compare with");
				}

				[Test]
				public async Task WhenIgnoringCase_ShouldThrowInvalidOperationException()
				{
					string subject = "{}";

					async Task Act()
						=> await That(subject).IsEqualTo("{}").AsJson().IgnoringCase();

					await That(Act).Throws<InvalidOperationException>()
						.WithMessage("IgnoringCase cannot be combined with AsJson.")
						.Because("JSON is compared by its structure, so the casing option cannot be honoured");
				}

				[Test]
				public async Task WhenUsingComparer_ShouldThrowInvalidOperationException()
				{
					string subject = "{}";

					async Task Act()
						=> await That(subject).IsEqualTo("{}").Using(StringComparer.Ordinal).AsJson();

					await That(Act).Throws<InvalidOperationException>()
						.WithMessage("Using cannot be combined with AsJson.")
						.Because("JSON is compared by its structure, so a custom comparer cannot be honoured");
				}

				[Test]
				public async Task WhenCombinedWithAnotherMatchType_ShouldThrowInvalidOperationException()
				{
					string subject = "{}";

					async Task Act()
						=> await That(subject).IsEqualTo("{}").AsJson().AsWildcard();

					await That(Act).Throws<InvalidOperationException>()
						.WithMessage("AsWildcard cannot be combined with AsJson.");
				}

				[Test]
				public async Task WhenSpecifiedTwice_ShouldUseTheLastJsonOptions()
				{
					string subject = "{\"foo\": 1, \"bar\": 2}";

					async Task Act()
						=> await That(subject).IsEqualTo("{\"foo\": 1}").AsJson()
							.AsJson(o => o.IgnoringAdditionalProperties());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsIncorrectJsonInWhose_ShouldFail()
				{
					JsonContainer subject = new("foo");

					async Task Act()
						=> await That(subject).Whose(x => x.Json, j => j.IsEqualTo("{}").AsJson());

					await That(Act).Throws<FailException>()
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

				[Test]
				public async Task WhenCollectionContainsNullItem_ShouldFail()
				{
					string?[] subject = ["{}", null,];

					async Task Act()
						=> await That(subject).All().AreEqualTo("{}").AsJson();

					await That(Act).Throws<FailException>()
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

				[Test]
				public async Task WithContains_ShouldUseTheItemWording()
				{
					string[] subject = ["{\"a\":2}",];

					async Task Act()
						=> await That(subject).Contains("{\"a\":1}").AsJson();

					await That(Act).Throws<FailException>()
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

				[Test]
				public async Task WithMessage_ShouldUseTheWithWording()
				{
					Action subject = () => throw new InvalidOperationException("{\"a\":2}");

					async Task Act()
						=> await That(subject).Throws<InvalidOperationException>().WithMessage("{\"a\":1}").AsJson();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             throws an InvalidOperationException with message JSON equivalent to {"a":1},
						             but message differed as $.a was 2 instead of 1

						             Message:
						             {"a":2}
						             """);
				}

				[Test]
				[Arguments("{ \"foo\": 2 }", "{  \"foo\": \"2\"  }", "$.foo was number 2 instead of \"2\"")]
				[Arguments("{ \"foo\": 1.23 }", "{  \"foo\": \"1.23\"  }",
					"$.foo was number 1.23 instead of \"1.23\"")]
				[Arguments("{ \"foo\": \"2\" }", "{  \"foo\": 2  }", "$.foo was string \"2\" instead of 2")]
				[Arguments("{ \"foo\": true }", "{  \"foo\": \"\"  }", "$.foo was boolean True instead of \"\"")]
				[Arguments("{ \"foo\": 1 }", "{  \"foo\": true  }", "$.foo was number 1 instead of True")]
				[Arguments("{ \"foo\": {\"value\":false} }", "{  \"foo\": false  }",
					"$.foo was object {\"value\":false} instead of False")]
				[Arguments("{ \"foo\": null }", "{  \"foo\": \"\"  }", "$.foo was <null> instead of \"\"")]
				public async Task WhenPropertiesOfValuesHaveDifferentType_ShouldFail(string subject, string expected,
					string message)
				{
					async Task Act()
						=> await That(subject).IsEqualTo(expected).AsJson();

					await That(Act).Throws<FailException>()
						.WithMessage($"*but it differed as {message}*").AsWildcard();
				}

				[Test]
				[Arguments("{ \"foo\": 2 }", "{  \"foo\": 3  }", "$.foo was 2 instead of 3")]
				[Arguments("{ \"foo\": 1.23 }", "{  \"foo\": 2.23  }", "$.foo was 1.23 instead of 2.23")]
				[Arguments("{ \"foo\": \"bar\" }", "{  \"foo\": \"baz\"  }", "$.foo was \"bar\" instead of \"baz\"")]
				[Arguments("{ \"foo\": true }", "{  \"foo\": false  }", "$.foo was True instead of False")]
				[Arguments("{ \"foo\": false }", "{  \"foo\": true  }", "$.foo was False instead of True")]
				[Arguments("{ \"foo\": { \"bar\" : 1 } }", "{\"foo\":{\"bar\":2}}", "$.foo.bar was 1 instead of 2")]
				[Arguments("{ \"foo\": [1, 3, 3] }", "{\"foo\":[1,2,3]}", "$.foo[1] was 3 instead of 2")]
				[Arguments("{ \"foo\": 9007199254740993 }", "{  \"foo\": 9007199254740992  }",
					"$.foo was 9007199254740993 instead of 9007199254740992")]
				[Arguments("{ \"foo\": 1e-30 }", "{  \"foo\": 2e-30  }", "$.foo was 1e-30 instead of 2e-30")]
				[Arguments("{ \"foo\": 1e400 }", "{  \"foo\": 2e400  }", "$.foo was 1e400 instead of 2e400")]
				public async Task WhenPropertiesOfValuesHaveDifferentValue_ShouldFail(string subject, string expected,
					string message)
				{
					async Task Act()
						=> await That(subject).IsEqualTo(expected).AsJson();

					await That(Act).Throws<FailException>()
						.WithMessage($"*but it differed as {message}*").AsWildcard();
				}

				[Test]
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

					await That(Act).Throws<FailException>()
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
						               $.baz had missing True and
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

				[Test]
				public async Task WhenSubjectAndExpectedAreNull_ShouldThrowArgumentNullException()
				{
					string? subject = null;
					string? expected = null;

					async Task Act()
						=> await That(subject).IsEqualTo(expected).AsJson();

					await That(Act).Throws<ArgumentNullException>()
						.WithMessage("The 'expected' JSON cannot be null.*").AsWildcard();
				}

				[Test]
				[Arguments(false)]
				[Arguments(true)]
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

					await That(Act).Throws<FailException>().OnlyIf(!ignoreAdditionalProperties)
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

				[Test]
				[Arguments(false)]
				[Arguments(true)]
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

					await That(Act).Throws<FailException>().OnlyIf(!treatNullAsAbsent)
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

				[Test]
				[Arguments(false)]
				[Arguments(true)]
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

					await That(Act).Throws<FailException>().OnlyIf(!treatNullAsAbsent)
						.WithMessage("""
						             Expected that subject
						             is JSON equivalent to {"title": "Let It Be", "album": null},
						             but it differed as $.album had missing <null>

						             Actual:
						             {"title": "Let It Be"}

						             Expected:
						             {"title": "Let It Be", "album": null}
						             """);
				}

				[Test]
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

				[Test]
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

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is JSON equivalent to {"foo": [1]},
						             but it differed as $.foo[1] had unexpected <null>

						             Actual:
						             {"foo": [1, null]}

						             Expected:
						             {"foo": [1]}

						             JSON options:
						              - treat null as absent
						             """)
						.Because("null elements of an array are not affected by the option");
				}

				[Test]
				[Arguments(false)]
				[Arguments(true)]
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

					await That(Act).Throws<FailException>().OnlyIf(!ignoreArrayOrder)
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

				[Test]
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

					await That(Act).Throws<FailException>()
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

						             JSON options:
						              - ignore array order
						             """);
				}

				[Test]
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

				[Test]
				public async Task WhenGeneratedValuesDiffer_WhenIgnoringThem_ShouldSucceed()
				{
					string subject = """
					                 {"id": 1, "title": "Let It Be", "meta": {"createdAt": "today", "etag": "a"}}
					                 """;
					string expected = """
					                  {"id": 2, "title": "Let It Be", "meta": {"createdAt": "yesterday"}}
					                  """;

					async Task Act()
						=> await That(subject).IsEqualTo(expected)
							.AsJson(o => o.IgnoringMember("id").IgnoringMember("meta.createdAt")
								.IgnoringMembers(path => path.EndsWith(".etag")));

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenOtherValueDiffers_WhenIgnoringMember_ShouldFail()
				{
					string subject = """
					                 {"id": 1, "title": "Let It Be"}
					                 """;
					string expected = """
					                  {"id": 2, "title": "Yesterday"}
					                  """;

					async Task Act()
						=> await That(subject).IsEqualTo(expected).AsJson(o => o.IgnoringMember("id"));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is JSON equivalent to {"id": 2, "title": "Yesterday"},
						             but it differed as $.title was "Let It Be" instead of "Yesterday"

						             Actual:
						             {"id": 1, "title": "Let It Be"}

						             Expected:
						             {"id": 2, "title": "Yesterday"}

						             JSON options:
						              - ignore members: ["id"]
						             """);
				}

				[Test]
				public async Task ForCollections_WhenIgnoringMember_ShouldIgnoreItForEachItem()
				{
					string[] subject = ["{\"id\": 1, \"a\": 1}", "{\"id\": 2, \"a\": 1}",];

					async Task Act()
						=> await That(subject).All().AreEqualTo("{\"id\": 3, \"a\": 1}")
							.AsJson(o => o.IgnoringMember("id"));

					await That(Act).DoesNotThrow();
				}

				[Test]
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

					await That(Act).Throws<FailException>()
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

						             JSON options:
						              - ignore additional properties
						             """);
				}

				[Test]
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

					await That(Act).Throws<FailException>()
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

				[Test]
				[Arguments(false)]
				[Arguments(true)]
				public async Task WhenSubjectHasMoreArrayItems_ShouldFailUnlessIgnoringAdditionalArrayElements(
					bool ignoreAdditionalArrayElements)
				{
					string subject = """
					                 {"foo": [1, 2, 3]}
					                 """;
					string expected = """
					                  {"foo": [1, 2]}
					                  """;

					async Task Act()
						=> await That(subject).IsEqualTo(expected)
							.AsJson(o => o.IgnoringAdditionalArrayElements(ignoreAdditionalArrayElements));

					await That(Act).Throws<FailException>().OnlyIf(!ignoreAdditionalArrayElements)
						.WithMessage("""
						             Expected that subject
						             is JSON equivalent to {"foo": [1, 2]},
						             but it differed as $.foo[2] had unexpected 3

						             Actual:
						             {"foo": [1, 2, 3]}

						             Expected:
						             {"foo": [1, 2]}
						             """);
				}

				[Test]
				public async Task WhenSubjectHasMoreArrayItems_WhenIgnoringAdditionalProperties_ShouldFail()
				{
					string subject = """
					                 {"foo": [1, 2, 3], "bar": 1}
					                 """;
					string expected = """
					                  {"foo": [1, 2]}
					                  """;

					async Task Act()
						=> await That(subject).IsEqualTo(expected).AsJson(o => o.IgnoringAdditionalProperties());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is JSON equivalent to {"foo": [1, 2]},
						             but it differed as $.foo[2] had unexpected 3

						             Actual:
						             {"foo": [1, 2, 3], "bar": 1}

						             Expected:
						             {"foo": [1, 2]}

						             JSON options:
						              - ignore additional properties
						             """)
						.Because("ignoring additional properties does not affect the elements of arrays");
				}

				[Test]
				public async Task WhenArrayElementsAreASubsetInAnyOrder_WhenIgnoringArrayOrderAndAdditionalArrayElements_ShouldSucceed()
				{
					string subject = """
					                 ["rock", "blues", "jazz"]
					                 """;
					string expected = """
					                  ["jazz", "rock"]
					                  """;

					async Task Act()
						=> await That(subject).IsEqualTo(expected)
							.AsJson(o => o.IgnoringArrayOrder().IgnoringAdditionalArrayElements());

					await That(Act).DoesNotThrow();
				}

				[Test]
				[Arguments("foo",
					"'foo' is an invalid JSON literal. Expected the literal 'false'. LineNumber: 0 | BytePositionInLine: 1.")]
				[Arguments("{\"foo\":{\"bar\":False}}",
					"'F' is an invalid start of a value. LineNumber: 0 | BytePositionInLine: 14.")]
				[Arguments("{\"foo\":{\"bar\":[1,2}}",
					"'}' is invalid without a matching open. LineNumber: 0 | BytePositionInLine: 18.")]
				public async Task WhenSubjectIsIncorrectJson_ShouldFail(string subject, string errorMessage)
				{
					string expected = "{}";

					async Task Act()
						=> await That(subject).IsEqualTo(expected).AsJson(o => o.IgnoringAdditionalProperties());

					await That(Act).Throws<FailException>()
						.WithMessage($$"""
						               Expected that subject
						               is JSON equivalent to {},
						               but it was "{{subject.Replace("\"", "\\\"")}}", which could not be parsed as JSON: {{errorMessage}}

						               Actual:
						               {{subject}}

						               Expected:
						               {}

						               JSON options:
						                - ignore additional properties
						               """);
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					string? subject = null;
					string expected = "{}";

					async Task Act()
						=> await That(subject).IsEqualTo(expected).AsJson();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is JSON equivalent to {},
						             but it was <null>
						             """);
				}


				[Test]
				[Arguments("{ \"foo\": 2 }", "{  \"foo\": 2  }", "integer values are supported")]
				[Arguments("{ \"foo\": 1.23 }", "{  \"foo\": 1.23  }", "floating values are supported")]
				[Arguments("{ \"foo\": 9007199254740993 }", "{  \"foo\": 9007199254740993  }",
					"integers beyond the precision of a double are supported")]
				[Arguments("{ \"foo\": 1.0 }", "{  \"foo\": 1  }", "the notation of a number is ignored")]
				[Arguments("{ \"foo\": 1.5e2 }", "{  \"foo\": 150  }", "the exponent notation is supported")]
				[Arguments("{ \"foo\": 1e400 }", "{  \"foo\": 1e400  }",
					"numbers beyond the range of a double are supported")]
				[Arguments("{ \"foo\": \"bar\" }", "{  \"foo\": \"bar\"  }", "string values are supported")]
				[Arguments("{ \"foo\": true }", "{  \"foo\": true  }", "boolean true is supported")]
				[Arguments("{ \"foo\": false }", "{  \"foo\": false  }", "boolean false is supported")]
				[Arguments("{ \"foo\": null }", "{  \"foo\": null  }", "null is supported")]
				[Arguments("{ \"foo\": { \"bar\" : 1 } }", "{\"foo\":{\"bar\":1}}", "nested objects are supported")]
				[Arguments("{ \"foo\": [ 1, 2, 3 ] }", "{\"foo\":[1,2,3]}", "arrays are supported")]
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
				[Test]
				public async Task WhenCollectionContainsIncorrectJson_ShouldFail()
				{
					string[] subject = ["{\"foo\":1}", "foo",];

					async Task Act()
						=> await That(subject).None().AreEqualTo("{}").AsJson();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is JSON equivalent to {} for no items,
						             but an item was "foo", which could not be parsed as JSON: 'foo' is an invalid JSON literal. Expected the literal 'false'. LineNumber: 0 | BytePositionInLine: 1.
						             """)
						.Because("an item that is no JSON is not different from the unexpected JSON either");
				}

				[Test]
				public async Task WhenCollectionContainsIncorrectJsonWithDoesNotContain_ShouldFail()
				{
					string[] subject = ["{\"foo\":1}", "foo",];

					async Task Act()
						=> await That(subject).DoesNotContain("{}").AsJson();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             does not contain an item JSON equivalent to {},
						             but an item was "foo", which could not be parsed as JSON: 'foo' is an invalid JSON literal. Expected the literal 'false'. LineNumber: 0 | BytePositionInLine: 1.
						             """)
						.Because("an item that is no JSON is not different from the unexpected JSON either");
				}

				[Test]
				public async Task WhenCollectionIsEmptyAndExpectedIsIncorrectJson_ShouldThrowArgumentException()
				{
					string[] subject = [];

					async Task Act()
						=> await That(subject).DoesNotContain("foo").AsJson();

					await That(Act).Throws<ArgumentException>()
						.WithMessage("The 'unexpected' JSON is invalid: *").AsWildcard()
						.Because("an invalid expected JSON would let the negation pass for every collection");
				}

				[Test]
				public async Task WhenExpectedIsIncorrectJson_ShouldThrowArgumentException()
				{
					string subject = "{}";

					async Task Act()
						=> await That(subject).IsNotEqualTo("foo").AsJson();

					await That(Act).Throws<ArgumentException>()
						.WithMessage("The 'unexpected' JSON is invalid: *").AsWildcard()
						.Because("an invalid expected JSON would let the negation pass for every subject");
				}

				[Test]
				public async Task WhenSubjectIsEquivalent_ShouldFail()
				{
					string subject = "{}";

					async Task Act()
						=> await That(subject).IsNotEqualTo("{ }").AsJson();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not JSON equivalent to { },
						             but it was "{}"

						             Expected:
						             { }
						             """);
				}

				[Test]
				public async Task WhenSubjectIsIncorrectJson_ShouldFail()
				{
					string subject = "foo";

					async Task Act()
						=> await That(subject).IsNotEqualTo("{}").AsJson();

					await That(Act).Throws<FailException>()
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

				[Test]
				public async Task WhenSubjectIsEquivalentWhenTreatingNullAsAbsent_ShouldFail()
				{
					string subject = "{\"foo\":null}";

					async Task Act()
						=> await That(subject).IsNotEqualTo("{}").AsJson(o => o.TreatingNullAsAbsent());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not JSON equivalent to {},
						             but it was "{\"foo\":null}"

						             Expected:
						             {}

						             JSON options:
						              - treat null as absent
						             """);
				}

				[Test]
				public async Task WhenSubjectIsEquivalentWhenIgnoringArrayOrder_ShouldFail()
				{
					string subject = "[1,2]";

					async Task Act()
						=> await That(subject).IsNotEqualTo("[2,1]").AsJson(o => o.IgnoringArrayOrder());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not JSON equivalent to [2,1],
						             but it was "[1,2]"

						             Expected:
						             [2,1]

						             JSON options:
						              - ignore array order
						             """);
				}

				[Test]
				public async Task WhenSubjectOnlyDiffersInIgnoredMember_ShouldFail()
				{
					string subject = "{\"id\":1,\"a\":2}";

					async Task Act()
						=> await That(subject).IsNotEqualTo("{\"id\":3,\"a\":2}").AsJson(o => o.IgnoringMember("id"));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not JSON equivalent to {"id":3,"a":2},
						             but it was "{\"id\":1,\"a\":2}"

						             Expected:
						             {"id":3,"a":2}

						             JSON options:
						              - ignore members: ["id"]
						             """);
				}

				[Test]
				public async Task WhenSubjectIsNotEquivalent_ShouldSucceed()
				{
					string subject = "{\"foo\":1}";

					async Task Act()
						=> await That(subject).IsNotEqualTo("{}").AsJson();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					string? subject = null;

					async Task Act()
						=> await That(subject).IsNotEqualTo("{}").AsJson();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not JSON equivalent to {},
						             but it was <null>
						             """)
						.Because("a null subject has no JSON content to inspect");
				}
			}

			public sealed class ComparingNumbersWithinTests
			{
				[Test]
				public async Task WhenNumbersAreWithinTheTolerance_ShouldSucceed()
				{
					string subject = """{"total": 0.30000000000000004, "tax": 0.0575}""";

					async Task Act()
						=> await That(subject).IsEqualTo("""{"total": 0.3, "tax": 0.057}""")
							.AsJson(o => o.ComparingNumbersWithin(0.001));

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenNumberIsOutsideTheTolerance_ShouldFail()
				{
					string result = """{"total": 0.3, "tax": 0.06}""";

					async Task Act()
						=> await That(result).IsEqualTo("""{"total": 0.3, "tax": 0.057}""")
							.AsJson(o => o.ComparingNumbersWithin(0.001));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that result
						             is JSON equivalent to {"total": 0.3, "tax": 0.057},
						             but it differed as $.tax was 0.06 instead of 0.057 ± 0.001

						             Actual:
						             {"total": 0.3, "tax": 0.06}

						             Expected:
						             {"total": 0.3, "tax": 0.057}

						             JSON options:
						              - compare numbers within ± 0.001
						             """);
				}

				[Test]
				[Arguments("1e400", true)]
				[Arguments("1.0e400", false)]
				[Arguments("2e400", false)]
				public async Task WhenNumbersAreOutsideTheRangeOfADouble_ShouldCompareTheirText(string json,
					bool isMatch)
				{
					async Task Act()
						=> await That(json).IsEqualTo("1e400").AsJson(o => o.ComparingNumbersWithin(1e300));

					await That(Act).Throws<FailException>().OnlyIf(!isMatch)
						.WithMessage($"""
						              Expected that json
						              is JSON equivalent to 1e400,
						              but it differed as $ was {json} instead of 1e400 ± 1E+300

						              Actual:
						              {json}

						              Expected:
						              1e400

						              JSON options:
						               - compare numbers within ± 1E+300
						              """)
						.Because("the tolerance cannot be applied to numbers outside the range of a double");
				}

				[Test]
				public async Task WhenNegated_WhenNumbersAreWithinTheTolerance_ShouldFail()
				{
					string subject = "[1.05, 2]";

					async Task Act()
						=> await That(subject).IsNotEqualTo("[1, 2]").AsJson(o => o.ComparingNumbersWithin(0.1));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not JSON equivalent to [1, 2],
						             but it was "[1.05, 2]"

						             Expected:
						             [1, 2]

						             JSON options:
						              - compare numbers within ± 0.1
						             """);
				}

				[Test]
				public async Task WhenNegated_WhenNumbersAreOutsideTheTolerance_ShouldSucceed()
				{
					string subject = "[1.2, 2]";

					async Task Act()
						=> await That(subject).IsNotEqualTo("[1, 2]").AsJson(o => o.ComparingNumbersWithin(0.1));

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
