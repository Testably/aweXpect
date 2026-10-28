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
///     Use it via
///     <see cref="ObjectMatchTypeHolderExtensions.SetMatchType{TElement}(OptionsHolder{ObjectMatchType{TElement}}, ObjectMatchType{TElement})" />
///     as the option <c>Equivalent</c>.<br />
///     Mark the parameters of the expectation that receive the compared values with the
///     <see cref="RequiresMemberMetadataAttribute" />, so that the members of their types are registered for trimming.
/// </remarks>
public sealed class EquivalencyMatchType<TElement> : ObjectMatchType<TElement>
{
	private static readonly MatchPhrase SharedPhrase =
		MatchPhrase.Being("equivalent to").WithName(" using equivalency");

	private readonly EquivalencyOptions _equivalencyOptions;

	/// <summary>
	///     Compares with the <paramref name="equivalencyOptions" />.
	/// </summary>
	/// <exception cref="ArgumentNullException">The <paramref name="equivalencyOptions" /> are <see langword="null" />.</exception>
	public EquivalencyMatchType(EquivalencyOptions equivalencyOptions) : base("Equivalent")
	{
		equivalencyOptions.ThrowIfNull();
		_equivalencyOptions = equivalencyOptions;
	}

	/// <inheritdoc />
	/// <remarks>
	///     The expectations of an <c>It.Is…</c> in the expected object are canceled by the cancellation of the
	///     <paramref name="environment" /> and use the timeout and the clock of its evaluation. The matcher keeps the
	///     members to ignore that matched nothing across the items of a collection.
	/// </remarks>
	public override ObjectMatcher<TElement> ForEvaluation(MatchEnvironment environment)
		=> new EquivalencyMatcher(_equivalencyOptions, environment.Evaluation, environment.CancellationToken);

	/// <inheritdoc />
	public override MatchPhrase Phrase => SharedPhrase;

	private sealed class EquivalencyMatcher(
		EquivalencyOptions equivalencyOptions,
		IEvaluationContext? evaluation,
		CancellationToken cancellationToken) : ObjectMatcher<TElement>
	{
		private StringBuilder? _failureBuilder;
		private Dictionary<string, string?>? _unmatchedMembersToIgnore;

		/// <inheritdoc />
		/// <remarks>
		///     Only options that can ignore members by name track them, so that most comparisons of collection items stay
		///     as they are.
		/// </remarks>
		public override ValueTask<MatchResult> Match<TExpected>(TElement actual, TExpected expected)
			=> CanIgnoreMembersByName(equivalencyOptions)
				? MatchTrackingMembersToIgnore(actual, expected)
				: MatchWithoutTracking(actual, expected);

		/// <inheritdoc />
		/// <remarks>
		///     The items of a collection are compared one after the other with the same matcher, so the members to ignore
		///     of every failed comparison are kept.
		/// </remarks>
		public override async ValueTask<MatchResult> MatchWithExplanation<TExpected>(TElement actual,
			TExpected expected)
		{
			StringBuilder failureBuilder = _failureBuilder ??= new StringBuilder();
			failureBuilder.Clear();
			(bool isMatch, _unmatchedMembersToIgnore) = await EquivalencyComparison.CompareExplaining(actual,
				expected, equivalencyOptions, failureBuilder, evaluation, _unmatchedMembersToIgnore,
				cancellationToken);
			return isMatch
				? MatchResult.Match
				: MatchResult.NoMatchBecause(new DifferencesExplanation(failureBuilder.ToString()));
		}

		/// <inheritdoc />
		/// <remarks>
		///     The differences keep their layout and are shifted as a whole by the <paramref name="indentation" />.
		/// </remarks>
		public override string GetFailure<TExpected>(string it, ExpectationGrammars grammars, TElement? actual,
			TExpected expected, MatchResult result, string? indentation)
		{
			if (grammars.IsNegated())
			{
				return $"{it}{grammars.SubjectVerb(it, " was ", " were ")}{Formatter.Format(actual, FormattingOptions.Indented(indentation))}, which is considered equivalent";
			}

			if (actual is null != expected is null)
			{
				StringBuilder failureBuilder = new();
				failureBuilder.Append(it);
				failureBuilder.Append(grammars.SubjectVerb(it, " was ", " were "));
				Formatter.Format(failureBuilder, actual, FormattingOptions.SingleLine);
				failureBuilder.Append(" instead of ");
				Formatter.Format(failureBuilder, expected, FormattingOptions.SingleLine);
				return failureBuilder.ToString();
			}

			return $"{it}{grammars.SubjectVerb(it, " was not:", " were not:")}{(result.Explanation as DifferencesExplanation)?.Differences.Indent(indentation, false)}";
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
		{
			if (_unmatchedMembersToIgnore is null)
			{
				contexts.AddEquivalencyContext(equivalencyOptions);
				return;
			}

			Dictionary<string, string?> unmatchedMembersToIgnore = new(_unmatchedMembersToIgnore,
				StringComparer.OrdinalIgnoreCase);
			contexts.Add(new ResultContext.SyncCallback("Equivalency options",
				() => equivalencyOptions.ToString(unmatchedMembersToIgnore), int.MinValue));
		}

		private async ValueTask<MatchResult> MatchWithoutTracking<TExpected>(TElement actual, TExpected expected)
			=> MatchResult.From(await EquivalencyComparison.IsEquivalent(actual, expected, equivalencyOptions,
				evaluation, cancellationToken));

		private async ValueTask<MatchResult> MatchTrackingMembersToIgnore<TExpected>(TElement actual,
			TExpected expected)
		{
			(bool isEquivalent, _unmatchedMembersToIgnore) =
				await EquivalencyComparison.IsEquivalentTrackingMembersToIgnore(actual, expected, equivalencyOptions,
					evaluation, _unmatchedMembersToIgnore, cancellationToken);
			return MatchResult.From(isEquivalent);
		}

		/// <remarks>
		///     The options registered for a type are only created during the comparison, so they may ignore members by
		///     name as well.
		/// </remarks>
		private static bool CanIgnoreMembersByName(EquivalencyOptions equivalencyOptions)
			=> equivalencyOptions.HasRegistrations ||
			   equivalencyOptions.MembersToIgnore.Any(memberToIgnore => memberToIgnore is MemberToIgnore.ByName);
	}

	/// <summary>
	///     The differences that an equivalency comparison found.
	/// </summary>
	private sealed class DifferencesExplanation(string differences) : MatchExplanation
	{
		public string Differences { get; } = differences;

		/// <inheritdoc />
		public override string GetFailure(string it, ExpectationGrammars grammars, string? indentation)
			=> $"{it}{grammars.SubjectVerb(it, " was not:", " were not:")}{Differences.Indent(indentation, false)}";
	}
}
