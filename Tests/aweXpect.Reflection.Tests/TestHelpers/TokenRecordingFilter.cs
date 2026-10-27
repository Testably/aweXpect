using System.Collections.Generic;
using System.Threading;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.TestHelpers;

/// <summary>
///     A filter that applies to every entity and records the tokens it receives.
/// </summary>
public sealed class TokenRecordingFilter<TEntity>(Action? onApplies = null) : IFilter<TEntity>
{
	public List<CancellationToken> Tokens { get; } = [];

	public ValueTask<bool> Applies(TEntity value, CancellationToken cancellationToken)
	{
		Tokens.Add(cancellationToken);
		onApplies?.Invoke();
		return new ValueTask<bool>(true);
	}

	public string Describes(string text) => text;
}
