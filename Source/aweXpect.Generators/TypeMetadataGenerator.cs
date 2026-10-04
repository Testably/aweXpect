using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace aweXpect.Generators;

/// <summary>
///     The <see cref="IIncrementalGenerator" /> that registers the members of every type reaching an equivalency
///     comparison and the events of every type reaching an event recording in
///     <c>aweXpect.Core.Metadata.TypeMetadataRegistry</c>.
/// </summary>
/// <remarks>
///     A call site whose parameter or type parameter carries <c>RequiresMemberMetadataAttribute</c> seeds the static
///     type of its argument, and the walk follows the declared members transitively. One carrying
///     <c>RequiresEventMetadataAttribute</c> seeds the events of the static type. Without the registration, the
///     comparison or recording has to reflect over the type, which publishing with trimming or Native AOT enabled
///     removes.
/// </remarks>
[Generator]
public class TypeMetadataGenerator : IIncrementalGenerator
{
	private const string MarkerAttribute = "aweXpect.Core.Metadata.RequiresMemberMetadataAttribute";
	private const string EventMarkerAttribute = "aweXpect.Core.Metadata.RequiresEventMetadataAttribute";
	private const string GenerateAttribute = "aweXpect.Core.Metadata.GenerateMetadataAttribute";
	private const string Registry = "global::aweXpect.Core.Metadata.TypeMetadataRegistry";
	private const string CoreAssembly = "aweXpect.Core";
	private const string ExperimentalAttribute = "System.Diagnostics.CodeAnalysis.ExperimentalAttribute";

	private static readonly string[] MarkerNames =
	[
		"RequiresMemberMetadata", "RequiresMemberMetadataAttribute",
		"RequiresEventMetadata", "RequiresEventMetadataAttribute",
	];

	private static readonly ConditionalWeakTable<Compilation, HashSet<string>> MarkedMethodNames = new();
	private static readonly ConditionalWeakTable<SyntaxTree, string[]> DeclaredMarkedMethodNames = new();
	private static readonly ConditionalWeakTable<IAssemblySymbol, string[]> ReferencedMarkedMethodNames = new();

	/// <summary>
	///     Reported for a type named in <c>GenerateMetadataAttribute</c> that yields no registration.
	/// </summary>
	private static readonly DiagnosticDescriptor NothingToRegister = new(
		"aweXpect2001",
		"The type yields no metadata registration",
		"'{0}' yields no metadata registration, because it is compared by value, enumerated, abstract, an open generic, not accessible from this assembly, has neither public instance members nor public events, or has a member or event the generated code cannot name; it stays on the reflection path",
		"aweXpect.Generators",
		DiagnosticSeverity.Warning,
		true,
		"The generator registers a type named in GenerateMetadataAttribute, so that an equivalency comparison or event recording does not have to reflect over it, which publishing with trimming or Native AOT enabled would break. A type it cannot register stays on the reflection path.",
		"https://docs.testably.org/aweXpect/analyzers#metadata-generator");

