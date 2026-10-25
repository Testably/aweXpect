using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;

namespace aweXpect.Json;

/// <summary>
///     Verifies that the subject is a JSON value of the kind of the <paramref name="reader" /> whose read value matches
///     the expected value.
/// </summary>
/// <remarks>
///     A value of another kind, or one that the <paramref name="reader" /> cannot read, fails the expectation and
///     satisfies its negation, like the expectations on the kind of the value.
/// </remarks>
internal abstract class IsJsonValueEqualToConstraint<TValue>(
	string it,
	ExpectationGrammars grammars,
	JsonValueReader<TValue> reader)
	: ConstraintResult.WithNotNullValue<JsonElement?>(it, grammars),
		IAsyncContextConstraint<JsonElement>,
		IAsyncContextConstraint<JsonElement?>
{
	private bool _doesNotFit;
	private bool _isCompared;

	public ValueTask<ConstraintResult> IsMetBy(JsonElement actual, IEvaluationContext context,
		CancellationToken cancellationToken)
		=> IsMetBy((JsonElement?)actual, context, cancellationToken);

	public async ValueTask<ConstraintResult> IsMetBy(JsonElement? actual, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		Actual = actual;
		_isCompared = false;
		_doesNotFit = false;
		if (actual is null || !JsonValidation.IsKind(actual.Value.ValueKind, reader.Kind))
		{
			Outcome = Outcome.Failure;
			return this;
		}

		if (!reader.Read(actual.Value, out TValue value))
		{
			_doesNotFit = true;
			Outcome = Outcome.Failure;
			return this;
		}

		_isCompared = true;
		Outcome = await IsMatch(value, context, cancellationToken) ? Outcome.Success : Outcome.Failure;
		return this;
	}

	/// <summary>
	///     Compares the read <paramref name="value" /> with the expected value.
	/// </summary>
	protected abstract ValueTask<bool> IsMatch(TValue value, IEvaluationContext context,
		CancellationToken cancellationToken);

	/// <summary>
	///     Appends the comparison with the expected value, e.g. <c>equal to 3</c>.
	/// </summary>
	protected abstract void AppendComparison(StringBuilder stringBuilder);

	/// <summary>
	///     Appends the result for a value that does not match the expected value.
	/// </summary>
	protected virtual void AppendMismatch(StringBuilder stringBuilder)
		=> AppendActualValue(stringBuilder);

	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> AppendExpectation(stringBuilder);

	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
	{
		if (_isCompared)
		{
			AppendMismatch(stringBuilder);
		}
		else
		{
			AppendOtherValue(stringBuilder);
		}
	}

	protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> AppendExpectation(stringBuilder);

	protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
	{
		if (_isCompared)
		{
			AppendActualValue(stringBuilder);
		}
		else
		{
			AppendOtherValue(stringBuilder);
		}
	}

	private void AppendExpectation(StringBuilder stringBuilder)
	{
		stringBuilder.Append(Grammars.Verb("is ", "are ")).Append(IsNegated ? "not " : "")
			.Append(reader.Description ?? JsonValidation.FormatExpected(reader.Kind, ExpectationGrammars.None))
			.Append(' ');
		AppendComparison(stringBuilder);
	}

	private void AppendActualValue(StringBuilder stringBuilder)
		=> stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "))
			.Append(JsonElementValidator.Format(Actual!.Value));

	private void AppendOtherValue(StringBuilder stringBuilder)
	{
		if (_doesNotFit)
		{
			AppendActualValue(stringBuilder);
			stringBuilder.Append(", which is not ").Append(reader.TypeName);
		}
		else
		{
			stringBuilder.Append(ThatJsonElement.DescribeValueKind(Actual!.Value, It, Grammars));
		}
	}
}
