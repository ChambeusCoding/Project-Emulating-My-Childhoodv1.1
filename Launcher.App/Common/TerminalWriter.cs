using System; //Kept for learning reference
using System.Diagnostics;
using System.IO; //Kept for learning reference
using System.Text;

public class TerminalTextWriter : TextWriter
{
    private readonly Action<string> _appendTerminal;
    private readonly StringBuilder _lineBuilder = new();

    public TerminalTextWriter(Action<string> appendTerminal)
    {
        _appendTerminal = appendTerminal;
    }

    public override Encoding Encoding => Encoding.UTF8;

    public override void Write(char value)
    {
        if (value == '\n' || value == '\r')
        {
            FlushLine();
        }
        else
        {
            _lineBuilder.Append(value);
        }
    }

    public override void Write(string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            foreach (char c in value)
            {
                Write(c);
            }
        }
    }

    public override void WriteLine(string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            _appendTerminal(value);
        }
    }

    private void FlushLine()
    {
        if (_lineBuilder.Length > 0)
        {
            _appendTerminal(_lineBuilder.ToString());
            _lineBuilder.Clear();
        }
    }
}

public class TerminalTraceListener : TraceListener
{
    private readonly Action<string> _appendTerminal;

    public TerminalTraceListener(Action<string> appendTerminal)
    {
        _appendTerminal = appendTerminal;
    }

    public override void WriteLine(string? message)
    {
        _appendTerminal(message ?? "");
    }

    public override void Write(string? message)
    {
        _appendTerminal(message ?? "");
    }
}