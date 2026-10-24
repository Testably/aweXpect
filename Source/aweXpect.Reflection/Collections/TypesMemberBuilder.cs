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

	public Filtered.Events.MemberScopeResult Events()
		=> new(_source, "events ", _state.BuildEventFilter(), MemberScope.DeclaredOnly);

	public Filtered.Fields.MemberScopeResult Fields()
		=> new(_source, "fields ", _state.BuildFieldFilter(), MemberScope.DeclaredOnly);

	public Filtered.Methods.MemberScopeResult Methods()
		=> new(_source, "methods ", _state.BuildMethodFilter(), MemberScope.DeclaredOnly);

	public Filtered.Properties.MemberScopeResult Properties()
		=> new(_source, "properties ", _state.BuildPropertyFilter(), MemberScope.DeclaredOnly);

	IMembers IMembers.IProtected.Internal
		=> new TypesMemberBuilder(_source, _state.WithAccess(AccessModifiers.ProtectedInternal));
}
