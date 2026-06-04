using System;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection;

public static partial class ThatConstructor
{
	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> does not have attribute of type
	///     <typeparamref name="TAttribute" />.
	/// </summary>
	public static AndOrResult<ConstructorInfo?, IThat<ConstructorInfo?>> DoesNotHave<TAttribute>(
		this IThat<ConstructorInfo?> subject)
		where TAttribute : Attribute
	{
		AttributeFilterOptions<ConstructorInfo?> attributeFilterOptions =
			new((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i));
		attributeFilterOptions.RegisterAttribute<TAttribute>(true);
		return new AndOrResult<ConstructorInfo?, IThat<ConstructorInfo?>>(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasAttributeConstraint(it, grammars, attributeFilterOptions).Invert()),
			subject);
	}
}
