using System.Diagnostics.CodeAnalysis;
using aweXpect.Core.EvaluationContext;

namespace aweXpect.Helpers;

internal class EvaluationContext
{
	internal static readonly IEvaluationContext None = new NoEvaluationContext();

	private sealed class NoEvaluationContext : IEvaluationContext
	{
		/// <inheritdoc cref="IEvaluationContext.Store{T}(string, T)" />
		public void Store<T>(string key, T value)
		{
			// Do nothing
		}

		/// <inheritdoc cref="IEvaluationContext.TryReceive{T}(string, out T)" />
		public bool TryReceive<T>(string key, [NotNullWhen(true)] out T? value)
		{
			value = default;
			return false;
		}
	}
}
