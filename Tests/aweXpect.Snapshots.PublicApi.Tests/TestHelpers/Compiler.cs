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
	///     Writes a library that forwards a type, a generic type and a type with a nested type to the dependency next to
	///     it.
	/// </summary>
	public static FileInfo WriteForwardingLibrary()
	{
		byte[] dependency = Compile("Dependency", "1.0.0.0", """
			namespace Dependency;
			public class Gate { }
			public class Box<T> { }
			public class Outer { public class Inner { } }
			""");
		FileInfo library = WriteFile("Library.dll", Compile("Library", "1.0.0.0", """
			[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Dependency.Gate))]
			[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Dependency.Box<>))]
			[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Dependency.Outer))]
			namespace Library;
			public class Api { }
			""", [MetadataReference.CreateFromImage(dependency),]));
		WriteFile("Dependency.dll", dependency, library.DirectoryName);
		return library;
	}

	/// <summary>
	///     Writes a library with the attributes that are stored as metadata flags, and with types for which the compiler
	///     could set them implicitly.
	/// </summary>
	/// <remarks>
	///     The .NET Framework declares <c>DefaultParameterValueAttribute</c> in <c>System.dll</c> and no
	///     <c>IsExternalInit</c>, which records require.
	/// </remarks>
	public static FileInfo WritePseudoAttributesLibrary()
		=> WriteFile("Library.dll", Compile("Library", "1.0.0.0", """
			namespace Library
			{
			    [System.Serializable] public class Payload { }
			    public class Plain { }
			    public enum Kind { A }
			    public delegate void Handler();
			    public record Note(int Id);
			    public static class Native
			    {
			        [System.Runtime.InteropServices.DllImport("x")]
			        public static extern void Beep();
			        [System.Runtime.InteropServices.DllImport("x", EntryPoint = "send",
			            CharSet = System.Runtime.InteropServices.CharSet.Unicode, SetLastError = true, ExactSpelling = true,
			            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl,
			            BestFitMapping = false, ThrowOnUnmappableChar = true)]
			        public static extern int Send(string message);
			        public static void Configure([System.Runtime.InteropServices.Optional] int retries,
			            [System.Runtime.InteropServices.Optional, System.Runtime.InteropServices.DefaultParameterValue(3)] int depth,
			            int timeout = 5, [System.Runtime.CompilerServices.CallerMemberName] string caller = "",
			            params int[] values) { }
			    }
			}
			namespace System.Runtime.CompilerServices { internal static class IsExternalInit { } }
			""", [
			MetadataReference.CreateFromFile(typeof(System.Runtime.InteropServices.DefaultParameterValueAttribute).Assembly
				.Location),
		]));

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
