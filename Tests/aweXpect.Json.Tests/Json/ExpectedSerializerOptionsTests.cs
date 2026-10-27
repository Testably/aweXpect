using System.Text.Json;
using System.Text.Json.Serialization;
using aweXpect.Customization;
using aweXpect.Equivalency;

namespace aweXpect.Json.Tests;

public sealed partial class ExpectedSerializerOptionsTests
{
	[Test]
	public async Task CustomizedDefault_ShouldSerializeTheExpectedValue()
	{
		JsonElement subject = Parse("{\"title\": \"Let It Be\"}");

		async Task Act()
			=> await That(subject).MatchesExactly(new
			{
				Title = "Let It Be",
			});

		using (Customize.aweXpect.Json().DefaultJsonSerializerOptions.Set(CamelCase()))
		{
			await That(Act).DoesNotThrow();
		}

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             matches new
			             {
			             	Title = "Let It Be",
			             } exactly,
			             but it differed as
			               $.Title had missing "Let It Be" and
			               $.title had unexpected "Let It Be"
			             """)
			.Because("without customized options, the expected value keeps the casing of its properties");
	}

	[Test]
	public async Task CustomizedDefault_ShouldSerializeTheExpectedValueAlsoForSimpleValues()
	{
		JsonElement subject = Parse("\"42\"");

		async Task Act()
			=> await That(subject).MatchesExactly(42);

		using (Customize.aweXpect.Json().DefaultJsonSerializerOptions.Set(new JsonSerializerOptions
		       {
			       NumberHandling = JsonNumberHandling.WriteAsString,
		       }))
		{
			await That(Act).DoesNotThrow()
				.Because("customized options can change the JSON of simple values, too");
		}
	}

	[Test]
	public async Task CustomizedDefault_ShouldSerializeTheExpectedValueOfNestedMatches()
	{
		JsonElement subject = Parse("{\"track\": {\"title\": \"Let It Be\"}}");

		async Task Act()
			=> await That(subject).HasProperty("track", t => t.Matches(new
			{
				Title = "Let It Be",
			}));

		using (Customize.aweXpect.Json().DefaultJsonSerializerOptions.Set(CamelCase()))
		{
			await That(Act).DoesNotThrow();
		}
	}

	[Test]
	public async Task CustomizedDefault_ShouldSerializeTheExpectedValueOfNestedMatchesAlsoForSimpleValues()
	{
		JsonElement subject = Parse("{\"rating\": 42}");

		async Task Act()
			=> await That(subject).HasProperty("rating", r => r.Matches(42));

		using (Customize.aweXpect.Json().DefaultJsonSerializerOptions.Set(new JsonSerializerOptions
		       {
			       NumberHandling = JsonNumberHandling.WriteAsString,
		       }))
		{
			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             has property "rating" which matches 42,
				             but $.rating differed as $.rating was number 42 instead of "42"

				             JSON options ($.rating):
				              - ignore additional properties
				             """)
				.Because("the customized options write the expected number as a string, although the property equals it");
		}
	}

	[Test]
	public async Task ExpectationInObjectMember_WithSourceGeneratedContext_ShouldBeEvaluated()
	{
		JsonElement subject = Parse("{\"Title\": \"Let It Be\", \"Rating\": 3}");

		async Task Act()
			=> await That(subject).MatchesExactly(new Track
			{
				Title = "Let It Be",
				Rating = It.Is<int>().That.IsGreaterThan(4),
			}, o => o.WithSerializerOptions(TrackContext.Default.Options));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             matches new Track
			             {
			             	Title = "Let It Be",
			             	Rating = It.Is<int>().That.IsGreaterThan(4),
			             } exactly,
			             but it differed as $.Rating was 3, which differs by -1
			             """);
	}

	[Test]
	public async Task ExpectationValue_WithSourceGeneratedContext_ShouldBeEvaluated()
	{
		JsonElement subject = Parse("{\"rating\": 3}");

		async Task Act()
			=> await That(subject).HasProperty("rating", r => r.Matches(It.Is<int>().That.IsGreaterThan(4),
				o => o.WithSerializerOptions(TrackContext.Default.Options)));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             has property "rating" which matches It.Is<int>().That.IsGreaterThan(4),
			             but $.rating differed as $.rating was 3, which differs by -1

			             JSON options ($.rating):
			              - ignore additional properties
			             """);
	}

	[Test]
	public async Task ExplicitOptions_ShouldNotBeChanged()
	{
		JsonSerializerOptions serializerOptions = CamelCase();
		JsonElement subject = Parse("{\"rating\": 5}");

		async Task Act()
			=> await That(subject).MatchesExactly(new
			{
				Rating = It.Is<int>().That.IsGreaterThan(4),
			}, o => o.WithSerializerOptions(serializerOptions));

		await That(Act).DoesNotThrow();
		await That(Act).DoesNotThrow();
		await That(serializerOptions.Converters).IsEmpty()
			.Because("the converter for It.Is… expectations is added to a copy of the options");
	}

	[Test]
	public async Task ExplicitOptions_ShouldSerializeTheExpectedValue()
	{
		JsonElement subject = Parse("{\"title\": \"Let It Be\"}");

		async Task Act()
			=> await That(subject).MatchesExactly(new
			{
				Title = "Let It Be",
			}, o => o.WithSerializerOptions(CamelCase()));

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task ExplicitOptions_ShouldTakePrecedenceOverTheCustomizedDefault()
	{
		JsonElement subject = Parse("{\"Title\": \"Let It Be\"}");

		async Task Act()
			=> await That(subject).MatchesExactly(new
			{
				Title = "Let It Be",
			}, o => o.WithSerializerOptions(new JsonSerializerOptions()));

		using (Customize.aweXpect.Json().DefaultJsonSerializerOptions.Set(CamelCase()))
		{
			await That(Act).DoesNotThrow();
		}
	}

	[Test]
	public async Task ExplicitOptions_WhenComparingANullableElement_ShouldSerializeTheExpectedValue()
	{
		JsonElement? subject = Parse("{\"title\": \"Let It Be\"}");

		async Task Act()
			=> await That(subject).MatchesExactly(new
			{
				Title = "Let It Be",
			}, o => o.WithSerializerOptions(CamelCase()));

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task ExplicitOptions_WhenComparingAString_ShouldSerializeTheExpectedValue()
	{
		string subject = "{\"title\": \"Let It Be\"}";

		async Task Act()
			=> await That(subject).IsValidJsonMatchingExactly(new
			{
				Title = "Let It Be",
			}, o => o.WithSerializerOptions(CamelCase()));

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task ExplicitOptions_WhenFailing_ShouldNotListTheOptions()
	{
		JsonElement subject = Parse("{\"title\": \"Yesterday\"}");

		async Task Act()
			=> await That(subject).MatchesExactly(new
			{
				Title = "Let It Be",
			}, o => o.WithSerializerOptions(CamelCase()));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             matches new
			             {
			             	Title = "Let It Be",
			             } exactly,
			             but it differed as $.title was "Yesterday" instead of "Let It Be"
			             """);
	}

	[Test]
	public async Task JsonText_WithSourceGeneratedContext_ShouldBeParsedWithoutTheSerializer()
	{
		JsonElement subject = Parse("{\"Title\": \"Let It Be\"}");

		async Task Act()
			=> await That(subject).MatchesExactly("{\"Title\": \"Let It Be\"}",
				o => o.WithSerializerOptions(TrackContext.Default.Options));

		await That(Act).DoesNotThrow()
			.Because("JSON text is parsed, so the serializer options do not apply to it");
	}

	[Test]
	public async Task MissingTypeInObjectMember_ShouldThrowNotSupportedException()
	{
		JsonElement subject = Parse("{}");

		async Task Act()
			=> await That(subject).MatchesExactly(new Track
			{
				Rating = new Album(),
			}, o => o.WithSerializerOptions(TrackContext.Default.Options));

		await That(Act).Throws<NotSupportedException>()
			.WithMessage(
				"The expected value cannot be serialized as JSON, because the TypeInfoResolver of the serializer options provides no metadata for ExpectedSerializerOptionsTests.Album. Add [JsonSerializable(typeof(ExpectedSerializerOptionsTests.Album))] to the JsonSerializerContext. The unsupported member type is located on type 'System.Object'. Path: $.Rating.")
			.Because("the serializer adds the path of the member to the message");
	}

	[Test]
	public async Task MissingTypeInSourceGeneratedContext_ShouldThrowNotSupportedException()
	{
		JsonElement subject = Parse("{}");

		async Task Act()
			=> await That(subject).MatchesExactly(new Album(), o => o.WithSerializerOptions(TrackContext.Default.Options));

		await That(Act).Throws<NotSupportedException>()
			.WithMessage(
				"The expected value cannot be serialized as JSON, because the TypeInfoResolver of the serializer options provides no metadata for ExpectedSerializerOptionsTests.Album. Add [JsonSerializable(typeof(ExpectedSerializerOptionsTests.Album))] to the JsonSerializerContext.");
	}

	[Test]
	public async Task NullWithSourceGeneratedContext_ShouldBeWrittenAsNull()
	{
		JsonElement subject = Parse("null");

		async Task Act()
			=> await That(subject).MatchesExactly(null, o => o.WithSerializerOptions(TrackContext.Default.Options));

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task ObjectInObjectMember_WithSourceGeneratedContext_ShouldBeWrittenAsEmptyObject()
	{
		JsonElement subject = Parse("{\"Title\": null, \"Rating\": {}}");

		async Task Act()
			=> await That(subject).MatchesExactly(new Track
			{
				Rating = new object(),
			}, o => o.WithSerializerOptions(TrackContext.Default.Options));

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task SourceGeneratedContext_ShouldSerializeTheExpectedValue()
	{
		JsonElement subject = Parse("{\"Title\": \"Let It Be\", \"Rating\": 5}");

		async Task Act()
			=> await That(subject).MatchesExactly(new Track
			{
				Title = "Let It Be",
				Rating = 5,
			}, o => o.WithSerializerOptions(TrackContext.Default.Options));

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task WithSerializerOptions_WhenNull_ShouldThrowArgumentNullException()
	{
		JsonElement subject = Parse("1");

		async Task Act()
			=> await That(subject).MatchesExactly(1, o => o.WithSerializerOptions(null!));

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("serializerOptions").And
			.WithMessage("The 'serializerOptions' cannot be null.").AsPrefix();
	}

	private static JsonSerializerOptions CamelCase()
		=> new()
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		};

	private static JsonElement Parse(string json)
	{
		using JsonDocument document = JsonDocument.Parse(json);
		return document.RootElement.Clone();
	}

	private sealed class Track
	{
		public string? Title { get; set; }
		public object? Rating { get; set; }
	}

	private sealed class Album;

	[JsonSerializable(typeof(Track))]
	[JsonSerializable(typeof(int))]
	private sealed partial class TrackContext : JsonSerializerContext;
}
