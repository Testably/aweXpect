using System;

namespace aweXpect.Core.Metadata;

/// <summary>
///     Marks a parameter or type parameter whose type needs its events registered.
/// </summary>
/// <remarks>
///     The source generator seeds <see cref="TypeMetadataRegistry" /> from the static type at each call site.
/// </remarks>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.GenericParameter)]
public sealed class RequiresEventMetadataAttribute : Attribute;
