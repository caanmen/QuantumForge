param(
    [Parameter(Mandatory = $true)][string]$InputPath,
    [Parameter(Mandatory = $true)][string]$OutputPath,
    [int]$Padding = 18
)

Add-Type -AssemblyName System.Drawing

$source = @'
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

public static class GeneratedBackgroundExtractor
{
    private static bool IsBackground(byte b, byte g, byte r)
    {
        int maximum = Math.Max(r, Math.Max(g, b));
        int minimum = Math.Min(r, Math.Min(g, b));
        int average = (r + g + b) / 3;
        return maximum - minimum <= 24 && average >= 180;
    }

    private static void TryEnqueue(int index, int width, int stride, byte[] pixels,
        bool[] queued, int[] queue, ref int tail)
    {
        if (queued[index]) return;
        int x = index % width;
        int y = index / width;
        int offset = y * stride + x * 4;
        if (!IsBackground(pixels[offset], pixels[offset + 1], pixels[offset + 2])) return;
        queued[index] = true;
        queue[tail++] = index;
    }

    public static string Extract(string inputPath, string outputPath, int padding)
    {
        using (var original = new Bitmap(inputPath))
        using (var source = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppArgb))
        {
            using (Graphics graphics = Graphics.FromImage(source))
                graphics.DrawImageUnscaled(original, 0, 0);

            int width = source.Width;
            int height = source.Height;
            var rect = new Rectangle(0, 0, width, height);
            BitmapData data = source.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int sourceStride = data.Stride;
            byte[] pixels = new byte[Math.Abs(sourceStride) * height];
            Marshal.Copy(data.Scan0, pixels, 0, pixels.Length);
            source.UnlockBits(data);

            bool[] background = new bool[width * height];
            bool[] queued = new bool[width * height];
            int[] queue = new int[width * height];
            int head = 0;
            int tail = 0;

            for (int x = 0; x < width; x++)
            {
                TryEnqueue(x, width, sourceStride, pixels, queued, queue, ref tail);
                TryEnqueue((height - 1) * width + x, width, sourceStride, pixels, queued, queue, ref tail);
            }
            for (int y = 0; y < height; y++)
            {
                TryEnqueue(y * width, width, sourceStride, pixels, queued, queue, ref tail);
                TryEnqueue(y * width + width - 1, width, sourceStride, pixels, queued, queue, ref tail);
            }

            while (head < tail)
            {
                int index = queue[head++];
                background[index] = true;
                int x = index % width;
                int y = index / width;
                for (int dy = -1; dy <= 1; dy++)
                for (int dx = -1; dx <= 1; dx++)
                {
                    if (dx == 0 && dy == 0) continue;
                    int nx = x + dx;
                    int ny = y + dy;
                    if (nx < 0 || nx >= width || ny < 0 || ny >= height) continue;
                    TryEnqueue(ny * width + nx, width, sourceStride, pixels, queued, queue, ref tail);
                }
            }

            int minX = width, minY = height, maxX = -1, maxY = -1;
            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                if (background[y * width + x]) continue;
                minX = Math.Min(minX, x);
                minY = Math.Min(minY, y);
                maxX = Math.Max(maxX, x);
                maxY = Math.Max(maxY, y);
            }
            if (maxX < minX || maxY < minY)
                throw new InvalidOperationException("No foreground detected.");

            int outputWidth = maxX - minX + 1 + padding * 2;
            int outputHeight = maxY - minY + 1 + padding * 2;
            using (var output = new Bitmap(outputWidth, outputHeight, PixelFormat.Format32bppArgb))
            {
                var outputRect = new Rectangle(0, 0, outputWidth, outputHeight);
                BitmapData outputData = output.LockBits(outputRect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                int outputStride = outputData.Stride;
                byte[] result = new byte[Math.Abs(outputStride) * outputHeight];
                for (int y = minY; y <= maxY; y++)
                for (int x = minX; x <= maxX; x++)
                {
                    int sourceIndex = y * width + x;
                    if (background[sourceIndex]) continue;
                    int sourceOffset = y * sourceStride + x * 4;
                    int targetX = x - minX + padding;
                    int targetY = y - minY + padding;
                    int targetOffset = targetY * outputStride + targetX * 4;
                    result[targetOffset] = pixels[sourceOffset];
                    result[targetOffset + 1] = pixels[sourceOffset + 1];
                    result[targetOffset + 2] = pixels[sourceOffset + 2];
                    result[targetOffset + 3] = 255;
                }
                Marshal.Copy(result, 0, outputData.Scan0, result.Length);
                output.UnlockBits(outputData);
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
                output.Save(outputPath, ImageFormat.Png);
            }
            return outputWidth + "x" + outputHeight;
        }
    }
}
'@

$drawingAssembly = [System.Drawing.Bitmap].Assembly.Location
$primitivesAssembly = [System.Drawing.Rectangle].Assembly.Location
$runtimeDirectory = Split-Path -Parent $drawingAssembly
$references = @(
    $drawingAssembly,
    $primitivesAssembly,
    (Join-Path $runtimeDirectory 'System.Private.Windows.Core.dll'),
    (Join-Path $runtimeDirectory 'System.Private.Windows.GdiPlus.dll'),
    (Join-Path $runtimeDirectory 'System.Collections.dll'),
    (Join-Path $runtimeDirectory 'System.Private.CoreLib.dll'),
    (Join-Path $runtimeDirectory 'System.Runtime.dll'),
    (Join-Path $runtimeDirectory 'System.Runtime.InteropServices.dll')
)
Add-Type -TypeDefinition $source -ReferencedAssemblies $references
$dimensions = [GeneratedBackgroundExtractor]::Extract($InputPath, $OutputPath, $Padding)
[PSCustomObject]@{
    Input = $InputPath
    Output = $OutputPath
    Dimensions = $dimensions
    PixelFormat = 'Format32bppArgb'
}
