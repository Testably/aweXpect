using System.Threading;
using aweXpect.Core.Helpers;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core;

public sealed class UserCodeTests
{
	[Fact]
	public async Task InvokeAsync_ShouldReturnTheResultOfTheCallback()
	{
		int result = await UserCode.InvokeAsync(() => new ValueTask<int>(42));

		await That(result).IsEqualTo(42);
	}

	[Fact]
	public async Task InvokeAsync_WhenCallbackIsCancelledWithTheToken_ShouldThrowTheCancellation()
	{
		using CancellationTokenSource cts = new();
		cts.Cancel();

		async Task Act()
			=> await UserCode.InvokeAsync<int>(() => throw new OperationCanceledException("canceled", cts.Token),
				cts.Token);

		await That(Act).Throws<OperationCanceledException>()
			.WithMessage("canceled")
			.Because("the caller can react to a requested cancellation before it aborts the evaluation");
	}

	[Fact]
	public async Task InvokeAsync_WhenCallbackThrowsOperationCanceledExceptionWithoutCancellation_ShouldCarryIt()
	{
		OperationCanceledException exception = new("nothing was canceled");

		async Task Act()
			=> await UserCode.InvokeAsync<int>(() => throw exception, CancellationToken.None);

		await That(Act).Throws<UserCodeException>()
			.WithMessage("The code of the caller threw an exception while the expectation was evaluated.").And
			.Whose(e => e.Exception, e => e.IsSameAs(exception))
			.Because("only a cancellation that was actually requested may abort the evaluation");
	}

	[Fact]
	public async Task InvokeAsync_WhenCallbackThrows_ShouldCarryTheException()
	{
		MyException exception = new();

		async Task Act()
			=> await UserCode.InvokeAsync<int>(() => throw exception);

		await That(Act).Throws<UserCodeException>()
			.WithMessage("The code of the caller threw an exception while the expectation was evaluated.").And
			.Whose(e => e.Exception, e => e.IsSameAs(exception))
			.Because("the evaluation reports the exception of the caller as its failure");
	}

	[Fact]
	public async Task Invoke_ShouldReturnTheResultOfTheCallback()
	{
		int result = UserCode.Invoke(() => 42);

		await That(result).IsEqualTo(42);
	}

	[Fact]
	public async Task Invoke_WhenCallbackThrowsForNestedCode_ShouldNotWrapItAgain()
	{
		MyException exception = new();

		void Act()
			=> UserCode.Invoke(() => UserCode.Invoke<int>(() => throw exception));

		await That(Act).Throws<UserCodeException>()
			.WithMessage("The code of the caller threw an exception while the expectation was evaluated.").And
			.Whose(e => e.Exception, e => e.IsSameAs(exception))
			.Because("only the innermost call knows the exception of the caller");
	}

	[Fact]
	public async Task Invoke_WhenCallbackThrows_ShouldCarryTheException()
	{
		MyException exception = new();

		void Act()
			=> UserCode.Invoke<int>(() => throw exception);

		await That(Act).Throws<UserCodeException>()
			.WithMessage("The code of the caller threw an exception while the expectation was evaluated.").And
			.Whose(e => e.Exception, e => e.IsSameAs(exception))
			.Because("the evaluation reports the exception of the caller as its failure");
	}

	[Fact]
	public async Task Invoke_WithArgument_ShouldPassTheArgument()
	{
		int result = UserCode.Invoke(value => value * 2, 21);

		await That(result).IsEqualTo(42);
	}

	[Fact]
	public async Task Invoke_WithArgument_WhenCallbackThrows_ShouldCarryTheException()
	{
		MyException exception = new();

		void Act()
			=> UserCode.Invoke<int, int>(_ => throw exception, 1);

		await That(Act).Throws<UserCodeException>()
			.WithMessage("The code of the caller threw an exception while the expectation was evaluated.").And
			.Whose(e => e.Exception, e => e.IsSameAs(exception))
			.Because("the evaluation reports the exception of the caller as its failure");
	}
}
