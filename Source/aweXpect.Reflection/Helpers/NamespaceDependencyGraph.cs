using System;
using System.Collections.Generic;
using System.Linq;

namespace aweXpect.Reflection.Helpers;

/// <summary>
///     Builds the directed namespace (or slice) dependency graph of a set of types and detects its dependency cycles.
/// </summary>
/// <remarks>
///     Nodes are the namespaces (or, when a slice root is given, the slices) that the analyzed types live in. An edge
///     <c>A → B</c> exists when some type in node <c>A</c> has a (resolved) dependency on a type whose node is <c>B</c>
///     and <c>B</c> is also part of the analyzed set — dependencies on framework or otherwise out-of-set namespaces
///     never create an edge, consistent with the namespace dependency assertions. Self-edges (<c>A → A</c>) are ignored,
///     so a single namespace referencing itself is not a cycle.
///     <para />
///     By default a namespace and its sub-namespaces collapse into a single node (a "family"), so references between
///     them never create an edge — and, because they are one node rather than merely an unconnected pair, an indirect
///     cycle that re-enters the family through a different sub-namespace is detected too. Passing
///     <c>excludeSubNamespaces</c> opts into strict per-namespace nodes, so a namespace referencing one of its
///     sub-namespaces can then form a cycle.
///     <para />
///     Dependencies are read exclusively through <see cref="TypeHelpers.ResolveDependencies" />, so a configured custom
///     dependency resolver automatically sharpens the cycle detection too.
///     <para />
///     Cycles are the strongly-connected components with more than one node, found via Tarjan's algorithm. For each one
///     a readable path back to its starting node is rendered (e.g. <c>A -&gt; B -&gt; A</c>).
/// </remarks>
internal sealed class NamespaceDependencyGraph
{
	private NamespaceDependencyGraph(IReadOnlyList<IReadOnlyList<string>> cycles)
		=> Cycles = cycles;

	/// <summary>
	///     The detected dependency cycles, each as an ordered list of node names that returns to its first node
	///     (e.g. <c>["A", "B", "A"]</c>). Empty when the graph is acyclic.
	/// </summary>
	public IReadOnlyList<IReadOnlyList<string>> Cycles { get; }

	/// <summary>
	///     Builds the graph for the <paramref name="types" />, grouping namespaces into slices below
	///     <paramref name="sliceRoot" /> when it is not <see langword="null" /> (a namespace below the root collapses
	///     to the root plus its next segment), and detects all dependency cycles. Unless
	///     <paramref name="excludeSubNamespaces" /> is set (or a slice root is given), a namespace and its
	///     sub-namespaces collapse into one node, so references within that family never create an edge.
	/// </summary>
	public static NamespaceDependencyGraph Build(IEnumerable<Type?> types, string? sliceRoot, bool excludeSubNamespaces)
	{
		List<Type> nodes = [];
		foreach (Type? type in types)
		{
			if (type is not null)
			{
				nodes.Add(type);
			}
		}

		Func<string?, string> nodeKey = CreateNodeKeySelector(nodes, sliceRoot, excludeSubNamespaces);
		return new NamespaceDependencyGraph(DetectCycles(BuildAdjacency(nodes, nodeKey)));
	}

	/// <summary>
	///     Builds the directed graph: every node the analyzed types occupy (the only legal edge endpoints), plus an
	///     edge <c>from → to</c> for every resolved dependency that lands on another in-set node (self-edges and
	///     dependencies outside the analyzed set are ignored).
	/// </summary>
	private static Dictionary<string, HashSet<string>> BuildAdjacency(
		IReadOnlyList<Type> nodes, Func<string?, string> nodeKey)
	{
		Dictionary<string, HashSet<string>> adjacency = new(StringComparer.Ordinal);
		foreach (Type type in nodes)
		{
			string node = nodeKey(type.Namespace);
			if (!adjacency.ContainsKey(node))
			{
				adjacency[node] = new HashSet<string>(StringComparer.Ordinal);
			}
		}

		foreach (Type type in nodes)
		{
			string from = nodeKey(type.Namespace);
			foreach (Type dependency in type.ResolveDependencies())
			{
				string to = nodeKey(dependency.Namespace);
				if (!string.Equals(from, to, StringComparison.Ordinal) && adjacency.ContainsKey(to))
				{
					adjacency[from].Add(to);
				}
			}
		}

		return adjacency;
	}

