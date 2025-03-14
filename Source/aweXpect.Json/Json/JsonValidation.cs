using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using aweXpect.Core;

namespace aweXpect.Json;

internal class JsonValidation : IJsonObjectResult,
	IJsonObjectResult.IJsonObjectLengthResult,
	IJsonArrayResult.IJsonArrayLengthResult,
	IJsonArrayResult.IJsonArrayElementsResult,
	IJsonPropertyResult<IJsonArrayResult>,
	IJsonPropertyResult<IJsonObjectResult>
{
	private const string And = " and ";
	private readonly Stack<JsonElement?> _currentElements = new();
	private readonly Stack<string> _currentPath = new();
	private readonly JsonElement? _element;

	private readonly StringBuilder _expectationBuilder;

	private readonly List<string> _failures = new();
	private readonly JsonOptions _options;
	private readonly JsonValueKind _valueKind;
	private int? _amount;

	public JsonValidation(JsonElement? element, JsonValueKind valueKind, JsonOptions options)
		: this("$", element, valueKind, options)
	{
	}

	private JsonValidation(string path, JsonElement? element, JsonValueKind valueKind, JsonOptions options)
	{
		_valueKind = valueKind;
		_options = options;
		_element = element;
		_currentElements.Push(element);
		_expectationBuilder = new StringBuilder().Append("is an ").Append(valueKind.ToString().ToLower());
		_currentPath.Push(path);
	}

	private string CurrentPath => string.Join("", _currentPath.Reverse());

	IJsonArrayResult IJsonArrayResult.And => this;

	IJsonArrayResult.IJsonArrayLengthResult IJsonArrayResult.With(int amount)
	{
		_amount = amount;
		return this;
	}

	IJsonPropertyResult<IJsonArrayResult> IJsonArrayResult.At(int index)
	{
#if NET8_0_OR_GREATER
		ArgumentOutOfRangeException.ThrowIfNegative(index);
#else
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(index), "The index must be greater than or equal to 0.");
		}
