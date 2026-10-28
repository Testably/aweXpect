using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Metadata;

namespace aweXpect.Equivalency;

/// <summary>
///     Allows comparing two instances for equivalency.
/// </summary>
public static partial class EquivalencyComparison
{
	private static readonly ConditionalWeakTable<EquivalencyOptions, ConcurrentDictionary<Type, EquivalencyTypeOptions?>>
		RegisteredOptionsCache = new();

	/// <remarks>
	///     Shared, because a comparison that only decides never writes to it.
	/// </remarks>
	private static readonly StringBuilder UnusedFailureBuilder = new();

	/// <summary>
	///     Checks if <paramref name="actual" /> is considered equivalent to <paramref name="expected" /> using the
	///     <paramref name="equivalencyOptions" />.
	/// </summary>
	/// <remarks>
	///     In case of a difference, the <paramref name="failureBuilder" /> contains a human readable explanation.
	/// </remarks>
	public static ValueTask<bool>
		Compare<TActual, TExpected>(
			[RequiresMemberMetadata] TActual actual,
			[RequiresMemberMetadata] TExpected expected,
			EquivalencyOptions equivalencyOptions,
			StringBuilder failureBuilder)
		=> Compare(actual, expected, equivalencyOptions, failureBuilder, null, CancellationToken.None);

	/// <summary>
	///     Checks if <paramref name="actual" /> is considered equivalent to <paramref name="expected" /> like
	///     <see cref="Compare{TActual, TExpected}(TActual, TExpected, EquivalencyOptions, StringBuilder)" />, as part of
	///     the <paramref name="evaluation" />.
	/// </summary>
	/// <param name="actual">The actual value.</param>
	/// <param name="expected">The expected value.</param>
	/// <param name="equivalencyOptions">The options of the comparison.</param>
	/// <param name="failureBuilder">Receives the explanation of a difference.</param>
	/// <param name="evaluation">
	///     The evaluation whose cancellation and time provider the expectations of an <c>It.Is…</c> in the
	///     <paramref name="expected" /> object use, or <see langword="null" /> outside an evaluation.
	/// </param>
	/// <param name="cancellationToken">The token that cancels the <paramref name="evaluation" />.</param>
	/// <remarks>
	///     A constraint passes the <see cref="IEvaluationContext" /> and the <see cref="CancellationToken" /> it is
	///     evaluated with, so that the timeout and the cancellation of its evaluation also end these expectations.
	/// </remarks>
	public static async ValueTask<bool>
		Compare<TActual, TExpected>(
			[RequiresMemberMetadata] TActual actual,
			[RequiresMemberMetadata] TExpected expected,
			EquivalencyOptions equivalencyOptions,
			StringBuilder failureBuilder,
			IEvaluationContext? evaluation,
			CancellationToken cancellationToken)
		=> (await CompareExplaining(actual, expected, equivalencyOptions, failureBuilder, evaluation,
			null, cancellationToken)).IsEquivalent;

	/// <summary>
	///     Compares like
	///     <see cref="Compare{TActual, TExpected}(TActual, TExpected, EquivalencyOptions, StringBuilder, IEvaluationContext?, CancellationToken)" />
	///     and, when the objects are not equivalent, adds the members to ignore by name that matched no compared
	///     member to the <paramref name="unmatchedMembers" />, with the compared member each one most likely meant.
	/// </summary>
	internal static async ValueTask<(bool IsEquivalent, Dictionary<string, string?>? UnmatchedMembers)>
		CompareExplaining<TActual, TExpected>(
			TActual actual,
			TExpected expected,
			EquivalencyOptions equivalencyOptions,
			StringBuilder failureBuilder,
			IEvaluationContext? evaluation,
			Dictionary<string, string?>? unmatchedMembers,
			CancellationToken cancellationToken)
	{
		int start = failureBuilder.Length;
		EquivalencyContext context = new(equivalencyOptions)
		{
			Evaluation = evaluation,
			CancellationToken = cancellationToken,
			TracksIgnoredMembers = true,
		};
		bool result = await Compare(
			actual,
			expected,
			equivalencyOptions,
			equivalencyOptions,
			failureBuilder,
			"",
			MemberType.Value,
			context);
		JoinSingleLineEntries(failureBuilder, start);
		return (result, result ? unmatchedMembers : context.AddUnmatchedMembersToIgnore(unmatchedMembers));
	}

