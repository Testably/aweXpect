using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace aweXpect.Core.Generators;

/// <summary>
///     The <see cref="IIncrementalGenerator" /> that emits a module initializer into the test project, which registers
///     the values that the build targets of an extension record in <c>aweXpect.Core.Extending.ExtensionRegistry</c>
///     and calls the initializers of the extensions.
/// </summary>
/// <remarks>
///     The build targets pass only names: <c>AweXpectRegisteredMetadataKeys</c> lists the keys of the
///     <c>AssemblyMetadataAttribute</c>s whose values are registered, and <c>AweXpectModuleInitializers</c> the public
///     static methods without parameters that are called afterwards. Both are separated by <c>,</c>, as the compiler
///     receives the properties in an <c>.editorconfig</c>, where a <c>;</c> starts a comment; the build targets of
///     aweXpect.Core replace a <c>;</c> before.<br />
///     An initializer that names no accessible static method without parameters is reported as an error instead of
///     being called, so that the build does not fail in the generated file.<br />
///     Nothing is emitted where the project cannot compile a module initializer, by the same rule as for the type
///     metadata, so that an extension falls back to reading the values itself.
/// </remarks>
[Generator]
public class ExtensionRegistrationGenerator : IIncrementalGenerator
{
	private const string MetadataKeysProperty = "build_property.AweXpectRegisteredMetadataKeys";
	private const string ModuleInitializersProperty = "build_property.AweXpectModuleInitializers";

	private static readonly DiagnosticDescriptor InvalidInitializer = new(
		"aweXpect4005",
		"The module initializer cannot be called",
		"'{0}' in the MSBuild property AweXpectModuleInitializers names no accessible static method without parameters",
		"aweXpect.Core.Generators",
		DiagnosticSeverity.Error,
		true,
		helpLinkUri: "https://docs.testably.org/aweXpect/analyzers#registration-generator");

	void IIncrementalGenerator.Initialize(IncrementalGeneratorInitializationContext context)
	{
		IncrementalValueProvider<(EquatableArray<string> Keys, EquatableArray<string> Initializers)> properties =
			context.AnalyzerConfigOptionsProvider.Select(static (options, _) => (
				Split(options.GlobalOptions.TryGetValue(MetadataKeysProperty, out string? keys) ? keys : null),
				Split(options.GlobalOptions.TryGetValue(ModuleInitializersProperty, out string? initializers)
					? initializers
					: null)));

		IncrementalValueProvider<Registrations> registrations = context.CompilationProvider
			.Combine(properties)
			.Select(static (source, _) => GetRegistrations(source.Left, source.Right.Keys, source.Right.Initializers));

		context.RegisterSourceOutput(registrations, Emit);
	}

	private static EquatableArray<string> Split(string? value)
		=> new((value ?? "")
			.Split([',',], StringSplitOptions.RemoveEmptyEntries)
			.Select(x => x.Trim())
			.Where(x => x.Length > 0)
			.Distinct(StringComparer.Ordinal)
			.ToImmutableArray());

	private static Registrations GetRegistrations(Compilation compilation, EquatableArray<string> keys,
		EquatableArray<string> initializers)
	{
		if ((keys.Count == 0 && initializers.Count == 0) ||
		    !TypeMetadataGenerator.HasModuleInitializer(compilation) ||
		    !TypeMetadataGenerator.HasLanguageVersion(compilation))
		{
			return new Registrations(new EquatableArray<MetadataValue>(ImmutableArray<MetadataValue>.Empty),
				new EquatableArray<string>(ImmutableArray<string>.Empty),
				new EquatableArray<string>(ImmutableArray<string>.Empty));
		}

		ImmutableArray<MetadataValue>.Builder values = ImmutableArray.CreateBuilder<MetadataValue>();
		foreach (AttributeData attribute in compilation.Assembly.GetAttributes())
		{
			if (attribute.AttributeClass?.ToDisplayString() == "System.Reflection.AssemblyMetadataAttribute" &&
			    attribute.ConstructorArguments.Length == 2 &&
			    attribute.ConstructorArguments[0].Value is string key &&
			    attribute.ConstructorArguments[1].Value is string value &&
			    keys.Values.Contains(key))
			{
				values.Add(new MetadataValue(key, value));
			}
		}

		ImmutableArray<string> callable = initializers.Values.Where(x => IsCallable(compilation, x)).ToImmutableArray();
		return new Registrations(new EquatableArray<MetadataValue>(values.ToImmutable()),
			new EquatableArray<string>(callable),
			new EquatableArray<string>(initializers.Values.Except(callable).ToImmutableArray()));
	}

