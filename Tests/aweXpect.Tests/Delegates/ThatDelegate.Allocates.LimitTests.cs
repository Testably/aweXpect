#if NET8_0_OR_GREATER
using aweXpect.Results;

namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class Allocates
	{
		public sealed class AtLeastTests
		{
			[Test]
			public async Task WhenDelegateAllocatesEnough_ShouldSucceed()
			{
				Action @delegate = () => GC.KeepAlive(new byte[1000]);

				async Task Act()
					=> await That(@delegate).Allocates().AtLeast(1000);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenDelegateAllocatesTooLittle_ShouldFail()
			{
				int value = 0;
				Action @delegate = () => value++;

				async Task Act()
					=> await That(@delegate).Allocates().AtLeast(16);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             allocates at least 16 bytes,
					             but it allocated only 0 bytes
					             """);
			}

			[Test]
			public async Task WhenMinimumIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				Action @delegate = () => { };

				async Task Act()
					=> await That(@delegate).Allocates().AtLeast(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("minimum").And
					.WithMessage("The minimum must not be negative.").AsPrefix();
			}
		}

		public sealed class AtMostTests
		{
			[Test]
			public async Task WhenDelegateAllocatesLittleEnough_ShouldSucceed()
			{
				Action @delegate = () => GC.KeepAlive(new byte[1000]);

				async Task Act()
					=> await That(@delegate).Allocates().AtMost(2000);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenDelegateAllocatesTooMuch_ShouldFail()
			{
				Action @delegate = () => GC.KeepAlive(new byte[1000]);

				async Task Act()
					=> await That(@delegate).Allocates().AtMost(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             allocates at most 1 byte,
					             but it allocated * bytes
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenLimitIsSpecifiedTwice_InTheOtherOrder_ShouldThrowInvalidOperationException()
			{
				Action @delegate = () => { };
				AllocatesResult<AndResult<Delegates.ThatDelegate.WithoutValue>> allocates =
					That(@delegate).Allocates();
				await allocates.Nothing();

				void Act()
					=> allocates.AtMost(1);

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("AtMost cannot be combined with Nothing.");
			}

			[Test]
			public async Task WhenLimitIsSpecifiedTwice_ShouldThrowInvalidOperationException()
			{
				Action @delegate = () => { };
				AllocatesResult<AndResult<Delegates.ThatDelegate.WithoutValue>> allocates =
					That(@delegate).Allocates();
				await allocates.AtMost(1);

				void Act()
					=> allocates.Nothing();

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("Nothing cannot be combined with AtMost.")
					.Because("the second limit would silently replace the first one");
			}

			[Test]
			public async Task WhenMaximumIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				Action @delegate = () => { };

				async Task Act()
					=> await That(@delegate).Allocates().AtMost(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("maximum").And
					.WithMessage("The maximum must not be negative.").AsPrefix();
			}
		}

		public sealed class BetweenTests
		{
			[Test]
			public async Task WhenAndIsSpecifiedTwice_ShouldUseTheLastRange()
			{
				Action @delegate = () => GC.KeepAlive(new byte[1000]);
				BetweenResult<AndResult<Delegates.ThatDelegate.WithoutValue>, long> sut =
					That(@delegate).Allocates().Between(1);
				_ = sut.And(1_000_000);

				async Task Act()
					=> await sut.And(100);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             allocates between 1 and 100 bytes,
					             but it allocated * bytes
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenDelegateAllocatesAboveTheRange_ShouldFail()
			{
				Action @delegate = () => GC.KeepAlive(new byte[1000]);

				async Task Act()
					=> await That(@delegate).Allocates().Between(1).And(100);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             allocates between 1 and 100 bytes,
					             but it allocated * bytes
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenDelegateAllocatesBelowTheRange_ShouldFail()
			{
				int value = 0;
				Action @delegate = () => value++;

				async Task Act()
					=> await That(@delegate).Allocates().Between(16).And(1024);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that @delegate
					             allocates between 16 and 1024 bytes,
					             but it allocated only 0 bytes
					             """);
			}

			[Test]
			public async Task WhenDelegateAllocatesWithinTheRange_ShouldSucceed()
			{
				Action @delegate = () => GC.KeepAlive(new byte[1000]);

				async Task Act()
					=> await That(@delegate).Allocates().Between(1000).And(2000);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMaximumIsBelowMinimum_ShouldThrowArgumentOutOfRangeException()
			{
				Action @delegate = () => { };

				async Task Act()
					=> await That(@delegate).Allocates().Between(100).And(10);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("maximum").And
					.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix();
			}

			[Test]
			public async Task WhenMinimumIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				Action @delegate = () => { };

				async Task Act()
					=> await That(@delegate).Allocates().Between(-1).And(10);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("minimum").And
					.WithMessage("The minimum must not be negative.").AsPrefix();
			}
		}
	}
}
#endif
