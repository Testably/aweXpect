using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using aweXpect.Core;
using aweXpect.Helpers;

namespace aweXpect.Json;

/// <summary>
///     A segment of a path relative to a JSON object, i.e. a property name or a zero-based array index.
/// </summary>
internal sealed class JsonPathSegment
{
	private static readonly char[] Delimiters = ['.', '[', ']',];

	private readonly string _text;

	private JsonPathSegment(string? propertyName, int index)
	{
		PropertyName = propertyName;
		Index = index;
		_text = propertyName is null ? $"[{index}]" : FormatPropertyName(propertyName);
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
	/// <remarks>
	///     A property name is written like in a normalized path of RFC 9535 when it is no plain identifier, e.g.
	///     <c>['@odata.context']</c>, so that a name with <c>.</c>, <c>[</c> or <c>]</c> cannot be read as several
	///     segments.
	/// </remarks>
	public override string ToString() => _text;

	/// <summary>
	///     A segment for the property with the literal <paramref name="name" />, which is not parsed as a path.
	/// </summary>
	public static JsonPathSegment Property(string name) => new(name, 0);

	/// <summary>
	///     A segment for the array element at the zero-based <paramref name="index" />.
	/// </summary>
	public static JsonPathSegment Element(int index) => new(null, index);

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

	private static string FormatPropertyName(string name)
	{
		if (IsShorthandName(name))
		{
			return "." + name;
		}

		StringBuilder sb = new("['");
		foreach (char c in name)
		{
			switch (c)
			{
				case '\'':
					sb.Append("\\'");
					break;
				case '\\':
					sb.Append(@"\\");
					break;
				case < ' ':
					sb.Append("\\u").Append(((int)c).ToString("x4", CultureInfo.InvariantCulture));
					break;
				default:
					sb.Append(c);
					break;
			}
		}

		return sb.Append("']").ToString();
	}

	/// <remarks>
	///     The member-name-shorthand of RFC 9535: an ASCII letter, <c>_</c> or a non-ASCII character, followed by any of
	///     these or ASCII digits.
	/// </remarks>
	private static bool IsShorthandName(string name)
	{
		if (name.Length == 0 || name[0] is >= '0' and <= '9')
		{
			return false;
		}

		foreach (char c in name)
		{
			if (c is not ((>= 'a' and <= 'z') or (>= 'A' and <= 'Z') or (>= '0' and <= '9') or '_' or >= '\u0080'))
			{
				return false;
			}
		}

		return true;
	}

	private static ArgumentException Invalid(string path, string reason)
		=> Tracing.WriteException(new ArgumentException($"The path '{path}' is invalid: {reason}.", nameof(path)));
}
