using System;
using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

namespace aweXpect.Reflection;

public static partial class ThatType
{
	/// <summary>
	///     Verifies that all fields and properties of the <see cref="Type" /> are non-nullable, including inherited
	///     members or only those declared directly on the type according to the <paramref name="memberScope" />.
	/// </summary>
	/// <remarks>
	///     A member is considered nullable if its type is a <see cref="Nullable{T}" /> value type or a
	///     reference type annotated as nullable (according to the nullable reference type metadata).
	/// </remarks>
	public static AndOrResult<Type?, IThat<Type?>> OnlyHasNonNullableMembers(
		this IThat<Type?> subject, MemberScope memberScope = MemberScope.DeclaredOnly)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new OnlyHasNonNullableMembersConstraint(it, grammars, memberScope)),
			subject);

	private sealed class OnlyHasNonNullableMembersConstraint(
		string it,
		ExpectationGrammars grammars,
		MemberScope memberScope)
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
				(_nullableMembers, _notNullableMembers) = actual.GetMembersByNullability(memberScope);
			}

			Outcome = actual is not null && _nullableMembers.Length == 0 ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("only has non-nullable members");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" contained nullable members ");
			Formatter.Format(stringBuilder, _nullableMembers, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not only have non-nullable members");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" only contained non-nullable members ");
			Formatter.Format(stringBuilder, _notNullableMembers, FormattingOptions.Indented(indentation));
		}
	}
}
