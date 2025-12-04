using FsCheck;
using FsCheck.Xunit;

namespace AdvancedConcepts.UnitTests.Advanced;

/// <summary>
/// Property-Based Testing with FsCheck.
/// Silicon Valley best practice: Test properties that should always hold true.
/// </summary>
public class PropertyBasedTests
{
    /// <summary>
    /// Property: Reversing a list twice should give the original list.
    /// </summary>
    [Property]
    public bool ReversingTwice_ShouldGiveOriginal(int[] array)
    {
        var reversed = array.Reverse().ToArray();
        var reversedTwice = reversed.Reverse().ToArray();
        return reversedTwice.SequenceEqual(array);
    }

    /// <summary>
    /// Property: Adding an element and then checking if it contains that element.
    /// </summary>
    [Property]
    public bool AddingElement_ShouldContainElement(List<int> list, int element)
    {
        var newList = new List<int>(list) { element };
        return newList.Contains(element);
    }

    /// <summary>
    /// Property: Concatenating lists preserves total count.
    /// </summary>
    [Property]
    public bool ConcatenatingLists_PreservesCount(int[] list1, int[] list2)
    {
        var combined = list1.Concat(list2).ToArray();
        return combined.Length == list1.Length + list2.Length;
    }

    /// <summary>
    /// Property: Sorting a list should not change its length.
    /// </summary>
    [Property]
    public bool Sorting_PreservesLength(int[] array)
    {
        var sorted = array.OrderBy(x => x).ToArray();
        return sorted.Length == array.Length;
    }

    /// <summary>
    /// Property: Maximum of a list is greater than or equal to all elements.
    /// </summary>
    [Property]
    public bool Maximum_IsGreaterThanAllElements(NonEmptyArray<int> array)
    {
        var max = array.Get.Max();
        return array.Get.All(x => x <= max);
    }

    /// <summary>
    /// Property: String concatenation is associative.
    /// </summary>
    [Property]
    public bool StringConcat_IsAssociative(string a, string b, string c)
    {
        if (a == null || b == null || c == null)
            return true; // Skip null cases

        var left = (a + b) + c;
        var right = a + (b + c);
        return left == right;
    }

    /// <summary>
    /// Property: Absolute value is always non-negative.
    /// </summary>
    [Property]
    public bool AbsoluteValue_AlwaysNonNegative(int number)
    {
        return Math.Abs(number) >= 0;
    }

    /// <summary>
    /// Property: Distinct removes duplicates.
    /// </summary>
    [Property]
    public bool Distinct_RemovesDuplicates(int[] array)
    {
        var distinct = array.Distinct().ToArray();
        return distinct.Length <= array.Length &&
               distinct.All(x => array.Contains(x));
    }

    /// <summary>
    /// Property: Map operation preserves list length.
    /// </summary>
    [Property]
    public bool Map_PreservesLength(int[] array)
    {
        var mapped = array.Select(x => x * 2).ToArray();
        return mapped.Length == array.Length;
    }

    /// <summary>
    /// Property: ToUpper and ToLower are reversible.
    /// </summary>
    [Property]
    public bool CaseConversion_IsReversible(string str)
    {
        if (string.IsNullOrEmpty(str))
            return true;

        var upper = str.ToUpper();
        var lower = upper.ToLower();
        var backToUpper = lower.ToUpper();

        return backToUpper == upper;
    }

    /// <summary>
    /// Property: Division and multiplication are inverse operations (for non-zero divisors).
    /// </summary>
    [Property]
    public bool DivisionMultiplication_AreInverse(int number, PositiveInt divisor)
    {
        if (divisor.Get == 0)
            return true;

        var divided = number / divisor.Get;
        var multiplied = divided * divisor.Get;

        // Due to integer division, we can only check the range
        return Math.Abs(multiplied - number) < divisor.Get;
    }
}
