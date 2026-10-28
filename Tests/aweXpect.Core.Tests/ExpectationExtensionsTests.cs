using aweXpect.Chronology;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Results;
using aweXpect.Signaling;

namespace aweXpect.Core.Tests;

public sealed class ExpectationExtensionsTests
{
	[Test]
	public async Task WithTimeProvider_OnACombination_ShouldRunEveryCombinedExpectationOnTheTimeProvider()
	{
		VirtualTimeProvider timeProvider = new();
		Signaler signaler1 = new();
		Signaler signaler2 = new();

		async Task Act()
			=> await ThatAll(
					That(signaler1).Signaled().Within(10.Seconds()),
					That(signaler2).Signaled().Within(20.Seconds()))
				.WithTimeProvider(timeProvider);

		await That(Act).Throws<FailException>()
			.WithMessage("*never recorded within 0:10*never recorded within 0:20*").AsWildcard();
		await That(timeProvider.Now).IsEqualTo(30.Seconds())
			.Because("both combined expectations waited one after the other on the virtual clock");
	}

	[Test]
	public async Task WithTimeProvider_ShouldReturnTheSameExpectation()
	{
		AndOrResult<bool, IThat<bool>> sut = That(true).IsTrue();

		AndOrResult<bool, IThat<bool>> result = sut.WithTimeProvider(new VirtualTimeProvider());

		await That(result).IsSameAs(sut);
		await result;
	}

	[Test]
	public async Task WithTimeProvider_ShouldRunTheEvaluationOnTheTimeProvider()
	{
		VirtualTimeProvider timeProvider = new();
		Signaler signaler = new();

		async Task Act()
			=> await That(signaler).Signaled().Within(30.Seconds()).WithTimeProvider(timeProvider);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that signaler
			             has recorded the callback at least once within 0:30,
			             but it was never recorded within 0:30
			             """);
		await That(timeProvider.Now).IsEqualTo(30.Seconds())
			.Because("the wait elapsed on the virtual clock instead of in real time");
	}

	[Test]
	public async Task WithTimeProvider_WhenExpectationIsNull_ShouldThrowArgumentNullException()
	{
		ExpectationResult? sut = null;

		void Act() => sut!.WithTimeProvider(new VirtualTimeProvider());

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("expectation").And
			.WithMessage("The 'expectation' cannot be null.").AsPrefix();
	}

	[Test]
	public async Task WithTimeProvider_WhenSpecifiedTwice_ShouldThrowInvalidOperationException()
	{
		AndOrResult<bool, IThat<bool>> sut = That(true).IsTrue().WithTimeProvider(new VirtualTimeProvider());

		void Act() => sut.WithTimeProvider(new VirtualTimeProvider());

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("WithTimeProvider cannot be specified more than once.");
	}

	[Test]
	public async Task WithTimeProvider_WhenTimeProviderIsNull_ShouldThrowArgumentNullException()
	{
		AndOrResult<bool, IThat<bool>> sut = That(true).IsTrue();

		void Act() => sut.WithTimeProvider(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("timeProvider").And
			.WithMessage("The 'timeProvider' cannot be null.").AsPrefix();
	}
}
