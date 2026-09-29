using aweXpect.Core;
using aweXpect.Core.Sources;

namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class DoesNotThrowExactly
	{
		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenDelegateDoesNotThrow_ShouldFail()
			{
				DelegateValue value = new(null, TimeSpan.Zero);

				async Task Act()
					=> await That(value).DoesNotComplyWith(it
						=> WithoutValue(it).DoesNotThrowExactly<CustomException>());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that value
					             throws exactly a ThatDelegate.CustomException,
					             but it did not throw any exception
					             """);
			}

			[Theory]
			[AutoData]
			public async Task WhenDelegateThrowsMatchingException_ShouldSucceed(string message)
			{
				DelegateValue value = new(new CustomException(message), TimeSpan.Zero);

				async Task Act()
					=> await That(value).DoesNotComplyWith(it
						=> WithoutValue(it).DoesNotThrowExactly<CustomException>());

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WhenDelegateThrowsSubtypeOfException_ShouldFail(string message)
			{
				DelegateValue value = new(new SubCustomException(message), TimeSpan.Zero);

				async Task Act()
					=> await That(value).DoesNotComplyWith(it
						=> WithoutValue(it).DoesNotThrowExactly<CustomException>());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that value
					              throws exactly a ThatDelegate.CustomException,
					              but it did throw a ThatDelegate.SubCustomException:
					                {message}
					              """);
			}

			[Fact]
			public async Task WithValue_WhenDelegateDoesNotThrow_ShouldFail()
			{
				DelegateValue<int> value = new(1, null, TimeSpan.Zero);

				async Task Act()
					=> await That(value).DoesNotComplyWith(it
						=> WithValue(it).DoesNotThrowExactly<CustomException>());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that value
					             throws exactly a ThatDelegate.CustomException,
					             but it did not throw any exception
					             """);
			}

			[Theory]
			[AutoData]
			public async Task WithValue_WhenDelegateThrowsMatchingException_ShouldSucceed(string message)
			{
				DelegateValue<int> value = new(0, new CustomException(message), TimeSpan.Zero);

				async Task Act()
					=> await That(value).DoesNotComplyWith(it
						=> WithValue(it).DoesNotThrowExactly<CustomException>());

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WithValue_WhenDelegateThrowsSubtypeOfException_ShouldFail(string message)
			{
				DelegateValue<int> value = new(0, new SubCustomException(message), TimeSpan.Zero);

				async Task Act()
					=> await That(value).DoesNotComplyWith(it
						=> WithValue(it).DoesNotThrowExactly<CustomException>());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that value
					              throws exactly a ThatDelegate.CustomException,
					              but it did throw a ThatDelegate.SubCustomException:
					                {message}
					              """);
			}

			private static Delegates.ThatDelegate.WithoutValue WithoutValue(IThat<DelegateValue> subject)
				=> new(((IExpectThat<DelegateValue>)subject).ExpectationBuilder);

			private static Delegates.ThatDelegate.WithValue<int> WithValue(IThat<DelegateValue<int>> subject)
				=> new(((IExpectThat<DelegateValue<int>>)subject).ExpectationBuilder);
		}
	}
}
