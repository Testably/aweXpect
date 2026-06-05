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
}
