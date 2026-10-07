using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace aweXpect.Generators;

/// <summary>
///     The <see cref="IIncrementalGenerator" /> for generating test framework adapters.
/// </summary>
/// <remarks>
///     The generated code runs in the consumer's compilation, so it is written for C# 7.3, the default of net48,
///     and qualifies every name with <c>global::</c>, because a namespace under <c>aweXpect.</c> would shadow it.
/// </remarks>
[Generator]
public class FrameworkGenerator : IIncrementalGenerator
{
	/// <summary>
	///     Reported for each generated adapter that is never used, because it cannot register itself and the
	///     referenced aweXpect.Core does not scan the loaded assemblies for it.
	/// </summary>
	private static readonly DiagnosticDescriptor CannotRegisterAdapter = new(
		"aweXpect2002",
		"The test framework adapter cannot register itself",
		"The {0} cannot register itself, because module initializers need C# 9 or later. Set <LangVersion> to 9 or later, or call TestFrameworkRegistry.Register(new aweXpect.Frameworks.{0}()) before the first expectation.",
		"aweXpect.Generators",
		DiagnosticSeverity.Warning,
		true,
		"On .NET 8 or later, the generated test framework adapter is only used when its module initializer registers it. Without it, aweXpect throws its own exceptions, so a skipped or inconclusive test is reported as failed and a failure as an unexpected exception.",
		"https://docs.testably.org/aweXpect/analyzers#test-framework-adapter");

	void IIncrementalGenerator.Initialize(IncrementalGeneratorInitializationContext context)
	{
		IncrementalValueProvider<Settings> settings = context.CompilationProvider
			.Select((c, _) => new Settings(
				References(c, "Microsoft.VisualStudio.TestPlatform.TestFramework") ||
				References(c, "MSTest.TestFramework"),
				References(c, "nunit.framework"),
				References(c, "TUnit.Core"),
				References(c, "TUnit.Assertions"),
				References(c, "xunit.assert"),
				References(c, "xunit.v3.core"),
				References(c, "xunit.v3.assert"),
				HasAttribute(c, "System.Diagnostics.CodeAnalysis.DoesNotReturnAttribute"),
				HasAttribute(c, "System.Diagnostics.StackTraceHiddenAttribute"),
				SupportsAdapterRegistration(c),
				HasAttribute(c, "System.Runtime.CompilerServices.ModuleInitializerAttribute"),
				HasModuleInitializerLanguageVersion(c),
				CoreScansForAdapters(c)));

		context.RegisterSourceOutput(settings, Emit);
	}

	private static void Emit(SourceProductionContext context, Settings settings)
	{
		string attributes = string.Empty;
		if (settings.HasDoesNotReturn)
		{
			attributes += "[global::System.Diagnostics.CodeAnalysis.DoesNotReturn]\n\t\t";
		}

		if (settings.HasStackTraceHidden)
		{
			attributes += "[global::System.Diagnostics.StackTraceHidden]\n\t\t";
		}

		List<(string File, string Name, string Declaration)> adapters = [];

		if (settings.HasMsTest)
		{
			adapters.Add(("MsTest", "MsTestAdapter", MsTestAdapter(attributes)));
		}

		if (settings.HasNunit)
		{
			adapters.Add(("Nunit", "NunitAdapter", NunitAdapter(attributes)));
		}

		if (settings.HasTUnit)
		{
			adapters.Add(("TUnit", "TUnitAdapter", TUnitAdapter(attributes, settings.HasTUnitAssertions)));
		}

		if (settings.HasXunit2)
		{
			adapters.Add(("Xunit2", "Xunit2Adapter", Xunit2Adapter(attributes)));
		}

		if (settings.HasXunit3Assert)
		{
			adapters.Add(("Xunit3", "Xunit3Adapter", Xunit3AssertAdapter(attributes)));
		}
		else if (settings.HasXunit3Core)
		{
			adapters.Add(("Xunit3", "Xunit3Adapter", Xunit3CoreAdapter(attributes)));
		}

		bool isRegistered = RegistersAdapters(context, settings, adapters.Select(x => x.Name));
		foreach ((string file, string name, string declaration) in adapters)
		{
			context.AddSource($"{file}.g.cs",
				isRegistered ? AdapterRegistration(name, declaration) : InFrameworksNamespace(declaration));
		}
	}

