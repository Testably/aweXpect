using System;
using System.Collections.Generic;
using System.Text;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Options;

/// <summary>
///     Options for adding additional predicates to filter the parameter list.
/// </summary>
/// <remarks>
///     Inherited attributes are considered, unless the result restricts the check to the attributes declared directly
///     on the member.
/// </remarks>
public class AttributeFilterOptions<TMember>
{
	private readonly List<Action<StringBuilder>> _descriptions = [];
	private readonly Func<TMember, Type, Func<Attribute, bool>, bool, bool> _memberHasAttribute;
	private readonly List<(Type, Func<Attribute, bool>)> _predicates = [];

	internal AttributeFilterOptions(Func<TMember, Type, Func<Attribute, bool>, bool, bool> memberHasAttribute)
	{
		_memberHasAttribute = memberHasAttribute;
	}

	/// <summary>
	///     The inheritance scope of the attributes.
	/// </summary>
	internal ScopeOptions Scope { get; } = new();

	/// <summary>
	///     Verifies that the <paramref name="member" /> matches any registered attribute.
	/// </summary>
	public bool Matches(TMember member)
	{
		bool inherit = !Scope.IsDirect;
		foreach ((Type? attributeType, Func<Attribute, bool>? predicate) in _predicates)
		{
			if (_memberHasAttribute(member, attributeType, predicate, inherit))
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

			if (Scope.IsDirect)
			{
				stringBuilder.Append("direct ");
			}

			description(stringBuilder);
		}
	}

	/// <summary>
	///     Registers an <typeparamref name="TAttribute" />
	/// </summary>
	public void RegisterAttribute<TAttribute>()
		where TAttribute : Attribute
	{
		_descriptions.Add(sb => Formatter.Format(sb, typeof(TAttribute)));
		_predicates.Add((typeof(TAttribute), AttributePredicates.IsOfType<TAttribute>()));
	}

	/// <summary>
	///     Registers an <typeparamref name="TAttribute" />
	/// </summary>
	/// <param name="predicate">
	///     (optional) A predicate to check the attribute values.
	///     <para />
	///     If not set (<see langword="null" />), will only check if the attribute is present.
	/// </param>
	/// <param name="predicateExpression">The expression of the predicate to use in the description message.</param>
	public void RegisterAttribute<TAttribute>(
		Func<TAttribute, bool> predicate,
		string predicateExpression)
		where TAttribute : Attribute
	{
		_descriptions.Add(sb =>
		{
			Formatter.Format(sb, typeof(TAttribute));
			sb.Append(" matching ");
			sb.Append(predicateExpression);
		});

		_predicates.Add((typeof(TAttribute),
			a => a is TAttribute value && (predicate is null || UserCode.Invoke(predicate, value, "the predicate"))));
	}
}
