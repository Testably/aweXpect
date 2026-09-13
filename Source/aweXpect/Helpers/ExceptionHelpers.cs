using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace aweXpect.Helpers;

internal static class ExceptionHelpers
{
	public static void ThrowIfNull(this object? parameter,
		[CallerArgumentExpression(nameof(parameter))] string? paramName = null)
	{
		if (parameter is null)
		{
			// ReSharper disable once LocalizableElement
			throw new ArgumentNullException(paramName, $"The {paramName} cannot be null.");
		}
	}

	public static void ThrowIfNullOrEmpty<T>(this IEnumerable<T>? parameter,
		[CallerArgumentExpression(nameof(parameter))] string? paramName = null)
	{
		parameter.ThrowIfNull(paramName);
		if (!parameter!.Any())
		{
			// ReSharper disable once LocalizableElement
			throw new ArgumentException($"The '{paramName}' collection cannot be empty.", paramName);
		}
	}

	public static string FormatForMessage(this Exception exception)
	{
		string message = Formatter.Format(exception.GetType()).PrependAOrAn();
		if (!string.IsNullOrEmpty(exception.Message))
		{
			message += ":" + Environment.NewLine + exception.Message.Indent();
		}

		return message;
	}

	public static IEnumerable<Exception> GetInnerExceptions(this Exception? actual)
	{
		switch (actual)
		{
			case AggregateException aggregateException:
				{
					foreach (Exception innerException in aggregateException.InnerExceptions)
					{
						yield return innerException;
						foreach (Exception inner in GetInnerExceptions(innerException))
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
						foreach (Exception inner in GetInnerExceptions(actual.InnerException))
						{
							yield return inner;
						}
					}

					break;
				}
		}
	}
}
