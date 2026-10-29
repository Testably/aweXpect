using System;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatField
{
	/// <summary>
	///     Verifies that the <see cref="FieldInfo" /> does not have attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<FieldInfo, IThat<FieldInfo?>> DoesNotHave<TAttribute>(this IThat<FieldInfo?> subject)
		where TAttribute : Attribute
		=> subject.DoesNotHave(typeof(TAttribute));

	/// <summary>
	///     Verifies that the <see cref="FieldInfo" /> does not have attribute of type <paramref name="attributeType" />.
	/// </summary>
	/// <exception cref="ArgumentException">Thrown if <paramref name="attributeType" /> is no attribute or an open generic type.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<FieldInfo, IThat<FieldInfo?>> DoesNotHave(
		this IThat<FieldInfo?> subject,
		Type attributeType)
	{
		AttributeFilter<FieldInfo?> attributeFilter = new(
			new AttributeFilterOptions<FieldInfo?>((a, type, p, i) => a.HasAttribute(type, p, i))
				.WithAttribute(attributeType));
		return new AndOrResult<FieldInfo, IThat<FieldInfo?>>(subject.Get().ExpectationBuilder.AddConstraint(attributeFilter,
				static (attributeFilter, it, grammars)
				=> new HasAttributeConstraint(it, grammars, attributeFilter).Invert()),
			subject);
	}
}
