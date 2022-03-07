namespace Vertical.JsonClassGenerator;

public class LineWriter
{
    private readonly TextWriter _textWriter;
    private bool _write;

    public LineWriter(TextWriter textWriter) => _textWriter = textWriter;

    public void Write()
    {
        if (!_write)
        {
            _write = true;
            return;
        }

        _textWriter.WriteLine();
    }
}