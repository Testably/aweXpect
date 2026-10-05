using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

	/// <summary>
	///     Throws when the <paramref name="parameter" /> is a null collection, which a default
	///     <c>ImmutableArray&lt;T&gt;</c> is as well.
	/// </summary>
	public static void ThrowIfNull<T>(this IEnumerable<T>? parameter,
		[CallerArgumentExpression(nameof(parameter))] string? paramName = null)
		=> ThrowIfNullNamed(parameter.NullIfDefaultImmutableArray(), paramName);

	/// <summary>
	///     <see cref="ThrowIfNull{T}(IEnumerable{T}?,string?)" /> for the <paramref name="parameter" /> named after the
	///     polarity of the expectation: the expected collection, or the unexpected one when <paramref name="negated" />.
	/// </summary>
	public static void ThrowIfNull<T>(this IEnumerable<T>? parameter, bool negated)
		=> ThrowIfNullNamed(parameter.NullIfDefaultImmutableArray(), negated ? "unexpected" : "expected");

	/// <summary>
	///     Throws when the <paramref name="type" /> is null or not an exception type, as no exception could ever match it.
	/// </summary>
	public static void ThrowIfNotAnExceptionType(this Type? type,
		[CallerArgumentExpression(nameof(type))] string? paramName = null)
	{
		ThrowIfNullNamed(type, paramName);
		if (!typeof(Exception).IsAssignableFrom(type))
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentException(
				$"The '{paramName}' must be an exception type, but {Formatter.Format(type)} is not.", paramName));
		}
	}

	private static void ThrowIfNullNamed(object? parameter, string? paramName)
	{
		if (parameter is null)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentNullException(paramName, $"The {Describe(paramName)} cannot be null."));
		}
	}

	/// <summary>
	///     Quotes the <paramref name="paramName" /> and follows the polarity names, which are adjectives, with a noun.
	/// </summary>
	private static string Describe(string? paramName)
		=> paramName is "expected" or "unexpected" ? $"'{paramName}' value" : $"'{paramName}'";

	/// <summary>
	///     Throws when the <paramref name="parameter" /> is null or empty, naming it after the polarity of the
	///     expectation, and returns it wrapped, so that a sequence which can only be enumerated once survives both the
	///     guard and the subsequent comparison, and an infinite sequence is only enumerated as far as needed. An exception
	///     of the sequence propagates unchanged instead of being reported as if the subject threw it.
	/// </summary>
	public static IEnumerable<T> ToNonEmptyValues<T>(this IEnumerable<T>? parameter, bool negated)
	{
		string paramName = negated ? "unexpected" : "expected";
		ThrowIfNullNamed(parameter.NullIfDefaultImmutableArray(), paramName);
		IEnumerable<T> values = MaterializingEnumerable<T>.WrapParameter(parameter!);
		if (!values.Any())
		{
			throw Tracing.WriteException(EmptyCollection(paramName));
		}

		return values;
	}

	private static ArgumentException EmptyCollection(string? paramName)
		// ReSharper disable once LocalizableElement
		=> new($"The '{paramName}' collection cannot be empty.", paramName);

	/// <summary>
	///     Rejects a <see langword="null" /> element of the <paramref name="parameter" />, which is a collection of
	///     expectations or predicates that are invoked for the items.
	/// </summary>
	/// <remarks>
	///     A materialized collection is verified right away. Any other sequence is returned wrapped and verified while
	///     it is enumerated, so that it is neither enumerated early nor more often.
	///     <para />
	///     The read-only collection is covariant, so it also recognizes a collection whose element type only converts
	///     to <typeparamref name="T" />, e.g. a list of expectations on a less specific subject.
	/// </remarks>
	[return: NotNullIfNotNull(nameof(parameter))]
	public static IEnumerable<T>? WithoutNullElements<T>(this IEnumerable<T>? parameter,
		[CallerArgumentExpression(nameof(parameter))] string? paramName = null)
		where T : class
		=> WithoutNullElementsNamed(parameter, paramName);

	/// <summary>
	///     <see cref="WithoutNullElements{T}(IEnumerable{T}?,string?)" /> for the <paramref name="parameter" /> named
	///     after the polarity of the expectation: the expected collection, or the unexpected one when
	///     <paramref name="negated" />.
	/// </summary>
	[return: NotNullIfNotNull(nameof(parameter))]
	public static IEnumerable<T>? WithoutNullElements<T>(this IEnumerable<T>? parameter, bool negated)
		where T : class
		=> WithoutNullElementsNamed(parameter, negated ? "unexpected" : "expected");

	[return: NotNullIfNotNull(nameof(parameter))]
	private static IEnumerable<T>? WithoutNullElementsNamed<T>(IEnumerable<T>? parameter, string? paramName)
		where T : class
	{
		if (parameter is not (ICollection<T> or IReadOnlyCollection<T>))
		{
			return parameter?.Select(element
				=> element ?? throw Tracing.WriteException(NullElement(paramName)));
		}

		if (parameter.Any(element => element is null))
		{
			throw Tracing.WriteException(NullElement(paramName));
		}

		return parameter;
	}

	private static ArgumentException NullElement(string? paramName)
		// ReSharper disable once LocalizableElement
		=> new($"The '{paramName}' collection cannot contain <null>.", paramName);

	/// <summary>
	///     Formats the type of the <paramref name="exception" /> and its message, or the <paramref name="exceptionMessage" />
	///     when it is given.
	/// </summary>
	public static string FormatForMessage(this Exception exception, string? indentation, string relation = "",
		string? exceptionMessage = null)
	{
		string message = (relation + Formatter.Format(exception.GetType())).PrependAOrAn();
		exceptionMessage ??= exception.Message;
		if (!string.IsNullOrEmpty(exceptionMessage))
		{
			message += ":" + Environment.NewLine + exceptionMessage.Indent(indentation + "  ");
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
