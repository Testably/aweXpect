global using static Snippets.Prelude;

namespace Snippets;

internal static class Prelude
{
	public static Task task = Task.CompletedTask;
	public static SystemUnderTest sut = new();
	public static RetryPolicy retryPolicy = new();
	public static Action alwaysFailing = () => throw new InvalidOperationException();

	public static Task DoAsync() => Task.CompletedTask;

	public class SystemUnderTest
	{
		public int MyProp { get; set; }
		public string? Name { get; set; }
		public Task<int> GetCountAsync() => Task.FromResult(MyProp);
	}

	public class RetryPolicy
	{
		public void Execute(Action action) => action();
	}
}
