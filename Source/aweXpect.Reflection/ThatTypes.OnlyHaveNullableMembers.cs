using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;
#if NET8_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#endif

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatTypes
{
	/// <summary>
	///     Verifies that all fields, properties and events of all items in the filtered collection of <see cref="Type" />
	///     are nullable, including inherited members or only those declared directly on the type according to
	///     the <paramref name="memberScope" />.
	/// </summary>
	/// <remarks>
	///     A member is considered nullable if its type is a <see cref="Nullable{T}" /> value type or a
	///     reference type annotated as nullable (according to the nullable reference type metadata).
	/// </remarks>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> OnlyHaveNullableMembers(
		this IThat<IEnumerable<Type?>> subject, MemberScope memberScope = MemberScope.DeclaredOnly)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<Type?>>((it, grammars)
				=> new OnlyHaveNullableMembersConstraint(it, grammars, memberScope)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all fields, properties and events of all items in the filtered collection of <see cref="Type" />
	///     are nullable, including inherited members or only those declared directly on the type according to
	///     the <paramref name="memberScope" />.
	/// </summary>
	/// <remarks>
	///     A member is considered nullable if its type is a <see cref="Nullable{T}" /> value type or a
	///     reference type annotated as nullable (according to the nullable reference type metadata).
	/// </remarks>
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>>> OnlyHaveNullableMembers(
		this IThat<IAsyncEnumerable<Type?>> subject, MemberScope memberScope = MemberScope.DeclaredOnly)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<Type?>>((it, grammars)
				=> new OnlyHaveNullableMembersConstraint(it, grammars, memberScope)),
			subject);
#endif

	private sealed class OnlyHaveNullableMembersConstraint(
		string it,
		ExpectationGrammars grammars,
		MemberScope memberScope)
		: CollectionConstraintResult<Type?>(grammars),
			IValueConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<Type?>>
#endif
	{
		private readonly Dictionary<Type, MemberInfo[]> _notNullableMembers = new();

		private bool OnlyHasNullableMembers(Type? type)
		{
			if (type is null)
			{
				return false;
			}

			MemberInfo[] notNullableMembers = type.GetNotNullableMembers(memberScope);
			if (notNullableMembers.Length > 0)
			{
				_notNullableMembers[type] = notNullableMembers;
			}

			return notNullableMembers.Length == 0;
		}

#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, OnlyHasNullableMembers);
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual)
			=> SetValue(actual, OnlyHasNullableMembers);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("only have nullable members");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> MemberViolationRenderer.AppendTypesWithViolatingMembers(stringBuilder, it,
				" contained types with non-nullable members ", NotMatching, _notNullableMembers,
				" with non-nullable members ", indentation);

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all only have nullable members");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained types with only nullable members ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}
}
