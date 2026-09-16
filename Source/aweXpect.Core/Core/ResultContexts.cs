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
	///     Adds the <paramref name="context" /> to the context list.
	/// </summary>
	public ResultContexts Add(ResultContext context)
	{
		if (_isOpen)
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
}
