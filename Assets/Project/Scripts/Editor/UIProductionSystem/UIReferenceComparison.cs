#if UNITY_EDITOR
using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class UIReferenceComparison
{
    private const float PixelTolerance = 0.10f;

    [MenuItem("Quantum Forge/UI Production/Comparar Carta Galáctica aprobada")]
    public static void RunDimension1Galaxy()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        string reference = Path.Combine(root,
            "SISTEMA_UI_QUANTUM_FORGE/05_REFERENCIAS/DIMENSION_1/CARTA_GALACTICA/referencia_carta_galactica.png");
        string capture = Path.Combine(root,
            "SISTEMA_UI_QUANTUM_FORGE/06_CAPTURAS_APROBADAS/DIMENSION_1/CARTA_GALACTICA/carta_galactica_neutral_1080x1920.png");
        string output = Path.Combine(root,
            "Logs/UIProductionSystem/Comparisons/Dimension1/CartaGalactica");

        ComparisonResult result = CompareFiles(reference, capture, output);
        Debug.Log("[UI Production System] COMPARISON_COMPLETE | similitud objetiva auxiliar=" +
                  (result.normalizedSimilarity * 100f).ToString("F2") + "% | " + result.reportPath);
    }

    public static ComparisonResult CompareFiles(string referencePath, string capturePath, string outputDirectory)
    {
        if (!File.Exists(referencePath))
            throw new FileNotFoundException("No existe la referencia.", referencePath);
        if (!File.Exists(capturePath))
            throw new FileNotFoundException("No existe la captura.", capturePath);

        Directory.CreateDirectory(outputDirectory);
        Texture2D reference = LoadTexture(referencePath);
        Texture2D capture = LoadTexture(capturePath);
        Texture2D normalizedReference = null;
        Texture2D overlay = null;
        Texture2D heatmap = null;

        try
        {
            normalizedReference = ResizeBilinear(reference, capture.width, capture.height);
            Color32[] referencePixels = normalizedReference.GetPixels32();
            Color32[] capturePixels = capture.GetPixels32();
            Color32[] overlayPixels = new Color32[capturePixels.Length];
            Color32[] heatmapPixels = new Color32[capturePixels.Length];

            double absoluteError = 0d;
            int matchingPixels = 0;
            int toleranceByte = Mathf.RoundToInt(PixelTolerance * 255f);

            for (int i = 0; i < capturePixels.Length; i++)
            {
                Color32 a = referencePixels[i];
                Color32 b = capturePixels[i];
                int dr = Mathf.Abs(a.r - b.r);
                int dg = Mathf.Abs(a.g - b.g);
                int db = Mathf.Abs(a.b - b.b);
                int maximumDifference = Mathf.Max(dr, Mathf.Max(dg, db));
                absoluteError += (dr + dg + db) / (3d * 255d);
                if (maximumDifference <= toleranceByte)
                    matchingPixels++;

                overlayPixels[i] = new Color32(
                    (byte)((a.r + b.r) / 2),
                    (byte)((a.g + b.g) / 2),
                    (byte)((a.b + b.b) / 2),
                    255);

                heatmapPixels[i] = HeatColor((byte)maximumDifference);
            }

            float meanAbsoluteError = (float)(absoluteError / capturePixels.Length);
            float similarity = Mathf.Clamp01(1f - meanAbsoluteError);
            float matchingRatio = matchingPixels / (float)capturePixels.Length;

            overlay = CreateTexture(capture.width, capture.height, overlayPixels);
            heatmap = CreateTexture(capture.width, capture.height, heatmapPixels);
            string overlayPath = Path.Combine(outputDirectory, "overlay_50_50.png");
            string heatmapPath = Path.Combine(outputDirectory, "difference_heatmap.png");
            File.WriteAllBytes(overlayPath, overlay.EncodeToPNG());
            File.WriteAllBytes(heatmapPath, heatmap.EncodeToPNG());

            string reportPath = Path.Combine(outputDirectory, "comparison_report.txt");
            File.WriteAllText(reportPath, BuildReport(
                referencePath,
                capturePath,
                reference.width,
                reference.height,
                capture.width,
                capture.height,
                meanAbsoluteError,
                similarity,
                matchingRatio,
                overlayPath,
                heatmapPath), new UTF8Encoding(false));

            AssetDatabase.Refresh();
            return new ComparisonResult
            {
                normalizedSimilarity = similarity,
                matchingPixelRatio = matchingRatio,
                reportPath = reportPath,
                overlayPath = overlayPath,
                heatmapPath = heatmapPath
            };
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(reference);
            UnityEngine.Object.DestroyImmediate(capture);
            if (normalizedReference != null) UnityEngine.Object.DestroyImmediate(normalizedReference);
            if (overlay != null) UnityEngine.Object.DestroyImmediate(overlay);
            if (heatmap != null) UnityEngine.Object.DestroyImmediate(heatmap);
        }
    }

    private static Texture2D LoadTexture(string path)
    {
        Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (!ImageConversion.LoadImage(texture, File.ReadAllBytes(path), false))
        {
            UnityEngine.Object.DestroyImmediate(texture);
            throw new InvalidOperationException("No se pudo leer la imagen: " + path);
        }
        return texture;
    }

    private static Texture2D ResizeBilinear(Texture2D source, int width, int height)
    {
        Texture2D result = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            float v = height > 1 ? y / (float)(height - 1) : 0f;
            for (int x = 0; x < width; x++)
            {
                float u = width > 1 ? x / (float)(width - 1) : 0f;
                pixels[y * width + x] = source.GetPixelBilinear(u, v);
            }
        }
        result.SetPixels(pixels);
        result.Apply(false, false);
        return result;
    }

    private static Texture2D CreateTexture(int width, int height, Color32[] pixels)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.SetPixels32(pixels);
        texture.Apply(false, false);
        return texture;
    }

    private static Color32 HeatColor(byte difference)
    {
        float value = difference / 255f;
        byte red = (byte)Mathf.RoundToInt(255f * value);
        byte green = (byte)Mathf.RoundToInt(255f * Mathf.Clamp01((value - 0.25f) * 1.5f));
        byte blue = (byte)Mathf.RoundToInt(80f * (1f - value));
        return new Color32(red, green, blue, 255);
    }

    private static string BuildReport(
        string referencePath,
        string capturePath,
        int referenceWidth,
        int referenceHeight,
        int captureWidth,
        int captureHeight,
        float meanAbsoluteError,
        float similarity,
        float matchingRatio,
        string overlayPath,
        string heatmapPath)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("UI PRODUCTION SYSTEM - COMPARACIÓN AUXILIAR");
        builder.AppendLine("Fecha: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        builder.AppendLine("Referencia: " + referencePath);
        builder.AppendLine("Captura: " + capturePath);
        builder.AppendLine("Tamaño original referencia: " + referenceWidth + "x" + referenceHeight);
        builder.AppendLine("Tamaño captura: " + captureWidth + "x" + captureHeight);
        builder.AppendLine("Normalización aplicada: estirado bilineal al tamaño de la captura.");
        builder.AppendLine("Error absoluto medio RGB: " + meanAbsoluteError.ToString("F6"));
        builder.AppendLine("Similitud objetiva auxiliar: " + (similarity * 100f).ToString("F2") + "%");
        builder.AppendLine("Píxeles dentro de tolerancia del 10 %: " + (matchingRatio * 100f).ToString("F2") + "%");
        builder.AppendLine("Overlay: " + overlayPath);
        builder.AppendLine("Mapa de diferencias: " + heatmapPath);
        builder.AppendLine();
        builder.AppendLine("IMPORTANTE: esta cifra no sustituye la meta de 95 % percibido ni la aprobación humana.");
        builder.AppendLine("Cambios de recorte, tipografía o render pueden bajar la cifra aunque la composición sea correcta.");
        return builder.ToString();
    }

    public struct ComparisonResult
    {
        public float normalizedSimilarity;
        public float matchingPixelRatio;
        public string reportPath;
        public string overlayPath;
        public string heatmapPath;
    }
}
#endif
