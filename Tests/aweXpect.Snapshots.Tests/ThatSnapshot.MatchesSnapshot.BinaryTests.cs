using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Customization;
using aweXpect.Results;
using aweXpect.Snapshots.Tests.TestHelpers;

namespace aweXpect.Snapshots.Tests;

public sealed partial class ThatSnapshot
{
	public sealed partial class MatchesSnapshot
	{
		public sealed class BinaryTests
		{
			private static readonly byte[] CoverArt = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A,];

			private SnapshotId Id { get; } = new($"/src/{Guid.NewGuid():N}/MyTests.cs", "MyTest");
			private SnapshotId PngId => Id with { Extension = "png", };

			[Fact]
			public async Task ByteArray_WhenSnapshotMatches_ShouldSucceed()
			{
				InMemorySnapshotStore store = new();
				store.SnapshotBytes[PngId] = [.. CoverArt,];
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				byte[] subject = [.. CoverArt,];

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("png");

				await That(Act).DoesNotThrow();
				await That(store.Matches).IsEqualTo(new[] { PngId, });
			}

			[Fact]
			public async Task ByteArray_WhenSnapshotIsMissing_ShouldFailAndRecordTheBytesUnchanged()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				byte[] subject = [.. CoverArt,];

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("png");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but the snapshot did not exist
					             """)
					.Because("the bytes are never shown in the failure");
				await That(store.MismatchBytes.Keys).IsEqualTo(new[] { PngId, }).InAnyOrder();
				await That(store.MismatchBytes[PngId]).IsEqualTo(CoverArt)
					.Because("binary data is stored as it is, without the text encoding or a final line break");
			}

			[Fact]
			public async Task ByteArray_WhenOneByteDiffers_ShouldNameItsOffsetAndTheLength()
			{
				InMemorySnapshotStore store = new();
				store.SnapshotBytes[PngId] = [1, 2, 3, 4,];
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				byte[] subject = [1, 2, 9, 4,];

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("png");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but it differed at byte 2 (both have 4 bytes)
					             """);
				await That(store.MismatchBytes[PngId]).IsEqualTo(subject);
			}

			[Fact]
			public async Task ByteArray_WhenSubjectIsLonger_ShouldNameTheEndOfTheSnapshot()
			{
				InMemorySnapshotStore store = new();
				store.SnapshotBytes[PngId] = [1, 2, 3,];
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				byte[] subject = [1, 2, 3, 4,];

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("png");

				await That(Act).Throws<XunitException>()
					.WithMessage("*but it differed at byte 3 (the snapshot has 3 bytes, the subject 4)").AsWildcard();
			}

			[Fact]
			public async Task ByteArray_WhenSubjectIsShorter_ShouldNameTheEndOfTheSubject()
			{
				InMemorySnapshotStore store = new();
				store.SnapshotBytes[PngId] = [1, 2, 3, 4,];
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				byte[] subject = [1,];

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("png");

				await That(Act).Throws<XunitException>()
					.WithMessage("*but it differed at byte 1 (the snapshot has 4 bytes, the subject 1)").AsWildcard();
			}

			[Fact]
			public async Task ByteArray_WhenTheSnapshotHasOneByte_ShouldNameItInTheSingular()
			{
				InMemorySnapshotStore store = new();
				store.SnapshotBytes[PngId] = [7,];
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				byte[] subject = [8,];

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("png");

				await That(Act).Throws<XunitException>()
					.WithMessage("*but it differed at byte 0 (both have 1 byte)").AsWildcard();
			}

			[Fact]
			public async Task ByteArray_WhenOnlyTheLineBreaksDiffer_ShouldFail()
			{
				InMemorySnapshotStore store = new();
				store.SnapshotBytes[PngId] = Encoding.UTF8.GetBytes("Let It Be\n");
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				byte[] subject = Encoding.UTF8.GetBytes("Let It Be\r\n");

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("png");

				await That(Act).Throws<XunitException>()
					.WithMessage("*but it differed at byte 9 (the snapshot has 10 bytes, the subject 11)").AsWildcard()
					.Because("binary data gets none of the text handling, so neither the newline style is ignored nor a final line break removed");
			}

			[Fact]
			public async Task ByteArray_WhenEmpty_AndTheSnapshotIsEmpty_ShouldSucceed()
			{
				InMemorySnapshotStore store = new();
				store.SnapshotBytes[PngId] = [];
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				byte[] subject = [];

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("png");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ByteArray_WhenAccepting_ShouldAcceptTheBytesUnchanged()
			{
				InMemorySnapshotStore store = new();
				store.SnapshotBytes[PngId] = [1,];
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				byte[] subject = [.. CoverArt,];

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("png")
						.Accepting(SnapshotAcceptance.All);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but it differed at byte 0 (the snapshot has 1 byte, the subject 8)

					             Accepted snapshot:
					             MyTests.MyTest
					             """);
				await That(store.AcceptedBytes[PngId]).IsEqualTo(CoverArt);
				await That(store.MismatchBytes).IsEmpty();
				await That(Act).DoesNotThrow()
					.Because("the accepted snapshot is read back unchanged");
			}

			[Fact]
			public async Task ByteArray_WhenTheStoreDescribesTheMismatch_ShouldNameThePendingSnapshot()
			{
				DescribingStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				byte[] subject = [.. CoverArt,];

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("png");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but the snapshot did not exist

					             Pending snapshot:
					             MyTests.MyTest.pending.png
					             """);
			}

			[Fact]
			public async Task ByteArray_WhenNull_ShouldFailWithoutAccessingTheStore()
			{
				InMemorySnapshotStore store = new()
				{
					OnRead = () => throw new InvalidOperationException("the store must not be read"),
				};
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				byte[]? subject = null;

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("png");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but it was <null>
					             """);
				await That(store.MismatchBytes).IsEmpty();
			}

			[Fact]
			public async Task ByteArray_WhenNegated_ShouldFail()
			{
				InMemorySnapshotStore store = new();
				store.SnapshotBytes[PngId] = [1,];
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				byte[] subject = [2,];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("png"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not match the snapshot "MyTests.MyTest",
					             but a snapshot cannot be negated
					             """);
			}

			[Fact]
			public async Task ByteArray_WhenNullAndNegated_ShouldFail()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				byte[]? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("png"));

				await That(Act).Throws<XunitException>()
					.WithMessage("*but it was <null>*").AsWildcard();
			}

			[Fact]
			public async Task ByteArray_WhenUsedForAnotherValue_ShouldFail()
			{
				InMemorySnapshotStore store = new();
				store.SnapshotBytes[PngId] = [1,];
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				await That(new byte[] { 1, }).MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("png");
				byte[] subject = [2,];

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("png");

				await That(Act).Throws<XunitException>()
					.WithMessage("*but the snapshot was already used for another value in this test run*")
					.AsWildcard();
			}

			[Fact]
			public async Task ByteArray_WhenUsedAgainForTheSameBytes_ShouldSucceed()
			{
				InMemorySnapshotStore store = new();
				store.SnapshotBytes[PngId] = [.. CoverArt,];
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

				async Task Act()
					=> await That(CoverArt.ToArray()).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.WithExtension("png");

				await That(Act).DoesNotThrow();
				await That(Act).DoesNotThrow()
					.Because("a retried or repeated test compares the same bytes again");
			}

			[Fact]
			public async Task ByteArray_WhenTheSnapshotWasUsedWithAnotherExtension_ShouldFail()
			{
				InMemorySnapshotStore store = new();
				store.SnapshotBytes[PngId] = [1,];
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				await That(new byte[] { 1, }).MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("png");
				byte[] subject = [1,];

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("jpg");

				await That(Act).Throws<XunitException>()
					.WithMessage("*but the snapshot was already used with the extension \"png\" in this test run*")
					.AsWildcard();
			}

			[Fact]
			public async Task ByteArray_ShouldCombineWithTheOtherOptions()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string title = "Abbey Road";
				byte[] subject = [.. CoverArt,];

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.WithExtension("png")
						.NamedAfter(title)
						.UniqueFor.OperatingSystem()
						.Accepting(SnapshotAcceptance.None);

				await That(Act).Throws<XunitException>();
				await That(store.MismatchBytes.Keys).IsEqualTo(new[]
				{
					PngId with
					{
						Name = "title=Abbey Road",
						Variant = CurrentEnvironment.OperatingSystem,
					},
				}).InAnyOrder();
			}

			[Fact]
			public async Task ByteArray_ShouldIgnoreTheRulesForAllSnapshots()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Rules.Set([
					SnapshotRule.For.Text("Abbey").Mask("{album}"),
					SnapshotRule.For.Lines().Containing("Road").Ignore(),
				]);
				byte[] subject = Encoding.UTF8.GetBytes("Abbey Road");

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("txt");

				await That(Act).Throws<XunitException>();
				await That(store.MismatchBytes[Id]).IsEqualTo(subject)
					.Because("rules apply to text, which binary data is not");
			}

			[Fact]
			public async Task ByteArray_ShouldNotBeSnapshottedAsACollection()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "[1, 2]";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				byte[] subject = [1, 2,];

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("txt");

				await That(Act).Throws<XunitException>()
					.Because("a byte[] is binary data, not written by the formatter like other collections");
			}

			[Fact]
			public async Task ByteArray_WithoutExtension_ShouldNotBeAnExpectation()
			{
				MethodInfo getAwaiter = typeof(BinarySnapshotWithoutExtension<byte[]?, IThat<byte[]?>>)
					.GetMethod(nameof(Task.GetAwaiter))!;

				await That(getAwaiter.GetCustomAttribute<ObsoleteAttribute>()).IsNotNull()
					.And.Satisfies(attribute => attribute!.IsError)
					.Because("awaiting a snapshot of binary data without .WithExtension(…) must not compile");
				await That(typeof(ExpectationResult).IsAssignableFrom(
						typeof(BinarySnapshotWithoutExtension<byte[]?, IThat<byte[]?>>))).IsFalse()
					.Because("it cannot be combined into another expectation without the extension either");
			}

			[Fact]
			public async Task ByteArray_ShouldOfferNoRules()
			{
				PropertyInfo? rules = typeof(BinarySnapshotResult<byte[]?, IThat<byte[]?>>).GetProperty("For");

				await That(rules).IsNull()
					.Because("rules apply to text, which binary data is not, so they are rejected at compile time");
			}

			[Theory]
			[InlineData("", "The 'extension' cannot be empty.*")]
			[InlineData(".png", "The 'extension' cannot contain a '.'*")]
			[InlineData("p@g", "The 'extension' cannot contain whitespace*")]
			[InlineData("pending", "The 'extension' cannot be \"pending\"*")]
			public async Task ByteArray_WithInvalidExtension_ShouldThrowArgumentException(string extension,
				string message)
			{
				byte[] subject = [1,];

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension(extension);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("extension").And
					.WithMessage(message).AsWildcard();
			}

			[Fact]
			public async Task ByteArray_WithNullExtension_ShouldThrowArgumentNullException()
			{
				byte[] subject = [1,];

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension(null!);

				await That(Act).Throws<ArgumentNullException>().WithParamName("extension");
			}

			[Fact]
			public async Task ByteArray_WhenCallerFilePathIsNotRooted_ShouldThrowBeforeTheExtension()
			{
				byte[] subject = [1,];

				async Task Act()
					=> await That(subject).MatchesSnapshot("cover").WithExtension("png");

				await That(Act).Throws<ArgumentException>().WithParamName("callerFilePath");
			}

			[Fact]
			public async Task Stream_ShouldReadFromTheCurrentPositionToTheEndWithoutDisposingIt()
			{
				InMemorySnapshotStore store = new();
				store.SnapshotBytes[Id with { Extension = "pdf", }] = [3, 4, 5,];
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using MemoryStream subject = new([1, 2, 3, 4, 5,]);
				subject.Position = 2;

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("pdf");

				await That(Act).DoesNotThrow();
				await That(subject.Position).IsEqualTo(5L)
					.Because("the stream is read to the end, but not reset");
				await That(subject.CanRead).IsTrue()
					.Because("the stream belongs to the test, so it is not disposed");
			}

			[Fact]
			public async Task Stream_WhenNotSeekable_ShouldReadIt()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using Stream subject = new ForwardOnlyStream(CoverArt);

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("pdf");

				await That(Act).Throws<XunitException>()
					.WithMessage("*but the snapshot did not exist").AsWildcard();
				await That(store.MismatchBytes[Id with { Extension = "pdf", }]).IsEqualTo(CoverArt);
			}

			[Fact]
			public async Task Stream_WhenItCannotBeRead_ShouldFailBothWaysWithTheExceptionAsCause()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				MemoryStream subject = new(CoverArt);
				subject.Dispose();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("pdf");

				async Task ActNegated()
					=> await That(subject).DoesNotComplyWith(it
						=> it.MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("pdf"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but it could not be read: *
					             """).AsWildcard()
					.And.WithInner<ObjectDisposedException>();
				await That(ActNegated).Throws<XunitException>()
					.WithMessage("*but a snapshot cannot be negated").AsWildcard()
					.Because("a stream that cannot be read fails both ways");
				await That(store.MismatchBytes).IsEmpty();
			}

			[Fact]
			public async Task Stream_WhenNull_ShouldFail()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				Stream? subject = null;

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("pdf");

				await That(Act).Throws<XunitException>()
					.WithMessage("*but it was <null>").AsWildcard();
			}

			[Fact]
			public async Task Stream_OfADerivedType_ShouldBeComparedAsBinaryData()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using MemoryStream subject = new(CoverArt);

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("pdf");

				await That(Act).Throws<XunitException>();
				await That(store.MismatchBytes[Id with { Extension = "pdf", }]).IsEqualTo(CoverArt)
					.Because("C# 13 chooses the overload for streams over the one for objects");
			}

			[Fact]
			public async Task Stream_OfADerivedType_WhenSnapshottedAsAnObject_ShouldThrowInvalidOperationException()
			{
				using MemoryStream subject = new(CoverArt);

				async Task Act()
					=> await That(subject).MatchesSnapshot<MemoryStream>(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage(
						"A MemoryStream is compared as binary data, but before C# 13 a type derived from Stream chooses the snapshot of an object. Declare the subject as a Stream, e.g. Expect.That<Stream>(stream), or use C# 13 or later.")
					.Because("a compiler before C# 13 ignores the priority of the overload for streams");
			}

			[Fact]
			public async Task FileInfo_ShouldUseTheExtensionOfTheFile()
			{
				using SnapshotDirectory directory = new();
				string path = Path.Combine(directory.Path, "liner-notes.pdf");
				File.WriteAllBytes(path, CoverArt);
				InMemorySnapshotStore store = new();
				store.SnapshotBytes[Id with { Extension = "pdf", }] = [.. CoverArt,];
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				FileInfo subject = new(path);

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).DoesNotThrow();
				await That(store.Matches).IsEqualTo(new[] { Id with { Extension = "pdf", }, });
			}

			[Fact]
			public async Task FileInfo_WhenDifferent_ShouldNameTheOffsetAndRecordTheContent()
			{
				using SnapshotDirectory directory = new();
				string path = Path.Combine(directory.Path, "cover.PNG");
				File.WriteAllBytes(path, CoverArt);
				InMemorySnapshotStore store = new();
				store.SnapshotBytes[Id with { Extension = "PNG", }] = [0x89, 0x50, 0x00,];
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				FileInfo subject = new(path);

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but it differed at byte 2 (the snapshot has 3 bytes, the subject 8)
					             """);
				await That(store.MismatchBytes[Id with { Extension = "PNG", }]).IsEqualTo(CoverArt)
					.Because("the extension of the file is kept as it is, like one from .WithExtension(…)");
			}

			[Fact]
			public async Task FileInfo_WithExtension_ShouldOverrideTheExtensionOfTheFile()
			{
				using SnapshotDirectory directory = new();
				string path = Path.Combine(directory.Path, "LICENSE");
				File.WriteAllBytes(path, CoverArt);
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				FileInfo subject = new(path);

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("bin");

				await That(Act).Throws<XunitException>()
					.WithMessage("*but the snapshot did not exist").AsWildcard();
				await That(store.MismatchBytes.Keys).IsEqualTo(new[] { Id with { Extension = "bin", }, }).InAnyOrder();
			}

			[Fact]
			public async Task FileInfo_WithExtension_ShouldValidateIt()
			{
				FileInfo subject = new("liner-notes.pdf");

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension(".bin");

				await That(Act).Throws<ArgumentException>()
					.WithParamName("extension").And
					.WithMessage("The 'extension' cannot contain a '.'*").AsWildcard();
			}

			[Fact]
			public async Task FileInfo_WhenTheFileHasNoExtension_ShouldFailBothWays()
			{
				using SnapshotDirectory directory = new();
				string path = Path.Combine(directory.Path, "LICENSE");
				File.WriteAllBytes(path, CoverArt);
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				FileInfo subject = new(path);

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				async Task ActNegated()
					=> await That(subject).DoesNotComplyWith(it => it.MatchesSnapshot(Id.SourceFile, Id.MemberName));

				await That(Act).Throws<XunitException>()
					.WithMessage("*but the file has no extension, so set one with .WithExtension(…)").AsWildcard();
				await That(ActNegated).Throws<XunitException>()
					.WithMessage("*but a snapshot cannot be negated").AsWildcard()
					.Because("a file without an extension fails both ways");
				await That(store.MismatchBytes).IsEmpty();
			}

			[Fact]
			public async Task FileInfo_WhenTheExtensionOfTheFileCannotBeUsed_ShouldFail()
			{
				using SnapshotDirectory directory = new();
				string path = Path.Combine(directory.Path, "liner-notes.pending");
				File.WriteAllBytes(path, CoverArt);
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				FileInfo subject = new(path);

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>()
					.WithMessage(
						"*but the extension \"pending\" of the file cannot be used for a snapshot, so set another one with .WithExtension(…)")
					.AsWildcard();
			}

			[Fact]
			public async Task FileInfo_WhenTheFileDoesNotExist_ShouldFailBothWays()
			{
				using SnapshotDirectory directory = new();
				string path = Path.Combine(directory.Path, "liner-notes.pdf");
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				FileInfo subject = new(path);

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				async Task ActNegated()
					=> await That(subject).DoesNotComplyWith(it => it.MatchesSnapshot(Id.SourceFile, Id.MemberName));

				await That(Act).Throws<XunitException>()
					.WithMessage($"*but the file \"{path}\" did not exist").AsWildcard();
				await That(ActNegated).Throws<XunitException>()
					.WithMessage("*but a snapshot cannot be negated").AsWildcard()
					.Because("a missing file fails both ways, like a null subject");
				await That(store.MismatchBytes).IsEmpty();
			}

			[Fact]
			public async Task FileInfo_WhenNull_ShouldFail()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				FileInfo? subject = null;

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>()
					.WithMessage("*but it was <null>").AsWildcard();
			}

			[Fact]
			public async Task FileInfo_WhenAccepting_ShouldAcceptTheContentWithTheExtensionOfTheFile()
			{
				using SnapshotDirectory directory = new();
				string path = Path.Combine(directory.Path, "cover.png");
				File.WriteAllBytes(path, CoverArt);
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				FileInfo subject = new(path);

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Accepting(SnapshotAcceptance.All);

				await That(Act).Throws<XunitException>()
					.WithMessage("*Accepted snapshot:*").AsWildcard();
				await That(store.AcceptedBytes[PngId]).IsEqualTo(CoverArt);
				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task FileInfo_WhenListed_ShouldBeClaimedWithTheExtensionOfTheFile()
			{
				using SnapshotDirectory directory = new();
				string path = Path.Combine(directory.Path, "cover.png");
				File.WriteAllBytes(path, CoverArt);
				InMemorySnapshotStore store = new();
				store.SnapshotBytes[PngId] = [.. CoverArt,];
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				await That(new FileInfo(path)).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				async Task Act() => await OrphanedSnapshots.CheckAsync(true);

				await That(Act).DoesNotThrow()
					.Because("the snapshot with the extension of the file is used");
			}

			[Fact]
			public async Task ByteArray_WhenTheSnapshotWithAnotherExtensionIsListed_ShouldReportItAsOrphaned()
			{
				InMemorySnapshotStore store = new();
				store.SnapshotBytes[PngId] = [1,];
				store.SnapshotBytes[Id with { Extension = "jpg", }] = [1,];
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				await That(new byte[] { 1, }).MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("png");

				async Task Act() => await OrphanedSnapshots.CheckAsync(true);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that every snapshot is used by a test of this run, but 1 snapshot was not used by any test:
					               MyTests.MyTest
					             """)
					.Because("only the snapshot with the extension 'png' is used");
			}

			[Fact]
			public async Task Text_ShouldBeStoredAsUtf8WithAnAdditionalLineBreak()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "äöü\nAbbey Road";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				await That(store.MismatchBytes[Id])
					.IsEqualTo(new UTF8Encoding(false).GetBytes(subject + Environment.NewLine))
					.Because("the library encodes the text on top of the byte-based store, as the disk store wrote it before");
			}

			private sealed class DescribingStore : SnapshotStore
			{
				public override ValueTask<byte[]?> ReadAsync(SnapshotId id,
					System.Threading.CancellationToken cancellationToken)
					=> default;

				public override ValueTask RecordMismatchAsync(SnapshotId id, byte[] actual,
					System.Threading.CancellationToken cancellationToken)
					=> default;

				public override ValueTask AcceptAsync(SnapshotId id, byte[] actual,
					System.Threading.CancellationToken cancellationToken)
					=> default;

				public override string DescribeMismatch(SnapshotId id) => $"{id}.pending.{id.Extension}";
			}

			/// <summary>
			///     A stream that can only be read forward, like a network stream.
			/// </summary>
			private sealed class ForwardOnlyStream(byte[] content) : Stream
			{
				private readonly MemoryStream _content = new(content);

				public override bool CanRead => true;
				public override bool CanSeek => false;
				public override bool CanWrite => false;
				public override long Length => throw new NotSupportedException();

				public override long Position
				{
					get => throw new NotSupportedException();
					set => throw new NotSupportedException();
				}

				public override void Flush()
				{
				}

				public override int Read(byte[] buffer, int offset, int count) => _content.Read(buffer, offset, count);

				public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

				public override void SetLength(long value) => throw new NotSupportedException();

				public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
			}
		}
	}
}
