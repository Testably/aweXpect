using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aweXpect.Equivalency;

public static partial class EquivalencyComparison
{
	/// <summary>
	///     Stands in for the actual object below an expected member that the actual object does not have, when a
	///     mapping runs through that member, so that every member below it that is not mapped is reported as missing.
	/// </summary>
	private static readonly object NoActualObject = new NoActual();

	/// <summary>
	///     Returns the mappings that apply to the members of the <paramref name="expected" /> object: the ones of the
	///     <paramref name="typeOptions" /> that apply to the objects, and the <paramref name="pendingMappings" /> that
	///     the enclosing objects handed down, which win as they are more specific.
	/// </summary>
	private static List<ActiveMapping>? GetActiveMappings(object actual, object expected,
		EquivalencyTypeOptions typeOptions, List<ActiveMapping>? pendingMappings)
	{
		List<ActiveMapping>? mappings = null;
		if (typeOptions.MemberMappings.Length > 0 && !ReferenceEquals(actual, NoActualObject))
		{
			foreach (MemberMapping mapping in typeOptions.MemberMappings
				         .Where(mapping => mapping.AppliesTo(actual, expected)))
			{
				(mappings ??= []).Add(new ActiveMapping(mapping, actual, 0));
			}
		}

		if (pendingMappings is null)
		{
			return mappings;
		}

		mappings ??= [];
		mappings.AddRange(pendingMappings);
		return mappings;
	}

	/// <summary>
	///     Finds the last of the <paramref name="mappings" /> that ends at the expected member
	///     <paramref name="memberName" />, and the ones that run through it, which then continue below it.
	/// </summary>
	private static bool FindMappings(List<ActiveMapping> mappings, string memberName, out ActiveMapping? mapping,
		out List<ActiveMapping>? mappingsBelow)
	{
		mapping = null;
		mappingsBelow = null;
		foreach (ActiveMapping candidate in mappings)
		{
			string[] expectedPath = candidate.Mapping.ExpectedPath;
			if (!string.Equals(expectedPath[candidate.Offset], memberName, StringComparison.OrdinalIgnoreCase))
			{
				continue;
			}

			if (candidate.Offset == expectedPath.Length - 1)
			{
				mapping = candidate;
			}
			else
			{
				(mappingsBelow ??= []).Add(new ActiveMapping(candidate.Mapping, candidate.ActualRoot,
					candidate.Offset + 1));
			}
		}

		return mapping is not null || mappingsBelow is not null;
	}

#pragma warning disable S107 // https://rules.sonarsource.com/csharp/RSPEC-107
	/// <summary>
	///     Compares an expected member that a mapping ends at or runs through.
	/// </summary>
	/// <remarks>
	///     A member that a mapping ends at is compared with the value of its actual member. Otherwise, the mappings run
	///     through it, so it is compared with the member of the same name, and when the actual object has none, on the
	///     expected side only.
	/// </remarks>
	private static async ValueTask<bool> CompareMappedMember(EquivalencyMemberPlan.PlannedMember member,
		MemberType memberType, MemberPath memberPath, object actual, object expected, ActiveMapping? mapping,
		List<ActiveMapping>? mappingsBelow, EquivalencyOptions options, EquivalencyTypeOptions typeOptions,
		StringBuilder failureBuilder, EquivalencyContext context)
	{
		if (mapping?.Mapping.ExpectedName is { } expectedName)
		{
			context.TrackMatchedName(expectedName);
		}

		if (IsIgnored(context, typeOptions.MembersToIgnore, memberType, memberPath, member.Expected.DeclaredType))
		{
			return true;
		}

		object? expectedValue = ReadMember(member.Expected.GetValue, expected, memberPath);
		object? actualValue;
		if (mapping is { } activeMapping)
		{
			if (!TryReadMappedValue(activeMapping, typeOptions, memberPath, out actualValue))
			{
				AppendMissingMappedMember(failureBuilder, memberType, memberPath.ToString(),
					activeMapping.Mapping.ActualPath, context);
				return false;
			}
		}
		else if (!ReferenceEquals(actual, NoActualObject) &&
		         member.GetActualAccessor(out _) is { } actualAccessor)
		{
			actualValue = ReadMember(actualAccessor, actual, memberPath);
		}
		else
		{
			return await CompareExpectedOnly(expectedValue, mappingsBelow!, memberType, memberPath, options,
				typeOptions, failureBuilder, context);
		}

		context.SetPendingMappings(expectedValue, mappingsBelow);
		try
		{
			return await Compare(actualValue, expectedValue, options, typeOptions, failureBuilder, memberPath,
				memberType, context);
		}
		finally
		{
			context.SetPendingMappings(null, null);
		}
	}

