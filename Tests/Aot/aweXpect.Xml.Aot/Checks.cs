using System;
using System.Threading.Tasks;
using static aweXpect.Expect;

namespace aweXpect.Xml.Aot;

internal static class Checks
{
	public static readonly Check[] All =
	[
		new("a dummy expectation passes",
			() => ShouldPass(async () => await That(true).IsTrue())),
	];

	private static async Task<string?> ShouldPass(Func<Task> act)
	{
		try
		{
			await act();
			return null;
		}
		catch (Exception exception)
		{
			return $"threw {exception.GetType().FullName}: {exception.Message}";
		}
	}
}