	/// <summary>
	///     Whether the <paramref name="adapters" /> register themselves, reporting each one that would have to but cannot.
	/// </summary>
	private static bool RegistersAdapters(SourceProductionContext context, Settings settings,
		IEnumerable<string> adapters)
	{
		// Without `ModuleInitializerAttribute` the target framework cannot be trimmed or AOT-published anyway,
		// and emitting a polyfill would collide with generators like PolySharp, which are invisible here.
		if (!settings.HasTestFrameworkRegistry || !settings.HasModuleInitializerAttribute)
		{
			return false;
		}

		if (!settings.CanCompileModuleInitializer)
		{
			if (!settings.IsAdapterFoundByScan)
			{
				foreach (string adapter in adapters)
				{
					context.ReportDiagnostic(Diagnostic.Create(CannotRegisterAdapter, Location.None, adapter));
				}
			}

			return false;
		}

		return true;
	}

	private static bool References(Compilation compilation, string assemblyName)
		=> compilation.ReferencedAssemblyNames.Any(x => x.Name == assemblyName);

	/// <remarks>
	///     Registering against an older aweXpect.Core would not compile, so anything but an exact match degrades to
	///     the assembly scan.
	/// </remarks>
	private static bool SupportsAdapterRegistration(Compilation compilation)
		=> compilation
			.GetTypeByMetadataName("aweXpect.Core.Adapters.TestFrameworkRegistry")
			?.GetMembers("Register")
			.OfType<IMethodSymbol>()
			.Any(x => x.IsStatic &&
			          x.DeclaredAccessibility == Accessibility.Public &&
			          x.Parameters.Length == 2 &&
			          x.Parameters[1].Type.SpecialType == SpecialType.System_Boolean) == true;

	/// <remarks>
	///     A consumer pinned below C# 9 could not compile the module initializer.
	/// </remarks>
	private static bool HasModuleInitializerLanguageVersion(Compilation compilation)
		=> compilation is CSharpCompilation { LanguageVersion: >= LanguageVersion.CSharp9, };

	/// <remarks>
	///     Only the builds of aweXpect.Core for .NET 8 or later no longer scan the loaded assemblies for an adapter. A
	///     consumer on .NET 5 to .NET 7 has a module initializer, but resolves the .NET Standard build.
	/// </remarks>
	private static bool CoreScansForAdapters(Compilation compilation)
	{
		const string netCoreApp = ".NETCoreApp,Version=v";
		string? targetFramework = compilation
			.GetTypeByMetadataName("aweXpect.Core.Adapters.TestFrameworkRegistry")?.ContainingAssembly
			.GetAttributes()
			.FirstOrDefault(x => x.AttributeClass?.ToDisplayString() ==
			                     "System.Runtime.Versioning.TargetFrameworkAttribute")?
			.ConstructorArguments.FirstOrDefault().Value as string;
		return targetFramework?.StartsWith(netCoreApp, StringComparison.Ordinal) != true ||
		       !Version.TryParse(targetFramework.Substring(netCoreApp.Length), out Version? version) ||
		       version.Major < 8;
	}

	/// <remarks>
	///     The adapter is nested in its registration, because a project that sees the internals of another project
	///     with its own generated adapter would otherwise resolve the name to both and warn with CS0436.
	/// </remarks>
	private static string AdapterRegistration(string adapterName, string adapterDeclaration) =>
		$$"""
		  namespace aweXpect.Frameworks
		  {
		  	internal static class {{adapterName}}Registration
		  	{
		  		/// <summary>
		  		///     Registers the <see cref="{{adapterName}}" /> when the assembly is loaded.
		  		/// </summary>
		  		/// <remarks>
		  		///     Without this registration, the adapter could only be found by scanning the loaded assemblies, which
		  		///     fails when the application is published with trimming or Native AOT enabled.<br />
		  		///     It does not overwrite an explicitly registered adapter.
		  		/// </remarks>
		  		[global::System.Runtime.CompilerServices.ModuleInitializer]
		  		internal static void Register()
		  			=> global::aweXpect.Core.Adapters.TestFrameworkRegistry.Register(new {{adapterName}}(), overwrite: false);

		  {{Indent(adapterDeclaration)}}
		  	}
		  }
		  """;

	private static string InFrameworksNamespace(string adapterDeclaration) =>
		$$"""
		  namespace aweXpect.Frameworks
		  {
		  {{adapterDeclaration}}
		  }
		  """;

