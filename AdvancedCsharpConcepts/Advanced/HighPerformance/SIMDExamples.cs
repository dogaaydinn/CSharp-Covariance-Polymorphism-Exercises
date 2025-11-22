using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;

namespace AdvancedCsharpConcepts.Advanced.HighPerformance;

/// <summary>
/// SIMD (Single Instruction, Multiple Data) Examples using Vector&lt;T&gt;.
/// NVIDIA GPU Developer best practice: Leverage hardware vectorization for parallel data processing.
/// </summary>
/// <remarks>
/// SIMD allows CPU to perform the same operation on multiple data points simultaneously.
/// Vector&lt;T&gt; automatically uses hardware SIMD instructions (SSE, AVX, AVX2, AVX-512).
/// Typical speedups: 2x-8x depending on vector width and CPU capabilities.
/// </remarks>
public static class SIMDExamples
{
    /// <summary>
    /// Adds two arrays element-wise using SIMD vectorization.
    /// Performance: ~4-8x faster than scalar loop on modern CPUs.
    /// </summary>
    public static float[] VectorAdd(float[] left, float[] right)
    {
        if (left == null) throw new ArgumentNullException(nameof(left));
        if (right == null) throw new ArgumentNullException(nameof(right));
        if (left.Length != right.Length)
            throw new ArgumentException("Arrays must have the same length");

        var result = new float[left.Length];
        var vectorSize = Vector<float>.Count;
        var i = 0;

        // Process data in SIMD chunks
        for (; i <= left.Length - vectorSize; i += vectorSize)
        {
            var v1 = new Vector<float>(left, i);
            var v2 = new Vector<float>(right, i);
            var sum = v1 + v2;
            sum.CopyTo(result, i);
        }

        // Handle remaining elements (scalar processing)
        for (; i < left.Length; i++)
        {
            result[i] = left[i] + right[i];
        }

        return result;
    }

    /// <summary>
    /// Scalar (non-SIMD) array addition for comparison.
    /// </summary>
    public static float[] ScalarAdd(float[] left, float[] right)
    {
        if (left == null) throw new ArgumentNullException(nameof(left));
        if (right == null) throw new ArgumentNullException(nameof(right));
        if (left.Length != right.Length)
            throw new ArgumentException("Arrays must have the same length");

        var result = new float[left.Length];
        for (var i = 0; i < left.Length; i++)
        {
            result[i] = left[i] + right[i];
        }
        return result;
    }

    /// <summary>
    /// Computes dot product using SIMD vectorization.
    /// Used in machine learning, graphics, physics simulations.
    /// </summary>
    public static float DotProduct(float[] left, float[] right)
    {
        if (left == null) throw new ArgumentNullException(nameof(left));
        if (right == null) throw new ArgumentNullException(nameof(right));
        if (left.Length != right.Length)
            throw new ArgumentException("Arrays must have the same length");

        var vectorSize = Vector<float>.Count;
        var result = Vector<float>.Zero;
        var i = 0;

        // SIMD processing
        for (; i <= left.Length - vectorSize; i += vectorSize)
        {
            var v1 = new Vector<float>(left, i);
            var v2 = new Vector<float>(right, i);
            result += v1 * v2;
        }

        // Sum all elements in the result vector
        var sum = 0f;
        for (var j = 0; j < vectorSize; j++)
        {
            sum += result[j];
        }

        // Handle remaining elements
        for (; i < left.Length; i++)
        {
            sum += left[i] * right[i];
        }

        return sum;
    }

