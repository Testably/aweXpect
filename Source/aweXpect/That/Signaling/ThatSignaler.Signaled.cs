using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Signaling;

namespace aweXpect;

public static partial class ThatSignaler
{
	private const string Times = " times";

	/// <summary>
	///     Verifies that the expected callback was signaled at least once.
	/// </summary>
	[GuaranteesNotNull]
	public static SignalCountResult Signaled(
		this IThat<Signaler> source)
	{
		SignalerOptions options = new();
		return new SignalCountResult(source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new SignaledConstraint(it, grammars, 1, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the expected callback with <typeparamref name="TParameter" /> was signaled at least once.
	/// </summary>
	[GuaranteesNotNull]
	public static SignalCountWhoseResult<TParameter> Signaled<TParameter>(
		this IThat<Signaler<TParameter>> source)
	{
		SignalerOptions<TParameter> options = new();
		return new SignalCountWhoseResult<TParameter>(source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new SignaledConstraint<TParameter>(it, grammars, 1, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the expected callback was signaled
	///     at least the given number of <paramref name="times" />.
	/// </summary>
	[GuaranteesNotNull]
	public static SignalCountResult Signaled(
		this IThat<Signaler> source,
		Times times)
	{
		SignalerOptions options = new();
		return new SignalCountResult(source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new SignaledConstraint(it, grammars, times.Value, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the expected callback with <typeparamref name="TParameter" /> was signaled
	///     at least the given number of <paramref name="times" />.
	/// </summary>
	[GuaranteesNotNull]
	public static SignalCountWhoseResult<TParameter> Signaled<TParameter>(
		this IThat<Signaler<TParameter>> source,
		Times times)
	{
		SignalerOptions<TParameter> options = new();
		return new SignalCountWhoseResult<TParameter>(source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new SignaledConstraint<TParameter>(it, grammars, times.Value, options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the expected callback was not signaled.
	/// </summary>
	[GuaranteesNotNull]
	public static SignalCountResult DidNotSignal(
		this IThat<Signaler> source)
	{
		SignalerOptions options = new();
		return new SignalCountResult(source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new SignaledConstraint(it, grammars, 1, options).Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the expected callback with <typeparamref name="TParameter" /> was not signaled.
	/// </summary>
	[GuaranteesNotNull]
	public static SignalCountResult<TParameter> DidNotSignal<TParameter>(
		this IThat<Signaler<TParameter>> source)
	{
		SignalerOptions<TParameter> options = new();
		return new SignalCountResult<TParameter>(source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new SignaledConstraint<TParameter>(it, grammars, 1, options).Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the expected callback was not signaled
	///     at least the given number of <paramref name="times" />.
	/// </summary>
	[GuaranteesNotNull]
	public static SignalCountResult DidNotSignal(
		this IThat<Signaler> source,
		Times times)
	{
		SignalerOptions options = new();
		return new SignalCountResult(source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new SignaledConstraint(it, grammars, times.Value, options).Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the expected callback with <typeparamref name="TParameter" /> was not signaled
	///     at least the given number of <paramref name="times" />.
	/// </summary>
	[GuaranteesNotNull]
	public static SignalCountResult<TParameter> DidNotSignal<TParameter>(
		this IThat<Signaler<TParameter>> source,
		Times times)
	{
		SignalerOptions<TParameter> options = new();
		return new SignalCountResult<TParameter>(source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new SignaledConstraint<TParameter>(it, grammars, times.Value, options).Invert()),
			source,
			options);
	}

	private sealed class SignaledConstraint(string it, ExpectationGrammars grammars, int count, SignalerOptions options)
		: ConstraintResult.WithNotNullValue<SignalerResult>(it, grammars), IAsyncConstraint<Signaler>
	{
		public async Task<ConstraintResult> IsMetBy(Signaler actual, CancellationToken cancellationToken)
		{
			// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
			if (actual == null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			TimeSpan? timeout = options.Timeout;
			if (count == 1)
			{
				Actual = await Task.Run(()
						=> actual.Wait(timeout, cancellationToken),
					CancellationToken.None);
			}
			else
			{
				int amount = count;
				Actual = await Task.Run(()
						=> actual.Wait(amount, timeout, cancellationToken),
					CancellationToken.None);
			}

			Outcome = Actual.IsSuccess ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (count == 1)
			{
				stringBuilder.Append("has recorded the callback at least once");
			}
			else if (count == 2)
			{
				stringBuilder.Append("has recorded the callback at least twice");
			}
			else
			{
				stringBuilder.Append("has recorded the callback at least ").Append(count).Append(Times);
			}

			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			if (Actual?.Count == 0)
			{
				stringBuilder.Append("never recorded");
			}
			else if (Actual?.Count == 1)
			{
				stringBuilder.Append("only recorded once");
			}
			else if (Actual?.Count == 2)
			{
				stringBuilder.Append("only recorded twice");
			}
			else
			{
				stringBuilder.Append("only recorded ").Append(Actual?.Count).Append(Times);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (count == 1)
			{
				stringBuilder.Append("does not have recorded the callback");
			}
			else if (count == 2)
			{
				stringBuilder.Append("does not have recorded the callback at least twice");
			}
			else
			{
				stringBuilder.Append("does not have recorded the callback at least ").Append(count).Append(Times);
			}

			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			if (Actual?.Count == 1)
			{
				stringBuilder.Append("recorded once");
			}
			else if (Actual?.Count == 2)
			{
				stringBuilder.Append("recorded twice");
			}
			else
			{
				stringBuilder.Append("recorded ").Append(Actual?.Count).Append(Times);
			}
		}
	}

	private sealed class SignaledConstraint<TParameter>(
		string it,
		ExpectationGrammars grammars,
		int count,
		SignalerOptions<TParameter> options)
		: ConstraintResult.WithNotNullValue<SignalerResult<TParameter>>(it, grammars),
			IAsyncConstraint<Signaler<TParameter>>
	{
		private int _actualCount;

		public async Task<ConstraintResult> IsMetBy(
			Signaler<TParameter> actual,
			CancellationToken cancellationToken)
		{
			// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
			if (actual == null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			SignalerOptions<TParameter> o = options;

			TimeSpan? timeout = options.Timeout;
			if (count == 1)
			{
				Actual = await Task.Run(()
						=> actual.Wait(o.Matches, timeout, cancellationToken),
					CancellationToken.None);
			}
			else
			{
				int amount = count;
				Actual = await Task.Run(()
						=> actual.Wait(amount, o.Matches, timeout, cancellationToken),
					CancellationToken.None);
			}

			_actualCount = Actual.Parameters.Count(p => o.Matches(p));

			Outcome = Actual.IsSuccess && _actualCount >= count ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (count == 1)
			{
				stringBuilder.Append("has recorded the callback at least once");
			}
			else if (count == 2)
			{
				stringBuilder.Append("has recorded the callback at least twice");
			}
			else
			{
				stringBuilder.Append("has recorded the callback at least ").Append(count).Append(Times);
			}

			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			if (_actualCount == 0)
			{
				stringBuilder.Append("never recorded");
			}
			else if (_actualCount == 1)
			{
				stringBuilder.Append("only recorded once");
			}
			else if (_actualCount == 2)
			{
				stringBuilder.Append("only recorded twice");
			}
			else
			{
				stringBuilder.Append("only recorded ").Append(_actualCount).Append(Times);
			}

			if (Actual?.Count > 0)
			{
				stringBuilder.Append(" in ");
				ValueFormatters.Format(Formatter, stringBuilder, Actual.Parameters, FormattingOptions.MultipleLines);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (count == 1)
			{
				stringBuilder.Append("does not have recorded the callback");
			}
			else if (count == 2)
			{
				stringBuilder.Append("does not have recorded the callback at least twice");
			}
			else
			{
				stringBuilder.Append("does not have recorded the callback at least ").Append(count).Append(Times);
			}

			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			if (_actualCount == 1)
			{
				stringBuilder.Append("recorded once");
			}
			else if (_actualCount == 2)
			{
				stringBuilder.Append("recorded twice");
			}
			else
			{
				stringBuilder.Append("recorded ").Append(_actualCount).Append(Times);
			}

			if (Actual?.Count > 0)
			{
				stringBuilder.Append(" in ");
				ValueFormatters.Format(Formatter, stringBuilder, Actual.Parameters, FormattingOptions.MultipleLines);
			}
		}
	}
}