	private static readonly SymbolDisplayFormat TypeFormat = SymbolDisplayFormat.FullyQualifiedFormat
		.WithMiscellaneousOptions(SymbolDisplayFormat.FullyQualifiedFormat.MiscellaneousOptions &
		                          ~SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

	void IIncrementalGenerator.Initialize(IncrementalGeneratorInitializationContext context)
	{
		IncrementalValueProvider<bool> isSupported = context.CompilationProvider
			.Select(static (c, _) => SupportsRegistration(c) && HasModuleInitializer(c) && HasLanguageVersion(c));

		IncrementalValueProvider<bool> supportsBatch = context.CompilationProvider
			.Select(static (c, _) => SupportsBatch(c));

		IncrementalValueProvider<EquatableArray<TypeRegistration>> fromCallSites = context.SyntaxProvider
			.CreateSyntaxProvider(
				static (node, _) => node is InvocationExpressionSyntax,
				static (ctx, cancellationToken) => FromCallSite(ctx, cancellationToken))
			.WithTrackingName("CallSites")
			.SelectMany(static (x, _) => x.Values)
			.Collect()
			.Select(static (x, _) => new EquatableArray<TypeRegistration>(x));

		IncrementalValueProvider<AssemblyRegistrations> fromAssembly = context.CompilationProvider
			.Select(static (c, cancellationToken) => FromAssemblyAttributes(c, cancellationToken));

		context.RegisterSourceOutput(fromCallSites.Combine(fromAssembly).Combine(isSupported).Combine(supportsBatch),
			static (spc, source) => Emit(spc, source.Left.Left.Left, source.Left.Left.Right, source.Left.Right,
				source.Right));

		context.RegisterSourceOutput(fromAssembly.Combine(isSupported).Combine(context.CompilationProvider),
			static (spc, source) => ReportUnregistered(spc, source.Left.Left, source.Left.Right, source.Right));
	}

	private static EquatableArray<TypeRegistration> FromCallSite(GeneratorSyntaxContext context,
		CancellationToken cancellationToken)
	{
		InvocationExpressionSyntax invocation = (InvocationExpressionSyntax)context.Node;
		if ((GetInvokedName(invocation) is { } name &&
		     !MarkedMethodNames.GetValue(context.SemanticModel.Compilation, CollectMarkedMethodNames)
			     .Contains(name)) ||
		    context.SemanticModel.GetSymbolInfo(invocation, cancellationToken).Symbol is not IMethodSymbol method)
		{
			return new EquatableArray<TypeRegistration>(ImmutableArray<TypeRegistration>.Empty);
		}

		IMethodSymbol constructed = method.GetConstructedReducedFrom() ?? method;
		SeedWalker walker = new(context.SemanticModel.Compilation, cancellationToken);
		bool hasMarkedParameter = SeedArguments(context, invocation, walker, method, constructed, cancellationToken);
		SeedTypeArguments(walker, constructed);

		if (hasMarkedParameter && GetReceiverType(method, constructed) is INamedTypeSymbol { IsGenericType: true, } receiver)
		{
			foreach (ITypeSymbol typeArgument in receiver.TypeArguments)
			{
				walker.Seed(typeArgument);
			}
		}

		return new EquatableArray<TypeRegistration>(walker.Registrations);
	}

	/// <returns>
	///     The name of the invoked method, or <see langword="null" /> when the syntax does not reveal it.
	/// </returns>
	private static string? GetInvokedName(InvocationExpressionSyntax invocation)
		=> invocation.Expression switch
		{
			SimpleNameSyntax name => name.Identifier.ValueText,
			MemberAccessExpressionSyntax memberAccess => memberAccess.Name.Identifier.ValueText,
			MemberBindingExpressionSyntax memberBinding => memberBinding.Name.Identifier.ValueText,
			_ => null,
		};

	/// <summary>
	///     Collects the names of the methods in the compilation that carry a marker on a parameter or type
	///     parameter, so that an invocation of any other method is dismissed without binding it.
	/// </summary>
	/// <remarks>
	///     Only referenced assemblies that know aweXpect.Core can declare a marked method, so the others are not
	///     searched. The names found in a referenced assembly or a syntax tree are kept as long as it lives, because
	///     both survive an edit elsewhere.
	/// </remarks>
	private static HashSet<string> CollectMarkedMethodNames(Compilation compilation)
	{
		HashSet<string> names = new(StringComparer.Ordinal);
		foreach (SyntaxTree tree in compilation.SyntaxTrees)
		{
			names.UnionWith(DeclaredMarkedMethodNames.GetValue(tree, CollectDeclaredMarkedMethodNames));
		}

		foreach (IAssemblySymbol assembly in compilation.SourceModule.ReferencedAssemblySymbols)
		{
			names.UnionWith(ReferencedMarkedMethodNames.GetValue(assembly, CollectReferencedMarkedMethodNames));
		}

		return names;
	}

	/// <remarks>
	///     The attribute is recognised by its name, as the syntax tree alone cannot resolve it, so a marker applied
	///     through a using alias is missed.
	/// </remarks>
	private static string[] CollectDeclaredMarkedMethodNames(SyntaxTree tree)
	{
		List<string> names = [];
		foreach (AttributeSyntax attribute in tree.GetRoot().DescendantNodes().OfType<AttributeSyntax>())
		{
			string attributeName = attribute.Name switch
			{
				QualifiedNameSyntax qualified => qualified.Right.Identifier.ValueText,
				AliasQualifiedNameSyntax aliasQualified => aliasQualified.Name.Identifier.ValueText,
				SimpleNameSyntax simple => simple.Identifier.ValueText,
				_ => "",
			};
			if (!MarkerNames.Contains(attributeName))
			{
				continue;
			}

			SyntaxNode? owner = attribute.Parent?.Parent is ParameterSyntax or TypeParameterSyntax
				? attribute.Parent.Parent.Parent?.Parent
				: null;
			switch (owner)
			{
				case MethodDeclarationSyntax methodDeclaration:
					names.Add(methodDeclaration.Identifier.ValueText);
					break;
				case LocalFunctionStatementSyntax localFunction:
					names.Add(localFunction.Identifier.ValueText);
					break;
			}
		}

		return names.ToArray();
	}

	private static string[] CollectReferencedMarkedMethodNames(IAssemblySymbol assembly)
	{
		if (assembly.Name != CoreAssembly &&
		    !assembly.Modules.Any(module => module.ReferencedAssemblies.Any(x => x.Name == CoreAssembly)))
		{
			return [];
		}

		List<string> names = [];
		Stack<INamespaceOrTypeSymbol> containers = new();
		containers.Push(assembly.GlobalNamespace);
		while (containers.Count > 0)
		{
			foreach (ISymbol member in containers.Pop().GetMembers())
			{
				if (member is INamespaceOrTypeSymbol container)
				{
					containers.Push(container);
				}
				else if (member is IMethodSymbol method &&
				         (method.Parameters.Any(HasMarker) || method.TypeParameters.Any(HasMarker)))
				{
					names.Add(method.Name);
				}
			}
		}

		return names.ToArray();

		static bool HasMarker(ISymbol symbol)
			=> IsMarked(symbol, MarkerAttribute) || IsMarked(symbol, EventMarkerAttribute);
	}

	/// <summary>
	///     Seeds the walk from every marked parameter and returns whether one carries the member marker.
	/// </summary>
	private static bool SeedArguments(GeneratorSyntaxContext context, InvocationExpressionSyntax invocation,
		SeedWalker walker, IMethodSymbol method, IMethodSymbol constructed, CancellationToken cancellationToken)
	{
		IMethodSymbol definition = constructed.OriginalDefinition;
		int reductionOffset = method.ReducedFrom is null ? 0 : 1;
		bool hasMarkedParameter = false;
		for (int i = 0; i < definition.Parameters.Length; i++)
		{
			bool requiresMembers = IsMarked(definition.Parameters[i], MarkerAttribute);
			bool requiresEvents = IsMarked(definition.Parameters[i], EventMarkerAttribute);
			if (!requiresMembers && !requiresEvents)
			{
				continue;
			}

			ExpressionSyntax? argument = FindArgument(invocation, constructed.Parameters[i], i - reductionOffset);
			ITypeSymbol? argumentType = argument is null
				? null
				: context.SemanticModel.GetTypeInfo(argument, cancellationToken).Type;
			if (requiresMembers)
			{
				hasMarkedParameter = true;
				walker.Seed(argumentType);
				walker.Seed(constructed.Parameters[i].Type);
			}

			if (requiresEvents)
			{
				walker.SeedEvents(argumentType);
				walker.SeedEvents(constructed.Parameters[i].Type);
			}
		}

		return hasMarkedParameter;
	}

	private static void SeedTypeArguments(SeedWalker walker, IMethodSymbol constructed)
	{
		IMethodSymbol definition = constructed.OriginalDefinition;
		for (int i = 0; i < definition.TypeParameters.Length; i++)
		{
			if (IsMarked(definition.TypeParameters[i], MarkerAttribute))
			{
				walker.Seed(constructed.TypeArguments[i]);
			}

			if (IsMarked(definition.TypeParameters[i], EventMarkerAttribute))
			{
				walker.SeedEvents(constructed.TypeArguments[i]);
			}
		}
	}

	/// <remarks>
	///     The comparison fetches the members of the subject by name as well, so the subject type of the receiver
	///     has to be registered alongside the expected type.
	/// </remarks>
	private static ITypeSymbol? GetReceiverType(IMethodSymbol method, IMethodSymbol constructed)
	{
		if (constructed.IsExtensionMethod)
		{
			return constructed.Parameters[0].Type;
		}

		return method.IsStatic ? null : method.ContainingType;
	}

	private static ExpressionSyntax? FindArgument(InvocationExpressionSyntax invocation, IParameterSymbol parameter,
		int position)
	{
		if (position < 0)
		{
			return (invocation.Expression as MemberAccessExpressionSyntax)?.Expression;
		}

		SeparatedSyntaxList<ArgumentSyntax> arguments = invocation.ArgumentList.Arguments;
		foreach (ArgumentSyntax argument in arguments)
		{
			if (argument.NameColon?.Name.Identifier.ValueText == parameter.Name)
			{
				return argument.Expression;
			}
		}

		return position < arguments.Count && arguments[position].NameColon is null
			? arguments[position].Expression
			: null;
	}

	private static AssemblyRegistrations FromAssemblyAttributes(Compilation compilation,
		CancellationToken cancellationToken)
	{
		MetadataWalker walker = new(compilation, cancellationToken);
		List<(ITypeSymbol Type, int AttributeIndex)> named = [];
		ImmutableArray<AttributeData> attributes = compilation.Assembly.GetAttributes();
		for (int i = 0; i < attributes.Length; i++)
		{
			if (attributes[i].AttributeClass?.ToDisplayString() == GenerateAttribute &&
			    attributes[i].ConstructorArguments.Length == 1 &&
			    attributes[i].ConstructorArguments[0].Value is ITypeSymbol type)
			{
				walker.Seed(type);
				walker.SeedEvents(type);
				named.Add((type, i));
			}
		}

		ImmutableArray<TypeRegistration> registrations = walker.Registrations;
		ImmutableArray<UnregisteredType> unregistered = named
			.Where(x => !registrations.Any(r => r.Key == SeedKey(x.Type) || r.Key == EventsKey(SeedKey(x.Type))))
			.Select(x => new UnregisteredType(x.Type.ToDisplayString(), x.AttributeIndex))
			.ToImmutableArray();
		return new AssemblyRegistrations(new EquatableArray<TypeRegistration>(registrations),
			new EquatableArray<UnregisteredType>(unregistered));
	}

	/// <remarks>
	///     The walk registers the element of an array, the underlying type of a <see cref="Nullable{T}" /> and the
	///     <c>ValueTuple</c> behind a tuple, so the diagnostic has to look for the same key.
	/// </remarks>
	private static string SeedKey(ITypeSymbol type)
	{
		while (true)
		{
			switch (type)
			{
				case IArrayTypeSymbol array:
					type = array.ElementType;
					continue;
				case INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T, } nullable:
					type = nullable.TypeArguments[0];
					continue;
				case INamedTypeSymbol { IsTupleType: true, TupleUnderlyingType: { } underlying, }:
					type = underlying;
					continue;
				default:
					return type.ToDisplayString(TypeFormat);
			}
		}
	}

	private static bool IsMarked(ISymbol symbol, string attribute)
		=> symbol.GetAttributes().Any(x
			=> x.AttributeClass is { } attributeClass &&
			   attribute.EndsWith(attributeClass.Name, StringComparison.Ordinal) &&
			   attributeClass.ToDisplayString() == attribute);

	/// <remarks>
	///     The events of a type are registered separately from its members, because a recording and a comparison seed
	///     the walk independently, so the two registrations of one type must not collapse into each other.
	/// </remarks>
	private static string EventsKey(string typeName)
		=> "events of " + typeName;

	/// <remarks>
	///     Registering against an older aweXpect.Core would not compile, so anything but a matching registry
	///     degrades to reflection.
	/// </remarks>
	private static bool SupportsRegistration(Compilation compilation)
		=> compilation
			.GetTypeByMetadataName("aweXpect.Core.Metadata.TypeMetadataRegistry")
			?.GetMembers("RegisterProperty")
			.OfType<IMethodSymbol>()
			.Any(x => x.IsStatic && x.DeclaredAccessibility == Accessibility.Public && x.Parameters.Length == 3) ==
		   true;

	/// <remarks>
	///     An older aweXpect.Core cannot publish the registrations together, so there each is published as it is made.
	/// </remarks>
	private static bool SupportsBatch(Compilation compilation)
		=> compilation
			.GetTypeByMetadataName("aweXpect.Core.Metadata.TypeMetadataRegistry")
			?.GetMembers("RegisterBatch")
			.OfType<IMethodSymbol>()
			.Any(x => x.IsStatic && x.DeclaredAccessibility == Accessibility.Public && x.Parameters.Length == 1) ==
		   true;

	/// <remarks>
	///     The generated file needs nothing newer than C# 9, but a consumer pinned below that could not compile the
	///     module initializer.
	/// </remarks>
	private static bool HasLanguageVersion(Compilation compilation)
		=> compilation is CSharpCompilation { LanguageVersion: >= LanguageVersion.CSharp9, };

	/// <remarks>
	///     Without <c>ModuleInitializerAttribute</c> the target framework cannot be trimmed or AOT-published anyway.
	/// </remarks>
	private static bool HasModuleInitializer(Compilation compilation)
	{
		INamedTypeSymbol? attribute =
			compilation.GetTypeByMetadataName("System.Runtime.CompilerServices.ModuleInitializerAttribute");
		return attribute != null &&
		       (attribute.DeclaredAccessibility == Accessibility.Public ||
		        (attribute.DeclaredAccessibility == Accessibility.Internal &&
		         SymbolEqualityComparer.Default.Equals(attribute.ContainingAssembly, compilation.Assembly)));
	}

