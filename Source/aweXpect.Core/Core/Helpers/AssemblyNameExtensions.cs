using System;
using System.Collections.Generic;

namespace aweXpect.Core.Helpers;

/// <remarks>
///     Also compiled into aweXpect.Reflection, so that one excluded assembly prefix has one meaning everywhere.
/// </remarks>
internal static class AssemblyNameExtensions
{
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
	///     either everything or nothing.
	/// </remarks>
	public static bool IsExcludedAssemblyName(this string? assemblyName, IReadOnlyList<string> excludedPrefixes)
	{
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
