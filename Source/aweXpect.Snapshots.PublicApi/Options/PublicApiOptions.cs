using System;
using System.Collections.Generic;
using aweXpect.Core;

namespace aweXpect.Snapshots.PublicApi.Options;

/// <summary>
///     Options for generating the public API, see <see cref="PublicApiDeclarations" />.
/// </summary>
/// <remarks>
///     The options are selected like the rules of a snapshot, e.g.
///     <c>o => o.Within.Namespace("MyLibrary").Ignoring.Type&lt;Legacy&gt;()</c>.
///     <para />
///     The options are immutable: each option returns a modified copy.
/// </remarks>
[ImmutableOptions]
public sealed class PublicApiOptions
{
	/// <summary>
	///     Creates options without any active option.
	/// </summary>
	internal PublicApiOptions()
	{
	}

	/// <summary>
	///     Copies the <paramref name="options" />, so that an option only has to set what it changes.
	/// </summary>
	/// <remarks>
	///     Every option must be copied here.
	/// </remarks>
	internal PublicApiOptions(PublicApiOptions options)
	{
		AttributesToIgnore = options.AttributesToIgnore;
		NamespacesToIgnore = options.NamespacesToIgnore;
		TypesToIgnore = options.TypesToIgnore;
		IncludesCompilerGeneratedMembers = options.IncludesCompilerGeneratedMembers;
		IncludesForwardedTypes = options.IncludesForwardedTypes;
		NamespacesToRestrictTo = options.NamespacesToRestrictTo;
		TypesToRestrictTo = options.TypesToRestrictTo;
		WritesParametersOnSeparateLines = options.WritesParametersOnSeparateLines;
	}

	/// <summary>
	///     Selects how the public API is formatted.
	/// </summary>
	public PublicApiFormatting Formatting => new(this);

	/// <summary>
	///     Selects what is left out of the public API.
	/// </summary>
	public PublicApiIgnoring Ignoring => new(this);

	/// <summary>
	///     Selects what the public API is restricted to.
	/// </summary>
	public PublicApiWithin Within => new(this);

	// The sets are shared between copies, so an option replaces them via Add instead of changing them.
	internal HashSet<string> AttributesToIgnore { get; init; } = new(StringComparer.Ordinal);
	internal HashSet<string> NamespacesToIgnore { get; init; } = new(StringComparer.Ordinal);
	internal HashSet<string> TypesToIgnore { get; init; } = new(StringComparer.Ordinal);
	internal bool IncludesCompilerGeneratedMembers { get; init; } = true;
	internal bool IncludesForwardedTypes { get; init; } = true;
	internal HashSet<string> NamespacesToRestrictTo { get; init; } = new(StringComparer.Ordinal);
	internal HashSet<string> TypesToRestrictTo { get; init; } = new(StringComparer.Ordinal);
	internal bool WritesParametersOnSeparateLines { get; init; }

	/// <summary>
	///     A copy of the <paramref name="set" /> that also contains the <paramref name="value" />.
	/// </summary>
	internal static HashSet<string> Add(HashSet<string> set, string value)
		=> new(set, StringComparer.Ordinal) { value, };

	/// <summary>
	///     The name of the <paramref name="type" /> like <see cref="Type.FullName" />, for a generic type the one of its
	///     definition, so that it applies with any type arguments.
	/// </summary>
	internal static string FullNameOf(Type type)
		=> (type.IsGenericType ? type.GetGenericTypeDefinition() : type).FullName!;
}
