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

		public sealed class WhichTests
		{
			[Test]
			public async Task WhenActualIsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsValidJson().Which(d => d.Matches([1, 2,]));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is valid JSON which matches [1, 2,],
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenExpectationInWhichFails_ShouldFail()
			{
				string subject = "[1, 2]";

				async Task Act()
					=> await That(subject).IsValidJson().Which(d => d.Matches([1, 3,]));

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
			public async Task WhenExpectationInWhichIsSatisfied_ShouldSucceed()
			{
				string subject = "[1, 2]";

				async Task Act()
					=> await That(subject).IsValidJson().Which(d => d.Matches([1, 2,]));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectationsAreNull_ShouldThrowArgumentNullException()
			{
				string subject = "[1, 2]";

				async Task Act()
					=> await That(subject).IsValidJson().Which(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectations").And
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}
		}
	}
}
