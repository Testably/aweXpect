namespace aweXpect.Snapshots.PublicApi;

/// <summary>
///     Selects how the public API is formatted, e.g. <c>o => o.Formatting.ParametersOnSeparateLines()</c>.
/// </summary>
public sealed class PublicApiFormatting
{
	private readonly PublicApiOptions _options;

	internal PublicApiFormatting(PublicApiOptions options)
	{
		_options = options;
	}

	/// <summary>
	///     Writes the parameters of methods, constructors, operators, delegates and indexers with more than one parameter
	///     on separate lines, so that a change of a single parameter shows up as a change of a single line.
	/// </summary>
	public PublicApiOptions ParametersOnSeparateLines()
	{
		_options.WritesParametersOnSeparateLines = true;
		return _options;
	}
}
