using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Results;

public sealed class ExecutesInResultTests
{
	[Fact]
	public async Task ShouldBeOptionsProvider_ForExecutionTimeOptions()
	{
		ExecutionTimeOptions options = new();
		ExecutesInResult<int[]> sut = CreateSut(Array.Empty<int>(), options);

		await That(sut).Is<IOptionsProvider<ExecutionTimeOptions>>()
			.Whose(x => x.Options, it => it.IsSameAs(options));
	}

	private static ExecutesInResult<T> CreateSut<T>(T subject, ExecutionTimeOptions options)
		=> new(subject, options);
}
