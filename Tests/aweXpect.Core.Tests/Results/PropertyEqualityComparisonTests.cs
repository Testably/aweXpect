using System.Collections.Generic;
using System.Text;
using aweXpect.Core.Constraints;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Results;

public sealed class PropertyEqualityComparisonTests
{
	[Test]
	public async Task AppendExpectation_ForNegatedOperator_ShouldLeaveOutTheNegation()
	{
		PropertyEqualityComparison<double> comparison = new DoubleResult().NotEqualTo(4.5);

		string expectation = Describe(comparison);

		await That(expectation).IsEqualTo("equal to 4.5")
			.Because("the implementer places the negation where its sentence needs it");
		await That(comparison.IsNegated).IsTrue();
	}

	[Test]
	public async Task AppendExpectation_ShouldIncludeTheTolerance()
	{
		PropertyEqualityComparison<double> comparison = new DoubleResult().EqualTo(4.5);
		comparison.Options.Within(0.1);

		string expectation = Describe(comparison);

		await That(expectation).IsEqualTo("equal to 4.5 ± 0.1");
	}

	[Test]
	[Arguments(4.5, Outcome.Failure)]
	[Arguments(4.7, Outcome.Success)]
	public async Task Evaluate_ForNegatedOperator_ShouldIncludeTheNegation(double actual, Outcome expectedOutcome)
	{
		PropertyEqualityComparison<double> comparison = new DoubleResult().NotEqualTo(4.5);

		Outcome outcome = comparison.Evaluate(actual);

		await That(outcome).IsEqualTo(expectedOutcome);
	}

	[Test]
	[Arguments(4.55, Outcome.Success)]
	[Arguments(4.65, Outcome.Failure)]
	public async Task Evaluate_ShouldApplyTheToleranceSetAfterTheOperator(double actual, Outcome expectedOutcome)
	{
		PropertyEqualityComparison<double> comparison = new DoubleResult().EqualTo(4.5);
		comparison.Options.Within(0.1);

		Outcome outcome = comparison.Evaluate(actual);

		await That(outcome).IsEqualTo(expectedOutcome);
	}

	[Test]
	public async Task Validation_ShouldReceiveTheNamesOfTheParameters()
	{
		List<string> parameterNames = [];
		DecimalResult result = new((_, parameterName) => parameterNames.Add(parameterName));

		_ = result.EqualTo(1m);
		_ = result.NotEqualTo(1m);
		_ = result.NotBetween(1m).And(2m);

		await That(parameterNames).IsEqualTo(["expected", "unexpected", "minimum", "maximum",]);
	}

	private static string Describe(PropertyEqualityComparison<double> comparison)
	{
		StringBuilder sb = new();
		comparison.AppendExpectation(sb);
		return sb.ToString();
	}

	private sealed class DecimalResult(Action<decimal?, string>? validation = null)
		: IDecimalPropertyResult<PropertyComparison<decimal>, PropertyEqualityComparison<decimal>>
	{
		Action<decimal?, string>?
			IDecimalPropertyResult<PropertyComparison<decimal>, PropertyEqualityComparison<decimal>>.Validation
			=> validation;

		PropertyComparison<decimal>
			IDecimalPropertyResult<PropertyComparison<decimal>, PropertyEqualityComparison<decimal>>.Compare(
				PropertyComparison<decimal> comparison)
			=> comparison;

		PropertyEqualityComparison<decimal>
			IDecimalPropertyResult<PropertyComparison<decimal>, PropertyEqualityComparison<decimal>>.CompareEquality(
				PropertyEqualityComparison<decimal> comparison)
			=> comparison;
	}

	private sealed class DoubleResult
		: IDoublePropertyResult<PropertyComparison<double>, PropertyEqualityComparison<double>>
	{
		Action<double?, string>?
			IDoublePropertyResult<PropertyComparison<double>, PropertyEqualityComparison<double>>.Validation
			=> null;

		PropertyComparison<double>
			IDoublePropertyResult<PropertyComparison<double>, PropertyEqualityComparison<double>>.Compare(
				PropertyComparison<double> comparison)
			=> comparison;

		PropertyEqualityComparison<double>
			IDoublePropertyResult<PropertyComparison<double>, PropertyEqualityComparison<double>>.CompareEquality(
				PropertyEqualityComparison<double> comparison)
			=> comparison;
	}
}
