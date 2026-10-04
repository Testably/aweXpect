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

	private sealed class TypedComparerMatchType(IEqualityComparer<TSubject> comparer) : IObjectMatchType
	{
		#region IEquality Members

		/// <inheritdoc cref="IObjectMatchType.AreConsideredEqual{TSubject, TExpected}(TSubject, TExpected)" />
		public ValueTask<bool> AreConsideredEqual<TActual, TExpected>(TActual actual, TExpected expected)
			=> new(IsEqual(actual, expected));

		/// <inheritdoc cref="IObjectMatchType.AreConsideredEqualWithExplanation{TActual, TExpected}(TActual, TExpected)" />
		public ValueTask<IObjectMatchResult> AreConsideredEqualWithExplanation<TActual, TExpected>(TActual actual,
			TExpected expected)
			=> ObjectEqualityOptions.ExplainWithActualValue(IsEqual(actual, expected));

		/// <inheritdoc cref="IObjectMatchType.GetExpectation(string, ExpectationGrammars)" />
		public string GetExpectation(string expected, ExpectationGrammars grammars)
			=> $"{grammars.Verb("is", "are")} {(grammars.HasFlag(ExpectationGrammars.Negated) ? "not " : "")}equal to {expected}" + ToString();

		/// <inheritdoc cref="IObjectMatchType.AppendContexts(ResultContextCollector)" />
		public void AppendContexts(ResultContextCollector contexts)
		{
			// A comparison with a comparer has no options to explain.
		}

		/// <inheritdoc cref="IObjectMatchType.PrependItemAndComparison" />
		public string PrependItemAndComparison(string expected, string? itemNoun = null, string? comparison = null)
			=> ObjectEqualityOptions.GetItemExpectation(expected, itemNoun, comparison) + ToString();

		/// <inheritdoc cref="object.ToString()" />
		public override string ToString()
			=> $" using {Formatter.Format(comparer.GetType())}";

		private bool IsEqual<TActual, TExpected>(TActual actual, TExpected expected)
			=> TryCast(actual, out TSubject typedActual) &&
			   TryCast(expected, out TSubject typedExpected) &&
			   UserCode.Invoke(static values => values.Comparer.Equals(values.Actual, values.Expected),
				   (Comparer: comparer, Actual: typedActual, Expected: typedExpected), "the comparer");

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
	}

	private sealed class ComparerMatchType(IEqualityComparer<object> comparer) : IObjectMatchType
	{
		#region IEquality Members

		/// <inheritdoc cref="IObjectMatchType.AreConsideredEqual{TSubject, TExpected}(TSubject, TExpected)" />
		public ValueTask<bool> AreConsideredEqual<TActual, TExpected>(TActual actual, TExpected expected)
			=> new(IsEqual(actual, expected));

		/// <inheritdoc cref="IObjectMatchType.AreConsideredEqualWithExplanation{TActual, TExpected}(TActual, TExpected)" />
		public ValueTask<IObjectMatchResult> AreConsideredEqualWithExplanation<TActual, TExpected>(TActual actual,
			TExpected expected)
			=> ObjectEqualityOptions.ExplainWithActualValue(IsEqual(actual, expected));

		/// <inheritdoc cref="IObjectMatchType.GetExpectation(string, ExpectationGrammars)" />
		public string GetExpectation(string expected, ExpectationGrammars grammars)
			=> $"{grammars.Verb("is", "are")} {(grammars.HasFlag(ExpectationGrammars.Negated) ? "not " : "")}equal to {expected}" + ToString();

		/// <inheritdoc cref="IObjectMatchType.AppendContexts(ResultContextCollector)" />
		public void AppendContexts(ResultContextCollector contexts)
		{
			// A comparison with a comparer has no options to explain.
		}

		/// <inheritdoc cref="IObjectMatchType.PrependItemAndComparison" />
		public string PrependItemAndComparison(string expected, string? itemNoun = null, string? comparison = null)
			=> ObjectEqualityOptions.GetItemExpectation(expected, itemNoun, comparison) + ToString();

		/// <inheritdoc cref="object.ToString()" />
		public override string ToString()
			=> $" using {Formatter.Format(comparer.GetType())}";

		private bool IsEqual<TActual, TExpected>(TActual actual, TExpected expected)
			=> UserCode.Invoke(static values => values.Comparer.Equals(values.Actual, values.Expected),
				(Comparer: comparer, Actual: (object?)actual, Expected: (object?)expected), "the comparer");

		#endregion
	}
}
