using System;
using System.Collections.Generic;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatFields
{
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="FieldInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<FieldInfo?>, IThat<IEnumerable<FieldInfo?>?>> DoNotHave<TAttribute>(
		this IThat<IEnumerable<FieldInfo?>?> subject)
		where TAttribute : Attribute
		=> subject.DoNotHave(typeof(TAttribute));

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="FieldInfo" /> have
	///     attribute of type <paramref name="attributeType" />.
	/// </summary>
	/// <exception cref="ArgumentException">Thrown if <paramref name="attributeType" /> is no attribute.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<FieldInfo?>, IThat<IEnumerable<FieldInfo?>?>> DoNotHave(
		this IThat<IEnumerable<FieldInfo?>?> subject,
		Type attributeType)
	{
		HaveAttributeExpectation<FieldInfo> expectation = new(AttributeOptions().WithAttribute(attributeType));
		return new(subject.ExpectNone(expectation), subject);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="DoNotHave{TAttribute}(IThat{IEnumerable{FieldInfo}})" />
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<FieldInfo?>, IThat<IAsyncEnumerable<FieldInfo?>?>> DoNotHave<TAttribute>(
		this IThat<IAsyncEnumerable<FieldInfo?>?> subject)
		where TAttribute : Attribute
		=> subject.DoNotHave(typeof(TAttribute));

	/// <inheritdoc cref="DoNotHave(IThat{IEnumerable{FieldInfo}}, Type)" />
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<FieldInfo?>, IThat<IAsyncEnumerable<FieldInfo?>?>> DoNotHave(
		this IThat<IAsyncEnumerable<FieldInfo?>?> subject,
		Type attributeType)
	{
		HaveAttributeExpectation<FieldInfo> expectation = new(AttributeOptions().WithAttribute(attributeType));
		return new(subject.ExpectNone(expectation), subject);
	}
#endif
}
