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
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/DIMENSION_1/CARTA_GALACTICA/referencia_carta_galactica.png");
        string capture = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/06_CAPTURAS_APROBADAS/DIMENSION_1/CARTA_GALACTICA/carta_galactica_neutral_1080x1920.png");
        string output = Path.Combine(root,
            "Logs/UIProductionSystem/Comparisons/Dimension1/CartaGalactica");

        ComparisonResult result = CompareFiles(reference, capture, output);
        Debug.Log("[UI Production System] COMPARISON_COMPLETE | similitud objetiva auxiliar=" +
                  (result.normalizedSimilarity * 100f).ToString("F2") + "% | " + result.reportPath);
    }

    [MenuItem("Quantum Forge/UI Production/Comparar Mapa de los Pactos")]
    public static void RunDimension2PactMap()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        string reference = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/DIMENSION_2/CORREGIDAS_V4_2026-08-21/01_Mapa_De_Los_Pactos_Corregido_V5.png");
        string capture = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/DIMENSION_2/MAPA_DE_LOS_PACTOS/CAPTURAS_CANDIDATAS/D2_Mapa_De_Los_Pactos_1080x1920.png");
        string output = Path.Combine(root,
            "Logs/UIProductionSystem/Comparisons/Dimension2/MapaDeLosPactos");
        ComparisonResult result = CompareFiles(reference, capture, output);
        Debug.Log("[D2 Pact Map] COMPARISON_COMPLETE | similitud objetiva auxiliar=" +
                  (result.normalizedSimilarity * 100f).ToString("F2") + "% | píxeles tolerancia=" +
                  (result.matchingPixelRatio * 100f).ToString("F2") + "% | similitud percibida AA=" +
                  (result.perceptualSimilarity * 100f).ToString("F2") + "% | " + result.reportPath);
    }

    [MenuItem("Quantum Forge/UI Production/Comparar Santuario Refugio")]
    public static void RunDimension2Sanctuary()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        string reference = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/DIMENSION_2/CORREGIDAS_V4_2026-08-21/02_El_Santuario_Refugio_Corregido_V4.png");
        string captures = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/DIMENSION_2/SANTUARIO_REFUGIO/CAPTURAS_CANDIDATAS");
        ComparisonResult result1080 = CompareFiles(reference,
            Path.Combine(captures, "D2_Santuario_Refugio_1080x1920.png"),
            Path.Combine(root,
                "Logs/UIProductionSystem/Comparisons/Dimension2/SantuarioRefugio/1080x1920"));
        ComparisonResult result720 = CompareFiles(reference,
            Path.Combine(captures, "D2_Santuario_Refugio_720x1280.png"),
            Path.Combine(root,
                "Logs/UIProductionSystem/Comparisons/Dimension2/SantuarioRefugio/720x1280"));
        Debug.Log("[D2 Sanctuary] COMPARISON_COMPLETE | 1080 percibida AA=" +
            (result1080.perceptualSimilarity * 100f).ToString("F2") +
            "% | 720 percibida AA=" +
            (result720.perceptualSimilarity * 100f).ToString("F2") + "% | " +
            result1080.reportPath);
    }

    [MenuItem("Quantum Forge/UI Production/Comparar Altares del Santuario")]
    public static void RunDimension2Altars()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        string reference = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/DIMENSION_2/CORREGIDAS_V4_2026-08-21/03_Altares_Del_Santuario_Corregido_V4.png");
        string captures = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/DIMENSION_2/ALTARES_DEL_SANTUARIO/CAPTURAS_CANDIDATAS");
        ComparisonResult result1080 = CompareFiles(reference,
            Path.Combine(captures, "D2_Altares_Del_Santuario_1080x1920.png"),
            Path.Combine(root,
                "Logs/UIProductionSystem/Comparisons/Dimension2/AltaresDelSantuario/1080x1920"));
        ComparisonResult result720 = CompareFiles(reference,
            Path.Combine(captures, "D2_Altares_Del_Santuario_720x1280.png"),
            Path.Combine(root,
                "Logs/UIProductionSystem/Comparisons/Dimension2/AltaresDelSantuario/720x1280"));
        Debug.Log("[D2 Altars] COMPARISON_COMPLETE | 1080 percibida AA=" +
            (result1080.perceptualSimilarity * 100f).ToString("F2") +
            "% | 720 percibida AA=" +
            (result720.perceptualSimilarity * 100f).ToString("F2") + "% | " +
            result1080.reportPath);
        if (result1080.perceptualSimilarity < 0.95f ||
            result720.perceptualSimilarity < 0.95f)
            throw new InvalidOperationException(
                "Altares no alcanzó el gate perceptual AA mínimo de 95 %.");
    }

    [MenuItem("Quantum Forge/UI Production/Comparar Peregrinaciones")]
    public static void RunDimension2Pilgrimages()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        string reference = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/DIMENSION_2/CORREGIDAS_V4_2026-08-21/04_Peregrinaciones_Corregidas_V4.png");
        string captures = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/DIMENSION_2/PEREGRINACIONES/CAPTURAS_CANDIDATAS");
        ComparisonResult result1080 = CompareFiles(reference,
            Path.Combine(captures, "D2_Peregrinaciones_1080x1920.png"),
            Path.Combine(root,
                "Logs/UIProductionSystem/Comparisons/Dimension2/Peregrinaciones/1080x1920"));
        ComparisonResult result720 = CompareFiles(reference,
            Path.Combine(captures, "D2_Peregrinaciones_720x1280.png"),
            Path.Combine(root,
                "Logs/UIProductionSystem/Comparisons/Dimension2/Peregrinaciones/720x1280"));
        Debug.Log("[D2 Pilgrimages] COMPARISON_COMPLETE | 1080 percibida AA=" +
            (result1080.perceptualSimilarity * 100f).ToString("F2") +
            "% | 720 percibida AA=" +
            (result720.perceptualSimilarity * 100f).ToString("F2") + "% | " +
            result1080.reportPath);
        if (result1080.perceptualSimilarity < 0.95f ||
            result720.perceptualSimilarity < 0.95f)
            throw new InvalidOperationException(
                "Peregrinaciones no alcanzó el gate perceptual AA mínimo de 95 %.");
    }

    [MenuItem("Quantum Forge/UI Production/Comparar Noviciado")]
    public static void RunDimension2Novitiate()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        string reference = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/DIMENSION_2/CORREGIDAS_V4_2026-08-21/05_Noviciado_Corregido_V4.png");
        string captures = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/DIMENSION_2/NOVICIADO/CAPTURAS_CANDIDATAS");
        ComparisonResult result1080 = CompareFiles(reference,
            Path.Combine(captures, "D2_Noviciado_1080x1920.png"),
            Path.Combine(root, "Logs/UIProductionSystem/Comparisons/Dimension2/Novitiate/1080x1920"));
        ComparisonResult result720 = CompareFiles(reference,
            Path.Combine(captures, "D2_Noviciado_720x1280.png"),
            Path.Combine(root, "Logs/UIProductionSystem/Comparisons/Dimension2/Novitiate/720x1280"));
        Debug.Log("[D2 Novitiate] COMPARISON_COMPLETE | 1080 percibida AA=" +
            (result1080.perceptualSimilarity * 100f).ToString("F2") +
            "% | 720 percibida AA=" + (result720.perceptualSimilarity * 100f).ToString("F2") +
            "% | " + result1080.reportPath);
        if (result1080.perceptualSimilarity < 0.95f || result720.perceptualSimilarity < 0.95f)
            throw new InvalidOperationException("Noviciado no alcanzó el gate perceptual AA mínimo de 95 %.");
    }

    [MenuItem("Quantum Forge/UI Production/Comparar Ritos del Santuario")]
    public static void RunDimension2Rites()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        string reference = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/DIMENSION_2/CORREGIDAS_V4_2026-08-21/06_Ritos_Del_Santuario_Corregidos_V4.png");
        string captures = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/DIMENSION_2/RITOS_DEL_SANTUARIO/CAPTURAS_CANDIDATAS");
        ComparisonResult result1080 = CompareFiles(reference,
            Path.Combine(captures, "D2_Ritos_Del_Santuario_1080x1920.png"),
            Path.Combine(root, "Logs/UIProductionSystem/Comparisons/Dimension2/Rites/1080x1920"));
        ComparisonResult result720 = CompareFiles(reference,
            Path.Combine(captures, "D2_Ritos_Del_Santuario_720x1280.png"),
            Path.Combine(root, "Logs/UIProductionSystem/Comparisons/Dimension2/Rites/720x1280"));
        Debug.Log("[D2 Rites] COMPARISON_COMPLETE | 1080 percibida AA=" +
            (result1080.perceptualSimilarity * 100f).ToString("F2") +
            "% | 720 percibida AA=" + (result720.perceptualSimilarity * 100f).ToString("F2") +
            "% | " + result1080.reportPath);
        if (result1080.perceptualSimilarity < 0.95f || result720.perceptualSimilarity < 0.95f)
            throw new InvalidOperationException("Ritos no alcanzó el gate perceptual AA mínimo de 95 %.");
    }

    [MenuItem("Quantum Forge/UI Production/Comparar Pactos de Civilización")]
    public static void RunDimension2CivilizationPacts()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        string reference = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/DIMENSION_2/CORREGIDAS_V4_2026-08-21/07_Pactos_De_Civilizacion_Corregidos_V4.png");
        string captures = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/DIMENSION_2/PACTOS_DE_CIVILIZACION/CAPTURAS_CANDIDATAS");
        ComparisonResult result1080 = CompareFiles(reference,
            Path.Combine(captures, "D2_Pactos_De_Civilizacion_1080x1920.png"),
            Path.Combine(root, "Logs/UIProductionSystem/Comparisons/Dimension2/PactosDeCivilizacion/1080x1920"));
        ComparisonResult result720 = CompareFiles(reference,
            Path.Combine(captures, "D2_Pactos_De_Civilizacion_720x1280.png"),
            Path.Combine(root, "Logs/UIProductionSystem/Comparisons/Dimension2/PactosDeCivilizacion/720x1280"));
        Debug.Log("[D2 Pacts] COMPARISON_COMPLETE | 1080 percibida AA=" +
            (result1080.perceptualSimilarity * 100f).ToString("F2") +
            "% | 720 percibida AA=" + (result720.perceptualSimilarity * 100f).ToString("F2") +
            "% | " + result1080.reportPath);
        if (result1080.perceptualSimilarity < 0.95f || result720.perceptualSimilarity < 0.95f)
            throw new InvalidOperationException(
                "Pactos de Civilización no alcanzó el gate perceptual AA mínimo de 95 %.");
    }

    [MenuItem("Quantum Forge/UI Production/Comparar Pacto Lugar de Vínculo")]
    public static void RunDimension2BondPlace()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        string reference = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/DIMENSION_2/CORREGIDAS_V4_2026-08-21/08_Pacto_Lugar_De_Vinculo_Corregido_V4.png");
        string captures = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/DIMENSION_2/PACTO_LUGAR_DE_VINCULO/CAPTURAS_CANDIDATAS");
        ComparisonResult result1080 = CompareFiles(reference,
            Path.Combine(captures, "D2_Pacto_Lugar_De_Vinculo_1080x1920.png"),
            Path.Combine(root, "Logs/UIProductionSystem/Comparisons/Dimension2/BondPlace/1080x1920"));
        ComparisonResult result720 = CompareFiles(reference,
            Path.Combine(captures, "D2_Pacto_Lugar_De_Vinculo_720x1280.png"),
            Path.Combine(root, "Logs/UIProductionSystem/Comparisons/Dimension2/BondPlace/720x1280"));
        Debug.Log("[D2 Bond] COMPARISON_COMPLETE | 1080 percibida AA=" +
            (result1080.perceptualSimilarity * 100f).ToString("F2") +
            "% | 720 percibida AA=" + (result720.perceptualSimilarity * 100f).ToString("F2") +
            "% | " + result1080.reportPath);
        if (result1080.perceptualSimilarity < 0.95f || result720.perceptualSimilarity < 0.95f)
            throw new InvalidOperationException(
                "Pacto/Lugar de Vínculo no alcanzó el gate perceptual AA mínimo de 95 %.");
    }

    [MenuItem("Quantum Forge/UI Production/Comparar Red de Resistencia")]
    public static void RunDimension2ResistanceRegions()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        string reference = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/" +
            "DIMENSION_2/CORREGIDAS_V4_2026-08-21/" +
            "09_Red_De_Resistencia_Regiones_Corregida_V4.png");
        string captures = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/" +
            "DIMENSION_2/RED_DE_RESISTENCIA/CAPTURAS_CANDIDATAS");
        ComparisonResult result1080 = CompareFiles(reference,
            Path.Combine(captures, "D2_Red_De_Resistencia_1080x1920.png"),
            Path.Combine(root,
                "Logs/UIProductionSystem/Comparisons/Dimension2/ResistanceRegions/1080x1920"));
        ComparisonResult result720 = CompareFiles(reference,
            Path.Combine(captures, "D2_Red_De_Resistencia_720x1280.png"),
            Path.Combine(root,
                "Logs/UIProductionSystem/Comparisons/Dimension2/ResistanceRegions/720x1280"));
        Debug.Log("[D2 Resistance Regions] COMPARISON_COMPLETE | 1080 percibida AA=" +
            (result1080.perceptualSimilarity * 100f).ToString("F2") +
            "% | 720 percibida AA=" +
            (result720.perceptualSimilarity * 100f).ToString("F2") + "% | " +
            result1080.reportPath);
        if (result1080.perceptualSimilarity < 0.95f ||
            result720.perceptualSimilarity < 0.95f)
            throw new InvalidOperationException(
                "Red de Resistencia no alcanzó el gate perceptual AA mínimo de 95 %.");
    }

    [MenuItem("Quantum Forge/UI Production/Comparar Operaciones de Resistencia")]
    public static void RunDimension2ResistanceOperations()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        string reference = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/" +
            "DIMENSION_2/CORREGIDAS_V4_2026-08-21/" +
            "10_Operaciones_De_Resistencia_Corregidas_V4.png");
        string captures = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/" +
            "DIMENSION_2/OPERACIONES_DE_RESISTENCIA/CAPTURAS_CANDIDATAS");
        ComparisonResult result1080 = CompareFiles(reference,
            Path.Combine(captures, "D2_Operaciones_De_Resistencia_1080x1920.png"),
            Path.Combine(root,
                "Logs/UIProductionSystem/Comparisons/Dimension2/ResistanceOperations/1080x1920"));
        ComparisonResult result720 = CompareFiles(reference,
            Path.Combine(captures, "D2_Operaciones_De_Resistencia_720x1280.png"),
            Path.Combine(root,
                "Logs/UIProductionSystem/Comparisons/Dimension2/ResistanceOperations/720x1280"));
        Debug.Log("[D2 Resistance Operations] COMPARISON_COMPLETE | 1080 percibida AA=" +
            (result1080.perceptualSimilarity * 100f).ToString("F2") +
            "% | 720 percibida AA=" +
            (result720.perceptualSimilarity * 100f).ToString("F2") + "% | " +
            result1080.reportPath);
        if (result1080.perceptualSimilarity < 0.95f ||
            result720.perceptualSimilarity < 0.95f)
            throw new InvalidOperationException(
                "Operaciones de Resistencia no alcanzó el gate perceptual AA mínimo de 95 %.");
    }

    [MenuItem("Quantum Forge/UI Production/Comparar Defensa y Represalias")]
    public static void RunDimension2ResistanceDefense()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        string reference = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/" +
            "DIMENSION_2/CORREGIDAS_V4_2026-08-21/" +
            "11_Defensa_Y_Represalias_Corregida_V4.png");
        string captures = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/" +
            "DIMENSION_2/DEFENSA_Y_REPRESALIAS/CAPTURAS_CANDIDATAS");
        ComparisonResult result1080 = CompareFiles(reference,
            Path.Combine(captures, "D2_Defensa_Y_Represalias_1080x1920.png"),
            Path.Combine(root,
                "Logs/UIProductionSystem/Comparisons/Dimension2/ResistanceDefense/1080x1920"));
        ComparisonResult result720 = CompareFiles(reference,
            Path.Combine(captures, "D2_Defensa_Y_Represalias_720x1280.png"),
            Path.Combine(root,
                "Logs/UIProductionSystem/Comparisons/Dimension2/ResistanceDefense/720x1280"));
        Debug.Log("[D2 Resistance Defense] COMPARISON_COMPLETE | 1080 percibida AA=" +
            (result1080.perceptualSimilarity * 100f).ToString("F2") +
            "% | 720 percibida AA=" +
            (result720.perceptualSimilarity * 100f).ToString("F2") + "% | " +
            result1080.reportPath);
        if (result1080.perceptualSimilarity < 0.95f ||
            result720.perceptualSimilarity < 0.95f)
            throw new InvalidOperationException(
                "Defensa y Represalias no alcanzó el gate perceptual AA mínimo de 95 %.");
    }

    [MenuItem("Quantum Forge/UI Production/Comparar Resistencia y Pactos")]
    public static void RunDimension2ResistancePacts()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        string reference = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/" +
            "DIMENSION_2/CORREGIDAS_V4_2026-08-21/" +
            "12_Resistencia_Y_Pactos_Corregida_V4.png");
        string captures = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/" +
            "DIMENSION_2/RESISTENCIA_Y_PACTOS/CAPTURAS_CANDIDATAS");
        ComparisonResult result1080 = CompareFiles(reference,
            Path.Combine(captures, "D2_Resistencia_Y_Pactos_1080x1920.png"),
            Path.Combine(root,
                "Logs/UIProductionSystem/Comparisons/Dimension2/ResistancePacts/1080x1920"));
        ComparisonResult result720 = CompareFiles(reference,
            Path.Combine(captures, "D2_Resistencia_Y_Pactos_720x1280.png"),
            Path.Combine(root,
                "Logs/UIProductionSystem/Comparisons/Dimension2/ResistancePacts/720x1280"));
        Debug.Log("[D2 Resistance Pacts] COMPARISON_COMPLETE | 1080 percibida AA=" +
            (result1080.perceptualSimilarity * 100f).ToString("F2") +
            "% | 720 percibida AA=" +
            (result720.perceptualSimilarity * 100f).ToString("F2") + "% | " +
            result1080.reportPath);
        if (result1080.perceptualSimilarity < 0.95f ||
            result720.perceptualSimilarity < 0.95f)
            throw new InvalidOperationException(
                "Resistencia y Pactos no alcanzó el gate perceptual AA mínimo de 95 %.");
    }

    [MenuItem("Quantum Forge/UI Production/Comparar Alerta del Ente")]
    public static void RunDimension2ResistanceAlert()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        string reference = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/" +
            "DIMENSION_2/CORREGIDAS_V4_2026-08-21/" +
            "13_Alerta_Del_Ente_Corregida_V4.png");
        string captures = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/" +
            "DIMENSION_2/ALERTA_DEL_ENTE/CAPTURAS_CANDIDATAS");
        ComparisonResult result1080 = CompareFiles(reference,
            Path.Combine(captures, "D2_Alerta_Del_Ente_1080x1920.png"),
            Path.Combine(root,
                "Logs/UIProductionSystem/Comparisons/Dimension2/ResistanceAlert/1080x1920"));
        ComparisonResult result720 = CompareFiles(reference,
            Path.Combine(captures, "D2_Alerta_Del_Ente_720x1280.png"),
            Path.Combine(root,
                "Logs/UIProductionSystem/Comparisons/Dimension2/ResistanceAlert/720x1280"));
        Debug.Log("[D2 Resistance Alert] COMPARISON_COMPLETE | 1080 percibida AA=" +
            (result1080.perceptualSimilarity * 100f).ToString("F2") +
            "% | 720 percibida AA=" +
            (result720.perceptualSimilarity * 100f).ToString("F2") + "% | " +
            result1080.reportPath);
        if (result1080.perceptualSimilarity < 0.95f ||
            result720.perceptualSimilarity < 0.95f)
            throw new InvalidOperationException(
                "Alerta del Ente no alcanzó el gate perceptual AA mínimo de 95 %.");
    }

    [MenuItem("Quantum Forge/UI Production/Comparar Contención")]
    public static void RunDimension2ResistanceContainment()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        string reference = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/" +
            "DIMENSION_2/CORREGIDAS_V4_2026-08-21/" +
            "14_Contencion_Intento_Corregida_V4.png");
        string captures = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/" +
            "DIMENSION_2/CONTENCION/CAPTURAS_CANDIDATAS");
        ComparisonResult result1080 = CompareFiles(reference,
            Path.Combine(captures, "D2_Contencion_1080x1920.png"),
            Path.Combine(root,
                "Logs/UIProductionSystem/Comparisons/Dimension2/ResistanceContainment/1080x1920"));
        ComparisonResult result720 = CompareFiles(reference,
            Path.Combine(captures, "D2_Contencion_720x1280.png"),
            Path.Combine(root,
                "Logs/UIProductionSystem/Comparisons/Dimension2/ResistanceContainment/720x1280"));
        Debug.Log("[D2 Resistance Containment] COMPARISON_COMPLETE | 1080 percibida AA=" +
            (result1080.perceptualSimilarity * 100f).ToString("F2") +
            "% | 720 percibida AA=" +
            (result720.perceptualSimilarity * 100f).ToString("F2") + "% | " +
            result1080.reportPath);
        if (result1080.perceptualSimilarity < 0.95f ||
            result720.perceptualSimilarity < 0.95f)
            throw new InvalidOperationException(
                "Contención no alcanzó el gate perceptual AA mínimo de 95 %.");
    }

    [MenuItem("Quantum Forge/UI Production/Comparar Pacto Mayor Civilización 2")]
    public static void RunDimension2ResistanceMajorPact()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        string reference = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/" +
            "DIMENSION_2/CORREGIDAS_V4_2026-08-21/" +
            "15_Pacto_Mayor_Civilizacion2_Corregido_V4.png");
        string captures = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/" +
            "DIMENSION_2/PACTO_MAYOR_CIV2/CAPTURAS_CANDIDATAS");
        ComparisonResult result1080 = CompareFiles(reference,
            Path.Combine(captures, "D2_PactoMayorCiv2_1080x1920.png"),
            Path.Combine(root,
                "Logs/UIProductionSystem/Comparisons/Dimension2/ResistanceMajorPact/1080x1920"));
        ComparisonResult result720 = CompareFiles(reference,
            Path.Combine(captures, "D2_PactoMayorCiv2_720x1280.png"),
            Path.Combine(root,
                "Logs/UIProductionSystem/Comparisons/Dimension2/ResistanceMajorPact/720x1280"));
        Debug.Log("[D2 Resistance Major Pact] COMPARISON_COMPLETE | 1080 percibida AA=" +
            (result1080.perceptualSimilarity * 100f).ToString("F2") +
            "% | 720 percibida AA=" +
            (result720.perceptualSimilarity * 100f).ToString("F2") + "% | " +
            result1080.reportPath);
        if (result1080.perceptualSimilarity < 0.95f ||
            result720.perceptualSimilarity < 0.95f)
            throw new InvalidOperationException(
                "Pacto Mayor de Civilización 2 no alcanzó el gate perceptual AA mínimo de 95 %.");
    }

    [MenuItem("Quantum Forge/UI Production/Comparar Ruinas Sepultadas")]
    public static void RunDimension2RuinsArchaeology()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        string reference = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/" +
            "DIMENSION_2/CORREGIDAS_V4_2026-08-21/" +
            "16_Ruinas_Sepultadas_Arqueologia_Corregida_V4.png");
        string captures = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/" +
            "DIMENSION_2/RUINAS_SEPULTADAS/CAPTURAS_CANDIDATAS");
        ComparisonResult result1080 = CompareFiles(reference,
            Path.Combine(captures, "D2_RuinasSepultadas_1080x1920.png"),
            Path.Combine(root,
                "Logs/UIProductionSystem/Comparisons/Dimension2/RuinasSepultadas/1080x1920"));
        ComparisonResult result720 = CompareFiles(reference,
            Path.Combine(captures, "D2_RuinasSepultadas_720x1280.png"),
            Path.Combine(root,
                "Logs/UIProductionSystem/Comparisons/Dimension2/RuinasSepultadas/720x1280"));
        Debug.Log("[D2 Ruins Archaeology] COMPARISON_COMPLETE | 1080 percibida AA=" +
            (result1080.perceptualSimilarity * 100f).ToString("F2") +
            "% | 720 percibida AA=" +
            (result720.perceptualSimilarity * 100f).ToString("F2") + "% | " +
            result1080.reportPath);
        if (result1080.perceptualSimilarity < 0.95f ||
            result720.perceptualSimilarity < 0.95f)
            throw new InvalidOperationException(
                "Ruinas Sepultadas no alcanzó el gate perceptual AA mínimo de 95 %.");
    }

    [MenuItem("Quantum Forge/UI Production/Comparar Analizar Restos")]
    public static void RunDimension2AnalyzeRemains()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        string reference = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/" +
            "DIMENSION_2/CORREGIDAS_V4_2026-08-21/" +
            "17_Analizar_Restos_Corregido_V4.png");
        string captures = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/" +
            "DIMENSION_2/ANALIZAR_RESTOS/CAPTURAS_CANDIDATAS");
        ComparisonResult result1080 = CompareFiles(reference,
            Path.Combine(captures, "D2_AnalizarRestos_1080x1920.png"),
            Path.Combine(root,
                "Logs/UIProductionSystem/Comparisons/Dimension2/AnalizarRestos/1080x1920"));
        ComparisonResult result720 = CompareFiles(reference,
            Path.Combine(captures, "D2_AnalizarRestos_720x1280.png"),
            Path.Combine(root,
                "Logs/UIProductionSystem/Comparisons/Dimension2/AnalizarRestos/720x1280"));
        Debug.Log("[D2 Analyze Remains] COMPARISON_COMPLETE | 1080 percibida AA=" +
            (result1080.perceptualSimilarity * 100f).ToString("F2") +
            "% | 720 percibida AA=" +
            (result720.perceptualSimilarity * 100f).ToString("F2") + "% | " +
            result1080.reportPath);
        if (result1080.perceptualSimilarity < 0.95f ||
            result720.perceptualSimilarity < 0.95f)
            throw new InvalidOperationException(
                "Analizar Restos no alcanzó el gate perceptual AA mínimo de 95 %.");
    }

    [MenuItem("Quantum Forge/UI Production/Comparar Archivo Mejoras Permanentes")]
    public static void RunDimension2Archive()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        string reference = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/" +
            "DIMENSION_2/CORREGIDAS_V4_2026-08-21/" +
            "18_Archivo_Mejoras_Permanentes_Corregido_V4.png");
        string captures = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/" +
            "DIMENSION_2/ARCHIVO_MEJORAS_PERMANENTES/CAPTURAS_CANDIDATAS");
        ComparisonResult result1080 = CompareFiles(reference,
            Path.Combine(captures, "D2_ArchivoMejoras_1080x1920.png"),
            Path.Combine(root,
                "Logs/UIProductionSystem/Comparisons/Dimension2/ArchivoMejoras/1080x1920"));
        ComparisonResult result720 = CompareFiles(reference,
            Path.Combine(captures, "D2_ArchivoMejoras_720x1280.png"),
            Path.Combine(root,
                "Logs/UIProductionSystem/Comparisons/Dimension2/ArchivoMejoras/720x1280"));
        Debug.Log("[D2 Archive] COMPARISON_COMPLETE | 1080 percibida AA=" +
            (result1080.perceptualSimilarity * 100f).ToString("F2") +
            "% | 720 percibida AA=" +
            (result720.perceptualSimilarity * 100f).ToString("F2") + "% | " +
            result1080.reportPath);
        if (result1080.perceptualSimilarity < 0.95f ||
            result720.perceptualSimilarity < 0.95f)
            throw new InvalidOperationException(
                "Archivo — Mejoras Permanentes no alcanzó el gate perceptual AA mínimo de 95 %.");
    }

    [MenuItem("Quantum Forge/UI Production/Comparar Investigación del Ente")]
    public static void RunDimension2EntityResearch()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        string reference = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/" +
            "DIMENSION_2/CORREGIDAS_V4_2026-08-21/" +
            "19_Investigacion_Del_Ente_Corregida_V5.png");
        string captures = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/" +
            "DIMENSION_2/INVESTIGACION_DEL_ENTE/CAPTURAS_CANDIDATAS");
        ComparisonResult result1080 = CompareFiles(reference,
            Path.Combine(captures, "D2_InvestigacionDelEnte_1080x1920.png"),
            Path.Combine(root,
                "Logs/UIProductionSystem/Comparisons/Dimension2/EntityResearch/1080x1920"));
        ComparisonResult result720 = CompareFiles(reference,
            Path.Combine(captures, "D2_InvestigacionDelEnte_720x1280.png"),
            Path.Combine(root,
                "Logs/UIProductionSystem/Comparisons/Dimension2/EntityResearch/720x1280"));
        Debug.Log("[D2 Entity Research] COMPARISON_COMPLETE | 1080 percibida AA=" +
            (result1080.perceptualSimilarity * 100f).ToString("F2") +
            "% | 720 percibida AA=" +
            (result720.perceptualSimilarity * 100f).ToString("F2") + "% | " +
            result1080.reportPath);
        if (result1080.perceptualSimilarity < 0.95f ||
            result720.perceptualSimilarity < 0.95f)
            throw new InvalidOperationException(
                "Investigación del Ente no alcanzó el gate perceptual AA mínimo de 95 %.");
    }

    [MenuItem("Quantum Forge/UI Production/Comparar Pacto con el Ente")]
    public static void RunDimension2EntityPact()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        string reference = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/05_REFERENCIAS/" +
            "DIMENSION_2/CORREGIDAS_V4_2026-08-21/" +
            "20_Pacto_Opcional_Con_El_Ente_Corregido_V5.png");
        string captures = Path.Combine(root,
            "SISTEMA_GENERAL_PRODUCCION_UI/09_PROYECTOS/QUANTUM_FORGE/10_PANTALLAS/" +
            "DIMENSION_2/PACTO_CON_EL_ENTE/CAPTURAS_CANDIDATAS");
        ComparisonResult result1080 = CompareFiles(reference,
            Path.Combine(captures, "D2_PactoConElEnte_1080x1920.png"),
            Path.Combine(root,
                "Logs/UIProductionSystem/Comparisons/Dimension2/EntityPact/1080x1920"));
        ComparisonResult result720 = CompareFiles(reference,
            Path.Combine(captures, "D2_PactoConElEnte_720x1280.png"),
            Path.Combine(root,
                "Logs/UIProductionSystem/Comparisons/Dimension2/EntityPact/720x1280"));
        Debug.Log("[D2 Entity Pact] COMPARISON_COMPLETE | 1080 percibida AA=" +
            (result1080.perceptualSimilarity * 100f).ToString("F2") +
            "% | 720 percibida AA=" +
            (result720.perceptualSimilarity * 100f).ToString("F2") + "% | " +
            result1080.reportPath);
        if (result1080.perceptualSimilarity < 0.95f ||
            result720.perceptualSimilarity < 0.95f)
            throw new InvalidOperationException(
                "Pacto con el Ente no alcanzó el gate perceptual AA mínimo de 95 %.");
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
            float perceptualSimilarity = ComputeAntialiasNormalizedSimilarity(
                referencePixels, capturePixels, capture.width, capture.height);

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
                perceptualSimilarity,
                matchingRatio,
                overlayPath,
                heatmapPath), new UTF8Encoding(false));

            AssetDatabase.Refresh();
            return new ComparisonResult
            {
                normalizedSimilarity = similarity,
                perceptualSimilarity = perceptualSimilarity,
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

    private static float ComputeAntialiasNormalizedSimilarity(
        Color32[] referencePixels, Color32[] capturePixels, int width, int height)
    {
        double absoluteError = 0d;
        int pixelCount = width * height;
        for (int y = 0; y < height; y++)
        {
            int minY = Mathf.Max(0, y - 1);
            int maxY = Mathf.Min(height - 1, y + 1);
            for (int x = 0; x < width; x++)
            {
                int minX = Mathf.Max(0, x - 1);
                int maxX = Mathf.Min(width - 1, x + 1);
                int referenceR = 0, referenceG = 0, referenceB = 0;
                int captureR = 0, captureG = 0, captureB = 0;
                int samples = 0;
                for (int sampleY = minY; sampleY <= maxY; sampleY++)
                {
                    int row = sampleY * width;
                    for (int sampleX = minX; sampleX <= maxX; sampleX++)
                    {
                        int index = row + sampleX;
                        Color32 a = referencePixels[index];
                        Color32 b = capturePixels[index];
                        referenceR += a.r; referenceG += a.g; referenceB += a.b;
                        captureR += b.r; captureG += b.g; captureB += b.b;
                        samples++;
                    }
                }
                absoluteError +=
                    (Mathf.Abs(referenceR - captureR) +
                     Mathf.Abs(referenceG - captureG) +
                     Mathf.Abs(referenceB - captureB)) /
                    (samples * 3d * 255d);
            }
        }
        return Mathf.Clamp01(1f - (float)(absoluteError / pixelCount));
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
        float perceptualSimilarity,
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
        builder.AppendLine("Similitud percibida normalizada por antialiasing 3x3: " +
            (perceptualSimilarity * 100f).ToString("F2") + "%");
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
        public float perceptualSimilarity;
        public float matchingPixelRatio;
        public string reportPath;
        public string overlayPath;
        public string heatmapPath;
    }
}
#endif
