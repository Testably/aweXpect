using System;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatConstructor
{
	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> does not have attribute of type
	///     <typeparamref name="TAttribute" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<ConstructorInfo, IThat<ConstructorInfo?>> DoesNotHave<TAttribute>(
		this IThat<ConstructorInfo?> subject)
		where TAttribute : Attribute
		=> subject.DoesNotHave(typeof(TAttribute));

	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> does not have attribute of type
	///     <paramref name="attributeType" />.
	/// </summary>
	/// <exception cref="ArgumentException">Thrown if <paramref name="attributeType" /> is no attribute.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<ConstructorInfo, IThat<ConstructorInfo?>> DoesNotHave(
		this IThat<ConstructorInfo?> subject,
		Type attributeType)
	{
		AttributeFilter<ConstructorInfo?> attributeFilter = new(
			new AttributeFilterOptions<ConstructorInfo?>((a, type, p, i) => a.HasAttribute(type, p, i))
				.WithAttribute(attributeType));
		return new AndOrResult<ConstructorInfo, IThat<ConstructorInfo?>>(subject.Get().ExpectationBuilder.AddConstraint(attributeFilter,
				static (attributeFilter, it, grammars)
				=> new HasAttributeConstraint(it, grammars, attributeFilter).Invert()),
			subject);
	}
}
