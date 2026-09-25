using System.Linq;
using aweXpect.Core;
using aweXpect.Signaling;

namespace aweXpect.Tests;

public sealed partial class ThatSignaler
{
	public sealed partial class Signaled
	{
		public sealed class WhoseParametersTests
		{
			[Fact]
			public async Task WhenNotTriggered_ShouldFail()
			{
				Signaler<int> signaler = new();

				async Task Act() =>
					await That(signaler).Signaled().AtLeast(2.Times()).With(x => x > 0).Within(50.Milliseconds())
						.WhoseParameters.All().AreUnique();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback at least twice with x => x > 0 within 0:00.050 with parameters of which all are unique,
					             but it was never recorded within 0:00.*

					             Collection:
					             []
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Signaler<int>? subject = null;

				async Task Act()
					=> await That(subject!).Signaled().WhoseParameters.HasCount().EqualTo(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has recorded the callback at least once with parameters that have exactly 0 items,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenTriggered_AndMemberOfParametersDoesNotMatch_ShouldFail()
			{
				Signaler<int> signaler = new();

				_ = Task.Delay(10.Milliseconds())
					.ContinueWith(_ => signaler.Signal(1));

				async Task Act() =>
					await That(signaler).Signaled().WhoseParameters.Whose(p => p.Count(), c => c.IsEqualTo(2));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback at least once within 0:30 with parameters that have Count() that is equal to 2,
					             but Count() was 1, which differs by -1
					             """);
			}

			[Fact]
			public async Task WhenTriggered_AndParametersDoNotMatch_ShouldFail()
			{
				Signaler<int> signaler = new();

				_ = Task.Delay(10.Milliseconds())
					.ContinueWith(_ => signaler.Signal(1));

				async Task Act() =>
					await That(signaler).Signaled().WhoseParameters.All().Satisfy(x => x < 1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback at least once within 0:30 with parameters of which all satisfy x => x < 1,
					             but none of 1 did
					             
					             Not matching items:
					             [1]
					             
					             Collection:
					             [1]
					             """);
			}

			[Fact]
			public async Task WhenTriggered_AndParametersMatch_ShouldSucceed()
			{
				Signaler<int> signaler = new();

				_ = Task.Delay(10.Milliseconds())
					.ContinueWith(_ => signaler.Signal(1));

				async Task Act() =>
					await That(signaler).Signaled().WhoseParameters.All().ComplyWith(x => x.IsGreaterThan(0));

				await That(Act).DoesNotThrow();
			}
		}
	}
}
