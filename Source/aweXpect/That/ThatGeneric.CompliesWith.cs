using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatGeneric
{
	/// <summary>
	///     Verifies that the actual value complies with the <paramref name="expectations" />.
	/// </summary>
	[GuaranteesNotNull]
	public static RepeatedCheckResult<T, IThat<T>> CompliesWith<T>(this IThat<T> subject,
		Action<IThatSubject<T>> expectations)
	{
		RepeatedCheckOptions options = new();
		return new RepeatedCheckResult<T, IThat<T>>(subject.Get().ExpectationBuilder
				.AddConstraint((expectationBuilder, _, grammars) =>
					new CompliesWithConstraint<T>(expectationBuilder, grammars, expectations, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the actual value does not comply with the <paramref name="expectations" />.
	/// </summary>
	[GuaranteesNotNull]
	public static RepeatedCheckResult<T, IThat<T>> DoesNotComplyWith<T>(this IThat<T> subject,
		Action<IThatSubject<T>> expectations)
	{
		RepeatedCheckOptions options = new();
		return new RepeatedCheckResult<T, IThat<T>>(subject.Get().ExpectationBuilder
				.AddConstraint((expectationBuilder, _, grammars) =>
					new CompliesWithConstraint<T>(expectationBuilder, grammars, expectations, options).Invert()),
			subject,
			options);
	}

	private sealed class CompliesWithConstraint<T>
		: ConstraintResult,
			IAsyncContextConstraint<T>
	{
		private readonly ManualExpectationBuilder<T> _itemExpectationBuilder;
		private readonly RepeatedCheckOptions _options;
		private bool _isNegated;
		private bool _isOnceNegated;

		public CompliesWithConstraint(ExpectationBuilder expectationBuilder, ExpectationGrammars grammars,
			Action<IThatSubject<T>> expectations, RepeatedCheckOptions options)
			: base(grammars)
		{
			_options = options;
			_itemExpectationBuilder = new ManualExpectationBuilder<T>(expectationBuilder, grammars);
			expectations.Invoke(new ThatSubject<T>(_itemExpectationBuilder));
		}

		public async Task<ConstraintResult> IsMetBy(
			T actual,
			IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			ConstraintResult isMatch = await _itemExpectationBuilder.IsMetBy(actual, context, cancellationToken);
			if (isMatch.Outcome == Outcome.Success != _isNegated)
			{
				return NegateOnceIfNegated(isMatch).AppendExpectationText(sb => sb.Append(_options));
			}

			if (_options.Timeout > TimeSpan.Zero)
			{
				Stopwatch sw = new();
				sw.Start();
				do
				{
					try
					{
						await Task.Delay(_options.Interval.NextCheckInterval(), cancellationToken);
					}
					catch (TaskCanceledException)
					{
						break;
					}

					isMatch = await _itemExpectationBuilder.IsMetBy(actual, context, cancellationToken);
					if (isMatch.Outcome == Outcome.Success != _isNegated)
					{
						return NegateOnceIfNegated(isMatch).AppendExpectationText(sb => sb.Append(_options));
					}
				} while (sw.Elapsed <= _options.Timeout && !cancellationToken.IsCancellationRequested);
			}

			return NegateOnceIfNegated(isMatch).AppendExpectationText(sb => sb.Append(_options));
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> _itemExpectationBuilder.AppendExpectation(stringBuilder, indentation);

		private ConstraintResult NegateOnceIfNegated(ConstraintResult constraintResult)
		{
			if (_isNegated && !_isOnceNegated)
			{
				_isOnceNegated = true;
				return constraintResult.Negate();
			}

			return constraintResult;
		}

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
		}

		public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
		{
			value = default;
			return false;
		}

		public override ConstraintResult Negate()
		{
			_isNegated = !_isNegated;
			return this;
		}
	}
}
