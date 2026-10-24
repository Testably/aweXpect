using System;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Options;
using aweXpect.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Results;

namespace aweXpect;

public static partial class ThatType
{
	/// <summary>
	///     Verifies that the <see cref="Type" /> contains methods matching the <paramref name="filter" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="filter" /> receives the methods of the type and may use the full method-filter
	///     DSL (e.g. <c>methods.With&lt;FactAttribute&gt;().OrWith&lt;TheoryAttribute&gt;()</c>).<br />
	///     By default the assertion succeeds when the type contains at least one matching method. Append a quantifier
	///     (e.g. <see cref="QuantifierExtensions.Exactly{TResult}(TResult, Times)" />) to require a specific count.<br />
	///     The inherited methods are considered, but neither those declared on <see cref="object" />,
	///     <see cref="ValueType" /> or <see cref="Enum" /> nor those the type cannot access. Restrict them with
	///     <see cref="TypeContainingMembersScopeResult{TType, TThat}.Directly" /> or add the members of these roots with
	///     <see cref="TypeContainingMembersScopeResult{TType, TThat}.IncludingObjectMembers" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static TypeContainingMembersScopeResult<Type, Type?> ContainsMethods(
		this IThat<Type?> subject,
		Func<Filtered.Methods, Filtered.Methods> filter)
		=> ContainsMembers<MethodInfo, Filtered.Methods>(subject,
			(types, memberScope) => types.Methods(memberScope), filter);

	private static TypeContainingMembersScopeResult<Type, Type?> ContainsMembers<TMember, TFiltered>(
		IThat<Type?> subject,
		Func<Filtered.Types, MemberScope, TFiltered> navigate,
		Func<TFiltered, TFiltered> filter)
		where TFiltered : Filtered<TMember, TFiltered>, IDescribableSubject
	{
		ScopeOptions scope = new();
		Quantifier quantifier = new();
		return new TypeContainingMembersScopeResult<Type, Type?>(
			AddContainsConstraint<TMember, TFiltered>(subject, types => navigate(types, scope.MemberScope), filter,
				quantifier, scope),
			subject,
			quantifier,
			scope);
	}

	private static TypeContainingMembersResult<Type, Type?> Contains<TMember, TFiltered>(
		IThat<Type?> subject,
		Func<Filtered.Types, TFiltered> navigate,
		Func<TFiltered, TFiltered> filter)
		where TFiltered : Filtered<TMember, TFiltered>, IDescribableSubject
	{
		Quantifier quantifier = new();
		return new TypeContainingMembersResult<Type, Type?>(
			AddContainsConstraint<TMember, TFiltered>(subject, navigate, filter, quantifier, null),
			subject,
			quantifier);
	}

	private static ExpectationBuilder AddContainsConstraint<TMember, TFiltered>(
		IThat<Type?> subject,
		Func<Filtered.Types, TFiltered> navigate,
		Func<TFiltered, TFiltered> filter,
		Quantifier quantifier,
		ScopeOptions? scope)
		where TFiltered : Filtered<TMember, TFiltered>, IDescribableSubject
	{
		IContainedMembersFilter memberFilter =
			TypeFilters.ContainedMembers<TMember, TFiltered>(navigate, filter, quantifier);
		return subject.Get().ExpectationBuilder.AddConstraint(
			(MemberFilter: memberFilter, Quantifier: quantifier, Scope: scope),
			static (s, it, grammars)
				=> new ContainsMembersConstraint(it, grammars, s.MemberFilter, s.Quantifier, s.Scope));
	}

	private sealed class ContainsMembersConstraint(
		string it,
		ExpectationGrammars grammars,
		IContainedMembersFilter memberFilter,
		Quantifier quantifier,
		ScopeOptions? scope)
		: ConstraintResult.WithNotNullValue<Type?>(it, grammars),
			IAsyncConstraint<Type?>
	{
		private int _count;

		public async ValueTask<ConstraintResult> IsMetBy(Type? actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			_count = await memberFilter.CountMatchingMembers(actual, cancellationToken);
			Outcome = quantifier.Check(_count, true) ?? false
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendMembers(stringBuilder.Append(scope?.IsDirect == true ? "declares " : "contains "));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" contained ").Append(_count)
				.Append(_count == 1 ? " matching member in " : " matching members in ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendMembers(stringBuilder.Append(scope?.IsDirect == true ? "does not declare " : "does not contain "));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);

		private void AppendMembers(StringBuilder stringBuilder)
			=> stringBuilder.Append(memberFilter.MembersDescription)
				.Append(scope?.MemberScope.ObjectMembersText())
				.Append(quantifier);
	}
}
