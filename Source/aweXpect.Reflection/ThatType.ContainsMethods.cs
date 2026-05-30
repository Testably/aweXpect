using System;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Results;

namespace aweXpect.Reflection;

public static partial class ThatType
{
	/// <summary>
	///     Verifies that the <see cref="Type" /> contains methods matching the <paramref name="filter" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="filter" /> receives the methods declared on the type and may use the full method-filter
	///     DSL (e.g. <c>methods.With&lt;FactAttribute&gt;().OrWith&lt;TheoryAttribute&gt;()</c>).<br />
	///     By default the assertion succeeds when the type contains at least one matching method. Append a quantifier
	///     (e.g. <see cref="TypeContainingMembersResult{TThat}.Exactly(Times)" />) to require a specific count.
	/// </remarks>
	public static TypeContainingMembersResult<Type?> ContainsMethods(
		this IThat<Type?> subject,
		Func<Filtered.Methods, Filtered.Methods> filter)
		=> Contains<MethodInfo, Filtered.Methods>(subject, types => types.Methods(), filter);

	private static TypeContainingMembersResult<Type?> Contains<TMember, TFiltered>(
		IThat<Type?> subject,
		Func<Filtered.Types, TFiltered> navigate,
		Func<TFiltered, TFiltered> filter)
		where TFiltered : Filtered<TMember, TFiltered>, IDescribableSubject
	{
		Quantifier quantifier = new();
		IContainedMembersFilter memberFilter =
			TypeFilters.ContainedMembers<TMember, TFiltered>(navigate, filter, quantifier);
		return new TypeContainingMembersResult<Type?>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new ContainsMembersConstraint(it, grammars, memberFilter, quantifier)),
			subject,
			quantifier);
	}

	private sealed class ContainsMembersConstraint(
		string it,
		ExpectationGrammars grammars,
		IContainedMembersFilter memberFilter,
		Quantifier quantifier)
		: ConstraintResult.WithNotNullValue<Type?>(it, grammars),
			IAsyncConstraint<Type?>
	{
		private int _count;

		public async Task<ConstraintResult> IsMetBy(Type? actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			_count = await memberFilter.CountMatchingMembers(actual);
			Outcome = quantifier.Check(_count, true) ?? false
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null) => stringBuilder.Append("contains ").Append(memberFilter.MembersDescription).Append(quantifier);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" contained ").Append(_count)
				.Append(_count == 1 ? " matching member in " : " matching members in ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null) => stringBuilder.Append("does not contain ").Append(memberFilter.MembersDescription).Append(quantifier);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
