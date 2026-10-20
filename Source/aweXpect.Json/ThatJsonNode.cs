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
	///     The node is written instead of serialized, which would require reflection, and the element owns its memory,
	///     so that it outlives the evaluation, e.g. for the failure message.
	/// </remarks>
	private static JsonElement ToJsonElement(JsonNode? node)
	{
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
			JsonElement element = ToJsonElement(actual);
			if (inner is IAsyncContextConstraint<JsonElement> asyncConstraint)
			{
				await asyncConstraint.IsMetBy(element, context, cancellationToken);
			}
			else
			{
				((IValueConstraint<JsonElement>)inner).IsMetBy(element);
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

			value = default;
			return typeof(TValue).IsAssignableFrom(typeof(TNode));
		}

		public override ConstraintResult Negate()
		{
			inner.Negate();
			Grammars = inner.Grammars;
			return this;
		}
	}
}
