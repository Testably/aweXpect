using System;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Results;

namespace aweXpect;

public static partial class ThatMethod
{
	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has a <see langword="params" /> parameter.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<MethodInfo, MethodInfo?, object?> HasParamsParameter(
		this IThat<MethodInfo?> subject)
		=> HasParameterOfKind(subject, p => p.IsParamsParameter(), "a params");

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has a <see langword="params" /> parameter of type
	///     <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<MethodInfo, MethodInfo?, TParameter> HasParamsParameter<TParameter>(
		this IThat<MethodInfo?> subject)
		=> subject.HasParameter<TParameter>().WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has a <see langword="params" /> parameter of type
	///     <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<MethodInfo, MethodInfo?, object?> HasParamsParameter(
		this IThat<MethodInfo?> subject, Type parameterType)
		=> subject.HasParameter(parameterType).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has a <see langword="params" /> parameter of type
	///     <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<MethodInfo, MethodInfo?, TParameter> HasParamsParameter<TParameter>(
		this IThat<MethodInfo?> subject, string expected)
		=> subject.HasParameter<TParameter>(expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has a <see langword="params" /> parameter of type
	///     <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<MethodInfo, MethodInfo?, object?> HasParamsParameter(
		this IThat<MethodInfo?> subject, Type parameterType, string expected)
		=> subject.HasParameter(parameterType, expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has a <see langword="params" /> parameter with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<MethodInfo, MethodInfo?, object?> HasParamsParameter(
		this IThat<MethodInfo?> subject, string expected)
		=> subject.HasParameter(expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has a <see langword="params" /> parameter of exact type
	///     <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<MethodInfo, MethodInfo?, TParameter> HasParamsParameterExactly<TParameter>(
		this IThat<MethodInfo?> subject)
		=> subject.HasParameterExactly<TParameter>().WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has a <see langword="params" /> parameter of exact type
	///     <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<MethodInfo, MethodInfo?, object?> HasParamsParameterExactly(
		this IThat<MethodInfo?> subject, Type parameterType)
		=> subject.HasParameterExactly(parameterType).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has a <see langword="params" /> parameter of exact type
	///     <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<MethodInfo, MethodInfo?, TParameter> HasParamsParameterExactly<TParameter>(
		this IThat<MethodInfo?> subject, string expected)
		=> subject.HasParameterExactly<TParameter>(expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has a <see langword="params" /> parameter of exact type
	///     <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<MethodInfo, MethodInfo?, object?> HasParamsParameterExactly(
		this IThat<MethodInfo?> subject, Type parameterType, string expected)
		=> subject.HasParameterExactly(parameterType, expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");
}
