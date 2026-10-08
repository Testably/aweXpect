using System;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Helpers;

namespace aweXpect.Core;

/// <summary>
///     A result context that is appended to a result error.
/// </summary>
public abstract class ResultContext
{
	/// <summary>
	///     A result context that is appended to a result error.
	/// </summary>
	/// <remarks>The optional <paramref name="priority" /> determines the displayed order (higher values are displayed first).</remarks>
	protected ResultContext(string title, int priority = 0)
	{
		Title = title;
		Priority = priority;
	}

	/// <summary>
	///     The title of the context.
	/// </summary>
	public string Title { get; }

	/// <summary>
	///     The priority of the context (determines the displayed order).
	/// </summary>
	/// <remarks>The higher values are displayed first.</remarks>
	public int Priority { get; }

	/// <summary>
	///     The content of the context.
	/// </summary>
	public abstract Task<string?> GetContent(CancellationToken cancellationToken = default);

	/// <summary>
	///     The content of the context, or <see langword="null" /> when code of the caller throws while it is created or
	///     when it is not available.
	/// </summary>
	/// <remarks>
	///     The exception already failed the expectation, e.g. when a context lists the items of a subject whose
	///     enumeration threw, so it must not abort the failure message.<br />
	///     A content that is still pending when the <paramref name="cancellationToken" /> is canceled is abandoned. It is
	///     not available then, like a content whose creation is canceled, because the failure of the expectation must
	///     be reported nevertheless.
	/// </remarks>
	internal async Task<string?> GetContentUnlessUserCodeThrows(CancellationToken cancellationToken)
	{
		try
		{
			return await GetContent(cancellationToken).AbandonOnCancellation(cancellationToken);
		}
		catch (UserCodeException)
		{
			return null;
		}
		catch (OperationCanceledException)
		{
			return null;
		}
	}

	/// <summary>
	///     A <see cref="ResultContext" /> from a fixed <see langword="string" /> content.
	/// </summary>
	public class Fixed : ResultContext
	{
		private readonly string? _content;

		/// <summary>
		///     A <see cref="ResultContext" /> from a fixed <see langword="string" /> <paramref name="content" />.
		/// </summary>
		/// <remarks>The optional <paramref name="priority" /> determines the displayed order (higher values are displayed first).</remarks>
		public Fixed(string title, string? content, int priority = 0) : base(title, priority)
		{
			_content = content;
		}

		/// <inheritdoc cref="ResultContext.GetContent(CancellationToken)" />
		public override Task<string?> GetContent(CancellationToken cancellationToken = default)
			=> Task.FromResult(_content);
	}

	/// <summary>
	///     A <see cref="ResultContext" /> from an async callback.
	/// </summary>
	/// <remarks>
	///     The callback receives a token that is canceled when the timeout of the expectation elapses or the evaluation
	///     is canceled. A callback that is still pending then is not awaited any longer, and the failure message is
	///     created without the context, like for a callback that throws an <see cref="OperationCanceledException" />.
	/// </remarks>
	public class AsyncCallback : ResultContext
	{
		private readonly Func<CancellationToken, Task<string?>> _callback;

		/// <summary>
		///     A <see cref="ResultContext" /> from an async <paramref name="callback" />.
		/// </summary>
		/// <remarks>The optional <paramref name="priority" /> determines the displayed order (higher values are displayed first).</remarks>
		public AsyncCallback(string title, Func<CancellationToken, Task<string?>> callback, int priority = 0) : base(
			title, priority)
		{
			_callback = callback;
		}

		/// <inheritdoc cref="ResultContext.GetContent(CancellationToken)" />
		public override Task<string?> GetContent(CancellationToken cancellationToken = default)
			=> _callback(cancellationToken);
	}

	/// <summary>
	///     A <see cref="ResultContext" /> from a sync callback.
	/// </summary>
	public class SyncCallback : ResultContext
	{
		private readonly Func<string?> _callback;

		/// <summary>
		///     A <see cref="ResultContext" /> from a sync <paramref name="callback" />.
		/// </summary>
		/// <remarks>The optional <paramref name="priority" /> determines the displayed order (higher values are displayed first).</remarks>
		public SyncCallback(string title, Func<string?> callback, int priority = 0) : base(title, priority)
		{
			_callback = callback;
		}

		/// <inheritdoc cref="ResultContext.GetContent(CancellationToken)" />
		public override Task<string?> GetContent(CancellationToken cancellationToken = default)
			=> Task.FromResult(_callback());
	}
}
