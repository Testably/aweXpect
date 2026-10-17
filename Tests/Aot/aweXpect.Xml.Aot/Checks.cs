using System;
using System.Threading.Tasks;
using static aweXpect.Expect;

namespace aweXpect.Xml.Aot;

internal static class Checks
{
	public static readonly Check[] All =
	[
		new("a valid XML string passes",
			() => ShouldPass(async () => await That("<a><b c=\"1\"/></a>").IsValidXml())),
		new("an invalid XML string fails and names the parse error",
			() => ShouldFail(async () => await That("<a><b>").IsValidXml(), "is valid XML", "could not be parsed")),
		new("an XML string with a DTD fails",
			() => ShouldFail(async () => await That("<!DOCTYPE a [<!ENTITY e \"x\">]><a>&e;</a>").IsValidXml(),
				"is valid XML", "contained a document type definition (DTD)")),
		new("the root element of a valid XML string can be verified",
			() => ShouldPass(async () => await That("<a><b c=\"1\"/></a>").IsValidXml()
				.Which(r => r.Satisfies(e => e!.Name.LocalName == "a")))),
		new("a failing expectation on the root element fails",
			() => ShouldFail(async () => await That("<a/>").IsValidXml()
				.Which(r => r.Satisfies(e => e!.Name.LocalName == "b")), "is valid XML which satisfies")),
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

	/// <summary>
	///     Expects the failure exception whose message contains every part.
	/// </summary>
	private static async Task<string?> ShouldFail(Func<Task> act, params string[] parts)
	{
		try
		{
			await act();
		}
		catch (FailException exception)
		{
			string? missing = Array.Find(parts, part => !exception.Message.Contains(part, StringComparison.Ordinal));
			return missing is null ? null : $"message lacks \"{missing}\": {exception.Message}";
		}
		catch (Exception exception)
		{
			return $"threw {exception.GetType().FullName} instead of {typeof(FailException).FullName}: {exception.Message}";
		}

		return "did not throw";
	}
}
