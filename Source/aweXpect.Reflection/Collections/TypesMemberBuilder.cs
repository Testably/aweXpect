using System.Reflection;

namespace aweXpect.Reflection.Collections;

internal sealed class TypesMemberBuilder :
	IMembers.IPrivate,
	IMembers.IProtected,
	ILimitedAbstractSealedMembers
{
	private readonly Filtered.Types _source;
	private readonly MemberFilterState _state;

	internal TypesMemberBuilder(Filtered.Types source, MemberFilterState state)
	{
		_source = source;
		_state = state;
	}

	public IMemberSelectors Static => new TypesMemberBuilder(_source, _state.WithStatic());

	public ILimitedAbstractSealedMembers Abstract => new TypesMemberBuilder(_source, _state.WithAbstract());

	public ILimitedAbstractSealedMembers Sealed => new TypesMemberBuilder(_source, _state.WithSealed());

	IMembers IMembers.IPrivate.Protected
		=> new TypesMemberBuilder(_source, _state.WithAccess(AccessModifiers.PrivateProtected));

	public Filtered.Constructors Constructors()
	{
		Filtered.Constructors constructors = new(_source, "constructors ");
		IFilter<ConstructorInfo>? filter = _state.BuildConstructorFilter();
		return filter is null ? constructors : constructors.Which(filter);
	}

	public Filtered.Events Events(MemberScope memberScope = MemberScope.DeclaredOnly)
	{
		Filtered.Events events = new(_source, "events ", memberScope);
		IFilter<EventInfo>? filter = _state.BuildEventFilter();
		return filter is null ? events : events.Which(filter);
	}

	public Filtered.Fields Fields(MemberScope memberScope = MemberScope.DeclaredOnly)
	{
		Filtered.Fields fields = new(_source, "fields ", memberScope);
		IFilter<FieldInfo>? filter = _state.BuildFieldFilter();
		return filter is null ? fields : fields.Which(filter);
	}

	public Filtered.Methods Methods(MemberScope memberScope = MemberScope.DeclaredOnly)
	{
		Filtered.Methods methods = new(_source, "methods ", memberScope);
		IFilter<MethodInfo>? filter = _state.BuildMethodFilter();
		return filter is null ? methods : methods.Which(filter);
	}

	public Filtered.Properties Properties(MemberScope memberScope = MemberScope.DeclaredOnly)
	{
		Filtered.Properties properties = new(_source, "properties ", memberScope);
		IFilter<PropertyInfo>? filter = _state.BuildPropertyFilter();
		return filter is null ? properties : properties.Which(filter);
	}

	IMembers IMembers.IProtected.Internal
		=> new TypesMemberBuilder(_source, _state.WithAccess(AccessModifiers.ProtectedInternal));
}
