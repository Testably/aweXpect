using System;
using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;
using aweXpect.Core;
#if !NET8_0_OR_GREATER
using System.Reflection;
using System.Runtime.ExceptionServices;
#endif
#if NET8_0_OR_GREATER
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
#endif

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
	/// <remarks>
	///     It only depends on the type, and every value that is compared by value asks for both of its types.
	/// </remarks>
	private static readonly ConcurrentDictionary<Type, bool> ComparedByContent = new();

	public static bool IsComparedByContent(Type type)
		=> ComparedByContent.GetOrAdd(type, static key
			=> key == typeof(StringBuilder) || typeof(Regex).IsAssignableFrom(key) || IsJson(key));

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

	private const string JsonElementName = "System.Text.Json.JsonElement";

	/// <remarks>
	///     The types are matched by name, so that comparing other values never loads System.Text.Json, which
	///     netstandard2.0 does not have at all. Only System.Text.Json itself can derive from <c>JsonNode</c>, as its
	///     constructor is internal, so any other namespace rules a type out without walking its base types.
	/// </remarks>
	private static bool IsJson(Type type)
	{
		if (type.FullName == JsonElementName)
		{
			return true;
		}

		if (type.Namespace != "System.Text.Json.Nodes")
		{
			return false;
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

#if NET8_0_OR_GREATER
	/// <remarks>
	///     The raw text of a parsed <see cref="JsonElement" /> keeps the formatting of its source, while whitespace
	///     between tokens says nothing about the JSON. A default <see cref="JsonElement" /> has no JSON, so its
	///     undefined value kind is its content.<br />
	///     Not inlined, so that only compiling this method, which runs for JSON values alone, loads System.Text.Json.
	/// </remarks>
	[MethodImpl(MethodImplOptions.NoInlining)]
	private static object GetJson(object value)
		=> value switch
		{
			JsonElement { ValueKind: JsonValueKind.Undefined, } => JsonValueKind.Undefined,
			JsonElement element => RemoveWhitespaceBetweenTokens(element.GetRawText()),
			_ => ((JsonNode)value).ToJsonString(),
		};
#else
	/// <remarks>
	///     The raw text of a parsed <c>JsonElement</c> keeps the formatting of its source, while whitespace between
	///     tokens says nothing about the JSON. A default <c>JsonElement</c> has no JSON, so its undefined value kind is
	///     its content. netstandard2.0 has no System.Text.Json, so it is read by reflection there.
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
