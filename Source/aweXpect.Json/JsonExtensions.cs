#if NET8_0_OR_GREATER
using System;
using aweXpect.Core;
using aweXpect.Json;
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
	public static TSelf AsJson<TType, TThat, TSelf>(
		this StringEqualityResult<TType, TThat, TSelf> result,
		Func<JsonOptions, JsonOptions>? options = null)
		where TSelf : StringEqualityResult<TType, TThat, TSelf>
	{
		JsonOptions jsonOptions = options?.Invoke(new JsonOptions()) ?? new JsonOptions();
		((IOptionsProvider<StringEqualityOptions>)result).Options.SetMatchType(new JsonMatchType(jsonOptions));
		return (TSelf)result;
	}
}

#endif
