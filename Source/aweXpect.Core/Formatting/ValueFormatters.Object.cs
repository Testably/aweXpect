using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using aweXpect.Core.Extending;
using aweXpect.Core.Helpers;
using aweXpect.Core.Metadata;
using aweXpect.Equivalency;

namespace aweXpect.Formatting;

public static partial class ValueFormatters
{
	private static readonly HashSet<Type> TupleDefinitions =
	[
		typeof(ValueTuple<>), typeof(ValueTuple<,>), typeof(ValueTuple<,,>), typeof(ValueTuple<,,,>),
		typeof(ValueTuple<,,,,>), typeof(ValueTuple<,,,,,>), typeof(ValueTuple<,,,,,,>), typeof(ValueTuple<,,,,,,,>),
		typeof(Tuple<>), typeof(Tuple<,>), typeof(Tuple<,,>), typeof(Tuple<,,,>),
		typeof(Tuple<,,,,>), typeof(Tuple<,,,,,>), typeof(Tuple<,,,,,,>), typeof(Tuple<,,,,,,,>),
	];

	internal static void FormatObject(StringBuilder stringBuilder, object value, FormattingOptions options,
		FormattingContext? context)
	{
		if (value.GetType() == typeof(object))
		{
			stringBuilder.Append($"System.Object (HashCode={value.GetHashCode()})");
		}
		else if (TryGetKeyValuePair(value, out object? key, out object? pairValue))
		{
			AppendTypeIfIncluded(stringBuilder, value, options);
			AppendKeyValuePair(Formatter, stringBuilder, key, pairValue, options with
			{
				IncludeType = false,
			}, context ?? new FormattingContext());
		}
		else if (TryGetTupleItems(value, out List<object?>? items))
		{
			AppendTypeIfIncluded(stringBuilder, value, options);
			AppendTupleItems(stringBuilder, value.GetType(), items, options with
			{
				IncludeType = false,
			}, context ?? new FormattingContext());
		}
		else
		{
			AppendRenderingOrMemberValues(stringBuilder, value, options, context);
		}
	}

	private static void AppendRenderingOrMemberValues(StringBuilder stringBuilder, object value,
		FormattingOptions options, FormattingContext? context)
	{
		string? toString = null;
		if (!IsAnonymousType(value.GetType()) && !IsCompilerGeneratedRecord(value.GetType()))
		{
			try
			{
				toString = value is IFormattable formattable
					? formattable.ToString(null, CultureInfo.InvariantCulture)
					: value.ToString();
			}
			catch (Exception exception)
			{
				stringBuilder.Append(FormatThrownException(
					$"ToString of {Formatter.Format(value.GetType())}", exception));
				return;
			}
		}

		if (toString is null || toString == value.GetType().ToString())
		{
			context ??= new FormattingContext();
			WriteTypeAndMemberValues(value, stringBuilder, options with
			{
				IncludeType = false,
			}, context);
			return;
		}

		AppendTypeIfIncluded(stringBuilder, value, options);
		if (options.UseLineBreaks)
		{
			stringBuilder.Append(toString.Indent(options.Indentation + "  ", false));
		}
		else
		{
			stringBuilder.Append(EscapeUnquoted(toString, options));
		}
	}

	/// <remarks>
	///     A rendering that is not written member-wise does not name the type, so it is prefixed like the values of
	///     the typed overloads.
	/// </remarks>
	private static void AppendTypeIfIncluded(StringBuilder stringBuilder, object value, FormattingOptions options)
	{
		if (options.IncludeType)
		{
			Formatter.Format(stringBuilder, value.GetType());
			stringBuilder.Append(' ');
		}
	}

