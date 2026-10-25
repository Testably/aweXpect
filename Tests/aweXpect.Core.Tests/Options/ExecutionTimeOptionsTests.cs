using System.Text;
using aweXpect.Chronology;
using aweXpect.Core.Helpers;
using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public class ExecutionTimeOptionsTests
{
	[Test]
	public async Task AllowingExceptions_WhenSpecifiedTwice_ShouldUseTheLastValue()
	{
		ExecutionTimeOptions sut = new();
		sut.AllowingExceptions(true);

		sut.AllowingExceptions(false);

		await That(sut.AllowsException(new InvalidOperationException())).IsFalse();
	}

	[Test]
	public async Task AppendFailureResult_WhenAtLeastIsSpecified_ShouldSayOnly()
	{
		ExecutionTimeOptions sut = new();
		sut.AtLeast(2.Seconds());
		StringBuilder sb = new();

		sut.AppendFailureResult(sb, 1.Seconds());

		await That(sb.ToString()).IsEqualTo("only 0:01");
	}

	[Test]
	public async Task AppendFailureResult_WhenNoLimitIsSpecified_ShouldAppendNothing()
	{
		ExecutionTimeOptions sut = new();
		StringBuilder sb = new();

		sut.AppendFailureResult(sb, 1.Seconds());

		await That(sb.ToString()).IsEmpty();
	}

	[Test]
	public async Task AppendTo_WhenAtLeastIsSpecified_ShouldDescribeTheMinimum()
	{
		ExecutionTimeOptions sut = new();
		sut.AtLeast(2.Seconds());
		StringBuilder sb = new();

		sut.AppendTo(sb, "in ");

		await That(sb.ToString()).IsEqualTo("in at least 0:02");
	}

	[Test]
	public async Task AppendTo_WhenNoLimitIsSpecified_ShouldAppendNothing()
	{
		ExecutionTimeOptions sut = new();
		StringBuilder sb = new();

		sut.AppendTo(sb, "in ");

		await That(sb.ToString()).IsEmpty();
	}

	[Test]
	public async Task IsWithinLimit_WhenActualIsNull_ShouldReturnFalse()
	{
		ExecutionTimeOptions sut = new();
		sut.AtMost(1.Seconds());

		bool result = sut.IsWithinLimit(null);

		await That(result).IsFalse();
	}

	[Test]
	public async Task IsWithinLimit_WhenNoLimitIsSpecified_ShouldReturnFalse()
	{
		ExecutionTimeOptions sut = new();

		bool result = sut.IsWithinLimit(1.Seconds());

		await That(result).IsFalse();
	}

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
	[Arguments("AtLeast")]
	[Arguments("AtMost")]
	[Arguments("Between")]
	[Arguments("Within")]
	public async Task WhenALimitIsSpecified_ShouldReturnTheSameOptions(string option)
	{
		ExecutionTimeOptions sut = new();

		ExecutionTimeOptions result = Specify(sut, option);

		await That(result).IsSameAs(sut);
	}

	[Test]
	public async Task WhenApproximatelyIsSpecifiedTwice_ShouldUseTheLastLimit()
	{
		ExecutionTimeOptions sut = new();
		sut.Approximately(1.Seconds(), 1.Seconds());

		sut.Approximately(5.Seconds(), 1.Seconds());

		await That(sut.IsWithinLimit(1.Seconds())).IsFalse();
		await That(sut.IsWithinLimit(5.Seconds())).IsTrue();
		await That(((ITimeoutBound)sut).UpperBound).IsEqualTo(6.Seconds());
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
	public async Task WhenTheLimitIsRejected_ShouldKeepTheFirstLimitAndItsUpperBound()
	{
		ExecutionTimeOptions sut = new();
		sut.AtMost(1.Seconds());

		void Act() => sut.Between(1.Seconds(), 3.Seconds());

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("Between cannot be combined with AtMost.");
		await That(sut.IsWithinLimit(2.Seconds())).IsFalse();
		await That(((ITimeoutBound)sut).UpperBound).IsEqualTo(1.Seconds());
	}

	[Test]
	[Arguments("AtLeast", "in at least 0:05", null)]
	[Arguments("AtMost", "in at most 0:05", 5.0)]
	[Arguments("Between", "in between 0:05 and 0:06", 6.0)]
	[Arguments("Within", "within 0:05", 5.0)]
	public async Task WhenTheSameLimitIsSpecifiedTwice_ShouldUseTheLastLimit(string option, string expectedText,
		double? expectedUpperBoundSeconds)
	{
		ExecutionTimeOptions sut = new();
		Specify(sut, option);

		_ = option switch
		{
			"AtLeast" => sut.AtLeast(5.Seconds()),
			"AtMost" => sut.AtMost(5.Seconds()),
			"Between" => sut.Between(5.Seconds(), 6.Seconds()),
			_ => sut.Within(5.Seconds()),
		};

		StringBuilder sb = new();
		sut.AppendTo(sb, "in ");
		await That(sb.ToString()).IsEqualTo(expectedText);
		await That(((ITimeoutBound)sut).UpperBound?.TotalSeconds).IsEqualTo(expectedUpperBoundSeconds);
	}

	private static ExecutionTimeOptions Specify(ExecutionTimeOptions sut, string option)
		=> option switch
		{
			"AtLeast" => sut.AtLeast(1.Seconds()),
			"AtMost" => sut.AtMost(1.Seconds()),
			"Between" => sut.Between(1.Seconds(), 2.Seconds()),
			_ => sut.Within(1.Seconds()),
		};
}
