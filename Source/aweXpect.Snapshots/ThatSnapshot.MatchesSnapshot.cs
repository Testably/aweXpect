using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Core.Metadata;
using aweXpect.Customization;
using aweXpect.Formatting;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Snapshots;
using static aweXpect.Formatting.Format;

namespace aweXpect;

/// <summary>
///     Snapshot expectations.
/// </summary>
public static class ThatSnapshot
{
	/// <summary>
	///     Verifies that the subject matches the snapshot <c>&lt;Class&gt;.&lt;Method&gt;.snapshot.txt</c> next to the
	///     test file.
	/// </summary>
	/// <remarks>
	///     <c>&lt;Class&gt;</c> is the name of the test file without its extension and <c>&lt;Method&gt;</c> the name
	///     of the calling member. The file name is known at compile time, so no stack walk or reflection is needed,
	///     which also holds under Native AOT and in async methods, and it is unique within its directory, which a class
	///     name is not across namespaces.<br />
	///     A missing or different snapshot fails the expectation and writes the subject to
	///     <c>&lt;Class&gt;.&lt;Method&gt;.snapshot.pending.txt</c>, which can be renamed to accept it; a matching
	///     snapshot deletes a leftover pending file. While accepting changes, see <c>.AcceptingChanges()</c>, the subject
	///     replaces the snapshot instead, and the expectation still fails.<br />
	///     The files are encoded as UTF-8 and the pending file ends with an additional line break, which is removed
	///     again when the snapshot is read, so that an editor which adds a final line break does not change it.<br />
	///     Another <see cref="SnapshotStore" /> can keep the snapshots elsewhere, see
	///     <c>Customize.aweXpect.Snapshots().Store</c>.<br />
	///     The newline style is ignored, because version control can change it when the snapshot is checked out.<br />
	///     Values that change from run to run can be replaced before comparing, see <c>.Masking</c>, e.g.
	///     <c>.Masking.Guids()</c>, and, for all snapshots, <c>Customize.aweXpect.Snapshots().Maskings</c>.<br />
	///     A snapshot cannot be negated, so a negated expectation always fails, as does a <see langword="null" />
	///     subject.<br />
	///     A snapshot that was already used for another value in the same test run fails, e.g. for the cases of a
	///     parameterized test, which have to be told apart with <c>.Named(…)</c>.
	/// </remarks>
	/// <param name="subject">The subject.</param>
	/// <param name="callerFilePath">
	///     The path of the test file, which is set by the compiler. It must be rooted, so that a name passed here by
	///     mistake is not taken as the path.
	/// </param>
	/// <param name="callerMemberName">The name of the test method, which is set by the compiler.</param>
	[GuaranteesNotNull]
	public static SnapshotResult<string?, IThat<string?>> MatchesSnapshot(
		this IThat<string?> subject,
		[CallerFilePath] string callerFilePath = "",
		[CallerMemberName] string callerMemberName = "")
		=> AddSnapshot(subject, callerFilePath, callerMemberName, static value => value!);

	/// <summary>
	///     Verifies that the subject, written by the aweXpect formatter, matches the snapshot
	///     <c>&lt;Class&gt;.&lt;Method&gt;.snapshot.txt</c> next to the test file.
	/// </summary>
	/// <remarks>
	///     The subject is written completely and indented, like the values in a failure message, but without shortening
	///     it, so that a change in a single member shows up as a changed line. Numbers and dates are written
	///     independently of the current culture. A collection is written in the order it enumerates its items, so an
	///     unordered collection like a <see cref="System.Collections.Generic.HashSet{T}" /> has to be ordered before,
	///     e.g. with <c>OrderBy</c>, as its order can change between runs.<br />
	///     A <see cref="string" /> subject is compared as raw text by the other overload; the static type decides, so a
	///     string in an <see cref="object" /> subject is written in quotes like any other value.<br />
	///     Otherwise it behaves like the overload for strings: see there for the files, accepting changes and the
	///     failures. A masking applies to the written text, e.g. to a <see cref="Guid" /> member.
	/// </remarks>
	/// <param name="subject">The subject.</param>
	/// <param name="callerFilePath">
	///     The path of the test file, which is set by the compiler. It must be rooted, so that a name passed here by
	///     mistake is not taken as the path.
	/// </param>
	/// <param name="callerMemberName">The name of the test method, which is set by the compiler.</param>
	[GuaranteesNotNull]
	public static SnapshotResult<T, IThat<T>> MatchesSnapshot<[RequiresMemberMetadata] T>(
		this IThat<T> subject,
		[CallerFilePath] string callerFilePath = "",
		[CallerMemberName] string callerMemberName = "")
		=> AddSnapshot(subject, callerFilePath, callerMemberName,
			static value => Formatter.Format(value, SnapshotFormatting));