	/// <summary>
	///     Decides like <see cref="IsEquivalent{TActual, TExpected}" /> and, when the objects are not equivalent, adds
	///     the members to ignore by name that matched no compared member to the <paramref name="unmatchedMembers" />.
	/// </summary>
	/// <remarks>
	///     The items of a collection are compared without an explanation, so the members to ignore that no item
	///     matched are only known from these comparisons.
	/// </remarks>
	internal static async ValueTask<(bool IsEquivalent, Dictionary<string, string?>? UnmatchedMembers)>
		IsEquivalentTrackingMembersToIgnore<TActual, TExpected>(
			TActual actual,
			TExpected expected,
			EquivalencyOptions equivalencyOptions,
			IEvaluationContext? evaluation,
			Dictionary<string, string?>? unmatchedMembers,
			CancellationToken cancellationToken)
	{
		EquivalencyContext context = new(equivalencyOptions)
		{
			IsDecidingOnly = true,
			Evaluation = evaluation,
			CancellationToken = cancellationToken,
			TracksIgnoredMembers = true,
		};
		bool result = await Compare(
			actual,
			expected,
			equivalencyOptions,
			equivalencyOptions,
			UnusedFailureBuilder,
			"",
			MemberType.Value,
			context);
		return (result, result ? unmatchedMembers : context.AddUnmatchedMembersToIgnore(unmatchedMembers));
	}

	/// <summary>
	///     Checks if <paramref name="actual" /> is considered equivalent to <paramref name="expected" /> using the
	///     <paramref name="equivalencyOptions" />, without explaining a difference.
	/// </summary>
	/// <param name="actual">The actual value.</param>
	/// <param name="expected">The expected value.</param>
	/// <param name="equivalencyOptions">The options of the comparison.</param>
	/// <param name="evaluation">
	///     The evaluation whose cancellation and time provider the expectations of an <c>It.Is…</c> in the
	///     <paramref name="expected" /> object use, or <see langword="null" /> outside an evaluation.
	/// </param>
	/// <param name="cancellationToken">The token that cancels the <paramref name="evaluation" />.</param>
	internal static ValueTask<bool>
		IsEquivalent<TActual, TExpected>(
			TActual actual,
			TExpected expected,
			EquivalencyOptions equivalencyOptions,
			IEvaluationContext? evaluation = null,
			CancellationToken cancellationToken = default)
		=> Compare(
			actual,
			expected,
			equivalencyOptions,
			equivalencyOptions,
			UnusedFailureBuilder,
			"",
			MemberType.Value,
			new EquivalencyContext(equivalencyOptions)
			{
				IsDecidingOnly = true,
				Evaluation = evaluation,
				CancellationToken = cancellationToken,
			});

	/// <remarks>
	///     The entries are written with the "and" on its own line, because whether any of them spans several lines is
	///     only known once all of them are written.
	/// </remarks>
	private static void JoinSingleLineEntries(StringBuilder failureBuilder, int start)
	{
		if (failureBuilder.Length == start)
		{
			return;
		}

		string separator = $"{Environment.NewLine}and{Environment.NewLine}";
		string failures = failureBuilder.ToString(start, failureBuilder.Length - start);
		if (failures.Split([separator,], StringSplitOptions.None).Any(entry => entry.TrimStart().Contains('\n', StringComparison.Ordinal)))
		{
			return;
		}

		failureBuilder.Replace(separator, $" and{Environment.NewLine}", start, failureBuilder.Length - start);
	}

	private sealed class EquivalencyContext(EquivalencyOptions equivalencyOptions)
	{
		/// <summary>
		///     The number of nested comparisons a pair must have taken to be remembered as equivalent.
		/// </summary>
		/// <remarks>
		///     Comparing a smaller pair again costs at most that many comparisons for each reference to it, while
		///     remembering every pair would cost memory in each comparison, most of which never reach a pair twice.
		/// </remarks>
		private const int ComparisonsToRemember = 64;

		/// <summary>
		///     The number of compared member paths that are kept as candidates for a member to ignore that matched
		///     none.
		/// </summary>
		private const int ComparedPathsToRemember = 1000;

