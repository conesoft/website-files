using System.IO.Compression;

class PositionWrapperStream : Stream
{
    private long pos = 0;

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public override long Position
    {
        get { return pos; }
        set { throw new NotSupportedException(); }
    }

    public override bool CanRead => false;

    public override long Length => pos;

    public override void Write(byte[] buffer, int offset, int count)
    {
        pos += count;
    }

    public override void Flush()
    {
    }

    public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

    public override void SetLength(long value) => throw new NotImplementedException();

    public override int Read(byte[] buffer, int offset, int count) => throw new NotImplementedException();

    static void FakeWriteFile(Stream target, string file, byte[] buffer)
    {
        var count = new FileInfo(file).Length;
        while (count > 0)
        {
            target.Write(buffer, 0, (int)Math.Min(buffer.Length, count));
            count -= buffer.Length;
        }
    }

    static readonly byte[] buffer = new byte[1024 * 1024 * 16];

    static public long? MeasureSizeWhenZipped(string[] files, int maxFiles)
    {
        if(files.Length > maxFiles)
        {
            return null;
        }
        using PositionWrapperStream m = new();
        using (ZipArchive t = new(m, ZipArchiveMode.Create))
        {
            foreach (var file in files)
            {
                using var e = t.CreateEntry(Path.GetFileName(file), CompressionLevel.NoCompression).Open();
                FakeWriteFile(e, file, buffer);
            }
        }
        return m.Position;
    }
}