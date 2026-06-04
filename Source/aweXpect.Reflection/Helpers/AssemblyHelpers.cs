using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;

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