		private List<string>? _comparedPaths;
		private HashSet<string>? _matchedNames;
		private HashSet<string>? _namesToIgnore;
		private object? _pendingExpected;
		private List<ActiveMapping>? _pendingMappings;
		private MemberMapping[]? _trackedMappings;
		private bool _trackedMappingsHaveNames;
		private MemberToIgnore[]? _trackedMembersToIgnore;

		/// <summary>
		///     The pairs that are compared on the current path, with their <see cref="Depth" />, to catch recursions.
		/// </summary>
		/// <remarks>
		///     Created on first use, as values that are compared by value have no nested pairs.
		/// </remarks>
		private Dictionary<ComparedPair, int>? _comparedPairs;

		/// <summary>
		///     The number of nested comparisons that were started so far.
		/// </summary>
		private int _comparisons;

		/// <summary>
		///     The pairs that were found equivalent, with the deepest <see cref="Depth" /> they were found equivalent at.
		/// </summary>
		/// <remarks>
		///     A pair that is reached again via another path is equivalent there as well, when nothing its result was
		///     derived from can differ on that path. It is therefore only remembered<br />
		///     - when it was equivalent, as a difference is reported with the path it was found on,<br />
		///     - together with its type options, as the same pair can be reached with other options,<br />
		///     - when neither it nor anything nested in it has members to ignore, as they are matched by the path,<br />
		///     - when nothing nested in it was skipped as a recursion into a pair that encloses it, as it would
		///     otherwise be remembered as equivalent on the assumption that the enclosing pair is, and<br />
		///     - for the depths up to the one it was compared at, as it could exceed the maximum recursion depth when it
		///     is nested deeper.
		/// </remarks>
		private Dictionary<EquivalentPair, int>? _equivalentPairs;

		private EquivalencyTypeOptions? _lastInheritedOptions;

		private EquivalencyTypeOptions? _lastParentOptions;

		/// <summary>
		///     The <see cref="Depth" /> of the outermost pair on the current path that the result of the current pair
		///     was derived from, or <c>0</c> when it was derived from the path itself.
		/// </summary>
		private int _outermostDependency = int.MaxValue;

		private ConcurrentDictionary<Type, EquivalencyTypeOptions?>? _registeredOptions;

		/// <summary>
		///     The number of nested comparisons on the current path.
		/// </summary>
		/// <remarks>
		///     Counted per path and not globally, so that two members on the same level are both at the same depth.
		/// </remarks>
		public int Depth { get; private set; }

		/// <summary>
		///     The number of differences that were appended to a failure message so far.
		/// </summary>
		/// <remarks>
		///     Counted globally, so that a caller which is only interested in one comparison reads it before and after
		///     that comparison and restores it afterwards when the differences were written into a throwaway builder.
		/// </remarks>
		public int DifferenceCount { get; set; }

		/// <summary>
		///     Whether the comparison only decides whether the objects are equivalent, without writing or counting their
		///     differences.
		/// </summary>
		/// <remarks>
		///     Set for a comparison whose differences nobody reads, e.g. of the items of a collection, and while items
		///     whose order is ignored are paired, as most of the pairs that are tried are not equivalent and never
		///     reported, so formatting their values would be wasted.
		/// </remarks>
		public bool IsDecidingOnly { get; set; }

		/// <summary>
		///     Whether the comparison only counts the differences of the objects, without writing them.
		/// </summary>
		/// <remarks>
		///     Set while the leftovers of items whose order is ignored are ranked by their number of differences, as
		///     only the pairs that are reported in the end are written.
		/// </remarks>
		public bool IsCountingOnly { get; set; }

		/// <summary>
		///     Whether the comparison tracks which members to ignore by name matched a compared member, so that a
		///     failure can name the ones that matched none.
		/// </summary>
		/// <remarks>
		///     A misspelled name ignores nothing without any notice, so the difference it was meant to hide is reported
		///     without a reason.
		/// </remarks>
		public bool TracksIgnoredMembers { get; set; }

		/// <summary>
		///     The evaluation that the comparison is part of, or <see langword="null" /> outside an evaluation.
		/// </summary>
		public IEvaluationContext? Evaluation { get; set; }

		/// <summary>
		///     The token that cancels the <see cref="Evaluation" />.
		/// </summary>
		public CancellationToken CancellationToken { get; set; }

