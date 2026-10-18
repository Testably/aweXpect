using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace aweXpect.Snapshots.PublicApi.Tests.TestHelpers;

public static class Compiler
{
	/// <summary>
	///     Compiles the <paramref name="source" /> to an assembly, or to a reference assembly when
	///     <paramref name="metadataOnly" /> is set.
	/// </summary>
	public static byte[] Compile(string name, string version, string source, MetadataReference[]? references = null,
		bool metadataOnly = false)
	{
		CSharpCompilation compilation = CSharpCompilation.Create(name,
			[CSharpSyntaxTree.ParseText($"[assembly: System.Reflection.AssemblyVersion(\"{version}\")]\n{source}"),],
			(references ?? []).Append(MetadataReference.CreateFromFile(typeof(object).Assembly.Location)),
			new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
		using MemoryStream stream = new();
		EmitResult result = compilation.Emit(stream,
			options: new EmitOptions(metadataOnly, includePrivateMembers: !metadataOnly));
		if (!result.Success)
		{
			throw new InvalidOperationException(string.Join(Environment.NewLine, result.Diagnostics));
		}

		return stream.ToArray();
	}

	/// <summary>
	///     Writes the <paramref name="bytes" /> of an assembly as <paramref name="fileName" /> into a new directory, or
	///     into the <paramref name="directory" />.
	/// </summary>
	public static FileInfo WriteFile(string fileName, byte[] bytes, string? directory = null)
	{
		directory ??= Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "aweXpect.Snapshots.PublicApi.Tests",
			Guid.NewGuid().ToString("N"))).FullName;
		string path = Path.Combine(directory, fileName);
		File.WriteAllBytes(path, bytes);
		return new FileInfo(path);
	}
}
