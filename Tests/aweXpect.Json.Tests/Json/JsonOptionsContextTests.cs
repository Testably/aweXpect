using System.Text.Json;

namespace aweXpect.Json.Tests;

public sealed class JsonOptionsContextTests
{
	public sealed class RenderingTests
	{
		[Fact]
		public async Task WhenIgnoringAdditionalProperties_ShouldListIt()
		{
			JsonElement subject = ThatJsonElement.FromString("{\"foo\": 1, \"bar\": 2}");

			async Task Act()
				=> await That(subject).MatchesExactly(new
				{
					foo = 2,
				}, o => o.IgnoringAdditionalProperties());

			await That(Act).Throws<XunitException>()
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

		[Fact]
		public async Task WhenIgnoringAdditionalArrayElements_ShouldListIt()
		{
			JsonElement subject = ThatJsonElement.FromString("[1, 3, 4]");

			async Task Act()
				=> await That(subject).MatchesExactly([1, 2,], o => o.IgnoringAdditionalArrayElements());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             matches [1, 2,],
				             but it differed as $[1] was 3 instead of 2

				             JSON options:
				              - ignore additional array elements
				             """);
		}

		[Fact]
		public async Task WhenIgnoringArrayOrder_ShouldListIt()
		{
			JsonElement subject = ThatJsonElement.FromString("[1, 3]");

			async Task Act()
				=> await That(subject).MatchesExactly([2, 1,], o => o.IgnoringArrayOrder());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             matches [2, 1,] exactly,
				             but it differed as $[1] was 3 instead of 2

				             JSON options:
				              - ignore array order
				             """);
		}

		[Fact]
		public async Task WhenTreatingNullAsAbsent_ShouldListIt()
		{
			JsonElement subject = ThatJsonElement.FromString("{\"foo\": 1, \"bar\": null}");

			async Task Act()
				=> await That(subject).MatchesExactly(new
				{
					foo = 2,
				}, o => o.TreatingNullAsAbsent());

			await That(Act).Throws<XunitException>()
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

		[Fact]
		public async Task WhenAllOptionsAreActive_ShouldListThemInAFixedOrder()
		{
			JsonElement subject = ThatJsonElement.FromString("[1, 3]");

			async Task Act()
				=> await That(subject).MatchesExactly([2, 1,], o => o
					.TreatingNullAsAbsent()
					.IgnoringArrayOrder()
					.IgnoringAdditionalArrayElements()
					.IgnoringAdditionalProperties());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             matches [2, 1,],
				             but it differed as $[1] was 3 instead of 2

				             JSON options:
				              - ignore additional properties
				              - ignore additional array elements
				              - ignore array order
				              - treat null as absent
				             """)
				.Because("the options are listed in the same order, independent of the order in which they were set");
		}

		[Fact]
		public async Task WithDefaultOptions_ShouldNotAddTheContext()
		{
			JsonElement subject = ThatJsonElement.FromString("[1, 3]");

			async Task Act()
				=> await That(subject).MatchesExactly([1, 2,]);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             matches [1, 2,] exactly,
				             but it differed as $[1] was 3 instead of 2
				             """)
				.Because("only active options are listed, so the context is omitted when no option is active");
		}

