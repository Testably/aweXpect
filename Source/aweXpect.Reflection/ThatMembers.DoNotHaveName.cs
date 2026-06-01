using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
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
	public static StringEqualityTypeResult<IEnumerable<TMember>, IThat<IEnumerable<TMember>>> DoNotHaveName<TMember>(
		this IThat<IEnumerable<TMember>> subject, string unexpected)
		where TMember : MemberInfo?
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<IEnumerable<TMember>, IThat<IEnumerable<TMember>>>(subject.Get()
				.ExpectationBuilder.AddConstraint<IEnumerable<TMember>>((it, grammars)
					=> new DoNotHaveNameConstraint<TMember>(it, grammars, unexpected, options)),
			subject,
			options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <typeparamref name="TMember" /> have
	///     the <paramref name="unexpected" /> name.
	/// </summary>
	public static StringEqualityTypeResult<IAsyncEnumerable<TMember>, IThat<IAsyncEnumerable<TMember>>>
		DoNotHaveName<TMember>(
			this IThat<IAsyncEnumerable<TMember>> subject, string unexpected)
		where TMember : MemberInfo?
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<IAsyncEnumerable<TMember>, IThat<IAsyncEnumerable<TMember>>>(subject.Get()
				.ExpectationBuilder.AddConstraint<IAsyncEnumerable<TMember>>((it, grammars)
					=> new DoNotHaveNameConstraint<TMember>(it, grammars, unexpected, options)),
			subject,
			options);
	}
#endif

	private sealed class DoNotHaveNameConstraint<TMember>(
		string it,
		ExpectationGrammars grammars,
		string unexpected,
		StringEqualityOptions options)
		: CollectionConstraintResult<TMember>(grammars),
			IAsyncConstraint<IEnumerable<TMember>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<TMember>>
#endif
		where TMember : MemberInfo?
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<TMember> actual, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual,
				async memberInfo => !await options.AreConsideredEqual(memberInfo?.Name, unexpected));
#endif

		public async Task<ConstraintResult> IsMetBy(IEnumerable<TMember> actual, CancellationToken cancellationToken)
			=> await SetValue(actual, async memberInfo => !await options.AreConsideredEqual(memberInfo?.Name, unexpected));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have name ").Append(options.GetExpectation(unexpected, Grammars.Negate()));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained not matching items ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all have name ").Append(options.GetExpectation(unexpected, Grammars.Negate()));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained matching items ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}
}
