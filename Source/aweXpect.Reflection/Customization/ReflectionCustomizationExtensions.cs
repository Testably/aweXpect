using System;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Customization;

/// <summary>
///     Extensions for <see cref="AwexpectCustomization" /> to customize aweXpect.Reflection.
/// </summary>
public static class ReflectionCustomizationExtensions
{
	/// <summary>
	///     Customize which normally-hidden members are included when reflecting over types and members, and how the
	///     dependencies of a type are resolved.
	/// </summary>
	/// <remarks>
	///     Works on <see cref="Customize.aweXpect" /> for the current async flow and on
	///     <see cref="AwexpectCustomization.Global" /> for all async flows. The excluded assembly prefixes are
	///     customized via <see cref="AwexpectCustomization.Reflection()" />.
	/// </remarks>
	public static ReflectionSettingsCustomization ReflectionSettings(
		this AwexpectCustomization awexpectCustomization)
		=> new(awexpectCustomization);

	/// <summary>
	///     Customize which normally-hidden members are included when reflecting over types and members, and how the
	///     dependencies of a type are resolved.
	/// </summary>
	public sealed class ReflectionSettingsCustomization
	{
		private const string KeyPrefix = "aweXpect.Reflection.";

		internal ReflectionSettingsCustomization(IAwexpectCustomization awexpectCustomization)
		{
			DependencyResolver = new DefaultWhenNull<Func<Type, IEnumerable<Type>>>(
				new CustomizationValue<Func<Type, IEnumerable<Type>>?>(awexpectCustomization,
					KeyPrefix + nameof(DependencyResolver), null),
				TypeHelpers.SignatureDependencies);
			ExcludedAttributeTypes = new CopiedOnGet(new CustomizationValue<string[]>(awexpectCustomization,
				KeyPrefix + nameof(ExcludedAttributeTypes), [],
				value =>
				{
					if (value is null)
					{
						throw Tracing.WriteException(
							new ArgumentNullException(nameof(value), "The 'value' cannot be null."));
					}
				}));
			IncludedCompilerGeneratedMembers = new CustomizationValue<CompilerGeneratedMembers>(
				awexpectCustomization, KeyPrefix + nameof(IncludedCompilerGeneratedMembers),
				CompilerGeneratedMembers.None);
			IncludedSpecialNameMembers = new CustomizationValue<SpecialNameMembers>(
				awexpectCustomization, KeyPrefix + nameof(IncludedSpecialNameMembers),
				SpecialNameMembers.None);
		}

		/// <summary>
		///     The resolver that determines which types a given type depends on.
		/// </summary>
		/// <remarks>
		///     Defaults to a built-in signature-level resolver (base type, interfaces, field/property/event types,
		///     method/constructor signatures, generic arguments and applied attributes). Method-body references are
		///     not detected by the default; supply a custom resolver (e.g. backed by Mono.Cecil) for IL/body-level
		///     accuracy. Setting <see langword="null" /> reverts to the built-in default, e.g. to opt a single test out
		///     of a globally configured resolver; <c>Get()</c> always returns the resolver currently in effect.
		///     The resolver's output is unwrapped, de-duplicated and cached per type for the lifetime of the resolver
		///     delegate, so it needs no caching of its own. It must, however, be pure, i.e. deterministic for a given
		///     <see cref="Type" /> within its scope.
		/// </remarks>
		public ICustomizationValueSetter<Func<Type, IEnumerable<Type>>?> DependencyResolver { get; }

		/// <summary>
		///     The full names of additional attribute types that the type-level dependency assertions treat as
		///     compiler-emitted, i.e. that never count as signature dependencies.
		/// </summary>
		/// <remarks>
		///     Defaults to an empty list. Extends the built-in set (nullability metadata, state machines, …), so that
		///     e.g. a marker attribute of a future compiler version can be excluded without a library update.
		/// </remarks>
		public ICustomizationValueSetter<string[]> ExcludedAttributeTypes { get; }

		/// <summary>
		///     The compiler-generated members that are included when reflecting over types and members.
		/// </summary>
		/// <remarks>
		///     Defaults to <see cref="CompilerGeneratedMembers.None" />, so that all compiler-generated members are
		///     excluded.
		/// </remarks>
		public ICustomizationValueSetter<CompilerGeneratedMembers> IncludedCompilerGeneratedMembers { get; }

		/// <summary>
		///     The special-name methods (operators and accessors) that are included when reflecting over methods.
		/// </summary>
		/// <remarks>
		///     Defaults to <see cref="SpecialNameMembers.None" />, so that all special-name methods are excluded.
		/// </remarks>
		public ICustomizationValueSetter<SpecialNameMembers> IncludedSpecialNameMembers { get; }

		/// <summary>
		///     Returns a copy of the stored array, so that changing it cannot bypass the scoping of the setting.
		/// </summary>
		private sealed class CopiedOnGet(ICustomizationValueSetter<string[]> inner)
			: ICustomizationValueSetter<string[]>
		{
			/// <inheritdoc cref="ICustomizationValueSetter{TValue}.Get()" />
			public string[] Get() => (string[])inner.Get().Clone();

			/// <inheritdoc cref="ICustomizationValueSetter{TValue}.Set(TValue)" />
			public CustomizationLifetime Set(string[] value) => inner.Set(value);
		}

		/// <summary>
		///     Returns the <paramref name="defaultValue" /> for a stored <see langword="null" />, so that setting
		///     <see langword="null" /> reverts to the default within its scope and <c>Get()</c> always returns the
		///     value in effect.
		/// </summary>
		private sealed class DefaultWhenNull<TValue>(ICustomizationValueSetter<TValue?> inner, TValue defaultValue)
			: ICustomizationValueSetter<TValue?>
			where TValue : class
		{
			/// <inheritdoc cref="ICustomizationValueSetter{TValue}.Get()" />
			public TValue Get() => inner.Get() ?? defaultValue;

			/// <inheritdoc cref="ICustomizationValueSetter{TValue}.Set(TValue)" />
			public CustomizationLifetime Set(TValue? value) => inner.Set(value);
		}
	}
}
