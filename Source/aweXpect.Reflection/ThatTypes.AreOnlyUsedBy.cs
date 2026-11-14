using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.EvaluationContext;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;

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
	///     is also used by a dependency on any construction of it, and any construction of the same generic type
	///     definition counts as the item itself. Each item may only use itself, unless
	///     <see cref="OnlyUsedByItemsResult{TType, TValue}.OrByEachOther" /> also allows the items to use each other.
	///     Unlike <c>DependOnlyOn</c>, neither the own namespace nor framework types are allowed implicitly.
	/// </remarks>
	[GuaranteesNotNull]
	public static OnlyUsedByResult<OnlyUsedByItemsResult<IEnumerable<Type?>, IEnumerable<Type?>?>> AreOnlyUsedBy(
		this IThat<IEnumerable<Type?>?> subject, Filtered.Types users, params Filtered.Types[] additional)
		=> OnlyUsedBy<IEnumerable<Type?>, IEnumerable<Type?>?>(users, additional, subject,
			expectation => subject.ExpectAll(expectation));

	/// <summary>
	///     …are only used (referenced in their signature) by types in the filtered collections of types
	///     <paramref name="users" /> or <paramref name="additional" />, or by themselves, within the types passed to
	///     <see cref="OnlyUsedByResult{TResult}.Within" />.
	/// </summary>
	/// <inheritdoc cref="AreOnlyUsedBy(IThat{IEnumerable{Type?}}, Filtered.Types, Filtered.Types[])" path="/remarks" />
	public static OnlyUsedByResult<OnlyUsedByItemsResult<IEnumerable<Type?>, IEnumerable<Type?>?>> AreOnlyUsedBy(
		this IEnumerableItems<Type?> items, Filtered.Types users, params Filtered.Types[] additional)
		=> OnlyUsedBy<IEnumerable<Type?>, IEnumerable<Type?>?>(users, additional, items.Subject,
			items.ExpectItems);

	/// <inheritdoc cref="AreOnlyUsedBy(IEnumerableItems{Type?}, Filtered.Types, Filtered.Types[])" />
	public static OnlyUsedByResult<OnlyUsedByItemsResult<TCollection, TCollection>> AreOnlyUsedBy<TCollection>(
		this IStructEnumerableItems<TCollection, Type?> items, Filtered.Types users,
		params Filtered.Types[] additional)
		where TCollection : struct, IEnumerable<Type?>
		=> OnlyUsedBy<TCollection, TCollection>(users, additional, items.Subject, items.ExpectItems);

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="AreOnlyUsedBy(IThat{IEnumerable{Type?}}, Filtered.Types, Filtered.Types[])" />
	[GuaranteesNotNull]
	public static OnlyUsedByResult<OnlyUsedByItemsResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?>>
		AreOnlyUsedBy(
			this IThat<IAsyncEnumerable<Type?>?> subject, Filtered.Types users, params Filtered.Types[] additional)
		=> OnlyUsedBy<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?>(users, additional, subject,
			expectation => subject.ExpectAll(expectation));

	/// <inheritdoc cref="AreOnlyUsedBy(IEnumerableItems{Type?}, Filtered.Types, Filtered.Types[])" />
	public static OnlyUsedByResult<OnlyUsedByItemsResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?>>
		AreOnlyUsedBy(
			this IAsyncEnumerableItems<Type?> items, Filtered.Types users, params Filtered.Types[] additional)
		=> OnlyUsedBy<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?>(users, additional, items.Subject,
			items.ExpectItems);
#endif

	private static OnlyUsedByResult<OnlyUsedByItemsResult<TType, TValue>> OnlyUsedBy<TType, TValue>(
		Filtered.Types users, Filtered.Types[] additional, IThat<TValue> subject,
		Func<ItemExpectation<Type>, ExpectationBuilder> expect)
	{
		TypeSetDependencyOptions allowed = TypeUsageOptions.CreateAllowed(users, additional);
		return new OnlyUsedByResult<OnlyUsedByItemsResult<TType, TValue>>(within =>
		{
			OptionsHolder<TypeUsageOptions> options = new(new TypeUsageOptions(allowed, within));
			return new OnlyUsedByItemsResult<TType, TValue>(expect(new AreOnlyUsedByExpectation(options)), subject,
				options);
		});
	}

	private sealed class AreOnlyUsedByExpectation(IReadOnlyOptionsHolder<TypeUsageOptions> options)
		: DependencyViolationsExpectation(false, " is used by ")
	{
		private HashSet<Type>? _items;

		public override bool NeedsAllItems => options.Value.ByEachOther;

		public override void OnEvaluating(IEvaluationContext context, CancellationToken cancellationToken)
		{
			base.OnEvaluating(context, cancellationToken);
			_items = null;
		}

		public override void OnAllItems(IReadOnlyCollection<Type> items)
			=> _items = [.. items.Select(TypeHelpers.StripElementTypes),];

		protected override async ValueTask<IReadOnlyList<string>> GetViolations(Type item,
			CancellationToken cancellationToken)
			=> (await options.Value.Resolve(cancellationToken)).GetDisallowedUsers(item, _items);

		public override void AppendExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars)
			=> stringBuilder.Append(grammars.IsPlural() ? "are only used by " : "is only used by ")
				.Append(options.Value.Describe());
	}
}
