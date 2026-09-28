using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Helpers;

namespace aweXpect.Equivalency;

internal sealed class EquivalencyComparer(EquivalencyOptions equivalencyOptions)
	: IObjectMatchType
{
	private readonly StringBuilder _failureBuilder = new();

	/// <inheritdoc cref="IObjectMatchType.AreConsideredEqual{TSubject, TExpected}(TSubject, TExpected)" />
	public async ValueTask<bool>
		AreConsideredEqual<TActual, TExpected>(TActual actual, TExpected expected)
	{
		_failureBuilder.Clear();
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

	public override string ToString() => " using equivalency";
}
