using System.Text.Json;

namespace aweXpect.Json.Tests;

public sealed class JsonOptionsContextTests
{
	public sealed class RenderingTests
	{
		[Test]
		public async Task WhenIgnoringAdditionalProperties_ShouldListIt()
		{
			JsonElement subject = ThatJsonElement.FromString("{\"foo\": 1, \"bar\": 2}");

			async Task Act()
				=> await That(subject).MatchesExactly(new
				{
					foo = 2,
				}, o => o.IgnoringAdditionalProperties());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             matches new
				             {
				             	foo = 2,
				             },
				             but it differed as $.foo was 1 instead of 2

				             JSON options:
				              - ignore additional properties
				             """);
		}

		[Test]
		public async Task WhenIgnoringAdditionalArrayElements_ShouldListIt()
		{
			JsonElement subject = ThatJsonElement.FromString("[1, 3, 4]");

			async Task Act()
				=> await That(subject).MatchesExactly([1, 2,], o => o.IgnoringAdditionalArrayElements());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             matches [1, 2,],
				             but it differed as $[1] was 3 instead of 2

				             JSON options:
				              - ignore additional array elements
				             """);
		}

		[Test]
		public async Task WhenIgnoringArrayOrder_ShouldListIt()
		{
			JsonElement subject = ThatJsonElement.FromString("[1, 3]");

			async Task Act()
				=> await That(subject).MatchesExactly([2, 1,], o => o.IgnoringArrayOrder());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             matches [2, 1,] exactly,
				             but it differed as $[1] was 3 instead of 2

				             JSON options:
				              - ignore array order
				             """);
		}

		[Test]
		public async Task WhenTreatingNullAsAbsent_ShouldListIt()
		{
			JsonElement subject = ThatJsonElement.FromString("{\"foo\": 1, \"bar\": null}");

			async Task Act()
				=> await That(subject).MatchesExactly(new
				{
					foo = 2,
				}, o => o.TreatingNullAsAbsent());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             matches new
				             {
				             	foo = 2,
				             } exactly,
				             but it differed as $.foo was 1 instead of 2

				             JSON options:
				              - treat null as absent
				             """);
		}

		[Test]
		public async Task WhenComparingNumbersWithinAnAbsoluteTolerance_ShouldListIt()
		{
			JsonElement subject = ThatJsonElement.FromString("{\"tax\": 0.06}");

			async Task Act()
				=> await That(subject).MatchesExactly(new
				{
					tax = 0.057m,
				}, o => o.ComparingNumbersWithin(0.001));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             matches new
				             {
				             	tax = 0.057m,
				             } exactly,
				             but it differed as $.tax was 0.06 instead of 0.057 ± 0.001

				             JSON options:
				              - compare numbers within ± 0.001
				             """);
		}

		[Test]
		public async Task WhenComparingNumbersWithinARelativeTolerance_ShouldListIt()
		{
			JsonElement subject = ThatJsonElement.FromString("{\"price\": 110}");

			async Task Act()
				=> await That(subject).MatchesExactly(new
				{
					price = 100,
				}, o => o.ComparingNumbersWithin(5).Percent());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             matches new
				             {
				             	price = 100,
				             } exactly,
				             but it differed as $.price was 110 instead of 100 ± 5%

				             JSON options:
				              - compare numbers within ± 5%
				             """);
		}

		[Test]
		public async Task WhenAllOptionsAreActive_ShouldListThemInAFixedOrder()
		{
			JsonElement subject = ThatJsonElement.FromString("[1, 3]");

			async Task Act()
				=> await That(subject).MatchesExactly([2, 1,], o => o
					.ComparingNumbersWithin(0.5)
					.TreatingNullAsAbsent()
					.IgnoringArrayOrder()
					.IgnoringAdditionalArrayElements()
					.IgnoringAdditionalProperties());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             matches [2, 1,],
				             but it differed as $[1] was 3 instead of 2 ± 0.5

				             JSON options:
				              - ignore additional properties
				              - ignore additional array elements
				              - ignore array order
				              - treat null as absent
				              - compare numbers within ± 0.5
				             """)
				.Because("the options are listed in the same order, independent of the order in which they were set");
		}

		[Test]
		public async Task WithDefaultOptions_ShouldNotAddTheContext()
		{
			JsonElement subject = ThatJsonElement.FromString("[1, 3]");

			async Task Act()
				=> await That(subject).MatchesExactly([1, 2,]);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             matches [1, 2,] exactly,
				             but it differed as $[1] was 3 instead of 2
				             """)
				.Because("only active options are listed, so the context is omitted when no option is active");
		}

		[Test]
		public async Task WhenOptionIsDisabled_ShouldNotListIt()
		{
			JsonElement subject = ThatJsonElement.FromString("[1, 3]");

			async Task Act()
				=> await That(subject).Matches([1, 2,], o => o.IgnoringAdditionalProperties(false));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             matches [1, 2,] exactly,
				             but it differed as $[1] was 3 instead of 2
				             """)
				.Because("Matches no longer ignores additional properties, so no option is active");
		}

		[Test]
		public async Task WithDocumentOptions_ShouldNotListThem()
		{
			JsonElement subject = ThatJsonElement.FromString("[1, 3]");

			async Task Act()
				=> await That(subject).MatchesExactly([1, 2,],
					o => o.WithDocumentOptions(d => d with
					{
						MaxDepth = 10,
					}));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             matches [1, 2,] exactly,
				             but it differed as $[1] was 3 instead of 2
				             """)
				.Because("the document options affect the parsing, not the comparison");
		}

		[Test]
		public async Task WhenIgnoringMembers_ShouldListThemLast()
		{
			JsonElement subject = ThatJsonElement.FromString("{\"foo\": 1, \"id\": 2}");

			async Task Act()
				=> await That(subject).MatchesExactly(new
				{
					foo = 2,
				}, o => o
					.IgnoringMember("id")
					.IgnoringMembers(path => path.EndsWith(".etag"))
					.TreatingNullAsAbsent());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             matches new
				             {
				             	foo = 2,
				             } exactly,
				             but it differed as $.foo was 1 instead of 2

				             JSON options:
				              - treat null as absent
				              - ignore members: ["id", path => path.EndsWith(".etag")]
				             """);
		}
	}

	public sealed class UnmatchedMembersToIgnoreTests
	{
		[Test]
		public async Task WhenNamesDifferOnlyInCasing_ShouldTrackEachSeparately()
		{
			JsonElement subject = ThatJsonElement.FromString("{\"id\": 1, \"name\": \"a\"}");

			async Task Act()
				=> await That(subject).MatchesExactly(new
				{
					id = 2,
					name = "b",
				}, o => o.IgnoringMember("id").IgnoringMember("ID"));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             matches new
				             {
				             	id = 2,
				             	name = "b",
				             } exactly,
				             but it differed as $.name was "a" instead of "b"

				             JSON options:
				              - ignore members: ["id", "ID"]
				                "ID" matched no member
				             """)
				.Because("\"id\" matching the property does not mean that \"ID\" matched it");
		}

		[Test]
		public async Task WhenNameMatchedNoMember_ShouldSuggestTheClosestPath()
		{
			JsonElement subject = ThatJsonElement.FromString("{\"createdAt\": \"today\", \"name\": \"a\"}");

			async Task Act()
				=> await That(subject).MatchesExactly(new
				{
					createdAt = "yesterday",
					name = "a",
				}, o => o.IgnoringMember("craetedAt"));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             matches new
				             {
				             	createdAt = "yesterday",
				             	name = "a",
				             } exactly,
				             but it differed as $.createdAt was "today" instead of "yesterday"

				             JSON options:
				              - ignore members: ["craetedAt"]
				                "craetedAt" matched no member (did you mean "createdAt"?)
				             """);
		}

		[Test]
		public async Task WhenNameWithSeveralSegmentsMatchedNoMember_ShouldSuggestAsManySegments()
		{
			JsonElement subject = ThatJsonElement.FromString("{\"items\": [{\"etag\": \"a\"}]}");

			async Task Act()
				=> await That(subject).MatchesExactly(new
				{
					items = new[]
					{
						new
						{
							etag = "b",
						},
					},
				}, o => o.IgnoringMember("items[0].etga"));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             matches new
				             {
				             	items = new[]
				             	{
				             		new
				             		{
				             			etag = "b",
				             		},
				             	},
				             } exactly,
				             but it differed as $.items[0].etag was "a" instead of "b"

				             JSON options:
				              - ignore members: ["items[0].etga"]
				                "items[0].etga" matched no member (did you mean "items[0].etag"?)
				             """);
		}

		[Test]
		public async Task WhenNameMatchedNoCloseMember_ShouldOnlyNameIt()
		{
			JsonElement subject = ThatJsonElement.FromString("{\"name\": \"a\"}");

			async Task Act()
				=> await That(subject).MatchesExactly(new
				{
					name = "b",
				}, o => o.IgnoringMember("timestamp"));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             matches new
				             {
				             	name = "b",
				             } exactly,
				             but it differed as $.name was "a" instead of "b"

				             JSON options:
				              - ignore members: ["timestamp"]
				                "timestamp" matched no member
				             """);
		}

		[Test]
		public async Task WhenSeveralNamesMatchedNoMember_ShouldNameEachInTheOrderOfTheMembersToIgnore()
		{
			JsonElement subject = ThatJsonElement.FromString("{\"id\": 1, \"name\": \"a\", \"etag\": \"x\"}");

			async Task Act()
				=> await That(subject).MatchesExactly(new
				{
					id = 2,
					name = "b",
					etag = "x",
				}, o => o
					.IgnoringMember("nmae")
					.IgnoringMember("etag")
					.IgnoringMembers(_ => false)
					.IgnoringMember("idd"));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             matches new
				             {
				             	id = 2,
				             	name = "b",
				             	etag = "x",
				             } exactly,
				             but it differed as
				               $.id was 1 instead of 2 and
				               $.name was "a" instead of "b"

				             JSON options:
				              - ignore members: ["nmae", "etag", _ => false, "idd"]
				                "nmae" matched no member (did you mean "name"?)
				                "idd" matched no member (did you mean "id"?)
				             """)
				.Because("a name that matched a member and a predicate are not named");
		}

		[Test]
		public async Task WhenNoMemberWasCompared_ShouldNotNameTheMembersToIgnore()
		{
			JsonElement subject = ThatJsonElement.FromString("1");

			async Task Act()
				=> await That(subject).MatchesExactly(2, o => o.IgnoringMember("id"));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             matches 2 exactly,
				             but it differed as $ was 1 instead of 2

				             JSON options:
				              - ignore members: ["id"]
				             """)
				.Because("like in aweXpect, the members to ignore are only tracked when a member is compared");
		}

		[Test]
		public async Task WhenSucceeding_ShouldNotThrow()
		{
			JsonElement subject = ThatJsonElement.FromString("{\"createdAt\": \"today\"}");

			async Task Act()
				=> await That(subject).MatchesExactly(new
				{
					createdAt = "today",
				}, o => o.IgnoringMember("craetedAt"));

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task Matches_ForNullable_ShouldNameTheUnmatchedName()
		{
			JsonElement? subject = ThatJsonElement.FromString("{\"createdAt\": 1}");

			async Task Act()
				=> await That(subject).Matches(new
				{
					createdAt = 2,
				}, o => o.IgnoringMember("craetedAt"));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             matches new
				             {
				             	createdAt = 2,
				             },
				             but it differed as $.createdAt was 1 instead of 2

				             JSON options:
				              - ignore additional properties
				              - ignore members: ["craetedAt"]
				                "craetedAt" matched no member (did you mean "createdAt"?)
				             """);
		}

		[Test]
		public async Task MatchesExactly_ForNullable_ShouldNameTheUnmatchedName()
		{
			JsonElement? subject = ThatJsonElement.FromString("{\"createdAt\": 1}");

			async Task Act()
				=> await That(subject).MatchesExactly(new
				{
					createdAt = 2,
				}, o => o.IgnoringMember("craetedAt"));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             matches new
				             {
				             	createdAt = 2,
				             } exactly,
				             but it differed as $.createdAt was 1 instead of 2

				             JSON options:
				              - ignore members: ["craetedAt"]
				                "craetedAt" matched no member (did you mean "createdAt"?)
				             """);
		}

		[Test]
		public async Task IsValidJsonMatching_ShouldNameTheUnmatchedName()
		{
			string subject = "{\"createdAt\": 1}";

			async Task Act()
				=> await That(subject).IsValidJsonMatching(new
				{
					createdAt = 2,
				}, o => o.IgnoringMember("craetedAt"));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is valid JSON which matches new
				             {
				             	createdAt = 2,
				             },
				             but it differed as $.createdAt was 1 instead of 2

				             JSON options:
				              - ignore additional properties
				              - ignore members: ["craetedAt"]
				                "craetedAt" matched no member (did you mean "createdAt"?)
				             """);
		}

		[Test]
		public async Task AsJson_ShouldNameTheUnmatchedName()
		{
			string subject = "{\"createdAt\": 1}";

			async Task Act()
				=> await That(subject).IsEqualTo("{\"createdAt\": 2}").AsJson(o => o.IgnoringMember("craetedAt"));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is JSON equivalent to {"createdAt": 2},
				             but it differed as $.createdAt was 1 instead of 2

				             Actual:
				             {"createdAt": 1}

				             Expected:
				             {"createdAt": 2}

				             JSON options:
				              - ignore members: ["craetedAt"]
				                "craetedAt" matched no member (did you mean "createdAt"?)
				             """);
		}

		[Test]
		public async Task AsJson_ForCollections_ShouldKeepTheSuggestionOfAnEarlierItem()
		{
			string[] subject = ["{\"a\": 1, \"createdAt\": 1}", "{\"a\": 2}",];

			async Task Act()
				=> await That(subject).All().AreEqualTo("{\"a\": 3}").AsJson(o => o.IgnoringMember("craetedAt"));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is JSON equivalent to {"a": 3} for all items,
				             but none of 2 were

				             Not matching items:
				             [
				               "{\"a\": 1, \"createdAt\": 1}",
				               "{\"a\": 2}"
				             ]

				             Collection:
				             [
				               "{\"a\": 1, \"createdAt\": 1}",
				               "{\"a\": 2}"
				             ]

				             JSON options:
				              - ignore members: ["craetedAt"]
				                "craetedAt" matched no member (did you mean "createdAt"?)
				             """)
				.Because("the items are compared with the same match type, and a later item without a close member must not hide the suggestion for an earlier one");
		}

		[Test]
		public async Task AsJson_ForCollections_WhenNameMatchedInAFailedItem_ShouldNotNameIt()
		{
			string[] subject = ["{\"a\": 1, \"id\": 1}", "{\"a\": 3}",];

			async Task Act()
				=> await That(subject).All().AreEqualTo("{\"a\": 3}").AsJson(o => o.IgnoringMember("id"));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is JSON equivalent to {"a": 3} for all items,
				             but only 1 of 2 were

				             Not matching items:
				             [
				               "{\"a\": 1, \"id\": 1}"
				             ]

				             Collection:
				             [
				               "{\"a\": 1, \"id\": 1}",
				               "{\"a\": 3}"
				             ]

				             JSON options:
				              - ignore members: ["id"]
				             """)
				.Because("the name matched a member in the item that failed");
		}
	}

	public sealed class UnmatchedMembersToIgnoreNegatedTests
	{
		[Test]
		public async Task Matches_ShouldNotNameTheUnmatchedName()
		{
			JsonElement subject = ThatJsonElement.FromString("{\"createdAt\": 1}");

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.Matches(new
				{
					createdAt = 1,
				}, o => o.IgnoringMember("craetedAt")));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not match new
				             {
				             	createdAt = 1,
				             },
				             but it did

				             Actual:
				             {
				               "createdAt": 1
				             }

				             JSON options:
				              - ignore additional properties
				              - ignore members: ["craetedAt"]
				             """)
				.Because("the comparison succeeded, so no member to ignore explains a difference");
		}

		[Test]
		public async Task AsJson_ShouldNotNameTheUnmatchedName()
		{
			string subject = "{\"createdAt\": 1}";

			async Task Act()
				=> await That(subject).IsNotEqualTo("{\"createdAt\": 1}").AsJson(o => o.IgnoringMember("craetedAt"));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is not JSON equivalent to {"createdAt": 1},
				             but it was "{\"createdAt\": 1}"

				             JSON options:
				              - ignore members: ["craetedAt"]
				             """);
		}
	}

	public sealed class Tests
	{
		[Test]
		public async Task AsJson_ShouldAddTheContextAfterActualAndExpected()
		{
			string subject = "[1, 3]";

			async Task Act()
				=> await That(subject).IsEqualTo("[2, 1]").AsJson(o => o.IgnoringArrayOrder());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is JSON equivalent to [2, 1],
				             but it differed as $[1] was 3 instead of 2

				             Actual:
				             [1, 3]

				             Expected:
				             [2, 1]

				             JSON options:
				              - ignore array order
				             """);
		}

		[Test]
		public async Task AsJson_ForCollections_ShouldAddTheContext()
		{
			string[] subject = ["[1, 2]", "[1, 3]",];

			async Task Act()
				=> await That(subject).All().AreEqualTo("[2, 1]").AsJson(o => o.IgnoringArrayOrder());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is JSON equivalent to [2, 1] for all items,
				             but only 1 of 2 were

				             Not matching items:
				             [
				               "[1, 3]"
				             ]

				             Collection:
				             [
				               "[1, 2]",
				               "[1, 3]"
				             ]

				             JSON options:
				              - ignore array order
				             """);
		}

		[Test]
		public async Task AsJson_ForEqualCollections_ShouldAddTheContext()
		{
			string[] subject = ["[1, 2]", "[1, 3]",];

			async Task Act()
				=> await That(subject).IsEqualTo(["[2, 1]", "[2, 1]",]).AsJson(o => o.IgnoringArrayOrder());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection ["[2, 1]", "[2, 1]",] as JSON in order,
				             but it contained item "[1, 3]" at index 1 instead of "[2, 1]"

				             Collection:
				             [
				               "[1, 2]",
				               "[1, 3]"
				             ]

				             Expected:
				             [
				               "[2, 1]",
				               "[2, 1]"
				             ]

				             JSON options:
				              - ignore array order
				             """);
		}

		[Test]
		public async Task AsJson_InWhose_ShouldLabelTheContextWithTheMember()
		{
			JsonContainer subject = new("[1, 3]");

			async Task Act()
				=> await That(subject).Whose(x => x.Json,
					j => j.IsEqualTo("[2, 1]").AsJson(o => o.IgnoringArrayOrder()));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             whose Json is JSON equivalent to [2, 1],
				             but Json differed as $[1] was 3 instead of 2

				             Actual (Json):
				             [1, 3]

				             Expected (Json):
				             [2, 1]

				             JSON options (Json):
				              - ignore array order
				             """);
		}

		[Test]
		public async Task AsJson_WhenAnotherExpectationOnTheSubjectFails_ShouldNotAddTheContext()
		{
			string subject = "[1, 3]";

			async Task Act()
				=> await That(subject).IsEqualTo("[3, 1]").AsJson(o => o.IgnoringArrayOrder()).And.HasLength(3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is JSON equivalent to [3, 1] and has length equal to 3,
				             but it had length 6
				             """)
				.Because("the options explain only a failure of the JSON comparison");
		}

		[Test]
		public async Task IsValidJsonMatching_ShouldAddTheContext()
		{
			string subject = "[1, 3]";

			async Task Act()
				=> await That(subject).IsValidJsonMatching([2, 1,], o => o.IgnoringArrayOrder());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is valid JSON which matches [2, 1,],
				             but it differed as $[1] was 3 instead of 2

				             JSON options:
				              - ignore additional properties
				              - ignore array order
				             """);
		}

		[Test]
		public async Task IsValidJsonMatchingExactly_ShouldAddTheContext()
		{
			string subject = "[1, 3]";

			async Task Act()
				=> await That(subject).IsValidJsonMatchingExactly([2, 1,], o => o.IgnoringArrayOrder());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is valid JSON which matches [2, 1,] exactly,
				             but it differed as $[1] was 3 instead of 2

				             JSON options:
				              - ignore array order
				             """);
		}

		[Test]
		public async Task IsValidJsonWhichMatches_ShouldAddTheContextOnce()
		{
			string subject = "[1, 3]";

			async Task Act()
				=> await That(subject).IsValidJson(j => j.Matches([1, 2,]));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is valid JSON which matches [1, 2,],
				             but it differed as $[1] was 3 instead of 2

				             JSON options (it):
				              - ignore additional properties
				             """);
		}

		[Test]
		public async Task Matches_ShouldAddTheContext()
		{
			JsonElement subject = ThatJsonElement.FromString("[1, 3]");

			async Task Act()
				=> await That(subject).Matches([1, 2,]);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             matches [1, 2,],
				             but it differed as $[1] was 3 instead of 2

				             JSON options:
				              - ignore additional properties
				             """)
				.Because("Matches ignores additional properties itself");
		}

		[Test]
		public async Task Matches_ForNullable_ShouldAddTheContext()
		{
			JsonElement? subject = ThatJsonElement.FromString("[1, 3]");

			async Task Act()
				=> await That(subject).Matches([1, 2,]);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             matches [1, 2,],
				             but it differed as $[1] was 3 instead of 2

				             JSON options:
				              - ignore additional properties
				             """);
		}

		[Test]
		public async Task MatchesExactly_ForNullable_ShouldAddTheContext()
		{
			JsonElement? subject = ThatJsonElement.FromString("[1, 3]");

			async Task Act()
				=> await That(subject).MatchesExactly([2, 1,], o => o.IgnoringArrayOrder());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             matches [2, 1,] exactly,
				             but it differed as $[1] was 3 instead of 2

				             JSON options:
				              - ignore array order
				             """);
		}

		[Test]
		public async Task WhenBothFailingExpectationsHaveTheSameOptions_ShouldAddTheContextOnce()
		{
			JsonElement subject = ThatJsonElement.FromString("[1, 3]");

			async Task Act()
				=> await That(subject).Matches([1, 2,]).And.Matches([4,]);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             matches [1, 2,] and matches [4,],
				             but it differed as $[1] was 3 instead of 2 and differed as
				               $[0] was 1 instead of 4 and
				               $[1] had unexpected 3

				             JSON options:
				              - ignore additional properties
				             """);
		}

		[Test]
		public async Task WhenBothFailingExpectationsHaveTheSameTolerance_ShouldAddTheContextOnce()
		{
			JsonElement subject = ThatJsonElement.FromString("[1, 3]");

			async Task Act()
				=> await That(subject).MatchesExactly([1, 2,], o => o.ComparingNumbersWithin(0.5))
					.And.MatchesExactly([1, 4,], o => o.ComparingNumbersWithin(0.5));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             matches [1, 2,] exactly and matches [1, 4,] exactly,
				             but it differed as $[1] was 3 instead of 2 ± 0.5 and differed as $[1] was 3 instead of 4 ± 0.5

				             JSON options:
				              - compare numbers within ± 0.5
				             """);
		}

		[Test]
		public async Task WhenFailingExpectationsHaveDifferentOptions_ShouldNumberTheContexts()
		{
			JsonElement subject = ThatJsonElement.FromString("[1, 3]");

			async Task Act()
				=> await That(subject).Matches([1, 2,]).Or.MatchesExactly([2, 1,], o => o.IgnoringArrayOrder());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             matches [1, 2,] or matches [2, 1,] exactly,
				             but it differed as $[1] was 3 instead of 2

				             JSON options #1:
				              - ignore additional properties

				             JSON options #2:
				              - ignore array order
				             """);
		}

		[Test]
		public async Task WhenOnlyAnotherExpectationFails_ShouldNotAddTheContextOfTheMetExpectation()
		{
			JsonElement subject = ThatJsonElement.FromString("[1, 3]");

			async Task Act()
				=> await That(subject).Matches([1, 3,]).And.MatchesExactly([1, 2,]);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             matches [1, 3,] and matches [1, 2,] exactly,
				             but it differed as $[1] was 3 instead of 2
				             """)
				.Because("the options of Matches did not contribute to the failure");
		}

		[Test]
		public async Task WhenSucceeding_ShouldNotThrow()
		{
			JsonElement subject = ThatJsonElement.FromString("[1, 3]");

			async Task Act()
				=> await That(subject).MatchesExactly([3, 1,], o => o.IgnoringArrayOrder());

			await That(Act).DoesNotThrow();
		}

		private sealed record JsonContainer(string Json);
	}

	public sealed class NegatedTests
	{
		[Test]
		public async Task Matches_ShouldAddTheContextAfterActual()
		{
			JsonElement subject = ThatJsonElement.FromString("[1, 3]");

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.Matches([3, 1,], o => o.IgnoringArrayOrder()));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not match [3, 1,],
				             but it did

				             Actual:
				             [
				               1,
				               3
				             ]

				             JSON options:
				              - ignore additional properties
				              - ignore array order
				             """);
		}

		[Test]
		public async Task IsValidJsonMatching_ShouldAddTheContext()
		{
			string subject = "[1, 3]";

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.IsValidJsonMatching([1, 3,]));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is not valid JSON which matches [1, 3,],
				             but it was "[1, 3]"

				             JSON options:
				              - ignore additional properties
				             """);
		}

		[Test]
		public async Task AsJson_ShouldAddTheContext()
		{
			string subject = "[1, 3]";

			async Task Act()
				=> await That(subject).IsNotEqualTo("[3, 1]").AsJson(o => o.IgnoringArrayOrder());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is not JSON equivalent to [3, 1],
				             but it was "[1, 3]"

				             Expected:
				             [3, 1]

				             JSON options:
				              - ignore array order
				             """);
		}
	}
}
