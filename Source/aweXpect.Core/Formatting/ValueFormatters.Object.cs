using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Helpers;
using aweXpect.Core.Metadata;
using aweXpect.Equivalency;

namespace aweXpect.Formatting;

public static partial class ValueFormatters
{
	internal static void FormatObject(StringBuilder stringBuilder, object value, FormattingOptions options,
		FormattingContext? context)
	{
		if (value.GetType() == typeof(object))
		{
			stringBuilder.Append($"System.Object (HashCode={value.GetHashCode()})");
		}
		else if (TryGetKeyValuePair(value, out object? key, out object? pairValue))
		{
			FormattingOptions pairOptions = options with
			{
				IncludeType = false,
			};
			context ??= new FormattingContext();
			stringBuilder.Append('[');
			Format(Formatter, stringBuilder, key, pairOptions, context);
			stringBuilder.Append("] = ");
			Format(Formatter, stringBuilder, pairValue, pairOptions, context);
		}
		else if (IsAnonymousType(value.GetType()) || HasDefaultToStringImplementation(value))
		{
			context ??= new FormattingContext();
			WriteTypeAndMemberValues(value, stringBuilder, options with
			{
				IncludeType = false,
			}, context);
		}
		else if (options.UseLineBreaks)
		{
			stringBuilder.Append(value.ToString().Indent(indentFirstLine: false));
		}
		else
		{
			stringBuilder.Append(value);
		}
	}

	/// <remarks>
	///     A registered type is served from the <see cref="TypeMetadataRegistry" />, so that publishing with trimming
	///     or Native AOT enabled does not remove the members from the message. Every other type is reflected over with
	///     the default binding flags, as before, or yields <see langword="null" /> where reflection is unavailable,
	///     because a message must not throw for what it cannot show.
	/// </remarks>
	private static List<EquivalencyMember>? GetMembers(Type type)
	{
		if (TypeMetadataRegistry.Instance.TryGet(type, out TypeMetadataRegistry.TypeMetadata? metadata) &&
		    !(metadata.Fields.IsEmpty && metadata.Properties.IsEmpty))
		{
			return metadata.Fields.Values.Concat(metadata.Properties.Values)
				.Select(member => new EquivalencyMember(member.Name, member.MemberType, member.GetValue))
				.ToList();
		}

		if (!ReflectionFallback.IsSupported)
		{
			return null;
		}

		return type.GetFields()
			.Select(field => new EquivalencyMember(field.Name, field.FieldType, subject => field.GetValue(subject)))
			.Concat(type.GetProperties()
				.Select(property => new EquivalencyMember(property.Name, property.PropertyType,
					subject => property.GetValue(subject))))
			.ToList();
	}

	/// <remarks>
	///     A <see cref="KeyValuePair{TKey,TValue}" /> that reaches the formatter boxed has lost its type arguments, so
	///     its key and value are read like any other member; a pair whose members were removed by the trimmer keeps
	///     the plain object rendering.
	/// </remarks>
	private static bool TryGetKeyValuePair(object value, out object? key, out object? pairValue)
	{
		key = null;
		pairValue = null;
		Type type = value.GetType();
		if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(KeyValuePair<,>) ||
		    !(ReflectionFallback.IsSupported || EquivalencyMembers.IsRegistered(type)))
		{
			return false;
		}

		Func<object, object?>? getKey = EquivalencyMembers.FindProperty(type,
			nameof(KeyValuePair<object, object>.Key), IncludeMembers.Public);
		Func<object, object?>? getValue = EquivalencyMembers.FindProperty(type,
			nameof(KeyValuePair<object, object>.Value), IncludeMembers.Public);
		if (getKey is null || getValue is null)
		{
			return false;
		}

		key = getKey(value);
		pairValue = getValue(value);
		return true;
	}

	private static bool HasDefaultToStringImplementation(object value)
	{
		string? str = value.ToString();

		return str is null || str == value.GetType().ToString();
	}

	/// <remarks>
	///     An anonymous type renders itself through a compiler-generated <see cref="object.ToString()" /> that passes
	///     every member through its own one, so a <see cref="Type" /> member reads as <c>System.Int64</c> where the
	///     rest of the message reads <c>long</c>. It is therefore written member-wise like a type without a rendering
	///     of its own, but without a name that would say nothing to the reader.
	/// </remarks>
	private static bool IsAnonymousType(Type type)
		=> type.Name.Contains("AnonymousType") &&
		   type.IsDefined(typeof(CompilerGeneratedAttribute), false);

	private static void WriteMemberValues(
		object obj,
		List<EquivalencyMember> members,
		StringBuilder stringBuilder,
		int indentation,
		FormattingOptions options,
		FormattingContext context)
	{
		foreach (EquivalencyMember member in members.OrderBy(member => member.Name, StringComparer.Ordinal))
		{
			WriteMemberValueTextFor(obj, member, stringBuilder, indentation, options, context);
			if (options.UseLineBreaks)
			{
				stringBuilder.AppendLine(",").Append(options.Indentation);
			}
			else
			{
				stringBuilder.Append(", ");
			}
		}

		stringBuilder.Length -= options.UseLineBreaks ? 1 + Environment.NewLine.Length + options.Indentation.Length : 2;
	}

	private static void WriteMemberValueTextFor(
		object value,
		EquivalencyMember member,
		StringBuilder stringBuilder,
		int indentation,
		FormattingOptions options,
		FormattingContext context)
	{
		string? formattedValue;

		try
		{
			formattedValue = Formatter.Format(member.GetValue(value), options, context);
		}
		catch (Exception ex)
		{
			ex = (ex as TargetInvocationException)?.InnerException ?? ex;
			formattedValue = $"[Member '{member.Name}' threw an exception: '{ex.Message}']";
		}

		stringBuilder.Append($"{new string(' ', indentation)}{member.Name} = ");
		if (options.UseLineBreaks)
		{
			formattedValue = formattedValue.Indent("  ", false);
		}

		stringBuilder.Append(formattedValue);
	}

	private static void WriteTypeAndMemberValues(
		object obj,
		StringBuilder stringBuilder,
		FormattingOptions options,
		FormattingContext context)
	{
		Type type = obj.GetType();
		if (!IsAnonymousType(type))
		{
			Formatter.Format(stringBuilder, type);
			stringBuilder.Append(' ');
		}

		WriteTypeValues(obj, stringBuilder, type, options, context);
	}

	private static void WriteTypeValues(
		object obj,
		StringBuilder stringBuilder,
		Type type,
		FormattingOptions options,
		FormattingContext context)
	{
		if (!context.FormattedObjects.Add(obj))
		{
			stringBuilder.Append("{ *recursive* }");
			return;
		}

		List<EquivalencyMember>? members = GetMembers(type);
		if (members is null)
		{
			stringBuilder.Append("{ *unregistered* }");
		}
		else if (members.Count == 0)
		{
			stringBuilder.Append("{ }");
		}
		else
		{
			stringBuilder.Append('{');
			AppendLineWithIndentationOrBlank(stringBuilder, options);
			WriteMemberValues(obj, members, stringBuilder, options.UseLineBreaks ? 2 : 0, options, context);
			AppendLineWithIndentationOrBlank(stringBuilder, options);
			stringBuilder.Append('}');
		}
	}

	private static void AppendLineWithIndentationOrBlank(StringBuilder stringBuilder, FormattingOptions options)
	{
		if (options.UseLineBreaks)
		{
			stringBuilder.AppendLine().Append(options.Indentation);
		}
		else
		{
			stringBuilder.Append(' ');
		}
	}
}
