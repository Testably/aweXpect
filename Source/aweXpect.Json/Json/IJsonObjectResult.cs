using System.Text.Json;

namespace aweXpect.Json;

/// <summary>
///     The result for an expectation on a JSON <see cref="JsonValueKind.Object" />.
/// </summary>
public interface IJsonObjectResult
{
	/// <summary>
	///     Combine multiple JSON expectations on this <see cref="JsonValueKind.Object" />.
	/// </summary>
	public IJsonObjectResult And { get; }

	/// <summary>
	///     Add an expectation on the number of properties in the object.
	/// </summary>
	public IJsonObjectLengthResult With(int amount);

	/// <summary>
	///     Add an expectation on the property with the given <paramref name="propertyName" />.
	/// </summary>
	public IJsonPropertyResult<IJsonObjectResult> With(string propertyName);

	/// <summary>
	///     Add an expectation on the value at the given <paramref name="path" />, relative to this object.
	/// </summary>
	/// <remarks>
	///     The <paramref name="path" /> is a property name, followed by any number of <c>.name</c> or zero-based
	///     <c>[index]</c> segments, e.g. <c>"customer.address.city"</c> or <c>"items[0].sku"</c>, and is written without
	///     a leading <c>$</c>. Wildcards, filters and recursive descent are not supported, and property names that contain
	///     <c>.</c>, <c>[</c> or <c>]</c> can only be addressed with <see cref="With(string)" />.
	///     <para />
	///     The value is verified as if each segment was addressed with <see cref="With(string)" /> or
	///     <see cref="IJsonArrayResult.At(int)" />, but only the first segment that cannot be reached is reported.
	/// </remarks>
	public IJsonPropertyResult<IJsonObjectResult> At(string path);

	/// <summary>
	///     Add an expectation that the object does not contain the property with the given
	///     <paramref name="propertyName" />.
	/// </summary>
	/// <remarks>
	///     A property with the value <see langword="null" /> exists and therefore fails the expectation, unless
	///     <see cref="JsonOptionsExtensions.TreatingNullAsAbsent(JsonOptions, bool)" /> is set.
	/// </remarks>
	public IJsonObjectResult Without(string propertyName);

	/// <summary>
	///     Result for the number of properties in a JSON <see cref="JsonValueKind.Object" />.
	/// </summary>
	public interface IJsonObjectLengthResult
	{
		/// <summary>
		///     The number of properties in a JSON <see cref="JsonValueKind.Object" />.
		/// </summary>
		public IJsonObjectResult Properties();
	}
}
