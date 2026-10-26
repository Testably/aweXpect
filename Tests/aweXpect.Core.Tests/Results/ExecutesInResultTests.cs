using aweXpect.Core.Extending;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Results;

public sealed class ExecutesInResultTests
{
	[Test]
	public async Task ShouldBeOptionsProvider_ForExecutionTimeOptions()
	{
		OptionsHolder<ExecutionTimeOptions> options = new(ExecutionTimeOptions.Default);
		ExecutesInResult<int[]> sut = CreateSut(Array.Empty<int>(), options);

		await That(sut).Is<IOptionsProvider<ExecutionTimeOptions>>()
			.Whose(x => x.Options, it => it.IsSameAs(options));
	}

	private static ExecutesInResult<T> CreateSut<T>(T subject, OptionsHolder<ExecutionTimeOptions> options)
	{
#pragma warning disable aweXpect0001
		IThat<T> source = That(subject);
#pragma warning restore aweXpect0001
		return new ExecutesInResult<T>(source.Get().ExpectationBuilder, subject, options);
	}
}
