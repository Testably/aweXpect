using System;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection;

public static partial class ThatEvent
{
	/// <summary>
	///     Verifies that the <see cref="EventInfo" /> does not have attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" />) specifies, if
	///     the attribute can be inherited from a base type.
	/// </remarks>
	public static AndOrResult<EventInfo?, IThat<EventInfo?>> DoesNotHave<TAttribute>(
		this IThat<EventInfo?> subject, bool inherit = true)
		where TAttribute : Attribute
	{
		AttributeFilterOptions<EventInfo?> attributeFilterOptions =
			new((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i));
		attributeFilterOptions.RegisterAttribute<TAttribute>(inherit);
		return new AndOrResult<EventInfo?, IThat<EventInfo?>>(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasAttributeConstraint(it, grammars, attributeFilterOptions).Invert()),
			subject);
	}
}
