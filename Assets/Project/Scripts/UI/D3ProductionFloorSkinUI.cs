using System;
using TMPro;
using UnityEngine;


public sealed class D3ProductionFloorSkinUI : MonoBehaviour
{
    public TMP_Text leText;
    public TMP_Text tracesText;
    public TMP_Text automatonsText;
    public TMP_Text productionInventoryText;
    public TMP_Text previewTitleText;
    public TMP_Text assemblyCostText;
    public D3AssignmentDisclosureUI assignmentDisclosure;
    public D3DropdownStepperUI productionQuantityStepper;
    public D3DropdownStepperUI assemblyQuantityStepper;

    public void Refresh(GameState state)
    {
        if (state == null)
            return;

        Set(leText, Compact(state.LE) + " LE");
        Set(tracesText, Compact(state.Traces) + " TRAZAS");

        long total = 0L;
        Dimension3State dimension = state.dimension3;
        if (dimension != null && dimension.automatons != null)
        {
            for (int i = 0; i < dimension.automatons.Count; i++)
            {
                D3AutomatonStackState stack = dimension.automatons[i];
                if (stack != null)
                    total += Math.Max(0L, stack.totalAmount);
            }
        }

        Set(automatonsText, Compact(total) + " AUTÓMATAS");
    }

    public void RefreshControls()
    {
        if (assignmentDisclosure != null)
            assignmentDisclosure.RefreshControls();
        if (productionQuantityStepper != null)
            productionQuantityStepper.Refresh();
        if (assemblyQuantityStepper != null)
            assemblyQuantityStepper.Refresh();
    }

    public void RefreshProductionInventory(Dimension3State state, int version)
    {
        if (state == null)
            return;
        Set(productionInventoryText,
            D3InventorySystem.GetPartAmount(
                state, Dimension3Catalog.PartChassis, version).ToString());
    }

    private static void Set(TMP_Text label, string value)
    {
        if (label != null)
            label.text = value;
    }

    private static string Compact(double value)
    {
        double absolute = Math.Abs(value);
        if (absolute >= 1000000000.0)
            return (value / 1000000000.0).ToString("0.##") + "B";
        if (absolute >= 1000000.0)
            return (value / 1000000.0).ToString("0.##") + "M";
        if (absolute >= 1000.0)
            return (value / 1000.0).ToString("0.##") + "K";
        return value.ToString("0");
    }
}
