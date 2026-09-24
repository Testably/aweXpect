using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatGeneric
{
	/// <summary>
	///     Verifies the <paramref name="expectations" /> on the member selected by the <paramref name="memberSelector" />.
	/// </summary>
	/// <remarks>
	///     If accessing the member throws, the expectation fails with <c>… did throw …</c> and the exception as inner
	///     exception, which a negation does not invert. An <see cref="OperationCanceledException" /> thrown while the
	///     evaluation is cancelled aborts the evaluation instead.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<T?, IThat<T?>> Whose<T, TMember>(
		this IThat<T?> subject,
		Func<T, TMember?> memberSelector,
		Action<IThatSubject<TMember?>> expectations,
		[CallerArgumentExpression("memberSelector")]
		string doNotPopulateThisValue = "")
	{
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		ExpectationGrammars grammars = expectationBuilder.ExpectationGrammars;
		expectationBuilder
			.ForMember(
				MemberAccessor<T, TMember?>.FromFuncAsMemberAccessor(memberSelector, doNotPopulateThisValue),
				(member, stringBuilder) => AppendMember(stringBuilder, grammars, member))
			.AddExpectations(e => expectations(new ThatSubject<TMember?>(e)),
				memberGrammars => MemberGrammars<TMember>(memberGrammars, grammars));
		return new AndOrResult<T?, IThat<T?>>(expectationBuilder, subject);
	}

	/// <summary>
	///     Verifies the <paramref name="expectations" /> on the awaited result of the member selected by the
	///     <paramref name="memberSelector" />.
	/// </summary>
	/// <remarks>
	///     The member is awaited before the <paramref name="expectations" /> are applied. If accessing or awaiting the
	///     member throws, the expectation fails with <c>… did throw …</c> and the exception as inner exception, which a
	///     negation does not invert. Cancelling the evaluation while the member is awaited aborts it with an
	///     <see cref="OperationCanceledException" />, even if the member ignores the cancellation.
	/// </remarks>
	[GuaranteesNotNull]
	[OverloadResolutionPriority(2)]
	public static AndOrResult<T?, IThat<T?>> Whose<T, TMember>(
		this IThat<T?> subject,
		Func<T, Task<TMember>> memberSelector,
		Action<IThatSubject<TMember?>> expectations,
		[CallerArgumentExpression("memberSelector")]
		string doNotPopulateThisValue = "")
	{
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		ExpectationGrammars grammars = expectationBuilder.ExpectationGrammars;
		expectationBuilder
			.ForAsyncMember(
				MemberAccessor<T, Task<TMember>>.FromFuncAsMemberAccessor(memberSelector, doNotPopulateThisValue),
				(member, stringBuilder) => AppendMember(stringBuilder, grammars, member))
			.AddExpectations(e => expectations(new ThatSubject<TMember?>(e)),
				memberGrammars => MemberGrammars<TMember>(memberGrammars, grammars));
		return new AndOrResult<T?, IThat<T?>>(expectationBuilder, subject);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies the <paramref name="expectations" /> on the awaited result of the member selected by the
	///     <paramref name="memberSelector" />.
	/// </summary>
	/// <remarks>
	///     The member is awaited before the <paramref name="expectations" /> are applied. If accessing or awaiting the
	///     member throws, the expectation fails with <c>… did throw …</c> and the exception as inner exception, which a
	///     negation does not invert. Cancelling the evaluation while the member is awaited aborts it with an
	///     <see cref="OperationCanceledException" />, even if the member ignores the cancellation.
	/// </remarks>
	[GuaranteesNotNull]
	[OverloadResolutionPriority(1)]
	public static AndOrResult<T?, IThat<T?>> Whose<T, TMember>(
		this IThat<T?> subject,
		Func<T, ValueTask<TMember>> memberSelector,
		Action<IThatSubject<TMember?>> expectations,
		[CallerArgumentExpression("memberSelector")]
		string doNotPopulateThisValue = "")
		=> subject.Whose(x => memberSelector(x).AsTask(), expectations, doNotPopulateThisValue);
#endif

	/// <summary>
	///     Appends the text for the <paramref name="member" /> in the form the enclosing
	///     <paramref name="grammars" /> allow.
	/// </summary>
	/// <remarks>
	///     A connector that already introduced the subject cannot be followed by a second relative pronoun, so the
	///     member becomes the object of the connector's clause and its expectations are attached with <c>which</c>.
	/// </remarks>
	private static void AppendMember(StringBuilder stringBuilder, ExpectationGrammars grammars, MemberAccessor member)
	{
		if (grammars.HasFlag(ExpectationGrammars.Introduced))
		{
			stringBuilder.Append(grammars.Verb("has ", "have ")).Append(member).Append("which ");
		}
		else
		{
			stringBuilder.Append("whose ").Append(member);
		}
	}

	/// <summary>
	///     The <paramref name="memberGrammars" /> for the expectations on a member written in the form the
	///     <paramref name="enclosingGrammars" /> required.
	/// </summary>
	/// <remarks>
	///     <c>whose Member </c> introduces the member as the subject of the expectations, while the <c>which</c> of the
	///     other form is dropped again before a nested <c>whose</c>.<br />
	///     The number of the member follows its static type alone: a collection other than a <see langword="string" />
	///     or a dictionary is plural (<c>whose Items are</c>), anything else is singular, whatever the number of the
	///     enclosing subject.
	/// </remarks>
	private static ExpectationGrammars MemberGrammars<TMember>(ExpectationGrammars memberGrammars,
		ExpectationGrammars enclosingGrammars)
	{
		memberGrammars = IsCollection(typeof(TMember))
			? memberGrammars | ExpectationGrammars.Plural
			: memberGrammars & ~ExpectationGrammars.Plural;
		return enclosingGrammars.HasFlag(ExpectationGrammars.Introduced)
			? memberGrammars
			: memberGrammars | ExpectationGrammars.Introduced;
	}

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
		return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(System.Collections.Generic.IAsyncEnumerable<>);
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
		return definition == typeof(System.Collections.Generic.IDictionary<,>) ||
		       definition == typeof(System.Collections.Generic.IReadOnlyDictionary<,>);
	}
}
