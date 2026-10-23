using System;
using System.Collections.Generic;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatMethods
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveInParameter(
		this IThat<IEnumerable<MethodInfo?>?> subject)
		=> HaveParameterOfKind(subject, p => p.IsInParameter(), "an in");

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveInParameter(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject)
		=> HaveParameterOfKind(subject, p => p.IsInParameter(), "an in");
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter> HaveInParameter<TParameter>(
		this IThat<IEnumerable<MethodInfo?>?> subject)
		=> subject.HaveParameter<TParameter>().WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter of type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveInParameter(
		this IThat<IEnumerable<MethodInfo?>?> subject, Type parameterType)
		=> subject.HaveParameter(parameterType).WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter of type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter> HaveInParameter<TParameter>(
		this IThat<IEnumerable<MethodInfo?>?> subject, string expected)
		=> subject.HaveParameter<TParameter>(expected).WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter of type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveInParameter(
		this IThat<IEnumerable<MethodInfo?>?> subject, Type parameterType, string expected)
		=> subject.HaveParameter(parameterType, expected).WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveInParameter(
		this IThat<IEnumerable<MethodInfo?>?> subject, string expected)
		=> subject.HaveParameter(expected).WithModifier(p => p.IsInParameter(), "with in modifier");

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter> HaveInParameter<TParameter>(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject)
		=> subject.HaveParameter<TParameter>().WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter of type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveInParameter(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, Type parameterType)
		=> subject.HaveParameter(parameterType).WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter of type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter> HaveInParameter<TParameter>(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, string expected)
		=> subject.HaveParameter<TParameter>(expected).WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter of type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveInParameter(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, Type parameterType, string expected)
		=> subject.HaveParameter(parameterType, expected).WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveInParameter(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, string expected)
		=> subject.HaveParameter(expected).WithModifier(p => p.IsInParameter(), "with in modifier");
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter of exact type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter> HaveInParameterExactly<TParameter>(
		this IThat<IEnumerable<MethodInfo?>?> subject)
		=> subject.HaveParameterExactly<TParameter>().WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveInParameterExactly(
		this IThat<IEnumerable<MethodInfo?>?> subject, Type parameterType)
		=> subject.HaveParameterExactly(parameterType).WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter of exact type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter> HaveInParameterExactly<TParameter>(
		this IThat<IEnumerable<MethodInfo?>?> subject, string expected)
		=> subject.HaveParameterExactly<TParameter>(expected).WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter of exact type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveInParameterExactly(
		this IThat<IEnumerable<MethodInfo?>?> subject, Type parameterType, string expected)
		=> subject.HaveParameterExactly(parameterType, expected).WithModifier(p => p.IsInParameter(), "with in modifier");

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter of exact type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter> HaveInParameterExactly<TParameter>(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject)
		=> subject.HaveParameterExactly<TParameter>().WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveInParameterExactly(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, Type parameterType)
		=> subject.HaveParameterExactly(parameterType).WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter of exact type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter> HaveInParameterExactly<TParameter>(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, string expected)
		=> subject.HaveParameterExactly<TParameter>(expected).WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter of exact type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveInParameterExactly(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, Type parameterType, string expected)
		=> subject.HaveParameterExactly(parameterType, expected).WithModifier(p => p.IsInParameter(), "with in modifier");
#endif
}
