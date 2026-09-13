using System;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatGeneric
{
	/// <summary>
	///     Verifies the <paramref name="expectations" /> on the member selected by the <paramref name="memberSelector" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<T, IThat<T>> For<T, TMember>(
		this IThat<T> source,
		Func<T, TMember?> memberSelector,
		Action<IThatSubject<TMember?>> expectations,
		[CallerArgumentExpression("memberSelector")]
		string doNotPopulateThisValue = "")
	{
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		expectationBuilder
			.ForMember(
				MemberAccessor<T, TMember?>.FromFuncAsMemberAccessor(memberSelector, doNotPopulateThisValue),
				(member, stringBuilder) => stringBuilder.Append("for ").Append(member))
			.AddExpectations(e => expectations(new ThatSubject<TMember?>(e)));
		return new AndOrResult<T, IThat<T>>(expectationBuilder, source);
	}
}
