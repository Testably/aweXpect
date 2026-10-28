namespace aweXpect.Core;

/// <summary>
///     Applies options to a result that offers them with an <see cref="IOptionsProvider{TOptions}" />.
/// </summary>
public static class OptionsProvider
{
	/// <summary>
	///     Returns the <paramref name="result" /> with the <paramref name="options" />: a copy when it implements
	///     <see cref="ICopyOnOption{TOptions, TSelf}" />, otherwise the <paramref name="result" /> itself with the
	///     <paramref name="options" /> in its <see cref="OptionsHolder{TOptions}" />.
	/// </summary>
	/// <remarks>
	///     Use it in an option method of your own, so that it keeps a stored result unchanged, e.g.
	///     <c>=&gt; OptionsProvider.WithOptions(result, result.Options.Value.IgnoringCase());</c>.
	/// </remarks>
	public static TSelf WithOptions<TSelf, TOptions>(TSelf result, TOptions options)
		where TSelf : IOptionsProvider<TOptions>
	{
		if (result is ICopyOnOption<TOptions, TSelf> copyOnOption)
		{
			return copyOnOption.WithOptions(options);
		}

		result.Options.Value = options;
		return result;
	}
}
