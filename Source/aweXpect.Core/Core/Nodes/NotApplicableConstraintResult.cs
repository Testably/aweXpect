using System.Text;
using aweXpect.Core.Constraints;

namespace aweXpect.Core.Nodes;

/// <summary>
///     The result of the expectations of the <paramref name="expectationResult" />, which were not applied, because the
///     member has a different runtime type than the narrowed type they are written for, or the expectation in front of
///     them already failed.
/// </summary>
/// <remarks>
///     It only contributes the expectation text, so that the expectation in front of it reports the failure on its own.
/// </remarks>
/// <param name="expectationResult">
///     The result of the expectations, evaluated only for their expectation text, which follows a negation.
/// </param>
internal sealed class NotApplicableConstraintResult(ConstraintResult expectationResult)
	: ConstraintResult(FurtherProcessingStrategy.Continue)
{
	/// <inheritdoc cref="ConstraintResult.AppendExpectation(StringBuilder, string?)" />
	public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> expectationResult.AppendExpectation(stringBuilder, indentation);

	/// <inheritdoc cref="ConstraintResult.AppendResult(StringBuilder, string?)" />
	public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
	{
	}

	/// <inheritdoc cref="ConstraintResult.IsExpectationOnly" />
	/// <remarks>
	///     The expectations were not applied, so they must not turn an expectation that fails both ways into an undecided
	///     one under negation.
	/// </remarks>
	internal override bool IsExpectationOnly => true;

	/// <inheritdoc cref="ConstraintResult.TryGetStoredValue{TValue}(out TValue)" />
	public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
	{
		value = default;
		return false;
	}

	/// <inheritdoc cref="ConstraintResult.Negate()" />
	public override ConstraintResult Negate()
	{
		expectationResult.Negate();
		return this;
	}
}
