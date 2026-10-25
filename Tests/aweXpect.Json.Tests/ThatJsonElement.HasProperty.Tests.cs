using System.Text.Json;

namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonElement
{
	public sealed class HasProperty
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldReturnTheSubject()
			{
				JsonElement subject = FromString("{\"foo\": 1}");

				JsonElement result = await That(subject).HasProperty("foo");

				await That(result.GetRawText()).IsEqualTo(subject.GetRawText());
			}

			[Test]
			public async Task WhenCombinedWithAnd_ShouldReportEveryFailingPart()
			{
				JsonElement subject = FromString("{\"title\": \"Let It Be\", \"artist\": \"The Beatles\"}");

				async Task Act()
					=> await That(subject).HasProperty("album").And.DoesNotHaveProperty("artist");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has property "album" and does not have property "artist",
					             but property $.album did not exist and it had property "artist" with "The Beatles"
					             """);
			}

			[Test]
			public async Task WhenJsonIsNoObject_ShouldFail()
			{
				JsonElement subject = FromString("[1, 2]");

				async Task Act()
					=> await That(subject).HasProperty("foo");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has property "foo",
					             but it was an array
					             """);
			}

			[Test]
			public async Task WhenMemberIsPlural_ShouldUsePluralForm()
			{
				JsonElement subject = FromString("{\"foo\": 1}");

				async Task Act()
					=> await WhosePluralItems(That(subject), items => items
						.HasProperty("bar").And.DoesNotHaveProperty("foo"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             whose Items have property "bar" and do not have property "foo",
					             but property $.bar did not exist and Items had property "foo" with 1
					             """)
					.Because("the plural member is the subject of the expectation");
			}

			[Test]
			public async Task WhenNameContainsPathCharacters_ShouldTakeItLiterally()
			{
				JsonElement subject = FromString("{\"@odata.context\": \"x\", \"items[0]\": 1}");

				async Task Act()
					=> await That(subject).HasProperty("@odata.context").And.HasProperty("items[0]");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNameIsNull_ShouldThrowArgumentNullException()
			{
				JsonElement subject = FromString("{}");

				async Task Act()
					=> await That(subject).HasProperty(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'name' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenPropertyDiffersInCase_ShouldFail()
			{
				JsonElement subject = FromString("{\"Foo\": 1}");

				async Task Act()
					=> await That(subject).HasProperty("foo");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has property "foo",
					             but property $.foo did not exist
					             """);
			}

			[Test]
			public async Task WhenPropertyDoesNotExist_ShouldFail()
			{
				JsonElement subject = FromString("{\"bar\": 1}");

				async Task Act()
					=> await That(subject).HasProperty("foo");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has property "foo",
					             but property $.foo did not exist
					             """);
			}

			[Test]
			[Arguments("1")]
			[Arguments("null")]
			[Arguments("{}")]
			public async Task WhenPropertyExists_ShouldSucceed(string value)
			{
				JsonElement subject = FromString($"{{\"foo\": {value}}}");

				async Task Act()
					=> await That(subject).HasProperty("foo");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyIsNull_WhenTreatingNullAsAbsent_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": null}");

				async Task Act()
					=> await That(subject).HasProperty("foo").TreatingNullAsAbsent();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has property "foo",
					             but property $.foo did not exist

					             JSON options:
					              - treat null as absent
					             """);
			}

			[Test]
			public async Task WhenPropertyIsNull_WhenTreatingNullAsAbsentIsDisabled_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"foo\": null}");

				async Task Act()
					=> await That(subject).HasProperty("foo").TreatingNullAsAbsent(false);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WithExpectationsTests
		{
			[Test]
			public async Task WhenExpectationsAreMet_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"title\": \"Let It Be\"}");

				async Task Act()
					=> await That(subject).HasProperty("title", p => p.IsString(s => s.StartsWith("Let")));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectationsAreNotMet_ShouldFailWithThePathOfTheProperty()
			{
				JsonElement subject = FromString("{\"title\": \"Yesterday\"}");

				async Task Act()
					=> await That(subject).HasProperty("title", p => p.IsString(s => s.StartsWith("Let")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has property "title" which is a string which starts with "Let",
					             but $.title was "Yesterday", which differs at index 0:
					                ↓ (actual)
					               "Yesterday"
					               "Let"
					                ↑ (expected prefix)
					             """);
			}

			[Test]
			public async Task WhenExpectationsAreNull_ShouldThrowArgumentNullException()
			{
				JsonElement subject = FromString("{}");

				async Task Act()
					=> await That(subject).HasProperty("foo", null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Test]
			[Arguments("a.b", "$['a.b'].c")]
			[Arguments("track[1]", "$['track[1]'].c")]
			[Arguments("it's", @"$['it\'s'].c")]
			[Arguments(@"back\slash", @"$['back\\slash'].c")]
			[Arguments("user-id", "$['user-id'].c")]
			[Arguments("1st", "$['1st'].c")]
			[Arguments("", "$[''].c")]
			[Arguments("_privé1", "$._privé1.c")]
			public async Task WhenNameIsNoIdentifier_ShouldUseTheBracketNotationInThePath(string name,
				string expectedPath)
			{
				string jsonName = name.Replace(@"\", @"\\");
				JsonElement subject = FromString(
					"{\"" + jsonName + "\": {\"c\": 1}, \"a\": {\"b\": {\"c\": 2}}}");

				async Task Act()
					=> await That(subject).HasProperty(name, x => x.HasProperty("c", c => c.IsNumber(5)));

				await That(Act).Throws<FailException>()
					.WithMessage($"*but {expectedPath} was 1").AsWildcard()
					.Because("a name with '.', '[' or ']' would otherwise read as several segments");
			}

			[Test]
			public async Task WhenNested_ShouldContinueThePath()
			{
				JsonElement subject = FromString("{\"artist\": {\"origin\": {\"city\": \"London\"}}}");

				async Task Act()
					=> await That(subject).HasProperty("artist", a => a
						.HasProperty("origin", o => o
							.HasProperty("city", c => c.IsString(s => s.IsEqualTo("Liverpool")))));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has property "artist" which has property "origin" which has property "city" which is a string which is equal to "Liverpool",
					             but $.artist.origin.city was "London", which differs at index 1:
					                 ↓ (actual)
					               "London"
					               "Liverpool"
					                 ↑ (expected)
					             """);
			}

			[Test]
			public async Task WhenNestedInTheElementsOfAnArray_ShouldLabelTheContextAfterTheElement()
			{
				JsonElement subject = FromString(
					"[{\"origin\": {\"city\": \"Liverpool\"}}, {\"origin\": {\"city\": \"London\"}}]");

				async Task Act()
					=> await That(subject).IsArray().WhoseElements.All()
						.ComplyWith(t => t.HasProperty("origin", o => o.Matches(new { city = "Liverpool", })));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             *

					             JSON options (elements[1].origin):
					              - ignore additional properties
					             """).AsWildcard()
					.Because("the JSON path within the element follows the label of the element without its root");
			}

			[Test]
			public async Task WhenNestedMatchesFails_ShouldNameTheFullPath()
			{
				JsonElement subject = FromString("{\"artist\": {\"origin\": \"London\"}}");

				async Task Act()
					=> await That(subject).HasProperty("artist", a => a.Matches(new
					{
						origin = "Liverpool",
					}));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has property "artist" which matches new
					             {
					             	origin = "Liverpool",
					             },
					             but $.artist differed as $.artist.origin was "London" instead of "Liverpool"

					             JSON options ($.artist):
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenNestedTwice_ShouldLabelTheContextWithTheFullPath()
			{
				JsonElement subject = FromString("{\"artist\": {\"origin\": {\"city\": \"London\"}}}");

				async Task Act()
					=> await That(subject).HasProperty("artist", a => a
						.HasProperty("origin", o => o.Matches(new { city = "Liverpool", })));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has property "artist" which has property "origin" which matches new { city = "Liverpool", },
					             but $.artist.origin differed as $.artist.origin.city was "London" instead of "Liverpool"

					             JSON options ($.artist.origin):
					              - ignore additional properties
					             """);
			}

			[Test]
			public async Task WhenPropertyDoesNotExist_ShouldNotVerifyTheExpectations()
			{
				JsonElement subject = FromString("{\"bar\": 1}");

				async Task Act()
					=> await That(subject).HasProperty("foo", p => p.IsNumber());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has property "foo" which is a number,
					             but property $.foo did not exist
					             """);
			}

			[Test]
			public async Task WhenPropertyIsNull_ShouldVerifyTheJsonNull()
			{
				JsonElement subject = FromString("{\"label\": null}");

				async Task Act()
					=> await That(subject).HasProperty("label", p => p.IsNull());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyIsNull_WhenTreatingNullAsAbsent_ShouldNotVerifyTheExpectations()
			{
				JsonElement subject = FromString("{\"label\": null}");

				async Task Act()
					=> await That(subject).HasProperty("label", p => p.IsNull()).TreatingNullAsAbsent();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has property "label" which is null,
					             but property $.label did not exist

					             JSON options:
					              - treat null as absent
					             """);
			}

			[Test]
			public async Task WhenSeveralPropertiesFail_ShouldReportEveryFailingPart()
			{
				JsonElement subject = FromString("{\"title\": \"Let It Be\", \"durationSeconds\": \"243\"}");

				async Task Act()
					=> await That(subject)
						.HasProperty("title", p => p.IsNumber()).And
						.HasProperty("durationSeconds", p => p.IsNumber<int>());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has property "title" which is a number and has property "durationSeconds" which is an int,
					             but $.title was a string and $.durationSeconds was a string
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenPropertyDoesNotExist_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"bar\": 1}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasProperty("foo"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyExists_ShouldFail()
			{
				JsonElement subject = FromString("{\"foo\": [1, 2]}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasProperty("foo"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have property "foo",
					             but it had property "foo" with [1,2]
					             """);
			}

			[Test]
			public async Task WhenPropertyExistsAndSatisfiesTheExpectations_ShouldFail()
			{
				JsonElement subject = FromString("{\"title\": \"Let It Be\"}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasProperty("title", p => p.IsString()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have property "title" or $.title is not a string,
					             but $.title was a string
					             """);
			}

			[Test]
			public async Task WhenPropertyExistsButDoesNotSatisfyTheExpectations_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"title\": \"Let It Be\"}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasProperty("title", p => p.IsNumber()));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyHasTheExpectedValue_ShouldReportTheValue()
			{
				JsonElement subject = FromString("{\"rating\": 4}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasProperty("rating", r => r.IsNumber(4)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have property "rating" or $.rating is not an int equal to 4,
					             but $.rating was 4
					             """);
			}

			[Test]
			public async Task WhenPropertyIsNested_ShouldNameEveryAlternative()
			{
				JsonElement subject = FromString("{\"artist\": {\"name\": \"The Beatles\"}}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it
						.HasProperty("artist", a => a.HasProperty("name", n => n.IsString("The Beatles"))));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have property "artist" or $.artist does not have property "name" or $.artist.name is not a string equal to "The Beatles",
					             but $.artist.name was "The Beatles"
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNoObject_ShouldFail()
			{
				JsonElement subject = FromString("\"foo\"");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasProperty("foo"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have property "foo",
					             but it was a string
					             """)
					.Because("a property cannot be looked up in a value that is no object");
			}
		}
	}

	public sealed class DoesNotHaveProperty
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenJsonIsNoObject_ShouldFail()
			{
				JsonElement subject = FromString("2");

				async Task Act()
					=> await That(subject).DoesNotHaveProperty("foo");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have property "foo",
					             but it was a number
					             """);
			}

			[Test]
			public async Task WhenNameIsNull_ShouldThrowArgumentNullException()
			{
				JsonElement subject = FromString("{}");

				async Task Act()
					=> await That(subject).DoesNotHaveProperty(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'name' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenPropertyDoesNotExist_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"bar\": 1}");

				async Task Act()
					=> await That(subject).DoesNotHaveProperty("foo");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyExists_ShouldFail()
			{
				JsonElement subject = FromString("{\"title\": \"Let It Be\", \"artist\": \"The Beatles\"}");

				async Task Act()
					=> await That(subject).DoesNotHaveProperty("artist");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have property "artist",
					             but it had property "artist" with "The Beatles"
					             """);
			}

			[Test]
			public async Task WhenPropertyHasValue_WhenTreatingNullAsAbsent_ShouldFail()
			{
				JsonElement subject = FromString("{\"label\": \"Apple\"}");

				async Task Act()
					=> await That(subject).DoesNotHaveProperty("label").TreatingNullAsAbsent();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have property "label",
					             but it had property "label" with "Apple"

					             JSON options:
					              - treat null as absent
					             """);
			}

			[Test]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				JsonElement subject = FromString("{\"label\": null}");

				async Task Act()
					=> await That(subject).DoesNotHaveProperty("label");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have property "label",
					             but it had property "label" with <null>
					             """)
					.Because("a property with the value null exists");
			}

			[Test]
			public async Task WhenPropertyIsNull_WhenTreatingNullAsAbsent_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"label\": null}");

				async Task Act()
					=> await That(subject).DoesNotHaveProperty("label").TreatingNullAsAbsent();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenPropertyDoesNotExist_ShouldFail()
			{
				JsonElement subject = FromString("{\"bar\": 1}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.DoesNotHaveProperty("foo"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has property "foo",
					             but property $.foo did not exist
					             """);
			}

			[Test]
			public async Task WhenPropertyExists_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"foo\": 1}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.DoesNotHaveProperty("foo"));

				await That(Act).DoesNotThrow();
			}
		}
	}
}
