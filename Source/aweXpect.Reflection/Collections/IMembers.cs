namespace aweXpect.Reflection.Collections;

/// <summary>
///     A terminal interface that exposes the five member-kind selectors.
/// </summary>
public interface IMemberSelectors
{
	/// <summary>
	///     Get all constructors matching the current member filter.
	/// </summary>
	Filtered.Constructors Constructors();

	/// <summary>
	///     Get all methods matching the current member filter.
	/// </summary>
	Filtered.Methods Methods();

	/// <summary>
	///     Get all properties matching the current member filter.
	/// </summary>
	Filtered.Properties Properties();

	/// <summary>
	///     Get all fields matching the current member filter.
	/// </summary>
	Filtered.Fields Fields();

	/// <summary>
	///     Get all events matching the current member filter.
	/// </summary>
	Filtered.Events Events();
}

/// <summary>
///     An interface to allow filtering for members of types.
/// </summary>
/// <remarks>
///     Supports chaining a single one of <c>Static</c>/<c>Abstract</c>/<c>Sealed</c> before selecting
///     the member kind. The three modifiers are mutually exclusive (a static member cannot be abstract
///     or sealed) and are not repeatable.
/// </remarks>
public interface IMembers : IMemberSelectors
{
	/// <summary>
	///     Filters for static members.
	/// </summary>
	IMemberSelectors Static { get; }

	/// <summary>
	///     Filters for abstract members.
	/// </summary>
	ILimitedAbstractSealedMembers Abstract { get; }

	/// <summary>
	///     Filters for sealed members.
	/// </summary>
	ILimitedAbstractSealedMembers Sealed { get; }

	/// <summary>
	///     An interface to allow filtering for members of types after the <c>Private</c> access modifier.
	/// </summary>
	public interface IPrivate : IMembers
	{
		/// <summary>
		///     Filters for private protected members.
		/// </summary>
		IMembers Protected { get; }
	}

	/// <summary>
	///     An interface to allow filtering for members of types after the <c>Protected</c> access modifier.
	/// </summary>
	public interface IProtected : IMembers
	{
		/// <summary>
		///     Filters for protected internal members.
		/// </summary>
		IMembers Internal { get; }
	}
}

/// <summary>
///     A limited terminal interface for members whose modifiers are compatible with
///     <c>Abstract</c> or <c>Sealed</c> (methods, properties, events).
/// </summary>
public interface ILimitedAbstractSealedMembers
{
	/// <summary>
	///     Get all methods matching the current member filter.
	/// </summary>
	Filtered.Methods Methods();

	/// <summary>
	///     Get all properties matching the current member filter.
	/// </summary>
	Filtered.Properties Properties();

	/// <summary>
	///     Get all events matching the current member filter.
	/// </summary>
	Filtered.Events Events();
}
