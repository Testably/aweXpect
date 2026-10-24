using System.Collections.Generic;
using System.Text;
using aweXpect.Core;

namespace aweXpect.Json;

/// <summary>
///     The value that <c>HasProperty</c>, <c>HasValueAt</c> or <c>HasElementAt</c> look up in the subject.
/// </summary>
/// <remarks>
///     It is created for a single expectation and holds its options, so that a result can still change them.
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
	///     Flag indicating, if a property with the value <see langword="null" /> should be treated as if it was absent.
	/// </summary>
	public bool TreatNullAsAbsent { get; set; }

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

	public HasJsonValueConstraint CreateConstraint(string it, ExpectationGrammars grammars)
	{
		_rootPath = ThatJsonElement.GetPath(it);
		return new HasJsonValueConstraint(it, grammars, this);
	}

	/// <summary>
	///     The JSON path of the value after the first <paramref name="segmentCount" /> segments.
	/// </summary>
	public string GetPath(int segmentCount)
	{
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
