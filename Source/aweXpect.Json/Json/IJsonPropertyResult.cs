using System;
using System.Runtime.CompilerServices;

namespace aweXpect.Json;

/// <summary>
///     The result for an expectation on a JSON property.
/// </summary>
public interface IJsonPropertyResult<out TResult>
{
	/// <summary>
	///     Add an expectation that the property value matches the given <paramref name="expected" /> value.
	/// </summary>
	public TResult Matching(object? expected,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "");

	/// <summary>
	///     Add an expectation that the property value is an array.
	/// </summary>
	public TResult AnArray();

	/// <summary>
	///     Add an expectation that the property value is an array which satisfies the <paramref name="expectations" />.
	/// </summary>
	public TResult AnArray(Action<IJsonArrayResult> expectations);

	/// <summary>
	///     Add an expectation that the property value is an object.
	/// </summary>
	public TResult AnObject();

	/// <summary>
	///     Add an expectation that the property value is an object which satisfies the <paramref name="expectations" />.
	/// </summary>
	public TResult AnObject(Action<IJsonObjectResult> expectations);

	/// <summary>
	///     Add an expectation that the property value is a string.
	/// </summary>
	public TResult AString();

	/// <summary>
	///     Add an expectation that the property value is a number.
	/// </summary>
	public TResult ANumber();

	/// <summary>
	///     Add an expectation that the property value is a boolean, i.e. <see langword="true" /> or
	///     <see langword="false" />.
	/// </summary>
	public TResult ABoolean();

	/// <summary>
	///     Add an expectation that the property value is <see langword="null" />.
	/// </summary>
	public TResult Null();
}
