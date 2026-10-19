namespace aweXpect.Reflection.Tests.TestHelpers.Types;

public class ClassWithAsyncMembers
{
#pragma warning disable CA1822 // Mark members as static
	public async Task AsyncMethod() => await Task.CompletedTask;

	public async Task<int> AsyncMethodWithResult()
	{
		await Task.CompletedTask;
		return 42;
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators
	public async void AsyncVoidMethod()
	{
	}
#pragma warning restore CS1998

	public Task NonAsyncTaskMethod() => Task.CompletedTask;

	public void RegularMethod() { }
#pragma warning restore CA1822
}
