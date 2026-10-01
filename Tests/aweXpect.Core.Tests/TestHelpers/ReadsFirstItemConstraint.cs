using System.Collections.Generic;
using System.Linq;
using System.Text;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;

namespace aweXpect.Core.Tests.TestHelpers;

/// <summary>
///     Reads only the first item of the materialized subject, and lists the subject in its result, which reads further
///     items while the failure message is created.
/// </summary>
internal sealed class ReadsFirstItemConstraint(DisposeTrackingEnumerable source, Outcome outcome)
	: ConstraintResult(FurtherProcessingStrategy.Continue),
		IContextConstraint<IEnumerable<int>>
{
	private IEnumerable<int>? _materialized;

	/// <summary>
	///     How often the <paramref name="source" /> was disposed when the subject was listed in the result.
	/// </summary>
	public int? DisposeCountWhenListed { get; private set; }

	public ConstraintResult IsMetBy(IEnumerable<int> actual, IEvaluationContext context)
	{
		_materialized = context.UseMaterializedEnumerable(actual);
		_ = _materialized.First();
		Outcome = outcome;
		return this;
	}

	public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append("reads the first item");

	public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
	{
		DisposeCountWhenListed = source.DisposeCount;
		stringBuilder.Append("it was ");
		Formatter.Format(stringBuilder, _materialized);
	}

	public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
	{
		value = default;
		return false;
	}

	public override ConstraintResult Negate() => this;
}
