using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace aweXpect.Generators;

/// <summary>
///     The <see cref="IIncrementalGenerator" /> that registers the members of every type reaching an equivalency
///     comparison in <c>aweXpect.Core.Metadata.TypeMetadataRegistry</c>.
/// </summary>
/// <remarks>
///     A call site whose parameter or type parameter carries <c>RequiresMemberMetadataAttribute</c> seeds the static
///     type of its argument, and the walk follows the declared members transitively. Without the registration, the
///     comparison has to reflect over the members, which publishing with trimming or Native AOT enabled removes.
/// </remarks>
[Generator]
public class TypeMetadataGenerator : IIncrementalGenerator
{
	private const string MarkerAttribute = "aweXpect.Core.Metadata.RequiresMemberMetadataAttribute";
	private const string GenerateAttribute = "aweXpect.Core.Metadata.GenerateMetadataAttribute";
	private const string Registry = "global::aweXpect.Core.Metadata.TypeMetadataRegistry";

	/// <summary>
	///     Reported for a type named in <c>GenerateMetadataAttribute</c> that yields no registration.
	/// </summary>
	private static readonly DiagnosticDescriptor NothingToRegister = new(
		"aweXpect2001",
		"The type yields no metadata registration",
		"'{0}' yields no metadata registration, because it is compared by value, enumerated, abstract, an open generic or not accessible from this assembly; it stays on the reflection path",
		"aweXpect.Generators",
		DiagnosticSeverity.Warning,
		true);

