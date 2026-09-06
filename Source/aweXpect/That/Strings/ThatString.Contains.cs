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
	public static StringEqualityTypeCountResult<string?, IThat<string?>> Contains(
		this IThat<string?> source,
		string expected)
	{
		if (expected == string.Empty)
		{
			// ReSharper disable once LocalizableElement
			throw new ArgumentException("The 'expected' string cannot be empty.", nameof(expected));
		}

		Quantifier quantifier = new();
		StringEqualityOptions options = new();
		return new StringEqualityTypeCountResult<string?, IThat<string?>>(
			source.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars) =>
				new ContainsConstraint(expectationBuilder, it, grammars, expected, quantifier, options)),
			source,
			quantifier,
			options);
	}

	/// <summary>
	///     Verifies that the subject contains the <paramref name="unexpected" /> <see langword="string" />.
	/// </summary>
	public static StringEqualityTypeCountResult<string?, IThat<string?>> DoesNotContain(
		this IThat<string?> source,
		string unexpected)
	{
		if (unexpected == string.Empty)
		{
			// ReSharper disable once LocalizableElement
			throw new ArgumentException("The 'unexpected' string cannot be empty.", nameof(unexpected));
		}

		Quantifier quantifier = new();
		StringEqualityOptions options = new();
		return new StringEqualityTypeCountResult<string?, IThat<string?>>(
			source.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars) =>
				new ContainsConstraint(expectationBuilder, it, grammars, unexpected, quantifier, options).Invert()),
			source,
			quantifier,
			options);
	}

	private sealed class ContainsConstraint(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		string? expected,
		Quantifier quantifier,
		StringEqualityOptions options)
		: ConstraintResult(grammars),
			IAsyncConstraint<string?>
	{
		private string? _actual;
		private int _actualCount;
		private bool _isNegated;

		/// <inheritdoc />
		public async Task<ConstraintResult> IsMetBy(string? actual, CancellationToken cancellationToken)
		{
			_actual = actual;
			if (actual is null || expected is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			_actualCount = await options.CountOccurrences(actual, expected);
			Outcome = quantifier.Check(_actualCount, true) ?? _isNegated ? Outcome.Success : Outcome.Failure;
			if (Outcome != Outcome.Success && !string.IsNullOrEmpty(actual))
			{
				expectationBuilder.AddContext(new ResultContext.Fixed("Actual", actual));
				if (!string.IsNullOrEmpty(expected))
				{
					expectationBuilder.AddContext(new ResultContext.Fixed("Expected", expected));
				}
			}
			
			return this;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (quantifier.IsNever)
			{
				stringBuilder.Append("does not contain ");
				Formatter.Format(stringBuilder, expected);
			}
			else if (_isNegated)
			{
				stringBuilder.Append("does not contain ");
				Formatter.Format(stringBuilder, expected);
				stringBuilder.Append(' ').Append(quantifier.ToNegatedString());
			}
			else
			{
				stringBuilder.Append("contains ");
				Formatter.Format(stringBuilder, expected);
				stringBuilder.Append(' ').Append(quantifier);
			}

			stringBuilder.Append(options);
		}

		/// <inheritdoc cref="ConstraintResult.TryGetValue{TValue}(out TValue)" />
		public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
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
				stringBuilder.ItWasNull(it);
			}
			else if (expected is null)
			{
				Formatter.Format(stringBuilder, _actual);
				stringBuilder.Append(" cannot be validated against <null>");
			}
			else
			{
				if (_actualCount == 0)
				{
					stringBuilder.Append(it).Append(" did not contain ");
					Formatter.Format(stringBuilder, expected);
					stringBuilder.Append(" in ");
				}
				else if (_actualCount == 1)
				{
					stringBuilder.Append(it).Append(" contained ");
					Formatter.Format(stringBuilder, expected);
					stringBuilder.Append(" once in ");
				}
				else if (_actualCount == 2)
				{
					stringBuilder.Append(it).Append(" contained ");
					Formatter.Format(stringBuilder, expected);
					stringBuilder.Append(" twice in ");
				}
				else
				{
					stringBuilder.Append(it).Append(" contained ");
					Formatter.Format(stringBuilder, expected);
					stringBuilder.Append(' ').Append(_actualCount).Append(" times in ");
				}

				Formatter.Format(stringBuilder, _actual);
			}
		}

		public override ConstraintResult Negate()
		{
			_isNegated = !_isNegated;
			quantifier.Negate();
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
