using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatString
{
	/// <summary>
	///     Verifies that the subject contains the <paramref name="expected" /> <see langword="string" />.
	/// </summary>
	[GuaranteesNotNull]
	public static StringOccurrenceCountResult<string, IThat<string?>> Contains(
		this IThat<string?> subject,
		string expected)
	{
		expected.ThrowIfNull();
		if (expected == string.Empty)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentException("The 'expected' string cannot be empty.", nameof(expected)));
		}

		Quantifier quantifier = new();
		StringEqualityOptions options = new(nameof(expected));
		return new StringOccurrenceCountResult<string, IThat<string?>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(Expected: expected, Quantifier: quantifier, Options: options),
				static (state, it, grammars) =>
					new ContainsConstraint(it, grammars, state.Expected, state.Quantifier, state.Options)),
			subject,
			quantifier,
			options);
	}

	/// <summary>
	///     Verifies that the subject does not contain the <paramref name="unexpected" /> <see langword="string" />.
	/// </summary>
	[GuaranteesNotNull]
	public static StringOccurrenceCountResult<string, IThat<string?>> DoesNotContain(
		this IThat<string?> subject,
		string unexpected)
	{
		unexpected.ThrowIfNull();
		if (unexpected == string.Empty)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentException("The 'unexpected' string cannot be empty.", nameof(unexpected)));
		}

		Quantifier quantifier = new();
		StringEqualityOptions options = new(nameof(unexpected));
		return new StringOccurrenceCountResult<string, IThat<string?>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(Unexpected: unexpected, Quantifier: quantifier, Options: options),
				static (state, it, grammars) =>
					new ContainsConstraint(it, grammars, state.Unexpected, state.Quantifier, state.Options).Invert()),
			subject,
			quantifier,
			options);
	}

	private sealed class ContainsConstraint(
		string it,
		ExpectationGrammars grammars,
		string expected,
		Quantifier quantifier,
		StringEqualityOptions options)
		: ConstraintResult(grammars),
			IAsyncConstraint<string?>
	{
		private string? _actual;
		private int _actualCount;
		private bool _isNegated;

		/// <inheritdoc />
		public async ValueTask<ConstraintResult> IsMetBy(string? actual, CancellationToken cancellationToken)
		{
			_actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			_actualCount = await options.CountOccurrences(actual, expected);
			Outcome = quantifier.Check(_actualCount, true, _isNegated) ?? _isNegated ? Outcome.Success : Outcome.Failure;
			return this;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
		{
			if (!string.IsNullOrEmpty(_actual))
			{
				contexts.AddStringContext("Actual", _actual, this);
				contexts.AddStringContext("Expected", expected, this);
			}
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(quantifier.ToContainsExpectation(Grammars, $"{Formatter.Format(expected)}{options}",
				_isNegated));

		/// <inheritdoc cref="ConstraintResult.TryGetStoredValue{TValue}(out TValue)" />
		public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
		{
			if (_actual is TValue typedValue)
			{
				value = typedValue;
				return true;
			}

			value = default;
			return typeof(TValue).IsAssignableFrom(typeof(string));
		}

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_actual is null)
			{
				stringBuilder.ItWasNull(it, Grammars);
			}
			else
			{
				if (_actualCount == 0)
				{
					stringBuilder.Append(it).Append(" did not contain ");
					Formatter.Format(stringBuilder, expected);
					stringBuilder.Append(" in ");
				}
				else
				{
					stringBuilder.Append(it).Append(" contained ");
					Formatter.Format(stringBuilder, expected);
					stringBuilder.Append(' ').AppendOccurrences(_actualCount).Append(" in ");
				}

				Formatter.Format(stringBuilder, _actual);
			}
		}

		public override ConstraintResult Negate()
		{
			_isNegated = !_isNegated;
			Outcome = Outcome switch
			{
				Outcome.Failure => Outcome.Success,
				Outcome.Success => Outcome.Failure,
				_ => Outcome,
			};
			return this;
		}
	}
}
