namespace aweXpect.Snapshots.PublicApi;

/// <summary>
///     Options for generating the public API, see <see cref="PublicApiDeclarations" />.
/// </summary>
public sealed class PublicApiOptions
{
	internal bool IncludesForwardedTypes { get; private set; } = true;

	/// <summary>
	///     Excludes the types that the assembly forwards to another assembly, which are included by default as
	///     <c>[assembly: TypeForwardedTo(typeof(…))]</c>.
	/// </summary>
	public PublicApiOptions ExcludingForwardedTypes()
	{
		IncludesForwardedTypes = false;
		return this;
	}
}
