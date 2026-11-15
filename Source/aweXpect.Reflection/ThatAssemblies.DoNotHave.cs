using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatAssemblies
{
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Assembly" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<Assembly?>, IThat<IEnumerable<Assembly?>?>> DoNotHave<TAttribute>(
		this IThat<IEnumerable<Assembly?>?> subject)
		where TAttribute : Attribute
		=> subject.DoNotHave(typeof(TAttribute));

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Assembly" /> have
	///     attribute of type <paramref name="attributeType" />.
	/// </summary>
	/// <exception cref="ArgumentException">Thrown if <paramref name="attributeType" /> is no attribute or an open generic type.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<Assembly?>, IThat<IEnumerable<Assembly?>?>> DoNotHave(
		this IThat<IEnumerable<Assembly?>?> subject,
		Type attributeType)
	{
		HaveAttributeExpectation<Assembly> expectation = new(AttributeOptions().WithAttribute(attributeType));
		return new(subject.ExpectNone(expectation), subject);
	}

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Assembly" /> have
	///     attribute of type <typeparamref name="TAttribute" /> that matches the <paramref name="predicate" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<Assembly?>, IThat<IEnumerable<Assembly?>?>> DoNotHave<TAttribute>(
		this IThat<IEnumerable<Assembly?>?> subject,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
	{
		HaveAttributeExpectation<Assembly> expectation = new(
			AttributeOptions().WithAttribute(predicate, doNotPopulateThisValue.TrimExpression()));
		return new(subject.ExpectNone(expectation), subject);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="DoNotHave{TAttribute}(IThat{IEnumerable{Assembly}})" />
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<Assembly?>, IThat<IAsyncEnumerable<Assembly?>?>> DoNotHave<TAttribute>(
		this IThat<IAsyncEnumerable<Assembly?>?> subject)
		where TAttribute : Attribute
		=> subject.DoNotHave(typeof(TAttribute));

	/// <inheritdoc cref="DoNotHave(IThat{IEnumerable{Assembly}}, Type)" />
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<Assembly?>, IThat<IAsyncEnumerable<Assembly?>?>> DoNotHave(
		this IThat<IAsyncEnumerable<Assembly?>?> subject,
		Type attributeType)
	{
		HaveAttributeExpectation<Assembly> expectation = new(AttributeOptions().WithAttribute(attributeType));
		return new(subject.ExpectNone(expectation), subject);
	}

	/// <inheritdoc cref="DoNotHave{TAttribute}(IThat{IEnumerable{Assembly}}, Func{TAttribute, bool}, string)" />
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<Assembly?>, IThat<IAsyncEnumerable<Assembly?>?>> DoNotHave<TAttribute>(
		this IThat<IAsyncEnumerable<Assembly?>?> subject,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
	{
		HaveAttributeExpectation<Assembly> expectation = new(
			AttributeOptions().WithAttribute(predicate, doNotPopulateThisValue.TrimExpression()));
		return new(subject.ExpectNone(expectation), subject);
	}
#endif
}
