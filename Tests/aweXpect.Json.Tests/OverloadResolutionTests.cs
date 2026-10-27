using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using aweXpect.Core;
using aweXpect.Json.Results;
using aweXpect.Results;

namespace aweXpect.Json.Tests;

public sealed class OverloadResolutionTests
{
	[Test]
	public async Task JsonArray_ShouldKeepItsTypeUnlessTheNumberTypeIsSpecified()
	{
		JsonArray subject = new(1);

		IsArrayResult<JsonArray, IThat<JsonArray?>, JsonNode?> isArray = That(subject).IsArray();

		await That(await isArray).IsSameAs(subject);
		await That(subject).DoesNotComplyWith(it =>
		{
			IsObjectResult<JsonArray, IThat<JsonArray?>, KeyValuePair<string, JsonNode?>> result = it.IsObject();
		});
		await That(subject).DoesNotComplyWith(it =>
		{
			IsNumberResult<JsonArray, IThat<JsonArray?>> result = it.IsNumber();
		});
		await That(subject).DoesNotComplyWith(it =>
		{
			NumberToleranceResult<JsonArray, IThat<JsonArray?>, int> result = it.IsNumber(243);
		});
		await That(subject).DoesNotComplyWith(it =>
		{
			NumberToleranceResult<JsonNode, IThat<JsonNode?>, double> result = it.IsNumber<double>(243);
		});
		await That(subject).DoesNotComplyWith(it =>
		{
			AndOrResult<JsonNode, IThat<JsonNode?>> result = it.IsNumber<int>();
		});
		await That(subject).DoesNotComplyWith(it =>
		{
			AndOrResult<JsonNode, IThat<JsonNode?>> result = it.IsNumber<int>(n => n.IsPositive());
		});
	}

	[Test]
	public async Task JsonElement_ShouldBindTheOverloadsOfJsonElement()
	{
		JsonElement array = JsonDocument.Parse("[1]").RootElement;
		JsonElement obj = JsonDocument.Parse("{}").RootElement;
		JsonElement number = JsonDocument.Parse("243").RootElement;

		IsArrayResult<JsonElement, IThat<JsonElement>, JsonElement> isArray = That(array).IsArray();
		IsObjectResult<JsonElement, IThat<JsonElement>, JsonProperty> isObject = That(obj).IsObject();
		IsNumberResult<JsonElement, IThat<JsonElement>> isNumber = That(number).IsNumber();
		NumberToleranceResult<JsonElement, IThat<JsonElement>, int> isNumberValue = That(number).IsNumber(243);
		NumberToleranceResult<JsonElement, IThat<JsonElement>, double> isNumberValueOfType =
			That(number).IsNumber<double>(243);
		AndOrResult<JsonElement, IThat<JsonElement>> isNumberOfType = That(number).IsNumber<int>();
		AndOrResult<JsonElement, IThat<JsonElement>> isNumberOfTypeWithExpectations =
			That(number).IsNumber<int>(n => n.IsPositive());

		await That((await isArray).GetArrayLength()).IsEqualTo(1);
		await That((await isObject).EnumerateObject()).IsEmpty();
		await That((await isNumber).GetInt32()).IsEqualTo(243);
		await That((await isNumberValue).GetInt32()).IsEqualTo(243);
		await That((await isNumberValueOfType).GetInt32()).IsEqualTo(243);
		await That((await isNumberOfType).GetInt32()).IsEqualTo(243);
		await That((await isNumberOfTypeWithExpectations).GetInt32()).IsEqualTo(243);
	}

	[Test]
	public async Task JsonNode_ShouldBindTheOverloadsOfJsonNode()
	{
		JsonNode array = new JsonArray(1);
		JsonNode obj = new JsonObject();
		JsonNode number = JsonValue.Create(243);

		IsArrayResult<JsonNode, IThat<JsonNode?>, JsonNode?> isArray = That(array).IsArray();
		IsObjectResult<JsonNode, IThat<JsonNode?>, KeyValuePair<string, JsonNode?>> isObject =
			That(obj).IsObject();
		IsNumberResult<JsonNode, IThat<JsonNode?>> isNumber = That(number).IsNumber();
		NumberToleranceResult<JsonNode, IThat<JsonNode?>, int> isNumberValue = That(number).IsNumber(243);
		NumberToleranceResult<JsonNode, IThat<JsonNode?>, double> isNumberValueOfType =
			That(number).IsNumber<double>(243);
		AndOrResult<JsonNode, IThat<JsonNode?>> isNumberOfType = That(number).IsNumber<int>();
		AndOrResult<JsonNode, IThat<JsonNode?>> isNumberOfTypeWithExpectations =
			That(number).IsNumber<int>(n => n.IsPositive());

		await That(await isArray).IsSameAs(array);
		await That(await isObject).IsSameAs(obj);
		await That(await isNumber).IsSameAs(number);
		await That(await isNumberValue).IsSameAs(number);
		await That(await isNumberValueOfType).IsSameAs(number);
		await That(await isNumberOfType).IsSameAs(number);
		await That(await isNumberOfTypeWithExpectations).IsSameAs(number);
	}

