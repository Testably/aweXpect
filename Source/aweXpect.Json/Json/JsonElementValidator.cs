using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using aweXpect.Core.Constraints;
using aweXpect.Customization;
using aweXpect.Equivalency;
using aweXpect.Json.Options;

namespace aweXpect.Json;

/// <summary>
///     Validates a <see cref="JsonElement" /> against an <see cref="object" />.
/// </summary>
internal static class JsonElementValidator
{
	public static ValueTask<JsonComparisonResult> Compare(
		JsonElement actualElement,
		JsonElement expectedElement,
		JsonOptions options,
		ExpectationJsonConverter? converter = null)
		=> Compare(new JsonComparisonResult(options.MembersToIgnore), "$", actualElement, expectedElement, options,
			converter);

	/// <remarks>
	///     The <paramref name="basePath" /> itself is never ignored, as it is the target of an explicit expectation.
	/// </remarks>
	public static ValueTask<JsonComparisonResult> Compare(
		string basePath,
		JsonElement actualElement,
		JsonElement expectedElement,
		JsonOptions options,
		ExpectationJsonConverter? converter)
		=> Compare(new JsonComparisonResult(options.MembersToIgnore), basePath, actualElement, expectedElement,
			options, converter);

	private static async ValueTask<JsonComparisonResult> Compare(
		this JsonComparisonResult result,
		string path,
		JsonElement actualElement,
		JsonElement expectedElement,
		JsonOptions options,
		ExpectationJsonConverter? converter)
	{
		if (await result.TryMatchExpectation(path, actualElement, expectedElement, converter))
		{
			return result;
		}

		return actualElement.ValueKind switch
		{
			JsonValueKind.Array => await result.CompareJsonArray(path, actualElement, expectedElement, options,
				converter),
			JsonValueKind.False => result.CompareJsonBoolean(JsonValueKind.False, path, actualElement, expectedElement,
				converter),
			JsonValueKind.True => result.CompareJsonBoolean(JsonValueKind.True, path, actualElement, expectedElement,
				converter),
			JsonValueKind.Null => result.CompareJsonNull(path, actualElement, expectedElement, converter),
			JsonValueKind.Number => result.CompareJsonNumber(path, actualElement, expectedElement,
				options.NumberTolerance, converter),
			JsonValueKind.String => result.CompareJsonString(path, actualElement, expectedElement, converter),
			JsonValueKind.Object => await result.CompareJsonObject(path, actualElement, expectedElement, options,
				converter),
			_ => throw new ArgumentOutOfRangeException($"Unsupported JsonValueKind: {actualElement.ValueKind}"),
		};
	}

	private static async ValueTask<JsonComparisonResult> CompareJsonArray(
		this JsonComparisonResult result,
		string path,
		JsonElement actualElement,
		JsonElement expectedElement,
		JsonOptions options,
		ExpectationJsonConverter? converter)
	{
		if (expectedElement.ValueKind != JsonValueKind.Array)
		{
			result.AddKindDifference(path, actualElement, expectedElement, converter);
			return result;
		}

		if (options.IgnoreArrayOrder)
		{
			return await result.CompareJsonArrayIgnoringOrder(path, actualElement, expectedElement, options,
				converter);
		}

		for (int index = 0; index < expectedElement.GetArrayLength(); index++)
		{
			string memberPath = path + "[" + index + "]";
			if (result.IsIgnored(memberPath))
			{
				continue;
			}

			JsonElement expectedArrayElement = expectedElement[index];
			if (actualElement.GetArrayLength() <= index)
			{
				result.AddError(memberPath, $"had missing {FormatExpected(expectedArrayElement, converter)}");
				continue;
			}

			JsonElement actualArrayElement = actualElement[index];
			await result.Compare(memberPath, actualArrayElement, expectedArrayElement, options, converter);
		}

		if (!options.IgnoreAdditionalArrayElements)
		{
			for (int index = expectedElement.GetArrayLength(); index < actualElement.GetArrayLength(); index++)
			{
				JsonElement actualArrayElement = actualElement[index];
				string memberPath = path + "[" + index + "]";
				if (!result.IsIgnored(memberPath))
				{
					result.AddError(memberPath, $"had unexpected {Format(actualArrayElement)}");
				}
			}
		}

		return result;
	}

