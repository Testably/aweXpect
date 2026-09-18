namespace aweXpect.Core.Tests.Delegates;

public sealed partial class ThatDelegateTests
{
	public sealed class ArticleTests
	{
		[Fact]
		public async Task DoesNotThrow_WhenInitialismWithVowelSoundIsThrown_ShouldUseAn()
		{
			Action @delegate = () => throw new HResultException("foo");

			async Task Act()
				=> await That(@delegate).DoesNotThrow();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that @delegate
				             does not throw any exception,
				             but it did throw an HResultException:
				               foo
				             """);
		}

		[Fact]
		public async Task Throws_WhenInitialismWithVowelSoundIsExpected_ShouldUseAn()
		{
			Action @delegate = () => { };

			async Task Act()
				=> await That(@delegate).Throws<HResultException>();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that @delegate
				             throws an HResultException,
				             but it did not throw any exception
				             """);
		}

		[Fact]
		public async Task Throws_WhenUIsReadAsYou_ShouldUseA()
		{
			Action @delegate = () => { };

			async Task Act()
				=> await That(@delegate).Throws<UserException>();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that @delegate
				             throws a UserException,
				             but it did not throw any exception
				             """);
		}
	}
}

public sealed class HResultException(string message) : Exception(message);

public sealed class UserException(string message) : Exception(message);
