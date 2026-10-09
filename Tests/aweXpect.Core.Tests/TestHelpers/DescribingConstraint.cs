using System.Text;
using aweXpect.Core.Constraints;

namespace aweXpect.Core.Tests.TestHelpers;

/// <summary>
///     A failing constraint that describes the subject as <paramref name="description" />, or throws the
///     <paramref name="exception" /> instead, when it is set.
/// </summary>
internal sealed class DescribingConstraint<T>(string description, Exception? exception = null)
	: ConstraintResult.ExpectationOnly<T>(ExpectationGrammars.None, "is described", "is not described"),
		IValueConstraint<T>
{
	ConstraintResult IValueConstraint<T>.IsMetBy(T actual)
	{
		if (exception is not null)
		{
			throw exception;
		}

		Outcome = Outcome.Failure;
		return this;
	}

	public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append("it was not described");

	public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
	{
		if (new Description(description) is TValue describableSubject)
		{
			value = describableSubject;
			return true;
		}

		value = default;
		return false;
	}

	private sealed class Description(string description) : IDescribableSubject
	{
		public string GetDescription() => description;
	}
}