	/// <remarks>
	///     The registrations run in one batch where aweXpect.Core supports it, so that a comparison on another thread,
	///     which reaches a type of this assembly through its runtime type while the assembly is loaded, never sees the
	///     type with only some of its members.
	/// </remarks>
	private static void Emit(SourceProductionContext context, EquatableArray<TypeRegistration> fromCallSites,
		AssemblyRegistrations fromAssembly, bool isSupported, bool supportsBatch)
	{
		if (!isSupported)
		{
			return;
		}

		List<TypeRegistration> registrations = fromCallSites.Values.Concat(fromAssembly.Registrations.Values)
			.GroupBy(x => x.Key, StringComparer.Ordinal)
			.Select(x => x.First())
			.OrderBy(x => x.Key, StringComparer.Ordinal)
			.ToList();
		if (registrations.Count == 0)
		{
			return;
		}

		string suppressions = string.Join(", ", registrations
			.SelectMany(x => x.Suppressions.Split([',',], StringSplitOptions.RemoveEmptyEntries))
			.Prepend("CS0618").Prepend("CS0612")
			.Distinct());
		StringBuilder body = new();
		body.AppendLine("""
		                internal static class TypeMetadataRegistration
		                {
		                	/// <summary>
		                	///     Registers the members of the types that reach an equivalency comparison and the events of the types
		                	///     that reach an event recording when the assembly is loaded.
		                	/// </summary>
		                	/// <remarks>
		                	///     Without this registration, the members and events could only be found by reflection, which fails
		                	///     when the application is published with trimming or Native AOT enabled.
		                	/// </remarks>
		                	[global::System.Runtime.CompilerServices.ModuleInitializer]
		                	internal static void Register()
		                	{
		                """);
		string indent = supportsBatch ? "\t\t\t" : "\t\t";
		if (supportsBatch)
		{
			body.Append("\t\t").Append(Registry).AppendLine(".RegisterBatch(static () =>");
			body.AppendLine("\t\t{");
		}

		for (int i = 0; i < registrations.Count; i++)
		{
			body.Append(indent).Append("Register").Append(i).AppendLine("();");
		}

		if (supportsBatch)
		{
			body.AppendLine("\t\t});");
		}

		body.AppendLine("\t}");
		for (int i = 0; i < registrations.Count; i++)
		{
			body.AppendLine();
			body.Append("\t// ").AppendLine(registrations[i].Key);
			if (registrations[i].Nameable is { } nameable)
			{
				// A comparison finds the interfaces that select it through `GetInterfaces()`, which reports an
				// implementation the trimmer removed as absent, silently turning a set into an ordered collection
				// and a dictionary into a sequence of pairs. Rooting them keeps the comparison the same as the JIT's.
				body.AppendLine("#if NET5_0_OR_GREATER");
				body.Append(
						"\t[global::System.Diagnostics.CodeAnalysis.DynamicDependency(global::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.Interfaces, typeof(")
					.Append(nameable).AppendLine("))]");
				body.AppendLine("#if !NET9_0_OR_GREATER");
				body.AppendLine("\t// ILC 8 cannot resolve interfaces from a dependency, but ILLink 8 honours it when trimming.");
				body.AppendLine(
					"\t[global::System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage(\"Trimming\", \"IL2037\")]");
				body.AppendLine("#endif");
				body.AppendLine("#endif");
			}

			body.Append("\tprivate static void Register").Append(i).AppendLine("()");
			body.AppendLine("\t{");
			body.Append(registrations[i].Source);
			body.AppendLine("\t}");
		}

		body.AppendLine("}");

		StringBuilder sb = new();
		sb.AppendLine("// <auto-generated />");
		sb.AppendLine("#nullable disable");
		sb.Append("#pragma warning disable ").AppendLine(suppressions);
		sb.AppendLine("namespace aweXpect.Generators");
		sb.AppendLine("{");
		foreach (string line in body.ToString().Split('\n'))
		{
			string content = line.TrimEnd('\r');
			if (content.Length > 0)
			{
				sb.Append('\t').Append(content);
			}

			sb.AppendLine();
		}

		sb.AppendLine("}");
		context.AddSource("TypeMetadataRegistration.g.cs", sb.ToString());
	}

	/// <remarks>
	///     The diagnostic is reported at the syntax of the attribute, because only a location in a syntax tree of the
	///     compilation honours a <c>#pragma warning disable</c> around it.
	/// </remarks>
	private static void ReportUnregistered(SourceProductionContext context, AssemblyRegistrations fromAssembly,
		bool isSupported, Compilation compilation)
	{
		if (!isSupported)
		{
			return;
		}

		ImmutableArray<AttributeData> attributes = compilation.Assembly.GetAttributes();
		foreach (UnregisteredType type in fromAssembly.Unregistered.Values)
		{
			Location location = attributes[type.AttributeIndex].ApplicationSyntaxReference
				?.GetSyntax(context.CancellationToken).GetLocation() ?? Location.None;
			context.ReportDiagnostic(Diagnostic.Create(NothingToRegister, location, type.Name));
		}
	}

	/// <remarks>
	///     <paramref name="Suppressions" /> holds the comma-separated obsolete diagnostic ids the registration triggers.
	/// </remarks>
	/// <remarks>
	///     <c>Nameable</c> is set only for a registration that exists to keep a type's interfaces, and holds the
	///     type as a <c>typeof</c> operand. Rooting the interfaces of a type that has none is an error, so a
	///     registration of members or events leaves it <see langword="null" />.
	/// </remarks>
	private readonly record struct TypeRegistration(string Key, string Source, string Suppressions, string? Nameable);

	private readonly record struct AssemblyRegistrations(
		EquatableArray<TypeRegistration> Registrations,
		EquatableArray<UnregisteredType> Unregistered);

	/// <remarks>
	///     A <see cref="Location" /> holds a syntax tree and would defeat the caching of the pipeline, so only the
	///     index of the attribute among the assembly attributes is kept.
	/// </remarks>
	private readonly record struct UnregisteredType(string Name, int AttributeIndex);

	private readonly record struct Member(ISymbol Symbol, string Name, ITypeSymbol Type, bool IsField);

	/// <remarks>
	///     Every seed walks with its own <see cref="MetadataWalker" />, but a compilation that imports every member
	///     only needs to exist once per input compilation, so it is shared and dies with the compilation it was made for.
	/// </remarks>
	private static readonly ConditionalWeakTable<Compilation, AllImport> AllImports = new();

	/// <remarks>
	///     The registrations a seed yields do not depend on the call site that reaches it, so each seed is walked once
	///     per input compilation, however many call sites share it.
	/// </remarks>
	private static readonly ConditionalWeakTable<Compilation, SeedWalks> Walks = new();

	private sealed class SeedWalks
	{
		public ConcurrentDictionary<ITypeSymbol, ImmutableArray<TypeRegistration>> Members { get; } =
			new(SymbolEqualityComparer.Default);

		public ConcurrentDictionary<ITypeSymbol, ImmutableArray<TypeRegistration>> Events { get; } =
			new(SymbolEqualityComparer.Default);
	}

	/// <summary>
	///     Collects the registrations of the seeds of one call site from the walks shared across the compilation.
	/// </summary>
	private sealed class SeedWalker(Compilation compilation, CancellationToken cancellationToken)
	{
		private readonly ImmutableArray<TypeRegistration>.Builder _registrations =
			ImmutableArray.CreateBuilder<TypeRegistration>();

		private readonly SeedWalks _walks = Walks.GetValue(compilation, static _ => new SeedWalks());

		public ImmutableArray<TypeRegistration> Registrations => _registrations.ToImmutable();

		public void Seed(ITypeSymbol? type)
			=> Add(_walks.Members, type, static (walker, seed) => walker.Seed(seed));

		public void SeedEvents(ITypeSymbol? type)
			=> Add(_walks.Events, type, static (walker, seed) => walker.SeedEvents(seed));

		private void Add(ConcurrentDictionary<ITypeSymbol, ImmutableArray<TypeRegistration>> walks, ITypeSymbol? type,
			Action<MetadataWalker, ITypeSymbol> walk)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (type is null)
			{
				return;
			}

