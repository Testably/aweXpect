using System.Collections.Generic;
using System.Text;
using System.Threading;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Results;

public sealed class StringPropertyComparisonTests
{
	[Test]
	public async Task AppendExpectation_ForContaining_ShouldIncludeTheQuantifier()
	{
		StringPropertyComparison comparison = new StringResult().Containing("e");
		comparison.Quantifier!.AtLeast(2);

		string expectation = Describe(comparison, ExpectationGrammars.None);

		await That(expectation).IsEqualTo("containing \"e\" at least twice");
	}

	[Test]
	public async Task AppendExpectation_ForNegatedOperator_ShouldNotIncludeTheNegation()
	{
		StringPropertyComparison comparison = new StringResult().NotStartingWith("Let");

		string expectation = Describe(comparison, ExpectationGrammars.None);

		await That(expectation).IsEqualTo("starting with \"Let\"")
			.Because("the caller places the negation where its sentence needs it, like for a PropertyComparison");
	}

	[Test]
	public async Task AppendExpectation_ShouldIncludeTheOptions()
	{
		StringPropertyComparison comparison = new StringResult().EqualTo("Let It Be");
		comparison.Options.IgnoringCase();

		string expectation = Describe(comparison, ExpectationGrammars.None);

		await That(expectation).IsEqualTo("equal to \"Let It Be\" ignoring case");
	}

	[Test]
	public async Task AppendExpectation_WhenGrammarsAreActive_ShouldUseTheVerb()
	{
		StringPropertyComparison comparison = new StringResult().StartingWith("Let");

		string expectation = Describe(comparison, ExpectationGrammars.Active);

		await That(expectation).IsEqualTo("starts with \"Let\"");
	}

	[Test]
	public async Task AppendExpectation_WhenGrammarsAreNegated_ShouldIncludeTheNegation()
	{
		StringPropertyComparison comparison = new StringResult().StartingWith("Let");

		string expectation = Describe(comparison, ExpectationGrammars.Negated);

		await That(expectation).IsEqualTo("not starting with \"Let\"");
	}

	[Test]
	[Arguments("Let It Be", Outcome.Failure)]
	[Arguments("Yesterday", Outcome.Success)]
	public async Task Evaluate_ForNegatedOperator_ShouldIncludeTheNegation(string actual, Outcome expectedOutcome)
	{
		StringPropertyComparison comparison = new StringResult().NotStartingWith("Let");

		StringPropertyOutcome outcome = await Evaluate(comparison, actual);

		await That(outcome.Outcome).IsEqualTo(expectedOutcome);
	}

	[Test]
	public async Task Evaluate_WhenValueIsNullForContaining_ShouldFailBothWays()
	{
		StringPropertyComparison comparison = new StringResult().Containing("e");

		StringPropertyOutcome outcome = await Evaluate(comparison, null);

		await That(outcome.Outcome).IsEqualTo(Outcome.FailureBothWays);
	}

	[Test]
	public async Task Evaluate_WhenValueIsNullForNegatedPrefix_ShouldFailBothWays()
	{
		StringPropertyComparison comparison = new StringResult().NotStartingWith("Let");

		StringPropertyOutcome outcome = await Evaluate(comparison, null);

		await That(outcome.Outcome).IsEqualTo(Outcome.FailureBothWays)
			.Because("a null value has no prefix, so neither the expectation nor its negation can be answered");
	}

	[Test]
	public async Task IsNegated_ShouldOnlyBeSetForNegatedOperators()
	{
		StringResult result = new();

		StringPropertyComparison startingWith = result.StartingWith("e");
		StringPropertyComparison notStartingWith = result.NotStartingWith("e");
		StringPropertyComparison notContaining = result.NotContaining("e");
		StringPropertyComparison containing = result.Containing("e");

		await That(startingWith.IsNegated).IsFalse();
		await That(notStartingWith.IsNegated).IsTrue();
		await That(notContaining.IsNegated).IsTrue();
		await That(containing.IsNegated).IsFalse()
			.Because("the quantifier of Containing expresses its negation, e.g. Never()");
	}

	[Test]
	public async Task Outcome_BeforeAnEvaluation_ShouldBeUndecided()
	{
		StringPropertyOutcome outcome = default;

		await That(outcome.Outcome).IsEqualTo(Outcome.Undecided)
			.Because("an outcome that was never evaluated must not report a success");
	}

