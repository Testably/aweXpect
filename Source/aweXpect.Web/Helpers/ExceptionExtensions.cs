using System;
using System.Text;
using System.Threading;
using aweXpect.Core.Extending;

namespace aweXpect.Helpers;

internal static class ExceptionExtensions
{
	/// <summary>
	///     Checks if the <paramref name="exception" /> signals the cancellation of the evaluation, which must abort it
	///     instead of failing the expectation.
	/// </summary>
	public static bool IsCancellationOf(this Exception exception, CancellationToken cancellationToken)
		=> exception is OperationCanceledException && cancellationToken.IsCancellationRequested;

	/// <summary>
	///     Appends that the <paramref name="thrower" /> did throw the <paramref name="exception" />, with its type and
	///     message, like aweXpect.Core does for a member that throws.
	/// </summary>
	public static void AppendDidThrow(this StringBuilder stringBuilder, string thrower, Exception exception,
		string? indentation)
	{
		stringBuilder.Append(thrower).Append(" did throw ")
			.Append(Formatter.Format(exception.GetType()).PrependAOrAn());
		string message = GetMessage(exception);
		if (!string.IsNullOrEmpty(message))
		{
			stringBuilder.Append(':').Append(Environment.NewLine).Append(message.Indent(indentation + "  "));
		}
	}

	/// <remarks>
	///     Reading the message of an exception of the caller can throw as well, which must not hide the failure.
	/// </remarks>
	private static string GetMessage(Exception exception)
	{
		try
		{
			return exception.Message;
		}
		catch (Exception thrownException)
		{
			return $"[Message of {Formatter.Format(exception.GetType())} did throw " +
			       $"{Formatter.Format(thrownException.GetType()).PrependAOrAn()}]";
		}
	}
}
