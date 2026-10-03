using System;
using System.Collections.Concurrent;
using aweXpect.Core.Metadata;

namespace aweXpect.Equivalency;

/// <summary>
///     The members of an expected type that are compared, together with the accessors that read them on an actual
///     type.
/// </summary>
/// <remarks>
///     Every object of a type pair resolves the same members, so they are resolved once per pair of types and
///     visibilities. A registration that is published later can change them, so a plan is only used for the
///     <see cref="TypeMetadataRegistry.Registration.Version" /> it was created for.
/// </remarks>
internal sealed class EquivalencyMemberPlan
{
	private static readonly ConcurrentDictionary<(Type Expected, Type Actual, IncludeMembers Fields,
		IncludeMembers Properties), EquivalencyMemberPlan> Plans = new();

	private readonly Type _actualType;
	private readonly Type _expectedType;
	private readonly IncludeMembers _fields;
	private readonly IncludeMembers _properties;
	private readonly int _version;
	private PlannedMember[]? _plannedFields;
	private PlannedMember[]? _plannedProperties;

	private EquivalencyMemberPlan(int version, Type expectedType, Type actualType, IncludeMembers fields,
		IncludeMembers properties)
	{
		_version = version;
		_expectedType = expectedType;
		_actualType = actualType;
		_fields = fields;
		_properties = properties;
	}

	/// <summary>
	///     The fields of the expected type, in the order they are compared.
	/// </summary>
	/// <remarks>
	///     The fields and the properties are only resolved when they are compared, so that an exception while the
	///     properties are read still comes after the fields were compared.
	/// </remarks>
	public PlannedMember[] Fields => _plannedFields ??= _fields == IncludeMembers.None
		? []
		: Array.ConvertAll(EquivalencyMembers.GetFields(_expectedType, _fields),
			member => new PlannedMember(member, true, _actualType, _fields, _properties));

	/// <inheritdoc cref="Fields" />
	public PlannedMember[] Properties => _plannedProperties ??= _properties == IncludeMembers.None
		? []
		: Array.ConvertAll(EquivalencyMembers.GetProperties(_expectedType, _properties),
			member => new PlannedMember(member, false, _actualType, _fields, _properties));

	/// <summary>
	///     Returns the plan for comparing an object of the <paramref name="actualType" /> with one of the
	///     <paramref name="expectedType" />.
	/// </summary>
	public static EquivalencyMemberPlan For(Type expectedType, Type actualType, IncludeMembers fields,
		IncludeMembers properties)
	{
		int version = TypeMetadataRegistry.Instance.Version;
		(Type, Type, IncludeMembers, IncludeMembers) key = (expectedType, actualType, fields, properties);
		if (Plans.TryGetValue(key, out EquivalencyMemberPlan? plan) && plan._version == version)
		{
			return plan;
		}

		plan = new EquivalencyMemberPlan(version, expectedType, actualType, fields, properties);
		Plans[key] = plan;
		return plan;
	}

	/// <summary>
	///     A member of the expected type and how it is read on the actual type.
	/// </summary>
	internal sealed class PlannedMember(
		EquivalencyMember expected,
		bool isField,
		Type actualType,
		IncludeMembers fields,
		IncludeMembers properties)
	{
		private Resolution? _resolution;

		public EquivalencyMember Expected { get; } = expected;

		/// <summary>
		///     Returns the accessor of the member on the actual type, or <see langword="null" /> when it has none, in
		///     which case <paramref name="isAmbiguous" /> tells whether it implements it explicitly more than once.
		/// </summary>
		/// <remarks>
		///     The member is looked up by its own kind first and falls back to the other kind of the same name, and only
		///     then to a property that the actual type implements explicitly. It is only looked up when it is compared,
		///     so a member that is ignored is never looked up.
		/// </remarks>
		public Func<object, object?>? GetActualAccessor(out bool isAmbiguous)
		{
			Resolution resolution = _resolution ??= Resolve();
			isAmbiguous = resolution.IsAmbiguous;
			return resolution.Accessor;
		}

		private Resolution Resolve()
		{
			bool isAmbiguous = false;
			Func<object, object?>? accessor = isField
				? EquivalencyMembers.FindField(actualType, Expected.Name, fields) ??
				  EquivalencyMembers.FindProperty(actualType, Expected.Name, properties) ??
				  EquivalencyMembers.FindExplicitProperty(actualType, Expected.Name, properties, out isAmbiguous)
				: EquivalencyMembers.FindProperty(actualType, Expected.Name, properties) ??
				  EquivalencyMembers.FindField(actualType, Expected.Name, fields) ??
				  EquivalencyMembers.FindExplicitProperty(actualType, Expected.Name, properties, out isAmbiguous);
			return new Resolution(accessor, isAmbiguous);
		}

		private sealed class Resolution(Func<object, object?>? accessor, bool isAmbiguous)
		{
			public Func<object, object?>? Accessor { get; } = accessor;
			public bool IsAmbiguous { get; } = isAmbiguous;
		}
	}
}
