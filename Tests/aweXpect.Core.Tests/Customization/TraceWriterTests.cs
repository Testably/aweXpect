using aweXpect.Chronology;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Customization;

namespace aweXpect.Core.Tests.Customization;

public class TraceWriterTests
{
	[Fact]
	public async Task FailTest_ShouldBeLogged()
	{
		TestTraceWriter traceWriter = new();
		using (traceWriter.Register())
		{
			try
			{
				Fail.Test("foo");
			}
			catch (Exception)
			{
				// Ignore failed exception
			}
		}

		await That(traceWriter.Exceptions).Contains(e => e is XunitException && e.Message == "foo");
	}

	[Fact]
	public async Task ForBooleanExpectations_ShouldTraceSuccessfulVerificationDetails()
	{
		bool subject = true;
		TestTraceWriter traceWriter = new();
		using (traceWriter.Register())
		{
			using (Customize.aweXpect.Settings().TestCancellation
				       .Set(TestCancellation.FromTimeout(TimeSpan.FromSeconds(7))))
			{
				await That(subject).IsTrue();
			}
		}

		await That(traceWriter.Messages).IsEqualTo([
			"Checking expectation for subject True with timeout of 0:07",
			"  Successfully verified that subject is True",
		]);
	}

	[Fact]
	public async Task ForFailedDelegateWithoutReturnValue_ShouldTraceSuccessfulVerificationDetails()
	{
		Action callback = () => throw new Exception("foo");
		TestTraceWriter traceWriter = new();
		using (traceWriter.Register())
		{
			await That(callback).Throws<Exception>().WithMessage("foo");
		}

		await That(traceWriter.Messages).HasCount(2);
		await That(traceWriter.Messages[0])
			.IsEqualTo("Checking expectation for callback delegate throwing Exception: foo after ")
			.AsPrefix();
		await That(traceWriter.Messages[1])
			.IsEqualTo("  Successfully verified that callback throws an exception with message equal to \"foo\"");
	}

	[Fact]
	public async Task ForFailedDelegateWithReturnValue_ShouldTraceSuccessfulVerificationDetails()
	{
		Func<int> callback = () => throw new Exception("foo");
		TestTraceWriter traceWriter = new();
		using (traceWriter.Register())
		{
			await That(callback).Throws<Exception>().WithMessage("foo");
		}

		await That(traceWriter.Messages).HasCount(2);
		await That(traceWriter.Messages[0])
			.IsEqualTo("Checking expectation for callback delegate returning int throwing Exception: foo after ")
			.AsPrefix();
		await That(traceWriter.Messages[1])
			.IsEqualTo("  Successfully verified that callback throws an exception with message equal to \"foo\"");
	}

	[Fact]
	public async Task ForSuccessfulDelegateWithoutReturnValue_ShouldTraceSuccessfulVerificationDetails()
	{
		Action callback = () => { };
		TestTraceWriter traceWriter = new();
		using (traceWriter.Register())
		{
			await That(callback).ExecutesIn().AtMost(500.Milliseconds());
		}

		await That(traceWriter.Messages).IsEqualTo([
			"Checking expectation for callback delegate returning in 0:*",
			"  Successfully verified that callback executes in at most 0:00.500",
		]).AsWildcard();
	}

	[Fact]
	public async Task ForSuccessfulDelegateWithReturnValue_ShouldTraceSuccessfulVerificationDetails()
	{
		Func<int> callback = () => 4;
		TestTraceWriter traceWriter = new();
		using (traceWriter.Register())
		{
			await That(callback).ExecutesIn().AtMost(500.Milliseconds());
		}

		await That(traceWriter.Messages).HasCount(2);
		await That(traceWriter.Messages[0]).IsEqualTo("Checking expectation for callback delegate returning int 4 in 0:00").AsPrefix();
		await That(traceWriter.Messages[1]).IsEqualTo("  Successfully verified that callback executes in at most 0:00.500");
	}

	[Fact]
	public async Task SkipTest_ShouldBeLogged()
	{
		TestTraceWriter traceWriter = new();
		using (traceWriter.Register())
		{
			try
			{
				Skip.Test("foo");
			}
			catch (Exception)
			{
				// Ignore failed exception
			}
		}

		await That(traceWriter.Exceptions).Contains(e
			=> e is SkipException && e.Message == "SKIPPED: foo (xunit v2 does not support skipping test)");
	}
}
