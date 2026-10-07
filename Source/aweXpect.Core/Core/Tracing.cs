using System;
using System.Text;
using aweXpect.Core.Constraints;
using aweXpect.Customization;

namespace aweXpect.Core;

/// <summary>
///     Writes tracing information to the <see cref="ITraceWriter" /> registered via
///     <see cref="AwexpectCustomization.EnableTracing(ITraceWriter)" />.
/// </summary>
/// <remarks>
///     Tracing only observes: an exception while a trace is built or written is ignored, so that it neither changes
///     the outcome of an expectation nor replaces the exception that is thrown.
/// </remarks>
public static class Tracing
{
	/// <summary>
	///     Writes the <paramref name="exception" /> to the registered <see cref="ITraceWriter" /> and returns it,
	///     so that the caller can throw it.
	/// </summary>
	public static TException WriteException<TException>(TException exception)
		where TException : Exception
	{
		try
		{
			Customize.aweXpect.TraceWriter?.WriteException(exception);
		}
		catch (Exception)
		{
			// The caller throws the exception, which a failing trace writer must not replace.
		}

		return exception;
	}

	/// <summary>
	///     Writes that the expectations on the <paramref name="subject" /> are checked for the
	///     <paramref name="value" />.
	/// </summary>
	internal static void WriteSubject<TValue>(string subject, TValue value, TimeSpan? timeout = null)
	{
		if (Customize.aweXpect.TraceWriter is not { } traceWriter)
		{
			return;
		}

		try
		{
			string formattedValue = ToString(value);
			traceWriter.WriteMessage(timeout is null
				? $"Checking expectation for {subject} {formattedValue}"
				: $"Checking expectation for {subject} {formattedValue} with timeout of {Formatter.Format(timeout)}");
		}
		catch (Exception)
		{
			// The subject is checked regardless of whether it could be traced.
		}
	}

	/// <summary>
	///     Writes that evaluating the <paramref name="subject" /> threw an exception.
	/// </summary>
	internal static void WriteThrowingSubject(string subject)
	{
		if (Customize.aweXpect.TraceWriter is not { } traceWriter)
		{
			return;
		}

		try
		{
			traceWriter.WriteMessage($"Checking expectation for {subject} threw an exception");
		}
		catch (Exception)
		{
			// The exception of the subject decides the outcome, not the one of the trace writer.
		}
	}

	/// <summary>
	///     Writes that the expectations on the <paramref name="subject" /> were met with the <paramref name="result" />.
	/// </summary>
	internal static void WriteSuccess(string subject, ConstraintResult result)
	{
		if (Customize.aweXpect.TraceWriter is not { } traceWriter)
		{
			return;
		}

		try
		{
			StringBuilder sb = new();
			sb.Append("  Successfully verified that ");
			sb.Append(result.TryGetValue(out IDescribableSubject? describableSubject)
				? describableSubject.GetDescription()
				: subject);
			sb.Append(' ');
			result.AppendExpectation(sb);
			traceWriter.WriteMessage(sb.ToString());
		}
		catch (Exception)
		{
			// The expectations are met regardless of whether that could be traced.
		}
	}

	/// <remarks>
	///     The value is not formatted, because enumerating a collection or reading the members of an object could
	///     change the subject that is checked afterwards.
	/// </remarks>
	private static string ToString<TValue>(TValue value)
	{
		try
		{
			return $"{value}";
		}
		catch (Exception exception)
		{
			return ValueFormatters.FormatThrownException(
				$"ToString of {Formatter.Format(value!.GetType())}", exception);
		}
	}
}
