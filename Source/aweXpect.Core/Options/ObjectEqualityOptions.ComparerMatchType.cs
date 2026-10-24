using System.Collections.Generic;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Helpers;

namespace aweXpect.Options;

public partial class ObjectEqualityOptions<TSubject>
{
	/// <summary>
	///     Specifies a specific <see cref="IEqualityComparer{T}" /> to use for comparing <see cref="object" />s.
	/// </summary>
	/// <exception cref="System.InvalidOperationException">
	///     Another option already specified how two objects are compared, or a comparer is already set.
	/// </exception>
	public ObjectEqualityOptions<TSubject> Using(IEqualityComparer<object> comparer)
	{
		comparer.ThrowIfNull();
		SetMatchType(new ComparerMatchType(comparer), nameof(Using));
		return this;
	}

	/// <summary>
	///     Specifies a specific <see cref="IEqualityComparer{T}" /> to use for comparing
	///     <typeparamref name="TSubject" /> values.
	/// </summary>
	/// <remarks>
	///     A value that is not a <typeparamref name="TSubject" /> is never equal to one.
	/// </remarks>
	/// <exception cref="System.InvalidOperationException">
	///     Another option already specified how two objects are compared, or a comparer is already set.
	/// </exception>
	public ObjectEqualityOptions<TSubject> Using(IEqualityComparer<TSubject> comparer)
	{
		comparer.ThrowIfNull();
		SetMatchType(new TypedComparerMatchType(comparer), nameof(Using));
		return this;
	}

	private sealed class TypedComparerMatchType : ObjectMatchType<TSubject>
	{
		private readonly ObjectMatcher<TSubject> _matcher;

		public TypedComparerMatchType(IEqualityComparer<TSubject> comparer)
		{
			_matcher = new Matcher(comparer);
			string name = $" using {Formatter.Format(comparer.GetType())}";
			Phrase = MatchPhrase.EqualTo.WithSuffix(name).WithName(name);
		}

		/// <inheritdoc />
		public override MatchPhrase Phrase { get; }

		/// <inheritdoc />
		public override ObjectMatcher<TSubject> ForEvaluation(MatchEnvironment environment)
			=> _matcher;

		/// <remarks>
		///     A value that is not a <typeparamref name="TSubject" /> never matches.
		/// </remarks>
		private sealed class Matcher(IEqualityComparer<TSubject> comparer) : ObjectMatcher<TSubject, TSubject>
		{
			/// <inheritdoc />
			public override ValueTask<MatchResult> Match(TSubject actual, TSubject expected)
				=> new(MatchResult.From(UserCode.Invoke(
					static values => values.Comparer.Equals(values.Actual, values.Expected),
					(Comparer: comparer, Actual: actual, Expected: expected), "the comparer")));
		}
	}

	private sealed class ComparerMatchType : ObjectMatchType<TSubject>
	{
		private readonly ObjectMatcher<TSubject> _matcher;

		public ComparerMatchType(IEqualityComparer<object> comparer)
		{
			_matcher = new Matcher(comparer);
			string name = $" using {Formatter.Format(comparer.GetType())}";
			Phrase = MatchPhrase.EqualTo.WithSuffix(name).WithName(name);
		}

		/// <inheritdoc />
		public override MatchPhrase Phrase { get; }

		/// <inheritdoc />
		public override ObjectMatcher<TSubject> ForEvaluation(MatchEnvironment environment)
			=> _matcher;

		private sealed class Matcher(IEqualityComparer<object> comparer) : ObjectMatcher<TSubject>
		{
			/// <inheritdoc />
			public override ValueTask<MatchResult> Match<TExpected>(TSubject actual, TExpected expected)
				=> new(MatchResult.From(UserCode.Invoke(
					static values => values.Comparer.Equals(values.Actual, values.Expected),
					(Comparer: comparer, Actual: (object?)actual, Expected: (object?)expected), "the comparer")));
		}
	}
}