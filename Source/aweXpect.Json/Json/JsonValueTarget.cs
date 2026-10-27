using System.Collections.Generic;
using System.Text;
using aweXpect.Core;
using aweXpect.Json.Options;

namespace aweXpect.Json;

/// <summary>
///     The value that <c>HasProperty</c>, <c>HasValueAt</c> or <c>HasElementAt</c> look up in the subject.
/// </summary>
/// <remarks>
///     It is created for a single expectation, as it learns the JSON path of the subject when its constraint is
///     created.
/// </remarks>
internal sealed class JsonValueTarget
{
	private readonly int _index;
	private readonly string? _name;
	private readonly string? _valueAtPath;
	private string _rootPath = "$";

	private JsonValueTarget(IReadOnlyList<JsonPathSegment> segments, string? name, string? valueAtPath, int index)
	{
		Segments = segments;
		_name = name;
		_valueAtPath = valueAtPath;
		_index = index;
	}

	/// <summary>
	///     The segments from the subject to the value.
	/// </summary>
	public IReadOnlyList<JsonPathSegment> Segments { get; }

	/// <summary>
	///     The JSON path of the value, which continues the JSON path of the subject, when the subject is a value that an
	///     enclosing expectation found.
	/// </summary>
	/// <remarks>
	///     It is only known once the constraint is created, see <see cref="CreateConstraint" />.
	/// </remarks>
	public string Path => GetPath(Segments.Count);

	public static JsonValueTarget Property(string name)
		=> new([JsonPathSegment.Property(name),], name, null, 0);

	public static JsonValueTarget ValueAt(string path)
		=> new(JsonPathSegment.Parse(path), null, path, 0);

	public static JsonValueTarget ElementAt(int index)
		=> new([JsonPathSegment.Element(index),], null, null, index);

	/// <summary>
	///     Creates the constraint, which reads the <paramref name="lookupOptions" />, if any, when it evaluates.
	/// </summary>
	public HasJsonValueConstraint CreateConstraint(string it, ExpectationGrammars grammars,
		IReadOnlyOptionsHolder<JsonLookupOptions>? lookupOptions = null)
	{
		_rootPath = ThatJsonElement.GetPath(it);
		return new HasJsonValueConstraint(it, grammars, this, lookupOptions);
	}

	/// <summary>
	///     The JSON path of the value after the first <paramref name="segmentCount" /> segments.
	/// </summary>
	/// <remarks>
	///     A single segment, like the one of <c>HasProperty</c> or <c>HasElementAt</c>, is concatenated without a
	///     <see cref="StringBuilder" />, as their nested expectations need the path when they are added.
	/// </remarks>
	public string GetPath(int segmentCount)
	{
		if (segmentCount == 1)
		{
			return _rootPath + Segments[0];
		}

		StringBuilder sb = new(_rootPath);
		for (int i = 0; i < segmentCount; i++)
		{
			sb.Append(Segments[i]);
		}

		return sb.ToString();
	}

	public void AppendDescription(StringBuilder stringBuilder)
	{
		if (_name is not null)
		{
			Formatter.Format(stringBuilder.Append("property "), _name);
		}
		else if (_valueAtPath is not null)
		{
			Formatter.Format(stringBuilder.Append("value at "), _valueAtPath);
		}
		else
		{
			stringBuilder.Append("element at index ").Append(_index);
		}
	}
}
