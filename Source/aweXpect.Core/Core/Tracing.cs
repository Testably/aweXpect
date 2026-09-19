using System;
using aweXpect.Customization;

namespace aweXpect.Core;

/// <summary>
///     Writes tracing information to the <see cref="ITraceWriter" /> registered via
///     <see cref="AwexpectCustomization.EnableTracing(ITraceWriter)" />.
/// </summary>
public static class Tracing
{
	/// <summary>
	///     Writes the <paramref name="exception" /> to the registered <see cref="ITraceWriter" /> and returns it,
	///     so that the caller can throw it.
	/// </summary>
	public static TException WriteException<TException>(TException exception)
		where TException : Exception
	{
		Customize.aweXpect.TraceWriter.Value?.WriteException(exception);
		return exception;
	}
}
