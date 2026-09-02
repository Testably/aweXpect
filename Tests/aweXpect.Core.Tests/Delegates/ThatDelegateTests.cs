using aweXpect.Chronology;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Delegates;

public sealed class ThatDelegateTests
{
	public sealed class FailureCauseTests
	{
		[Fact]
		public async Task DoesNotThrow_AndWhoseResult_WhenDelegateThrows_ShouldForwardExceptionAsInnerException()
		{
			Exception exception = new MyException();
			Func<int> @delegate = () => throw exception;

			async Task Act()
				=> await That(@delegate).DoesNotThrow().AndWhoseResult.IsEqualTo(1);

			await That(Act).ThrowsException()
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Fact]
		public async Task DoesNotThrow_WhenDelegateThrows_ShouldForwardExceptionAsInnerException()
		{
			Exception exception = new MyException();
			Action @delegate = () => throw exception;

			async Task Act()
				=> await That(@delegate).DoesNotThrow();

			await That(Act).ThrowsException()
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Fact]
		public async Task DoesNotThrow_WithValue_WhenDelegateThrows_ShouldForwardExceptionAsInnerException()
		{
			Exception exception = new MyException();
			Func<int> @delegate = () => throw exception;

			async Task Act()
				=> await That(@delegate).DoesNotThrow();

			await That(Act).ThrowsException()
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Fact]
		public async Task DoesNotThrowExactly_WhenMatchingExceptionIsThrown_ShouldForwardExceptionAsInnerException()
		{
			Exception exception = new MyException();
			Action @delegate = () => throw exception;

			async Task Act()
				=> await That(@delegate).DoesNotThrowExactly<MyException>();

			await That(Act).ThrowsException()
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Fact]
		public async Task
			DoesNotThrowExactly_WithValue_WhenMatchingExceptionIsThrown_ShouldForwardExceptionAsInnerException()
		{
			Exception exception = new MyException();
			Func<int> @delegate = () => throw exception;

			async Task Act()
				=> await That(@delegate).DoesNotThrowExactly<MyException>();

			await That(Act).ThrowsException()
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Fact]
		public async Task ExecutesWithin_WhenDelegateThrows_ShouldForwardExceptionAsInnerException()
		{
			Exception exception = new MyException();
			Action @delegate = () => throw exception;

			async Task Act()
				=> await That(@delegate).ExecutesWithin(5.Seconds());

			await That(Act).ThrowsException()
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Fact]
		public async Task ExecutesWithin_WithValue_WhenDelegateThrows_ShouldForwardExceptionAsInnerException()
		{
			Exception exception = new MyException();
			Func<int> @delegate = () => throw exception;

			async Task Act()
				=> await That(@delegate).ExecutesWithin(5.Seconds());

			await That(Act).ThrowsException()
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Fact]
		public async Task Throws_WhenExpectedExceptionIsThrownTooLate_ShouldNotForwardExceptionAsInnerException()
		{
			Exception exception = new MyException();
			Action @delegate = () =>
			{
				Task.Delay(50.Milliseconds()).Wait();
				throw exception;
			};

			async Task Act()
				=> await That(@delegate).Throws<MyException>().Within(5.Milliseconds());

			await That(Act).ThrowsException()
				.Whose(e => e.InnerException, i => i.IsNull());
		}

		[Fact]
		public async Task Throws_WhenOtherExceptionIsThrown_ShouldForwardExceptionAsInnerException()
		{
			Exception exception = new MyException();
			Action @delegate = () => throw exception;

			async Task Act()
				=> await That(@delegate).Throws<InvalidOperationException>();

			await That(Act).ThrowsException()
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Fact]
		public async Task ThrowsExactly_WhenOtherExceptionIsThrown_ShouldForwardExceptionAsInnerException()
		{
			Exception exception = new MyException();
			Action @delegate = () => throw exception;

			async Task Act()
				=> await That(@delegate).ThrowsExactly<InvalidOperationException>();

			await That(Act).ThrowsException()
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}
	}
}
