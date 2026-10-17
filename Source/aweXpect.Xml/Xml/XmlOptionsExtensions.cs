using System;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Helpers;

namespace aweXpect.Xml;

/// <summary>
///     Extension methods for <see cref="XmlOptions" />.
/// </summary>
public static class XmlOptionsExtensions
{
	/// <summary>
	///     Ignores the order of the child nodes of all compared elements
	///     when <paramref name="ignoreElementOrder" /> is <see langword="true" />
	/// </summary>
	/// <remarks>
	///     Each expected child is paired with the first unpaired child of the subject with the same name that matches it,
	///     and the remaining children of the same name are paired in their order, so that their differences are
	///     reported. This greedy pairing can fail for <c>Matches</c>, which ignores additional nodes, when an expected
	///     child matches several children of the subject, even when another pairing would succeed.
	///     <para />
	///     A position in a member to ignore, e.g. <c>item[2]</c>, refers to the child in each document by its own
	///     position.
	/// </remarks>
	public static XmlOptions IgnoringElementOrder(
		this XmlOptions @this,
		bool ignoreElementOrder = true)
		=> @this with
		{
			IgnoreElementOrder = ignoreElementOrder,
		};

	/// <summary>
	///     Ignores the <paramref name="memberToIgnore" /> in the comparison.
	/// </summary>
	/// <remarks>
	///     The <paramref name="memberToIgnore" /> must cover whole trailing segments of the path, e.g.
	///     <c>"@createdAt"</c> ignores the attribute on every element and <c>"item/@id"</c> the attribute of every
	///     <c>item</c>, while <c>"/order/@id"</c>, starting with <c>/</c>, only ignores the attribute of the root. A
	///     segment without a position, e.g. <c>item</c>, matches every element of the name, and <c>item[2]</c> only the
	///     second. The names are compared case-sensitively.
	///     <para />
	///     An ignored attribute, element or text, and everything below it, is neither compared nor reported as missing
	///     or unexpected, and an ignored child is not considered for the order of the child nodes.
	/// </remarks>
	/// <exception cref="ArgumentException">The <paramref name="memberToIgnore" /> is empty.</exception>
	public static XmlOptions IgnoringMember(
		this XmlOptions @this,
		string memberToIgnore)
	{
		ThrowHelper.ThrowIfNull(memberToIgnore, nameof(memberToIgnore));
		if (XmlMemberToIgnore.ByPath.Split(memberToIgnore).Length == 0)
		{
			throw Tracing.WriteException(new ArgumentException(
				"The 'memberToIgnore' cannot be empty.", nameof(memberToIgnore)));
		}

		return @this with
		{
			MembersToIgnore = [..@this.MembersToIgnore, new XmlMemberToIgnore.ByPath(memberToIgnore),],
		};
	}

	/// <summary>
	///     Ignores the members whose path, e.g. <c>/order/item[2]/@price</c>, satisfies the
	///     <paramref name="predicate" /> in the comparison.
	/// </summary>
	/// <remarks>
	///     An ignored attribute, element or text, and everything below it, is neither compared nor reported as missing
	///     or unexpected, and an ignored child is not considered for the order of the child nodes.
	/// </remarks>
	public static XmlOptions Ignoring(
		this XmlOptions @this,
		Func<string, bool> predicate,
		[CallerArgumentExpression("predicate")] string doNotPopulateThisValue = "")
	{
		ThrowHelper.ThrowIfNull(predicate, nameof(predicate));
		return @this with
		{
			MembersToIgnore =
			[
				..@this.MembersToIgnore,
				new XmlMemberToIgnore.ByPredicate(predicate, doNotPopulateThisValue.TrimCommonWhiteSpace()),
			],
		};
	}
}
