using System;
using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Options;

/// <summary>
///     Options for a namespace-based type dependency assertion or filter.
/// </summary>
/// <remarks>
///     An immutable value: <c>OrOn(…)</c> and <c>ExcludingSubNamespaces(…)</c> return new options that widen or refine
///     the (lazily evaluated) expression. An expectation shares them in an <see cref="OptionsHolder{TOptions}" /> with
///     its result and its constraint, a filter keeps them.
/// </remarks>
internal sealed class NamespaceDependencyOptions : IDependencyTarget
{
	private readonly bool _excludeOwnSubNamespaces;
	private readonly string[] _namespaces;

	public NamespaceDependencyOptions(IEnumerable<string> namespaces)
		: this(Validate(namespaces), false, false)
	{
	}

	private NamespaceDependencyOptions(string[] namespaces, bool excludeSubNamespaces,
		bool excludeOwnSubNamespaces)
	{
		_namespaces = namespaces;
		ExcludeSubNamespaces = excludeSubNamespaces;
		_excludeOwnSubNamespaces = excludeOwnSubNamespaces;
	}

	/// <summary>
	///     The namespaces that are targeted (for depends-on / does-not-depend-on) or allowed (for depends-only-on).
	/// </summary>
	public IReadOnlyList<string> Namespaces => _namespaces;

	/// <summary>
	///     Indicates whether sub-namespaces are excluded from matching.
	/// </summary>
	public bool ExcludeSubNamespaces { get; }

	/// <summary>
	///     Indicates whether sub-namespaces of the type's own namespace are still allowed (for depends-only-on
	///     and has-dependencies-outside).
	/// </summary>
	/// <remarks>
	///     The type's own namespace is always allowed; its sub-namespaces stay allowed unless the caller opted into
	///     <see cref="ExcludingOwnSubNamespaces" />.
	/// </remarks>
	public bool IncludeOwnSubNamespaces => !_excludeOwnSubNamespaces;

	/// <summary>
	///     Returns options that widen the set of targeted/allowed namespaces by the given <paramref name="namespaces" />.
	/// </summary>
	public NamespaceDependencyOptions OrOn(IEnumerable<string> namespaces)
		=> new([.. _namespaces, .. Validate(namespaces),], ExcludeSubNamespaces, _excludeOwnSubNamespaces);

	/// <remarks>
	///     The constructor and <see cref="OrOn" /> validate through this method, so that the validation cannot diverge
	///     between the two entry points.
	/// </remarks>
	private static string[] Validate(IEnumerable<string> namespaces)
	{
		ThrowHelper.ThrowIfNull(namespaces, nameof(namespaces));
		List<string> added = namespaces.ToList();
		if (added.Count == 0)
		{
			throw Tracing.WriteException(new ArgumentException(
				$"The '{nameof(namespaces)}' collection cannot be empty.", nameof(namespaces)));
		}

		if (added.Contains(null!))
		{
			throw Tracing.WriteException(new ArgumentException(
				$"The '{nameof(namespaces)}' collection cannot contain <null>.", nameof(namespaces)));
		}

		ThrowOnUnmatchableNamespace(added);
		return [.. added,];
	}

	/// <summary>
	///     A namespace that could never match any real namespace would make negative assertions silently pass,
	///     so it is rejected upfront: real namespaces do not end with a dot (the sub-namespace boundary check
	///     expects the dot after the given namespace, and the trailing-dot convention belongs to the excluded
	///     assembly prefixes), nor do they contain empty segments or whitespace. The empty string stays allowed
	///     and targets exactly the global namespace.
	/// </summary>
	private static void ThrowOnUnmatchableNamespace(IEnumerable<string> namespaces)
	{
		foreach (string @namespace in namespaces)
		{
			if (@namespace.Length == 0)
			{
				continue;
			}

			if (@namespace.EndsWith('.'))
			{
				throw Tracing.WriteException(new ArgumentException(
					"The namespaces must not end with a dot (sub-namespaces are matched automatically)."));
			}

			if (@namespace[0] == '.' ||
			    @namespace.Contains("..") ||
			    @namespace.Any(char.IsWhiteSpace))
			{
				throw Tracing.WriteException(new ArgumentException(
					"The namespaces must not contain empty segments or whitespace (such a namespace could never match)."));
			}
		}
	}

	/// <summary>
	///     Returns options that exclude sub-namespaces of the targeted/allowed namespaces from matching for the whole
	///     expression.
	/// </summary>
	public NamespaceDependencyOptions ExcludingSubNamespaces()
		=> new(_namespaces, true, _excludeOwnSubNamespaces);

	/// <summary>
	///     Returns options that exclude sub-namespaces of the type's own namespace from being implicitly allowed (for
	///     depends-only-on and has-dependencies-outside).
	/// </summary>
	public NamespaceDependencyOptions ExcludingOwnSubNamespaces()
		=> new(_namespaces, ExcludeSubNamespaces, true);

	/// <summary>
	///     Checks whether the <paramref name="dependencyNamespace" /> matches any of the configured namespaces.
	/// </summary>
	public bool Matches(string? dependencyNamespace)
		=> _namespaces.Any(@namespace
			=> TypeHelpers.NamespaceMatches(dependencyNamespace, @namespace, !ExcludeSubNamespaces));

	/// <summary>
	///     Checks whether the <paramref name="type" /> has at least one dependency in a configured namespace.
	/// </summary>
	public bool IsMatchedBy(Type type)
		=> type.ResolveDependencies().Any(dependency => Matches(dependency.Namespace));

	/// <summary>
	///     Describes the configured namespaces for an expectation message.
	/// </summary>
	/// <remarks>
	///     The constructor guarantees at least one namespace, so an empty set cannot occur here.
	/// </remarks>
	public string Describe()
		=> $"namespace {string.Join(" or ", _namespaces.Select(@namespace => Formatter.Format(@namespace)))}";
}
