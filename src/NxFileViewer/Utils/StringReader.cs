namespace Emignatik.NxFileViewer.Utils;

public class StringReader
{
    private readonly System.IO.StringReader _sr;
    private char _lastReadChar = (char)0;
    private int _position;

    public StringReader(string str)
    {
        _sr = new System.IO.StringReader(str);
        _position = 0;
    }

    public char PrevChar { get; private set; } = (char)0;

    public int Position => _position;

    public bool ReadChar(out char c)
    {
        var ci = _sr.Read();
        if (ci < 0)
        {
            c = char.MinValue;
            return false;
        }

        _position++;
        PrevChar = _lastReadChar;
        c = (char)ci;
        _lastReadChar = c;
        return true;
    }
}