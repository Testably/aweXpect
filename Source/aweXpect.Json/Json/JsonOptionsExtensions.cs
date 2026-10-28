using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Json;
using aweXpect.Json.Options;

namespace aweXpect;

/// <summary>
///     Extension methods for <see cref="JsonOptions" />.
/// </summary>
public static class JsonOptionsExtensions
{
	/// <summary>
	///     Specify the <see cref="JsonDocumentOptions" /> to use when interpreting a <see langword="string" /> as JSON.
	/// </summary>
	public static JsonOptions WithDocumentOptions(
		this JsonOptions options,
		Func<JsonDocumentOptions, JsonDocumentOptions> documentOptions)
	{
		ThrowHelper.ThrowIfNull(documentOptions, nameof(documentOptions));
		return new JsonOptions(options)
		{
			DocumentOptions = documentOptions(options.DocumentOptions),
		};
	}

	/// <summary>
	///     Serializes the expected value with the <paramref name="serializerOptions" />, e.g. with a
	///     <see cref="JsonSerializerOptions.PropertyNamingPolicy" /> or the options of a source-generated
	///     <c>JsonSerializerContext</c>, instead of the customizable default.
	/// </summary>
	/// <remarks>
	///     With a <see cref="JsonSerializerOptions.TypeInfoResolver" />, the expected value is serialized without reflection,
	///     so the type of the expected value must be known to the resolver.
	/// </remarks>
	public static JsonOptions WithSerializerOptions(
		this JsonOptions options,
		JsonSerializerOptions serializerOptions)
	{
		ThrowHelper.ThrowIfNull(serializerOptions, nameof(serializerOptions));
		return new JsonOptions(options)
		{
			SerializerOptions = serializerOptions,
		};
	}

	/// <summary>
	///     Ignores additional properties of objects in the subject
	///     when <paramref name="ignoreAdditionalProperties" /> is <see langword="true" />
	/// </summary>
	/// <remarks>
	///     Additional elements of arrays are not affected, use <see cref="IgnoringAdditionalArrayElements" /> instead.
	/// </remarks>
	public static JsonOptions IgnoringAdditionalProperties(
		this JsonOptions options,
		bool ignoreAdditionalProperties = true)
		=> new JsonOptions(options)
		{
			IgnoreAdditionalProperties = ignoreAdditionalProperties,
		};

	/// <summary>
	///     Ignores additional elements of arrays in the subject
	///     when <paramref name="ignoreAdditionalArrayElements" /> is <see langword="true" />
	/// </summary>
	/// <remarks>
	///     Additional elements are those after the expected elements or, when the order of the elements is ignored, those
	///     that are not paired with an expected element.
	/// </remarks>
	public static JsonOptions IgnoringAdditionalArrayElements(
		this JsonOptions options,
		bool ignoreAdditionalArrayElements = true)
		=> new JsonOptions(options)
		{
			IgnoreAdditionalArrayElements = ignoreAdditionalArrayElements,
		};

	/// <summary>
	///     Ignores the order of the elements in all compared arrays
	///     when <paramref name="ignoreArrayOrder" /> is <see langword="true" />
	/// </summary>
	/// <remarks>
	///     Each expected element is paired with the first unpaired element of the subject that matches it. This greedy
	///     pairing can fail for overlapping <c>It.Is…</c> expectations, even when another pairing would succeed.
	/// </remarks>
	public static JsonOptions IgnoringArrayOrder(
		this JsonOptions options,
		bool ignoreArrayOrder = true)
		=> new JsonOptions(options)
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
		this JsonOptions options,
		bool treatNullAsAbsent = true)
		=> new JsonOptions(options)
		{
			TreatNullAsAbsent = treatNullAsAbsent,
		};

	/// <summary>
	///     Compares all numbers within the <paramref name="tolerance" />, which can be made relative to the expected
	///     value with <c>Percent()</c>.
	/// </summary>
	/// <remarks>
	///     The tolerance applies to every number, also to integers such as ids or counts. A number is within the tolerance,
	///     when it differs from the expected number by at most the tolerance, or with <c>Percent()</c> by at most the
	///     percentage of the expected number, so that an expected <c>0</c> only matches an equal number. The numbers are
	///     compared as <see langword="double" />, so differences below its precision are not detected, and numbers outside
	///     its range are compared by their text.
	/// </remarks>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="tolerance" /> is negative or NaN.</exception>
	public static JsonNumbersWithinOptions ComparingNumbersWithin(
		this JsonOptions options,
		double tolerance)
		=> new(options, tolerance);

	/// <summary>
	///     Ignores the <paramref name="memberToIgnore" /> in the comparison.
	/// </summary>
	/// <remarks>
	///     The <paramref name="memberToIgnore" /> must cover whole trailing segments of the JSON path, with the same casing:
	///     <c>"id"</c> ignores <c>$.id</c> and <c>$.items[0].id</c>, but not <c>$.parentId</c> or <c>$.ID</c>, while
	///     <c>"$.id"</c> only ignores the property of the root. An ignored value and everything below it is neither compared
	///     nor reported as missing or unexpected.
	///     <para />
	///     With <see cref="IgnoringArrayOrder" />, an index refers to the element of the subject, which is then left out
	///     of the pairing.
	/// </remarks>
	public static JsonOptions IgnoringMember(
		this JsonOptions options,
		string memberToIgnore)
	{
		ThrowHelper.ThrowIfNull(memberToIgnore, nameof(memberToIgnore));
		if (memberToIgnore.Length == 0)
		{
			throw Tracing.WriteException(new ArgumentException(
				"The 'memberToIgnore' cannot be empty.", nameof(memberToIgnore)));
		}

		return new JsonOptions(options)
		{
			MembersToIgnore = [..options.MembersToIgnore, new JsonMemberToIgnore.ByName(memberToIgnore),],
		};
	}

	/// <summary>
	///     Ignores the members whose JSON path, e.g. <c>$.items[0].etag</c>, satisfies the <paramref name="predicate" />
	///     in the comparison.
	/// </summary>
	/// <remarks>
	///     An ignored value and everything below it is neither compared nor reported as missing or unexpected.
	///     <para />
	///     With <see cref="IgnoringArrayOrder" />, an index refers to the element of the subject, which is then left out
	///     of the pairing.
	/// </remarks>
	public static JsonOptions IgnoringMembers(
		this JsonOptions options,
		Func<string, bool> predicate,
		[CallerArgumentExpression("predicate")] string doNotPopulateThisValue = "")
	{
		ThrowHelper.ThrowIfNull(predicate, nameof(predicate));
		return new JsonOptions(options)
		{
			MembersToIgnore =
			[
				..options.MembersToIgnore,
				new JsonMemberToIgnore.ByPredicate(predicate, doNotPopulateThisValue.TrimCommonWhiteSpace()),
			],
		};
	}
}
