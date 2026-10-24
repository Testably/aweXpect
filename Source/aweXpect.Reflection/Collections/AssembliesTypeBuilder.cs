using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Collections;

internal sealed class AssembliesTypeBuilder :
	ITypeAssemblies.IPrivate,
	ITypeAssemblies.IProtected
{
	private readonly MemberFilterState _memberState;
	private readonly Filtered.Assemblies _source;
	private readonly string _typeFilterDescription;
	private readonly List<Func<Type, bool>> _typeFilters;

	internal AssembliesTypeBuilder(
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

	public ITypeAssemblies Generic => AppendTypeFilter(t => t.IsGenericType, "generic ");

	public ITypeAssemblies Nested => AppendTypeFilter(t => t.IsNested, "nested ");

	public ILimitedAbstractSealedTypeAssemblies<ILimitedAbstractSealedTypeAssemblies> Abstract
		=> new AssembliesAbstractSealedBuilder(
			_source,
			_memberState.WithAbstract(),
			AppendFilter(t => t.IsReallyAbstract()),
			_typeFilterDescription + "abstract ");

	public ILimitedAbstractSealedTypeAssemblies<ILimitedAbstractSealedTypeAssemblies> Sealed
		=> new AssembliesAbstractSealedBuilder(
			_source,
			_memberState.WithSealed(),
			AppendFilter(t => t.IsReallySealed()),
			_typeFilterDescription + "sealed ");

	public ILimitedStaticTypeAssemblies<ILimitedStaticTypeAssemblies> Static
		=> new AssembliesTypeBuilder(
			_source,
			_memberState.WithStatic(),
			AppendFilter(t => t.IsReallyStatic()),
			_typeFilterDescription + "static ");

	ITypeAssemblies ITypeAssemblies.IPrivate.Protected
		=> new AssembliesTypeBuilder(
			_source,
			_memberState.WithAccess(AccessModifiers.PrivateProtected),
			_typeFilters,
			_typeFilterDescription);

	public Filtered.Types Types()
		=> BuildTypes(_typeFilters, _typeFilterDescription, "types ", false);

	public Filtered.Types Classes()
		=> BuildTypes(AppendFilter(t => t.IsReallyClass()), _typeFilterDescription, "classes ");

	public Filtered.Types Records()
		=> BuildTypes(AppendFilter(t => t.IsRecordClass()), _typeFilterDescription, "records ");

	public Filtered.Types RecordStructs()
		=> BuildTypes(AppendFilter(t => t.IsRecordStruct()), _typeFilterDescription, "record structs ");

	public Filtered.Types Structs()
		=> BuildTypes(AppendFilter(t => t.IsReallyStruct()), _typeFilterDescription, "structs ");

	public Filtered.Types Interfaces()
		=> BuildTypes(AppendFilter(t => t.IsInterface), _typeFilterDescription, "interfaces ");

	public Filtered.Types Enums()
		=> BuildTypes(AppendFilter(t => t.IsEnum), _typeFilterDescription, "enums ");

	// Member selectors below intentionally do NOT apply _typeFilters to the source type set:
	// `assemblies.Static.Methods()` returns static methods on ALL types in the assembly, not
	// methods on static types. This matches pre-refactor behavior; tracked as a separate concern.
	public Filtered.Constructors Constructors()
	{
		Filtered.Constructors constructors = new(new Filtered.Types(_source, ""), "constructors ");
		IFilter<ConstructorInfo>? filter = _memberState.BuildConstructorFilter();
		return filter is null ? constructors : constructors.Which(filter);
	}

	public Filtered.Events Events()
	{
		Filtered.Events events = new(new Filtered.Types(_source, ""), "events ", MemberScopes.DeclaredOnly);
		IFilter<EventInfo>? filter = _memberState.BuildEventFilter();
		return filter is null ? events : events.Which(filter);
	}

	public Filtered.Fields Fields()
	{
		Filtered.Fields fields = new(new Filtered.Types(_source, ""), "fields ", MemberScopes.DeclaredOnly);
		IFilter<FieldInfo>? filter = _memberState.BuildFieldFilter();
		return filter is null ? fields : fields.Which(filter);
	}

	public Filtered.Methods Methods()
	{
		Filtered.Methods methods = new(new Filtered.Types(_source, ""), "methods ", MemberScopes.DeclaredOnly);
		IFilter<MethodInfo>? filter = _memberState.BuildMethodFilter();
		return filter is null ? methods : methods.Which(filter);
	}

	public Filtered.Properties Properties()
	{
		Filtered.Properties properties = new(new Filtered.Types(_source, ""), "properties ", MemberScopes.DeclaredOnly);
		IFilter<PropertyInfo>? filter = _memberState.BuildPropertyFilter();
		return filter is null ? properties : properties.Which(filter);
	}

	ITypeAssemblies ITypeAssemblies.IProtected.Internal
		=> new AssembliesTypeBuilder(
			_source,
			_memberState.WithAccess(AccessModifiers.ProtectedInternal),
			_typeFilters,
			_typeFilterDescription);

	private AssembliesTypeBuilder AppendTypeFilter(Func<Type, bool> predicate, string descriptionAppend)
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
		string typeDescription,
		bool includeWhenEmpty = true)
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

		Filtered.Types result = new(_source, typeDescription);
		if (typeFilters.Count == 0 && !includeWhenEmpty)
		{
			return result;
		}

		List<Func<Type, bool>> filters = typeFilters;
		return result.Which(Filter.Prefix<Type>(
			type => filters.All(predicate => predicate.Invoke(type)),
			typeFilterDescription));
	}
}
