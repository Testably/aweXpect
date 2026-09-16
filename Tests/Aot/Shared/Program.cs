using System;
using System.Threading.Tasks;

namespace aweXpect.Aot;

internal static class Program
{
	/// <summary>
	///     Runs every check and returns a non-zero exit code when one of them fails.
	/// </summary>
	/// <remarks>
	///     The same program runs from the compiled output and published with Native AOT, so a check states what has
	///     to hold in both: the result the compiled output produces, or a failure whose message names the fix. The
	///     AOT feature switches apply to both runs, so only the missing assembly file tells the published one apart.
	/// </remarks>
	public static async Task<int> Main()
	{
		bool isPublished = typeof(Program).Assembly.Location.Length == 0;
		Console.WriteLine($"Framework: {Framework.Name}, published: {isPublished}");
		int failures = 0;
		foreach (Check check in Checks.All)
		{
			string? failure = await check.Run();
			Console.WriteLine(failure is null ? $"PASS {check.Name}" : $"FAIL {check.Name}: {failure}");
			if (failure is not null)
			{
				failures++;
			}
		}

		Console.WriteLine($"{Checks.All.Length - failures} of {Checks.All.Length} checks passed");
		return failures == 0 ? 0 : 1;
	}
}

internal sealed class Check(string name, Func<Task<string?>> run)
{
	public string Name { get; } = name;

	/// <summary>
	///     Returns <see langword="null" /> when the check holds, otherwise a description of what went wrong.
	/// </summary>
	public Task<string?> Run() => run();
}
