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
	///     Verifies that the subject has an attribute with the <paramref name="name" />.
	/// </summary>
	/// <remarks>
	///     The attribute is looked up like by <see cref="XElement.Attribute(XName)" />, so that its namespace must be
	///     specified, e.g. <c>XNamespace.Xml + "lang"</c>, but not its prefix.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<XElement?, IThat<XElement?>> HasAttribute(
		this IThat<XElement?> source,
		XName name)
	{
		ThrowHelper.ThrowIfNull(name, nameof(name));
		return new AndOrResult<XElement?, IThat<XElement?>>(source.Get().ExpectationBuilder.AddConstraint(name,
				static (name, it, grammars) => CreateHasAttributeConstraint(it, grammars, name)),
			source);
	}

	/// <summary>
	///     Verifies that the subject has an attribute with the <paramref name="name" /> whose value satisfies the
	///     <paramref name="expectations" />.
	/// </summary>
	/// <remarks>
	///     The attribute is looked up like by <see cref="XElement.Attribute(XName)" />, so that its namespace must be
	///     specified, e.g. <c>XNamespace.Xml + "lang"</c>, but not its prefix. The <paramref name="expectations" /> are only
	///     verified, when the attribute exists.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<XElement?, IThat<XElement?>> HasAttribute(
		this IThat<XElement?> source,
		XName name,
		Action<IThat<string?>> expectations)
	{
		ThrowHelper.ThrowIfNull(name, nameof(name));
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		expectationBuilder.AddConstraint(name,
			static (name, it, grammars) => CreateHasAttributeConstraint(it, grammars, name));
		expectationBuilder
			.ForMember(MemberAccessor<FoundNode<XAttribute>, string?>.FromFunc(found => found.Node.Value,
					$"value of attribute {Formatter.Format(name.ToString())}"),
				(_, stringBuilder) => stringBuilder.Append(" whose value "))
			.AddExpectations(e => expectations(new ThatSubject<string?>(e)));
		return new AndOrResult<XElement?, IThat<XElement?>>(expectationBuilder, source);
	}

	/// <summary>
	///     Verifies that the subject does not have an attribute with the <paramref name="name" />.
	/// </summary>
	/// <remarks>
	///     The attribute is looked up like by <see cref="XElement.Attribute(XName)" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<XElement?, IThat<XElement?>> DoesNotHaveAttribute(
		this IThat<XElement?> source,
		XName name)
	{
		ThrowHelper.ThrowIfNull(name, nameof(name));
		return new AndOrResult<XElement?, IThat<XElement?>>(source.Get().ExpectationBuilder.AddConstraint(name,
				static (name, it, grammars) => CreateHasAttributeConstraint(it, grammars, name).Invert()),
			source);
	}

	private static HasNodeConstraint<XAttribute> CreateHasAttributeConstraint(string it,
		ExpectationGrammars grammars, XName name)
		=> new(it, grammars, $"attribute {Formatter.Format(name.ToString())}", element => element.Attribute(name));
}
