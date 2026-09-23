using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Helpers;

namespace aweXpect.Equivalency;

internal sealed class EquivalencyComparer(EquivalencyOptions equivalencyOptions)
	: IObjectMatchType
{
	private readonly StringBuilder _failureBuilder = new();

	/// <inheritdoc cref="IObjectMatchType.AreConsideredEqual{TSubject, TExpected}(TSubject, TExpected)" />
#if NET8_0_OR_GREATER
	public async ValueTask<bool>
#else
	public async Task<bool>
#endif
		AreConsideredEqual<TActual, TExpected>(TActual actual, TExpected expected)
	{
		_failureBuilder.Clear();
		if (HandleSpecialCases(actual, expected, _failureBuilder, out bool? specialCaseResult))
		{
			return specialCaseResult.Value;
		}

		return await EquivalencyComparison.Compare(actual, expected, equivalencyOptions, _failureBuilder);
	}

	/// <inheritdoc cref="IObjectMatchType.GetExpectation(string, ExpectationGrammars)" />
	public string GetExpectation(string expected, ExpectationGrammars grammars)
		=> $"{grammars.Verb("is", "are")} {(grammars.HasFlag(ExpectationGrammars.Negated) ? "not " : "")}equivalent to {expected}";

	/// <inheritdoc cref="IObjectMatchType.GetExtendedFailure(string, ExpectationGrammars, object?, object?)" />
	public string GetExtendedFailure(string it, ExpectationGrammars grammars, object? actual, object? expected)
	{
		if (grammars.HasFlag(ExpectationGrammars.Negated))
		{
			return $"{it}{grammars.SubjectVerb(it, " was ", " were ")}{Formatter.Format(actual, FormattingOptions.Indented())}, which is considered equivalent";
		}

		if (actual is null != expected is null)
		{
			_failureBuilder.Clear();
			_failureBuilder.Append(it);
			_failureBuilder.Append(grammars.SubjectVerb(it, " was ", " were "));
			Formatter.Format(_failureBuilder, actual, FormattingOptions.SingleLine);
			_failureBuilder.Append(" instead of ");
			Formatter.Format(_failureBuilder, expected, FormattingOptions.SingleLine);
			return _failureBuilder.ToString();
		}

		return $"{it}{grammars.SubjectVerb(it, " was not:", " were not:")}{_failureBuilder}";
	}

	/// <inheritdoc cref="IObjectMatchType.PrependItemAndComparison" />
	public string PrependItemAndComparison(string expected, string? itemNoun = null, string? comparison = null)
		=> itemNoun is null ? $"equivalent to {expected}" : $"{itemNoun} equivalent to {expected}";

	private static bool HandleSpecialCases<TActual, TExpected>(TActual actual, TExpected expected,
		StringBuilder failureBuilder,
		[NotNullWhen(true)] out bool? isConsideredEqual)
	{
		if (actual is IEqualityComparer actualEqualityComparer)
		{
			isConsideredEqual = actualEqualityComparer.Equals(actual, expected);
			if (isConsideredEqual == false)
			{
				failureBuilder.AppendLine();
				if (failureBuilder.Length > 2)
				{
					failureBuilder.AppendLine("and");
				}

				failureBuilder.Append("  ");
				Formatter.Format(failureBuilder, actual, FormattingOptions.SingleLine);
				failureBuilder.Append(" did not equal ");
				Formatter.Format(failureBuilder, expected, FormattingOptions.SingleLine);
			}

			return true;
		}

		if (expected is IEqualityComparer expectedEqualityComparer)
		{
			isConsideredEqual = expectedEqualityComparer.Equals(actual, expected);
			if (isConsideredEqual == false)
			{
				failureBuilder.AppendLine();
				if (failureBuilder.Length > 2)
				{
					failureBuilder.AppendLine("and");
				}

				failureBuilder.Append("  ");
				Formatter.Format(failureBuilder, actual, FormattingOptions.SingleLine);
				failureBuilder.Append(" did not equal ");
				Formatter.Format(failureBuilder, expected, FormattingOptions.SingleLine);
			}

			return true;
		}

		isConsideredEqual = null;
		return false;
	}

	public override string ToString() => " using equivalency";
}
