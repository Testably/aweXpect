using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using aweXpect.Customization;

namespace aweXpect.Reflection.Tests;

public sealed class CompilerGeneratedFilteringTests
{
	[Fact]
	public async Task Types_ShouldExcludeCompilerGeneratedTypesByDefault()
	{
		IReadOnlyList<Type> types = await GetTypes();

		await That(types).All().Satisfy(t => !t.IsDefined(typeof(CompilerGeneratedAttribute), false));
		await That(types).Contains(t => t == typeof(Sample));
	}

	[Fact]
	public async Task Types_WhenIncludingCompilerGenerated_ShouldContainStateMachinesAndClosures()
	{
		using (Customize.aweXpect.Reflection().IncludedCompilerGeneratedMembers()
			       .Set(CompilerGeneratedMembers.Types))
		{
			IReadOnlyList<Type> types = await GetTypes();

			await That(types).Contains(t => t.Name.StartsWith("<>c")); // closure
			await That(types).Contains(t => t.Name.Contains("AsyncMethod")); // async state machine
		}
	}

	[Fact]
	public async Task Methods_ShouldExcludeLocalFunctionsByDefault()
	{
		IReadOnlyList<MethodInfo> methods = await GetMethods<Sample>();

		await That(methods).None().Satisfy(m => m.Name.Contains("g__Local"));
		await That(methods).Contains(m => m.Name == nameof(Sample.UsesLocalFunction));
	}

	[Fact]
	public async Task Methods_WhenIncludingMethods_ShouldContainLocalFunctions()
	{
		using (Customize.aweXpect.Reflection().IncludedCompilerGeneratedMembers()
			       .Set(CompilerGeneratedMembers.Methods))
		{
			IReadOnlyList<MethodInfo> methods = await GetMethods<Sample>();

			await That(methods).Contains(m => m.Name.Contains("g__Local"));
		}
	}

	[Fact]
	public async Task Methods_ShouldExcludeRecordMembersByDefault()
	{
		IReadOnlyList<MethodInfo> methods = await GetMethods<SampleRecord>();

		await That(methods).None().Satisfy(m => m.Name == "<Clone>$");
		await That(methods).None().Satisfy(m => m.Name == "PrintMembers");
	}

	[Fact]
	public async Task Methods_WhenIncludingOperators_ShouldContainOperatorMethods()
	{
		// Operators are special-name, not compiler-generated => controlled by IncludedSpecialNameMembers.
		IReadOnlyList<MethodInfo> withoutOperators = await GetMethods<SampleRecord>();
		await That(withoutOperators).None().Satisfy(m => m.Name == "op_Equality");

		using (Customize.aweXpect.Reflection().IncludedSpecialNameMembers()
			       .Set(SpecialNameMembers.Operators))
		{
			IReadOnlyList<MethodInfo> withOperators = await GetMethods<SampleRecord>();

			await That(withOperators).Contains(m => m.Name == "op_Equality");
		}
	}

	[Fact]
	public async Task Constructors_WhenIncludingConstructors_ShouldContainRecordCopyConstructor()
	{
		IReadOnlyList<ConstructorInfo> withoutCopy = await GetConstructors<SampleRecord>();
		await That(withoutCopy).None()
			.Satisfy(c => c.GetParameters().Any(p => p.ParameterType == typeof(SampleRecord)));

		using (Customize.aweXpect.Reflection().IncludedCompilerGeneratedMembers()
			       .Set(CompilerGeneratedMembers.Constructors))
		{
			IReadOnlyList<ConstructorInfo> withCopy = await GetConstructors<SampleRecord>();

			await That(withCopy)
				.Contains(c => c.GetParameters().Any(p => p.ParameterType == typeof(SampleRecord)));
		}
	}

	private static async Task<IReadOnlyList<Type>> GetTypes()
	{
		List<Type> types = await ToList(In.AssemblyContaining<Sample>().Types());
		return types
			.Where(t => t.FullName!.Contains(nameof(CompilerGeneratedFilteringTests)))
			.ToList();
	}

	private static async Task<IReadOnlyList<MethodInfo>> GetMethods<T>()
		=> await ToList(In.Type<T>().Methods());

	private static async Task<IReadOnlyList<ConstructorInfo>> GetConstructors<T>()
		=> await ToList(In.Type<T>().Constructors());

#if NET8_0_OR_GREATER
	private static async Task<List<T>> ToList<T>(System.Collections.Generic.IAsyncEnumerable<T> source)
	{
		List<T> result = new();
		await foreach (T item in source)
		{
			result.Add(item);
		}

		return result;
	}
#else
	private static Task<List<T>> ToList<T>(IEnumerable<T> source)
		=> Task.FromResult(source.ToList());
#endif

#pragma warning disable CA1822
#pragma warning disable CS1998
	public class Sample
	{
		public int UsesLocalFunction()
		{
			return Local();
			int Local() => 42;
		}

		public async Task<int> AsyncMethod()
		{
			await Task.Yield();
			return 1;
		}
	}

	public record SampleRecord
	{
		public int Value { get; set; }
		public string Name { get; set; } = "";
	}
}
