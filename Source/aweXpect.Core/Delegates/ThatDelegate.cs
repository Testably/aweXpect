using System;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Helpers;
using aweXpect.Core.Sources;
using aweXpect.Options;

namespace aweXpect.Delegates;

/// <summary>
///     Expectations on delegate values.
/// </summary>
public abstract partial class ThatDelegate(ExpectationBuilder expectationBuilder)
{
	/// <summary>
	///     The expectation builder.
	/// </summary>
	public ExpectationBuilder ExpectationBuilder { get; } = expectationBuilder;

	internal static string FormatForMessage(Exception? exception, string? indentation, string relation = "")
	{
		if (exception is null)
		{
			return "<null>";
		}

		string message = (relation + Formatter.Format(exception.GetType())).PrependAOrAn();
		if (!string.IsNullOrEmpty(exception.Message))
		{
			message += ":" + Environment.NewLine + exception.Message.Indent(indentation + "  ");
		}

		return message;
	}

	/// <summary>
	///     Appends the result for a delegate that was <see langword="null" /> or returned a <see langword="null" /> task,
	///     or for a <see langword="null" /> task subject.
	/// </summary>
	internal static void AppendNullResult(StringBuilder stringBuilder, string it, DelegateValue? actual)
	{
		if (actual?.IsNullTask == true)
		{
			stringBuilder.Append(it).Append(" returned <null> instead of a task");
		}
		else if (actual?.IsNullTaskSubject == true)
		{
			stringBuilder.Append(it).Append(" was a <null> task");
		}
		else
		{
			stringBuilder.ItWasNull(it);
		}
	}

	private static void AppendThrowsExpectation(StringBuilder stringBuilder, ThrowsOption options,
		Type exceptionType, bool exactly)
	{
		if (!options.DoCheckThrow)
		{
			stringBuilder.Append(options.IsNegated ? "throws an exception" : "does not throw any exception");
		}
		else if (!exactly && exceptionType == typeof(Exception))
		{
			stringBuilder.Append(options.IsNegated ? "does not throw any exception" : "throws an exception");
		}
		else
		{
			stringBuilder.Append(options.IsNegated ? "does not throw " : "throws ")
				.Append(exactly ? "exactly " : "")
				.Append(Formatter.Format(exceptionType).PrependAOrAn());
		}

		if (options.ExecutionTimeOptions is not null)
		{
			stringBuilder.Append(' ');
			options.ExecutionTimeOptions.AppendTo(stringBuilder, "in ");
		}
	}

	/// <summary>
	///     Options on expectations if a delegate throws.
	/// </summary>
	public class ThrowsOption
	{
		/// <summary>
		///     Flag indicating if the delegate is expected to throw an exception.
		/// </summary>
		/// <remarks>
		///     If set to <see langword="false" />, the delegate must not throw any exception.
		/// </remarks>
		public bool DoCheckThrow { get; set; } = true;

		/// <summary>
		///     Options on the execution time to allow specifying a timeout.
		/// </summary>
		public ExecutionTimeOptions? ExecutionTimeOptions { get; set; }

		/// <summary>
		///     Flag indicating if the whole expectation on the thrown exception is negated.
		/// </summary>
		internal bool IsNegated { get; set; }

		/// <summary>
		///     Flag indicating if a duration was already specified with <c>Within(…)</c>, even an infinite one.
		/// </summary>
		internal bool IsWithinSpecified { get; set; }
	}
}
