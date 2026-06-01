using System;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection;

public static partial class ThatAssembly
{
	/// <summary>
	///     Verifies that the <see cref="Assembly" /> does not have attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" />) specifies, if
	///     the attribute can be inherited from a base type.
	/// </remarks>
	public static AndOrResult<Assembly?, IThat<Assembly?>> DoesNotHave<TAttribute>(
		this IThat<Assembly?> subject, bool inherit = true)
		where TAttribute : Attribute
	{
		AttributeFilterOptions<Assembly?> attributeFilterOptions =
			new((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i));
		attributeFilterOptions.RegisterAttribute<TAttribute>(inherit);
		return new AndOrResult<Assembly?, IThat<Assembly?>>(subject.Get().ExpectationBuilder.AddConstraint(
				(it, grammars)
					=> new HasAttributeConstraint(it, grammars, attributeFilterOptions).Invert()),
			subject);
	}
}
