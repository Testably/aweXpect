using System;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection;

public static partial class ThatProperty
{
	/// <summary>
	///     Verifies that the <see cref="PropertyInfo" /> does not have attribute of type
	///     <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" />) specifies, if
	///     the attribute can be inherited from a base type.
	/// </remarks>
	public static AndOrResult<PropertyInfo?, IThat<PropertyInfo?>> DoesNotHave<TAttribute>(
		this IThat<PropertyInfo?> subject, bool inherit = true)
		where TAttribute : Attribute
	{
		AttributeFilterOptions<PropertyInfo?> attributeFilterOptions =
			new((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i));
		attributeFilterOptions.RegisterAttribute<TAttribute>(inherit);
		return new AndOrResult<PropertyInfo?, IThat<PropertyInfo?>>(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasAttributeConstraint(it, grammars, attributeFilterOptions).Invert()),
			subject);
	}
}
