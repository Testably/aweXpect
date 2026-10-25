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
			sut.Within(Timeout.InfiniteTimeSpan);

			string result = sut.ToString();

			await That(result).IsEmpty()
				.Because("an infinite timeout adds no information to the expectation");
		}

		[Test]
		public async Task ToString_WithTimeout_ShouldIncludeTimeout()
		{
			SignalerOptions sut = new();
			sut.Within(5.Seconds());

			string result = sut.ToString();

			await That(result).IsEqualTo(" within 0:05");
		}

		[Test]
		public async Task Within_ShouldReturnTheSameOptions()
		{
			SignalerOptions sut = new();

			SignalerOptions result = sut.Within(5.Seconds());

			await That(result).IsSameAs(sut);
		}

		[Test]
		public async Task Within_WhenSpecifiedTwice_ShouldUseTheLastTimeout()
		{
			SignalerOptions sut = new();
			sut.Within(5.Seconds());

			sut.Within(6.Seconds());

			await That(sut.Timeout).IsEqualTo(6.Seconds());
			await That(sut.ToString()).IsEqualTo(" within 0:06");
		}
	}
}
