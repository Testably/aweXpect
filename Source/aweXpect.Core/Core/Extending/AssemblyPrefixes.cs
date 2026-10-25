using System;
using System.Collections.Generic;
using aweXpect.Core.Helpers;
using aweXpect.Customization;

namespace aweXpect.Core.Extending;

/// <summary>
///     Matches assembly names against the
///     <see cref="AwexpectCustomization.SettingsCustomization.ExcludedAssemblyPrefixes" />, so that an
///     extension that scans assemblies excludes the same assemblies as aweXpect.
/// </summary>
public static class AssemblyPrefixes
{
	/// <summary>
	///     Checks if the <paramref name="assemblyName" /> is excluded by the current
	///     <see cref="AwexpectCustomization.SettingsCustomization.ExcludedAssemblyPrefixes" />.
	/// </summary>
	/// <remarks>
	///     The setting is read on each call. When you check many names, read it once and use
	///     <see cref="IsExcluded(string?, IReadOnlyList{string})" /> instead.
	/// </remarks>
	public static bool IsExcluded(string? assemblyName)
		=> IsExcluded(assemblyName, Customize.aweXpect.Settings().ExcludedAssemblyPrefixes.Get());

	/// <summary>
	///     Checks if the <paramref name="assemblyName" /> matches one of the <paramref name="excludedPrefixes" />
	///     at a name-segment boundary, i.e. it equals the prefix or continues with a <c>.</c> after it.
	/// </summary>
	/// <remarks>
	///     The boundary check prevents unrelated assemblies from being swallowed by a framework prefix:
	///     <c>System</c> matches <c>System</c> and <c>System.Text.Json</c>, but not <c>SystemsBiology.Core</c>.
	///     <para />
	///     A prefix that already ends with the <c>.</c> separator (e.g. a customized <c>MyCompany.</c>) is
	///     boundary-safe by construction and matches everything that starts with it.
	///     <para />
	///     An empty prefix is ignored: it cannot identify an assembly and would otherwise silently exclude
	///     either everything or nothing. A <see langword="null" /> <paramref name="assemblyName" /> is never excluded.
	///     The names are compared ordinally, so the case must match.
	/// </remarks>
	/// <exception cref="ArgumentNullException">The <paramref name="excludedPrefixes" /> are <see langword="null" />.</exception>
	public static bool IsExcluded(string? assemblyName, IReadOnlyList<string> excludedPrefixes)
	{
		excludedPrefixes.ThrowIfNull();
		if (assemblyName is null)
		{
			return false;
		}

		for (int i = 0; i < excludedPrefixes.Count; i++)
		{
			string prefix = excludedPrefixes[i];
			if (prefix.Length > 0 &&
			    assemblyName.StartsWith(prefix, StringComparison.Ordinal) &&
			    (prefix[prefix.Length - 1] == '.' ||
			     assemblyName.Length == prefix.Length ||
			     assemblyName[prefix.Length] == '.'))
			{
				return true;
			}
		}

		return false;
	}
}
