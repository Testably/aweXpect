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
	{
		AttributeFilterOptions<EventInfo?> attributeFilterOptions =
			new((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i));
		attributeFilterOptions.RegisterAttribute<TAttribute>();
		return new InheritanceScopeResult<EventInfo, EventInfo?>(subject.Get().ExpectationBuilder.AddConstraint(attributeFilterOptions,
				static (attributeFilterOptions, it, grammars)
				=> new HasAttributeConstraint(it, grammars, attributeFilterOptions).Invert()),
			subject,
			attributeFilterOptions.Scope);
	}
}
