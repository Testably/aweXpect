using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Core.Helpers;
using aweXpect.Core.Sources;
using aweXpect.Results;

namespace aweXpect.Delegates;

public abstract partial class ThatDelegate
{
	public sealed partial class WithValue<T>
	{
		/// <remarks>
		///     Only offered through <see cref="ThatDelegateExtensions.IsCollectedAfter{T}" />, which restricts the
		///     returned object to a reference type, as a boxed value is always collectable.
		/// </remarks>
		internal ExpectationResult IsCollectedAfterCore(Action<T> cleanup, string cleanupExpression)
		{
			cleanup.ThrowIfNull();
			ExpectationBuilder expectationBuilder = new ExpectationBuilder<DelegateValue<CleanedUpObject>>(
				CreateCleanedUpObjectSource(cleanup), _expectationBuilder.Subject);
			return new ExpectationResult(expectationBuilder.AddConstraint(cleanupExpression.TrimCommonWhiteSpace(),
				static (expression, it, grammars)
					=> new IsCollectedAfterConstraint(it, grammars, expression)));
		}

		/// <remarks>
		///     The usual delegate source keeps the returned object for <c>Which</c>, so the object would stay alive.
		/// </remarks>
		private IValueSource<DelegateValue<CleanedUpObject>> CreateCleanedUpObjectSource(Action<T> cleanup)
		{
			if (_synchronousSubject is { } factory)
			{
				return new DelegateValueSource<CleanedUpObject>(() => CreateAndCleanUp(factory, cleanup));
			}

			if (_subject is { } asyncFactory)
			{
				return new DelegateAsyncValueSource<CleanedUpObject>(token
					=> CreateAndCleanUp(asyncFactory, cleanup, token)!);
			}

			return new DelegateValueSource<CleanedUpObject>((Func<CleanedUpObject>?)null);
		}

		/// <remarks>
		///     The object only lives in the frame of this method, which is gone when the collection is forced.
		/// </remarks>
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static CleanedUpObject CreateAndCleanUp(Func<T> factory, Action<T> cleanup)
			=> CleanUp(factory(), cleanup);

		private static Task<CleanedUpObject>? CreateAndCleanUp(Func<CancellationToken, Task<T>> factory,
			Action<T> cleanup, CancellationToken cancellationToken)
		{
			Task<T>? task = factory(cancellationToken);
			return task is null ? null : CleanUpAsync(task, cleanup);
		}

		/// <remarks>
		///     A completing task runs its continuations on the stack of the factory, whose frames may still hold the
		///     object, so the cleanup continues on a new stack after <see cref="Task.Yield" />.
		/// </remarks>
		private static async Task<CleanedUpObject> CleanUpAsync(Task<T>? task, Action<T> cleanup)
		{
			await (Task)task!;
			await Task.Yield();
			return TakeResultAndCleanUp(ref task, cleanup);
		}

		/// <remarks>
		///     Clears the <paramref name="task" />, because the state machine of the caller, which holds it, is still
		///     running when the collection is forced.
		/// </remarks>
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static CleanedUpObject TakeResultAndCleanUp(ref Task<T>? task, Action<T> cleanup)
		{
			T value = task!.Result;
			task = null;
			return CleanUp(value, cleanup);
		}

		private static CleanedUpObject CleanUp(T value, Action<T> cleanup)
		{
			if (value is null)
			{
				return new CleanedUpObject(null, null);
			}

			try
			{
				cleanup(value);
			}
			catch (Exception exception)
			{
				return new CleanedUpObject(null, exception);
			}

			return new CleanedUpObject(new WeakReference(value), null);
		}
	}

	/// <summary>
	///     The weak <see cref="Reference" /> to the object returned by the delegate, which is <see langword="null" />
	///     when it returned <see langword="null" /> or the cleanup threw the <see cref="CleanupException" />.
	/// </summary>
	private sealed class CleanedUpObject(WeakReference? reference, Exception? cleanupException)
	{
		public WeakReference? Reference { get; } = reference;
		public Exception? CleanupException { get; } = cleanupException;
	}

	private sealed class IsCollectedAfterConstraint(
		string it,
		ExpectationGrammars grammars,
		string cleanupExpression)
		: ConstraintResult(grammars),
			IValueConstraint<DelegateValue<CleanedUpObject>>
	{
		private const int MaxCollectionAttempts = 3;
		private DelegateValue<CleanedUpObject>? _actual;

		/// <inheritdoc cref="ConstraintResult.FailureCause" />
		public override Exception? FailureCause
			=> _actual?.Exception ?? _actual?.Value?.CleanupException;

		/// <inheritdoc />
		public ConstraintResult IsMetBy(DelegateValue<CleanedUpObject> value)
		{
			_actual = value;
			Outcome = value.Value?.Reference is { } reference && IsCollected(reference)
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		/// <remarks>
		///     Each attempt waits for the pending finalizers, which can release further objects.
		/// </remarks>
		private static bool IsCollected(WeakReference reference)
		{
			for (int attempt = 0; attempt < MaxCollectionAttempts && reference.IsAlive; attempt++)
			{
				GC.Collect();
				GC.WaitForPendingFinalizers();
				GC.Collect();
			}

			return !reference.IsAlive;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is collected after ").Append(cleanupExpression);

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_actual?.IsNull != false)
			{
				AppendNullResult(stringBuilder, it, _actual);
			}
			else if (_actual.ExceededTimeout is { } exceededTimeout)
			{
				stringBuilder.ItDidNotFinishWithin(it, exceededTimeout);
			}
			else if (_actual.Exception is { } exception)
			{
				stringBuilder.Append(it).Append(" did throw ").Append(FormatForMessage(exception, indentation));
			}
			else if (_actual.Value?.CleanupException is { } cleanupException)
			{
				stringBuilder.Append("the cleanup did throw ").Append(FormatForMessage(cleanupException, indentation));
			}
			else if (_actual.Value?.Reference is null)
			{
				stringBuilder.Append(it).Append(" returned <null>");
			}
			else
			{
				stringBuilder.Append(it).Append(" was still alive");
			}
		}

		public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
		{
			value = default;
			return false;
		}

		public override ConstraintResult Negate()
			=> throw Tracing.WriteException(
				new NotSupportedException($"Negation of {nameof(ThatDelegateExtensions.IsCollectedAfter)} is not supported."));
	}
}
