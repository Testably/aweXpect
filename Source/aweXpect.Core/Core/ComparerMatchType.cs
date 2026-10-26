using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using aweXpect.Core.Helpers;

namespace aweXpect.Core;

/// <summary>
///     The match type of <c>Using</c>, which compares values with an <see cref="IEqualityComparer{T}" />.
/// </summary>
internal sealed class ComparerMatchType<TElement> : ObjectMatchType<TElement>
{
	private readonly ObjectMatcher<TElement> _matcher;

	private ComparerMatchType(ObjectMatcher<TElement> matcher, Type comparerType) : base("Using")
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
	public static ComparerMatchType<TElement> For(IEqualityComparer<object> comparer)
		=> new(new UntypedMatcher(comparer), comparer.GetType());

	/// <summary>
	///     Compares <typeparamref name="TElement" /> values with the <paramref name="comparer" />.
	/// </summary>
	/// <remarks>
	///     A value that is not a <typeparamref name="TElement" /> never matches.
	/// </remarks>
	public static ComparerMatchType<TElement> For(IEqualityComparer<TElement> comparer)
		=> new(new TypedMatcher(comparer), comparer.GetType());

	/// <inheritdoc />
	public override ObjectMatcher<TElement> ForEvaluation(MatchEnvironment environment)
		=> _matcher;

	private sealed class TypedMatcher(IEqualityComparer<TElement> comparer) : ObjectMatcher<TElement, TElement>
	{
		/// <inheritdoc />
		public override ValueTask<MatchResult> Match(TElement actual, TElement expected)
			=> new(MatchResult.From(UserCode.Invoke(
				static values => values.Comparer.Equals(values.Actual, values.Expected),
				(Comparer: comparer, Actual: actual, Expected: expected), "the comparer")));
	}

	private sealed class UntypedMatcher(IEqualityComparer<object> comparer) : ObjectMatcher<TElement>
	{
		/// <inheritdoc />
		public override ValueTask<MatchResult> Match<TExpected>(TElement actual, TExpected expected)
			=> new(MatchResult.From(UserCode.Invoke(
				static values => values.Comparer.Equals(values.Actual, values.Expected),
				(Comparer: comparer, Actual: (object?)actual, Expected: (object?)expected), "the comparer")));
	}
}
