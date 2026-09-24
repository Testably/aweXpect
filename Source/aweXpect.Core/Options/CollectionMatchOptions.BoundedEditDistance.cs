using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace aweXpect.Options;

public partial class CollectionMatchOptions
{
	private enum EditKind
	{
		Incorrect,
		Additional,
		Missing,
	}

	/// <summary>
	///     The edit distance between the subject and the expected items, which is only computed up to a maximum number
	///     of edits, so that only a band of cells around the positional alignment is needed.
	/// </summary>
	/// <remarks>
	///     It starts at the first positional deviation: the items before it match the expected items at their position,
	///     so their distance to a prefix of the expected items is the difference in length.
	/// </remarks>
	private sealed class BoundedEditDistance<T, T3>
	{
		private readonly T3[] _expectedItems;
		private readonly int _maximumEdits;
		private readonly List<byte[]> _rows = new();
		private readonly int _start;
		private readonly int _unreachable;
		private readonly int _width;

		public BoundedEditDistance(T3[] expectedItems, int maximumEdits, int start)
		{
			_expectedItems = expectedItems;
			// The distances are stored as bytes, which bounds the band.
			_maximumEdits = Math.Min(maximumEdits, byte.MaxValue - 1);
			_unreachable = _maximumEdits + 1;
			_width = (2 * _maximumEdits) + 1;
			_start = start;
			byte[] row = CreateRow();
			for (int offset = 0; offset < _width; offset++)
			{
				int expectedIndex = start - _maximumEdits + offset;
				if (expectedIndex >= 0 && expectedIndex <= expectedItems.Length)
				{
					row[offset] = (byte)Math.Abs(start - expectedIndex);
				}
			}

			_rows.Add(row);
		}

		/// <summary>
		///     Adds the next subject item.
		/// </summary>
		/// <returns>
		///     <see langword="true" />, when the subject can still be aligned within the maximum number of edits.
		/// </returns>
#if NET8_0_OR_GREATER
		public async ValueTask<bool> Add(T value, Func<T, T3, ValueTask<bool>> areConsideredEqual)
#else
		public async Task<bool> Add(T value, Func<T, T3, Task<bool>> areConsideredEqual)
#endif
		{
			int subjectIndex = _start + _rows.Count - 1;
			byte[] previous = _rows[_rows.Count - 1];
			byte[] row = CreateRow();
			int minimum = _unreachable;
			for (int offset = 0; offset < _width; offset++)
			{
				int expectedIndex = subjectIndex + 1 - _maximumEdits + offset;
				if (expectedIndex < 0 || expectedIndex > _expectedItems.Length)
				{
					continue;
				}

				int distance = _unreachable;
				if (offset + 1 < _width)
				{
					distance = previous[offset + 1] + 1;
				}

				if (offset > 0)
				{
					distance = Math.Min(distance, row[offset - 1] + 1);
				}

				// The comparison is only needed when aligning the item can beat the other two edits.
				if (expectedIndex > 0 && previous[offset] < distance)
				{
					bool isEqual = await areConsideredEqual(value, _expectedItems[expectedIndex - 1]);
					distance = Math.Min(distance, previous[offset] + (isEqual ? 0 : 1));
				}

				row[offset] = (byte)Math.Min(distance, _unreachable);
				minimum = Math.Min(minimum, row[offset]);
			}

			_rows.Add(row);
			return minimum <= _maximumEdits;
		}

		/// <summary>
		///     Traces back the alignment with the fewest edits, preferring to align an item with an expected item over
		///     treating it as additional or missing.
		/// </summary>
		/// <returns>
		///     The edits in the order of the subject, or <see langword="null" /> when more than the maximum number of edits
		///     are needed.
		/// </returns>
#if NET8_0_OR_GREATER
		public async ValueTask<List<(EditKind Kind, int SubjectIndex, int ExpectedIndex)>?>
			GetEdits(List<T> values, Func<T, T3, ValueTask<bool>> areConsideredEqual)
#else
		public async Task<List<(EditKind Kind, int SubjectIndex, int ExpectedIndex)>?>
			GetEdits(List<T> values, Func<T, T3, Task<bool>> areConsideredEqual)
#endif
		{
			int subjectIndex = _start + _rows.Count - 1;
			int expectedIndex = _expectedItems.Length;
			int finalOffset = expectedIndex - subjectIndex + _maximumEdits;
			if (finalOffset < 0 || finalOffset >= _width || _rows[_rows.Count - 1][finalOffset] > _maximumEdits)
			{
				return null;
			}

			List<(EditKind Kind, int SubjectIndex, int ExpectedIndex)> edits = new();
			while (subjectIndex > _start)
			{
				EditKind? edit = await TraceBackOneStep(values, subjectIndex, expectedIndex, areConsideredEqual);
				switch (edit)
				{
					case EditKind.Additional:
						edits.Add((EditKind.Additional, subjectIndex - 1, -1));
						subjectIndex--;
						break;
					case EditKind.Missing:
						edits.Add((EditKind.Missing, -1, expectedIndex - 1));
						expectedIndex--;
						break;
					default:
						if (edit == EditKind.Incorrect)
						{
							edits.Add((EditKind.Incorrect, subjectIndex - 1, expectedIndex - 1));
						}

						subjectIndex--;
						expectedIndex--;
						break;
				}
			}

			for (int index = _start - 1; index >= expectedIndex; index--)
			{
				edits.Add((EditKind.Additional, index, -1));
			}

			for (int index = expectedIndex - 1; index >= _start; index--)
			{
				edits.Add((EditKind.Missing, -1, index));
			}

			edits.Reverse();
			return edits;
		}

		/// <summary>
		///     Finds the cell in the previous row from which the distance of the current cell was computed.
		/// </summary>
		/// <returns>
		///     The edit of this step, or <see langword="null" /> when the item matches the expected item it is aligned
		///     with.
		/// </returns>
#if NET8_0_OR_GREATER
		private async ValueTask<EditKind?> TraceBackOneStep(List<T> values, int subjectIndex, int expectedIndex,
			Func<T, T3, ValueTask<bool>> areConsideredEqual)
#else
		private async Task<EditKind?> TraceBackOneStep(List<T> values, int subjectIndex, int expectedIndex,
			Func<T, T3, Task<bool>> areConsideredEqual)
#endif
		{
			byte[] row = _rows[subjectIndex - _start];
			byte[] previous = _rows[subjectIndex - _start - 1];
			int offset = expectedIndex - subjectIndex + _maximumEdits;
			if (expectedIndex > 0 && previous[offset] < _unreachable)
			{
				bool isEqual = await areConsideredEqual(values[subjectIndex - 1], _expectedItems[expectedIndex - 1]);
				if (previous[offset] + (isEqual ? 0 : 1) == row[offset])
				{
					return isEqual ? null : EditKind.Incorrect;
				}
			}

			return offset + 1 < _width && previous[offset + 1] + 1 == row[offset]
				? EditKind.Additional
				: EditKind.Missing;
		}

		private byte[] CreateRow()
		{
			byte[] row = new byte[_width];
			for (int offset = 0; offset < _width; offset++)
			{
				row[offset] = (byte)_unreachable;
			}

			return row;
		}
	}
}
