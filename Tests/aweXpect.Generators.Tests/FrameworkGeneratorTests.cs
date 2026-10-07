using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace aweXpect.Generators.Tests;

/// <remarks>
///     The frameworks are stand-ins that declare only the exception types the adapters throw. Every run references
///     the stand-in for the xunit v2 assertions, so that it also emits the xunit v2 adapter next to the adapter of
///     the framework under test.
/// </remarks>
public sealed class FrameworkGeneratorTests
{
	private static readonly Dictionary<string, MetadataReference> Frameworks = new()
	{
		["MSTest.TestFramework"] = GeneratorRunner.CompileToReference("MSTest.TestFramework", """
		                                                                                      namespace Microsoft.VisualStudio.TestTools.UnitTesting
		                                                                                      {
		                                                                                      	public class AssertFailedException : System.Exception
		                                                                                      	{
		                                                                                      		public AssertFailedException(string message) : base(message) { }
		                                                                                      		public AssertFailedException(string message, System.Exception inner) : base(message, inner) { }
		                                                                                      	}
		                                                                                      	public class AssertInconclusiveException(string message) : System.Exception(message);
		                                                                                      }
		                                                                                      """),
		["nunit.framework"] = GeneratorRunner.CompileToReference("nunit.framework", """
		                                                                            namespace NUnit.Framework
		                                                                            {
		                                                                            	public class AssertionException : System.Exception
		                                                                            	{
		                                                                            		public AssertionException(string message) : base(message) { }
		                                                                            		public AssertionException(string message, System.Exception inner) : base(message, inner) { }
		                                                                            	}
		                                                                            	public class IgnoreException(string message) : System.Exception(message);
		                                                                            	public class InconclusiveException(string message) : System.Exception(message);
		                                                                            }
		                                                                            """),
		["TUnit.Core"] = GeneratorRunner.CompileToReference("TUnit.Core", """
		                                                                  namespace TUnit.Core.Exceptions
		                                                                  {
		                                                                  	public class SkipTestException(string reason) : System.Exception(reason);
		                                                                  	public class InconclusiveTestException(string message, System.Exception? inner)
		                                                                  		: System.Exception(message, inner);
		                                                                  }
		                                                                  """),
		["TUnit.Assertions"] = GeneratorRunner.CompileToReference("TUnit.Assertions", """
		                                                                              namespace TUnit.Assertions.Exceptions
		                                                                              {
		                                                                              	public class AssertionException : System.Exception
		                                                                              	{
		                                                                              		public AssertionException(string? message) : base(message) { }
		                                                                              		public AssertionException(string? message, System.Exception inner) : base(message, inner) { }
		                                                                              	}
		                                                                              }
		                                                                              """),
		// The xunit v3 adapters need nothing from xunit.v3.core, and `Xunit.Sdk.XunitException` from xunit.v3.assert
		// is the same as the one of the xunit v2 assertions.
		["xunit.v3.core"] = GeneratorRunner.CompileToReference("xunit.v3.core", "namespace Xunit.v3 { }"),
		["xunit.v3.assert"] = GeneratorRunner.CompileToReference("xunit.v3.assert", "namespace Xunit.v3 { }"),
	};

	private static readonly MetadataReference Xunit2Assert = GeneratorRunner.CompileToReference("xunit.assert", """
	                                                                                                            namespace Xunit.Sdk
	                                                                                                            {
	                                                                                                            	public class XunitException : System.Exception
	                                                                                                            	{
	                                                                                                            		public XunitException(string message) : base(message) { }
	                                                                                                            		public XunitException(string message, System.Exception inner) : base(message, inner) { }
	                                                                                                            	}
	                                                                                                            }
	                                                                                                            """);

	public static IEnumerable<(string, string)> AdaptersAndFrameworks =>
	[
		("MsTestAdapter", "MSTest.TestFramework"),
		("NunitAdapter", "nunit.framework"),
		("TUnitAdapter", "TUnit.Core"),
		("TUnitAdapter", "TUnit.Core,TUnit.Assertions"),
		("Xunit2Adapter", ""),
		("Xunit3Adapter", "xunit.v3.core"),
		("Xunit3Adapter", "xunit.v3.core,xunit.v3.assert"),
	];

