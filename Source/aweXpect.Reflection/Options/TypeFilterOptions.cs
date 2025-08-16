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
public class TypeFilterOptions
{
	private readonly List<(Type, bool)> _types = [];

	/// <summary>
	///     Verifies that the <paramref name="type" /> matches any registered type.
	/// </summary>
	public bool Matches(Type? type)
		=> _types.Any(t => type?.IsOrInheritsFrom(t.Item1, t.Item2) == true);

	/// <summary>
	///     Returns the combination of all descriptions joined by <c>" or "</c>.
	/// </summary>
	public void AppendDescription(StringBuilder stringBuilder, ExpectationGrammars grammar)
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
		foreach (var (type, isDirect) in _types)
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
	///     Registers a <paramref name="type" />.
	/// </summary>
	public void RegisterType(Type type, bool forceDirect) => _types.Add((type, forceDirect));
}
