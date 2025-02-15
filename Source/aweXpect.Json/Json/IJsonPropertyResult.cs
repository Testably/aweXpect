#if NET8_0_OR_GREATER
using System;
using System.Runtime.CompilerServices;

namespace aweXpect.Json;

/// <summary>
///     The result for an expectation on a JSON property.
/// </summary>
public interface IJsonPropertyResult<out TReturn>
{
	/// <summary>
	///     Add an expectation that the property value matches the given <paramref name="expected" /> value.
	/// </summary>
	public TReturn Matching(object? expected,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "");

	/// <summary>
	///     Add an expectation that the property value is an array.
	/// </summary>
	public TReturn AnArray();

	/// <summary>
	///     Add an expectation that the property value is an array which satisfies the <paramref name="expectation" />.
	/// </summary>
	public TReturn AnArray(Action<IJsonArrayResult> expectation);

	/// <summary>
	///     Add an expectation that the property value is an object.
	/// </summary>
	public TReturn AnObject();

	/// <summary>
	///     Add an expectation that the property value is an object which satisfies the <paramref name="expectation" />.
	/// </summary>
	public TReturn AnObject(Action<IJsonObjectResult> expectation);
}
#endif
