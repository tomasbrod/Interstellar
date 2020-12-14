﻿using KIT.ResourceScheduler;
using KSP.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using KSP.UI.Screens;
using UnityEngine;
using KIT.Resources;

namespace KIT
{

    public class ResourceUI
    {
        protected const int LABEL_WIDTH = 240;
        protected const int VALUE_WIDTH = 55;
        protected const int PRIORITY_WIDTH = 30;
        protected const int OVERVIEW_WIDTH = 65;

        protected const int MAX_PRIORITY = 6;
        private const int POWER_HISTORY_LEN = 10;

        protected readonly string resourceName;

        internal bool renderWindow;
        protected GUIStyle left_bold_label;
        protected GUIStyle right_bold_label;
        protected GUIStyle green_label;
        protected GUIStyle red_label;
        protected GUIStyle left_aligned_label;
        protected GUIStyle right_aligned_label;
        private readonly int windowID;

        private string windowTitle;

        public Rect WindowPosition { get; protected set; }

        private static Font mainFont;
        public static Font MainFont()
        {
            if (mainFont == null)
                mainFont = Font.CreateDynamicFontFromOSFont("Arial", 11);

            return mainFont;
        }

        public ResourceUI(string resource_name, int windowID, float x, float y)
        {
            
            resourceName = resource_name;
            renderWindow = false;

            windowTitle = $"{resourceName} {Localizer.Format("#LOC_KSPIE_ResourceManager_title")}"; //Management Display
            WindowPosition = new Rect(x, y, LABEL_WIDTH + VALUE_WIDTH + PRIORITY_WIDTH, 50);
            this.windowID = windowID;

            left_bold_label = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold,
                font = MainFont()
            };

            right_bold_label = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold,
                font = MainFont(),
                alignment = TextAnchor.MiddleRight
            };

            green_label = new GUIStyle(GUI.skin.label)
            {
                normal = { textColor = resourceName == KITResourceSettings.WasteHeat ? Color.red : Color.green },
                font = MainFont(),
                alignment = TextAnchor.MiddleRight
            };

            red_label = new GUIStyle(GUI.skin.label)
            {
                normal = { textColor = resourceName == KITResourceSettings.WasteHeat ? Color.green : Color.red },
                font = MainFont(),
                alignment = TextAnchor.MiddleRight
            };

