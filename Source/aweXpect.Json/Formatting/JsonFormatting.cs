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

		JsonSerializerOptions serializerOptions = Customize.aweXpect.Json().DefaultJsonSerializerOptions.Get();
		serializerOptions.WriteIndented = options?.UseLineBreaks == true;
		return JsonSerializer.Serialize(value, serializerOptions);
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
			JsonSerializerOptions serializerOptions = Customize.aweXpect.Json().DefaultJsonSerializerOptions.Get();
			serializerOptions.WriteIndented = options?.UseLineBreaks == true;
			stringBuilder.Append(JsonSerializer.Serialize(value, serializerOptions));
		}
	}
}
