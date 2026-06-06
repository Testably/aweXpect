using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using aweXpect.Customization;
using aweXpect.Options;

namespace aweXpect.Reflection.Helpers;

/// <summary>
///     Extension methods for <see cref="Assembly" />.
/// </summary>
internal static class AssemblyHelpers
{
	/// <summary>
	///     Checks if the <paramref name="assembly" /> has an attribute which satisfies the <paramref name="predicate" />.
	/// </summary>
	/// <typeparam name="TAttribute">The type of the <see cref="Attribute" />.</typeparam>
	/// <param name="assembly">The <see cref="Assembly" /> which is checked to have the attribute.</param>
	/// <param name="predicate">
	///     (optional) A predicate to check the attribute values.
	///     <para />
	///     If not set (<see langword="null" />), will only check if the attribute is present.
	/// </param>
	/// <param name="inherit">
	///     <see langword="true" /> to search the inheritance chain to find the attributes; otherwise,
	///     <see langword="false" />.<br />
	///     Defaults to <see langword="true" />
	/// </param>
	public static bool HasAttribute<TAttribute>(
		this Assembly assembly,
		Func<TAttribute, bool>? predicate = null,
		bool inherit = true)
		where TAttribute : Attribute
	{
		object? attribute = Attribute.GetCustomAttributes(assembly, typeof(TAttribute), inherit)
			.FirstOrDefault();
		if (attribute is TAttribute castedAttribute)
		{
			return predicate?.Invoke(castedAttribute) ?? true;
		}

		return false;
	}

	/// <summary>
	///     Checks if the <paramref name="assembly" /> has an attribute which satisfies the <paramref name="predicate" />.
	/// </summary>
	/// <param name="assembly">The <see cref="Assembly" /> which is checked to have the attribute.</param>
	/// <param name="attributeType">The type of the attribute to check for.</param>
	/// <param name="predicate">
	///     (optional) A predicate to check the attribute values.
	///     <para />
	///     If not set (<see langword="null" />), will only check if the attribute is present.
	/// </param>
	/// <param name="inherit">
	///     <see langword="true" /> to search the inheritance chain to find the attributes; otherwise,
	///     <see langword="false" />.<br />
	///     Defaults to <see langword="true" />
	/// </param>
	public static bool HasAttribute(
		this Assembly? assembly,
		Type attributeType,
		Func<Attribute, bool>? predicate = null,
		bool inherit = true)
	{
		object? attribute = assembly?.GetCustomAttributes(attributeType, inherit)
			.FirstOrDefault();
		if (attribute is Attribute attributeValue)
		{
			return predicate?.Invoke(attributeValue) ?? true;
		}

		return false;
	}

	/// <summary>
	///     Gets the target framework moniker (e.g. <c>net8.0</c>) of the <paramref name="assembly" />,
	///     or <see langword="null" /> when it cannot be determined.
	/// </summary>
	/// <remarks>
	///     The moniker is derived from the assembly-level
	///     <see cref="TargetFrameworkAttribute" />. When the attribute is absent,
	///     <see langword="null" /> is returned (treated as "no target").
	/// </remarks>
	public static string? GetTargetFramework(this Assembly? assembly)
		=> MapFrameworkName(assembly?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName);

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
	public static bool IsExcludedAssemblyName(this string? assemblyName, string[] excludedPrefixes)
		=> assemblyName is not null &&
		   excludedPrefixes.Any(prefix
			   => prefix.Length > 0 &&
			      assemblyName.StartsWith(prefix, StringComparison.Ordinal) &&
			      (prefix.EndsWith(".", StringComparison.Ordinal) ||
			       assemblyName.Length == prefix.Length ||
			       assemblyName[prefix.Length] == '.'));

	/// <summary>
	///     Returns the names of all assemblies the <paramref name="assembly" /> references which are neither
	///     covered by the
	///     <see cref="AwexpectCustomization.ReflectionCustomizationValue.ExcludedAssemblyPrefixes" /> nor
	///     considered equal to one of the <paramref name="allowed" /> names.
	/// </summary>
	/// <remarks>
	///     Shared by the single-assembly and collection assertions and the assembly filter, so that the three
	///     code paths cannot drift apart in how a disallowed dependency is determined.
	/// </remarks>
	public static async Task<string?[]> GetDisallowedAssemblyDependencies(this Assembly assembly,
		string[] allowed, StringEqualityOptions options)
	{
		string[] prefixes = Customize.aweXpect.Reflection().ExcludedAssemblyPrefixes.Get();
		List<string?> violations = [];
		foreach (AssemblyName dependency in assembly.GetReferencedAssemblies())
		{
			if (dependency.Name.IsExcludedAssemblyName(prefixes))
			{
				continue;
			}

			if (!await allowed.AnyAsync(expected => options.AreConsideredEqual(dependency.Name, expected)))
			{
				violations.Add(dependency.Name);
			}
		}

		return violations.ToArray();
	}

	/// <summary>
	///     Checks if the <paramref name="assembly" /> is strong named.
	/// </summary>
	/// <remarks>
	///     An assembly is considered strong named if its name carries a non-empty public key token.
	/// </remarks>
	public static bool IsStrongNamed(this Assembly? assembly)
		=> assembly?.GetName().GetPublicKeyToken() is { Length: > 0, };

	/// <summary>
	///     Maps a <see cref="TargetFrameworkAttribute.FrameworkName" /> (e.g. <c>.NETCoreApp,Version=v8.0</c>)
	///     to its short target framework moniker (e.g. <c>net8.0</c>).
	/// </summary>
	/// <remarks>
	///     Returns <see langword="null" /> when <paramref name="frameworkName" /> is <see langword="null" /> or
	///     blank, and the unmodified <paramref name="frameworkName" /> when it cannot be parsed.
	/// </remarks>
	public static string? MapFrameworkName(string? frameworkName)
	{
		const string versionPrefix = "Version=v";
		if (string.IsNullOrWhiteSpace(frameworkName))
		{
			return null;
		}

		string[] parts = frameworkName!.Split(',');
		Version? version = null;
		foreach (string part in parts)
		{
			string trimmed = part.Trim();
			if (trimmed.StartsWith(versionPrefix, StringComparison.OrdinalIgnoreCase) &&
			    Version.TryParse(trimmed.Substring(versionPrefix.Length), out Version? parsed))
			{
				version = parsed;
			}
		}

		if (version is null)
		{
			return frameworkName;
		}

		return parts[0].Trim() switch
		{
			".NETCoreApp" => version.Major >= 5
				? $"net{version.Major}.{version.Minor}"
				: $"netcoreapp{version.Major}.{version.Minor}",
			".NETStandard" => $"netstandard{version.Major}.{version.Minor}",
			".NETFramework" => $"net{version.Major}{version.Minor}{(version.Build > 0 ? $"{version.Build}" : "")}",
			_ => frameworkName,
		};
	}
}
