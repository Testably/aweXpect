using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;

namespace aweXpect.Json;

/// <summary>
///     Verifies that the subject is a JSON value of the kind of the <paramref name="reader" />, and when an operator on
///     the result handed a comparison to it, that its read value satisfies the comparison.
/// </summary>
/// <remarks>
///     Without a comparison, the value is not read, so that e.g. every number is a number, also one that does not fit
///     into a <see langword="double" />. A value of another kind, or one that the <paramref name="reader" /> cannot read
///     for the comparison, fails the expectation and satisfies its negation, like the expectations on the kind.
/// </remarks>
internal abstract class IsJsonValueKindConstraint<TValue>(
	string it,
	ExpectationGrammars grammars,
	JsonValueReader<TValue> reader)
	: ConstraintResult.WithNotNullValue<JsonElement?>(it, grammars),
		IAsyncContextConstraint<JsonElement>,
		IAsyncContextConstraint<JsonElement?>
{
	private bool _doesNotFit;

	/// <summary>
	///     Whether the value was read and compared.
	/// </summary>
	protected bool IsCompared { get; private set; }

	/// <summary>
	///     Whether an operator handed a comparison to the constraint.
	/// </summary>
	protected abstract bool HasComparison { get; }

	public ValueTask<ConstraintResult> IsMetBy(JsonElement actual, IEvaluationContext context,
		CancellationToken cancellationToken)
		=> IsMetBy((JsonElement?)actual, context, cancellationToken);

	public async ValueTask<ConstraintResult> IsMetBy(JsonElement? actual, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		Actual = actual;
		IsCompared = false;
		_doesNotFit = false;
		if (actual is null || !JsonValidation.IsKind(actual.Value.ValueKind, reader.Kind))
		{
			Outcome = Outcome.Failure;
			return this;
		}

		if (!HasComparison)
		{
			Outcome = Outcome.Success;
			return this;
		}

		if (!reader.Read(actual.Value, out TValue value))
		{
			_doesNotFit = true;
			Outcome = Outcome.Failure;
			return this;
		}

		IsCompared = true;
		Outcome = await Compare(value, context, cancellationToken);
		return this;
	}

	/// <summary>
	///     Compares the read <paramref name="value" /> with the comparison of the operator.
	/// </summary>
	protected abstract ValueTask<Outcome> Compare(TValue value, IEvaluationContext context,
		CancellationToken cancellationToken);

	/// <summary>
	///     Appends the comparison of the operator, e.g. <c>starting with "Let"</c>.
	/// </summary>
	protected abstract void AppendComparison(StringBuilder stringBuilder);

	/// <summary>
	///     Appends the result for a compared value that does not satisfy the comparison.
	/// </summary>
	protected abstract void AppendComparedResult(StringBuilder stringBuilder, string? indentation);

	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> AppendExpectation(stringBuilder);

	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
	{
		if (IsCompared)
		{
			AppendComparedResult(stringBuilder, indentation);
		}
		else if (_doesNotFit)
		{
			AppendActualValue(stringBuilder);
			stringBuilder.Append(", which is not ").Append(reader.TypeName);
		}
		else
		{
			stringBuilder.Append(ThatJsonElement.DescribeValueKind(Actual!.Value, It, Grammars));
		}
	}

	protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> AppendExpectation(stringBuilder);

	protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		=> AppendNormalResult(stringBuilder, indentation);

	/// <summary>
	///     Appends the value as it is written in the JSON document, e.g. <c>it was 2.5</c>.
	/// </summary>
	protected void AppendActualValue(StringBuilder stringBuilder)
		=> stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "))
			.Append(JsonElementValidator.Format(Actual!.Value));

	private void AppendExpectation(StringBuilder stringBuilder)
	{
		stringBuilder.Append(Grammars.Verb("is ", "are "));
		if (!HasComparison)
		{
			stringBuilder.Append(JsonValidation.FormatExpected(reader.Kind, Grammars & ~ExpectationGrammars.Plural));
			return;
		}

		stringBuilder.Append(IsNegated ? "not " : "")
			.Append(JsonValidation.FormatExpected(reader.Kind, ExpectationGrammars.None))
			.Append(' ');
		AppendComparison(stringBuilder);
	}
}
