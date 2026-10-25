namespace aweXpect.Json;

/// <inheritdoc cref="IFoundJsonValue" />
/// <remarks>
///     The value is wrapped, so that the nested expectations are never verified on the subject itself, e.g. when the
///     property was not found and the subject is a <see cref="System.Text.Json.JsonElement" /> as well.
/// </remarks>
internal sealed class FoundJsonValue<TValue>(TValue value) : IFoundJsonValue
{
	public TValue Value { get; } = value;
}
