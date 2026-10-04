using System;
using System.Text;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Helpers;
using aweXpect.Core.Metadata;
using aweXpect.Options;

namespace aweXpect.Equivalency;

/// <summary>
///     Compares two objects for equivalency with the <see cref="EquivalencyOptions" />, with the same texts as
///     <c>IsEquivalentTo</c>.
/// </summary>
/// <remarks>
///     Use it via <see cref="ObjectEqualityOptions{TSubject}.SetMatchType(IObjectMatchType, string)" />.<br />
///     <see cref="AreConsideredEqualWithExplanation{TActual, TExpected}(TActual, TExpected)" /> returns the match type
///     itself, which keeps the differences of that comparison for the failure message, so use a new instance for each
///     expectation.<br />
///     Mark the parameters of the expectation that receive the compared values with the
///     <see cref="RequiresMemberMetadataAttribute" />, so that the members of their types are registered for trimming.
/// </remarks>
public sealed class EquivalencyMatchType : IObjectMatchType, IObjectMatchResult
{
	private readonly EquivalencyOptions _equivalencyOptions;
	private readonly StringBuilder _failureBuilder = new();
	private bool _isMatch;

	/// <summary>
	///     Compares with the <paramref name="equivalencyOptions" />.
	/// </summary>
	/// <exception cref="ArgumentNullException">The <paramref name="equivalencyOptions" /> are <see langword="null" />.</exception>
	public EquivalencyMatchType(EquivalencyOptions equivalencyOptions)
	{
		equivalencyOptions.ThrowIfNull();
		_equivalencyOptions = equivalencyOptions;
	}

	/// <inheritdoc cref="IObjectMatchType.AreConsideredEqual{TActual, TExpected}(TActual, TExpected)" />
	public ValueTask<bool> AreConsideredEqual<TActual, TExpected>(TActual actual, TExpected expected)
		=> EquivalencyComparison.IsEquivalent(actual, expected, _equivalencyOptions);

	/// <inheritdoc cref="IObjectMatchType.AreConsideredEqualWithExplanation{TActual, TExpected}(TActual, TExpected)" />
	public async ValueTask<IObjectMatchResult> AreConsideredEqualWithExplanation<TActual, TExpected>(TActual actual,
		TExpected expected)
	{
		_failureBuilder.Clear();
		_isMatch = await EquivalencyComparison.Compare(actual, expected, _equivalencyOptions, _failureBuilder);
		return this;
	}

	/// <inheritdoc cref="IObjectMatchType.GetExpectation(string, ExpectationGrammars)" />
	public string GetExpectation(string expected, ExpectationGrammars grammars)
		=> $"{grammars.Verb("is", "are")} {(grammars.IsNegated() ? "not " : "")}equivalent to {expected}";

	/// <inheritdoc cref="IObjectMatchType.PrependItemAndComparison" />
	public string PrependItemAndComparison(string expected, string? itemNoun = null, string? comparison = null)
		=> itemNoun is null ? $"equivalent to {expected}" : $"{itemNoun} equivalent to {expected}";

	/// <inheritdoc cref="IObjectMatchType.AppendContexts(ResultContextCollector)" />
	public void AppendContexts(ResultContextCollector contexts)
		=> contexts.AddEquivalencyContext(_equivalencyOptions);

	/// <inheritdoc />
	public override string ToString() => " using equivalency";

	/// <inheritdoc cref="IObjectMatchResult.IsMatch" />
	bool IObjectMatchResult.IsMatch => _isMatch;

	/// <inheritdoc cref="IObjectMatchResult.GetExtendedFailure(string, ExpectationGrammars, object?, object?)" />
	string IObjectMatchResult.GetExtendedFailure(string it, ExpectationGrammars grammars, object? actual,
		object? expected)
	{
		if (grammars.IsNegated())
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
}
