using aweXpect.Chronology;

namespace aweXpect.Core.Tests.Delegates;

public sealed partial class ThatDelegateTests
{
	public sealed class FaultedTaskTests
	{
		[Fact]
		public async Task DoesNotThrow_WhenTaskFaultsWithSeveralExceptions_ShouldListTheOtherExceptions()
		{
			Task sut = FaultWithSeveralExceptions();

			async Task Act()
				=> await That(sut).DoesNotThrow();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that sut
				             does not throw any exception,
				             but it did throw an InvalidOperationException:
				               A

				             Other exceptions:
				             [
				               ArgumentException: B,
				               NotSupportedException: C
				             ]
				             """);
		}

		[Fact]
		public async Task DoesNotThrow_WithValue_WhenTaskFaultsWithSeveralExceptions_ShouldListTheOtherExceptions()
		{
			Func<Task<int>> sut = FaultWithSeveralExceptions;

			async Task Act()
				=> await That(sut).DoesNotThrow();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that sut
				             does not throw any exception,
				             but it did throw an InvalidOperationException:
				               A

				             Other exceptions:
				             [
				               ArgumentException: B,
				               NotSupportedException: C
				             ]
				             """);
		}

		[Fact]
		public async Task Eventually_WhenTaskFaultsWithSeveralExceptions_ShouldListTheOtherExceptions()
		{
			Func<Task<int>> sut = FaultWithSeveralExceptions;

			async Task Act()
				=> await That(sut).Eventually().Within(50.Milliseconds()).IsEqualTo(1);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that sut
				             eventually is equal to 1 within 0:00.050,
				             but it did throw an InvalidOperationException:
				               A

				             Other exceptions:
				             [
				               ArgumentException: B,
				               NotSupportedException: C
				             ]
				             """);
		}

		[Fact]
		public async Task Throws_WhenTaskFaultsWithSeveralExceptions_ShouldCheckTheFirstException()
		{
			Task sut = FaultWithSeveralExceptions();

			async Task Act()
				=> await That(sut).Throws<InvalidOperationException>().WithMessage("A");

			await That(Act).DoesNotThrow()
				.Because("awaiting a faulted task throws its first exception");
		}

		[Fact]
		public async Task Throws_WhenTaskFaultsWithSeveralExceptions_ShouldListTheOtherExceptions()
		{
			Task sut = FaultWithSeveralExceptions();

			async Task Act()
				=> await That(sut).Throws<NotSupportedException>();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that sut
				             throws a NotSupportedException,
				             but it did throw an InvalidOperationException:
				               A

				             Other exceptions:
				             [
				               ArgumentException: B,
				               NotSupportedException: C
				             ]
				             """);
		}

		[Fact]
		public async Task WhenTaskSubjectFaultsWithSeveralExceptions_ShouldListTheOtherExceptions()
		{
			Task<int> sut = FaultWithSeveralExceptions();

			async Task Act()
				=> await That(sut).IsEqualTo(1);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that sut
				             is equal to 1,
				             but it did throw an InvalidOperationException:
				               A

				             Other exceptions:
				             [
				               ArgumentException: B,
				               NotSupportedException: C
				             ]
				             """);
		}

		[Fact]
		public async Task WhenValueTaskSubjectFaultsWithSeveralExceptions_ShouldListTheOtherExceptions()
		{
			ValueTask<int> sut = new(FaultWithSeveralExceptions());

			async Task Act()
				=> await That(sut).IsEqualTo(1);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that sut
				             is equal to 1,
				             but it did throw an InvalidOperationException:
				               A

				             Other exceptions:
				             [
				               ArgumentException: B,
				               NotSupportedException: C
				             ]
				             """);
		}

		private static Task<int> FaultWithSeveralExceptions()
		{
			TaskCompletionSource<int> source = new();
			source.SetException([
				new InvalidOperationException("A"),
				new ArgumentException("B"),
				new NotSupportedException("C"),
			]);
			return source.Task;
		}
	}
}
