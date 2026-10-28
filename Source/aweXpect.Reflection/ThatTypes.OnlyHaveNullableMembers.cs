using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Results;

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
		OnlyHaveMembersExpectation expectation = new(true);
		return new(subject.ExpectAll(expectation), subject, expectation.Scope);
	}

	/// <summary>
	///     …only have nullable fields, properties and events.
	/// </summary>
	/// <inheritdoc cref="OnlyHaveNullableMembers(IThat{IEnumerable{Type}})" path="/remarks" />
	public static MemberScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> OnlyHaveNullableMembers(
		this IEnumerableItems<Type?> items)
	{
		OnlyHaveMembersExpectation expectation = new(true);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Scope);
	}

	/// <inheritdoc cref="OnlyHaveNullableMembers(IEnumerableItems{Type})" />
	public static MemberScopeResult<TCollection, TCollection> OnlyHaveNullableMembers<TCollection>(
		this IStructEnumerableItems<TCollection, Type?> items)
		where TCollection : struct, IEnumerable<Type?>
	{
		OnlyHaveMembersExpectation expectation = new(true);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Scope);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="OnlyHaveNullableMembers(IThat{IEnumerable{Type}})" />
	[GuaranteesNotNull]
	public static MemberScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> OnlyHaveNullableMembers(
		this IThat<IAsyncEnumerable<Type?>?> subject)
	{
		OnlyHaveMembersExpectation expectation = new(true);
		return new(subject.ExpectAll(expectation), subject, expectation.Scope);
	}

	/// <inheritdoc cref="OnlyHaveNullableMembers(IEnumerableItems{Type})" />
	public static MemberScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> OnlyHaveNullableMembers(
		this IAsyncEnumerableItems<Type?> items)
	{
		OnlyHaveMembersExpectation expectation = new(true);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Scope);
	}
#endif

	/// <summary>
	///     Checks that a type only has nullable, or only non-nullable, fields, properties and events, and keeps the
	///     members that violate it for the failure message.
	/// </summary>
	private sealed class OnlyHaveMembersExpectation(bool nullable) : ItemExpectation<Type>
	{
		private readonly Dictionary<Type, MemberInfo[]> _violatingMembers = new();

		public OptionsHolder<MemberScopes> Scope { get; } = new(MemberScopes.None);

		public override ValueTask<PredicateResult> IsMatch(Type item, CancellationToken cancellationToken)
		{
			MemberInfo[] violatingMembers = nullable
				? item.GetNotNullableMembers(Scope.Value)
				: item.GetNullableMembers(Scope.Value);
			if (violatingMembers.Length > 0)
			{
				_violatingMembers[item] = violatingMembers;
			}

			return new ValueTask<PredicateResult>(violatingMembers.Length == 0);
		}

		public override void AppendExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars)
		{
			stringBuilder.Append("only ");
			if (Scope.Value.IsDirect())
			{
				stringBuilder.Append(grammars.IsPlural() ? "declare " : "declares ");
			}
			else
			{
				stringBuilder.Append(grammars.IsPlural() ? "have " : "has ");
			}

			stringBuilder.Append(nullable ? "nullable members" : "non-nullable members");
			if (Scope.Value.IncludesObjectMembers())
			{
				stringBuilder.Append(" including object members");
			}
		}

		public override string FormatItems(IEnumerable<Type?> items, FormattingOptions options)
			=> MemberViolationRenderer.FormatTypesWithViolatingMembers(items.ToList(), _violatingMembers,
				nullable ? " with non-nullable members " : " with nullable members ", options.TotalItemCount);
	}
}
