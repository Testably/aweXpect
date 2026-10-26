using aweXpect.Reflection.Options;

namespace aweXpect.Reflection.Internal.Tests.Options;

public sealed class TypeFilterOptionsTests
{
	[Test]
	public async Task WithType_ShouldNotChangeTheOptionsItIsCalledOn()
	{
		TypeFilterOptions sut = TypeFilterOptions.Default.WithType(typeof(int), false);

		TypeFilterOptions result = sut.WithType(typeof(string), true);

		await That(sut.Matches(typeof(string))).IsFalse();
		await That(result.Matches(typeof(string))).IsTrue();
		await That(TypeFilterOptions.Default.Matches(typeof(int))).IsFalse();
	}
}
