using System.Collections.Generic;
using System.Text;
using aweXpect.Core.Constraints;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Results;

public sealed class PropertyComparisonTests
{
	[Test]
	public async Task AppendExpectation_ForBetween_ShouldIncludeMinimumAndMaximum()
	{
		PropertyComparison<int> comparison = new IntResult().Between(1).And(3);

		string expectation = Describe(comparison);

		await That(expectation).IsEqualTo("between 1 and 3");
	}

	[Test]
	public async Task AppendExpectation_ForNegatedOperator_ShouldLeaveOutTheNegation()
	{
		PropertyComparison<int> comparison = new IntResult().NotGreaterThan(2);

		string expectation = Describe(comparison);

		await That(expectation).IsEqualTo("greater than 2")
			.Because("the implementer places the negation where its sentence needs it");
		await That(comparison.IsNegated).IsTrue();
	}

	[Test]
	public async Task AppendExpectation_ForSatisfying_ShouldWriteOnlyTheExpectation()
	{
		PropertyComparison<int> comparison = PropertyComparison<int>.Satisfying(_ => false, "even", true);

		string expectation = Describe(comparison);

		await That(expectation).IsEqualTo("even")
			.Because("there is no expected value to format and the negation is placed by the implementer");
		await That(comparison.IsNegated).IsTrue();
	}

	[Test]
	[Arguments(2, Outcome.Success)]
	[Arguments(3, Outcome.Failure)]
	public async Task Evaluate_ForNegatedOperator_ShouldIncludeTheNegation(int actual, Outcome expectedOutcome)
	{
		PropertyComparison<int> comparison = new IntResult().NotGreaterThan(2);

		Outcome outcome = comparison.Evaluate(actual);

		await That(outcome).IsEqualTo(expectedOutcome);
	}

	[Test]
	[Arguments(2, Outcome.Failure)]
	[Arguments(3, Outcome.Success)]
	[Arguments(null, Outcome.Success)]
	public async Task Evaluate_ForNegatedSatisfying_ShouldNegateThePredicate(int? actual, Outcome expectedOutcome)
	{
		PropertyComparison<int> comparison = PropertyComparison<int>.Satisfying(v => v % 2 == 0, "even", true);

		Outcome outcome = comparison.Evaluate(actual);

		await That(outcome).IsEqualTo(expectedOutcome);
	}

	[Test]
	[Arguments(2, Outcome.Success)]
	[Arguments(3, Outcome.Failure)]
	[Arguments(null, Outcome.Failure)]
	public async Task Evaluate_ForSatisfying_ShouldUseThePredicate(int? actual, Outcome expectedOutcome)
	{
		PropertyComparison<int> comparison = PropertyComparison<int>.Satisfying(v => v % 2 == 0, "even");

		Outcome outcome = comparison.Evaluate(actual);

		await That(outcome).IsEqualTo(expectedOutcome);
	}

	[Test]
	public async Task Evaluate_WhenOrderedAgainstNull_ShouldFailBothWays()
	{
		PropertyComparison<int> comparison = new IntResult().GreaterThan(null);

		Outcome outcome = comparison.Evaluate(2);

		await That(outcome).IsEqualTo(Outcome.FailureBothWays)
			.Because("an ordering against null can be answered neither way");
	}

	[Test]
	public async Task Satisfying_WhenExpectationIsNull_ShouldThrowArgumentNullException()
	{
		void Act()
			=> _ = PropertyComparison<int>.Satisfying(_ => true, null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("expectation").And
			.WithMessage("The 'expectation' cannot be null.").AsPrefix();
	}

	[Test]
	public async Task Satisfying_WhenPredicateIsNull_ShouldThrowArgumentNullException()
	{
		void Act()
			=> _ = PropertyComparison<int>.Satisfying(null!, "even");

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("predicate").And
			.WithMessage("The 'predicate' cannot be null.").AsPrefix();
	}

	[Test]
	public async Task Validation_ShouldReceiveTheNamesOfTheParameters()
	{
		List<string> parameterNames = [];
		IntResult result = new((_, parameterName) => parameterNames.Add(parameterName));

		_ = result.EqualTo(1);
		_ = result.NotEqualTo(1);
		_ = result.NotBetween(1).And(2);

		await That(parameterNames).IsEqualTo(["expected", "unexpected", "minimum", "maximum",]);
	}

	private static string Describe(PropertyComparison<int> comparison)
	{
		StringBuilder sb = new();
		comparison.AppendExpectation(sb);
		return sb.ToString();
	}

	private sealed class IntResult(Action<int?, string>? validation = null)
		: IIntPropertyResult<PropertyComparison<int>>
	{
		Action<int?, string>? IIntPropertyResult<PropertyComparison<int>>.Validation => validation;

		PropertyComparison<int> IIntPropertyResult<PropertyComparison<int>>.Compare(PropertyComparison<int> comparison)
			=> comparison;
	}
}
