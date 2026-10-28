using System;
using System.Collections.Generic;
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

public static partial class ThatTypes
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are generic.
	/// </summary>
	[GuaranteesNotNull]
	public static GenericArgumentCollectionResult<IEnumerable<Type?>, IEnumerable<Type?>?> AreGeneric(
		this IThat<IEnumerable<Type?>?> subject)
	{
		AreGenericExpectation expectation = new();
		return new(subject.ExpectAll(expectation), subject, expectation.Options);
	}

	/// <summary>
	///     …are generic.
	/// </summary>
	public static GenericArgumentCollectionResult<IEnumerable<Type?>, IEnumerable<Type?>?> AreGeneric(
		this IEnumerableItems<Type?> items)
	{
		AreGenericExpectation expectation = new();
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}

	/// <summary>
	///     …are generic.
	/// </summary>
	public static GenericArgumentCollectionResult<TCollection, TCollection> AreGeneric<TCollection>(
		this IStructEnumerableItems<TCollection, Type?> items)
		where TCollection : struct, IEnumerable<Type?>
	{
		AreGenericExpectation expectation = new();
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are generic.
	/// </summary>
	[GuaranteesNotNull]
	public static GenericArgumentCollectionResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> AreGeneric(
		this IThat<IAsyncEnumerable<Type?>?> subject)
	{
		AreGenericExpectation expectation = new();
		return new(subject.ExpectAll(expectation), subject, expectation.Options);
	}

	/// <summary>
	///     …are generic.
	/// </summary>
	public static GenericArgumentCollectionResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> AreGeneric(
		this IAsyncEnumerableItems<Type?> items)
	{
		AreGenericExpectation expectation = new();
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}
#endif

	/// <summary>
	///     Verifies that no items in the filtered collection of <see cref="Type" /> are generic.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> AreNotGeneric(
		this IThat<IEnumerable<Type?>?> subject)
		=> new(subject.ExpectNone(new AreGenericExpectation()), subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that no items in the filtered collection of <see cref="Type" /> are generic.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> AreNotGeneric(
		this IThat<IAsyncEnumerable<Type?>?> subject)
		=> new(subject.ExpectNone(new AreGenericExpectation()), subject);
#endif

	private sealed class AreGenericExpectation : ItemExpectation<Type>
	{
		public OptionsHolder<GenericArgumentsFilterOptions> Options { get; } =
			new(GenericArgumentsFilterOptions.Default);

		public override string Verb => "were";

		public override async ValueTask<PredicateResult> IsMatch(Type item, CancellationToken cancellationToken)
			=> await Options.Value.Matches(item);

		public override void AppendExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars)
			=> stringBuilder.Append(grammars.IsPlural() ? "are generic" : "is generic")
				.Append(Options.Value.GetDescription());
	}
}
