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
	void IIncrementalGenerator.Initialize(IncrementalGeneratorInitializationContext context)
	{
		IncrementalValueProvider<Settings> settings = context.CompilationProvider
			.Select((c, _) => new Settings(
				HasMsTest: References(c, "Microsoft.VisualStudio.TestPlatform.TestFramework") ||
				           References(c, "MSTest.TestFramework"),
				HasNunit: References(c, "nunit.framework"),
				HasTUnit: References(c, "TUnit.Core"),
				HasTUnitAssertions: References(c, "TUnit.Assertions"),
				HasXunit2: References(c, "xunit.assert"),
				HasXunit3Core: References(c, "xunit.v3.core"),
				HasXunit3Assert: References(c, "xunit.v3.assert"),
				HasDoesNotReturn: HasAttribute(c, "System.Diagnostics.CodeAnalysis.DoesNotReturnAttribute"),
				HasStackTraceHidden: HasAttribute(c, "System.Diagnostics.StackTraceHiddenAttribute"),
				HasTestFrameworkRegistry: SupportsAdapterRegistration(c),
				HasModuleInitializer: HasAttribute(c, "System.Runtime.CompilerServices.ModuleInitializerAttribute") &&
				                      HasModuleInitializerLanguageVersion(c)));

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

		List<string> registeredAdapters = [];

		if (settings.HasMsTest)
		{
			context.AddSource("MsTest.g.cs", MsTestAdapter(attributes));
			registeredAdapters.Add("MsTestAdapter");
		}

		if (settings.HasNunit)
		{
			context.AddSource("Nunit.g.cs", NunitAdapter(attributes));
			registeredAdapters.Add("NunitAdapter");
		}

		if (settings.HasTUnit)
		{
			context.AddSource("TUnit.g.cs", TUnitAdapter(attributes, settings.HasTUnitAssertions));
			registeredAdapters.Add("TUnitAdapter");
		}

		if (settings.HasXunit2)
		{
			context.AddSource("Xunit2.g.cs", Xunit2Adapter(attributes));
			registeredAdapters.Add("Xunit2Adapter");
		}

		if (settings.HasXunit3Assert)
		{
			context.AddSource("Xunit3.g.cs", Xunit3AssertAdapter(attributes));
			registeredAdapters.Add("Xunit3Adapter");
		}
		else if (settings.HasXunit3Core)
		{
			context.AddSource("Xunit3.g.cs", Xunit3CoreAdapter(attributes));
			registeredAdapters.Add("Xunit3Adapter");
		}

		// Without `ModuleInitializerAttribute` the target framework cannot be trimmed or AOT-published anyway,
		// and emitting a polyfill would collide with generators like PolySharp, which are invisible here.
		if (!settings.HasTestFrameworkRegistry || !settings.HasModuleInitializer || registeredAdapters.Count == 0)
		{
			return;
		}

		foreach (string adapter in registeredAdapters)
		{
			context.AddSource($"{adapter}.Registration.g.cs", AdapterRegistration(adapter));
		}
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
	///     A consumer pinned below C# 9 could not compile the module initializer, so the adapter is then found by the
	///     assembly scan.
	/// </remarks>
	private static bool HasModuleInitializerLanguageVersion(Compilation compilation)
		=> compilation is CSharpCompilation { LanguageVersion: >= LanguageVersion.CSharp9, };

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
		bool HasModuleInitializer);

	private static string AdapterRegistration(string adapterName) =>
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
		  	}
		  }
		  """;

	private static string MsTestAdapter(string attributes) =>
		$$"""
		  namespace aweXpect.Frameworks
		  {
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
		  }
		  """;

	private static string NunitAdapter(string attributes) =>
		$$"""
		  namespace aweXpect.Frameworks
		  {
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
		         namespace aweXpect.Frameworks
		         {
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
		         }
		         """;
	}

	private static string Xunit2Adapter(string attributes) =>
		$$"""
		  namespace aweXpect.Frameworks
		  {
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
		  }
		  """;

	private static string Xunit3CoreAdapter(string attributes) =>
		$$"""
		  namespace aweXpect.Frameworks
		  {
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
		  }
		  """;

	private static string Xunit3AssertAdapter(string attributes) =>
		$$"""
		  namespace aweXpect.Frameworks
		  {
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
}