	/// <summary>
	///     Builds the function that maps a type's namespace to its graph node. With a <paramref name="sliceRoot" />
	///     the node is the slice; otherwise it is the namespace itself, except that — unless
	///     <paramref name="excludeSubNamespaces" /> is set — a namespace is merged into the shortest namespace of the
	///     analyzed set that is an ancestor of it, so a namespace and its sub-namespaces collapse into one node. The
	///     merge (rather than a mere edge suppression) is what lets an indirect cycle that re-enters the family through
	///     a different sub-namespace be detected.
	/// </summary>
	private static Func<string?, string> CreateNodeKeySelector(
		IReadOnlyList<Type> nodes, string? sliceRoot, bool excludeSubNamespaces)
	{
		if (sliceRoot is not null || excludeSubNamespaces)
		{
			return @namespace => SliceKey(@namespace, sliceRoot);
		}

		Dictionary<string, string> family =
			BuildFamilyMap(nodes.Select(node => SliceKey(node.Namespace, sliceRoot)));
		return @namespace =>
		{
			string key = SliceKey(@namespace, sliceRoot);
			return family.TryGetValue(key, out string? representative) ? representative : key;
		};
	}

	/// <summary>
	///     Maps every distinct namespace of the analyzed set to its family representative — the shortest namespace in
	///     the set that is an ancestor of it (or itself when none is shorter) — so a namespace and its sub-namespaces
	///     share one node. The global-namespace node is never in a family with a real namespace.
	/// </summary>
	private static Dictionary<string, string> BuildFamilyMap(IEnumerable<string> namespaces)
	{
		List<string> present = namespaces.Distinct(StringComparer.Ordinal).ToList();
		Dictionary<string, string> map = new(StringComparer.Ordinal);
		foreach (string key in present)
		{
			string representative = key;
			foreach (string candidate in present)
			{
				if (candidate.Length < representative.Length &&
				    TypeHelpers.NamespaceMatches(key, candidate, includeSubNamespaces: true))
				{
					representative = candidate;
				}
			}

			map[key] = representative;
		}

		return map;
	}

	/// <summary>
	///     Maps the <paramref name="namespace" /> to its node name: the namespace itself by default, or — when a
	///     <paramref name="sliceRoot" /> is given and the namespace lies below it — the root plus its next segment.
	/// </summary>
	private static string SliceKey(string? @namespace, string? sliceRoot)
	{
		if (@namespace is null)
		{
			return TypeHelpers.GlobalNamespaceDisplay;
		}

		if (sliceRoot is null ||
		    !TypeHelpers.NamespaceMatches(@namespace, sliceRoot, includeSubNamespaces: true) ||
		    string.Equals(@namespace, sliceRoot, StringComparison.Ordinal))
		{
			return @namespace;
		}

		int segmentStart = sliceRoot.Length + 1;
		int nextDot = @namespace.IndexOf('.', segmentStart);
		return nextDot < 0 ? @namespace : @namespace.Substring(0, nextDot);
	}

	/// <summary>
	///     Returns the strongly-connected components with more than one node (the cycles), each rendered as a path
	///     that returns to its starting node. The result is ordered (and the path within each cycle starts at its
	///     ordinally smallest node) so failure messages are deterministic.
	/// </summary>
	private static List<IReadOnlyList<string>> DetectCycles(
		Dictionary<string, HashSet<string>> adjacency)
	{
		List<IReadOnlyList<string>> cycles = [];
		foreach (HashSet<string> component in new TarjanScc(adjacency).StronglyConnectedComponents())
		{
			if (component.Count > 1)
			{
				cycles.Add(ExtractCyclePath(component, adjacency));
			}
		}

		cycles.Sort((left, right) => string.Compare(string.Join(" -> ", left), string.Join(" -> ", right),
			StringComparison.Ordinal));
		return cycles;
	}