	[Test]
	public async Task Quantifier_ShouldOnlyBeSetForContaining()
	{
		StringResult result = new();

		StringPropertyComparison containing = result.Containing("e");
		StringPropertyComparison notContaining = result.NotContaining("e");
		StringPropertyComparison equalTo = result.EqualTo("e");

		await That(containing.Quantifier).IsNotNull();
		await That(notContaining.Quantifier).IsNull();
		await That(equalTo.Quantifier).IsNull();
	}

	[Test]
	public async Task ValidateExpected_WhenPatternIsInvalid_ShouldThrowArgumentException()
	{
		StringPropertyComparison comparison = new StringResult().EqualTo("[");
		comparison.Options.AsRegex();

		void Act()
			=> comparison.ValidateExpected();

		await That(Act).Throws<ArgumentException>()
			.WithParamName("expected").And
			.WithMessage("The 'expected' regex pattern is invalid: ").AsPrefix();
	}

	[Test]
	public async Task ValidateExpected_WhenPatternIsValid_ShouldNotThrow()
	{
		StringPropertyComparison comparison = new StringResult().EqualTo("L.*");
		comparison.Options.AsRegex();

		void Act()
			=> comparison.ValidateExpected();

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task Validation_ShouldReceiveTheNamesOfTheParameters()
	{
		List<string> parameterNames = [];
		StringResult result = new((_, parameterName) => parameterNames.Add(parameterName));

		_ = result.EqualTo("a");
		_ = result.NotEqualTo("a");
		_ = result.StartingWith("a");
		_ = result.NotEndingWith("a");
		_ = result.Containing("a");

		await That(parameterNames).IsEqualTo(["expected", "unexpected", "expected", "unexpected", "expected",]);
	}

	[Test]
	public async Task WhenAppendingTheResult_ShouldNameTheValue()
	{
		StringPropertyComparison comparison = new StringResult().Containing("e");
		comparison.Quantifier!.AtLeast(3);
		StringPropertyOutcome outcome = await Evaluate(comparison, "Let It Be");
		StringBuilder sb = new();

		outcome.AppendResult(sb, "title", ExpectationGrammars.None);

		await That(sb.ToString()).IsEqualTo("title contained \"e\" twice in \"Let It Be\"");
	}

	[Test]
	public async Task WhenCustomResultComparesOccurrences_ShouldOfferTheQuantifier()
	{
		Track track = new("Let It Be");

		async Task Act()
			=> await That(track).HasTitle().Containing("e").AtLeast(3);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that track
			             has title containing "e" at least 3 times,
			             but title contained "e" twice in "Let It Be"
			             """);
	}

	[Test]
	public async Task WhenCustomResultComparesThePrefix_ShouldWriteTheComparisonIntoItsMessage()
	{
		Track track = new("Yesterday");

		async Task Act()
			=> await That(track).HasTitle().StartingWith("let").IgnoringCase();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that track
			             has title starting with "let" ignoring case,
			             but title was "Yesterday", which differs at index 0:
			                ↓ (actual)
			               "Yesterday"
			               "let"
			                ↑ (expected prefix)
			             """);
	}

