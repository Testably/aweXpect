using System.Text.Json;
using aweXpect.Core;

namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonString
{
	public sealed class IsValidJson
	{
		public sealed class Tests
		{
			[Test]
			[Arguments(true)]
			[Arguments(false)]
			public async Task ShouldUseProvidedOptions(bool allowTrailingCommas)
			{
				string subject = "[1, 2,]";

				async Task Act()
					=> await That(subject).IsValidJson(o => o with
					{
						AllowTrailingCommas = allowTrailingCommas,
					});

				await That(Act).Throws<FailException>().OnlyIf(!allowTrailingCommas);
			}

			[Test]
			public async Task WhenActualIsEmpty_ShouldFail()
			{
				string subject = "";

				async Task Act()
					=> await That(subject).IsValidJson();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is valid JSON,
					             but it could not be parsed: The input does not contain any JSON tokens.*
					             """).AsWildcard();
			}

			[Test]
			[Arguments("True", "'T' is an invalid start of a value")]
			[Arguments("False", "'F' is an invalid start of a value")]
			[Arguments("Null", "'N' is an invalid start of a value")]
			[Arguments("{\"foo\":1}}", "'}' is invalid after a single JSON value. Expected end of data")]
			[Arguments("{}{\"foo\":1}", "'{' is invalid after a single JSON value. Expected end of data")]
			[Arguments("[",
				"Expected depth to be zero at the end of the JSON payload. There is an open JSON object or array that should be closed")]
			public async Task WhenActualIsInvalidJson_ShouldFail(string subject, string errorMessage)
			{
				async Task Act()
					=> await That(subject).IsValidJson();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is valid JSON,
					              but it could not be parsed: {errorMessage}*
					              """).AsWildcard();
			}

			[Test]
			[Arguments("42")]
			[Arguments("\"foo\"")]
			[Arguments("true")]
			[Arguments("false")]
			[Arguments("null")]
			[Arguments("[]")]
			[Arguments("[1]")]
			[Arguments("[\"foo\", \"bar\"]")]
			[Arguments("{}")]
			[Arguments("{\"foo\": null}")]
			[Arguments("{\"foo\": {\"bar\":[1,2,3], \"baz\": true,}}")]
			public async Task WhenActualIsValidJson_ShouldSucceed(string subject)
			{
				async Task Act()
					=> await That(subject).IsValidJson();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItemsArePlural_ShouldUsePluralVerb()
			{
				string subject = "[1]\n{";

				async Task Act()
					=> await That(subject).HasLines(l => l.All().ComplyWith(it => it.IsValidJson()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has lines of which all are valid JSON,
					             *
					             """).AsWildcard()
					.Because("the lines are the plural subject of the expectation");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsValidJson();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is valid JSON,
					             but it was <null>
					             """);
			}
		}

		public sealed class ExpectationsTests
		{
			[Test]
			public async Task WhenActualIsInvalidJson_ShouldFail()
			{
				string subject = "[1, 2";

				async Task Act()
					=> await That(subject).IsValidJson(d => d.Matches([1, 2,]));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is valid JSON which matches [1, 2,],
					             but it could not be parsed: '2' is an invalid end of a number. Expected a delimiter. LineNumber: 0 | BytePositionInLine: 5. and it did throw a JsonReaderException:
					               '2' is an invalid end of a number. Expected a delimiter. LineNumber: 0 | BytePositionInLine: 5.
					             """);
			}

			[Test]
			public async Task WhenActualIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsValidJson(d => d.Matches([1, 2,]));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is valid JSON which matches [1, 2,],
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenCombinedWithAnd_ShouldApplyFollowingExpectationToString()
			{
				string subject = "[1, 2]";

				async Task Act()
					=> await That(subject).IsValidJson(d => d.Matches([1, 2,]))
						.And.HasLength().GreaterThan(6);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is valid JSON which matches [1, 2,] and has length greater than 6,
					             but it had length 6
					             """);
			}

			[Test]
			public async Task WhenExpectationFails_ShouldFail()
			{
				string subject = "[1, 2]";

				async Task Act()
					=> await That(subject).IsValidJson(d => d.Matches([1, 3,]));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is valid JSON which matches [1, 3,],
					             but it differed as $[1] was 2 instead of 3

					             JSON options (it):
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenExpectationIsSatisfied_ShouldSucceed()
			{
				string subject = "[1, 2]";

				async Task Act()
					=> await That(subject).IsValidJson(d => d.Matches([1, 2,]));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectationsAreNull_ShouldThrowArgumentNullException()
			{
				string subject = "[1, 2]";

				async Task Act()
					=> await That(subject).IsValidJson(expectations: null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectations").And
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenNegatedAndExpectationFails_ShouldSucceed()
			{
				string subject = "[1, 2]";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsValidJson(d => d.Matches([1, 3,])));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNegatedAndExpectationIsSatisfied_ShouldFail()
			{
				string subject = "[1, 2]";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsValidJson(d => d.Matches([1, 2,])));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not valid JSON which matches [1, 2,],
					             but it was "[1, 2]"
					             """);
			}

			[Test]
			[Arguments(true)]
			[Arguments(false)]
			public async Task WhenOptionsAreProvided_ShouldParseWithThem(bool allowTrailingCommas)
			{
				string subject = "[1, 2,]";

				async Task Act()
					=> await That(subject).IsValidJson(d => d.Matches([1, 2,]), o => o with
					{
						AllowTrailingCommas = allowTrailingCommas,
					});

				await That(Act).Throws<FailException>().OnlyIf(!allowTrailingCommas)
					.WithMessage("""
					             Expected that subject
					             is valid JSON which matches [1, 2,],
					             but it could not be parsed: The JSON array contains a trailing comma at the end which is not supported in this mode. Change the reader options. LineNumber: 0 | BytePositionInLine: 6. and it did throw a JsonReaderException:
					               The JSON array contains a trailing comma at the end which is not supported in this mode. Change the reader options. LineNumber: 0 | BytePositionInLine: 6.
					             """);
			}

			[Test]
			public async Task WhenRootIsJsonNull_ShouldProvideNonNullableElement()
			{
				string subject = "null";

				async Task Act()
					=> await That(subject).IsValidJson(d => d.IsNull());

				await That(Act).DoesNotThrow()
					.Because("the root element of a parsed document always exists, so it is a non-nullable JsonElement that offers IsNull for the JSON null");
			}
		}

		public sealed class OverloadResolutionTests
		{
			[Test]
			public async Task WhenLambdaCreatesOptions_ShouldBindToOptions()
			{
				string subject = "[1, 2,]";

				async Task Act()
					=> await That(subject).IsValidJson(_ => new JsonDocumentOptions
					{
						AllowTrailingCommas = true,
					});

				await That(Act).DoesNotThrow()
					.Because("an object creation is also a valid statement for the expectations, but a lambda with a return value binds better to the options");
			}

			[Test]
			public async Task WhenLambdaReturnsModifiedOptions_ShouldBindToOptions()
			{
				string subject = "[1, 2,]";

				async Task Act()
					=> await That(subject).IsValidJson(o => o with
					{
						AllowTrailingCommas = true,
					});

				await That(Act).DoesNotThrow()
					.Because("a with expression only compiles for the options");
			}

			[Test]
			public async Task WhenLambdaUsesExpectation_ShouldBindToExpectations()
			{
				string subject = "[1]";

				async Task Act()
					=> await That(subject).IsValidJson(j => j.IsObject());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is valid JSON which is an object,
					             but it was an array
					             """)
					.Because("an expectation only compiles on the subject");
			}

			[Test]
			public async Task WhenMethodGroupReturnsOptions_ShouldBindToOptions()
			{
				string subject = "[1, 2,]";

				async Task Act()
					=> await That(subject).IsValidJson(AllowTrailingCommas);

				await That(Act).DoesNotThrow()
					.Because("the method group only converts to the options");

				static JsonDocumentOptions AllowTrailingCommas(JsonDocumentOptions options)
					=> options with
					{
						AllowTrailingCommas = true,
					};
			}

			[Test]
			public async Task WhenMethodGroupTakesSubject_ShouldBindToExpectations()
			{
				string subject = "[1]";

				async Task Act()
					=> await That(subject).IsValidJson(IsAnObject);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is valid JSON which is an object,
					             but it was an array
					             """)
					.Because("the method group only converts to the expectations");

				static void IsAnObject(IThatSubject<JsonElement> subject)
					=> subject.IsObject();
			}

			[Test]
			public async Task WhenOptionsAreNamedNull_ShouldBindToOptions()
			{
				string subject = "[1]";

				async Task Act()
					=> await That(subject).IsValidJson(options: null);

				await That(Act).DoesNotThrow()
					.Because("an unnamed null is ambiguous, but a named null binds to the overload without expectations");
			}
		}
	}

	public sealed class IsNotValidJson
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("")]
			[Arguments(" \t\r\n")]
			[Arguments("True")]
			[Arguments("{\"foo\":1}}")]
			[Arguments("{}{\"foo\":1}")]
			[Arguments("[")]
			[Arguments("[1, 2")]
			[Arguments("{\"foo\" 1}")]
			public async Task WhenActualIsInvalidJson_ShouldSucceed(string subject)
			{
				async Task Act()
					=> await That(subject).IsNotValidJson();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("42")]
			[Arguments("true")]
			[Arguments("null")]
			[Arguments("[]")]
			[Arguments("[1, 2]")]
			[Arguments("{}")]
			public async Task WhenActualIsValidJson_ShouldFail(string subject)
			{
				async Task Act()
					=> await That(subject).IsNotValidJson();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not valid JSON,
					              but it was "{subject}"
					              """);
			}

			[Test]
			public async Task WhenCombinedWithAnd_ShouldApplyFollowingExpectationToString()
			{
				string subject = "[1, 2";

				async Task Act()
					=> await That(subject).IsNotValidJson().And.HasLength().GreaterThan(5);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not valid JSON and has length greater than 5,
					             but it had length 5
					             """);
			}

			[Test]
			public async Task WhenItemsArePlural_ShouldUsePluralVerb()
			{
				string subject = "{\n[1]";

				async Task Act()
					=> await That(subject).HasLines(l => l.All().ComplyWith(it => it.IsNotValidJson()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has lines of which all are not valid JSON,
					             *
					             """).AsWildcard()
					.Because("the lines are the plural subject of the expectation");
			}

			[Test]
			public async Task WhenNegatedAndActualIsInvalidJson_ShouldFail()
			{
				string subject = "";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotValidJson());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is valid JSON,
					             but it could not be parsed: The input does not contain any JSON tokens.*
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenNegatedAndSubjectIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotValidJson());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is valid JSON,
					             but it was <null>
					             """);
			}

			[Test]
			[Arguments(true)]
			[Arguments(false)]
			public async Task WhenOptionsAreProvided_ShouldParseWithThem(bool allowTrailingCommas)
			{
				string subject = "[1, 2,]";

				async Task Act()
					=> await That(subject).IsNotValidJson(o => o with
					{
						AllowTrailingCommas = allowTrailingCommas,
					});

				await That(Act).Throws<FailException>().OnlyIf(allowTrailingCommas)
					.WithMessage("""
					             Expected that subject
					             is not valid JSON,
					             but it was "[1, 2,]"
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsNotValidJson();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not valid JSON,
					             but it was <null>
					             """);
			}
		}
	}
}
