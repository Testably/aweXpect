using System;
using System.Text;
using System.Threading.Tasks;
using aweXpect.Core.Constraints;

namespace aweXpect.Core.Helpers;

/// <remarks>
///     The message is only created once it is written, and the delegate that writes it is created once per reason
///     instead of once per evaluation.
/// </remarks>
internal sealed class BecauseReason(string reason) : IBecauseReason
{
	private Action<StringBuilder>? _appendMessage;
	private string? _message;

	private string Message => _message ??= CreateMessage(reason);

	private static string CreateMessage(string reason)
	{
		const string prefix = "because";
		string message = reason.Trim();

		return !message.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
			? $", {prefix} {message}"
			: $", {message}";
	}

	public override string ToString()
		=> Message;

	public ValueTask<ConstraintResult>
		ApplyTo(ConstraintResult result)
		=> new(result.AppendExpectationText(_appendMessage ??= stringBuilder => stringBuilder.Append(Message)));
}
