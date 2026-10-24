using System;
using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Results;

namespace aweXpect;

public static partial class ThatType
{
	/// <summary>
	///     Verifies that all fields, properties and events of the <see cref="Type" /> are nullable.
	/// </summary>
	/// <remarks>
	///     A member is considered nullable if its type is a <see cref="Nullable{T}" /> value type or a
	///     reference type annotated as nullable (according to the nullable reference type metadata).<br />
	///     The inherited members are considered, but neither those declared on <see cref="object" />,
	///     <see cref="ValueType" /> or <see cref="Enum" /> nor those the type cannot access. Restrict them with
	///     <see cref="InheritanceScopeResult{TType, TValue}.Directly" /> or add the members of these roots with
	///     <see cref="MemberScopeResult{TType, TValue}.IncludingObjectMembers" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static MemberScopeResult<Type, Type?> OnlyHasNullableMembers(
		this IThat<Type?> subject)
	{
		ScopeOptions scope = new();
		return new MemberScopeResult<Type, Type?>(subject.Get().ExpectationBuilder.AddConstraint(scope,
				static (scope, it, grammars)
				=> new OnlyHasNullableMembersConstraint(it, grammars, scope)),
			subject,
			scope);
	}

	private sealed class OnlyHasNullableMembersConstraint(
		string it,
		ExpectationGrammars grammars,
		ScopeOptions scope)
		: ConstraintResult.WithNotNullValue<Type?>(it, grammars),
			IValueConstraint<Type?>
	{
		private MemberInfo[] _notNullableMembers = [];
		private MemberInfo[] _nullableMembers = [];

		public ConstraintResult IsMetBy(Type? actual)
		{
			Actual = actual;
			if (actual is not null)
			{
				(_nullableMembers, _notNullableMembers) = actual.GetMembersByNullability(scope.MemberScope);
			}

			Outcome = actual is not null && _notNullableMembers.Length == 0 ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendScope(stringBuilder.Append(scope.IsDirect ? "only declares nullable members" : "only has nullable members"));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained non-nullable members");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendScope(stringBuilder.Append(scope.IsDirect
				? "does not only declare nullable members"
				: "does not only have nullable members"));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained nullable members");

		private void AppendScope(StringBuilder stringBuilder)
		{
			if (scope.MemberScope.IncludesObjectMembers())
			{
				stringBuilder.Append(" including object members");
			}
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
		{
			if (Outcome == Outcome.Failure)
			{
				contexts.AddMembersContext(IsNegated ? "Nullable members" : "Non-nullable members",
					IsNegated ? _nullableMembers : _notNullableMembers);
			}
		}
	}
}
