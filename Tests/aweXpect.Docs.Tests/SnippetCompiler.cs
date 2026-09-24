using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace aweXpect.Docs.Tests;

/// <summary>
///     Compiles the code blocks of a page together with scaffold files, which declare the placeholders that the
///     samples take for granted.
/// </summary>
/// <remarks>
///     All blocks of a page share the namespace <c>Snippets</c>, so that a type or a <c>using</c> directive that a page
///     shows once applies to all of its blocks, like it does for a reader. A type that several blocks declare, e.g. to
///     show variants, stays local to each of them. The statements of each block become the body of an async method,
///     and its members, e.g. an extension method, the members of the surrounding class. A <c>#line</c> directive maps
///     every part back to the page, and an initializer that is only a <c>//...</c> comment becomes <c>default!</c>.
/// </remarks>
internal static class SnippetCompiler
{
	private static readonly CSharpParseOptions ParseOptions = new(LanguageVersion.Latest);
	private static readonly Regex OmittedInitializer = new(@"=\s*//\s*\.\.\.");
	private static readonly Lazy<MetadataReference[]> References = new(GetReferences);

	/// <summary>
	///     Returns the compiler errors of the <paramref name="blocks" /> and <paramref name="scaffoldFiles" />, each
	///     prefixed with the file and line.
	/// </summary>
	public static List<string> GetErrors(IReadOnlyList<CodeBlock> blocks, IEnumerable<string> scaffoldFiles)
	{
		List<(CodeBlock Block, CompilationUnitSyntax Root)> parsed = blocks
			.Select(block => (block, CSharpSyntaxTree.ParseText(
				OmittedInitializer.Replace(block.Code, "= default!; //..."), ParseOptions).GetCompilationUnitRoot()))
			.ToList();
		HashSet<string> sharedTypes = parsed
			.SelectMany(x => x.Root.Members.Select(GetTypeName).OfType<string>().Distinct())
			.GroupBy(name => name)
			.Where(group => group.Count() == 1)
			.Select(group => group.Key)
			.ToHashSet();

		StringBuilder source = new();
		source.AppendLine("namespace Snippets").AppendLine("{");
		foreach ((CodeBlock block, CompilationUnitSyntax root) in parsed)
		{
			AppendMapped(source, block, root.Usings);
		}

		for (int i = 0; i < parsed.Count; i++)
		{
			AppendBlock(source, parsed[i].Block, parsed[i].Root, i, sharedTypes);
		}

		source.AppendLine("}");

		CSharpCompilation compilation = CSharpCompilation.Create("Snippets",
			scaffoldFiles
				.Select(file => CSharpSyntaxTree.ParseText(File.ReadAllText(file), ParseOptions, file))
				.Append(CSharpSyntaxTree.ParseText(source.ToString(), ParseOptions, "Snippets.g.cs")),
			References.Value,
			new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
				nullableContextOptions: NullableContextOptions.Enable));

