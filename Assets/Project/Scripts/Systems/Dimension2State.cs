using System;
using System.Collections.Generic;


[Serializable]
public class D2AltarState
{
    public string altarId = "";
    public bool unlocked;
    public double offeringAmount;
    public double totalOfferingProduced;
    public long followersAssigned;
}


[Serializable]
public class D2PilgrimageState
{
    public bool active;
    public string pilgrimageId = "";
    public double remainingSeconds;
    public long followersCommitted;
    public long supportFollowersCommitted;
    public long acolytesCommitted;
    public bool preparedMediumFollowerReward;
}


[Serializable]
public class D2NovitiateTrainingState
{
    public bool active;
    public int trainingLevel;
    public double remainingSeconds;
    public long followersCommitted;
    public long supportFollowersCommitted;
    public long acolytesToCreate;
}


[Serializable]
public class D2RiteState
{
    public string riteId = "";
    public long followersAssigned;
    public long acolytesAssigned;
}


[Serializable]
public class D2CivilizationPactState
{
    public string pactId = "";
    public bool active;
    public bool suspended;
}


[Serializable]
public class D2BondLineState
{
    public string lineId = "";
    public int level;
}


[Serializable]
public class D2OperationState
{
    public string operationId = "";
    public long membersAssigned;
}

[Serializable]
public class D2ResistanceUpgradeState { public string upgradeId = ""; public int level; }

[Serializable]
public class D2ResistancePactState
{
    public string pactId = "";
    public bool active;
    public long membersAssigned;
    public double wearProgressSeconds;
}

[Serializable]
public class D2C2MajorPactLineState
{
    public string lineId = "";
    public int level;
}

[Serializable]
public class D2ExhaustedMemberBatch
{
    public long amount;
    public double remainingSeconds;
}


[Serializable]
public class D2RegionState
{
    public string regionId = "";
    public bool unlocked;
    public double dominance = 100.0;
    public double threat;
    public long membersAssigned;
    public List<D2OperationState> operations = new List<D2OperationState>();
    public double coverage;
    public double nextReprisalEspionageReduction;
    public string weakenedOperationId = "";
    public double weakenedOperationRemainingSeconds;
    public long totalReprisals;
    public bool alertMarked;
}


[Serializable]
public class D2Civilization1State
{
    public int progressVersion = Dimension2System.Civilization1ProgressVersion;
    public bool initialFollowersGranted = true;
    public long followersAvailable = D2Civilization1System.InitialFollowers;
    public long followersAssignedToRefuge;
    public long totalFollowersReceived = D2Civilization1System.InitialFollowers;
    public double followerArrivalProgress;
    public int refugeLevel = D2Civilization1System.MinRefugeLevel;
    public List<D2AltarState> altars = new List<D2AltarState>();
    public double trust;
    public bool entityContactAvailable;
    public long totalPilgrimagesCompleted;
    public long shortPilgrimagesCompleted;
    public long mediumPilgrimagesCompleted;
    public long longPilgrimagesCompleted;
    public string lastPilgrimageResult = "";
    public long pilgrimageSupportFollowersSelected;
    public D2PilgrimageState activePilgrimage = new D2PilgrimageState();
    public int novitiateLevel = D2NovitiateSystem.MinLevel;
    public long acolytesAvailable;
    public long totalAcolytesCreated;
    public long novitiateBatchesCompleted;
    public string lastNovitiateResult = "";
    public long novitiateSupportFollowersSelected;
    public D2NovitiateTrainingState activeNovitiateTraining =
        new D2NovitiateTrainingState();
    public List<D2RiteState> rites = new List<D2RiteState>();
    public bool thirdRiteSlotUnlocked;
    public List<D2CivilizationPactState> civilizationPacts =
        new List<D2CivilizationPactState>();
    public bool secondCivilizationPactSlotUnlocked;
    public double pactPilgrimageFollowerRewardProgress;
    public double pactConsecrationAcolyteProgress;
    public string lastCivilizationPactResult = "";
    public bool bondPlacePrepared;
    public long acolytesAssignedToBond;
    public double bondProgress;
    public List<D2BondLineState> bondLines = new List<D2BondLineState>();
    public string lastBondResult = "";
}


