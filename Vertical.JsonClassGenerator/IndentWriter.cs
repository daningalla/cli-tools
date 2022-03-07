namespace Vertical.JsonClassGenerator;

public class IndentWriter
{
    private readonly Options _options;
    private readonly TextWriter _textWriter;

    public IndentWriter(Options options, TextWriter textWriter)
    {
        _options = options;
        _textWriter = textWriter;
    }
    
    public int Indent { get; set; }

    public void Write()
    {
        for (var c = 0; c < Indent; c++)
        {
            WriteIndentChars();
        }
    }

    private void WriteIndentChars()
    {
        if (_options.Indent == 0)
        {
            _textWriter.Write('\t');
            return;
        }

        for (var c = 0; c < _options.Indent; c++)
        {
            _textWriter.Write(' ');
        }
    }
}