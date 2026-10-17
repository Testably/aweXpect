using System;
using System.Xml.Linq;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Formatting;
using aweXpect.Helpers;
using aweXpect.Results;
using aweXpect.Xml;
using static aweXpect.Formatting.Format;

namespace aweXpect;

public static partial class ThatXElement
{
	/// <summary>
	///     Verifies that the subject has a child element with the <paramref name="name" />.
	/// </summary>
	/// <remarks>
	///     The child element is looked up like by <see cref="XContainer.Element(XName)" />, so that its namespace must be
	///     specified, e.g. <c>ns + "item"</c>, but not its prefix.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<XElement?, IThat<XElement?>> HasElement(
		this IThat<XElement?> source,
		XName name)
	{
		ThrowHelper.ThrowIfNull(name, nameof(name));
		return new AndOrResult<XElement?, IThat<XElement?>>(source.Get().ExpectationBuilder.AddConstraint(name,
				static (name, it, grammars) => CreateHasElementConstraint(it, grammars, name)),
			source);
	}

	/// <summary>
	///     Verifies that the subject has a child element with the <paramref name="name" /> which satisfies the
	///     <paramref name="expectations" />.
	/// </summary>
	/// <remarks>
	///     The child element is looked up like by <see cref="XContainer.Element(XName)" />, so that its namespace must be
	///     specified, e.g. <c>ns + "item"</c>, but not its prefix, and the <paramref name="expectations" /> are verified on
	///     the first child element with the <paramref name="name" />, when it exists.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<XElement?, IThat<XElement?>> HasElement(
		this IThat<XElement?> source,
		XName name,
		Action<IThat<XElement?>> expectations)
	{
		ThrowHelper.ThrowIfNull(name, nameof(name));
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		expectationBuilder.AddConstraint(name,
			static (name, it, grammars) => CreateHasElementConstraint(it, grammars, name));
		expectationBuilder
			.ForMember(MemberAccessor<FoundNode<XElement>, XElement?>.FromFunc(found => found.Node,
					$"element {Formatter.Format(name.ToString())}"),
				(_, stringBuilder) => stringBuilder.Append(" which "))
			.AddExpectations(e => expectations(new ThatSubject<XElement?>(e)));
		return new AndOrResult<XElement?, IThat<XElement?>>(expectationBuilder, source);
	}

	/// <summary>
	///     Verifies that the subject does not have a child element with the <paramref name="name" />.
	/// </summary>
	/// <remarks>
	///     The child element is looked up like by <see cref="XContainer.Element(XName)" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<XElement?, IThat<XElement?>> DoesNotHaveElement(
		this IThat<XElement?> source,
		XName name)
	{
		ThrowHelper.ThrowIfNull(name, nameof(name));
		return new AndOrResult<XElement?, IThat<XElement?>>(source.Get().ExpectationBuilder.AddConstraint(name,
				static (name, it, grammars) => CreateHasElementConstraint(it, grammars, name).Invert()),
			source);
	}

	private static HasNodeConstraint<XElement> CreateHasElementConstraint(string it,
		ExpectationGrammars grammars, XName name)
		=> new(it, grammars, $"element {Formatter.Format(name.ToString())}", element => element.Element(name));
}
