using System.Text.Json;
using aweXpect.Equivalency;

namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonElement
{
	public sealed class IsObject
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenExpectationIsNull_ShouldThrowArgumentNullException()
			{
				JsonElement subject = FromString("{}");

				async Task Act()
					=> await That(subject).IsObject(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectations").And
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenExpectationThrows_ShouldFail()
			{
				JsonElement subject = FromString("{}");

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
			public async Task WhenNestedExpectationThrows_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": {}}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With("foo").AnObject(_ => throw new InvalidOperationException("Yesterday")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo is an object,
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
				JsonElement subject = FromString(json);

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
				JsonElement subject = FromString(json);

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
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").Matching(true));

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
			public async Task WhenMemberIsPlural_WithExpectation_ShouldUsePluralFormInResult()
			{
				JsonElement subject = FromString("[]");

				async Task Act()
					=> await WhosePluralItems(That(subject), items => items.IsObject(o => o.With("foo").Matching(1)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             whose Items are objects and $.foo matches 1,
					             but Items were an array

					             JSON options (Items):
					              - ignore additional properties
					             """)
					.Because("the plural member is the subject of the result");
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenPropertyIsExpectedToBeAnArrayWithoutExpectation_ShouldNegateIt()
			{
				JsonElement subject = FromString("{\"bar\": []}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsObject(o => o.With("bar").AnArray()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an object or $.bar is not an array,
					             but it was

					             Actual:
					             {
					               "bar": []
					             }

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task IsObject_ShouldBeChainable()
			{
				string json = """
				              {
				                "foo": 21,
				                "bar": []
				              }
				              """;
				JsonElement subject = FromString(json);

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
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsObject());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an object,
					             but it was
					             """);
			}

			[Test]
			public async Task WhenExpectationThrows_ShouldFail()
			{
				JsonElement subject = FromString("{}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsObject(_ => throw new InvalidOperationException("Yesterday")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an object,
					             but the expectation did throw an InvalidOperationException:
					               Yesterday

					             JSON options:
					              - ignore additional properties
					             """)
					.Because("an expectation that throws answered nothing, so its negation must not be met either");
			}

			[Test]
			[Arguments("[]")]
			[Arguments("2")]
			[Arguments("\"foo\"")]
			public async Task WhenJsonIsNoObject_ShouldSucceed(string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsObject());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertiesAreExpectedToHaveValueKinds_ShouldNegateThem()
			{
				JsonElement subject = FromString("{\"id\": 42, \"name\": \"foo\", \"active\": false, \"deletedAt\": null}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsObject(o => o
						.With("id").ANumber().And
						.With("name").AString().And
						.With("active").ABoolean().And
						.With("deletedAt").Null()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an object or $.id is not a number or $.name is not a string or $.active is not a boolean or $.deletedAt is not null,
					             but it was

					             Actual:
					             {
					               "id": 42,
					               "name": "foo",
					               "active": false,
					               "deletedAt": null
					             }

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenPropertyHasAnotherValueKind_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"id\": 42, \"name\": \"foo\"}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsObject(o => o
						.With("id").ANumber().And
						.With("name").Null()));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyIsExpectedNotToExistAndDoesNotExist_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": 1}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsObject(o => o.Without("bar")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an object or $.bar exists,
					             but it was

					             Actual:
					             {
					               "foo": 1
					             }

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenPropertyIsExpectedNotToExistButExists_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"foo\": 1}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsObject(o => o.Without("foo")));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyIsExpectedNotToExistButIsNull_WhenTreatingNullAsAbsent_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": null}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsObject(o => o.Without("foo"),
						o => o.TreatingNullAsAbsent()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an object or $.foo exists,
					             but it was

					             Actual:
					             {
					               "foo": null
					             }

					             JSON options:
					              - ignore additional properties
					              - treat null as absent
					             """);
			}

			[Test]
			public async Task WhenPropertyIsExpectedToMatchNullButIsMissing_WhenTreatingNullAsAbsent_ShouldFail()
			{
				JsonElement subject = FromString("{}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsObject(o => o.With("foo").Matching(null),
						o => o.TreatingNullAsAbsent()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an object or $.foo does not match null,
					             but it was

					             Actual:
					             {}

					             JSON options:
					              - ignore additional properties
					              - treat null as absent
					             """);
			}

			[Test]
			public async Task WhenValueAtPathMatches_ShouldFail()
			{
				JsonElement subject = FromString("{\"artist\": {\"origin\": {\"city\": \"Liverpool\"}}}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsObject(o => o
						.At("artist.origin.city").Matching("Liverpool").And
						.At("artist.origin").AnObject()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an object or $.artist.origin.city does not match "Liverpool" or $.artist.origin is not an object,
					             but it was

					             Actual:
					             {
					               "artist": {
					                 "origin": {
					                   "city": "Liverpool"
					                 }
					               }
					             }

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			[Arguments("{\"artist\": {\"origin\": {\"city\": \"London\"}}}")]
			[Arguments("{\"artist\": {\"origin\": {}}}")]
			[Arguments("{\"artist\": {}}")]
			[Arguments("{\"artist\": \"Liverpool\"}")]
			public async Task WhenValueAtPathDiffersOrCannotBeReached_ShouldSucceed(string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsObject(o => o.At("artist.origin.city").Matching("Liverpool")));

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class AtTests
		{
			[Test]
			public async Task WhenPathAddressesNestedProperty_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"artist\": {\"origin\": {\"city\": \"Liverpool\"}}}");

				async Task Act()
					=> await That(subject).IsObject(o => o.At("artist.origin.city").Matching("Liverpool"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPathContainsIndexes_ShouldSucceed()
			{
				JsonElement subject = FromString(
					"{\"tracks\": [{\"title\": \"Come Together\"}, {\"title\": \"Something\", \"writers\": [\"Lennon\", \"Harrison\"]}], \"sides\": [[1, 2], [3, 4]]}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.At("tracks[1].title").Matching("Something").And
						.At("tracks[1].writers[1]").Matching("Harrison").And
						.At("sides[1][0]").Matching(3));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyNameContainsOtherSpecialCharacters_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"$label\": {\"record company\": {\"catalog-no*\": 1}}}");

				async Task Act()
					=> await That(subject).IsObject(o => o.At("$label.record company.catalog-no*").Matching(1));

				await That(Act).DoesNotThrow()
					.Because("only '.', '[' and ']' delimit the segments of a path");
			}

			[Test]
			public async Task WhenMatchFails_ShouldFailWithTheFullPath()
			{
				JsonElement subject = FromString("{\"artist\": {\"origin\": {\"city\": \"London\"}}}");

				async Task Act()
					=> await That(subject).IsObject(o => o.At("artist.origin.city").Matching("Liverpool"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.artist.origin.city matches "Liverpool",
					             but it differed as $.artist.origin.city was "London" instead of "Liverpool"

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenMatchingObject_ShouldCompareTheValueAtThePath()
			{
				JsonElement subject = FromString("{\"tracks\": [{\"title\": \"Come Together\", \"durationSeconds\": 259}]}");

				async Task Act()
					=> await That(subject).IsObject(o => o.At("tracks[0]").Matching(new
					{
						title = "Come Together",
						durationSeconds = 260,
					}));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.tracks[0] matches new
					             {
					             	title = "Come Together",
					             	durationSeconds = 260,
					             },
					             but it differed as $.tracks[0].durationSeconds was 259 instead of 260

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			[Arguments("true")]
			[Arguments("false")]
			public async Task WhenValuesHaveTheExpectedValueKinds_ShouldSucceed(string remastered)
			{
				JsonElement subject = FromString(
					$"{{\"album\": {{\"year\": 1969, \"title\": \"Abbey Road\", \"remastered\": {remastered}, \"producer\": null, \"tracks\": [], \"label\": {{}}}}}}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.At("album.year").ANumber().And
						.At("album.title").AString().And
						.At("album.remastered").ABoolean().And
						.At("album.producer").Null().And
						.At("album.tracks").AnArray().And
						.At("album.label").AnObject());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenValuesHaveOtherValueKinds_ShouldListAllFailuresInOrder()
			{
				JsonElement subject = FromString(
					"{\"album\": {\"year\": \"1969\", \"title\": 1, \"remastered\": \"yes\", \"producer\": false, \"tracks\": {}, \"label\": []}}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.At("album.year").ANumber().And
						.At("album.title").AString().And
						.At("album.remastered").ABoolean().And
						.At("album.producer").Null().And
						.At("album.tracks").AnArray().And
						.At("album.label").AnObject());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.album.year is a number and $.album.title is a string and $.album.remastered is a boolean and $.album.producer is null and $.album.tracks is an array and $.album.label is an object,
					             but it differed as
					               $.album.year was a string and
					               $.album.title was a number and
					               $.album.remastered was a string and
					               $.album.producer was false and
					               $.album.tracks was an object and
					               $.album.label was an array

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenNestedExpectationsFail_ShouldReportTheFullPaths()
			{
				JsonElement subject = FromString(
					"{\"album\": {\"year\": 1969, \"artist\": {\"city\": \"London\", \"members\": [{\"born\": 1940}]}}}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.At("album.year").Matching(1970).And
						.With("album").AnObject(album => album
							.At("artist.city").Matching("Liverpool").And
							.At("artist").AnObject(a => a.At("members[0].born").Matching(1941)).And
							.At("artist.members").AnArray(m => m.At(0).AnObject(e => e.At("born").Matching(1942)))));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.album.year matches 1970 and $.album is an object and $.album.artist.city matches "Liverpool" and $.album.artist is an object and $.album.artist.members[0].born matches 1941 and $.album.artist.members is an array and $.album.artist.members[0] is an object and $.album.artist.members[0].born matches 1942,
					             but it differed as
					               $.album.year was 1969 instead of 1970 and
					               $.album.artist.city was "London" instead of "Liverpool" and
					               $.album.artist.members[0].born was 1940 instead of 1941 and
					               $.album.artist.members[0].born was 1940 instead of 1942

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenCombinedWithWithAndWithout_ShouldListAllFailuresInOrder()
			{
				JsonElement subject = FromString(
					"{\"year\": 1969, \"artist\": {\"name\": \"The Beatles\", \"label\": \"Apple\"}}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With("year").Matching(1970).And
						.At("artist.name").Matching("Wings").And
						.Without("artist").And
						.With("artist").AnObject(a => a.Without("label")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.year matches 1970 and $.artist.name matches "Wings" and $.artist does not exist and $.artist is an object and $.artist.label does not exist,
					             but it differed as
					               $.year was 1969 instead of 1970 and
					               $.artist.name was "The Beatles" instead of "Wings" and
					               $.artist was {"name":"The Beatles","label":"Apple"} and
					               $.artist.label was "Apple"

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenFinalPropertyDoesNotExist_ShouldFail()
			{
				JsonElement subject = FromString("{\"artist\": {\"origin\": {}}}");

				async Task Act()
					=> await That(subject).IsObject(o => o.At("artist.origin.city").AString());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.artist.origin.city is a string,
					             but it differed as property $.artist.origin.city did not exist

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenIntermediatePropertyDoesNotExist_ShouldOnlyReportTheFirstMissingProperty()
			{
				JsonElement subject = FromString("{\"artist\": {}}");

				async Task Act()
					=> await That(subject).IsObject(o => o.At("artist.origin.country.name").Matching("England"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.artist.origin.country.name matches "England",
					             but it differed as property $.artist.origin did not exist

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenIntermediatePropertyDoesNotExist_ShouldNotAffectOtherPaths()
			{
				JsonElement album = FromString("{\"artist\": {}, \"tracks\": [{\"title\": \"Come Together\"}]}");

				async Task Act()
					=> await That(album).IsObject(o => o
						.At("artist.origin.city").Matching("Liverpool").And
						.At("tracks[0].title").AString());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that album
					             is an object and $.artist.origin.city matches "Liverpool" and $.tracks[0].title is a string,
					             but it differed as property $.artist.origin did not exist

					             JSON options:
					              - ignore additional properties
					             """)
					.Because("the documentation shows this failure message");
			}

			[Test]
			[Arguments("\"Liverpool\"", "a string")]
			[Arguments("[]", "an array")]
			[Arguments("1", "a number")]
			[Arguments("true", "true")]
			[Arguments("null", "null")]
			public async Task WhenIntermediateValueIsNoObject_ShouldFail(string value, string kindString)
			{
				JsonElement subject = FromString($"{{\"artist\": {{\"origin\": {value}}}}}");

				async Task Act()
					=> await That(subject).IsObject(o => o.At("artist.origin.country.name").Matching("England"));

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is an object and $.artist.origin.country.name matches "England",
					              but it differed as $.artist.origin was {kindString}

					              JSON options:
					               - ignore additional properties
					              """);
			}

			[Test]
			[Arguments("{}", "an object")]
			[Arguments("\"Come Together\"", "a string")]
			[Arguments("null", "null")]
			public async Task WhenIntermediateValueIsNoArray_ShouldFail(string value, string kindString)
			{
				JsonElement subject = FromString($"{{\"tracks\": {value}}}");

				async Task Act()
					=> await That(subject).IsObject(o => o.At("tracks[0].title").AString());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is an object and $.tracks[0].title is a string,
					              but it differed as $.tracks was {kindString}

					              JSON options:
					               - ignore additional properties
					              """);
			}

			[Test]
			public async Task WhenIntermediateIndexDoesNotExist_ShouldFail()
			{
				JsonElement subject = FromString("{\"tracks\": [{\"title\": \"Come Together\"}]}");

				async Task Act()
					=> await That(subject).IsObject(o => o.At("tracks[3].title").AString());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.tracks[3].title is a string,
					             but it differed as index $.tracks[3] did not exist

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenFinalIndexDoesNotExist_ShouldFail()
			{
				JsonElement subject = FromString("{\"tracks\": [{\"writers\": [\"Lennon\"]}]}");

				async Task Act()
					=> await That(subject).IsObject(o => o.At("tracks[0].writers[1]").Matching("McCartney"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.tracks[0].writers[1] matches "McCartney",
					             but it differed as index $.tracks[0].writers[1] did not exist

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenMatchingNullOnMissingProperty_ShouldFail()
			{
				JsonElement subject = FromString("{\"album\": {}}");

				async Task Act()
					=> await That(subject).IsObject(o => o.At("album.producer").Matching(null));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.album.producer matches null,
					             but it differed as property $.album.producer did not exist

					             JSON options:
					              - ignore additional properties
					             """)
					.Because("by default a missing property differs from a property with a null value");
			}

			[Test]
			[Arguments("{\"album\": {\"producer\": null}}")]
			[Arguments("{\"album\": {}}")]
			public async Task WhenMatchingNullOrNull_WhenTreatingNullAsAbsent_ShouldSucceed(string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject(o => o
							.At("album.producer").Matching(null).And
							.At("album.producer").Null(),
						o => o.TreatingNullAsAbsent());

				await That(Act).DoesNotThrow()
					.Because("an absent final property is accepted when it is expected to be null");
			}

			[Test]
			[Arguments("{\"album\": {\"title\": null}}")]
			[Arguments("{\"album\": {}}")]
			public async Task WhenFinalPropertyIsAbsent_WhenTreatingNullAsAbsent_ShouldFail(string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject(o => o.At("album.title").AString(),
						o => o.TreatingNullAsAbsent());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.album.title is a string,
					             but it differed as property $.album.title did not exist

					             JSON options:
					              - ignore additional properties
					              - treat null as absent
					             """);
			}

			[Test]
			[Arguments("{\"album\": null}")]
			[Arguments("{}")]
			public async Task WhenIntermediatePropertyIsAbsent_WhenTreatingNullAsAbsent_ShouldFailEvenWhenMatchingNull(
				string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject(o => o.At("album.producer").Matching(null),
						o => o.TreatingNullAsAbsent());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.album.producer matches null,
					             but it differed as property $.album did not exist

					             JSON options:
					              - ignore additional properties
					              - treat null as absent
					             """)
					.Because("only the final value is accepted to be absent, as with nested builders");
			}

			[Test]
			public async Task WhenIntermediateArrayElementIsNull_WhenTreatingNullAsAbsent_ShouldReportItsKind()
			{
				JsonElement subject = FromString("{\"tracks\": [null]}");

				async Task Act()
					=> await That(subject).IsObject(o => o.At("tracks[0].title").Matching(null),
						o => o.TreatingNullAsAbsent());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.tracks[0].title matches null,
					             but it differed as $.tracks[0] was null

					             JSON options:
					              - ignore additional properties
					              - treat null as absent
					             """)
					.Because("an array element is never absent, as with At(int)");
			}

			[Test]
			[Arguments("{}")]
			[Arguments("{\"album\": null}")]
			[Arguments("{\"album\": 1}")]
			[Arguments("{\"album\": {}}")]
			[Arguments("{\"album\": {\"tracks\": null}}")]
			[Arguments("{\"album\": {\"tracks\": {}}}")]
			[Arguments("{\"album\": {\"tracks\": []}}")]
			[Arguments("{\"album\": {\"tracks\": [null]}}")]
			[Arguments("{\"album\": {\"tracks\": [[]]}}")]
			[Arguments("{\"album\": {\"tracks\": [{}]}}")]
			[Arguments("{\"album\": {\"tracks\": [{\"durationSeconds\": null}]}}")]
			[Arguments("{\"album\": {\"tracks\": [{\"durationSeconds\": 2}]}}")]
			[Arguments("{\"album\": {\"tracks\": [{\"durationSeconds\": 1}]}}")]
			public async Task ShouldVerifyTheSameAsTheEquivalentNestedBuilders(string json)
			{
				JsonElement subject = FromString(json);
				(bool TreatNullAsAbsent, object? Expected)[] variants = [(false, 1), (false, null), (true, 1), (true, null),];

				foreach ((bool treatNullAsAbsent, object? expected) in variants)
				{
					Exception? pathException = await Catch.ExceptionAsync(async () => await That(subject).IsObject(
						o => o.At("album.tracks[0].durationSeconds").Matching(expected),
						o => o.TreatingNullAsAbsent(treatNullAsAbsent)));
					Exception? nestedException = await Catch.ExceptionAsync(async () => await That(subject).IsObject(
						o => o.With("album").AnObject(album => album
							.With("tracks").AnArray(tracks => tracks
								.At(0).AnObject(track => track
									.With("durationSeconds").Matching(expected)))),
						o => o.TreatingNullAsAbsent(treatNullAsAbsent)));

					await That(GetResult(pathException)).IsEqualTo(GetResult(nestedException))
						.Because($"the path must be verified like the nested builders (treat null as absent: {treatNullAsAbsent}, expected: {expected ?? "null"})");
				}

				static string? GetResult(Exception? exception)
					=> exception?.Message.Substring(exception.Message.IndexOf("\nbut ", StringComparison.Ordinal));
			}

			[Test]
			public async Task WhenPropertyIsIgnored_ShouldStillVerifyIt()
			{
				JsonElement subject = FromString("{\"album\": {\"year\": 1969}}");

				async Task Act()
					=> await That(subject).IsObject(o => o.At("album.year").Matching(1970), o => o.IgnoringMember("year"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.album.year matches 1970,
					             but it differed as $.album.year was 1969 instead of 1970

					             JSON options:
					              - ignore additional properties
					              - ignore members: ["year"]
					             """)
					.Because("an explicit expectation wins over an ignored member");
			}

			[Test]
			public async Task WhenMemberIsPlural_ShouldUseSingularFormForThePath()
			{
				JsonElement subject = FromString("{\"album\": {\"title\": \"Abbey Road\"}}");

				async Task Act()
					=> await WhosePluralItems(That(subject), items => items.IsObject(o => o.At("album.title").ANumber()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             whose Items are objects and $.album.title is a number,
					             but Items differed as $.album.title was a string

					             JSON options (Items):
					              - ignore additional properties
					             """)
					.Because("the path is the subject of its expectation");
			}

			[Test]
			[Arguments("[]", "an array")]
			[Arguments("\"Abbey Road\"", "a string")]
			public async Task WhenJsonIsNoObject_ShouldOnlyReportTheKind(string json, string kindString)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject(o => o.At("album.title").AString());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is an object and $.album.title is a string,
					              but it was {kindString}

					              JSON options:
					               - ignore additional properties
					              """);
			}

			[Test]
			public async Task WhenPathIsNull_ShouldThrowArgumentNullException()
			{
				JsonElement subject = FromString("{\"foo\": 1}");

				async Task Act()
					=> await That(subject).IsObject(o => o.At(null!).Matching(1));

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("path").And
					.WithMessage("The 'path' cannot be null.*").AsWildcard()
					.Because("an invalid argument is thrown as it is instead of failing the expectation");
			}

			[Test]
			public async Task WhenPathIsEmpty_ShouldThrowArgumentException()
			{
				JsonElement subject = FromString("{\"foo\": 1}");

				async Task Act()
					=> await That(subject).IsObject(o => o.At("").Matching(1));

				await That(Act).Throws<ArgumentException>()
					.WithParamName("path").And
					.WithMessage("The 'path' cannot be empty.").AsPrefix()
					.Because("an invalid argument is thrown as it is instead of failing the expectation");
			}

			[Test]
			[Arguments("$", "it is relative to the current object and must not start with '$'")]
			[Arguments("$.foo", "it is relative to the current object and must not start with '$'")]
			[Arguments("$[0]", "it is relative to the current object and must not start with '$'")]
			[Arguments(".foo", "expected a property name at position 0")]
			[Arguments("[0]", "expected a property name at position 0")]
			[Arguments("foo.", "expected a property name at position 4")]
			[Arguments("foo.[0]", "expected a property name at position 4")]
			[Arguments("foo..bar", "recursive descent ('..') is not supported")]
			[Arguments("..bar", "recursive descent ('..') is not supported")]
			[Arguments("*", "wildcards ('*') are not supported")]
			[Arguments("foo.*", "wildcards ('*') are not supported")]
			[Arguments("foo[*]", "wildcards ('*') are not supported")]
			[Arguments("foo[?(@.bar)]", "filters are not supported")]
			[Arguments("foo[]", "expected a non-negative integer index at position 4")]
			[Arguments("foo[bar]", "expected a non-negative integer index at position 4")]
			[Arguments("foo[-1]", "expected a non-negative integer index at position 4")]
			[Arguments("foo[ 1]", "expected a non-negative integer index at position 4")]
			[Arguments("foo[1.5]", "expected a non-negative integer index at position 4")]
			[Arguments("foo[0:2]", "expected a non-negative integer index at position 4")]
			[Arguments("foo[99999999999]", "expected a non-negative integer index at position 4")]
			[Arguments("foo[0", "the '[' at position 3 is not closed")]
			[Arguments("foo]", "expected '.' or '[' at position 3")]
			[Arguments("foo[0]bar", "expected '.' or '[' at position 6")]
			public async Task WhenPathIsInvalid_ShouldThrowArgumentException(string path, string reason)
			{
				JsonElement subject = FromString("{\"foo\": [{\"bar\": 1}]}");

				async Task Act()
					=> await That(subject).IsObject(o => o.At(path).Matching(1));

				await That(Act).Throws<ArgumentException>()
					.WithParamName("path").And
					.WithMessage($"The path '{path}' is invalid: {reason}.").AsPrefix()
					.Because("an invalid argument is thrown as it is instead of failing the expectation");
			}

			[Test]
			public async Task WhenPathIsInvalid_ShouldThrowArgumentExceptionAlsoWhenNegated()
			{
				JsonElement subject = FromString("{\"foo\": 1}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsObject(o => o.At("$.foo").Matching(1)));

				await That(Act).Throws<ArgumentException>()
					.WithParamName("path")
					.Because("an invalid path would otherwise let the negation pass");
			}
		}

		public sealed class WithTests
		{
			[Test]
			public async Task WhenNestedInThatAll_ShouldIndentTheFailures()
			{
				JsonElement subject = FromString("{\"foo\": 1, \"bar\": 2}");

				async Task Act()
					=> await ThatAll(That(subject).IsObject(o => o.With("foo").Matching(2).With("bar").Matching(1)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected all of the following to succeed:
					              [01] Expected that subject is an object and $.foo matches 2 and $.bar matches 1
					             but
					              [01] it differed as
					                     $.foo was 1 instead of 2 and
					                     $.bar was 2 instead of 1

					             [01] JSON options:
					              - ignore additional properties
					             """)
					.Because("every line of the failures belongs to the nested result");
			}

			[Test]
			public async Task WhenPropertyNameIsNull_ShouldThrowArgumentNullException()
			{
				JsonElement subject = FromString("{\"foo\": 1}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With(null!).Matching(1));

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("propertyName").And
					.WithMessage("The 'propertyName' cannot be null.*").AsWildcard()
					.Because("an invalid argument is thrown as it is instead of failing the expectation");
			}

			[Test]
			public async Task WhenMatchFails_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": 1}");

				async Task Act()
					=> await That(subject).IsObject(
						o => o.With("foo").Matching(2),
						o => o.IgnoringAdditionalProperties());

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
			[Arguments(false)]
			[Arguments(true)]
			public async Task WhenMatchingArrayInDifferentOrder_ShouldFailUnlessIgnoringArrayOrder(bool ignoreArrayOrder)
			{
				JsonElement subject = FromString("{\"foo\": [1, 2]}");

				async Task Act()
					=> await That(subject).IsObject(
						o => o.With("foo").Matching(new[] { 2, 1, }),
						o => o.IgnoringArrayOrder(ignoreArrayOrder));

				await That(Act).Throws<FailException>().OnlyIf(!ignoreArrayOrder)
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo matches new[] { 2, 1, },
					             but it differed as
					               $.foo[0] was 1 instead of 2 and
					               $.foo[1] was 2 instead of 1

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			[Arguments(false)]
			[Arguments(true)]
			public async Task WhenMatchingArrayWithAdditionalElements_ShouldFailUnlessIgnoringAdditionalArrayElements(
				bool ignoreAdditionalArrayElements)
			{
				JsonElement subject = FromString("{\"foo\": [1, 2, 3]}");

				async Task Act()
					=> await That(subject).IsObject(
						o => o.With("foo").Matching(new[] { 1, 2, }),
						o => o.IgnoringAdditionalArrayElements(ignoreAdditionalArrayElements));

				await That(Act).Throws<FailException>().OnlyIf(!ignoreAdditionalArrayElements)
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo matches new[] { 1, 2, },
					             but it differed as $.foo[2] had unexpected 3

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenMatchingArrayWithAdditionalObject_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": [{\"bar\": 1, \"baz\": 2}, {\"bar\": 2}]}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").Matching(new[] { new { bar = 1, }, }));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo matches new[] { new { bar = 1, }, },
					             but it differed as $.foo[1] had unexpected {"bar":2}

					             JSON options:
					              - ignore additional properties
					             """)
					.Because("additional properties are ignored by default, but additional array elements are not");
			}

			[Test]
			public async Task WhenPropertyIsIgnored_ShouldStillVerifyIt()
			{
				JsonElement subject = FromString("{\"id\": 1}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("id").Matching(2), o => o.IgnoringMember("id"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.id matches 2,
					             but it differed as $.id was 1 instead of 2

					             JSON options:
					              - ignore additional properties
					              - ignore members: ["id"]
					             """)
					.Because("an explicit expectation wins over an ignored member");
			}

			[Test]
			public async Task WhenMatchingObjectWithIgnoredMember_ShouldIgnoreItInTheComparison()
			{
				JsonElement subject = FromString("{\"foo\": {\"id\": 1, \"name\": \"a\"}}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").Matching(new
					{
						id = 2,
						name = "a",
					}), o => o.IgnoringMember("id"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMatchingItIsOnNullProperty_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": null}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").Matching(It.Is<string>().That.StartsWith("a")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo matches It.Is<string>().That.StartsWith("a"),
					             but it differed as $.foo was <null>

					             JSON options:
					              - ignore additional properties
					             """)
					.Because("a null value has no content that could start with the prefix");
			}

			[Test]
			public async Task WhenMatchingItIsWaitsLongerThanTheTimeout_ShouldStopAtTheTimeout()
			{
				JsonElement subject = FromString("{\"foo\": \"bar\"}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo")
							.Matching(It.Is<string>().That.Satisfies(_ => false).Within(TimeSpan.FromSeconds(30))))
						.WithTimeout(TimeSpan.FromMilliseconds(100));

				await That(Act).Throws<FailException>().Within(TimeSpan.FromSeconds(10))
					.WithMessage("""
					             *but it did not finish within 0:00.100

					             JSON options:
					              - ignore additional properties
					             """).AsWildcard()
					.Because("the nested expectation must be canceled by the timeout of the evaluation");
			}

			[Test]
			public async Task WhenNestedMatchingItIsWaits_ShouldNotBlockTheCaller()
			{
				JsonElement subject = FromString("{\"foo\": {\"bar\": \"baz\"}}");
				bool isSatisfied = false;

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").AnObject(f => f.With("bar")
						.Matching(It.Is<string>().That.Satisfies(_ => isSatisfied).Within(TimeSpan.FromSeconds(5)))));

				Task evaluation = Act();
				isSatisfied = true;

				await That(() => evaluation).DoesNotThrow()
					.Because("the evaluation must return to the caller while it waits instead of blocking it");
			}

			[Test]
			[Arguments("1.5", 1, "$.a was 1.5 instead of 1")]
			[Arguments("2147483648", int.MaxValue, "$.a was 2147483648 instead of 2147483647")]
			[Arguments("9223372036854775808", long.MaxValue, "$.a was 9223372036854775808 instead of 9223372036854775807")]
			[Arguments("\"1\"", 1, "$.a was string \"1\" instead of 1")]
			[Arguments("1", "1", "$.a was number 1 instead of \"1\"")]
			[Arguments("true", false, "$.a was True instead of False")]
			public async Task WhenMatchingDifferentSimpleValue_ShouldFail(string value, object expected, string error)
			{
				JsonElement subject = FromString($"{{\"a\": {value}}}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("a").Matching(expected));

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is an object and $.a matches expected,
					              but it differed as {error}

					              JSON options:
					               - ignore additional properties
					              """);
			}

			[Test]
			[Arguments("\"Let It Be\"", "Let It Be")]
			[Arguments("\"Caf\\u00e9 \\\"live\\\"\\n\"", "Caf\u00e9 \"live\"\n")]
			[Arguments("\"\"", "")]
			[Arguments("true", true)]
			[Arguments("false", false)]
			[Arguments("-0", 0)]
			[Arguments("1.0", 1)]
			[Arguments("1e0", 1L)]
			[Arguments("2147483647", int.MaxValue)]
			[Arguments("-2147483648", int.MinValue)]
			[Arguments("9223372036854775807", long.MaxValue)]
			[Arguments("-9223372036854775808", long.MinValue)]
			[Arguments("1", 1.0)]
			public async Task WhenMatchingEqualSimpleValue_ShouldSucceed(string value, object expected)
			{
				JsonElement subject = FromString($"{{\"a\": {value}}}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("a").Matching(expected));

				await That(Act).DoesNotThrow()
					.Because("an equal string, boolean or number matches regardless of how the JSON writes it");
			}

			[Test]
			public async Task WhenMatchSucceeds_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"foo\": 2}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").Matching(2));


				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMultipleMatchesFail_ShouldListAllFailures()
			{
				JsonElement subject = FromString("{\"foo\": 1, \"bar\": 2}");

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
				JsonElement subject = FromString("{\"foo\": 1, \"bar\": {\"baz\": 1, \"bat\": 2}}");

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
				JsonElement subject = FromString("{\"foo\": 1}");

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
			public async Task WhenMatchingNullOnMissingProperty_ShouldFail()
			{
				JsonElement subject = FromString("{}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").Matching(null));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo matches null,
					             but it differed as property $.foo did not exist

					             JSON options:
					              - ignore additional properties
					             """)
					.Because("by default a missing property differs from a property with a null value");
			}

			[Test]
			[Arguments("{\"foo\": null}")]
			[Arguments("{}")]
			public async Task WhenMatchingNull_WhenTreatingNullAsAbsent_ShouldSucceed(string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").Matching(null),
						o => o.TreatingNullAsAbsent());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMatchingValueOnNullProperty_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": null}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").Matching(1));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo matches 1,
					             but it differed as $.foo was <null> instead of 1

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			[Arguments("{\"foo\": null}")]
			[Arguments("{}")]
			public async Task WhenMatchingValue_WhenTreatingNullAsAbsent_ShouldFail(string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").Matching(1),
						o => o.TreatingNullAsAbsent());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo matches 1,
					             but it differed as property $.foo did not exist

					             JSON options:
					              - ignore additional properties
					              - treat null as absent
					             """)
					.Because("a property with a null value is treated as absent");
			}

			[Test]
			public async Task WhenMatchingItIsOnNullProperty_WhenTreatingNullAsAbsent_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": null}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").Matching(It.Is<string?>().That.IsNull()),
						o => o.TreatingNullAsAbsent());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo matches It.Is<string?>().That.IsNull(),
					             but it differed as property $.foo did not exist

					             JSON options:
					              - ignore additional properties
					              - treat null as absent
					             """)
					.Because("only an expected null value accepts an absent property");
			}

			[Test]
			[Arguments("{\"foo\": null}")]
			[Arguments("{}")]
			public async Task WhenAnObject_WhenTreatingNullAsAbsent_ShouldFail(string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").AnObject(),
						o => o.TreatingNullAsAbsent());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo is an object,
					             but it differed as property $.foo did not exist

					             JSON options:
					              - ignore additional properties
					              - treat null as absent
					             """);
			}

			[Test]
			public async Task WhenAnObjectOnNullProperty_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": null}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").AnObject());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo is an object,
					             but it differed as $.foo was null

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			[Arguments("{\"foo\": null}")]
			[Arguments("{}")]
			public async Task WhenAnObjectWithExpectation_WhenTreatingNullAsAbsent_ShouldFail(string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").AnObject(f => f.With("bar").Matching(1)),
						o => o.TreatingNullAsAbsent());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo is an object and $.foo.bar matches 1,
					             but it differed as property $.foo did not exist

					             JSON options:
					              - ignore additional properties
					              - treat null as absent
					             """);
			}

			[Test]
			[Arguments("{\"foo\": null}")]
			[Arguments("{}")]
			public async Task WhenAnArray_WhenTreatingNullAsAbsent_ShouldFail(string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").AnArray(a => a.With(0).Elements()),
						o => o.TreatingNullAsAbsent());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo is an array with 0 elements,
					             but it differed as property $.foo did not exist

					             JSON options:
					              - ignore additional properties
					              - treat null as absent
					             """);
			}

			[Test]
			public async Task WhenAbsentPropertyIsCombinedWithOtherExpectations_WhenTreatingNullAsAbsent_ShouldListAllFailuresInOrder()
			{
				JsonElement subject = FromString("{\"foo\": 1, \"bar\": null, \"baz\": 3}");

				async Task Act()
					=> await That(subject).IsObject(o => o
							.With("foo").Matching(2).And
							.With("bar").Matching(true).And
							.With("baz").Matching(4),
						o => o.TreatingNullAsAbsent());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo matches 2 and $.bar matches true and $.baz matches 4,
					             but it differed as
					               $.foo was 1 instead of 2 and
					               property $.bar did not exist and
					               $.baz was 3 instead of 4

					             JSON options:
					              - ignore additional properties
					              - treat null as absent
					             """);
			}

			[Test]
			public async Task WhenMatchingNullInNestedObject_WhenTreatingNullAsAbsent_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"foo\": {\"bar\": 1}}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").AnObject(f => f.With("baz").Matching(null)),
						o => o.TreatingNullAsAbsent());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMatchingObjectWithNullProperty_WhenTreatingNullAsAbsent_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"foo\": {\"bar\": 1}}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").Matching(new
						{
							bar = 1,
							baz = (int?)null,
						}),
						o => o.TreatingNullAsAbsent());

				await That(Act).DoesNotThrow()
					.Because("the option also applies to the comparison of the matched value");
			}

			[Test]
			public async Task WhenMatchingObjectWithNullProperty_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": {\"bar\": 1}}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").Matching(new
					{
						bar = 1,
						baz = (int?)null,
					}));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo matches new
					             {
					             	bar = 1,
					             	baz = (int?)null,
					             },
					             but it differed as $.foo.baz was missing

					             JSON options:
					              - ignore additional properties
					             """);
			}
		}

		public sealed class WithValueKindTests
		{
			[Test]
			[Arguments("true")]
			[Arguments("false")]
			public async Task WhenPropertiesHaveTheExpectedValueKinds_ShouldSucceed(string active)
			{
				JsonElement subject = FromString(
					$"{{\"id\": 42, \"name\": \"foo\", \"active\": {active}, \"deletedAt\": null}}");

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
				JsonElement subject = FromString(
					"{\"id\": \"42\", \"name\": 1, \"active\": \"yes\", \"deletedAt\": false}");

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
					               $.name was a number and
					               $.active was a string and
					               $.deletedAt was false

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			[Arguments("{}", "an object")]
			[Arguments("[]", "an array")]
			[Arguments("2", "a number")]
			[Arguments("true", "true")]
			[Arguments("false", "false")]
			[Arguments("null", "null")]
			public async Task WhenPropertyIsNoString_ShouldFail(string value, string kindString)
			{
				JsonElement subject = FromString($"{{\"foo\": {value}}}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").AString());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is an object and $.foo is a string,
					              but it differed as $.foo was {kindString}

					              JSON options:
					               - ignore additional properties
					              """);
			}

			[Test]
			[Arguments("{}", "an object")]
			[Arguments("[]", "an array")]
			[Arguments("\"2\"", "a string")]
			[Arguments("true", "true")]
			[Arguments("false", "false")]
			[Arguments("null", "null")]
			public async Task WhenPropertyIsNoNumber_ShouldFail(string value, string kindString)
			{
				JsonElement subject = FromString($"{{\"foo\": {value}}}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").ANumber());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is an object and $.foo is a number,
					              but it differed as $.foo was {kindString}

					              JSON options:
					               - ignore additional properties
					              """);
			}

			[Test]
			[Arguments("{}", "an object")]
			[Arguments("[]", "an array")]
			[Arguments("1", "a number")]
			[Arguments("\"true\"", "a string")]
			[Arguments("null", "null")]
			public async Task WhenPropertyIsNoBoolean_ShouldFail(string value, string kindString)
			{
				JsonElement subject = FromString($"{{\"foo\": {value}}}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").ABoolean());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is an object and $.foo is a boolean,
					              but it differed as $.foo was {kindString}

					              JSON options:
					               - ignore additional properties
					              """);
			}

			[Test]
			[Arguments("{}", "an object")]
			[Arguments("[]", "an array")]
			[Arguments("0", "a number")]
			[Arguments("\"null\"", "a string")]
			[Arguments("true", "true")]
			[Arguments("false", "false")]
			public async Task WhenPropertyIsNotNull_ShouldFail(string value, string kindString)
			{
				JsonElement subject = FromString($"{{\"foo\": {value}}}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").Null());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is an object and $.foo is null,
					              but it differed as $.foo was {kindString}

					              JSON options:
					               - ignore additional properties
					              """);
			}

			[Test]
			public async Task WhenPropertyDoesNotExist_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": 1}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("bar").Null());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.bar is null,
					             but it differed as property $.bar did not exist

					             JSON options:
					              - ignore additional properties
					             """)
					.Because("a missing property is not a property with a null value");
			}

			[Test]
			[Arguments("{\"foo\": null}")]
			[Arguments("{}")]
			public async Task WhenNull_WhenTreatingNullAsAbsent_ShouldSucceed(string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").Null(), o => o.TreatingNullAsAbsent());

				await That(Act).DoesNotThrow()
					.Because("an absent property is accepted when it is expected to be null");
			}

			[Test]
			[Arguments("{\"foo\": null}")]
			[Arguments("{}")]
			public async Task WhenAString_WhenTreatingNullAsAbsent_ShouldFail(string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").AString(), o => o.TreatingNullAsAbsent());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo is a string,
					             but it differed as property $.foo did not exist

					             JSON options:
					              - ignore additional properties
					              - treat null as absent
					             """);
			}

			[Test]
			public async Task WhenNestedPropertyHasAnotherValueKind_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": {\"bar\": {\"baz\": \"1\"}}}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With("foo").AnObject(f => f
							.With("bar").AnObject(b => b
								.With("baz").ANumber())));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo is an object and $.foo.bar is an object and $.foo.bar.baz is a number,
					             but it differed as $.foo.bar.baz was a string

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			[Arguments("[]", "an array")]
			[Arguments("\"foo\"", "a string")]
			public async Task WhenJsonIsNoObject_ShouldOnlyReportTheKind(string json, string kindString)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").AString());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is an object and $.foo is a string,
					              but it was {kindString}

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
				JsonElement subject = FromString("{}");

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
				JsonElement subject = FromString("{\"foo\": 1, \"bar\": 2}");

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
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With(expected).Properties());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyIsNull_ShouldCountIt()
			{
				JsonElement subject = FromString("{\"foo\": 1, \"bar\": null}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With(1).Properties());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object with 1 property,
					             but it differed as $ had 2 properties

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			[Arguments(1, true)]
			[Arguments(2, false)]
			public async Task WhenPropertyIsNull_WhenTreatingNullAsAbsent_ShouldNotCountIt(int expected, bool isMatch)
			{
				JsonElement subject = FromString("{\"foo\": 1, \"bar\": null}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With(expected).Properties(), o => o.TreatingNullAsAbsent());

				await That(Act).Throws<FailException>().OnlyIf(!isMatch)
					.WithMessage("""
					             Expected that subject
					             is an object with 2 properties,
					             but it differed as $ had 1 property

					             JSON options:
					              - ignore additional properties
					              - treat null as absent
					             """);
			}
		}

		public sealed class WithoutTests
		{
			[Test]
			public async Task WhenPropertyIsIgnored_ShouldStillVerifyItsAbsence()
			{
				JsonElement subject = FromString("{\"id\": 1}");

				async Task Act()
					=> await That(subject).IsObject(o => o.Without("id"), o => o.IgnoringMember("id"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.id does not exist,
					             but it differed as $.id was 1

					             JSON options:
					              - ignore additional properties
					              - ignore members: ["id"]
					             """)
					.Because("an explicit expectation wins over an ignored member");
			}

			[Test]
			public async Task WhenCombinedWithOtherExpectations_ShouldListAllFailuresInOrder()
			{
				JsonElement subject = FromString("{\"foo\": 1, \"bar\": 2, \"baz\": 3}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With("foo").Matching(2).And
						.Without("bar").And
						.With("baz").Matching(4));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo matches 2 and $.bar does not exist and $.baz matches 4,
					             but it differed as
					               $.foo was 1 instead of 2 and
					               $.bar was 2 and
					               $.baz was 3 instead of 4

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			[Arguments("[]", "an array")]
			[Arguments("2", "a number")]
			public async Task WhenJsonIsNoObject_ShouldOnlyReportTheKind(string json, string kindString)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject(o => o.Without("foo"));

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is an object and $.foo does not exist,
					              but it was {kindString}

					              JSON options:
					               - ignore additional properties
					              """);
			}

			[Test]
			public async Task WhenNestedObjectContainsProperty_ShouldFail()
			{
				JsonElement subject = FromString("{\"address\": {\"street\": \"Main\", \"internalId\": 42}}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With("address").AnObject(a => a.Without("internalId")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.address is an object and $.address.internalId does not exist,
					             but it differed as $.address.internalId was 42

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenNestedObjectDoesNotContainProperty_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"address\": {\"street\": \"Main\"}, \"internalId\": 42}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With("address").AnObject(a => a.Without("internalId")));

				await That(Act).DoesNotThrow()
					.Because("only the property of the nested object is considered");
			}

			[Test]
			public async Task WhenNestedPropertyIsNoObject_ShouldOnlyReportTheKind()
			{
				JsonElement subject = FromString("{\"foo\": 1}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With("foo").AnObject(a => a.Without("bar")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo is an object and $.foo.bar does not exist,
					             but it differed as $.foo was a number

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenPropertyDiffersInCase_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"Foo\": 1}");

				async Task Act()
					=> await That(subject).IsObject(o => o.Without("foo"));

				await That(Act).DoesNotThrow()
					.Because("property names in JSON are case-sensitive");
			}

			[Test]
			public async Task WhenPropertyDoesNotExist_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"foo\": 1}");

				async Task Act()
					=> await That(subject).IsObject(o => o.Without("bar"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyExists_ShouldFail()
			{
				JsonElement subject = FromString("{\"name\": \"John\", \"passwordHash\": \"8f14e4\"}");

				async Task Act()
					=> await That(subject).IsObject(o => o
						.With("name").Matching("John").And
						.Without("passwordHash"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.name matches "John" and $.passwordHash does not exist,
					             but it differed as $.passwordHash was "8f14e4"

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenPropertyExistsMultipleTimes_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": 1, \"foo\": 2}");

				async Task Act()
					=> await That(subject).IsObject(o => o.Without("foo"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo does not exist,
					             but it differed as $.foo was 2

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenPropertyIsAnObject_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": {\"bar\": [1, 2]}}");

				async Task Act()
					=> await That(subject).IsObject(o => o.Without("foo"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo does not exist,
					             but it differed as $.foo was {"bar":[1,2]}

					             JSON options:
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": null}");

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
			[Arguments("{\"foo\": null}")]
			[Arguments("{}")]
			public async Task WhenPropertyIsNullOrMissing_WhenTreatingNullAsAbsent_ShouldSucceed(string json)
			{
				JsonElement subject = FromString(json);

				async Task Act()
					=> await That(subject).IsObject(o => o.Without("foo"), o => o.TreatingNullAsAbsent());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyHasValue_WhenTreatingNullAsAbsent_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": 1}");

				async Task Act()
					=> await That(subject).IsObject(o => o.Without("foo"), o => o.TreatingNullAsAbsent());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo does not exist,
					             but it differed as $.foo was 1

					             JSON options:
					              - ignore additional properties
					              - treat null as absent
					             """);
			}

			[Test]
			public async Task WhenNestedPropertyIsNull_WhenTreatingNullAsAbsent_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"address\": {\"street\": \"Main\", \"internalId\": null}}");

				async Task Act()
					=> await That(subject).IsObject(o => o
							.With("address").AnObject(a => a.Without("internalId")),
						o => o.TreatingNullAsAbsent());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyNameContainsSpecialCharacters_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo.bar\": 1, \"foo\": {\"bar\": 2}}");

				async Task Act()
					=> await That(subject).IsObject(o => o.Without("foo.bar"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo.bar does not exist,
					             but it differed as $.foo.bar was 1

					             JSON options:
					              - ignore additional properties
					             """)
					.Because("the property name is no path");
			}

			[Test]
			public async Task WhenPropertyNameIsNull_ShouldThrowArgumentNullException()
			{
				JsonElement subject = FromString("{\"foo\": 1}");

				async Task Act()
					=> await That(subject).IsObject(o => o.Without(null!));

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("propertyName").And
					.WithMessage("The 'propertyName' cannot be null.*").AsWildcard()
					.Because("an invalid argument is thrown as it is instead of failing the expectation");
			}
		}

		public sealed class ComparingNumbersWithinTests
		{
			[Test]
			public async Task WhenMatchingNumberIsWithinTheTolerance_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"foo\": {\"bar\": 1.04}}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").Matching(new
					{
						bar = 1,
					}), o => o.ComparingNumbersWithin(0.05));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMatchingNumberIsOutsideTheTolerance_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": {\"bar\": 1.06}}");

				async Task Act()
					=> await That(subject).IsObject(o => o.With("foo").Matching(new
					{
						bar = 1,
					}), o => o.ComparingNumbersWithin(0.05));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an object and $.foo matches new
					             {
					             	bar = 1,
					             },
					             but it differed as $.foo.bar was 1.06 instead of 1 ± 0.05

					             JSON options:
					              - ignore additional properties
					              - compare numbers within ± 0.05
					             """);
			}
		}
	}
}
