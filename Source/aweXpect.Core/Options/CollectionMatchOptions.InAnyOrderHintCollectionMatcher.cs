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
		/// <remarks>
		///     Only kept when the in-order matcher does not keep the items itself.
		/// </remarks>
		private readonly List<T>? _values = inOrderMatcher is IRecordingCollectionMatcher<T> ? null : new();

		public bool IsDetermined => inOrderMatcher.IsDetermined;

		public ValueTask<(bool, string?)>
			Verify(string it, T value, IOptionsEquality<T2> options, int maximumNumber)
		{
			_values?.Add(value);
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

			return (isFailure, error + Environment.NewLine + ItemsMatchInADifferentOrderHint);
		}

		private async ValueTask<bool>
			MatchesInAnyOrder(string it, IOptionsEquality<T2> options, int maximumNumber)
		{
			ICollectionMatcher<T, T2> matcher = anyOrderMatcher();
			IReadOnlyList<T> values = _values ?? ((IRecordingCollectionMatcher<T>)inOrderMatcher).Values;
			foreach (T value in values)
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

	/// <summary>
	///     A matcher that keeps every item it verified, in the order of the subject.
	/// </summary>
	private interface IRecordingCollectionMatcher<out T>
	{
		IReadOnlyList<T> Values { get; }
	}
}
