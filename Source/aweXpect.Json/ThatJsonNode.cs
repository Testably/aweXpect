using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Json;

namespace aweXpect;

/// <summary>
///     Expectations on <see cref="JsonNode" /> values.
/// </summary>
/// <remarks>
///     A <see langword="null" /> node is the JSON <c>null</c>, as <c>JsonNode.Parse("null")</c> returns it.
/// </remarks>
public static partial class ThatJsonNode
{
	/// <summary>
	///     Converts the <paramref name="node" /> in its current state to a <see cref="JsonElement" />.
	/// </summary>
	/// <remarks>
	///     A <see cref="JsonValue" /> that holds a <see cref="JsonElement" />, like every value of parsed JSON, returns
	///     it as is, as writing and parsing it again would allocate a new document for every evaluation.
	///     <para />
	///     Any other node is written instead of serialized, which would require reflection, and the element owns its
	///     memory, so that it outlives the evaluation, e.g. for the failure message.
	/// </remarks>
	internal static JsonElement ToJsonElement(JsonNode? node)
	{
		if (node is JsonValue value && value.TryGetValue(out JsonElement element))
		{
			return element;
		}

		using MemoryStream stream = new();
		using (Utf8JsonWriter writer = new(stream))
		{
			if (node is null)
			{
				writer.WriteNullValue();
			}
			else
			{
				node.WriteTo(writer);
			}
		}

		Utf8JsonReader reader = new(new ReadOnlySpan<byte>(stream.GetBuffer(), 0, (int)stream.Length));
		return JsonElement.ParseValue(ref reader);
	}

	private static JsonNode? ToJsonNode(JsonElement element)
		=> JsonNode.Parse(element.GetRawText());

	/// <inheritdoc cref="ToJsonElement(JsonNode?)" />
	/// <remarks>
	///     A root object or array is converted once per evaluation in the <paramref name="context" />, as every
	///     expectation on it, e.g. each <c>HasProperty</c> combined with <c>And</c>, would otherwise write and parse it
	///     again. A node within another one is mostly the value that an enclosing expectation found, whose nested
	///     evaluation would allocate more for a stored value than a single conversion costs.
	/// </remarks>
	private static JsonElement ToJsonElement(JsonNode? node, IEvaluationContext context)
	{
		if (node is not (JsonObject or JsonArray) || node.Parent is not null)
		{
			return ToJsonElement(node);
		}

		if (context.TryReceive(ConvertedNode.Key, out ConvertedNode? converted) &&
		    ReferenceEquals(converted.Node, node))
		{
			return converted.Element;
		}

		JsonElement element = ToJsonElement(node);
		context.Store(ConvertedNode.Key, new ConvertedNode(node, element));
		return element;
	}

	/// <summary>
	///     The <paramref name="node" /> that was last converted in an evaluation, with its <paramref name="element" />.
	/// </summary>
	private sealed class ConvertedNode(JsonNode node, JsonElement element)
	{
		public const string Key = "aweXpect.Json." + nameof(ConvertedNode);

		public JsonNode Node { get; } = node;
		public JsonElement Element { get; } = element;
	}

	/// <summary>
	///     Verifies the <see cref="JsonElement" /> that the <typeparamref name="TNode" /> represents with the
	///     <paramref name="inner" /> constraint, while the node remains the value of the expectation.
	/// </summary>
	private sealed class JsonNodeConstraint<TNode>(ConstraintResult inner)
		: ConstraintResult(inner.Grammars),
			IAsyncContextConstraint<TNode?>
		where TNode : JsonNode
	{
		private TNode? _actual;

		public override Outcome Outcome => inner.Outcome;

		public override Exception? FailureCause => inner.FailureCause;

		public override string? LeadingSubject => inner.LeadingSubject;

		public override string? TrailingSubject => inner.TrailingSubject;

		public async ValueTask<ConstraintResult> IsMetBy(TNode? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_actual = actual;
			if (inner is HasJsonValueConstraint lookup && lookup.TryIsMetBy(actual))
			{
				return this;
			}

			JsonElement element = ToJsonElement(actual, context);
			switch (inner)
			{
				case IValueConstraint<JsonElement> valueConstraint:
					valueConstraint.IsMetBy(element);
					break;
				case IContextConstraint<JsonElement> contextConstraint:
					contextConstraint.IsMetBy(element, context);
					break;
				case IAsyncConstraint<JsonElement> asyncConstraint:
					await asyncConstraint.IsMetBy(element, cancellationToken);
					break;
				case IAsyncContextConstraint<JsonElement> asyncContextConstraint:
					await asyncContextConstraint.IsMetBy(element, context, cancellationToken);
					break;
				default:
					throw new NotSupportedException(
						$"The constraint {inner.GetType().Name} does not verify a {nameof(JsonElement)}.");
			}

			return this;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> inner.AppendExpectation(stringBuilder, indentation);

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
			=> inner.AppendResult(stringBuilder, indentation);

		public override void AppendContexts(ResultContextCollector contexts)
			=> contexts.Visit(inner);

		public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
		{
			if (_actual is TValue typedValue)
			{
				value = typedValue;
				return true;
			}

			if (inner is IJsonNodeProjection projection && projection.TryProject(_actual, out value))
			{
				return true;
			}

			if (inner.TryGetStoredValue(out TValue? innerValue) && innerValue is IFoundJsonValue)
			{
				value = innerValue;
				return true;
			}

			value = default;
			return typeof(TValue).IsAssignableFrom(typeof(TNode));
		}

		protected override void NegateInPlace()
		{
			inner.Invert();
			Grammars = inner.Grammars;
		}
	}
}
