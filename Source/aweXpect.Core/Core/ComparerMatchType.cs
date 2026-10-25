using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using aweXpect.Core.Helpers;

namespace aweXpect.Core;

/// <summary>
///     The match type of <c>Using</c>, which compares values with an <see cref="IEqualityComparer{T}" />.
/// </summary>
internal sealed class ComparerMatchType<TSubject> : ObjectMatchType<TSubject>
{
	private readonly ObjectMatcher<TSubject> _matcher;

	private ComparerMatchType(ObjectMatcher<TSubject> matcher, Type comparerType) : base("Using")
	{
		_matcher = matcher;
		string name = $" using {Formatter.Format(comparerType)}";
		Phrase = MatchPhrase.EqualTo.WithSuffix(name).WithName(name);
	}

	/// <inheritdoc />
	public override MatchPhrase Phrase { get; }

	/// <summary>
	///     Compares values of any type with the <paramref name="comparer" />.
	/// </summary>
	public static ComparerMatchType<TSubject> For(IEqualityComparer<object> comparer)
		=> new(new UntypedMatcher(comparer), comparer.GetType());

	/// <summary>
	///     Compares <typeparamref name="TSubject" /> values with the <paramref name="comparer" />.
	/// </summary>
	/// <remarks>
	///     A value that is not a <typeparamref name="TSubject" /> never matches.
	/// </remarks>
	public static ComparerMatchType<TSubject> For(IEqualityComparer<TSubject> comparer)
		=> new(new TypedMatcher(comparer), comparer.GetType());

	/// <inheritdoc />
	public override ObjectMatcher<TSubject> ForEvaluation(MatchEnvironment environment)
		=> _matcher;

	private sealed class TypedMatcher(IEqualityComparer<TSubject> comparer) : ObjectMatcher<TSubject, TSubject>
	{
		/// <inheritdoc />
		public override ValueTask<MatchResult> Match(TSubject actual, TSubject expected)
			=> new(MatchResult.From(UserCode.Invoke(
				static values => values.Comparer.Equals(values.Actual, values.Expected),
				(Comparer: comparer, Actual: actual, Expected: expected), "the comparer")));
	}

	private sealed class UntypedMatcher(IEqualityComparer<object> comparer) : ObjectMatcher<TSubject>
	{
		/// <inheritdoc />
		public override ValueTask<MatchResult> Match<TExpected>(TSubject actual, TExpected expected)
			=> new(MatchResult.From(UserCode.Invoke(
				static values => values.Comparer.Equals(values.Actual, values.Expected),
				(Comparer: comparer, Actual: (object?)actual, Expected: (object?)expected), "the comparer")));
	}
}