            left_aligned_label = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Normal,
                font = MainFont()
            };

            right_aligned_label = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Normal,
                font = MainFont(),
                alignment = TextAnchor.MiddleRight,
            };
        }

        public static string GetFormattedPowerString(double power)
        {
            var absPower = Math.Abs(power);
            string suffix;

            if (absPower >= 1e6)
            {
                suffix = " TW";
                absPower *= 1e-6;
                power *= 1e-6;
            }
            else if (absPower >= 1000)
            {
                suffix = " GW";
                absPower *= 1e-3;
                power *= 1e-3;
            }
            else if (absPower >= 1)
            {
                suffix = " MW";
            }
            else if (absPower >= 0.001)
            {
                suffix = " KW";
                absPower *= 1e3;
                power *= 1e3;
            }
            else
                return (power * 1e6).ToString("0") + " W";
            if (absPower > 100.0)
                return power.ToString("0") + suffix;
            else if (absPower > 10.0)
                return power.ToString("0.0") + suffix;
            else
                return power.ToString("0.00") + suffix;
        }


        protected void DoWindow()
        {
         
            List <DialogGUIBase> producers = new List<DialogGUIBase>();
            foreach(var x in resourceProducers)
            {
                producers.Add(new DialogGUILabel(x));
            }

            List<DialogGUIBase> consumers = new List<DialogGUIBase>();
            foreach (var x in resourceConsumers)
            {
                consumers.Add(new DialogGUILabel(x));
            }

            List<DialogGUIBase> dialog = new List<DialogGUIBase>();
            dialog.Add(new DialogGUILabel($"{resourceName} Producers", true, true));
            dialog.Add(new DialogGUIHorizontalLayout(producers.ToArray()));
            dialog.Add(new DialogGUILabel($"{resourceName} Consumers", true, true));
            dialog.Add(new DialogGUIHorizontalLayout(consumers.ToArray()));
            dialog.Add(new DialogGUIButton("Close", () => { }, 140f, 30f, true));

            Rect pos = new Rect(0.5f, 0.5f, 550, 550);
            PopupDialog.SpawnPopupDialog(/*new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), */new MultiOptionDialog(
                "ThisIsMyName",
                "Quick summary, are we good?",
                $"{resourceName} Resource Manager",
                UISkinManager.defaultSkin,
                pos,
                dialog.ToArray()), false, UISkinManager.defaultSkin);

            // if (renderWindow && GUI.Button(new Rect(WindowPosition.width - 20, 2, 18, 18), "x"))
            //    renderWindow = false;

        }

        double supplyInTheory;
        double currentSupply;
        double currentDemand;
        string[] resourceProducers;
        string[] resourceConsumers;

        ulong previousStepCount;

        public void DisplayData(
            ulong stepCount,
            Dictionary<IKITMod, PerPartResourceInformation> modConsumption,
            Dictionary<IKITMod, PerPartResourceInformation> modProduction,
            IResourceProduction resourceProduction
        )
        {

            if (stepCount == previousStepCount) return;

            currentSupply = resourceProduction.CurrentSupplied();
            currentDemand = resourceProduction.CurrentlyRequested();
            resourceProducers = new string[modProduction.Count];
            resourceConsumers = new string[modConsumption.Count];

            foreach (var key in modProduction.Keys)
            {
                var data = modProduction[key];
                resourceProducers.Append($"{key.KITPartName()} amount {data.amount}, maxAmount {data.maxAmount}");
            }

            foreach (var key in modConsumption.Keys)
            {
                var data = modConsumption[key];
                resourceConsumers.Append($"{key.KITPartName()} amount {data.amount}, maxAmount {data.maxAmount}");
            }

            previousStepCount = stepCount;

            //if(stepCount++ % 25 == 0)
            //    Debug.Log($"{resourceName}, current Supply: {currentSupply}, current Demand: {currentDemand}\nProducers:\n{String.Join("\n", resourceProducers)}\nConsumers:\n{String.Join("\n", resourceConsumers)}\n");


        }

        private List<DialogGUIBase> electricChargeProducers = new List<DialogGUIBase>();
        private List<DialogGUIBase> electricChargeConsumers = new List<DialogGUIBase>();

        private List<DialogGUIBase> thermalPowerProducers = new List<DialogGUIBase>();
        private List<DialogGUIBase> thermalPowerConsumers = new List<DialogGUIBase>();

        private List<DialogGUIBase> chargedParticleProducers = new List<DialogGUIBase>();
        private List<DialogGUIBase> chargedParticleConsumers = new List<DialogGUIBase>();

        private List<DialogGUIBase> wasteHeatProducers = new List<DialogGUIBase>();
        private List<DialogGUIBase> wasteHeatConsumers = new List<DialogGUIBase>();

        private KITResourceVesselModule vesselResourceManager;

        public DialogGUIVerticalLayout electricChargeProducersLayout = new DialogGUIVerticalLayout();
        public DialogGUIVerticalLayout electricChargeConsumersLayout = new DialogGUIVerticalLayout();
        public DialogGUIVerticalLayout thermalPowerProducersLayout = new DialogGUIVerticalLayout();
        public DialogGUIVerticalLayout thermalPowerConsumersLayout = new DialogGUIVerticalLayout();
        public DialogGUIVerticalLayout chargedParticleProducersLayout = new DialogGUIVerticalLayout();
        public DialogGUIVerticalLayout chargedParticleConsumersLayout = new DialogGUIVerticalLayout();
        public DialogGUIVerticalLayout wasteHeatProducersLayout = new DialogGUIVerticalLayout();
        public DialogGUIVerticalLayout wasteHeatConsumersLayout = new DialogGUIVerticalLayout();


        public ResourceUI(KITResourceVesselModule vesselResourceManager)
        {
            this.vesselResourceManager = vesselResourceManager;
            electricChargeProducersLayout.OnFixedUpdate = UpdateLists;
        }

        public void UpdateLists()
        {
            electricChargeProducers.Clear(); electricChargeConsumers.Clear(); thermalPowerProducers.Clear(); thermalPowerConsumers.Clear();
            chargedParticleProducers.Clear(); chargedParticleConsumers.Clear(); wasteHeatProducers.Clear(); wasteHeatConsumers.Clear();

            if (vesselResourceManager.resourceManager.ModProduction.TryGetValue(ResourceName.ElectricCharge, out var resourceList))
            {
                foreach (var key in resourceList.Keys)
                {
                    if(resourceList.TryGetValue(key, out var ppri))
                    {
                        electricChargeProducers.Add(new DialogGUILabel($"{key.KITPartName()} -> amount: {ppri.amount} max: {ppri.maxAmount}"));
                    }
                }
            }

            if (vesselResourceManager.resourceManager.ModConsumption.TryGetValue(ResourceName.ElectricCharge, out resourceList))
            {
                foreach (var key in resourceList.Keys)
                {
                    if (resourceList.TryGetValue(key, out var ppri))
                    {
                        electricChargeConsumers.Add(new DialogGUILabel($"{key.KITPartName()} -> amount: {ppri.amount} max: {ppri.maxAmount}"));
                    }
                }
            }
        } 



        public static PopupDialog CreateDialog(string vesselName, KITResourceVesselModule vesselResourceManager)
        {
            var resourceUI = new ResourceUI(vesselResourceManager);

            List<DialogGUIBase> loremIpsum = new List<DialogGUIBase>();
            loremIpsum.Add(new DialogGUILabel("Lorem ipsum dolor sit amet"));
            loremIpsum.Add(new DialogGUILabel("consectetur adipiscing elit"));
            loremIpsum.Add(new DialogGUILabel("sed do eiusmod tempor incididunt"));
            loremIpsum.Add(new DialogGUILabel("ut labore et dolore magna aliqua"));
            loremIpsum.Add(new DialogGUILabel("Ut enim ad minim veniam"));
            loremIpsum.Add(new DialogGUILabel("quis nostrud exercitation ullamco"));
            loremIpsum.Add(new DialogGUILabel($"¯\\_(ツ)_/¯"));

            List<DialogGUIBase> dialog = new List<DialogGUIBase>();
            dialog.Add(new DialogGUILabel($"<b>{KITResourceSettings.ElectricCharge} Producers</b>", true, true));
            dialog.Add(new DialogGUIScrollList(new Vector2(200, 200), false, true, resourceUI.electricChargeProducersLayout));

            dialog.Add(new DialogGUILabel($"<b>{KITResourceSettings.ElectricCharge} Consumers</b>", true, true));
            dialog.Add(new DialogGUIScrollList(new Vector2(200, 200), false, true, resourceUI.electricChargeConsumersLayout));

            dialog.Add(new DialogGUILabel($"<b>{KITResourceSettings.ThermalPower} Producers</b>", true, true));
            dialog.Add(new DialogGUIScrollList(new Vector2(200, 50), false, true, resourceUI.thermalPowerProducersLayout));

            dialog.Add(new DialogGUILabel($"<b>{KITResourceSettings.ThermalPower} Consumers</b>", true, true));
            dialog.Add(new DialogGUIScrollList(new Vector2(200, 50), false, true, resourceUI.thermalPowerConsumersLayout));

            dialog.Add(new DialogGUILabel($"<b>{KITResourceSettings.ChargedParticle} Producers</b>", true, true));
            dialog.Add(new DialogGUIScrollList(new Vector2(200, 50), false, true, resourceUI.chargedParticleProducersLayout));

            dialog.Add(new DialogGUILabel($"<b>{KITResourceSettings.ChargedParticle} Consumers</b>", true, true));
            dialog.Add(new DialogGUIScrollList(new Vector2(200, 50), false, true, resourceUI.chargedParticleConsumersLayout));

            dialog.Add(new DialogGUILabel($"<b>{KITResourceSettings.WasteHeat} Producers</b>", true, true));
            dialog.Add(new DialogGUIScrollList(new Vector2(200, 50), false, true, resourceUI.wasteHeatProducersLayout));

            dialog.Add(new DialogGUILabel($"<b>{KITResourceSettings.WasteHeat} Consumers</b>", true, true));
            dialog.Add(new DialogGUIScrollList(new Vector2(200, 50), false, true, resourceUI.wasteHeatProducersLayout));

            dialog.Add(new DialogGUIButton("Close", () => { }, 140f, 30f, true));

            List<DialogGUIBase> virtLayout = new List<DialogGUIBase>();
            virtLayout.Add(new DialogGUIHorizontalLayout(dialog.ToArray()));

            Rect pos = new Rect(0.5f, 0.5f, 750, 750);
            return PopupDialog.SpawnPopupDialog(/*new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), */new MultiOptionDialog(
                "ThisIsMyName",
                "Quick summary, are we good?",
                $"{vesselName} Resource Manager",
                UISkinManager.defaultSkin,
                pos,
                virtLayout.ToArray()), false, UISkinManager.defaultSkin);
        }

    }
    // startup once during flight
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class ResourceManagerFlightUI : MonoBehaviour
    {
        public static bool close_window = false;
        public static bool show_window = false;

        PopupDialog dialog;

        public void Update()
        {
            Vessel vessel = FlightGlobals.ActiveVessel;
            if (vessel == null || vessel.vesselModules == null || (show_window == false && close_window == false)) {
                // is this needed? show_window = close_window = false;
                return;
            }

            if (show_window)
            {
                if(dialog == null)
                {
                    var vrm = FindVesselResourceManager(vessel);
                    dialog = ResourceUI.CreateDialog(vessel.vesselName, vrm);
                    dialog.OnDismiss = dismissDialog;
                }
                show_window = false;
                return;
            }

            // otherwise, close_window is true

            dismissDialog();
            close_window = false;
            return;
        }

        private void dismissDialog()
        {
            if (dialog == null) return;

            dialog.Dismiss();
            dialog = null;
        }

        private KITResourceVesselModule FindVesselResourceManager(Vessel vessel)
        {
            KITResourceVesselModule vesselResourceManager = null;

            for (int i = 0; i < vessel.vesselModules.Count; i++)
            {
                if (vessel.vesselModules[i] == null) continue;
                vesselResourceManager = vessel.vesselModules[i] as KITResourceVesselModule;

                if (vesselResourceManager != null) break;
            }
            return vesselResourceManager == null ? null : vesselResourceManager.resourceManager == null ? null : vesselResourceManager;
        }
    }
}