[Serializable]
public class D2Civilization2State
{
    public int progressVersion = Dimension2System.Civilization2ProgressVersion;
    public bool initialMembersGranted;
    public long membersAvailable;
    public long totalMembersRecruited;
    public double memberProductionProgress;
    public long controlFragments;
    public long totalReprisals;
    public string selectedRegionId = D2Civilization2System.Region1Id;
    public List<D2RegionState> regions = new List<D2RegionState>();
    public List<D2ResistanceUpgradeState> resistanceUpgrades = new List<D2ResistanceUpgradeState>();
    public List<D2ResistancePactState> resistancePacts = new List<D2ResistancePactState>();
    public List<D2ExhaustedMemberBatch> exhaustedMemberBatches = new List<D2ExhaustedMemberBatch>();
    public double hiddenSheltersPenaltySeconds;
    public double silencedBellsPenaltySeconds;
    public double knivesPenaltySeconds;
    public bool alertActive;
    public bool containmentAvailable;
    public double alertMarkProgressSeconds;
    public long totalAlertMarks;
    public string lastAlertResult = "";
    public bool entityContained;
    public bool majorPactPrepared;
    public bool majorPactEstablished;
    public double containmentStability;
    public List<D2C2MajorPactLineState> majorPactLines =
        new List<D2C2MajorPactLineState>();
    public string lastMajorPactResult = "";
    public double containmentCooldownSeconds;
    public long membersAssignedToContainment;
    public long totalContainmentAttempts;
    public long totalContainmentFailures;
    public string lastContainmentResult = "";
    public string lastResult = "";
}


[Serializable]
public class D2Civilization3State
{
    public int progressVersion = Dimension2System.Civilization3ProgressVersion;
    public string selectedZoneId = D2Civilization3System.Zone1Id;
    public List<D2C3ZoneState> zones = new List<D2C3ZoneState>();
    public double ancientKnowledge;
    public bool archiveUnlocked;
    public int archiveLevel;
    public bool stratifiedCartographyUnlocked;
    public bool anomalousConcordanceUnlocked;
    public bool deepExegesisUnlocked;
    public bool anomalyClueDetectionUnlocked;
    public bool entityResearchUnlocked;
    public bool entityResearchActive;
    public double entityResearchProgress;
    public bool entityResearchMilestone30Completed;
    public bool entityResearchMilestone60Completed;
    public bool entityResearchMilestone85Completed;
    public bool entityResearchMilestone100Completed;
    public long entityKnowledge;
    public bool entityPactAvailable;
    public bool entityPactEstablished;
    public List<D2EntityPactLineState> entityPactLines = new List<D2EntityPactLineState>();
    public string lastEntityPactResult = "";
    public string lastResult = "";
}


[Serializable]
public class D2EntityPactLineState
{
    public string lineId = "";
    public int level;
}


[Serializable]
public class D2C3ZoneState
{
    public string zoneId = "";
    public bool unlocked;
    public bool excavationActive;
    public double excavationRemainingSeconds;
    public long lowQualityRemains;
    public long mediumQualityRemains;
    public long highQualityRemains;
    public long totalExcavationsCompleted;
    public bool scholarHired;
    public int scholarLevel;
    public bool analysisActive;
    public double analysisRemainingSeconds;
    public string analysisQualityId = "";
    public long zoneResourceAmount;
    public double zoneResourceRewardProgress;
    public double researchProgress;
    public long totalAnalysesCompleted;
    public double bonusRemainsProgress;
    public long anomalyClues;
    public double anomalyClueProgress;
    public bool anomalyRevealed;
    public bool anomalyRead;
    public long anomalousData;
}


[Serializable]
public class Dimension2State
{
    public int progressVersion = Dimension2System.ProgressVersion;
    public bool firstEntrySeen;
    public string selectedTerritoryId = Dimension2System.Civilization1TerritoryId;

    public bool civilization1Unlocked = true;
    public bool civilization2Unlocked;
    public bool civilization3Unlocked;

    public D2Civilization1State civilization1 = new D2Civilization1State();
    public D2Civilization2State civilization2 = new D2Civilization2State();
    public D2Civilization3State civilization3 = new D2Civilization3State();
}
