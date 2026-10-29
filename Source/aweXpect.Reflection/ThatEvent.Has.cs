using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Formatting;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;

namespace aweXpect;

public static partial class ThatEvent
{
	/// <summary>
	///     Verifies that the <see cref="EventInfo" /> has attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the check
	///     to the attributes declared directly on the member with <see cref="HasAttributeResult{TType, TMember}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static HasAttributeResult<EventInfo, EventInfo?> Has<TAttribute>(this IThat<EventInfo?> subject)
		where TAttribute : Attribute
		=> subject.Has(typeof(TAttribute));

	/// <summary>
	///     Verifies that the <see cref="EventInfo" /> has attribute of type <paramref name="attributeType" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the check
	///     to the attributes declared directly on the member with <see cref="HasAttributeResult{TType, TMember}.Directly" />.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <paramref name="attributeType" /> is no attribute or an open generic type.</exception>
	[GuaranteesNotNull]
	public static HasAttributeResult<EventInfo, EventInfo?> Has(
		this IThat<EventInfo?> subject,
		Type attributeType)
	{
		AttributeFilter<EventInfo?> attributeFilter = new(
			new AttributeFilterOptions<EventInfo?>((a, type, p, i) => a.HasAttribute(type, p, i))
				.WithAttribute(attributeType));
		return new HasAttributeResult<EventInfo, EventInfo?>(subject.Get().ExpectationBuilder.AddConstraint(attributeFilter,
				static (attributeFilter, it, grammars)
				=> new HasAttributeConstraint(it, grammars, attributeFilter)),
			subject,
			attributeFilter);
	}

	/// <summary>
	///     Verifies that the <see cref="EventInfo" /> has attribute of type <typeparamref name="TAttribute" /> that
	///     matches the <paramref name="predicate" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the check
	///     to the attributes declared directly on the member with <see cref="HasAttributeResult{TType, TMember}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static HasAttributeResult<EventInfo, EventInfo?> Has<TAttribute>(
		this IThat<EventInfo?> subject,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
	{
		AttributeFilter<EventInfo?> attributeFilter = new(
			new AttributeFilterOptions<EventInfo?>((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i))
				.WithAttribute(predicate, doNotPopulateThisValue.TrimExpression()));
		return new HasAttributeResult<EventInfo, EventInfo?>(subject.Get().ExpectationBuilder.AddConstraint(attributeFilter,
				static (attributeFilter, it, grammars)
				=> new HasAttributeConstraint(it, grammars, attributeFilter)),
			subject,
			attributeFilter);
	}

	private sealed class HasAttributeConstraint(
		string it,
		ExpectationGrammars grammars,
		AttributeFilter<EventInfo?> attributeFilter)
		: ConstraintResult.WithNotNullValue<EventInfo?>(it, grammars),
			IValueConstraint<EventInfo?>
	{
		public ConstraintResult IsMetBy(EventInfo? actual)
		{
			Actual = actual;
			Outcome = attributeFilter.Matches(actual) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> attributeFilter.AppendExpectation(stringBuilder, Grammars);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" did not in ");
			Formatter.Format(stringBuilder, FormattableMember.From(Actual));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> attributeFilter.AppendExpectation(stringBuilder, Grammars);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" did in ");
			Formatter.Format(stringBuilder, FormattableMember.From(Actual));
		}
	}
}
