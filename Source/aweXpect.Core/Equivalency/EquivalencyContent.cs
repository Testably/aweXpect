using System;
#if !NET8_0_OR_GREATER
using System.Reflection;
using System.Runtime.ExceptionServices;
#endif
using System.Text;
#if NET8_0_OR_GREATER
using System.Text.Json;
using System.Text.Json.Nodes;
#endif
using System.Text.RegularExpressions;
using aweXpect.Core;

namespace aweXpect.Equivalency;

/// <summary>
///     The types that are compared by value, but whose content stands for them instead of the instance.
/// </summary>
/// <remarks>
///     Their public members do not hold their state, and their <see cref="object.Equals(object)" /> compares the
///     reference, so neither the members nor <see cref="object.Equals(object)" /> could tell two of them apart. A
///     <see cref="StringBuilder" /> stands for its text, a JSON value for its JSON and a <see cref="Regex" /> for its
///     pattern and options. The content is a <see langword="string" /> or a tuple that is compared by value and named
///     in the failure message, so a <see cref="StringBuilder" /> or a JSON value also matches a
///     <see langword="string" /> with its text.
/// </remarks>
internal static class EquivalencyContent
{
	public static bool IsComparedByContent(Type type)
		=> type == typeof(StringBuilder) || typeof(Regex).IsAssignableFrom(type) || IsJson(type);

	/// <summary>
	///     Returns the content of the <paramref name="value" />, or the <paramref name="value" /> itself when it is not
	///     compared by content.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="thrower">Who threw in the failure message when reading the JSON of a disposed document fails.</param>
	public static object GetContent(object value, string? thrower)
		=> value switch
		{
			StringBuilder stringBuilder => stringBuilder.ToString(),
			Regex regex => (regex.ToString(), regex.Options),
			_ => IsJson(value.GetType()) ? UserCode.Invoke(GetJson, value, thrower) : value,
		};

#if NET8_0_OR_GREATER
	private static bool IsJson(Type type)
		=> type == typeof(JsonElement) || typeof(JsonNode).IsAssignableFrom(type);

	/// <remarks>
	///     The raw text of a parsed <see cref="JsonElement" /> keeps the formatting of its source, while whitespace
	///     between tokens says nothing about the JSON. A default <see cref="JsonElement" /> has no JSON, so its
	///     undefined value kind is its content.
	/// </remarks>
	private static object GetJson(object value)
		=> value switch
		{
			JsonElement { ValueKind: JsonValueKind.Undefined, } => JsonValueKind.Undefined,
			JsonElement element => RemoveWhitespaceBetweenTokens(element.GetRawText()),
			_ => ((JsonNode)value).ToJsonString(),
		};
#else
	private const string JsonElementName = "System.Text.Json.JsonElement";

	/// <remarks>
	///     netstandard2.0 has no System.Text.Json, but is served to runtimes and applications that have it, so its
	///     types are matched by name and read by reflection there.
	/// </remarks>
	private static bool IsJson(Type type)
	{
		if (type.FullName == JsonElementName)
		{
			return true;
		}

		for (Type? current = type; current is not null; current = current.BaseType)
		{
			if (current.FullName == "System.Text.Json.Nodes.JsonNode")
			{
				return true;
			}
		}

		return false;
	}

	/// <remarks>
	///     The raw text of a parsed <c>JsonElement</c> keeps the formatting of its source, while whitespace between
	///     tokens says nothing about the JSON. A default <c>JsonElement</c> has no JSON, so its undefined value kind is
	///     its content.
	/// </remarks>
	private static object GetJson(object value)
	{
		Type type = value.GetType();
		if (type.FullName != JsonElementName)
		{
			return Invoke(type.GetMethod("ToJsonString")!, value, [null,]);
		}

		object valueKind = Invoke(type.GetProperty("ValueKind")!.GetGetMethod()!, value, null);
		return Convert.ToInt32(valueKind) == 0
			? valueKind
			: RemoveWhitespaceBetweenTokens((string)Invoke(type.GetMethod("GetRawText", Type.EmptyTypes)!, value,
				null));
	}

	/// <remarks>
	///     Reflection wraps an exception of the invoked method, which is unwrapped so that the failure names it.
	/// </remarks>
	private static object Invoke(MethodInfo method, object value, object?[]? arguments)
	{
		try
		{
			return method.Invoke(value, arguments)!;
		}
		catch (TargetInvocationException exception) when (exception.InnerException is not null)
		{
			ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
			throw;
		}
	}
#endif

	private static string RemoveWhitespaceBetweenTokens(string json)
	{
		StringBuilder builder = new(json.Length);
		bool isInString = false;
		bool isEscaped = false;
		foreach (char c in json)
		{
			if (isInString)
			{
				isInString = isEscaped || c != '"';
				isEscaped = !isEscaped && c == '\\';
			}
			else if (c is ' ' or '\t' or '\r' or '\n')
			{
				continue;
			}
			else
			{
				isInString = c == '"';
			}

			builder.Append(c);
		}

		return builder.ToString();
	}
}
