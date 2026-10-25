using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatGeneric
{
	/// <summary>
	///     Verifies the <paramref name="expectations" /> on the member selected by the <paramref name="memberAccessor" />.
	/// </summary>
	/// <remarks>
	///     If accessing the member throws, the expectation fails with <c>… did throw …</c> and the exception as inner
	///     exception, which a negation does not invert. An <see cref="OperationCanceledException" /> thrown while the
	///     evaluation is canceled aborts the evaluation instead.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<T, IThat<T?>> Whose<T, TMember>(
		this IThat<T?> subject,
		Func<T, TMember?> memberAccessor,
		Action<IThatSubject<TMember?>> expectations,
		[CallerArgumentExpression("memberAccessor")]
		string doNotPopulateThisValue = "")
	{
		memberAccessor.ThrowIfNull();
		expectations.ThrowIfNull();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		ExpectationGrammars grammars = expectationBuilder.ExpectationGrammars;
		expectationBuilder
			.ForMember(
				MemberAccessor<T, TMember?>.FromFuncWithCallerExpression(memberAccessor, doNotPopulateThisValue),
				GetMemberText(grammars))
			.AddExpectations(expectations, static (expectations, e) => expectations(new ThatSubject<TMember?>(e)),
				GetMemberGrammars<TMember>(grammars));
		return new AndOrResult<T, IThat<T?>>(expectationBuilder, subject);
	}

	/// <summary>
	///     Verifies the <paramref name="expectations" /> on the awaited result of the member selected by the
	///     <paramref name="memberAccessor" />.
	/// </summary>
	/// <remarks>
	///     The member is awaited before the <paramref name="expectations" /> are applied. If accessing or awaiting the
	///     member throws, the expectation fails with <c>… did throw …</c> and the exception as inner exception, which a
	///     negation does not invert. Canceling the evaluation while the member is awaited leaves the expectation inconclusive, and a
	///     timeout fails it with <c>did not finish within …</c>, even if the member ignores the cancellation.
	/// </remarks>
	[GuaranteesNotNull]
	[OverloadResolutionPriority(2)]
	public static AndOrResult<T, IThat<T?>> Whose<T, TMember>(
		this IThat<T?> subject,
		Func<T, Task<TMember>> memberAccessor,
		Action<IThatSubject<TMember?>> expectations,
		[CallerArgumentExpression("memberAccessor")]
		string doNotPopulateThisValue = "")
	{
		memberAccessor.ThrowIfNull();
		expectations.ThrowIfNull();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		ExpectationGrammars grammars = expectationBuilder.ExpectationGrammars;
		expectationBuilder
			.ForAsyncMember(
				MemberAccessor<T, Task<TMember>>.FromFuncWithCallerExpression(memberAccessor, doNotPopulateThisValue),
				GetMemberText(grammars))
			.AddExpectations(expectations, static (expectations, e) => expectations(new ThatSubject<TMember?>(e)),
				GetMemberGrammars<TMember>(grammars));
		return new AndOrResult<T, IThat<T?>>(expectationBuilder, subject);
	}

	/// <summary>
	///     Verifies the <paramref name="expectations" /> on the awaited result of the member selected by the
	///     <paramref name="memberAccessor" />.
	/// </summary>
	/// <remarks>
	///     The member is awaited before the <paramref name="expectations" /> are applied. If accessing or awaiting the
	///     member throws, the expectation fails with <c>… did throw …</c> and the exception as inner exception, which a
	///     negation does not invert. Canceling the evaluation while the member is awaited leaves the expectation inconclusive, and a
	///     timeout fails it with <c>did not finish within …</c>, even if the member ignores the cancellation.
	/// </remarks>
	[GuaranteesNotNull]
	[OverloadResolutionPriority(1)]
	public static AndOrResult<T, IThat<T?>> Whose<T, TMember>(
		this IThat<T?> subject,
		Func<T, ValueTask<TMember>> memberAccessor,
		Action<IThatSubject<TMember?>> expectations,
		[CallerArgumentExpression("memberAccessor")]
		string doNotPopulateThisValue = "")
	{
		memberAccessor.ThrowIfNull();
		expectations.ThrowIfNull();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		ExpectationGrammars grammars = expectationBuilder.ExpectationGrammars;
		expectationBuilder
			.ForAsyncMember(
				MemberAccessor<T, ValueTask<TMember>>.FromFuncWithCallerExpression(memberAccessor,
					doNotPopulateThisValue),
				GetMemberText(grammars))
			.AddExpectations(expectations, static (expectations, e) => expectations(new ThatSubject<TMember?>(e)),
				GetMemberGrammars<TMember>(grammars));
		return new AndOrResult<T, IThat<T?>>(expectationBuilder, subject);
	}

	/// <summary>
	///     Returns the text for the member in the form the enclosing <paramref name="grammars" /> allow.
	/// </summary>
	/// <remarks>
	///     A connector that already introduced the subject cannot be followed by a second relative pronoun, so the
	///     member becomes the object of the connector's clause and its expectations are attached with <c>that</c>.
	///     <para />
	///     A cached delegate for each form instead of a closure over the <paramref name="grammars" /> keeps every
	///     expectation on a member smaller.
	/// </remarks>
	private static Action<MemberAccessor, StringBuilder> GetMemberText(ExpectationGrammars grammars)
	{
		if (!grammars.IsIntroduced())
		{
			return static (member, stringBuilder) => stringBuilder.Append("whose ").Append(member);
		}

		return grammars.IsPlural()
			? static (member, stringBuilder) => stringBuilder.Append("have ").Append(member).Append("that ")
			: static (member, stringBuilder) => stringBuilder.Append("has ").Append(member).Append("that ");
	}

	/// <summary>
	///     Returns the grammars for the expectations on a member written in the form the
	///     <paramref name="enclosingGrammars" /> required.
	/// </summary>
	/// <remarks>
	///     <c>whose Member </c> introduces the member as the subject of the expectations, while the <c>that</c> of the
	///     other form is dropped again before a nested <c>whose</c>.
	///     <para />
	///     A cached delegate for each form instead of a closure over the <paramref name="enclosingGrammars" /> keeps
	///     every expectation on a member smaller.
	/// </remarks>
	private static Func<ExpectationGrammars, ExpectationGrammars> GetMemberGrammars<TMember>(
		ExpectationGrammars enclosingGrammars)
		=> enclosingGrammars.IsIntroduced()
			? static memberGrammars => MemberGrammars<TMember>(memberGrammars)
			: static memberGrammars => MemberGrammars<TMember>(memberGrammars) | ExpectationGrammars.Introduced;

	/// <remarks>
	///     The number of the member follows its static type alone: a collection other than a <see langword="string" />
	///     or a dictionary is plural (<c>whose Items are</c>), anything else is singular, whatever the number of the
	///     enclosing subject.
	/// </remarks>
	private static ExpectationGrammars MemberGrammars<TMember>(ExpectationGrammars memberGrammars)
		=> IsCollection(typeof(TMember))
			? memberGrammars | ExpectationGrammars.Plural
			: memberGrammars & ~ExpectationGrammars.Plural;

	private static bool IsCollection(Type type)
	{
		if (type == typeof(string) || IsDictionary(type))
		{
			return false;
		}

		if (typeof(IEnumerable).IsAssignableFrom(type))
		{
			return true;
		}

#if NET8_0_OR_GREATER
		// Only the interface itself, because searching the implemented interfaces is not trim-safe.
		return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>);
#else
		return false;
#endif
	}

	/// <remarks>
	///     A dictionary reads as a single lookup (<c>whose Map contains key 1</c>), not as a plural noun.<br />
	///     The generic interfaces are only matched by their definition, because searching the implemented interfaces is
	///     not trim-safe; the framework dictionaries also implement <see cref="IDictionary" />.
	/// </remarks>
	private static bool IsDictionary(Type type)
	{
		if (typeof(IDictionary).IsAssignableFrom(type))
		{
			return true;
		}

		if (!type.IsGenericType)
		{
			return false;
		}

		Type definition = type.GetGenericTypeDefinition();
		return definition == typeof(IDictionary<,>) ||
		       definition == typeof(IReadOnlyDictionary<,>);
	}
}