	private static string Indent(string declaration)
		=> string.Join("\n", declaration.Split('\n')
			.Select(line => line.TrimEnd('\r').Length == 0 ? line : "\t" + line));

	private static string MsTestAdapter(string attributes) =>
		$$"""
		  	internal class MsTestAdapter : global::aweXpect.Core.Adapters.ITestFrameworkAdapter
		  	{
		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.IsAvailable" />
		  		public bool IsAvailable { get; } = true;

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Skip(string)" />
		  		{{attributes}}public void Skip(string message)
		  			=> throw new global::Microsoft.VisualStudio.TestTools.UnitTesting.AssertInconclusiveException(message);

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Fail(string)" />
		  		{{attributes}}public void Fail(string message)
		  			=> throw new global::Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException(message);

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Fail(string, global::System.Exception)" />
		  		{{attributes}}public void Fail(string message, global::System.Exception innerException)
		  			=> throw new global::Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException(message, innerException);

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Inconclusive(string)" />
		  		{{attributes}}public void Inconclusive(string message)
		  			=> throw new global::Microsoft.VisualStudio.TestTools.UnitTesting.AssertInconclusiveException(message);
		  	}
		  """;

	private static string NunitAdapter(string attributes) =>
		$$"""
		  	internal class NunitAdapter : global::aweXpect.Core.Adapters.ITestFrameworkAdapter
		  	{
		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.IsAvailable" />
		  		public bool IsAvailable { get; } = true;

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Skip(string)" />
		  		{{attributes}}public void Skip(string message)
		  			=> throw new global::NUnit.Framework.IgnoreException(message);

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Fail(string)" />
		  		{{attributes}}public void Fail(string message)
		  			=> throw new global::NUnit.Framework.AssertionException(message);

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Fail(string, global::System.Exception)" />
		  		{{attributes}}public void Fail(string message, global::System.Exception innerException)
		  			=> throw new global::NUnit.Framework.AssertionException(message, innerException);

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Inconclusive(string)" />
		  		{{attributes}}public void Inconclusive(string message)
		  			=> throw new global::NUnit.Framework.InconclusiveException(message);
		  	}
		  """;

	/// <remarks>
	///     A project that uses aweXpect instead of TUnit's assertions references only TUnit.Core, which is enough to skip
	///     a test, so a failure then throws the exception of aweXpect.
	/// </remarks>
	private static string TUnitAdapter(string attributes, bool hasAssertions)
	{
		string failException = hasAssertions
			? "global::TUnit.Assertions.Exceptions.AssertionException"
			: "global::aweXpect.FailException";
		return $$"""
		         	internal class TUnitAdapter : global::aweXpect.Core.Adapters.ITestFrameworkAdapter
		         	{
		         		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.IsAvailable" />
		         		public bool IsAvailable { get; } = true;

		         		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Skip(string)" />
		         		{{attributes}}public void Skip(string message)
		         			=> throw new global::TUnit.Core.Exceptions.SkipTestException(message);

		         		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Fail(string)" />
		         		{{attributes}}public void Fail(string message)
		         			=> throw new {{failException}}(message);

		         		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Fail(string, global::System.Exception)" />
		         		{{attributes}}public void Fail(string message, global::System.Exception innerException)
		         			=> throw new {{failException}}(message, innerException);

		         		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Inconclusive(string)" />
		         		{{attributes}}public void Inconclusive(string message)
		         			=> throw new global::TUnit.Core.Exceptions.InconclusiveTestException(message, null);
		         	}
		         """;
	}

	private static string Xunit2Adapter(string attributes) =>
		$$"""
		  	internal class Xunit2Adapter : global::aweXpect.Core.Adapters.ITestFrameworkAdapter
		  	{
		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.IsAvailable" />
		  		public bool IsAvailable { get; } = true;

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Skip(string)" />
		  		{{attributes}}public void Skip(string message)
		  			=> throw new global::aweXpect.SkipException($"SKIPPED: {message} (xunit v2 does not support skipping test)");

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Fail(string)" />
		  		{{attributes}}public void Fail(string message)
		  			=> throw new global::Xunit.Sdk.XunitException(message);

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Fail(string, global::System.Exception)" />
		  		{{attributes}}public void Fail(string message, global::System.Exception innerException)
		  			=> throw new global::Xunit.Sdk.XunitException(message, innerException);

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Inconclusive(string)" />
		  		{{attributes}}public void Inconclusive(string message)
		  			=> throw new global::aweXpect.InconclusiveException(message);
		  	}
		  """;

