using System.Collections;

namespace Vertical.JsonClassGenerator;

public class FormattedTextWriter
{
    private readonly Options _options;
    private readonly TextWriter _textWriter;
    private bool _writeSpacingLine;
    private bool _writeIndent = true;

    public FormattedTextWriter(Options options, TextWriter textWriter)
    {
        _options = options;
        _textWriter = textWriter;
    }

    public int Indent { get; set; }
    
    public void Write(string str)
    {
        WriteIndent();
        _textWriter.Write(str);
    }

    public void WriteLine(string str = "")
    {
        Write(str);
        _textWriter.WriteLine();
        _writeIndent = true;
    }

    public void WriteSpacingLine()
    {
        if (!_writeSpacingLine)
        {
            _writeSpacingLine = true;
            return;
        }

        WriteLine();
    }

    public void ResetLineSpacing() => _writeSpacingLine = false;

    public void WriteDocumentationSection(string sectionName, params object[] content)
    {
        WriteLine($"/// <{sectionName}>");
        foreach (var obj in content)
        {
            switch (obj)
            {
                case string str:
                    WriteLine($"/// {str}");
                    break;
                
                case IEnumerable enumerable:
                    foreach (var item in enumerable)
                    {
                        WriteLine($"/// {item}");
                    }
                    break;
                
                default:
                    WriteLine($"/// {obj}");
                    break;
            }
        }
        WriteLine($"/// </{sectionName}>");
    }
    
    private void WriteIndent()
    {
        if (!_writeIndent)
        {
            return;
        }
        
        for (var c = 0; c < Indent; c++)
        {
            WriteIndentChars();
        }

        _writeIndent = false;
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