	private static readonly SymbolDisplayFormat TypeFormat = SymbolDisplayFormat.FullyQualifiedFormat
		.WithMiscellaneousOptions(SymbolDisplayFormat.FullyQualifiedFormat.MiscellaneousOptions &
		                          ~SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

	void IIncrementalGenerator.Initialize(IncrementalGeneratorInitializationContext context)
	{
		IncrementalValueProvider<bool> isSupported = context.CompilationProvider
			.Select(static (c, _) => SupportsRegistration(c) && HasModuleInitializer(c));

		IncrementalValueProvider<EquatableArray<TypeRegistration>> fromCallSites = context.SyntaxProvider
			.CreateSyntaxProvider(
				static (node, _) => node is InvocationExpressionSyntax,
				static (ctx, cancellationToken) => FromCallSite(ctx, cancellationToken))
			.SelectMany(static (x, _) => x)
			.Collect()
			.Select(static (x, _) => new EquatableArray<TypeRegistration>(x));

		IncrementalValueProvider<AssemblyRegistrations> fromAssembly = context.CompilationProvider
			.Select(static (c, _) => FromAssemblyAttributes(c));

		context.RegisterSourceOutput(fromCallSites.Combine(fromAssembly).Combine(isSupported),
			static (spc, source) => Emit(spc, source.Left.Left, source.Left.Right, source.Right));
	}

	private static ImmutableArray<TypeRegistration> FromCallSite(GeneratorSyntaxContext context,
		CancellationToken cancellationToken)
	{
		InvocationExpressionSyntax invocation = (InvocationExpressionSyntax)context.Node;
		if (context.SemanticModel.GetSymbolInfo(invocation, cancellationToken).Symbol is not IMethodSymbol method)
		{
			return ImmutableArray<TypeRegistration>.Empty;
		}

		IMethodSymbol constructed = method.GetConstructedReducedFrom() ?? method;
		IMethodSymbol definition = constructed.OriginalDefinition;
		int reductionOffset = method.ReducedFrom is null ? 0 : 1;
		MetadataWalker walker = new(context.SemanticModel.Compilation);
		bool hasMarkedParameter = false;

		for (int i = 0; i < definition.Parameters.Length; i++)
		{
			if (!IsMarked(definition.Parameters[i]))
			{
				continue;
			}

			hasMarkedParameter = true;
			ExpressionSyntax? argument = FindArgument(invocation, constructed.Parameters[i], i - reductionOffset);
			if (argument is not null)
			{
				walker.Seed(context.SemanticModel.GetTypeInfo(argument, cancellationToken).Type);
			}

			walker.Seed(constructed.Parameters[i].Type);
		}

		for (int i = 0; i < definition.TypeParameters.Length; i++)
		{
			if (IsMarked(definition.TypeParameters[i]))
			{
				walker.Seed(constructed.TypeArguments[i]);
			}
		}

		if (hasMarkedParameter && GetReceiverType(method, constructed) is INamedTypeSymbol { IsGenericType: true, } receiver)
		{
			foreach (ITypeSymbol typeArgument in receiver.TypeArguments)
			{
				walker.Seed(typeArgument);
			}
		}

		return walker.Registrations;
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

	private static AssemblyRegistrations FromAssemblyAttributes(Compilation compilation)
	{
		MetadataWalker walker = new(compilation);
		List<(ITypeSymbol Type, AttributeData Attribute)> named = [];
		foreach (AttributeData attribute in compilation.Assembly.GetAttributes())
		{
			if (attribute.AttributeClass?.ToDisplayString() == GenerateAttribute &&
			    attribute.ConstructorArguments.Length == 1 &&
			    attribute.ConstructorArguments[0].Value is ITypeSymbol type)
			{
				walker.Seed(type);
				named.Add((type, attribute));
			}
		}

		ImmutableArray<TypeRegistration> registrations = walker.Registrations;
		ImmutableArray<UnregisteredType> unregistered = named
			.Where(x => !registrations.Any(r => r.Key == x.Type.ToDisplayString(TypeFormat)))
			.Select(x => UnregisteredType.Create(x.Type, x.Attribute))
			.ToImmutableArray();
		return new AssemblyRegistrations(new EquatableArray<TypeRegistration>(registrations),
			new EquatableArray<UnregisteredType>(unregistered));
	}

	private static bool IsMarked(ISymbol symbol)
		=> symbol.GetAttributes().Any(x
			=> x.AttributeClass?.Name == "RequiresMemberMetadataAttribute" &&
			   x.AttributeClass.ToDisplayString() == MarkerAttribute);

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

	private static void Emit(SourceProductionContext context, EquatableArray<TypeRegistration> fromCallSites,
		AssemblyRegistrations fromAssembly, bool isSupported)
	{
		if (!isSupported)
		{
			return;
		}

		foreach (UnregisteredType type in fromAssembly.Unregistered.Values)
		{
			context.ReportDiagnostic(Diagnostic.Create(NothingToRegister, type.GetLocation(), type.Name));
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

		StringBuilder sb = new();
		sb.AppendLine("""
		              // <auto-generated />
		              #nullable disable
		              #pragma warning disable CS0612, CS0618
		              namespace aweXpect.Generators;

		              internal static class TypeMetadataRegistration
		              {
		              	/// <summary>
		              	///     Registers the members of the types that reach an equivalency comparison when the assembly is loaded.
		              	/// </summary>
		              	/// <remarks>
		              	///     Without this registration, the members could only be found by reflection, which fails when the
		              	///     application is published with trimming or Native AOT enabled.
		              	/// </remarks>
		              	[System.Runtime.CompilerServices.ModuleInitializer]
		              	internal static void Register()
		              	{
		              """);
		for (int i = 0; i < registrations.Count; i++)
		{
			sb.Append("\t\tRegister").Append(i).AppendLine("();");
		}

		sb.AppendLine("\t}");
		for (int i = 0; i < registrations.Count; i++)
		{
			sb.AppendLine();
			sb.Append("\t// ").AppendLine(registrations[i].Key);
			sb.Append("\tprivate static void Register").Append(i).AppendLine("()");
			sb.AppendLine("\t{");
			sb.Append(registrations[i].Source);
			sb.AppendLine("\t}");
		}

		sb.AppendLine("}");
		context.AddSource("TypeMetadataRegistration.g.cs", sb.ToString());
	}

	private readonly record struct TypeRegistration(string Key, string Source);

	private readonly record struct AssemblyRegistrations(
		EquatableArray<TypeRegistration> Registrations,
		EquatableArray<UnregisteredType> Unregistered);

	/// <remarks>
	///     A <see cref="Location" /> holds a syntax tree and would defeat the caching of the pipeline, so only its
	///     coordinates are kept.
	/// </remarks>
	private readonly record struct UnregisteredType(
		string Name,
		string? Path,
		TextSpan Span,
		LinePositionSpan LineSpan)
	{
		public static UnregisteredType Create(ITypeSymbol type, AttributeData attribute)
		{
			Location? location = attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation();
			return new UnregisteredType(type.ToDisplayString(), location?.SourceTree?.FilePath,
				location?.SourceSpan ?? default, location?.GetLineSpan().Span ?? default);
		}

		public Location GetLocation()
			=> Path is null ? Location.None : Location.Create(Path, Span, LineSpan);
	}

	private readonly record struct Member(ISymbol Symbol, string Name, ITypeSymbol Type, bool IsField);

	/// <summary>
	///     Walks a type graph the way the equivalency comparison does, and collects a registration for every type
	///     whose members it would compare.
	/// </summary>
	private sealed class MetadataWalker(Compilation compilation)
	{
		private readonly ImmutableArray<TypeRegistration>.Builder _registrations =
			ImmutableArray.CreateBuilder<TypeRegistration>();

		private readonly HashSet<ITypeSymbol> _visited = new(SymbolEqualityComparer.Default);

		public ImmutableArray<TypeRegistration> Registrations => _registrations.ToImmutable();

		public void Seed(ITypeSymbol? type)
		{
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
				SeedElements(named);
				return;
			}

			SeedMembers(named);
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
			foreach (Member member in members)
			{
				Seed(member.Type);
			}

			if (members.Count == 0 || type.IsAbstract || type.IsStatic || !IsReferenceable(type) ||
			    members.Any(member => IsUnreferenceable(member.Symbol) || !IsReferenceable(member.Type) ||
			                          !IsReferenceable(member.Symbol.ContainingType)))
			{
				return;
			}

			string? source = EmitRegistration(type, members);
			if (source is not null)
			{
				_registrations.Add(new TypeRegistration(type.ToDisplayString(TypeFormat), source));
			}
		}

		private static bool IsWalkable(INamedTypeSymbol type)
			=> type.TypeKind is not (TypeKind.Error or TypeKind.Delegate or TypeKind.Pointer
				   or TypeKind.FunctionPointer) &&
			   type.SpecialType != SpecialType.System_Object;

		/// <remarks>
		///     Mirrors the reflection over <c>BindingFlags.Public | BindingFlags.Instance</c>: inherited members are
		///     included, a hidden member is taken from the most derived type only, and indexers are skipped because
		///     their accessors take arguments. Fields and properties are tracked separately, because a field may hide
		///     a property of the same name and reflection compares both.
		/// </remarks>
		private static List<Member> CollectMembers(INamedTypeSymbol type)
		{
			HashSet<string> fieldNames = new(StringComparer.Ordinal);
			HashSet<string> propertyNames = new(StringComparer.Ordinal);
			List<Member> members = [];
			for (INamedTypeSymbol? current = type;
			     current is not null && current.SpecialType != SpecialType.System_Object;
			     current = current.BaseType)
			{
				foreach (ISymbol symbol in current.GetMembers())
				{
					Member? member = symbol switch
					{
						IFieldSymbol field when IsComparedField(field) && fieldNames.Add(field.Name)
							=> new Member(field, field.Name, field.Type, true),
						IPropertySymbol property when IsComparedProperty(property) && propertyNames.Add(property.Name)
							=> new Member(property, property.Name, property.Type, false),
						_ => null,
					};
					if (member is not null && CanBeMemberType(member.Value.Type))
					{
						members.Add(member.Value);
					}
				}
			}

			return members;
		}

		private static bool IsComparedField(IFieldSymbol field)
			=> field is { IsStatic: false, IsConst: false, DeclaredAccessibility: Accessibility.Public, } &&
			   (field.CorrespondingTupleField is null ||
			    SymbolEqualityComparer.Default.Equals(field, field.CorrespondingTupleField));

		private static bool IsComparedProperty(IPropertySymbol property)
			=> property is
			{
				IsStatic: false, IsIndexer: false, RefKind: RefKind.None,
				GetMethod.DeclaredAccessibility: Accessibility.Public,
			};

		private static string? EmitRegistration(INamedTypeSymbol type, List<Member> members)
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

				return sb.ToString();
			}

			string? probe = ProbeExpression(type);
			if (probe is null)
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

			return sb.ToString();
		}

		/// <remarks>
		///     An anonymous type has no name that can be written in source, so the type argument is inferred from an
		///     instance instead: two anonymous creation expressions with the same members unify to one type within an
		///     assembly.
		/// </remarks>
		private static string? ProbeExpression(ITypeSymbol type)
		{
			if (IsNameable(type))
			{
				return $"default({type.ToDisplayString(TypeFormat)})";
			}

			switch (type)
			{
				case IArrayTypeSymbol { IsSZArray: true, } array:
				{
					string? element = ProbeExpression(array.ElementType);
					return element is null ? null : $"new[] {{ {element}, }}";
				}
				case INamedTypeSymbol { IsAnonymousType: true, } anonymous:
				{
					StringBuilder sb = new("new { ");
					foreach (IPropertySymbol property in anonymous.GetMembers().OfType<IPropertySymbol>())
					{
						string? value = ProbeExpression(property.Type);
						if (value is null)
						{
							return null;
						}

						sb.Append(Identifier(property.Name)).Append(" = ").Append(value).Append(", ");
					}

					return sb.Append('}').ToString();
				}
				default:
					return null;
			}
		}

		/// <remarks>
		///     A member hidden by a member of another kind on a derived type, such as a field hiding a property, is only
		///     reachable through its declaring type.
		/// </remarks>
		private static string Receiver(INamedTypeSymbol type, Member member)
			=> SymbolEqualityComparer.Default.Equals(member.Symbol.ContainingType, type)
				? "o"
				: $"(({member.Symbol.ContainingType.ToDisplayString(TypeFormat)})o)";

		private static string Identifier(string name)
			=> SyntaxFacts.GetKeywordKind(name) != SyntaxKind.None ||
			   SyntaxFacts.GetContextualKeywordKind(name) != SyntaxKind.None
				? "@" + name
				: name;

		private static bool IsNameable(ITypeSymbol type)
			=> type switch
			{
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
				                          compilation.IsSymbolAccessibleWithin(named, compilation.Assembly) &&
				                          named.TypeArguments.All(IsReferenceable) &&
				                          (named.ContainingType is null || IsReferenceable(named.ContainingType)),
				_ => true,
			};

		private static bool IsUnreferenceable(ISymbol symbol)
			=> symbol.GetAttributes().Any(x
				=> x.AttributeClass?.ToDisplayString() switch
				{
					"System.ObsoleteAttribute" => x.ConstructorArguments.Length == 2 &&
					                              x.ConstructorArguments[1].Value is true,
					"System.Diagnostics.CodeAnalysis.ExperimentalAttribute" => true,
					_ => false,
				});

		private static bool CanBeMemberType(ITypeSymbol type)
			=> type.TypeKind is not (TypeKind.Pointer or TypeKind.FunctionPointer) && !type.IsRefLikeType &&
			   type.SpecialType != SpecialType.System_Void;

		private static bool ContainsTypeParameter(ITypeSymbol type)
			=> type switch
			{
				ITypeParameterSymbol => true,
				IArrayTypeSymbol array => ContainsTypeParameter(array.ElementType),
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
				if (current.ToDisplayString() == "aweXpect.Results.Expectation")
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
			   type.ToDisplayString() is "System.DateTimeOffset" or "System.TimeSpan" or "System.Guid";
	}
}
