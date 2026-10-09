using System.Collections.Generic;
using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;
#if NET8_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#endif

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatMembers
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <typeparamref name="TMember" /> are public.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<TMember>, IThat<IEnumerable<TMember>>> ArePublic<TMember>(
		this IThat<IEnumerable<TMember>> subject)
		where TMember : MemberInfo?
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<TMember>>((it, grammars)
				=> new ArePublicConstraint<TMember>(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <typeparamref name="TMember" /> are public.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<TMember>, IThat<IAsyncEnumerable<TMember>>> ArePublic<TMember>(
		this IThat<IAsyncEnumerable<TMember>> subject)
		where TMember : MemberInfo?
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<TMember>>((it, grammars)
				=> new ArePublicConstraint<TMember>(it, grammars)),
			subject);
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <typeparamref name="TMember" /> are not public.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<TMember>, IThat<IEnumerable<TMember>>> AreNotPublic<TMember>(
		this IThat<IEnumerable<TMember>> subject)
		where TMember : MemberInfo?
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<TMember>>((it, grammars)
				=> new AreNotPublicConstraint<TMember>(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <typeparamref name="TMember" /> are not public.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<TMember>, IThat<IAsyncEnumerable<TMember>>> AreNotPublic<TMember>(
		this IThat<IAsyncEnumerable<TMember>> subject)
		where TMember : MemberInfo?
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<TMember>>((it, grammars)
				=> new AreNotPublicConstraint<TMember>(it, grammars)),
			subject);
#endif

	private sealed class ArePublicConstraint<TMember>(
		string it,
		ExpectationGrammars grammars)
		: CollectionConstraintResult<TMember>(it, grammars),
			IContextConstraint<IEnumerable<TMember>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<TMember>>
#endif
		where TMember : MemberInfo?
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<TMember> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context,
				member => member.HasAccessModifier(AccessModifiers.Public), cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<TMember> actual, IEvaluationContext context)
			=> SetValue(actual, context, member => member.HasAccessModifier(AccessModifiers.Public));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all are public");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained not matching items");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all are public");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained public items");
	}

	private sealed class AreNotPublicConstraint<TMember>(
		string it,
		ExpectationGrammars grammars)
		: CollectionConstraintResult<TMember>(it, grammars),
			IContextConstraint<IEnumerable<TMember>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<TMember>>
#endif
		where TMember : MemberInfo?
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<TMember> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context,
				member => !member.HasAccessModifier(AccessModifiers.Public), cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<TMember> actual, IEvaluationContext context)
			=> SetValue(actual, context, member => !member.HasAccessModifier(AccessModifiers.Public));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all are not public");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained public items");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("at least one is public");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained no public items");
	}
}