	/// <remarks>
	///     A missing element has no index in the subject, so it is reported on the array itself.
	///     When exactly one element remains unpaired on each side, they are compared with each other instead, as their
	///     differences are more precise than reporting them as missing and unexpected.
	///     An ignored element of the subject is left out of the pairing, so it neither pairs with an expected element nor
	///     is reported as unexpected.
	/// </remarks>
	private static async ValueTask<JsonComparisonResult> CompareJsonArrayIgnoringOrder(
		this JsonComparisonResult result,
		string path,
		JsonElement actualElement,
		JsonElement expectedElement,
		JsonOptions options,
		ExpectationJsonConverter? converter)
	{
		JsonElement[] actualElements = actualElement.EnumerateArray().ToArray();
		bool[] isPaired = new bool[actualElements.Length];
		if (result.HasMembersToIgnore)
		{
			for (int index = 0; index < actualElements.Length; index++)
			{
				isPaired[index] = result.IsIgnored(path + "[" + index + "]");
			}
		}

		List<JsonElement> missingElements = new();
		JsonComparisonResult candidate = result.ForCandidate();
		foreach (JsonElement expectedArrayElement in expectedElement.EnumerateArray())
		{
			int index = await candidate.FindUnpairedMatch(path, actualElements, expectedArrayElement, isPaired, options,
				converter);
			if (index >= 0)
			{
				isPaired[index] = true;
			}
			else if (result.StopsAtFirstDifference)
			{
				result.AddUndescribedDifference();
				return result;
			}
			else
			{
				missingElements.Add(expectedArrayElement);
			}
		}

		List<int> unpairedIndices = GetUnpairedIndices(isPaired);
		if (missingElements.Count == 1 && unpairedIndices.Count == 1)
		{
			int index = unpairedIndices[0];
			return await result.Compare(path + "[" + index + "]", actualElements[index], missingElements[0], options,
				converter);
		}

		foreach (JsonElement missingElement in missingElements)
		{
			result.AddError(path, $"had missing {FormatExpected(missingElement, converter)}");
		}

		if (!options.IgnoreAdditionalArrayElements)
		{
			result.AddUnexpectedElements(path, actualElements, unpairedIndices);
		}

		return result;
	}

	private static List<int> GetUnpairedIndices(bool[] isPaired)
	{
		List<int> unpairedIndices = new();
		for (int index = 0; index < isPaired.Length; index++)
		{
			if (!isPaired[index])
			{
				unpairedIndices.Add(index);
			}
		}

		return unpairedIndices;
	}

	private static void AddUnexpectedElements(this JsonComparisonResult result, string path,
		JsonElement[] actualElements, List<int> unexpectedIndices)
	{
		if (result.StopsAtFirstDifference)
		{
			if (unexpectedIndices.Count > 0)
			{
				result.AddUndescribedDifference();
			}

			return;
		}

		foreach (int index in unexpectedIndices)
		{
			result.AddError(path + "[" + index + "]", $"had unexpected {Format(actualElements[index])}");
		}
	}

	/// <summary>
	///     Returns the index of the first unpaired element in the <paramref name="actualElements" /> that matches the
	///     <paramref name="expectedElement" />, or <c>-1</c> if there is none.
	/// </summary>
	/// <remarks>
	///     Each candidate is compared in the separate <paramref name="candidate" /> result, so that the differences of a
	///     candidate that does not match are not reported, but which shares the tracking of the members to ignore. It is
	///     reused for every candidate, so it is cleared first.
	/// </remarks>
	private static async ValueTask<int> FindUnpairedMatch(
		this JsonComparisonResult candidate,
		string path,
		JsonElement[] actualElements,
		JsonElement expectedElement,
		bool[] isPaired,
		JsonOptions options,
		ExpectationJsonConverter? converter)
	{
		for (int index = 0; index < actualElements.Length; index++)
		{
			if (isPaired[index])
			{
				continue;
			}

			candidate.Clear();
			if (!(await candidate.Compare(path + "[" + index + "]", actualElements[index], expectedElement, options,
				    converter)).HasError)
			{
				return index;
			}
		}

		return -1;
	}

