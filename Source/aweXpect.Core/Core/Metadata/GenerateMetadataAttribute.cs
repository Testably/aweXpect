using System;

namespace aweXpect.Core.Metadata;

/// <summary>
///     Names a type whose metadata is needed although no marked call site reveals it.
/// </summary>
/// <remarks>
///     The source generator only sees declared types, so a member declared as <see langword="object" /> or as a base
///     type that holds a derived instance stays invisible to it. Naming such a type here registers it anyway.
/// </remarks>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class GenerateMetadataAttribute(Type type) : Attribute
{
	/// <summary>
	///     The type whose metadata is needed.
	/// </summary>
	public Type Type { get; } = type;
}
