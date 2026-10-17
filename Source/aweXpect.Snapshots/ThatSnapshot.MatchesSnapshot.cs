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
	///     replaces the snapshot instead, and the expectation still fails. Both only happen when the whole expectation
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
	///     string in an <see cref="object" /> subject is written in quotes like any other value.<br />
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
		=> AddSnapshot(subject, callerFilePath, callerMemberName,
			static (value, formatValue) => Formatter.Format(value, formatValue is null
				? SnapshotFormatting
				: SnapshotFormatting with
				{
					FormatValue = formatValue,
				}));

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
		if (!Path.IsPathRooted(callerFilePath))
		{
			throw Tracing.WriteException(new ArgumentException(
				$"The '{nameof(callerFilePath)}' must be the rooted path of the test file, which the compiler sets. Name a snapshot with '.Named(…)' instead.",
				nameof(callerFilePath)));
		}

		SnapshotOptions options = new(SourceRoots.GetSourceFile(callerFilePath), callerMemberName);
		return new SnapshotResult<T, IThat<T>>(subject.Get().ExpectationBuilder.AddConstraint(
				(Options: options, Format: format),
				static (state, it, grammars) => new MatchesSnapshotConstraint<T>(it, grammars, SnapshotSettings.Read(),
					state.Options, state.Format)),
			subject,
			options);
	}

	private sealed class MatchesSnapshotConstraint<T>(
		string it,
		ExpectationGrammars grammars,
		SnapshotSettings settings,
		SnapshotOptions snapshotOptions,
		Func<T, Func<FormattedValue, ValueFormatting>?, string> format)
		: ConstraintResult.WithNotNullValue<T>(it, grammars),
			IAsyncContextConstraint<T>
	{
		private readonly StringEqualityOptions _options = new StringEqualityOptions("snapshot").IgnoringNewlineStyle();
		private string? _acceptError;
		private InsufficientExecutionStackException? _formatError;
		private bool _hasWritten;
		private bool _isAccepted;
		private bool _isMatch;
		private bool _isMismatch;
		private bool _isUsedForAnotherValue;
		private Exception? _readError;
		private RegexMatchTimeoutException? _ruleTimeout;
		private string? _snapshot;
		private string? _timedOutRule;
		private string? _usedExtension;
		private string? _value;
		private string? _writeError;

		/// <inheritdoc />
		public override Exception? FailureCause
			=> (Exception?)_formatError ?? _ruleTimeout ?? _readError;

		private SnapshotId Id => snapshotOptions.Id;

		/// <remarks>
		///     A subject that is nested too deeply to be written, or a snapshot that cannot be read, fails the expectation
		///     both ways instead of throwing, so that the failure names the expectation and a negation cannot turn it into
		///     a success; the exception is kept as the <see cref="FailureCause" />. The formatter writes every other error,
		///     e.g. of a member that throws, into the text instead.<br />
		///     A mismatch that cannot be recorded or accepted must not hide why the expectation failed, so the error is
		///     reported with the failure, and an error while recording a match is ignored, as the snapshot matched.<br />
		///     A mismatch is only written once the whole expectation is decided, see <see cref="WriteMismatchAsync" />.<br />
		///     The line rules of a <see cref="string" /> subject remove its lines before the masking, so that they don't use
		///     up numbered placeholders. Everything after the masking only sees the masked value, so that the comparison,
		///     the stored snapshot and the failure agree, and two values that only differ in a masked part are no
		///     conflict. A line rule or a masking that times out fails the expectation both ways, with the exception as the
		///     <see cref="FailureCause" />.
		/// </remarks>
		public async ValueTask<ConstraintResult> IsMetBy(T actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			_acceptError = null;
			_hasWritten = false;
			_isAccepted = false;
			_isMatch = false;
			_isMismatch = false;
			_isUsedForAnotherValue = false;
			_ruleTimeout = null;
			_timedOutRule = null;
			_usedExtension = null;
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
			string rule = "the member rule with the pattern ";
			try
			{
				SnapshotNumbering numbering = new();
				value = format(actual, snapshotOptions.CreateFormatValue(settings.Rules, numbering));
				if (typeof(T) == typeof(string))
				{
					rule = "the line rule with the pattern ";
					value = await snapshotOptions.ApplyLineRules(value, settings.Rules, numbering);
				}

				rule = "the masking with the regex ";
				value = snapshotOptions.Mask(value, settings.Rules, numbering);
			}
			catch (InsufficientExecutionStackException exception)
			{
				_formatError = exception;
				Outcome = Outcome.FailureBothWays;
				return this;
			}
			catch (RegexMatchTimeoutException exception)
			{
				_ruleTimeout = exception;
				_timedOutRule = rule;
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			_value = value;
			SnapshotId id = Id;
			int valueHashCode = StringComparer.Ordinal.GetHashCode(value);
			(string extension, int hashCode) = UsedSnapshots.GetOrAdd(
				id with { Extension = SnapshotId.DefaultExtension, }, (id.Extension, valueHashCode));
			if (extension != id.Extension)
			{
				_usedExtension = extension;
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			if (hashCode != valueHashCode)
			{
				_isUsedForAnotherValue = true;
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			try
			{
				_snapshot = await settings.Store.ReadAsync(id, cancellationToken);
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

			_isAccepted = snapshotOptions.AcceptChanges ?? settings.AcceptChanges ?? AcceptEnvironment.IsAccepting();
			_isMismatch = true;
			context.OnCompleted(WriteMismatchAsync);
			Outcome = Outcome.FailureBothWays;
			return this;
		}

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
			if (evaluation.Outcome != Outcome.Failure || IsNegated || !_isMismatch || _value is null)
			{
				return;
			}

			_hasWritten = true;
			SnapshotId id = Id;
			if (_isAccepted)
			{
				try
				{
					await settings.Store.AcceptAsync(id, _value, cancellationToken);
					return;
				}
				catch (Exception exception) when (IsNotCanceled(exception, cancellationToken))
				{
					_acceptError = exception.Message;
				}
			}

			try
			{
				await settings.Store.RecordMismatchAsync(id, _value, cancellationToken);
			}
			catch (Exception exception) when (IsNotCanceled(exception, cancellationToken))
			{
				_writeError = exception.Message;
			}
		}

		private static bool IsNotCanceled(Exception exception, CancellationToken cancellationToken)
			=> !(exception is OperationCanceledException && cancellationToken.IsCancellationRequested);

		public override void AppendContexts(ResultContextCollector contexts)
		{
			if (_value is null || _isMatch || _isUsedForAnotherValue || _usedExtension is not null ||
			    _readError is not null || IsNegated)
			{
				return;
			}

			contexts.AddStringDifferencesContext(_value, _snapshot, _options, Grammars);
			contexts.AddStringContext("Actual", _value, this);
			contexts.AddStringContext("Snapshot", _snapshot, this);
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
				.Append(settings.Store.Describe(Id)).Append('"');

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("a snapshot cannot be negated");
	}
}