	private static JsonComparisonResult CompareJsonBoolean(
		this JsonComparisonResult result,
		JsonValueKind valueKind,
		string path,
		JsonElement actualElement,
		JsonElement expectedElement,
		ExpectationJsonConverter? converter)
	{
		if (expectedElement.ValueKind != valueKind)
		{
			if (expectedElement.ValueKind is JsonValueKind.False or JsonValueKind.True)
			{
				result.AddValueDifference(path, actualElement, expectedElement);
			}
			else
			{
				result.AddKindDifference(path, actualElement, expectedElement, converter);
			}
		}

		return result;
	}

	private static JsonComparisonResult CompareJsonNull(
		this JsonComparisonResult result,
		string path,
		JsonElement actualElement,
		JsonElement expectedElement,
		ExpectationJsonConverter? converter)
	{
		if (expectedElement.ValueKind != JsonValueKind.Null)
		{
			result.AddKindDifference(path, actualElement, expectedElement, converter);
		}

		return result;
	}

	private static JsonComparisonResult CompareJsonNumber(
		this JsonComparisonResult result,
		string path,
		JsonElement actualElement,
		JsonElement expectedElement,
		JsonNumberTolerance? tolerance,
		ExpectationJsonConverter? converter)
	{
		if (expectedElement.ValueKind != JsonValueKind.Number)
		{
			result.AddKindDifference(path, actualElement, expectedElement, converter);
			return result;
		}

		if (!AreEqualNumbers(actualElement, expectedElement, tolerance))
		{
			result.AddValueDifference(path, actualElement, expectedElement, tolerance);
		}

		return result;
	}

	/// <remarks>
	///     Integers are compared exactly, as a <see langword="double" /> loses their precision above 2^53. Fractions are
	///     compared with the precision of a <see langword="double" />, because an expected <see langword="double" /> is
	///     serialized with 17 digits on .NET Framework (e.g. <c>42.1</c> as <c>42.100000000000001</c>).
	///     Rounding is monotonic, so integers whose rounded <see langword="decimal" /> values differ are different.
	///     A <paramref name="tolerance" /> is applied with the precision of a <see langword="double" />, like in aweXpect.
	///     Numbers outside the range of a <see langword="double" /> are compared by their text.
	/// </remarks>
	private static bool AreEqualNumbers(JsonElement actualElement, JsonElement expectedElement,
		JsonNumberTolerance? tolerance)
	{
		if (tolerance is not null &&
		    TryGetFiniteDoubles(actualElement, expectedElement, out double actual, out double expected))
		{
			return tolerance.IsWithinTolerance(actual, expected);
		}

		if (actualElement.TryGetDecimal(out decimal d1) && expectedElement.TryGetDecimal(out decimal d2) &&
		    d1 != d2 && d1 == decimal.Truncate(d1) && d2 == decimal.Truncate(d2))
		{
			return false;
		}

		if (TryGetFiniteDoubles(actualElement, expectedElement, out double n1, out double n2))
		{
			return n1.Equals(n2);
		}

		return actualElement.GetRawText() == expectedElement.GetRawText();
	}

	private static bool TryGetFiniteDoubles(JsonElement actualElement, JsonElement expectedElement,
		out double actual, out double expected)
	{
		if (!actualElement.TryGetDouble(out actual))
		{
			expected = 0;
			return false;
		}

		return expectedElement.TryGetDouble(out expected) &&
		       !double.IsInfinity(actual) && !double.IsInfinity(expected);
	}

