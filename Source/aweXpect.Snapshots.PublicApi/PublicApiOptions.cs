using System;
using System.Collections.Generic;

namespace aweXpect.Snapshots.PublicApi;

/// <summary>
///     Options for generating the public API, see <see cref="PublicApiDeclarations" />.
/// </summary>
public sealed class PublicApiOptions
{
	private readonly HashSet<string> _excludedAttributes = new(StringComparer.Ordinal);

	internal bool IncludesForwardedTypes { get; private set; } = true;

	/// <summary>
	///     Excludes the attribute <typeparamref name="TAttribute" /> everywhere in the public API, for a generic
	///     attribute with any type arguments.
	/// </summary>
	public PublicApiOptions ExcludingAttribute<TAttribute>() where TAttribute : Attribute
		=> ExcludingAttribute((typeof(TAttribute).IsGenericType
			? typeof(TAttribute).GetGenericTypeDefinition()
			: typeof(TAttribute)).FullName!);

	/// <summary>
	///     Excludes the attribute with the <paramref name="fullName" /> everywhere in the public API, e.g. one that the
	///     tests cannot reference.
	/// </summary>
	/// <remarks>
	///     The <paramref name="fullName" /> is the one of <see cref="Type.FullName" />, e.g.
	///     <c>System.ObsoleteAttribute</c>, <c>MyNamespace.Outer+InnerAttribute</c> for a nested and
	///     <c>MyNamespace.TagAttribute`1</c> for a generic attribute.
	/// </remarks>
	public PublicApiOptions ExcludingAttribute(string fullName)
	{
		_excludedAttributes.Add(fullName);
		return this;
	}

	/// <summary>
	///     Excludes the types that the assembly forwards to another assembly, which are included by default as
	///     <c>[assembly: TypeForwardedTo(typeof(…))]</c>.
	/// </summary>
	public PublicApiOptions ExcludingForwardedTypes()
	{
		IncludesForwardedTypes = false;
		return this;
	}

	internal bool IsExcluded(string attributeFullName)
		=> _excludedAttributes.Contains(attributeFullName);
}