	/// <summary>
	///     Compares the <paramref name="expectedValue" /> of a member that the actual object does not have, through
	///     which the <paramref name="mappingsBelow" /> run.
	/// </summary>
	/// <remarks>
	///     Its members are compared with the values of their mappings, and each one without a mapping is reported as
	///     missing. A value that is not compared by its members has no members to map, so the member itself is
	///     missing. When the <paramref name="expectedValue" /> is <see langword="null" />, the members below it read as
	///     <see langword="null" />, like the members below a <see langword="null" /> value on the way to an actual
	///     member.
	/// </remarks>
	private static async ValueTask<bool> CompareExpectedOnly(object? expectedValue,
		List<ActiveMapping> mappingsBelow, MemberType memberType, MemberPath memberPath, EquivalencyOptions options,
		EquivalencyTypeOptions typeOptions, StringBuilder failureBuilder, EquivalencyContext context)
	{
		if (expectedValue is null)
		{
			bool result = true;
			foreach (ActiveMapping mapping in mappingsBelow)
			{
				if (mapping.Mapping.ExpectedName is { } expectedName)
				{
					context.TrackMatchedName(expectedName);
				}

				string[] expectedPath = mapping.Mapping.ExpectedPath;
				string pathBelow = string.Join(".", expectedPath, mapping.Offset, expectedPath.Length - mapping.Offset);
				MemberPath leafPath = $"{memberPath}.{pathBelow}";
				if (!TryReadMappedValue(mapping, typeOptions, leafPath, out object? actualValue))
				{
					AppendMissingMappedMember(failureBuilder, memberType, leafPath.ToString(),
						mapping.Mapping.ActualPath, context);
					result = false;
				}
				else if (!await Compare(actualValue, (object?)null, options, typeOptions, failureBuilder, leafPath,
					         memberType, context))
				{
					result = false;
				}
			}

			return result;
		}

		EquivalencyTypeOptions expectedOptions = GetRegisteredOptions(expectedValue.GetType(), options, context) ??
		                                         context.GetInheritedOptions(options, typeOptions);
		if (expectedValue is IEnumerable ||
		    IsComparedByValue(expectedValue.GetType(), expectedOptions, options))
		{
			AppendMissingMember(failureBuilder, memberType, memberPath.ToString(), false, context);
			return false;
		}

		context.SetPendingMappings(expectedValue, mappingsBelow);
		try
		{
			return await CompareNested(NoActualObject, expectedValue, options, expectedOptions, failureBuilder,
				memberPath, memberType, context);
		}
		finally
		{
			context.SetPendingMappings(null, null);
		}
	}
#pragma warning restore S107

	/// <summary>
	///     Reads the actual member of the <paramref name="mapping" />, or returns <see langword="false" /> when a member
	///     on the way does not exist.
	/// </summary>
	/// <remarks>
	///     A <see langword="null" /> value on the way reads as <see langword="null" />. The members are looked up on the
	///     runtime type of each value, as a property first, with the visibilities of the
	///     <paramref name="typeOptions" />.
	/// </remarks>
	private static bool TryReadMappedValue(ActiveMapping mapping, EquivalencyTypeOptions typeOptions,
		MemberPath memberPath, out object? value)
	{
		value = mapping.ActualRoot;
		foreach (string name in mapping.Mapping.ActualPath)
		{
			if (value is null)
			{
				return true;
			}

			Type type = value.GetType();
			Func<object, object?>? accessor =
				(typeOptions.Properties == IncludeMembers.None
					? null
					: EquivalencyMembers.FindProperty(type, name, typeOptions.Properties)) ??
				(typeOptions.Fields == IncludeMembers.None
					? null
					: EquivalencyMembers.FindField(type, name, typeOptions.Fields));
			if (accessor is null)
			{
				value = null;
				return false;
			}

			value = ReadMember(accessor, value, memberPath);
		}

		return true;
	}

	private static void AppendMissingMappedMember(StringBuilder failureBuilder, MemberType memberType,
		string memberPath, string[] actualPath, EquivalencyContext context)
	{
		if (SkipsText(context))
		{
			return;
		}

		AppendEntry(failureBuilder, memberType, memberPath, context);
		failureBuilder.Append(", mapped to ").Append(string.Join(".", actualPath))
			.Append(", was missing on the actual object");
	}

	/// <summary>
	///     A mapping that applies to the members of the objects that are currently compared.
	/// </summary>
	/// <param name="mapping">The mapping.</param>
	/// <param name="actualRoot">The actual object that the actual path of the mapping is read from.</param>
	/// <param name="offset">The index of the member of the expected path at the current depth.</param>
	private readonly struct ActiveMapping(MemberMapping mapping, object actualRoot, int offset)
	{
		public MemberMapping Mapping { get; } = mapping;
		public object ActualRoot { get; } = actualRoot;
		public int Offset { get; } = offset;
	}

	private sealed class NoActual
	{
	}
}
