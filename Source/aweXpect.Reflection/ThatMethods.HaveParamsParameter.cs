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
	///     a <see langword="params" /> parameter.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveParamsParameter(
		this IThat<IEnumerable<MethodInfo?>?> subject)
		=> HaveParameterOfKind(subject, p => p.IsParamsParameter(), "a params");

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="params" /> parameter.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveParamsParameter(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject)
		=> HaveParameterOfKind(subject, p => p.IsParamsParameter(), "a params");
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="params" /> parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter> HaveParamsParameter<TParameter>(
		this IThat<IEnumerable<MethodInfo?>?> subject)
		=> subject.HaveParameter<TParameter>().WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="params" /> parameter of type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveParamsParameter(
		this IThat<IEnumerable<MethodInfo?>?> subject, Type parameterType)
		=> subject.HaveParameter(parameterType).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="params" /> parameter of type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter> HaveParamsParameter<TParameter>(
		this IThat<IEnumerable<MethodInfo?>?> subject, string expected)
		=> subject.HaveParameter<TParameter>(expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="params" /> parameter of type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveParamsParameter(
		this IThat<IEnumerable<MethodInfo?>?> subject, Type parameterType, string expected)
		=> subject.HaveParameter(parameterType, expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="params" /> parameter with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveParamsParameter(
		this IThat<IEnumerable<MethodInfo?>?> subject, string expected)
		=> subject.HaveParameter(expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="params" /> parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter> HaveParamsParameter<TParameter>(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject)
		=> subject.HaveParameter<TParameter>().WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="params" /> parameter of type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveParamsParameter(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, Type parameterType)
		=> subject.HaveParameter(parameterType).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="params" /> parameter of type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter> HaveParamsParameter<TParameter>(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, string expected)
		=> subject.HaveParameter<TParameter>(expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="params" /> parameter of type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveParamsParameter(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, Type parameterType, string expected)
		=> subject.HaveParameter(parameterType, expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="params" /> parameter with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveParamsParameter(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, string expected)
		=> subject.HaveParameter(expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="params" /> parameter of exact type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter> HaveParamsParameterExactly<TParameter>(
		this IThat<IEnumerable<MethodInfo?>?> subject)
		=> subject.HaveParameterExactly<TParameter>().WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="params" /> parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveParamsParameterExactly(
		this IThat<IEnumerable<MethodInfo?>?> subject, Type parameterType)
		=> subject.HaveParameterExactly(parameterType).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="params" /> parameter of exact type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, TParameter> HaveParamsParameterExactly<TParameter>(
		this IThat<IEnumerable<MethodInfo?>?> subject, string expected)
		=> subject.HaveParameterExactly<TParameter>(expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="params" /> parameter of exact type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?, object?> HaveParamsParameterExactly(
		this IThat<IEnumerable<MethodInfo?>?> subject, Type parameterType, string expected)
		=> subject.HaveParameterExactly(parameterType, expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="params" /> parameter of exact type <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter> HaveParamsParameterExactly<TParameter>(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject)
		=> subject.HaveParameterExactly<TParameter>().WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="params" /> parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveParamsParameterExactly(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, Type parameterType)
		=> subject.HaveParameterExactly(parameterType).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="params" /> parameter of exact type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, TParameter> HaveParamsParameterExactly<TParameter>(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, string expected)
		=> subject.HaveParameterExactly<TParameter>(expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     a <see langword="params" /> parameter of exact type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?, object?> HaveParamsParameterExactly(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, Type parameterType, string expected)
		=> subject.HaveParameterExactly(parameterType, expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");
#endif
}
