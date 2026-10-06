using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed class OptionInferenceTests
{
	[Test]
	public async Task Equivalent_OnAClass_ShouldCompareTheMembers()
	{
		Album subject = new("Abbey Road", [new Song("Come Together"),]);

		async Task Act()
			=> await That(subject).IsEqualTo(new Album("Abbey Road", [new Song("Come Together"),])).Equivalent();

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task Using_WithAComparerForABaseTypeOfTheItems_ShouldUseIt()
	{
		Derived[] subject = [new("a"),];

		async Task Act()
			=> await That(subject).Contains(new Derived("A")).Using(new CaseInsensitiveBaseComparer());

		await That(Act).DoesNotThrow()
			.Because("the comparer of a base type applies to the items through contravariance");
	}

	[Test]
	public async Task Within_OnAByteCollection_ShouldConvertAnIntLiteral()
	{
		byte[] subject = [5,];

		async Task Act()
			=> await That(subject).Contains((byte)6).Within(1);

		await That(Act).DoesNotThrow()
			.Because("an int literal converts to a byte only as a constant, so a dedicated overload takes it");
	}

	[Test]
	public async Task Within_OnADateTime_ShouldTakeATimeSpan()
	{
		DateTime subject = new(2024, 1, 1, 12, 0, 0);

		async Task Act()
			=> await That(subject).IsEqualTo(subject.AddMilliseconds(500)).Within(TimeSpan.FromSeconds(1));

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task Within_OnADecimal_ShouldConvertAnIntLiteral()
	{
		decimal subject = 1m;

		async Task Act()
			=> await That(subject).IsEqualTo(1m).Within(0);

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task Within_OnADouble_ShouldConvertAnIntLiteral()
	{
		double subject = 1.0;

		async Task Act()
			=> await That(subject).IsEqualTo(2.0).Within(1);

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task Within_OnADoubleCollection_ShouldConvertAnIntLiteral()
	{
		double[] subject = [1.0,];

		async Task Act()
			=> await That(subject).IsEqualTo(new[] { 1.5, }).Within(1);

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task Within_OnAFloat_ShouldTakeAFloat()
	{
		float subject = 1.0f;

		async Task Act()
			=> await That(subject).IsEqualTo(1.1f).Within(0.2f);

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task Within_OnAnUlong_ShouldConvertAnIntLiteral()
	{
		ulong subject = 5;

		async Task Act()
			=> await That(subject).IsEqualTo(6UL).Within(1);

		await That(Act).DoesNotThrow()
			.Because("an int literal converts to an ulong only as a constant, so a dedicated overload takes it");
	}

	private sealed class Album(string title, Song[] songs)
	{
		public Song[] Songs { get; } = songs;
		public string Title { get; } = title;
	}

	private sealed class Song(string title)
	{
		public string Title { get; } = title;
	}

	private class Base(string name)
	{
		public string Name { get; } = name;
	}

	private sealed class Derived(string name) : Base(name);

	private sealed class CaseInsensitiveBaseComparer : IEqualityComparer<Base>
	{
		public bool Equals(Base? x, Base? y)
			=> string.Equals(x?.Name, y?.Name, StringComparison.OrdinalIgnoreCase);

		public int GetHashCode(Base obj) => obj.Name.ToUpperInvariant().GetHashCode();
	}
}
