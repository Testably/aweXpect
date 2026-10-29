using System;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;

namespace aweXpect;

public static partial class ThatMethod
{
	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> does not have attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the check
	///     to the attributes declared directly on the member with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<MethodInfo, MethodInfo?> DoesNotHave<TAttribute>(
		this IThat<MethodInfo?> subject)
		where TAttribute : Attribute
		=> subject.DoesNotHave(typeof(TAttribute));

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> does not have attribute of type <paramref name="attributeType" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the check
	///     to the attributes declared directly on the member with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <paramref name="attributeType" /> is no attribute or an open generic type.</exception>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<MethodInfo, MethodInfo?> DoesNotHave(
		this IThat<MethodInfo?> subject,
		Type attributeType)
	{
		AttributeFilter<MethodInfo?> attributeFilter = new(
			new AttributeFilterOptions<MethodInfo?>((a, type, p, i) => a.HasAttribute(type, p, i))
				.WithAttribute(attributeType));
		return new InheritanceScopeResult<MethodInfo, MethodInfo?>(subject.Get().ExpectationBuilder.AddConstraint(attributeFilter,
				static (attributeFilter, it, grammars)
				=> new HasAttributeConstraint(it, grammars, attributeFilter).Invert()),
			subject,
			attributeFilter.Scope);
	}
}