	private static async ValueTask<JsonComparisonResult> CompareJsonObject(
		this JsonComparisonResult result,
		string path,
		JsonElement actualElement,
		JsonElement expectedElement,
		JsonOptions options,
		ExpectationJsonConverter? converter)
	{
		if (expectedElement.ValueKind != JsonValueKind.Object)
		{
			result.AddKindDifference(path, actualElement, expectedElement, converter);
			return result;
		}

		foreach (JsonProperty item in expectedElement.EnumerateObject())
		{
			if (result.IsDecided)
			{
				return result;
			}

			string name = item.Name;
			string memberPath = path + "." + name;
			if (result.IsIgnored(memberPath))
			{
				continue;
			}

			if (!actualElement.TryGetProperty(name, out JsonElement property))
			{
				if (IsTreatedAsAbsent(item.Value, options))
				{
					continue;
				}

				result.AddError(memberPath, $"had missing {FormatExpected(item.Value, converter)}");
				continue;
			}

			await result.Compare(memberPath, property, item.Value, options, converter);
		}

		if (!options.IgnoreAdditionalProperties && !result.IsDecided)
		{
			result.AddUnexpectedProperties(path, actualElement, expectedElement, options);
		}

		return result;
	}

	private static void AddUnexpectedProperties(
		this JsonComparisonResult result,
		string path,
		JsonElement actualElement,
		JsonElement expectedElement,
		JsonOptions options)
	{
		foreach (JsonProperty property in actualElement.EnumerateObject())
		{
			string name = property.Name;
			if (IsTreatedAsAbsent(property.Value, options) || expectedElement.TryGetProperty(name, out _))
			{
				continue;
			}

			string memberPath = path + "." + name;
			if (result.HasMemberError(memberPath) || result.IsIgnored(memberPath))
			{
				continue;
			}

			result.AddError(memberPath, $"had unexpected {Format(property.Value)}");
		}
	}

	private static bool IsTreatedAsAbsent(JsonElement value, JsonOptions options)
		=> options.TreatNullAsAbsent && value.ValueKind == JsonValueKind.Null;

	private static JsonComparisonResult CompareJsonString(
		this JsonComparisonResult result,
		string path,
		JsonElement actualElement,
		JsonElement expectedElement,
		ExpectationJsonConverter? converter)
	{
		if (expectedElement.ValueKind != JsonValueKind.String)
		{
			result.AddKindDifference(path, actualElement, expectedElement, converter);
			return result;
		}

		if (!actualElement.ValueEquals(expectedElement.GetString()))
		{
			result.AddValueDifference(path, actualElement, expectedElement);
		}

		return result;
	}

	internal static string Format(JsonElement jsonElement, bool includeType = false)
	{
		string GetKindName(JsonValueKind kind)
			=> kind switch
			{
				JsonValueKind.False => "boolean",
				JsonValueKind.True => "boolean",
				JsonValueKind.Number => "number",
				JsonValueKind.String => "string",
				JsonValueKind.Array => "array",
				JsonValueKind.Object => "object",
				_ => "",
			};

		string value = jsonElement.ValueKind switch
		{
			JsonValueKind.Null => ValueFormatter.NullString,
			JsonValueKind.String => Formatter.Format(jsonElement.GetString()),
			JsonValueKind.False or JsonValueKind.True => Formatter.Format(jsonElement.GetBoolean()),
			_ => Formatter.Format(jsonElement),
		};
		return includeType && jsonElement.ValueKind != JsonValueKind.Null
			? $"{GetKindName(jsonElement.ValueKind)} {value}"
			: value;
	}

	private static void AddKindDifference(this JsonComparisonResult result, string path, JsonElement actualElement,
		JsonElement expectedElement, ExpectationJsonConverter? converter)
	{
		if (result.StopsAtFirstDifference)
		{
			result.AddUndescribedDifference();
			return;
		}

		result.AddError(path,
			$"was {Format(actualElement, true)} instead of {FormatExpected(expectedElement, converter)}");
	}

