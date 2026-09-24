using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Helpers;
using aweXpect.Core.Sources;
using aweXpect.Options;

namespace aweXpect.Delegates;

/// <summary>
///     Expectations on delegate values.
/// </summary>
public abstract partial class ThatDelegate(ExpectationBuilder expectationBuilder)
{
	/// <summary>
	///     The expectation builder.
	/// </summary>
	public ExpectationBuilder ExpectationBuilder { get; } = expectationBuilder;

	internal static string FormatForMessage(Exception? exception, string? indentation, string relation = "")
	{
		if (exception is null)
		{
			return "<null>";
		}

		string message = (relation + Formatter.Format(exception.GetType())).PrependAOrAn();
		if (!string.IsNullOrEmpty(exception.Message))
		{
			message += ":" + Environment.NewLine + exception.Message.Indent(indentation + "  ");
		}

		return message;
	}

	private sealed class DelegateIsNotNullWithinTimeoutConstraint(
		string it,
		ExpectationGrammars grammars,
		ThrowsOption options)
		: ConstraintResult(grammars),
			IValueConstraint<DelegateValue>
	{
		private DelegateValue? _actual;

		/// <inheritdoc cref="ConstraintResult.FailureCause" />
		public override Exception? FailureCause
			=> Outcome == Outcome.Failure && _actual?.ExceededTimeout is not null ? _actual.Exception : null;

		public ConstraintResult IsMetBy(DelegateValue value)
		{
			_actual = value;
			if (value.IsNull || value.ExceededTimeout is not null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			if (options.ExecutionTimeOptions is not null &&
			    !options.ExecutionTimeOptions.IsWithinLimit(value.Duration))
			{
				Outcome = Outcome.Failure;
				return this;
			}

			Outcome = Outcome.Success;
			return this;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			// Do nothing
		}

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_actual?.IsNull != false)
			{
				stringBuilder.ItWasNull(it);
			}
			else if (_actual.ExceededTimeout is { } exceededTimeout)
			{
				stringBuilder.ItDidNotFinishWithin(it, exceededTimeout);
			}
			else if (options.ExecutionTimeOptions is not null)
			{
				stringBuilder.Append(it).Append(" took ");
				options.ExecutionTimeOptions.AppendFailureResult(stringBuilder, _actual.Duration);
			}
		}

		public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
		{
			value = default;
			return false;
		}

		public override ConstraintResult Negate()
			=> this;
	}

	/// <summary>
	///     Options on expectations if a delegate throws.
	/// </summary>
	public class ThrowsOption
	{
		/// <summary>
		///     Flag indicating if the delegate is expected to throw an exception.
		/// </summary>
		/// <remarks>
		///     If set to <see langword="false" />, the delegate must not throw any exception.
		/// </remarks>
		public bool DoCheckThrow { get; set; } = true;

		/// <summary>
		///     Options on the execution time to allow specifying a timeout.
		/// </summary>
		public ExecutionTimeOptions? ExecutionTimeOptions { get; set; }
	}
}
