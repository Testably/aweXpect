using System.Linq;
using aweXpect.SourceGenerators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace aweXpect.Generators.Tests;

public sealed class ExpectationGeneratorTests
{
	[Test]
	public async Task WhenClassHasNoNamespace_ShouldEmitItWithoutANamespace()
	{
		GeneratorRunner.GeneratorResult result = Run(
			"""
			using aweXpect.SourceGenerators;

			[CreateExpectationOn<int>("IsZero", "{value} == 0", ExpectationText = "is zero")]
			public static partial class ThatInt;
			""");

		await That(result.Generated).Contains("public static partial class ThatInt").Once();
		await That(result.Generated).DoesNotContain("namespace <global namespace>")
			.Because("a class in the global namespace has no namespace to declare");
	}

	[Test]
	public async Task WhenClassIsDeclaredInSeveralParts_ShouldEmitEachExpectationOnce()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(new ExpectationGenerator(),
		[
			"""
			using aweXpect.SourceGenerators;

			namespace Lib;

			[CreateExpectationOn<int>("IsZero", "{value} == 0", ExpectationText = "is zero")]
			public static partial class ThatInt;
			""",
			"""
			using aweXpect.SourceGenerators;

			namespace Lib;

			[CreateExpectationOn<int>("IsOne", "{value} == 1", ExpectationText = "is one")]
			public static partial class ThatInt;
			""",
		], false);