	private static void AddValueDifference(this JsonComparisonResult result, string path, JsonElement actualElement,
		JsonElement expectedElement, JsonNumberTolerance? tolerance = null)
	{
		if (result.StopsAtFirstDifference)
		{
			result.AddUndescribedDifference();
			return;
		}

		result.AddError(path, $"was {Format(actualElement)} instead of {Format(expectedElement)}{tolerance}");
	}

	private static string FormatExpected(JsonElement expectedElement, ExpectationJsonConverter? converter)
		=> converter?.DescribeExpectations(Format(expectedElement)) ?? Format(expectedElement);

	private static async Task<bool> TryMatchExpectation(
		this JsonComparisonResult result,
		string path,
		JsonElement actualElement,
		JsonElement expectedElement,
		ExpectationJsonConverter? converter)
	{
		static object? GetValue(JsonElement jsonElement)
			=> jsonElement.ValueKind switch
			{
				JsonValueKind.True => true,
				JsonValueKind.False => false,
				JsonValueKind.Null => null,
				JsonValueKind.String => jsonElement.GetString(),
				JsonValueKind.Number when jsonElement.TryGetInt32(out int value) => value,
				JsonValueKind.Number when jsonElement.TryGetSByte(out sbyte value) => value,
				JsonValueKind.Number when jsonElement.TryGetByte(out byte value) => value,
				JsonValueKind.Number when jsonElement.TryGetInt16(out short value) => value,
				JsonValueKind.Number when jsonElement.TryGetUInt16(out ushort value) => value,
				JsonValueKind.Number when jsonElement.TryGetUInt32(out uint value) => value,
				JsonValueKind.Number when jsonElement.TryGetInt64(out long value) => value,
				JsonValueKind.Number when jsonElement.TryGetUInt64(out ulong value) => value,
				JsonValueKind.Number when jsonElement.TryGetDouble(out double value) => value,
				JsonValueKind.Array => jsonElement.EnumerateArray().Select(GetValue).ToArray(),
				_ => throw new NotSupportedException()
			};

		static object? GetNumber(JsonElement jsonElement, Type expectedType)
			=> jsonElement.ValueKind != JsonValueKind.Number
				? null
				: Type.GetTypeCode(Nullable.GetUnderlyingType(expectedType) ?? expectedType) switch
				{
					TypeCode.SByte when jsonElement.TryGetSByte(out sbyte value) => value,
					TypeCode.Byte when jsonElement.TryGetByte(out byte value) => value,
					TypeCode.Int16 when jsonElement.TryGetInt16(out short value) => value,
					TypeCode.UInt16 when jsonElement.TryGetUInt16(out ushort value) => value,
					TypeCode.Int32 when jsonElement.TryGetInt32(out int value) => value,
					TypeCode.UInt32 when jsonElement.TryGetUInt32(out uint value) => value,
					TypeCode.Int64 when jsonElement.TryGetInt64(out long value) => value,
					TypeCode.UInt64 when jsonElement.TryGetUInt64(out ulong value) => value,
					TypeCode.Single when jsonElement.TryGetSingle(out float value) => value,
					TypeCode.Double when jsonElement.TryGetDouble(out double value) => value,
					TypeCode.Decimal when jsonElement.TryGetDecimal(out decimal value) => value,
					_ => null,
				};

		if (converter?.TryGetExpectation(expectedElement, out EquivalencyExpectationBuilder? expectationBuilder) ==
		    true)
		{
			object? actualValue = GetNumber(actualElement, expectationBuilder.ExpectedType) ?? GetValue(actualElement);
			ConstraintResult constraintResult = await expectationBuilder.IsMetBy(actualValue,
				converter.Context, converter.CancellationToken);
			if (constraintResult.Outcome == Outcome.Success)
			{
				return true;
			}

			if (constraintResult.Outcome is not (Outcome.Failure or Outcome.FailureBothWays))
			{
				converter.CancellationToken.ThrowIfCancellationRequested();
			}

			StringBuilder failureBuilder = new();
			constraintResult.AppendResult(failureBuilder);
			result.AddError(path, failureBuilder.ToString().Trim());
			return true;
		}

		return false;
	}


