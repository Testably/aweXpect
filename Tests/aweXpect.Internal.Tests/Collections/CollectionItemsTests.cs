using System.Collections;
using System.Collections.Generic;

namespace aweXpect.Internal.Tests.Collections;

public sealed class CollectionItemsTests
{
	[Fact]
	public async Task Enumerate_WhenArray_ShouldReturnAllItems()
	{
		int[] subject = [1, 2, 3,];

		List<int> result = Enumerate(CollectionItems<int>.Of<IEnumerable<int>>(subject));

		await That(result).IsEqualTo([1, 2, 3,]);
	}

	[Fact]
	public async Task Enumerate_WhenEmptyArray_ShouldReturnNoItems()
	{
		int[] subject = [];

		List<int> result = Enumerate(CollectionItems<int>.Of<IEnumerable<int>>(subject));

		await That(result).IsEmpty();
	}

	[Fact]
	public async Task Enumerate_WhenList_ShouldReturnAllItems()
	{
		List<int> subject = [1, 2, 3,];

		List<int> result = Enumerate(CollectionItems<int>.Of<IEnumerable<int>>(subject));

		await That(result).IsEqualTo([1, 2, 3,]);
	}

	[Fact]
	public async Task Enumerate_WhenListIsModified_ShouldThrowLikeTheListItself()
	{
		string expectedMessage = GetMessageOfModifiedList();
		List<int> subject = [1, 2, 3,];

		void Act()
		{
			foreach (int _ in CollectionItems<int>.Of<IEnumerable<int>>(subject))
			{
				subject.Add(4);
			}
		}

		await That(Act).ThrowsExactly<InvalidOperationException>().WithMessage(expectedMessage);
	}

	[Fact]
	public async Task Enumerate_WhenOtherEnumerable_ShouldReturnAllItems()
	{
		IEnumerable<int> subject = GetItems();

		List<int> result = Enumerate(CollectionItems<int>.Of(subject));

		await That(result).IsEqualTo([1, 2, 3,]);
	}

	[Fact]
	public async Task Enumerate_WhenUntyped_ShouldReturnAllItems()
	{
		ArrayList subject = new() { "a", "b", };

		List<object?> result = Enumerate(CollectionItems<object?>.Of<IEnumerable>(subject));

		await That(result).IsEqualTo(["a", "b",]);
	}

	private static List<TItem> Enumerate<TItem>(CollectionItems<TItem> items)
	{
		List<TItem> result = [];
		foreach (TItem item in items)
		{
			result.Add(item);
		}

		return result;
	}

	private static IEnumerable<int> GetItems()
	{
		yield return 1;
		yield return 2;
		yield return 3;
	}

	/// <remarks>
	///     The message of the runtime is localized on .NET Framework.
	/// </remarks>
	private static string GetMessageOfModifiedList()
	{
		List<int> list = [1,];
		try
		{
			foreach (int _ in list)
			{
				list.Add(2);
			}
		}
		catch (InvalidOperationException exception)
		{
			return exception.Message;
		}

		throw new InvalidOperationException("The list did not throw.");
	}
}
