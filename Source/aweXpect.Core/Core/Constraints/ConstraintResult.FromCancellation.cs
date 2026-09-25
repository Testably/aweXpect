using System.Diagnostics.CodeAnalysis;
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
		public override Outcome Outcome
		{
			get => Outcome.Undecided;

			// The outcome of a canceled expectation is always undecided, so the value is discarded.
			protected set => _ = value;
		}

		/// <inheritdoc cref="ConstraintResult.AppendExpectation(StringBuilder, string?)" />
		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> inner.AppendExpectation(stringBuilder, indentation);

		/// <inheritdoc cref="ConstraintResult.AppendResult(StringBuilder, string?)" />
		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("it").Append(CancelledResultSuffix);

		/// <inheritdoc cref="ConstraintResult.TryGetValue{TValue}(out TValue)" />
		public override bool TryGetValue<T>([NotNullWhen(true)] out T? value) where T : default
			=> inner.TryGetValue(out value);

		/// <inheritdoc cref="ConstraintResult.Negate()" />
		public override ConstraintResult Negate()
		{
			inner.Negate();
			return this;
		}
	}
}
