namespace aweXpect.Reflection.Tests.Examples;

public sealed class ExampleTests
{
#if NET8_0_OR_GREATER
	[Fact]
	public async Task AllAsyncMethodsHaveAsyncSuffix()
		=> await That(In.AssemblyContaining(typeof(In))
				.Methods().WhichReturn<Task>().OrReturn<ValueTask>())
			.HaveName("Async").AsSuffix();
#endif

	[Fact]
	public async Task AllTestClassesHaveNameThatEndsWithTests()
		=> await That(In.AssemblyContaining<ExampleTests>()
				.Methods().With<FactAttribute>().OrWith<TheoryAttribute>()
				.DeclaringTypes())
			.HaveName("Tests").AsSuffix();
}