	/// <remarks>
	///     Every namespace or type with a name part is followed, as the same name can exist in several assemblies, and
	///     a generic type is skipped, as the generated call names no type arguments.
	/// </remarks>
	private static bool IsCallable(Compilation compilation, string initializer)
	{
		string[] parts = initializer.Split('.');
		IEnumerable<INamespaceOrTypeSymbol> containers = [compilation.GlobalNamespace,];
		for (int i = 0; i < parts.Length - 1; i++)
		{
			string part = parts[i];
			containers = containers
				.SelectMany(container => container.GetMembers(part).OfType<INamespaceOrTypeSymbol>())
				.Where(container => container is not INamedTypeSymbol { Arity: > 0, })
				.ToList();
		}

		return containers.OfType<INamedTypeSymbol>()
			.SelectMany(type => type.GetMembers(parts[parts.Length - 1]).OfType<IMethodSymbol>())
			.Any(method => method is
			               {
				               MethodKind: MethodKind.Ordinary, IsStatic: true, Arity: 0, Parameters.Length: 0,
			               } &&
			               compilation.IsSymbolAccessibleWithin(method, compilation.Assembly));
	}

	/// <remarks>
	///     The values are registered before the initializers are called, so that an initializer can read them.
	/// </remarks>
	private static void Emit(SourceProductionContext context, Registrations registrations)
	{
		foreach (string initializer in registrations.InvalidInitializers.Values)
		{
			context.ReportDiagnostic(Diagnostic.Create(InvalidInitializer, Location.None, initializer));
		}

		if (registrations.Values.Count == 0 && registrations.Initializers.Count == 0)
		{
			return;
		}

		StringBuilder sb = new();
		sb.AppendLine("// <auto-generated />");
		sb.AppendLine("namespace aweXpect.Generators");
		sb.AppendLine("{");
		sb.AppendLine("\tinternal static class ExtensionRegistration");
		sb.AppendLine("\t{");
		sb.AppendLine("\t\t/// <summary>");
		sb.AppendLine("\t\t///     Registers the values that the build targets of the extensions record, and calls their initializers,");
		sb.AppendLine("\t\t///     when the assembly is loaded.");
		sb.AppendLine("\t\t/// </summary>");
		sb.AppendLine("\t\t[global::System.Runtime.CompilerServices.ModuleInitializer]");
		sb.AppendLine("\t\tinternal static void Register()");
		sb.AppendLine("\t\t{");
		foreach (MetadataValue value in registrations.Values.Values)
		{
			sb.Append("\t\t\tglobal::aweXpect.Core.Extending.ExtensionRegistry.Register(")
				.Append(SymbolDisplay.FormatLiteral(value.Key, true)).Append(", ")
				.Append(SymbolDisplay.FormatLiteral(value.Value, true)).AppendLine(");");
		}

		foreach (string initializer in registrations.Initializers.Values)
		{
			sb.Append("\t\t\tglobal::").Append(initializer).AppendLine("();");
		}

		sb.AppendLine("\t\t}");
		sb.AppendLine("\t}");
		sb.AppendLine("}");
		context.AddSource("ExtensionRegistration.g.cs", sb.ToString());
	}

	private readonly record struct MetadataValue(string Key, string Value);

	private readonly record struct Registrations(
		EquatableArray<MetadataValue> Values,
		EquatableArray<string> Initializers,
		EquatableArray<string> InvalidInitializers);
}
