using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;

// ReSharper disable MemberHidesStaticFromOuterClass

namespace aweXpect.Reflection.Collections;

public static partial class Filtered
{
	/// <summary>
	///     Container for a filterable collection of <see cref="Assembly" />.
	/// </summary>
	public class Assemblies : Filtered<Assembly, Assemblies>,
		IDescribableSubject,
		ITypeAssemblies.IProtected,
		ITypeAssemblies.IPrivate
	{
		private readonly Func<string> _description;
		private readonly IEnumerable<Assembly?>? _unfilteredSource;

		/// <summary>
		///     Container for a filterable collection of <see cref="Assembly" />.
		/// </summary>
		public Assemblies(IEnumerable<Assembly?> source, string description)
			: this(source, () => description)
		{
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Assembly" />.
		/// </summary>
		public Assemblies(Assembly? source, string description) : this([source,], description)
		{
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Assembly" />, whose <paramref name="description" /> is
		///     only created for a failure, as formatting it is a large part of a successful expectation.
		/// </summary>
		internal Assemblies(Assembly? source, Func<string> description) : this([source,], description)
		{
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Assembly" />.
		/// </summary>
		internal Assemblies(Types types) : base(types.Select(type => type.Assembly).Distinct())
		{
			string description = "assemblies containing " + types.GetDescription();
			_description = () => description;
		}

		private Assemblies(IEnumerable<Assembly?> source, Func<string> description)
			: base(source.WhereNotNull())
		{
			_description = description;
			_unfilteredSource = source;
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Assembly" />.
		/// </summary>
		protected Assemblies(Assemblies inner) : this(inner, inner.Filters)
		{
		}

		private Assemblies(Assemblies inner, List<IFilter<Assembly>> filters) : base(inner, filters)
		{
			_description = inner._description;
			_unfilteredSource = inner._unfilteredSource;
		}

		/// <summary>
		///     The source of the assemblies when no filter applies, or <see langword="null" />.
		/// </summary>
		/// <remarks>
		///     The types of the assemblies are listed from it directly, as enumerating this collection asynchronously costs
		///     more than listing the types.
		/// </remarks>
		internal IEnumerable<Assembly?>? UnfilteredSource => Filters.Count == 0 ? _unfilteredSource : null;

		/// <summary>
		///     Filters for public types.
		/// </summary>
		public ITypeAssemblies Public => NewBuilder(MemberFilterState.Empty.WithAccess(AccessModifiers.Public));

		/// <summary>
		///     Filters for private types.
		/// </summary>
		public ITypeAssemblies.IPrivate Private => NewBuilder(MemberFilterState.Empty.WithAccess(AccessModifiers.Private));

		/// <summary>
		///     Filters for protected types.
		/// </summary>
		public ITypeAssemblies.IProtected Protected
			=> NewBuilder(MemberFilterState.Empty.WithAccess(AccessModifiers.Protected));

		/// <summary>
		///     Filters for internal types.
		/// </summary>
		public ITypeAssemblies Internal => NewBuilder(MemberFilterState.Empty.WithAccess(AccessModifiers.Internal));

		/// <inheritdoc />
		public string GetDescription()
		{
			string description = _description();
			foreach (IFilter<Assembly> filter in Filters)
			{
				description = filter.Describes(description);
			}

			return description;
		}

		/// <inheritdoc cref="ITypeAssemblies.IPrivate.Protected" />
		ITypeAssemblies ITypeAssemblies.IPrivate.Protected
			=> NewBuilder(MemberFilterState.Empty.WithAccess(AccessModifiers.PrivateProtected));

		/// <inheritdoc cref="ITypeAssemblies.Abstract" />
		public ILimitedAbstractSealedTypeAssemblies<ILimitedAbstractSealedTypeAssemblies> Abstract
			=> NewBuilder(MemberFilterState.Empty).Abstract;

		/// <inheritdoc cref="ITypeAssemblies.Sealed" />
		public ILimitedAbstractSealedTypeAssemblies<ILimitedAbstractSealedTypeAssemblies> Sealed
			=> NewBuilder(MemberFilterState.Empty).Sealed;

		/// <inheritdoc cref="ITypeAssemblies.Static" />
		public ILimitedStaticTypeAssemblies<ILimitedStaticTypeAssemblies> Static
			=> NewBuilder(MemberFilterState.Empty).Static;

		/// <inheritdoc cref="ILimitedStaticTypeAssemblies{TLimitedTypeAssemblies}.Generic" />
		public ITypeAssemblies Generic => NewBuilder(MemberFilterState.Empty).Generic;

		/// <inheritdoc cref="ILimitedStaticTypeAssemblies{TLimitedTypeAssemblies}.Nested" />
		public ITypeAssemblies Nested => NewBuilder(MemberFilterState.Empty).Nested;

		/// <inheritdoc cref="ITypeAssemblies.IProtected.Internal" />
		ITypeAssemblies ITypeAssemblies.IProtected.Internal
			=> NewBuilder(MemberFilterState.Empty.WithAccess(AccessModifiers.ProtectedInternal));

		/// <inheritdoc cref="ILimitedStaticTypeAssemblies.Types(AccessModifiers)" />
		public Types Types(AccessModifiers accessModifier = AccessModifiers.Any)
			=> NewBuilder(MemberFilterState.Empty).Types(accessModifier);

		/// <inheritdoc cref="ILimitedStaticTypeAssemblies.Classes(AccessModifiers)" />
		public Types Classes(AccessModifiers accessModifier = AccessModifiers.Any)
			=> NewBuilder(MemberFilterState.Empty).Classes(accessModifier);

		/// <inheritdoc cref="ILimitedAbstractSealedTypeAssemblies.Records(AccessModifiers)" />
		public Types Records(AccessModifiers accessModifier = AccessModifiers.Any)
			=> NewBuilder(MemberFilterState.Empty).Records(accessModifier);

		/// <inheritdoc cref="ITypeAssemblies.RecordStructs(AccessModifiers)" />
		public Types RecordStructs(AccessModifiers accessModifier = AccessModifiers.Any)
			=> NewBuilder(MemberFilterState.Empty).RecordStructs(accessModifier);

		/// <inheritdoc cref="ITypeAssemblies.Structs(AccessModifiers)" />
		public Types Structs(AccessModifiers accessModifier = AccessModifiers.Any)
			=> NewBuilder(MemberFilterState.Empty).Structs(accessModifier);

		/// <inheritdoc cref="ITypeAssemblies.Interfaces(AccessModifiers)" />
		public Types Interfaces(AccessModifiers accessModifier = AccessModifiers.Any)
			=> NewBuilder(MemberFilterState.Empty).Interfaces(accessModifier);

		/// <inheritdoc cref="ITypeAssemblies.Enums(AccessModifiers)" />
		public Types Enums(AccessModifiers accessModifier = AccessModifiers.Any)
			=> NewBuilder(MemberFilterState.Empty).Enums(accessModifier);

		/// <inheritdoc cref="ILimitedStaticTypeAssemblies.Constructors()" />
		public Constructors Constructors() => NewBuilder(MemberFilterState.Empty).Constructors();

		/// <inheritdoc cref="ILimitedStaticTypeAssemblies.Events()" />
		public Events Events() => NewBuilder(MemberFilterState.Empty).Events();

		/// <inheritdoc cref="ILimitedStaticTypeAssemblies.Fields()" />
		public Fields Fields() => NewBuilder(MemberFilterState.Empty).Fields();

		/// <inheritdoc cref="ILimitedStaticTypeAssemblies.Methods()" />
		public Methods Methods() => NewBuilder(MemberFilterState.Empty).Methods();

		/// <inheritdoc cref="ILimitedStaticTypeAssemblies.Properties()" />
		public Properties Properties() => NewBuilder(MemberFilterState.Empty).Properties();

		/// <inheritdoc />
		private protected override Assemblies CloneWith(List<IFilter<Assembly>> filters)
			=> new(this, filters);

		private AssembliesTypeBuilder NewBuilder(MemberFilterState memberState)
			=> new(this, memberState, new List<Func<Type, bool>>(), "");

		/// <summary>
		///     A Container for a filterable collection of <see cref="Assembly" />,
		///     that also allows specifying string equality options via <see cref="StringEqualityOptionsExtensions" />.
		/// </summary>
		public class StringEqualityResult : Assemblies, IOptionsProvider<StringEqualityOptions>, IStringMatchTypeOptions
		{
			private readonly StringEqualityOptions _options;

			internal StringEqualityResult(Assemblies inner, StringEqualityOptions options) : base(inner)
			{
				_options = options;
			}

			/// <inheritdoc cref="IOptionsProvider{StringEqualityOptions}.Options" />
			StringEqualityOptions IOptionsProvider<StringEqualityOptions>.Options => _options;
		}
	}
}
