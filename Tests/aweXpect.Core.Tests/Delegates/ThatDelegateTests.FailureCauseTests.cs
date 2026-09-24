using aweXpect.Chronology;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Delegates;

public sealed partial class ThatDelegateTests
{
	public sealed class FailureCauseTests
	{
		[Fact]
		public async Task DoesNotThrow_WhenDelegateThrows_ShouldForwardExceptionAsInnerException()
		{
			Exception exception = new MyException();
			Action @delegate = () => throw exception;

			async Task Act()
				=> await That(@delegate).DoesNotThrow();

			await That(Act).Throws<XunitException>()
				.WithMessage($"""
				              Expected that @delegate
				              does not throw any exception,
				              but it did throw a MyException:
				                {nameof(DoesNotThrow_WhenDelegateThrows_ShouldForwardExceptionAsInnerException)}
				              """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Fact]
		public async Task DoesNotThrow_WhoseResult_WhenDelegateThrows_ShouldForwardExceptionAsInnerException()
		{
			Exception exception = new MyException();
			Func<int> @delegate = () => throw exception;

			async Task Act()
				=> await That(@delegate).DoesNotThrow().WhoseResult.IsEqualTo(1);

			await That(Act).Throws<XunitException>()
				.WithMessage($"""
				              Expected that @delegate
				              does not throw any exception and its result is equal to 1,
				              but it did throw a MyException:
				                {nameof(DoesNotThrow_WhoseResult_WhenDelegateThrows_ShouldForwardExceptionAsInnerException)}
				              """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Fact]
		public async Task DoesNotThrow_WithValue_WhenDelegateThrows_ShouldForwardExceptionAsInnerException()
		{
			Exception exception = new MyException();
			Func<int> @delegate = () => throw exception;

			async Task Act()
				=> await That(@delegate).DoesNotThrow();

			await That(Act).Throws<XunitException>()
				.WithMessage($"""
				              Expected that @delegate
				              does not throw any exception,
				              but it did throw a MyException:
				                {nameof(DoesNotThrow_WithValue_WhenDelegateThrows_ShouldForwardExceptionAsInnerException)}
				              """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Fact]
		public async Task DoesNotThrowExactly_WhenMatchingExceptionIsThrown_ShouldForwardExceptionAsInnerException()
		{
			Exception exception = new MyException();
			Action @delegate = () => throw exception;

			async Task Act()
				=> await That(@delegate).DoesNotThrowExactly<MyException>();

			await That(Act).Throws<XunitException>()
				.WithMessage($"""
				              Expected that @delegate
				              does not throw exactly a MyException,
				              but it did throw a MyException:
				                {nameof(DoesNotThrowExactly_WhenMatchingExceptionIsThrown_ShouldForwardExceptionAsInnerException)}
				              """).And
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

			await That(Act).Throws<XunitException>()
				.WithMessage($"""
				              Expected that @delegate
				              does not throw exactly a MyException,
				              but it did throw a MyException:
				                {nameof(DoesNotThrowExactly_WithValue_WhenMatchingExceptionIsThrown_ShouldForwardExceptionAsInnerException)}
				              """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Fact]
		public async Task ExecutesWithin_WhenDelegateThrows_ShouldForwardExceptionAsInnerException()
		{
			Exception exception = new MyException();
			Action @delegate = () => throw exception;

			async Task Act()
				=> await That(@delegate).ExecutesWithin(5.Seconds());

			await That(Act).Throws<XunitException>()
				.WithMessage($"""
				              Expected that @delegate
				              executes within 0:05,
				              but it did throw a MyException:
				                {nameof(ExecutesWithin_WhenDelegateThrows_ShouldForwardExceptionAsInnerException)}
				              """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Fact]
		public async Task ExecutesWithin_WithValue_WhenDelegateThrows_ShouldForwardExceptionAsInnerException()
		{
			Exception exception = new MyException();
			Func<int> @delegate = () => throw exception;

			async Task Act()
				=> await That(@delegate).ExecutesWithin(5.Seconds());

			await That(Act).Throws<XunitException>()
				.WithMessage($"""
				              Expected that @delegate
				              executes within 0:05,
				              but it did throw a MyException:
				                {nameof(ExecutesWithin_WithValue_WhenDelegateThrows_ShouldForwardExceptionAsInnerException)}
				              """).And
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

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that @delegate
				             throws a MyException within 0:00.005,
				             but it took 0:*
				             """).AsWildcard().And
				.Whose(e => e.InnerException, i => i.IsNull());
		}

		[Fact]
		public async Task Throws_WhenOtherExceptionIsThrown_ShouldForwardExceptionAsInnerException()
		{
			Exception exception = new MyException();
			Action @delegate = () => throw exception;

			async Task Act()
				=> await That(@delegate).Throws<InvalidOperationException>();

			await That(Act).Throws<XunitException>()
				.WithMessage($"""
				              Expected that @delegate
				              throws an InvalidOperationException,
				              but it did throw a MyException:
				                {nameof(Throws_WhenOtherExceptionIsThrown_ShouldForwardExceptionAsInnerException)}
				              """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Fact]
		public async Task ThrowsExactly_WhenOtherExceptionIsThrown_ShouldForwardExceptionAsInnerException()
		{
			Exception exception = new MyException();
			Action @delegate = () => throw exception;

			async Task Act()
				=> await That(@delegate).ThrowsExactly<InvalidOperationException>();

			await That(Act).Throws<XunitException>()
				.WithMessage($"""
				              Expected that @delegate
				              throws exactly an InvalidOperationException,
				              but it did throw a MyException:
				                {nameof(ThrowsExactly_WhenOtherExceptionIsThrown_ShouldForwardExceptionAsInnerException)}
				              """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}
	}
}
