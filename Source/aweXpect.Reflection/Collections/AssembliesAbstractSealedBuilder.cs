using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Collections;

internal sealed class AssembliesAbstractSealedBuilder
	: ILimitedAbstractSealedTypeAssemblies<ILimitedAbstractSealedTypeAssemblies>
{
	private readonly MemberFilterState _memberState;
	private readonly Filtered.Assemblies _source;
	private readonly string _typeFilterDescription;
	private readonly List<Func<Type, bool>> _typeFilters;

	internal AssembliesAbstractSealedBuilder(
		Filtered.Assemblies source,
		MemberFilterState memberState,
		List<Func<Type, bool>> typeFilters,
		string typeFilterDescription)
	{
		_source = source;
		_memberState = memberState;
		_typeFilters = typeFilters;
		_typeFilterDescription = typeFilterDescription;
	}

	public ILimitedAbstractSealedTypeAssemblies Generic
		=> AppendTypeFilter(t => t.IsGenericType, "generic ");

	public ILimitedAbstractSealedTypeAssemblies Nested
		=> AppendTypeFilter(t => t.IsNested, "nested ");

	public Filtered.Types Types(AccessModifiers accessModifier = AccessModifiers.Any)
		=> BuildTypes(_typeFilters, _typeFilterDescription, accessModifier, "types ");

	public Filtered.Types Classes(AccessModifiers accessModifier = AccessModifiers.Any)
		=> BuildTypes(AppendFilter(t => t.IsReallyClass()), _typeFilterDescription, accessModifier, "classes ");

	public Filtered.Types Records(AccessModifiers accessModifier = AccessModifiers.Any)
		=> BuildTypes(AppendFilter(t => t.IsRecordClass()), _typeFilterDescription, accessModifier, "records ");

	// Member selectors below intentionally do NOT apply _typeFilters to the source type set:
	// `assemblies.Sealed.Methods()` returns sealed methods on ALL types in the assembly, not
	// methods on sealed types. This matches pre-refactor behavior; tracked as a separate concern.
	public Filtered.Events Events()
	{
		Filtered.Events events = new(new Filtered.Types(_source, ""), "events ");
		IFilter<EventInfo>? filter = _memberState.BuildEventFilter();
		return filter is null ? events : events.Which(filter);
	}

	public Filtered.Methods Methods()
	{
		Filtered.Methods methods = new(new Filtered.Types(_source, ""), "methods ");
		IFilter<MethodInfo>? filter = _memberState.BuildMethodFilter();
		return filter is null ? methods : methods.Which(filter);
	}

	public Filtered.Properties Properties()
	{
		Filtered.Properties properties = new(new Filtered.Types(_source, ""), "properties ");
		IFilter<PropertyInfo>? filter = _memberState.BuildPropertyFilter();
		return filter is null ? properties : properties.Which(filter);
	}

	private AssembliesAbstractSealedBuilder AppendTypeFilter(Func<Type, bool> predicate, string descriptionAppend)
		=> new(
			_source,
			_memberState,
			AppendFilter(predicate),
			_typeFilterDescription + descriptionAppend);

	private List<Func<Type, bool>> AppendFilter(Func<Type, bool> predicate)
	{
		List<Func<Type, bool>> newFilters = new(_typeFilters.Count + 1);
		newFilters.AddRange(_typeFilters);
		newFilters.Add(predicate);
		return newFilters;
	}

	private Filtered.Types BuildTypes(
		List<Func<Type, bool>> typeFilters,
		string typeFilterDescription,
		AccessModifiers accessModifier,
		string typeDescription)
	{
		AccessModifiers? implicitAccess = _memberState.AccessModifier;
		if (implicitAccess is not null)
		{
			List<Func<Type, bool>> withImplicit = new(typeFilters.Count + 1);
			withImplicit.AddRange(typeFilters);
			withImplicit.Add(t => t.HasAccessModifier(implicitAccess.Value));
			typeFilters = withImplicit;
			typeFilterDescription += implicitAccess.Value.GetString(" ");
		}

		if (accessModifier != AccessModifiers.Any)
		{
			List<Func<Type, bool>> withExplicit = new(typeFilters.Count + 1);
			withExplicit.AddRange(typeFilters);
			withExplicit.Add(t => t.HasAccessModifier(accessModifier));
			typeFilters = withExplicit;
			typeFilterDescription = accessModifier.GetString(" ") + typeFilterDescription;
		}

		Filtered.Types result = new(_source, typeDescription);
		List<Func<Type, bool>> filters = typeFilters;
		return result.Which(Filter.Prefix<Type>(
			type => filters.All(predicate => predicate.Invoke(type)),
			typeFilterDescription));
	}
}
