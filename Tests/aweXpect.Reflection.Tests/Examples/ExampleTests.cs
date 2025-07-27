namespace aweXpect.Reflection.Tests.Examples;

public sealed class ExampleTests
{
	[Fact]
	public async Task AllTestClassesHaveNameThatEndsWithTests()
		=> await That(In.AssemblyContaining<ExampleTests>()
				.Methods().With<FactAttribute>().OrWith<TheoryAttribute>()
				.DeclaringTypes())
			.HaveName("Tests").AsSuffix();
}
