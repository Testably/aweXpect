using System.Threading;
using aweXpect.Chronology;
using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public sealed class SignalerOptionsTests
{
	public sealed class Tests
	{
		[Test]
		public async Task ToString_Empty_ShouldReturnEmptyString()
		{
			SignalerOptions sut = new();

			string result = sut.ToString();

			await That(result).IsEmpty();
		}

		[Test]
		public async Task ToString_WithInfiniteTimeout_ShouldReturnEmptyString()
		{
			SignalerOptions sut = new();
			sut = sut.Within(Timeout.InfiniteTimeSpan);

			string result = sut.ToString();

			await That(result).IsEmpty()
				.Because("an infinite timeout adds no information to the expectation");
		}

		[Test]
		public async Task ToString_WithTimeout_ShouldIncludeTimeout()
		{
			SignalerOptions sut = new();
			sut = sut.Within(5.Seconds());

			string result = sut.ToString();

			await That(result).IsEqualTo(" within 0:05");
		}

		[Test]
		public async Task Within_ShouldNotChangeTheOriginalOptions()
		{
			SignalerOptions sut = SignalerOptions.Default;

			SignalerOptions result = sut.Within(5.Seconds());

			await That(result.Timeout).IsEqualTo(5.Seconds());
			await That(sut.Timeout).IsNull();
		}

		[Test]
		public async Task Within_WhenSpecifiedAsZero_ShouldKeepIt()
		{
			SignalerOptions sut = SignalerOptions.Default.Within(TimeSpan.Zero);

			await That(sut.Timeout).IsEqualTo(TimeSpan.Zero);
			await That(sut.ToString()).IsEqualTo(" within 0:00");
		}

		[Test]
		public async Task Within_WhenSpecifiedTwice_ShouldUseTheLastTimeout()
		{
			SignalerOptions sut = new();
			sut = sut.Within(5.Seconds());

			sut = sut.Within(6.Seconds());

			await That(sut.Timeout).IsEqualTo(6.Seconds());
			await That(sut.ToString()).IsEqualTo(" within 0:06");
		}
	}
}
