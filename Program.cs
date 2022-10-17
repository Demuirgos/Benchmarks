using System;

var (n, m) = (1, 10);
foreach (int i in n..m)
{
    Console.WriteLine(i);
}

public static class RangeExtensions
{
    public static CustomEnumerator GetEnumerator(this Range range)
    {
        return new CustomEnumerator(range);
    }
}

public class CustomEnumerator
{
    private int start;
    private int end;
    private int current = 0;   

    public CustomEnumerator(Range range)
    {
        if(range.Start.IsFromEnd || range.End.IsFromEnd)
        {
            throw new ArgumentException("Range must be from start");
        }

        start = range.Start.Value;
        end = range.End.Value + 1;
    }

    public int Current => current;
    public bool MoveNext()
        => ++current <= end - start;
}
