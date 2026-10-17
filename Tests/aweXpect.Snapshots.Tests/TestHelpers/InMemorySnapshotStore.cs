using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using aweXpect.Snapshots;

namespace aweXpect.Snapshots.Tests.TestHelpers;

/// <summary>
///     A <see cref="SnapshotStore" /> that keeps the snapshots in memory and records every call.
/// </summary>
/// <remarks>
///     The bytes are kept as the store receives them; <see cref="Snapshots" />, <see cref="Mismatches" /> and
///     <see cref="Accepted" /> show them as the text that the library reads back.
/// </remarks>
internal sealed class InMemorySnapshotStore : SnapshotStore
{
	public InMemorySnapshotStore()
	{
		Accepted = new TextView(AcceptedBytes);
		Mismatches = new TextView(MismatchBytes);
		Snapshots = new TextView(SnapshotBytes);
	}

	public Exception? AcceptException { get; set; }
	public TextView Accepted { get; }
	public Dictionary<SnapshotId, byte[]> AcceptedBytes { get; } = new();
	public bool CanList { get; set; } = true;
	public List<SnapshotId> Deleted { get; } = new();
	public Dictionary<SnapshotId, Exception> DeleteExceptions { get; } = new();
	public Exception? ListException { get; set; }
	public List<SnapshotId> Matches { get; } = new();
	public Dictionary<SnapshotId, byte[]> MismatchBytes { get; } = new();
	public TextView Mismatches { get; }
	public Action? OnRead { get; set; }
	public Exception? ReadException { get; set; }
	public Exception? RecordMatchException { get; set; }
	public Exception? RecordMismatchException { get; set; }
	public Dictionary<SnapshotId, byte[]> SnapshotBytes { get; } = new();
	public TextView Snapshots { get; }

	public override ValueTask AcceptAsync(SnapshotId id, byte[] actual, CancellationToken cancellationToken)
	{
		if (AcceptException is not null)
		{
			throw AcceptException;
		}

		AcceptedBytes[id] = actual;
		SnapshotBytes[id] = actual;
		return default;
	}

	public override ValueTask<byte[]?> ReadAsync(SnapshotId id, CancellationToken cancellationToken)
	{
		OnRead?.Invoke();
		if (ReadException is not null)
		{
			throw ReadException;
		}

		return new ValueTask<byte[]?>(SnapshotBytes.TryGetValue(id, out byte[]? snapshot) ? snapshot : null);
	}

	public override ValueTask RecordMatchAsync(SnapshotId id, CancellationToken cancellationToken)
	{
		if (RecordMatchException is not null)
		{
			throw RecordMatchException;
		}

		Matches.Add(id);
		return default;
	}

	public override ValueTask RecordMismatchAsync(SnapshotId id, byte[] actual, CancellationToken cancellationToken)
	{
		if (RecordMismatchException is not null)
		{
			throw RecordMismatchException;
		}

		MismatchBytes[id] = actual;
		return default;
	}

	public override ValueTask<IReadOnlyList<SnapshotId>?> ListAsync(CancellationToken cancellationToken)
	{
		if (ListException is not null)
		{
			throw ListException;
		}

		return new ValueTask<IReadOnlyList<SnapshotId>?>(CanList
			? SnapshotBytes.Keys.Union(MismatchBytes.Keys).ToArray()
			: null);
	}

	public override ValueTask DeleteAsync(SnapshotId id, CancellationToken cancellationToken)
	{
		if (DeleteExceptions.TryGetValue(id, out Exception? exception))
		{
			throw exception;
		}

		Deleted.Add(id);
		SnapshotBytes.Remove(id);
		MismatchBytes.Remove(id);
		return default;
	}

	/// <summary>
	///     The bytes of the snapshots as text: written as UTF-8, and read like the library reads a text snapshot, without
	///     its additional final line break.
	/// </summary>
	internal sealed class TextView(Dictionary<SnapshotId, byte[]> bytes) : IReadOnlyDictionary<SnapshotId, string>
	{
		public string this[SnapshotId key]
		{
			get => Decode(bytes[key]);
			set => bytes[key] = Encoding.UTF8.GetBytes(value);
		}

		public int Count => bytes.Count;
		public IEnumerable<SnapshotId> Keys => bytes.Keys;
		public IEnumerable<string> Values => bytes.Values.Select(Decode);

		public bool ContainsKey(SnapshotId key) => bytes.ContainsKey(key);

		public IEnumerator<KeyValuePair<SnapshotId, string>> GetEnumerator()
			=> bytes.Select(pair => new KeyValuePair<SnapshotId, string>(pair.Key, Decode(pair.Value)))
				.GetEnumerator();

		public bool TryGetValue(SnapshotId key, out string value)
		{
			bool exists = bytes.TryGetValue(key, out byte[]? content);
			value = exists ? Decode(content!) : "";
			return exists;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		private static string Decode(byte[] content)
		{
			using StreamReader reader = new(new MemoryStream(content), Encoding.UTF8, true);
			string text = reader.ReadToEnd();
			if (text.EndsWith("\r\n", StringComparison.Ordinal))
			{
				return text.Substring(0, text.Length - 2);
			}

			return text.Length > 0 && text[text.Length - 1] == '\n' ? text.Substring(0, text.Length - 1) : text;
		}
	}
}
