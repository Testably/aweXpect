using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
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
public static partial class ThatSnapshot
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
	///     snapshot deletes a leftover pending file. While accepting changes, see <c>.Accepting(…)</c>, the subject
	///     replaces the snapshot instead (with <see cref="SnapshotAcceptance.New" /> only a missing one), and the
	///     expectation still fails. Both only happen when the whole expectation
	///     fails, not when e.g. another branch of an <c>Or</c> is met, and never for a negated snapshot.<br />
	///     The files are encoded as UTF-8 and the pending file ends with an additional line break, which is removed
	///     again when the snapshot is read, so that an editor which adds a final line break does not change it. Another
	///     file extension, e.g. for JSON, is set with <c>.WithExtension(…)</c>.<br />
	///     Another <see cref="SnapshotStore" /> can keep the snapshots elsewhere, see
	///     <c>Customize.aweXpect.Snapshots().Store</c>.<br />
	///     The newline style is ignored, because version control can change it when the snapshot is checked out.<br />
	///     Values that change from run to run can be replaced before comparing, and lines that are irrelevant for the test
	///     left out, see <c>.For</c>, e.g. <c>.For.Guids().Mask()</c> or <c>.For.Lines().Containing("elapsed").Ignore()</c>,
	///     and, for all snapshots, <c>Customize.aweXpect.Snapshots().Rules</c>. Paths of the environment, e.g. of the
	///     test project, are replaced by default, see <c>.For.Paths().Keep()</c>.<br />
	///     A snapshot cannot be negated, so a negated expectation always fails, as does a <see langword="null" />
	///     subject.<br />
	///     A snapshot that was already used for another value in the same test run fails, e.g. for the cases of a
	///     parameterized test, which have to be told apart with <c>.Named(…)</c> or <c>.NamedAfter(…)</c>.
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
		=> AddSnapshot(subject, callerFilePath, callerMemberName, static (value, _) => value!);

	/// <summary>
	///     Verifies that the subject, written by the aweXpect formatter, matches the snapshot
	///     <c>&lt;Class&gt;.&lt;Method&gt;.snapshot.txt</c> next to the test file.
	/// </summary>
	/// <remarks>
	///     The subject is written completely and indented, like the values in a failure message, but without shortening
	///     it, so that a change in a single member shows up as a changed line. Numbers and dates are written
	///     independently of the current culture. A collection is written in the order it enumerates its items, except for
	///     the hash-based collections whose order can change between runs (e.g. a
	///     <see cref="System.Collections.Generic.HashSet{T}" />), whose items are sorted.<br />
	///     A <see cref="string" /> subject is compared as raw text by the other overload; the static type decides, so a
	///     string in an <see cref="object" /> subject is written in quotes like any other value. Binary data, a
	///     <see cref="T:byte[]" />, a <see cref="Stream" /> or a <see cref="FileInfo" />, is compared byte by byte by the
	///     other overloads; a type derived from <see cref="Stream" /> throws an
	///     <see cref="InvalidOperationException" /> here, which only happens before C# 13.<br />
	///     Otherwise it behaves like the overload for strings: see there for the files, accepting changes and the
	///     failures. A text rule applies to the written text, e.g. to a <see cref="Guid" /> member, and member and value
	///     rules, e.g. <c>.For.Members().WithName("Id").Ignore()</c>, while the subject is written.
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
	{
		if (typeof(Stream).IsAssignableFrom(typeof(T)))
		{
			throw Tracing.WriteException(new InvalidOperationException(
				$"A {typeof(T).Name} is compared as binary data, but before C# 13 a type derived from Stream chooses the snapshot of an object. Declare the subject as a Stream, e.g. Expect.That<Stream>(stream), or use C# 13 or later."));
		}

		return AddSnapshot(subject, callerFilePath, callerMemberName,
			static (value, formatValue) => Formatter.Format(value, formatValue is null
				? SnapshotFormatting
				: SnapshotFormatting with
				{
					FormatValue = formatValue,
				}));
	}

	/// <summary>
	///     Verifies that the binary data matches the snapshot <c>&lt;Class&gt;.&lt;Method&gt;.snapshot.&lt;extension&gt;</c>
	///     next to the test file, with the extension from <c>.WithExtension(…)</c>, e.g. <c>png</c>.
	/// </summary>
	/// <remarks>
	///     The data is compared byte by byte and stored unchanged; the failure names the first byte that differs and the
	///     lengths, but never shows the bytes. It has no rules, and the rules for all snapshots are ignored.<br />
	///     Otherwise it behaves like the overload for strings: see there for the files, accepting changes and the
	///     failures.
	/// </remarks>
	/// <param name="subject">The subject.</param>
	/// <param name="callerFilePath">
	///     The path of the test file, which is set by the compiler. It must be rooted, so that a name passed here by
	///     mistake is not taken as the path.
	/// </param>
	/// <param name="callerMemberName">The name of the test method, which is set by the compiler.</param>
	[GuaranteesNotNull]
	public static BinarySnapshotWithoutExtension<byte[]?, IThat<byte[]?>> MatchesSnapshot(
		this IThat<byte[]?> subject,
		[CallerFilePath] string callerFilePath = "",
		[CallerMemberName] string callerMemberName = "")
	{
		string sourceFile = GetSourceFile(callerFilePath);
		return new BinarySnapshotWithoutExtension<byte[]?, IThat<byte[]?>>(extension => AddBinarySnapshot(subject,
			new SnapshotOptions(sourceFile, callerMemberName, extension),
			static (bytes, _) => new ValueTask<byte[]>(bytes!)));
	}

	/// <summary>
	///     Verifies that the content of the stream matches the snapshot
	///     <c>&lt;Class&gt;.&lt;Method&gt;.snapshot.&lt;extension&gt;</c> next to the test file, with the extension from
	///     <c>.WithExtension(…)</c>, e.g. <c>pdf</c>.
	/// </summary>
	/// <remarks>
	///     The stream is read from its current position to the end, so it does not need to be seekable, and is neither
	///     reset nor disposed; a stream that cannot be read fails the expectation.<br />
	///     Otherwise it behaves like the overload for a <see cref="T:byte[]" />.
	/// </remarks>
	/// <param name="subject">The subject.</param>
	/// <param name="callerFilePath">
	///     The path of the test file, which is set by the compiler. It must be rooted, so that a name passed here by
	///     mistake is not taken as the path.
	/// </param>
	/// <param name="callerMemberName">The name of the test method, which is set by the compiler.</param>
	[GuaranteesNotNull]
	[OverloadResolutionPriority(1)]
	public static BinarySnapshotWithoutExtension<Stream?, IThat<Stream?>> MatchesSnapshot(
		this IThat<Stream?> subject,
		[CallerFilePath] string callerFilePath = "",
		[CallerMemberName] string callerMemberName = "")
	{
		string sourceFile = GetSourceFile(callerFilePath);
		return new BinarySnapshotWithoutExtension<Stream?, IThat<Stream?>>(extension => AddBinarySnapshot(subject,
			new SnapshotOptions(sourceFile, callerMemberName, extension),
			static (stream, cancellationToken) => ReadToEndAsync(stream!, cancellationToken)));
	}

	/// <summary>
	///     Verifies that the content of the file matches the snapshot
	///     <c>&lt;Class&gt;.&lt;Method&gt;.snapshot.&lt;extension&gt;</c> next to the test file, with the extension of
	///     the file, e.g. <c>pdf</c>.
	/// </summary>
	/// <remarks>
	///     Another extension can be set with <c>.WithExtension(…)</c>, e.g. for a file without one, which fails the
	///     expectation like a file whose extension a snapshot cannot have. A file that does not exist fails the
	///     expectation both ways, like a <see langword="null" /> subject.<br />
	///     Otherwise it behaves like the overload for a <see cref="T:byte[]" />.
	/// </remarks>
	/// <param name="subject">The subject.</param>
	/// <param name="callerFilePath">
	///     The path of the test file, which is set by the compiler. It must be rooted, so that a name passed here by
	///     mistake is not taken as the path.
	/// </param>
	/// <param name="callerMemberName">The name of the test method, which is set by the compiler.</param>
	[GuaranteesNotNull]
	public static BinarySnapshotResult<FileInfo?, IThat<FileInfo?>> MatchesSnapshot(
		this IThat<FileInfo?> subject,
		[CallerFilePath] string callerFilePath = "",
		[CallerMemberName] string callerMemberName = "")
		=> AddBinarySnapshot(subject, new SnapshotOptions(GetSourceFile(callerFilePath), callerMemberName),
			static (file, _) => new ValueTask<byte[]>(File.ReadAllBytes(file!.FullName)));

	/// <summary>
	///     Stores the snapshot of a <see cref="string" /> with the file <paramref name="extension" /> instead of
	///     <c>txt</c>, e.g. <c>json</c> in <c>&lt;Class&gt;.&lt;Method&gt;.snapshot.json</c>, so that editors and diff
	///     tools highlight it.
	/// </summary>
	/// <param name="result">The snapshot expectation of a string.</param>
	/// <param name="extension">The file extension without a leading dot, e.g. <c>json</c>, <c>html</c> or <c>xml</c>.</param>
	/// <remarks>
	///     The pending file keeps the extension as well, e.g. <c>&lt;Class&gt;.&lt;Method&gt;.snapshot.pending.json</c>.
	///     The extension is kept as given, and a store receives it as <see cref="SnapshotId.Extension" />.<br />
	///     It cannot be empty, contain a <c>.</c>, whitespace, control characters or any of
	///     <c>&lt; &gt; : " / \ | ? * % @</c>, or be <c>pending</c>.<br />
	///     A snapshot that was already used with another extension in the same test run fails.
	/// </remarks>
	public static SnapshotResult<string?, IThat<string?>> WithExtension(
		this SnapshotResult<string?, IThat<string?>> result, string extension)
	{
		SnapshotExtension.ThrowIfInvalid(extension);
		result.Options.Extension = extension;
		return result;
	}

	/// <summary>
	///     Stores the snapshot of a <see cref="FileInfo" /> with the file <paramref name="extension" /> instead of the
	///     extension of the file, e.g. for a file without one.
	/// </summary>
	/// <param name="result">The snapshot expectation of a file.</param>
	/// <param name="extension">The file extension without a leading dot, e.g. <c>png</c>, <c>pdf</c> or <c>bin</c>.</param>
	/// <remarks>
	///     It cannot be empty, contain a <c>.</c>, whitespace, control characters or any of
	///     <c>&lt; &gt; : " / \ | ? * % @</c>, or be <c>pending</c>.
	/// </remarks>
	public static BinarySnapshotResult<FileInfo?, IThat<FileInfo?>> WithExtension(
		this BinarySnapshotResult<FileInfo?, IThat<FileInfo?>> result, string extension)
	{
		SnapshotExtension.ThrowIfInvalid(extension);
		result.Options.Extension = extension;
		return result;
	}

	private static readonly FormattingOptions SnapshotFormatting = FormattingOptions.Indented("") with
	{
		IsSnapshot = true,
	};

	/// <summary>
	///     The extension and the hash code of the value that each snapshot was used for in this process, by its id with the
	///     default extension.
	/// </summary>
	/// <remarks>
	///     Only another value or another extension counts as a conflict, so that a test which is retried or repeated, or
	///     run again by a test runner that keeps its process, still compares with its snapshot. The hash code keeps large
	///     values out of memory; a collision only misses a conflict.
	/// </remarks>
	private static readonly ConcurrentDictionary<SnapshotId, (string Extension, int HashCode)> UsedSnapshots = new();

	/// <summary>
	///     Returns the test file relative to the source root, see <see cref="SnapshotId.SourceFile" />.
	/// </summary>
	/// <param name="callerFilePath">The path of the test file.</param>
	private static string GetSourceFile(string callerFilePath)
	{
		if (!Path.IsPathRooted(callerFilePath))
		{
			throw Tracing.WriteException(new ArgumentException(
				$"The '{nameof(callerFilePath)}' must be the rooted path of the test file, which the compiler sets. Name a snapshot with '.Named(…)' instead.",
				nameof(callerFilePath)));
		}

		return SourceRoots.GetSourceFile(callerFilePath);
	}

	/// <param name="subject">The subject.</param>
	/// <param name="callerFilePath">The path of the test file.</param>
	/// <param name="callerMemberName">The name of the test method.</param>
	/// <param name="format">
	///     Writes the subject as text, with the decision about each nested value from the member and type rules, which
	///     is <see langword="null" /> without such rules and which a <see cref="string" /> subject ignores.
	/// </param>
	private static SnapshotResult<T, IThat<T>> AddSnapshot<T>(IThat<T> subject, string callerFilePath,
		string callerMemberName, Func<T, Func<FormattedValue, ValueFormatting>?, string> format)
	{
		SnapshotOptions options = new(GetSourceFile(callerFilePath), callerMemberName);
		return new SnapshotResult<T, IThat<T>>(subject.Get().ExpectationBuilder.AddConstraint(
				(Options: options, Format: format),
				static (state, it, grammars) => new MatchesSnapshotConstraint<T>(it, grammars, SnapshotSettings.Read(),
					state.Options, state.Format)),
			subject,
			options);
	}

	/// <param name="subject">The subject.</param>
	/// <param name="options">The options of the snapshot.</param>
	/// <param name="read">Reads the bytes of the subject, which is not <see langword="null" />.</param>
	private static BinarySnapshotResult<T, IThat<T>> AddBinarySnapshot<T>(IThat<T> subject, SnapshotOptions options,
		Func<T, CancellationToken, ValueTask<byte[]>> read)
		=> new(subject.Get().ExpectationBuilder.AddConstraint(
				(Options: options, Read: read),
				static (state, it, grammars) => new MatchesBinarySnapshotConstraint<T>(it, grammars,
					SnapshotSettings.Read(), state.Options, state.Read)),
			subject,
			options);

	private static async ValueTask<byte[]> ReadToEndAsync(Stream stream, CancellationToken cancellationToken)
	{
		using MemoryStream content = new();
		await stream.CopyToAsync(content, 81920, cancellationToken);
		return content.ToArray();
	}

	/// <summary>
	///     Compares the subject with its snapshot in the <see cref="SnapshotStore" />, which keeps it as the bytes that
	///     <see cref="CreateContentAsync" /> returns.
	/// </summary>
	private abstract class SnapshotConstraint<T>(
		string it,
		ExpectationGrammars grammars,
		SnapshotSettings settings,
		SnapshotOptions snapshotOptions)
		: ConstraintResult.WithNotNullValue<T>(it, grammars),
			IAsyncContextConstraint<T>
	{
		private string? _acceptError;
		private byte[]? _content;
		private bool _hasSnapshot;
		private bool _hasSubjectError;
		private bool _hasWritten;
		private bool _isAccepted;
		private bool _isMatch;
		private bool _isMismatch;
		private bool _isUsedForAnotherValue;
		private Exception? _readError;
		private string? _usedExtension;
		private string? _writeError;

		/// <inheritdoc />
		public override Exception? FailureCause => SubjectError ?? _readError;

		protected SnapshotSettings Settings => settings;

		protected SnapshotOptions SnapshotOptions => snapshotOptions;

		/// <summary>
		///     The exception that kept the subject from being compared, which is the <see cref="FailureCause" />.
		/// </summary>
		protected abstract Exception? SubjectError { get; }

		private SnapshotId Id => snapshotOptions.Id;

		/// <remarks>
		///     A subject that cannot be compared, or a snapshot that cannot be read, fails the expectation both ways
		///     instead of throwing, so that the failure names the expectation and a negation cannot turn it into a
		///     success; the exception is kept as the <see cref="FailureCause" />.<br />
		///     A mismatch that cannot be recorded or accepted must not hide why the expectation failed, so the error is
		///     reported with the failure, and an error while recording a match is ignored, as the snapshot matched.<br />
		///     A mismatch is only written once the whole expectation is decided, see <see cref="WriteMismatchAsync" />.
		/// </remarks>
		public async ValueTask<ConstraintResult> IsMetBy(T actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			_acceptError = null;
			_content = null;
			_hasSnapshot = false;
			_hasSubjectError = false;
			_hasWritten = false;
			_isAccepted = false;
			_isMatch = false;
			_isMismatch = false;
			_isUsedForAnotherValue = false;
			_usedExtension = null;
			_writeError = null;
			_readError = null;
			if (actual is null)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			byte[]? content = await CreateContentAsync(actual, cancellationToken);
			if (content is null)
			{
				_hasSubjectError = true;
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			_content = content;
			SnapshotId id = Id;
			int contentHashCode = GetHashCode(content);
			(string extension, int hashCode) = UsedSnapshots.GetOrAdd(
				id with { Extension = SnapshotId.DefaultExtension, }, (id.Extension, contentHashCode));
			if (extension != id.Extension)
			{
				_usedExtension = extension;
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			if (hashCode != contentHashCode)
			{
				_isUsedForAnotherValue = true;
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			byte[]? snapshot;
			try
			{
				snapshot = await settings.Store.ReadAsync(id, cancellationToken);
			}
			catch (Exception exception) when (IsNotCanceled(exception, cancellationToken))
			{
				_readError = exception;
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			_hasSnapshot = snapshot is not null;
			if (snapshot is not null && await MatchesAsync(snapshot))
			{
				try
				{
					await settings.Store.RecordMatchAsync(id, cancellationToken);
				}
				catch (Exception exception) when (IsNotCanceled(exception, cancellationToken))
				{
					// The snapshot matched, see the remarks.
				}

				_isMatch = true;
				Outcome = Outcome.Success;
				return this;
			}

			SnapshotAcceptance acceptance = snapshotOptions.Acceptance ?? settings.Acceptance ??
			                                AcceptEnvironment.GetAcceptance();
			_isAccepted = acceptance == SnapshotAcceptance.All ||
			              (acceptance == SnapshotAcceptance.New && !_hasSnapshot);
			_isMismatch = true;
			context.OnCompleted(WriteMismatchAsync);
			Outcome = Outcome.FailureBothWays;
			return this;
		}

		/// <summary>
		///     Returns the bytes that the snapshot of the <paramref name="actual" /> value consists of, or
		///     <see langword="null" /> when it cannot be compared, which fails the expectation both ways and is described
		///     by <see cref="AppendSubjectError" />.
		/// </summary>
		protected abstract ValueTask<byte[]?> CreateContentAsync(T actual, CancellationToken cancellationToken);

		/// <summary>
		///     Compares the content with the bytes of the existing <paramref name="snapshot" />.
		/// </summary>
		protected abstract ValueTask<bool> MatchesAsync(byte[] snapshot);

		/// <summary>
		///     Describes why the subject could not be compared, see <see cref="CreateContentAsync" />.
		/// </summary>
		protected abstract void AppendSubjectError(StringBuilder stringBuilder);

		/// <summary>
		///     Describes how the subject differs from the existing snapshot.
		/// </summary>
		protected abstract void AppendDifference(StringBuilder stringBuilder, string? indentation);

		/// <summary>
		///     Adds the contexts that show the subject and the snapshot when they were compared and differed, or the
		///     snapshot did not exist.
		/// </summary>
		protected virtual void AppendValueContexts(ResultContextCollector contexts)
		{
		}

		protected static bool IsNotCanceled(Exception exception, CancellationToken cancellationToken)
			=> !(exception is OperationCanceledException && cancellationToken.IsCancellationRequested);

		/// <summary>
		///     Accepts the mismatch, or records it for review, once the whole expectation is decided.
		/// </summary>
		/// <remarks>
		///     Nothing is written when the whole expectation does not fail, e.g. because another branch of an <c>Or</c>,
		///     a <c>ThatAny</c> or a quantifier is met, or when the evaluation was canceled, so that a passing test never
		///     changes a snapshot and accepting always fails. A negated snapshot fails, but writes nothing either, as a
		///     snapshot cannot be negated.<br />
		///     The callback runs once per expectation, also when the snapshot was evaluated several times, e.g. by a
		///     repeated check, and only writes when its last evaluation was a mismatch. It runs before the failure message
		///     is built, so that the failure names the written snapshot or the error.<br />
		///     A snapshot that cannot be accepted, e.g. because the file is read-only, is recorded as a mismatch instead,
		///     so that it can still be reviewed and accepted by hand.
		/// </remarks>
		private async ValueTask WriteMismatchAsync(CompletedEvaluation evaluation, CancellationToken cancellationToken)
		{
			if (evaluation.Outcome != Outcome.Failure || IsNegated || !_isMismatch || _content is null)
			{
				return;
			}

			_hasWritten = true;
			SnapshotId id = Id;
			if (_isAccepted)
			{
				try
				{
					await settings.Store.AcceptAsync(id, _content, cancellationToken);
					return;
				}
				catch (Exception exception) when (IsNotCanceled(exception, cancellationToken))
				{
					_acceptError = exception.Message;
				}
			}

			try
			{
				await settings.Store.RecordMismatchAsync(id, _content, cancellationToken);
			}
			catch (Exception exception) when (IsNotCanceled(exception, cancellationToken))
			{
				_writeError = exception.Message;
			}
		}

		/// <summary>
		///     The FNV-1a hash of the <paramref name="content" />, which is the same in every process.
		/// </summary>
		private static int GetHashCode(byte[] content)
		{
			unchecked
			{
				int hashCode = (int)2166136261;
				foreach (byte value in content)
				{
					hashCode = (hashCode ^ value) * 16777619;
				}

				return hashCode;
			}
		}

		public override void AppendContexts(ResultContextCollector contexts)
		{
			if (_content is null || _isMatch || _isUsedForAnotherValue || _usedExtension is not null ||
			    _readError is not null || IsNegated)
			{
				return;
			}

			AppendValueContexts(contexts);
			if (!_hasWritten)
			{
				return;
			}

			if (_isAccepted)
			{
				contexts.Add(new ResultContext.Fixed("Accepted snapshot",
					_acceptError is null ? settings.Store.Describe(Id) : $"could not be written: {_acceptError}"));
				if (_acceptError is null)
				{
					return;
				}
			}

			string? mismatch = _writeError is null
				? settings.Store.DescribeMismatch(Id)
				: $"could not be written: {_writeError}";
			if (mismatch is not null)
			{
				contexts.Add(new ResultContext.Fixed("Pending snapshot", mismatch));
			}
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("matches", "match")).Append(" the snapshot \"")
				.Append(settings.Store.Describe(Id)).Append('"');

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_hasSubjectError)
			{
				AppendSubjectError(stringBuilder);
			}
			else if (_usedExtension is not null)
			{
				stringBuilder.Append("the snapshot was already used with the extension ")
					.Append(Formatter.Format(_usedExtension))
					.Append(" in this test run, so use one extension or name each one with .Named(…) or .NamedAfter(…)");
			}
			else if (_isUsedForAnotherValue)
			{
				stringBuilder.Append(
					"the snapshot was already used for another value in this test run, so name each one with .Named(…) or .NamedAfter(…)");
			}
			else if (_readError is not null)
			{
				stringBuilder.Append("the snapshot could not be read: ").Append(_readError.Message);
			}
			else if (!_hasSnapshot)
			{
				stringBuilder.Append("the snapshot did not exist");
			}
			else
			{
				AppendDifference(stringBuilder, indentation);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("does not match", "do not match")).Append(" the snapshot \"")
				.Append(settings.Store.Describe(Id)).Append('"');

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("a snapshot cannot be negated");
	}

	/// <summary>
	///     Compares the subject as text, written by <paramref name="format" />.
	/// </summary>
	/// <remarks>
	///     The text is stored as encoded by <see cref="SnapshotTextEncoding" />, and the newline style is ignored when
	///     comparing.
	/// </remarks>
	private sealed class MatchesSnapshotConstraint<T>(
		string it,
		ExpectationGrammars grammars,
		SnapshotSettings settings,
		SnapshotOptions snapshotOptions,
		Func<T, Func<FormattedValue, ValueFormatting>?, string> format)
		: SnapshotConstraint<T>(it, grammars, settings, snapshotOptions)
	{
		private readonly StringEqualityOptions _options = new StringEqualityOptions("snapshot").IgnoringNewlineStyle();
		private InsufficientExecutionStackException? _formatError;
		private RegexMatchTimeoutException? _ruleTimeout;
		private string? _snapshot;
		private string? _timedOutRule;
		private string? _value;

		protected override Exception? SubjectError => (Exception?)_formatError ?? _ruleTimeout;

		/// <remarks>
		///     A subject that is nested too deeply to be written fails the expectation; the formatter writes every other
		///     error, e.g. of a member that throws, into the text instead.<br />
		///     The line rules of a <see cref="string" /> subject remove its lines before the masking, so that they don't
		///     use up numbered placeholders. Everything after the masking only sees the masked value, so that the
		///     comparison, the stored snapshot and the failure agree, and two values that only differ in a masked part are
		///     no conflict. A line rule or a masking that times out fails the expectation.
		/// </remarks>
		protected override async ValueTask<byte[]?> CreateContentAsync(T actual, CancellationToken cancellationToken)
		{
			_formatError = null;
			_ruleTimeout = null;
			_snapshot = null;
			_timedOutRule = null;
			_value = null;
			string value;
			string rule = "the member rule with the pattern ";
			try
			{
				SnapshotNumbering numbering = new();
				value = format(actual, SnapshotOptions.CreateFormatValue(Settings.Rules, numbering));
				if (typeof(T) == typeof(string))
				{
					rule = "the line rule with the pattern ";
					value = await SnapshotOptions.ApplyLineRules(value, Settings.Rules, numbering);
				}

				rule = "the masking with the regex ";
				value = SnapshotOptions.Mask(value, Settings.Rules, numbering);
			}
			catch (InsufficientExecutionStackException exception)
			{
				_formatError = exception;
				return null;
			}
			catch (RegexMatchTimeoutException exception)
			{
				_ruleTimeout = exception;
				_timedOutRule = rule;
				return null;
			}

			_value = value;
			return SnapshotTextEncoding.Encode(value);
		}

		protected override async ValueTask<bool> MatchesAsync(byte[] snapshot)
		{
			_snapshot = SnapshotTextEncoding.Decode(snapshot);
			return await _options.AreConsideredEqual(_value, _snapshot);
		}

		protected override void AppendValueContexts(ResultContextCollector contexts)
		{
			contexts.AddStringDifferencesContext(_value, _snapshot, _options, Grammars);
			contexts.AddStringContext("Actual", _value, this);
			contexts.AddStringContext("Snapshot", _snapshot, this);
		}

		protected override void AppendSubjectError(StringBuilder stringBuilder)
		{
			if (_formatError is not null)
			{
				stringBuilder.Append(It).Append(" could not be written: ").Append(_formatError.Message);
			}
			else if (_ruleTimeout is not null)
			{
				stringBuilder.Append(_timedOutRule).Append(Formatter.Format(_ruleTimeout.Pattern))
					.Append(" did not complete within ").Append(Formatter.Format(_ruleTimeout.MatchTimeout))
					.Append(". Simplify the pattern to avoid catastrophic backtracking");
			}
		}

		protected override void AppendDifference(StringBuilder stringBuilder, string? indentation)
			=> stringBuilder.Append(_options.GetExtendedFailure(It, Grammars, _value, _snapshot)
				.Indent(indentation, false));
	}
}
