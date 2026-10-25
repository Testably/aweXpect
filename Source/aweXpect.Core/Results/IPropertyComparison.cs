using System.Text;
using aweXpect.Core.Constraints;

namespace aweXpect.Results;

/// <summary>
///     The comparison of a <typeparamref name="TProperty" /> property that the constraint of a property result
///     evaluates and writes, whether it compares by order or within a tolerance.
/// </summary>
internal interface IPropertyComparison<TProperty>
	where TProperty : struct
{
	bool IsNegated { get; }

	void AppendExpectation(StringBuilder stringBuilder);

	Outcome Evaluate(TProperty? actual);
}
