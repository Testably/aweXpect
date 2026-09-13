using System;

namespace aweXpect.Core.Metadata;

/// <summary>
///     Marks a parameter or type parameter whose type needs its fields and properties registered.
/// </summary>
/// <remarks>
///     The source generator seeds <see cref="TypeMetadataRegistry" /> from the static type at each call site, and
///     follows it through its declared members.
///     <para />
///     Apply it to every API whose argument reaches an equivalency comparison, including extension methods that only
///     forward to one: a generic extension method carries just an open type parameter at its own call site, so a
///     consumer's call site is the only place the concrete type is visible.
/// </remarks>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.GenericParameter)]
public sealed class RequiresMemberMetadataAttribute : Attribute;
