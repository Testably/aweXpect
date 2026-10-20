using System;
using System.Collections.Generic;
using System.Globalization;
using aweXpect.Core;
using aweXpect.Helpers;

namespace aweXpect.Json;

/// <summary>
///     A segment of a path relative to a JSON object, i.e. a property name or a zero-based array index.
/// </summary>
internal sealed class JsonPathSegment
{
	private static readonly char[] Delimiters = ['.', '[', ']',];

	private JsonPathSegment(string? propertyName, int index)
	{
		PropertyName = propertyName;
		Index = index;
	}

	/// <summary>
	///     The property name, or <see langword="null" /> when the segment is an array index.
	/// </summary>
	public string? PropertyName { get; }

	/// <summary>
	///     The zero-based array index, when the segment is no property name.
	/// </summary>
	public int Index { get; }

	/// <inheritdoc cref="object.ToString()" />
	public override string ToString()
		=> PropertyName is null ? $"[{Index}]" : $".{PropertyName}";

	/// <summary>
	///     Parses the <paramref name="path" />, which starts with a property name, followed by any number of
	///     <c>.name</c> or <c>[index]</c> segments.
	/// </summary>
	public static IReadOnlyList<JsonPathSegment> Parse(string path)
	{
		ThrowHelper.ThrowIfNull(path, nameof(path));
		if (path.Length == 0)
		{
			throw Tracing.WriteException(new ArgumentException("The 'path' cannot be empty.", nameof(path)));
		}

		if (path == "$" || path.StartsWith("$.", StringComparison.Ordinal) ||
		    path.StartsWith("$[", StringComparison.Ordinal))
		{
			throw Invalid(path, "it is relative to the current object and must not start with '$'");
		}

		if (path.Contains(".."))
		{
			throw Invalid(path, "recursive descent ('..') is not supported");
		}

		int position = 0;
		List<JsonPathSegment> segments = [ReadPropertyName(path, ref position),];
		while (position < path.Length)
		{
			segments.Add(ReadSegment(path, ref position));
		}

		return segments;
	}

	private static JsonPathSegment ReadSegment(string path, ref int position)
	{
		char delimiter = path[position];
		position++;
		return delimiter switch
		{
			'.' => ReadPropertyName(path, ref position),
			'[' => ReadIndex(path, ref position),
			_ => throw Invalid(path, $"expected '.' or '[' at position {position - 1}"),
		};
	}

	private static JsonPathSegment ReadPropertyName(string path, ref int position)
	{
		int end = path.IndexOfAny(Delimiters, position);
		if (end < 0)
		{
			end = path.Length;
		}

		string propertyName = path.Substring(position, end - position);
		if (propertyName.Length == 0)
		{
			throw Invalid(path, $"expected a property name at position {position}");
		}

		if (propertyName == "*")
		{
			throw Invalid(path, "wildcards ('*') are not supported");
		}

		position = end;
		return new JsonPathSegment(propertyName, 0);
	}

	private static JsonPathSegment ReadIndex(string path, ref int position)
	{
		int end = path.IndexOf(']', position);
		if (end < 0)
		{
			throw Invalid(path, $"the '[' at position {position - 1} is not closed");
		}

		string index = path.Substring(position, end - position);
		if (index == "*")
		{
			throw Invalid(path, "wildcards ('*') are not supported");
		}

		if (index.Length > 0 && index[0] == '?')
		{
			throw Invalid(path, "filters are not supported");
		}

		if (!int.TryParse(index, NumberStyles.None, CultureInfo.InvariantCulture, out int value))
		{
			throw Invalid(path, $"expected a non-negative integer index at position {position}");
		}

		position = end + 1;
		return new JsonPathSegment(null, value);
	}

	private static ArgumentException Invalid(string path, string reason)
		=> Tracing.WriteException(new ArgumentException($"The path '{path}' is invalid: {reason}.", nameof(path)));
}
