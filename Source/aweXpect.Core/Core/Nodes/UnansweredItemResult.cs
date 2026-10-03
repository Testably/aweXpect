using System;
using System.Text;
using aweXpect.Core.Constraints;

namespace aweXpect.Core.Nodes;

/// <summary>
///     The result of a node whose collection expectation was decided by an item that an item expectation did not answer.
/// </summary>
/// <remarks>
///     Like the item result, it fails the expectation and its negation alike.
/// </remarks>
internal sealed class UnansweredItemResult : ConstraintResult
{
	private readonly int? _index;
	private readonly ConstraintResult _inner;
	private readonly object? _item;
	private readonly ConstraintResult _itemResult;
	private readonly object? _value;
	private readonly Type _valueType;

	private UnansweredItemResult(ConstraintResult inner, ConstraintResult itemResult, object? item, int? index,
		object? value, Type valueType)
		: base(inner.FurtherProcessingStrategy)
	{
		_inner = inner;
		_itemResult = itemResult;
		_item = item;
		_index = index;
		_value = value;
		_valueType = valueType;
		Outcome = Outcome.Failure;
	}

	/// <inheritdoc />
	public override Exception? FailureCause => _itemResult.FailureCause;

	/// <summary>
	///     Creates an <see cref="UnansweredItemResult" /> which uses the <paramref name="inner" /> result for the
	///     expectation text.
	/// </summary>
	/// <remarks>
	///     The item is named by its <paramref name="index" /> in the collection, or by its value when the index is
	///     unknown.
	/// </remarks>
	public static UnansweredItemResult Create<T>(ConstraintResult inner, ConstraintResult itemResult, object? item,
		int? index, T value)
		=> new(inner, itemResult, item, index, value, typeof(T));

	/// <inheritdoc />
	public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> _inner.AppendExpectation(stringBuilder, indentation);

	/// <remarks>
	///     The item result refers to the item as "it", so the item is named first.
	/// </remarks>
	public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
	{
		if (_index is not null)
		{
			stringBuilder.Append("for the item at index ").Append(_index.Value);
		}
		else
		{
			stringBuilder.Append("for item ");
			Formatter.Format(stringBuilder, _item);
		}

		stringBuilder.Append(", ");
		_itemResult.AppendResult(stringBuilder, indentation);
	}

	/// <inheritdoc />
	public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
	{
		if (_value is TValue typedValue)
		{
			value = typedValue;
			return true;
		}

		value = default;
		return typeof(TValue).IsAssignableFrom(_valueType);
	}

	/// <inheritdoc />
	/// <remarks>
	///     The constraint describes what it evaluated until the item was not answered.
	/// </remarks>
	public override void AppendContexts(ResultContextCollector contexts)
		=> contexts.Visit(_inner);

	/// <inheritdoc />
	public override ConstraintResult Negate()
	{
		_inner.Negate();
		return this;
	}
}
