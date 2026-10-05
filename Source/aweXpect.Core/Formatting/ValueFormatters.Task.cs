using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Equivalency;

namespace aweXpect.Formatting;

public static partial class ValueFormatters
{
	/// <summary>
	///     Appends a task as its type and status, and a <see cref="CancellationToken" /> as its state.
	/// </summary>
	/// <remarks>
	///     A task has no rendering of its own, and its members include the <c>Result</c>, which blocks until the task
	///     completes, so the result is only read from a task that ran to completion. The only member of a
	///     <see cref="CancellationToken" /> besides its state is a wait handle that is allocated when it is read.
	/// </remarks>
	private static bool TryFormatTask(StringBuilder stringBuilder, object value, FormattingOptions? options,
		FormattingContext? context)
	{
		switch (value)
		{
			case Task task:
				FormatTask(stringBuilder, task, options, context);
				return true;
			case ValueTask valueTask:
				stringBuilder.Append("ValueTask (").Append(GetValueTaskStatus(valueTask.IsCompletedSuccessfully,
					valueTask.IsFaulted, valueTask.IsCanceled)).Append(')');
				return true;
			case CancellationToken cancellationToken:
				stringBuilder.Append(cancellationToken.IsCancellationRequested
					? "CancellationToken (canceled)"
					: "CancellationToken (not canceled)");
				return true;
			default:
				return TryFormatValueTaskWithResult(stringBuilder, value, options, context);
		}
	}

	private static void FormatTask(StringBuilder stringBuilder, Task task, FormattingOptions? options,
		FormattingContext? context)
	{
		Type? typeWithResult = GetTaskTypeWithResult(task.GetType());
		TaskStatus status = task.Status;
		FormatType(typeWithResult ?? typeof(Task), stringBuilder);
		stringBuilder.Append(" (").Append(status);
		if (status == TaskStatus.Faulted && task.Exception?.InnerException is { } exception)
		{
			stringBuilder.Append(", ");
			Format(Formatter, stringBuilder, exception, options);
		}
		else if (status == TaskStatus.RanToCompletion && typeWithResult is not null)
		{
			AppendTaskResult(stringBuilder, task, typeWithResult, options, context);
		}

		stringBuilder.Append(')');
	}

	/// <summary>
	///     Returns the <see cref="Task{TResult}" /> that the <paramref name="type" /> is or derives from, or
	///     <see langword="null" /> when the task has no result.
	/// </summary>
	/// <remarks>
	///     The runtime type of a task is often a derived one, like the state machine box of an async method, and a task
	///     without a result can still be a <see cref="Task{TResult}" /> of the internal <c>VoidTaskResult</c>.
	/// </remarks>
	private static Type? GetTaskTypeWithResult(Type? type)
	{
		while (type is not null && !(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>)))
		{
			type = type.BaseType;
		}

		return type?.GenericTypeArguments[0].FullName == "System.Threading.Tasks.VoidTaskResult" ? null : type;
	}

	/// <remarks>
	///     A <see cref="ValueTask{TResult}" /> that reaches the formatter boxed has lost its type argument, so its state
	///     is read like any other member; without its members it keeps the plain object rendering. Only a task knows the
	///     exception it failed with, so a faulted <see cref="ValueTask{TResult}" /> is written without it.
	/// </remarks>
	private static bool TryFormatValueTaskWithResult(StringBuilder stringBuilder, object value,
		FormattingOptions? options, FormattingContext? context)
	{
		Type type = value.GetType();
		if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(ValueTask<>) ||
		    !(ReflectionFallback.IsSupported || EquivalencyMembers.IsRegistered(type)))
		{
			return false;
		}

		bool isCompletedSuccessfully = IsTrue(value, type, nameof(ValueTask<object>.IsCompletedSuccessfully));
		FormatType(type, stringBuilder);
		stringBuilder.Append(" (").Append(GetValueTaskStatus(isCompletedSuccessfully,
			IsTrue(value, type, nameof(ValueTask<object>.IsFaulted)),
			IsTrue(value, type, nameof(ValueTask<object>.IsCanceled))));
		if (isCompletedSuccessfully)
		{
			AppendTaskResult(stringBuilder, value, type, options, context);
		}

		stringBuilder.Append(')');
		return true;
	}

	private static bool IsTrue(object value, Type type, string propertyName)
		=> EquivalencyMembers.FindProperty(type, propertyName, IncludeMembers.Public)?.Invoke(value) is true;

	/// <remarks>
	///     A value task has no status, so its state is named like the matching status of a task.
	/// </remarks>
	private static string GetValueTaskStatus(bool isCompletedSuccessfully, bool isFaulted, bool isCanceled)
	{
		if (isCompletedSuccessfully)
		{
			return nameof(TaskStatus.RanToCompletion);
		}

		if (isFaulted)
		{
			return nameof(TaskStatus.Faulted);
		}

		return isCanceled ? nameof(TaskStatus.Canceled) : "Pending";
	}

	/// <remarks>
	///     The <paramref name="task" /> is tracked while its result is written, because the result can refer to the
	///     task again.
	/// </remarks>
	private static void AppendTaskResult(StringBuilder stringBuilder, object task, Type type,
		FormattingOptions? options, FormattingContext? context)
	{
		if (!(ReflectionFallback.IsSupported || EquivalencyMembers.IsRegistered(type)) ||
		    EquivalencyMembers.FindProperty(type, nameof(Task<object>.Result), IncludeMembers.Public) is not
			    { } getResult)
		{
			return;
		}

		stringBuilder.Append(", ");
		context ??= new FormattingContext();
		if (!context.FormattedObjects.Add(task))
		{
			stringBuilder.Append("*recursive*");
			return;
		}

		try
		{
			if (!EnterContent(context))
			{
				stringBuilder.Append('\u2026');
				return;
			}

			object? result = getResult(task);
			Format(Formatter, stringBuilder, result, WithoutLineBreaksForString(result, options), context);
		}
		finally
		{
			context.Depth--;
			context.FormattedObjects.Remove(task);
		}
	}
}
