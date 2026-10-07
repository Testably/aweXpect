using System.Collections;
using System.Collections.Generic;
using aweXpect.Core.Internal;
using aweXpect.Core.Tests.TestHelpers;
#if NET8_0_OR_GREATER
using System.Threading;
#endif

namespace aweXpect.Core.Tests.Core.EvaluationContext;

public class MaterializedSubjectExceptionTests
{
#if NET8_0_OR_GREATER
	[Test]
	public async Task Async_WhenCurrentThrows_AndExpectationIsNegated_ShouldFailWithTheExceptionAsInnerException()
	{
		InvalidOperationException exception = new("the item is broken");
		IAsyncEnumerable<int> subject = new BrokenAsyncEnumerable(null, exception);

		async Task Act()
			=> await That(subject).DoesNotComplyWith(it => it.Contains(1));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             does not contain an item equal to 1,
			             but it did throw an InvalidOperationException:
			               the item is broken
			             """).And
			.Whose(e => e.InnerException, i => i.IsSameAs(exception));
	}
#endif

#if NET8_0_OR_GREATER
	[Test]
	public async Task Async_WhenCurrentThrows_ShouldFailWithTheExceptionAsInnerException()
	{
		InvalidOperationException exception = new("the item is broken");
		IAsyncEnumerable<int> subject = new BrokenAsyncEnumerable(null, exception);

		async Task Act()
			=> await That(subject).Contains(1);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             contains an item equal to 1 at least once,
			             but it did throw an InvalidOperationException:
			               the item is broken
			             """).And
			.Whose(e => e.InnerException, i => i.IsSameAs(exception));
	}
#endif

#if NET8_0_OR_GREATER
	[Test]
	public async Task Async_WhenGetAsyncEnumeratorThrows_AndExpectationIsNegated_ShouldFailWithTheExceptionAsInnerException()
	{
		InvalidOperationException exception = new("the source is broken");
		IAsyncEnumerable<int> subject = new BrokenAsyncEnumerable(exception, null);

		async Task Act()
			=> await That(subject).DoesNotComplyWith(it => it.Contains(1));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             does not contain an item equal to 1,
			             but it did throw an InvalidOperationException:
			               the source is broken
			             """).And
			.Whose(e => e.InnerException, i => i.IsSameAs(exception));
	}
#endif

#if NET8_0_OR_GREATER
	[Test]
	public async Task Async_WhenGetAsyncEnumeratorThrows_InThatAll_ShouldFailWithTheException()
	{
		InvalidOperationException exception = new("the source is broken");
		IAsyncEnumerable<int> subject = new BrokenAsyncEnumerable(exception, null);

		async Task Act()
			=> await ThatAll(
				That(subject).Contains(1),
				That(true).IsTrue());

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected all of the following to succeed:
			              [01] Expected that subject contains an item equal to 1 at least once
			              [02] Expected that true is True
			             but
			              [01] it did throw an InvalidOperationException:
			                     the source is broken
			             """);
	}
#endif

#if NET8_0_OR_GREATER
	[Test]
	public async Task Async_WhenGetAsyncEnumeratorThrows_InThatAny_ShouldFailWithTheException()
	{
		InvalidOperationException exception = new("the source is broken");
		IAsyncEnumerable<int> subject = new BrokenAsyncEnumerable(exception, null);

		async Task Act()
			=> await ThatAny(
				That(subject).Contains(1),
				That(true).IsFalse());

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected any of the following to succeed:
			              [01] Expected that subject contains an item equal to 1 at least once
			              [02] Expected that true is False
			             but
			              [01] it did throw an InvalidOperationException:
			                     the source is broken
			              [02] it was True
			             """);
	}
#endif

#if NET8_0_OR_GREATER
	[Test]
	public async Task Async_WhenGetAsyncEnumeratorThrows_ShouldFailWithTheExceptionAsInnerException()
	{
		InvalidOperationException exception = new("the source is broken");
		IAsyncEnumerable<int> subject = new BrokenAsyncEnumerable(exception, null);

		async Task Act()
			=> await That(subject).Contains(1);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             contains an item equal to 1 at least once,
			             but it did throw an InvalidOperationException:
			               the source is broken
			             """).And
			.Whose(e => e.InnerException, i => i.IsSameAs(exception));
	}