	private static string Xunit3CoreAdapter(string attributes) =>
		$$"""
		  	internal class Xunit3Adapter : global::aweXpect.Core.Adapters.ITestFrameworkAdapter
		  	{
		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.IsAvailable" />
		  		public bool IsAvailable { get; } = true;

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Skip(string)" />
		  		{{attributes}}public void Skip(string message)
		  			=> throw new global::aweXpect.SkipException($"$XunitDynamicSkip${message}");

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Fail(string)" />
		  		{{attributes}}public void Fail(string message)
		  			=> throw new XunitException(message);

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Fail(string, global::System.Exception)" />
		  		{{attributes}}public void Fail(string message, global::System.Exception innerException)
		  			=> throw new XunitException(message, innerException);

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Inconclusive(string)" />
		  		{{attributes}}public void Inconclusive(string message)
		  			=> throw new XunitTimeoutException(message);

		  		/// <summary>
		  		///     Interface is required by xunit v3 to identify an assertion exception.
		  		/// </summary>
		  		private interface IAssertionException { }

		  #pragma warning disable S3871 // Exception types should be "public"
		  		private sealed class XunitException : global::System.Exception, IAssertionException
		  		{
		  			public XunitException(string message) : base(message) { }

		  			public XunitException(string message, global::System.Exception innerException) : base(message, innerException) { }
		  		}
		  #pragma warning restore S3871 // Exception types should be "public"

		  #pragma warning disable S3871 // Exception types should be "public"
		  		private sealed class XunitTimeoutException : global::aweXpect.InconclusiveException, ITestTimeoutException
		  		{
		  			public XunitTimeoutException(string message) : base(message) { }
		  		}
		  #pragma warning restore S3871 // Exception types should be "public"

		  		private interface ITestTimeoutException { }
		  	}
		  """;

	private static string Xunit3AssertAdapter(string attributes) =>
		$$"""
		  	internal class Xunit3Adapter : global::aweXpect.Core.Adapters.ITestFrameworkAdapter
		  	{
		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.IsAvailable" />
		  		public bool IsAvailable { get; } = true;

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Skip(string)" />
		  		{{attributes}}public void Skip(string message)
		  			=> throw new global::aweXpect.SkipException($"$XunitDynamicSkip${message}");

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Fail(string)" />
		  		{{attributes}}public void Fail(string message)
		  			=> throw new global::Xunit.Sdk.XunitException(message);

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Fail(string, global::System.Exception)" />
		  		{{attributes}}public void Fail(string message, global::System.Exception innerException)
		  			=> throw new global::Xunit.Sdk.XunitException(message, innerException);

		  		/// <inheritdoc cref="global::aweXpect.Core.Adapters.ITestFrameworkAdapter.Inconclusive(string)" />
		  		{{attributes}}public void Inconclusive(string message)
		  			=> throw new XunitTimeoutException(message);

		  #pragma warning disable S3871 // Exception types should be "public"
		  		private sealed class XunitTimeoutException : global::aweXpect.InconclusiveException, ITestTimeoutException
		  		{
		  			public XunitTimeoutException(string message) : base(message) { }
		  		}
		  #pragma warning restore S3871 // Exception types should be "public"

		  		private interface ITestTimeoutException { }
		  	}
		  """;

	private static bool HasAttribute(Compilation c, string attributeName)
	{
		INamedTypeSymbol? attributeSymbol = c.GetTypeByMetadataName(attributeName);
		return attributeSymbol != null &&
		       (attributeSymbol.DeclaredAccessibility == Accessibility.Public ||
		        (attributeSymbol.DeclaredAccessibility == Accessibility.Internal &&
		         SymbolEqualityComparer.Default.Equals(attributeSymbol.ContainingAssembly, c.Assembly)));
	}

	private readonly record struct Settings(
		bool HasMsTest,
		bool HasNunit,
		bool HasTUnit,
		bool HasTUnitAssertions,
		bool HasXunit2,
		bool HasXunit3Core,
		bool HasXunit3Assert,
		bool HasDoesNotReturn,
		bool HasStackTraceHidden,
		bool HasTestFrameworkRegistry,
		bool HasModuleInitializerAttribute,
		bool CanCompileModuleInitializer,
		bool IsAdapterFoundByScan);
}
