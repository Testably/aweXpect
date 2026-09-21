using System.Diagnostics;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core;

public sealed class UnexpectedExceptionTests
{
	[Fact]
	public async Task AsyncDelegate_DoesNotThrow_ShouldRenderTypeAndIndentedMessage()
	{
		Func<Task> subject = () => Task.FromException(new MyException($"first line{Environment.NewLine}second line"));

		async Task Act()
			=> await That(subject).DoesNotThrow();

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             does not throw any exception,
			             but it did throw a MyException:
			               first line
			               second line
			             """);
	}

	[Fact]
	public async Task Delegate_DoesNotThrow_WhenMessageIsEmpty_ShouldOnlyRenderType()
	{
		Action subject = () => throw new MyException("");

		async Task Act()
			=> await That(subject).DoesNotThrow();

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             does not throw any exception,
			             but it did throw a MyException
			             """);
	}

	[Fact]
	public async Task Delegate_WithInner_ShouldNameTheInnerRelation()
	{
		Action subject = () => throw new MyException("outer", new ArgumentException("inner"));

		async Task Act()
			=> await That(subject).Throws<MyException>().WithInner<InvalidOperationException>();

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             throws a MyException with an inner InvalidOperationException,
			             but it had an inner ArgumentException:
			               inner
			             """);
	}

	[Fact]
	public async Task Task_WhenFailed_ShouldRenderTypeAndIndentedMessage()
	{
		Task<int> subject = Task.FromException<int>(new MyException($"first line{Environment.NewLine}second line"));

		async Task Act()
			=> await That(subject).IsEqualTo(1).Because("the value is required");

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             is equal to 1, because the value is required,
			             but it did throw a MyException:
			               first line
			               second line
			             """);
	}

	[Fact]
	public async Task Task_WhenFailed_ShouldForwardExceptionAsInnerException()
	{
		MyException exception = new("failure");
		Task<int> subject = Task.FromException<int>(exception);

		async Task Act()
			=> await That(subject).IsEqualTo(1);

		await That(Act).Throws<XunitException>()
			.Whose(e => e.InnerException, i => i.IsSameAs(exception));
	}

	[Fact]
	public async Task Task_WhenFailed_AndExpectationThrowsOnDefault_ShouldFailWithTheException()
	{
		Task<int> subject = Task.FromException<int>(new MyException("failure"));

		async Task Act()
			=> await That(subject).Satisfies(x => 10 / x > 1);

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             satisfies x => 10 / x > 1,
			             but it did throw a MyException:
			               failure
			             """)
			.And.WithInner<MyException>(inner => inner.HasMessage("failure"));
	}

	[Fact]
	public async Task Task_WhenFailed_AndExpectationIsNegated_ShouldRenderTheNegatedExpectation()
	{
		Task<int> subject = Task.FromException<int>(new MyException("failure"));

		async Task Act()
			=> await That(subject).DoesNotComplyWith(it => it.IsEqualTo(1));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             is not equal to 1,
			             but it did throw a MyException:
			               failure
			             """)
			.And.WithInner<MyException>(inner => inner.HasMessage("failure"));
	}

	[Fact]
	public async Task Task_WhenFailed_AndExpectationIsRepeated_ShouldFailWithoutRetrying()
	{
		Task<int> subject = Task.FromException<int>(new MyException("failure"));
		Stopwatch stopwatch = Stopwatch.StartNew();

		async Task Act()
			=> await That(subject).Satisfies(x => x == 1).Within(TimeSpan.FromSeconds(2));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             satisfies x => x == 1 within 0:02,
			             but it did throw a MyException:
			               failure
			             """)
			.And.WithInner<MyException>(inner => inner.HasMessage("failure"));
		await That(stopwatch.Elapsed).IsLessThan(TimeSpan.FromSeconds(1));
	}

	[Fact]
	public async Task Task_WhenFailedWithPlainException_ShouldRenderTypeAndMessage()
	{
		Task<int> subject = Task.FromException<int>(new Exception("failure"));

		async Task Act()
			=> await That(subject).IsEqualTo(1);

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             is equal to 1,
			             but it did throw an Exception:
			               failure
			             """);
	}

	[Fact]
	public async Task ThatAll_WhenTaskAndDelegateFail_ShouldIndentTheMessages()
	{
		Task<int> task = Task.FromException<int>(new MyException($"task line 1{Environment.NewLine}task line 2"));
		Action action = () => throw new MyException($"action line 1{Environment.NewLine}action line 2");

		async Task Act()
			=> await ThatAll(
				That(task).IsEqualTo(1),
				That(action).DoesNotThrow());

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected all of the following to succeed:
			              [01] Expected that task is equal to 1
			              [02] Expected that action does not throw any exception
			             but
			              [01] it did throw a MyException:
			                     task line 1
			                     task line 2
			              [02] it did throw a MyException:
			                     action line 1
			                     action line 2
			             """);
	}

#if NET8_0_OR_GREATER
	[Fact]
	public async Task ValueTask_WhenFailed_ShouldRenderTypeAndIndentedMessage()
	{
		ValueTask<int> subject = new(Task.FromException<int>(new MyException("failure")));

		async Task Act()
			=> await That(subject).IsEqualTo(1);

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             is equal to 1,
			             but it did throw a MyException:
			               failure
			             """);
	}
#endif
}
