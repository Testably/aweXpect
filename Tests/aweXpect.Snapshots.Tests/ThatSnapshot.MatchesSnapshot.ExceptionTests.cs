using aweXpect.Customization;
using aweXpect.Snapshots.Tests.TestHelpers;

namespace aweXpect.Snapshots.Tests;

public sealed partial class ThatSnapshot
{
	public sealed partial class MatchesSnapshot
	{
		public sealed class ExceptionTests
		{
			private SnapshotId Id { get; } = new($"/src/{Guid.NewGuid():N}/MyTests.cs", "MyTest");

			[Fact]
			public async Task WhenSubjectIsAFrameworkException_ShouldWriteItsMembersWithoutTheStackTrace()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				ArgumentOutOfRangeException exception = new("playCount", -1, "Play count must be positive.")
				{
					Data =
					{
						["Track"] = "Come Together",
					},
				};

				void Play() => throw exception;

				async Task Act()
					=> await That(Play).Throws<ArgumentOutOfRangeException>()
						.Which.MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo($$"""
				                                             ArgumentOutOfRangeException {
				                                               ActualValue = -1,
				                                               Data = {
				                                                 ["Track"] = "Come Together"
				                                               },
				                                               Message = "{{Escaped(exception.Message)}}",
				                                               ParamName = "playCount"
				                                             }
				                                             """).IgnoringNewlineStyle()
					.Because("the stack trace and the other members that differ between builds, runtimes and machines are left out");
			}

			[Fact]
			public async Task WhenSubjectIsACustomException_ShouldWriteItsOwnMembersAndTheInnerException()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

				ArgumentException innerException = new("The tonearm is up", "tonearm");

				void Eject() => throw new SnapshotPlayerException("The record is stuck", 3, innerException);

				async Task Act()
					=> await That(Eject).Throws<SnapshotPlayerException>()
						.Which.MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo($$"""
				                                             SnapshotPlayerException {
				                                               InnerException = ArgumentException {
				                                                 Message = "{{Escaped(innerException.Message)}}",
				                                                 ParamName = "tonearm"
				                                               },
				                                               Message = "The record is stuck",
				                                               Side = 3
				                                             }
				                                             """).IgnoringNewlineStyle()
					.Because("the inner exception is written member-wise as well");
			}

			[Fact]
			public async Task WhenSubjectIsAnAggregateException_ShouldWriteItsInnerExceptions()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				AggregateException exception = new("The record player failed",
					new InvalidOperationException("The needle is worn"),
					new SnapshotPlayerException("The record is stuck", 3, null));

				void PlayAll() => throw exception;

				async Task Act()
					=> await That(PlayAll).Throws<AggregateException>()
						.Which.MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo($$"""
				                                             AggregateException {
				                                               InnerExceptions = [
				                                                 InvalidOperationException {
				                                                   Message = "The needle is worn"
				                                                 },
				                                                 SnapshotPlayerException {
				                                                   Message = "The record is stuck",
				                                                   Side = 3
				                                                 }
				                                               ],
				                                               Message = "{{Escaped(exception.Message)}}"
				                                             }
				                                             """).IgnoringNewlineStyle()
					.Because("the inner exceptions of an aggregate exception are written instead of its inner exception, which only repeats the first of them");
			}

			[Fact]
			public async Task WhenApplyingMemberRulesToAnException_ShouldLeaveOutAndMaskItsMembers()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

				void Play()
					=> throw new ArgumentOutOfRangeException("playCount", -1, "Play count must be positive.")
					{
						Data =
						{
							["Track"] = "Come Together",
						},
					};

				async Task Act()
					=> await That(Play).Throws<ArgumentOutOfRangeException>()
						.Which.MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().WithName("Message").Ignore()
						.For.Members().WithName("Data").Ignore()
						.For.Members().WithName("ActualValue").Mask();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           ArgumentOutOfRangeException {
				                                             ActualValue = {ActualValue_1},
				                                             ParamName = "playCount"
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("the members of an exception are written like those of any other object, so the member rules apply to them");
			}

			/// <summary>
			///     Returns the <paramref name="message" /> as the snapshot writes it in quotes, as the message of a framework
			///     exception differs between the runtimes, operating systems and cultures.
			/// </summary>
			private static string Escaped(string message)
				=> message.Replace("\r", @"\r").Replace("\n", @"\n");
		}
	}
}

public sealed class SnapshotPlayerException(string message, int side, Exception? innerException)
	: Exception(message, innerException)
{
	public int Side { get; } = side;
}
