using System;
using System.Threading.Tasks;
using aweXpect.Core.Constraints;

namespace aweXpect.Core.Helpers;

internal readonly struct BecauseReason(string reason) : IBecauseReason
{
	private readonly Lazy<string> _message = new(() => CreateMessage(reason));

	private static string CreateMessage(string reason)
	{
		const string prefix = "because";
		string message = reason.Trim();

		return !message.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
			? $", {prefix} {message}"
			: $", {message}";
	}

	public override string ToString()
		=> _message.Value;
	
	public ValueTask<ConstraintResult>
	ApplyTo(ConstraintResult result)
	{
		string message = _message.Value;
		return new ValueTask<ConstraintResult>(result.AppendExpectationText(e => e.Append(message)));
	}
}
