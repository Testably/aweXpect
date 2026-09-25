using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core;

public class BecauseTests
{
	[Fact]
	public async Task ActionDelegate_ShouldApplyAsyncBecauseReason()
	{
		string because = "this is the reason";
		Task<string?> becauseTask = Task.Delay(5).ContinueWith(_ => because)!;
		Action subject = () => throw new MyException();

		async Task Act()
		{
			await That(subject).DoesNotThrow().Because(becauseTask);
		}

		await That(Act).Throws().WithMessage($"*{because}*").AsWildcard();
	}

	[Fact]
	public async Task ActionDelegate_ShouldApplyBecauseReason()
	{
		string because = "this is the reason";
		Action subject = () => throw new MyException();

		async Task Act()
		{
			await That(subject).DoesNotThrow().Because(because);
		}

		await That(Act).Throws().WithMessage($"*{because}*").AsWildcard();
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	public async Task ActionDelegate_WhenAsyncReasonIsNullOrEmpty_ShouldNotIncludeBecause(string? because)
	{
		Task<string?> becauseTask = Task.FromResult(because);
		Action subject = () => throw new MyException();

		async Task Act()
		{
			await That(subject).DoesNotThrow().Because(becauseTask);
		}

		Exception exception = await That(Act).Throws();
		await That(exception.Message).DoesNotContain("because");
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	public async Task ActionDelegate_WhenReasonIsNullOrEmpty_ShouldNotIncludeBecause(string? because)
	{
		Action subject = () => throw new MyException();

		async Task Act()
		{
			await That(subject).DoesNotThrow().Because(because);
		}

		Exception exception = await That(Act).Throws();
		await That(exception.Message).DoesNotContain("because");
	}

	[Fact]
	public async Task ASpecifiedAsyncBecauseReason_ShouldBeIncludedInMessage()
	{
		Task<string?> becauseTask = Task.FromResult<string?>("I want to test an async 'because'");
		bool subject = true;

		async Task Act()
		{
			await That(subject).IsFalse().Because(becauseTask);
		}

		await That(Act).Throws()
			.WithMessage("""
			             Expected that subject
			             is False, because I want to test an async 'because',
			             but it was True
			             """);
	}

	[Fact]
	public async Task ASpecifiedBecauseReason_ShouldBeIncludedInMessage()
	{
		string because = "I want to test 'because'";
		bool subject = true;

		async Task Act()
		{
			await That(subject).IsFalse().Because(because);
		}

		await That(Act).Throws().WithMessage($"*{because}*").AsWildcard();
	}

	[Fact]
	public async Task FuncDelegate_ShouldApplyAsyncBecauseReason()
	{
		string because = "this is the reason";
		Task<string?> becauseTask = Task.Delay(5).ContinueWith(_ => because)!;
		Func<int> subject = () => throw new MyException();

		async Task Act()
		{
			await That(subject).DoesNotThrow().Because(becauseTask);
		}

		await That(Act).Throws().WithMessage($"*{because}*").AsWildcard();
	}

	[Fact]
	public async Task FuncDelegate_ShouldApplyBecauseReason()
	{
		string because = "this is the reason";
		Func<int> subject = () => throw new MyException();

		async Task Act()
		{
			await That(subject).DoesNotThrow().Because(because);
		}

		await That(Act).Throws().WithMessage($"*{because}*").AsWildcard();
	}

	[Theory]
	[InlineData("we prefix the reason", "because we prefix the reason")]
	[InlineData("  we ignore whitespace", "because we ignore whitespace")]
	[InlineData("because we honor a leading 'because'", "because we honor a leading 'because'")]
	public async Task ShouldPrefixReasonWithBecause(string because, string expectedWithPrefix)
	{
		bool subject = true;

		async Task Act()
		{
			await That(subject).IsFalse().Because(because);
		}

		await That(Act).Throws().WithMessage($"*{expectedWithPrefix}*")
			.AsWildcard();
	}

	[Fact]
	public async Task WhenApplyBecauseReasonMultipleTimes_ShouldNotOverwritePreviousReason()
	{
		string because1 = "this is the first reason";
		string because2 = "this is the second reason";
		bool subject = false;

		async Task Act()
		{
			await That(subject).IsTrue().Because(because1)
				.And.IsFalse().Because(because2);
		}

		await That(Act).Throws().WithMessage($"*{because1}*").AsWildcard();
	}

	[Fact]
	public async Task WhenAsyncReasonIsCancelled_ShouldStillReportTheAssertionFailure()
	{
		TaskCompletionSource<string?> becauseSource = new();
		becauseSource.SetCanceled();
		bool subject = true;

		async Task Act()
		{
			await That(subject).IsFalse().Because(becauseSource.Task);
		}

		await That(Act).Throws()
			.WithMessage("""
			             Expected that subject
			             is False, because the reason did throw a TaskCanceledException: *,
			             but it was True
			             """).AsWildcard();
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	public async Task WhenAsyncReasonIsNullOrEmpty_ShouldNotIncludeBecause(string? because)
	{
		Task<string?> becauseTask = Task.FromResult(because);
		bool subject = true;

		async Task Act()
		{
			await That(subject).IsFalse().Because(becauseTask);
		}

		await That(Act).Throws()
			.WithMessage("""
			             Expected that subject
			             is False,
			             but it was True
			             """);
	}

	[Fact]
	public async Task WhenAsyncReasonIsSlow_WhenExpectationIsMet_ShouldNotAwaitTheReason()
	{
		bool reasonWasResolved = false;
		Task<string?> becauseTask = Task.Delay(500).ContinueWith(_ =>
		{
			reasonWasResolved = true;
			return (string?)"of reasons";
		});

		await That(1).IsEqualTo(1).Because(becauseTask);

		await That(reasonWasResolved).IsFalse()
			.Because("a met expectation never builds a failure message, so it must not wait for the reason");
	}

	[Fact]
	public async Task WhenAsyncReasonThrows_WhenExpectationFails_ShouldStillReportTheAssertionFailure()
	{
		Task<string?> becauseTask = Task.FromException<string?>(new MyException("the reason provider is broken"));
		bool subject = true;

		async Task Act()
		{
			await That(subject).IsFalse().Because(becauseTask);
		}

		await That(Act).Throws()
			.WithMessage("""
			             Expected that subject
			             is False, because the reason did throw a MyException: the reason provider is broken,
			             but it was True
			             """);
	}

	[Fact]
	public async Task WhenAsyncReasonThrows_WhenExpectationIsMet_ShouldNotThrow()
	{
		Task<string?> becauseTask = Task.FromException<string?>(new MyException("the reason provider is broken"));

		async Task Act()
		{
			await That(1).IsEqualTo(1).Because(becauseTask);
		}

		await That(Act).DoesNotThrow();
	}

	[Fact]
	public async Task WhenCombineWithAnd_ShouldApplyBecauseReason()
	{
		string because1 = "this is the first reason";
		string because2 = "this is the second reason";
		bool subject = true;

		async Task Act()
		{
			await That(subject).IsTrue().Because(because1)
				.And.IsFalse().Because(because2);
		}

		await That(Act).Throws().WithMessage($"*{because2}*").AsWildcard();
	}

	[Fact]
	public async Task WhenCombineWithAnd_ShouldAppendReasonAfterAllConstraints()
	{
		string because = "we append it after all constraints";
		bool subject = true;

		async Task Act()
		{
			await That(subject).IsTrue().Because(because)
				.And.IsFalse();
		}

		await That(Act).Throws()
			.WithMessage("""
			             Expected that subject
			             is True and is False, because we append it after all constraints,
			             but it was True
			             """);
	}

	[Fact]
	public async Task WhenCombineWithAnd_WithReasonOnEachConstraint_ShouldAppendAllReasonsInOrder()
	{
		bool subject = true;

		async Task Act()
		{
			await That(subject).IsTrue().Because("of the first reason")
				.And.IsFalse().Because(Task.FromResult<string?>("of the second reason"));
		}

		await That(Act).Throws()
			.WithMessage("""
			             Expected that subject
			             is True and is False, because of the first reason, because of the second reason,
			             but it was True
			             """);
	}

	[Fact]
	public async Task WhenCombineWithOr_ShouldAppendReasonAfterAllConstraints()
	{
		bool subject = true;

		async Task Act()
		{
			await That(subject).IsFalse().Because("of reasons")
				.Or.IsFalse();
		}

		await That(Act).Throws()
			.WithMessage("""
			             Expected that subject
			             is False or is False, because of reasons,
			             but it was True
			             """);
	}

	[Fact]
	public async Task WhenCombinedWithWhichContinuation_ShouldAppendReasonAfterTheContinuation()
	{
		Action subject = () => throw new MyException("foo");

		async Task Act()
		{
			await That(subject).Throws<MyException>().Because("of reasons")
				.WithMessage("bar");
		}

		await That(Act).Throws()
			.WithMessage("""
			             Expected that subject
			             throws a MyException with message equal to "bar", because of reasons,
			             but it had message "foo" which differs at index 0:
			                ↓ (actual)
			               "foo"
			               "bar"
			                ↑ (expected)

			             Message:
			             foo
			             """);
	}

	[Fact]
	public async Task WhenUsedInExpectThatAll_ShouldAppendReasonToEachExpectation()
	{
		async Task Act()
		{
			await ThatAll(
				That(true).IsFalse().Because("of the first reason").And.IsTrue(),
				That(1).IsEqualTo(2).Because("of the second reason"));
		}

		await That(Act).Throws()
			.WithMessage("""
			             Expected all of the following to succeed:
			              [01] Expected that true is False and is True, because of the first reason
			              [02] Expected that 1 is equal to 2, because of the second reason
			             but
			              [01] it was True
			              [02] it was 1 which differs by -1
			             """);
	}

	[Fact]
	public async Task WhenCombineWithOr_ShouldApplyBecauseReason()
	{
		string because1 = "this is the first reason";
		string because2 = "this is the second reason";
		bool subject = true;

		async Task Act()
		{
			await That(subject).IsFalse().Because(because1)
				.Or.IsFalse().Because(because2);
		}

		await That(Act).Throws().WithMessage($"*{because1}*{because2}*")
			.AsWildcard();
	}

	[Fact]
	public async Task WhenNoBecauseReasonIsGiven_ShouldNotIncludeBecause()
	{
		bool subject = true;

		async Task Act()
		{
			await That(subject).IsFalse();
		}

		await That(Act).Throws()
			.WithMessage("""
			             Expected that subject
			             is False,
			             but it was True
			             """);
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	public async Task WhenReasonIsNullOrEmpty_ShouldNotIncludeBecause(string? because)
	{
		bool subject = true;

		async Task Act()
		{
			await That(subject).IsFalse().Because(because);
		}

		await That(Act).Throws()
			.WithMessage("""
			             Expected that subject
			             is False,
			             but it was True
			             """);
	}

	[Fact]
	public async Task WhenReasonStartsWithBecause_ShouldHonorExistingPrefix()
	{
		string because = "because we honor a leading 'because'";
		bool subject = true;

		async Task Act()
		{
			await That(subject).IsFalse().Because(because);
		}

		Exception exception = await That(Act).Throws()
			.WithMessage("*because*").AsWildcard();
		await That(exception.Message).DoesNotContain("because because");
	}
}