	/// <remarks>
	///     A record renders itself through a compiler-generated <see cref="object.ToString()" /> that passes every
	///     member through its own one, which uses the current culture and does not quote strings, so it is written
	///     member-wise instead. A record with a user-written <see cref="object.ToString()" /> or <c>PrintMembers</c>
	///     keeps its own rendering. The source generator registers which records the compiler rendered, so that they
	///     are recognized without reflection; an unregistered record is only recognized by reflection and otherwise
	///     keeps its rendering as well. A registration without the mark, as made by an older generator, still reflects.
	/// </remarks>
	private static bool IsCompilerGeneratedRecord(Type type)
	{
		if (TypeMetadataRegistry.Instance.TryGet(type, out TypeMetadataRegistry.TypeMetadata? metadata) &&
		    metadata.IsCompilerGeneratedRecord)
		{
			return true;
		}

		if (!ReflectionFallback.IsSupported)
		{
			return false;
		}

		const BindingFlags instanceMembers = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		MethodInfo? toString = type.GetMethod(nameof(ToString), instanceMembers, null, Type.EmptyTypes, null);
		MethodInfo? printMembers = type.GetMethod("PrintMembers", instanceMembers, null, [typeof(StringBuilder),],
			null);
		return toString?.IsDefined(typeof(CompilerGeneratedAttribute), false) == true &&
		       printMembers?.IsDefined(typeof(CompilerGeneratedAttribute), false) == true;
	}

	/// <remarks>
	///     A registered type is served from the <see cref="TypeMetadataRegistry" />, so that publishing with trimming
	///     or Native AOT enabled does not remove the members from the message. Every other type is reflected over for
	///     its public instance fields and non-indexed properties, or yields <see langword="null" /> where reflection is
	///     unavailable, because a message must not throw for what it cannot show. Static members are left out, as they
	///     do not describe the instance, and a static member of the type's own struct type (like
	///     <c>CancellationToken.None</c>) boxes a new value on every read, so the recursion guard would never stop. A
	///     property without a public getter is left out as well, as the registration does, because it cannot be read
	///     from outside the type. The members are selected like those of an equivalency comparison, so that the message
	///     shows the members that were compared.
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

