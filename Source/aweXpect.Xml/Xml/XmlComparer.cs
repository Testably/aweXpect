using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using aweXpect.Customization;
using aweXpect.Formatting;
using static aweXpect.Formatting.Format;

namespace aweXpect.Xml;

/// <summary>
///     Compares XML elements by their content instead of their formatting.
/// </summary>
/// <remarks>
///     Elements and attributes are compared by their namespace and local name, so that prefixes are not compared, and
///     attributes regardless of their order. Comments and processing instructions are ignored, and adjacent text and
///     CDATA sections are compared as one text.
///     <para />
///     Each difference is reported at the path of the node, which names every element by its local name, followed by
///     its 1-based position among the siblings of the same local name, if there are several, e.g.
///     <c>/order/item[2]/@price</c>, and every text as <c>text()</c>. The child nodes are paired by this name and
///     position, so that an additional or a missing element does not shift the elements after it, and their order is
///     verified separately, unless <see cref="XmlOptions.IgnoreElementOrder" /> is set.
/// </remarks>
internal static class XmlComparer
{
	internal const char PathSeparator = '/';

	/// <param name="actual">The element of the subject.</param>
	/// <param name="expected">The expected element.</param>
	/// <param name="options">The options of the comparison.</param>
	/// <param name="ignoreAdditionalNodes">
	///     Whether the attributes and child nodes of the subject that are not paired with an expected one are ignored.
	/// </param>
	public static XmlComparisonResult Compare(XElement actual, XElement expected, XmlOptions options,
		bool ignoreAdditionalNodes)
	{
		Comparison comparison = new(options, ignoreAdditionalNodes, new MembersToIgnoreTracker(options));
		string path = PathSeparator + expected.Name.LocalName;
		if (comparison.IsIgnored(path))
		{
			return comparison.Result;
		}

		if (actual.Name.LocalName != expected.Name.LocalName)
		{
			comparison.Result.AddError(path, $"was <{actual.Name.LocalName}> instead of <{expected.Name.LocalName}>");
		}
		else
		{
			CompareElements(actual, expected, path, comparison);
		}

		return comparison.Result;
	}

	private static void CompareElements(XElement actual, XElement expected, string path, Comparison comparison)
	{
		if (actual.Name.Namespace != expected.Name.Namespace)
		{
			comparison.Result.AddError(path, DescribeNamespaces(actual.Name.Namespace, expected.Name.Namespace));
			return;
		}

		CompareAttributes(actual, expected, path, comparison);
		CompareChildren(actual, expected, path, comparison);
	}

	/// <remarks>
	///     An attribute in another namespace is paired with an expected attribute of the same local name, so that the
	///     difference is reported once instead of as a missing and an unexpected attribute.
	/// </remarks>
	private static void CompareAttributes(XElement actual, XElement expected, string path, Comparison comparison)
	{
		XmlComparisonResult result = comparison.Result;
		List<XAttribute> unpairedAttributes = actual.Attributes().Where(a => !a.IsNamespaceDeclaration).ToList();
		foreach (XAttribute expectedAttribute in expected.Attributes().Where(a => !a.IsNamespaceDeclaration))
		{
			string attributePath = $"{path}{PathSeparator}@{expectedAttribute.Name.LocalName}";
			XAttribute? actualAttribute =
				unpairedAttributes.Find(a => a.Name == expectedAttribute.Name) ??
				unpairedAttributes.Find(a => a.Name.LocalName == expectedAttribute.Name.LocalName);
			if (actualAttribute is not null)
			{
				unpairedAttributes.Remove(actualAttribute);
			}

			if (comparison.IsIgnored(attributePath))
			{
				continue;
			}

			if (actualAttribute is null)
			{
				result.AddError(attributePath, "was missing");
			}
			else if (actualAttribute.Name.Namespace != expectedAttribute.Name.Namespace)
			{
				result.AddError(attributePath,
					DescribeNamespaces(actualAttribute.Name.Namespace, expectedAttribute.Name.Namespace));
			}
			else if (actualAttribute.Value != expectedAttribute.Value)
			{
				result.AddError(attributePath,
					$"was {Formatter.Format(actualAttribute.Value)} instead of {Formatter.Format(expectedAttribute.Value)}");
			}
		}

		if (comparison.IgnoreAdditionalNodes)
		{
			return;
		}

		foreach (XAttribute unexpectedAttribute in unpairedAttributes)
		{
			string attributePath = $"{path}{PathSeparator}@{unexpectedAttribute.Name.LocalName}";
			if (!comparison.IsIgnored(attributePath))
			{
				result.AddError(attributePath, "was unexpected");
			}
		}
	}

