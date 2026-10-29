using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;

namespace aweXpect;

public static partial class ThatTypes
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> depend on (reference in their
	///     signature) at least one type in one of the <paramref name="namespaces" /> (including sub-namespaces).
	/// </summary>
	[GuaranteesNotNull]
	public static NamespaceDependencyResult<IEnumerable<Type?>, IEnumerable<Type?>?> DependOn(
		this IThat<IEnumerable<Type?>?> subject, params string[] namespaces)
		=> subject.DependOn(namespaces.AsEnumerable());

	/// <inheritdoc cref="DependOn(IThat{IEnumerable{Type?}}, string[])" />
	[GuaranteesNotNull]
	public static NamespaceDependencyResult<IEnumerable<Type?>, IEnumerable<Type?>?> DependOn(
		this IThat<IEnumerable<Type?>?> subject, IEnumerable<string> namespaces)
	{
		OptionsHolder<NamespaceDependencyOptions> options = new(new NamespaceDependencyOptions(namespaces));
		return new(subject.ExpectAll(new DependOnExpectation(options)), subject, options);
	}

	/// <summary>
	///     …depend on (reference in their signature) at least one type in one of the <paramref name="namespaces" />
	///     (including sub-namespaces).
	/// </summary>
	public static NamespaceDependencyResult<IEnumerable<Type?>, IEnumerable<Type?>?> DependOn(
		this IEnumerableItems<Type?> items, params string[] namespaces)
		=> items.DependOn(namespaces.AsEnumerable());

	/// <inheritdoc cref="DependOn(IEnumerableItems{Type?}, string[])" />
	public static NamespaceDependencyResult<IEnumerable<Type?>, IEnumerable<Type?>?> DependOn(
		this IEnumerableItems<Type?> items, IEnumerable<string> namespaces)
	{
		OptionsHolder<NamespaceDependencyOptions> options = new(new NamespaceDependencyOptions(namespaces));
		return new(items.ExpectItems(new DependOnExpectation(options)), items.Subject, options);
	}

	/// <inheritdoc cref="DependOn(IEnumerableItems{Type?}, string[])" />
	public static NamespaceDependencyResult<TCollection, TCollection> DependOn<TCollection>(
		this IStructEnumerableItems<TCollection, Type?> items, params string[] namespaces)
		where TCollection : struct, IEnumerable<Type?>
		=> items.DependOn(namespaces.AsEnumerable());

	/// <inheritdoc cref="DependOn(IEnumerableItems{Type?}, string[])" />
	public static NamespaceDependencyResult<TCollection, TCollection> DependOn<TCollection>(
		this IStructEnumerableItems<TCollection, Type?> items, IEnumerable<string> namespaces)
		where TCollection : struct, IEnumerable<Type?>
	{
		OptionsHolder<NamespaceDependencyOptions> options = new(new NamespaceDependencyOptions(namespaces));
		return new(items.ExpectItems(new DependOnExpectation(options)), items.Subject, options);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="DependOn(IThat{IEnumerable{Type?}}, string[])" />
	[GuaranteesNotNull]
	public static NamespaceDependencyResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DependOn(
		this IThat<IAsyncEnumerable<Type?>?> subject, params string[] namespaces)
		=> subject.DependOn(namespaces.AsEnumerable());

	/// <inheritdoc cref="DependOn(IThat{IEnumerable{Type?}}, string[])" />
	[GuaranteesNotNull]
	public static NamespaceDependencyResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DependOn(
		this IThat<IAsyncEnumerable<Type?>?> subject, IEnumerable<string> namespaces)
	{
		OptionsHolder<NamespaceDependencyOptions> options = new(new NamespaceDependencyOptions(namespaces));
		return new(subject.ExpectAll(new DependOnExpectation(options)), subject, options);
	}

	/// <inheritdoc cref="DependOn(IEnumerableItems{Type?}, string[])" />
	public static NamespaceDependencyResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DependOn(
		this IAsyncEnumerableItems<Type?> items, params string[] namespaces)
		=> items.DependOn(namespaces.AsEnumerable());

	/// <inheritdoc cref="DependOn(IEnumerableItems{Type?}, string[])" />
	public static NamespaceDependencyResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DependOn(
		this IAsyncEnumerableItems<Type?> items, IEnumerable<string> namespaces)
	{
		OptionsHolder<NamespaceDependencyOptions> options = new(new NamespaceDependencyOptions(namespaces));
		return new(items.ExpectItems(new DependOnExpectation(options)), items.Subject, options);
	}
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> depend on (reference in their
	///     signature) the type <typeparamref name="T" />.
	/// </summary>
	[GuaranteesNotNull]
	public static TypeDependencyResult<IEnumerable<Type?>, IEnumerable<Type?>?> DependOn<T>(
		this IThat<IEnumerable<Type?>?> subject)
		=> subject.DependOn(typeof(T));

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> depend on (reference in their
	///     signature) the <paramref name="type" />.
	/// </summary>
	[GuaranteesNotNull]
	public static TypeDependencyResult<IEnumerable<Type?>, IEnumerable<Type?>?> DependOn(
		this IThat<IEnumerable<Type?>?> subject, Type type)
	{
		OptionsHolder<TypeDependencyOptions> options = new(new TypeDependencyOptions(type));
		return new(subject.ExpectAll(new DependOnExpectation(options)), subject, options);
	}

	/// <summary>
	///     …depend on (reference in their signature) the type <typeparamref name="T" />.
	/// </summary>
	public static TypeDependencyResult<IEnumerable<Type?>, IEnumerable<Type?>?> DependOn<T>(
		this IEnumerableItems<Type?> items)
		=> items.DependOn(typeof(T));

	/// <summary>
	///     …depend on (reference in their signature) the <paramref name="type" />.
	/// </summary>
	public static TypeDependencyResult<IEnumerable<Type?>, IEnumerable<Type?>?> DependOn(
		this IEnumerableItems<Type?> items, Type type)
	{
		OptionsHolder<TypeDependencyOptions> options = new(new TypeDependencyOptions(type));
		return new(items.ExpectItems(new DependOnExpectation(options)), items.Subject, options);
	}

	/// <inheritdoc cref="DependOn(IEnumerableItems{Type?}, Type)" />
	public static TypeDependencyResult<TCollection, TCollection> DependOn<TCollection>(
		this IStructEnumerableItems<TCollection, Type?> items, Type type)
		where TCollection : struct, IEnumerable<Type?>
	{
		OptionsHolder<TypeDependencyOptions> options = new(new TypeDependencyOptions(type));
		return new(items.ExpectItems(new DependOnExpectation(options)), items.Subject, options);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="DependOn{T}(IThat{IEnumerable{Type?}})" />
	[GuaranteesNotNull]
	public static TypeDependencyResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DependOn<T>(
		this IThat<IAsyncEnumerable<Type?>?> subject)
		=> subject.DependOn(typeof(T));

	/// <inheritdoc cref="DependOn(IThat{IEnumerable{Type?}}, Type)" />
	[GuaranteesNotNull]
	public static TypeDependencyResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DependOn(
		this IThat<IAsyncEnumerable<Type?>?> subject, Type type)
	{
		OptionsHolder<TypeDependencyOptions> options = new(new TypeDependencyOptions(type));
		return new(subject.ExpectAll(new DependOnExpectation(options)), subject, options);
	}

	/// <inheritdoc cref="DependOn{T}(IEnumerableItems{Type?})" />
	public static TypeDependencyResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DependOn<T>(
		this IAsyncEnumerableItems<Type?> items)
		=> items.DependOn(typeof(T));

	/// <inheritdoc cref="DependOn(IEnumerableItems{Type?}, Type)" />
	public static TypeDependencyResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DependOn(
		this IAsyncEnumerableItems<Type?> items, Type type)
	{
		OptionsHolder<TypeDependencyOptions> options = new(new TypeDependencyOptions(type));
		return new(items.ExpectItems(new DependOnExpectation(options)), items.Subject, options);
	}
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> depend on (reference in their
	///     signature) at least one type in the filtered collections of types <paramref name="target" /> or
	///     <paramref name="additional" />.
	/// </summary>
	/// <remarks>
	///     The target collections are resolved once per assertion; a dependency matches when it is a member of the
	///     union of the resolved collections (by <see cref="Type" /> identity; a generic type definition in a
	///     collection matches any construction of it).
	/// </remarks>
	[GuaranteesNotNull]
	public static TypeSetDependencyResult<IEnumerable<Type?>, IEnumerable<Type?>?> DependOn(
		this IThat<IEnumerable<Type?>?> subject, Filtered.Types target, params Filtered.Types[] additional)
	{
		OptionsHolder<TypeSetDependencyOptions> options = new(new TypeSetDependencyOptions(target, additional));
		return new(subject.ExpectAll(new DependOnTypeSetExpectation(options)), subject, options);
	}

	/// <summary>
	///     …depend on (reference in their signature) at least one type in the filtered collections of types
	///     <paramref name="target" /> or <paramref name="additional" />.
	/// </summary>
	/// <inheritdoc cref="DependOn(IThat{IEnumerable{Type?}}, Filtered.Types, Filtered.Types[])" path="/remarks" />
	public static TypeSetDependencyResult<IEnumerable<Type?>, IEnumerable<Type?>?> DependOn(
		this IEnumerableItems<Type?> items, Filtered.Types target, params Filtered.Types[] additional)
	{
		OptionsHolder<TypeSetDependencyOptions> options = new(new TypeSetDependencyOptions(target, additional));
		return new(items.ExpectItems(new DependOnTypeSetExpectation(options)), items.Subject, options);
	}

	/// <inheritdoc cref="DependOn(IEnumerableItems{Type?}, Filtered.Types, Filtered.Types[])" />
	public static TypeSetDependencyResult<TCollection, TCollection> DependOn<TCollection>(
		this IStructEnumerableItems<TCollection, Type?> items, Filtered.Types target,
		params Filtered.Types[] additional)
		where TCollection : struct, IEnumerable<Type?>
	{
		OptionsHolder<TypeSetDependencyOptions> options = new(new TypeSetDependencyOptions(target, additional));
		return new(items.ExpectItems(new DependOnTypeSetExpectation(options)), items.Subject, options);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="DependOn(IThat{IEnumerable{Type?}}, Filtered.Types, Filtered.Types[])" />
	[GuaranteesNotNull]
	public static TypeSetDependencyResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DependOn(
		this IThat<IAsyncEnumerable<Type?>?> subject, Filtered.Types target, params Filtered.Types[] additional)
	{
		OptionsHolder<TypeSetDependencyOptions> options = new(new TypeSetDependencyOptions(target, additional));
		return new(subject.ExpectAll(new DependOnTypeSetExpectation(options)), subject, options);
	}

	/// <inheritdoc cref="DependOn(IEnumerableItems{Type?}, Filtered.Types, Filtered.Types[])" />
	public static TypeSetDependencyResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DependOn(
		this IAsyncEnumerableItems<Type?> items, Filtered.Types target, params Filtered.Types[] additional)
	{
		OptionsHolder<TypeSetDependencyOptions> options = new(new TypeSetDependencyOptions(target, additional));
		return new(items.ExpectItems(new DependOnTypeSetExpectation(options)), items.Subject, options);
	}
#endif

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> depend on (reference in
	///     their signature) any type in one of the <paramref name="namespaces" /> (including sub-namespaces).
	/// </summary>
	[GuaranteesNotNull]
	public static NamespaceDependencyResult<IEnumerable<Type?>, IEnumerable<Type?>?> DoNotDependOn(
		this IThat<IEnumerable<Type?>?> subject, params string[] namespaces)
		=> subject.DoNotDependOn(namespaces.AsEnumerable());

	/// <inheritdoc cref="DoNotDependOn(IThat{IEnumerable{Type?}}, string[])" />
	[GuaranteesNotNull]
	public static NamespaceDependencyResult<IEnumerable<Type?>, IEnumerable<Type?>?> DoNotDependOn(
		this IThat<IEnumerable<Type?>?> subject, IEnumerable<string> namespaces)
	{
		OptionsHolder<NamespaceDependencyOptions> options = new(new NamespaceDependencyOptions(namespaces));
		return new(subject.ExpectNone(new DependOnExpectation(options)), subject, options);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="DoNotDependOn(IThat{IEnumerable{Type?}}, string[])" />
	[GuaranteesNotNull]
	public static NamespaceDependencyResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DoNotDependOn(
		this IThat<IAsyncEnumerable<Type?>?> subject, params string[] namespaces)
		=> subject.DoNotDependOn(namespaces.AsEnumerable());

	/// <inheritdoc cref="DoNotDependOn(IThat{IEnumerable{Type?}}, string[])" />
	[GuaranteesNotNull]
	public static NamespaceDependencyResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DoNotDependOn(
		this IThat<IAsyncEnumerable<Type?>?> subject, IEnumerable<string> namespaces)
	{
		OptionsHolder<NamespaceDependencyOptions> options = new(new NamespaceDependencyOptions(namespaces));
		return new(subject.ExpectNone(new DependOnExpectation(options)), subject, options);
	}
#endif

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> depend on (reference in
	///     their signature) the type <typeparamref name="T" />.
	/// </summary>
	[GuaranteesNotNull]
	public static TypeDependencyResult<IEnumerable<Type?>, IEnumerable<Type?>?> DoNotDependOn<T>(
		this IThat<IEnumerable<Type?>?> subject)
		=> subject.DoNotDependOn(typeof(T));

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> depend on (reference in
	///     their signature) the <paramref name="type" />.
	/// </summary>
	[GuaranteesNotNull]
	public static TypeDependencyResult<IEnumerable<Type?>, IEnumerable<Type?>?> DoNotDependOn(
		this IThat<IEnumerable<Type?>?> subject, Type type)
	{
		OptionsHolder<TypeDependencyOptions> options = new(new TypeDependencyOptions(type));
		return new(subject.ExpectNone(new DependOnExpectation(options)), subject, options);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="DoNotDependOn{T}(IThat{IEnumerable{Type?}})" />
	[GuaranteesNotNull]
	public static TypeDependencyResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DoNotDependOn<T>(
		this IThat<IAsyncEnumerable<Type?>?> subject)
		=> subject.DoNotDependOn(typeof(T));

	/// <inheritdoc cref="DoNotDependOn(IThat{IEnumerable{Type?}}, Type)" />
	[GuaranteesNotNull]
	public static TypeDependencyResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DoNotDependOn(
		this IThat<IAsyncEnumerable<Type?>?> subject, Type type)
	{
		OptionsHolder<TypeDependencyOptions> options = new(new TypeDependencyOptions(type));
		return new(subject.ExpectNone(new DependOnExpectation(options)), subject, options);
	}
#endif

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> depend on (reference in
	///     their signature) any type in the filtered collections of types <paramref name="target" /> or
	///     <paramref name="additional" />.
	/// </summary>
	/// <remarks>
	///     The target collections are resolved once per assertion; a dependency matches when it is a member of the
	///     union of the resolved collections (by <see cref="Type" /> identity; a generic type definition in a
	///     collection matches any construction of it).
	/// </remarks>
	[GuaranteesNotNull]
	public static TypeSetDependencyResult<IEnumerable<Type?>, IEnumerable<Type?>?> DoNotDependOn(
		this IThat<IEnumerable<Type?>?> subject, Filtered.Types target, params Filtered.Types[] additional)
	{
		OptionsHolder<TypeSetDependencyOptions> options = new(new TypeSetDependencyOptions(target, additional));
		return new(subject.ExpectNone(new DependOnTypeSetExpectation(options)), subject, options);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="DoNotDependOn(IThat{IEnumerable{Type?}}, Filtered.Types, Filtered.Types[])" />
	[GuaranteesNotNull]
	public static TypeSetDependencyResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DoNotDependOn(
		this IThat<IAsyncEnumerable<Type?>?> subject, Filtered.Types target, params Filtered.Types[] additional)
	{
		OptionsHolder<TypeSetDependencyOptions> options = new(new TypeSetDependencyOptions(target, additional));
		return new(subject.ExpectNone(new DependOnTypeSetExpectation(options)), subject, options);
	}
#endif

	private sealed class DependOnExpectation(IReadOnlyOptionsHolder<IDependencyTarget> target)
		: ItemExpectation<Type>
	{
		public override ValueTask<PredicateResult> IsMatch(Type item, CancellationToken cancellationToken)
			=> new(target.Value.IsMatchedBy(item));

		public override void AppendExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars)
			=> stringBuilder.Append(grammars.IsPlural() ? "depend on " : "depends on ")
				.Append(target.Value.Describe());
	}

	private sealed class DependOnTypeSetExpectation(IReadOnlyOptionsHolder<TypeSetDependencyOptions> targets)
		: ItemExpectation<Type>
	{
		public override async ValueTask<PredicateResult> IsMatch(Type item, CancellationToken cancellationToken)
		{
			ResolvedTypeSet targetSet = await targets.Value.Resolve(cancellationToken);
			return targetSet.IsMatchedBy(item);
		}

		public override void AppendExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars)
			=> stringBuilder.Append(grammars.IsPlural() ? "depend on " : "depends on ")
				.Append(targets.Value.Describe());
	}
}
