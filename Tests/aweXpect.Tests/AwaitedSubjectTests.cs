using System.Collections.Generic;
using System.IO;

namespace aweXpect.Tests;

public sealed class AwaitedSubjectTests
{
	[Fact]
	public async Task ForCollection_HasItem_ShouldReturnTheNotNullSubject()
	{
		IEnumerable<int>? subject = [1, 2, 3,];

		IEnumerable<int> result = await That(subject).HasItem(2);

		await That(result).IsSameAs(subject);
	}

	[Fact]
	public async Task ForException_HasMessage_ShouldReturnTheNotNullSubject()
	{
		Exception? subject = new("foo");

		Exception result = await That(subject).HasMessage("foo");

		await That(result).IsSameAs(subject);
	}

	[Fact]
	public async Task ForNullableBool_IsNotNull_ShouldReturnTheNotNullSubject()
	{
		bool? subject = true;

		bool result = await That(subject).IsNotNull();

		await That(result).IsTrue();
	}

	[Fact]
	public async Task ForNullableBool_IsTrue_ShouldReturnTheNotNullSubject()
	{
		bool? subject = true;

		bool result = await That(subject).IsTrue();

		await That(result).IsTrue();
	}

	[Fact]
	public async Task ForNullableChar_IsADigit_ShouldReturnTheNotNullSubject()
	{
		char? subject = '1';

		char result = await That(subject).IsADigit();

		await That(result).IsEqualTo('1');
	}

	[Fact]
	public async Task ForNullableDateTime_HasYear_ShouldReturnTheNotNullSubject()
	{
		DateTime? subject = new(2010, 11, 12);

		DateTime result = await That(subject).HasYear().EqualTo(2010);

		await That(result).IsEqualTo(subject);
	}

	[Fact]
	public async Task ForNullableDateTime_IsAfter_ShouldReturnTheNotNullSubject()
	{
		DateTime? subject = new(2010, 11, 12);

		DateTime result = await That(subject).IsAfter(new DateTime(2010, 11, 11));

		await That(result).IsEqualTo(subject);
	}

	[Fact]
	public async Task ForNullableEnum_HasValue_ShouldReturnTheNotNullSubject()
	{
		DayOfWeek? subject = DayOfWeek.Monday;

		DayOfWeek result = await That(subject).HasValue(1);

		await That(result).IsEqualTo(DayOfWeek.Monday);
	}

	[Fact]
	public async Task ForNullableNumber_IsPositive_ShouldReturnTheNotNullSubject()
	{
		int? subject = 1;

		int result = await That(subject).IsPositive();

		await That(result).IsEqualTo(1);
	}

	[Fact]
	public async Task ForStream_IsReadable_ShouldReturnTheNotNullSubject()
	{
		using MemoryStream stream = new();
		Stream? subject = stream;

		Stream result = await That(subject).IsReadable();

		await That(result).IsSameAs(subject);
	}

	[Fact]
	public async Task ForString_IsLowerCased_ShouldReturnTheNotNullSubject()
	{
		string? subject = "abc";

		string result = await That(subject).IsLowerCased();

		await That(result).IsSameAs(subject);
	}

	[Fact]
	public async Task ForString_StartsWith_ShouldReturnTheNotNullSubject()
	{
		string? subject = "abc";

		string result = await That(subject).StartsWith("a");

		await That(result).IsSameAs(subject);
	}

	[Fact]
	public async Task ForVersion_HasMajor_ShouldReturnTheNotNullSubject()
	{
		Version? subject = new(1, 2, 3);

		Version result = await That(subject).HasMajor(1);

		await That(result).IsSameAs(subject);
	}

	[Fact]
	public async Task Whose_ShouldReturnTheNotNullSubject()
	{
		string? subject = "abc";

		string result = await That(subject).Whose(s => s.Length, length => length.IsEqualTo(3));

		await That(result).IsSameAs(subject);
	}
}
