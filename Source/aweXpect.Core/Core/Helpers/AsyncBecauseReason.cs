using System;
using System.Threading.Tasks;
using aweXpect.Core.Constraints;

namespace aweXpect.Core.Helpers;

internal struct AsyncBecauseReason(Task<string?> reason) : IBecauseReason
{
	private string? _message;

	private static string CreateMessage(string reason)
	{
		const string prefix = "because";
		string message = reason.Trim();

		return !message.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
			? $", {prefix} {message}"
			: $", {message}";
	}

#if NET8_0_OR_GREATER
	public async ValueTask<ConstraintResult>
#else
	public async Task<ConstraintResult>
#endif
		ApplyTo(ConstraintResult result)
	{
		if (_message is null)
		{
			string? resolvedReason = await reason.ConfigureAwait(false);
			if (string.IsNullOrEmpty(resolvedReason))
			{
				return result;
			}

			_message = CreateMessage(resolvedReason);
		}

		string message = _message;
		return result.AppendExpectationText(e => e.Append(message));
	}
}
