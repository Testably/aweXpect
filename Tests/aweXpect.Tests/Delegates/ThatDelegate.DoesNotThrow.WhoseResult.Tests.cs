namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class DoesNotThrow
	{
		public sealed class WhoseResult
		{
			public sealed class Tests
			{
				[Test]
				public async Task WhenAfterOrInsideTheExpectationsOnTheResult_ShouldOnlyContinueTheRightOperand()
				{
					Func<int[]> @delegate = () => [];

					async Task Act()
						=> await That(@delegate).DoesNotThrow().WhoseResult.IsEmpty().Or.HasSingle().Which.IsEqualTo(1);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenDelegateThrows_ShouldFail()
				{
					Func<int> @delegate = () => throw new CustomException();

					async Task Act() => await That(@delegate).DoesNotThrow().WhoseResult.IsGreaterThan(5);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that @delegate
						             does not throw any exception and its result is greater than 5,
						             but it did throw a ThatDelegate.CustomException:
						               WhenDelegateThrows_ShouldFail
						             """);
				}

				[Test]
				[AutoArguments]
				public async Task WhenReturnValueDoesNotMatch_ShouldFail(int value)
				{
					Func<int> @delegate = () => value;

					async Task Act() => await That(@delegate).DoesNotThrow().WhoseResult.IsLessThan(value);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that @delegate
						              does not throw any exception and its result is less than {value},
						              but it was {value}
						              """);
				}

				[Test]
				[AutoArguments]
				public async Task WhenReturnValueMatches_ShouldSucceed(int value)
				{
					Func<int> @delegate = () => value;

					await That(@delegate).DoesNotThrow().WhoseResult.IsEqualTo(value);
				}
			}

			public sealed class GenericTests
			{
				[Test]
				public async Task WhenDelegateThrowsExpectedException_ShouldFail()
				{
					Func<int> @delegate = () => throw new CustomException();

					async Task Act()
						=> await That(@delegate).DoesNotThrow<CustomException>().WhoseResult.IsEqualTo(5);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that @delegate
						             does not throw a ThatDelegate.CustomException and its result is equal to 5,
						             but it did throw a ThatDelegate.CustomException:
						               WhenDelegateThrowsExpectedException_ShouldFail
						             """);
				}

				[Test]
				public async Task WhenDelegateThrowsOtherException_ShouldFail()
				{
					Func<int> @delegate = () => throw new CustomException();

					async Task Act()
						=> await That(@delegate).DoesNotThrow<OtherException>().WhoseResult.IsEqualTo(5);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that @delegate
						             does not throw a ThatDelegate.OtherException and its result is equal to 5,
						             but it did throw a ThatDelegate.CustomException:
						               WhenDelegateThrowsOtherException_ShouldFail
						             """);
				}

				[Test]
				[AutoArguments]
				public async Task WhenReturnValueDoesNotMatch_ShouldFail(int value)
				{
					Func<int> @delegate = () => value;

					async Task Act() => await That(@delegate).DoesNotThrow<CustomException>().WhoseResult
						.IsEqualTo(value + 1);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that @delegate
						              does not throw a ThatDelegate.CustomException and its result is equal to {value + 1},
						              but it was {value}, which differs by -1
						              """);
				}

				[Test]
				[AutoArguments]
				public async Task WhenReturnValueMatches_ShouldSucceed(int value)
				{
					Func<int> @delegate = () => value;

					await That(@delegate).DoesNotThrow<CustomException>().WhoseResult.IsEqualTo(value);
				}
			}

#pragma warning disable CA2263 // these tests deliberately cover the Type overloads
			public sealed class TypeTests
			{
				[Test]
				public async Task WhenDelegateThrowsExpectedException_ShouldFail()
				{
					Func<int> @delegate = () => throw new CustomException();

					async Task Act()
						=> await That(@delegate).DoesNotThrow(typeof(CustomException)).WhoseResult.IsEqualTo(5);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that @delegate
						             does not throw a ThatDelegate.CustomException and its result is equal to 5,
						             but it did throw a ThatDelegate.CustomException:
						               WhenDelegateThrowsExpectedException_ShouldFail
						             """);
				}

				[Test]
				public async Task WhenDelegateThrowsOtherException_ShouldFail()
				{
					Func<int> @delegate = () => throw new CustomException();

					async Task Act()
						=> await That(@delegate).DoesNotThrow(typeof(OtherException)).WhoseResult.IsEqualTo(5);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that @delegate
						             does not throw a ThatDelegate.OtherException and its result is equal to 5,
						             but it did throw a ThatDelegate.CustomException:
						               WhenDelegateThrowsOtherException_ShouldFail
						             """);
				}

				[Test]
				[AutoArguments]
				public async Task WhenReturnValueDoesNotMatch_ShouldFail(int value)
				{
					Func<int> @delegate = () => value;

					async Task Act() => await That(@delegate).DoesNotThrow(typeof(CustomException)).WhoseResult
						.IsEqualTo(value + 1);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that @delegate
						              does not throw a ThatDelegate.CustomException and its result is equal to {value + 1},
						              but it was {value}, which differs by -1
						              """);
				}

				[Test]
				[AutoArguments]
				public async Task WhenReturnValueMatches_ShouldSucceed(int value)
				{
					Func<int> @delegate = () => value;

					await That(@delegate).DoesNotThrow(typeof(CustomException)).WhoseResult.IsEqualTo(value);
				}
			}
#pragma warning restore CA2263
		}
	}
}