		[Fact]
		public async Task WhenOptionIsDisabled_ShouldNotListIt()
		{
			JsonElement subject = ThatJsonElement.FromString("[1, 3]");

			async Task Act()
				=> await That(subject).Matches([1, 2,], o => o.IgnoringAdditionalProperties(false));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             matches [1, 2,] exactly,
				             but it differed as $[1] was 3 instead of 2
				             """)
				.Because("Matches no longer ignores additional properties, so no option is active");
		}

		[Fact]
		public async Task WithDocumentOptions_ShouldNotListThem()
		{
			JsonElement subject = ThatJsonElement.FromString("[1, 3]");

			async Task Act()
				=> await That(subject).MatchesExactly([1, 2,],
					o => o.WithJsonOptions(d => d with
					{
						MaxDepth = 10,
					}));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             matches [1, 2,] exactly,
				             but it differed as $[1] was 3 instead of 2
				             """)
				.Because("the document options affect the parsing, not the comparison");
		}
	}

	public sealed class Tests
	{
		[Fact]
		public async Task AsJson_ShouldAddTheContextAfterActualAndExpected()
		{
			string subject = "[1, 3]";

			async Task Act()
				=> await That(subject).IsEqualTo("[2, 1]").AsJson(o => o.IgnoringArrayOrder());

			await That(Act).Throws<XunitException>()
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

		[Fact]
		public async Task AsJson_ForCollections_ShouldAddTheContext()
		{
			string[] subject = ["[1, 2]", "[1, 3]",];

			async Task Act()
				=> await That(subject).All().AreEqualTo("[2, 1]").AsJson(o => o.IgnoringArrayOrder());

			await That(Act).Throws<XunitException>()
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

		[Fact]
		public async Task AsJson_InWhose_ShouldLabelTheContextWithTheMember()
		{
			JsonContainer subject = new("[1, 3]");

			async Task Act()
				=> await That(subject).Whose(x => x.Json,
					j => j.IsEqualTo("[2, 1]").AsJson(o => o.IgnoringArrayOrder()));

			await That(Act).Throws<XunitException>()
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

		[Fact]
		public async Task IsValidJsonMatching_ShouldAddTheContext()
		{
			string subject = "[1, 3]";

			async Task Act()
				=> await That(subject).IsValidJsonMatching([2, 1,], o => o.IgnoringArrayOrder());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is valid JSON which matches [2, 1,],
				             but it differed as $[1] was 3 instead of 2

				             JSON options:
				              - ignore additional properties
				              - ignore array order
				             """);
		}

		[Fact]
		public async Task IsValidJsonMatchingExactly_ShouldAddTheContext()
		{
			string subject = "[1, 3]";

			async Task Act()
				=> await That(subject).IsValidJsonMatchingExactly([2, 1,], o => o.IgnoringArrayOrder());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is valid JSON which matches [2, 1,] exactly,
				             but it differed as $[1] was 3 instead of 2

				             JSON options:
				              - ignore array order
				             """);
		}

		[Fact]
		public async Task IsValidJsonWhichMatches_ShouldAddTheContextOnce()
		{
			string subject = "[1, 3]";

			async Task Act()
				=> await That(subject).IsValidJson().Which(j => j.Matches([1, 2,]));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is valid JSON which matches [1, 2,],
				             but it differed as $[1] was 3 instead of 2

				             JSON options (it):
				              - ignore additional properties
				             """);
		}

		[Fact]
		public async Task Matches_ShouldAddTheContext()
		{
			JsonElement subject = ThatJsonElement.FromString("[1, 3]");

			async Task Act()
				=> await That(subject).Matches([1, 2,]);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             matches [1, 2,],
				             but it differed as $[1] was 3 instead of 2

				             JSON options:
				              - ignore additional properties
				             """)
				.Because("Matches ignores additional properties itself");
		}

		[Fact]
		public async Task Matches_ForNullable_ShouldAddTheContext()
		{
			JsonElement? subject = ThatJsonElement.FromString("[1, 3]");

			async Task Act()
				=> await That(subject).Matches([1, 2,]);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             matches [1, 2,],
				             but it differed as $[1] was 3 instead of 2

				             JSON options:
				              - ignore additional properties
				             """);
		}

		[Fact]
		public async Task MatchesExactly_ForNullable_ShouldAddTheContext()
		{
			JsonElement? subject = ThatJsonElement.FromString("[1, 3]");

			async Task Act()
				=> await That(subject).MatchesExactly([2, 1,], o => o.IgnoringArrayOrder());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             matches [2, 1,] exactly,
				             but it differed as $[1] was 3 instead of 2

				             JSON options:
				              - ignore array order
				             """);
		}

		[Fact]
		public async Task IsObject_ShouldAddTheContextOnceForNestedObjects()
		{
			JsonElement subject = ThatJsonElement.FromString("{\"foo\": {\"bar\": [1, 3]}}");

			async Task Act()
				=> await That(subject).IsObject(o => o
						.With("foo").AnObject(foo => foo.With("bar").Matching(new[] { 2, 1, })),
					o => o.IgnoringArrayOrder());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is an object and $.foo is an object and $.foo.bar matches new[] { 2, 1, },
				             but it differed as $.foo.bar[1] was 3 instead of 2

				             JSON options:
				              - ignore additional properties
				              - ignore array order
				             """);
		}

		[Fact]
		public async Task IsObject_ForNullable_ShouldAddTheContext()
		{
			JsonElement? subject = ThatJsonElement.FromString("{\"foo\": 1}");

			async Task Act()
				=> await That(subject).IsObject(o => o.With("foo").Matching(2), o => o.TreatingNullAsAbsent());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is an object and $.foo matches 2,
				             but it differed as $.foo was 1 instead of 2

				             JSON options:
				              - ignore additional properties
				              - treat null as absent
				             """);
		}

		[Fact]
		public async Task IsArray_ShouldAddTheContext()
		{
			JsonElement subject = ThatJsonElement.FromString("[1, 3]");

			async Task Act()
				=> await That(subject).IsArray(a => a.At(1).Matching(2));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is an array and $[1] matches 2,
				             but it differed as $[1] was 3 instead of 2

				             JSON options:
				              - ignore additional properties
				             """);
		}

		[Fact]
		public async Task IsArray_ForNullable_ShouldAddTheContext()
		{
			JsonElement? subject = ThatJsonElement.FromString("[1, 3]");

			async Task Act()
				=> await That(subject).IsArray(a => a.At(1).Matching(2), o => o.IgnoringArrayOrder());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is an array and $[1] matches 2,
				             but it differed as $[1] was 3 instead of 2

				             JSON options:
				              - ignore additional properties
				              - ignore array order
				             """);
		}

		[Fact]
		public async Task WhenBothFailingExpectationsHaveTheSameOptions_ShouldAddTheContextOnce()
		{
			JsonElement subject = ThatJsonElement.FromString("[1, 3]");

			async Task Act()
				=> await That(subject).Matches([1, 2,]).And.Matches([4,]);

			await That(Act).Throws<XunitException>()
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

		[Fact]
		public async Task WhenFailingExpectationsHaveDifferentOptions_ShouldNumberTheContexts()
		{
			JsonElement subject = ThatJsonElement.FromString("[1, 3]");

			async Task Act()
				=> await That(subject).Matches([1, 2,]).Or.MatchesExactly([2, 1,], o => o.IgnoringArrayOrder());

			await That(Act).Throws<XunitException>()
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

		[Fact]
		public async Task WhenOnlyAnotherExpectationFails_ShouldNotAddTheContextOfTheMetExpectation()
		{
			JsonElement subject = ThatJsonElement.FromString("[1, 3]");

			async Task Act()
				=> await That(subject).Matches([1, 3,]).And.MatchesExactly([1, 2,]);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             matches [1, 3,] and matches [1, 2,] exactly,
				             but it differed as $[1] was 3 instead of 2
				             """)
				.Because("the options of Matches did not contribute to the failure");
		}

		[Fact]
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
		[Fact]
		public async Task Matches_ShouldAddTheContextAfterActual()
		{
			JsonElement subject = ThatJsonElement.FromString("[1, 3]");

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.Matches([3, 1,], o => o.IgnoringArrayOrder()));

			await That(Act).Throws<XunitException>()
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

		[Fact]
		public async Task IsValidJsonMatching_ShouldAddTheContext()
		{
			string subject = "[1, 3]";

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.IsValidJsonMatching([1, 3,]));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is not valid JSON which matches [1, 3,],
				             but it was "[1, 3]"

				             JSON options:
				              - ignore additional properties
				             """);
		}

		[Fact]
		public async Task IsObject_ShouldAddTheContext()
		{
			JsonElement subject = ThatJsonElement.FromString("{\"foo\": 1, \"bar\": null}");

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it
					=> it.IsObject(o => o.Without("bar"), o => o.TreatingNullAsAbsent()));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is not an object or $.bar exists,
				             but it was

				             Actual:
				             {
				               "foo": 1,
				               "bar": null
				             }

				             JSON options:
				              - ignore additional properties
				              - treat null as absent
				             """);
		}

		[Fact]
		public async Task AsJson_ShouldAddTheContext()
		{
			string subject = "[1, 3]";

			async Task Act()
				=> await That(subject).IsNotEqualTo("[3, 1]").AsJson(o => o.IgnoringArrayOrder());

			await That(Act).Throws<XunitException>()
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
