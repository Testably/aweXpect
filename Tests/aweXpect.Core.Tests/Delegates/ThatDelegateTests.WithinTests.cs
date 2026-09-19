using System.Diagnostics;
using System.Threading;
using aweXpect.Chronology;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Delegates;

public sealed partial class ThatDelegateTests
{
	public sealed class WithinTests
	{
		[Fact]
		public async Task WhenDelegateExceedsTheDuration_ShouldCancelTheCancellationToken()
		{
			Func<CancellationToken, Task> @delegate = token => Task.Delay(6.Seconds(), token);
			Stopwatch sw = new();

			async Task Act()
				=> await That(@delegate).Throws<MyException>().Within(50.Milliseconds());

			sw.Start();
			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that @delegate
				             throws a MyException within 0:00.050,
				             but it *
				             """).AsWildcard();
			sw.Stop();

			await That(sw.Elapsed).IsLessThan(5.Seconds())
				.Because("the elapsed duration must cancel the token instead of awaiting the delegate");
		}
	}
}
