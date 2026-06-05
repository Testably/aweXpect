using System;
using System.Collections.Generic;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Customization;

/// <summary>
///     Extensions for <see cref="AwexpectCustomization.ReflectionCustomization" /> to control which
///     normally-hidden members are included when reflecting over types and members.
/// </summary>
public static class ReflectionCustomizationExtensions
{
	private const string CompilerGeneratedKey = "Reflection.IncludedCompilerGeneratedMembers";
	private const string SpecialNameKey = "Reflection.IncludedSpecialNameMembers";
	private const string ExcludedAttributeTypesKey = "Reflection.ExcludedAttributeTypes";
	private const string DependencyResolverKey = "Reflection.DependencyResolver";

	/// <summary>
	///     The resolver that determines which types a given type depends on.
	/// </summary>
	/// <remarks>
	///     Defaults to a built-in signature-level resolver (base type, interfaces, field/property/event types,
	///     method/constructor signatures, generic arguments and applied attributes). Method-body references are not
	///     detected by the default; supply a custom resolver (e.g. backed by Mono.Cecil) for IL/body-level accuracy.
	///     Setting <see langword="null" /> reverts to the built-in default, e.g. to opt a single test out of a
	///     globally configured resolver; <c>Get()</c> always returns the resolver currently in effect.
	///     The resolver's output is unwrapped, de-duplicated and cached per type for the lifetime of the resolver
	///     delegate, so it needs no caching of its own. It must, however, be pure, i.e. deterministic for a given
	///     <see cref="Type" /> within its scope.
	/// </remarks>
	public static ICustomizationValueSetter<Func<Type, IEnumerable<Type>>?> DependencyResolver(
		this AwexpectCustomization.ReflectionCustomization reflection)
		=> new NullableCustomizationValue<Func<Type, IEnumerable<Type>>>(
			Customize.aweXpect, DependencyResolverKey, TypeHelpers.SignatureDependencies);

	/// <summary>
	///     The full names of additional attribute types that the type-level dependency assertions treat as
	///     compiler-emitted, i.e. that never count as signature dependencies.
	/// </summary>
	/// <remarks>
	///     Defaults to an empty list. Extends the built-in set (nullability metadata, state machines, …), so that
	///     e.g. a marker attribute of a future compiler version can be excluded without a library update.
	/// </remarks>
	public static ICustomizationValueSetter<string[]> ExcludedAttributeTypes(
		this AwexpectCustomization.ReflectionCustomization reflection)
		=> new CustomizationValue<string[]>(
			Customize.aweXpect, ExcludedAttributeTypesKey, []);

	/// <summary>
	///     The compiler-generated members that are included when reflecting over types and members.
	/// </summary>
	/// <remarks>
	///     Defaults to <see cref="CompilerGeneratedMembers.None" />, so that all compiler-generated members are excluded.
	/// </remarks>
	public static ICustomizationValueSetter<CompilerGeneratedMembers> IncludedCompilerGeneratedMembers(
		this AwexpectCustomization.ReflectionCustomization reflection)
		=> new CustomizationValue<CompilerGeneratedMembers>(
			Customize.aweXpect, CompilerGeneratedKey, CompilerGeneratedMembers.None);

	/// <summary>
	///     The special-name methods (operators and accessors) that are included when reflecting over methods.
	/// </summary>
	/// <remarks>
	///     Defaults to <see cref="SpecialNameMembers.None" />, so that all special-name methods are excluded.
	/// </remarks>
	public static ICustomizationValueSetter<SpecialNameMembers> IncludedSpecialNameMembers(
		this AwexpectCustomization.ReflectionCustomization reflection)
		=> new CustomizationValue<SpecialNameMembers>(
			Customize.aweXpect, SpecialNameKey, SpecialNameMembers.None);

	private sealed class CustomizationValue<TValue>(
		IAwexpectCustomization customization,
		string key,
		TValue defaultValue)
		: ICustomizationValueSetter<TValue>
	{
		/// <inheritdoc cref="ICustomizationValueSetter{TValue}.Get()" />
		public TValue Get() => customization.Get(key, defaultValue);

		/// <inheritdoc cref="ICustomizationValueSetter{TValue}.Set(TValue)" />
		public CustomizationLifetime Set(TValue value) => customization.Set(key, value);
	}

	/// <summary>
	///     Like <see cref="CustomizationValue{TValue}" />, but accepts <see langword="null" /> to revert to the
	///     <paramref name="defaultValue" /> within the scope: <see langword="null" /> is stored as the default
	///     itself, so <c>Get()</c> always returns the value in effect (never <see langword="null" />).
	/// </summary>
	private sealed class NullableCustomizationValue<TValue>(
		IAwexpectCustomization customization,
		string key,
		TValue defaultValue)
		: ICustomizationValueSetter<TValue?>
		where TValue : class
	{
		/// <inheritdoc cref="ICustomizationValueSetter{TValue}.Get()" />
		public TValue? Get() => customization.Get(key, defaultValue);

		/// <inheritdoc cref="ICustomizationValueSetter{TValue}.Set(TValue)" />
		public CustomizationLifetime Set(TValue? value) => customization.Set(key, value ?? defaultValue);
	}
}
