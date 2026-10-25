using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using aweXpect.Core;
using aweXpect.Core.Constraints;

namespace aweXpect.Json;

/// <summary>
///     Verifies that the subject has the value of the <paramref name="target" />.
/// </summary>
/// <remarks>
///     A subject or a value on the way to the target, which has another kind than the next segment requires, fails the
///     expectation and its negation alike, as the target cannot be looked up in it.
/// </remarks>
internal sealed class HasJsonValueConstraint(string it, ExpectationGrammars grammars, JsonValueTarget target)
	: ConstraintResult.WithNotNullValue<JsonElement?>(it, grammars),
		IValueConstraint<JsonElement>,
		IValueConstraint<JsonElement?>,
		IJsonNodeProjection
{
	private JsonElement _failedValue;
	private FailureKind _failure;
	private int _failedSegment;
	private FoundJsonValue<JsonElement>? _found;

	public ConstraintResult IsMetBy(JsonElement actual)
		=> IsMetBy((JsonElement?)actual);

	public ConstraintResult IsMetBy(JsonElement? actual)
	{
		Actual = actual;
		_found = null;
		_failure = FailureKind.None;
		if (actual is null)
		{
			Outcome = Outcome.Failure;
			return this;
		}

		JsonElement current = actual.Value;
		for (int i = 0; i < target.Segments.Count; i++)
		{
			JsonPathSegment segment = target.Segments[i];
			JsonValueKind requiredKind = segment.PropertyName is null ? JsonValueKind.Array : JsonValueKind.Object;
			if (current.ValueKind != requiredKind)
			{
				return Fail(FailureKind.WrongKind, i, current);
			}

			if (!TryGetValue(current, segment, out JsonElement next))
			{
				return Fail(FailureKind.Missing, i, current);
			}

			current = next;
		}

		_found = new FoundJsonValue<JsonElement>(current);
		Outcome = Outcome.Success;
		return this;
	}

	/// <remarks>
	///     A <see cref="JsonValue" /> can also represent an object or an array, e.g. one that holds a .NET object, whose
	///     values are no nodes of their own, so that the found value is parsed as a new node then.
	/// </remarks>
	public bool TryProject<TValue>(JsonNode? node, out TValue? value)
	{
		if (_found is null || typeof(TValue) != typeof(FoundJsonValue<JsonNode?>))
		{
			value = default;
			return false;
		}

		JsonNode? current = node;
		foreach (JsonPathSegment segment in target.Segments)
		{
			if (current is JsonObject jsonObject && segment.PropertyName is not null)
			{
				current = jsonObject[segment.PropertyName];
			}
			else if (current is JsonArray jsonArray && segment.PropertyName is null)
			{
				current = jsonArray[segment.Index];
			}
			else
			{
				current = JsonNode.Parse(_found.Value.GetRawText());
				break;
			}
		}

		value = (TValue)(object)new FoundJsonValue<JsonNode?>(current);
		return true;
	}

	/// <remarks>
	///     Stores the found value, so that the expectations on it are only verified when it exists.
	/// </remarks>
	public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
	{
		if (_found is TValue found)
		{
			value = found;
			return true;
		}

		return base.TryGetStoredValue(out value);
	}

	public override void AppendContexts(ResultContextCollector contexts)
	{
		if (Outcome == Outcome.Success)
		{
			return;
		}

		ThatJsonElement.AddActualContext(contexts, Actual);
		if (target.TreatNullAsAbsent)
		{
			contexts.Add(new ResultContext.Fixed("JSON options", " - treat null as absent", int.MinValue));
		}
	}

	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> target.AppendDescription(stringBuilder.Append(Grammars.Verb("has ", "have ")));

	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
	{
		JsonPathSegment segment = target.Segments[_failedSegment];
		if (_failure == FailureKind.Missing)
		{
			stringBuilder.Append(segment.PropertyName is null ? "index " : "property ")
				.Append(target.GetPath(_failedSegment + 1)).Append(" did not exist");
		}
		else if (_failedSegment == 0)
		{
			stringBuilder.Append(ThatJsonElement.DescribeValueKind(_failedValue, It, Grammars));
		}
		else
		{
			stringBuilder.Append(target.GetPath(_failedSegment)).Append(" was ")
				.Append(JsonValidation.Format(_failedValue.ValueKind))
				.Append(segment.PropertyName is null ? " instead of an array" : " instead of an object");
		}
	}

	protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> target.AppendDescription(stringBuilder.Append(Grammars.Verb("does not have ", "do not have ")));

	protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
	{
		if (_found is null)
		{
			AppendNormalResult(stringBuilder, indentation);
			return;
		}

		target.AppendDescription(stringBuilder.Append(It).Append(" had "));
		stringBuilder.Append(" with ").Append(JsonElementValidator.Format(_found.Value));
	}

	private HasJsonValueConstraint Fail(FailureKind failure, int segment, JsonElement value)
	{
		_failure = failure;
		_failedSegment = segment;
		_failedValue = value;
		Outcome = failure == FailureKind.WrongKind ? Outcome.FailureBothWays : Outcome.Failure;
		return this;
	}

	private bool TryGetValue(JsonElement current, JsonPathSegment segment, out JsonElement value)
	{
		if (segment.PropertyName is null)
		{
			if (segment.Index < current.GetArrayLength())
			{
				value = current[segment.Index];
				return true;
			}

			value = default;
			return false;
		}

		return current.TryGetProperty(segment.PropertyName, out value) &&
		       !(target.TreatNullAsAbsent && value.ValueKind == JsonValueKind.Null);
	}

	private enum FailureKind
	{
		None,
		WrongKind,
		Missing,
	}
}
