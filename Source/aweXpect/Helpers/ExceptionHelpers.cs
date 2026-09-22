using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using aweXpect.Core;

namespace aweXpect.Helpers;

internal static class ExceptionHelpers
{
	public static void ThrowIfNull(this object? parameter,
		[CallerArgumentExpression(nameof(parameter))] string? paramName = null)
		=> ThrowIfNullNamed(parameter, paramName);

	/// <summary>
	///     Throws when the <paramref name="parameter" /> is null, naming it after the polarity of the expectation: the
	///     expected value, or the unexpected one when <paramref name="negated" />.
	/// </summary>
	public static void ThrowIfNull(this object? parameter, bool negated)
		=> ThrowIfNullNamed(parameter, negated ? "unexpected" : "expected");

	private static void ThrowIfNullNamed(object? parameter, string? paramName)
	{
		if (parameter is null)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentNullException(paramName, $"The {paramName} cannot be null."));
		}
	}

	public static void ThrowIfNullOrEmpty<T>(this IEnumerable<T>? parameter,
		[CallerArgumentExpression(nameof(parameter))] string? paramName = null)
		=> ThrowIfNullOrEmptyNamed(parameter, paramName);

	/// <summary>
	///     Throws when the <paramref name="parameter" /> is null or empty, naming it after the polarity of the
	///     expectation: the expected collection, or the unexpected one when <paramref name="negated" />.
	/// </summary>
	public static void ThrowIfNullOrEmpty<T>(this IEnumerable<T>? parameter, bool negated)
		=> ThrowIfNullOrEmptyNamed(parameter, negated ? "unexpected" : "expected");

	private static void ThrowIfNullOrEmptyNamed<T>(IEnumerable<T>? parameter, string? paramName)
	{
		parameter.ThrowIfNull(paramName);
		if (!parameter!.Any())
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentException($"The '{paramName}' collection cannot be empty.", paramName));
		}
	}

	public static string FormatForMessage(this Exception exception, string? indentation, string relation = "")
	{
		string message = (relation + Formatter.Format(exception.GetType())).PrependAOrAn();
		if (!string.IsNullOrEmpty(exception.Message))
		{
			message += ":" + Environment.NewLine + exception.Message.Indent(indentation + "  ");
		}

		return message;
	}

	public static IEnumerable<Exception> GetInnerExceptions(this Exception? actual)
		=> new RecursiveInnerExceptions(EnumerateInnerExceptions(actual));

	private static IEnumerable<Exception> EnumerateInnerExceptions(Exception? actual)
	{
		switch (actual)
		{
			case AggregateException aggregateException:
				{
					foreach (Exception innerException in aggregateException.InnerExceptions)
					{
						yield return innerException;
						foreach (Exception inner in EnumerateInnerExceptions(innerException))
						{
							yield return inner;
						}
					}

					break;
				}
			default:
				{
					if (actual?.InnerException is not null)
					{
						yield return actual.InnerException;
						foreach (Exception inner in EnumerateInnerExceptions(actual.InnerException))
						{
							yield return inner;
						}
					}

					break;
				}
		}
	}

	/// <remarks>
	///     An exception without any inner exception must not satisfy an expectation on its inner exceptions
	///     just by having none, so the collection is marked as <see cref="IRejectVacuousSuccess" />.
	/// </remarks>
	private sealed class RecursiveInnerExceptions(IEnumerable<Exception> innerExceptions)
		: IEnumerable<Exception>, IRejectVacuousSuccess
	{
		public IEnumerator<Exception> GetEnumerator() => innerExceptions.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
