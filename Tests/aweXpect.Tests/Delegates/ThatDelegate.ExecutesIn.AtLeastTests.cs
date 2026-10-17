using aweXpect.Core.Internal;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class ExecutesIn
	{
		public sealed class AtLeastTests
		{
			[Test]
			public async Task WhenDelegateIsTooFast_ShouldFail()
			{
				VirtualTimeSystem time = new();
				Action @delegate = () => time.Advance(5.Milliseconds());

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtLeast(5123.Milliseconds()).WithTimeSystem(time);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             executes in at least 0:05.123,
					             but it took only 0:*
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenDelegateTakesLongEnough_ShouldSucceed()
			{
				VirtualTimeSystem time = new();
				Action @delegate = () => time.Advance(50.Milliseconds());

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtLeast(10.Milliseconds()).WithTimeSystem(time);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenDelegateThrowsAnException_ShouldFail()
			{
				VirtualTimeSystem time = new();
				Action @delegate = () =>
				{
					time.Advance(500.Milliseconds());
					throw new MyException();
				};

				async Task Act()
					=> await That(@delegate).ExecutesIn().AtLeast(5.Milliseconds()).WithTimeSystem(time);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that @delegate
					              executes in at least 0:00.005,
					              but it did throw a MyException:
					                {nameof(WhenDelegateThrowsAnException_ShouldFail)}
					              """)
					.Because("a crashed delegate must fail even though it ran long enough");
			}
		}
	}
}
