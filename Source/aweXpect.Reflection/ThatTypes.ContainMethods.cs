using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Options;
using aweXpect.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Results;

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
	{
		ContainMembersExpectation expectation = ContainMethodsExpectation(filter);
		return new(subject.ExpectAll(expectation), subject, expectation.Quantifier, expectation.Scope!);
	}

	/// <summary>
	///     …contain methods matching the <paramref name="filter" />.
	/// </summary>
	/// <inheritdoc cref="ContainMethods(IThat{IEnumerable{Type}}, Func{Filtered.Methods, Filtered.Methods})" path="/remarks" />
	public static TypeContainingMembersScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> ContainMethods(
		this IEnumerableItems<Type?> items,
		Func<Filtered.Methods, Filtered.Methods> filter)
	{
		ContainMembersExpectation expectation = ContainMethodsExpectation(filter);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Quantifier, expectation.Scope!);
	}

	/// <inheritdoc cref="ContainMethods(IEnumerableItems{Type}, Func{Filtered.Methods, Filtered.Methods})" />
	public static TypeContainingMembersScopeResult<TCollection, TCollection> ContainMethods<TCollection>(
		this IStructEnumerableItems<TCollection, Type?> items,
		Func<Filtered.Methods, Filtered.Methods> filter)
		where TCollection : struct, IEnumerable<Type?>
	{
		ContainMembersExpectation expectation = ContainMethodsExpectation(filter);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Quantifier, expectation.Scope!);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="ContainMethods(IThat{IEnumerable{Type}}, Func{Filtered.Methods, Filtered.Methods})" />
	[GuaranteesNotNull]
	public static TypeContainingMembersScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> ContainMethods(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Func<Filtered.Methods, Filtered.Methods> filter)
	{
		ContainMembersExpectation expectation = ContainMethodsExpectation(filter);
		return new(subject.ExpectAll(expectation), subject, expectation.Quantifier, expectation.Scope!);
	}

	/// <inheritdoc cref="ContainMethods(IEnumerableItems{Type}, Func{Filtered.Methods, Filtered.Methods})" />
	public static TypeContainingMembersScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> ContainMethods(
		this IAsyncEnumerableItems<Type?> items,
		Func<Filtered.Methods, Filtered.Methods> filter)
	{
		ContainMembersExpectation expectation = ContainMethodsExpectation(filter);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Quantifier, expectation.Scope!);
	}
#endif

	private static ContainMembersExpectation ContainMethodsExpectation(Func<Filtered.Methods, Filtered.Methods> filter)
		=> ContainMembersExpectation.WithScope<MethodInfo, Filtered.Methods>(
			(types, memberScope) => types.Methods(memberScope), filter);

	/// <summary>
	///     Checks that a type contains members matching a filter, as often as the <see cref="Quantifier" /> requires.
	/// </summary>
	private sealed class ContainMembersExpectation : ItemExpectation<Type>
	{
		private readonly IContainedMembersFilter _memberFilter;

		private ContainMembersExpectation(Func<OptionsHolder<Quantifier>, IContainedMembersFilter> createMemberFilter,
			OptionsHolder<MemberScopes>? scope)
		{
			Quantifier = new OptionsHolder<Quantifier>(aweXpect.Options.Quantifier.Default);
			Scope = scope;
			_memberFilter = createMemberFilter(Quantifier);
		}

		/// <summary>
		///     How often a type must contain a matching member.
		/// </summary>
		public OptionsHolder<Quantifier> Quantifier { get; }

		/// <summary>
		///     Which members are considered, or <see langword="null" /> when the expectation offers no scope.
		/// </summary>
		public OptionsHolder<MemberScopes>? Scope { get; }

		/// <summary>
		///     Creates the expectation on the members that <paramref name="navigate" /> lists for the scope.
		/// </summary>
		public static ContainMembersExpectation WithScope<TMember, TFiltered>(
			Func<Filtered.Types, MemberScopes, TFiltered> navigate,
			Func<TFiltered, TFiltered> filter)
			where TFiltered : Filtered<TMember, TFiltered>, IDescribableSubject
		{
			OptionsHolder<MemberScopes> scope = new(MemberScopes.None);
			return new ContainMembersExpectation(quantifier => TypeFilters.ContainedMembers<TMember, TFiltered>(
				types => navigate(types, scope.Value), filter, quantifier), scope);
		}

		/// <summary>
		///     Creates the expectation on the members that <paramref name="navigate" /> lists.
		/// </summary>
		public static ContainMembersExpectation WithoutScope<TMember, TFiltered>(
			Func<Filtered.Types, TFiltered> navigate,
			Func<TFiltered, TFiltered> filter)
			where TFiltered : Filtered<TMember, TFiltered>, IDescribableSubject
			=> new(quantifier => TypeFilters.ContainedMembers<TMember, TFiltered>(navigate, filter, quantifier),
				null);

		public override async ValueTask<PredicateResult> IsMatch(Type item, CancellationToken cancellationToken)
			=> await _memberFilter.Applies(item, cancellationToken);

		public override void AppendExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars)
		{
			if (Scope?.Value.IsDirect() == true)
			{
				stringBuilder.Append(grammars.IsPlural() ? "declare " : "declares ");
			}
			else
			{
				stringBuilder.Append(grammars.IsPlural() ? "contain " : "contains ");
			}

			stringBuilder.Append(_memberFilter.MembersDescription)
				.Append(Scope?.Value.ObjectMembersText())
				.Append(Quantifier.Value);
		}
	}
}
