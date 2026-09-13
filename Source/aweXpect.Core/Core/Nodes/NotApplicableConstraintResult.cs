using System.Diagnostics.CodeAnalysis;
using System.Text;
using aweXpect.Core.Constraints;

namespace aweXpect.Core.Nodes;

/// <summary>
///     The result of the expectations of <paramref name="node" />, which were not applied, because the member has a
///     different runtime type than the narrowed type they are written for.
/// </summary>
/// <remarks>
///     It stays <see cref="Outcome.Undecided" />, so that the expectation which narrowed the type reports the mismatch
///     on its own.
/// </remarks>
internal sealed class NotApplicableConstraintResult(Node node)
	: ConstraintResult(FurtherProcessingStrategy.Continue)
{
	/// <inheritdoc cref="ConstraintResult.AppendExpectation(StringBuilder, string?)" />
	public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> node.AppendExpectation(stringBuilder, indentation);

	/// <inheritdoc cref="ConstraintResult.AppendResult(StringBuilder, string?)" />
	public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
	{
	}

	/// <inheritdoc cref="ConstraintResult.TryGetValue{TValue}(out TValue)" />
	public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
	{
		value = default;
		return false;
	}

	/// <inheritdoc cref="ConstraintResult.Negate()" />
	public override ConstraintResult Negate() => this;
}
