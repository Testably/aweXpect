using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using aweXpect.Core.Metadata;

namespace aweXpect.Core;

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
///     fail with a message naming the switch otherwise.
/// </remarks>
public static class ReflectionFallback
{
	/// <summary>
	///     The name of the runtime switch that forces the fallback on or off.
	/// </summary>
	internal const string SwitchName = "aweXpect.ReflectionFallback.IsSupported";

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
	///     The exception for <paramref name="what" /> the fallback cannot provide, naming the <paramref name="remedy" />.
	/// </summary>
	internal static NotSupportedException NotSupported(string what, string remedy)
		=> new(
			$"{what} cannot be found by reflection, which is switched off when publishing with trimming or Native AOT enabled. {remedy} Alternatively, set the runtime switch '{SwitchName}' to true to reflect anyway.");

	/// <summary>
	///     The exception for the <paramref name="members" /> of a <paramref name="type" /> the fallback cannot provide.
	/// </summary>
	/// <remarks>
	///     A compiler-generated type, such as an anonymous one, cannot be named in an attribute, so the generator has
	///     to see it at a marked call site instead.
	/// </remarks>
	internal static NotSupportedException NotSupported(Type type, string members)
		=> NotSupported($"The {members} of {Formatter.Format(type)}",
			type.Name.Length > 0 && type.Name[0] == '<'
				? "Let the source generator see the type at a marked call site."
				: $"Register the type, for example with [assembly: GenerateMetadata(typeof({Formatter.Format(type)}))].");
}
