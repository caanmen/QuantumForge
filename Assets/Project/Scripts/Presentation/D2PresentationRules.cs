using System;


public static class D2PresentationRules
{
    public static void EnsurePresentationState(GameState gameState)
    {
        if (gameState == null || gameState.dimension2 == null)
            return;

        Dimension2State d2 = gameState.dimension2;
        bool requiresMigration = d2.presentation == null ||
            d2.presentation.presentationVersion < DimensionPresentationState.CurrentVersion;
        if (d2.presentation == null)
            d2.presentation = new DimensionPresentationState();

        PresentationStateUtility.Normalize(d2.presentation);
        if (requiresMigration)
            InferIntroducedFeatures(d2);
        IntroduceCivilization1Features(gameState);
        IntroduceCivilization2Features(gameState);
        IntroduceCivilization3Features(gameState);
        IntroduceActiveFeatures(d2);
        d2.presentation.presentationVersion = DimensionPresentationState.CurrentVersion;
        PresentationStateUtility.Normalize(d2.presentation);
    }

    public static FeaturePresentationState GetFeatureState(
        GameState gameState,
        string featureId)
    {
        if (gameState == null || gameState.dimension2 == null ||
            !IsKnownFeatureId(featureId))
        {
            return new FeaturePresentationState(
                featureId, FeaturePresentationVisualState.Hidden, false);
        }

        Dimension2State d2 = gameState.dimension2;
        DimensionPresentationState presentation = d2.presentation;
        FeaturePresentationVisualState visualState;
        if (IsActive(d2, featureId))
            visualState = FeaturePresentationVisualState.Active;
        else if (IsCompleted(d2, featureId))
            visualState = FeaturePresentationVisualState.Completed;
        else if (IsAvailable(d2, presentation, featureId))
            visualState = FeaturePresentationVisualState.Available;
        else if (IsTeaser(d2, featureId))
            visualState = FeaturePresentationVisualState.Teaser;
        else
            visualState = FeaturePresentationVisualState.Hidden;

        return PresentationStateUtility.Build(presentation, featureId, visualState);
    }

    public static bool IsKnownFeatureId(string featureId)
    {
        for (int i = 0; i < PresentationFeatureIds.D2All.Length; i++)
        {
            if (string.Equals(
                    PresentationFeatureIds.D2All[i], featureId,
                    StringComparison.Ordinal))
                return true;
        }
        return false;
    }

    public static bool IsActive(Dimension2State d2, string featureId)
    {
        if (d2 == null)
            return false;

        D2Civilization1State c1 = d2.civilization1;
        D2Civilization2State c2 = d2.civilization2;
        D2Civilization3State c3 = d2.civilization3;
        switch (featureId)
        {
            case PresentationFeatureIds.D2C1Pilgrimages:
                return c1 != null && c1.activePilgrimage != null &&
                    c1.activePilgrimage.active;
            case PresentationFeatureIds.D2C1Novitiate:
                return c1 != null && c1.activeNovitiateTraining != null &&
                    c1.activeNovitiateTraining.active;
            case PresentationFeatureIds.D2C1Rites:
                return c1 != null && D2RiteSystem.GetActiveRiteCount(c1) > 0;
            case PresentationFeatureIds.D2C1Pacts:
                return c1 != null && HasActiveCivilizationPact(c1);
            case PresentationFeatureIds.D2C2Operations:
                return c2 != null && HasActiveOperation(c2);
            case PresentationFeatureIds.D2C2Resistance:
                return c2 != null && HasActiveResistancePact(c2);
            case PresentationFeatureIds.D2C2Alert:
                return c2 != null && c2.alertActive && !c2.entityContained;
            case PresentationFeatureIds.D2C2Containment:
                return c2 != null && (c2.membersAssignedToContainment > 0L ||
                    (c2.alertActive && c2.containmentAvailable && !c2.entityContained));
            case PresentationFeatureIds.D2C3Archaeology:
                return c3 != null && HasActiveExcavation(c3);
            case PresentationFeatureIds.D2C3Analysis:
                return c3 != null && HasActiveAnalysis(c3);
            case PresentationFeatureIds.D2C3EntityResearch:
                return c3 != null && c3.entityResearchActive;
            default:
                return false;
        }
    }

