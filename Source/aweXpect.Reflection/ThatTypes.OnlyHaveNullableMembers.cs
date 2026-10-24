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
	///     are nullable.
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
	public static MemberScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> OnlyHaveNullableMembers(
		this IThat<IEnumerable<Type?>?> subject)
	{
		ScopeOptions scope = new();
		return new MemberScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?>(subject.Get().ExpectationBuilder.AddConstraint<ScopeOptions, IEnumerable<Type?>>(scope,
				static (scope, it, grammars)
				=> new OnlyHaveNullableMembersConstraint(it, grammars, scope)),
			subject,
			scope);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all fields, properties and events of all items in the filtered collection of <see cref="Type" />
	///     are nullable.
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
	public static MemberScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> OnlyHaveNullableMembers(
		this IThat<IAsyncEnumerable<Type?>?> subject)
	{
		ScopeOptions scope = new();
		return new MemberScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?>(subject.Get().ExpectationBuilder.AddConstraint<ScopeOptions, IAsyncEnumerable<Type?>>(scope,
				static (scope, it, grammars)
				=> new OnlyHaveNullableMembersConstraint(it, grammars, scope)),
			subject,
			scope);
	}
#endif

	private sealed class OnlyHaveNullableMembersConstraint(
		string it,
		ExpectationGrammars grammars,
		ScopeOptions scope)
		: CollectionConstraintResult<Type?>(it, grammars),
			IContextConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<Type?>>
#endif
	{
		private readonly Dictionary<Type, MemberInfo[]> _notNullableMembers = new();

		private bool OnlyHasNullableMembers(Type? type)
		{
			if (type is null)
			{
				return false;
			}

			MemberInfo[] notNullableMembers = type.GetNotNullableMembers(scope.MemberScope);
			if (notNullableMembers.Length > 0)
			{
				_notNullableMembers[type] = notNullableMembers;
			}

			return notNullableMembers.Length == 0;
		}

#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context, OnlyHasNullableMembers, cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual, IEvaluationContext context)
			=> SetValue(actual, context, OnlyHasNullableMembers);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendScope(stringBuilder.Append(scope.IsDirect ? "only declare nullable members" : "only have nullable members"));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained types with non-nullable members");

		protected override Func<string?> FormatItems(Type?[] items)
			=> () => MemberViolationRenderer.FormatTypesWithViolatingMembers(items, _notNullableMembers,
				" with non-nullable members ");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendScope(stringBuilder.Append(scope.IsDirect
				? "not all only declare nullable members"
				: "not all only have nullable members"));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained types with only nullable members");

		private void AppendScope(StringBuilder stringBuilder)
		{
			if (scope.MemberScope.IncludesObjectMembers())
			{
				stringBuilder.Append(" including object members");
			}
		}
	}
}
