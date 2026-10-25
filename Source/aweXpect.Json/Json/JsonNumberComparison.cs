using System;
using System.Text;
using aweXpect.Core.Constraints;
using aweXpect.Results;

namespace aweXpect.Json;

/// <summary>
///     The comparison of an operator on the result of <c>IsNumber()</c>, either an ordering, e.g.
///     <c>GreaterThan(3)</c>, or an equality within a tolerance, e.g. <c>EqualTo(4.5)</c>.
/// </summary>
internal sealed class JsonNumberComparison
{
	private readonly Action<StringBuilder> _appendExpectation;
	private readonly Func<double?, Outcome> _evaluate;

	public JsonNumberComparison(PropertyComparison<double> comparison)
	{
		IsNegated = comparison.IsNegated;
		_appendExpectation = comparison.AppendExpectation;
		_evaluate = comparison.Evaluate;
	}

	public JsonNumberComparison(PropertyEqualityComparison<double> comparison)
	{
		IsNegated = comparison.IsNegated;
		_appendExpectation = comparison.AppendExpectation;
		_evaluate = comparison.Evaluate;
	}

	/// <summary>
	///     Whether the operator is a negated one, e.g. <c>NotGreaterThan(3)</c>.
	/// </summary>
	public bool IsNegated { get; }

	/// <summary>
	///     Appends the comparison without its negation, e.g. <c>greater than 3</c>.
	/// </summary>
	public void AppendExpectation(StringBuilder stringBuilder) => _appendExpectation(stringBuilder);

	/// <summary>
	///     Evaluates the comparison against the <paramref name="actual" /> number, including the negation.
	/// </summary>
	public Outcome Evaluate(double actual) => _evaluate(actual);
}