    private static void InferIntroducedFeatures(Dimension2State d2)
    {
        DimensionPresentationState presentation = d2.presentation;
        Introduce(presentation, PresentationFeatureIds.D2Map);

        D2Civilization1State c1 = d2.civilization1;
        if (c1 != null)
        {
            Introduce(presentation, PresentationFeatureIds.D2C1Refuge);
            if (HasAltarProgress(c1)) Introduce(presentation, PresentationFeatureIds.D2C1Altars);
            if (c1.totalPilgrimagesCompleted > 0L || c1.trust > 0.0 ||
                (c1.activePilgrimage != null && c1.activePilgrimage.active))
                Introduce(presentation, PresentationFeatureIds.D2C1Pilgrimages);
            if (c1.totalAcolytesCreated > 0L || c1.acolytesAvailable > 0L ||
                c1.novitiateLevel > D2NovitiateSystem.MinLevel ||
                c1.novitiateBatchesCompleted > 0L ||
                (c1.activeNovitiateTraining != null && c1.activeNovitiateTraining.active))
                Introduce(presentation, PresentationFeatureIds.D2C1Novitiate);
            if (D2RiteSystem.GetActiveRiteCount(c1) > 0 || c1.totalAcolytesCreated > 0L)
                Introduce(presentation, PresentationFeatureIds.D2C1Rites);
            if (D2CivilizationPactSystem.ArePactsUnlocked(c1) ||
                HasCivilizationPactProgress(c1))
                Introduce(presentation, PresentationFeatureIds.D2C1Pacts);
            if (D2VeiledThresholdSystem.IsUnlocked(c1) ||
                c1.entityContactAvailable || c1.bondPlacePrepared)
                Introduce(presentation, PresentationFeatureIds.D2C1VeiledThreshold);
        }

        D2Civilization2State c2 = d2.civilization2;
        if (d2.civilization2Unlocked)
        {
            Introduce(presentation, PresentationFeatureIds.D2C2Regions);
            if (c2 != null &&
                D2Civilization2PresentationRules.HasRegionalAssignment(c2))
                Introduce(presentation, PresentationFeatureIds.D2C2Operations);
        }
        if (c2 != null)
        {
            if (HasDefenseProgress(c2)) Introduce(presentation, PresentationFeatureIds.D2C2Defense);
            if (c2.controlFragments > 0L || c2.totalReprisals > 0L ||
                HasResistanceUpgradeProgress(c2))
                Introduce(presentation, PresentationFeatureIds.D2C2Resistance);
            if (c2.alertActive || c2.totalAlertMarks > 0L)
                Introduce(presentation, PresentationFeatureIds.D2C2Alert);
            if (c2.containmentAvailable || c2.entityContained || c2.totalContainmentAttempts > 0L)
                Introduce(presentation, PresentationFeatureIds.D2C2Containment);
            if (c2.majorPactPrepared || c2.majorPactEstablished)
                Introduce(presentation, PresentationFeatureIds.D2C2MajorPact);
        }

        D2Civilization3State c3 = d2.civilization3;
        if (d2.civilization3Unlocked)
            Introduce(presentation, PresentationFeatureIds.D2C3Archaeology);
        if (c3 != null)
        {
            if (HasAnalysisProgress(c3)) Introduce(presentation, PresentationFeatureIds.D2C3Analysis);
            if (c3.archiveUnlocked || c3.archiveLevel > 0)
                Introduce(presentation, PresentationFeatureIds.D2C3Archive);
            if (HasClueProgress(c3)) Introduce(presentation, PresentationFeatureIds.D2C3Clues);
            if (HasAnomalyProgress(c3)) Introduce(presentation, PresentationFeatureIds.D2C3Anomalies);
            if (c3.entityResearchUnlocked || c3.entityResearchActive ||
                c3.entityResearchProgress > 0.0)
                Introduce(presentation, PresentationFeatureIds.D2C3EntityResearch);
            if (c3.entityPactAvailable || c3.entityPactEstablished)
                Introduce(presentation, PresentationFeatureIds.D2C3EntityPact);
        }

        presentation.onboardingStage = Math.Max(
            presentation.onboardingStage,
            d2.firstEntrySeen ? 1 : 0);
    }

    private static void IntroduceActiveFeatures(Dimension2State d2)
    {
        for (int i = 0; i < PresentationFeatureIds.D2All.Length; i++)
        {
            string featureId = PresentationFeatureIds.D2All[i];
            if (IsActive(d2, featureId))
                Introduce(d2.presentation, featureId);
        }
    }

    private static void IntroduceCivilization1Features(GameState gameState)
    {
        D2Civilization1State c1 = gameState.dimension2.civilization1;
        if (c1 == null) return;
        if (D2Civilization1PresentationRules.CanIntroduceAltars(c1))
            Introduce(gameState.dimension2.presentation,
                PresentationFeatureIds.D2C1Altars);
        if (D2Civilization1PresentationRules.CanIntroducePilgrimages(c1))
            Introduce(gameState.dimension2.presentation,
                PresentationFeatureIds.D2C1Pilgrimages);
        if (D2Civilization1PresentationRules.CanIntroduceNovitiate(gameState))
            Introduce(gameState.dimension2.presentation,
                PresentationFeatureIds.D2C1Novitiate);
        if (D2Civilization1PresentationRules.CanIntroduceRites(c1))
            Introduce(gameState.dimension2.presentation,
                PresentationFeatureIds.D2C1Rites);
        if (D2Civilization1PresentationRules.CanIntroducePacts(c1))
            Introduce(gameState.dimension2.presentation,
                PresentationFeatureIds.D2C1Pacts);
        if (D2VeiledThresholdSystem.IsUnlocked(c1))
            Introduce(gameState.dimension2.presentation,
                PresentationFeatureIds.D2C1VeiledThreshold);
    }

