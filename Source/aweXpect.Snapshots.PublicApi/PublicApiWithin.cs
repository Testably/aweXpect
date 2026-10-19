using System;
using System.Linq;
using aweXpect.Core;

namespace aweXpect.Snapshots.PublicApi;

/// <summary>
///     Selects what the public API is restricted to, e.g. <c>o => o.Within.Namespace("MyLibrary")</c>.
/// </summary>
/// <remarks>
///     Without a restriction, the whole public API is written; with several, every type that is within one of them.
///     <see cref="PublicApiOptions.Ignoring" /> wins over a restriction.
/// </remarks>
public sealed class PublicApiWithin
{
	private readonly PublicApiOptions _options;

	internal PublicApiWithin(PublicApiOptions options)
	{
		_options = options;
	}

	/// <summary>
	///     Restricts the public API to the types in the <paramref name="namespace" /> and in its sub-namespaces.
	/// </summary>
	/// <remarks>
	///     The namespace matches whole segments, so <c>MyLibrary</c> does not include <c>MyLibraryTools</c>. An empty
	///     <paramref name="namespace" /> is the global namespace.
	/// </remarks>
	public PublicApiOptions Namespace(string @namespace)
	{
		_options.NamespacesToRestrictTo.Add(ArgumentChecks.ThrowIfNull(@namespace, nameof(@namespace)));
		return _options;
	}

	/// <summary>
	///     Restricts the public API to the type <typeparamref name="TType" /> with its nested types, for a generic type
	///     with any type arguments.
	/// </summary>
	/// <exception cref="ArgumentException"><typeparamref name="TType" /> is a nested type.</exception>
	public PublicApiOptions Type<TType>()
		=> Type(PublicApiOptions.FullNameOf(typeof(TType)));

	/// <summary>
	///     Restricts the public API to the type with the <paramref name="fullName" /> with its nested types, e.g. one that
	///     the tests cannot reference.
	/// </summary>
	/// <remarks>
	///     The <paramref name="fullName" /> is the one of <see cref="System.Type.FullName" />, e.g.
	///     <c>MyNamespace.Box`1</c> for a generic type.
	/// </remarks>
	/// <exception cref="ArgumentException">
	///     The <paramref name="fullName" /> is empty or the one of a nested type.
	/// </exception>
	public PublicApiOptions Type(string fullName)
	{
		if (ArgumentChecks.ThrowIfNullOrEmpty(fullName, nameof(fullName)).Contains('+'))
		{
			throw Tracing.WriteException(new ArgumentException(
				$"The type '{fullName}' is nested, so it is written with its containing type: restrict the public API to the containing type instead.",
				nameof(fullName)));
		}

		_options.TypesToRestrictTo.Add(fullName);
		return _options;
	}
}
