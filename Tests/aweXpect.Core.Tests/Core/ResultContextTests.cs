using System.Threading;

namespace aweXpect.Core.Tests.Core;

public sealed class ResultContextTests
{
	public sealed class AsyncCallbackTests
	{
		[Test]
		public async Task GetContent_ShouldForwardTheCancellationTokenToTheCallback()
		{
			using CancellationTokenSource cts = new();
			CancellationToken? receivedToken = null;
			ResultContext sut = new ResultContext.AsyncCallback("foo", token =>
			{
				receivedToken = token;
				return Task.FromResult<string?>("bar");
			});

			string? content = await sut.GetContent(cts.Token);

			await That(content).IsEqualTo("bar");
			await That(receivedToken).IsEqualTo(cts.Token);
		}

		[Test]
		public async Task ShouldUseTitleAndPriority()
		{
			ResultContext sut = new ResultContext.AsyncCallback("foo", _ => Task.FromResult<string?>("bar"), 3);

			await That(sut.Title).IsEqualTo("foo");
			await That(sut.Priority).IsEqualTo(3);
		}

		[Test]
		public async Task WhenAddedToAFailingExpectation_ShouldShowTheContentOfTheCallback()
		{
			async Task Act()
				=> await That(1).ShowsContexts((contexts, actual, _)
					=> contexts.Add(new ResultContext.AsyncCallback("Value",
						_ => Task.FromResult<string?>($"async {actual}"))));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that 1
				             shows contexts,
				             but it did not

				             Value:
				             async 1
				             """);
		}
	}
}
