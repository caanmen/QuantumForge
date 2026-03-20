using System;
using System.Collections.Generic;

public enum F2UpgradeCurrency
{
    LE = 0,
    Traces = 1
}

public enum F2UpgradeEffectType
{
    None = 0,
    GlobalLEMult = 1
}

[Serializable]
public class F2UpgradeTierDef
{
    public double cost;
    public double effectValue;
}

[Serializable]
public class F2UpgradeDef
{
    public string id;
    public string name;
    public string description;
    public F2UpgradeCurrency currency;
    public F2UpgradeEffectType effectType;
    public List<F2UpgradeTierDef> tiers;
}

[Serializable]
public class F2UpgradeDefList
{
    public List<F2UpgradeDef> upgrades;
}
