using aweXpect.Chronology;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Customization;

namespace aweXpect.Core.Tests.Customization;

public class TraceWriterTests
{
	[Test]
	public async Task EnableTracing_DisposedOutOfOrder_ShouldKeepTheRemainingTraceWriter()
	{
		AwexpectCustomization customization = new();
		TestTraceWriter firstTraceWriter = new();
		TestTraceWriter secondTraceWriter = new();
		CustomizationLifetime firstLifetime = customization.EnableTracing(firstTraceWriter);
		CustomizationLifetime secondLifetime = customization.EnableTracing(secondTraceWriter);

		firstLifetime.Dispose();
		ITraceWriter? traceWriterAfterFirstDispose = customization.TraceWriter;
		secondLifetime.Dispose();
		ITraceWriter? traceWriterAfterSecondDispose = customization.TraceWriter;

		await That(traceWriterAfterFirstDispose).IsSameAs(secondTraceWriter)
			.Because("disposing an earlier lifetime must not disable a trace writer that was enabled afterwards");
		await That(traceWriterAfterSecondDispose).IsNull()
			.Because("disposing the last lifetime must not restore a trace writer whose lifetime was already disposed");
	}

	[Test]
	public async Task EnableTracing_DoubleDispose_ShouldNotDisableLaterTraceWriter()
	{
		TestTraceWriter firstTraceWriter = new();
		TestTraceWriter secondTraceWriter = new();
		IDisposable firstLifetime = firstTraceWriter.Register();
		firstLifetime.Dispose();
		using (secondTraceWriter.Register())
		{
			firstLifetime.Dispose();
			await That(true).IsTrue();
		}

		await That(secondTraceWriter.Messages).IsNotEmpty()
			.Because("disposing a lifetime a second time must not disable a trace writer that was enabled afterwards");
	}

	[Test]
	public async Task EnableTracing_Global_DoubleDispose_ShouldNotDisableLaterTraceWriter()
	{
		AwexpectCustomization customization = new();
		TestTraceWriter firstTraceWriter = new();
		TestTraceWriter secondTraceWriter = new();
		CustomizationLifetime firstLifetime = customization.Global.EnableTracing(firstTraceWriter);
		firstLifetime.Dispose();
		ITraceWriter? traceWriter;
		using (customization.Global.EnableTracing(secondTraceWriter))
		{
			firstLifetime.Dispose();
			traceWriter = customization.TraceWriter;
		}

		await That(traceWriter).IsSameAs(secondTraceWriter)
			.Because("disposing a lifetime a second time must not disable a trace writer that was enabled afterwards");
		await That(customization.TraceWriter).IsNull()
			.Because("disposing the global lifetime disables the global trace writer again");
	}

	[Test]
	public async Task EnableTracing_Global_ShouldBeUsedWhenTheCurrentFlowHasNoTraceWriter()
	{
		AwexpectCustomization customization = new();
		TestTraceWriter globalTraceWriter = new();
		TestTraceWriter flowTraceWriter = new();
		using CustomizationLifetime globalLifetime =
			await Task.Run(() => customization.Global.EnableTracing(globalTraceWriter));
		ITraceWriter? traceWriterWithoutFlowTraceWriter = customization.TraceWriter;
		ITraceWriter? traceWriterWithFlowTraceWriter;
		using (customization.EnableTracing(flowTraceWriter))
		{
			traceWriterWithFlowTraceWriter = customization.TraceWriter;
		}

		await That(traceWriterWithoutFlowTraceWriter).IsSameAs(globalTraceWriter)
			.Because("the global trace writer applies to all flows");
		await That(traceWriterWithFlowTraceWriter).IsSameAs(flowTraceWriter)
			.Because("a trace writer enabled in the current flow takes precedence over the global one");
	}

	[Test]
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

		await That(traceWriter.Exceptions).Contains(e => e is FailException && e.Message == "foo");
	}

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
	public async Task ForSuccessfulDelegateWithReturnValue_ShouldTraceSuccessfulVerificationDetails()
	{
		Func<int> callback = () => 4;
		TestTraceWriter traceWriter = new();
		using (traceWriter.Register())
		{
			await That(callback).ExecutesIn().AtMost(500.Milliseconds());
		}

		await That(traceWriter.Messages).HasCount(2);
		await That(traceWriter.Messages[0]).IsEqualTo("Checking expectation for callback delegate returning int 4 in 0:*")
			.AsWildcard()
			.Because("the measured duration is wall-clock time, which a busy machine can stretch beyond a second");
		await That(traceWriter.Messages[1]).IsEqualTo("  Successfully verified that callback executes in at most 0:00.500");
	}

	[Test]
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
			=> e is SkipTestException && e.Message == "foo");
	}
}
