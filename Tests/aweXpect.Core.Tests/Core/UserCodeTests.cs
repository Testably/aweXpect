using System.Threading;
using aweXpect.Core.Helpers;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core;

public sealed class UserCodeTests
{
	[Test]
	public async Task Invoke_ShouldReturnTheResultOfTheCallback()
	{
		int result = UserCode.Invoke(() => 42);

		await That(result).IsEqualTo(42);
	}

	[Test]
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

	[Test]
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

	[Test]
	public async Task Invoke_WithArgument_ShouldPassTheArgument()
	{
		int result = UserCode.Invoke(value => value * 2, 21);

		await That(result).IsEqualTo(42);
	}

	[Test]
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

	[Test]
	public async Task Invoke_WithThrowerFactory_ShouldNotCreateTheThrowerWhileTheCallbackSucceeds()
	{
		bool isThrowerCreated = false;

		int result = UserCode.Invoke(() => 42, () =>
		{
			isThrowerCreated = true;
			return "the predicate";
		});

		await That(result).IsEqualTo(42);
		await That(isThrowerCreated).IsFalse()
			.Because("a name that has to be formatted must cost nothing while the code of the caller succeeds");
	}

	[Test]
	public async Task Invoke_WithThrowerFactory_WhenCallbackThrows_ShouldCarryTheExceptionAndTheThrower()
	{
		MyException exception = new();

		void Act()
			=> UserCode.Invoke<int>(() => throw exception, () => "the predicate");

		await That(Act).Throws<UserCodeException>()
			.WithMessage("The code of the caller threw an exception while the expectation was evaluated.").And
			.Whose(e => e.Exception, e => e.IsSameAs(exception)).And
			.Whose(e => e.Thrower, thrower => thrower.IsEqualTo("the predicate"));
	}

	[Test]
	public async Task Invoke_WithThrowerFactory_WhenCallbackThrowsForNestedCode_ShouldNotWrapItAgain()
	{
		MyException exception = new();

		void Act()
			=> UserCode.Invoke(() => UserCode.Invoke<int>(() => throw exception, "the inner code"),
				() => "the outer code");

		await That(Act).Throws<UserCodeException>()
			.WithMessage("The code of the caller threw an exception while the expectation was evaluated.").And
			.Whose(e => e.Thrower, thrower => thrower.IsEqualTo("the inner code"))
			.Because("only the innermost call knows the exception of the caller");
	}

	[Test]
	public async Task InvokeAsync_ShouldReturnTheResultOfTheCallback()
	{
		int result = await UserCode.InvokeAsync(() => new ValueTask<int>(42));

		await That(result).IsEqualTo(42);
	}

	[Test]
	public async Task InvokeAsync_WhenCallbackIsCancelledWithTheToken_ShouldThrowTheCancellation()
	{
		using CancellationTokenSource cts = new();
		cts.Cancel();

		async Task Act()
			=> await UserCode.InvokeAsync<int>(() => throw new OperationCanceledException("canceled", cts.Token),
				cancellationToken: cts.Token);

		await That(Act).Throws<OperationCanceledException>()
			.WithMessage("canceled")
			.Because("the caller can react to a requested cancellation before it aborts the evaluation");
	}

	[Test]
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

	[Test]
	public async Task InvokeAsync_WhenCallbackThrowsForNestedCode_ShouldNotWrapItAgain()
	{
		MyException exception = new();

		async Task Act()
			=> await UserCode.InvokeAsync(() => new ValueTask<int>(UserCode.Invoke<int>(() => throw exception,
				"the inner code")), "the outer code");

		await That(Act).Throws<UserCodeException>()
			.WithMessage("The code of the caller threw an exception while the expectation was evaluated.").And
			.Whose(e => e.Thrower, thrower => thrower.IsEqualTo("the inner code"))
			.Because("only the innermost call knows the exception of the caller");
	}

	[Test]
	public async Task InvokeAsync_WhenCallbackThrowsOperationCanceledExceptionWithoutCancellation_ShouldCarryIt()
	{
		OperationCanceledException exception = new("nothing was canceled");

		async Task Act()
			=> await UserCode.InvokeAsync<int>(() => throw exception, cancellationToken: CancellationToken.None);

		await That(Act).Throws<UserCodeException>()
			.WithMessage("The code of the caller threw an exception while the expectation was evaluated.").And
			.Whose(e => e.Exception, e => e.IsSameAs(exception))
			.Because("only a cancellation that was actually requested may abort the evaluation");
	}

	[Test]
	public async Task InvokeAsync_WithArgument_WhenCallbackIsCancelledWithTheToken_ShouldThrowTheCancellation()
	{
		using CancellationTokenSource cts = new();
		cts.Cancel();

		async Task Act()
			=> await UserCode.InvokeAsync<int, int>(_ => throw new OperationCanceledException("canceled", cts.Token),
				1, cts.Token);

		await That(Act).Throws<OperationCanceledException>()
			.WithMessage("canceled")
			.Because("the caller can react to a requested cancellation before it aborts the evaluation");
	}

	[Test]
	public async Task InvokeAsync_WithArgument_WhenCallbackThrowsForNestedCode_ShouldNotWrapItAgain()
	{
		MyException exception = new();

		async Task Act()
			=> await UserCode.InvokeAsync<int, int>(
				_ => new ValueTask<int>(UserCode.Invoke<int>(() => throw exception, "the inner code")), 1,
				CancellationToken.None);

		await That(Act).Throws<UserCodeException>()
			.WithMessage("The code of the caller threw an exception while the expectation was evaluated.").And
			.Whose(e => e.Thrower, thrower => thrower.IsEqualTo("the inner code"))
			.Because("only the innermost call knows the exception of the caller");
	}
}
