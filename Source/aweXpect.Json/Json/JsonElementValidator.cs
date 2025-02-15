#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using aweXpect.Customization;

namespace aweXpect.Json;

/// <summary>
///     Validates a <see cref="JsonElement" /> against an <see cref="object" />.
/// </summary>
internal static class JsonElementValidator
{
	public static JsonComparisonResult Compare(
		JsonElement actualElement,
		JsonElement expectedElement,
		JsonOptions options)
		=> Compare(new JsonComparisonResult(), "$", actualElement, expectedElement, options);

	public static JsonComparisonResult Compare(
		string basePath,
		JsonElement actualElement,
		JsonElement expectedElement,
		JsonOptions options)
		=> Compare(new JsonComparisonResult(), basePath, actualElement, expectedElement, options);

	private static JsonComparisonResult Compare(
		this JsonComparisonResult result,
		string path,
		JsonElement actualElement,
		JsonElement expectedElement,
		JsonOptions options)
		=> actualElement.ValueKind switch
		{
			JsonValueKind.Array => result.CompareJsonArray(path, actualElement, expectedElement, options),
			JsonValueKind.False => result.CompareJsonBoolean(JsonValueKind.False, path, actualElement, expectedElement),
			JsonValueKind.True => result.CompareJsonBoolean(JsonValueKind.True, path, actualElement, expectedElement),
			JsonValueKind.Null => result.CompareJsonNull(path, actualElement, expectedElement),
			JsonValueKind.Number => result.CompareJsonNumber(path, actualElement, expectedElement),
			JsonValueKind.String => result.CompareJsonString(path, actualElement, expectedElement),
			JsonValueKind.Object => result.CompareJsonObject(path, actualElement, expectedElement, options),
			_ => throw new ArgumentOutOfRangeException($"Unsupported JsonValueKind: {actualElement.ValueKind}"),
		};

	private static JsonComparisonResult CompareJsonArray(
		this JsonComparisonResult result,
		string path,
		JsonElement actualElement,
		JsonElement expectedElement,
		JsonOptions options)
	{
		if (expectedElement.ValueKind != JsonValueKind.Array)
		{
			result.AddError(path, $"was {Format(actualElement, true)} instead of {Format(expectedElement)}");
			return result;
		}

		for (int index = 0; index < expectedElement.GetArrayLength(); index++)
		{
			string memberPath = path + "[" + index + "]";
			JsonElement expectedArrayElement = expectedElement[index];
			if (actualElement.GetArrayLength() <= index)
			{
				result.AddError(memberPath, $"had missing {Format(expectedArrayElement)}");
				continue;
			}

			JsonElement actualArrayElement = actualElement[index];
			result.Compare(memberPath, actualArrayElement, expectedArrayElement, options);
		}

		if (!options.IgnoreAdditionalProperties)
		{
			for (int index = expectedElement.GetArrayLength(); index < actualElement.GetArrayLength(); index++)
			{
				JsonElement actualArrayElement = actualElement[index];
				string memberPath = path + "[" + index + "]";
				result.AddError(memberPath, $"had unexpected {Format(actualArrayElement)}");
			}
		}

		return result;
	}

	private static JsonComparisonResult CompareJsonBoolean(
		this JsonComparisonResult result,
		JsonValueKind valueKind,
		string path,
		JsonElement actualElement,
		JsonElement expectedElement)
	{
		if (expectedElement.ValueKind != valueKind)
		{
			result.AddError(path, expectedElement.ValueKind is JsonValueKind.False or JsonValueKind.True
				? $"was {Format(actualElement)} instead of {Format(expectedElement)}"
				: $"was {Format(actualElement, true)} instead of {Format(expectedElement)}");
		}

		return result;
	}

	private static JsonComparisonResult CompareJsonNull(
		this JsonComparisonResult result,
		string path,
		JsonElement actualElement,
		JsonElement expectedElement)
	{
		if (expectedElement.ValueKind != JsonValueKind.Null)
		{
			result.AddError(path, $"was {Format(actualElement, true)} instead of {Format(expectedElement)}");
		}

		return result;
	}

