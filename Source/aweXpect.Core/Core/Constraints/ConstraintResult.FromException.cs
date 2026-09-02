using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using aweXpect.Core.Helpers;

namespace aweXpect.Core.Constraints;

/// <summary>
///     The result of the check if an expectation is met.
/// </summary>
public abstract partial class ConstraintResult
{
#pragma warning disable S2166 // Rename this class to remove "Exception" or correct its inheritance
	/// <summary>
	///     A failed <see cref="ConstraintResult" /> due to an <see cref="Exception" />.
	/// </summary>
	internal class FromException : ConstraintResult
	{
		private readonly Exception _exception;
		private readonly ConstraintResult _inner;

		/// <summary>
		///     A failed <see cref="ConstraintResult" /> due to a thrown <paramref name="exception" />.
		/// </summary>
		public FromException(
			ConstraintResult inner,
			Exception exception,
			ExpectationBuilder expectationBuilder)
			: base(inner.Grammars)
		{
			_inner = inner;
			_exception = exception;
			FurtherProcessingStrategy = inner.FurtherProcessingStrategy;
			expectationBuilder.AddContext(new ResultContext.Fixed("Exception", exception.ToString()));
		}

		public override Outcome Outcome
		{
			get => Outcome.Failure;
			protected set => _inner.Outcome = value;
		}

		/// <inheritdoc cref="ConstraintResult.FailureCause" />
		public override Exception? FailureCause => _exception;

		/// <inheritdoc cref="ConstraintResult.AppendExpectation(StringBuilder, string?)" />
		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> _inner.AppendExpectation(stringBuilder, indentation);

		/// <inheritdoc cref="ConstraintResult.AppendResult(StringBuilder, string?)" />
		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder
				.Append("it did throw ");

			Type? exceptionType = _exception.GetType();
			if (exceptionType == typeof(Exception))
			{
				stringBuilder.Append("an exception");
			}
			else
			{
				stringBuilder.Append(Formatter.Format(exceptionType).PrependAOrAn());
			}
		}

		/// <inheritdoc cref="ConstraintResult.TryGetValue{TValue}(out TValue)" />
		public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value)
			where TValue : default
			=> _inner.TryGetValue(out value);

		/// <inheritdoc cref="ConstraintResult.Negate()" />
		public override ConstraintResult Negate()
			=> _inner.Negate();
	}
#pragma warning restore S2166 // Rename this class to remove "Exception" or correct its inheritance
}