	internal class JsonComparisonResult
	{
		/// <remarks>
		///     A list instead of a dictionary, because the missing elements of an array that ignores the order of its elements
		///     are all reported on the path of the array.
		/// </remarks>
		private readonly List<KeyValuePair<string, string>> _errors = new();

		private readonly MembersToIgnoreTracker _membersToIgnore;

		private bool _hasUndescribedDifference;

		public JsonComparisonResult(JsonMemberToIgnore[] membersToIgnore)
			: this(new MembersToIgnoreTracker(membersToIgnore), false)
		{
		}

		private JsonComparisonResult(MembersToIgnoreTracker membersToIgnore, bool stopsAtFirstDifference)
		{
			_membersToIgnore = membersToIgnore;
			StopsAtFirstDifference = stopsAtFirstDifference;
		}

		public bool HasError => _hasUndescribedDifference || _errors.Count > 0;

		public bool HasMembersToIgnore => _membersToIgnore.HasMembersToIgnore;

		/// <summary>
		///     Whether only the existence of a difference matters, so that a difference is not described.
		/// </summary>
		public bool StopsAtFirstDifference { get; }

		/// <summary>
		///     Whether the outcome is known and the remaining values need not be compared.
		/// </summary>
		public bool IsDecided => StopsAtFirstDifference && HasError;

		/// <summary>
		///     Creates a separate result for a candidate of a pairing, which shares the tracking of the members to ignore.
		/// </summary>
		/// <remarks>
		///     The candidate only needs to know whether it differs, so it stops at the first difference without describing
		///     it, unless members are ignored, whose tracking has to see every path.
		/// </remarks>
		public JsonComparisonResult ForCandidate() => new(_membersToIgnore, !HasMembersToIgnore);

		public bool IsIgnored(string memberPath)
			=> _membersToIgnore.IsIgnored(memberPath);

		/// <summary>
		///     Records a difference of a comparison that only needs to know whether there is one.
		/// </summary>
		public void AddUndescribedDifference()
			=> _hasUndescribedDifference = true;

		/// <summary>
		///     Returns each member to ignore by name that matched no compared path, with the path it most likely meant, or
		///     <see langword="null" /> when the comparison succeeded or every one matched.
		/// </summary>
		public Dictionary<string, string?>? GetUnmatchedMembersToIgnore()
			=> HasError ? _membersToIgnore.GetUnmatchedMembersToIgnore() : null;

		/// <summary>
		///     Removes the differences, so that the result can be reused for the next candidate of a pairing.
		/// </summary>
		public void Clear()
		{
			_errors.Clear();
			_hasUndescribedDifference = false;
		}

		public void AddError(string memberPath, string error)
			=> _errors.Add(new KeyValuePair<string, string>(memberPath, error));

		public bool HasMemberError(string memberPath)
		{
			for (int i = 0; i < _errors.Count; i++)
			{
				if (_errors[i].Key == memberPath)
				{
					return true;
				}
			}

			return false;
		}

		public override string ToString()
		{
			StringBuilder sb = new();
			if (_errors.Any())
			{
				bool hasMoreThanOneDifference = _errors.Count > 1;
				int count = 0;
				int maximumNumberOfCollectionItems =
					Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
				foreach (KeyValuePair<string, string> differentMember in _errors)
				{
					if (sb.Length > 0)
					{
						sb.Append(" and");
					}

					if (count++ >= maximumNumberOfCollectionItems)
					{
						sb.AppendLine().Append("  (… and ")
							.Append(_errors.Count - maximumNumberOfCollectionItems)
							.Append(" more)");
						return sb.ToString();
					}

					if (hasMoreThanOneDifference)
					{
						sb.AppendLine().Append(' ');
					}

					sb.Append(' ').Append(differentMember.Key).Append(' ').Append(differentMember.Value);
				}
			}

			return sb.ToString();
		}
	}
}
