using System;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Results;

namespace aweXpect;

public static partial class ThatConstructor
{
	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has a <see langword="ref" /> parameter.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<ConstructorInfo, ConstructorInfo?, object?> HasRefParameter(
		this IThat<ConstructorInfo?> subject)
		=> HasParameterOfKind(subject, p => p.IsRefParameter(), "a ref");

	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has a <see langword="ref" /> parameter of type
	///     <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<ConstructorInfo, ConstructorInfo?, TParameter> HasRefParameter<TParameter>(
		this IThat<ConstructorInfo?> subject)
		=> subject.HasParameter<TParameter>().WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has a <see langword="ref" /> parameter of type
	///     <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<ConstructorInfo, ConstructorInfo?, object?> HasRefParameter(
		this IThat<ConstructorInfo?> subject, Type parameterType)
		=> subject.HasParameter(parameterType).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has a <see langword="ref" /> parameter of type
	///     <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<ConstructorInfo, ConstructorInfo?, TParameter> HasRefParameter<TParameter>(
		this IThat<ConstructorInfo?> subject, string expected)
		=> subject.HasParameter<TParameter>(expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has a <see langword="ref" /> parameter of type
	///     <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<ConstructorInfo, ConstructorInfo?, object?> HasRefParameter(
		this IThat<ConstructorInfo?> subject, Type parameterType, string expected)
		=> subject.HasParameter(parameterType, expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has a <see langword="ref" /> parameter with the
	///     <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<ConstructorInfo, ConstructorInfo?, object?> HasRefParameter(
		this IThat<ConstructorInfo?> subject, string expected)
		=> subject.HasParameter(expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has a <see langword="ref" /> parameter of exact type
	///     <typeparamref name="TParameter" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<ConstructorInfo, ConstructorInfo?, TParameter> HasRefParameterExactly<TParameter>(
		this IThat<ConstructorInfo?> subject)
		=> subject.HasParameterExactly<TParameter>().WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has a <see langword="ref" /> parameter of exact type
	///     <paramref name="parameterType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ParameterCollectionResult<ConstructorInfo, ConstructorInfo?, object?> HasRefParameterExactly(
		this IThat<ConstructorInfo?> subject, Type parameterType)
		=> subject.HasParameterExactly(parameterType).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has a <see langword="ref" /> parameter of exact type
	///     <typeparamref name="TParameter" /> with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<ConstructorInfo, ConstructorInfo?, TParameter> HasRefParameterExactly<TParameter>(
		this IThat<ConstructorInfo?> subject, string expected)
		=> subject.HasParameterExactly<TParameter>(expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");

	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has a <see langword="ref" /> parameter of exact type
	///     <paramref name="parameterType" /> with the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static NamedParameterCollectionResult<ConstructorInfo, ConstructorInfo?, object?> HasRefParameterExactly(
		this IThat<ConstructorInfo?> subject, Type parameterType, string expected)
		=> subject.HasParameterExactly(parameterType, expected).WithModifier(p => p.IsRefParameter(), "with ref modifier");
}
