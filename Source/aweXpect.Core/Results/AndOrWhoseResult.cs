using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using aweXpect.Core;

namespace aweXpect.Results;

/// <summary>
///     The result of an expectation with an underlying value of type <typeparamref name="TType" />.
///     <para />
///     In addition to the combinations from <see cref="AndOrResult{TType,TThat}" />, allows accessing
///     underlying
///     properties with <see cref="AndOrWhoseResult{TResult,TValue,TSelf}.Whose{TMember}(Func{TResult,TMember}, Action{IThatSubject{TMember}}, string)" />.
/// </summary>
public class AndOrWhoseResult<TType, TThat>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue)
	: AndOrWhoseResult<TType, TThat, AndOrWhoseResult<TType, TThat>>(
		expectationBuilder, returnValue);

/// <summary>
///     The result of an expectation with an underlying value of type <typeparamref name="TType" />.
///     <para />
///     In addition to the combinations from <see cref="AndOrResult{TType,TThat}" />, allows accessing
///     underlying members with <see cref="Whose{TMember}(Func{TType,TMember}, Action{IThatSubject{TMember}}, string)" />.
/// </summary>
public class AndOrWhoseResult<TType, TThat, TSelf>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue)
	: AndOrResult<TType, TThat, TSelf>(expectationBuilder, returnValue)
	where TSelf : AndOrWhoseResult<TType, TThat, TSelf>
{
	private readonly ExpectationBuilder _expectationBuilder = expectationBuilder;
	private readonly TThat _returnValue = returnValue;

	/// <summary>
	///     Allows specifying <paramref name="expectations" /> on the member selected by the
	///     <paramref name="memberSelector" />.
	/// </summary>
	/// <remarks>
	///     If accessing the member throws, the expectation fails with <c>… did throw …</c> and the exception as inner
	///     exception, which a negation does not invert. An <see cref="OperationCanceledException" /> thrown while the
	///     evaluation is cancelled aborts the evaluation instead.
	/// </remarks>
	public AdditionalAndOrWhoseResult
		Whose<TMember>(
			Func<TType, TMember?> memberSelector,
			Action<IThatSubject<TMember?>> expectations,
			[CallerArgumentExpression("memberSelector")]
			string doNotPopulateThisValue = "")
		=> new(
			_expectationBuilder
				.ForMember(
					MemberAccessor<TType, TMember?>.FromFuncAsMemberAccessor(memberSelector, doNotPopulateThisValue),
					(member, stringBuilder) => stringBuilder.Append(" whose ").Append(member))
				.AddExpectations(e => expectations(new ThatSubject<TMember?>(e)),
					grammars => grammars | ExpectationGrammars.Introduced),
			_returnValue);

	/// <summary>
	///     Allows specifying <paramref name="expectations" /> on the awaited result of the member selected by the
	///     <paramref name="memberSelector" />.
	/// </summary>
	/// <remarks>
	///     The member is awaited before the <paramref name="expectations" /> are applied. If accessing or awaiting the
	///     member throws, the expectation fails with <c>… did throw …</c> and the exception as inner exception, which a
	///     negation does not invert. Cancelling the evaluation while the member is awaited aborts it with an
	///     <see cref="OperationCanceledException" /> and a timeout fails it with
	///     <c>did not finish within …</c>, even if the member ignores the cancellation.
	/// </remarks>
	[OverloadResolutionPriority(2)]
	public AdditionalAndOrWhoseResult
		Whose<TMember>(
			Func<TType, Task<TMember>> memberSelector,
			Action<IThatSubject<TMember?>> expectations,
			[CallerArgumentExpression("memberSelector")]
			string doNotPopulateThisValue = "")
		=> new(
			_expectationBuilder
				.ForAsyncMember(
					MemberAccessor<TType, Task<TMember>>.FromFuncAsMemberAccessor(memberSelector,
						doNotPopulateThisValue),
					(member, stringBuilder) => stringBuilder.Append(" whose ").Append(member))
				.AddExpectations(e => expectations(new ThatSubject<TMember?>(e)),
					grammars => grammars | ExpectationGrammars.Introduced),
			_returnValue);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Allows specifying <paramref name="expectations" /> on the awaited result of the member selected by the
	///     <paramref name="memberSelector" />.
	/// </summary>
	/// <remarks>
	///     The member is awaited before the <paramref name="expectations" /> are applied. If accessing or awaiting the
	///     member throws, the expectation fails with <c>… did throw …</c> and the exception as inner exception, which a
	///     negation does not invert. Cancelling the evaluation while the member is awaited aborts it with an
	///     <see cref="OperationCanceledException" /> and a timeout fails it with
	///     <c>did not finish within …</c>, even if the member ignores the cancellation.
	/// </remarks>
	[OverloadResolutionPriority(1)]
	public AdditionalAndOrWhoseResult
		Whose<TMember>(
			Func<TType, ValueTask<TMember>> memberSelector,
			Action<IThatSubject<TMember?>> expectations,
			[CallerArgumentExpression("memberSelector")]
			string doNotPopulateThisValue = "")
		=> Whose(x => memberSelector(x).AsTask(), expectations, doNotPopulateThisValue);
#endif

	/// <summary>
	///     The result of an additional expectation for the underlying type.
	///     <para />
	///     In addition to the combinations from <see cref="AndOrResult{TType,TThat}" />, allows accessing
	///     underlying members with <see cref="AndWhose{TMember}(Func{TType,TMember}, Action{IThatSubject{TMember}}, string)" />.
	/// </summary>
	public class AdditionalAndOrWhoseResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue)
		: AndOrResult<TType, TThat, TSelf>(expectationBuilder, returnValue)
	{
		private readonly ExpectationBuilder _expectationBuilder = expectationBuilder;
		private readonly TThat _returnValue = returnValue;

		/// <summary>
		///     Allows specifying <paramref name="expectations" /> on the member selected by the
		///     <paramref name="memberSelector" />.
		/// </summary>
		/// <remarks>
		///     If accessing the member throws, the expectation fails with <c>… did throw …</c> and the exception as inner
		///     exception, which a negation does not invert. An <see cref="OperationCanceledException" /> thrown while the
		///     evaluation is cancelled aborts the evaluation instead.
		/// </remarks>
		public AdditionalAndOrWhoseResult
			AndWhose<TMember>(
				Func<TType, TMember?> memberSelector,
				Action<IThatSubject<TMember?>> expectations,
				[CallerArgumentExpression("memberSelector")]
				string doNotPopulateThisValue = "")
		{
			_expectationBuilder.And(" and");
			return new AdditionalAndOrWhoseResult(
				_expectationBuilder
					.ForMember(
						MemberAccessor<TType, TMember?>.FromFuncAsMemberAccessor(memberSelector,
							doNotPopulateThisValue),
						(member, stringBuilder) => stringBuilder.Append(" whose ").Append(member))
					.AddExpectations(
						e => expectations(new ThatSubject<TMember?>(e)),
						grammars => grammars | ExpectationGrammars.Introduced),
				_returnValue);
		}

		/// <summary>
		///     Allows specifying <paramref name="expectations" /> on the awaited result of the member selected by the
		///     <paramref name="memberSelector" />.
		/// </summary>
		/// <remarks>
		///     The member is awaited before the <paramref name="expectations" /> are applied. If accessing or awaiting
		///     the member throws, the expectation fails with <c>… did throw …</c> and the exception as inner exception,
		///     which a negation does not invert. Cancelling the evaluation while the member is awaited aborts it with an
		///     <see cref="OperationCanceledException" /> and a timeout fails it with
		///     <c>did not finish within …</c>, even if the member ignores the cancellation.
		/// </remarks>
		[OverloadResolutionPriority(2)]
		public AdditionalAndOrWhoseResult
			AndWhose<TMember>(
				Func<TType, Task<TMember>> memberSelector,
				Action<IThatSubject<TMember?>> expectations,
				[CallerArgumentExpression("memberSelector")]
				string doNotPopulateThisValue = "")
		{
			_expectationBuilder.And(" and");
			return new AdditionalAndOrWhoseResult(
				_expectationBuilder
					.ForAsyncMember(
						MemberAccessor<TType, Task<TMember>>.FromFuncAsMemberAccessor(memberSelector,
							doNotPopulateThisValue),
						(member, stringBuilder) => stringBuilder.Append(" whose ").Append(member))
					.AddExpectations(
						e => expectations(new ThatSubject<TMember?>(e)),
						grammars => grammars | ExpectationGrammars.Introduced),
				_returnValue);
		}

#if NET8_0_OR_GREATER
		/// <summary>
		///     Allows specifying <paramref name="expectations" /> on the awaited result of the member selected by the
		///     <paramref name="memberSelector" />.
		/// </summary>
		/// <remarks>
		///     The member is awaited before the <paramref name="expectations" /> are applied. If accessing or awaiting
		///     the member throws, the expectation fails with <c>… did throw …</c> and the exception as inner exception,
		///     which a negation does not invert. Cancelling the evaluation while the member is awaited aborts it with an
		///     <see cref="OperationCanceledException" /> and a timeout fails it with
		///     <c>did not finish within …</c>, even if the member ignores the cancellation.
		/// </remarks>
		[OverloadResolutionPriority(1)]
		public AdditionalAndOrWhoseResult
			AndWhose<TMember>(
				Func<TType, ValueTask<TMember>> memberSelector,
				Action<IThatSubject<TMember?>> expectations,
				[CallerArgumentExpression("memberSelector")]
				string doNotPopulateThisValue = "")
			=> AndWhose(x => memberSelector(x).AsTask(), expectations, doNotPopulateThisValue);
#endif
	}
}