		return type.GetFields(IncludeMembers.Public)
			.Select(field => new EquivalencyMember(field.Name, field.FieldType, subject => field.GetValue(subject)))
			.Concat(type.GetProperties(IncludeMembers.Public)
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

	/// <remarks>
	///     A tuple renders itself through a compiler-generated <see cref="object.ToString()" /> that passes every item
	///     through its own one, which uses the current culture and does not quote strings, so its items are formatted
	///     like any other value instead, in the positional syntax of C#.
	/// </remarks>
	private static bool TryGetTupleItems(object value, [NotNullWhen(true)] out List<object?>? items)
	{
		items = null;
		if (!IsTupleType(value.GetType()))
		{
			return false;
		}

		items = [];
#if NET8_0_OR_GREATER
		ITuple tuple = (ITuple)value;
		for (int i = 0; i < tuple.Length; i++)
		{
			items.Add(tuple[i]);
		}
#else
		AddTupleItems(value, items);
#endif
		return true;
	}

#if !NET8_0_OR_GREATER
	/// <remarks>
	///     netstandard2.0 lacks <c>ITuple</c>, so the items are read by name, from the fields of a value tuple or the
	///     properties of a reference tuple. The eighth item holds the remaining items as another tuple.
	/// </remarks>
	private static void AddTupleItems(object tuple, List<object?> items)
	{
		Type type = tuple.GetType();
		int count = type.IsGenericType ? type.GetGenericArguments().Length : 0;
		for (int i = 1; i <= count; i++)
		{
			string name = i == 8 ? "Rest" : $"Item{i}";
			FieldInfo? field = type.GetField(name);
			object? item = field is not null ? field.GetValue(tuple) : type.GetProperty(name)!.GetValue(tuple);
			if (i == 8 && item is not null && IsTupleType(item.GetType()))
			{
				AddTupleItems(item, items);
			}
			else
			{
				items.Add(item);
			}
		}
	}
#endif

	private static bool IsTupleType(Type type)
		=> type == typeof(ValueTuple) ||
		   (type.IsGenericType && TupleDefinitions.Contains(type.GetGenericTypeDefinition()));

	private static void AppendTupleItems(StringBuilder stringBuilder, Type tupleType, List<object?> items,
		FormattingOptions options, FormattingContext context)
	{
		try
		{
			if (!EnterContent(context, options))
			{
				stringBuilder.Append("( \u2026 )");
				return;
			}

			List<Type>? itemTypes = options.FormatValue is null ? null : GetTupleItemTypes(tupleType);
			bool isFirst = true;
			stringBuilder.Append('(');
			for (int i = 0; i < items.Count; i++)
			{
				object? item = items[i];
				FormattedValue? offeredItem = null;
				ValueFormatting formatting = ValueFormatting.Write;
				if (itemTypes is not null)
				{
					offeredItem = FormattedValue.TupleItem(context.Value, i, GetTupleItemType(itemTypes, i, item),
						tupleType, item);
					formatting = OfferValue(offeredItem, options, context);
					if (formatting.IsOmitted)
					{
						continue;
					}
				}

				if (!isFirst)
				{
					stringBuilder.Append(", ");
				}

				isFirst = false;
				if (offeredItem is null)
				{
					Formatter.Format(stringBuilder, item, WithoutLineBreaksForString(item, options), context);
				}
				else
				{
					AppendOfferedTupleItem(stringBuilder, offeredItem, formatting, options, context);
				}
			}

			stringBuilder.Append(')');
		}
		finally
		{
			context.Depth--;
		}
	}

	/// <remarks>
	///     The declared type of the item, or its runtime type when the tuple type does not tell it.
	/// </remarks>
	private static Type GetTupleItemType(List<Type> itemTypes, int index, object? item)
		=> index < itemTypes.Count ? itemTypes[index] : item?.GetType() ?? typeof(object);

	private static void AppendOfferedTupleItem(StringBuilder stringBuilder, FormattedValue item,
		ValueFormatting formatting, FormattingOptions options, FormattingContext context)
	{
		if (formatting.IsReplaced)
		{
			object? replacement = formatting.Replacement;
			Formatter.Format(stringBuilder, replacement,
				WithoutLineBreaksForString(replacement, WithoutFormatValue(options)), context);
			return;
		}

		FormattedValue? parent = context.Value;
		context.Value = item;
		try
		{
			object? value = item.GetValue();
			Formatter.Format(stringBuilder, value, WithoutLineBreaksForString(value, options), context);
		}
		finally
		{
			context.Value = parent;
		}
	}

	/// <summary>
	///     The declared types of the items of a tuple, with the items of its eighth item flattened like the items.
	/// </summary>
	/// <remarks>
	///     A <see cref="Nullable{T}" /> is unwrapped, like the declared type of a member.
	/// </remarks>
	private static List<Type> GetTupleItemTypes(Type tupleType)
	{
		List<Type> itemTypes = [];
		Type? type = tupleType;
		while (type is { IsGenericType: true, })
		{
			Type[] arguments = type.GetGenericArguments();
			type = null;
			for (int i = 0; i < arguments.Length; i++)
			{
				if (i == 7 && IsTupleType(arguments[i]))
				{
					type = arguments[i];
				}
				else
				{
					itemTypes.Add(Nullable.GetUnderlyingType(arguments[i]) ?? arguments[i]);
				}
			}
		}

		return itemTypes;
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

	/// <summary>
	///     Writes the members and returns whether any was written, as each can be left out.
	/// </summary>
	private static bool WriteMemberValues(
		object obj,
		List<EquivalencyMember> members,
		StringBuilder stringBuilder,
		int indentation,
		FormattingOptions options,
		FormattingContext context)
	{
		bool isWritten = false;
		foreach (EquivalencyMember member in members.OrderBy(member => member.Name, StringComparer.Ordinal))
		{
			if (options.FormatValue is null)
			{
				WriteMemberValueTextFor(obj, member, stringBuilder, indentation, options, context);
			}
			else if (!TryWriteOfferedMemberValueTextFor(obj, member, stringBuilder, indentation, options, context))
			{
				continue;
			}

			isWritten = true;
			if (options.UseLineBreaks)
			{
				stringBuilder.AppendLine(",").Append(options.Indentation);
			}
			else
			{
				stringBuilder.Append(", ");
			}
		}

		if (isWritten)
		{
			stringBuilder.Length -=
				options.UseLineBreaks ? 1 + Environment.NewLine.Length + options.Indentation.Length : 2;
		}

		return isWritten;
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
			formattedValue = FormatMemberValue(member.GetValue(value), options, context);
		}
		catch (Exception exception) when (IsWrittenAsPlaceholder(exception, options, context))
		{
			formattedValue = FormatThrownException(member.Name, exception);
		}

		AppendMemberValueText(stringBuilder, member.Name, formattedValue, indentation, options);
	}

	/// <summary>
	///     Writes the member as <see cref="FormattingOptions.FormatValue" /> decides, and returns whether it was written.
	/// </summary>
	private static bool TryWriteOfferedMemberValueTextFor(
		object value,
		EquivalencyMember member,
		StringBuilder stringBuilder,
		int indentation,
		FormattingOptions options,
		FormattingContext context)
	{
		FormattedValue? parent = context.Value;
		FormattedValue offeredMember = FormattedValue.Member(parent, member.Name, member.DeclaredType,
			value.GetType(), value, member.GetValue);
		ValueFormatting formatting = OfferValue(offeredMember, options, context);
		if (formatting.IsOmitted)
		{
			return false;
		}

		string? formattedValue;
		context.Value = offeredMember;
		try
		{
			formattedValue = formatting.IsReplaced
				? FormatMemberValue(formatting.Replacement, WithoutFormatValue(options), context)
				: FormatMemberValue(offeredMember.GetValue(), options, context);
		}
		catch (Exception exception) when (IsWrittenAsPlaceholder(exception, options, context))
		{
			formattedValue = FormatThrownException(member.Name, exception);
		}
		finally
		{
			context.Value = parent;
		}

		AppendMemberValueText(stringBuilder, member.Name, formattedValue, indentation, options);
		return true;
	}

	private static string FormatMemberValue(object? memberValue, FormattingOptions options,
		FormattingContext context)
		=> Formatter.Format(memberValue, memberValue is string && options.UseLineBreaks
			? options with
			{
				UseLineBreaks = false,
			}
			: options, context);

	private static void AppendMemberValueText(StringBuilder stringBuilder, string name, string formattedValue,
		int indentation, FormattingOptions options)
	{
		stringBuilder.Append($"{new string(' ', indentation)}{name} = ");
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

		WriteTypeValues(obj, stringBuilder, value => GetMembers(value.GetType()), options, context);
	}

	/// <remarks>
	///     The instance is only tracked while its own members are written, so that it is a recursion only within
	///     itself, not when it appears a second time beside itself. The members are only looked up when they are
	///     written.
	/// </remarks>
	private static void WriteTypeValues(
		object obj,
		StringBuilder stringBuilder,
		Func<object, List<EquivalencyMember>?> getMembers,
		FormattingOptions options,
		FormattingContext context)
	{
		if (!context.FormattedObjects.Add(obj))
		{
			stringBuilder.Append("{ *recursive* }");
			return;
		}

		try
		{
			if (!EnterContent(context, options))
			{
				stringBuilder.Append("{ \u2026 }");
			}
			else
			{
				WriteMembers(obj, stringBuilder, getMembers(obj), options, context);
			}
		}
		finally
		{
			context.Depth--;
			context.FormattedObjects.Remove(obj);
		}
	}

	private static void WriteMembers(
		object obj,
		StringBuilder stringBuilder,
		List<EquivalencyMember>? members,
		FormattingOptions options,
		FormattingContext context)
	{
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
			int length = stringBuilder.Length;
			stringBuilder.Append('{');
			AppendLineWithIndentationOrBlank(stringBuilder, options);
			if (!WriteMemberValues(obj, members, stringBuilder, options.UseLineBreaks ? 2 : 0, options, context))
			{
				stringBuilder.Length = length;
				stringBuilder.Append("{ }");
				return;
			}

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
