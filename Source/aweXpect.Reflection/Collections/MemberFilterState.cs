using System.Reflection;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Collections;

internal sealed class MemberFilterState
{
	public AccessModifiers? AccessModifier;
	public bool IsAbstract;
	public bool IsSealed;
	public bool IsStatic;
	private string _modifierDescription = "";

	public bool HasAnything => AccessModifier is not null || IsStatic || IsAbstract || IsSealed;

	public void Reset()
	{
		AccessModifier = null;
		IsStatic = false;
		IsAbstract = false;
		IsSealed = false;
		_modifierDescription = "";
	}

	public void SetAccess(AccessModifiers modifier) => AccessModifier = modifier;

	public void SetStatic()
	{
		if (IsStatic)
		{
			return;
		}

		IsStatic = true;
		_modifierDescription += "static ";
	}

	public void SetAbstract()
	{
		if (IsAbstract)
		{
			return;
		}

		IsAbstract = true;
		_modifierDescription += "abstract ";
	}

	public void SetSealed()
	{
		if (IsSealed)
		{
			return;
		}

		IsSealed = true;
		_modifierDescription += "sealed ";
	}

	public string BuildDescription()
		=> (AccessModifier?.GetString(" ") ?? "") + _modifierDescription;

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