	private static void CompareChildren(XElement actual, XElement expected, string path, Comparison comparison)
	{
		XmlComparisonResult result = comparison.Result;
		List<Content> actualChildren = GetContent(actual, path);
		List<Content> expectedChildren = GetContent(expected, path);
		Dictionary<Content, Content> pairs = comparison.Options.IgnoreElementOrder
			? PairIgnoringOrder(actualChildren, expectedChildren, comparison)
			: PairByKey(actualChildren, expectedChildren, path, comparison);

		foreach (Content expectedChild in expectedChildren)
		{
			if (!pairs.TryGetValue(expectedChild, out Content? actualChild))
			{
				result.AddError(expectedChild.Path, "was missing");
			}
			else if (expectedChild.Element is not null)
			{
				CompareElements(actualChild.Element!, expectedChild.Element, expectedChild.Path, comparison);
			}
			else if (actualChild.Text != expectedChild.Text)
			{
				result.AddError(expectedChild.Path,
					$"was {Formatter.Format(actualChild.Text)} instead of {Formatter.Format(expectedChild.Text)}");
			}
		}

		if (comparison.IgnoreAdditionalNodes)
		{
			return;
		}

		HashSet<Content> pairedChildren = new(pairs.Values);
		foreach (Content unexpectedChild in actualChildren.Where(child => !pairedChildren.Contains(child)))
		{
			result.AddError(unexpectedChild.Path, "was unexpected");
		}
	}

	/// <summary>
	///     Pairs every expected child with the child of the subject with the same <see cref="Content.Key" />, and
	///     verifies that the paired children are in the expected order.
	/// </summary>
	/// <remarks>
	///     A child is ignored with its pair, when the path of either is ignored, so that the documents cannot disagree
	///     about it, e.g. when only one of them has several siblings of the same name.
	/// </remarks>
	private static Dictionary<Content, Content> PairByKey(List<Content> actualChildren,
		List<Content> expectedChildren, string path, Comparison comparison)
	{
		HashSet<string> ignoredKeys = new(actualChildren.Concat(expectedChildren)
			.Where(child => comparison.IsIgnored(child.Path))
			.Select(child => child.Key));
		actualChildren.RemoveAll(child => ignoredKeys.Contains(child.Key));
		expectedChildren.RemoveAll(child => ignoredKeys.Contains(child.Key));
		Dictionary<string, Content> actualChildrenByKey = actualChildren.ToDictionary(child => child.Key);
		Dictionary<Content, Content> pairs = expectedChildren
			.Where(child => actualChildrenByKey.ContainsKey(child.Key))
			.ToDictionary(child => child, child => actualChildrenByKey[child.Key]);

		List<Content> pairedChildren = expectedChildren.Where(pairs.ContainsKey).ToList();
		List<Content> actualOrder = pairedChildren.OrderBy(child => pairs[child].Position).ToList();
		if (!actualOrder.SequenceEqual(pairedChildren))
		{
			comparison.Result.AddError(path,
				$"had the child nodes in the order {string.Join(", ", actualOrder.Select(child => child.Segment))} instead of {string.Join(", ", pairedChildren.Select(child => child.Segment))}");
		}

		return pairs;
	}

