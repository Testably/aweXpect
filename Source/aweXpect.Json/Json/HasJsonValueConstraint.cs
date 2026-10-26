using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Json.Options;

namespace aweXpect.Json;

/// <summary>
///     Verifies that the subject has the value of the <paramref name="target" />.
/// </summary>
/// <remarks>
///     A subject or a value on the way to the target, which has another kind than the next segment requires, fails the
///     expectation and its negation alike, as the target cannot be looked up in it.
/// </remarks>
internal sealed class HasJsonValueConstraint(
	string it,
	ExpectationGrammars grammars,
	JsonValueTarget target,
	IReadOnlyOptionsHolder<JsonLookupOptions>? lookupOptions)
	: ConstraintResult.WithNotNullValue<JsonElement?>(it, grammars),
		IValueConstraint<JsonElement>,
		IValueConstraint<JsonElement?>,
		IJsonNodeProjection
{
	private JsonElement _failedValue;
	private FailureKind _failure;
	private int _failedSegment;

	/// <summary>
	///     The <see cref="FoundJsonValue{TValue}" /> of the <see cref="JsonElement" /> that was found, or the
	///     <see cref="NodeDecision" /> of an expectation that was decided on the node.
	/// </summary>
	/// <remarks>
	///     It is a single field, so that a constraint on a <see cref="JsonElement" /> allocates nothing for nodes.
	/// </remarks>
	private object? _found;

	private bool TreatNullAsAbsent => lookupOptions?.Value.TreatNullAsAbsent == true;

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

	/// <summary>
	///     Tries to verify the <paramref name="node" /> itself, without converting it to a <see cref="JsonElement" />.
	/// </summary>
	/// <remarks>
	///     It only decides, when the value is found in objects and arrays that look it up like their
	///     <see cref="JsonElement" /> does, and the expectation is met in its current negation, as a failure needs the
	///     converted subject for its message. Otherwise the caller verifies the converted subject, e.g. for a value
	///     within a <see cref="JsonValue" />, case-insensitive property names or duplicate property names, which the
	///     <see cref="JsonElement" /> keeps while the <see cref="JsonObject" /> throws.
	///     <para />
	///     A later negation, e.g. of <c>DoesNotComplyWith</c>, converts the subject for the failure message. Until then,
	///     the actual value is an undefined element, as no value would be a <see langword="null" /> subject.
	/// </remarks>
	public bool TryIsMetBy(JsonNode? node)
	{
		NodeLookup lookup = Find(node, out JsonNode? found, out int missingSegment);
		if (lookup == NodeLookup.Unknown || (lookup == NodeLookup.Found) == IsNegated)
		{
			return false;
		}

		Actual = default(JsonElement);
		_found = new NodeDecision(node, found);
		_failedSegment = missingSegment;
		_failure = lookup == NodeLookup.Found ? FailureKind.None : FailureKind.Missing;
		Outcome = lookup == NodeLookup.Found ? Outcome.Success : Outcome.Failure;
		return true;
	}

	/// <remarks>
	///     A <see cref="JsonValue" /> can also represent an object or an array, e.g. one that holds a .NET object, whose
	///     values are no nodes of their own, so that the found value is parsed as a new node then.
	/// </remarks>
	public bool TryProject<TValue>(JsonNode? node, out TValue? value)
	{
		if (typeof(TValue) != typeof(FoundJsonValue<JsonNode?>) || !TryGetFoundNode(node, out JsonNode? found))
		{
			value = default;
			return false;
		}

		value = (TValue)(object)new FoundJsonValue<JsonNode?>(found);
		return true;
	}

	private bool TryGetFoundNode(JsonNode? node, out JsonNode? found)
	{
		if (_found is NodeDecision decision)
		{
			found = decision.Found;
			return _failure == FailureKind.None;
		}

		if (_found is not FoundJsonValue<JsonElement> foundElement)
		{
			found = null;
			return false;
		}

		found = node;
		foreach (JsonPathSegment segment in target.Segments)
		{
			if (found is JsonObject jsonObject && segment.PropertyName is not null)
			{
				found = jsonObject[segment.PropertyName];
			}
			else if (found is JsonArray jsonArray && segment.PropertyName is null)
			{
				found = jsonArray[segment.Index];
			}
			else
			{
				found = JsonNode.Parse(foundElement.Value.GetRawText());
				break;
			}
		}

		return true;
	}

	/// <remarks>
	///     Stores the found value, so that the expectations on it are only verified when it exists.
	/// </remarks>
	public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
	{
		if (_found is FoundJsonValue<JsonElement> and TValue found)
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

		ConvertDecidedNode();
		ThatJsonElement.AddActualContext(contexts, Actual);
		lookupOptions?.Value.AddContext(contexts);
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
		ConvertDecidedNode();
		if (_found is not FoundJsonValue<JsonElement> found)
		{
			AppendNormalResult(stringBuilder, indentation);
			return;
		}

		target.AppendDescription(stringBuilder.Append(It).Append(" had "));
		stringBuilder.Append(" with ").Append(JsonElementValidator.Format(found.Value));
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
		       !(TreatNullAsAbsent && value.ValueKind == JsonValueKind.Null);
	}

	/// <summary>
	///     Converts the subject and the found value of an expectation that was decided on the node, as its failure
	///     message shows them.
	/// </summary>
	private void ConvertDecidedNode()
	{
		if (_found is not NodeDecision decision)
		{
			return;
		}

		Actual = ThatJsonNode.ToJsonElement(decision.Subject);
		_found = _failure == FailureKind.None
			? new FoundJsonValue<JsonElement>(ThatJsonNode.ToJsonElement(decision.Found))
			: null;
	}

	private NodeLookup Find(JsonNode? node, out JsonNode? found, out int missingSegment)
	{
		found = node;
		for (int i = 0; i < target.Segments.Count; i++)
		{
			JsonPathSegment segment = target.Segments[i];
			NodeLookup lookup = segment.PropertyName is null
				? FindElement(found, segment.Index, out found)
				: FindProperty(found, segment.PropertyName, out found);
			if (lookup != NodeLookup.Found)
			{
				missingSegment = i;
				return lookup;
			}
		}

		missingSegment = 0;
		return NodeLookup.Found;
	}

	private static NodeLookup FindElement(JsonNode? current, int index, out JsonNode? value)
	{
		value = null;
		if (current is not JsonArray jsonArray)
		{
			return NodeLookup.Unknown;
		}

		if (index >= jsonArray.Count)
		{
			return NodeLookup.Missing;
		}

		value = jsonArray[index];
		return NodeLookup.Found;
	}

	/// <remarks>
	///     The property is looked up only where the <see cref="JsonObject" /> finds it like its
	///     <see cref="JsonElement" />: with case-sensitive names and without duplicate names, which the
	///     <see cref="JsonObject" /> rejects when it reads its properties.
	/// </remarks>
	private NodeLookup FindProperty(JsonNode? current, string name, out JsonNode? value)
	{
		value = null;
		if (current is not JsonObject jsonObject || jsonObject.Options?.PropertyNameCaseInsensitive == true)
		{
			return NodeLookup.Unknown;
		}

		try
		{
			if (!jsonObject.TryGetPropertyValue(name, out value))
			{
				return NodeLookup.Missing;
			}
		}
		catch (ArgumentException)
		{
			return NodeLookup.Unknown;
		}

		return TreatNullAsAbsent && (value is null || value.GetValueKind() == JsonValueKind.Null)
			? NodeLookup.Missing
			: NodeLookup.Found;
	}

	/// <summary>
	///     The <paramref name="subject" /> and the <paramref name="found" /> value of an expectation that was decided on
	///     the node, which are converted only for a failure message.
	/// </summary>
	private sealed class NodeDecision(JsonNode? subject, JsonNode? found)
	{
		public JsonNode? Subject { get; } = subject;
		public JsonNode? Found { get; } = found;
	}

	private enum FailureKind
	{
		None,
		WrongKind,
		Missing,
	}

	private enum NodeLookup
	{
		Unknown,
		Found,
		Missing,
	}
}
