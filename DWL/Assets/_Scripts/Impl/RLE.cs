using System.Collections.Generic;
using System.Linq;

public class RLE
{
    public int[] RunLengthEncode(int[] inputs)
    {
        List<int> result = new List<int>();

        int current = inputs[0];
        int count = 1;

        for (int i = 1; i < inputs.Length; i++)
        {
            if (inputs[i] == current)
            {
                count++;
            }
            else
            {
                result.Add(current);
                result.Add(count);
                current = inputs[i];
                count = 1;
            }
        }

        // Add the last element
        result.Add(current);
        result.Add(count);

        return result.ToArray();
    }

    public List<int> RunLengthEncode(List<int> inputs)
    {
        return RunLengthEncode(inputs.ToArray()).ToList();
    }
}
