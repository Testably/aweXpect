using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using aweXpect.Core.Helpers;
using aweXpect.Delegates;

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
		private readonly TimeSpan? _exceededTimeout;
		private readonly ConstraintResult _inner;

		/// <summary>
		///     A failed <see cref="ConstraintResult" /> due to a thrown <paramref name="exception" />, or due to the
		///     subject not finishing within the <paramref name="exceededTimeout" />.
		/// </summary>
		public FromException(
			ConstraintResult inner,
			Exception exception,
			TimeSpan? exceededTimeout = null)
			: base(inner.Grammars)
		{
			_inner = inner;
			_exception = exception;
			_exceededTimeout = exceededTimeout;
			FurtherProcessingStrategy = inner.FurtherProcessingStrategy;
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
			if (_exceededTimeout is not null)
			{
				stringBuilder.ItDidNotFinishWithin("it", _exceededTimeout.Value);
				return;
			}

			stringBuilder
				.Append("it did throw ")
				.Append(ThatDelegate.FormatForMessage(_exception, indentation));
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