#endif

	[Test]
	public async Task Untyped_WhenCurrentThrows_ShouldFailWithTheExceptionAsInnerException()
	{
		InvalidOperationException exception = new("the item is broken");
		IEnumerable subject = new DisposeTrackingEnumerable(null, 1, 2)
		{
			CurrentException = exception,
		};

		async Task Act()
			=> await That(subject).Contains(1);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             contains an item equal to 1 at least once,
			             but it did throw an InvalidOperationException:
			               the item is broken
			             """).And
			.Whose(e => e.InnerException, i => i.IsSameAs(exception));
	}

	[Test]
	public async Task Untyped_WhenGetEnumeratorThrows_ShouldFailWithTheExceptionAsInnerException()
	{
		InvalidOperationException exception = new("the source is broken");
		IEnumerable subject = new DisposeTrackingEnumerable(null, 1, 2)
		{
			GetEnumeratorException = exception,
		};

		async Task Act()
			=> await That(subject).Contains(1);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             contains an item equal to 1 at least once,
			             but it did throw an InvalidOperationException:
			               the source is broken
			             """).And
			.Whose(e => e.InnerException, i => i.IsSameAs(exception));
	}

	[Test]
	public async Task WhenCurrentThrows_AndExpectationIsNegated_ShouldFailWithTheExceptionAsInnerException()
	{
		InvalidOperationException exception = new("the item is broken");
		IEnumerable<int> subject = new DisposeTrackingEnumerable(null, 1, 2)
		{
			CurrentException = exception,
		};

		async Task Act()
			=> await That(subject).DoesNotComplyWith(it => it.Contains(1));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             does not contain an item equal to 1,
			             but it did throw an InvalidOperationException:
			               the item is broken
			             """).And
			.Whose(e => e.InnerException, i => i.IsSameAs(exception));
	}

	[Test]
	public async Task WhenCurrentThrows_ShouldFailWithTheExceptionAsInnerException()
	{
		InvalidOperationException exception = new("the item is broken");
		IEnumerable<int> subject = new DisposeTrackingEnumerable(null, 1, 2)
		{
			CurrentException = exception,
		};

		async Task Act()
			=> await That(subject).Contains(1);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             contains an item equal to 1 at least once,
			             but it did throw an InvalidOperationException:
			               the item is broken
			             """).And
			.Whose(e => e.InnerException, i => i.IsSameAs(exception));
	}

	[Test]
	public async Task WhenGetEnumeratorThrows_AndExpectationIsNegated_ShouldFailWithTheExceptionAsInnerException()
	{
		InvalidOperationException exception = new("the source is broken");
		IEnumerable<int> subject = new DisposeTrackingEnumerable(null, 1, 2)
		{
			GetEnumeratorException = exception,
		};

		async Task Act()
			=> await That(subject).DoesNotComplyWith(it => it.Contains(1));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             does not contain an item equal to 1,
			             but it did throw an InvalidOperationException:
			               the source is broken
			             """).And
			.Whose(e => e.InnerException, i => i.IsSameAs(exception));
	}

	[Test]
	public async Task WhenGetEnumeratorThrows_AndExpectationIsRepeated_ShouldAskTheSourceAgainForEveryCheck()
	{
		VirtualTimeSystem time = new();
		InvalidOperationException exception = new("the source is broken");
		DisposeTrackingEnumerable source = new(null, 1, 2)
		{
			GetEnumeratorException = exception,
		};
		IEnumerable<int> subject = source;

		async Task Act()
			=> await That(subject).CompliesWith(it => it.Contains(1))
				.Within(TimeSpan.FromMilliseconds(30)).CheckEvery(TimeSpan.FromMilliseconds(10)).WithTimeSystem(time);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             contains an item equal to 1 at least once within 0:00.030,
			             but it did throw an InvalidOperationException:
			               the source is broken
			             """).And
			.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		await That(source.GetEnumeratorCount).IsEqualTo(4)
			.Because("the checks are made at once and then every 10 ms until the timeout");
	}

	[Test]
	public async Task WhenGetEnumeratorThrows_InThatAll_ShouldFailWithTheException()
	{
		InvalidOperationException exception = new("the source is broken");
		IEnumerable<int> subject = new DisposeTrackingEnumerable(null, 1, 2)
		{
			GetEnumeratorException = exception,
		};

		async Task Act()
			=> await ThatAll(
				That(subject).Contains(1),
				That(true).IsTrue());

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected all of the following to succeed:
			              [01] Expected that subject contains an item equal to 1 at least once
			              [02] Expected that true is True
			             but
			              [01] it did throw an InvalidOperationException:
			                     the source is broken
			             """);
	}

	[Test]
	public async Task WhenGetEnumeratorThrows_InThatAny_ShouldFailWithTheException()
	{
		InvalidOperationException exception = new("the source is broken");
		IEnumerable<int> subject = new DisposeTrackingEnumerable(null, 1, 2)
		{
			GetEnumeratorException = exception,
		};

		async Task Act()
			=> await ThatAny(
				That(subject).Contains(1),
				That(true).IsFalse());

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected any of the following to succeed:
			              [01] Expected that subject contains an item equal to 1 at least once
			              [02] Expected that true is False
			             but
			              [01] it did throw an InvalidOperationException:
			                     the source is broken
			              [02] it was True
			             """);
	}

	[Test]
	public async Task WhenGetEnumeratorThrows_ShouldFailWithTheExceptionAsInnerException()
	{
		InvalidOperationException exception = new("the source is broken");
		DisposeTrackingEnumerable source = new(null, 1, 2)
		{
			GetEnumeratorException = exception,
		};
		IEnumerable<int> subject = source;

		async Task Act()
			=> await That(subject).Contains(1);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             contains an item equal to 1 at least once,
			             but it did throw an InvalidOperationException:
			               the source is broken
			             """).And
			.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		await That(source.GetEnumeratorCount).IsEqualTo(1)
			.Because("the failure message must not ask the source for its enumerator again");
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Throws the <paramref name="getAsyncEnumeratorException" /> when its enumerator is requested, or the
	///     <paramref name="currentException" /> when its first item is read.
	/// </summary>
	private sealed class BrokenAsyncEnumerable(Exception? getAsyncEnumeratorException, Exception? currentException)
		: IAsyncEnumerable<int>, IAsyncEnumerator<int>
	{
		public IAsyncEnumerator<int> GetAsyncEnumerator(CancellationToken cancellationToken = default)
			=> getAsyncEnumeratorException is null ? this : throw getAsyncEnumeratorException;

		public int Current => currentException is null ? 1 : throw currentException;

		public ValueTask<bool> MoveNextAsync() => new(true);

		public ValueTask DisposeAsync() => default;
	}
#endif
}
