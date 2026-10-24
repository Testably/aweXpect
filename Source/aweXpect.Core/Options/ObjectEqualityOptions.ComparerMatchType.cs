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

	private sealed class TypedComparerMatchType : ObjectMatchType
	{
		private readonly IEqualityComparer<TSubject> _comparer;
		private readonly ObjectMatcher _matcher;

		public TypedComparerMatchType(IEqualityComparer<TSubject> comparer)
		{
			_comparer = comparer;
			_matcher = new Matcher(this);
		}

		#region IEquality Members

		/// <inheritdoc />
		public override ObjectMatcher ForEvaluation(MatchEnvironment environment)
			=> _matcher;

		/// <inheritdoc />
		public override string GetExpectation(string expected, ExpectationGrammars grammars)
			=> $"{grammars.Verb("is", "are")} {(grammars.HasFlag(ExpectationGrammars.Negated) ? "not " : "")}equal to {expected}" + ToString();

		/// <inheritdoc />
		public override string PrependItemAndComparison(string expected, string? itemNoun = null,
			string? comparison = null)
			=> ObjectEqualityOptions.GetItemExpectation(expected, itemNoun, comparison) + ToString();

		/// <inheritdoc cref="object.ToString()" />
		public override string ToString()
			=> $" using {Formatter.Format(_comparer.GetType())}";

		private bool IsEqual<TActual, TExpected>(TActual actual, TExpected expected)
			=> TryCast(actual, out TSubject typedActual) &&
			   TryCast(expected, out TSubject typedExpected) &&
			   UserCode.Invoke(static values => values.Comparer.Equals(values.Actual, values.Expected),
				   (Comparer: _comparer, Actual: typedActual, Expected: typedExpected), "the comparer");

		/// <remarks>
		///     A <see langword="null" /> value is not matched by a type pattern, but is a valid value whenever
		///     <typeparamref name="TSubject" /> admits it.
		/// </remarks>
		private static bool TryCast<TValue>(TValue value, out TSubject typedValue)
		{
			if (value is TSubject typed)
			{
				typedValue = typed;
				return true;
			}

			typedValue = default!;
			return value is null && default(TSubject) is null;
		}

		#endregion

		private sealed class Matcher(TypedComparerMatchType matchType) : ObjectMatcher
		{
			/// <inheritdoc />
			public override ValueTask<MatchResult> Match<TActual, TExpected>(TActual actual, TExpected expected)
				=> new(MatchResult.From(matchType.IsEqual(actual, expected)));
		}
	}

	private sealed class ComparerMatchType : ObjectMatchType
	{
		private readonly IEqualityComparer<object> _comparer;
		private readonly ObjectMatcher _matcher;

		public ComparerMatchType(IEqualityComparer<object> comparer)
		{
			_comparer = comparer;
			_matcher = new Matcher(this);
		}

		#region IEquality Members

		/// <inheritdoc />
		public override ObjectMatcher ForEvaluation(MatchEnvironment environment)
			=> _matcher;

		/// <inheritdoc />
		public override string GetExpectation(string expected, ExpectationGrammars grammars)
			=> $"{grammars.Verb("is", "are")} {(grammars.HasFlag(ExpectationGrammars.Negated) ? "not " : "")}equal to {expected}" + ToString();

		/// <inheritdoc />
		public override string PrependItemAndComparison(string expected, string? itemNoun = null,
			string? comparison = null)
			=> ObjectEqualityOptions.GetItemExpectation(expected, itemNoun, comparison) + ToString();

		/// <inheritdoc cref="object.ToString()" />
		public override string ToString()
			=> $" using {Formatter.Format(_comparer.GetType())}";

		private bool IsEqual<TActual, TExpected>(TActual actual, TExpected expected)
			=> UserCode.Invoke(static values => values.Comparer.Equals(values.Actual, values.Expected),
				(Comparer: _comparer, Actual: (object?)actual, Expected: (object?)expected), "the comparer");

		#endregion

		private sealed class Matcher(ComparerMatchType matchType) : ObjectMatcher
		{
			/// <inheritdoc />
			public override ValueTask<MatchResult> Match<TActual, TExpected>(TActual actual, TExpected expected)
				=> new(MatchResult.From(matchType.IsEqual(actual, expected)));
		}
	}
}
