using System;
using System.Collections.Generic;
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

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatTypes
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> contain methods matching the
	///     <paramref name="filter" />.
	/// </summary>
	/// <remarks>
	///     By default each type must contain at least one matching method. Append a quantifier
	///     (e.g. <see cref="TypeContainingMembersResult{TThat}.Exactly(Times)" />) to require a specific count.<br />
	///     The <paramref name="memberScope" /> controls whether inherited methods are considered.
	/// </remarks>
	public static TypeContainingMembersResult<IEnumerable<Type?>> ContainMethods(
		this IThat<IEnumerable<Type?>> subject,
		Func<Filtered.Methods, Filtered.Methods> filter,
		MemberScope memberScope = MemberScope.DeclaredOnly)
		=> Contain<MethodInfo, Filtered.Methods>(subject, types => types.Methods(memberScope), filter);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> contain methods matching the
	///     <paramref name="filter" />.
	/// </summary>
	/// <remarks>
	///     By default each type must contain at least one matching method. Append a quantifier
	///     (e.g. <see cref="TypeContainingMembersResult{TThat}.Exactly(Times)" />) to require a specific count.<br />
	///     The <paramref name="memberScope" /> controls whether inherited methods are considered.
	/// </remarks>
	public static TypeContainingMembersResult<IAsyncEnumerable<Type?>> ContainMethods(
		this IThat<IAsyncEnumerable<Type?>> subject,
		Func<Filtered.Methods, Filtered.Methods> filter,
		MemberScope memberScope = MemberScope.DeclaredOnly)
		=> Contain<MethodInfo, Filtered.Methods>(subject, types => types.Methods(memberScope), filter);
#endif

	private static TypeContainingMembersResult<IEnumerable<Type?>> Contain<TMember, TFiltered>(
		IThat<IEnumerable<Type?>> subject,
		Func<Filtered.Types, TFiltered> navigate,
		Func<TFiltered, TFiltered> filter)
		where TFiltered : Filtered<TMember, TFiltered>, IDescribableSubject
	{
		Quantifier quantifier = new();
		IContainedMembersFilter memberFilter =
			TypeFilters.ContainedMembers<TMember, TFiltered>(navigate, filter, quantifier);
		return new TypeContainingMembersResult<IEnumerable<Type?>>(
			subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<Type?>>((it, grammars)
				=> new ContainMembersConstraint(it, grammars, memberFilter, quantifier)),
			subject,
			quantifier);
	}

#if NET8_0_OR_GREATER
	private static TypeContainingMembersResult<IAsyncEnumerable<Type?>> Contain<TMember, TFiltered>(
		IThat<IAsyncEnumerable<Type?>> subject,
		Func<Filtered.Types, TFiltered> navigate,
		Func<TFiltered, TFiltered> filter)
		where TFiltered : Filtered<TMember, TFiltered>, IDescribableSubject
	{
		Quantifier quantifier = new();
		IContainedMembersFilter memberFilter =
			TypeFilters.ContainedMembers<TMember, TFiltered>(navigate, filter, quantifier);
		return new TypeContainingMembersResult<IAsyncEnumerable<Type?>>(
			subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<Type?>>((it, grammars)
				=> new ContainMembersConstraint(it, grammars, memberFilter, quantifier)),
			subject,
			quantifier);
	}
#endif

	private sealed class ContainMembersConstraint(
		string it,
		ExpectationGrammars grammars,
		IContainedMembersFilter memberFilter,
		Quantifier quantifier)
		: CollectionConstraintResult<Type?>(grammars),
			IAsyncConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<Type?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, Matches);
#endif

		public async Task<ConstraintResult> IsMetBy(IEnumerable<Type?> actual,
			CancellationToken cancellationToken)
			=> await SetValue(actual, Matches);

#if NET8_0_OR_GREATER
		private ValueTask<bool> Matches(Type? type)
			=> type is null ? new ValueTask<bool>(false) : memberFilter.Applies(type);
#else
		private Task<bool> Matches(Type? type)
			=> type is null ? Task.FromResult(false) : memberFilter.Applies(type);
#endif

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null) => stringBuilder.Append("all contain ").Append(memberFilter.MembersDescription).Append(quantifier);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained not matching types ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null) => stringBuilder.Append("do not all contain ").Append(memberFilter.MembersDescription).Append(quantifier);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained matching types ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}
}