		await That(result.Generated).Contains(" IsZero(this ").Once()
			.Because("every part only contributes the expectations of its own attributes");
		await That(result.Generated).Contains(" IsOne(this ").Once();
	}

	[Test]
	public async Task WhenClassIsInternal_ShouldKeepItsAccessibility()
	{
		GeneratorRunner.GeneratorResult result = Run(
			"""
			using aweXpect.SourceGenerators;

			namespace Lib;

			[CreateExpectationOn<int>("IsZero", "{value} == 0", ExpectationText = "is zero")]
			internal static partial class ThatInt;
			""");

		await That(result.Generated).Contains("internal static partial class ThatInt").Once()
			.Because("the parts of a partial class must not declare conflicting accessibilities");
	}

	[Test]
	public async Task WhenClassIsNested_AndOnlyTouched_ShouldReuseTheCachedProblem()
	{
		CSharpCompilation compilation = GeneratorRunner.CreateCompilation([
			"""
			using aweXpect.SourceGenerators;

			namespace Lib;

			public static partial class Outer
			{
				[CreateExpectationOn<int>("IsZero", "{value} == 0", ExpectationText = "is zero")]
				public static partial class ThatInt;
			}
			""",
		], false);
		GeneratorDriver driver = CSharpGeneratorDriver.Create([new ExpectationGenerator().AsSourceGenerator(),],
			parseOptions: (CSharpParseOptions)compilation.SyntaxTrees[0].Options,
			driverOptions: new GeneratorDriverOptions(IncrementalGeneratorOutputKind.None, true));
		driver = driver.RunGenerators(compilation);
		SyntaxTree tree = compilation.SyntaxTrees.Last();
		Compilation touched = compilation.ReplaceSyntaxTree(tree,
			CSharpSyntaxTree.ParseText(tree + Environment.NewLine + "// touched", (CSharpParseOptions)tree.Options));

		driver = driver.RunGenerators(touched);

		GeneratorRunResult result = driver.GetRunResult().Results[0];
		IncrementalStepRunReason[] reasons = result.TrackedSteps["Expectations"]
			.SelectMany(x => x.Outputs).Select(x => x.Reason).ToArray();
		await That(result.Diagnostics).HasSingle().Which
			.Satisfies(x => x.Id == "aweXpect3005" && x.Location.GetLineSpan().StartLinePosition.Line == 7);
		await That(reasons).IsNotEmpty();
		await That(reasons).All()
			.Satisfy(x => x is IncrementalStepRunReason.Cached or IncrementalStepRunReason.Unchanged)
			.Because("the problem keeps only the position of its location, not the syntax tree that every edit replaces");
	}

	[Test]
	public async Task WhenClassIsNested_ShouldReportADiagnostic()
	{
		GeneratorRunner.GeneratorResult result = Run(
			"""
			using aweXpect.SourceGenerators;

			namespace Lib;

			public static partial class Outer
			{
				[CreateExpectationOn<int>("IsZero", "{value} == 0", ExpectationText = "is zero")]
				public static partial class ThatInt;
			}
			""");

		await That(result.Generated).DoesNotContain(" IsZero(this ")
			.Because("extension methods can only be declared in a top-level class");
		await That(result.GeneratorDiagnostics).HasSingle().Which
			.Satisfies(x => x.Id == "aweXpect3005" && x.Severity == DiagnosticSeverity.Error &&
			                x.GetMessage() ==
			                "'Lib.Outer.ThatInt' cannot hold the generated expectations, because extension methods need a top-level static class");
	}

	[Test]
	public async Task WhenCompilationChanges_ShouldCacheTheExpectations()
	{
		GeneratorDriverRunResult result = GeneratorRunner.RunTwice(new ExpectationGenerator(),
		[
			"""
			using aweXpect.SourceGenerators;

			namespace Lib;

			[CreateExpectationOn<int>("IsZero", "{value} == 0", ExpectationText = "is zero", Using = ["System.Text"])]
			public static partial class ThatInt;
			""",
		], "public class Unrelated;", false);

		IncrementalStepRunReason[] reasons = result.Results[0].TrackedSteps["Expectations"]
			.SelectMany(x => x.Outputs).Select(x => x.Reason).ToArray();
		await That(reasons).IsNotEmpty();
		await That(reasons).All()
			.Satisfy(x => x is IncrementalStepRunReason.Cached or IncrementalStepRunReason.Unchanged)
			.Because("an unchanged declaration has to compare equal, so that its output is not produced again");
	}

	[Test]
	public async Task WhenCompilationChanges_ShouldKeepTheGeneratedSources()
	{
		GeneratorDriverRunResult result = GeneratorRunner.RunTwice(new ExpectationGenerator(),
		[
			"""
			using aweXpect.SourceGenerators;

			namespace Lib;

			[CreateExpectationOn<int>("IsZero", "{value} == 0", ExpectationText = "is zero")]
			public static partial class ThatInt;
			""",
		], "public class Unrelated;", false);

		await That(string.Concat(result.GeneratedTrees.Select(x => x.ToString()))).Contains(" IsZero(this ").Once()
			.Because("an edit elsewhere must not drop the output of an unchanged declaration");
	}

	[Test]
	public async Task WhenExpectationTextContainsQuotesAndBackslashes_ShouldEscapeThem()
	{
		GeneratorRunner.GeneratorResult result = Run(
			"""
			using aweXpect.SourceGenerators;

			namespace Lib;

			[CreateExpectationOn<string>("IsQuoted", "{value} == \"\\\"\"", ExpectationText = "is {not} \"\\\"")]
			public static partial class ThatString;
			""");

		await That(result.Generated).Contains("""Grammars.Verb("is \"\\\"", "are \"\\\"")""").Once()
			.Because("the expectation text is embedded in a string literal");
		await That(result.Generated).Contains("""Grammars.Verb("is not \"\\\"", "are not \"\\\"")""").Once();
	}

	[Test]
	public async Task WhenExpectationTextContainsXmlCharacters_ShouldEscapeThemInTheSummary()
	{
		GeneratorRunner.GeneratorResult result = Run(
			"""
			using aweXpect.SourceGenerators;

			namespace Lib;

			[CreateExpectationOn<int>("Is{Not}Between", "{value} is > 0 and < 10", ExpectationText = "is {not} > 0 & < 10")]
			public static partial class ThatInt;
			""");

		await That(result.Generated).Contains("///     Verifies that the subject is &gt; 0 &amp; &lt; 10.").Once()
			.Because("the expectation text is plain text, but the summary is XML");
		await That(result.Generated).Contains("///     Verifies that the subject is not &gt; 0 &amp; &lt; 10.").Once();
	}

	[Test]
	public async Task WhenSameNamedClassesAreInDifferentNamespaces_ShouldEmitBoth()
	{
		GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(new ExpectationGenerator(),
		[
			"""
			using aweXpect.SourceGenerators;

			namespace Lib.A;

			[CreateExpectationOn<int>("IsZero", "{value} == 0", ExpectationText = "is zero")]
			public static partial class ThatInt;
			""",
			"""
			using aweXpect.SourceGenerators;

			namespace Lib.B;

			[CreateExpectationOn<int>("IsZero", "{value} == 0", ExpectationText = "is zero")]
			public static partial class ThatInt;
			""",
		], false);

		await That(result.GeneratorDiagnostics).IsEmpty();
		await That(result.Generated).Contains(" IsZero(this ").Exactly(2)
			.Because("the hint name has to tell the two classes apart");
		await That(result.Generated).Contains("namespace Lib.A;").Once();
		await That(result.Generated).Contains("namespace Lib.B;").Once();
	}

	[Test]
	public async Task WhenSummaryHasSeveralLines_ShouldContinueTheDocumentationComment()
	{
		GeneratorRunner.GeneratorResult result = Run(
			"""
			using aweXpect.SourceGenerators;

			namespace Lib;

			[CreateExpectationOnNullable<string>("IsEmpty", "{value} == \"\"", ExpectationText = "is empty",
				Summary = "Verifies that the subject\nis empty.")]
			public static partial class ThatString;
			""");

		await That(result.Generated).Contains("\t///     is empty.").Once()
			.Because("every line of the summary has to stay inside the documentation comment");
	}

	[Test]
	public async Task WhenTheNameHasNoNotPlaceholder_ShouldEmitOnlyThePositiveOverload()
	{
		GeneratorRunner.GeneratorResult result = Run(
			"""
			using aweXpect.SourceGenerators;

			namespace Lib;

			[CreateExpectationOn<int>("IsZero", "{value} == 0", ExpectationText = "is zero")]
			public static partial class ThatInt;
			""");

		await That(result.Generated).Contains(" IsZero(this ").Once();
		await That(result.Generated).DoesNotContain(".Invert()")
			.Because("only a name with {Not} declares a negated overload");
	}

	[Test]
	[Arguments("CreateExpectationOn")]
	[Arguments("CreateExpectationOnNullable")]
	public async Task WithNegatedRemarks_ShouldUseThemForTheNegatedOverload(string attribute)
	{
		GeneratorRunner.GeneratorResult result = Run(
			$$"""
			  using aweXpect.SourceGenerators;

			  namespace Lib;

			  [{{attribute}}<int>("Is{Not}Zero", "{value} == 0", ExpectationText = "is {not} zero",
			  	Remarks = "The value is zero.",
			  	NegatedRemarks = "The value is\nnot zero.")]
			  public static partial class ThatInt;
			  """);

		await That(DocumentationOf(result.Generated, "IsZero"))
			.Contains("\t/// <remarks>\n\t///     The value is zero.\n\t/// </remarks>").And
			.DoesNotContain("not zero");
		await That(DocumentationOf(result.Generated, "IsNotZero"))
			.Contains("\t/// <remarks>\n\t///     The value is\n\t///     not zero.\n\t/// </remarks>").And
			.DoesNotContain("The value is zero.")
			.Because("the remarks of the positive overload describe the opposite");
	}

	[Test]
	public async Task WithNotPlaceholder_ShouldEmitBothPolarities()
	{
		GeneratorRunner.GeneratorResult result = Run(
			"""
			using aweXpect.SourceGenerators;

			namespace Lib;

			[CreateExpectationOn<int>("Is{Not}Zero", "{value} == 0", ExpectationText = "is {not} zero")]
			public static partial class ThatInt;
			""");

		await That(result.Generated).Contains(" IsZero(this ").Once();
		await That(result.Generated).Contains(" IsNotZero(this ").Once();
		await That(result.Generated).Contains("///     Verifies that the subject is not zero.").Once();
		await That(result.Generated).Contains("Grammars.Verb(\"is not zero\", \"are not zero\")").Once();
	}

	[Test]
	[Arguments("CreateExpectationOn")]
	[Arguments("CreateExpectationOnNullable")]
	public async Task WithRemarks_WithoutNegatedRemarks_ShouldEmitNoRemarksForTheNegatedOverload(string attribute)
	{
		GeneratorRunner.GeneratorResult result = Run(
			$$"""
			  using aweXpect.SourceGenerators;

			  namespace Lib;

			  [{{attribute}}<int>("Is{Not}Zero", "{value} == 0", ExpectationText = "is {not} zero",
			  	Remarks = "The value is zero.")]
			  public static partial class ThatInt;
			  """);

		await That(DocumentationOf(result.Generated, "IsZero"))
			.Contains("\t/// <remarks>\n\t///     The value is zero.\n\t/// </remarks>");
		await That(DocumentationOf(result.Generated, "IsNotZero")).DoesNotContain("<remarks>")
			.Because("the remarks of the positive overload describe the opposite");
	}

	[Test]
	public async Task WithSummary_ShouldUseItForThePositiveOverload()
	{
		GeneratorRunner.GeneratorResult result = Run(
			"""
			using aweXpect.SourceGenerators;

			namespace Lib;

			[CreateExpectationOnNullable<string>("Is{Not}NullOrEmpty", "string.IsNullOrEmpty({value})",
				ExpectationText = "is {not} null or empty",
				Summary = "Verifies that the subject is <see langword=\"null\" /> or <see cref=\"string.Empty\" />.",
				FailOnNull = false,
				NegatedFailsOnNull = true)]
			public static partial class ThatString;
			""");

		await That(result.Generated)
			.Contains("///     Verifies that the subject is <see langword=\"null\" /> or <see cref=\"string.Empty\" />.")
			.Once();
		await That(result.Generated).Contains("///     Verifies that the subject is not null or empty.").Once()
			.Because("the summary only replaces the one of the positive overload");
	}

	/// <summary>
	///     The documentation comment and the attributes of the generated <paramref name="methodName" />.
	/// </summary>
	private static string DocumentationOf(string generated, string methodName)
	{
		int method = generated.IndexOf($" {methodName}(this ", StringComparison.Ordinal);
		int summary = generated.LastIndexOf("/// <summary>", method, StringComparison.Ordinal);
		return generated.Substring(summary, method - summary).Replace("\r\n", "\n");
	}

	private static GeneratorRunner.GeneratorResult Run(string source)
		=> GeneratorRunner.Run(new ExpectationGenerator(), [source,], false);
}
