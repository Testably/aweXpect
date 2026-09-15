using System;
using aweXpect.Core;

namespace aweXpect.Results;

/// <summary>
///     The result on a single dictionary value.
/// </summary>
/// <remarks>
///     <seealso cref="AndOrResult{TCollection, TThat}" />
/// </remarks>
public class ContainsKeyResult<TCollection, TThat, TKey, TValue>
	: AndOrResult<TCollection, TThat>
{
	private readonly ExpectationBuilder _expectationBuilder;
	private readonly TKey _key;
	private readonly Func<TCollection, TValue> _memberAccessor;

	internal ContainsKeyResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		TKey key,
		Func<TCollection, TValue> memberAccessor)
		: base(expectationBuilder, returnValue)
	{
		_expectationBuilder = expectationBuilder;
		_key = key;
		_memberAccessor = memberAccessor;
	}

	/// <summary>
	///     Further expectations on the selected value of the dictionary.
	/// </summary>
	public IThat<TValue> WhoseValue
		=> new ThatSubject<TValue>(_expectationBuilder.ForWhich(_memberAccessor, " whose value ",
			$"value [{Formatter.Format(_key)}]"));
}
