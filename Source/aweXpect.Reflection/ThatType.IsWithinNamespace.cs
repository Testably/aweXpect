using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatType
{
	[GenerateExpectation("Is{Not}WithinNamespace", "is {not} within namespace {expected}",
		Result = nameof(DescribeNamespace),
		Summary = """
		          Verifies that the <see cref="Type" /> is within the <paramref name="expected" /> namespace
		          (including sub-namespaces).
		          """,
		NegatedSummary = """
		                 Verifies that the <see cref="Type" /> is not within the <paramref name="expected" /> namespace
		                 (including sub-namespaces).
		                 """)]
	private static bool IsWithinNamespaceType(Type type, string expected) => type.IsWithinNamespace(expected);

	private static string DescribeNamespace(Type type)
		=> $"was in namespace {Formatter.Format(type.Namespace)}";
}