		/// <summary>
		///     The options registered for a type, or <see langword="null" /> when it has no registration.
		/// </summary>
		/// <remarks>
		///     Cached per <see cref="EquivalencyOptions" /> instance instead of per comparison, because resolving a
		///     registration invokes its callback, every compared pair looks up both of its types, and the items of a
		///     collection are compared one by one with the same options. Only read when the options have registrations.
		/// </remarks>
		public ConcurrentDictionary<Type, EquivalencyTypeOptions?> RegisteredOptions
			=> _registeredOptions ??= RegisteredOptionsCache.GetOrCreateValue(equivalencyOptions);

		/// <summary>
		///     Records that the <paramref name="memberToIgnore" /> out of the <paramref name="membersToIgnore" />
		///     matched a member.
		/// </summary>
		public void TrackIgnoredMember(MemberToIgnore[] membersToIgnore, MemberToIgnore memberToIgnore)
		{
			if (!TracksIgnoredMembers)
			{
				return;
			}

			TrackMembersToIgnore(membersToIgnore);
			if (memberToIgnore is MemberToIgnore.ByName byName)
			{
				(_matchedNames ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase)).Add(byName.MemberName);
			}
		}

		/// <summary>
		///     Records that the member at the <paramref name="memberPath" /> was compared, because none of the
		///     <paramref name="membersToIgnore" /> matched it.
		/// </summary>
		public void TrackComparedMember(MemberToIgnore[] membersToIgnore, string memberPath)
		{
			if (!TracksIgnoredMembers)
			{
				return;
			}

			TrackMembersToIgnore(membersToIgnore);
			TrackComparedPath(memberPath);
		}

		/// <summary>
		///     Records the expected members of the <paramref name="mappings" /> by name, so that a failure can name the
		///     ones that matched no member, and returns whether the compared member paths have to be recorded for them.
		/// </summary>
		/// <remarks>
		///     They share the names with the members to ignore, as both name a member by the end of its path.
		/// </remarks>
		public bool TrackMappings(MemberMapping[] mappings)
		{
			if (!TracksIgnoredMembers || mappings.Length == 0)
			{
				return false;
			}

			if (ReferenceEquals(mappings, _trackedMappings))
			{
				return _trackedMappingsHaveNames;
			}

			_trackedMappings = mappings;
			_trackedMappingsHaveNames = false;
			foreach (MemberMapping mapping in mappings)
			{
				if (mapping.ExpectedName is { } name)
				{
					(_namesToIgnore ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase)).Add(name);
					_trackedMappingsHaveNames = true;
				}
			}

