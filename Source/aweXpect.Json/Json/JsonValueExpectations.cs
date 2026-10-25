using System;
using aweXpect.Core;

namespace aweXpect.Json;

/// <summary>
///     Adds the expectations on a value that a constraint found in the subject.
/// </summary>
internal static class JsonValueExpectations
{
	/// <summary>
	///     Adds the <paramref name="expectations" /> on the value of the <paramref name="target" />, which are named by
	///     its JSON path.
	/// </summary>
	public static void AddOnTarget<TValue>(ExpectationBuilder expectationBuilder, JsonValueTarget target,
		Action<IThatSubject<TValue>> expectations)
	{
		JsonFormatting.AddValueFormatters(expectationBuilder);
		expectationBuilder
			.ForMember(MemberAccessor<FoundJsonValue<TValue>, TValue>.FromFuncWithPath(
					static found => found.Value, target.Path, "$"),
				static (_, stringBuilder) => stringBuilder.Append(" which "))
			.AddExpectations(expectations, static (expectations, e) => expectations(new ThatSubject<TValue>(e)),
				Singular);
	}

	/// <summary>
	///     The value is a single value, also when the subject is plural, e.g. the elements of an array.
	/// </summary>
	private static ExpectationGrammars Singular(ExpectationGrammars grammars)
		=> grammars & ~ExpectationGrammars.Plural;

	/// <summary>
	///     Adds the <paramref name="expectations" /> on the typed value of the subject, which keep the name of the
	///     subject.
	/// </summary>
	public static void AddOnValue<TValue>(ExpectationBuilder expectationBuilder,
		Action<IThatSubject<TValue>> expectations)
		=> expectationBuilder
			.ForMember(MemberAccessor<FoundJsonValue<TValue>, TValue>.FromFuncWithName(
					static found => found.Value, "value"),
				static (_, stringBuilder) => stringBuilder.Append(" which "),
				false)
			.AddExpectations(expectations, static (expectations, e) => expectations(new ThatSubject<TValue>(e)),
				Singular);
}
