using System;
using aweXpect.Core;
using aweXpect.Json;
using aweXpect.Json.Options;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

/// <summary>
///     Extension methods for working with JSON strings.
/// </summary>
public static class JsonExtensions
{
	/// <summary>
	///     Interpret the <see cref="string" /> as JSON.
	/// </summary>
	/// <remarks>
	///     The active <paramref name="options" /> are listed in the "JSON options" context of a failed comparison.
	/// </remarks>
	public static TSelf AsJson<TSelf>(
		this TSelf result,
		Func<JsonOptions, JsonOptions>? options = null)
		where TSelf : IOptionsProvider<StringEqualityOptions>, IStringMatchTypeOptions
	{
		JsonOptions jsonOptions = options?.Invoke(JsonOptions.Default) ?? JsonOptions.Default;
		JsonMatchType matchType = new(jsonOptions);
		if (result is ICopyOnOption<StringEqualityOptions, TSelf> copyOnOption)
		{
			return copyOnOption.WithOptions(result.Options.Value.SetMatchType(matchType, nameof(AsJson)));
		}

		result.Options.SetMatchType(matchType, nameof(AsJson));
		return result;
	}
}
