using System.Linq;

namespace aweXpect.Reflection.Tests.Examples;

public sealed class ExampleTests
{
#if NET8_0_OR_GREATER
	[Fact(Skip = "TODO: Check")]
	public async Task AllAsyncMethodsHaveAsyncSuffix()
		=> await That(In.AssemblyContaining(typeof(In))
				.Methods().WhichReturn<Task>().OrReturn<ValueTask>())
			.HaveName("Async").AsSuffix();
#endif

#if NET8_0_OR_GREATER
	[Fact(Skip = "TODO: Check")]
	public async Task AllMethodsWithAsyncSuffixReturnTaskOrValueTask()
		=> await That(In.AssemblyContaining(typeof(In))
				.Methods().WithName("Async").AsSuffix())
			.Return<Task>().OrReturn<ValueTask>();
#endif

	[Fact]
	public async Task AllTestClassesHaveNameThatEndsWithTests()
		=> await That(In.AssemblyContaining<ExampleTests>()
				.Methods().With<FactAttribute>().OrWith<TheoryAttribute>()
				.DeclaringTypes())
			.HaveName("Tests").AsSuffix();
}
