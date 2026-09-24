using aweXpect.Core;

namespace aweXpect.Results;

/// <summary>
///     The result of an expectation with no underlying value.
///     <para />
///     Allows combining multiple expectations with <see cref="And" />.
/// </summary>
public class AndResult<TThat>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue)
	: ExpectationResult(expectationBuilder)
{
	private readonly ExpectationBuilder _expectationBuilder = expectationBuilder;

	/// <summary>
	///     Combines the previous expectation with the next one; both must be met.
	/// </summary>
	public TThat And
	{
		get
		{
			_expectationBuilder.And();
			return returnValue;
		}
	}
}

/// <summary>
///     The result of an expectation with an underlying value of type <typeparamref name="TType" />.
///     <para />
///     Allows combining multiple expectations with <see cref="AndResult{TResult,TValue,TSelf}.And" />.
/// </summary>
public class AndResult<TType, TThat>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue)
	: AndResult<TType, TThat, AndResult<TType, TThat>>(
		expectationBuilder,
		returnValue);

/// <summary>
///     The result of an expectation with an underlying value of type <typeparamref name="TType" />.
///     <para />
///     Allows combining multiple expectations with <see cref="And" />.
/// </summary>
public class AndResult<TType, TThat, TSelf>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue)
	: ExpectationResult<TType, TSelf>(expectationBuilder)
	where TSelf : AndResult<TType, TThat, TSelf>
{
	private readonly ExpectationBuilder _expectationBuilder = expectationBuilder;

	/// <summary>
	///     Combines the previous expectation with the next one; both must be met.
	/// </summary>
	public TThat And
	{
		get
		{
			_expectationBuilder.And();
			return returnValue;
		}
	}
}
