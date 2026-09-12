using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatGeneric
{
	/// <summary>
	///     Verifies the actual value to satisfy the <paramref name="predicate" />.
	/// </summary>
	[GuaranteesNotNull]
	public static RepeatedCheckResult<T, IThat<T>> Satisfies<T>(this IThat<T> source,
		Func<T, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		predicate.ThrowIfNull();
		RepeatedCheckOptions options = new();
		return new RepeatedCheckResult<T, IThat<T>>(source.Get().ExpectationBuilder
				.AddConstraint((it, grammars) =>
					new SatisfiesConstraint<T>(
						it,
						grammars,
						predicate,
						doNotPopulateThisValue.TrimCommonWhiteSpace(),
						options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies the actual value to not satisfy the <paramref name="predicate" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<T, IThat<T>> DoesNotSatisfy<T>(this IThat<T> source,
		Func<T, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		predicate.ThrowIfNull();
		return new AndOrResult<T, IThat<T>>(source.Get().ExpectationBuilder
				.AddConstraint((it, grammars) =>
					new SatisfiesConstraint<T>(
						it,
						grammars,
						predicate,
						doNotPopulateThisValue.TrimCommonWhiteSpace(),
						null).Invert()),
			source);
	}

	private sealed class SatisfiesConstraint<T>(
		string it,
		ExpectationGrammars grammars,
		Func<T, bool> predicate,
		string predicateExpression,
		RepeatedCheckOptions? options)
		: ConstraintResult.WithNotNullValue<T?>(it, grammars),
			IAsyncConstraint<T>
	{
		public async Task<ConstraintResult> IsMetBy(T actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			if (predicate(actual))
			{
				Outcome = Outcome.Success;
				return this;
			}

			if (options != null && options.Timeout > TimeSpan.Zero)
			{
				Stopwatch sw = new();
				sw.Start();
				do
				{
					try
					{
						await Task.Delay(options.Interval.NextCheckInterval(), cancellationToken);
					}
					catch (TaskCanceledException)
					{
						break;
					}

					if (predicate(actual))
					{
						Outcome = Outcome.Success;
						return this;
					}
				} while (sw.Elapsed <= options.Timeout && !cancellationToken.IsCancellationRequested);
			}

			Outcome = Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("satisfies ").Append(predicateExpression.TrimCommonWhiteSpace())
				.Append(options);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not satisfy ").Append(predicateExpression.TrimCommonWhiteSpace())
				.Append(options);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
