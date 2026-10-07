using System.Collections.Generic;
using aweXpect.Chronology;
using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public class ExecutionTimeOptionsTests
{
	[Test]
	[Arguments("AtLeast")]
	[Arguments("AtMost")]
	[Arguments("Between")]
	[Arguments("Within")]
	public async Task WhenALimitIsSpecified_ShouldBeWithinLimit(string option)
	{
		ExecutionTimeOptions sut = new();

		Specify(sut, option);

		await That(sut.IsWithinLimit(1.Seconds())).IsTrue();
	}

	[Test]
	public async Task WhenApproximatelyIsSpecifiedTwice_ShouldThrowInvalidOperationException()
	{
		ExecutionTimeOptions sut = new();
		sut.Approximately(1.Seconds(), 1.Seconds());

		void Act() => sut.Approximately(2.Seconds(), 1.Seconds());

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("Within cannot be specified more than once.")
			.Because("the tolerance is specified with Within");
	}

	[Test]
	[Arguments("AtLeast", "AtMost")]
	[Arguments("AtMost", "AtLeast")]
	[Arguments("AtMost", "Between")]
	[Arguments("Between", "Within")]
	[Arguments("Within", "AtMost")]
	public async Task WhenAnotherLimitIsSpecified_ShouldThrowInvalidOperationException(string first, string second)
	{
		ExecutionTimeOptions sut = new();
		Specify(sut, first);

		void Act() => Specify(sut, second);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage($"{second} cannot be combined with {first}.")
			.Because("the second limit would silently replace the first one");
	}

	[Test]
	public async Task WhenTheLimitIsInvalid_AndALimitIsSpecified_ShouldThrowArgumentOutOfRangeException()
	{
		ExecutionTimeOptions sut = new();
		sut.AtMost(1.Seconds());

		void Act() => sut.AtMost(-1.Seconds());

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("maximum");
	}

	[Test]
	public async Task WhenTheLimitIsRejected_ShouldKeepTheFirstLimitAndNotReportItsUpperBound()
	{
		List<TimeSpan> upperBounds = [];
		ExecutionTimeOptions sut = new();
		sut.OnUpperBound(upperBounds.Add);
		sut.AtMost(1.Seconds());

		void Act() => sut.AtMost(3.Seconds());

		await That(Act).Throws<InvalidOperationException>();
		await That(sut.IsWithinLimit(2.Seconds())).IsFalse();
		await That(upperBounds).IsEqualTo([1.Seconds(),]);
	}

	[Test]
	[Arguments("AtLeast")]
	[Arguments("AtMost")]
	[Arguments("Between")]
	[Arguments("Within")]
	public async Task WhenTheSameLimitIsSpecifiedTwice_ShouldThrowInvalidOperationException(string option)
	{
		ExecutionTimeOptions sut = new();
		Specify(sut, option);

		void Act() => Specify(sut, option);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage($"{option} cannot be specified more than once.");
	}

	private static void Specify(ExecutionTimeOptions sut, string option)
	{
		switch (option)
		{
			case "AtLeast":
				sut.AtLeast(1.Seconds());
				break;
			case "AtMost":
				sut.AtMost(1.Seconds());
				break;
			case "Between":
				sut.Between(1.Seconds(), 2.Seconds());
				break;
			default:
				sut.Within(1.Seconds());
				break;
		}
	}
}
