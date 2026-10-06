using System.Threading.Tasks;
using aweXpect.Core;

namespace aweXpect.Helpers;

/// <summary>
///     Lets the comparer of a collection subject decide how its items are compared.
/// </summary>
/// <remarks>
///     The constraints call it directly instead of receiving a delegate for it, because every expectation would
///     otherwise allocate that delegate.
/// </remarks>
internal interface ISubjectComparing
{
	/// <summary>
	///     Lets the comparer of the <paramref name="subject" /> decide, when it has one and the options do not change the
	///     comparison.
	/// </summary>
	/// <returns>
	///     <see langword="true" />, when the comparer of the <paramref name="subject" /> decides.
	/// </returns>
	bool UseComparerOf(object? subject);
}

/// <summary>
///     Equality options that let the comparer of a collection subject decide, as long as the options do not change the
///     comparison.
/// </summary>
/// <remarks>
///     The subject is only known during the evaluation, so <see cref="UseComparerOf" /> has to be called before
///     comparing, and the text of the options names the comparer only afterwards.
/// </remarks>
internal sealed class SubjectEqualityOptions<TItem, TMatch>
	: IOptionsEquality<TMatch>, IOptionsProvider<IOptionsEquality<TMatch>>, ISubjectComparing
{
	private readonly bool _canUseSubjectComparer;

	/// <summary>
	///     Lets the comparer of the subject decide, when <paramref name="canUseSubjectComparer" /> and the comparison of
	///     the <paramref name="options" /> was not changed when the subject is evaluated.
	/// </summary>
	public SubjectEqualityOptions(IOptionsEquality<TMatch> options, bool canUseSubjectComparer = true)
	{
		Options = options;
		_canUseSubjectComparer = canUseSubjectComparer;
	}

	/// <summary>
	///     Lets the <paramref name="comparer" /> decide, which was already read from the subject.
	/// </summary>
	public SubjectEqualityOptions(IOptionsEquality<TMatch> options, SubjectComparer<TItem> comparer)
	{
		Options = options;
		Comparer = comparer;
	}

	/// <summary>
	///     The comparer of the subject that decides, or <see langword="null" /> when the options decide.
	/// </summary>
	public SubjectComparer<TItem>? Comparer { get; private set; }

	/// <inheritdoc cref="IOptionsEquality{TSubject}.AreConsideredEqual{TExpected}(TSubject, TExpected)" />
	public ValueTask<bool> AreConsideredEqual<TExpected>(TMatch actual, TExpected expected)
		=> Comparer is null
			? Options.AreConsideredEqual(actual, expected)
			: new ValueTask<bool>(Comparer.AreEqual(actual, expected));

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	/// <remarks>
	///     Lets a failure message describe the expected items by the options that decide whenever the comparer of the
	///     subject does not.
	/// </remarks>
	public IOptionsEquality<TMatch> Options { get; }

	/// <inheritdoc cref="ISubjectComparing.UseComparerOf(object?)" />
	public bool UseComparerOf(object? subject)
	{
		Comparer = _canUseSubjectComparer && DefaultEquality.IsUsedBy(Options)
			? CollectionComparerHelpers.GetSubjectComparer<TItem>(subject)
			: null;
		return Comparer is not null;
	}

	/// <inheritdoc cref="object.ToString()" />
	public override string ToString() => $"{Options}{Comparer}";
}
