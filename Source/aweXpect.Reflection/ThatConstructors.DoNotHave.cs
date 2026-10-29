using System;
using System.Collections.Generic;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatConstructors
{
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<ConstructorInfo?>, IThat<IEnumerable<ConstructorInfo?>?>> DoNotHave<TAttribute>(
		this IThat<IEnumerable<ConstructorInfo?>?> subject)
		where TAttribute : Attribute
		=> subject.DoNotHave(typeof(TAttribute));

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     attribute of type <paramref name="attributeType" />.
	/// </summary>
	/// <exception cref="ArgumentException">Thrown if <paramref name="attributeType" /> is no attribute or an open generic type.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<ConstructorInfo?>, IThat<IEnumerable<ConstructorInfo?>?>> DoNotHave(
		this IThat<IEnumerable<ConstructorInfo?>?> subject,
		Type attributeType)
	{
		HaveAttributeExpectation<ConstructorInfo> expectation = new(AttributeOptions().WithAttribute(attributeType));
		return new(subject.ExpectNone(expectation), subject);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="DoNotHave{TAttribute}(IThat{IEnumerable{ConstructorInfo}})" />
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<ConstructorInfo?>, IThat<IAsyncEnumerable<ConstructorInfo?>?>> DoNotHave<TAttribute>(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject)
		where TAttribute : Attribute
		=> subject.DoNotHave(typeof(TAttribute));

	/// <inheritdoc cref="DoNotHave(IThat{IEnumerable{ConstructorInfo}}, Type)" />
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<ConstructorInfo?>, IThat<IAsyncEnumerable<ConstructorInfo?>?>> DoNotHave(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject,
		Type attributeType)
	{
		HaveAttributeExpectation<ConstructorInfo> expectation = new(AttributeOptions().WithAttribute(attributeType));
		return new(subject.ExpectNone(expectation), subject);
	}
#endif
}
