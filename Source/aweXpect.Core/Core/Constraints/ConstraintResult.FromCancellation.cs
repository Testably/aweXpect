using System.Text;

namespace aweXpect.Core.Constraints;

/// <summary>
///     The result of the check if an expectation is met.
/// </summary>
public abstract partial class ConstraintResult
{
	/// <summary>
	///     An undecided <see cref="ConstraintResult" /> for an expectation whose evaluation was canceled by the caller
	///     before it could be verified.
	/// </summary>
	internal sealed class FromCancellation(ConstraintResult inner) : ConstraintResult(inner.Grammars)
	{
		/// <inheritdoc cref="ConstraintResult.Outcome" />
		public override Outcome Outcome => Outcome.Undecided;

		/// <inheritdoc cref="ConstraintResult.AppendExpectation(StringBuilder, string?)" />
		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> inner.AppendExpectation(stringBuilder, indentation);

		/// <inheritdoc cref="ConstraintResult.AppendResult(StringBuilder, string?)" />
		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendCanceledResult(stringBuilder, "it");

		/// <inheritdoc cref="ConstraintResult.TryGetStoredValue{TValue}(out TValue)" />
		public override bool TryGetStoredValue<T>(out T? value) where T : default
			=> inner.TryGetStoredValue(out value);

		/// <inheritdoc cref="ConstraintResult.AppendContexts(ResultContextCollector)" />
		/// <remarks>
		///     The constraints that were evaluated until then describe what the expectation saw.
		/// </remarks>
		public override void AppendContexts(ResultContextCollector contexts)
			=> contexts.Visit(inner);

		/// <inheritdoc cref="ConstraintResult.Negate()" />
		public override ConstraintResult Negate()
		{
			inner.Negate();
			return this;
		}
	}
}
