using System.Reflection;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Collections;

internal sealed class MemberFilterState
{
	public static readonly MemberFilterState Empty = new(null, false, false, false);

	private MemberFilterState(AccessModifiers? accessModifier, bool isStatic, bool isAbstract, bool isSealed)
	{
		AccessModifier = accessModifier;
		IsStatic = isStatic;
		IsAbstract = isAbstract;
		IsSealed = isSealed;
	}

	public AccessModifiers? AccessModifier { get; }
	public bool IsAbstract { get; }
	public bool IsSealed { get; }
	public bool IsStatic { get; }

	public MemberFilterState WithAccess(AccessModifiers modifier)
		=> new(modifier, IsStatic, IsAbstract, IsSealed);

	public MemberFilterState WithStatic()
		=> IsStatic ? this : new MemberFilterState(AccessModifier, true, IsAbstract, IsSealed);

	public MemberFilterState WithAbstract()
		=> IsAbstract ? this : new MemberFilterState(AccessModifier, IsStatic, true, IsSealed);

	public MemberFilterState WithSealed()
		=> IsSealed ? this : new MemberFilterState(AccessModifier, IsStatic, IsAbstract, true);

	public string BuildDescription()
	{
		string description = AccessModifier?.GetString(" ") ?? "";
		if (IsStatic)
		{
			description += "static ";
		}

		if (IsAbstract)
		{
			description += "abstract ";
		}

		if (IsSealed)
		{
			description += "sealed ";
		}

		return description;
	}

	public IFilter<ConstructorInfo>? BuildConstructorFilter()
	{
		AccessModifiers? access = AccessModifier;
		bool isStatic = IsStatic;
		if (access is null && !isStatic)
		{
			return null;
		}

		return Filter.Prefix<ConstructorInfo>(
			c => (access is null || c.HasAccessModifier(access.Value)) &&
			     (!isStatic || c.IsStatic),
			BuildDescription());
	}

	public IFilter<MethodInfo>? BuildMethodFilter()
	{
		AccessModifiers? access = AccessModifier;
		bool isStatic = IsStatic;
		bool isAbstract = IsAbstract;
		bool isSealed = IsSealed;
		if (access is null && !isStatic && !isAbstract && !isSealed)
		{
			return null;
		}

		return Filter.Prefix<MethodInfo>(
			m => (access is null || m.HasAccessModifier(access.Value)) &&
			     (!isStatic || m.IsStatic) &&
			     (!isAbstract || m.IsAbstract) &&
			     (!isSealed || m.IsReallySealed()),
			BuildDescription());
	}

	public IFilter<PropertyInfo>? BuildPropertyFilter()
	{
		AccessModifiers? access = AccessModifier;
		bool isStatic = IsStatic;
		bool isAbstract = IsAbstract;
		bool isSealed = IsSealed;
		if (access is null && !isStatic && !isAbstract && !isSealed)
		{
			return null;
		}

		return Filter.Prefix<PropertyInfo>(
			p => (access is null || p.HasAccessModifier(access.Value)) &&
			     (!isStatic || p.IsReallyStatic()) &&
			     (!isAbstract || p.IsReallyAbstract()) &&
			     (!isSealed || p.IsReallySealed()),
			BuildDescription());
	}

	public IFilter<FieldInfo>? BuildFieldFilter()
	{
		AccessModifiers? access = AccessModifier;
		bool isStatic = IsStatic;
		if (access is null && !isStatic)
		{
			return null;
		}

		return Filter.Prefix<FieldInfo>(
			f => (access is null || f.HasAccessModifier(access.Value)) &&
			     (!isStatic || f.IsStatic),
			BuildDescription());
	}

	public IFilter<EventInfo>? BuildEventFilter()
	{
		AccessModifiers? access = AccessModifier;
		bool isStatic = IsStatic;
		bool isAbstract = IsAbstract;
		bool isSealed = IsSealed;
		if (access is null && !isStatic && !isAbstract && !isSealed)
		{
			return null;
		}

		return Filter.Prefix<EventInfo>(
			e => (access is null || e.HasAccessModifier(access.Value)) &&
			     (!isStatic || e.AddMethod?.IsStatic == true) &&
			     (!isAbstract || e.IsReallyAbstract()) &&
			     (!isSealed || e.IsReallySealed()),
			BuildDescription());
	}
}
