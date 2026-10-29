using System;
using System.Text;
using aweXpect.Core;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Options;

/// <summary>
///     Options for filtering for a list of alternative attributes.
/// </summary>
/// <remarks>
///     An immutable value: <see cref="WithAttribute{TAttribute}()" /> returns new options. Inherited attributes are
///     considered, unless the scope passed to <see cref="Matches" /> restricts the check to the attributes declared
///     directly on the member.
/// </remarks>
internal sealed class AttributeFilterOptions<TMember>
{
	private readonly Alternative[] _alternatives;
	private readonly Func<TMember, Type, Func<Attribute, bool>, bool, bool> _memberHasAttribute;

	internal AttributeFilterOptions(Func<TMember, Type, Func<Attribute, bool>, bool, bool> memberHasAttribute)
		: this(memberHasAttribute, [])
	{
	}

	private AttributeFilterOptions(Func<TMember, Type, Func<Attribute, bool>, bool, bool> memberHasAttribute,
		Alternative[] alternatives)
	{
		_memberHasAttribute = memberHasAttribute;
		_alternatives = alternatives;
	}

	/// <summary>
	///     Verifies that the <paramref name="member" /> has any of the attributes in the <paramref name="scope" />.
	/// </summary>
	public bool Matches(TMember member, MemberScopes scope)
	{
		bool inherit = !scope.IsDirect();
		foreach (Alternative alternative in _alternatives)
		{
			if (_memberHasAttribute(member, alternative.AttributeType, alternative.Predicate, inherit))
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	///     Appends the expectation for the attributes in the <paramref name="scope" />, joined by <c>" or "</c>, to the
	///     <paramref name="stringBuilder" />.
	/// </summary>
	public void AppendExpectation(StringBuilder stringBuilder, ExpectationGrammars grammar, MemberScopes scope)
	{
		string prefix = grammar.HasFlag(ExpectationGrammars.Plural) ? "have " : "has ";
		if (grammar.HasFlag(ExpectationGrammars.Negated))
		{
			prefix += "no ";
		}

		stringBuilder.Append(prefix);
		int index = 0;
		foreach (Alternative alternative in _alternatives)
		{
			if (index++ > 0)
			{
				stringBuilder.Append(" or ");
			}

			if (scope.IsDirect())
			{
				stringBuilder.Append("direct ");
			}

			alternative.Description(stringBuilder);
		}
	}

	/// <summary>
	///     Returns options with the alternative <typeparamref name="TAttribute" />.
	/// </summary>
	public AttributeFilterOptions<TMember> WithAttribute<TAttribute>()
		where TAttribute : Attribute
		=> With(new Alternative(typeof(TAttribute), AttributePredicates.IsOfType<TAttribute>(),
			sb => Formatter.Format(sb, typeof(TAttribute))));

	/// <summary>
	///     Returns options with the alternative <paramref name="attributeType" />.
	/// </summary>
	/// <exception cref="ArgumentException">Thrown if <paramref name="attributeType" /> is no attribute.</exception>
	public AttributeFilterOptions<TMember> WithAttribute(Type attributeType)
	{
		ThrowHelper.ThrowIfNull(attributeType, nameof(attributeType));
		if (!typeof(Attribute).IsAssignableFrom(attributeType))
		{
			throw Tracing.WriteException(new ArgumentException(
				$"The type must be an attribute, but it was {Formatter.Format(attributeType)}."));
		}

		return With(new Alternative(attributeType, AttributePredicates.IsOfType(attributeType),
			sb => Formatter.Format(sb, attributeType)));
	}

	/// <summary>
	///     Returns options with the alternative <typeparamref name="TAttribute" /> that matches the
	///     <paramref name="predicate" />.
	/// </summary>
	/// <param name="predicate">
	///     (optional) A predicate to check the attribute values.
	///     <para />
	///     If not set (<see langword="null" />), will only check if the attribute is present.
	/// </param>
	/// <param name="predicateExpression">The expression of the predicate to use in the description message.</param>
	public AttributeFilterOptions<TMember> WithAttribute<TAttribute>(
		Func<TAttribute, bool> predicate,
		string predicateExpression)
		where TAttribute : Attribute
		=> With(new Alternative(typeof(TAttribute),
			a => a is TAttribute value && (predicate is null || UserCode.Invoke(predicate, value, "the predicate")),
			sb =>
			{
				Formatter.Format(sb, typeof(TAttribute));
				sb.Append(" matching ");
				sb.Append(predicateExpression);
			}));

	private AttributeFilterOptions<TMember> With(Alternative alternative)
	{
		Alternative[] alternatives = new Alternative[_alternatives.Length + 1];
		_alternatives.CopyTo(alternatives, 0);
		alternatives[_alternatives.Length] = alternative;
		return new AttributeFilterOptions<TMember>(_memberHasAttribute, alternatives);
	}

	private sealed class Alternative(
		Type attributeType,
		Func<Attribute, bool> predicate,
		Action<StringBuilder> description)
	{
		public Type AttributeType { get; } = attributeType;
		public Func<Attribute, bool> Predicate { get; } = predicate;
		public Action<StringBuilder> Description { get; } = description;
	}
}
