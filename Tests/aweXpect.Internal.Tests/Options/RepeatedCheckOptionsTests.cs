using System.Diagnostics.CodeAnalysis;
using System.Threading;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Customization;
using aweXpect.Options;
using FluentAssertions.Extensions;

namespace aweXpect.Internal.Tests.Options;

public class RepeatedCheckOptionsTests
{
	[Fact]
	public async Task CheckEvery_WhenIntervalIsSpecified_ShouldThrowInvalidOperationException()
	{
		RepeatedCheckOptions sut = new();
		sut.CheckEvery(10.Milliseconds());

		void Act() => sut.CheckEvery(20.Milliseconds());

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("CheckEvery cannot be specified more than once.")
			.Because("the second interval would silently replace the first one");
	}

	[Fact]
	public async Task CheckEvery_WhenTheDefaultIntervalWasRead_ShouldNotThrow()
	{
		RepeatedCheckOptions sut = new();
		_ = sut.Interval;

		void Act() => sut.CheckEvery(20.Milliseconds());

		await That(Act).DoesNotThrow()
			.Because("the customized default interval is not an explicit interval");
		await That(sut.Interval).IsEqualTo(20.Milliseconds());
	}

	[Fact]
	public async Task CheckRepeatedly_WhenIntervalIsNotPositive_ShouldYieldBetweenChecks()
	{
		RepeatedCheckOptions sut = new();
		sut.Within(Timeout.InfiniteTimeSpan);
		bool isMet = false;
		Task<Outcome> result;
		using (((IAwexpectCustomization)Customize.aweXpect).Set("aweXpect.Settings.DefaultCheckInterval",
			       TimeSpan.Zero))
		{
			await That(sut.Interval).IsEqualTo(TimeSpan.Zero)
				.Because("the interval must be stored under the key that the setting reads");
			result = sut.CheckRepeatedly(() => Task.FromResult(Volatile.Read(ref isMet)), new NoEvaluationContext());
		}

		bool isCompletedSynchronously = result.IsCompleted;
		Volatile.Write(ref isMet, true);

		await That(isCompletedSynchronously).IsFalse()
			.Because("checking without waiting must still hand the thread back between the checks");
		await That(await result).IsEqualTo(Outcome.Success).WithTimeout(10.Seconds());
	}

	[Fact]
	public async Task Interval_ShouldReadTheCurrentDefaultEachTime()
	{
		RepeatedCheckOptions sut = new();
		TimeSpan interval1, interval2;
		using (Customize.aweXpect.Settings().DefaultCheckInterval.Set(103.Milliseconds()))
		{
			interval1 = sut.Interval;
		}

		using (Customize.aweXpect.Settings().DefaultCheckInterval.Set(107.Milliseconds()))
		{
			interval2 = sut.Interval;
		}

		await That(interval1).IsEqualTo(103.Milliseconds());
		await That(interval2).IsEqualTo(107.Milliseconds())
			.Because("a re-evaluated expectation uses the default of its current evaluation, like Eventually() does");
	}

	[Fact]
	public async Task ToString_WhenTimeoutIsNotSpecified_ShouldBeEmpty()
	{
		RepeatedCheckOptions sut = new();

		string result = sut.ToString();

		await That(result).IsEmpty();
	}

	[Fact]
	public async Task ToString_WhenTimeoutIsZero_ShouldIncludeTheTimeout()
	{
		RepeatedCheckOptions sut = new();
		sut.Within(TimeSpan.Zero);

		string result = sut.ToString();

		await That(result).IsEqualTo(" within 0:00")
			.Because("an explicit timeout is named like on a signaler, even when it is zero");
	}

	[Fact]
	public async Task Within_WhenTimeoutIsSpecified_ShouldThrowInvalidOperationException()
	{
		RepeatedCheckOptions sut = new();
		sut.Within(1.Seconds());

		void Act() => sut.Within(2.Seconds());

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("Within cannot be specified more than once.")
			.Because("the second timeout would silently replace the first one");
	}

	private sealed class NoEvaluationContext : IEvaluationContext
	{
		public EvaluationCancellation Cancellation => EvaluationCancellation.None;

		public void Store<T>(string key, T value) { }

		public bool TryReceive<T>(string key, [NotNullWhen(true)] out T? value)
		{
			value = default;
			return false;
		}
	}
}
