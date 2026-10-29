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
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> are generic.
	/// </summary>
	[GuaranteesNotNull]
	public static GenericArgumentCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?> AreGeneric(
		this IThat<IEnumerable<MethodInfo?>?> subject)
	{
		AreGenericExpectation expectation = new();
		return new(subject.ExpectAll(expectation), subject, expectation.Options);
	}

	/// <summary>
	///     …are generic.
	/// </summary>
	public static GenericArgumentCollectionResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?> AreGeneric(
		this IEnumerableItems<MethodInfo?> items)
	{
		AreGenericExpectation expectation = new();
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}

	/// <summary>
	///     …are generic.
	/// </summary>
	public static GenericArgumentCollectionResult<TCollection, TCollection> AreGeneric<TCollection>(
		this IStructEnumerableItems<TCollection, MethodInfo?> items)
		where TCollection : struct, IEnumerable<MethodInfo?>
	{
		AreGenericExpectation expectation = new();
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> are generic.
	/// </summary>
	[GuaranteesNotNull]
	public static GenericArgumentCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?> AreGeneric(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject)
	{
		AreGenericExpectation expectation = new();
		return new(subject.ExpectAll(expectation), subject, expectation.Options);
	}

	/// <summary>
	///     …are generic.
	/// </summary>
	public static GenericArgumentCollectionResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?> AreGeneric(
		this IAsyncEnumerableItems<MethodInfo?> items)
	{
		AreGenericExpectation expectation = new();
		return new(items.ExpectItems(expectation), items.Subject, expectation.Options);
	}
#endif

	/// <summary>
	///     Verifies that no items in the filtered collection of <see cref="MethodInfo" /> are generic.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<MethodInfo?>, IThat<IEnumerable<MethodInfo?>?>> AreNotGeneric(
		this IThat<IEnumerable<MethodInfo?>?> subject)
		=> new(subject.ExpectNone(new AreGenericExpectation()), subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that no items in the filtered collection of <see cref="MethodInfo" /> are generic.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<MethodInfo?>, IThat<IAsyncEnumerable<MethodInfo?>?>> AreNotGeneric(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject)
		=> new(subject.ExpectNone(new AreGenericExpectation()), subject);
#endif

	private sealed class AreGenericExpectation : ItemExpectation<MethodInfo>
	{
		public OptionsHolder<GenericArgumentsFilterOptions> Options { get; } =
			new(GenericArgumentsFilterOptions.Default);

		public override string Verb => "were";

		public override async ValueTask<PredicateResult> IsMatch(MethodInfo item, CancellationToken cancellationToken)
			=> await Options.Value.Matches(item);

		public override void AppendExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars)
			=> stringBuilder.Append(grammars.IsPlural() ? "are generic" : "is generic")
				.Append(Options.Value.GetDescription());
	}
}