	/// <summary>
	///     Pairs every expected child with the first unpaired child of the subject with the same name that is equivalent
	///     to it, and then the remaining children of the same name in their order, so that their differences are
	///     reported instead of a missing and an unexpected child.
	/// </summary>
	/// <remarks>
	///     The children are ignored by their own path in each document, as a position no longer pairs them.
	///     <para />
	///     The pairing is greedy, so that an expected child can be paired with a child of the subject that a later
	///     expected child would have needed, when additional nodes are ignored.
	/// </remarks>
	private static Dictionary<Content, Content> PairIgnoringOrder(List<Content> actualChildren,
		List<Content> expectedChildren, Comparison comparison)
	{
		actualChildren.RemoveAll(child => comparison.IsIgnored(child.Path));
		expectedChildren.RemoveAll(child => comparison.IsIgnored(child.Path));
		List<Content> unpairedChildren = new(actualChildren);
		Dictionary<Content, Content> pairs = new();
		foreach (Content expectedChild in expectedChildren)
		{
			Pair(expectedChild, unpairedChildren.Find(child =>
				child.Name == expectedChild.Name && IsEquivalent(child, expectedChild, comparison)));
		}

		foreach (Content expectedChild in expectedChildren.Where(child => !pairs.ContainsKey(child)))
		{
			Pair(expectedChild, unpairedChildren.Find(child => child.Name == expectedChild.Name));
		}

		return pairs;

		void Pair(Content expectedChild, Content? actualChild)
		{
			if (actualChild is not null)
			{
				pairs.Add(expectedChild, actualChild);
				unpairedChildren.Remove(actualChild);
			}
		}
	}

	private static bool IsEquivalent(Content actualChild, Content expectedChild, Comparison comparison)
	{
		if (expectedChild.Element is null)
		{
			return actualChild.Text == expectedChild.Text;
		}

		Comparison candidate = new(comparison.Options, comparison.IgnoreAdditionalNodes, comparison.Tracker);
		CompareElements(actualChild.Element!, expectedChild.Element, expectedChild.Path, candidate);
		return !candidate.Result.HasError;
	}

	/// <summary>
	///     Returns the child elements and texts of the <paramref name="parent" />, with adjacent text and CDATA sections
	///     joined to one text and without insignificant whitespace, comments and processing instructions.
	/// </summary>
	/// <remarks>
	///     Whitespace-only text is dropped like by <see cref="System.Xml.XmlReaderSettings.IgnoreWhitespace" />, which
	///     the expected XML is parsed with, so that a subject loaded with <see cref="LoadOptions.PreserveWhitespace" />
	///     is compared the same way.
	/// </remarks>
	private static List<Content> GetContent(XElement parent, string parentPath)
	{
		List<(XElement? Element, string? Text)> nodes = new();
		StringBuilder? text = null;
		bool preservesWhitespace = PreservesWhitespace(parent);
		foreach (XNode node in parent.Nodes())
		{
			if (node is XText textNode)
			{
				if (!preservesWhitespace && node is not XCData && IsWhitespace(textNode.Value))
				{
					continue;
				}

				text ??= new StringBuilder();
				text.Append(textNode.Value);
			}
			else if (node is XElement element)
			{
				AddText(nodes, text);
				text = null;
				nodes.Add((element, null));
			}
		}

		AddText(nodes, text);

		Dictionary<string, int> counts = nodes
			.GroupBy(GetName)
			.ToDictionary(group => group.Key, group => group.Count());
		Dictionary<string, int> occurrences = new();
		List<Content> content = new(nodes.Count);
		foreach ((XElement? Element, string? Text) node in nodes)
		{
			string name = GetName(node);
			occurrences.TryGetValue(name, out int occurrence);
			occurrences[name] = ++occurrence;
			content.Add(new Content(node.Element, node.Text, content.Count, parentPath, name, occurrence,
				counts[name] > 1));
		}

		return content;

		static void AddText(List<(XElement?, string?)> nodes, StringBuilder? text)
		{
			if (text is { Length: > 0, })
			{
				nodes.Add((null, text.ToString()));
			}
		}

		static string GetName((XElement? Element, string? Text) node)
			=> node.Element?.Name.LocalName ?? "text()";
	}