	[Test]
	public async Task JsonObject_ShouldKeepItsTypeUnlessTheNumberTypeIsSpecified()
	{
		JsonObject subject = new();

		IsObjectResult<JsonObject, IThat<JsonObject?>, KeyValuePair<string, JsonNode?>> isObject = That(subject).IsObject();

		await That(await isObject).IsSameAs(subject);
		await That(subject).DoesNotComplyWith(it =>
		{
			IsArrayResult<JsonObject, IThat<JsonObject?>, JsonNode?> result = it.IsArray();
		});
		await That(subject).DoesNotComplyWith(it =>
		{
			IsNumberResult<JsonObject, IThat<JsonObject?>> result = it.IsNumber();
		});
		await That(subject).DoesNotComplyWith(it =>
		{
			NumberToleranceResult<JsonObject, IThat<JsonObject?>, int> result = it.IsNumber(243);
		});
		await That(subject).DoesNotComplyWith(it =>
		{
			NumberToleranceResult<JsonNode, IThat<JsonNode?>, double> result = it.IsNumber<double>(243);
		});
		await That(subject).DoesNotComplyWith(it =>
		{
			AndOrResult<JsonNode, IThat<JsonNode?>> result = it.IsNumber<int>();
		});
		await That(subject).DoesNotComplyWith(it =>
		{
			AndOrResult<JsonNode, IThat<JsonNode?>> result = it.IsNumber<int>(n => n.IsPositive());
		});
	}

	[Test]
	public async Task JsonValue_ShouldKeepItsType()
	{
		JsonValue subject = JsonValue.Create(243);

		IsNumberResult<JsonValue, IThat<JsonValue?>> isNumber = That(subject).IsNumber();
		NumberToleranceResult<JsonValue, IThat<JsonValue?>, int> isNumberValue = That(subject).IsNumber(243);
		NumberToleranceResult<JsonValue, IThat<JsonValue?>, double> isNumberValueOfType =
			That(subject).IsNumber<double>(243);
		AndOrResult<JsonValue, IThat<JsonValue?>> isNumberOfType = That(subject).IsNumber<int>();
		AndOrResult<JsonValue, IThat<JsonValue?>> isNumberOfTypeWithExpectations =
			That(subject).IsNumber<int>(n => n.IsPositive());

		await That(await isNumber).IsSameAs(subject);
		await That(await isNumberValue).IsSameAs(subject);
		await That(await isNumberValueOfType).IsSameAs(subject);
		await That(await isNumberOfType).IsSameAs(subject);
		await That(await isNumberOfTypeWithExpectations).IsSameAs(subject);
		await That(subject).DoesNotComplyWith(it =>
		{
			IsArrayResult<JsonValue, IThat<JsonValue?>, JsonNode?> result = it.IsArray();
		});
		await That(subject).DoesNotComplyWith(it =>
		{
			IsObjectResult<JsonValue, IThat<JsonValue?>, KeyValuePair<string, JsonNode?>> result = it.IsObject();
		});
	}

	[Test]
	public async Task NullableJsonElement_ShouldBindTheOverloadsOfNullableJsonElement()
	{
		JsonElement? array = JsonDocument.Parse("[1]").RootElement;
		JsonElement? obj = JsonDocument.Parse("{}").RootElement;
		JsonElement? number = JsonDocument.Parse("243").RootElement;

		IsArrayResult<JsonElement, IThat<JsonElement?>, JsonElement> isArray = That(array).IsArray();
		IsObjectResult<JsonElement, IThat<JsonElement?>, JsonProperty> isObject = That(obj).IsObject();
		IsNumberResult<JsonElement, IThat<JsonElement?>> isNumber = That(number).IsNumber();
		NumberToleranceResult<JsonElement, IThat<JsonElement?>, int> isNumberValue = That(number).IsNumber(243);
		NumberToleranceResult<JsonElement, IThat<JsonElement?>, double> isNumberValueOfType =
			That(number).IsNumber<double>(243);
		AndOrResult<JsonElement, IThat<JsonElement?>> isNumberOfType = That(number).IsNumber<int>();
		AndOrResult<JsonElement, IThat<JsonElement?>> isNumberOfTypeWithExpectations =
			That(number).IsNumber<int>(n => n.IsPositive());

		await That((await isArray).GetArrayLength()).IsEqualTo(1);
		await That((await isObject).EnumerateObject()).IsEmpty();
		await That((await isNumber).GetInt32()).IsEqualTo(243);
		await That((await isNumberValue).GetInt32()).IsEqualTo(243);
		await That((await isNumberValueOfType).GetInt32()).IsEqualTo(243);
		await That((await isNumberOfType).GetInt32()).IsEqualTo(243);
		await That((await isNumberOfTypeWithExpectations).GetInt32()).IsEqualTo(243);
	}

	[Test]
	public async Task NullableJsonNode_ShouldBindTheOverloadsOfJsonNode()
	{
		JsonNode? array = new JsonArray(1);
		JsonNode? obj = new JsonObject();
		JsonNode? number = JsonValue.Create(243);

		IsArrayResult<JsonNode, IThat<JsonNode?>, JsonNode?> isArray = That(array).IsArray();
		IsObjectResult<JsonNode, IThat<JsonNode?>, KeyValuePair<string, JsonNode?>> isObject =
			That(obj).IsObject();
		IsNumberResult<JsonNode, IThat<JsonNode?>> isNumber = That(number).IsNumber();
		NumberToleranceResult<JsonNode, IThat<JsonNode?>, int> isNumberValue = That(number).IsNumber(243);
		NumberToleranceResult<JsonNode, IThat<JsonNode?>, double> isNumberValueOfType =
			That(number).IsNumber<double>(243);
		AndOrResult<JsonNode, IThat<JsonNode?>> isNumberOfType = That(number).IsNumber<int>();
		AndOrResult<JsonNode, IThat<JsonNode?>> isNumberOfTypeWithExpectations =
			That(number).IsNumber<int>(n => n.IsPositive());

		await That(await isArray).IsSameAs(array);
		await That(await isObject).IsSameAs(obj);
		await That(await isNumber).IsSameAs(number);
		await That(await isNumberValue).IsSameAs(number);
		await That(await isNumberValueOfType).IsSameAs(number);
		await That(await isNumberOfType).IsSameAs(number);
		await That(await isNumberOfTypeWithExpectations).IsSameAs(number);
	}
}
