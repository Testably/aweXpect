using System.Text.Json;

namespace aweXpect.Json;

/// <summary>
///     Reads the value of a <see cref="JsonElement" /> of the expected kind as a <typeparamref name="TValue" />.
/// </summary>
internal delegate bool JsonValueRead<TValue>(JsonElement element, out TValue value);