    private static void IntroduceCivilization2Features(GameState gameState)
    {
        Dimension2State d2 = gameState.dimension2;
        D2Civilization2State c2 = d2.civilization2;
        if (c2 == null || !d2.civilization2Unlocked) return;
        Introduce(d2.presentation, PresentationFeatureIds.D2C2Regions);
        if (D2Civilization2PresentationRules.HasRegionalAssignment(c2))
            Introduce(d2.presentation, PresentationFeatureIds.D2C2Operations);
        if (D2Civilization2PresentationRules.HasThreat(c2))
            Introduce(d2.presentation, PresentationFeatureIds.D2C2Defense);
        if (D2Civilization2PresentationRules.CanIntroduceResistance(c2))
            Introduce(d2.presentation, PresentationFeatureIds.D2C2Resistance);
        if (c2.alertActive || c2.totalAlertMarks > 0L)
            Introduce(d2.presentation, PresentationFeatureIds.D2C2Alert);
        if (c2.containmentAvailable || c2.entityContained ||
            c2.totalContainmentAttempts > 0L)
            Introduce(d2.presentation, PresentationFeatureIds.D2C2Containment);
        if (c2.entityContained || c2.majorPactPrepared || c2.majorPactEstablished)
            Introduce(d2.presentation, PresentationFeatureIds.D2C2MajorPact);
    }

    private static void IntroduceCivilization3Features(GameState gameState)
    {
        Dimension2State d2 = gameState.dimension2;
        D2Civilization3State c3 = d2.civilization3;
        if (c3 == null || !d2.civilization3Unlocked) return;
        Introduce(d2.presentation, PresentationFeatureIds.D2C3Archaeology);
        if (D2Civilization3PresentationRules.HasExcavationProgress(c3))
            Introduce(d2.presentation, PresentationFeatureIds.D2C3Analysis);
        if (D2Civilization3PresentationRules.HasAnalysisProgress(c3) ||
            c3.archiveUnlocked)
            Introduce(d2.presentation, PresentationFeatureIds.D2C3Archive);
        if (c3.anomalyClueDetectionUnlocked)
            Introduce(d2.presentation, PresentationFeatureIds.D2C3Clues);
        if (D2Civilization3PresentationRules.HasClueProgress(c3) ||
            D2Civilization3PresentationRules.HasAnomalyProgress(c3))
            Introduce(d2.presentation, PresentationFeatureIds.D2C3Anomalies);
        if (c3.entityResearchUnlocked || c3.entityResearchActive ||
            c3.entityResearchProgress > 0.0 || c3.entityPactAvailable ||
            c3.entityPactEstablished)
            Introduce(d2.presentation, PresentationFeatureIds.D2C3EntityResearch);
        if (c3.entityPactAvailable || c3.entityPactEstablished)
            Introduce(d2.presentation, PresentationFeatureIds.D2C3EntityPact);
    }

    private static bool IsAvailable(
        Dimension2State d2,
        DimensionPresentationState presentation,
        string featureId)
    {
        if (PresentationStateUtility.Contains(
                presentation == null ? null : presentation.introducedFeatureIds,
                featureId))
            return true;

        switch (featureId)
        {
            case PresentationFeatureIds.D2Map:
            case PresentationFeatureIds.D2C1Refuge:
                return true;
            case PresentationFeatureIds.D2C2Regions:
                return d2.civilization2Unlocked;
            case PresentationFeatureIds.D2C3Archaeology:
                return d2.civilization3Unlocked;
            default:
                return false;
        }
    }

    private static bool IsCompleted(Dimension2State d2, string featureId)
    {
        if (d2.civilization2 != null &&
            featureId == PresentationFeatureIds.D2C2Containment &&
            d2.civilization2.entityContained)
            return true;
        if (d2.civilization2 != null &&
            featureId == PresentationFeatureIds.D2C2MajorPact &&
            d2.civilization2.majorPactEstablished)
            return true;
        return d2.civilization3 != null &&
            featureId == PresentationFeatureIds.D2C3EntityPact &&
            d2.civilization3.entityPactEstablished;
    }

    private static bool IsTeaser(Dimension2State d2, string featureId)
    {
        if (featureId == PresentationFeatureIds.D2C1Pacts)
            return D2Civilization1PresentationRules.IsPactsNear(
                d2.civilization1);
        if (featureId == PresentationFeatureIds.D2C2Regions)
            return d2.civilization1 != null && d2.civilization1.trust >= 0.0;
        return false;
    }

