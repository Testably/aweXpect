using System;
using System.Collections.Generic;
using System.Text;
using aweXpect.Core;

namespace aweXpect.Reflection.Options;

/// <summary>
///     Options for adding additional predicates to filter the parameter list.
/// </summary>
public class AttributeFilterOptions<TMember>(Func<TMember, Type, Func<Attribute, bool>, bool, bool> memberHasAttribute)
{
	private readonly List<Action<StringBuilder>> _descriptions = [];
	private readonly List<(Type, Func<Attribute, bool>, bool)> _predicates = [];

	/// <summary>
	///     Verifies that the <paramref name="member" /> matches any registered attribute.
	/// </summary>
	public bool Matches(TMember member)
	{
		foreach (var (attributeType, predicate, inherits) in _predicates)
		{
			if (memberHasAttribute(member, attributeType, predicate, inherits))
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	///     Returns the combination of all descriptions joined by <c>" and "</c>.
	/// </summary>
	public void AppendDescription(StringBuilder stringBuilder, ExpectationGrammars grammar)
	{
		string prefix = grammar.HasFlag(ExpectationGrammars.Plural) ? "have " : "has ";
		if (grammar.HasFlag(ExpectationGrammars.Negated))
		{
			prefix += "no ";
		}

		stringBuilder.Append(prefix);
		int index = 0;
		foreach (Action<StringBuilder> description in _descriptions)
		{
			if (index++ > 0)
			{
				stringBuilder.Append(" or ");
			}

			description(stringBuilder);
		}
	}

	/// <summary>
	///     Registers an <typeparamref name="TAttribute" />
	/// </summary>
	/// <param name="inherit">
	///     <see langword="true" /> to search the inheritance chain to find the attributes; otherwise,
	///     <see langword="false" />.<br />
	///     Defaults to <see langword="true" />
	/// </param>
	public void RegisterAttribute<TAttribute>(bool inherit)
		where TAttribute : Attribute
	{
		_descriptions.Add(sb =>
		{
			if (!inherit)
			{
				sb.Append("direct ");
			}

			Formatter.Format(sb, typeof(TAttribute));
		});

		_predicates.Add((typeof(TAttribute), a => a is TAttribute, inherit));
	}

	/// <summary>
	///     Registers an <typeparamref name="TAttribute" />
	/// </summary>
	/// <param name="inherit">
	///     <see langword="true" /> to search the inheritance chain to find the attributes; otherwise,
	///     <see langword="false" />.<br />
	///     Defaults to <see langword="true" />
	/// </param>
	/// <param name="predicate">
	///     (optional) A predicate to check the attribute values.
	///     <para />
	///     If not set (<see langword="null" />), will only check if the attribute is present.
	/// </param>
	/// <param name="predicateExpression">The expression of the predicate to use in the description message.</param>
	public void RegisterAttribute<TAttribute>(bool inherit,
		Func<TAttribute, bool> predicate,
		string predicateExpression)
		where TAttribute : Attribute
	{
		_descriptions.Add(sb =>
		{
			if (!inherit)
			{
				sb.Append("direct ");
			}

			Formatter.Format(sb, typeof(TAttribute));
			sb.Append(" matching ");
			sb.Append(predicateExpression);
		});

		_predicates.Add((typeof(TAttribute), a => a is TAttribute value && predicate?.Invoke(value) != false, inherit));
	}
}
