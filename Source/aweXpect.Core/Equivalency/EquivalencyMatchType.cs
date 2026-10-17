using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.EvaluationContext;
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
	private readonly CancellationToken _cancellationToken;
	private readonly EquivalencyOptions _equivalencyOptions;
	private readonly IEvaluationContext? _evaluation;
	private StringBuilder? _failureBuilder;
	private bool _isMatch;
	private Dictionary<string, string?>? _unmatchedMembersToIgnore;

	/// <summary>
	///     Compares with the <paramref name="equivalencyOptions" />.
	/// </summary>
	/// <exception cref="ArgumentNullException">The <paramref name="equivalencyOptions" /> are <see langword="null" />.</exception>
	public EquivalencyMatchType(EquivalencyOptions equivalencyOptions)
	{
		equivalencyOptions.ThrowIfNull();
		_equivalencyOptions = equivalencyOptions;
	}

	private EquivalencyMatchType(EquivalencyOptions equivalencyOptions, IEvaluationContext evaluation,
		CancellationToken cancellationToken)
	{
		_equivalencyOptions = equivalencyOptions;
		_evaluation = evaluation;
		_cancellationToken = cancellationToken;
	}

	/// <remarks>
	///     Created by the first comparison that explains its differences, because the items of a collection are
	///     compared without an explanation.
	/// </remarks>
	private StringBuilder FailureBuilder => _failureBuilder ??= new StringBuilder();

	/// <inheritdoc cref="IObjectMatchResult.IsMatch" />
	bool IObjectMatchResult.IsMatch => _isMatch;

	/// <inheritdoc cref="IObjectMatchResult.GetExtendedFailure(string, ExpectationGrammars, object?, object?, string?)" />
	/// <remarks>
	///     The differences keep their layout and are shifted as a whole by the <paramref name="indentation" />.
	/// </remarks>
	string IObjectMatchResult.GetExtendedFailure(string it, ExpectationGrammars grammars, object? actual,
		object? expected, string? indentation)
	{
		if (grammars.IsNegated())
		{
			return $"{it}{grammars.SubjectVerb(it, " was ", " were ")}{Formatter.Format(actual, FormattingOptions.Indented(indentation))}, which is considered equivalent";
		}

		if (actual is null != expected is null)
		{
			StringBuilder failureBuilder = FailureBuilder;
			failureBuilder.Clear();
			failureBuilder.Append(it);
			failureBuilder.Append(grammars.SubjectVerb(it, " was ", " were "));
			Formatter.Format(failureBuilder, actual, FormattingOptions.SingleLine);
			failureBuilder.Append(" instead of ");
			Formatter.Format(failureBuilder, expected, FormattingOptions.SingleLine);
			return failureBuilder.ToString();
		}

		return $"{it}{grammars.SubjectVerb(it, " was not:", " were not:")}{_failureBuilder?.ToString().Indent(indentation, false)}";
	}

	/// <inheritdoc cref="IObjectMatchType.AreConsideredEqual{TActual, TExpected}(TActual, TExpected)" />
	/// <remarks>
	///     Only options that can ignore members by name track them, so that most comparisons of collection items stay
	///     as they are.
	/// </remarks>
	public ValueTask<bool> AreConsideredEqual<TActual, TExpected>(TActual actual, TExpected expected)
		=> CanIgnoreMembersByName(_equivalencyOptions)
			? AreConsideredEqualTrackingMembersToIgnore(actual, expected)
			: EquivalencyComparison.IsEquivalent(actual, expected, _equivalencyOptions, _evaluation,
				_cancellationToken);

	/// <inheritdoc cref="IObjectMatchType.AreConsideredEqualWithExplanation{TActual, TExpected}(TActual, TExpected)" />
	/// <remarks>
	///     The items of a collection are compared one after the other with the same match type, so the members to
	///     ignore of every failed comparison are kept.
	/// </remarks>
	public async ValueTask<IObjectMatchResult> AreConsideredEqualWithExplanation<TActual, TExpected>(TActual actual,
		TExpected expected)
	{
		StringBuilder failureBuilder = FailureBuilder;
		failureBuilder.Clear();
		(_isMatch, _unmatchedMembersToIgnore) = await EquivalencyComparison.CompareExplaining(actual, expected,
			_equivalencyOptions, failureBuilder, _evaluation, _unmatchedMembersToIgnore, _cancellationToken);
		return this;
	}

	private async ValueTask<bool> AreConsideredEqualTrackingMembersToIgnore<TActual, TExpected>(TActual actual,
		TExpected expected)
	{
		(bool isEquivalent, _unmatchedMembersToIgnore) =
			await EquivalencyComparison.IsEquivalentTrackingMembersToIgnore(actual, expected, _equivalencyOptions,
				_evaluation, _unmatchedMembersToIgnore, _cancellationToken);
		return isEquivalent;
	}

	/// <remarks>
	///     The options registered for a type are only created during the comparison, so they may ignore members by
	///     name as well.
	/// </remarks>
	private static bool CanIgnoreMembersByName(EquivalencyOptions equivalencyOptions)
		=>equivalencyOptions.HasRegistrations ||
		   equivalencyOptions.MembersToIgnore.Any(memberToIgnore => memberToIgnore is MemberToIgnore.ByName);

	/// <summary>
	///     Returns a match type for the comparisons of the <paramref name="evaluation" />, so that the expectations of
	///     an <c>It.Is…</c> in the expected object are canceled by the <paramref name="cancellationToken" /> and use
	///     the timeout and the time system of the <paramref name="evaluation" />.
	/// </summary>
	/// <remarks>
	///     It also keeps the differences of its own comparisons, so that evaluations do not share them.
	/// </remarks>
	internal EquivalencyMatchType ForEvaluation(IEvaluationContext evaluation, CancellationToken cancellationToken)
		=> new(_equivalencyOptions, evaluation, cancellationToken);

	/// <inheritdoc cref="IObjectMatchType.GetExpectation(string, ExpectationGrammars)" />
	public string GetExpectation(string expected, ExpectationGrammars grammars)
		=> $"{grammars.Verb("is", "are")} {(grammars.IsNegated() ? "not " : "")}equivalent to {expected}";

	/// <inheritdoc cref="IObjectMatchType.PrependItemAndComparison" />
	public string PrependItemAndComparison(string expected, string? itemNoun = null, string? comparison = null)
		=> itemNoun is null ? $"equivalent to {expected}" : $"{itemNoun} equivalent to {expected}";

	/// <inheritdoc cref="IObjectMatchType.AppendContexts(ResultContextCollector)" />
	public void AppendContexts(ResultContextCollector contexts)
	{
		if (_unmatchedMembersToIgnore is null)
		{
			contexts.AddEquivalencyContext(_equivalencyOptions);
			return;
		}

		Dictionary<string, string?> unmatchedMembersToIgnore = new(_unmatchedMembersToIgnore,
			StringComparer.OrdinalIgnoreCase);
		contexts.Add(new ResultContext.SyncCallback("Equivalency options",
			() => _equivalencyOptions.ToString(unmatchedMembersToIgnore), int.MinValue));
	}

	/// <inheritdoc />
	public override string ToString() => " using equivalency";
}
