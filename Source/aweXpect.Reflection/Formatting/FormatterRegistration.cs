using System.Runtime.CompilerServices;

namespace aweXpect.Reflection.Formatting;

internal static class FormatterRegistration
{
	/// <summary>
	///     Registers the formatter for the <see cref="FormattableMember" /> when the assembly is loaded.
	/// </summary>
	/// <remarks>
	///     A module initializer keeps the registration reachable without reflection, so it survives trimming and
	///     Native AOT - which is why CA2255 (discouraging module initializers in libraries) is suppressed here.
	///     The registration intentionally lasts for the lifetime of the process, hence the
	///     <see cref="System.IDisposable" /> handle returned by <see cref="ValueFormatter.Register" /> is discarded.
	/// </remarks>
#pragma warning disable CA2255
	[ModuleInitializer]
	internal static void Initialize()
		=> ValueFormatter.Register(new FormattableMember.MemberFormatter());
#pragma warning restore CA2255
}
