using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Options;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatMembers
{
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <typeparamref name="TMember" /> have
	///     the <paramref name="unexpected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IEnumerable<TMember>, IThat<IEnumerable<TMember>?>> DoNotHaveName<TMember>(
		this IThat<IEnumerable<TMember>?> subject, string unexpected)
		where TMember : MemberInfo?
	{
		StringEqualityOptions options = new(nameof(unexpected));
		return new StringEqualityTypeResult<IEnumerable<TMember>, IThat<IEnumerable<TMember>?>>(subject.Get()
				.ExpectationBuilder.AddConstraint<(string Unexpected, StringEqualityOptions Options), IEnumerable<TMember>>((unexpected, options),
					static (s, it, grammars)
					=> new DoNotHaveNameConstraint<TMember>(it, grammars, s.Unexpected, s.Options)),
			subject,
			options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <typeparamref name="TMember" /> have
	///     the <paramref name="unexpected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IAsyncEnumerable<TMember>, IThat<IAsyncEnumerable<TMember>?>>
		DoNotHaveName<TMember>(
			this IThat<IAsyncEnumerable<TMember>?> subject, string unexpected)
		where TMember : MemberInfo?
	{
		StringEqualityOptions options = new(nameof(unexpected));
		return new StringEqualityTypeResult<IAsyncEnumerable<TMember>, IThat<IAsyncEnumerable<TMember>?>>(subject.Get()
				.ExpectationBuilder.AddConstraint<(string Unexpected, StringEqualityOptions Options), IAsyncEnumerable<TMember>>((unexpected, options),
					static (s, it, grammars)
					=> new DoNotHaveNameConstraint<TMember>(it, grammars, s.Unexpected, s.Options)),
			subject,
			options);
	}
#endif

	private sealed class DoNotHaveNameConstraint<TMember>(
		string it,
		ExpectationGrammars grammars,
		string unexpected,
		StringEqualityOptions options)
		: CollectionConstraintResult<TMember>(it, grammars),
			IAsyncContextConstraint<IEnumerable<TMember>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<TMember>>
#endif
		where TMember : MemberInfo?
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<TMember> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context,
				async memberInfo => !await options.AreConsideredEqual(memberInfo?.Name, unexpected), cancellationToken);
#endif

		public async ValueTask<ConstraintResult> IsMetBy(IEnumerable<TMember> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetValue(actual, context,
				async memberInfo => !await options.AreConsideredEqual(memberInfo?.Name, unexpected), cancellationToken);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have name ").Append(options.GetExpectation(unexpected, Grammars.Negate()));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained not matching items");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all have name ").Append(options.GetExpectation(unexpected, Grammars.Negate()));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained matching items");
	}
}
