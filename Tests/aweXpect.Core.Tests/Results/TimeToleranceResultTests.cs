using aweXpect.Core.Extending;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Options;
using aweXpect.Results;
#if NET8_0_OR_GREATER
#endif

namespace aweXpect.Core.Tests.Results;

public sealed class TimeToleranceResultTests
{
	[Fact]
	public async Task ShouldBeOptionsProvider_ForTimeTolerance()
	{
		TimeTolerance options = new();
		TimeToleranceResult<DateTime, IThat<DateTime>> sut = CreateSut(DateTime.Now, options);

		await That(sut).Is<IOptionsProvider<TimeTolerance>>()
			.Whose(x => x.Options, it => it.IsSameAs(options));
	}

	private static TimeToleranceResult<DateTime, IThat<DateTime>> CreateSut(DateTime subject, TimeTolerance options)
	{
#pragma warning disable aweXpect0001
		IThat<DateTime> source = That(subject);
#pragma warning restore aweXpect0001
		return new TimeToleranceResult<DateTime, IThat<DateTime>>(source.Get().ExpectationBuilder.AddConstraint((it, _)
				=> new DummyConstraint(it)),
			source,
			options);
	}
}
