using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace aweXpect.Core.Tests.Formatting;

public partial class ValueFormatters
{
	public sealed class TypeTests
	{
		[Fact]
		public async Task NestedGenericTypeInGenericTypes_ShouldIncludeTheDeclaringTypeAndName()
		{
			Type value = typeof(NestedGenericType<TypeTests>.InnerClass<int, string>);
			string expectedResult =
				"ValueFormatters.TypeTests.NestedGenericType<ValueFormatters.TypeTests>.InnerClass<int, string>";
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task NestedGenericTypes_ShouldIncludeTheDeclaringTypeAndName()
		{
			Type value = typeof(NestedGenericType<TypeTests>);
			string expectedResult = "ValueFormatters.TypeTests.NestedGenericType<ValueFormatters.TypeTests>";
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task NestedTypeInGenericTypes_ShouldIncludeTheDeclaringTypeAndName()
		{
			Type value = typeof(NestedGenericType<TypeTests>.InnerRegularClass);
			string expectedResult =
				"ValueFormatters.TypeTests.NestedGenericType<ValueFormatters.TypeTests>.InnerRegularClass";
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task NestedTypes_ShouldIncludeTheDeclaringTypeAndName()
		{
			Type value = typeof(TypeTests);
			string expectedResult = $"{nameof(ValueFormatters)}.{nameof(TypeTests)}";
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task ShouldSupportArraySyntax()
		{
			Type value = typeof(int[]);
			string expectedResult = "int[]";
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task ShouldSupportArraySyntaxWithComplexObjects()
		{
			Type value = typeof(TypeTests[]);
			string expectedResult = $"{nameof(ValueFormatters)}.{nameof(TypeTests)}[]";
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task ShouldSupportGenericTypeDefinitions()
		{
			Type value = typeof(IEnumerable<int>);
			string expectedResult = "IEnumerable<int>";
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task ShouldSupportNestedGenericTypeDefinitions()
		{
			Type value = typeof(Expression<Func<TypeTests[], bool>>);
			string expectedResult = $"Expression<Func<{nameof(ValueFormatters)}.{nameof(TypeTests)}[], bool>>";
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Theory]
		[InlineData(typeof(IDictionary<,>), 0, "TKey")]
		[InlineData(typeof(IDictionary<,>), 1, "TValue")]
		[InlineData(typeof(IEnumerable<>), 0, "T")]
		public async Task ShouldSupportOpenGenericParametersOfIDictionary(
			Type genericType, int argumentIndex, string expectedResult)
		{
			Type value = genericType.GetGenericArguments()[argumentIndex];
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task ShouldSupportOpenGenericTypeDefinitions()
		{
			Type value = typeof(IEnumerable<>);
			string expectedResult = "IEnumerable<>";
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Theory]
		[InlineData(typeof(IDictionary<,>), "IDictionary<,>")]
		[InlineData(typeof(Func<,,>), "Func<,,>")]
		public async Task ShouldSupportOpenGenericTypeWithMultipleParametersDefinitions(
			Type value, string expectedResult)
		{
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Theory]
		[MemberData(nameof(SimpleTypes))]
		public async Task SimpleTypes_ShouldUseSimpleNames(Type value, string expectedResult)
		{
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task Types_ShouldOnlyIncludeTheName()
		{
			Type value = typeof(ValueFormatter);
			string expectedResult = nameof(ValueFormatter);
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task WhenGenericParameter_ShouldUseOnlyName()
		{
			MethodInfo method = GetType()
				.GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
				.Single(x => x.Name.StartsWith(nameof(DummyMethodToGetSpecialTypes)));
			string expectedResult = "TParameter";
			Type value = method.GetGenericArguments()[0];
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task WhenNull_ShouldUseDefaultNullString()
		{
			Type? value = null;
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(ValueFormatter.NullString);
			await That(objectResult).IsEqualTo(ValueFormatter.NullString);
			await That(sb.ToString()).IsEqualTo(ValueFormatter.NullString);
		}

		[Fact]
		public async Task WhenNullable_ShouldUseQuestionMarkSyntax()
		{
			string expectedResult = "DateTime?";
			Type value = typeof(DateTime?);
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task WhenVoid_ShouldUseSimpleName()
		{
			MethodInfo method = GetType()
				.GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
				.Single(x => x.Name.StartsWith(nameof(DummyMethodToGetSpecialTypes)));
			string expectedResult = "void";
			Type value = method.ReturnType;
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format((object?)value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(expectedResult);
			await That(objectResult).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		// ReSharper disable once UnusedTypeParameter
		private class NestedGenericType<T>
		{
			public sealed class InnerClass<T1, T2>;

			public sealed class InnerRegularClass;
		}

		// ReSharper disable once UnusedParameter.Local
		private static void DummyMethodToGetSpecialTypes<TParameter>(TParameter value)
		{
			// This method is only used to get a void return type and generic parameter types.
		}

		public static TheoryData<Type, string> SimpleTypes
			=> new()
			{
				{
					typeof(int), "int"
				},
				{
					typeof(int?), "int?"
				},
				{
					typeof(uint), "uint"
				},
				{
					typeof(uint?), "uint?"
				},
				{
					typeof(nint), "nint"
				},
				{
					typeof(nint?), "nint?"
				},
				{
					typeof(nuint), "nuint"
				},
				{
					typeof(nuint?), "nuint?"
				},
				{
					typeof(byte), "byte"
				},
				{
					typeof(byte?), "byte?"
				},
				{
					typeof(sbyte), "sbyte"
				},
				{
					typeof(sbyte?), "sbyte?"
				},
				{
					typeof(short), "short"
				},
				{
					typeof(short?), "short?"
				},
				{
					typeof(ushort), "ushort"
				},
				{
					typeof(ushort?), "ushort?"
				},
				{
					typeof(long), "long"
				},
				{
					typeof(long?), "long?"
				},
				{
					typeof(ulong), "ulong"
				},
				{
					typeof(ulong?), "ulong?"
				},
				{
					typeof(float), "float"
				},
				{
					typeof(float?), "float?"
				},
				{
					typeof(double), "double"
				},
				{
					typeof(double?), "double?"
				},
				{
					typeof(decimal), "decimal"
				},
				{
					typeof(decimal?), "decimal?"
				},
				{
					typeof(string), "string"
				},
				{
					typeof(object), "object"
				},
				{
					typeof(bool), "bool"
				},
				{
					typeof(bool?), "bool?"
				},
				{
					typeof(char), "char"
				},
				{
					typeof(char?), "char?"
				},
				{
					typeof(void), "void"
				},
			};
	}
}
