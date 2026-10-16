using System;
using System.Text.Json;
using aweXpect.Helpers;

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
		ThrowHelper.ThrowIfNull(jsonDocumentOptions, nameof(jsonDocumentOptions));
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

	/// <summary>
	///     Ignores the order of the elements in all compared arrays
	///     when <paramref name="ignoreArrayOrder" /> is <see langword="true" />
	/// </summary>
	/// <remarks>
	///     Each expected element is paired with the first unpaired element of the subject that matches it. This greedy
	///     pairing can fail for overlapping <c>It.Is…</c> expectations, even when another pairing would succeed.
	///     <para />
	///     The positional members of <see cref="IJsonArrayResult" /> (<see cref="IJsonArrayResult.At(int)" />,
	///     <c>WithElements</c>, <c>WithArrays</c> and <c>WithObjects</c>) still verify the element at each index.
	/// </remarks>
	public static JsonOptions IgnoringArrayOrder(
		this JsonOptions @this,
		bool ignoreArrayOrder = true)
		=> @this with
		{
			IgnoreArrayOrder = ignoreArrayOrder,
		};

	/// <summary>
	///     Treats a property with the value <see langword="null" /> as if it was absent
	///     when <paramref name="treatNullAsAbsent" /> is <see langword="true" />
	/// </summary>
	/// <remarks>
	///     Elements of an array with the value <see langword="null" /> are not affected.
	/// </remarks>
	public static JsonOptions TreatingNullAsAbsent(
		this JsonOptions @this,
		bool treatNullAsAbsent = true)
		=> @this with
		{
			TreatNullAsAbsent = treatNullAsAbsent,
		};
}
