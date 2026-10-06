using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Internal.Tests.Results;

public sealed class RepeatedCheckResultTests
{
	[Test]
	public async Task ShouldBeOptionsProvider_ForRepeatedCheckOptions()
	{
		RepeatedCheckOptions options = new();
		RepeatedCheckResult<int, IThat<int>> sut = CreateSut(2, options);

		await That(sut).Is<IOptionsProvider<RepeatedCheckOptions>>()
			.Whose(x => x.Options, it => it.IsSameAs(options));
	}

	private static RepeatedCheckResult<T, IThat<T>> CreateSut<T>(T subject, RepeatedCheckOptions options)
		where T : notnull
	{
#pragma warning disable aweXpect0001
		IThat<T> source = That(subject);
#pragma warning restore aweXpect0001
		return new RepeatedCheckResult<T, IThat<T>>(source.Get().ExpectationBuilder,
			source,
			options);
	}
}
