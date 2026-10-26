using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Options;

/// <summary>
///     Options for filtering for a list of alternative types.
/// </summary>
internal sealed class TypeFilterOptions
{
	private readonly List<(Type, bool)> _types = [];

	/// <summary>
	///     Verifies that the <paramref name="type" /> matches any registered type.
	/// </summary>
	public bool Matches(Type? type)
		=> _types.Any(t => type?.IsOrInheritsFrom(t.Item1, t.Item2) == true);

	/// <summary>
	///     Appends the expectation for the registered types, joined by <c>" or "</c>, to the
	///     <paramref name="stringBuilder" />.
	/// </summary>
	public void AppendExpectation(StringBuilder stringBuilder, ExpectationGrammars grammar)
	{
		if (grammar.IsPlural())
		{
			stringBuilder.Append("return ");
		}
		else
		{
			stringBuilder.Append(grammar.HasFlag(ExpectationGrammars.Negated)
				? "does not return "
				: "returns ");
		}

		int index = 0;
		foreach ((Type? type, bool isDirect) in _types)
		{
			if (index++ > 0)
			{
				stringBuilder.Append(" or ");
			}

			if (isDirect)
			{
				stringBuilder.Append("exactly ");
			}

			Formatter.Format(stringBuilder, type);
		}
	}

	/// <summary>
	///     Appends the registered types as <c>"of type {type}"</c> (or <c>"of exact type {type}"</c> for exact
	///     matches), joined by <c>" or "</c>.
	/// </summary>
	internal void AppendOfTypeDescription(StringBuilder stringBuilder)
	{
		int index = 0;
		foreach ((Type type, bool isDirect) in _types)
		{
			if (index++ > 0)
			{
				stringBuilder.Append(" or ");
			}

			stringBuilder.Append(isDirect ? "of exact type " : "of type ");
			Formatter.Format(stringBuilder, type);
		}
	}

	/// <summary>
	///     Adds the alternative <paramref name="type" />, matched exactly when <paramref name="exactly" /> is
	///     <see langword="true" />.
	/// </summary>
	public void AddType(Type type, bool exactly) => _types.Add((type, exactly));
}
