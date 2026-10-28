using System;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;

namespace aweXpect.Helpers;

internal static class SingleValueExpectations
{
	/// <summary>
	///     Adds the <paramref name="expectations" /> on the single value of a header or query parameter, which fail both
	///     ways when it does not have exactly one value.
	/// </summary>
	/// <remarks>
	///     The values are the member, and the expectations on the value follow a check of their number, as the
	///     expectations on a member are evaluated even after the expectation in front of them failed, and would then see a
	///     value that does not exist.
	/// </remarks>
	public static void AddSingleValueExpectations<TType>(this ExpectationBuilder expectationBuilder,
		Func<TType, string?[]?> valuesAccessor,
		string valuesDescription,
		Action<IThatSubject<string?>> expectations)
		=> expectationBuilder
			.ForMember(MemberAccessor<TType, string?[]?>.FromFuncWithName(valuesAccessor, "the values "),
				static (_, stringBuilder) => stringBuilder.Append(" whose value "),
				false)
			.AddExpectations((valuesDescription, expectations), static (state, valuesBuilder) => valuesBuilder
				.AddConstraint(state.valuesDescription, static (valuesDescription, it, grammars)
					=> new SingleValueConstraint(it, grammars, valuesDescription))
				.And("")
				.ForMember(MemberAccessor<string?[]?, string?>.FromFuncWithName(static values => values![0],
						"the value "),
					static (_, _) => { })
				.AddExpectations(state.expectations));

	/// <summary>
	///     Fails both ways when there is not exactly one value, and stops the expectations on the value.
	/// </summary>
	private sealed class SingleValueConstraint(string it, ExpectationGrammars grammars, string valuesDescription)
		: ConstraintResult.ExpectationOnly<string?[]?>(grammars),
			IValueConstraint<string?[]?>
	{
		private string?[]? _values;

		ConstraintResult IValueConstraint<string?[]?>.IsMetBy(string?[]? actual)
		{
			_values = actual;
			if (actual?.Length != 1)
			{
				Outcome = Outcome.FailureBothWays;
				FurtherProcessingStrategy = FurtherProcessingStrategy.IgnoreResult;
			}

			return this;
		}

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" had ").Append(_values?.Length ?? 0).Append(' ')
				.Append(valuesDescription).Append(' ');
			Formatter.Format(stringBuilder, _values);
		}
	}
}
