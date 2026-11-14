using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatTypes
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are only used (referenced in their
	///     signature) by types in the filtered collections of types <paramref name="users" /> or
	///     <paramref name="additional" />, or by themselves, within the types passed to
	///     <see cref="OnlyUsedByResult{TResult}.Within" />.
	/// </summary>
	/// <remarks>
	///     The collections are resolved once per assertion; every type within the scope that is not a member of the
	///     union of the allowed collections (by <see cref="Type" /> identity; a generic type definition in a
	///     collection matches any construction of it) must not depend on an item. A generic type definition as item
	///     is also used by a dependency on any construction of it. Each item may only use itself: to allow the items
	///     to use each other, add their collection to the allowed ones. Unlike <c>DependOnlyOn</c>, neither the own
	///     namespace nor framework types are allowed implicitly.
	/// </remarks>
	[GuaranteesNotNull]
	public static OnlyUsedByResult<AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>>> AreOnlyUsedBy(
		this IThat<IEnumerable<Type?>?> subject, Filtered.Types users, params Filtered.Types[] additional)
	{
		TypeSetDependencyOptions allowed = TypeUsageOptions.CreateAllowed(users, additional);
		return new OnlyUsedByResult<AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>>>(within
			=> new AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>>(
				subject.ExpectAll(new AreOnlyUsedByExpectation(new TypeUsageOptions(allowed, within))), subject));
	}

	/// <summary>
	///     …are only used (referenced in their signature) by types in the filtered collections of types
	///     <paramref name="users" /> or <paramref name="additional" />, or by themselves, within the types passed to
	///     <see cref="OnlyUsedByResult{TResult}.Within" />.
	/// </summary>
	/// <inheritdoc cref="AreOnlyUsedBy(IThat{IEnumerable{Type?}}, Filtered.Types, Filtered.Types[])" path="/remarks" />
	public static OnlyUsedByResult<AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>>> AreOnlyUsedBy(
		this IEnumerableItems<Type?> items, Filtered.Types users, params Filtered.Types[] additional)
	{
		TypeSetDependencyOptions allowed = TypeUsageOptions.CreateAllowed(users, additional);
		return new OnlyUsedByResult<AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>>>(within
			=> new AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>>(
				items.ExpectItems(new AreOnlyUsedByExpectation(new TypeUsageOptions(allowed, within))),
				items.Subject));
	}

	/// <inheritdoc cref="AreOnlyUsedBy(IEnumerableItems{Type?}, Filtered.Types, Filtered.Types[])" />
	public static OnlyUsedByResult<AndOrResult<TCollection, IThat<TCollection>>> AreOnlyUsedBy<TCollection>(
		this IStructEnumerableItems<TCollection, Type?> items, Filtered.Types users,
		params Filtered.Types[] additional)
		where TCollection : struct, IEnumerable<Type?>
	{
		TypeSetDependencyOptions allowed = TypeUsageOptions.CreateAllowed(users, additional);
		return new OnlyUsedByResult<AndOrResult<TCollection, IThat<TCollection>>>(within
			=> new AndOrResult<TCollection, IThat<TCollection>>(
				items.ExpectItems(new AreOnlyUsedByExpectation(new TypeUsageOptions(allowed, within))),
				items.Subject));
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="AreOnlyUsedBy(IThat{IEnumerable{Type?}}, Filtered.Types, Filtered.Types[])" />
	[GuaranteesNotNull]
	public static OnlyUsedByResult<AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>>>
		AreOnlyUsedBy(
			this IThat<IAsyncEnumerable<Type?>?> subject, Filtered.Types users, params Filtered.Types[] additional)
	{
		TypeSetDependencyOptions allowed = TypeUsageOptions.CreateAllowed(users, additional);
		return new OnlyUsedByResult<AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>>>(within
			=> new AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>>(
				subject.ExpectAll(new AreOnlyUsedByExpectation(new TypeUsageOptions(allowed, within))), subject));
	}

	/// <inheritdoc cref="AreOnlyUsedBy(IEnumerableItems{Type?}, Filtered.Types, Filtered.Types[])" />
	public static OnlyUsedByResult<AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>>>
		AreOnlyUsedBy(
			this IAsyncEnumerableItems<Type?> items, Filtered.Types users, params Filtered.Types[] additional)
	{
		TypeSetDependencyOptions allowed = TypeUsageOptions.CreateAllowed(users, additional);
		return new OnlyUsedByResult<AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>>>(within
			=> new AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>>(
				items.ExpectItems(new AreOnlyUsedByExpectation(new TypeUsageOptions(allowed, within))),
				items.Subject));
	}
#endif

	private sealed class AreOnlyUsedByExpectation(TypeUsageOptions options)
		: DependencyViolationsExpectation(false, " is used by ")
	{
		protected override async ValueTask<IReadOnlyList<string>> GetViolations(Type item,
			CancellationToken cancellationToken)
			=> (await options.Resolve(cancellationToken)).GetDisallowedUsers(item);

		public override void AppendExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars)
			=> stringBuilder.Append(grammars.IsPlural() ? "are only used by " : "is only used by ")
				.Append(options.Describe());
	}
}
