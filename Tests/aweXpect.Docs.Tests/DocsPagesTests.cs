using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace aweXpect.Docs.Tests;

public sealed class DocsPagesTests
{
	private static readonly string ProjectDirectory = GetProjectDirectory();
	private static readonly string PagesDirectory =
		Path.GetFullPath(Path.Combine(ProjectDirectory, "..", "..", "Docs", "pages"));
	private static readonly string ScaffoldDirectory = Path.Combine(ProjectDirectory, "Scaffold");

	/// <summary>
	///     All pages on .NET, but on .NET Framework only the pages for extension authors, so that they also compile
	///     against the netstandard2.0 build.
	/// </summary>
	public static TheoryData<string> Pages()
	{
#if NETFRAMEWORK
		return ExtensionPages();
#else
		TheoryData<string> pages = new();
		foreach (string page in AllPages())
		{
			pages.Add(page);
		}

		return pages;
#endif
	}

	[Test]
	[MethodDataSource(nameof(Pages))]
	public void CodeBlocks_ShouldCompile(string page)
	{
		List<string> errors = SnippetCompiler.GetErrors(ExtractCodeBlocks(page), ScaffoldFiles(page));

		Fail.Unless(errors.Count == 0, string.Join(Environment.NewLine, errors));
	}

	/// <summary>
	///     The pages for extension authors, which tell them to reference only aweXpect.Core.
	/// </summary>
	public static TheoryData<string> ExtensionPages()
	{
		TheoryData<string> pages = new();
		foreach (string page in AllPages().Where(page => page.StartsWith("11-extending/")))
		{
			pages.Add(page);
		}

		return pages;
	}

	[Test]
	[MethodDataSource(nameof(ExtensionPages))]
	public void ExtensionCode_ShouldCompileAgainstCoreOnly(string page)
	{
		List<string> errors = SnippetCompiler.GetErrors(ExtractCodeBlocks(page), ScaffoldFiles(page), true);

		Fail.Unless(errors.Count == 0, string.Join(Environment.NewLine, errors));
	}

	[Test]
	public async Task Extract_ShouldFindTheCodeBlocksOfAllPages()
	{
		int count = AllPages().Sum(page => ExtractCodeBlocks(page).Count);

		await That(count).IsGreaterThan(250)
			.Because("the check must not pass because it no longer finds the code blocks");
	}

	[Test]
	public async Task Extract_ShouldOnlyReturnCSharpBlocksWithoutNoCompileMarker()
	{
		string[] lines =
		[
			"# Title",
			"```csharp",
			"int a = 1;",
			"```",
			"```csharp no-compile",
			"int b = 2;",
			"```",
			"  ```csharp title=\"aweXpect\"",
			"  int c = 3;",
			"  int d = 4;",
			"  ```",
			"```xml",
			"<e />",
			"```",
		];

		List<CodeBlock> blocks = CodeBlock.Extract("page.md", lines);

		await That(blocks).IsEqualTo([
			new CodeBlock("page.md", 3, "int a = 1;"),
			new CodeBlock("page.md", 9, "  int c = 3;\n  int d = 4;"),
		]);
	}

	[Test]
	public async Task GetErrors_ShouldReportThePageAndLineOfTheBlock()
	{
		CodeBlock block = new("Docs/pages/page.md", 10, "int value = 1;\nawait Expect.That(value).IsFoo();");

		List<string> errors = SnippetCompiler.GetErrors([block,], ScaffoldFiles("page.md"));

		await That(errors).HasSingle().Which.StartsWith("Docs/pages/page.md(11,26): CS1061:");
	}

	[Test]
	public async Task GetErrors_ShouldShareTypesAndUsingsBetweenTheBlocksOfAPage()
	{
		CodeBlock first = new("page.md", 1, """
		                                    using aweXpect.Core;

		                                    public static IThat<int> IsAnswer(this IThat<int> subject)
		                                        => subject;

		                                    public class Answer
		                                    {
		                                        public int Value => 42;
		                                    }
		                                    """);
		CodeBlock second = new("page.md", 20, "Times times = 2.Times();\nawait Expect.That(new Answer().Value).IsAnswer().IsEqualTo(42);");

		List<string> errors = SnippetCompiler.GetErrors([first, second,], ScaffoldFiles("page.md"));

		await That(errors).IsEmpty();
	}

	private static IEnumerable<string> AllPages()
		=> Directory.EnumerateFiles(PagesDirectory, "*.*", SearchOption.AllDirectories)
			.Where(path => path.EndsWith(".md") || path.EndsWith(".mdx"))
			.Select(path => path.Substring(PagesDirectory.Length + 1).Replace('\\', '/'))
			.OrderBy(page => page, StringComparer.Ordinal);

	private static List<CodeBlock> ExtractCodeBlocks(string page)
		=> CodeBlock.Extract($"Docs/pages/{page}", File.ReadAllLines(Path.Combine(PagesDirectory, page)));

	private static string GetProjectDirectory([CallerFilePath] string path = "")
		=> Path.GetDirectoryName(path)!;

	private static IEnumerable<string> ScaffoldFiles(string page)
	{
		yield return Path.Combine(ScaffoldDirectory, "Scaffold.cs");
		string pageScaffold = Path.Combine(ScaffoldDirectory, Path.ChangeExtension(page, ".cs"));
		if (File.Exists(pageScaffold))
		{
			yield return pageScaffold;
		}
	}
}
