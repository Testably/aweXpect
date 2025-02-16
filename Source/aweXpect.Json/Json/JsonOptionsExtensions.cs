using System;
using System.Text.Json;

namespace aweXpect.Json;

/// <summary>
///     Extension methods for <see cref="JsonOptions" />.
/// </summary>
public static class JsonOptionsExtensions
{
	/// <summary>
	///     Specify the <see cref="JsonDocumentOptions" /> to use when interpreting a <see langword="string" /> as JSON.
	/// </summary>
	public static JsonOptions WithJsonOptions(
		this JsonOptions @this,
		Func<JsonDocumentOptions, JsonDocumentOptions> jsonDocumentOptions)
	{
		JsonDocumentOptions options = jsonDocumentOptions(@this.DocumentOptions);
		return @this with
		{
			DocumentOptions = options,
		};
	}

	/// <summary>
	///     Ignores additional properties in the subject
	///     when <paramref name="ignoreAdditionalProperties" /> is <see langword="true" />
	/// </summary>
	public static JsonOptions IgnoringAdditionalProperties(
		this JsonOptions @this,
		bool ignoreAdditionalProperties = true)
		=> @this with
		{
			IgnoreAdditionalProperties = ignoreAdditionalProperties,
		};
}
