using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Formatting;
using aweXpect.Snapshots;
using static aweXpect.Formatting.Format;

namespace aweXpect;

public static partial class ThatSnapshot
{
	/// <summary>
	///     Compares the bytes of the subject, which <paramref name="read" /> returns, with the snapshot byte by byte.
	/// </summary>
	/// <remarks>
	///     The bytes are stored unchanged and never shown in the failure, which names the first byte that differs and
	///     the lengths instead.<br />
	///     A <see cref="FileInfo" /> subject is stored with the extension of the file, unless one was set, which is only
	///     known when the expectation is evaluated. A file that does not exist fails both ways, like a
	///     <see langword="null" /> subject.
	/// </remarks>
	private sealed class MatchesBinarySnapshotConstraint<T>(
		string it,
		ExpectationGrammars grammars,
		SnapshotSettings settings,
		SnapshotOptions snapshotOptions,
		Func<T, CancellationToken, ValueTask<byte[]>> read)
		: SnapshotConstraint<T>(it, grammars, settings, snapshotOptions)
	{
		private byte[]? _content;
		private int _differenceIndex;
		private string? _invalidExtension;
		private string? _missingFile;
		private Exception? _readError;
		private int _snapshotLength;

		protected override Exception? SubjectError => _readError;

		protected override async ValueTask<byte[]?> CreateContentAsync(T actual,
			CancellationToken cancellationToken)
		{
			_content = null;
			_invalidExtension = null;
			_missingFile = null;
			_readError = null;
			if (actual is FileInfo file && !TryUseFile(file))
			{
				return null;
			}

			try
			{
				_content = await read(actual, cancellationToken);
				return _content;
			}
			catch (Exception exception) when (IsNotCanceled(exception, cancellationToken))
			{
				_readError = exception;
				return null;
			}
		}

		/// <remarks>
		///     The extension is taken from the name before the file is checked, so that the failure for a missing file
		///     names the snapshot that it would be compared with.
		/// </remarks>
		private bool TryUseFile(FileInfo file)
		{
			string extension = Path.GetExtension(file.Name).TrimStart('.');
			bool isValid = SnapshotExtension.IsValid(extension);
			if (isValid)
			{
				SnapshotOptions.UseExtensionOfSubject(extension);
			}

			if (!File.Exists(file.FullName))
			{
				_missingFile = file.FullName;
				return false;
			}

			if (!isValid && !SnapshotOptions.IsExtensionSet)
			{
				_invalidExtension = extension;
				return false;
			}

			return true;
		}

		protected override ValueTask<bool> MatchesAsync(byte[] snapshot)
		{
			byte[] content = _content!;
			int length = Math.Min(content.Length, snapshot.Length);
			int index = 0;
			while (index < length && content[index] == snapshot[index])
			{
				index++;
			}

			_differenceIndex = index;
			_snapshotLength = snapshot.Length;
			return new ValueTask<bool>(index == content.Length && index == snapshot.Length);
		}

		protected override void AppendSubjectError(StringBuilder stringBuilder)
		{
			if (_missingFile is not null)
			{
				stringBuilder.Append("the file \"").Append(_missingFile).Append("\" did not exist");
			}
			else if (_invalidExtension?.Length == 0)
			{
				stringBuilder.Append("the file has no extension, so set one with .WithExtension(…)");
			}
			else if (_invalidExtension is not null)
			{
				stringBuilder.Append("the extension ").Append(Formatter.Format(_invalidExtension))
					.Append(" of the file cannot be used for a snapshot, so set another one with .WithExtension(…)");
			}
			else if (_readError is not null)
			{
				stringBuilder.Append(It).Append(" could not be read: ").Append(_readError.Message);
			}
		}

		protected override void AppendDifference(StringBuilder stringBuilder, string? indentation)
		{
			int length = _content!.Length;
			stringBuilder.Append(It).Append(" differed at byte ")
				.Append(_differenceIndex.ToString(CultureInfo.InvariantCulture))
				.Append(length == _snapshotLength
					? $" (both have {Bytes(length)})"
					: $" (the snapshot has {Bytes(_snapshotLength)}, the subject {length.ToString(CultureInfo.InvariantCulture)})");
		}

		private static string Bytes(int count)
			=> count == 1 ? "1 byte" : $"{count.ToString(CultureInfo.InvariantCulture)} bytes";
	}
}
