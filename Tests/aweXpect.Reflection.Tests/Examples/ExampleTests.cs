namespace aweXpect.Reflection.Tests.Examples;

public sealed class ExampleTests
{
#if NET8_0_OR_GREATER
	[Test]
	[Skip("Does not apply in this project")]
	public async Task AllAsyncMethodsHaveAsyncSuffix()
		=> await That(In.AssemblyContaining(typeof(In))
				.Methods().WhichAreAsync())
			.HaveName("Async").AsSuffix();
#endif

#if NET8_0_OR_GREATER
	[Test]
	[Skip("Does not apply in this project")]
	public async Task AllMethodsWithAsyncSuffixReturnTaskOrValueTask()
		=> await That(In.AssemblyContaining(typeof(In))
				.Methods().WithName("Async").AsSuffix())
			.Return<Task>().OrReturn<ValueTask>();
#endif

	[Test]
	public async Task AllTestClassesHaveNameThatEndsWithTests()
		=> await That(In.AssemblyContaining<ExampleTests>()
				.Methods().With<TestAttribute>()
				.DeclaringTypes())
			.HaveName("Tests").AsSuffix();

	[Test]
	public async Task NoPublicMethodTakesAParamsCollection()
		=> await That(In.AssemblyContaining(typeof(In)).Public.Methods()
				.WithParamsParameter().Matching(p => !p.ParameterType.IsArray))
			.IsEmpty()
			.Because("consumers below C# 13 (C# 7.3 on net48, C# 12 on net8.0) cannot pass single values to a params collection");
}
