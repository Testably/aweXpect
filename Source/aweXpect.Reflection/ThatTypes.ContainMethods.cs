using System;
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
using aweXpect.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatTypes
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> contain methods matching the
	///     <paramref name="filter" />.
	/// </summary>
	/// <remarks>
	///     By default each type must contain at least one matching method. Append a quantifier
	///     (e.g. <see cref="QuantifierExtensions.Exactly{TSelf}(TSelf, Times)" />) to require a specific count.<br />
	///     The inherited methods are considered, but neither those declared on <see cref="object" />,
	///     <see cref="ValueType" /> or <see cref="Enum" /> nor those a type cannot access. Restrict them with
	///     <see cref="TypeContainingMembersScopeResult{TType, TValue}.Directly" /> or add the members of these roots with
	///     <see cref="TypeContainingMembersScopeResult{TType, TValue}.IncludingObjectMembers" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static TypeContainingMembersScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> ContainMethods(
		this IThat<IEnumerable<Type?>?> subject,
		Func<Filtered.Methods, Filtered.Methods> filter)
		=> ContainMembers<MethodInfo, Filtered.Methods>(subject,
			(types, memberScope) => types.Methods(memberScope), filter);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> contain methods matching the
	///     <paramref name="filter" />.
	/// </summary>
	/// <remarks>
	///     By default each type must contain at least one matching method. Append a quantifier
	///     (e.g. <see cref="QuantifierExtensions.Exactly{TSelf}(TSelf, Times)" />) to require a specific count.<br />
	///     The inherited methods are considered, but neither those declared on <see cref="object" />,
	///     <see cref="ValueType" /> or <see cref="Enum" /> nor those a type cannot access. Restrict them with
	///     <see cref="TypeContainingMembersScopeResult{TType, TValue}.Directly" /> or add the members of these roots with
	///     <see cref="TypeContainingMembersScopeResult{TType, TValue}.IncludingObjectMembers" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static TypeContainingMembersScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> ContainMethods(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Func<Filtered.Methods, Filtered.Methods> filter)
		=> ContainMembers<MethodInfo, Filtered.Methods>(subject,
			(types, memberScope) => types.Methods(memberScope), filter);
#endif

	private static TypeContainingMembersScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?>
		ContainMembers<TMember, TFiltered>(
			IThat<IEnumerable<Type?>?> subject,
			Func<Filtered.Types, MemberScopes, TFiltered> navigate,
			Func<TFiltered, TFiltered> filter)
		where TFiltered : Filtered<TMember, TFiltered>, IDescribableSubject
	{
		OptionsHolder<MemberScopes> scope = new(MemberScopes.None);
		OptionsHolder<Quantifier> quantifier = new(Quantifier.Default);
		return new TypeContainingMembersScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?>(
			AddContainConstraint<TMember, TFiltered>(subject, types => navigate(types, scope.Value), filter,
				quantifier, scope),
			subject,
			quantifier,
			scope);
	}

#if NET8_0_OR_GREATER
	private static TypeContainingMembersScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?>
		ContainMembers<TMember, TFiltered>(
			IThat<IAsyncEnumerable<Type?>?> subject,
			Func<Filtered.Types, MemberScopes, TFiltered> navigate,
			Func<TFiltered, TFiltered> filter)
		where TFiltered : Filtered<TMember, TFiltered>, IDescribableSubject
	{
		OptionsHolder<MemberScopes> scope = new(MemberScopes.None);
		OptionsHolder<Quantifier> quantifier = new(Quantifier.Default);
		return new TypeContainingMembersScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?>(
			AddContainConstraint<TMember, TFiltered>(subject, types => navigate(types, scope.Value), filter,
				quantifier, scope),
			subject,
			quantifier,
			scope);
	}
#endif

	private static TypeContainingMembersResult<IEnumerable<Type?>, IEnumerable<Type?>?> Contain<TMember, TFiltered>(
		IThat<IEnumerable<Type?>?> subject,
		Func<Filtered.Types, TFiltered> navigate,
		Func<TFiltered, TFiltered> filter)
		where TFiltered : Filtered<TMember, TFiltered>, IDescribableSubject
	{
		OptionsHolder<Quantifier> quantifier = new(Quantifier.Default);
		return new TypeContainingMembersResult<IEnumerable<Type?>, IEnumerable<Type?>?>(
			AddContainConstraint<TMember, TFiltered>(subject, navigate, filter, quantifier, null),
			subject,
			quantifier);
	}

