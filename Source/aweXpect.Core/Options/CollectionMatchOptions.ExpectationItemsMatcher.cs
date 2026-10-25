using System.Threading.Tasks;
using aweXpect.Core;

namespace aweXpect.Options;

public readonly partial struct CollectionMatchOptions
{
	/// <summary>
	///     A collection matcher of <see cref="ExpectationItem{TItem}" /> items, whose expectations can stay undecided.
	/// </summary>
	internal interface IExpectationItemsMatcher
	{
		/// <summary>
		///     Whether the evaluation of an expected item was undecided, e.g. because it was canceled.
		/// </summary>
		bool HasUndecidedExpectation { get; }
	}

	/// <summary>
	///     Lets <see cref="CollectionMatcherExtensions.VerifyItems{TItem, TMatch}" /> tell a canceled item expectation
	///     from a mismatch.
	/// </summary>
	private sealed class ExpectationItemsMatcher<T, T2>(
		ICollectionMatcher<T, T2> matcher,
		ExpectationItem<T>[] expectations)
		: ICollectionMatcher<T, T2>, IExpectationItemsMatcher
		where T : T2
	{
		public bool HasUndecidedExpectation
		{
			get
			{
				foreach (ExpectationItem<T> expectation in expectations)
				{
					if (expectation.IsUndecided)
					{
						return true;
					}
				}

				return false;
			}
		}

		public bool IsDetermined => matcher.IsDetermined;

		public ValueTask<(bool IsFailure, string? Failure)> Verify(string it, T value, IEqualityEvaluator<T2> options,
			int maximumNumber)
			=> matcher.Verify(it, value, options, maximumNumber);

		public ValueTask<(bool IsFailure, string? Failure)> VerifyComplete(string it, IEqualityEvaluator<T2> options,
			int maximumNumber)
			=> matcher.VerifyComplete(it, options, maximumNumber);
	}
}