	[Test]
	public async Task WhenCustomResultIsNegated_ShouldCombineBothNegations()
	{
		Track track = new("Let It Be");

		async Task Act()
			=> await That(track).DoesNotComplyWith(t => t.HasTitle().NotEqualTo("Yesterday"));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that track
			             does not have title not equal to "Yesterday",
			             but title was "Let It Be", which differs at index 0:
			                ↓ (actual)
			               "Let It Be"
			               "Yesterday"
			                ↑ (expected)
			             """)
			.Because("the negation of the expectation cancels the negation of the operator");
	}

	[Test]
	public async Task WhenCustomResultMatches_ShouldSucceed()
	{
		Track track = new("Let It Be");

		async Task Act()
			=> await That(track).HasTitle().EqualTo("Let*").AsWildcard();

		await That(Act).DoesNotThrow();
	}

	private static string Describe(StringPropertyComparison comparison, ExpectationGrammars grammars)
	{
		StringBuilder sb = new();
		comparison.AppendExpectation(sb, grammars);
		return sb.ToString();
	}

	private static async Task<StringPropertyOutcome> Evaluate(StringPropertyComparison comparison, string? actual)
		=> await comparison.Evaluate(actual, new EvaluationContext.EvaluationContext(), CancellationToken.None);

	private sealed class StringResult(Action<string?, string>? validation = null)
		: IStringEqualityPropertyResult<StringPropertyComparison>,
			IStringAffixPropertyResult<StringPropertyComparison>,
			IStringOccurrencePropertyResult<StringPropertyComparison>
	{
		Action<string?, string>? IStringAffixPropertyResult<StringPropertyComparison>.Validation => validation;

		StringPropertyComparison IStringAffixPropertyResult<StringPropertyComparison>.Compare(
			StringPropertyComparison comparison)
			=> comparison;

		Action<string?, string>? IStringEqualityPropertyResult<StringPropertyComparison>.Validation => validation;

		StringPropertyComparison IStringEqualityPropertyResult<StringPropertyComparison>.Compare(
			StringPropertyComparison comparison)
			=> comparison;

		Action<string?, string>? IStringOccurrencePropertyResult<StringPropertyComparison>.Validation => validation;

		StringPropertyComparison IStringOccurrencePropertyResult<StringPropertyComparison>.Compare(
			StringPropertyComparison comparison)
			=> comparison;
	}
}

file sealed class Track(string title)
{
	public string Title { get; } = title;
}

file sealed class TitleResult(IThat<Track> subject)
	: IStringEqualityPropertyResult<StringEqualityTypeResult<Track, IThat<Track>>>,
		IStringAffixPropertyResult<StringEqualityResult<Track, IThat<Track>>>,
		IStringOccurrencePropertyResult<StringOccurrenceCountResult<Track, IThat<Track>>>
{
	Action<string?, string>? IStringAffixPropertyResult<StringEqualityResult<Track, IThat<Track>>>.Validation
		=> null;

	StringEqualityResult<Track, IThat<Track>> IStringAffixPropertyResult<StringEqualityResult<Track, IThat<Track>>>.
		Compare(StringPropertyComparison comparison)
		=> new(AddConstraint(comparison), subject, comparison.Options);

	Action<string?, string>? IStringEqualityPropertyResult<StringEqualityTypeResult<Track, IThat<Track>>>.Validation
		=> null;

	StringEqualityTypeResult<Track, IThat<Track>>
		IStringEqualityPropertyResult<StringEqualityTypeResult<Track, IThat<Track>>>.Compare(
			StringPropertyComparison comparison)
		=> new(AddConstraint(comparison), subject, comparison.Options);

	Action<string?, string>? IStringOccurrencePropertyResult<StringOccurrenceCountResult<Track, IThat<Track>>>.
		Validation => null;

	StringOccurrenceCountResult<Track, IThat<Track>>
		IStringOccurrencePropertyResult<StringOccurrenceCountResult<Track, IThat<Track>>>.Compare(
			StringPropertyComparison comparison)
		=> new(AddConstraint(comparison), subject, comparison.Quantifier!, comparison.Options);

	private ExpectationBuilder AddConstraint(StringPropertyComparison comparison)
		=> subject.Get().ExpectationBuilder.AddConstraint(comparison,
			static (c, it, grammars) => new HasTitleConstraint(it, grammars, c));

	private sealed class HasTitleConstraint(string it, ExpectationGrammars grammars, StringPropertyComparison comparison)
		: ConstraintResult.WithNotNullValue<Track>(it, grammars),
			IAsyncContextConstraint<Track>
	{
		private StringPropertyOutcome _outcome;

		public async ValueTask<ConstraintResult> IsMetBy(Track actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			_outcome = await comparison.Evaluate(actual.Title, context, cancellationToken);
			Outcome = _outcome.Outcome;
			return this;
		}

		public override void AppendContexts(ResultContextCollector contexts)
			=> _outcome.AppendContexts(contexts);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("has title ");
			comparison.AppendExpectation(stringBuilder, OperatorGrammars(ExpectationGrammars.None));
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> _outcome.AppendResult(stringBuilder, "title", OperatorGrammars(Grammars), indentation);

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("does not have title ");
			comparison.AppendExpectation(stringBuilder, OperatorGrammars(ExpectationGrammars.None));
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> _outcome.AppendResult(stringBuilder, "title", OperatorGrammars(Grammars), indentation);

		private ExpectationGrammars OperatorGrammars(ExpectationGrammars grammars)
			=> comparison.IsNegated ? grammars.Negate() : grammars;
	}
}

file static class TitleExpectations
{
	public static TitleResult HasTitle(this IThat<Track> subject) => new(subject);
}
