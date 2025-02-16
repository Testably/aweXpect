using System;
using System.Text.Json;

namespace aweXpect.Json;

/// <summary>
///     The result for an expectation on a JSON <see cref="JsonValueKind.Array" />.
/// </summary>
public interface IJsonArrayResult
{
	/// <summary>
	///     Combine multiple JSON expectations on this <see cref="JsonValueKind.Array" />.
	/// </summary>
	public IJsonArrayResult And { get; }

	/// <summary>
	///     Add an expectation on the number of elements in the array.
	/// </summary>
	public IJsonArrayLengthResult With(int amount);

	/// <summary>
	///     Add an expectation on the element at the given zero-based <paramref name="index" />.
	/// </summary>
	public IJsonPropertyResult<IJsonArrayResult> At(int index);

	/// <summary>
	///     Add an expectation on the elements of the array. They are matched against the <paramref name="expected" /> values.
	/// </summary>
	public IJsonArrayElementsResult WithElements(params object?[] expected);

	/// <summary>
	///     Add an expectation that the elements of the array are arrays which satisfy the <paramref name="expectations" />.
	/// </summary>
	public IJsonArrayElementsResult WithArrays(params Action<IJsonArrayResult>?[] expectations);

	/// <summary>
	///     Add an expectation that the elements of the array are objects which satisfy the <paramref name="expectations" />.
	/// </summary>
	public IJsonArrayElementsResult WithObjects(params Action<IJsonObjectResult>?[] expectations);

	/// <summary>
	///     Result for the number of elements in a JSON <see cref="JsonValueKind.Array" />.
	/// </summary>
	public interface IJsonArrayLengthResult
	{
		/// <summary>
		///     The number of elements in a JSON <see cref="JsonValueKind.Array" />.
		/// </summary>
		public IJsonArrayResult Elements();
	}

	/// <summary>
	///     Result for the enumeration of elements in a JSON array.
	/// </summary>
	public interface IJsonArrayElementsResult : IJsonArrayResult;
}
