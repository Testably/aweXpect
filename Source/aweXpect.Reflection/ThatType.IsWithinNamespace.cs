using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatType
{
	[GenerateExpectation("IsWithinNamespace", "is {not} within namespace {expected}",
		Result = nameof(DescribeNamespace),
		Summary = """
		          Verifies that the <see cref="Type" /> is within the <paramref name="expected" /> namespace
		          (including sub-namespaces).
		          """)]
	private static bool IsWithinNamespaceType(Type type, string expected) => type.IsWithinNamespace(expected);

	[GenerateExpectation("IsNotWithinNamespace", "is not within namespace {unexpected}",
		NegatedExpectationText = "is within namespace {unexpected}",
		Result = nameof(DescribeNamespace),
		Summary = """
		          Verifies that the <see cref="Type" /> is not within the <paramref name="unexpected" /> namespace
		          (including sub-namespaces).
		          """)]
	private static bool IsNotWithinNamespaceType(Type type, string unexpected) => !type.IsWithinNamespace(unexpected);

	private static string DescribeNamespace(Type type)
		=> $"was in namespace {Formatter.Format(type.Namespace)}";
}
