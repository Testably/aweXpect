using System;
using System.Threading.Tasks;
using aweXpect.Core;

namespace aweXpect.Reflection.Aot;

internal static class Program
{
	/// <summary>
	///     Whether the program runs as the executable published with Native AOT instead of from the compiled output.
	/// </summary>
	/// <remarks>
	///     A published executable has no dependency file.
	/// </remarks>
	public static bool IsPublished { get; } = AppContext.GetData("APP_CONTEXT_DEPS_FILES") is null;

	/// <summary>
	///     Runs every check and returns a non-zero exit code when one of them fails.
	/// </summary>
	/// <remarks>
	///     The same program runs from the compiled output, which reflects like an application that is not trimmed, and
	///     published with Native AOT, where the reflection fallback is switched off. A check therefore states the
	///     result of the compiled output and accepts instead only a failure that names the switch.
	/// </remarks>
	public static async Task<int> Main()
	{
		Console.WriteLine(
			$"Published: {IsPublished}, reflection fallback supported: {ReflectionFallback.IsSupported}");
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
