using System.Collections.Generic;
using System.Text;
using aweXpect.Core.Constraints;

namespace aweXpect.Core;

/// <summary>
///     Collects the <see cref="ResultContext" />s of the parts of a failed result that explain the failure, together
///     with the member of the subject they belong to.
/// </summary>
/// <remarks>
///     It is used while the failure message is created, so no context is created for an expectation that is met. Only
///     the item of a collection that decides the outcome has its contexts collected earlier, as its result is reused
///     for the next item.
/// </remarks>
public sealed class ResultContextCollector
{
	private List<Entry>? _entries;
	private List<Segment>? _subjectPath;
	private string _titlePrefix = "";

	internal ResultContextCollector()
	{
	}

	/// <summary>
	///     The collected contexts, in the order in which they were added.
	/// </summary>
	internal IReadOnlyList<Entry> Entries => _entries ?? [];

	/// <summary>
	///     Adds the <paramref name="context" />, which belongs to the subject or member that is currently visited.
	/// </summary>
	public void Add(ResultContext context)
		=> (_entries ??= []).Add(new Entry(context, _subjectPath?.ToArray() ?? [], _titlePrefix, false));

	/// <summary>
	///     Visits the <paramref name="result" />, which explains the failure, so that it adds its contexts.
	/// </summary>
	public void Visit(ConstraintResult result)
		=> result.AppendContexts(this);

	/// <summary>
	///     Visits the <paramref name="result" /> of the expectations on the <paramref name="member" />, so that its
	///     contexts are labelled with the <paramref name="member" />, e.g. <c>Collection (Items):</c>.
	/// </summary>
	public void VisitMember(string member, ConstraintResult result)
		=> VisitMember(new Segment(member, null), result);

	private void VisitMember(Segment member, ConstraintResult result)
	{
		List<Segment> subjectPath = _subjectPath ??= [];
		subjectPath.Add(member);
		try
		{
			Visit(result);
		}
		finally
		{
			subjectPath.RemoveAt(subjectPath.Count - 1);
		}
	}

	/// <summary>
	///     Visits the <paramref name="result" /> of the expectations on the item at the <paramref name="index" /> of a
	///     collection, so that its contexts are labelled with the index, e.g. <c>Actual (item [2]):</c>, and follow the
	///     contexts of the collection.
	/// </summary>
	public void VisitItem(int index, ConstraintResult result)
	{
		if (Capture(result) is { } captured)
		{
			AddCaptured($"[{index}]", captured);
		}
	}

	/// <summary>
	///     Visits the <paramref name="result" /> of the expectations on the <paramref name="member" />, or as part of the
	///     current subject when the member has no name.
	/// </summary>
	/// <param name="member">The name of the member.</param>
	/// <param name="result">The result of the expectations on the member.</param>
	/// <param name="pathRoot">
	///     The root that the <paramref name="member" /> starts with, when it is a path within the subject, see
	///     <see cref="MemberAccessor{TSource,TTarget}.FromFuncWithPath" />.
	/// </param>
	internal void VisitOptionalMember(string? member, ConstraintResult result, string? pathRoot = null)
	{
		if (member is null)
		{
			Visit(result);
		}
		else
		{
			VisitMember(new Segment(member, pathRoot), result);
		}
	}

	/// <summary>
	///     Visits the <paramref name="result" /> of one of several combined expectations, whose context titles start
	///     with the <paramref name="titlePrefix" />, e.g. <c>[01] </c>.
	/// </summary>
	internal void VisitWithTitlePrefix(string titlePrefix, ConstraintResult result)
	{
		string previousPrefix = _titlePrefix;
		_titlePrefix = titlePrefix;
		try
		{
			Visit(result);
		}
		finally
		{
			_titlePrefix = previousPrefix;
		}
	}

	/// <summary>
	///     Collects the contexts of the <paramref name="result" /> now, e.g. of the item of a collection that decides the
	///     outcome, before the result is evaluated for the next item.
	/// </summary>
	/// <returns>The contexts, or <see langword="null" /> when the <paramref name="result" /> has none.</returns>
	internal static IReadOnlyList<Entry>? Capture(ConstraintResult result)
	{
		ResultContextCollector collector = new();
		collector.Visit(result);
		return collector._entries;
	}

	/// <summary>
	///     Adds the <paramref name="captured" /> contexts as those of the <paramref name="member" />.
	/// </summary>
	internal void AddCaptured(string member, IReadOnlyList<Entry> captured)
	{
		foreach (Entry entry in captured)
		{
			List<Segment> subject = [.. _subjectPath ?? [], new Segment(member, null),];
			subject.AddRange(entry.Subject);
			(_entries ??= []).Add(new Entry(entry.Context, subject.ToArray(), _titlePrefix, true));
		}
	}

	/// <summary>
	///     A collected context with the path of the member it belongs to.
	/// </summary>
	internal readonly struct Entry(ResultContext context, Segment[] subject, string titlePrefix, bool isOfItem)
	{
		public ResultContext Context { get; } = context;

		/// <summary>
		///     Whether the context belongs to an item of a collection, which follows the contexts of the collection.
		/// </summary>
		public bool IsOfItem { get; } = isOfItem;

		public Segment[] Subject { get; } = subject;
		public string TitlePrefix { get; } = titlePrefix;

		/// <summary>
		///     The member path, e.g. <c>Order.Items</c> or <c>Items[2]</c>, or <see langword="null" /> for the subject.
		/// </summary>
		/// <remarks>
		///     An item index directly follows its collection, and an item of the subject is called <c>item [2]</c>. A path
		///     within the subject, e.g. <c>$.artist.origin</c>, replaces the label of a directly enclosing path of the same
		///     root, e.g. <c>$.artist</c>, and otherwise follows the label without its root, e.g.
		///     <c>elements[1].origin</c>.
		/// </remarks>
		public string? GetSubjectLabel()
		{
			if (Subject.Length == 0)
			{
				return null;
			}

			StringBuilder sb = new();
			string? previousRoot = null;
			int pathStart = 0;
			foreach (Segment segment in Subject)
			{
				AppendSegment(sb, segment, previousRoot, ref pathStart);
				previousRoot = segment.PathRoot;
			}

			return sb.ToString();
		}

		private static void AppendSegment(StringBuilder sb, Segment segment, string? previousRoot, ref int pathStart)
		{
			if (segment.PathRoot is { } root)
			{
				if (root == previousRoot)
				{
					sb.Length = pathStart;
				}
				else
				{
					pathStart = sb.Length;
				}

				int start = sb.Length == 0 ? 0 : root.Length;
				sb.Append(segment.Name, start, segment.Name.Length - start);
			}
			else if (segment.Name.StartsWith('['))
			{
				sb.Append(sb.Length == 0 ? "item " : "").Append(segment.Name);
			}
			else
			{
				sb.Append(sb.Length == 0 ? "" : ".").Append(segment.Name);
			}
		}
	}

	/// <summary>
	///     A segment of the member path of a context, see <see cref="Entry.GetSubjectLabel" />.
	/// </summary>
	/// <param name="name">The name of the member.</param>
	/// <param name="pathRoot">
	///     The root that the <paramref name="name" /> starts with, when it is a path within the subject, see
	///     <see cref="MemberAccessor{TSource,TTarget}.FromFuncWithPath" />.
	/// </param>
	internal readonly struct Segment(string name, string? pathRoot)
	{
		public string Name { get; } = name;
		public string? PathRoot { get; } = pathRoot;
	}
}
