using System;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Core.Extending;

namespace aweXpect.Reflection.Helpers;

/// <summary>
///     The failure for reflection over a subject while <see cref="ReflectionFallback.IsSupported" /> is off.
/// </summary>
/// <remarks>
///     The trimmer can remove the members, interfaces, types and assembly references of a subject, so reflecting
///     over them after trimming would silently check less than the expectation claims. Every method that reflects
///     over them checks the switch itself, because the trim analyzer does not follow the check into a called method
///     or a lambda. The message comes from <see cref="ReflectionFallback.NotSupported(string, string?)" />, like the
///     failure of the built-in expectations.
/// </remarks>
internal static class ReflectionFallbackHelpers
{
	/// <summary>
	///     The exception for the <paramref name="members" /> of the <paramref name="type" />.
	/// </summary>
	public static NotSupportedException NotSupported(Type type, string members)
		=> NotSupported($"The {members} of {Formatter.Format(type)}");

	/// <summary>
	///     The exception for the <paramref name="members" /> of the <paramref name="assembly" />.
	/// </summary>
	public static NotSupportedException NotSupported(Assembly assembly, string members)
		=> NotSupported($"The {members} of {Formatter.Format(assembly)}");

	private static NotSupportedException NotSupported(string what)
		=> Tracing.WriteException(ReflectionFallback.NotSupported(what, "Run the expectation without trimming."));
}
