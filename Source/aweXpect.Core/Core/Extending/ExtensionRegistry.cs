using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using aweXpect.Core.Helpers;

namespace aweXpect.Core.Extending;

/// <summary>
///     Values that the build of a test project records for an extension, registered when the test assembly is loaded,
///     so that the extension reads them without scanning the loaded assemblies.
/// </summary>
/// <remarks>
///     A source generator of aweXpect.Core registers them from a module initializer in the test project. The build
///     targets of an extension name the keys of the <c>AssemblyMetadataAttribute</c>s whose values are registered in
///     the MSBuild property <c>AweXpectRegisteredMetadataKeys</c>.
/// </remarks>
public static class ExtensionRegistry
{
	private static readonly ConcurrentDictionary<string, IReadOnlyList<string>> Values = new(StringComparer.Ordinal);
	private static readonly object RegisterLock = new();

	/// <summary>
	///     Registers the <paramref name="value" /> under the <paramref name="key" />.
	/// </summary>
	/// <remarks>
	///     Each test assembly registers its own values, so a value that is registered several times is kept each time.
	/// </remarks>
	/// <exception cref="ArgumentNullException">The <paramref name="key" /> or <paramref name="value" /> is <see langword="null" />.</exception>
	public static void Register(string key, string value)
	{
		key.ThrowIfNull();
		value.ThrowIfNull();
		lock (RegisterLock)
		{
			IReadOnlyList<string> registered = GetValues(key);
			string[] values = new string[registered.Count + 1];
			for (int i = 0; i < registered.Count; i++)
			{
				values[i] = registered[i];
			}

			values[registered.Count] = value;
			Values[key] = Array.AsReadOnly(values);
		}
	}

	/// <summary>
	///     Returns the values that are registered under the <paramref name="key" />, in the order of their registration.
	/// </summary>
	/// <remarks>
	///     The returned list does not change; it is the same instance until another value is registered under the
	///     <paramref name="key" />, so a caller can keep what it derived from it until then.
	/// </remarks>
	/// <exception cref="ArgumentNullException">The <paramref name="key" /> is <see langword="null" />.</exception>
	public static IReadOnlyList<string> GetValues(string key)
	{
		key.ThrowIfNull();
		return Values.TryGetValue(key, out IReadOnlyList<string>? values) ? values : [];
	}
}