		return compilation.GetDiagnostics()
			.Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
			.Select(diagnostic =>
			{
				FileLinePositionSpan span = diagnostic.Location.GetMappedLineSpan();
				return $"{span.Path}({span.StartLinePosition.Line + 1},{span.StartLinePosition.Character + 1}): " +
				       $"{diagnostic.Id}: {diagnostic.GetMessage(CultureInfo.InvariantCulture)}";
			})
			.ToList();
	}

	private static void AppendBlock(StringBuilder source, CodeBlock block, CompilationUnitSyntax root, int index,
		HashSet<string> sharedTypes)
	{
		List<SyntaxNode> statements = [];
		List<SyntaxNode> members = [];
		List<SyntaxNode> types = [];
		foreach (MemberDeclarationSyntax member in root.Members)
		{
			if (member is GlobalStatementSyntax { Statement: var statement, } &&
			    !(statement is LocalFunctionStatementSyntax localFunction && IsMethod(localFunction)))
			{
				statements.Add(member);
			}
			else if (member is BaseNamespaceDeclarationSyntax ||
			         (GetTypeName(member) is { } name && sharedTypes.Contains(name) &&
			          !member.Modifiers.Any(m => m.IsKind(SyntaxKind.PrivateKeyword) || m.IsKind(SyntaxKind.ProtectedKeyword))))
			{
				types.Add(member);
			}
			else
			{
				members.Add(member);
			}
		}

		string modifier = members.Any(member => member is GlobalStatementSyntax
		{
			Statement: LocalFunctionStatementSyntax localFunction,
		} && IsExtensionMethod(localFunction))
			? "static "
			: "";

		source.AppendLine("#line default")
			.AppendLine($"public {modifier}class Block{index}")
			.AppendLine("{")
			.AppendLine($"public {modifier}async Task Run()")
			.AppendLine("{");
		AppendMapped(source, block, statements);
		source.AppendLine("#line default").AppendLine("}");
		AppendMapped(source, block, members);
		source.AppendLine("#line default").AppendLine("}");
		AppendMapped(source, block, types);
	}

	private static string? GetTypeName(MemberDeclarationSyntax member)
		=> member switch
		{
			BaseTypeDeclarationSyntax type => type.Identifier.Text,
			DelegateDeclarationSyntax @delegate => @delegate.Identifier.Text,
			_ => null,
		};

	/// <summary>
	///     A method at the top level of a block is parsed as a local function, so its accessibility or a <c>this</c>
	///     parameter tells that the sample shows a method of a class.
	/// </summary>
	private static bool IsMethod(LocalFunctionStatementSyntax localFunction)
		=> IsExtensionMethod(localFunction) || localFunction.Modifiers.Any(modifier =>
			modifier.IsKind(SyntaxKind.PublicKeyword) || modifier.IsKind(SyntaxKind.InternalKeyword) ||
			modifier.IsKind(SyntaxKind.PrivateKeyword) || modifier.IsKind(SyntaxKind.ProtectedKeyword));

	private static bool IsExtensionMethod(LocalFunctionStatementSyntax localFunction)
		=> localFunction.ParameterList.Parameters.FirstOrDefault()?.Modifiers.Any(SyntaxKind.ThisKeyword) == true;

	private static void AppendMapped(StringBuilder source, CodeBlock block, IEnumerable<SyntaxNode> nodes)
	{
		foreach (SyntaxNode node in nodes)
		{
			int line = block.Line + node.SyntaxTree.GetLineSpan(node.FullSpan).StartLinePosition.Line;
			source.AppendLine($"#line {line} \"{block.Page}\"").AppendLine(node.ToFullString());
		}
	}

	private static MetadataReference[] GetReferences()
	{
		Dictionary<string, string> paths = new(StringComparer.OrdinalIgnoreCase);
#if NETFRAMEWORK
		foreach (string path in Directory.GetFiles(
			         System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(), "*.dll")
			         .Concat(Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")))
		{
			if (IsManaged(path))
			{
				paths[Path.GetFileNameWithoutExtension(path)] = path;
			}
		}
#else
		foreach (string path in ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator))
		{
			paths[Path.GetFileNameWithoutExtension(path)] = path;
		}
#endif

		paths.Remove(typeof(SnippetCompiler).Assembly.GetName().Name!);
		return paths.Values.Select(path => (MetadataReference)MetadataReference.CreateFromFile(path)).ToArray();
	}
#if NETFRAMEWORK

	private static bool IsManaged(string path)
	{
		try
		{
			using FileStream stream = File.OpenRead(path);
			using System.Reflection.PortableExecutable.PEReader reader = new(stream);
			return reader.HasMetadata &&
			       System.Reflection.Metadata.PEReaderExtensions.GetMetadataReader(reader).IsAssembly;
		}
		catch (BadImageFormatException)
		{
			return false;
		}
	}
#endif
}
