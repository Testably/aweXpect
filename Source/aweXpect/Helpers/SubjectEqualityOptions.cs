using System;
using System.Threading.Tasks;
using aweXpect.Core;

namespace aweXpect.Helpers;

/// <summary>
///     Equality options that let the comparer of a collection subject decide, as long as the
///     <paramref name="options" /> do not change the comparison.
/// </summary>
/// <remarks>
///     The subject is only known during the evaluation, so <see cref="UseComparerOf" /> has to be called before
///     comparing, and the text of the options names the comparer only afterwards.
/// </remarks>
internal sealed class SubjectEqualityOptions<TItem, TMatch>(
	IOptionsEquality<TMatch> options,
	Func<bool> usesDefaultEquality)
	: IOptionsEquality<TMatch>, IOptionsProvider<IOptionsEquality<TMatch>>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	/// <remarks>
	///     Lets a failure message describe the expected items by the options that decide whenever the comparer of the
	///     subject does not.
	/// </remarks>
	public IOptionsEquality<TMatch> Options => options;

	/// <summary>
	///     The comparer of the subject that decides, or <see langword="null" /> when the options decide.
	/// </summary>
	public SubjectComparer<TItem>? Comparer { get; private set; }

	/// <inheritdoc cref="IOptionsEquality{TSubject}.AreConsideredEqual{TExpected}(TSubject, TExpected)" />
	public ValueTask<bool> AreConsideredEqual<TExpected>(TMatch actual, TExpected expected)
		=> Comparer is null
			? options.AreConsideredEqual(actual, expected)
			: new ValueTask<bool>(Comparer.AreEqual(actual, expected));

	/// <summary>
	///     Lets the comparer of the <paramref name="subject" /> decide, when it has one and the options do not change the
	///     comparison.
	/// </summary>
	/// <returns>
	///     <see langword="true" />, when the comparer of the <paramref name="subject" /> decides.
	/// </returns>
	public bool UseComparerOf(object? subject)
	{
		Comparer = usesDefaultEquality() ? CollectionComparerHelpers.GetSubjectComparer<TItem>(subject) : null;
		return Comparer is not null;
	}

	/// <inheritdoc cref="object.ToString()" />
	public override string ToString() => $"{options}{Comparer}";
}
