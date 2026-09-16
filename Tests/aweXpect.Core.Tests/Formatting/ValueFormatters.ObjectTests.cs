using System.Text;
using aweXpect.Core.Metadata;

namespace aweXpect.Core.Tests.Formatting;

public partial class ValueFormatters
{
	public sealed class ObjectTests
	{
		[Fact]
		public async Task ShouldDisplayNestedObjects()
		{
			Dummy value = new()
			{
				Inner = new InnerDummy
				{
					InnerValue = "foo",
				},
				Value = 2,
			};
			string expectedResult = """
			                        ValueFormatters.ObjectTests.Dummy { Inner = ValueFormatters.ObjectTests.InnerDummy { InnerValue = "foo" }, Value = 2 }
			                        """;
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.SingleLine);
			Formatter.Format(sb, value, FormattingOptions.SingleLine);

			await That(result).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task ShouldDisplayRecursiveObjects()
		{
			RecursiveDummy value = new()
			{
				Value = 1,
			};
			value.Inner = value;
			string expectedResult = """
			                        ValueFormatters.ObjectTests.RecursiveDummy { Inner = ValueFormatters.ObjectTests.RecursiveDummy { *recursive* }, Value = 1 }
			                        """;
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.SingleLine);
			Formatter.Format(sb, value, FormattingOptions.SingleLine);

			await That(result).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task ShouldSupportIndentation()
		{
			Dummy value = new()
			{
				Inner = new InnerDummy
				{
					InnerValue = "foo",
				},
				Value = 2,
			};
			string expectedResult = """
			                        ValueFormatters.ObjectTests.Dummy {
			                            Inner = ValueFormatters.ObjectTests.InnerDummy {
			                              InnerValue = "foo"
			                            },
			                            Value = 2
			                          }
			                        """;
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.Indented());
			Formatter.Format(sb, value, FormattingOptions.Indented());

			await That(result).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task ShouldUseMultipleLinesPerDefault()
		{
			Dummy value = new()
			{
				Inner = new InnerDummy
				{
					InnerValue = "foo",
				},
				Value = 2,
			};
			string expectedResult = """
			                        ValueFormatters.ObjectTests.Dummy {
			                          Inner = ValueFormatters.ObjectTests.InnerDummy {
			                            InnerValue = "foo"
			                          },
			                          Value = 2
			                        }
			                        """;
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.MultipleLines);
			Formatter.Format(sb, value, FormattingOptions.MultipleLines);

			await That(result).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Theory]
		[AutoData]
		public async Task ShouldUseToStringWhenImplemented_Default(string[] values)
		{
			string value = string.Join(Environment.NewLine, values);
			string expectedResult = string.Join($"{Environment.NewLine}  ", values);
			ClassWithToString subject = new(value);
			StringBuilder sb = new();

			string result = Formatter.Format(subject, FormattingOptions.MultipleLines);
			Formatter.Format(sb, subject, FormattingOptions.MultipleLines);

			await That(result).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Theory]
		[AutoData]
		public async Task ShouldUseToStringWhenImplemented_WithSingleLine(string value)
		{
			ClassWithToString subject = new(value);
			string expectedResult = value;
			StringBuilder sb = new();

			string result = Formatter.Format(subject, FormattingOptions.SingleLine);
			Formatter.Format(sb, subject, FormattingOptions.SingleLine);

			await That(result).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task WhenClassContainsField_ShouldDisplayFieldValue()
		{
			object value = new ClassWithField
			{
				Value = 42,
			};
			string expectedResult = "ValueFormatters.ObjectTests.ClassWithField { Value = 42 }";
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.SingleLine);
			Formatter.Format(sb, value, FormattingOptions.SingleLine);

			await That(result).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task WhenClassIsEmpty_ShouldDisplayClassName()
		{
			object value = new EmptyClass();
			string expectedResult = "ValueFormatters.ObjectTests.EmptyClass { }";
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.SingleLine);
			Formatter.Format(sb, value, FormattingOptions.SingleLine);

			await That(result).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task WhenClassMemberThrowsException_ShouldDisplayException()
		{
			Exception exception = new("foo");
			object value = new ClassWithExceptionProperty(exception);
			string expectedResult =
				"ValueFormatters.ObjectTests.ClassWithExceptionProperty { Value = [Member 'Value' threw an exception: 'foo'] }";
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.SingleLine);
			Formatter.Format(sb, value, FormattingOptions.SingleLine);

			await That(result).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task WhenNull_ShouldUseDefaultNullString()
		{
			object? value = null;
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			string objectResult = Formatter.Format(value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo(ValueFormatter.NullString);
			await That(objectResult).IsEqualTo(ValueFormatter.NullString);
			await That(sb.ToString()).IsEqualTo(ValueFormatter.NullString);
		}

		[Fact]
		public async Task WhenObject_ShouldDisplayHashCode()
		{
			object value = new();
			string expectedResult = "System.Object (HashCode=*)";
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.SingleLine);
			Formatter.Format(sb, value, FormattingOptions.SingleLine);

			await That(result).IsEqualTo(expectedResult).AsWildcard();
			await That(sb.ToString()).IsEqualTo(expectedResult).AsWildcard();
		}

		[Fact]
		public async Task WhenRegistered_ShouldDisplayOnlyTheRegisteredMembers()
		{
			TypeMetadataRegistry.RegisterProperty<RegisteredDummy, int>(nameof(RegisteredDummy.Registered),
				x => x.Registered);
			object value = new RegisteredDummy
			{
				Registered = 1,
				NotRegistered = 2,
			};
			string expectedResult = "ValueFormatters.ObjectTests.RegisteredDummy { Registered = 1 }";
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.SingleLine);
			Formatter.Format(sb, value, FormattingOptions.SingleLine);

			await That(result).IsEqualTo(expectedResult)
				.Because("a registered type is formatted through its registration instead of by reflection");
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task WithType_ShouldDisplayClassNameOnlyOnce()
		{
			object value = new EmptyClass();
			string expectedResult = "ValueFormatters.ObjectTests.EmptyClass { }";
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.WithType);
			Formatter.Format(sb, value, FormattingOptions.WithType);

			await That(result).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		private sealed class ClassWithExceptionProperty(Exception exception)
		{
			// ReSharper disable once UnusedMember.Local
			public int Value => throw exception;
		}

		private sealed class ClassWithField
		{
			// ReSharper disable once NotAccessedField.Local
			public int Value = 2;
		}

		private sealed class ClassWithToString(string value)
		{
			/// <inheritdoc />
			public override string ToString()
				=> value;
		}

		private sealed class Dummy
		{
			// ReSharper disable once UnusedAutoPropertyAccessor.Local
			public InnerDummy? Inner { get; set; }

			// ReSharper disable once UnusedAutoPropertyAccessor.Local
			public int Value { get; set; }
		}

		private sealed class RegisteredDummy
		{
			public int NotRegistered { get; set; }
			public int Registered { get; set; }
		}

		private sealed class RecursiveDummy
		{
			// ReSharper disable once UnusedAutoPropertyAccessor.Local
			public RecursiveDummy? Inner { get; set; }

			// ReSharper disable once UnusedAutoPropertyAccessor.Local
			public int Value { get; set; }
		}

		private sealed class EmptyClass;

		private sealed class InnerDummy
		{
			// ReSharper disable once UnusedAutoPropertyAccessor.Local
			public string? InnerValue { get; set; }
		}
	}
}
