using System;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatAssembly
{
	/// <summary>
	///     Verifies that the <see cref="Assembly" /> does not have attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Assembly, IThat<Assembly?>> DoesNotHave<TAttribute>(
		this IThat<Assembly?> subject)
		where TAttribute : Attribute
	{
		AttributeFilterOptions<Assembly?> attributeFilterOptions =
			new((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i));
		attributeFilterOptions.RegisterAttribute<TAttribute>();
		return new AndOrResult<Assembly, IThat<Assembly?>>(subject.Get().ExpectationBuilder.AddConstraint(attributeFilterOptions,
				static (attributeFilterOptions, it, grammars)
				=> new HasAttributeConstraint(it, grammars, attributeFilterOptions).Invert()),
			subject);
	}
}
