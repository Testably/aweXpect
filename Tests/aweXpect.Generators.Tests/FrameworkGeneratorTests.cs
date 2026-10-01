using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace aweXpect.Generators.Tests;

/// <remarks>
///     The test host references xunit v2, so every run also emits the xunit v2 adapter. The other frameworks are
///     stand-ins that declare only the exception types the adapters throw.
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
		// is the same as the one of the xunit v2 assertions the test host references.
		["xunit.v3.core"] = GeneratorRunner.CompileToReference("xunit.v3.core", "namespace Xunit.v3 { }"),
		["xunit.v3.assert"] = GeneratorRunner.CompileToReference("xunit.v3.assert", "namespace Xunit.v3 { }"),
	};

	public static TheoryData<string, string> AdaptersAndFrameworks => new()
	{
		{ "MsTestAdapter", "MSTest.TestFramework" },
		{ "NunitAdapter", "nunit.framework" },
		{ "TUnitAdapter", "TUnit.Core" },
		{ "TUnitAdapter", "TUnit.Core,TUnit.Assertions" },
		{ "Xunit2Adapter", "" },
		{ "Xunit3Adapter", "xunit.v3.core" },
		{ "Xunit3Adapter", "xunit.v3.core,xunit.v3.assert" },
	};

	public static TheoryData<string, string, LanguageVersion> AdaptersAndFrameworksForEachLanguageVersion
	{
		get
		{
			TheoryData<string, string, LanguageVersion> data = new();
			foreach (object[] adapterAndFrameworks in AdaptersAndFrameworks)
			{
				foreach (LanguageVersion languageVersion in new[]
				         {
					         LanguageVersion.CSharp7_3, LanguageVersion.CSharp8, LanguageVersion.CSharp9,
					         LanguageVersion.CSharp10, LanguageVersion.CSharp11, LanguageVersion.Latest,
				         })
				{
					data.Add((string)adapterAndFrameworks[0], (string)adapterAndFrameworks[1], languageVersion);
				}
			}

			return data;
		}
	}

	[Theory]
	[MemberData(nameof(AdaptersAndFrameworks))]
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

	[Theory]
	[MemberData(nameof(AdaptersAndFrameworksForEachLanguageVersion))]
	public async Task WhenConsumerUsesCSharp7_3OrLater_ShouldCompile(string adapter, string frameworks,
		LanguageVersion languageVersion)
	{
		GeneratorRunner.GeneratorResult result = Run(frameworks, languageVersion);

		await That(result.Errors).IsEmpty()
			.Because("net48 defaults to C# 7.3 and net6 to C# 10");
		await That(result.Warnings).IsEmpty();
		await That(result.Generated).Contains($"class {adapter} ");
	}

	[Fact]
	public async Task WhenConsumerUsesCSharp8_ShouldNotRegisterTheAdapter()
	{
		GeneratorRunner.GeneratorResult result = Run("nunit.framework", LanguageVersion.CSharp8);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).Contains("class NunitAdapter ").And
			.DoesNotContain("ModuleInitializer")
			.Because("a module initializer needs C# 9, so the adapter is found by scanning the loaded assemblies");
	}

	[Fact]
	public async Task WhenConsumerUsesCSharp9_ShouldRegisterTheAdapter()
	{
		GeneratorRunner.GeneratorResult result = Run("nunit.framework", LanguageVersion.CSharp9);

		await That(result.Errors).IsEmpty();
		await That(result.Generated).Contains(
			"global::aweXpect.Core.Adapters.TestFrameworkRegistry.Register(new NunitAdapter(), overwrite: false);");
	}

	[Fact]
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

	[Fact]
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
		=> GeneratorRunner.Run(new FrameworkGenerator(), [source,], true, languageVersion,
			frameworks.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => Frameworks[x]).ToArray());
}
