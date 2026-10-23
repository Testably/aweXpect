using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using aweXpect.Customization;
using aweXpect.Json;

namespace aweXpect.Formatting;

/// <summary>
///     Formatting extensions for <see cref="JsonElement" />.
/// </summary>
public static class JsonFormatting
{
	private static int _isValueFormatterRegistered;

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

		return Write(value.Value, options);
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
			stringBuilder.Append(Write(value.Value, options));
		}
	}

	/// <summary>
	///     Registers the formatting of <see cref="JsonElement" /> and <see cref="JsonProperty" /> for values that
	///     aweXpect formats as <see langword="object" />, e.g. the items of a collection, once per process.
	/// </summary>
	/// <remarks>
	///     Without it, aweXpect formats a <see cref="JsonElement" /> with its <see cref="JsonElement.ToString()" />,
	///     which omits the quotes of strings and writes <see langword="null" /> as an empty string, and a
	///     <see cref="JsonProperty" /> with its raw text, which keeps the line breaks of the document.
	///     <para />
	///     The element is written in a single line, like the raw text of an array or object that
	///     <see cref="JsonElement.ToString()" /> returns, and the property as it is written within such an object.
	/// </remarks>
	internal static void RegisterValueFormatter()
	{
		if (Interlocked.Exchange(ref _isValueFormatterRegistered, 1) == 0)
		{
			ValueFormatter.Register(new JsonElementValueFormatter());
			ValueFormatter.Register(new JsonPropertyValueFormatter());
		}
	}

	private static string Write(JsonElement value, FormattingOptions? options)
		=> Write(value.WriteTo, options);

	/// <remarks>
	///     Writes the value directly with the writer settings of the customized serializer options, because
	///     serializing it with options that have no type info resolver requires reflection.
	///     <para />
	///     The validation is skipped, so that a property can be written without its enclosing object.
	/// </remarks>
	private static string Write(Action<Utf8JsonWriter> write, FormattingOptions? options)
	{
		JsonSerializerOptions serializerOptions = Customize.aweXpect.Json().DefaultJsonSerializerOptions.Get();
		JsonWriterOptions writerOptions = new()
		{
			Encoder = serializerOptions.Encoder,
			Indented = options?.UseLineBreaks == true,
			SkipValidation = true,
#if !NET8_0
			IndentCharacter = serializerOptions.IndentCharacter,
			IndentSize = serializerOptions.IndentSize,
			NewLine = serializerOptions.NewLine,
#endif
		};
		using MemoryStream stream = new();
		using (Utf8JsonWriter writer = new(stream, writerOptions))
		{
			write(writer);
		}

		return Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);
	}

	private sealed class JsonElementValueFormatter : IValueFormatter
	{
		public bool TryFormat(StringBuilder stringBuilder, object value, FormattingOptions? options)
		{
			if (value is not JsonElement jsonElement)
			{
				return false;
			}

			stringBuilder.Append(Write(jsonElement, FormattingOptions.SingleLine));
			return true;
		}
	}

	private sealed class JsonPropertyValueFormatter : IValueFormatter
	{
		public bool TryFormat(StringBuilder stringBuilder, object value, FormattingOptions? options)
		{
			if (value is not JsonProperty jsonProperty)
			{
				return false;
			}

			stringBuilder.Append(Write(jsonProperty.WriteTo, FormattingOptions.SingleLine));
			return true;
		}
	}
}
