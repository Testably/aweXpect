using System;
using aweXpect.Core.Metadata;
#if NET8_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
#endif

namespace aweXpect.Core.Extending;

/// <summary>
///     Whether a type without a registration in the <see cref="TypeMetadataRegistry" /> is reflected over.
/// </summary>
/// <remarks>
///     Reflection is complete under the JIT and unreliable when publishing with trimming or Native AOT enabled, where
///     the trimmer removes members and the generic event recorder cannot be instantiated. The fallback therefore
///     defaults to whether dynamic code is supported, a trimmer that honours feature guards treats it as unavailable,
///     and the <see cref="SwitchName" /> runtime switch forces it either way.
///     <para />
///     An extension that reflects over a subject should guard its reflection with <see cref="IsSupported" /> and
///     throw the exception of <see cref="NotSupported(string, string?)" /> otherwise.
/// </remarks>
public static class ReflectionFallback
{
	/// <summary>
	///     The name of the runtime switch that forces the fallback on or off.
	/// </summary>
	/// <remarks>
	///     Name it in the message of an extension that cannot reflect over a subject while <see cref="IsSupported" /> is
	///     <see langword="false" />, so that users learn how to reflect anyway.
	/// </remarks>
	public const string SwitchName = "aweXpect.ReflectionFallback.IsSupported";

	/// <summary>
	///     Whether the fallback is available.
	/// </summary>
#if NET8_0_OR_GREATER
	[FeatureSwitchDefinition(SwitchName)]
	[FeatureGuard(typeof(RequiresUnreferencedCodeAttribute))]
	[FeatureGuard(typeof(RequiresDynamicCodeAttribute))]
#endif
	public static bool IsSupported { get; } = AppContext.TryGetSwitch(SwitchName, out bool isSupported)
		? isSupported
		: IsSupportedByDefault;

	/// <remarks>
	///     Without the switch, the fallback follows dynamic code support, which Native AOT lacks; a target that
	///     cannot be published that way always has it.
	/// </remarks>
	private static bool IsSupportedByDefault
#if NET8_0_OR_GREATER
		=> RuntimeFeature.IsDynamicCodeSupported;
#else
		=> true;
#endif

	/// <summary>
	///     The exception for <paramref name="what" /> that cannot be found while <see cref="IsSupported" /> is
	///     <see langword="false" />, naming the <paramref name="remedy" /> and the <see cref="SwitchName" /> runtime switch.
	/// </summary>
	/// <param name="what">The subject of the sentence, e.g. <c>"The properties of Album"</c>.</param>
	/// <param name="remedy">An optional sentence that tells how to avoid reflection, e.g. <c>"Register the type."</c></param>
	public static NotSupportedException NotSupported(string what, string? remedy = null)
		=> new(
			$"{what} cannot be found by reflection, which is switched off when publishing with trimming or Native AOT enabled. {(remedy is null ? "Set" : $"{remedy} Alternatively, set")} the runtime switch '{SwitchName}' to true to reflect anyway.");

	/// <summary>
	///     The exception for the <paramref name="members" /> of a <paramref name="type" /> the fallback cannot provide.
	/// </summary>
	/// <remarks>
	///     A compiler-generated type, such as an anonymous one, cannot be named in an attribute, so the generator has
	///     to see it at a marked call site instead, which it cannot through a member typed as <see cref="object" /> or
	///     in another assembly than the one creating it.
	/// </remarks>
	internal static NotSupportedException NotSupported(Type type, string members)
		=> NotSupported($"The {members} of {Formatter.Format(type)}",
			type.Name.Length > 0 && type.Name[0] == '<'
				? "Let the source generator see the type as the static type of an argument at a marked call site, or of a member reached from one, in the assembly that creates it."
				: $"Register the type, for example with [assembly: GenerateMetadata(typeof({Formatter.Format(type)}))].");
}
