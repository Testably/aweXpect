using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Helpers;
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
			_description = () => "assemblies containing " + types.GetDescription();
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Assembly" />, whose <paramref name="description" /> is
		///     only created for a failure, as formatting it is a large part of a successful expectation.
		/// </summary>
		internal Assemblies(IEnumerable<Assembly?> source, Func<string> description)
			: base(source.WhereNotNull())
		{
			_description = description;
			_unfilteredSource = source;
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Assembly" />.
		/// </summary>
		private protected Assemblies(Assemblies inner) : this(inner, inner.Filters)
		{
		}

		private Assemblies(Assemblies inner, IReadOnlyList<IFilter<Assembly>> filters) : base(inner, filters)
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

		/// <inheritdoc cref="ILimitedStaticTypeAssemblies.Types()" />
		public Types Types() => NewBuilder(MemberFilterState.Empty).Types();

		/// <inheritdoc cref="ILimitedStaticTypeAssemblies.Classes()" />
		public Types Classes() => NewBuilder(MemberFilterState.Empty).Classes();

		/// <inheritdoc cref="ILimitedAbstractSealedTypeAssemblies.Records()" />
		public Types Records() => NewBuilder(MemberFilterState.Empty).Records();

		/// <inheritdoc cref="ITypeAssemblies.RecordStructs()" />
		public Types RecordStructs() => NewBuilder(MemberFilterState.Empty).RecordStructs();

		/// <inheritdoc cref="ITypeAssemblies.Structs()" />
		public Types Structs() => NewBuilder(MemberFilterState.Empty).Structs();

		/// <inheritdoc cref="ITypeAssemblies.Interfaces()" />
		public Types Interfaces() => NewBuilder(MemberFilterState.Empty).Interfaces();

		/// <inheritdoc cref="ITypeAssemblies.Enums()" />
		public Types Enums() => NewBuilder(MemberFilterState.Empty).Enums();

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
		private protected override Assemblies CloneWith(IReadOnlyList<IFilter<Assembly>> filters)
			=> new(this, filters);

		private AssembliesTypeBuilder NewBuilder(MemberFilterState memberState)
			=> new(this, memberState, new List<Func<Type, bool>>(), "");

		/// <summary>
		///     A Container for a filterable collection of <see cref="Assembly" />,
		///     that also allows specifying string equality options via <see cref="StringEqualityOptionsExtensions" />.
		/// </summary>
		public class StringEqualityResult : Assemblies, IOptionsProvider<StringEqualityOptions>, IStringMatchTypeOptions,
			ICopyOnOption<StringEqualityOptions, StringEqualityResult>
		{
			private readonly Func<IReadOnlyOptionsHolder<StringEqualityOptions>, IFilter<Assembly>> _createFilter;
			private readonly OptionsHolder<StringEqualityOptions> _options;
			private readonly Assemblies _source;

			internal StringEqualityResult(Assemblies source, Func<IReadOnlyOptionsHolder<StringEqualityOptions>, IFilter<Assembly>> createFilter,
				OptionsHolder<StringEqualityOptions> options) : base(source.Which(createFilter(options)))
			{
				_source = source;
				_createFilter = createFilter;
				_options = options;
			}

			/// <inheritdoc cref="IOptionsProvider{StringEqualityOptions}.Options" />
			OptionsHolder<StringEqualityOptions> IOptionsProvider<StringEqualityOptions>.Options => _options;

			/// <inheritdoc cref="ICopyOnOption{TOptions, TSelf}.WithOptions(TOptions)" />
			StringEqualityResult ICopyOnOption<StringEqualityOptions, StringEqualityResult>.WithOptions(
				StringEqualityOptions options)
				=> new(_source, _createFilter, new OptionsHolder<StringEqualityOptions>(options));
		}
	}
}
