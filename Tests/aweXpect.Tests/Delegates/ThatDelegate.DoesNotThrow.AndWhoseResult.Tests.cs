namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class DoesNotThrow
	{
		public sealed class AndWhoseResult
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenDelegateThrows_ShouldFail()
				{
					Func<int> @delegate = () => throw new CustomException();

					async Task Act() => await That(@delegate).DoesNotThrow().AndWhoseResult.IsGreaterThan(5);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that @delegate
						             does not throw any exception and its result is greater than 5,
						             but it did throw a ThatDelegate.CustomException:
						               WhenDelegateThrows_ShouldFail
						             """);
				}

				[Theory]
				[AutoData]
				public async Task WhenReturnValueDoesNotMatch_ShouldFail(int value)
				{
					Func<int> @delegate = () => value;

					async Task Act() => await That(@delegate).DoesNotThrow().AndWhoseResult.IsLessThan(value);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that @delegate
						              does not throw any exception and its result is less than {value},
						              but the result was {value}
						              """);
				}

				[Theory]
				[AutoData]
				public async Task WhenReturnValueMatches_ShouldSucceed(int value)
				{
					Func<int> @delegate = () => value;

					await That(@delegate).DoesNotThrow().AndWhoseResult.IsEqualTo(value);
				}
			}

			public sealed class GenericTests
			{
				[Fact]
				public async Task WhenDelegateThrowsExpectedException_ShouldFail()
				{
					Func<int> @delegate = () => throw new CustomException();

					async Task Act()
						=> await That(@delegate).DoesNotThrow<CustomException>().AndWhoseResult.IsEqualTo(5);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that @delegate
						             does not throw a ThatDelegate.CustomException and its result is equal to 5,
						             but it did throw a ThatDelegate.CustomException:
						               WhenDelegateThrowsExpectedException_ShouldFail
						             """);
				}

				[Fact]
				public async Task WhenDelegateThrowsOtherException_ShouldFail()
				{
					Func<int> @delegate = () => throw new CustomException();

					async Task Act()
						=> await That(@delegate).DoesNotThrow<OtherException>().AndWhoseResult.IsEqualTo(5);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that @delegate
						             does not throw a ThatDelegate.OtherException and its result is equal to 5,
						             but it did throw a ThatDelegate.CustomException:
						               WhenDelegateThrowsOtherException_ShouldFail
						             """);
				}

				[Theory]
				[AutoData]
				public async Task WhenReturnValueDoesNotMatch_ShouldFail(int value)
				{
					Func<int> @delegate = () => value;

					async Task Act() => await That(@delegate).DoesNotThrow<CustomException>().AndWhoseResult
						.IsEqualTo(value + 1);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that @delegate
						              does not throw a ThatDelegate.CustomException and its result is equal to {value + 1},
						              but the result was {value} which differs by -1
						              """);
				}

				[Theory]
				[AutoData]
				public async Task WhenReturnValueMatches_ShouldSucceed(int value)
				{
					Func<int> @delegate = () => value;

					await That(@delegate).DoesNotThrow<CustomException>().AndWhoseResult.IsEqualTo(value);
				}
			}

			public sealed class TypeTests
			{
				[Fact]
				public async Task WhenDelegateThrowsExpectedException_ShouldFail()
				{
					Func<int> @delegate = () => throw new CustomException();

					async Task Act()
						=> await That(@delegate).DoesNotThrow(typeof(CustomException)).AndWhoseResult.IsEqualTo(5);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that @delegate
						             does not throw a ThatDelegate.CustomException and its result is equal to 5,
						             but it did throw a ThatDelegate.CustomException:
						               WhenDelegateThrowsExpectedException_ShouldFail
						             """);
				}

				[Fact]
				public async Task WhenDelegateThrowsOtherException_ShouldFail()
				{
					Func<int> @delegate = () => throw new CustomException();

					async Task Act()
						=> await That(@delegate).DoesNotThrow(typeof(OtherException)).AndWhoseResult.IsEqualTo(5);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that @delegate
						             does not throw a ThatDelegate.OtherException and its result is equal to 5,
						             but it did throw a ThatDelegate.CustomException:
						               WhenDelegateThrowsOtherException_ShouldFail
						             """);
				}

				[Theory]
				[AutoData]
				public async Task WhenReturnValueDoesNotMatch_ShouldFail(int value)
				{
					Func<int> @delegate = () => value;

					async Task Act() => await That(@delegate).DoesNotThrow(typeof(CustomException)).AndWhoseResult
						.IsEqualTo(value + 1);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that @delegate
						              does not throw a ThatDelegate.CustomException and its result is equal to {value + 1},
						              but the result was {value} which differs by -1
						              """);
				}

				[Theory]
				[AutoData]
				public async Task WhenReturnValueMatches_ShouldSucceed(int value)
				{
					Func<int> @delegate = () => value;

					await That(@delegate).DoesNotThrow(typeof(CustomException)).AndWhoseResult.IsEqualTo(value);
				}
			}
		}
	}
}
