using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Results;
#if NET8_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#endif

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatTypes
{
	/// <summary>
	///     Verifies that all fields, properties and events of all items in the filtered collection of <see cref="Type" />
	///     are non-nullable.
	/// </summary>
	/// <remarks>
	///     A member is considered nullable if its type is a <see cref="Nullable{T}" /> value type or a
	///     reference type annotated as nullable (according to the nullable reference type metadata).<br />
	///     The inherited members are considered, but neither those declared on <see cref="object" />,
	///     <see cref="ValueType" /> or <see cref="Enum" /> nor those a type cannot access. Restrict them with
	///     <see cref="InheritanceScopeResult{TType, TValue}.Directly" /> or add the members of these roots with
	///     <see cref="MemberScopeResult{TType, TValue}.IncludingObjectMembers" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static MemberScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> OnlyHaveNonNullableMembers(
		this IThat<IEnumerable<Type?>?> subject)
	{
		ScopeOptions scope = new();
		return new MemberScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?>(subject.Get().ExpectationBuilder.AddConstraint<ScopeOptions, IEnumerable<Type?>>(scope,
				static (scope, it, grammars)
				=> new OnlyHaveNonNullableMembersConstraint(it, grammars, scope)),
			subject,
			scope);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all fields, properties and events of all items in the filtered collection of <see cref="Type" />
	///     are non-nullable.
	/// </summary>
	/// <remarks>
	///     A member is considered nullable if its type is a <see cref="Nullable{T}" /> value type or a
	///     reference type annotated as nullable (according to the nullable reference type metadata).<br />
	///     The inherited members are considered, but neither those declared on <see cref="object" />,
	///     <see cref="ValueType" /> or <see cref="Enum" /> nor those a type cannot access. Restrict them with
	///     <see cref="InheritanceScopeResult{TType, TValue}.Directly" /> or add the members of these roots with
	///     <see cref="MemberScopeResult{TType, TValue}.IncludingObjectMembers" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static MemberScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> OnlyHaveNonNullableMembers(
		this IThat<IAsyncEnumerable<Type?>?> subject)
	{
		ScopeOptions scope = new();
		return new MemberScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?>(subject.Get().ExpectationBuilder.AddConstraint<ScopeOptions, IAsyncEnumerable<Type?>>(scope,
				static (scope, it, grammars)
				=> new OnlyHaveNonNullableMembersConstraint(it, grammars, scope)),
			subject,
			scope);
	}
#endif

	private sealed class OnlyHaveNonNullableMembersConstraint(
		string it,
		ExpectationGrammars grammars,
		ScopeOptions scope)
		: CollectionConstraintResult<Type?>(it, grammars),
			IContextConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<Type?>>
#endif
	{
		private readonly Dictionary<Type, MemberInfo[]> _nullableMembers = new();

		private bool OnlyHasNonNullableMembers(Type? type)
		{
			if (type is null)
			{
				return false;
			}

			MemberInfo[] nullableMembers = type.GetNullableMembers(scope.MemberScope);
			if (nullableMembers.Length > 0)
			{
				_nullableMembers[type] = nullableMembers;
			}

			return nullableMembers.Length == 0;
		}

#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context, OnlyHasNonNullableMembers, cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual, IEvaluationContext context)
			=> SetValue(actual, context, OnlyHasNonNullableMembers);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendScope(stringBuilder.Append(scope.IsDirect
				? "only declare non-nullable members"
				: "only have non-nullable members"));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained types with nullable members");

		protected override Func<string?> FormatItems(Type?[] items)
			=> () => MemberViolationRenderer.FormatTypesWithViolatingMembers(items, _nullableMembers,
				" with nullable members ");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendScope(stringBuilder.Append(scope.IsDirect
				? "not all only declare non-nullable members"
				: "not all only have non-nullable members"));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained types with only non-nullable members");

		private void AppendScope(StringBuilder stringBuilder)
		{
			if (scope.MemberScope.IncludesObjectMembers())
			{
				stringBuilder.Append(" including object members");
			}
		}
	}
}