	public static IEnumerable<(string, string, LanguageVersion)> AdaptersAndFrameworksForEachLanguageVersion
	{
		get
		{
			List<(string, string, LanguageVersion)> data = [];
			foreach ((string adapter, string frameworks) in AdaptersAndFrameworks)
			{
				foreach (LanguageVersion languageVersion in new[]
				         {
					         LanguageVersion.CSharp7_3, LanguageVersion.CSharp8, LanguageVersion.CSharp9, LanguageVersion.CSharp10, LanguageVersion.CSharp11, LanguageVersion.Latest,
				         })
				{
					data.Add((adapter, frameworks, languageVersion));
				}
			}

			return data;
		}
	}

	[Test]
	[MethodDataSource(nameof(AdaptersAndFrameworks))]
	public async Task WhenConsumerDeclaresNamespacesAndTypesThatShadowTheUsedOnes_ShouldCompile(string adapter,
		string frameworks)
	{
		GeneratorRunner.GeneratorResult result = Run(frameworks, LanguageVersion.Latest, """
		                                                                                 namespace aweXpect.Microsoft { internal class Shadow { } }
		                                                                                 namespace aweXpect.NUnit { internal class Shadow { } }
		                                                                                 namespace aweXpect.System { internal class Shadow { } }
		                                                                                 namespace aweXpect.TUnit { internal class Shadow { } }
		                                                                                 namespace aweXpect.Xunit { internal class Shadow { } }
		                                                                                 namespace aweXpect.Frameworks
		                                                                                 {
		                                                                                 	internal class Exception { }
		                                                                                 	internal class FailException { }
		                                                                                 	internal class InconclusiveException { }
		                                                                                 	internal interface ITestFrameworkAdapter { }
		                                                                                 	internal class SkipException { }
		                                                                                 }
		                                                                                 """);

		await That(result.Errors).IsEmpty()
			.Because("an extension package could declare a namespace under `aweXpect.` that shadows a framework or the BCL");
		await That(result.Generated).Contains($"class {adapter} ");
	}

	[Test]
	[MethodDataSource(nameof(AdaptersAndFrameworks))]
	public async Task WhenConsumerSeesTheAdapterOfAnotherAssembly_ShouldNotWarn(string adapter, string frameworks)
	{
		MetadataReference helpers = GeneratorRunner.CompileToReference("Company.Testing", $$"""
		                                                                                    [assembly: System.Runtime.CompilerServices.InternalsVisibleTo("GeneratorTests")]
		                                                                                    namespace aweXpect.Frameworks
		                                                                                    {
		                                                                                    	internal class {{adapter}} { }
		                                                                                    	internal static class {{adapter}}Registration { }
		                                                                                    }
		                                                                                    """);

		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(new FrameworkGenerator(),
			["public class Foo { }",], true, LanguageVersion.Latest, References(frameworks, helpers));

		await That(result.Errors).IsEmpty();
		await That(result.Warnings).IsEmpty()
			.Because("a helper library that uses aweXpect with the same test framework and exposes its internals to the test project has its own generated adapter, which must not conflict with the one generated in the test project");
	}

	[Test]
	[MethodDataSource(nameof(AdaptersAndFrameworksForEachLanguageVersion))]
	public async Task WhenConsumerUsesCSharp7_3OrLater_ShouldCompile(string adapter, string frameworks,
		LanguageVersion languageVersion)
	{
		GeneratorRunner.GeneratorResult result = Run(frameworks, languageVersion);

		await That(result.Errors).IsEmpty()
			.Because("net48 defaults to C# 7.3 and net6 to C# 10");
		await That(result.Warnings).IsEmpty();
		await That(result.Generated).Contains($"class {adapter} ");
	}

	[Test]
	public async Task WhenConsumerUsesCSharp8_AndCoreScansTheLoadedAssemblies_ShouldNotWarn()
	{
		MetadataReference netStandardCore = GeneratorRunner.CompileToReference("aweXpect.Core", """
		                                                                                        [assembly: System.Runtime.Versioning.TargetFramework(".NETStandard,Version=v2.0")]
		                                                                                        namespace aweXpect.Core.Adapters
		                                                                                        {
		                                                                                        	public interface ITestFrameworkAdapter { }
		                                                                                        	public static class TestFrameworkRegistry
		                                                                                        	{
		                                                                                        		public static void Register(ITestFrameworkAdapter testFrameworkAdapter, bool overwrite = true) { }
		                                                                                        	}
		                                                                                        }
		                                                                                        """);

		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(new FrameworkGenerator(),
			["public class Foo { }",], false, LanguageVersion.CSharp8, References("nunit.framework", netStandardCore));

		await That(result.Generated).Contains("class NunitAdapter ");
		await That(result.GeneratorDiagnostics).IsEmpty()
			.Because("a consumer below .NET 8 resolves the .NET Standard build of aweXpect.Core, which still finds the adapter by scanning the loaded assemblies");
	}