#endif

		_currentPath.Push($"[{index}]");
		JsonElement? currentElement = _currentElements.Peek();
		if (currentElement == null)
		{
			_currentElements.Push(null);
			return this;
		}

		if (currentElement.Value.ValueKind != JsonValueKind.Array)
		{
			_currentElements.Push(null);
		}
		else if (currentElement.Value.GetArrayLength() > index)
		{
			_currentElements.Push(currentElement.Value[index]);
		}
		else
		{
			_currentElements.Push(null);
			_failures.Add($" index {CurrentPath} did not exist");
		}

		return this;
	}

	IJsonArrayResult.IJsonArrayElementsResult IJsonArrayResult.WithElements(params object?[] expected)
	{
		JsonElement? arrayElement = _currentElements.Pop();
		int? arrayLength = arrayElement?.GetArrayLength();

		for (int i = 0; i < expected.Length; i++)
		{
			object? expectedValue = expected[i];
			JsonElement? currentElement = null;
			if (arrayElement == null || i >= arrayLength)
			{
				_failures.Add($" {CurrentPath}[{i}] did not exist");
			}
			else
			{
				currentElement = arrayElement.Value[i];
			}

			_currentPath.Push($"[{i}]");

			_expectationBuilder.Append(And).Append(CurrentPath).Append(" matches ")
				.Append(expectedValue == null ? "Null" : Formatter.Format(expectedValue));

			if (currentElement != null)
			{
				using JsonDocument expectedDocument =
					JsonDocument.Parse(JsonSerializer.Serialize(expectedValue), _options.DocumentOptions);
				JsonElementValidator.JsonComparisonResult comparisonResult = JsonElementValidator.Compare(
					CurrentPath,
					currentElement.Value,
					expectedDocument.RootElement,
					_options);

				if (comparisonResult.HasError)
				{
					_failures.Add(comparisonResult.ToString());
				}
			}


			_currentPath.Pop();
		}

		return this;
	}

	IJsonArrayResult.IJsonArrayElementsResult IJsonArrayResult.WithArrays(
		params Action<IJsonArrayResult>?[] expectations)
	{
		JsonElement? arrayElement = _currentElements.Pop();
		int? arrayLength = arrayElement?.GetArrayLength();

		for (int i = 0; i < expectations.Length; i++)
		{
			Action<IJsonArrayResult>? expectation = expectations[i];
			if (expectation == null)
			{
				continue;
			}

			JsonElement? currentElement = null;
			if (arrayElement == null || i >= arrayLength)
			{
				_failures.Add($" {CurrentPath}[{i}] did not exist");
			}
			else
			{
				currentElement = arrayElement.Value[i];
			}

			_currentPath.Push($"[{i}]");

			_expectationBuilder.Append(And).Append(CurrentPath).Append(' ');

			JsonValidation jsonValidation = new(CurrentPath, currentElement, JsonValueKind.Array, _options);
			expectation.Invoke(jsonValidation);
			_expectationBuilder.Append(jsonValidation.GetExpectation());

			if (currentElement != null)
			{
				if (currentElement.Value.ValueKind != JsonValueKind.Array)
				{
					_failures.Add(
						$" {CurrentPath} was {Format(currentElement.Value.ValueKind)} instead of {Format(JsonValueKind.Array)}");
				}
				else if (!jsonValidation.IsMet())
				{
					_failures.Add(jsonValidation.GetFailures());
				}
			}

			_currentPath.Pop();
		}

		return this;
	}

	IJsonArrayResult.IJsonArrayElementsResult IJsonArrayResult.WithObjects(
		params Action<IJsonObjectResult>?[] expectations)
	{
		JsonElement? arrayElement = _currentElements.Pop();
		int? arrayLength = arrayElement?.GetArrayLength();

		for (int i = 0; i < expectations.Length; i++)
		{
			Action<IJsonObjectResult>? expectation = expectations[i];
			if (expectation == null)
			{
				continue;
			}

			JsonElement? currentElement = null;
			if (arrayElement == null || i >= arrayLength)
			{
				_failures.Add($" {CurrentPath}[{i}] did not exist");
			}
			else
			{
				currentElement = arrayElement.Value[i];
			}

			_currentPath.Push($"[{i}]");

			_expectationBuilder.Append(And).Append(CurrentPath).Append(' ');

			JsonValidation jsonValidation = new(CurrentPath, currentElement, JsonValueKind.Object, _options);
			expectation.Invoke(jsonValidation);
			_expectationBuilder.Append(jsonValidation.GetExpectation());

			if (currentElement != null)
			{
				if (currentElement.Value.ValueKind != JsonValueKind.Object)
				{
					_failures.Add(
						$" {CurrentPath} was {Format(currentElement.Value.ValueKind)} instead of {Format(JsonValueKind.Object)}");
				}
				else if (!jsonValidation.IsMet())
				{
					_failures.Add(jsonValidation.GetFailures());
				}
			}

			_currentPath.Pop();
		}

		return this;
	}

	IJsonArrayResult IJsonArrayResult.IJsonArrayLengthResult.Elements()
	{
		_expectationBuilder.Append(" with ").Append(_amount).Append(_amount == 1 ? " element" : " elements");
		JsonElement? currentElement = _currentElements.Peek();
		if (currentElement is not { ValueKind: JsonValueKind.Array, })
		{
			_amount = null;
			return this;
		}

		int actualLength = currentElement.Value.GetArrayLength();
		if (actualLength != _amount)
		{
			_failures.Add($" {CurrentPath} had {actualLength} {(actualLength == 1 ? "element" : "elements")}");
		}

		_amount = null;
		return this;
	}

	IJsonObjectResult IJsonObjectResult.IJsonObjectLengthResult.Properties()
	{
		_expectationBuilder.Append(" with ").Append(_amount).Append(_amount == 1 ? " property" : " properties");
		JsonElement? currentElement = _currentElements.Peek();
		if (currentElement is not { ValueKind: JsonValueKind.Object, })
		{
			_amount = null;
			return this;
		}

		int actualLength = currentElement.Value.EnumerateObject().Count();
		if (actualLength != _amount)
		{
			_failures.Add($" {CurrentPath} had {actualLength} {(actualLength == 1 ? "property" : "properties")}");
		}

		_amount = null;
		return this;
	}

	IJsonObjectResult.IJsonObjectLengthResult IJsonObjectResult.With(int amount)
	{
		_amount = amount;
		return this;
	}

	IJsonPropertyResult<IJsonObjectResult> IJsonObjectResult.With(string propertyName)
	{
		_currentPath.Push($".{propertyName}");
		JsonElement? currentElement = _currentElements.Peek();
		if (currentElement == null)
		{
			_currentElements.Push(null);
			return this;
		}

		if (currentElement.Value.ValueKind != JsonValueKind.Object)
		{
			_currentElements.Push(null);
			_failures.Add($" {CurrentPath} was {Format(currentElement.Value.ValueKind)} instead of an object");
		}
		else if (currentElement.Value.TryGetProperty(propertyName, out JsonElement propertyValue))
		{
			_currentElements.Push(propertyValue);
		}
		else
		{
			_currentElements.Push(null);
			_failures.Add($" property {CurrentPath} did not exist");
		}

		return this;
	}

	IJsonObjectResult IJsonObjectResult.And => this;

	IJsonArrayResult IJsonPropertyResult<IJsonArrayResult>.Matching(object? expected, string doNotPopulateThisValue)
	{
		_expectationBuilder.Append(And).Append(CurrentPath).Append(" matches ").Append(doNotPopulateThisValue);
		JsonElement? currentElement = _currentElements.Pop();
		if (currentElement == null)
		{
			_currentPath.Pop();
			return this;
		}

		using JsonDocument expectedDocument =
			JsonDocument.Parse(JsonSerializer.Serialize(expected), _options.DocumentOptions);
		JsonElementValidator.JsonComparisonResult comparisonResult = JsonElementValidator.Compare(
			CurrentPath,
			currentElement.Value,
			expectedDocument.RootElement,
			_options);

		if (comparisonResult.HasError)
		{
			_failures.Add(comparisonResult.ToString());
		}

		_currentPath.Pop();
		return this;
	}

	IJsonArrayResult IJsonPropertyResult<IJsonArrayResult>.AnArray()
		=> An(JsonValueKind.Array);

	IJsonArrayResult IJsonPropertyResult<IJsonArrayResult>.AnArray(Action<IJsonArrayResult> expectation)
		=> An(JsonValueKind.Array, expectation);

	IJsonArrayResult IJsonPropertyResult<IJsonArrayResult>.AnObject()
		=> An(JsonValueKind.Object);

	IJsonArrayResult IJsonPropertyResult<IJsonArrayResult>.AnObject(Action<IJsonObjectResult> expectation)
		=> An(JsonValueKind.Object, expectation);

	IJsonObjectResult IJsonPropertyResult<IJsonObjectResult>.AnArray()
		=> An(JsonValueKind.Array);

	IJsonObjectResult IJsonPropertyResult<IJsonObjectResult>.AnArray(Action<IJsonArrayResult> expectation)
		=> An(JsonValueKind.Array, expectation);

	IJsonObjectResult IJsonPropertyResult<IJsonObjectResult>.AnObject()
		=> An(JsonValueKind.Object);

	IJsonObjectResult IJsonPropertyResult<IJsonObjectResult>.AnObject(Action<IJsonObjectResult> expectation)
		=> An(JsonValueKind.Object, expectation);

	IJsonObjectResult IJsonPropertyResult<IJsonObjectResult>.Matching(object? expected, string doNotPopulateThisValue)
	{
		_expectationBuilder.Append(And).Append(CurrentPath).Append(" matches ").Append(doNotPopulateThisValue);
		JsonElement? currentElement = _currentElements.Pop();
		if (currentElement == null)
		{
			_currentPath.Pop();
			return this;
		}

		using JsonDocument expectedDocument =
			JsonDocument.Parse(JsonSerializer.Serialize(expected), _options.DocumentOptions);
		JsonElementValidator.JsonComparisonResult comparisonResult = JsonElementValidator.Compare(
			CurrentPath,
			currentElement.Value,
			expectedDocument.RootElement,
			_options);

		if (comparisonResult.HasError)
		{
			_failures.Add(comparisonResult.ToString());
		}

		_currentPath.Pop();
		return this;
	}

	private JsonValidation An(JsonValueKind kind, Action<JsonValidation> expectation)
	{
		_expectationBuilder.Append(And).Append(CurrentPath).Append(' ');
		JsonElement? currentElement = _currentElements.Pop();

		JsonValidation jsonValidation = new(CurrentPath, currentElement, kind, _options);
		expectation.Invoke(jsonValidation);
		_expectationBuilder.Append(jsonValidation.GetExpectation());

		if (currentElement != null)
		{
			if (currentElement.Value.ValueKind != kind)
			{
				_failures.Add($" {CurrentPath} was {Format(currentElement.Value.ValueKind)} instead of {Format(kind)}");
			}
			else if (!jsonValidation.IsMet())
			{
				_failures.Add(jsonValidation.GetFailures());
			}
		}

		_currentPath.Pop();
		return this;
	}

	private JsonValidation An(JsonValueKind kind)
	{
		_expectationBuilder.Append(And).Append(CurrentPath).Append(" is ").Append(Format(kind));
		JsonElement? currentElement = _currentElements.Pop();

		if (currentElement != null && currentElement.Value.ValueKind != kind)
		{
			_failures.Add($" {CurrentPath} was {Format(currentElement.Value.ValueKind)} instead of {Format(kind)}");
		}

		_currentPath.Pop();
		return this;
	}

	public bool IsMet()
		=> _element is not null && _element.Value.ValueKind == _valueKind && _failures.Count == 0;

	public string GetExpectation()
		=> _expectationBuilder.ToString();

	public string GetFailure(string it)
	{
		if (_element is null)
		{
			return $"{it} was <null>";
		}

		if (_element.Value.ValueKind != _valueKind)
		{
			return $"{it} was {Format(_element.Value.ValueKind)} instead of {Format(_valueKind)}";
		}

		return
			$"{it} differed as{(_failures.Count > 1 ? Environment.NewLine + " " : "")}{GetFailures()}";
	}

	private string GetFailures()
	{
		string failureSeparator = " and" + Environment.NewLine + " ";
		return string.Join(failureSeparator, _failures);
	}

	internal static string Format(JsonValueKind valueKind, ExpectationGrammars grammars = ExpectationGrammars.None)
		=> (valueKind, grammars.IsNegated()) switch
		{
			(JsonValueKind.Array, false) => "an array",
			(JsonValueKind.Object, false) => "an object",
			(JsonValueKind.Number, false) => "a number",
			(JsonValueKind.String, false) => "a string",
			(_, false) => valueKind.ToString().ToLower(),
			(JsonValueKind.Array, true) => "no array",
			(JsonValueKind.Object, true) => "no object",
			(JsonValueKind.Number, true) => "no number",
			(JsonValueKind.String, true) => "no string",
			(_, true) => $"not {valueKind.ToString().ToLower()}",
		};
}
