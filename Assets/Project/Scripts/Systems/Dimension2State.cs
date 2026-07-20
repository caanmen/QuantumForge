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
}


[Serializable]
public class D2Civilization3State
{
    public int progressVersion = Dimension2System.Civilization3ProgressVersion;
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
