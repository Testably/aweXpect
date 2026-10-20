using System;
using System.IO;
using System.Text;

namespace aweXpect.Snapshots;

/// <summary>
///     Encodes the text of a string or object snapshot into the bytes that a <see cref="SnapshotStore" /> keeps.
/// </summary>
/// <remarks>
///     The text is encoded as UTF-8 without a byte order mark and ends with an additional line break, which is removed
///     again when it is read, so that an editor which adds a final line break does not change the snapshot. A byte order
///     mark that an editor added is recognized when reading.
/// </remarks>
internal static class SnapshotTextEncoding
{
	private static readonly Encoding Utf8WithoutBom = new UTF8Encoding(false);

	public static byte[] Encode(string text) => Utf8WithoutBom.GetBytes(text + Environment.NewLine);

	public static string Decode(byte[] content)
	{
		using StreamReader reader = new(new MemoryStream(content), Encoding.UTF8, true);
		return RemoveFinalLineBreak(reader.ReadToEnd());
	}

	private static string RemoveFinalLineBreak(string text)
	{
		if (text.EndsWith("\r\n", StringComparison.Ordinal))
		{
			return text.Substring(0, text.Length - 2);
		}

		return text.Length > 0 && text[text.Length - 1] == '\n'
			? text.Substring(0, text.Length - 1)
			: text;
	}
}
