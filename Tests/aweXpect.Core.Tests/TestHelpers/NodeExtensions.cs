using System.Text;
using aweXpect.Core.Nodes;

namespace aweXpect.Core.Tests.TestHelpers;

/// <summary>
///     Adds the mapping nodes that the expectation builder creates for a member.
/// </summary>
internal static class NodeExtensions
{
	public static Node AddMapping<TValue, TTarget>(this Node node, MemberAccessor<TValue, TTarget> memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null)
		=> node.AddMapping(new MappingNode<TValue, TTarget, TTarget>(memberAccessor, expectationTextGenerator));

	public static Node AddNarrowingMapping<TValue, TTarget, TNarrowed>(this Node node,
		MemberAccessor<TValue, TTarget> memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null)
		=> node.AddMapping(new MappingNode<TValue, TTarget, TNarrowed>(memberAccessor, expectationTextGenerator));

	public static Node AddAsyncMapping<TValue, TTarget>(this Node node,
		MemberAccessor<TValue, Task<TTarget>> memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null)
		=> node.AddMapping(new MappingNode<TValue, TTarget, TTarget>(memberAccessor, expectationTextGenerator));

	public static Node AddAsyncNarrowingMapping<TValue, TTarget, TNarrowed>(this Node node,
		MemberAccessor<TValue, Task<TTarget>> memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null)
		=> node.AddMapping(new MappingNode<TValue, TTarget, TNarrowed>(memberAccessor, expectationTextGenerator));
}
