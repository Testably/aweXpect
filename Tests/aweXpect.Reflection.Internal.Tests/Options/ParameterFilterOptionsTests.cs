using System.Reflection;
using aweXpect.Reflection.Options;

namespace aweXpect.Reflection.Internal.Tests.Options;

public sealed class ParameterFilterOptionsTests
{
	[Test]
	public async Task WithModifier_ShouldNotChangeTheOptionsItIsCalledOn()
	{
		ParameterFilterOptions sut = new(_ => true, () => "of any type");

		ParameterFilterOptions result = sut.WithModifier(p => p.IsIn, () => "with in modifier");

		await That(sut.GetModifierDescription()).IsEmpty();
		await That(result.GetModifierDescription()).IsEqualTo(" with in modifier");
		await That(await sut.Matches(Parameter())).IsTrue();
		await That(await result.Matches(Parameter())).IsFalse();
	}

	[Test]
	public async Task WithPredicate_ShouldNotChangeTheOptionsItIsCalledOn()
	{
		ParameterFilterOptions sut = new(_ => true, () => "of any type");

		ParameterFilterOptions result = sut.WithPredicate(_ => false, () => "never");

		await That(sut.GetDescription()).IsEqualTo("of any type");
		await That(result.GetDescription()).IsEqualTo("of any type and never");
		await That(await sut.Matches(Parameter())).IsTrue();
		await That(await result.Matches(Parameter())).IsFalse();
	}

	private static ParameterInfo Parameter()
		=> typeof(ParameterFilterOptionsTests).GetMethod(nameof(Sample), BindingFlags.NonPublic | BindingFlags.Static)!
			.GetParameters()[0];

	private static void Sample(int value)
	{
	}
}
