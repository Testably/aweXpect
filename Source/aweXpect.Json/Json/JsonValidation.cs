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
using aweXpect.Core.Extending;

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
	private bool _isAbsentProperty;
	private Exception? _ownException;
	private Dictionary<string, string?>? _unmatchedMembersToIgnore;

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

	/// <summary>
	///     The members to ignore by name that matched no member in a failed comparison of the whole validation, with the
	///     path each one most likely meant.
	/// </summary>
	public IReadOnlyDictionary<string, string?>? UnmatchedMembersToIgnore => _root._unmatchedMembersToIgnore;

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
		_currentElements.Push(GetElement(_currentElements.Peek(), index, CurrentPath));
		return this;
	}

	private JsonElement? GetElement(JsonElement? array, int index, string path)
	{
		if (array is not { ValueKind: JsonValueKind.Array, })
		{
			return null;
		}

		if (array.Value.GetArrayLength() > index)
		{
			return array.Value[index];
		}

		_failures.Add($" index {path} did not exist");
		return null;
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
			using JsonDocument expectedDocument = converter.ParseExpected(expectedValue, _options.DocumentOptions,
				_options.ExpectedSerializerOptions);
			JsonElementValidator.JsonComparisonResult comparisonResult = await JsonElementValidator.Compare(
				path,
				element,
				expectedDocument.RootElement,
				_options,
				converter);
			_root._unmatchedMembersToIgnore = MembersToIgnoreTracker.Keep(_root._unmatchedMembersToIgnore,
				comparisonResult.GetUnmatchedMembersToIgnore());
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

		int actualLength = currentElement.Value.EnumerateObject().Count(property => !IsAbsent(property.Value));
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
		_currentElements.Push(GetProperty(_currentElements.Peek(), propertyName, CurrentPath));
		return this;
	}

	IJsonPropertyResult<IJsonObjectResult> IJsonObjectResult.At(string path)
	{
		IReadOnlyList<JsonPathSegment> segments;
		try
		{
			segments = JsonPathSegment.Parse(path);
		}
		catch (ArgumentException exception)
		{
			Own(exception);
			throw;
		}

		StringBuilder pathBuilder = new(CurrentPath);
		JsonElement? currentElement = _currentElements.Peek();
		for (int i = 0; i < segments.Count - 1; i++)
		{
			string segmentPath = pathBuilder.Append(segments[i]).ToString();
			JsonValueKind kind = segments[i + 1].PropertyName is null ? JsonValueKind.Array : JsonValueKind.Object;
			currentElement = RequireKind(GetSegment(currentElement, segments[i], segmentPath), kind, segmentPath);
		}

		_currentPath.Push(string.Concat(segments));
		_currentElements.Push(GetSegment(currentElement, segments[segments.Count - 1], CurrentPath));
		return this;
	}

	private JsonElement? GetSegment(JsonElement? currentElement, JsonPathSegment segment, string path)
		=> segment.PropertyName is null
			? GetElement(currentElement, segment.Index, path)
			: GetProperty(currentElement, segment.PropertyName, path);

	private JsonElement? GetProperty(JsonElement? jsonObject, string propertyName, string path)
	{
		if (jsonObject == null)
		{
			return null;
		}

		if (jsonObject.Value.ValueKind != JsonValueKind.Object)
		{
			_failures.Add($" {path} was {Format(jsonObject.Value.ValueKind)} instead of an object");
			return null;
		}

		if (jsonObject.Value.TryGetProperty(propertyName, out JsonElement propertyValue) && !IsAbsent(propertyValue))
		{
			return propertyValue;
		}

		if (_options.TreatNullAsAbsent)
		{
			_isAbsentProperty = true;
			return null;
		}

		_failures.Add($" property {path} did not exist");
		return null;
	}

	/// <summary>
	///     Verifies that the <paramref name="element" /> in the middle of a path has the <paramref name="kind" /> that its
	///     next segment requires, like <c>AnObject</c> or <c>AnArray</c> would, so that only the first segment that cannot
	///     be reached is reported.
	/// </summary>
	private JsonElement? RequireKind(JsonElement? element, JsonValueKind kind, string path)
	{
		if (_isAbsentProperty)
		{
			_isAbsentProperty = false;
			_failures.Add($" property {path} did not exist");
			return null;
		}

		if (element != null && element.Value.ValueKind != kind)
		{
			_failures.Add($" {path} was {Format(element.Value.ValueKind)}");
			return null;
		}

		return element;
	}

	IJsonObjectResult IJsonObjectResult.Without(string propertyName)
	{
		if (propertyName is null)
		{
			throw Own(Tracing.WriteException(
				new ArgumentNullException(nameof(propertyName), "The 'propertyName' cannot be null.")));
		}

		string currentPath = $"{CurrentPath}.{propertyName}";
		_expectationBuilder.Add((sb, grammars) => sb.Append(And(grammars)).Append(currentPath)
			.Append(grammars.IsNegated() ? " exists" : " does not exist"));
		JsonElement? currentElement = _currentElements.Peek();
		if (currentElement is { ValueKind: JsonValueKind.Object, } &&
		    currentElement.Value.TryGetProperty(propertyName, out JsonElement propertyValue) &&
		    !IsAbsent(propertyValue))
		{
			_failures.Add($" {currentPath} was {JsonElementValidator.Format(propertyValue)}");
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

	IJsonArrayResult IJsonPropertyResult<IJsonArrayResult>.AString()
		=> An(JsonValueKind.String);

	IJsonArrayResult IJsonPropertyResult<IJsonArrayResult>.ANumber()
		=> An(JsonValueKind.Number);

	IJsonArrayResult IJsonPropertyResult<IJsonArrayResult>.ABoolean()
		=> An(JsonValueKind.True);

	IJsonArrayResult IJsonPropertyResult<IJsonArrayResult>.Null()
		=> An(JsonValueKind.Null);

	IJsonObjectResult IJsonPropertyResult<IJsonObjectResult>.AnArray()
		=> An(JsonValueKind.Array);

	IJsonObjectResult IJsonPropertyResult<IJsonObjectResult>.AnArray(Action<IJsonArrayResult> expectation)
		=> An(JsonValueKind.Array, expectation);

	IJsonObjectResult IJsonPropertyResult<IJsonObjectResult>.AnObject()
		=> An(JsonValueKind.Object);

	IJsonObjectResult IJsonPropertyResult<IJsonObjectResult>.AnObject(Action<IJsonObjectResult> expectation)
		=> An(JsonValueKind.Object, expectation);

	IJsonObjectResult IJsonPropertyResult<IJsonObjectResult>.AString()
		=> An(JsonValueKind.String);

	IJsonObjectResult IJsonPropertyResult<IJsonObjectResult>.ANumber()
		=> An(JsonValueKind.Number);

	IJsonObjectResult IJsonPropertyResult<IJsonObjectResult>.ABoolean()
		=> An(JsonValueKind.True);

	IJsonObjectResult IJsonPropertyResult<IJsonObjectResult>.Null()
		=> An(JsonValueKind.Null);

	IJsonObjectResult IJsonPropertyResult<IJsonObjectResult>.Matching(object? expected, string doNotPopulateThisValue)
	{
		string currentPath = CurrentPath;
		_expectationBuilder.Add((sb, grammars) => sb.Append(And(grammars)).Append(currentPath)
			.Append(grammars.IsNegated() ? " does not match " : " matches ")
			.Append(doNotPopulateThisValue.TrimCommonWhiteSpace()));
		JsonElement? currentElement = PopProperty(expected is null);
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
		JsonElement? currentElement = PopProperty();

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
			=> sb.Append(And(grammars)).Append(currentPath).Append(" is ")
				.Append(FormatExpected(kind, grammars & ~ExpectationGrammars.Plural)));
		JsonElement? currentElement = PopProperty(kind == JsonValueKind.Null);

		if (currentElement != null && !IsKind(currentElement.Value.ValueKind, kind))
		{
			_failures.Add($" {CurrentPath} was {Format(currentElement.Value.ValueKind)}");
		}

		_currentPath.Pop();
		return this;
	}

	private bool IsAbsent(JsonElement propertyValue)
		=> _options.TreatNullAsAbsent && propertyValue.ValueKind == JsonValueKind.Null;

	/// <summary>
	///     Pops the current element and reports a property that is absent, unless its absence is accepted.
	/// </summary>
	/// <remarks>
	///     When <see langword="null" /> is treated as absent, the absence of a property is only reported here, because an
	///     absent property is accepted when it is expected to be <see langword="null" />.
	/// </remarks>
	private JsonElement? PopProperty(bool isAbsenceAccepted = false)
	{
		if (_isAbsentProperty && !isAbsenceAccepted)
		{
			_failures.Add($" property {CurrentPath} did not exist");
		}

		_isAbsentProperty = false;
		return _currentElements.Pop();
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

	public string GetFailure(string it, ExpectationGrammars grammars, string? indentation)
	{
		if (_element is null)
		{
			return $"{it}{grammars.SubjectVerb(it, " was", " were")} <null>";
		}

		if (_element.Value.ValueKind != _valueKind)
		{
			return $"{it}{grammars.SubjectVerb(it, " was ", " were ")}{Format(_element.Value.ValueKind)}";
		}

		return $"{it} differed as{(_failures.Count > 1 ? Environment.NewLine + " " : "")}{GetFailures()}"
			.Indent(indentation, false);
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

	/// <summary>
	///     Checks if the <paramref name="actual" /> kind is the <paramref name="expected" /> kind, where an expected
	///     <see cref="JsonValueKind.True" /> stands for a boolean and is therefore also met by
	///     <see cref="JsonValueKind.False" />.
	/// </summary>
	internal static bool IsKind(JsonValueKind actual, JsonValueKind expected)
		=> actual == expected || (expected == JsonValueKind.True && actual == JsonValueKind.False);

	/// <summary>
	///     Formats the <paramref name="expected" /> kind, where <see cref="JsonValueKind.True" /> stands for a boolean.
	/// </summary>
	internal static string FormatExpected(JsonValueKind expected, ExpectationGrammars grammars)
	{
		if (expected != JsonValueKind.True)
		{
			return Format(expected, grammars);
		}

		string kind = grammars.IsPlural() ? "booleans" : "a boolean";
		return grammars.IsNegated() ? "not " + kind : kind;
	}
}
