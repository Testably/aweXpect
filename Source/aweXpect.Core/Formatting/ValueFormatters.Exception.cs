using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using aweXpect.Core.Helpers;
using aweXpect.Equivalency;

namespace aweXpect.Formatting;

public static partial class ValueFormatters
{
	/// <summary>
	///     The members that a snapshot does not take from the type of an exception: those of <see cref="Exception" />,
	///     which it either writes itself or leaves out, and the fusion log of the assembly loading exceptions, because
	///     they depend on the build, the runtime or the machine.
	/// </summary>
	private static readonly HashSet<string> ExceptionMembersWrittenSeparately = new(StringComparer.Ordinal)
	{
		nameof(Exception.Data),
		nameof(Exception.HelpLink),
		nameof(Exception.HResult),
		nameof(Exception.InnerException),
		nameof(Exception.Message),
		nameof(Exception.Source),
		nameof(Exception.StackTrace),
		nameof(Exception.TargetSite),
		nameof(FileNotFoundException.FusionLog),
	};

	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	/// <remarks>
	///     An exception is formatted as its type name and message, without namespace, inner exceptions or stack trace.
	///     A snapshot writes it member-wise instead, with its message, its own public members, its non-empty data and its
	///     inner exceptions, but without the members that depend on the build or the runtime, like the stack trace.
	/// </remarks>
	public static string Format(
		this ValueFormatter formatter,
		Exception? value,
		FormattingOptions? options = null)
	{
		StringBuilder stringBuilder = new();
		formatter.Format(stringBuilder, value, options);
		return stringBuilder.ToString();
	}

	/// <summary>
	///     Appends the formatted <paramref name="value" /> according to the <paramref name="options" />
	///     to the <paramref name="stringBuilder" />.
	/// </summary>
	/// <remarks>
	///     An exception is formatted as its type name and message, without namespace, inner exceptions or stack trace.
	///     A snapshot writes it member-wise instead, with its message, its own public members, its non-empty data and its
	///     inner exceptions, but without the members that depend on the build or the runtime, like the stack trace.
	/// </remarks>
	public static void Format(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		Exception? value,
		FormattingOptions? options = null)
		=> FormatException(stringBuilder, value, options, null);

	private static void FormatException(
		StringBuilder stringBuilder,
		Exception? value,
		FormattingOptions? options,
		FormattingContext? context)
	{
		if (value == null)
		{
			stringBuilder.Append(ValueFormatter.NullString);
			return;
		}

		if (TryFormatWithRegistrations(stringBuilder, value, options, context))
		{
			return;
		}

		FormatType(value.GetType(), stringBuilder);
		if (options?.IsSnapshot == true)
		{
			stringBuilder.Append(' ');
			WriteTypeValues(value, stringBuilder, exception => GetExceptionMembers((Exception)exception), options with
			{
				IncludeType = false,
			}, context ?? new FormattingContext());
			return;
		}

		string? message = GetMessage(value);
		if (string.IsNullOrEmpty(message))
		{
			return;
		}

		stringBuilder.Append(": ");
		options ??= FormattingOptions.SingleLine;
		if (options.UseLineBreaks)
		{
			stringBuilder.Append(message.Indent(options.Indentation + "  ", false));
		}
		else
		{
			stringBuilder.Append(EscapeUnquoted(TruncateUnlessSnapshot(message, options), options));
		}
	}

	/// <remarks>
	///     The members of the well-known framework exceptions are read without reflection, so that a snapshot is the same
	///     when published with Native AOT. Every other member is found like the members of any object, from the
	///     <see cref="aweXpect.Core.Metadata.TypeMetadataRegistry" /> or by reflection. The inner exception of an
	///     <see cref="AggregateException" /> only repeats the first of its inner exceptions, so it is left out.
	/// </remarks>
	private static List<EquivalencyMember> GetExceptionMembers(Exception exception)
	{
		List<EquivalencyMember> members =
		[
			new(nameof(Exception.Message), typeof(string), value => ((Exception)value).Message),
		];
		if (HasValue(exception, value => value.Data.Count > 0))
		{
			members.Add(new EquivalencyMember(nameof(Exception.Data), typeof(IDictionary),
				value => ((Exception)value).Data));
		}

		if (exception is AggregateException)
		{
			members.Add(new EquivalencyMember(nameof(AggregateException.InnerExceptions),
				typeof(ReadOnlyCollection<Exception>), value => ((AggregateException)value).InnerExceptions));
		}
		else if (HasValue(exception, value => value.InnerException is not null))
		{
			members.Add(new EquivalencyMember(nameof(Exception.InnerException), typeof(Exception),
				value => ((Exception)value).InnerException));
		}

		AddFrameworkExceptionMembers(exception, members);

		HashSet<string> names = new(StringComparer.Ordinal);
		foreach (EquivalencyMember member in members)
		{
			names.Add(member.Name);
		}

		foreach (EquivalencyMember member in GetMembers(exception.GetType()) ?? [])
		{
			if (!ExceptionMembersWrittenSeparately.Contains(member.Name) && names.Add(member.Name))
			{
				members.Add(member);
			}
		}

		return members;
	}

	private static void AddFrameworkExceptionMembers(Exception exception, List<EquivalencyMember> members)
	{
		if (exception is ArgumentException)
		{
			members.Add(new EquivalencyMember(nameof(ArgumentException.ParamName), typeof(string),
				value => ((ArgumentException)value).ParamName));
		}

		if (exception is ArgumentOutOfRangeException)
		{
			members.Add(new EquivalencyMember(nameof(ArgumentOutOfRangeException.ActualValue), typeof(object),
				value => ((ArgumentOutOfRangeException)value).ActualValue));
		}

		if (exception is ObjectDisposedException)
		{
			members.Add(new EquivalencyMember(nameof(ObjectDisposedException.ObjectName), typeof(string),
				value => ((ObjectDisposedException)value).ObjectName));
		}

		if (exception is FileNotFoundException)
		{
			members.Add(new EquivalencyMember(nameof(FileNotFoundException.FileName), typeof(string),
				value => ((FileNotFoundException)value).FileName));
		}
	}

	/// <remarks>
	///     A member whose getter throws is kept, so that reading it writes the placeholder like for any other member.
	/// </remarks>
	private static bool HasValue(Exception exception, Func<Exception, bool> hasValue)
	{
		try
		{
			return hasValue(exception);
		}
		catch (Exception)
		{
			return true;
		}
	}
}
