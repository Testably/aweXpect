using System.Runtime.CompilerServices;

namespace aweXpect.Reflection.Formatting;

internal static class FormatterRegistration
{
	/// <summary>
	///     Registers the reflection value formatters when the assembly is loaded.
	/// </summary>
	/// <remarks>
	///     A module initializer keeps the registration reachable without reflection, so it survives trimming and
	///     Native AOT - which is why CA2255 (discouraging module initializers in libraries) is suppressed here.
	///     The registrations intentionally last for the lifetime of the process, hence the
	///     <see cref="System.IDisposable" /> handles returned by <see cref="ValueFormatter.Register" /> are discarded.
	/// </remarks>
#pragma warning disable CA2255
	[ModuleInitializer]
	internal static void Initialize()
	{
		ValueFormatter.Register(new ConstructorFormatter());
		ValueFormatter.Register(new EventFormatter());
		ValueFormatter.Register(new FieldFormatter());
		ValueFormatter.Register(new MethodFormatter());
		ValueFormatter.Register(new PropertyFormatter());
	}
#pragma warning restore CA2255
}
