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
					string[] subject = ["{ }", "{foo:1}", "[]",];
					string expected = "{}";

					async Task Act()
						=> await That(subject).All().AreEqualTo(expected).AsJson();

					await That(Act).Throws<XunitException>()
						.WithMessage(""""
						             Expected that subject
						             is equal to "{}" as JSON for all items,
						             but only 1 of 3 were
						             
						             Not matching items:
						             [
						               "{foo:1}",
						               "[]"
						             ]
						             
						             Collection:
						             [
						               "{ }",
						               "{foo:1}",
						               "[]"
						             ]
						             """");
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
						               $.foo1 had unexpected Null and
						               $.foo2 had unexpected Null and
						               $.foo3 had unexpected Null and
						               $.foo4 had unexpected Null and
						               $.foo5 had unexpected Null and
						               $.foo6 had unexpected Null and
						               $.foo7 had unexpected Null and
						               $.foo8 had unexpected Null and
						               $.foo9 had unexpected Null and
						               $.foo10 had unexpected Null and
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
						               $.foo1 had unexpected Null and
						               $.foo2 had unexpected Null and
						               $.foo3 had unexpected Null and
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
						             """);
				}

				[Theory]
				[InlineData("foo",
					"'foo' is an invalid JSON literal. Expected the literal 'false'. LineNumber: 0 | BytePositionInLine: 1.")]
				[InlineData("{\"foo\":{\"bar\":False}}",
					"'F' is an invalid start of a value. LineNumber: 0 | BytePositionInLine: 14.")]
				[InlineData("{\"foo\":{\"bar\":[1,2}}",
					"'}' is invalid without a matching open. LineNumber: 0 | BytePositionInLine: 18.")]
				public async Task WhenExpectedIsIncorrectJson_ShouldFail(string expected, string errorMessage)
				{
					string subject = "{}";

					async Task Act()
						=> await That(subject).IsEqualTo(expected).AsJson(o => o.IgnoringAdditionalProperties());

					await That(Act).Throws<XunitException>()
						.WithMessage($$"""
						               Expected that subject
						               is JSON equivalent to {{expected}},
						               but could not parse expected: {{errorMessage}}

						               Actual:
						               {}
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
				[InlineData("{ \"foo\": null }", "{  \"foo\": \"\"  }", "$.foo was Null instead of \"\"")]
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
						             """);
				}

				[Fact]
				public async Task WhenSubjectAndExpectedAreNull_ShouldSucceed()
				{
					string? subject = null;
					string? expected = null;

					async Task Act()
						=> await That(subject).IsEqualTo(expected).AsJson();

					await That(Act).DoesNotThrow();
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
						             """);
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
						               but could not parse subject: {{errorMessage}}

						               Actual:
						               {{subject}}
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
			}
		}
	}
}