	/// <summary>
	///     Finds the shortest cycle through the ordinally smallest node of the strongly-connected
	///     <paramref name="component" /> via a breadth-first search over the component-internal edges.
	/// </summary>
	private static List<string> ExtractCyclePath(
		HashSet<string> component, Dictionary<string, HashSet<string>> adjacency)
	{
		string start = component.OrderBy(node => node, StringComparer.Ordinal).First();
		Dictionary<string, string> predecessor = new(StringComparer.Ordinal);
		HashSet<string> visited = new(StringComparer.Ordinal) { start, };
		Queue<string> queue = new();
		queue.Enqueue(start);

		while (queue.Count > 0)
		{
			string current = queue.Dequeue();
			foreach (string next in adjacency[current]
				         .Where(component.Contains)
				         .OrderBy(node => node, StringComparer.Ordinal))
			{
				if (string.Equals(next, start, StringComparison.Ordinal))
				{
					return BuildPath(predecessor, start, current);
				}

				if (visited.Add(next))
				{
					predecessor[next] = current;
					queue.Enqueue(next);
				}
			}
		}

		// A strongly-connected component with more than one node always contains a cycle through its start node,
		// so the breadth-first search above always returns; this is unreachable.
		return [start, start,];
	}

	/// <summary>
	///     Reconstructs the path <c>start → … → end → start</c> from the breadth-first <paramref name="predecessor" />
	///     map (the trailing <paramref name="start" /> closes the cycle).
	/// </summary>
	private static List<string> BuildPath(
		Dictionary<string, string> predecessor, string start, string end)
	{
		List<string> path = [];
		for (string node = end; !string.Equals(node, start, StringComparison.Ordinal); node = predecessor[node])
		{
			path.Add(node);
		}

		path.Add(start);
		path.Reverse();
		path.Add(start);
		return path;
	}

	/// <summary>
	///     Tarjan strongly-connected-components search over the namespace/slice graph. The node count is the number of
	///     distinct namespaces/slices in the analyzed set, so the recursive walk cannot reach a problematic depth.
	/// </summary>
	private sealed class TarjanScc(Dictionary<string, HashSet<string>> adjacency)
	{
		private readonly Dictionary<string, int> _indices = new(StringComparer.Ordinal);
		private readonly Dictionary<string, int> _lowLinks = new(StringComparer.Ordinal);
		private readonly HashSet<string> _onStack = new(StringComparer.Ordinal);
		private readonly Stack<string> _stack = new();
		private readonly List<HashSet<string>> _components = [];
		private int _nextIndex;

		public List<HashSet<string>> StronglyConnectedComponents()
		{
			// StrongConnect mutates _indices during the walk, so the already-visited check is done inside the loop
			// (evaluated per node) rather than as a Where over the ordered keys.
			foreach (string node in adjacency.Keys.OrderBy(node => node, StringComparer.Ordinal))
			{
				if (!_indices.ContainsKey(node))
				{
					StrongConnect(node);
				}
			}

			return _components;
		}

		private void StrongConnect(string node)
		{
			_indices[node] = _nextIndex;
			_lowLinks[node] = _nextIndex;
			_nextIndex++;
			_stack.Push(node);
			_onStack.Add(node);

			foreach (string next in adjacency[node])
			{
				if (!_indices.TryGetValue(next, out int nextIndex))
				{
					StrongConnect(next);
					_lowLinks[node] = Math.Min(_lowLinks[node], _lowLinks[next]);
				}
				else if (_onStack.Contains(next))
				{
					_lowLinks[node] = Math.Min(_lowLinks[node], nextIndex);
				}
			}

			if (_lowLinks[node] != _indices[node])
			{
				return;
			}

			HashSet<string> component = new(StringComparer.Ordinal);
			string member;
			do
			{
				member = _stack.Pop();
				_onStack.Remove(member);
				component.Add(member);
			} while (!string.Equals(member, node, StringComparison.Ordinal));

			_components.Add(component);
		}
	}
}
