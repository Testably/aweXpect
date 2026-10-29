using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatMethods
{
	/// <summary>
	///     Verifies that all methods in the filtered collection return type <typeparamref name="TReturn" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ReturnResult<IEnumerable<MethodInfo?>> Return<TReturn>(
		this IThat<IEnumerable<MethodInfo?>?> subject)
		=> Return(subject, typeof(TReturn));

	/// <summary>
	///     Verifies that all methods in the filtered collection return type <paramref name="returnType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ReturnResult<IEnumerable<MethodInfo?>> Return(
		this IThat<IEnumerable<MethodInfo?>?> subject, Type returnType)
	{
		ReturnExpectation expectation = new(returnType, false);
		return new(subject.ExpectAll(expectation), subject, expectation.Options);
	}

	/// <summary>
	///     …return type <typeparamref name="TReturn" />.
	/// </summary>
	public static ReturnResult<IEnumerable<MethodInfo?>> Return<TReturn>(
		this IEnumerableItems<MethodInfo?> items)
		=> items.Return(typeof(TReturn));

	/// <summary>
	///     …return type <paramref name="returnType" />.
	/// </summary>
	public static ReturnResult<IEnumerable<MethodInfo?>> Return(
		this IEnumerableItems<MethodInfo?> items, Type returnType)
	{
		ReturnExpectation expectation = new(returnType, false);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}

	/// <inheritdoc cref="Return(IEnumerableItems{MethodInfo}, Type)" />
	public static ReturnResult<TCollection> Return<TCollection>(
		this IStructEnumerableItems<TCollection, MethodInfo?> items, Type returnType)
		where TCollection : struct, IEnumerable<MethodInfo?>
	{
		ReturnExpectation expectation = new(returnType, false);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all methods in the filtered collection return type <typeparamref name="TReturn" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ReturnResult<IAsyncEnumerable<MethodInfo?>> Return<TReturn>(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject)
		=> Return(subject, typeof(TReturn));

	/// <summary>
	///     Verifies that all methods in the filtered collection return type <paramref name="returnType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static ReturnResult<IAsyncEnumerable<MethodInfo?>> Return(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, Type returnType)
	{
		ReturnExpectation expectation = new(returnType, false);
		return new(subject.ExpectAll(expectation), subject, expectation.Options);
	}

	/// <inheritdoc cref="Return{TReturn}(IEnumerableItems{MethodInfo})" />
	public static ReturnResult<IAsyncEnumerable<MethodInfo?>> Return<TReturn>(
		this IAsyncEnumerableItems<MethodInfo?> items)
		=> items.Return(typeof(TReturn));

	/// <inheritdoc cref="Return(IEnumerableItems{MethodInfo}, Type)" />
	public static ReturnResult<IAsyncEnumerable<MethodInfo?>> Return(
		this IAsyncEnumerableItems<MethodInfo?> items, Type returnType)
	{
		ReturnExpectation expectation = new(returnType, false);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}
#endif

	/// <summary>
	///     Verifies that none of the methods in the filtered collection return type <typeparamref name="TReturn" /> (or a
	///     subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<MethodInfo?>, IThat<IEnumerable<MethodInfo?>?>> DoNotReturn<TReturn>(
		this IThat<IEnumerable<MethodInfo?>?> subject)
		=> DoNotReturn(subject, typeof(TReturn));

	/// <summary>
	///     Verifies that none of the methods in the filtered collection return type <paramref name="returnType" /> (or a
	///     subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<MethodInfo?>, IThat<IEnumerable<MethodInfo?>?>> DoNotReturn(
		this IThat<IEnumerable<MethodInfo?>?> subject, Type returnType)
		=> new(subject.ExpectNone(new ReturnExpectation(returnType, false)), subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that none of the methods in the filtered collection return type <typeparamref name="TReturn" /> (or a
	///     subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<MethodInfo?>, IThat<IAsyncEnumerable<MethodInfo?>?>> DoNotReturn<TReturn>(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject)
		=> DoNotReturn(subject, typeof(TReturn));

	/// <summary>
	///     Verifies that none of the methods in the filtered collection return type <paramref name="returnType" /> (or a
	///     subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<MethodInfo?>, IThat<IAsyncEnumerable<MethodInfo?>?>> DoNotReturn(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, Type returnType)
		=> new(subject.ExpectNone(new ReturnExpectation(returnType, false)), subject);
#endif

	private sealed class ReturnExpectation(Type returnType, bool exactly) : ItemExpectation<MethodInfo>
	{
		public OptionsHolder<TypeFilterOptions> Options { get; } =
			new(TypeFilterOptions.Default.WithType(returnType, exactly));

		public override ValueTask<PredicateResult> IsMatch(MethodInfo item, CancellationToken cancellationToken)
			=> new(Options.Value.Matches(item.ReturnType));

		public override void AppendExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars)
			=> Options.Value.AppendExpectation(stringBuilder,
				grammars.IsPlural() ? ExpectationGrammars.Plural : ExpectationGrammars.None);
	}
}