    private static void Introduce(DimensionPresentationState state, string id)
    {
        PresentationStateUtility.Introduce(state, id);
    }

    private static bool HasAltarProgress(D2Civilization1State c1)
    {
        if (c1.altars == null) return false;
        for (int i = 0; i < c1.altars.Count; i++)
        {
            D2AltarState altar = c1.altars[i];
            if (altar != null && (altar.followersAssigned > 0L ||
                altar.offeringAmount > 0.0 || altar.totalOfferingProduced > 0.0))
                return true;
        }
        return false;
    }

    private static bool HasCivilizationPactProgress(D2Civilization1State c1)
    {
        if (c1.civilizationPacts == null) return false;
        for (int i = 0; i < c1.civilizationPacts.Count; i++)
        {
            D2CivilizationPactState pact = c1.civilizationPacts[i];
            if (pact != null && (pact.active || pact.suspended)) return true;
        }
        return false;
    }

    private static bool HasActiveCivilizationPact(D2Civilization1State c1)
    {
        if (c1.civilizationPacts == null) return false;
        for (int i = 0; i < c1.civilizationPacts.Count; i++)
        {
            D2CivilizationPactState pact = c1.civilizationPacts[i];
            if (pact != null && pact.active) return true;
        }
        return false;
    }

    private static bool HasActiveOperation(D2Civilization2State c2)
    {
        if (c2.regions == null) return false;
        for (int i = 0; i < c2.regions.Count; i++)
        {
            D2RegionState region = c2.regions[i];
            if (region == null || region.operations == null) continue;
            for (int j = 0; j < region.operations.Count; j++)
            {
                if (D2Civilization2System.IsOperationActive(region.operations[j]))
                    return true;
            }
        }
        return false;
    }

    private static bool HasDefenseProgress(D2Civilization2State c2)
    {
        if (c2.totalReprisals > 0L || c2.alertActive) return true;
        if (c2.regions == null) return false;
        for (int i = 0; i < c2.regions.Count; i++)
        {
            D2RegionState region = c2.regions[i];
            if (region != null && (region.threat > 0.0 || region.totalReprisals > 0L ||
                D2Civilization2System.IsProtectionActive(region))) return true;
        }
        return false;
    }

    private static bool HasResistanceUpgradeProgress(D2Civilization2State c2)
    {
        if (c2.resistanceUpgrades == null) return false;
        for (int i = 0; i < c2.resistanceUpgrades.Count; i++)
        {
            D2ResistanceUpgradeState upgrade = c2.resistanceUpgrades[i];
            if (upgrade != null && upgrade.level > 0) return true;
        }
        return false;
    }

    private static bool HasActiveResistancePact(D2Civilization2State c2)
    {
        if (c2.resistancePacts == null) return false;
        for (int i = 0; i < c2.resistancePacts.Count; i++)
        {
            D2ResistancePactState pact = c2.resistancePacts[i];
            if (pact != null && pact.active) return true;
        }
        return false;
    }

    private static bool HasActiveExcavation(D2Civilization3State c3)
    {
        if (c3.zones == null) return false;
        for (int i = 0; i < c3.zones.Count; i++)
            if (c3.zones[i] != null && c3.zones[i].excavationActive) return true;
        return false;
    }

    private static bool HasActiveAnalysis(D2Civilization3State c3)
    {
        if (c3.zones == null) return false;
        for (int i = 0; i < c3.zones.Count; i++)
            if (c3.zones[i] != null && c3.zones[i].analysisActive) return true;
        return false;
    }

    private static bool HasAnalysisProgress(D2Civilization3State c3)
    {
        if (c3.zones == null) return false;
        for (int i = 0; i < c3.zones.Count; i++)
        {
            D2C3ZoneState zone = c3.zones[i];
            if (zone != null && (zone.analysisActive || zone.totalAnalysesCompleted > 0L ||
                zone.lowQualityRemains > 0L || zone.mediumQualityRemains > 0L ||
                zone.highQualityRemains > 0L)) return true;
        }
        return false;
    }

    private static bool HasClueProgress(D2Civilization3State c3)
    {
        if (c3.zones == null) return false;
        for (int i = 0; i < c3.zones.Count; i++)
            if (c3.zones[i] != null && c3.zones[i].anomalyClues > 0L) return true;
        return false;
    }

    private static bool HasAnomalyProgress(D2Civilization3State c3)
    {
        if (c3.zones == null) return false;
        for (int i = 0; i < c3.zones.Count; i++)
        {
            D2C3ZoneState zone = c3.zones[i];
            if (zone != null && (zone.anomalyRevealed || zone.anomalyRead)) return true;
        }
        return false;
    }
}