	private static JsonComparisonResult CompareJsonNumber(
		this JsonComparisonResult result,
		string path,
		JsonElement actualElement,
		JsonElement expectedElement)
	{
		if (expectedElement.ValueKind != JsonValueKind.Number)
		{
			result.AddError(path, $"was {Format(actualElement, true)} instead of {Format(expectedElement)}");
			return result;
		}

		if (actualElement.TryGetInt32(out int v1) && expectedElement.TryGetInt32(out int v2))
		{
			if (v1 == v2)
			{
				return result;
			}

			result.AddError(path, $"was {Format(actualElement)} instead of {Format(expectedElement)}");
			return result;
		}

		if (actualElement.TryGetDouble(out double n1) && expectedElement.TryGetDouble(out double n2))
		{
			if (n1.Equals(n2))
			{
				return result;
			}

			result.AddError(path, $"was {Format(actualElement)} instead of {Format(expectedElement)}");
			return result;
		}

		return result;
	}

	private static JsonComparisonResult CompareJsonObject(
		this JsonComparisonResult result,
		string path,
		JsonElement actualElement,
		JsonElement expectedElement,
		JsonOptions options)
	{
		if (expectedElement.ValueKind != JsonValueKind.Object)
		{
			result.AddError(path, $"was {Format(actualElement, true)} instead of {Format(expectedElement)}");
			return result;
		}

		foreach (JsonProperty item in expectedElement.EnumerateObject())
		{
			string memberPath = path + "." + item.Name;
			if (!actualElement.TryGetProperty(item.Name, out JsonElement property))
			{
				result.AddError(memberPath, "was missing");
				continue;
			}

			result.Compare(memberPath, property, item.Value, options);
		}

		if (!options.IgnoreAdditionalProperties)
		{
			foreach (JsonProperty property in actualElement.EnumerateObject())
			{
				string memberPath = path + "." + property.Name;
				if (result.HasMemberError(memberPath) ||
				    expectedElement.TryGetProperty(property.Name, out _))
				{
					continue;
				}

				result.AddError(memberPath, $"had unexpected {Format(property.Value)}");
			}
		}

		return result;
	}

	private static JsonComparisonResult CompareJsonString(
		this JsonComparisonResult result,
		string path,
		JsonElement actualElement,
		JsonElement expectedElement)
	{
		if (expectedElement.ValueKind != JsonValueKind.String)
		{
			result.AddError(path, $"was {Format(actualElement, true)} instead of {Format(expectedElement)}");
			return result;
		}

		string? value1 = actualElement.GetString();
		string? value2 = expectedElement.GetString();
		if (value1 != value2)
		{
			result.AddError(path, $"was {Format(actualElement)} instead of {Format(expectedElement)}");
		}

		return result;
	}

	private static string Format(JsonElement jsonElement, bool includeType = false)
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

		if (jsonElement.ValueKind == JsonValueKind.Null)
		{
			return "Null";
		}

		if (jsonElement.ValueKind == JsonValueKind.String)
		{
			return includeType
				? $"string \"{jsonElement}\""
				: $"\"{jsonElement}\"";
		}

		return includeType
			? $"{GetKindName(jsonElement.ValueKind)} {jsonElement}"
			: jsonElement.ToString();
	}


	internal class JsonComparisonResult
	{
		private readonly Dictionary<string, string> _errors = new();

		public bool HasError => _errors.Any();

		public void AddError(string memberPath, string error)
			=> _errors.Add(memberPath, error);

		public bool HasMemberError(string memberPath)
			=> _errors.ContainsKey(memberPath);

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
					if (count++ >= maximumNumberOfCollectionItems)
					{
						sb.AppendLine().Append("   … (")
							.Append(_errors.Count - maximumNumberOfCollectionItems)
							.Append(" more)");
						return sb.ToString();
					}

					if (hasMoreThanOneDifference)
					{
						sb.AppendLine().Append(' ');
					}

					sb.Append(' ').Append(differentMember.Key).Append(' ').Append(differentMember.Value).Append(" and");
				}
			}

			sb.Length -= 4;

			return sb.ToString();
		}
	}
}

#endif
