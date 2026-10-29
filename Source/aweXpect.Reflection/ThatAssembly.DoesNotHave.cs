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
		=> subject.DoesNotHave(typeof(TAttribute));

	/// <summary>
	///     Verifies that the <see cref="Assembly" /> does not have attribute of type <paramref name="attributeType" />.
	/// </summary>
	/// <exception cref="ArgumentException">Thrown if <paramref name="attributeType" /> is no attribute.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<Assembly, IThat<Assembly?>> DoesNotHave(
		this IThat<Assembly?> subject,
		Type attributeType)
	{
		AttributeFilter<Assembly?> attributeFilter = new(
			new AttributeFilterOptions<Assembly?>((a, type, p, i) => a.HasAttribute(type, p, i))
				.WithAttribute(attributeType));
		return new AndOrResult<Assembly, IThat<Assembly?>>(subject.Get().ExpectationBuilder.AddConstraint(attributeFilter,
				static (attributeFilter, it, grammars)
				=> new HasAttributeConstraint(it, grammars, attributeFilter).Invert()),
			subject);
	}
}