	[Test]
	public async Task WhenConsumerUsesCSharp8_ShouldDeclareTheAdapterThatCanBeRegisteredManually()
	{
		GeneratorRunner.GeneratorResult result = Run("nunit.framework", LanguageVersion.CSharp8, """
		                                                                                         public static class Setup
		                                                                                         {
		                                                                                         	public static void Register()
		                                                                                         		=> aweXpect.Core.Adapters.TestFrameworkRegistry.Register(new aweXpect.Frameworks.NunitAdapter());
		                                                                                         }
		                                                                                         """);

		await That(result.Errors).IsEmpty()
			.Because("the warning aweXpect2002 tells a consumer that cannot compile a module initializer to register `aweXpect.Frameworks.NunitAdapter` manually");
	}

	[Test]
	public async Task WhenConsumerUsesCSharp8_ShouldNotRegisterTheAdapter()
	{
		GeneratorRunner.GeneratorResult result = Run("nunit.framework", LanguageVersion.CSharp8);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).Contains("class NunitAdapter ").And
			.DoesNotContain("ModuleInitializer")
			.Because("a module initializer needs C# 9, which a consumer pinned to an older language version cannot compile");
	}

	[Test]
	public async Task WhenConsumerUsesCSharp8_ShouldWarnThatTheAdapterIsNotRegistered()
	{
		GeneratorRunner.GeneratorResult result = Run("nunit.framework", LanguageVersion.CSharp8);

		await That(result.GeneratorDiagnostics.Select(x => $"{x.Id} {x.Severity}: {x.GetMessage()}")).Contains(
				"aweXpect2002 Warning: The NunitAdapter cannot register itself, because module initializers need C# 9 or later. Set <LangVersion> to 9 or later, or call TestFrameworkRegistry.Register(new aweXpect.Frameworks.NunitAdapter()) before the first expectation.")
			.Because("the .NET 8 build of aweXpect.Core no longer scans the loaded assemblies, so without the registration a skipped or inconclusive test would fail");
	}

	[Test]
	public async Task WhenConsumerUsesCSharp9_ShouldRegisterTheAdapter()
	{
		GeneratorRunner.GeneratorResult result = Run("nunit.framework", LanguageVersion.CSharp9);

		await That(result.Errors).IsEmpty();
		await That(result.GeneratorDiagnostics).IsEmpty();
		await That(result.Generated).Contains(
			"global::aweXpect.Core.Adapters.TestFrameworkRegistry.Register(new NunitAdapter(), overwrite: false);");
	}

	[Test]
	public async Task WhenTUnitAssertionsIsNotReferenced_ShouldFailWithTheFailExceptionOfAweXpect()
	{
		GeneratorRunner.GeneratorResult result = Run("TUnit.Core", LanguageVersion.Latest);

		await That(result.Errors).IsEmpty();
		await That(result.Generated)
			.Contains("=> throw new global::TUnit.Core.Exceptions.SkipTestException(message);").And
			.Contains("=> throw new global::aweXpect.FailException(message);").And
			.Contains("=> throw new global::aweXpect.FailException(message, innerException);").And
			.DoesNotContain("TUnit.Assertions")
			.Because("skipping only needs TUnit.Core, which is all a project that uses aweXpect instead of TUnit's assertions references");
	}

	[Test]
	public async Task WhenTUnitAssertionsIsReferenced_ShouldFailWithTheAssertionExceptionOfTUnit()
	{
		GeneratorRunner.GeneratorResult result = Run("TUnit.Core,TUnit.Assertions", LanguageVersion.Latest);

		await That(result.Errors).IsEmpty();
		await That(result.Generated)
			.Contains("=> throw new global::TUnit.Assertions.Exceptions.AssertionException(message);").And
			.Contains("=> throw new global::TUnit.Assertions.Exceptions.AssertionException(message, innerException);");
	}

	private static GeneratorRunner.GeneratorResult Run(string frameworks, LanguageVersion languageVersion,
		string source = "public class Foo { }")
		=> GeneratorRunner.Run(new FrameworkGenerator(), [source,], true, languageVersion, References(frameworks));

	private static MetadataReference[] References(string frameworks, params MetadataReference[] additionalReferences)
		=> frameworks.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => Frameworks[x])
			.Append(Xunit2Assert).Concat(additionalReferences).ToArray();
}
