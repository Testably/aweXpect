using System.Text.Json;

namespace aweXpect.Json;

/// <summary>
///     Reads the <typeparamref name="TValue" /> of a JSON value of the <see cref="Kind" />.
/// </summary>
/// <param name="kind">The kind of the JSON value, where <see cref="JsonValueKind.True" /> stands for a boolean.</param>
/// <param name="description">
///     The expected value, e.g. <c>an int</c>, or <see langword="null" /> to describe it by its <paramref name="kind" />.
/// </param>
/// <param name="typeName">The type that a value of the <paramref name="kind" /> may not fit, e.g. <c>a double</c>.</param>
/// <param name="read">Reads the value, or returns <see langword="false" /> if it does not fit.</param>
internal sealed class JsonValueReader<TValue>(
	JsonValueKind kind,
	string? description,
	string typeName,
	JsonValueRead<TValue> read)
{
	public JsonValueKind Kind { get; } = kind;
	public string? Description { get; } = description;
	public string TypeName { get; } = typeName;
	public JsonValueRead<TValue> Read { get; } = read;
}
