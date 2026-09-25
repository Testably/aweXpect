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

	private sealed class ComparerMatchType(IEqualityComparer<object> comparer) : IObjectMatchType
	{
		#region IEquality Members

		/// <inheritdoc cref="IObjectMatchType.AreConsideredEqual{TSubject, TExpected}(TSubject, TExpected)" />
		public ValueTask<bool> AreConsideredEqual<TActual, TExpected>(TActual actual, TExpected expected)
			=> new ValueTask<bool>(UserCode.Invoke(() => comparer.Equals(actual, expected), "the comparer"));

		/// <inheritdoc cref="IObjectMatchType.GetExpectation(string, ExpectationGrammars)" />
		public string GetExpectation(string expected, ExpectationGrammars grammars)
			=> $"{grammars.Verb("is", "are")} {(grammars.HasFlag(ExpectationGrammars.Negated) ? "not " : "")}equal to {expected}" + ToString();

		/// <inheritdoc cref="IObjectMatchType.GetExtendedFailure(string, ExpectationGrammars, object?, object?)" />
		public string GetExtendedFailure(string it, ExpectationGrammars grammars, object? actual, object? expected)
			=> $"{it}{grammars.SubjectVerb(it, " was ", " were ")}{Formatter.Format(actual, FormattingOptions.Indented())}";

		/// <inheritdoc cref="IObjectMatchType.PrependItemAndComparison" />
		public string PrependItemAndComparison(string expected, string? itemNoun = null, string? comparison = null)
			=> ObjectEqualityOptions.GetItemExpectation(expected, itemNoun, comparison) + ToString();

		/// <inheritdoc cref="object.ToString()" />
		public override string ToString()
			=> $" using {Formatter.Format(comparer.GetType())}";

		#endregion
	}
}
