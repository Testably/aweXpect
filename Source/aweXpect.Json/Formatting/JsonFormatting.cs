using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using aweXpect.Core;
using aweXpect.Customization;

namespace aweXpect.Json;

/// <summary>
///     Formatting extensions for <see cref="JsonElement" />.
/// </summary>
public static class JsonFormatting
{
	private static readonly IValueFormatter ElementValueFormatter = new JsonElementValueFormatter();
	private static readonly IValueFormatter PropertyValueFormatter = new JsonPropertyValueFormatter();
	private static readonly IValueFormatter NodeValueFormatter = new JsonNodeValueFormatter();

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
	///     Formats <see cref="JsonElement" />, <see cref="JsonProperty" /> and <see cref="JsonNode" /> values and the
	///     properties of a <see cref="JsonNode" /> that aweXpect formats as <see langword="object" />, e.g. the items of a
	///     collection, within the expectation of the <paramref name="expectationBuilder" />.
	/// </summary>
	/// <remarks>
	///     Without it, aweXpect formats a <see cref="JsonElement" /> with its <see cref="JsonElement.ToString()" />,
	///     which omits the quotes of strings and writes <see langword="null" /> as an empty string, a
	///     <see cref="JsonProperty" /> with its raw text, which keeps the line breaks of the document, and a
	///     <see cref="JsonObject" /> or <see cref="JsonArray" /> as a dictionary or a collection. Formatting outside of
	///     the expectation stays unchanged.
	///     <para />
	///     The element or node is written in a single line, like the raw text of an array or object that
	///     <see cref="JsonElement.ToString()" /> returns, and the property as it is written within such an object.
	/// </remarks>
	internal static void AddValueFormatters(ExpectationBuilder expectationBuilder)
		=> expectationBuilder
			.AddFormatter(ElementValueFormatter)
			.AddFormatter(PropertyValueFormatter)
			.AddFormatter(NodeValueFormatter);

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

	private sealed class JsonNodeValueFormatter : IValueFormatter
	{
		public bool TryFormat(StringBuilder stringBuilder, object value, FormattingOptions? options)
		{
			switch (value)
			{
				case JsonNode jsonNode:
					stringBuilder.Append(Write(writer => jsonNode.WriteTo(writer), FormattingOptions.SingleLine));
					return true;
				case KeyValuePair<string, JsonNode?> property:
					stringBuilder.Append(Write(writer =>
					{
						writer.WritePropertyName(property.Key);
						if (property.Value is null)
						{
							writer.WriteNullValue();
						}
						else
						{
							property.Value.WriteTo(writer);
						}
					}, FormattingOptions.SingleLine));
					return true;
				default:
					return false;
			}
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
