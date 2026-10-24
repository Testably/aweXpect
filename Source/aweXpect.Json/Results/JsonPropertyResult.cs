using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect.Json.Results;

/// <summary>
///     The result for verifying that a JSON value has or does not have a property or a value at a path.
/// </summary>
/// <remarks>
///     <seealso cref="AndOrResult{TType,TThat}" />
/// </remarks>
public class JsonPropertyResult<TType, TThat> : AndOrResult<TType, TThat>
{
	private readonly JsonValueTarget _target;

	internal JsonPropertyResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		JsonValueTarget target)
		: base(expectationBuilder, returnValue)
	{
		_target = target;
	}

	/// <summary>
	///     Treats a property with the value <see langword="null" /> as if it was absent, also on the way to a value at a
	///     path.
	/// </summary>
	public AndOrResult<TType, TThat> TreatingNullAsAbsent(bool treatNullAsAbsent = true)
	{
		_target.TreatNullAsAbsent = treatNullAsAbsent;
		return this;
	}
}