#if NET8_0_OR_GREATER
	private static TypeContainingMembersResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?>
		Contain<TMember, TFiltered>(
			IThat<IAsyncEnumerable<Type?>?> subject,
			Func<Filtered.Types, TFiltered> navigate,
			Func<TFiltered, TFiltered> filter)
		where TFiltered : Filtered<TMember, TFiltered>, IDescribableSubject
	{
		OptionsHolder<Quantifier> quantifier = new(Quantifier.Default);
		return new TypeContainingMembersResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?>(
			AddContainConstraint<TMember, TFiltered>(subject, navigate, filter, quantifier, null),
			subject,
			quantifier);
	}
#endif

	private static ExpectationBuilder AddContainConstraint<TMember, TFiltered>(
		IThat<IEnumerable<Type?>?> subject,
		Func<Filtered.Types, TFiltered> navigate,
		Func<TFiltered, TFiltered> filter,
		OptionsHolder<Quantifier> quantifier,
		OptionsHolder<MemberScopes>? scope)
		where TFiltered : Filtered<TMember, TFiltered>, IDescribableSubject
	{
		IContainedMembersFilter memberFilter =
			TypeFilters.ContainedMembers<TMember, TFiltered>(navigate, filter, quantifier);
		return subject.Get().ExpectationBuilder
			.AddConstraint<(IContainedMembersFilter MemberFilter, OptionsHolder<Quantifier> Quantifier, OptionsHolder<MemberScopes>? Scope),
				IEnumerable<Type?>>(
				(memberFilter, quantifier, scope),
				static (s, it, grammars)
					=> new ContainMembersConstraint(it, grammars, s.MemberFilter, s.Quantifier, s.Scope));
	}

#if NET8_0_OR_GREATER
	private static ExpectationBuilder AddContainConstraint<TMember, TFiltered>(
		IThat<IAsyncEnumerable<Type?>?> subject,
		Func<Filtered.Types, TFiltered> navigate,
		Func<TFiltered, TFiltered> filter,
		OptionsHolder<Quantifier> quantifier,
		OptionsHolder<MemberScopes>? scope)
		where TFiltered : Filtered<TMember, TFiltered>, IDescribableSubject
	{
		IContainedMembersFilter memberFilter =
			TypeFilters.ContainedMembers<TMember, TFiltered>(navigate, filter, quantifier);
		return subject.Get().ExpectationBuilder
			.AddConstraint<(IContainedMembersFilter MemberFilter, OptionsHolder<Quantifier> Quantifier, OptionsHolder<MemberScopes>? Scope),
				IAsyncEnumerable<Type?>>(
				(memberFilter, quantifier, scope),
				static (s, it, grammars)
					=> new ContainMembersConstraint(it, grammars, s.MemberFilter, s.Quantifier, s.Scope));
	}
#endif

	private sealed class ContainMembersConstraint(
		string it,
		ExpectationGrammars grammars,
		IContainedMembersFilter memberFilter,
		OptionsHolder<Quantifier> quantifier,
		OptionsHolder<MemberScopes>? scope)
		: CollectionConstraintResult<Type?>(it, grammars),
			IAsyncContextConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<Type?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context, type => Matches(type, cancellationToken), cancellationToken);
#endif

		public async ValueTask<ConstraintResult> IsMetBy(IEnumerable<Type?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetValue(actual, context, type => Matches(type, cancellationToken), cancellationToken);

		private ValueTask<bool> Matches(Type? type, CancellationToken cancellationToken)
			=> type is null ? new ValueTask<bool>(false) : memberFilter.Applies(type, cancellationToken);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendMembers(stringBuilder.Append(scope?.Value.IsDirect() == true ? "all declare " : "all contain "));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained not matching types");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendMembers(stringBuilder.Append(scope?.Value.IsDirect() == true ? "do not all declare " : "do not all contain "));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained matching types");

		private void AppendMembers(StringBuilder stringBuilder)
			=> stringBuilder.Append(memberFilter.MembersDescription)
				.Append(scope?.Value.ObjectMembersText())
				.Append(quantifier);
	}
}
