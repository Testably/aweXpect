using aweXpect.Core.Extending;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Results;

public sealed class TimeToleranceResultTests
{
	[Test]
	public async Task ShouldBeOptionsProvider_ForTimeTolerance()
	{
		OptionsHolder<TimeTolerance> options = new(TimeTolerance.Default);
		TimeToleranceResult<DateTime, IThat<DateTime>> sut = CreateSut(DateTime.Now, options);

		await That(sut).Is<IOptionsProvider<TimeTolerance>>()
			.Whose(x => x.Options, it => it.IsSameAs(options));
	}

	[Test]
	public async Task Within_ShouldReplaceTheToleranceInTheHolder()
	{
		OptionsHolder<TimeTolerance> options = new(TimeTolerance.Default);
		TimeToleranceResult<DateTime, IThat<DateTime>> sut = CreateSut(DateTime.Now, options);

		TimeToleranceResult<DateTime, IThat<DateTime>> result = sut.Within(TimeSpan.FromSeconds(2));

		await That(result).IsSameAs(sut);
		await That(options.Value.Tolerance).IsEqualTo(TimeSpan.FromSeconds(2));
	}

	private static TimeToleranceResult<DateTime, IThat<DateTime>> CreateSut(DateTime subject,
		OptionsHolder<TimeTolerance> options)
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
