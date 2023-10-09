using System.Collections.Generic;

[System.Serializable]
public class ActionUnitList
{
    public List<ActionUnit> ActionUnits = new List<ActionUnit>();
    public Dictionary<string, ActionUnit> AuDict = new Dictionary<string, ActionUnit>();

    public void FillActionUnitListDict()
    {
        foreach (var au in ActionUnits)
        {
            AuDict.Add(au.Name, au);
        }
    }
}