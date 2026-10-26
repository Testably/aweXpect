using System;
using System.Linq;
using System.Text;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Options;

/// <summary>
///     Options for filtering for a list of alternative types.
/// </summary>
/// <remarks>
///     An immutable value: <see cref="WithType(Type, bool)" /> returns new options. Start from <see cref="Default" />.
/// </remarks>
internal sealed class TypeFilterOptions
{
	private readonly (Type Type, bool IsExact)[] _types;

	private TypeFilterOptions((Type Type, bool IsExact)[] types)
	{
		_types = types;
	}

	/// <summary>
	///     The options without any type.
	/// </summary>
	public static TypeFilterOptions Default { get; } = new([]);

	/// <summary>
	///     Verifies that the <paramref name="type" /> matches any of the types.
	/// </summary>
	public bool Matches(Type? type)
		=> _types.Any(t => type?.IsOrInheritsFrom(t.Type, t.IsExact) == true);

	/// <summary>
	///     Appends the expectation for the types, joined by <c>" or "</c>, to the <paramref name="stringBuilder" />.
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
		foreach ((Type type, bool isExact) in _types)
		{
			if (index++ > 0)
			{
				stringBuilder.Append(" or ");
			}

			if (isExact)
			{
				stringBuilder.Append("exactly ");
			}

			Formatter.Format(stringBuilder, type);
		}
	}

	/// <summary>
	///     Appends the types as <c>"of type {type}"</c> (or <c>"of exact type {type}"</c> for exact matches), joined by
	///     <c>" or "</c>.
	/// </summary>
	internal void AppendOfTypeDescription(StringBuilder stringBuilder)
	{
		int index = 0;
		foreach ((Type type, bool isExact) in _types)
		{
			if (index++ > 0)
			{
				stringBuilder.Append(" or ");
			}

			stringBuilder.Append(isExact ? "of exact type " : "of type ");
			Formatter.Format(stringBuilder, type);
		}
	}

	/// <summary>
	///     Returns options with the alternative <paramref name="type" />, matched exactly when
	///     <paramref name="exactly" /> is <see langword="true" />.
	/// </summary>
	public TypeFilterOptions WithType(Type type, bool exactly)
	{
		(Type, bool)[] types = new (Type, bool)[_types.Length + 1];
		_types.CopyTo(types, 0);
		types[_types.Length] = (type, exactly);
		return new TypeFilterOptions(types);
	}
}
