global using static Snippets.Prelude;

namespace Snippets;

internal static class Prelude
{
	public static RetryPolicy retryPolicy = new();
	public static Action alwaysFailing = () => throw new InvalidOperationException();

	public class RetryPolicy
	{
		public void Execute(Action action) => action();
	}
}
