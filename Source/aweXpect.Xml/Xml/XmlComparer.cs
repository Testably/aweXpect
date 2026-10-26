using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using aweXpect.Customization;
using aweXpect.Formatting;
using aweXpect.Xml.Options;
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
	private static readonly XName XmlSpace = XNamespace.Xml + "space";

	/// <param name="actual">The element of the subject.</param>
	/// <param name="expected">The expected element.</param>
	/// <param name="options">The options of the comparison.</param>
	public static XmlComparisonResult Compare(XElement actual, XElement expected, XmlOptions options)
	{
		Comparison comparison = new(options, new MembersToIgnoreTracker(options));
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
		if (!comparison.IsDecided)
		{
			CompareChildren(actual, expected, path, comparison);
		}
	}

	/// <remarks>
	///     An attribute in another namespace is paired with an expected attribute of the same local name, so that the
	///     difference is reported once instead of as a missing and an unexpected attribute.
	/// </remarks>
	private static void CompareAttributes(XElement actual, XElement expected, string path, Comparison comparison)
	{
		if (!comparison.HasMembersToIgnore && HasEveryAttributeByName(actual, expected))
		{
			CompareAttributesByName(actual, expected, path, comparison);
			return;
		}

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
				result.AddError(attributePath, $"had missing {Formatter.Format(expectedAttribute.Value)}");
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
				result.AddError(attributePath, $"had unexpected {Formatter.Format(unexpectedAttribute.Value)}");
			}
		}
	}

	private static bool HasEveryAttributeByName(XElement actual, XElement expected)
	{
		for (XAttribute? attribute = expected.FirstAttribute; attribute is not null; attribute = attribute.NextAttribute)
		{
			if (!attribute.IsNamespaceDeclaration && actual.Attribute(attribute.Name) is null)
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>
	///     Compares the attributes by their name, when every expected attribute has an attribute with the same name in the
	///     subject and no member is ignored.
	/// </summary>
	/// <remarks>
	///     The general comparison pairs the attributes the same way then, but allocates a list of the unpaired
	///     attributes and a path for every attribute.
	/// </remarks>
	private static void CompareAttributesByName(XElement actual, XElement expected, string path,
		Comparison comparison)
	{
		XmlComparisonResult result = comparison.Result;
		for (XAttribute? expectedAttribute = expected.FirstAttribute;
		     expectedAttribute is not null;
		     expectedAttribute = expectedAttribute.NextAttribute)
		{
			if (expectedAttribute.IsNamespaceDeclaration)
			{
				continue;
			}

			XAttribute actualAttribute = actual.Attribute(expectedAttribute.Name)!;
			if (actualAttribute.Value != expectedAttribute.Value)
			{
				if (comparison.StopsAtFirstDifference)
				{
					result.AddUndescribedDifference();
					return;
				}

				result.AddError($"{path}{PathSeparator}@{expectedAttribute.Name.LocalName}",
					$"was {Formatter.Format(actualAttribute.Value)} instead of {Formatter.Format(expectedAttribute.Value)}");
			}
		}

		if (comparison.IgnoreAdditionalNodes)
		{
			return;
		}

		for (XAttribute? actualAttribute = actual.FirstAttribute;
		     actualAttribute is not null;
		     actualAttribute = actualAttribute.NextAttribute)
		{
			if (!actualAttribute.IsNamespaceDeclaration && expected.Attribute(actualAttribute.Name) is null)
			{
				result.AddError($"{path}{PathSeparator}@{actualAttribute.Name.LocalName}",
					$"had unexpected {Formatter.Format(actualAttribute.Value)}");
			}
		}
	}

	private static void CompareChildren(XElement actual, XElement expected, string path, Comparison comparison)
	{
		XmlComparisonResult result = comparison.Result;
		List<Content> actualChildren = GetContent(actual, path);
		List<Content> expectedChildren = GetContent(expected, path);
		if (!comparison.Options.IgnoreElementOrder && !comparison.HasMembersToIgnore &&
		    HaveSameNames(actualChildren, expectedChildren))
		{
			for (int i = 0; i < expectedChildren.Count; i++)
			{
				CompareChild(actualChildren[i], expectedChildren[i], comparison);
			}

			return;
		}

		Dictionary<Content, Content> pairs = comparison.Options.IgnoreElementOrder
			? PairIgnoringOrder(actualChildren, expectedChildren, comparison)
			: PairByKey(actualChildren, expectedChildren, path, comparison);

		foreach (Content expectedChild in expectedChildren)
		{
			if (!pairs.TryGetValue(expectedChild, out Content? actualChild))
			{
				result.AddError(expectedChild.Path, $"had missing {expectedChild.Format()}");
			}
			else if (!expectedChild.IsPairedAsEquivalent)
			{
				CompareChild(actualChild, expectedChild, comparison);
			}
		}

		if (comparison.IgnoreAdditionalNodes)
		{
			return;
		}

		HashSet<Content> pairedChildren = new(pairs.Values);
		foreach (Content unexpectedChild in actualChildren.Where(child => !pairedChildren.Contains(child)))
		{
			result.AddError(unexpectedChild.Path, $"had unexpected {unexpectedChild.Format()}");
		}
	}

	private static void CompareChild(Content actualChild, Content expectedChild, Comparison comparison)
	{
		if (expectedChild.Element is not null)
		{
			CompareElements(actualChild.Element!, expectedChild.Element, expectedChild.Path, comparison);
		}
		else if (actualChild.Text != expectedChild.Text)
		{
			if (comparison.StopsAtFirstDifference)
			{
				comparison.Result.AddUndescribedDifference();
			}
			else
			{
				comparison.Result.AddError(expectedChild.Path,
					$"was {Formatter.Format(actualChild.Text)} instead of {Formatter.Format(expectedChild.Text)}");
			}
		}
	}

	/// <summary>
	///     Checks if the children have the same names in the same order, so that <see cref="PairByKey" /> would pair
	///     them by their position without any difference in their order, and they can be compared one by one.
	/// </summary>
	private static bool HaveSameNames(List<Content> actualChildren, List<Content> expectedChildren)
	{
		if (actualChildren.Count != expectedChildren.Count)
		{
			return false;
		}

		for (int i = 0; i < expectedChildren.Count; i++)
		{
			if (actualChildren[i].Name != expectedChildren[i].Name)
			{
				return false;
			}
		}

		return true;
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
	///     <para />
	///     A child that is paired as equivalent is marked, so that it is not compared a second time.
	/// </remarks>
	private static Dictionary<Content, Content> PairIgnoringOrder(List<Content> actualChildren,
		List<Content> expectedChildren, Comparison comparison)
	{
		if (comparison.HasMembersToIgnore)
		{
			actualChildren.RemoveAll(child => comparison.IsIgnored(child.Path));
			expectedChildren.RemoveAll(child => comparison.IsIgnored(child.Path));
		}

		List<Content> unpairedChildren = new(actualChildren);
		Dictionary<Content, Content> pairs = new();
		Comparison candidate = comparison.ForCandidate();
		foreach (Content expectedChild in expectedChildren)
		{
			int index = FindEquivalent(unpairedChildren, expectedChild, candidate);
			if (index >= 0)
			{
				expectedChild.IsPairedAsEquivalent = true;
				pairs.Add(expectedChild, unpairedChildren[index]);
				unpairedChildren.RemoveAt(index);
			}
		}

		if (pairs.Count < expectedChildren.Count)
		{
			PairRemainingByName(unpairedChildren, expectedChildren, pairs);
		}

		return pairs;
	}

	private static int FindEquivalent(List<Content> unpairedChildren, Content expectedChild, Comparison candidate)
	{
		for (int i = 0; i < unpairedChildren.Count; i++)
		{
			Content child = unpairedChildren[i];
			if (child.Name == expectedChild.Name && IsEquivalent(child, expectedChild, candidate))
			{
				return i;
			}
		}

		return -1;
	}

	private static void PairRemainingByName(List<Content> unpairedChildren, List<Content> expectedChildren,
		Dictionary<Content, Content> pairs)
	{
		foreach (Content expectedChild in expectedChildren)
		{
			if (pairs.ContainsKey(expectedChild))
			{
				continue;
			}

			int index = unpairedChildren.FindIndex(child => child.Name == expectedChild.Name);
			if (index >= 0)
			{
				pairs.Add(expectedChild, unpairedChildren[index]);
				unpairedChildren.RemoveAt(index);
			}
		}
	}

	/// <remarks>
	///     The <paramref name="candidate" /> comparison is reused for every child, so its result is cleared first.
	/// </remarks>
	private static bool IsEquivalent(Content actualChild, Content expectedChild, Comparison candidate)
	{
		if (expectedChild.Element is null)
		{
			return actualChild.Text == expectedChild.Text;
		}

		candidate.Result.Clear();
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
		ContentCollector collector = new(parentPath);
		bool preservesWhitespace = PreservesWhitespace(parent);
		for (XNode? node = parent.FirstNode; node is not null; node = node.NextNode)
		{
			if (node is XText textNode)
			{
				if (preservesWhitespace || node is XCData || !IsWhitespace(textNode.Value))
				{
					collector.AddText(textNode.Value);
				}
			}
			else if (node is XElement element)
			{
				collector.AddElement(element);
			}
		}

		return collector.Complete();
	}

	/// <remarks>
	///     The ancestors are included, as <c>xml:space</c> is inherited, also from outside a subject nested in a document.
	/// </remarks>
	private static bool PreservesWhitespace(XElement element)
	{
		for (XElement? ancestor = element; ancestor is not null; ancestor = ancestor.Parent)
		{
			XAttribute? attribute = ancestor.Attribute(XmlSpace);
			if (attribute is not null)
			{
				return attribute.Value == "preserve";
			}
		}

		return false;
	}

	private static bool IsWhitespace(string text)
	{
		foreach (char c in text)
		{
			if (c is not (' ' or '\t' or '\r' or '\n'))
			{
				return false;
			}
		}

		return true;
	}

	private static string DescribeNamespaces(XNamespace actual, XNamespace expected)
		=> $"was in {DescribeNamespace(actual)} instead of {DescribeNamespace(expected)}";

	private static string DescribeNamespace(XNamespace ns)
		=> ns == XNamespace.None ? "no namespace" : $"namespace {Formatter.Format(ns.NamespaceName)}";

	/// <remarks>
	///     The <paramref name="tracker" /> is shared with the comparisons of the candidates for a pair, so that a member
	///     to ignore that matched while looking for an equivalent child counts as matched.
	/// </remarks>
	private sealed class Comparison(
		XmlOptions options,
		MembersToIgnoreTracker tracker,
		bool stopsAtFirstDifference = false)
	{
		public XmlOptions Options { get; } = options;
		public bool IgnoreAdditionalNodes => Options.IgnoreAdditionalNodes;
		public MembersToIgnoreTracker Tracker { get; } = tracker;
		public XmlComparisonResult Result { get; } = new(tracker);
		public bool HasMembersToIgnore => Options.MembersToIgnore.Count > 0;

		/// <summary>
		///     Whether only the existence of a difference matters, so that a difference is not described.
		/// </summary>
		public bool StopsAtFirstDifference { get; } = stopsAtFirstDifference;

		/// <summary>
		///     Whether the outcome is known and the remaining nodes need not be compared.
		/// </summary>
		public bool IsDecided => StopsAtFirstDifference && Result.HasError;

		/// <summary>
		///     Creates the comparison of the candidates for a pair, which shares the tracking of the members to ignore.
		/// </summary>
		/// <remarks>
		///     A candidate only needs to know whether it differs, so it stops at the first difference without describing
		///     it, unless members are ignored, whose tracking has to see every path.
		/// </remarks>
		public Comparison ForCandidate()
			=> new(Options, Tracker, !HasMembersToIgnore);

		public bool IsIgnored(string path)
			=> Tracker.IsIgnored(path);
	}

	/// <summary>
	///     A child element or text, identified by its <see cref="Key" /> of its name and its occurrence among the
	///     siblings of the same name.
	/// </summary>
	/// <remarks>
	///     The <see cref="Key" /> and the <see cref="Segment" /> are only built when needed, as children with the same
	///     names in both documents are compared without them.
	/// </remarks>
	private sealed class Content(
		XElement? element,
		string? text,
		int position,
		string parentPath,
		string name,
		int occurrence)
	{
		private string? _key;
		private string? _path;
		private string? _segment;

		public XElement? Element { get; } = element;
		public string? Text { get; } = text;
		public int Position { get; } = position;
		public string Name { get; } = name;
		public bool HasSiblingsOfSameName { get; set; }
		public bool IsPairedAsEquivalent { get; set; }
		public string Key => _key ??= $"{Name}[{occurrence}]";
		public string Segment => _segment ??= HasSiblingsOfSameName ? Key : Name;
		public string Path => _path ??= parentPath + PathSeparator + Segment;

		/// <summary>
		///     Formats an element on a single line like the "Actual" context formats the subject, and a text like a value.
		/// </summary>
		public string Format()
			=> Element?.ToString(SaveOptions.DisableFormatting) ?? Formatter.Format(Text);
	}

	/// <summary>
	///     Collects the children for <see cref="GetContent" />, joining adjacent texts and counting the siblings of the
	///     same name.
	/// </summary>
	/// <remarks>
	///     The occurrences are only counted from the second child on, as a single child needs no dictionary.
	/// </remarks>
	private sealed class ContentCollector(string parentPath)
	{
		private readonly List<Content> _content = new();
		private StringBuilder? _joinedText;
		private Dictionary<string, int>? _occurrences;
		private string? _text;

		public void AddText(string value)
		{
			if (_text is null)
			{
				_text = value;
			}
			else
			{
				(_joinedText ??= new StringBuilder(_text)).Append(value);
			}
		}

		public void AddElement(XElement element)
		{
			CompleteText();
			Add(element, null, element.Name.LocalName);
		}

		public List<Content> Complete()
		{
			CompleteText();
			if (_occurrences is not null)
			{
				foreach (Content child in _content)
				{
					child.HasSiblingsOfSameName = _occurrences[child.Name] > 1;
				}
			}

			return _content;
		}

		private void CompleteText()
		{
			string? joined = _joinedText?.ToString() ?? _text;
			_text = null;
			_joinedText = null;
			if (joined is { Length: > 0, })
			{
				Add(null, joined, "text()");
			}
		}

		private void Add(XElement? element, string? value, string name)
		{
			int occurrence = 1;
			if (_content.Count > 0)
			{
				_occurrences ??= new Dictionary<string, int> { [_content[0].Name] = 1, };
				_occurrences.TryGetValue(name, out occurrence);
				_occurrences[name] = ++occurrence;
			}

			_content.Add(new Content(element, value, _content.Count, parentPath, name, occurrence));
		}
	}

	internal sealed class XmlComparisonResult(MembersToIgnoreTracker tracker)
	{
		private readonly List<KeyValuePair<string, string>> _errors = new();
		private bool _hasUndescribedDifference;

		public bool HasError => _hasUndescribedDifference || _errors.Count > 0;

		public void AddError(string path, string error)
			=> _errors.Add(new KeyValuePair<string, string>(path, error));

		public void Clear()
		{
			_errors.Clear();
			_hasUndescribedDifference = false;
		}

		/// <summary>
		///     Records a difference of a comparison that only needs to know whether there is one.
		/// </summary>
		public void AddUndescribedDifference()
			=> _hasUndescribedDifference = true;

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
					sb.AppendLine().Append("  (… and ")
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
