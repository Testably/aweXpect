using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.EvaluationContext;
using aweXpect.Helpers;

namespace aweXpect.Json;

internal class JsonValidation : IJsonObjectResult,
	IJsonObjectResult.IJsonObjectLengthResult,
	IJsonArrayResult.IJsonArrayLengthResult,
	IJsonArrayResult.IJsonArrayElementsResult,
	IJsonPropertyResult<IJsonArrayResult>,
	IJsonPropertyResult<IJsonObjectResult>
{
	private readonly IEvaluationContext _context;
	private readonly Stack<JsonElement?> _currentElements = new();
	private readonly Stack<string> _currentPath = new();
	private readonly JsonElement? _element;

	private readonly List<Action<StringBuilder, ExpectationGrammars>> _expectationBuilder;

	private readonly List<string?> _failures = new();
	private readonly JsonOptions _options;
	private readonly List<Func<CancellationToken, Task>> _pendingChecks = new();
	private readonly JsonValidation _root;
	private readonly JsonValueKind _valueKind;
	private int? _amount;
	private Exception? _ownException;

	public JsonValidation(JsonElement? element, JsonValueKind valueKind, JsonOptions options,
		IEvaluationContext context)
		: this(null, "$", element, valueKind, options, context)
	{
	}

	private JsonValidation(JsonValidation? root, string path, JsonElement? element, JsonValueKind valueKind,
		JsonOptions options, IEvaluationContext context)
	{
		_root = root ?? this;
		_valueKind = valueKind;
		_options = options;
		_context = context;
		_element = element;
		_currentElements.Push(element);
		_expectationBuilder =
		[
			(sb, grammars) => sb.Append(grammars.Verb("is ", "are ")).Append(Format(valueKind, grammars)),
		];
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
		if (index < 0)
		{
			throw Own(Tracing.WriteException(
				new ArgumentOutOfRangeException(nameof(index), "The index must not be negative.")));
		}

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
		JsonElement? arrayElement = PopArray();
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

			string currentPath = CurrentPath;
			_expectationBuilder.Add((sb, grammars) => sb.Append(And(grammars)).Append(currentPath)
				.Append(grammars.IsNegated() ? " does not match " : " matches ")
				.Append(Formatter.Format(expectedValue)));

			if (currentElement != null)
			{
				CompareElement(currentElement.Value, expectedValue);
			}


			_currentPath.Pop();
		}

		return this;
	}

	private void CompareElement(JsonElement element, object? expectedValue)
	{
		string path = CurrentPath;
		AddPendingCheck(async cancellationToken =>
		{
			ExpectationJsonConverter converter = new(_context, cancellationToken);
			using JsonDocument expectedDocument = converter.ParseExpected(expectedValue, _options.DocumentOptions);
			JsonElementValidator.JsonComparisonResult comparisonResult = await JsonElementValidator.Compare(
				path,
				element,
				expectedDocument.RootElement,
				_options,
				converter);
			return comparisonResult.HasError ? comparisonResult.ToString() : null;
		});
	}

	private void ValidateNested(JsonValidation jsonValidation)
		=> AddPendingCheck(async cancellationToken =>
		{
			await jsonValidation.ValidateAsync(cancellationToken);
			return jsonValidation.IsMet() ? null : jsonValidation.GetFailures();
		});

	/// <summary>
	///     Reserves the place of the failure of the <paramref name="check" />, which is only made in
	///     <see cref="ValidateAsync" />, so that the failures keep the order in which the expectation specified them.
	/// </summary>
	private void AddPendingCheck(Func<CancellationToken, Task<string?>> check)
	{
		int index = _failures.Count;
		_failures.Add(null);
		_pendingChecks.Add(async cancellationToken => _failures[index] = await check(cancellationToken));
	}

	IJsonArrayResult.IJsonArrayElementsResult IJsonArrayResult.WithArrays(
		params Action<IJsonArrayResult>?[] expectations)
	{
		JsonElement? arrayElement = PopArray();
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

			string currentPath = CurrentPath;
			_expectationBuilder.Add((sb, grammars) => sb.Append(And(grammars)).Append(currentPath).Append(' '));

			JsonValidation jsonValidation = new(_root, CurrentPath, currentElement, JsonValueKind.Array, _options,
				_context);
			_expectationBuilder.Add(jsonValidation.GetNestedExpectation);
			expectation.Invoke(jsonValidation);

			if (currentElement != null)
			{
				if (currentElement.Value.ValueKind != JsonValueKind.Array)
				{
					_failures.Add($" {CurrentPath} was {Format(currentElement.Value.ValueKind)}");
				}
				else
				{
					ValidateNested(jsonValidation);
				}
			}

			_currentPath.Pop();
		}

		return this;
	}

	IJsonArrayResult.IJsonArrayElementsResult IJsonArrayResult.WithObjects(
		params Action<IJsonObjectResult>?[] expectations)
	{
		JsonElement? arrayElement = PopArray();
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

			string currentPath = CurrentPath;
			_expectationBuilder.Add((sb, grammars) => sb.Append(And(grammars)).Append(currentPath).Append(' '));

			JsonValidation jsonValidation = new(_root, CurrentPath, currentElement, JsonValueKind.Object, _options,
				_context);
			_expectationBuilder.Add(jsonValidation.GetNestedExpectation);
			expectation.Invoke(jsonValidation);

			if (currentElement != null)
			{
				if (currentElement.Value.ValueKind != JsonValueKind.Object)
				{
					_failures.Add($" {CurrentPath} was {Format(currentElement.Value.ValueKind)}");
				}
				else
				{
					ValidateNested(jsonValidation);
				}
			}

			_currentPath.Pop();
		}

		return this;
	}

	IJsonArrayResult IJsonArrayResult.IJsonArrayLengthResult.Elements()
	{
		int? amount = _amount;
		_expectationBuilder.Add((sb, grammars) => sb.Append(grammars.IsNegated() ? " or not with " : " with ")
			.Append(amount).Append(amount == 1 ? " element" : " elements"));
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
		int? amount = _amount;
		_expectationBuilder.Add((sb, grammars) => sb.Append(grammars.IsNegated() ? " or not with " : " with ")
			.Append(amount).Append(amount == 1 ? " property" : " properties"));
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
		if (propertyName is null)
		{
			throw Own(Tracing.WriteException(
				new ArgumentNullException(nameof(propertyName), "The 'propertyName' cannot be null.")));
		}

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
		string currentPath = CurrentPath;
		_expectationBuilder.Add((sb, grammars) => sb.Append(And(grammars)).Append(currentPath)
			.Append(grammars.IsNegated() ? " does not match " : " matches ")
			.Append(doNotPopulateThisValue.TrimCommonWhiteSpace()));
		JsonElement? currentElement = _currentElements.Pop();
		if (currentElement == null)
		{
			_currentPath.Pop();
			return this;
		}

		CompareElement(currentElement.Value, expected);
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
		string currentPath = CurrentPath;
		_expectationBuilder.Add((sb, grammars) => sb.Append(And(grammars)).Append(currentPath)
			.Append(grammars.IsNegated() ? " does not match " : " matches ")
			.Append(doNotPopulateThisValue.TrimCommonWhiteSpace()));
		JsonElement? currentElement = _currentElements.Pop();
		if (currentElement == null)
		{
			_currentPath.Pop();
			return this;
		}

		CompareElement(currentElement.Value, expected);
		_currentPath.Pop();
		return this;
	}

	private static string And(ExpectationGrammars grammars) => grammars.IsNegated() ? " or " : " and ";

	private JsonValidation An(JsonValueKind kind, Action<JsonValidation> expectation)
	{
		string currentPath = CurrentPath;
		_expectationBuilder.Add((sb, grammars) => sb.Append(And(grammars)).Append(currentPath).Append(' '));
		JsonElement? currentElement = _currentElements.Pop();

		JsonValidation jsonValidation = new(_root, CurrentPath, currentElement, kind, _options, _context);
		_expectationBuilder.Add(jsonValidation.GetExpectation);
		expectation.Invoke(jsonValidation);

		if (currentElement != null)
		{
			if (currentElement.Value.ValueKind != kind)
			{
				_failures.Add($" {CurrentPath} was {Format(currentElement.Value.ValueKind)}");
			}
			else
			{
				ValidateNested(jsonValidation);
			}
		}

		_currentPath.Pop();
		return this;
	}

	private JsonValidation An(JsonValueKind kind)
	{
		string currentPath = CurrentPath;
		_expectationBuilder.Add((sb, grammars)
			=> sb.Append(And(grammars)).Append(currentPath).Append(" is ").Append(Format(kind)));
		JsonElement? currentElement = _currentElements.Pop();

		if (currentElement != null && currentElement.Value.ValueKind != kind)
		{
			_failures.Add($" {CurrentPath} was {Format(currentElement.Value.ValueKind)}");
		}

		_currentPath.Pop();
		return this;
	}

	/// <summary>
	///     Pops the current element, or <see langword="null" /> when it is no array, which is already reported where its
	///     kind is verified.
	/// </summary>
	private JsonElement? PopArray()
	{
		JsonElement? element = _currentElements.Pop();
		return element is { ValueKind: JsonValueKind.Array, } ? element : null;
	}

	/// <summary>
	///     Remembers the <paramref name="exception" /> as thrown by the validation itself and not by the expectation of
	///     the caller, so that it is thrown as it is.
	/// </summary>
	private TException Own<TException>(TException exception) where TException : Exception
	{
		_root._ownException = exception;
		return exception;
	}

	/// <summary>
	///     Calls the <paramref name="expectation" /> of the caller on this array.
	/// </summary>
	public void Invoke(Func<IJsonArrayResult, IJsonArrayResult> expectation)
		=> Invoke(expectation, this);

	/// <summary>
	///     Calls the <paramref name="expectation" /> of the caller on this object.
	/// </summary>
	public void Invoke(Func<IJsonObjectResult, IJsonObjectResult> expectation)
		=> Invoke(expectation, this);

	/// <remarks>
	///     An exception of the <paramref name="expectation" /> fails the expectation and its negation alike, but an
	///     exception that the validation throws itself, e.g. for an invalid argument, is thrown as it is.
	/// </remarks>
	private void Invoke<TResult>(Func<TResult, TResult> expectation, TResult result)
	{
		try
		{
			UserCode.Invoke(expectation, result, "the expectation");
		}
		catch (Exception) when (_ownException is not null)
		{
			ExceptionDispatchInfo.Capture(_ownException).Throw();
		}
	}

	/// <summary>
	///     Makes the checks that the expectation of the caller specified, after it was invoked.
	/// </summary>
	public async Task ValidateAsync(CancellationToken cancellationToken)
	{
		foreach (Func<CancellationToken, Task> pendingCheck in _pendingChecks)
		{
			await pendingCheck(cancellationToken);
		}

		_failures.RemoveAll(failure => failure is null);
	}

	public bool IsMet()
		=> _element is not null && _element.Value.ValueKind == _valueKind && _failures.Count == 0;

	public void GetExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars)
	{
		foreach (Action<StringBuilder, ExpectationGrammars>? callback in _expectationBuilder)
		{
			callback(stringBuilder, grammars);
		}
	}

	/// <summary>
	///     The expectation on a nested element, whose path is the subject of the sentence instead of the subject of the
	///     expectation, so it is always singular.
	/// </summary>
	private void GetNestedExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars)
		=> GetExpectation(stringBuilder, grammars & ~ExpectationGrammars.Plural);

	public string GetFailure(string it, string? indentation)
	{
		if (_element is null)
		{
			return $"{it} was <null>";
		}

		if (_element.Value.ValueKind != _valueKind)
		{
			return $"{it} was {Format(_element.Value.ValueKind)}";
		}

		return $"{it} differed as{(_failures.Count > 1 ? Environment.NewLine + " " : "")}{GetFailures()}"
			.IndentFollowingLines(indentation);
	}

	private string GetFailures()
	{
		string failureSeparator = " and" + Environment.NewLine + " ";
		return string.Join(failureSeparator, _failures);
	}

	internal static string Format(JsonValueKind valueKind, ExpectationGrammars grammars = ExpectationGrammars.None)
	{
		string kind = (valueKind, grammars.IsPlural()) switch
		{
			(JsonValueKind.Array, false) => "an array",
			(JsonValueKind.Object, false) => "an object",
			(JsonValueKind.Number, false) => "a number",
			(JsonValueKind.String, false) => "a string",
			(JsonValueKind.Array, true) => "arrays",
			(JsonValueKind.Object, true) => "objects",
			(JsonValueKind.Number, true) => "numbers",
			(JsonValueKind.String, true) => "strings",
			_ => valueKind.ToString().ToLower(),
		};
		return grammars.IsNegated() ? "not " + kind : kind;
	}
}
