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
///     verified separately.
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
		Comparison comparison = new(options, ignoreAdditionalNodes);
		string path = PathSeparator + expected.Name.LocalName;
		if (options.IsIgnored(path))
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

			if (comparison.Options.IsIgnored(attributePath))
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
			if (!comparison.Options.IsIgnored(attributePath))
			{
				result.AddError(attributePath, "was unexpected");
			}
		}
	}

	/// <remarks>
	///     A child is ignored with its pair, when the path of either is ignored, so that the documents cannot disagree
	///     about it, e.g. when only one of them has several siblings of the same name.
	/// </remarks>
	private static void CompareChildren(XElement actual, XElement expected, string path, Comparison comparison)
	{
		XmlComparisonResult result = comparison.Result;
		List<Content> actualChildren = GetContent(actual, path);
		List<Content> expectedChildren = GetContent(expected, path);
		HashSet<string> ignoredKeys = new(actualChildren.Concat(expectedChildren)
			.Where(child => comparison.Options.IsIgnored(child.Path))
			.Select(child => child.Key));
		actualChildren.RemoveAll(child => ignoredKeys.Contains(child.Key));
		expectedChildren.RemoveAll(child => ignoredKeys.Contains(child.Key));
		Dictionary<string, Content> actualChildrenByKey = actualChildren.ToDictionary(child => child.Key);
		HashSet<string> expectedKeys = new(expectedChildren.Select(child => child.Key));

		CompareOrder(expectedChildren, actualChildrenByKey, path, result);
		foreach (Content expectedChild in expectedChildren)
		{
			if (!actualChildrenByKey.TryGetValue(expectedChild.Key, out Content? actualChild))
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

		foreach (Content unexpectedChild in actualChildren.Where(child => !expectedKeys.Contains(child.Key)))
		{
			result.AddError(unexpectedChild.Path, "was unexpected");
		}
	}

	/// <summary>
	///     Verifies that the paired children of the subject are in the expected order.
	/// </summary>
	private static void CompareOrder(List<Content> expectedChildren,
		Dictionary<string, Content> actualChildrenByKey, string path, XmlComparisonResult result)
	{
		List<Content> pairedChildren = expectedChildren
			.Where(child => actualChildrenByKey.ContainsKey(child.Key))
			.ToList();
		List<Content> actualOrder = pairedChildren
			.OrderBy(child => actualChildrenByKey[child.Key].Position)
			.ToList();
		if (!actualOrder.SequenceEqual(pairedChildren))
		{
			result.AddError(path,
				$"had the child nodes in the order {string.Join(", ", actualOrder.Select(child => child.Segment))} instead of {string.Join(", ", pairedChildren.Select(child => child.Segment))}");
		}
	}

	/// <summary>
	///     Returns the child elements and texts of the <paramref name="parent" />, with adjacent text and CDATA sections
	///     joined to one text and without comments and processing instructions.
	/// </summary>
	private static List<Content> GetContent(XElement parent, string parentPath)
	{
		List<(XElement? Element, string? Text)> nodes = new();
		StringBuilder? text = null;
		foreach (XNode node in parent.Nodes())
		{
			if (node is XText textNode)
			{
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

	private static string DescribeNamespaces(XNamespace actual, XNamespace expected)
		=> $"was in {DescribeNamespace(actual)} instead of {DescribeNamespace(expected)}";

	private static string DescribeNamespace(XNamespace ns)
		=> ns == XNamespace.None ? "no namespace" : $"namespace {Formatter.Format(ns.NamespaceName)}";

	private sealed class Comparison(XmlOptions options, bool ignoreAdditionalNodes)
	{
		public XmlOptions Options { get; } = options;
		public bool IgnoreAdditionalNodes { get; } = ignoreAdditionalNodes;
		public XmlComparisonResult Result { get; } = new();
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
		public string Key { get; } = $"{name}[{occurrence}]";
		public string Segment { get; } = hasSiblingsOfSameName ? $"{name}[{occurrence}]" : name;
		public string Path => parentPath + PathSeparator + Segment;
	}

	internal sealed class XmlComparisonResult
	{
		private readonly List<KeyValuePair<string, string>> _errors = new();

		public bool HasError => _errors.Count > 0;

		public void AddError(string path, string error)
			=> _errors.Add(new KeyValuePair<string, string>(path, error));

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
