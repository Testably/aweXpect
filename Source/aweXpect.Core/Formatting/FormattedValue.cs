using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace aweXpect.Formatting;

/// <summary>
///     A value nested in the formatted value, which is offered to <see cref="FormattingOptions.FormatValue" />.
/// </summary>
public sealed class FormattedValue
{
	private readonly FormattedValue? _parent;
	private int _index;
	private int[]? _indices;
	private bool _isEntry;
	private object? _key;
	private string? _name;
	private string? _path;
	private Func<object, object?>? _read;
	private Exception? _readException;
	private object? _source;
	private object? _value;

	private FormattedValue(FormattedValueKind kind, FormattedValue? parent, Type type, Type declaringType)
	{
		Kind = kind;
		_parent = parent;
		Type = type;
		DeclaringType = declaringType;
	}

	/// <summary>
	///     The kind of the value.
	/// </summary>
	public FormattedValueKind Kind { get; }

	/// <summary>
	///     The path of the value in the format of an equivalency comparison, so that a <c>MemberToIgnore</c> can be
	///     reused, e.g. <c>Customer.Name</c>, <c>Lines[0].Id</c>, <c>Prices[1.5]</c> or <c>Grid[0,1]</c>.
	/// </summary>
	/// <remarks>
	///     A member of the formatted value itself has no prefix. The items of a collection whose order depends on hash
	///     codes or threads, like a <see cref="System.Collections.Generic.HashSet{T}" />, have the index <c>[*]</c>, as
	///     their index can differ in every run.
	/// </remarks>
	public string Path => _path ??= CreatePath();

	/// <summary>
	///     The name of the value: the name of a member or tuple item (<c>Item1</c>), or the index of a collection item
	///     (<c>[0]</c>) or the key of a dictionary entry (<c>[1.5]</c>) in brackets.
	/// </summary>
	public string Name => _name ??= CreateName();

	/// <summary>
	///     The declared type of a member or tuple item, or the runtime type of a collection item or the value of a
	///     dictionary entry, which is <see cref="object" /> for <see langword="null" />.
	/// </summary>
	public Type Type { get; }

	/// <summary>
	///     The type of the object, tuple or collection that contains the value.
	/// </summary>
	public Type DeclaringType { get; }

	internal Exception? ReadException => _readException;

	/// <summary>
	///     Returns the value.
	/// </summary>
	/// <remarks>
	///     A member is only read when its value is needed, and at most once, so that a member that is left out is not
	///     read unless its value is asked for. An exception of the getter, unwrapped from a
	///     <see cref="TargetInvocationException" /> of reflection, is thrown again on every call, and the value is written
	///     as the usual placeholder.
	/// </remarks>
	public object? GetValue()
	{
		if (_readException is not null)
		{
			ExceptionDispatchInfo.Capture(_readException).Throw();
		}

		if (_read is not null)
		{
			Func<object, object?> read = _read;
			_read = null;
			try
			{
				_value = read(_source!);
			}
			catch (Exception exception)
			{
				_readException = (exception as TargetInvocationException)?.InnerException ?? exception;
				ExceptionDispatchInfo.Capture(_readException).Throw();
			}
		}

		return _value;
	}

	internal static FormattedValue Member(FormattedValue? parent, string name, Type type, Type declaringType,
		object source, Func<object, object?> read)
		=> new(FormattedValueKind.Member, parent, type, declaringType)
		{
			_name = name,
			_source = source,
			_read = read,
		};

	/// <summary>
	///     A segment of the path that is not offered, like the <c>Key</c> or <c>Value</c> of a key-value pair.
	/// </summary>
	internal static FormattedValue Segment(FormattedValue? parent, string name)
		=> new(FormattedValueKind.Member, parent, typeof(object), typeof(object))
		{
			_name = name,
		};

	internal static FormattedValue TupleItem(FormattedValue? parent, int index, Type type, Type declaringType,
		object? value)
		=> new(FormattedValueKind.TupleItem, parent, type, declaringType)
		{
			_index = index,
			_value = value,
		};

	/// <summary>
	///     An item of a collection at the <paramref name="index" />, or at an unknown index when it is negative, or at
	///     the <paramref name="indices" /> of an array of rank greater than one.
	/// </summary>
	internal static FormattedValue Item(FormattedValue? parent, int index, int[]? indices, Type declaringType,
		object? value)
		=> new(FormattedValueKind.CollectionItem, parent, value?.GetType() ?? typeof(object), declaringType)
		{
			_index = index,
			_indices = indices,
			_value = value,
		};

	internal static FormattedValue Entry(FormattedValue? parent, object? key, Type declaringType, object? value)
		=> new(FormattedValueKind.CollectionItem, parent, value?.GetType() ?? typeof(object), declaringType)
		{
			_isEntry = true,
			_key = key,
			_value = value,
		};

	private string CreateName()
	{
		if (Kind == FormattedValueKind.TupleItem)
		{
			return $"Item{_index + 1}";
		}

		if (_isEntry)
		{
			return
				$"[{(_key is IFormattable formattable ? formattable.ToString(null, CultureInfo.InvariantCulture) : _key)}]";
		}

		if (_indices is not null)
		{
			return $"[{string.Join(",", _indices)}]";
		}

		return _index < 0 ? "[*]" : $"[{_index}]";
	}

	private string CreatePath()
	{
		string parentPath = _parent?.Path ?? "";
		if (Kind == FormattedValueKind.CollectionItem)
		{
			return parentPath + Name;
		}

		return parentPath.Length == 0 ? Name : $"{parentPath}.{Name}";
	}
}
