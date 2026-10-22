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
	///     Add an expectation that the property value is an array which satisfies the <paramref name="expectations" />.
	/// </summary>
	public TReturn AnArray(Action<IJsonArrayResult> expectations);

	/// <summary>
	///     Add an expectation that the property value is an object.
	/// </summary>
	public TReturn AnObject();

	/// <summary>
	///     Add an expectation that the property value is an object which satisfies the <paramref name="expectations" />.
	/// </summary>
	public TReturn AnObject(Action<IJsonObjectResult> expectations);

	/// <summary>
	///     Add an expectation that the property value is a string.
	/// </summary>
	public TReturn AString();

	/// <summary>
	///     Add an expectation that the property value is a number.
	/// </summary>
	public TReturn ANumber();

	/// <summary>
	///     Add an expectation that the property value is a boolean, i.e. <see langword="true" /> or
	///     <see langword="false" />.
	/// </summary>
	public TReturn ABoolean();

	/// <summary>
	///     Add an expectation that the property value is <see langword="null" />.
	/// </summary>
	public TReturn Null();
}
