using System.Text;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Core.Constraints;

namespace aweXpect.Json;

/// <summary>
///     Verifies that the subject is a JSON value of the kind of the <paramref name="reader" /> that it can read, and
///     stores the read value for the nested expectations.
/// </summary>
internal sealed class IsJsonValueConstraint<TValue>(
	string it,
	ExpectationGrammars grammars,
	JsonValueReader<TValue> reader)
	: ConstraintResult.WithNotNullValue<JsonElement?>(it, grammars),
		IValueConstraint<JsonElement>,
		IValueConstraint<JsonElement?>
{
	private bool _doesNotFit;
	private FoundJsonValue<TValue>? _found;

	public ConstraintResult IsMetBy(JsonElement actual)
		=> IsMetBy((JsonElement?)actual);

	public ConstraintResult IsMetBy(JsonElement? actual)
	{
		Actual = actual;
		_found = null;
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

		_found = new FoundJsonValue<TValue>(value);
		Outcome = Outcome.Success;
		return this;
	}

	/// <remarks>
	///     Stores the read value, so that the expectations on it are only verified when it could be read.
	/// </remarks>
	public override bool TryGetStoredValue<T>(out T? value) where T : default
	{
		if (_found is T found)
		{
			value = found;
			return true;
		}

		return base.TryGetStoredValue(out value);
	}

	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> AppendExpected(stringBuilder.Append(Grammars.Verb("is ", "are ")));

	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
	{
		if (_doesNotFit)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "))
				.Append(JsonElementValidator.Format(Actual!.Value)).Append(", which is not ").Append(reader.TypeName);
		}
		else
		{
			stringBuilder.Append(ThatJsonElement.DescribeValueKind(Actual!.Value, It, Grammars));
		}
	}

	protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> AppendExpected(stringBuilder.Append(Grammars.Verb("is ", "are ")));

	protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
	{
		if (reader.Description is null)
		{
			stringBuilder.Append(ThatJsonElement.DescribeValueKind(Actual!.Value, It, Grammars));
		}
		else
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "))
				.Append(JsonElementValidator.Format(Actual!.Value));
		}
	}

	private void AppendExpected(StringBuilder stringBuilder)
	{
		if (reader.Description is null)
		{
			stringBuilder.Append(JsonValidation.FormatExpected(reader.Kind, Grammars & ~ExpectationGrammars.Plural));
		}
		else
		{
			stringBuilder.Append(Grammars.IsNegated() ? "not " : "").Append(reader.Description);
		}
	}
}
