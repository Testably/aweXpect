#if NET8_0_OR_GREATER
using aweXpect.Synchronous;

// ReSharper disable CollectionNeverUpdated.Local
// ReSharper disable CollectionNeverQueried.Local

namespace aweXpect.Core.Tests.Core;

public sealed class SpanWrapperTests
{
	[Test]
	public void Add_ShouldThrowNotSupportedException()
	{
		SpanWrapper<char> sut = new("foo".AsSpan());

		void Act()
			=> sut.Add('b');

		Synchronously.Verify(That(Act).Throws<NotSupportedException>()
			.WithMessage("You may not change a SpanWrapper."));
	}

	[Test]
	public void Clear_ShouldThrowNotSupportedException()
	{
		SpanWrapper<char> sut = new("foo".AsSpan());

		void Act()
			=> sut.Clear();

		Synchronously.Verify(That(Act).Throws<NotSupportedException>()
			.WithMessage("You may not change a SpanWrapper."));
	}

	[Test]
	[Arguments('o', true)]
	[Arguments('x', false)]
	public void Contains_ShouldReturnExpectedResult(char character, bool expectedResult)
	{
		SpanWrapper<char> sut = new("foo".AsSpan());

		bool result = sut.Contains(character);

		Synchronously.Verify(That(result).IsEqualTo(expectedResult));
	}

	[Test]
	public void CopyTo_ShouldCopyValuesToArray()
	{
		char[] buffer = "some-prefilled-buffer".ToCharArray();
		SpanWrapper<char> sut = new("foo".AsSpan());

		sut.CopyTo(buffer, 1);

		Synchronously.Verify(That(new string(buffer)).IsEqualTo("sfoo-prefilled-buffer"));
	}

	[Test]
	[Arguments("foo", 3)]
	[Arguments("foobar", 6)]
	public void Count_ShouldReturnExpectedLength(string subject, int expectedLength)
	{
		SpanWrapper<char> sut = new(subject.AsSpan());

		Synchronously.Verify(That(sut.Count).IsEqualTo(expectedLength));
	}

	[Test]
	public void IsReadOnly_ShouldBeTrue()
	{
		Span<int> span = [1, 2, 3,];
		SpanWrapper<int> sut = new(span);

		Synchronously.Verify(That(sut.IsReadOnly).IsTrue());
	}

	[Test]
	public void Remove_ShouldThrowNotSupportedException()
	{
		SpanWrapper<char> sut = new("foo".AsSpan());

		void Act()
			=> sut.Remove('b');

		Synchronously.Verify(That(Act).Throws<NotSupportedException>()
			.WithMessage("You may not change a SpanWrapper."));
	}
}
#endif
