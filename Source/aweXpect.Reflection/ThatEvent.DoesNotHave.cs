using System;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;

namespace aweXpect;

public static partial class ThatEvent
{
	/// <summary>
	///     Verifies that the <see cref="EventInfo" /> does not have attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the check
	///     to the attributes declared directly on the member with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<EventInfo, EventInfo?> DoesNotHave<TAttribute>(
		this IThat<EventInfo?> subject)
		where TAttribute : Attribute
		=> subject.DoesNotHave(typeof(TAttribute));

	/// <summary>
	///     Verifies that the <see cref="EventInfo" /> does not have attribute of type <paramref name="attributeType" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the check
	///     to the attributes declared directly on the member with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <paramref name="attributeType" /> is no attribute or an open generic type.</exception>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<EventInfo, EventInfo?> DoesNotHave(
		this IThat<EventInfo?> subject,
		Type attributeType)
	{
		AttributeFilter<EventInfo?> attributeFilter = new(
			new AttributeFilterOptions<EventInfo?>((a, type, p, i) => a.HasAttribute(type, p, i))
				.WithAttribute(attributeType));
		return new InheritanceScopeResult<EventInfo, EventInfo?>(subject.Get().ExpectationBuilder.AddConstraint(attributeFilter,
				static (attributeFilter, it, grammars)
				=> new HasAttributeConstraint(it, grammars, attributeFilter).Invert()),
			subject,
			attributeFilter.Scope);
	}
}
