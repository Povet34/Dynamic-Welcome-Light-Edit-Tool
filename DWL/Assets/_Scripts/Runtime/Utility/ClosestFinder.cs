using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClosestFinder : MonoBehaviour
{
    public static int FindClosestPoint(int target, List<int> numbers)
    {
        if (numbers == null || numbers.Count == 0)
        {
            throw new ArgumentException("The list cannot be empty.", nameof(numbers));
        }

        int closest = numbers.OrderBy(n => Math.Abs(n - target)).First();
        return closest;
    }

    public static int FindClosestTopValue(List<int> topValues, int targetValue)
    {
        int closestValue = topValues[0];
        int minDifference = Mathf.Abs(targetValue - closestValue);

        foreach (int value in topValues)
        {
            int difference = Mathf.Abs(targetValue - value);
            if (difference < minDifference)
            {
                minDifference = difference;
                closestValue = value;
            }
        }

        return closestValue;
    }
}