	/// <remarks>
	///     The ancestors are included, as <c>xml:space</c> is inherited, also from outside a subject nested in a document.
	/// </remarks>
	private static bool PreservesWhitespace(XElement element)
		=> element.AncestorsAndSelf()
			.Select(e => e.Attribute(XNamespace.Xml + "space"))
			.FirstOrDefault(attribute => attribute is not null)?.Value == "preserve";

	private static bool IsWhitespace(string text)
		=> text.All(c => c is ' ' or '\t' or '\r' or '\n');

	private static string DescribeNamespaces(XNamespace actual, XNamespace expected)
		=> $"was in {DescribeNamespace(actual)} instead of {DescribeNamespace(expected)}";

	private static string DescribeNamespace(XNamespace ns)
		=> ns == XNamespace.None ? "no namespace" : $"namespace {Formatter.Format(ns.NamespaceName)}";

	/// <remarks>
	///     The <paramref name="tracker" /> is shared with the comparisons of the candidates for a pair, so that a member
	///     to ignore that matched while looking for an equivalent child counts as matched.
	/// </remarks>
	private sealed class Comparison(XmlOptions options, bool ignoreAdditionalNodes, MembersToIgnoreTracker tracker)
	{
		public XmlOptions Options { get; } = options;
		public bool IgnoreAdditionalNodes { get; } = ignoreAdditionalNodes;
		public MembersToIgnoreTracker Tracker { get; } = tracker;
		public XmlComparisonResult Result { get; } = new(tracker);

		public bool IsIgnored(string path)
			=> Tracker.IsIgnored(path);
	}

	/// <summary>
	///     A child element or text, identified by its <see cref="Key" /> of its name and its occurrence among the
	///     siblings of the same name.
	/// </summary>
	private sealed class Content(
		XElement? element,
		string? text,
		int position,
		string parentPath,
		string name,
		int occurrence,
		bool hasSiblingsOfSameName)
	{
		public XElement? Element { get; } = element;
		public string? Text { get; } = text;
		public int Position { get; } = position;
		public string Name { get; } = name;
		public string Key { get; } = $"{name}[{occurrence}]";
		public string Segment { get; } = hasSiblingsOfSameName ? $"{name}[{occurrence}]" : name;
		public string Path => parentPath + PathSeparator + Segment;
	}

	internal sealed class XmlComparisonResult(MembersToIgnoreTracker tracker)
	{
		private readonly List<KeyValuePair<string, string>> _errors = new();

		public bool HasError => _errors.Count > 0;

		public void AddError(string path, string error)
			=> _errors.Add(new KeyValuePair<string, string>(path, error));

		/// <summary>
		///     Returns each member to ignore by path that matched no member, with the path it most likely meant, when the
		///     comparison failed, otherwise <see langword="null" />.
		/// </summary>
		public Dictionary<string, string?>? GetUnmatchedMembersToIgnore()
			=> HasError ? tracker.GetUnmatchedMembersToIgnore() : null;

		public override string ToString()
		{
			StringBuilder sb = new();
			bool hasMoreThanOneDifference = _errors.Count > 1;
			int count = 0;
			int maximumNumberOfCollectionItems =
				Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
			foreach (KeyValuePair<string, string> error in _errors)
			{
				if (sb.Length > 0)
				{
					sb.Append(" and");
				}

				if (count++ >= maximumNumberOfCollectionItems)
				{
					sb.AppendLine().Append("   … (")
						.Append(_errors.Count - maximumNumberOfCollectionItems)
						.Append(" more)");
					return sb.ToString();
				}

				if (hasMoreThanOneDifference)
				{
					sb.AppendLine().Append(' ');
				}

				sb.Append(' ').Append(error.Key).Append(' ').Append(error.Value);
			}

			return sb.ToString();
		}
	}
}
