using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core;

public class BecauseTests
{
	[Fact]
	public async Task ActionDelegate_ShouldApplyAsyncBecauseReason()
	{
		string because = "this is the reason";
		Task<string> becauseTask = Task.Delay(5).ContinueWith(_ => because);
		Action subject = () => throw new MyException();

		async Task Act()
		{
			await That(subject).DoesNotThrow().Because(becauseTask);
		}

		await That(Act).ThrowsException().WithMessage($"*{because}*").AsWildcard();
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

		await That(Act).ThrowsException().WithMessage($"*{because}*").AsWildcard();
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

		Exception exception = await That(Act).ThrowsException();
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

		Exception exception = await That(Act).ThrowsException();
		await That(exception.Message).DoesNotContain("because");
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

		await That(Act).ThrowsException().WithMessage($"*{because}*").AsWildcard();
	}

	[Fact]
	public async Task FuncDelegate_ShouldApplyAsyncBecauseReason()
	{
		string because = "this is the reason";
		Task<string> becauseTask = Task.Delay(5).ContinueWith(_ => because);
		Func<int> subject = () => throw new MyException();

		async Task Act()
		{
			await That(subject).DoesNotThrow().Because(becauseTask);
		}

		await That(Act).ThrowsException().WithMessage($"*{because}*").AsWildcard();
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

		await That(Act).ThrowsException().WithMessage($"*{because}*").AsWildcard();
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

		await That(Act).ThrowsException().WithMessage($"*{expectedWithPrefix}*")
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

		await That(Act).ThrowsException().WithMessage($"*{because1}*").AsWildcard();
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

		await That(Act).ThrowsException()
			.WithMessage("""
			             Expected that subject
			             is False,
			             but it was True
			             """);
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

		await That(Act).ThrowsException().WithMessage($"*{because2}*").AsWildcard();
	}

	[Fact]
	public async Task WhenCombineWithAnd_ShouldApplyBecauseReasonOnlyOnPreviousConstraint()
	{
		string because = "we only apply it to previous constraints";
		bool subject = true;

		async Task Act()
		{
			await That(subject).IsTrue().Because(because)
				.And.IsFalse();
		}

		await That(Act).ThrowsException()
			.WithMessage("""
			             Expected that subject
			             is True, because we only apply it to previous constraints and is False,
			             but it was True
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

		await That(Act).ThrowsException().WithMessage($"*{because1}*{because2}*")
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

		await That(Act).ThrowsException()
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

		await That(Act).ThrowsException()
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

		Exception exception = await That(Act).ThrowsException()
			.WithMessage("*because*").AsWildcard();
		await That(exception.Message).DoesNotContain("because because");
	}
}
