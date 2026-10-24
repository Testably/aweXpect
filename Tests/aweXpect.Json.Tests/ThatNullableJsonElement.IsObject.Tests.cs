using System.Text.Json;
using aweXpect.Equivalency;

namespace aweXpect.Json.Tests;

public sealed partial class ThatNullableJsonElement
{
	public sealed class IsObject
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldReturnTheSubject()
			{
				JsonElement? subject = FromString("{\"foo\": 1}");

				JsonElement result = await That(subject).IsObject();

				await That(result.GetRawText()).IsEqualTo("{\"foo\": 1}");
			}

			[Test]
			public async Task WhenExpectationIsNull_ShouldThrowArgumentNullException()
			{
				JsonElement? subject = FromString("{}");

				async Task Act()
					=> await That(subject).IsObject(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectations").And
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenExpectationThrows_ShouldFail()
			{
				JsonElement? subject = FromString("{}");

				async Task Act()
					=> await That(subject).IsObject(_ => throw new InvalidOperationException("Yesterday"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object,
					             but the expectation did throw an InvalidOperationException:
					               Yesterday

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			[Arguments("{}")]
			[Arguments("{\"foo\": 1}")]
			public async Task WhenJsonIsAnObject_ShouldSucceed(string json)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("[]", "an array")]
			[Arguments("2", "a number")]
			[Arguments("\"foo\"", "a string")]
			public async Task WhenJsonIsNoObject_ShouldFail(string json, string kindString)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is an object,
					              but it was {kindString}
					              """);
			}

			[Test]
			[Arguments("[]", "an array")]
			[Arguments("2", "a number")]
			[Arguments("\"foo\"", "a string")]
			public async Task WhenJsonIsNoObject_WithExpectations_ShouldFail(string json, string kindString)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject(
						o => o.With("foo").Matching(true),
						o => o.IgnoringAdditionalProperties());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is an object and $.foo matches true,
					              but it was {kindString}

					              JSON options:
					               - ignore additional properties
					              """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).IsObject();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_WithExpectations_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").Matching(true));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo matches true,
					             but it was <null>

					             JSON options:
					              - ignore additional properties
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task IsObject_ShouldBeChainable()
			{
				string json = """
				              {
				                "foo": 21,
				                "bar": []
				              }
				              """;
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsObject(o
							=> o.With(2).Properties().And.With("foo").Matching(21).And.With("bar")
								.AnArray(a => a.With(0).Elements())));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an object or not with 2 properties or $.foo does not match 21 or $.bar is not an array or not with 0 elements,
					             but it was

					             Actual:
					             {
					               "foo": 21,
					               "bar": []
					             }

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			[Arguments("{}")]
			[Arguments("{\"foo\": 1}")]
			public async Task WhenJsonIsAnObject_ShouldFail(string json)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsObject());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an object,
					             but it was an object
					             """);
			}

			[Test]
			[Arguments("[]")]
			[Arguments("2")]
			[Arguments("\"foo\"")]
			public async Task WhenJsonIsNoObject_ShouldSucceed(string json)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsObject());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenValueAtPathDiffers_ShouldSucceed()
			{
				JsonElement? subject = FromString("{\"tracks\": [{\"title\": \"Come Together\"}]}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsObject(o => o.At("tracks[0].title").Matching("Something")));

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class AtTests
		{
			[Test]
			public async Task WhenValueAtPathMatches_ShouldSucceed()
			{
				JsonElement? subject = FromString("{\"tracks\": [{\"title\": \"Come Together\"}]}");

				async Task Act()
					=> await That(subject).IsObject(o => o.At("tracks[0].title").Matching("Come Together"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenIntermediatePropertyDoesNotExist_ShouldFail()
			{
				JsonElement? subject = FromString("{\"tracks\": [{\"title\": \"Come Together\"}]}");

				async Task Act()
					=> await That(subject).IsObject(o => o.At("artist.origin.city").Matching("Liverpool"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.artist.origin.city matches "Liverpool",
					             but it differed as property $.artist did not exist

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).IsObject(o => o.At("tracks[0].title").AString());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.tracks[0].title is a string,
					             but it was <null>

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenPathIsInvalid_ShouldThrowArgumentException()
			{
				JsonElement? subject = FromString("{\"foo\": 1}");

				async Task Act()
					=> await That(subject).IsObject(o => o.At("foo[").Matching(1));

				await That(Act).Throws<ArgumentException>()
					.WithParamName("path").And
					.WithMessage("The path 'foo[' is invalid: the '[' at position 3 is not closed.").AsPrefix()
					.Because("an invalid argument is thrown as it is instead of failing the expectation");
			}
		}

		public sealed class WithTests
		{
			[Test]
			public async Task WhenMatchingObjectWithIgnoredMember_ShouldIgnoreItInTheComparison()
			{
				JsonElement? subject = FromString("{\"foo\": {\"id\": 1, \"name\": \"a\"}}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").Matching(new
					{
						id = 2,
						name = "a",
					}), o => o.IgnoringMember("id"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMatchFails_ShouldFail()
			{
				JsonElement? subject = FromString("{\"foo\": 1}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").Matching(2));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo matches 2,
					             but it differed as $.foo was 1 instead of 2

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenMatchSucceeds_ShouldSucceed()
			{
				JsonElement? subject = FromString("{\"foo\": 2}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").Matching(2));


				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMatchingItIsWaits_ShouldNotBlockTheCaller()
			{
				JsonElement? subject = FromString("{\"foo\": \"bar\"}");
				bool isSatisfied = false;

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo")
						.Matching(It.Is<string>().That.Satisfies(_ => isSatisfied).Within(TimeSpan.FromSeconds(5))));

				Task evaluation = Act();
				isSatisfied = true;

				await That(() => evaluation).DoesNotThrow()
					.Because("the evaluation must return to the caller while it waits instead of blocking it");
			}

			[Test]
			public async Task WhenMultipleMatchesFail_ShouldListAllFailures()
			{
				JsonElement? subject = FromString("{\"foo\": 1, \"bar\": 2}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").Matching(2).With("bar").Matching(1));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo matches 2 and $.bar matches 1,
					             but it differed as
					               $.foo was 1 instead of 2 and
					               $.bar was 2 instead of 1

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenNestedMatchesFail_ShouldListAllFailures()
			{
				JsonElement? subject = FromString("{\"foo\": 1, \"bar\": {\"baz\": 1, \"bat\": 2}}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With("foo").Matching(2).And
						.With("bar").AnObject(p => p
							.With("baz").Matching(3).And
							.With("bat").Matching(3)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo matches 2 and $.bar is an object and $.bar.baz matches 3 and $.bar.bat matches 3,
					             but it differed as
					               $.foo was 1 instead of 2 and
					               $.bar.baz was 1 instead of 3 and
					               $.bar.bat was 2 instead of 3

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenPropertyDoesNotExist_ShouldFail()
			{
				JsonElement? subject = FromString("{\"foo\": 1}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("bar").Matching(true));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.bar matches true,
					             but it differed as property $.bar did not exist

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenMatchingNullOnMissingProperty_WhenTreatingNullAsAbsent_ShouldSucceed()
			{
				JsonElement? subject = FromString("{\"foo\": 1}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("bar").Matching(null), o => o.TreatingNullAsAbsent());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMatchingValueOnNullProperty_WhenTreatingNullAsAbsent_ShouldFail()
			{
				JsonElement? subject = FromString("{\"bar\": null}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("bar").Matching(true), o => o.TreatingNullAsAbsent());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.bar matches true,
					             but it differed as property $.bar did not exist

					             JSON options:
					              - ignore additional properties
					              - treat null as absent
					             """);
			}
		}

		public sealed class WithValueKindTests
		{
			[Test]
			public async Task WhenPropertiesHaveTheExpectedValueKinds_ShouldSucceed()
			{
				JsonElement? subject = FromString("{\"id\": 42, \"name\": \"foo\", \"active\": true, \"deletedAt\": null}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With("id").ANumber().And
						.With("name").AString().And
						.With("active").ABoolean().And
						.With("deletedAt").Null());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertiesHaveOtherValueKinds_ShouldListAllFailuresInOrder()
			{
				JsonElement? subject = FromString("{\"id\": \"42\", \"name\": null, \"active\": 1, \"deletedAt\": {}}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With("id").ANumber().And
						.With("name").AString().And
						.With("active").ABoolean().And
						.With("deletedAt").Null());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.id is a number and $.name is a string and $.active is a boolean and $.deletedAt is null,
					             but it differed as
					               $.id was a string and
					               $.name was null and
					               $.active was a number and
					               $.deletedAt was an object

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").AString());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo is a string,
					             but it was <null>

					             JSON options:
					              - ignore additional properties
					             """);
			}
		}

		public sealed class WithNumberOfPropertiesTests
		{
			[Test]
			public async Task WhenNull_ShouldNotAddFailureMessage()
			{
				JsonElement? subject = FromString("{}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With("foo").AnObject(a => a
							.With(1).Properties()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo is an object with 1 property,
					             but it differed as property $.foo did not exist

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenNumberDiffers_ShouldFail()
			{
				JsonElement? subject = FromString("{\"foo\": 1, \"bar\": 2}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With(3).Properties());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object with 3 properties,
					             but it differed as $ had 2 properties

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			[Arguments("{}", 0)]
			[Arguments("{\"foo\": 1}", 1)]
			[Arguments("{\"foo\": 1, \"bar\": 2, \"baz\": 3}", 3)]
			public async Task WhenNumberMatches_ShouldSucceed(string json, int expected)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With(expected).Properties());

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WithoutTests
		{
			[Test]
			public async Task WhenPropertyDoesNotExist_ShouldSucceed()
			{
				JsonElement? subject = FromString("{\"foo\": 1}");

				async Task Act()
					=> await That(subject).IsObject(o => o.Without("bar"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyExists_ShouldFail()
			{
				JsonElement? subject = FromString("{\"foo\": 1, \"bar\": \"baz\"}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").Matching(1).And.Without("bar"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo matches 1 and $.bar does not exist,
					             but it differed as $.bar was "baz"

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				JsonElement? subject = FromString("{\"foo\": null}");

				async Task Act()
					=> await That(subject).IsObject(o => o.Without("foo"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo does not exist,
					             but it differed as $.foo was <null>

					             JSON options:
					              - ignore additional properties
					             """)
					.Because("a property with a null value exists");
			}

			[Test]
			public async Task WhenPropertyIsNull_WhenTreatingNullAsAbsent_ShouldSucceed()
			{
				JsonElement? subject = FromString("{\"foo\": null}");

				async Task Act()
					=> await That(subject).IsObject(o => o.Without("foo"), o => o.TreatingNullAsAbsent());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).IsObject(o => o.Without("foo"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo does not exist,
					             but it was <null>

					             JSON options:
					              - ignore additional properties
					             """);
			}
		}

		public sealed class WhosePropertiesTests
		{
			[Test]
			public async Task All_WhenAllValuesAreStrings_ShouldSucceed()
			{
				JsonElement? subject = FromString("{\"artist\": \"The Beatles\", \"title\": \"Hey Jude\"}");

				async Task Act()
					=> await That(subject).IsObject().WhoseProperties
						.All().Satisfy(p => p.Value.ValueKind == JsonValueKind.String);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task All_WhenSomeValuesAreNoStrings_ShouldFail()
			{
				JsonElement? subject = FromString("{\"title\": \"Hey Jude\", \"durationSeconds\": 431}");

				async Task Act()
					=> await That(subject).IsObject().WhoseProperties
						.All().Satisfy(p => p.Value.ValueKind == JsonValueKind.String);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object whose properties satisfy p => p.Value.ValueKind == JsonValueKind.String for all items,
					             but only 1 of 2 did

					             Not matching items (properties):
					             [
					               "durationSeconds":431
					             ]

					             Collection (properties):
					             [
					               "title":"Hey Jude",
					               "durationSeconds":431
					             ]
					             """);
			}

			[Test]
			[Arguments("[]", "an array")]
			[Arguments("2", "a number")]
			[Arguments("\"foo\"", "a string")]
			public async Task WhenJsonIsNoObject_ShouldOnlyFailWithIsObject(string json, string kindString)
			{
				JsonElement? subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject().WhoseProperties.HasCount(2);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is an object whose properties have exactly 2 items,
					              but it was {kindString}
					              """)
					.Because("the properties are only accessed when the subject is an object");
			}

			[Test]
			public async Task WhenNamesAreDuplicated_ShouldKeepAllProperties()
			{
				JsonElement? subject = FromString("{\"a\": 1, \"a\": 2}");

				async Task Act()
					=> await That(subject).IsObject().WhoseProperties.All().Satisfy(p => p.Value.GetInt32() == 1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object whose properties satisfy p => p.Value.GetInt32() == 1 for all items,
					             but only 1 of 2 did

					             Not matching items (properties):
					             [
					               "a":2
					             ]

					             Collection (properties):
					             [
					               "a":1,
					               "a":2
					             ]
					             """)
					.Because("the properties keep a duplicate name in the order of the document");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldOnlyFailWithIsObject()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).IsObject().WhoseProperties.HasCount(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object whose properties have exactly 2 items,
					             but it was <null>
					             """)
					.Because("the properties are only accessed when the subject is an object");
			}

			[Test]
			public async Task WithExpectation_WhenSubjectIsNull_ShouldOnlyFailWithIsObject()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).IsObject(o => o.With(2).Properties())
						.WhoseProperties.Contains(p => p.Name == "a");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object with 2 properties whose properties contain an item matching p => p.Name == "a" at least once,
					             but it was <null>

					             JSON options:
					              - ignore additional properties
					             """);
			}
		}

		public sealed class WhosePropertiesNegatedTests
		{
			[Test]
			public async Task WhenPropertiesMeetTheExpectation_ShouldFail()
			{
				JsonElement? subject = FromString("{\"a\": 1, \"b\": 2}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsObject().WhoseProperties.HasCount(2));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an object whose properties have exactly 2 items,
					             but it was an object
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonElement? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsObject().WhoseProperties.HasCount(2));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an object whose properties have exactly 2 items,
					             but it was <null>
					             """)
					.Because("a null subject fails IsObject also when it is negated");
			}
		}
	}
}