			_registrations.AddRange(walks.GetOrAdd(type, seed =>
			{
				MetadataWalker walker = new(compilation, cancellationToken);
				walk(walker, seed);
				return walker.Registrations;
			}));
		}
	}

	private sealed class AllImport(Compilation compilation)
	{
		public Compilation Compilation { get; } =
			compilation.WithOptions(compilation.Options.WithMetadataImportOptions(MetadataImportOptions.All));

		public ConcurrentDictionary<IAssemblySymbol, IAssemblySymbol?> Assemblies { get; } =
			new(SymbolEqualityComparer.Default);
	}

	/// <summary>
	///     Walks a type graph the way the equivalency comparison does, and collects a registration for every type
	///     whose members it would compare.
	/// </summary>
	private sealed class MetadataWalker(Compilation compilation, CancellationToken cancellationToken)
	{
		private readonly ImmutableArray<TypeRegistration>.Builder _registrations =
			ImmutableArray.CreateBuilder<TypeRegistration>();

		private readonly HashSet<ITypeSymbol> _visited = new(SymbolEqualityComparer.Default);

		private readonly HashSet<ITypeSymbol> _visitedEvents = new(SymbolEqualityComparer.Default);

		private readonly Dictionary<IAssemblySymbol, bool> _isGlobal = new(SymbolEqualityComparer.Default);

		private readonly Dictionary<INamedTypeSymbol, bool> _isUnambiguous = new(SymbolEqualityComparer.Default);

		private readonly Dictionary<IModuleSymbol, bool> _isExperimental = new(SymbolEqualityComparer.Default);

		private bool? _supportsDictionaryRegistration;

		private bool? _supportsSetRegistration;

		private bool? _supportsExplicitRegistration;

		public ImmutableArray<TypeRegistration> Registrations => _registrations.ToImmutable();

		public void Seed(ITypeSymbol? type)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (type is IArrayTypeSymbol array)
			{
				Seed(array.ElementType);
				return;
			}

			if (type is not INamedTypeSymbol named || !IsWalkable(named))
			{
				return;
			}

			if (named.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
			{
				Seed(named.TypeArguments[0]);
				return;
			}

			named = named.TupleUnderlyingType ?? named;
			if (!_visited.Add(named) || IsComparedByValue(named) || ContainsTypeParameter(named) ||
			    IsExpectation(named))
			{
				return;
			}

			if (IsEnumerable(named))
			{
				SeedInterfaceRoot(named);
				SeedDictionaries(named);
				SeedSets(named);
				SeedElements(named);
				return;
			}

			SeedMembers(named);
		}

		/// <remarks>
		///     A collection registers no members, because it is compared element by element, but the comparison still
		///     asks its runtime type whether it is a set or a dictionary, and that answer comes from the interface
		///     list. The trimmer drops an implementation nothing else uses, so the collection is recorded with no
		///     members of its own, only to keep its interfaces.
		/// </remarks>
		private void SeedInterfaceRoot(INamedTypeSymbol type)
		{
			if (ContainsTypeParameter(type) || !IsNameable(type) || !IsReferenceable(type))
			{
				return;
			}

			string name = type.ToDisplayString(TypeFormat);
			_registrations.Add(new TypeRegistration(InterfacesKey(name), "", "", name));
		}

		private static string InterfacesKey(string typeName)
			=> "interfaces of " + typeName;

		/// <remarks>
		///     The comparison reads the key comparer of a dictionary for type arguments it only knows at runtime, which
		///     needs a reader instantiated for them in advance. An aweXpect.Core without the registration reads the
		///     comparer by reflection instead.
		/// </remarks>
		private void SeedDictionaries(INamedTypeSymbol type)
		{
			_supportsDictionaryRegistration ??= compilation
				.GetTypeByMetadataName("aweXpect.Core.Metadata.TypeMetadataRegistry")
				?.GetMembers("RegisterDictionary").OfType<IMethodSymbol>()
				.Any(x => x.IsStatic && x.DeclaredAccessibility == Accessibility.Public) == true;
			if (_supportsDictionaryRegistration != true)
			{
				return;
			}

			foreach (ImmutableArray<ITypeSymbol> typeArguments in type.AllInterfaces.Prepend(type).Where(x
				         => x.OriginalDefinition.ToDisplayString() is "System.Collections.Generic.IDictionary<TKey, TValue>"
					         or "System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>")
				         .Select(dictionary => dictionary.TypeArguments))
			{
				if (!typeArguments.All(x => IsNameable(x) && IsReferenceable(x)))
				{
					continue;
				}

				string arguments = string.Join(", ",
					typeArguments.Select(x => x.ToDisplayString(TypeFormat)));
				_registrations.Add(new TypeRegistration("dictionary of " + arguments,
					new StringBuilder().Append("\t\t").Append(Registry).Append(".RegisterDictionary<").Append(arguments)
						.AppendLine(">();").ToString(), "", null));
			}
		}

		/// <remarks>
		///     The comparison reads the comparer of a set for an item type it only knows at runtime, which needs a
		///     reader instantiated for it in advance. An aweXpect.Core without the registration matches the items of a
		///     set by the equivalency comparison alone.
		/// </remarks>
		private void SeedSets(INamedTypeSymbol type)
		{
			_supportsSetRegistration ??= compilation
				.GetTypeByMetadataName("aweXpect.Core.Metadata.TypeMetadataRegistry")
				?.GetMembers("RegisterSet").OfType<IMethodSymbol>()
				.Any(x => x.IsStatic && x.DeclaredAccessibility == Accessibility.Public) == true;
			if (_supportsSetRegistration != true)
			{
				return;
			}

			foreach (ITypeSymbol itemType in type.AllInterfaces.Prepend(type).Where(x
				         => x.OriginalDefinition.ToDisplayString() is "System.Collections.Generic.ISet<T>"
					         or "System.Collections.Generic.IReadOnlySet<T>")
				         .Select(set => set.TypeArguments[0])
				         .Where(x => IsNameable(x) && IsReferenceable(x)))
			{
				string argument = itemType.ToDisplayString(TypeFormat);
				_registrations.Add(new TypeRegistration("set of " + argument,
					new StringBuilder().Append("\t\t").Append(Registry).Append(".RegisterSet<").Append(argument)
						.AppendLine(">();").ToString(), "", null));
			}
		}

		private void SeedElements(INamedTypeSymbol enumerable)
		{
			foreach (INamedTypeSymbol type in enumerable.AllInterfaces.Prepend(enumerable).Where(x
				         => x.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T))
			{
				Seed(type.TypeArguments[0]);
			}
		}

		/// <remarks>
		///     The members are walked even when the type itself cannot be registered, because their types can be.
		///     Skipping a type that has a member the generated code cannot name keeps the registration in parity with
		///     reflection, which would compare that member.
		/// </remarks>
		private void SeedMembers(INamedTypeSymbol type)
		{
			List<Member> members = CollectMembers(type);
			List<IPropertySymbol> explicitProperties = CollectExplicitProperties(type);
			foreach (ITypeSymbol memberType in members.Select(member => member.Type)
				         .Concat(explicitProperties.Select(property => property.Type)))
			{
				Seed(memberType);
			}

			List<string> diagnosticIds = DiagnosticIds(type, members)
				.Concat(explicitProperties.SelectMany(ExplicitDiagnosticIds))
				.Distinct().OrderBy(x => x, StringComparer.Ordinal).ToList();
			if ((members.Count == 0 && explicitProperties.Count == 0) || type.IsAbstract || type.IsStatic ||
			    !IsReferenceable(type) ||
			    HasUnknownBase(type) ||
			    members.Any(member => IsUnreferenceable(member) || !CanBeMemberType(member.Type) ||
			                          !SyntaxFacts.IsValidIdentifier(member.Name) ||
			                          !IsReferenceable(member.Type) ||
			                          !IsReferenceable(member.Symbol.ContainingType)) ||
			    !diagnosticIds.All(IsDiagnosticId))
			{
				return;
			}

			string? source = EmitRegistration(type, members, explicitProperties);
			if (source is not null)
			{
				_registrations.Add(new TypeRegistration(type.ToDisplayString(TypeFormat), source,
					string.Join(",", diagnosticIds),
					null));
			}
		}

		/// <summary>
		///     Registers the events of the <paramref name="type" /> the way <c>Type.GetEvents()</c> would return them.
		/// </summary>
		/// <remarks>
		///     The recording looks the runtime type of its subject up, so an abstract class or an interface, which is
		///     never the runtime type, yields nothing. A struct is skipped, because a handler added to a boxed copy
		///     never sees the events of the caller's value.
		/// </remarks>
		public void SeedEvents(ITypeSymbol? type)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (type is not INamedTypeSymbol { TypeKind: TypeKind.Class, IsAbstract: false, IsStatic: false, } named ||
			    !IsWalkable(named) || !_visitedEvents.Add(named) || ContainsTypeParameter(named) ||
			    !IsNameable(named) || !IsReferenceable(named) || HasUnknownBase(named))
			{
				return;
			}

			List<IEventSymbol> events = CollectEvents(named);
			List<string> diagnosticIds = DiagnosticIds(events
					.SelectMany(x => new ISymbol?[] { x, x.AddMethod, x.RemoveMethod, })
					.Concat(events.SelectMany(x => TypeSymbols(x.Type)))
					.Concat(events.SelectMany(x => TypeSymbols(x.ContainingType)))
					.Concat(TypeSymbols(named)))
				.Distinct().OrderBy(x => x, StringComparer.Ordinal).ToList();
			if (events.Count == 0 || !events.All(CanBeRecorded) || !diagnosticIds.All(IsDiagnosticId))
			{
				return;
			}

			_registrations.Add(new TypeRegistration(EventsKey(named.ToDisplayString(TypeFormat)),
				EmitEventRegistration(named, events), string.Join(",", diagnosticIds), null));
		}

		/// <remarks>
		///     Mirrors <c>Type.GetEvents()</c>, which returns public events hidden by name only: a base event is dropped
		///     when a derived type declares any event of the same name, unless that declaration is private and sits on
		///     a base of the reflected type, because private members of a base are never considered. Static events are
		///     returned for the reflected type only, because the flags do not flatten the hierarchy.
		/// </remarks>
		private List<IEventSymbol> CollectEvents(INamedTypeSymbol type)
		{
			HashSet<string> names = new(StringComparer.Ordinal);
			List<IEventSymbol> events = [];
			for (INamedTypeSymbol? current = type;
			     current is not null && current.SpecialType != SpecialType.System_Object;
			     current = current.BaseType)
			{
				bool isInherited = !SymbolEqualityComparer.Default.Equals(current, type);
				foreach (IEventSymbol @event in current.GetMembers().OfType<IEventSymbol>())
				{
					if (!names.Contains(@event.Name) && @event.DeclaredAccessibility == Accessibility.Public &&
					    !(isInherited && @event.IsStatic))
					{
						events.Add(@event);
					}
				}

				names.UnionWith(DeclaredMembers(current).OfType<IEventSymbol>()
					.Where(@event => Hides(@event, current, type))
					.Select(@event => @event.Name));
			}

			return events;
		}

		/// <remarks>
		///     The generated handler is a lambda that boxes the arguments of the event, so it can take any number of
		///     parameters of any type that can be boxed. A parameter passed by reference cannot be captured that way,
		///     and a handler that returns a value cannot be bound by the reflective fallback either, so such an event
		///     keeps its owner on the reflection path.
		/// </remarks>
		private bool CanBeRecorded(IEventSymbol @event)
			=> SyntaxFacts.IsValidIdentifier(@event.Name) &&
			   !IsUnreferenceable(@event) &&
			   (@event.AddMethod is null || !IsUnreferenceable(@event.AddMethod)) &&
			   (@event.RemoveMethod is null || !IsUnreferenceable(@event.RemoveMethod)) &&
			   @event.Type is INamedTypeSymbol
			   {
				   TypeKind: TypeKind.Delegate, DelegateInvokeMethod: { ReturnsVoid: true, } invoke,
			   } handler &&
			   !ContainsTypeParameter(handler) && IsReferenceable(handler) &&
			   IsReferenceable(@event.ContainingType) &&
			   invoke.Parameters.All(parameter => parameter.RefKind == RefKind.None &&
			                                      CanBeMemberType(parameter.Type) && IsReferenceable(parameter.Type));

		private static string EmitEventRegistration(INamedTypeSymbol type, List<IEventSymbol> events)
		{
			StringBuilder sb = new();
			string typeName = type.ToDisplayString(TypeFormat);
			foreach (IEventSymbol @event in events)
			{
				string handler = @event.Type.ToDisplayString(TypeFormat);
				int count = ((INamedTypeSymbol)@event.Type).DelegateInvokeMethod!.Parameters.Length;
				string parameters = string.Join(", ", Enumerable.Range(1, count).Select(i => "arg" + i));
				string arguments = string.Concat(Enumerable.Range(1, count).Select(i => "arg" + i + ", "));
				string access = (@event.IsStatic ? typeName : Receiver(type, @event.ContainingType)) + "." +
				                Identifier(@event.Name);
				sb.Append("\t\t").Append(Registry).Append(".RegisterEvent<").Append(typeName).Append(">(\"")
					.Append(@event.Name).AppendLine("\",")
					.Append("\t\t\tcallback => new ").Append(handler).Append("((").Append(parameters)
					.Append(") => callback(new object[] { ").Append(arguments).AppendLine("})),")
					.Append("\t\t\t(o, h) => ").Append(access).Append(" += (").Append(handler).AppendLine(")h,")
					.Append("\t\t\t(o, h) => ").Append(access).Append(" -= (").Append(handler).AppendLine(")h);");
			}

			return sb.ToString();
		}

		/// <remarks>
		///     A ref struct cannot be boxed, so the comparison never reaches its members, and it cannot be a type
		///     argument of a registration either.
		/// </remarks>
		private static bool IsWalkable(INamedTypeSymbol type)
			=> type.TypeKind is not (TypeKind.Error or TypeKind.Delegate or TypeKind.Pointer
				   or TypeKind.FunctionPointer) &&
			   !type.IsRefLikeType &&
			   type.SpecialType != SpecialType.System_Object;

		/// <remarks>
		///     Mirrors the reflection over <c>BindingFlags.Public | BindingFlags.Instance</c>: inherited members are
		///     included, a hidden member is taken from the most derived type only, and indexers are skipped because
		///     their accessors take arguments. Fields and properties are tracked separately, because a field may hide
		///     a property of the same name and reflection compares both.
		/// </remarks>
		private List<Member> CollectMembers(INamedTypeSymbol type)
		{
			HashSet<string> fieldNames = new(StringComparer.Ordinal);
			HashSet<string> propertyNames = new(StringComparer.Ordinal);
			HashSet<string> propertySignatures = new(StringComparer.Ordinal);
			List<Member> members = [];
			for (INamedTypeSymbol? current = type;
			     current is not null && current.SpecialType != SpecialType.System_Object;
			     current = current.BaseType)
			{
				members.AddRange(current.GetMembers()
					.Select(symbol => ToMember(symbol, fieldNames, propertyNames, propertySignatures))
					.OfType<Member>());
				propertySignatures.UnionWith(DeclaredMembers(current).OfType<IPropertySymbol>()
					.Where(property => property is { IsStatic: false, IsIndexer: false, } &&
					                   Hides(property, current, type))
					.Select(Signature));
			}

			return members;
		}

		/// <remarks>
		///     Mirrors <c>IncludeMembersExtensions.GetExplicitProperties</c>: the readable properties the type or a base
		///     type implements explicitly for an interface, with a re-implementation hiding the one on its base by name.
		///     An implementation the generated code cannot reach through its interface is left out, so the comparison
		///     reports it as missing instead of failing to compile, and none is collected against an aweXpect.Core
		///     that cannot register it, which also lacks the fallback that would read it.
		/// </remarks>
		private List<IPropertySymbol> CollectExplicitProperties(INamedTypeSymbol type)
		{
			_supportsExplicitRegistration ??= compilation
				.GetTypeByMetadataName("aweXpect.Core.Metadata.TypeMetadataRegistry")
				?.GetMembers("RegisterExplicitProperty").OfType<IMethodSymbol>()
				.Any(x => x.IsStatic && x.DeclaredAccessibility == Accessibility.Public) == true;
			HashSet<string> names = new(StringComparer.Ordinal);
			List<IPropertySymbol> properties = [];
			if (_supportsExplicitRegistration != true)
			{
				return properties;
			}

			for (INamedTypeSymbol? current = type;
			     current is not null && current.SpecialType != SpecialType.System_Object;
			     current = current.BaseType)
			{
				foreach (IPropertySymbol property in current.GetMembers().OfType<IPropertySymbol>())
				{
					if (property is { IsStatic: false, IsIndexer: false, GetMethod: not null, } &&
					    property.ExplicitInterfaceImplementations.Length > 0 && names.Add(property.Name) &&
					    IsRegisterable(property))
					{
						properties.Add(property);
					}
				}
			}

			return properties;
		}

		private bool IsRegisterable(IPropertySymbol property)
		{
			IPropertySymbol implemented = property.ExplicitInterfaceImplementations[0];
			return SyntaxFacts.IsValidIdentifier(implemented.Name) &&
			       !IsUnreferenceable(implemented) &&
			       (implemented.GetMethod is null || !IsUnreferenceable(implemented.GetMethod)) &&
			       CanBeMemberType(property.Type) && IsReferenceable(property.Type) &&
			       IsReferenceable(implemented.ContainingType) &&
			       ExplicitDiagnosticIds(property).All(IsDiagnosticId);
		}

		/// <remarks>
		///     The generated accessor reads the property through its interface, so the attributes of the interface
		///     property decide which obsolete diagnostics it triggers.
		/// </remarks>
		private static IEnumerable<string> ExplicitDiagnosticIds(IPropertySymbol property)
		{
			IPropertySymbol implemented = property.ExplicitInterfaceImplementations[0];
			return DiagnosticIds(new ISymbol?[] { implemented, implemented.GetMethod, }
				.Concat(TypeSymbols(property.Type))
				.Concat(TypeSymbols(implemented.ContainingType)));
		}

		/// <remarks>
		///     The name sets record what reflection returns so far, so a member of the same name on a base type is
		///     dropped, and the signatures record what hides a base property, whether collected or not. A public
		///     property without a public getter is returned too, so it hides a base property of the same name without
		///     being compared itself.
		/// </remarks>
		private static Member? ToMember(ISymbol symbol, HashSet<string> fieldNames, HashSet<string> propertyNames,
			HashSet<string> propertySignatures)
			=> symbol switch
			{
				IFieldSymbol field when IsComparedField(field) && fieldNames.Add(field.Name)
					=> new Member(field, field.Name, field.Type, true),
				IPropertySymbol
					{
						IsStatic: false, IsIndexer: false, DeclaredAccessibility: Accessibility.Public,
					} property
					when !propertySignatures.Contains(Signature(property)) && propertyNames.Add(property.Name) &&
					     IsComparedProperty(property)
					=> new Member(property, property.Name, property.Type, false),
				_ => null,
			};

		/// <remarks>
		///     The runtime drops a base member that a derived declaration hides, unless the hiding declaration is
		///     private and sits on a base of the reflected type, because private members of a base are never
		///     considered. So a private <see langword="new" /> member only hides on the walked type itself.
		/// </remarks>
		private static bool Hides(ISymbol member, INamedTypeSymbol declaring, INamedTypeSymbol walked)
			=> member.DeclaredAccessibility != Accessibility.Private ||
			   SymbolEqualityComparer.Default.Equals(declaring, walked);

		/// <remarks>
		///     Properties hide by name and type, so the signature is taken from the declaration, in which a substituted
		///     type argument is still the type parameter.
		/// </remarks>
		private static string Signature(IPropertySymbol property)
			=> property.Name + "|" + property.RefKind + "|" + MetadataSignature(property.OriginalDefinition.Type);

		/// <remarks>
		///     Roslyn imports only the public and protected members of a referenced assembly, but reflection hides a
		///     base member behind a declaration of any accessibility, so the hiding pass reads a metadata type through a
		///     compilation that imports everything. Its definition suffices, because hiding is decided by name and
		///     declared signature, not by substituted types.
		/// </remarks>
		private IEnumerable<ISymbol> DeclaredMembers(INamedTypeSymbol type)
		{
			if (type.Locations.Any(location => location.IsInSource))
			{
				return type.GetMembers();
			}

			AllImport all = AllImports.GetValue(compilation, static c => new AllImport(c));
			IAssemblySymbol? assembly = all.Assemblies.GetOrAdd(type.ContainingAssembly, containing => all.Compilation
				.References
				.Select(reference => all.Compilation.GetAssemblyOrModuleSymbol(reference))
				.OfType<IAssemblySymbol>()
				.FirstOrDefault(candidate => candidate.Identity.Equals(containing.Identity)));

			return assembly?.GetTypeByMetadataName(FullMetadataName(type.OriginalDefinition))?.GetMembers() ??
			       type.GetMembers();
		}

		/// <remarks>
		///     A name that more than one assembly defines cannot be spelled out in the generated code, because the
		///     compiler reports it as ambiguous, or as a conflict with the consumer's own declaration. An inaccessible
		///     declaration, such as a polyfill compiled into a referenced library, does not take part in the lookup.
		/// </remarks>
		private bool IsUnambiguous(INamedTypeSymbol type)
		{
			INamedTypeSymbol definition = type.OriginalDefinition;
			if (!_isUnambiguous.TryGetValue(definition, out bool isUnambiguous))
			{
				List<INamedTypeSymbol> candidates = compilation.GetTypesByMetadataName(FullMetadataName(definition))
					.Where(candidate =>
						SymbolEqualityComparer.Default.Equals(candidate.ContainingAssembly, compilation.Assembly) ||
						compilation.IsSymbolAccessibleWithin(candidate, compilation.Assembly))
					.ToList();
				isUnambiguous = candidates.Count == 1 &&
				                SymbolEqualityComparer.Default.Equals(candidates[0], definition);
				_isUnambiguous[definition] = isUnambiguous;
			}

			return isUnambiguous;
		}

		private static string FullMetadataName(INamedTypeSymbol type)
		{
			StringBuilder name = new(type.MetadataName);
			for (INamedTypeSymbol? containing = type.ContainingType;
			     containing is not null;
			     containing = containing.ContainingType)
			{
				name.Insert(0, '+').Insert(0, containing.MetadataName);
			}

			if (type.ContainingNamespace is { IsGlobalNamespace: false, } ns)
			{
				name.Insert(0, '.').Insert(0, ns.ToDisplayString());
			}

			return name.ToString();
		}

		/// <remarks>
		///     A base type from an assembly the consumer does not reference has no visible members, so the walk would
		///     register fewer members or events than reflection returns.
		/// </remarks>
		private static bool HasUnknownBase(INamedTypeSymbol type)
		{
			for (INamedTypeSymbol? current = type.BaseType; current is not null; current = current.BaseType)
			{
				if (current.TypeKind == TypeKind.Error)
				{
					return true;
				}
			}

			return false;
		}

		/// <remarks>
		///     The runtime compares the metadata signature of the declaration, in which a type parameter is a position,
		///     <see langword="dynamic" /> is <see cref="object" />, tuple element names do not exist and a nested type
		///     carries the positions of its container's type arguments, so a substituted or annotated type must not
		///     tell two identical signatures apart.
		/// </remarks>
		private static string MetadataSignature(ITypeSymbol type)
			=> type switch
			{
				ITypeParameterSymbol parameter => "!" + Position(parameter),
				IDynamicTypeSymbol => "System.Object",
				IArrayTypeSymbol array => MetadataSignature(array.ElementType) + "[" + new string(',', array.Rank - 1) +
				                          "]",
				IPointerTypeSymbol pointer => MetadataSignature(pointer.PointedAtType) + "*",
				INamedTypeSymbol { IsTupleType: true, TupleUnderlyingType: { } underlying, } => MetadataSignature(
					underlying),
				INamedTypeSymbol named => (named.ContainingType is null
					                          ? named.ContainingNamespace.ToDisplayString() + "."
					                          : MetadataSignature(named.ContainingType) + ".") +
				                          named.MetadataName +
				                          (named.TypeArguments.Length == 0
					                          ? ""
					                          : "<" + string.Join(",", named.TypeArguments.Select(MetadataSignature)) +
					                            ">"),
				_ => type.ToDisplayString(TypeFormat),
			};

		/// <remarks>
		///     Roslyn numbers a type parameter within its declaring type, while metadata numbers it across the containing
		///     types as well, so <c>U</c> in <c>Outer&lt;T&gt;.Inner&lt;U&gt;</c> is position one, not zero.
		/// </remarks>
		private static int Position(ITypeParameterSymbol parameter)
		{
			int position = parameter.Ordinal;
			for (INamedTypeSymbol? containing = parameter.DeclaringType?.ContainingType;
			     containing is not null;
			     containing = containing.ContainingType)
			{
				position += containing.Arity;
			}

			return position;
		}

		/// <remarks>
		///     A tuple exposes its elements as fields named after the declaration and, from the eighth element on, as
		///     virtual fields such as <c>Item8</c>. Reflection sees only the fields of <c>ValueTuple</c> itself, so a
		///     field has to exist on the tuple's definition to be registered.
		/// </remarks>
		private static bool IsComparedField(IFieldSymbol field)
			=> field is { IsStatic: false, IsConst: false, DeclaredAccessibility: Accessibility.Public, } &&
			   (!field.ContainingType.IsTupleType ||
			    field.ContainingType.OriginalDefinition.GetMembers(field.Name).OfType<IFieldSymbol>().Any());

		private static bool IsComparedProperty(IPropertySymbol property)
			=> property is { IsStatic: false, IsIndexer: false, } &&
			   Getter(property)?.DeclaredAccessibility == Accessibility.Public;

		/// <remarks>
		///     An override that declares only a setter inherits the getter of the property it overrides.
		/// </remarks>
		private static IMethodSymbol? Getter(IPropertySymbol property)
		{
			for (IPropertySymbol? current = property; current is not null; current = current.OverriddenProperty)
			{
				if (current.GetMethod is not null)
				{
					return current.GetMethod;
				}
			}

			return null;
		}

		private static string? EmitRegistration(INamedTypeSymbol type, List<Member> members,
			List<IPropertySymbol> explicitProperties)
		{
			StringBuilder sb = new();
			if (IsNameable(type))
			{
				string typeName = type.ToDisplayString(TypeFormat);
				foreach (Member member in members)
				{
					sb.Append("\t\t").Append(Registry).Append(member.IsField ? ".RegisterField<" : ".RegisterProperty<")
						.Append(typeName).Append(", ").Append(member.Type.ToDisplayString(TypeFormat)).Append(">(\"")
						.Append(member.Name).Append("\", o => ").Append(Receiver(type, member)).Append('.')
						.Append(Identifier(member.Name)).AppendLine(");");
				}

				foreach (IPropertySymbol property in explicitProperties)
				{
					sb.Append("\t\t").Append(Registry).Append(".RegisterExplicitProperty<").Append(typeName)
						.Append(", ").Append(property.Type.ToDisplayString(TypeFormat)).Append(">(")
						.Append(ExplicitPropertyArguments(property)).AppendLine(");");
				}

				return sb.ToString();
			}

			List<string> helpers = [];
			string? probe = ProbeExpression(type, helpers);
			if (probe is null || explicitProperties.Any(property
				    => !IsNameable(property.ExplicitInterfaceImplementations[0].ContainingType)))
			{
				return null;
			}

			sb.Append("\t\tvar probe = ").Append(probe).AppendLine(";");
			foreach (Member member in members)
			{
				sb.Append("\t\t").Append(Registry).Append(member.IsField ? ".RegisterField(" : ".RegisterProperty(")
					.Append("probe, \"").Append(member.Name).Append("\", o => o.").Append(Identifier(member.Name))
					.AppendLine(");");
			}

			foreach (IPropertySymbol property in explicitProperties)
			{
				sb.Append("\t\tRegisterExplicitProperty(probe, ").Append(ExplicitPropertyArguments(property))
					.AppendLine(");");
			}

			if (explicitProperties.Count > 0)
			{
				// aweXpect.Core has no overload that infers the type from a probe, so a helper provides it.
				helpers.Add("static void RegisterExplicitProperty<T, TMember>(T p, string name, " +
				            "global::System.Func<T, TMember> getValue) => " + Registry +
				            ".RegisterExplicitProperty(name, getValue);");
			}

			foreach (string helper in helpers)
			{
				sb.Append("\t\t").AppendLine(helper);
			}

			return sb.ToString();
		}

		/// <summary>
		///     The name of the explicit implementation and an accessor that reads it through its interface.
		/// </summary>
		private static string ExplicitPropertyArguments(IPropertySymbol property)
		{
			IPropertySymbol implemented = property.ExplicitInterfaceImplementations[0];
			return SymbolDisplay.FormatLiteral(property.Name, true) + ", o => ((" +
			       implemented.ContainingType.ToDisplayString(TypeFormat) + ")o)." + Identifier(implemented.Name);
		}

		/// <remarks>
		///     An anonymous type has no name that can be written in source, so the type argument is inferred from an
		///     instance instead: two anonymous creation expressions with the same members unify to one type within an
		///     assembly. Any other type that cannot be named, such as a generic over an anonymous type, is produced by
		///     one of the <paramref name="helpers" />, a generic local function that infers the anonymous types from
		///     their probes.
		/// </remarks>
		private static string? ProbeExpression(ITypeSymbol type, List<string> helpers)
		{
			if (IsNameable(type))
			{
				return $"default({type.ToDisplayString(TypeFormat)})";
			}

			switch (type)
			{
				case IArrayTypeSymbol { IsSZArray: true, } array:
				{
					string? element = ProbeExpression(array.ElementType, helpers);
					return element is null ? null : $"new[] {{ {element}, }}";
				}
				case INamedTypeSymbol { IsAnonymousType: true, } anonymous:
				{
					StringBuilder sb = new("new { ");
					foreach (IPropertySymbol property in anonymous.GetMembers().OfType<IPropertySymbol>())
					{
						string? value = ProbeExpression(property.Type, helpers);
						if (value is null)
						{
							return null;
						}

						sb.Append(Identifier(property.Name)).Append(" = ").Append(value).Append(", ");
					}

					return sb.Append('}').ToString();
				}
				default:
				{
					List<string> probes = [];
					string? spelled = SpellWithTypeParameters(type, probes, helpers);
					if (spelled is null || probes.Count == 0)
					{
						return null;
					}

					string name = "Probe" + helpers.Count;
					string[] typeParameters = probes.Select((_, i) => "T" + i).ToArray();
					helpers.Add($"static {spelled} {name}<{string.Join(", ", typeParameters)}>(" +
					            string.Join(", ", typeParameters.Select((x, i) => $"{x} p{i}")) + ")" +
					            string.Concat(typeParameters.Select(x => $" where {x} : class")) + " => default;");
					return $"{name}({string.Join(", ", probes)})";
				}
			}
		}

		/// <summary>
		///     Spells the <paramref name="type" /> with a type parameter <c>T0</c>, <c>T1</c>, ... in place of each
		///     anonymous type, whose probe expression is added to the <paramref name="probes" />.
		/// </summary>
		/// <remarks>
		///     The type parameters are constrained to reference types, which every anonymous type satisfies, so that a
		///     generic type that constrains its own parameter that way accepts them.
		/// </remarks>
		private static string? SpellWithTypeParameters(ITypeSymbol type, List<string> probes, List<string> helpers)
		{
			if (IsNameable(type))
			{
				return type.ToDisplayString(TypeFormat);
			}

			return type switch
			{
				IArrayTypeSymbol array => SpellArrayWithTypeParameters(array, probes, helpers),
				INamedTypeSymbol { IsAnonymousType: true, } => SpellAnonymousWithTypeParameter(type, probes, helpers),
				INamedTypeSymbol named => SpellNamedWithTypeParameters(named.TupleUnderlyingType ?? named, probes,
					helpers),
				_ => null,
			};
		}

		private static string? SpellArrayWithTypeParameters(IArrayTypeSymbol array, List<string> probes,
			List<string> helpers)
		{
			StringBuilder ranks = new();
			ITypeSymbol element = array;
			while (element is IArrayTypeSymbol current)
			{
				ranks.Append('[').Append(',', current.Rank - 1).Append(']');
				element = current.ElementType;
			}

			string? spelled = SpellWithTypeParameters(element, probes, helpers);
			return spelled is null ? null : spelled + ranks;
		}

		private static string? SpellAnonymousWithTypeParameter(ITypeSymbol type, List<string> probes,
			List<string> helpers)
		{
			string? probe = ProbeExpression(type, helpers);
			if (probe is null)
			{
				return null;
			}

			probes.Add(probe);
			return "T" + (probes.Count - 1);
		}

		private static string? SpellNamedWithTypeParameters(INamedTypeSymbol named, List<string> probes,
			List<string> helpers)
		{
			string? prefix = SpellContainerWithTypeParameters(named, probes, helpers);
			if (prefix is null)
			{
				return null;
			}

			List<string> arguments = [];
			foreach (ITypeSymbol typeArgument in named.TypeArguments)
			{
				string? argument = SpellWithTypeParameters(typeArgument, probes, helpers);
				if (argument is null)
				{
					return null;
				}

				arguments.Add(argument);
			}

			return prefix + Identifier(named.Name) +
			       (arguments.Count == 0 ? "" : "<" + string.Join(", ", arguments) + ">");
		}

		private static string? SpellContainerWithTypeParameters(INamedTypeSymbol named, List<string> probes,
			List<string> helpers)
		{
			if (named.ContainingType is not null)
			{
				string? container = SpellWithTypeParameters(named.ContainingType, probes, helpers);
				return container is null ? null : container + ".";
			}

			if (named.ContainingNamespace.IsGlobalNamespace)
			{
				return "global::";
			}

			return named.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) + ".";
		}

		/// <remarks>
		///     A member hidden by a member of another kind on a derived type, such as a field hiding a property, is only
		///     reachable through its declaring type.
		/// </remarks>
		private static string Receiver(INamedTypeSymbol type, Member member)
			=> Receiver(type, member.Symbol.ContainingType);

		private static string Receiver(INamedTypeSymbol type, INamedTypeSymbol declaringType)
			=> SymbolEqualityComparer.Default.Equals(declaringType, type)
				? "o"
				: $"(({declaringType.ToDisplayString(TypeFormat)})o)";

		private static string Identifier(string name)
			=> SyntaxFacts.GetKeywordKind(name) != SyntaxKind.None ||
			   SyntaxFacts.GetContextualKeywordKind(name) != SyntaxKind.None
				? "@" + name
				: name;

		private static bool IsNameable(ITypeSymbol type)
			=> type switch
			{
				ITypeParameterSymbol => false,
				IArrayTypeSymbol array => IsNameable(array.ElementType),
				INamedTypeSymbol { IsAnonymousType: true, } => false,
				INamedTypeSymbol named => named.TypeArguments.All(IsNameable) &&
				                          (named.ContainingType is null || IsNameable(named.ContainingType)),
				_ => true,
			};

		/// <remarks>
		///     Generated code lives in its own file in the consumer assembly, so it can only name a type that is
		///     accessible from there, is not file-local, is not an error type from an assembly the consumer does not
		///     reference, and is neither obsolete with <c>error: true</c> nor experimental.
		/// </remarks>
		private bool IsReferenceable(ITypeSymbol type)
			=> type switch
			{
				IArrayTypeSymbol array => IsReferenceable(array.ElementType),
				INamedTypeSymbol { TypeKind: TypeKind.Error, } => false,
				INamedTypeSymbol { IsAnonymousType: true, } anonymous => anonymous.GetMembers()
					.OfType<IPropertySymbol>().All(x => IsReferenceable(x.Type)),
				INamedTypeSymbol named => !named.IsFileLocal && !IsUnreferenceable(named) &&
				                          !IsFromExperimentalModule(named) &&
				                          compilation.IsSymbolAccessibleWithin(named, compilation.Assembly) &&
				                          IsGlobal(named) && IsUnambiguous(named) &&
				                          named.TypeArguments.All(IsReferenceable) &&
				                          (named.ContainingType is null || IsReferenceable(named.ContainingType)),
				_ => true,
			};

		/// <remarks>
		///     A getter that requires unreferenced or dynamic code would make the generated registration itself a
		///     trimming warning, which is what the registration exists to avoid.
		/// </remarks>
		/// <remarks>
		///     A type from an assembly that is referenced only under an extern alias cannot be reached through
		///     <c>global::</c>.
		/// </remarks>
		private bool IsGlobal(INamedTypeSymbol type)
		{
			IAssemblySymbol assembly = type.ContainingAssembly;
			if (SymbolEqualityComparer.Default.Equals(assembly, compilation.Assembly))
			{
				return true;
			}

			if (!_isGlobal.TryGetValue(assembly, out bool isGlobal))
			{
				List<MetadataReference> references = compilation.References
					.Where(reference => SymbolEqualityComparer.Default.Equals(
						compilation.GetAssemblyOrModuleSymbol(reference), assembly))
					.ToList();
				isGlobal = references.Count == 0 ||
				           references.Any(reference => reference.Properties.Aliases.IsEmpty ||
				                                       reference.Properties.Aliases.Contains("global"));
				_isGlobal[assembly] = isGlobal;
			}

			return isGlobal;
		}

		/// <remarks>
		///     The compiler treats every type of an assembly or module marked as experimental as experimental, but only
		///     where another assembly uses it.
		/// </remarks>
		private bool IsFromExperimentalModule(INamedTypeSymbol type)
		{
			IModuleSymbol module = type.ContainingModule;
			if (SymbolEqualityComparer.Default.Equals(module.ContainingAssembly, compilation.Assembly))
			{
				return false;
			}

			if (!_isExperimental.TryGetValue(module, out bool isExperimental))
			{
				isExperimental = module.GetAttributes().Concat(module.ContainingAssembly.GetAttributes())
					.Any(x => x.AttributeClass?.ToDisplayString() == ExperimentalAttribute);
				_isExperimental[module] = isExperimental;
			}

			return isExperimental;
		}

		private static bool IsUnreferenceable(ISymbol symbol)
			=> symbol.GetAttributes().Any(x
				=> x.AttributeClass?.ToDisplayString() switch
				{
					"System.ObsoleteAttribute" => x.ConstructorArguments.Length == 2 &&
					                              x.ConstructorArguments[1].Value is true,
					ExperimentalAttribute => true,
					"System.Diagnostics.CodeAnalysis.RequiresUnreferencedCodeAttribute" => true,
					"System.Diagnostics.CodeAnalysis.RequiresDynamicCodeAttribute" => true,
					"System.Diagnostics.CodeAnalysis.RequiresAssemblyFilesAttribute" => true,
					_ => false,
				});

		private static bool IsUnreferenceable(Member member)
			=> IsUnreferenceable(member.Symbol) ||
			   (member.Symbol is IPropertySymbol property && Getter(property) is { } getter &&
			    IsUnreferenceable(getter));

		/// <remarks>
		///     An obsolete member reports under its own diagnostic id when one is declared, so the plain
		///     <c>CS0618</c> suppression in the generated file does not cover it.
		/// </remarks>
		private static IEnumerable<string> DiagnosticIds(INamedTypeSymbol type, List<Member> members)
			=> DiagnosticIds(members
				.SelectMany(member => new[]
				{
					member.Symbol, member.Symbol is IPropertySymbol property ? Getter(property) : null,
				})
				.Concat(members.SelectMany(member => TypeSymbols(member.Type)))
				.Concat(members.SelectMany(member => TypeSymbols(member.Symbol.ContainingType)))
				.Concat(TypeSymbols(type)));

		private static IEnumerable<string> DiagnosticIds(IEnumerable<ISymbol?> symbols)
		{
			return symbols
				.Where(symbol => symbol is not null)
				.SelectMany(symbol => symbol!.GetAttributes())
				.Where(attribute => attribute.AttributeClass?.ToDisplayString() == "System.ObsoleteAttribute")
				.Select(attribute => attribute.NamedArguments
					.FirstOrDefault(argument => argument.Key == "DiagnosticId").Value.Value as string)
				.Where(id => !string.IsNullOrEmpty(id))!;
		}

		private static IEnumerable<INamedTypeSymbol> TypeSymbols(ITypeSymbol type)
			=> type switch
			{
				IArrayTypeSymbol array => TypeSymbols(array.ElementType),
				INamedTypeSymbol named => named.TypeArguments.SelectMany(TypeSymbols).Prepend(named)
					.Concat(named.ContainingType is null
						? Enumerable.Empty<INamedTypeSymbol>()
						: TypeSymbols(named.ContainingType)),
				_ => Enumerable.Empty<INamedTypeSymbol>(),
			};

		/// <remarks>
		///     The compiler accepts any string as an obsolete diagnostic id, but a <c>#pragma</c> only accepts an
		///     identifier, so a member with an id that cannot be suppressed keeps its owner on the reflection path.
		/// </remarks>
		private static bool IsDiagnosticId(string id)
			=> id.Length > 0 && (char.IsLetter(id[0]) || id[0] == '_') &&
			   id.All(c => char.IsLetterOrDigit(c) || c == '_');

		private static bool CanBeMemberType(ITypeSymbol type)
			=> type.TypeKind is not (TypeKind.Pointer or TypeKind.FunctionPointer) && !type.IsRefLikeType &&
			   type.SpecialType != SpecialType.System_Void;

		/// <remarks>
		///     An anonymous type has no type arguments of its own, but its properties can still be typed by a type
		///     parameter of the enclosing method.
		/// </remarks>
		private static bool ContainsTypeParameter(ITypeSymbol type)
			=> type switch
			{
				ITypeParameterSymbol => true,
				IArrayTypeSymbol array => ContainsTypeParameter(array.ElementType),
				INamedTypeSymbol { IsAnonymousType: true, } anonymous => anonymous.GetMembers()
					.OfType<IPropertySymbol>().Any(x => ContainsTypeParameter(x.Type)),
				INamedTypeSymbol named => named.TypeArguments.Any(ContainsTypeParameter) ||
				                          (named.ContainingType is not null &&
				                           ContainsTypeParameter(named.ContainingType)),
				_ => false,
			};

		/// <remarks>
		///     An expectation such as <c>It.Is&lt;T&gt;()</c> is evaluated against the actual value instead of being
		///     compared member by member, so its own members are never enumerated.
		/// </remarks>
		private static bool IsExpectation(INamedTypeSymbol type)
		{
			for (INamedTypeSymbol? current = type; current is not null; current = current.BaseType)
			{
				if (current.ToDisplayString() == "aweXpect.Core.Expectation")
				{
					return true;
				}
			}

			return false;
		}

		private static bool IsEnumerable(INamedTypeSymbol type)
			=> type.AllInterfaces.Prepend(type)
				.Any(x => x.SpecialType == SpecialType.System_Collections_IEnumerable);

		/// <remarks>
		///     Mirrors <c>EquivalencyDefaults.DefaultComparisonType</c>: these types are compared by value, so their
		///     members are never enumerated.
		/// </remarks>
		private static bool IsComparedByValue(INamedTypeSymbol type)
			=> type.TypeKind == TypeKind.Enum ||
			   type.SpecialType is SpecialType.System_Boolean or SpecialType.System_Char or SpecialType.System_SByte
				   or SpecialType.System_Byte or SpecialType.System_Int16 or SpecialType.System_UInt16
				   or SpecialType.System_Int32 or SpecialType.System_UInt32 or SpecialType.System_Int64
				   or SpecialType.System_UInt64 or SpecialType.System_IntPtr or SpecialType.System_UIntPtr
				   or SpecialType.System_Single or SpecialType.System_Double or SpecialType.System_String
				   or SpecialType.System_Decimal or SpecialType.System_DateTime ||
			   type.ToDisplayString() is "System.DateTimeOffset" or "System.TimeSpan" or "System.Guid"
				   or "System.Text.StringBuilder" or "System.Numerics.BigInteger" or "System.Numerics.Complex"
				   or "System.Half" or "System.Runtime.InteropServices.NFloat" or "System.Int128" or "System.UInt128" ||
			   IsHandle(type);

		/// <remarks>
		///     A handle is matched on its bases too, because the type that reaches the comparison is a derived one:
		///     a <c>RuntimeType</c> is not <c>typeof(Type)</c>.
		/// </remarks>
		private static bool IsHandle(INamedTypeSymbol type)
		{
			for (INamedTypeSymbol? current = type; current is not null; current = current.BaseType)
			{
				if (current.ToDisplayString() is "System.Reflection.MemberInfo" or "System.Reflection.Assembly"
				    or "System.Reflection.Module" or "System.Delegate" or "System.Uri"
				    or "System.Globalization.CultureInfo")
				{
					return true;
				}
			}

			return false;
		}
	}
}
