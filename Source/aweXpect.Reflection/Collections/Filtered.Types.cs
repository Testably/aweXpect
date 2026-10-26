using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Customization;
using aweXpect.Options;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Results;

// ReSharper disable MemberHidesStaticFromOuterClass

namespace aweXpect.Reflection.Collections;

public static partial class Filtered
{
	/// <summary>
	///     Container for a filterable collection of <see cref="Type" />.
	/// </summary>
	public class Types : Filtered<Type, Types>,
		IDescribableSubject,
		IMembers.IPrivate,
		IMembers.IProtected,
		ILimitedAbstractSealedMembers
	{
		private const string DeclaringTypesPrefix = "declaring types of ";

		private static readonly ConditionalWeakTable<Assembly, LoadableTypes> LoadableTypesCache = new();

		private readonly Assemblies? _assemblies;
		private readonly Func<string> _description;

		/// <summary>
		///     Container for a filterable collection of <see cref="Type" />.
		/// </summary>
		internal Types(IEnumerable<Type?> source, Func<string> description)
			: base(source.WhereNotNull())
		{
			_description = description;
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Type" />.
		/// </summary>
		internal Types(Assemblies assemblies, string description) : base(
			assemblies.UnfilteredSource is { } source
				? source.SelectManyNotNull(GetLoadableTypes)
				: assemblies.SelectMany(GetLoadableTypes))
		{
			_assemblies = assemblies;
			_description = () => description;
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Type" />.
		/// </summary>
		internal Types(Constructors constructors) : base(constructors
			.Select(constructorInfo => constructorInfo.DeclaringType)
			.WhereNotNull()
			.Distinct())
		{
			_description = () => DeclaringTypesPrefix + constructors.GetDescription();
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Type" />.
		/// </summary>
		internal Types(Properties properties) : base(properties
			.Select(propertyInfo => propertyInfo.DeclaringType)
			.WhereNotNull()
			.Distinct())
		{
			_description = () => DeclaringTypesPrefix + properties.GetDescription();
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Type" />.
		/// </summary>
		internal Types(Methods methods) : base(methods
			.Select(methodInfo => methodInfo.DeclaringType)
			.WhereNotNull()
			.Distinct())
		{
			_description = () => DeclaringTypesPrefix + methods.GetDescription();
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Type" />.
		/// </summary>
		internal Types(Fields fields) : base(fields
			.Select(fieldInfo => fieldInfo.DeclaringType)
			.WhereNotNull()
			.Distinct())
		{
			_description = () => DeclaringTypesPrefix + fields.GetDescription();
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Type" />.
		/// </summary>
		internal Types(Events events) : base(events
			.Select(eventInfo => eventInfo.DeclaringType)
			.WhereNotNull()
			.Distinct())
		{
			_description = () => DeclaringTypesPrefix + events.GetDescription();
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Type" />.
		/// </summary>
		private protected Types(Types inner) : this(inner, inner.Filters)
		{
		}

		private Types(Types inner, IReadOnlyList<IFilter<Type>> filters) : base(inner, filters)
		{
			_description = inner._description;
			_assemblies = inner._assemblies;
		}

		/// <summary>
		///     Filters for public members.
		/// </summary>
		public IMembers Public => new TypesMemberBuilder(this, MemberFilterState.Empty.WithAccess(AccessModifiers.Public));

		/// <summary>
		///     Filters for private members.
		/// </summary>
		public IMembers.IPrivate Private
			=> new TypesMemberBuilder(this, MemberFilterState.Empty.WithAccess(AccessModifiers.Private));

		/// <summary>
		///     Filters for protected members.
		/// </summary>
		public IMembers.IProtected Protected
			=> new TypesMemberBuilder(this, MemberFilterState.Empty.WithAccess(AccessModifiers.Protected));

		/// <summary>
		///     Filters for internal members.
		/// </summary>
		public IMembers Internal
			=> new TypesMemberBuilder(this, MemberFilterState.Empty.WithAccess(AccessModifiers.Internal));

		/// <inheritdoc />
		public string GetDescription()
		{
			string description = _description();
			foreach (IFilter<Type> filter in Filters)
			{
				description = filter.Describes(description);
			}

			if (_assemblies is not null)
			{
				return description + _assemblies.GetDescription();
			}

			return description;
		}

		/// <summary>
		///     Filters for static members.
		/// </summary>
		public IMemberSelectors Static => new TypesMemberBuilder(this, MemberFilterState.Empty.WithStatic());

		/// <summary>
		///     Filters for abstract members.
		/// </summary>
		public ILimitedAbstractSealedMembers Abstract
			=> new TypesMemberBuilder(this, MemberFilterState.Empty.WithAbstract());

		/// <summary>
		///     Filters for sealed members.
		/// </summary>
		public ILimitedAbstractSealedMembers Sealed
			=> new TypesMemberBuilder(this, MemberFilterState.Empty.WithSealed());

		/// <inheritdoc cref="IMembers.IPrivate.Protected" />
		IMembers IMembers.IPrivate.Protected
			=> new TypesMemberBuilder(this, MemberFilterState.Empty.WithAccess(AccessModifiers.PrivateProtected));

		/// <summary>
		///     Get all constructors in the filtered types.
		/// </summary>
		public Constructors Constructors() => new TypesMemberBuilder(this, MemberFilterState.Empty).Constructors();

		/// <inheritdoc cref="IMemberSelectors.Events()" />
		public Events.MemberScopeResult Events()
			=> new TypesMemberBuilder(this, MemberFilterState.Empty).Events();

		/// <inheritdoc cref="IMemberSelectors.Fields()" />
		public Fields.MemberScopeResult Fields()
			=> new TypesMemberBuilder(this, MemberFilterState.Empty).Fields();

		/// <inheritdoc cref="IMemberSelectors.Methods()" />
		public Methods.MemberScopeResult Methods()
			=> new TypesMemberBuilder(this, MemberFilterState.Empty).Methods();

		/// <inheritdoc cref="IMemberSelectors.Properties()" />
		public Properties.MemberScopeResult Properties()
			=> new TypesMemberBuilder(this, MemberFilterState.Empty).Properties();

		/// <summary>
		///     Get all events in the filtered types in the <paramref name="memberScope" />.
		/// </summary>
		internal Events Events(MemberScopes memberScope) => new(this, "events ", memberScope);

		/// <summary>
		///     Get all fields in the filtered types in the <paramref name="memberScope" />.
		/// </summary>
		internal Fields Fields(MemberScopes memberScope) => new(this, "fields ", memberScope);

		/// <summary>
		///     Get all methods in the filtered types in the <paramref name="memberScope" />.
		/// </summary>
		internal Methods Methods(MemberScopes memberScope) => new(this, "methods ", memberScope);

		/// <summary>
		///     Get all properties in the filtered types in the <paramref name="memberScope" />.
		/// </summary>
		internal Properties Properties(MemberScopes memberScope) => new(this, "properties ", memberScope);

		/// <inheritdoc cref="IMembers.IProtected.Internal" />
		IMembers IMembers.IProtected.Internal
			=> new TypesMemberBuilder(this, MemberFilterState.Empty.WithAccess(AccessModifiers.ProtectedInternal));

		/// <inheritdoc />
		private protected override Types CloneWith(IReadOnlyList<IFilter<Type>> filters)
			=> new(this, filters);

		/// <summary>
		///     Gets the types of the <paramref name="assembly" />, ignoring those that fail to load.
		/// </summary>
		/// <remarks>
		///     <see cref="Assembly.GetTypes()" /> throws a <see cref="ReflectionTypeLoadException" /> when any type cannot be
		///     loaded (e.g. an unresolvable dependency). The exception still exposes the types that loaded successfully via
		///     <see cref="ReflectionTypeLoadException.Types" />, so we fall back to those instead of failing the whole query.
		///     <para />
		///     The types of a loaded assembly never change, so they are cached per assembly, as reading them and checking
		///     each for <see cref="CompilerGeneratedAttribute" /> is the main cost of an
		///     architecture rule. A dynamic assembly can still define types and a type that failed to load might load
		///     later, so these are read again every time.
		/// </remarks>
		private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
		{
			if (!ReflectionFallback.IsSupported)
			{
				throw ReflectionFallbackHelpers.NotSupported(assembly, "types");
			}

			bool includeCompilerGenerated = (Customize.aweXpect.Reflection().IncludedCompilerGeneratedMembers.Get() &
			                                 CompilerGeneratedMembers.Types) != 0;
			if (LoadableTypesCache.TryGetValue(assembly, out LoadableTypes? cached))
			{
				return cached.Get(includeCompilerGenerated);
			}

			Type[] types;
			bool isComplete = !assembly.IsDynamic;
			try
			{
				types = assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException exception)
			{
				types = exception.Types.Where(type => type is not null).ToArray()!;
				isComplete = false;
			}

			LoadableTypes loadableTypes = new(types);
			if (isComplete)
			{
				loadableTypes = LoadableTypesCache.GetValue(assembly, _ => loadableTypes);
			}

			return loadableTypes.Get(includeCompilerGenerated);
		}

		/// <summary>
		///     Get all assemblies of the filtered types.
		/// </summary>
		public Assemblies Assemblies() => new(this);

		private sealed class LoadableTypes(Type[] types)
		{
			private Type[]? _withoutCompilerGenerated;

			public Type[] Get(bool includeCompilerGenerated)
				=> includeCompilerGenerated
					? types
					: _withoutCompilerGenerated ??= Array.FindAll(types, type => !type.IsCompilerGenerated());
		}

		/// <summary>
		///     A Container for a filterable collection of <see cref="Type" />,
		///     that also allows specifying string equality options via <see cref="StringEqualityOptionsExtensions" />.
		/// </summary>
		public class StringEqualityResult : Types, IOptionsProvider<StringEqualityOptions>, IStringMatchTypeOptions,
			ICopyOnOption<StringEqualityOptions, StringEqualityResult>
		{
			private readonly Func<IReadOnlyOptionsHolder<StringEqualityOptions>, IFilter<Type>> _createFilter;
			private readonly OptionsHolder<StringEqualityOptions> _options;
			private readonly Types _source;

			internal StringEqualityResult(Types source, Func<IReadOnlyOptionsHolder<StringEqualityOptions>, IFilter<Type>> createFilter,
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

		/// <summary>
		///     A filtered collection of <see cref="System.Type" /> from a namespace-based dependency filter, allowing
		///     to widen the targeted/allowed namespaces and to opt out of sub-namespace matching.
		/// </summary>
		/// <remarks>
		///     Like all filtered collections, this is an immutable value object: <see cref="OrOn(string[])" /> and
		///     <see cref="ExcludingSubNamespaces" /> do not mutate this instance but rebuild a fresh filter from the
		///     original base collection, so deriving multiple views from the same instance cannot corrupt each other.
		/// </remarks>
		public sealed class NamespaceDependencyFilterResult : Types
		{
			private readonly Func<NamespaceDependencyOptions, Types> _build;
			private readonly NamespaceDependencyOptions _options;

			internal NamespaceDependencyFilterResult(
				NamespaceDependencyOptions options,
				Func<NamespaceDependencyOptions, Types> build)
				: base(build(options))
			{
				_options = options;
				_build = build;
			}

			/// <summary>
			///     Widens the filter by the given <paramref name="namespaces" /> (including sub-namespaces unless
			///     <see cref="ExcludingSubNamespaces" /> is used).
			/// </summary>
			public NamespaceDependencyFilterResult OrOn(params string[] namespaces)
				=> OrOn(namespaces.AsEnumerable());

			/// <summary>
			///     Widens the filter by the given <paramref name="namespaces" /> (including sub-namespaces unless
			///     <see cref="ExcludingSubNamespaces" /> is used).
			/// </summary>
			public NamespaceDependencyFilterResult OrOn(IEnumerable<string> namespaces)
			{
				NamespaceDependencyOptions widened = _options.OrOn(namespaces);
				return new NamespaceDependencyFilterResult(widened, _build);
			}

			/// <summary>
			///     Excludes sub-namespaces from matching for the whole filter (including any <see cref="OrOn(string[])" /> additions).
			/// </summary>
			/// <remarks>
			///     Without this call, a namespace matches itself and all its sub-namespaces (so <c>Foo.Bar</c> includes
			///     <c>Foo.Bar.Baz</c> but not <c>Foo.BarBaz</c>).
			/// </remarks>
			public NamespaceDependencyFilterResult ExcludingSubNamespaces()
			{
				NamespaceDependencyOptions refined = _options.ExcludingSubNamespaces();
				return new NamespaceDependencyFilterResult(refined, _build);
			}
		}

		/// <summary>
		///     A filtered collection of <see cref="System.Type" /> from a namespace-based depends-only-on filter,
		///     allowing to widen the allowed namespaces and to opt out of sub-namespace matching — for the allowed
		///     namespaces and for the type's own namespace.
		/// </summary>
		/// <remarks>
		///     Like all filtered collections, this is an immutable value object: <see cref="OrOn(string[])" />,
		///     <see cref="ExcludingSubNamespaces" /> and <see cref="ExcludingOwnSubNamespaces" /> do not mutate this
		///     instance but rebuild a fresh filter from the original base collection, so deriving multiple views from
		///     the same instance cannot corrupt each other.
		/// </remarks>
		public sealed class NamespaceDependencyOnlyOnFilterResult : Types
		{
			private readonly Func<NamespaceDependencyOptions, Types> _build;
			private readonly NamespaceDependencyOptions _options;

			internal NamespaceDependencyOnlyOnFilterResult(
				NamespaceDependencyOptions options,
				Func<NamespaceDependencyOptions, Types> build)
				: base(build(options))
			{
				_options = options;
				_build = build;
			}

			/// <summary>
			///     Widens the filter by the given <paramref name="namespaces" /> (including sub-namespaces unless
			///     <see cref="ExcludingSubNamespaces" /> is used).
			/// </summary>
			public NamespaceDependencyOnlyOnFilterResult OrOn(params string[] namespaces)
				=> OrOn(namespaces.AsEnumerable());

			/// <summary>
			///     Widens the filter by the given <paramref name="namespaces" /> (including sub-namespaces unless
			///     <see cref="ExcludingSubNamespaces" /> is used).
			/// </summary>
			public NamespaceDependencyOnlyOnFilterResult OrOn(IEnumerable<string> namespaces)
			{
				NamespaceDependencyOptions widened = _options.OrOn(namespaces);
				return new NamespaceDependencyOnlyOnFilterResult(widened, _build);
			}

			/// <summary>
			///     Excludes sub-namespaces of the allowed namespaces from matching for the whole filter (including any
			///     <see cref="OrOn(string[])" /> additions).
			/// </summary>
			/// <remarks>
			///     Without this call, a namespace matches itself and all its sub-namespaces (so <c>Foo.Bar</c> includes
			///     <c>Foo.Bar.Baz</c> but not <c>Foo.BarBaz</c>).
			///     <para />
			///     The type's own namespace is always allowed, and its sub-namespaces stay allowed unless
			///     <see cref="ExcludingOwnSubNamespaces" /> is also used.
			/// </remarks>
			public NamespaceDependencyOnlyOnFilterResult ExcludingSubNamespaces()
			{
				NamespaceDependencyOptions refined = _options.ExcludingSubNamespaces();
				return new NamespaceDependencyOnlyOnFilterResult(refined, _build);
			}

			/// <summary>
			///     Excludes sub-namespaces of the type's own namespace from being implicitly allowed (so a <c>Foo</c>
			///     type referencing <c>Foo.Bar</c> is filtered out unless <c>Foo.Bar</c> is explicitly allowed).
			/// </summary>
			/// <remarks>
			///     The type's own namespace itself is always allowed.
			/// </remarks>
			public NamespaceDependencyOnlyOnFilterResult ExcludingOwnSubNamespaces()
			{
				NamespaceDependencyOptions refined = _options.ExcludingOwnSubNamespaces();
				return new NamespaceDependencyOnlyOnFilterResult(refined, _build);
			}
		}

		/// <summary>
		///     A filtered collection of <see cref="System.Type" /> from a namespace-based has-dependencies-outside
		///     filter, allowing to widen the allowed namespaces and to opt out of sub-namespace matching — for the
		///     allowed namespaces and for the type's own namespace.
		/// </summary>
		/// <remarks>
		///     Like all filtered collections, this is an immutable value object: <see cref="OrOn(string[])" />,
		///     <see cref="ExcludingSubNamespaces" /> and <see cref="ExcludingOwnSubNamespaces" /> do not mutate this
		///     instance but rebuild a fresh filter from the original base collection, so deriving multiple views from
		///     the same instance cannot corrupt each other.
		/// </remarks>
		public sealed class NamespaceDependencyOutsideFilterResult : Types
		{
			private readonly Func<NamespaceDependencyOptions, Types> _build;
			private readonly NamespaceDependencyOptions _options;

			internal NamespaceDependencyOutsideFilterResult(
				NamespaceDependencyOptions options,
				Func<NamespaceDependencyOptions, Types> build)
				: base(build(options))
			{
				_options = options;
				_build = build;
			}

			/// <summary>
			///     Widens the allowed set by the given <paramref name="namespaces" /> (including sub-namespaces unless
			///     <see cref="ExcludingSubNamespaces" /> is used), so that dependencies on them no longer count as
			///     outside.
			/// </summary>
			public NamespaceDependencyOutsideFilterResult OrOn(params string[] namespaces)
				=> OrOn(namespaces.AsEnumerable());

			/// <summary>
			///     Widens the allowed set by the given <paramref name="namespaces" /> (including sub-namespaces unless
			///     <see cref="ExcludingSubNamespaces" /> is used), so that dependencies on them no longer count as
			///     outside.
			/// </summary>
			public NamespaceDependencyOutsideFilterResult OrOn(IEnumerable<string> namespaces)
			{
				NamespaceDependencyOptions widened = _options.OrOn(namespaces);
				return new NamespaceDependencyOutsideFilterResult(widened, _build);
			}

			/// <summary>
			///     Excludes sub-namespaces of the allowed namespaces from matching for the whole filter (including any
			///     <see cref="OrOn(string[])" /> additions), so that dependencies on them count as outside.
			/// </summary>
			/// <remarks>
			///     Without this call, a namespace matches itself and all its sub-namespaces (so <c>Foo.Bar</c> includes
			///     <c>Foo.Bar.Baz</c> but not <c>Foo.BarBaz</c>).
			///     <para />
			///     The type's own namespace never counts as outside, and neither do its sub-namespaces unless
			///     <see cref="ExcludingOwnSubNamespaces" /> is also used.
			/// </remarks>
			public NamespaceDependencyOutsideFilterResult ExcludingSubNamespaces()
			{
				NamespaceDependencyOptions refined = _options.ExcludingSubNamespaces();
				return new NamespaceDependencyOutsideFilterResult(refined, _build);
			}

			/// <summary>
			///     Excludes sub-namespaces of the type's own namespace from being implicitly allowed (so a <c>Foo</c>
			///     type referencing <c>Foo.Bar</c> is selected unless <c>Foo.Bar</c> is explicitly allowed).
			/// </summary>
			/// <remarks>
			///     The type's own namespace itself never counts as outside.
			/// </remarks>
			public NamespaceDependencyOutsideFilterResult ExcludingOwnSubNamespaces()
			{
				NamespaceDependencyOptions refined = _options.ExcludingOwnSubNamespaces();
				return new NamespaceDependencyOutsideFilterResult(refined, _build);
			}
		}

		/// <summary>
		///     A filtered collection of <see cref="System.Type" /> within a namespace, that also allows clarifying
		///     the assembly source once (it defaults to all loaded assemblies).
		/// </summary>
		/// <remarks>
		///     The clarification methods return a plain <see cref="Types" /> collection rooted at the given source,
		///     so the source can only be specified once and must be specified before applying further filters.
		/// </remarks>
		public sealed class InNamespaceResult : Types
		{
			private readonly string _namespace;

			internal InNamespaceResult(string @namespace)
				: base(In.AllLoadedAssemblies().Types().InNamespace(@namespace))
			{
				_namespace = @namespace;
			}

			/// <summary>
			///     Clarifies that the types within the namespace are searched in all loaded assemblies from the
			///     current <see cref="System.AppDomain.CurrentDomain" /> (the default).
			/// </summary>
			public Types InAllLoadedAssemblies()
				=> From(In.AllLoadedAssemblies());

			/// <summary>
			///     Clarifies that the types within the namespace are searched in the given <paramref name="assemblies" />.
			/// </summary>
			public Types InAssemblies(params Assembly?[] assemblies)
				=> InAssemblies(assemblies.AsEnumerable());

			/// <summary>
			///     Clarifies that the types within the namespace are searched in the given <paramref name="assemblies" />.
			/// </summary>
			public Types InAssemblies(IEnumerable<Assembly?> assemblies)
				=> From(In.Assemblies(assemblies));

			/// <summary>
			///     Clarifies that the types within the namespace are searched in the assembly that contains
			///     the <typeparamref name="TType" />.
			/// </summary>
			public Types InAssemblyContaining<TType>()
				=> From(In.AssemblyContaining<TType>());

			/// <summary>
			///     Clarifies that the types within the namespace are searched in the assembly that contains
			///     the <paramref name="type" />.
			/// </summary>
			public Types InAssemblyContaining(Type type)
				=> From(In.AssemblyContaining(type));

			private Types From(Assemblies assemblies)
				=> assemblies.Types().InNamespace(_namespace);
		}

		/// <summary>
		///     A filtered collection of <see cref="System.Type" /> from a specific-type dependency filter, allowing to
		///     widen the targeted types.
		/// </summary>
		/// <remarks>
		///     Like all filtered collections, this is an immutable value object: <see cref="OrOn{T}" /> and
		///     <see cref="OrOn(System.Type)" /> do not mutate this instance but rebuild a fresh filter from the original
		///     base collection, so deriving multiple views from the same instance cannot corrupt each other.
		/// </remarks>
		public sealed class TypeDependencyFilterResult : Types
		{
			private readonly Func<TypeDependencyOptions, Types> _build;
			private readonly TypeDependencyOptions _options;

			internal TypeDependencyFilterResult(
				TypeDependencyOptions options,
				Func<TypeDependencyOptions, Types> build)
				: base(build(options))
			{
				_options = options;
				_build = build;
			}

			/// <summary>
			///     Widens the filter by the type <typeparamref name="T" />.
			/// </summary>
			public TypeDependencyFilterResult OrOn<T>()
				=> OrOn(typeof(T));

			/// <summary>
			///     Widens the filter by the <paramref name="type" />.
			/// </summary>
			public TypeDependencyFilterResult OrOn(Type type)
				=> new(_options.OrOn(type), _build);
		}

		/// <summary>
		///     A filtered collection of <see cref="System.Type" /> from a dependency filter whose targets are filtered
		///     collections of types, allowing to widen the targeted/allowed collections.
		/// </summary>
		/// <remarks>
		///     Like all filtered collections, this is an immutable value object: <see cref="OrOn" /> does not mutate
		///     this instance but rebuilds a fresh filter from the original base collection, so deriving multiple views
		///     from the same instance cannot corrupt each other.
		/// </remarks>
		public sealed class TypeSetDependencyFilterResult : Types
		{
			private readonly Func<TypeSetDependencyOptions, Types> _build;
			private readonly TypeSetDependencyOptions _options;

			internal TypeSetDependencyFilterResult(
				TypeSetDependencyOptions options,
				Func<TypeSetDependencyOptions, Types> build)
				: base(build(options))
			{
				_options = options;
				_build = build;
			}

			/// <summary>
			///     Widens the filter by the given <paramref name="targets" />.
			/// </summary>
			public TypeSetDependencyFilterResult OrOn(params Types[] targets)
			{
				TypeSetDependencyOptions widened = _options.OrOn(targets);
				return new TypeSetDependencyFilterResult(widened, _build);
			}
		}

		/// <summary>
		///     A filtered collection of <see cref="System.Type" /> from a depends-only-on filter whose allowed
		///     targets are filtered collections of types, allowing to widen the allowed collections and to opt out
		///     of the implicit allowance of the type's own sub-namespaces.
		/// </summary>
		/// <remarks>
		///     Like all filtered collections, this is an immutable value object: <see cref="OrOn" /> and
		///     <see cref="ExcludingOwnSubNamespaces" /> do not mutate this instance but rebuild a fresh filter from
		///     the original base collection, so deriving multiple views from the same instance cannot corrupt each
		///     other.
		/// </remarks>
		public sealed class TypeSetDependencyOnlyOnFilterResult : Types
		{
			private readonly Func<TypeSetDependencyOptions, Types> _build;
			private readonly TypeSetDependencyOptions _options;

			internal TypeSetDependencyOnlyOnFilterResult(
				TypeSetDependencyOptions options,
				Func<TypeSetDependencyOptions, Types> build)
				: base(build(options))
			{
				_options = options;
				_build = build;
			}

			/// <summary>
			///     Widens the filter by the given <paramref name="targets" />.
			/// </summary>
			public TypeSetDependencyOnlyOnFilterResult OrOn(params Types[] targets)
			{
				TypeSetDependencyOptions widened = _options.OrOn(targets);
				return new TypeSetDependencyOnlyOnFilterResult(widened, _build);
			}

			/// <summary>
			///     Excludes sub-namespaces of the type's own namespace from being implicitly allowed (so a <c>Foo</c>
			///     type referencing <c>Foo.Bar</c> is filtered out unless <c>Foo.Bar</c> types are part of an allowed
			///     collection).
			/// </summary>
			/// <remarks>
			///     The type's own namespace itself is always allowed.
			/// </remarks>
			public TypeSetDependencyOnlyOnFilterResult ExcludingOwnSubNamespaces()
			{
				TypeSetDependencyOptions refined = _options.ExcludingOwnSubNamespaces();
				return new TypeSetDependencyOnlyOnFilterResult(refined, _build);
			}
		}

		/// <summary>
		///     A filtered collection of <see cref="System.Type" /> from a has-dependencies-outside filter whose
		///     allowed targets are filtered collections of types, allowing to widen the allowed collections and to
		///     opt out of the implicit allowance of the type's own sub-namespaces.
		/// </summary>
		/// <remarks>
		///     Like all filtered collections, this is an immutable value object: <see cref="OrOn" /> and
		///     <see cref="ExcludingOwnSubNamespaces" /> do not mutate this instance but rebuild a fresh filter from
		///     the original base collection, so deriving multiple views from the same instance cannot corrupt each
		///     other.
		/// </remarks>
		public sealed class TypeSetDependencyOutsideFilterResult : Types
		{
			private readonly Func<TypeSetDependencyOptions, Types> _build;
			private readonly TypeSetDependencyOptions _options;

			internal TypeSetDependencyOutsideFilterResult(
				TypeSetDependencyOptions options,
				Func<TypeSetDependencyOptions, Types> build)
				: base(build(options))
			{
				_options = options;
				_build = build;
			}

			/// <summary>
			///     Widens the allowed set by the given <paramref name="targets" />, so that dependencies on their
			///     types no longer count as outside.
			/// </summary>
			public TypeSetDependencyOutsideFilterResult OrOn(params Types[] targets)
			{
				TypeSetDependencyOptions widened = _options.OrOn(targets);
				return new TypeSetDependencyOutsideFilterResult(widened, _build);
			}

			/// <summary>
			///     Excludes sub-namespaces of the type's own namespace from being implicitly allowed (so a <c>Foo</c>
			///     type referencing <c>Foo.Bar</c> is selected unless <c>Foo.Bar</c> types are part of an allowed
			///     collection).
			/// </summary>
			/// <remarks>
			///     The type's own namespace itself never counts as outside.
			/// </remarks>
			public TypeSetDependencyOutsideFilterResult ExcludingOwnSubNamespaces()
			{
				TypeSetDependencyOptions refined = _options.ExcludingOwnSubNamespaces();
				return new TypeSetDependencyOutsideFilterResult(refined, _build);
			}
		}
	}
}
