using System.Text;

namespace aweXpect.Core.Constraints;

/// <summary>
///     The result of the check if an expectation is met.
/// </summary>
public abstract partial class ConstraintResult
{
	/// <summary>
	///     A failed <see cref="ConstraintResult" /> for an expectation whose outcome stayed undecided, although its
	///     evaluation was not canceled.
	/// </summary>
	/// <remarks>
	///     Every result starts undecided, so this is a constraint that did not set its outcome, e.g. in one of its
	///     branches.
	/// </remarks>
	internal sealed class WithoutDecision(ConstraintResult inner) : ConstraintResult(inner.Grammars)
	{
		/// <inheritdoc cref="ConstraintResult.Outcome" />
		public override Outcome Outcome => Outcome.Failure;

		/// <inheritdoc cref="ConstraintResult.AppendExpectation(StringBuilder, string?)" />
		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> inner.AppendExpectation(stringBuilder, indentation);

		/// <inheritdoc cref="ConstraintResult.AppendResult(StringBuilder, string?)" />
		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("it could not be verified, because the expectation did not decide its outcome");

		/// <inheritdoc cref="ConstraintResult.TryGetStoredValue{TValue}(out TValue)" />
		public override bool TryGetStoredValue<T>(out T? value) where T : default
			=> inner.TryGetStoredValue(out value);

		/// <inheritdoc cref="ConstraintResult.AppendContexts(ResultContextCollector)" />
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
