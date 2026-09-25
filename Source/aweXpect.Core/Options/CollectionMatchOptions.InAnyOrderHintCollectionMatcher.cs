using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using aweXpect.Core;

namespace aweXpect.Options;

public partial class CollectionMatchOptions
{
	/// <summary>
	///     Appends a hint to the failure of the <paramref name="inOrderMatcher" />, when the same items match in any
	///     order.
	/// </summary>
	private sealed class InAnyOrderHintCollectionMatcher<T, T2>(
		ICollectionMatcher<T, T2> inOrderMatcher,
		Func<ICollectionMatcher<T, T2>> anyOrderMatcher)
		: ICollectionMatcher<T, T2>
		where T : T2
	{
		private const string Hint = "(but the items match in a different order)";
		private readonly List<T> _values = new();

		public ValueTask<(bool, string?)>
			Verify(string it, T value, IOptionsEquality<T2> options, int maximumNumber)
		{
			_values.Add(value);
			return inOrderMatcher.Verify(it, value, options, maximumNumber);
		}

		public async ValueTask<(bool, string?)>
			VerifyComplete(string it, IOptionsEquality<T2> options, int maximumNumber)
		{
			(bool isFailure, string? error) = await inOrderMatcher.VerifyComplete(it, options, maximumNumber);
			if (error is null || !await MatchesInAnyOrder(it, options, maximumNumber))
			{
				return (isFailure, error);
			}

			return (isFailure, error + Environment.NewLine + Hint);
		}

		private async ValueTask<bool>
			MatchesInAnyOrder(string it, IOptionsEquality<T2> options, int maximumNumber)
		{
			ICollectionMatcher<T, T2> matcher = anyOrderMatcher();
			foreach (T value in _values)
			{
				(bool isFailure, string? _) = await matcher.Verify(it, value, options, maximumNumber);
				if (isFailure)
				{
					return false;
				}
			}

			(bool isCompleteFailure, string? _) = await matcher.VerifyComplete(it, options, maximumNumber);
			return !isCompleteFailure;
		}
	}
}
