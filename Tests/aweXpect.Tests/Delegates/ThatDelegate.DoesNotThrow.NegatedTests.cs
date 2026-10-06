using aweXpect.Core;
using aweXpect.Core.Sources;

namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class DoesNotThrow
	{
		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenDelegateDoesNotThrow_ShouldFail()
			{
				DelegateValue value = new(null, TimeSpan.Zero);

				async Task Act()
					=> await That(value).DoesNotComplyWith(it => WithoutValue(it).DoesNotThrow());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that value
					             throws an exception,
					             but it did not throw any exception
					             """);
			}

			[Test]
			[AutoArguments]
			public async Task WhenDelegateThrowsMatchingException_ShouldSucceed(string message)
			{
				DelegateValue value = new(new CustomException(message), TimeSpan.Zero);

				async Task Act()
					=> await That(value).DoesNotComplyWith(it => WithoutValue(it).DoesNotThrow<CustomException>());

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task WhenDelegateThrowsOtherException_ShouldFail(string message)
			{
				DelegateValue value = new(new OtherException(message), TimeSpan.Zero);

				async Task Act()
					=> await That(value).DoesNotComplyWith(it => WithoutValue(it).DoesNotThrow<CustomException>());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that value
					              throws a ThatDelegate.CustomException,
					              but it did throw a ThatDelegate.OtherException:
					                {message}
					              """);
			}

			[Test]
			public async Task WithValue_WhenDelegateDoesNotThrow_ShouldFail()
			{
				DelegateValue<int> value = new(1, null, TimeSpan.Zero);

				async Task Act()
					=> await That(value).DoesNotComplyWith(it => WithValue(it).DoesNotThrow());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that value
					             throws an exception,
					             but it did not throw any exception
					             """);
			}

			[Test]
			[AutoArguments]
			public async Task WithValue_WhenDelegateThrowsMatchingException_ShouldSucceed(string message)
			{
				DelegateValue<int> value = new(0, new CustomException(message), TimeSpan.Zero);

				async Task Act()
					=> await That(value).DoesNotComplyWith(it => WithValue(it).DoesNotThrow<CustomException>());

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task WithValue_WhenDelegateThrowsOtherException_ShouldFail(string message)
			{
				DelegateValue<int> value = new(0, new OtherException(message), TimeSpan.Zero);

				async Task Act()
					=> await That(value).DoesNotComplyWith(it => WithValue(it).DoesNotThrow<CustomException>());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that value
					              throws a ThatDelegate.CustomException,
					              but it did throw a ThatDelegate.OtherException:
					                {message}
					              """);
			}

			[Test]
			public async Task WithValue_WhoseResult_WhenDelegateThrows_ShouldSucceed()
			{
				DelegateValue<int> value = new(5, new CustomException(), TimeSpan.Zero);

				async Task Act()
					=> await That(value).DoesNotComplyWith(it => WithValue(it).DoesNotThrow().WhoseResult.IsEqualTo(5));

				await That(Act).DoesNotThrow()
					.Because("a delegate that throws meets the negation, whatever its result");
			}

			[Test]
			public async Task WithValue_WhoseResult_WhenResultDiffers_ShouldSucceed()
			{
				DelegateValue<int> value = new(6, null, TimeSpan.Zero);

				async Task Act()
					=> await That(value).DoesNotComplyWith(it => WithValue(it).DoesNotThrow().WhoseResult.IsEqualTo(5));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithValue_WhoseResult_WhenResultIsEqual_ShouldFail()
			{
				DelegateValue<int> value = new(5, null, TimeSpan.Zero);

				async Task Act()
					=> await That(value).DoesNotComplyWith(it => WithValue(it).DoesNotThrow().WhoseResult.IsEqualTo(5));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that value
					             throws an exception or its result is not equal to 5,
					             but it did not throw any exception and it was 5
					             """)
					.Because("the delegate did not throw and its result is 5");
			}

			[Test]
			public async Task WithValue_WhoseResultOfType_WhenResultIsEqual_ShouldFail()
			{
				DelegateValue<int> value = new(5, null, TimeSpan.Zero);

				async Task Act()
					=> await That(value).DoesNotComplyWith(it
						=> WithValue(it).DoesNotThrow<CustomException>().WhoseResult.IsEqualTo(5));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that value
					             throws a ThatDelegate.CustomException or its result is not equal to 5,
					             but it did not throw any exception and it was 5
					             """)
					.Because("the delegate did not throw and its result is 5");
			}

			private static Delegates.ThatDelegate.WithoutValue WithoutValue(IThat<DelegateValue> subject)
				=> new(((IExpectThat<DelegateValue>)subject).ExpectationBuilder);

			private static Delegates.ThatDelegate.WithValue<int> WithValue(IThat<DelegateValue<int>> subject)
				=> new(((IExpectThat<DelegateValue<int>>)subject).ExpectationBuilder,
					_ => throw new NotSupportedException());
		}
	}
}
