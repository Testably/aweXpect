using System;
using System.Collections.Generic;

namespace aweXpect.Snapshots.PublicApi;

/// <summary>
///     Options for generating the public API, see <see cref="PublicApiDeclarations" />.
/// </summary>
/// <remarks>
///     The options are selected like the rules of a snapshot, e.g.
///     <c>o => o.Within.Namespace("MyLibrary").Ignoring.Type&lt;Legacy&gt;()</c>.
/// </remarks>
public sealed class PublicApiOptions
{
	/// <summary>
	///     Selects what is left out of the public API.
	/// </summary>
	public PublicApiIgnoring Ignoring => new(this);

	/// <summary>
	///     Selects what the public API is restricted to.
	/// </summary>
	public PublicApiWithin Within => new(this);

	internal HashSet<string> AttributesToIgnore { get; } = new(StringComparer.Ordinal);
	internal HashSet<string> NamespacesToIgnore { get; } = new(StringComparer.Ordinal);
	internal HashSet<string> TypesToIgnore { get; } = new(StringComparer.Ordinal);
	internal bool IncludesCompilerGeneratedMembers { get; set; } = true;
	internal bool IncludesForwardedTypes { get; set; } = true;
	internal HashSet<string> NamespacesToRestrictTo { get; } = new(StringComparer.Ordinal);
	internal HashSet<string> TypesToRestrictTo { get; } = new(StringComparer.Ordinal);

	/// <summary>
	///     The name of the <paramref name="type" /> like <see cref="Type.FullName" />, for a generic type the one of its
	///     definition, so that it applies with any type arguments.
	/// </summary>
	internal static string FullNameOf(Type type)
		=> (type.IsGenericType ? type.GetGenericTypeDefinition() : type).FullName!;
}