    /// <summary>
    /// Scales an array by a constant using SIMD.
    /// Common in graphics transformations and signal processing.
    /// </summary>
    public static void ScaleVector(float[] data, float scalar)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));

        var vectorSize = Vector<float>.Count;
        var scalarVector = new Vector<float>(scalar);
        var i = 0;

        // SIMD processing
        for (; i <= data.Length - vectorSize; i += vectorSize)
        {
            var v = new Vector<float>(data, i);
            var scaled = v * scalarVector;
            scaled.CopyTo(data, i);
        }

        // Handle remaining elements
        for (; i < data.Length; i++)
        {
            data[i] *= scalar;
        }
    }

    /// <summary>
    /// Finds minimum value in array using SIMD.
    /// Demonstrates conditional operations with SIMD.
    /// </summary>
    public static float FindMinimum(float[] data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (data.Length == 0) throw new ArgumentException("Array cannot be empty");

        var vectorSize = Vector<float>.Count;
        var minVector = new Vector<float>(float.MaxValue);
        var i = 0;

        // SIMD processing
        for (; i <= data.Length - vectorSize; i += vectorSize)
        {
            var v = new Vector<float>(data, i);
            minVector = Vector.Min(minVector, v);
        }

        // Find minimum in the min vector
        var min = float.MaxValue;
        for (var j = 0; j < vectorSize; j++)
        {
            if (minVector[j] < min)
                min = minVector[j];
        }

        // Handle remaining elements
        for (; i < data.Length; i++)
        {
            if (data[i] < min)
                min = data[i];
        }

        return min;
    }

    /// <summary>
    /// Matrix-vector multiplication using SIMD.
    /// Critical operation in graphics, machine learning, scientific computing.
    /// </summary>
    public static float[] MatrixVectorMultiply(float[,] matrix, float[] vector)
    {
        if (matrix == null) throw new ArgumentNullException(nameof(matrix));
        if (vector == null) throw new ArgumentNullException(nameof(vector));

        var rows = matrix.GetLength(0);
        var cols = matrix.GetLength(1);

        if (cols != vector.Length)
            throw new ArgumentException("Matrix columns must match vector length");

        var result = new float[rows];

        for (var row = 0; row < rows; row++)
        {
            var sum = 0f;
            var vectorSize = Vector<float>.Count;
            var col = 0;

            // SIMD processing for this row
            var accum = Vector<float>.Zero;
            for (; col <= cols - vectorSize; col += vectorSize)
            {
                // Extract row slice
                var rowData = new float[vectorSize];
                for (var k = 0; k < vectorSize; k++)
                {
                    rowData[k] = matrix[row, col + k];
                }

                var matrixVec = new Vector<float>(rowData);
                var vectorVec = new Vector<float>(vector, col);
                accum += matrixVec * vectorVec;
            }

            // Sum accumulator
            for (var k = 0; k < vectorSize; k++)
            {
                sum += accum[k];
            }

            // Handle remaining elements
            for (; col < cols; col++)
            {
                sum += matrix[row, col] * vector[col];
            }

            result[row] = sum;
        }

        return result;
    }

    /// <summary>
    /// Demonstrates SIMD-accelerated image processing (grayscale conversion).
    /// Simulates RGB to grayscale: Gray = 0.299*R + 0.587*G + 0.114*B
    /// </summary>
    public static byte[] RGBToGrayscale(byte[] rgbPixels)
    {
        if (rgbPixels == null) throw new ArgumentNullException(nameof(rgbPixels));
        if (rgbPixels.Length % 3 != 0)
            throw new ArgumentException("RGB array length must be multiple of 3");

        var pixelCount = rgbPixels.Length / 3;
        var grayscale = new byte[pixelCount];

        for (var i = 0; i < pixelCount; i++)
        {
            var r = rgbPixels[i * 3];
            var g = rgbPixels[i * 3 + 1];
            var b = rgbPixels[i * 3 + 2];

            // Grayscale formula
            grayscale[i] = (byte)(0.299 * r + 0.587 * g + 0.114 * b);
        }

        return grayscale;
    }

    /// <summary>
    /// Normalizes a vector to unit length using SIMD.
    /// Common in 3D graphics and machine learning.
    /// </summary>
    public static float[] Normalize(float[] vector)
    {
        if (vector == null) throw new ArgumentNullException(nameof(vector));
        if (vector.Length == 0) throw new ArgumentException("Vector cannot be empty");

        // Calculate magnitude
        var dotProduct = DotProduct(vector, vector);
        var magnitude = MathF.Sqrt(dotProduct);

        if (magnitude < 1e-10f)
            throw new InvalidOperationException("Cannot normalize zero vector");

        var result = new float[vector.Length];
        Array.Copy(vector, result, vector.Length);

        // Normalize
        ScaleVector(result, 1.0f / magnitude);

        return result;
    }

    /// <summary>
    /// Demonstrates SIMD capabilities and performance.
    /// </summary>
    public static void RunExample()
    {
        Console.WriteLine("=== SIMD (Vector<T>) Examples ===\n");

        // Display SIMD capabilities
        Console.WriteLine($"Vector<float>.Count: {Vector<float>.Count} (hardware vector width)");
        Console.WriteLine($"Vector<int>.Count: {Vector<int>.Count}");
        Console.WriteLine($"IsHardwareAccelerated: {Vector.IsHardwareAccelerated}");
        Console.WriteLine();

        // Example 1: Vector addition performance comparison
        Console.WriteLine("1. Vector Addition Performance:");
        var size = 1_000_000;
        var array1 = new float[size];
        var array2 = new float[size];

        for (var i = 0; i < size; i++)
        {
            array1[i] = i * 0.5f;
            array2[i] = i * 0.3f;
        }

        var sw = Stopwatch.StartNew();
        var scalarResult = ScalarAdd(array1, array2);
        var scalarTime = sw.Elapsed;

        sw.Restart();
        var vectorResult = VectorAdd(array1, array2);
        var vectorTime = sw.Elapsed;

        Console.WriteLine($"  Scalar time: {scalarTime.TotalMilliseconds:F3} ms");
        Console.WriteLine($"  SIMD time:   {vectorTime.TotalMilliseconds:F3} ms");
        Console.WriteLine($"  Speedup:     {scalarTime.TotalMilliseconds / vectorTime.TotalMilliseconds:F2}x");
        Console.WriteLine();

        // Example 2: Dot product
        Console.WriteLine("2. Dot Product (1M elements):");
        sw.Restart();
        var dotProduct = DotProduct(array1, array2);
        Console.WriteLine($"  Result: {dotProduct:F2}");
        Console.WriteLine($"  Time: {sw.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine();

        // Example 3: Vector scaling
        Console.WriteLine("3. Vector Scaling:");
        var testVector = new float[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        Console.WriteLine($"  Original: [{string.Join(", ", testVector)}]");
        ScaleVector(testVector, 2.5f);
        Console.WriteLine($"  Scaled by 2.5: [{string.Join(", ", testVector.Select(x => $"{x:F1}"))}]");
        Console.WriteLine();

        // Example 4: Find minimum
        Console.WriteLine("4. Find Minimum (SIMD):");
        var randomData = new float[100];
        var random = new Random(42);
        for (var i = 0; i < randomData.Length; i++)
        {
            randomData[i] = (float)random.NextDouble() * 1000;
        }
        var minimum = FindMinimum(randomData);
        Console.WriteLine($"  Minimum value: {minimum:F2}");
        Console.WriteLine($"  Verification: {randomData.Min():F2}");
        Console.WriteLine();

        // Example 5: Matrix-vector multiplication
        Console.WriteLine("5. Matrix-Vector Multiplication (SIMD):");
        var matrix = new float[,] {
            { 1, 2, 3, 4 },
            { 5, 6, 7, 8 },
            { 9, 10, 11, 12 }
        };
        var vec = new float[] { 1, 0, -1, 2 };
        var matVecResult = MatrixVectorMultiply(matrix, vec);
        Console.WriteLine($"  Input vector: [{string.Join(", ", vec)}]");
        Console.WriteLine($"  Result: [{string.Join(", ", matVecResult.Select(x => $"{x:F0}"))}]");
        Console.WriteLine();

        // Example 6: Vector normalization
        Console.WriteLine("6. Vector Normalization:");
        var unnormalized = new float[] { 3, 4, 0, 0 };
        Console.WriteLine($"  Original: [{string.Join(", ", unnormalized)}]");
        var normalized = Normalize(unnormalized);
        Console.WriteLine($"  Normalized: [{string.Join(", ", normalized.Select(x => $"{x:F3}"))}]");
        var magnitude = MathF.Sqrt(DotProduct(normalized, normalized));
        Console.WriteLine($"  Magnitude (should be 1.0): {magnitude:F10}");
        Console.WriteLine();

        // Example 7: Image processing simulation
        Console.WriteLine("7. RGB to Grayscale (Image Processing):");
        var rgbPixels = new byte[3 * 1920 * 1080]; // Full HD image
        random.NextBytes(rgbPixels);

        sw.Restart();
        var grayscale = RGBToGrayscale(rgbPixels);
        Console.WriteLine($"  Processed {grayscale.Length:N0} pixels");
        Console.WriteLine($"  Time: {sw.Elapsed.TotalMilliseconds:F2} ms");
        Console.WriteLine($"  Throughput: {grayscale.Length / sw.Elapsed.TotalSeconds / 1_000_000:F2} MPixels/sec");
    }
}
