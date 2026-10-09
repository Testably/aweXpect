using System.Text;
using System.Text.Json;
using aweXpect.Customization;
using aweXpect.Json;

namespace aweXpect.Formatting;

/// <summary>
///     Formatting extensions for <see cref="JsonElement" />.
/// </summary>
public static class JsonFormatting
{
	/// <summary>
	///     Returns the according to the <paramref name="options" /> formatted <paramref name="value" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		JsonElement? value,
		FormattingOptions? options = null)
	{
		if (value == null)
		{
			return ValueFormatter.NullString;
		}

		return JsonSerializer.Serialize(value, GetSerializerOptions(options));
	}

	/// <summary>
	///     Appends the according to the <paramref name="options" /> formatted <paramref name="value" />
	///     to the <paramref name="stringBuilder" />
	/// </summary>
	public static void Format(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		JsonElement? value,
		FormattingOptions? options = null)
	{
		if (value == null)
		{
			stringBuilder.Append(ValueFormatter.NullString);
		}
		else
		{
			stringBuilder.Append(JsonSerializer.Serialize(value, GetSerializerOptions(options)));
		}
	}

	/// <remarks>
	///     Copies the customized options instead of changing them, because they belong to the user and
	///     become read-only once they were used for serialization.
	/// </remarks>
	private static JsonSerializerOptions GetSerializerOptions(FormattingOptions? options)
	{
		JsonSerializerOptions serializerOptions = Customize.aweXpect.Json().DefaultJsonSerializerOptions.Get();
		bool writeIndented = options?.UseLineBreaks == true;
		if (serializerOptions.WriteIndented == writeIndented)
		{
			return serializerOptions;
		}

		return new JsonSerializerOptions(serializerOptions)
		{
			WriteIndented = writeIndented,
		};
	}
}
