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
	public static TResult AsJson<TResult>(
		this TResult result,
		Func<JsonOptions, JsonOptions>? options = null)
		where TResult : IOptionsProvider<StringEqualityOptions>, IStringMatchTypeOptions
	{
		JsonOptions jsonOptions = options?.Invoke(new JsonOptions()) ?? new JsonOptions();
		result.Options.SetMatchType(new JsonMatchType(jsonOptions), nameof(AsJson));
		return result;
	}
}
