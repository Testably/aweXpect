#if NET8_0_OR_GREATER
using System.Collections.Generic;
#endif
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
		public async Task WhenAnonymousObject_ShouldFormatMembersWithTheirFormatters()
		{
			object value = new
			{
				Text = "foo",
				Type = typeof(long),
			};
			string expectedResult = """{ Text = "foo", Type = long }""";
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.SingleLine);
			Formatter.Format(sb, value, FormattingOptions.SingleLine);

			await That(result).IsEqualTo(expectedResult)
				.Because(
					"the compiler-generated ToString would render the type as System.Int64, where the rest of the message says long");
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

#if NET8_0_OR_GREATER
		[Fact]
		public async Task WhenAsyncIterator_ShouldDisplayTheAsyncEnumerableType()
		{
			static async IAsyncEnumerable<int> Numbers()
			{
				await Task.Yield();
				yield return 1;
			}

			object value = Numbers();
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo("IAsyncEnumerable<int>")
				.Because("the items cannot be listed synchronously and the fields only show the state of the iterator");
			await That(sb.ToString()).IsEqualTo("IAsyncEnumerable<int>");
		}
#endif

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
		public async Task WhenClassHasIndexer_ShouldNotDisplayIt()
		{
			object value = new ClassWithIndexer
			{
				Value = 1,
			};
			string expectedResult = "ValueFormatters.ObjectTests.ClassWithIndexer { Value = 1 }";
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.SingleLine);
			Formatter.Format(sb, value, FormattingOptions.SingleLine);

			await That(result).IsEqualTo(expectedResult)
				.Because("an indexer cannot be read without an index");
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task WhenClassHasPropertyWithNonPublicGetter_ShouldNotDisplayIt()
		{
			object value = new ClassWithNonPublicGetter
			{
				Hidden = 2,
				Value = 1,
			};
			string expectedResult = "ValueFormatters.ObjectTests.ClassWithNonPublicGetter { Value = 1 }";
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.SingleLine);
			Formatter.Format(sb, value, FormattingOptions.SingleLine);

			await That(result).IsEqualTo(expectedResult)
				.Because("only publicly readable members are formatted, like for a registered type");
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task WhenClassHasStaticMembers_ShouldDisplayOnlyInstanceMembers()
		{
			object value = new ClassWithStaticMembers
			{
				Value = 1,
			};
			string expectedResult = "ValueFormatters.ObjectTests.ClassWithStaticMembers { Value = 1 }";
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.SingleLine);
			Formatter.Format(sb, value, FormattingOptions.SingleLine);

			await That(result).IsEqualTo(expectedResult)
				.Because("static members do not describe the formatted instance");
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task WhenClassHasWriteOnlyProperty_ShouldNotDisplayIt()
		{
			object value = new ClassWithWriteOnlyProperty
			{
				Hidden = 2,
				Value = 1,
			};
			string expectedResult = "ValueFormatters.ObjectTests.ClassWithWriteOnlyProperty { Value = 1 }";
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.SingleLine);
			Formatter.Format(sb, value, FormattingOptions.SingleLine);

			await That(result).IsEqualTo(expectedResult)
				.Because("a property without a getter cannot be read, like for a registered type");
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
		public async Task WhenStructHasStaticPropertyOfItsOwnType_ShouldNotFollowIt()
		{
			object value = new StructWithStaticPropertyOfItsOwnType
			{
				Value = 1,
			};
			string expectedResult = "ValueFormatters.ObjectTests.StructWithStaticPropertyOfItsOwnType { Value = 1 }";
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.SingleLine);
			Formatter.Format(sb, value, FormattingOptions.SingleLine);

			await That(result).IsEqualTo(expectedResult)
				.Because("each read boxes a new value, so the recursion guard would never stop following it");
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Fact]
		public async Task WhenTwoMembersAreEqualButNotTheSame_ShouldFormatBoth()
		{
			object value = new
			{
				A = new
				{
					X = 1,
				},
				B = new
				{
					X = 1,
				},
			};
			string expectedResult = "{ A = { X = 1 }, B = { X = 1 } }";
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.SingleLine);
			Formatter.Format(sb, value, FormattingOptions.SingleLine);

			await That(result).IsEqualTo(expectedResult)
				.Because("a recursion is the same instance coming round again, not an equal one");
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

		private sealed class ClassWithIndexer
		{
			// ReSharper disable once UnusedMember.Local
			public int this[int index] => index;

			// ReSharper disable once UnusedAutoPropertyAccessor.Local
			public int Value { get; set; }
		}

		private sealed class ClassWithNonPublicGetter
		{
			// ReSharper disable once UnusedAutoPropertyAccessor.Local
			public int Hidden { private get; set; }

			// ReSharper disable once UnusedAutoPropertyAccessor.Local
			public int Value { get; set; }
		}

		private sealed class ClassWithStaticMembers
		{
			// ReSharper disable once UnusedMember.Local
			public static int StaticField = 3;

			// ReSharper disable once UnusedMember.Local
			public static ClassWithStaticMembers Default { get; } = new();

			// ReSharper disable once UnusedAutoPropertyAccessor.Local
			public int Value { get; set; }
		}

		private sealed class ClassWithToString(string value)
		{
			/// <inheritdoc />
			public override string ToString()
				=> value;
		}

		private sealed class ClassWithWriteOnlyProperty
		{
#pragma warning disable CA1822 // a static property would already be excluded, so it must be an instance member
			// ReSharper disable once ValueParameterNotUsed
			public int Hidden
			{
				set { }
			}
#pragma warning restore CA1822

			// ReSharper disable once UnusedAutoPropertyAccessor.Local
			public int Value { get; set; }
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

		private struct StructWithStaticPropertyOfItsOwnType
		{
			private static int _reads;

			// ReSharper disable once NotAccessedField.Local
			public int Value;

			/// <remarks>
			///     Throws on the second read, so that following it fails the test instead of overflowing the stack.
			/// </remarks>
			// ReSharper disable once UnusedMember.Local
			public static StructWithStaticPropertyOfItsOwnType Default
				=> ++_reads > 1
					? throw new InvalidOperationException("read more than once")
					: new StructWithStaticPropertyOfItsOwnType();
		}

		private sealed class EmptyClass;

		private sealed class InnerDummy
		{
			// ReSharper disable once UnusedAutoPropertyAccessor.Local
			public string? InnerValue { get; set; }
		}
	}
}
