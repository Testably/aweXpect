using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
	public async Task ForDelegateReturningALazySequence_ShouldEnumerateItAsOftenAsWithoutTracing()
	{
		int enumerations = 0;
		Func<IEnumerable<int>> callback = () => Items();
		TestTraceWriter traceWriter = new();

		await That(callback).DoesNotThrow();
		int enumerationsWithoutTracing = enumerations;
		using (traceWriter.Register())
		{
			await That(callback).DoesNotThrow();
		}

		await That(enumerations).IsEqualTo(enumerationsWithoutTracing)
			.Because("tracing must not enumerate a sequence that the expectations do not enumerate");
		await That(traceWriter.Messages).IsEqualTo([
			"Checking expectation for callback delegate returning IEnumerable<int> in 0:*",
			"  Successfully verified that callback does not throw any exception",
		]).AsWildcard();

		IEnumerable<int> Items()
		{
			enumerations++;
			yield return 1;
		}
	}

	[Test]
	public async Task ForDelegateReturningALazySequenceAsObject_ShouldNotEnumerateIt()
	{
		int enumerations = 0;
		Func<object> callback = () => Items();
		TestTraceWriter traceWriter = new();
		using (traceWriter.Register())
		{
			await That(callback).DoesNotThrow();
		}

		await That(enumerations).IsEqualTo(0);
		await That(traceWriter.Messages[0])
			.IsEqualTo("Checking expectation for callback delegate returning object in 0:*").AsWildcard();

		IEnumerable<int> Items()
		{
			enumerations++;
			yield return 1;
		}
	}

	[Test]
	public async Task ForDelegateReturningALinqQuery_ShouldNotEnumerateIt()
	{
		int enumerations = 0;
		int[] source = [1, 2, 3,];
		Func<IEnumerable<int>> callback = () => source.Select(item =>
		{
			enumerations++;
			return item;
		});
		TestTraceWriter traceWriter = new();
		using (traceWriter.Register())
		{
			await That(callback).DoesNotThrow();
		}

		await That(enumerations).IsEqualTo(0);
		await That(traceWriter.Messages[0])
			.IsEqualTo("Checking expectation for callback delegate returning IEnumerable<int> in 0:*").AsWildcard();
	}

	[Test]
	public async Task ForDelegateReturningAnArray_ShouldTraceTheItems()
	{
		Func<int[]> callback = () => [1, 2, 3,];
		TestTraceWriter traceWriter = new();
		using (traceWriter.Register())
		{
			await That(callback).DoesNotThrow();
		}

		await That(traceWriter.Messages[0])
			.IsEqualTo("Checking expectation for callback delegate returning int[] [1, 2, 3] in 0:*").AsWildcard();
	}

	[Test]
	public async Task ForDelegateReturningADictionary_ShouldTraceTheItems()
	{
		Func<IReadOnlyDictionary<string, int>> callback = () => new Dictionary<string, int>
		{
			["foo"] = 1,
		};
		TestTraceWriter traceWriter = new();
		using (traceWriter.Register())
		{
			await That(callback).DoesNotThrow();
		}

		await That(traceWriter.Messages[0])
			.IsEqualTo(
				"Checking expectation for callback delegate returning IReadOnlyDictionary<string, int> {[\"foo\"] = 1} in 0:*")
			.AsWildcard();
	}

	[Test]
	public async Task ForDelegateReturningAList_ShouldTraceTheItems()
	{
		Func<IEnumerable<int>> callback = () => new List<int>
		{
			1,
			2,
			3,
		};
		TestTraceWriter traceWriter = new();
		using (traceWriter.Register())
		{
			await That(callback).DoesNotThrow();
		}

		await That(traceWriter.Messages[0])
			.IsEqualTo("Checking expectation for callback delegate returning IEnumerable<int> [1, 2, 3] in 0:*")
			.AsWildcard();
	}

	[Test]
	public async Task ForDelegateReturningANullSequence_ShouldTraceNull()
	{
		Func<IEnumerable<int>?> callback = () => null;
		TestTraceWriter traceWriter = new();
		using (traceWriter.Register())
		{
			await That(callback).DoesNotThrow();
		}

		await That(traceWriter.Messages[0])
			.IsEqualTo("Checking expectation for callback delegate returning IEnumerable<int> <null> in 0:*")
			.AsWildcard();
	}

	[Test]
	public async Task ForDelegateReturningAReadOnlyCollection_ShouldTraceTheItems()
	{
		Func<IEnumerable<int>> callback = () => new MyReadOnlyCollection();
		TestTraceWriter traceWriter = new();
		using (traceWriter.Register())
		{
			await That(callback).DoesNotThrow();
		}

		await That(traceWriter.Messages[0])
			.IsEqualTo("Checking expectation for callback delegate returning IEnumerable<int> [1, 2] in 0:*")
			.AsWildcard()
			.Because("a collection that only implements the read-only interface holds its items as well");
	}

	[Test]
	public async Task ForDelegateReturningASet_ShouldTraceTheItems()
	{
		Func<IEnumerable<int>> callback = () => new HashSet<int>
		{
			1,
		};
		TestTraceWriter traceWriter = new();
		using (traceWriter.Register())
		{
			await That(callback).DoesNotThrow();
		}

		await That(traceWriter.Messages[0])
			.IsEqualTo("Checking expectation for callback delegate returning IEnumerable<int> [1] in 0:*")
			.AsWildcard()
			.Because("a set only implements the generic collection interfaces");
	}

	[Test]
	public async Task ForDelegateReturningAString_ShouldTraceTheString()
	{
		Func<string> callback = () => "foo";
		TestTraceWriter traceWriter = new();
		using (traceWriter.Register())
		{
			await That(callback).DoesNotThrow();
		}

		await That(traceWriter.Messages[0])
			.IsEqualTo("Checking expectation for callback delegate returning string \"foo\" in 0:*").AsWildcard();
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

	[Test]
	public async Task WhenTheSubjectThrows_ShouldTraceThatItThrewAndFail()
	{
		Task<int> subject = Task.FromException<int>(new MyException("foo"));
		TestTraceWriter traceWriter = new();

		async Task Act()
		{
			using (traceWriter.Register())
			{
				await That(subject).IsEqualTo(1);
			}
		}

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is equal to 1,
			             but it did throw a TraceWriterTests.MyException:
			               foo
			             """);
		await That(traceWriter.Messages).IsEqualTo(["Checking expectation for subject threw an exception",]);
	}

	[Test]
	public async Task WhenTheTraceWriterThrows_ShouldKeepTheFailureOfAnExpectation()
	{
		async Task Act()
		{
			using (new ThrowingTraceWriter().Register())
			{
				await That(true).IsFalse();
			}
		}

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that true
			             is False,
			             but it was True
			             """);
	}

	[Test]
	public async Task WhenTheTraceWriterThrows_ShouldKeepTheFailureOfAThrowingSubject()
	{
		Task<int> subject = Task.FromException<int>(new MyException("foo"));

		async Task Act()
		{
			using (new ThrowingTraceWriter().Register())
			{
				await That(subject).IsEqualTo(1);
			}
		}

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is equal to 1,
			             but it did throw a TraceWriterTests.MyException:
			               foo
			             """);
	}

	[Test]
	public async Task WhenTheTraceWriterThrows_ShouldKeepTheReasonOfAnExplicitFailure()
	{
		void Act()
		{
			using (new ThrowingTraceWriter().Register())
			{
				Fail.Test("foo");
			}
		}

		await That(Act).Throws<FailException>().WithMessage("foo");
	}

	[Test]
	public async Task WhenTheTraceWriterThrows_ShouldMeetTheExpectation()
	{
		ThrowingTraceWriter traceWriter = new();

		async Task Act()
		{
			using (traceWriter.Register())
			{
				await That(true).IsTrue();
			}
		}

		await That(Act).DoesNotThrow();
		await That(traceWriter.WrittenMessages).IsEqualTo(2)
			.Because("the trace writer is still called for every message");
	}

	[Test]
	public async Task WhenTheTraceWriterThrows_WithAsyncDelegate_ShouldMeetTheExpectation()
	{
		Func<Task<int>> callback = () => Task.FromResult(4);

		async Task Act()
		{
			using (new ThrowingTraceWriter().Register())
			{
				await That(callback).DoesNotThrow();
			}
		}

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task WhenTheTraceWriterThrows_WithDelegate_ShouldMeetTheExpectation()
	{
		Func<int> callback = () => 4;

		async Task Act()
		{
			using (new ThrowingTraceWriter().Register())
			{
				await That(callback).DoesNotThrow();
			}
		}

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task WhenTheTraceWriterThrows_WithEventually_ShouldMeetTheExpectationInTheFirstAttempt()
	{
		int attempts = 0;
		Func<int> subject = () => ++attempts;

		async Task Act()
		{
			using (new ThrowingTraceWriter().Register())
			{
				await That(subject).Eventually().OnVirtualTime().Within(5.Seconds()).IsEqualTo(1);
			}
		}

		await That(Act).DoesNotThrow();
		await That(attempts).IsEqualTo(1);
	}

	[Test]
	public async Task WhenTheTraceWriterThrows_WithEventually_ShouldKeepTheFailureOfAThrowingSubject()
	{
		Func<int> subject = () => throw new MyException("foo");

		async Task Act()
		{
			using (new ThrowingTraceWriter().Register())
			{
				await That(subject).Eventually().OnVirtualTime().Within(50.Milliseconds()).IsEqualTo(1);
			}
		}

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             eventually is equal to 1 within 0:00.050,
			             but it did throw a TraceWriterTests.MyException:
			               foo
			             """);
	}

	[Test]
	public async Task WhenToStringOfTheSubjectThrows_ShouldMeetTheExpectation()
	{
		ThrowingToString subject = new();
		TestTraceWriter traceWriter = new();

		async Task Act()
		{
			using (traceWriter.Register())
			{
				await That(subject).IsNotNull();
			}
		}

		await That(Act).DoesNotThrow();
		await That(traceWriter.Messages).IsEqualTo([
			"Checking expectation for subject [ToString of TraceWriterTests.ThrowingToString did throw a TraceWriterTests.MyException: foo]",
			"  Successfully verified that subject is not null",
		]);
	}

	[Test]
	public async Task WhenToStringOfTheSubjectThrows_WithAsyncDelegate_ShouldMeetTheExpectation()
	{
		Func<Task<ThrowingToString>> callback = () => Task.FromResult(new ThrowingToString());
		TestTraceWriter traceWriter = new();

		async Task Act()
		{
			using (traceWriter.Register())
			{
				await That(callback).DoesNotThrow();
			}
		}

		await That(Act).DoesNotThrow();
		await That(traceWriter.Messages).HasCount(2);
	}

	[Test]
	public async Task WhenToStringOfTheSubjectThrows_WithDelegate_ShouldMeetTheExpectation()
	{
		Func<ThrowingToString> callback = () => new ThrowingToString();
		TestTraceWriter traceWriter = new();

		async Task Act()
		{
			using (traceWriter.Register())
			{
				await That(callback).DoesNotThrow();
			}
		}

		await That(Act).DoesNotThrow();
		await That(traceWriter.Messages).HasCount(2);
	}

	[Test]
	public async Task WhenToStringOfTheSubjectThrows_WithEventually_ShouldMeetTheExpectationInTheFirstAttempt()
	{
		int attempts = 0;
		Func<ThrowingToString> subject = () =>
		{
			attempts++;
			return new ThrowingToString();
		};
		TestTraceWriter traceWriter = new();

		async Task Act()
		{
			using (traceWriter.Register())
			{
				await That(subject).Eventually().OnVirtualTime().Within(5.Seconds()).IsNotNull();
			}
		}

		await That(Act).DoesNotThrow();
		await That(attempts).IsEqualTo(1);
		await That(traceWriter.Messages).IsEqualTo([
			"Checking expectation for subject [ToString of TraceWriterTests.ThrowingToString did throw a TraceWriterTests.MyException: foo]",
			"  Successfully verified that subject eventually is not null",
		]);
	}

	private sealed class MyException(string message) : Exception(message);

	private sealed class MyReadOnlyCollection : IReadOnlyCollection<int>
	{
		public int Count => 2;

		public IEnumerator<int> GetEnumerator()
		{
			yield return 1;
			yield return 2;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	private sealed class ThrowingToString
	{
		public override string ToString()
			=> throw new MyException("foo");
	}

	private sealed class ThrowingTraceWriter : ITraceWriter
	{
		public int WrittenMessages { get; private set; }

		public void WriteMessage(string message)
		{
			WrittenMessages++;
			throw new MyException("the trace writer is broken");
		}

		public void WriteException(Exception exception)
			=> throw new MyException("the trace writer is broken");

		public CustomizationLifetime Register() => Customize.aweXpect.EnableTracing(this);
	}
}
