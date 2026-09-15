using System;
using System.Collections;
using System.Collections.Generic;

namespace aweXpect.Core;

/// <summary>
///     The list of <see cref="ResultContext" /> that is appended to the failure message.
/// </summary>
public class ResultContexts : IEnumerable<ResultContext>
{
	private readonly List<ResultContext> _results = new();
	private bool _isOpen = true;

	/// <inheritdoc cref="IEnumerable{ResultContext}.GetEnumerator()" />
	public IEnumerator<ResultContext> GetEnumerator()
		=> _results.GetEnumerator();

	/// <inheritdoc cref="IEnumerable.GetEnumerator()" />
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	/// <summary>
	///     Closes the context list for further modifications.
	/// </summary>
	public ResultContexts Close()
	{
		_isOpen = false;
		return this;
	}

	/// <summary>
	///     Opens the context list for further modifications.
	/// </summary>
	public ResultContexts Open()
	{
		_isOpen = true;
		return this;
	}

	/// <summary>
	///     Adds the <paramref name="context" /> to the context list, unless it is already contained in it.
	/// </summary>
	/// <remarks>
	///     A constraint adds its context while it is evaluated, so an expectation that inspects the same property
	///     twice (<c>HasMessage().Containing("a").And.HasMessage().Containing("b")</c>) would otherwise repeat the
	///     identical block. Only a <see cref="ResultContext.Fixed" /> can be recognized as a duplicate without
	///     evaluating it, so only those are suppressed.
	/// </remarks>
	public ResultContexts Add(ResultContext context)
	{
		if (_isOpen && !_results.Exists(existing => IsDuplicate(existing, context)))
		{
			_results.Add(context);
		}

		return this;
	}

	/// <summary>
	///     Removes all contexts from the context list.
	/// </summary>
	public ResultContexts Clear()
	{
		if (_isOpen)
		{
			_results.Clear();
		}

		return this;
	}

	/// <summary>
	///     Removes all contexts with the given <paramref name="title" />.
	/// </summary>
	public ResultContexts Remove(string title, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
		=> Remove(c => string.Equals(c.Title, title, stringComparison));

	/// <summary>
	///     Removes all contexts that match the <paramref name="predicate" />.
	/// </summary>
	public ResultContexts Remove(Predicate<ResultContext> predicate)
	{
		if (_isOpen)
		{
			_results.RemoveAll(predicate);
		}

		return this;
	}

	private static bool IsDuplicate(ResultContext existing, ResultContext context)
		=> existing is ResultContext.Fixed existingFixed &&
		   context is ResultContext.Fixed addedFixed &&
		   string.Equals(existingFixed.Title, addedFixed.Title, StringComparison.Ordinal) &&
		   string.Equals(existingFixed.Content, addedFixed.Content, StringComparison.Ordinal);
}