	private static readonly FormattingOptions SnapshotFormatting = FormattingOptions.Indented("") with
	{
		IsComplete = true,
	};

	/// <summary>
	///     The hash code of the value that each snapshot was used for in this process.
	/// </summary>
	/// <remarks>
	///     Only another value counts as a conflict, so that a test which is retried or repeated, or run again by a test
	///     runner that keeps its process, still compares with its snapshot. The hash code keeps large values out of
	///     memory; a collision only misses a conflict.
	/// </remarks>
	private static readonly ConcurrentDictionary<SnapshotId, int> UsedSnapshots = new();

	private static SnapshotResult<T, IThat<T>> AddSnapshot<T>(IThat<T> subject, string callerFilePath,
		string callerMemberName, Func<T, string> format)
	{
		if (!Path.IsPathRooted(callerFilePath))
		{
			throw Tracing.WriteException(new ArgumentException(
				$"The '{nameof(callerFilePath)}' must be the rooted path of the test file, which the compiler sets. Name a snapshot with '.Named(…)' instead.",
				nameof(callerFilePath)));
		}

		SnapshotOptions options = new();
		return new SnapshotResult<T, IThat<T>>(subject.Get().ExpectationBuilder.AddConstraint(
				(SourceFile: SourceRoots.GetSourceFile(callerFilePath), MemberName: callerMemberName, Options: options,
					Format: format),
				static (state, it, grammars) =>
				{
					SnapshotsAwexpectCustomizationExtensions.SnapshotsCustomization customization =
						Customize.aweXpect.Snapshots();
					return new MatchesSnapshotConstraint<T>(it, grammars, customization.Store.Get(),
						customization.AcceptChanges.Get(), customization.Maskings.Get(), state.SourceFile,
						state.MemberName, state.Options, state.Format);
				}),
			subject,
			options);
	}

