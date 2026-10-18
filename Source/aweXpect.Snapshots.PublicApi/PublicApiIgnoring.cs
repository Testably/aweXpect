namespace aweXpect.Snapshots.PublicApi;

/// <summary>
///     Selects what is left out of the public API, e.g. <c>o => o.Ignoring.Namespace("MyLibrary.Generated")</c>.
/// </summary>
/// <remarks>
///     Ignoring wins over <see cref="PublicApiOptions.Within" />, e.g. to leave single types out of a namespace that the
///     public API is restricted to.
/// </remarks>
public sealed class PublicApiIgnoring
{
	private readonly PublicApiOptions _options;

	internal PublicApiIgnoring(PublicApiOptions options)
	{
		_options = options;
	}

	/// <summary>
	///     Ignores the attribute <typeparamref name="TAttribute" /> everywhere in the public API, for a generic
	///     attribute with any type arguments.
	/// </summary>
	public PublicApiOptions Attribute<TAttribute>() where TAttribute : System.Attribute
		=> Attribute(PublicApiOptions.FullNameOf(typeof(TAttribute)));

	/// <summary>
	///     Ignores the attribute with the <paramref name="fullName" /> everywhere in the public API, e.g. one that the
	///     tests cannot reference.
	/// </summary>
	/// <remarks>
	///     The <paramref name="fullName" /> is the one of <see cref="System.Type.FullName" />, e.g.
	///     <c>System.ObsoleteAttribute</c>, <c>MyNamespace.Outer+InnerAttribute</c> for a nested and
	///     <c>MyNamespace.TagAttribute`1</c> for a generic attribute.
	/// </remarks>
	public PublicApiOptions Attribute(string fullName)
	{
		_options.AttributesToIgnore.Add(fullName);
		return _options;
	}

	/// <summary>
	///     Ignores the types that the assembly forwards to another assembly, which are written by default as
	///     <c>[assembly: TypeForwardedTo(typeof(…))]</c>.
	/// </summary>
	public PublicApiOptions ForwardedTypes()
	{
		_options.IncludesForwardedTypes = false;
		return _options;
	}

	/// <summary>
	///     Ignores the types in the <paramref name="namespace" /> and in its sub-namespaces.
	/// </summary>
	/// <remarks>
	///     The namespace matches whole segments, so <c>MyLibrary.Generated</c> does not ignore
	///     <c>MyLibrary.GeneratedTools</c>.
	/// </remarks>
	public PublicApiOptions Namespace(string @namespace)
	{
		_options.NamespacesToIgnore.Add(@namespace);
		return _options;
	}

	/// <summary>
	///     Ignores the type <typeparamref name="TType" /> with its nested types, for a generic type with any type
	///     arguments.
	/// </summary>
	public PublicApiOptions Type<TType>()
		=> Type(PublicApiOptions.FullNameOf(typeof(TType)));

	/// <summary>
	///     Ignores the type with the <paramref name="fullName" /> with its nested types, e.g. one that the tests cannot
	///     reference.
	/// </summary>
	/// <remarks>
	///     The <paramref name="fullName" /> is the one of <see cref="System.Type.FullName" />, e.g.
	///     <c>MyNamespace.Outer+Inner</c> for a nested and <c>MyNamespace.Box`1</c> for a generic type.
	/// </remarks>
	public PublicApiOptions Type(string fullName)
	{
		_options.TypesToIgnore.Add(fullName);
		return _options;
	}
}
