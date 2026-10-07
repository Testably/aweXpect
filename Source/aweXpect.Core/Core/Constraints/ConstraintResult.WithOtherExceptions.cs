using System;
using System.Text;

namespace aweXpect.Core.Constraints;

/// <summary>
///     The result of the check if an expectation is met.
/// </summary>
public abstract partial class ConstraintResult
{
	/// <summary>
	///     The <paramref name="inner" /> result of an expectation whose subject faulted with several exceptions, which
	///     lists the <paramref name="otherExceptions" /> besides the first one as context.
	/// </summary>
	internal sealed class WithOtherExceptions(ConstraintResult inner, Exception[] otherExceptions)
		: ConstraintResult(inner.FurtherProcessingStrategy)
	{
		/// <inheritdoc cref="ConstraintResult.Outcome" />
		public override Outcome Outcome => inner.Outcome;

		/// <inheritdoc cref="ConstraintResult.FailureCause" />
		public override Exception? FailureCause => inner.FailureCause;

		/// <inheritdoc cref="ConstraintResult.LeadingSubject" />
		public override string? LeadingSubject => inner.LeadingSubject;

		/// <inheritdoc cref="ConstraintResult.TrailingSubject" />
		public override string? TrailingSubject => inner.TrailingSubject;

		/// <inheritdoc cref="ConstraintResult.IsExpectationOnly" />
		internal override bool IsExpectationOnly => inner.IsExpectationOnly;

		/// <inheritdoc cref="ConstraintResult.AppendExpectation(StringBuilder, string?)" />
		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> inner.AppendExpectation(stringBuilder, indentation);

		/// <inheritdoc cref="ConstraintResult.AppendResult(StringBuilder, string?)" />
		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
			=> inner.AppendResult(stringBuilder, indentation);

		/// <inheritdoc cref="ConstraintResult.TryGetStoredValue{TValue}(out TValue)" />
		public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
			=> inner.TryGetStoredValue(out value);

		/// <inheritdoc cref="ConstraintResult.Negate()" />
		public override ConstraintResult Negate()
		{
			inner.Negate();
			return this;
		}

		/// <inheritdoc cref="ConstraintResult.AppendContexts(ResultContextCollector)" />
		public override void AppendContexts(ResultContextCollector contexts)
		{
			contexts.Visit(inner);
			contexts.Add(CreateContext(otherExceptions));
		}

		/// <summary>
		///     The "Other exceptions" context, which lists the <paramref name="otherExceptions" /> of a faulted task.
		/// </summary>
		internal static ResultContext CreateContext(Exception[] otherExceptions)
			=> new ResultContext.SyncCallback("Other exceptions",
				() => Formatter.Format(otherExceptions, FormattingOptions.MultipleLines));
	}
}
