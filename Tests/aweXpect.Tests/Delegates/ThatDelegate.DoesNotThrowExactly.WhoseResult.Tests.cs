namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class DoesNotThrowExactly
	{
		public sealed class WhoseResult
		{
			public sealed class GenericTests
			{
				[Fact]
				public async Task WhenDelegateThrowsExpectedException_ShouldFail()
				{
					Func<int> @delegate = () => throw new CustomException();

					async Task Act()
						=> await That(@delegate).DoesNotThrowExactly<CustomException>().WhoseResult.IsEqualTo(5);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that @delegate
						             does not throw exactly a ThatDelegate.CustomException and its result is equal to 5,
						             but it did throw a ThatDelegate.CustomException:
						               WhenDelegateThrowsExpectedException_ShouldFail
						             """);
				}

				[Fact]
				public async Task WhenDelegateThrowsOtherException_ShouldFail()
				{
					Func<int> @delegate = () => throw new CustomException();

					async Task Act()
						=> await That(@delegate).DoesNotThrowExactly<OtherException>().WhoseResult.IsEqualTo(5);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that @delegate
						             does not throw exactly a ThatDelegate.OtherException and its result is equal to 5,
						             but it did throw a ThatDelegate.CustomException:
						               WhenDelegateThrowsOtherException_ShouldFail
						             """);
				}

				[Theory]
				[AutoData]
				public async Task WhenReturnValueDoesNotMatch_ShouldFail(int value)
				{
					Func<int> @delegate = () => value;

					async Task Act() => await That(@delegate).DoesNotThrowExactly<CustomException>().WhoseResult
						.IsEqualTo(value + 1);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that @delegate
						              does not throw exactly a ThatDelegate.CustomException and its result is equal to {value + 1},
						              but it was {value}, which differs by -1
						              """);
				}

				[Theory]
				[AutoData]
				public async Task WhenReturnValueMatches_ShouldSucceed(int value)
				{
					Func<int> @delegate = () => value;

					await That(@delegate).DoesNotThrowExactly<CustomException>().WhoseResult.IsEqualTo(value);
				}
			}

			public sealed class TypeTests
			{
				[Fact]
				public async Task WhenDelegateThrowsExpectedException_ShouldFail()
				{
					Func<int> @delegate = () => throw new CustomException();

					async Task Act()
						=> await That(@delegate).DoesNotThrowExactly(typeof(CustomException)).WhoseResult.IsEqualTo(5);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that @delegate
						             does not throw exactly a ThatDelegate.CustomException and its result is equal to 5,
						             but it did throw a ThatDelegate.CustomException:
						               WhenDelegateThrowsExpectedException_ShouldFail
						             """);
				}

				[Fact]
				public async Task WhenDelegateThrowsOtherException_ShouldFail()
				{
					Func<int> @delegate = () => throw new CustomException();

					async Task Act()
						=> await That(@delegate).DoesNotThrowExactly(typeof(OtherException)).WhoseResult.IsEqualTo(5);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that @delegate
						             does not throw exactly a ThatDelegate.OtherException and its result is equal to 5,
						             but it did throw a ThatDelegate.CustomException:
						               WhenDelegateThrowsOtherException_ShouldFail
						             """);
				}

				[Theory]
				[AutoData]
				public async Task WhenReturnValueDoesNotMatch_ShouldFail(int value)
				{
					Func<int> @delegate = () => value;

					async Task Act() => await That(@delegate).DoesNotThrowExactly(typeof(CustomException)).WhoseResult
						.IsEqualTo(value + 1);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that @delegate
						              does not throw exactly a ThatDelegate.CustomException and its result is equal to {value + 1},
						              but it was {value}, which differs by -1
						              """);
				}

				[Theory]
				[AutoData]
				public async Task WhenReturnValueMatches_ShouldSucceed(int value)
				{
					Func<int> @delegate = () => value;

					await That(@delegate).DoesNotThrowExactly(typeof(CustomException)).WhoseResult.IsEqualTo(value);
				}
			}
		}
	}
}