			return _trackedMappingsHaveNames;
		}

		/// <summary>
		///     Records that the expected member of a mapping by the <paramref name="name" /> matched a member.
		/// </summary>
		public void TrackMatchedName(string name)
		{
			if (TracksIgnoredMembers)
			{
				(_matchedNames ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase)).Add(name);
			}
		}

		/// <summary>
		///     Records the <paramref name="memberPath" /> as a candidate for a name that matched no member.
		/// </summary>
		public void TrackComparedPath(string memberPath)
		{
			_comparedPaths ??= [];
			if (_comparedPaths.Count < ComparedPathsToRemember)
			{
				_comparedPaths.Add(memberPath);
			}
		}

		/// <summary>
		///     Hands the <paramref name="mappings" /> that run through a member to the nested comparison of its
		///     <paramref name="expected" /> value.
		/// </summary>
		/// <remarks>
		///     The nested comparison takes them before it compares anything else, so a single slot is enough.
		/// </remarks>
		public void SetPendingMappings(object? expected, List<ActiveMapping>? mappings)
		{
			_pendingExpected = mappings is null ? null : expected;
			_pendingMappings = mappings;
		}

		/// <summary>
		///     Takes the mappings that were handed to the comparison of the <paramref name="expected" /> value.
		/// </summary>
		public List<ActiveMapping>? TakePendingMappings(object expected)
		{
			if (_pendingMappings is null || !ReferenceEquals(_pendingExpected, expected))
			{
				return null;
			}

			List<ActiveMapping> mappings = _pendingMappings;
			SetPendingMappings(null, null);
			return mappings;
		}

		/// <summary>
		///     Adds each member to ignore by name that matched none of the compared members to the
		///     <paramref name="unmatched" /> ones, with the compared member it most likely meant.
		/// </summary>
		/// <remarks>
		///     A name that already has a suggestion keeps it, so the closest member is only searched until one is
		///     found, however many items of a collection fail.
		/// </remarks>
		public Dictionary<string, string?>? AddUnmatchedMembersToIgnore(Dictionary<string, string?>? unmatched)
		{
			foreach (string name in _namesToIgnore ?? [])
			{
				if (_matchedNames?.Contains(name) == true)
				{
					continue;
				}

				unmatched ??= new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
				if (unmatched.TryGetValue(name, out string? suggestion) && suggestion is not null)
				{
					continue;
				}

				int segments = name.Count(c => c == '.') + 1;
				unmatched[name] = ClosestMatch.Find(name,
					(_comparedPaths ?? []).Select(path => GetLastSegments(path, segments)).Distinct());
			}

			return unmatched;
		}

		/// <remarks>
		///     The members to ignore of a type are the same array for each of its members, so they are only collected
		///     when another type is compared.
		/// </remarks>
		private void TrackMembersToIgnore(MemberToIgnore[] membersToIgnore)
		{
			if (ReferenceEquals(membersToIgnore, _trackedMembersToIgnore))
			{
				return;
			}

			_trackedMembersToIgnore = membersToIgnore;
			foreach (MemberToIgnore memberToIgnore in membersToIgnore)
			{
				if (memberToIgnore is MemberToIgnore.ByName byName)
				{
					(_namesToIgnore ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase)).Add(byName.MemberName);
				}
			}
		}

		private static string GetLastSegments(string memberPath, int segments)
		{
			int start = memberPath.Length;
			for (int i = 0; i < segments && start > 0; i++)
			{
				start = memberPath.LastIndexOf('.', start - 1);
				if (start < 0)
				{
					return memberPath;
				}
			}

			return memberPath.Substring(start + 1);
		}

		/// <summary>
		///     Starts the nested comparison of the <paramref name="pair" />, unless it is equivalent without comparing
		///     it: because it is already compared on the current path, or because it was found equivalent before.
		/// </summary>
		/// <remarks>
		///     A pair with <paramref name="hasPendingMappings" /> reads members of an enclosing actual object, so its
		///     result depends on the path, like with members to ignore.
		/// </remarks>
		public bool TryEnter(ComparedPair pair, EquivalencyTypeOptions typeOptions, bool hasPendingMappings,
			out ComparisonScope scope)
		{
			scope = default;
			if (!hasPendingMappings &&
			    _equivalentPairs is not null &&
			    _equivalentPairs.TryGetValue(new EquivalentPair(pair, typeOptions), out int equivalentDepth) &&
			    Depth < equivalentDepth)
			{
				return false;
			}

			_comparedPairs ??= [];
#if NET8_0_OR_GREATER
			if (!_comparedPairs.TryAdd(pair, Depth + 1))
			{
				_outermostDependency = Math.Min(_outermostDependency, _comparedPairs[pair]);
				return false;
			}
#else
			if (_comparedPairs.TryGetValue(pair, out int depth))
			{
				_outermostDependency = Math.Min(_outermostDependency, depth);
				return false;
			}

			_comparedPairs.Add(pair, Depth + 1);
#endif
			Depth++;
			scope = new ComparisonScope(++_comparisons, _outermostDependency);
			_outermostDependency = typeOptions.MembersToIgnore.Length > 0 || hasPendingMappings ? 0 : int.MaxValue;
			return true;
		}

		/// <summary>
		///     Ends the nested comparison of the <paramref name="pair" /> that was started with
		///     <see cref="TryEnter" />.
		/// </summary>
		public void Leave(ComparedPair pair, EquivalencyTypeOptions typeOptions, ComparisonScope scope,
			bool isEquivalent)
		{
			bool dependsOnPath = _outermostDependency < Depth;
			if (isEquivalent && !dependsOnPath && _comparisons - scope.Comparison >= ComparisonsToRemember)
			{
				(_equivalentPairs ??= [])[new EquivalentPair(pair, typeOptions)] = Depth;
			}

			_outermostDependency = dependsOnPath
				? Math.Min(scope.OutermostDependency, _outermostDependency)
				: scope.OutermostDependency;
			_comparedPairs!.Remove(pair);
			Depth--;
		}

		/// <summary>
		///     Returns the options that a type without a registration inherits from the
		///     <paramref name="parentOptions" />.
		/// </summary>
		/// <remarks>
		///     The members of one object share their parent options, so the inherited copy is kept for the last parent
		///     instead of being created again for every member.
		/// </remarks>
		public EquivalencyTypeOptions GetInheritedOptions(EquivalencyOptions equivalencyOptions,
			EquivalencyTypeOptions parentOptions)
		{
			if (!ReferenceEquals(parentOptions, _lastParentOptions))
			{
				_lastParentOptions = parentOptions;
				_lastInheritedOptions = equivalencyOptions.GetInheritedOptions(parentOptions);
			}

			return _lastInheritedOptions!;
		}
	}

	/// <summary>
	///     The path of a compared value, which is only joined into a <see langword="string" /> when it is read.
	/// </summary>
	/// <remarks>
	///     Most members and items are compared by value and are equal, so nothing ever reads their path: only ignore
	///     rules, failures, code of the caller that throws and the members of a nested object do.
	/// </remarks>
	private readonly struct MemberPath
	{
		private readonly int _index;
		private readonly string? _name;
		private readonly string _parent;

		private MemberPath(string parent, string? name, int index)
		{
			_parent = parent;
			_name = name;
			_index = index;
		}

		public static implicit operator MemberPath(string path)
		{
			return new MemberPath(path, null, -1);
		}

		/// <summary>
		///     The path of the member <paramref name="name" /> of the value at the <paramref name="parent" /> path.
		/// </summary>
		public static MemberPath Member(string parent, string name) => new(parent, name, -1);

		/// <summary>
		///     The path of the element at the <paramref name="index" /> of the sequence at the <paramref name="parent" />
		///     path.
		/// </summary>
		public static MemberPath Element(string parent, int index) => new(parent, null, index);

		public override string ToString()
		{
			if (_name is not null)
			{
				return _parent.Length == 0 ? _name : $"{_parent}.{_name}";
			}

			return _index >= 0 ? $"{_parent}[{_index}]" : _parent;
		}
	}

	private readonly struct ComparedPair : IEquatable<ComparedPair>
	{
		private readonly object _actual;
		private readonly object _expected;

		public ComparedPair(object actual, object expected)
		{
			_actual = actual;
			_expected = expected;
		}

		public bool Equals(ComparedPair other)
			=> ReferenceEquals(_actual, other._actual) && ReferenceEquals(_expected, other._expected);

		public override bool Equals(object? obj) => obj is ComparedPair other && Equals(other);

		public override int GetHashCode()
			=> (RuntimeHelpers.GetHashCode(_actual) * 397) ^ RuntimeHelpers.GetHashCode(_expected);
	}

	/// <summary>
	///     A <see cref="ComparedPair" /> together with the type options it was compared with.
	/// </summary>
	private readonly struct EquivalentPair : IEquatable<EquivalentPair>
	{
		private readonly ComparedPair _pair;
		private readonly EquivalencyTypeOptions _typeOptions;

		public EquivalentPair(ComparedPair pair, EquivalencyTypeOptions typeOptions)
		{
			_pair = pair;
			_typeOptions = typeOptions;
		}

		public bool Equals(EquivalentPair other)
			=> _pair.Equals(other._pair) && _typeOptions.Equals(other._typeOptions);

		public override bool Equals(object? obj) => obj is EquivalentPair other && Equals(other);

		/// <remarks>
		///     Leaves out the type options, as a pair is rarely compared with more than one of them.
		/// </remarks>
		public override int GetHashCode() => _pair.GetHashCode();
	}

	/// <summary>
	///     The state of the enclosing comparison that a nested comparison restores when it ends.
	/// </summary>
	private readonly struct ComparisonScope
	{
		public ComparisonScope(int comparison, int outermostDependency)
		{
			Comparison = comparison;
			OutermostDependency = outermostDependency;
		}

		/// <summary>
		///     The number of nested comparisons that were started up to and including this one.
		/// </summary>
		public int Comparison { get; }

		/// <summary>
		///     The outermost dependency of the enclosing comparison when this one was started.
		/// </summary>
		public int OutermostDependency { get; }
	}
}
