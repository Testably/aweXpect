using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using aweXpect.Core.Metadata;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Formatting;

public partial class ValueFormatters
{
	public sealed class ObjectTests
	{
		[Test]
		public async Task InFailureMessage_WhenMemberGetterThrows_ShouldEscapeLineBreaksInTheMessage()
		{
			object subject = new ClassWithExceptionProperty(new InvalidOperationException("getter\nfailed"));

			async Task Act()
				=> await That(subject).IsNull();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is null,
				             but it was ValueFormatters.ObjectTests.ClassWithExceptionProperty {
				                 Value = [Value did throw an InvalidOperationException: getter\nfailed]
				               }
				             """)
				.Because("the placeholder must stay on the line of the member");
		}

		[Test]
		public async Task InFailureMessage_WhenMemberGetterThrows_ShouldRenderAPlaceholder()
		{
			object subject = new ClassWithExceptionProperty(new InvalidOperationException("getter failed"));

			async Task Act()
				=> await That(subject).IsNull();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is null,
				             but it was ValueFormatters.ObjectTests.ClassWithExceptionProperty {
				                 Value = [Value did throw an InvalidOperationException: getter failed]
				               }
				             """);
		}

		[Test]
		public async Task InFailureMessage_WhenMemberToStringThrows_ShouldRenderAPlaceholderForTheMember()
		{
			object subject = new ClassWithThrowingToStringMember(
				new ClassWithThrowingToString(new InvalidOperationException("ToString failed")));

			async Task Act()
				=> await That(subject).IsNull();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is null,
				             but it was ValueFormatters.ObjectTests.ClassWithThrowingToStringMember {
				                 Inner = [ToString of ValueFormatters.ObjectTests.ClassWithThrowingToString did throw an InvalidOperationException: ToString failed]
				               }
				             """)
				.Because("the getter of the member succeeded, only formatting its value failed");
		}

		[Test]
		public async Task InFailureMessage_WhenObjectContainsItselfThroughACollection_ShouldDetectTheRecursion()
		{
			Node subject = new();
			subject.Children.Add(subject);

			async Task Act()
				=> await That(subject).IsNull();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is null,
				             but it was ValueFormatters.ObjectTests.Node {
				                 Children = [
				                   ValueFormatters.ObjectTests.Node { *recursive* }
				                 ]
				               }
				             """);
		}

		[Test]
		public async Task InFailureMessage_WhenObjectHasACollectionMember_ShouldIndentTheItemsBelowTheMember()
		{
			ClassWithCollectionMember subject = new()
			{
				Name = "foo",
				Tags = ["a", "b",],
			};

			async Task Act()
				=> await That(subject).IsNull();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is null,
				             but it was ValueFormatters.ObjectTests.ClassWithCollectionMember {
				                 Name = "foo",
				                 Tags = [
				                   "a",
				                   "b"
				                 ]
				               }
				             """);
		}

		[Test]
		public async Task InFailureMessage_WhenToStringThrows_ShouldEscapeLineBreaksInTheMessage()
		{
			object subject = new ClassWithThrowingToString(new InvalidOperationException("ToString\nfailed"));

			async Task Act()
				=> await That(subject).IsNull();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is null,
				             but it was [ToString of ValueFormatters.ObjectTests.ClassWithThrowingToString did throw an InvalidOperationException: ToString\nfailed]
				             """);
		}

		[Test]
		public async Task InFailureMessage_WhenToStringThrows_ShouldRenderAPlaceholder()
		{
			object subject = new ClassWithThrowingToString(new InvalidOperationException("ToString failed"));

			async Task Act()
				=> await That(subject).IsNull();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is null,
				             but it was [ToString of ValueFormatters.ObjectTests.ClassWithThrowingToString did throw an InvalidOperationException: ToString failed]
				             """);
		}

		[Test]
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

		[Test]
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

		[Test]
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

		[Test]
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

		[Test]
		[AutoArguments]
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

		[Test]
		public async Task ShouldUseToStringWhenImplemented_WithIndentation_ShouldIndentTheFollowingLinesLikeMembers()
		{
			ClassWithToString subject = new($"line1{Environment.NewLine}line2");
			string expectedResult = """
			                        line1
			                            line2
			                        """;

			string result = Formatter.Format(subject, FormattingOptions.Indented());

			await That(result).IsEqualTo(expectedResult);
		}

		[Test]
		[AutoArguments]
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

		[Test]
		public async Task ShouldUseToStringWhenImplemented_WithSingleLine_ShouldEscapeLineBreaks()
		{
			ClassWithToString[] subject = [new("line1\r\nline2\0"),];
			string expectedResult = @"[line1\r\nline2\0]";
			StringBuilder sb = new();

			string result = Formatter.Format(subject, FormattingOptions.SingleLine);
			Formatter.Format(sb, subject, FormattingOptions.SingleLine);

			await That(result).IsEqualTo(expectedResult)
				.Because("a single-line rendering must not break the line, like a string in the same mode");
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
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
		[Test]
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

		[Test]
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

		[Test]
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

		[Test]
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

		[Test]
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

		[Test]
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

		[Test]
		public async Task WhenClassHidesAField_ShouldDisplayOnlyTheMostDerivedOne()
		{
			object value = new ClassHidingField();
			string expectedResult = "ValueFormatters.ObjectTests.ClassHidingField { Value = \"foo\" }";
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.SingleLine);
			Formatter.Format(sb, value, FormattingOptions.SingleLine);

			await That(result).IsEqualTo(expectedResult)
				.Because("equivalency only compares the most derived declaration of a hidden field");
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenClassHidesAPropertyByAReadableOne_ShouldDisplayItOnce()
		{
			object value = new ClassHidingPropertyByReadableOne();
			string expectedResult = "ValueFormatters.ObjectTests.ClassHidingPropertyByReadableOne { Value = \"foo\" }";
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.SingleLine);
			Formatter.Format(sb, value, FormattingOptions.SingleLine);

			await That(result).IsEqualTo(expectedResult)
				.Because("equivalency only compares the most derived declaration of a hidden property");
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenClassHidesAPropertyByOneWithoutPublicGetter_ShouldNotDisplayTheBaseProperty()
		{
			object value = new ClassHidingPropertyByOneWithoutPublicGetter
			{
				Own = 2,
			};
			string expectedResult = "ValueFormatters.ObjectTests.ClassHidingPropertyByOneWithoutPublicGetter { Own = 2 }";
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.SingleLine);
			Formatter.Format(sb, value, FormattingOptions.SingleLine);

			await That(result).IsEqualTo(expectedResult)
				.Because("the hiding declaration cannot be read publicly, so equivalency does not compare the property");
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
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

		[Test]
		public async Task WhenClassMemberThrowsException_ShouldDisplayException()
		{
			Exception exception = new("foo");
			object value = new ClassWithExceptionProperty(exception);
			string expectedResult =
				"ValueFormatters.ObjectTests.ClassWithExceptionProperty { Value = [Value did throw an Exception: foo] }";
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.SingleLine);
			Formatter.Format(sb, value, FormattingOptions.SingleLine);

			await That(result).IsEqualTo(expectedResult);
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenClassOverridesOnlyTheSetter_ShouldDisplayTheInheritedGetter()
		{
			object value = new ClassOverridingOnlyTheSetter
			{
				Value = 1,
			};
			string expectedResult = "ValueFormatters.ObjectTests.ClassOverridingOnlyTheSetter { Value = 1 }";
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.SingleLine);
			Formatter.Format(sb, value, FormattingOptions.SingleLine);

			await That(result).IsEqualTo(expectedResult)
				.Because("the override still inherits the getter, so equivalency compares the property");
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenFormattable_ShouldUseTheInvariantCulture()
		{
			using CultureOverride _ = new("de-DE");
			object value = new FormattableClass(1.5);
			StringBuilder sb = new();

			string result = Formatter.Format(value);
			Formatter.Format(sb, value);

			await That(result).IsEqualTo("1.5")
				.Because("a message must not depend on the current culture, even when the type renders itself");
			await That(sb.ToString()).IsEqualTo("1.5");
		}

		[Test]
		public async Task WhenGraphIsDeeperThanTheMaximumDepth_ShouldLeaveOutTheMembersOfTheDeepestObject()
		{
			LinkedNode value = LinkedNode.Chain(1000);
			string expectedResult =
				string.Concat(Enumerable.Repeat("ValueFormatters.ObjectTests.LinkedNode { Next = ", 20)) +
				"ValueFormatters.ObjectTests.LinkedNode { … }" +
				string.Concat(Enumerable.Repeat(" }", 20));
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.SingleLine);
			Formatter.Format(sb, value, FormattingOptions.SingleLine);

			await That(result).IsEqualTo(expectedResult)
				.Because("following a long chain would overflow the stack and end the whole test run");
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenGraphSharesNodesOnEveryLevel_ShouldStopAfterTheMaximumNumberOfWrittenObjects()
		{
			SharingNode value = new();
			for (int i = 0; i < 30; i++)
			{
				value = new SharingNode
				{
					Left = value,
					Right = value,
				};
			}

			string result = Formatter.Format(value, FormattingOptions.SingleLine);

			await That(Count(result, "SharingNode { Left = ")).IsEqualTo(1000)
				.Because("each level doubles the written nodes, so 30 levels would write more than a billion of them");
			await That(Count(result, "SharingNode { … }")).IsEqualTo(1001);

			static int Count(string text, string part)
				=> (text.Length - text.Replace(part, "").Length) / part.Length;
		}

		[Test]
		public async Task WhenMemberIsAStringWithLineBreaks_ShouldEscapeItLikeACollectionItem()
		{
			InnerDummy value = new()
			{
				InnerValue = "a\nb",
			};
			string expectedResult = """
			                        ValueFormatters.ObjectTests.InnerDummy {
			                          InnerValue = "a\nb"
			                        }
			                        """;
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.MultipleLines);
			Formatter.Format(sb, value, FormattingOptions.MultipleLines);

			await That(result).IsEqualTo(expectedResult)
				.Because("the indentation of the object must not be inserted into the string value");
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
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

		[Test]
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

#if NET8_0_OR_GREATER
		[Test]
		public async Task WhenRecordHasOwnRendering_ShouldUseIt()
		{
			string toStringResult = Formatter.Format(new RecordWithToString(1.5));
			string printMembersResult = Formatter.Format(new RecordWithPrintMembers("custom"));

			await That(toStringResult).IsEqualTo("custom")
				.Because("only the compiler-generated rendering of a record is replaced");
			await That(printMembersResult).IsEqualTo("RecordWithPrintMembers { custom }")
				.Because("a user-written PrintMembers is part of the rendering of the record");
		}
#endif

#if NET8_0_OR_GREATER
		[Test]
		public async Task WhenRecordIsCompilerGenerated_ShouldFormatMembersWithTheirFormatters()
		{
			using CultureOverride _ = new("de-DE");
			object value = new MyRecord(1.5, "a b");
			string expectedResult = "ValueFormatters.ObjectTests.MyRecord { S = \"a b\", X = 1.5 }";
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.SingleLine);
			Formatter.Format(sb, value, FormattingOptions.SingleLine);

			await That(result).IsEqualTo(expectedResult)
				.Because("the compiler-generated ToString uses the current culture and does not quote strings");
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}
#endif

#if NET8_0_OR_GREATER
		[Test]
		public async Task WhenRecordStructIsCompilerGenerated_ShouldFormatMembersWithTheirFormatters()
		{
			using CultureOverride _ = new("de-DE");
			object value = new MyRecordStruct(1.5, null);
			string expectedResult = "ValueFormatters.ObjectTests.MyRecordStruct { S = <null>, X = 1.5 }";

			string result = Formatter.Format(value, FormattingOptions.SingleLine);

			await That(result).IsEqualTo(expectedResult)
				.Because("the compiler-generated ToString renders null as an empty text");
		}
#endif

		[Test]
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

		[Test]
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

		[Test]
		public async Task WhenTuple_ShouldFormatItemsPositionally()
		{
			using CultureOverride _ = new("de-DE");
			StringBuilder sb = new();
			Formatter.Format(sb, (1.5, "a"));

			string[] results =
			[
				Formatter.Format((1.5, "a")),
				Formatter.Format(("a\"b", (string?)null)),
				Formatter.Format(Tuple.Create(1.5, "a")),
				Formatter.Format(((1, 2), 3)),
				Formatter.Format(ValueTuple.Create(1)),
				Formatter.Format(ValueTuple.Create()),
				Formatter.Format((1, 2, 3, 4, 5, 6, 7, 8, 9)),
				Formatter.Format(Tuple.Create(1, 2, 3, 4, 5, 6, 7, 8)),
			];

			await That(string.Join(" | ", results)).IsEqualTo(
				"(1.5, \"a\") | (\"a\\\"b\", <null>) | (1.5, \"a\") | ((1, 2), 3) | (1) | () | (1, 2, 3, 4, 5, 6, 7, 8, 9) | (1, 2, 3, 4, 5, 6, 7, 8)"
			).Because("the compiler-generated ToString uses the current culture, does not quote strings and renders null as an empty text");
			await That(sb.ToString()).IsEqualTo("(1.5, \"a\")");
		}

		[Test]
		public async Task WhenTupleIsNestedDeeperThanTheMaximumDepth_ShouldLeaveOutTheItemsOfTheDeepestTuple()
		{
			object value = 1;
			for (int i = 0; i < 1000; i++)
			{
				value = Tuple.Create(1, value);
			}

			string expectedResult =
				string.Concat(Enumerable.Repeat("(1, ", 20)) + "( … )" + new string(')', 20);

			string result = Formatter.Format(value);

			await That(result).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenTwoDeepGraphsAreSiblings_ShouldLeaveOutTheMembersOfBothAtTheSameDepth()
		{
			LinkedNode[] value = [LinkedNode.Chain(30), LinkedNode.Chain(30),];
			string chain =
				string.Concat(Enumerable.Repeat("ValueFormatters.ObjectTests.LinkedNode { Next = ", 19)) +
				"ValueFormatters.ObjectTests.LinkedNode { … }" +
				string.Concat(Enumerable.Repeat(" }", 19));

			string result = Formatter.Format(value, FormattingOptions.SingleLine);

			await That(result).IsEqualTo($"[{chain}, {chain}]")
				.Because("the collection counts as one level, and the first chain must not use up the depth of the second");
		}

		[Test]
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

		[Test]
		public async Task WhenTwoMembersAreTheSameInstance_ShouldFormatBoth()
		{
			InnerDummy inner = new()
			{
				InnerValue = "foo",
			};
			object value = new
			{
				A = inner,
				B = inner,
			};
			string expectedResult =
				"{ A = ValueFormatters.ObjectTests.InnerDummy { InnerValue = \"foo\" }, B = ValueFormatters.ObjectTests.InnerDummy { InnerValue = \"foo\" } }";
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.SingleLine);
			Formatter.Format(sb, value, FormattingOptions.SingleLine);

			await That(result).IsEqualTo(expectedResult)
				.Because("an instance is only a recursion within its own members, not next to itself");
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
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

		[Test]
		public async Task WithType_WhenToStringIsImplemented_ShouldIncludeTheType()
		{
			object value = new ClassWithToString("foo");
			string expectedResult = "ValueFormatters.ObjectTests.ClassWithToString foo";
			StringBuilder sb = new();

			string result = Formatter.Format(value, FormattingOptions.WithType);
			Formatter.Format(sb, value, FormattingOptions.WithType);

			await That(result).IsEqualTo(expectedResult)
				.Because("the own rendering of a type does not name it, unlike its members would");
			await That(sb.ToString()).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WithType_WhenTuple_ShouldIncludeTheType()
		{
			object value = (1, "a");
			string expectedResult = "ValueTuple<int, string> (1, \"a\")";

			string result = Formatter.Format(value, FormattingOptions.WithType);

			await That(result).IsEqualTo(expectedResult);
		}

		private class BaseWithField
		{
			// ReSharper disable once NotAccessedField.Local
			public int Value = 1;
		}

		private class BaseWithProperty
		{
			// ReSharper disable once UnusedAutoPropertyAccessor.Local
			public int Value { get; set; } = 1;
		}

		private class BaseWithVirtualProperty
		{
			public virtual int Value { get; set; }
		}

		private sealed class ClassHidingField : BaseWithField
		{
			// ReSharper disable once NotAccessedField.Local
			public new string Value = "foo";
		}

		private sealed class ClassHidingPropertyByOneWithoutPublicGetter : BaseWithProperty
		{
			// ReSharper disable once UnusedAutoPropertyAccessor.Local
			public new string Value { private get; set; } = "";

			// ReSharper disable once UnusedAutoPropertyAccessor.Local
			public int Own { get; set; }
		}

		private sealed class ClassHidingPropertyByReadableOne : BaseWithProperty
		{
			// ReSharper disable once UnusedMember.Local
			public new string Value { get; } = "foo";
		}

		private sealed class ClassOverridingOnlyTheSetter : BaseWithVirtualProperty
		{
			public override int Value
			{
				set => base.Value = value;
			}
		}

		private sealed class ClassWithCollectionMember
		{
			// ReSharper disable once UnusedAutoPropertyAccessor.Local
			public string Name { get; set; } = "";

			// ReSharper disable once UnusedAutoPropertyAccessor.Local
			public List<string> Tags { get; set; } = [];
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

		private sealed class ClassWithThrowingToString(Exception exception)
		{
			/// <inheritdoc />
			public override string ToString()
				=> throw exception;
		}

		private sealed class ClassWithThrowingToStringMember(ClassWithThrowingToString inner)
		{
			// ReSharper disable once UnusedMember.Local
			public ClassWithThrowingToString Inner => inner;
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

		private sealed class FormattableClass(double value) : IFormattable
		{
			public string ToString(string? format, IFormatProvider? formatProvider)
				=> value.ToString(format, formatProvider);

			/// <inheritdoc />
			public override string ToString()
				=> ToString(null, CultureInfo.CurrentCulture);
		}

		private sealed class RegisteredDummy
		{
			public int NotRegistered { get; set; }
			public int Registered { get; set; }
		}

		private sealed class LinkedNode
		{
			// ReSharper disable once UnusedAutoPropertyAccessor.Local
			public LinkedNode? Next { get; set; }

			public static LinkedNode Chain(int length)
			{
				LinkedNode root = new();
				LinkedNode current = root;
				for (int i = 1; i < length; i++)
				{
					current.Next = new LinkedNode();
					current = current.Next;
				}

				return root;
			}
		}

		/// <remarks>
		///     Throws once its children were read too often, so that following a cycle fails the test instead of
		///     overflowing the stack.
		/// </remarks>
		private sealed class Node
		{
			private readonly List<Node> _children = [];
			private int _reads;

			public List<Node> Children
				=> ++_reads > 100 ? throw new InvalidOperationException("read too often") : _children;
		}

		private sealed class RecursiveDummy
		{
			// ReSharper disable once UnusedAutoPropertyAccessor.Local
			public RecursiveDummy? Inner { get; set; }

			// ReSharper disable once UnusedAutoPropertyAccessor.Local
			public int Value { get; set; }
		}

		private sealed class SharingNode
		{
			// ReSharper disable once UnusedAutoPropertyAccessor.Local
			public SharingNode? Left { get; set; }

			// ReSharper disable once UnusedAutoPropertyAccessor.Local
			public SharingNode? Right { get; set; }
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

#if NET8_0_OR_GREATER
		private sealed record MyRecord(double X, string S);

		private record struct MyRecordStruct(double X, string? S);

		private sealed record RecordWithPrintMembers(string Text)
		{
			private bool PrintMembers(StringBuilder builder)
			{
				builder.Append(Text);
				return true;
			}
		}

		private sealed record RecordWithToString(double X)
		{
			/// <inheritdoc />
			public override string ToString()
				=> "custom";
		}
#endif
	}
}