	private sealed class MatchesSnapshotConstraint<T>(
		string it,
		ExpectationGrammars grammars,
		SnapshotStore store,
		bool? acceptChanges,
		IReadOnlyList<SnapshotMasking> globalMaskings,
		string sourceFile,
		string memberName,
		SnapshotOptions snapshotOptions,
		Func<T, string> format)
		: ConstraintResult.WithNotNullValue<T>(it, grammars),
			IAsyncConstraint<T>
	{
		private readonly StringEqualityOptions _options = new StringEqualityOptions("snapshot").IgnoringNewlineStyle();
		private string? _acceptError;
		private InsufficientExecutionStackException? _formatError;
		private bool _isAccepted;
		private bool _isMatch;
		private bool _isUsedForAnotherValue;
		private RegexMatchTimeoutException? _maskingTimeout;
		private Exception? _readError;
		private string? _snapshot;
		private string? _value;
		private string? _writeError;

		/// <inheritdoc />
		public override Exception? FailureCause => (Exception?)_formatError ?? _maskingTimeout ?? _readError;

		private SnapshotId Id => new(sourceFile, memberName, snapshotOptions.Name);

		/// <remarks>
		///     A subject that is nested too deeply to be written, or a snapshot that cannot be read, fails the expectation
		///     both ways instead of throwing, so that the failure names the expectation and a negation cannot turn it into
		///     a success; the exception is kept as the <see cref="FailureCause" />. The formatter writes every other error,
		///     e.g. of a member that throws, into the text instead.<br />
		///     A mismatch that cannot be recorded or accepted must not hide why the expectation failed, so the error is
		///     reported with the failure, and an error while recording a match is ignored, as the snapshot matched.<br />
		///     An accepted snapshot still fails the expectation, so that accepting by accident never goes unnoticed. A
		///     snapshot that cannot be accepted, e.g. because the file is read-only, is recorded as a mismatch instead, so
		///     that it can still be reviewed and accepted by hand.<br />
		///     Everything after the masking only sees the masked value, so that the comparison, the stored snapshot and
		///     the failure agree, and two values that only differ in a masked part are no conflict. A masking that times
		///     out fails the expectation both ways, with the exception as the <see cref="FailureCause" />.
		/// </remarks>
		public async ValueTask<ConstraintResult> IsMetBy(T actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			_acceptError = null;
			_isAccepted = false;
			_isMatch = false;
			_isUsedForAnotherValue = false;
			_maskingTimeout = null;
			_writeError = null;
			_readError = null;
			_snapshot = null;
			_value = null;
			_formatError = null;
			if (actual is null)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			string value;
			try
			{
				value = snapshotOptions.Mask(format(actual), globalMaskings);
			}
			catch (InsufficientExecutionStackException exception)
			{
				_formatError = exception;
				Outcome = Outcome.FailureBothWays;
				return this;
			}
			catch (RegexMatchTimeoutException exception)
			{
				_maskingTimeout = exception;
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			_value = value;
			SnapshotId id = Id;
			int valueHashCode = StringComparer.Ordinal.GetHashCode(value);
			if (UsedSnapshots.GetOrAdd(id, valueHashCode) != valueHashCode)
			{
				_isUsedForAnotherValue = true;
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			try
			{
				_snapshot = await store.ReadAsync(id, cancellationToken);
			}
			catch (Exception exception) when (IsNotCanceled(exception, cancellationToken))
			{
				_readError = exception;
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			if (_snapshot is not null && await _options.AreConsideredEqual(value, _snapshot))
			{
				try
				{
					await store.RecordMatchAsync(id, cancellationToken);
				}
				catch (Exception exception) when (IsNotCanceled(exception, cancellationToken))
				{
					// The snapshot matched, see the remarks.
				}

				_isMatch = true;
				Outcome = Outcome.Success;
				return this;
			}

			_isAccepted = snapshotOptions.AcceptChanges ?? acceptChanges ?? AcceptEnvironment.IsAccepting();
			if (_isAccepted)
			{
				try
				{
					await store.AcceptAsync(id, value, cancellationToken);
					Outcome = Outcome.FailureBothWays;
					return this;
				}
				catch (Exception exception) when (IsNotCanceled(exception, cancellationToken))
				{
					_acceptError = exception.Message;
				}
			}

			try
			{
				await store.RecordMismatchAsync(id, value, cancellationToken);
			}
			catch (Exception exception) when (IsNotCanceled(exception, cancellationToken))
			{
				_writeError = exception.Message;
			}

			Outcome = Outcome.FailureBothWays;
			return this;
		}

		private static bool IsNotCanceled(Exception exception, CancellationToken cancellationToken)
			=> !(exception is OperationCanceledException && cancellationToken.IsCancellationRequested);

		public override void AppendContexts(ResultContextCollector contexts)
		{
			if (_value is null || _isMatch || _isUsedForAnotherValue || _readError is not null || IsNegated)
			{
				return;
			}

			contexts.AddStringDifferencesContext(_value, _snapshot, _options, Grammars);
			contexts.AddStringContext("Actual", _value, this);
			contexts.AddStringContext("Snapshot", _snapshot, this);
			if (_isAccepted)
			{
				contexts.Add(new ResultContext.Fixed("Accepted snapshot",
					_acceptError is null ? store.Describe(Id) : $"could not be written: {_acceptError}"));
				if (_acceptError is null)
				{
					return;
				}
			}

			string? mismatch = _writeError is null
				? store.DescribeMismatch(Id)
				: $"could not be written: {_writeError}";
			if (mismatch is not null)
			{
				contexts.Add(new ResultContext.Fixed("Pending snapshot", mismatch));
			}
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("matches", "match")).Append(" the snapshot \"")
				.Append(store.Describe(Id)).Append('"');

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_formatError is not null)
			{
				stringBuilder.Append(It).Append(" could not be written: ").Append(_formatError.Message);
			}
			else if (_maskingTimeout is not null)
			{
				stringBuilder.Append("the masking with the regex ").Append(Formatter.Format(_maskingTimeout.Pattern))
					.Append(" did not complete within ").Append(Formatter.Format(_maskingTimeout.MatchTimeout))
					.Append(". Simplify the pattern to avoid catastrophic backtracking");
			}
			else if (_isUsedForAnotherValue)
			{
				stringBuilder.Append(
					"the snapshot was already used for another value in this test run, so name each one with .Named(…)");
			}
			else if (_readError is not null)
			{
				stringBuilder.Append("the snapshot could not be read: ").Append(_readError.Message);
			}
			else if (_snapshot is null)
			{
				stringBuilder.Append("the snapshot did not exist");
			}
			else
			{
				stringBuilder.Append(_options.GetExtendedFailure(It, Grammars, _value, _snapshot)
					.Indent(indentation, false));
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("does not match", "do not match")).Append(" the snapshot \"")
				.Append(store.Describe(Id)).Append('"');

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("a snapshot cannot be negated");
	}
}
