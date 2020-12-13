﻿using System;
using System.Collections.Generic;
using System.Linq;
using KIT.Constants;
using KIT.Extensions;
using KIT.Resources;
using KIT.ResourceScheduler;
using KSP.Localization;
using UnityEngine;
using static System.String;

namespace KIT.Propulsion
{
    [KSPModule("#LOC_KSPIE_AlcubierreDrive_partModuleName")]
    class AlcubierreDrive : PartModule, IKITMod
    {
        public const string GROUP = "AlcubierreDrive";
        public const string GROUP_TITLE = "#LOC_KSPIE_AlcubierreDrive_groupName";

        // Persistent
        [KSPField(isPersistant = true)]
        public bool IsEnabled;
        [KSPField(isPersistant = true)]
        public bool IsCharging;
        [KSPField(isPersistant = true)]
        private double existing_warp_speed;
        [KSPField(isPersistant = true)]
        public int selected_factor = -1;
        [KSPField(isPersistant = true)]
        public bool isupgraded;
        [KSPField(isPersistant = true)]
        public string serialisedwarpvector;

        // non Persistent
        public const string warpEffect1ShaderPath = "Unlit/Transparent";
        public const string warpEffect2ShaderPath = "Unlit/Transparent";
        //public const string warpWhiteFlashPath = "WarpPlugin/ParticleFX/warp10";
        //public const string warpRedFlashPath = "WarpPlugin/ParticleFX/warpr10";
        //public const string warpTexturePath = "WarpPlugin/ParticleFX/warp";
        //public const string warprTexturePath = "WarpPlugin/ParticleFX/warpr";

        [KSPField]
        public string warpSoundPath = "WarpPlugin/Sounds/warp_sound";
        [KSPField]
        public double warpSoundPitchExp = 0.1;

        [KSPField]
        public double warpPowerReqMult = 0.5;
        [KSPField]
        public double responseMultiplier = 0.005;
        [KSPField]
        public double antigravityMultiplier = 2;
        [KSPField]
        public double GThreshold = 2;
        [KSPField]
        public int InstanceID;
        [KSPField]
        public string AnimationName = "";
        [KSPField]
        public string upgradedName = "";
        [KSPField]
        public string originalName = "";
        [KSPField]
        public float effectSize1 = 0;
        [KSPField]
        public float effectSize2 = 0;
        [KSPField]
        public string upgradeTechReq = "";
        [KSPField]
        public double powerRequirementMultiplier = 1;
        [KSPField]
        public long maxPowerTimeout = 50;
        [KSPField]
        public float warpPowerMultTech0 = 10;
        [KSPField]
        public float warpPowerMultTech1 = 20;
        [KSPField]
        public double wasteheatRatio = 0.5;
        [KSPField]
        public double wasteheatRatioUpgraded = 0.25;
        [KSPField]
        public double wasteHeatMultiplier = 1;
        [KSPField]
        public double maximumWarpWeighted;
        [KSPField]
        public double magnitudeDiff;
        [KSPField]
        public double exotic_power_required = 1000;
        [KSPField]
        public bool useRotateStability = true;
        [KSPField]
        public bool allowWarpTurning = true;
        [KSPField]
        public float headingChangedTimeout = 50;
        [KSPField]
        public double gravityMaintenancePowerMultiplier = 4;

        [KSPField(groupName = GROUP, isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "#LOC_KSPIE_AlcubierreDrive_SafetyDistance", guiUnits = " Km"), UI_FloatRange(minValue = 0, maxValue = 200, stepIncrement = 1)]//Safety Distance
        public float spaceSafetyDistance = 30;
        [KSPField(groupName = GROUP, isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "#LOC_KSPIE_AlcubierreDrive_AntigravityPercentage"), UI_FloatRange(minValue = 0, maxValue = 200, stepIncrement = 5)]//Exotic Matter Percentage
        public float antigravityPercentage;

        //GUI
        [KSPField(groupName = GROUP, guiActive = false, guiActiveEditor = false, guiName = "#LOC_KSPIE_AlcubierreDrive_warpdriveType")]
        public string warpdriveType = "Alcubierre Drive";
        [KSPField(groupName = GROUP, guiActive = false, guiActiveEditor = true, guiName = "#LOC_KSPIE_AlcubierreDrive_engineMass", guiFormat = "F3", guiUnits = " t")]
        public float partMass;
        [KSPField(groupName = GROUP, guiActive = false, guiActiveEditor = true, guiName = "#LOC_KSPIE_AlcubierreDrive_warpStrength", guiFormat = "F1", guiUnits = " t")]
        public float warpStrength = 1;
        [KSPField(groupName = GROUP, guiActive = false, guiActiveEditor = true, guiName = "#LOC_KSPIE_AlcubierreDrive_totalWarpPower", guiFormat = "F1", guiUnits = " t")]
        public float totalWarpPower;
        [KSPField(groupName = GROUP, guiActive = false, guiActiveEditor = true, guiName = "#LOC_KSPIE_AlcubierreDrive_vesselTotalMass", guiFormat = "F3", guiUnits = " t")]
        public double vesselTotalMass;
        [KSPField(groupName = GROUP, guiActive = false, guiActiveEditor = true, guiName = "#LOC_KSPIE_AlcubierreDrive_warpToMassRatio", guiFormat = "F4")]
        public double warpToMassRatio;
        [KSPField(groupName = GROUP, guiActive = false, guiActiveEditor = false, guiName = "#LOC_KSPIE_AlcubierreDrive_gravityAtSeaLevel", guiUnits = " m/s\xB2", guiFormat = "F5")]
        public double gravityAtSeaLevel;
        [KSPField(groupName = GROUP, guiActive = false, guiActiveEditor = false, guiName = "#LOC_KSPIE_AlcubierreDrive_gravityVesselPull", guiUnits = " m/s\xB2", guiFormat = "F5")]
        public double gravityPull;
        [KSPField(groupName = GROUP, guiActive = false,  guiActiveEditor = false, guiName = "#LOC_KSPIE_AlcubierreDrive_gravityDragRatio", guiFormat = "F5")]
        public double gravityDragRatio;
        [KSPField(groupName = GROUP, guiActive = false, guiActiveEditor = false, guiName = "#LOC_KSPIE_AlcubierreDrive_gravityDragPercentage", guiUnits = "%", guiFormat = "F3")]
        public double gravityDragPercentage;
        [KSPField(groupName = GROUP, guiActive = false, guiActiveEditor = false, guiName = "#LOC_KSPIE_AlcubierreDrive_gravityRatio")]
        public double gravityRatio;
        [KSPField(groupName = GROUP, guiActive = false, guiActiveEditor = false, guiName = "#LOC_KSPIE_AlcubierreDrive_maxWarpGravityLimit", guiUnits = "c", guiFormat = "F4")]
        public double maximumWarpForGravityPull;
        [KSPField(groupName = GROUP, guiActive = false, guiActiveEditor = false, guiName = "#LOC_KSPIE_AlcubierreDrive_maxWarpAltitudeLimit", guiUnits = "c", guiFormat = "F4")]
        public double maximumWarpForAltitude;
        [KSPField(groupName = GROUP, guiActive = false, guiActiveEditor = false, guiName = "#LOC_KSPIE_AlcubierreDrive_maxAllowedThrotle", guiUnits = "c", guiFormat = "F4")]
        public double maximumAllowedWarpThrotle;
        [KSPField(groupName = GROUP, guiActive = true, guiActiveEditor = false, guiName = "#LOC_KSPIE_AlcubierreDrive_currentSelectedSpeed", guiUnits = "c", guiFormat = "F4")]
        public double warpEngineThrottle;
        [KSPField(groupName = GROUP, guiActive = false, guiActiveEditor = false, guiName = "#LOC_KSPIE_AlcubierreDrive_minPowerReqForLightSpeed", guiUnits = "#LOC_KSPIE_Reactor_megawattUnit", guiFormat = "F4")]
        public double minPowerRequirementForLightSpeed;
        [KSPField(groupName = GROUP, guiActive = false, guiActiveEditor = false, guiName = "#LOC_KSPIE_AlcubierreDrive_currentPowerReqForWarp", guiUnits = "#LOC_KSPIE_Reactor_megawattUnit", guiFormat = "F4")]
        public double currentPowerRequirementForWarp;
        [KSPField(groupName = GROUP, guiActive = false, guiActiveEditor = false, guiName = "#LOC_KSPIE_AlcubierreDrive_powerReqForMaxAllowedSpeed", guiUnits = "#LOC_KSPIE_Reactor_megawattUnit", guiFormat = "F4")]
        public double powerRequirementForMaximumAllowedLightSpeed;
        [KSPField(groupName = GROUP, guiActive = false, guiActiveEditor = false, guiName = "#LOC_KSPIE_AlcubierreDrive_PowerRequirementForSlowedSubLightSpeed", guiUnits = "#LOC_KSPIE_Reactor_megawattUnit", guiFormat = "F4")]//Power Requirement For Slowed SubLightSpeed
        public double powerRequirementForSlowedSubLightSpeed;
        [KSPField(groupName = GROUP, guiActive = false, guiActiveEditor = false, guiName = "#LOC_KSPIE_AlcubierreDrive_currentWarpExitSpeed", guiUnits = " m/s", guiFormat = "F3")]
        public double exitSpeed;
        [KSPField(groupName = GROUP, guiActive = false, guiActiveEditor = false, guiName = "#LOC_KSPIE_AlcubierreDrive_currentWarpExitApoapsis", guiUnits = " km", guiFormat = "F3")]
        public double exitApoapsis;
        [KSPField(groupName = GROUP, guiActive = false, guiActiveEditor = false, guiName = "#LOC_KSPIE_AlcubierreDrive_currentWarpExitPeriapsis", guiUnits = " km", guiFormat = "F3")]
        public double exitPeriapsis;
        [KSPField(groupName = GROUP, guiActive = false, guiActiveEditor = false, guiName = "#LOC_KSPIE_AlcubierreDrive_currentWarpExitEccentricity", guiFormat = "F3")]
        public double exitEccentricity;
        [KSPField(groupName = GROUP, guiActive = false, guiActiveEditor = false, guiName = "#LOC_KSPIE_AlcubierreDrive_currentWarpExitMeanAnomaly", guiUnits = "\xB0", guiFormat = "F3")]
        public double exitMeanAnomaly;
        [KSPField(groupName = GROUP, guiActive = false, guiActiveEditor = false, guiName = "#LOC_KSPIE_AlcubierreDrive_currentWarpExitBurnToCircularize", guiUnits = " m/s", guiFormat = "F3")]
        public double exitBurnCircularize;
        [KSPField(groupName = GROUP, guiActive = true, guiActiveEditor = false, guiName = "#LOC_KSPIE_AlcubierreDrive_status")]
        public string driveStatus;
        [KSPField(groupName = GROUP, guiActive = false, guiActiveEditor = false, guiName = "#LOC_KSPIE_AlcubierreDrive_CosineAngleToClosestBody", guiFormat = "F3")]//Cos Angle To Closest Body
        private double cosineAngleToClosestBody;
        [KSPField(groupName = GROUP, guiActive = false, guiActiveEditor = false, guiName = "#LOC_KSPIE_AlcubierreDrive_Distancetoclosestbody", guiFormat = "F0", guiUnits = " m")]//Distance to closest body
        private double distanceToClosestBody;
        [KSPField(groupName = GROUP, guiActive = false, guiActiveEditor = false, guiName = "#LOC_KSPIE_AlcubierreDrive_Nameofclosestbody")]//Name of closest body
        string closestCelestrialBodyName;
        [KSPField(groupName = GROUP, guiActive = false, guiActiveEditor = false, guiName = "#LOC_KSPIE_AlcubierreDrive_AllowedWarpDistancePerFrame", guiFormat = "F3")]//Max distance per frame
        private double allowedWarpDistancePerFrame;
        [KSPField(groupName = GROUP, guiActive = false, guiActiveEditor = false, guiName = "#LOC_KSPIE_AlcubierreDrive_MaximumWarpSpeed", guiFormat = "F3")]//Maximum Warp Speed
        private double maximumWarpSpeed;
        [KSPField(groupName = GROUP, guiActive = false, guiActiveEditor = false, guiName = "#LOC_KSPIE_AlcubierreDrive_Safetydistance", guiFormat = "F3", guiUnits = " m")]//Safety distance
        private double safetyDistance;
        [KSPField(groupName = GROUP, guiActive = false, guiActiveEditor = false, guiName = "#LOC_KSPIE_AlcubierreDrive_DropoutDistance", guiFormat = "F3", guiUnits = " m")]//Dropout Distance
        private double dropoutDistance;
        [KSPField(groupName = GROUP, guiActive = false, guiActiveEditor = false, guiName = "#LOC_KSPIE_AlcubierreDrive_AvailablePower", guiFormat = "F3", guiUnits = "MJ")]//Available Power for Warp
        private double availablePower;
        [KSPField(groupName = GROUP, guiActive = false, guiActiveEditor = false, guiName = "#LOC_KSPIE_AlcubierreDrive_GravityAcceleration", guiFormat = "F3", guiUnits = " m/s\xB2")]//Gravity Acceleration
        private double gravityAcceleration;
        [KSPField(groupName = GROUP, guiActive = false, guiActiveEditor = false, guiName = "#LOC_KSPIE_AlcubierreDrive_AntiGravityAcceleration", guiFormat = "F3", guiUnits = " m/s\xB2")]//Anti Gravity Acceleration
        private double antigravityAcceleration;
        [KSPField(groupName = GROUP, guiActive = false, guiActiveEditor = false, guiName = "#LOC_KSPIE_AlcubierreDrive_VerticalSpeed", guiFormat = "F3", guiUnits = " m/s")]//Vertical Speed
        private double verticalSpeed;
        [KSPField(groupName = GROUP, guiActive = false, guiActiveEditor = false, guiName = "#LOC_KSPIE_AlcubierreDrive_MaintenancePowerReq", guiFormat = "F3", guiUnits = " m/s")]//Maintenance Power Req
        private double requiredExoticMaintenancePower;
        [KSPField(groupName = GROUP, guiActive = false, guiActiveEditor = false, guiName = "#LOC_KSPIE_AlcubierreDrive_ChargePowerDraw", guiFormat = "F3", guiUnits = " m/s")]//Charge Power Draw
        private double chargePowerDraw;
        [KSPField(groupName = GROUP, guiActive = false, guiActiveEditor = false, guiName = "#LOC_KSPIE_AlcubierreDrive_MaxChargePowerRequired", guiFormat = "F3", guiUnits = " m/s")]//Max Charge Power Required
        private double maxChargePowerRequired;

        [KSPField(groupName = GROUP, groupDisplayName = GROUP_TITLE, isPersistant = true, guiActive = true, guiName = "#LOC_KSPIE_AlcubierreDrive_WarpWindow"), UI_Toggle(disabledText = "#LOC_KSPIE_AlcubierreDrive_WarpWindow_Hidden", enabledText = "#LOC_KSPIE_AlcubierreDrive_WarpWindow_Shown", affectSymCounterparts = UI_Scene.None)]//Warp Window--Hidden--Shown
        public bool showWindow;
        [KSPField(groupName = GROUP, isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "#LOC_KSPIE_AlcubierreDrive_AutoRendevousCircularize"), UI_Toggle(disabledText = "#LOC_KSPIE_AlcubierreDrive_False", enabledText = "#LOC_KSPIE_AlcubierreDrive_True", affectSymCounterparts = UI_Scene.All)]//Auto Rendevous/Circularize-False-True
        public bool matchExitToDestinationSpeed = true;
        [KSPField(groupName = GROUP, isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "#LOC_KSPIE_AlcubierreDrive_AutoMaximizeWarpSpeed"), UI_Toggle(disabledText = "#LOC_KSPIE_AlcubierreDrive_Disabled", enabledText = "#LOC_KSPIE_AlcubierreDrive_Enabled", affectSymCounterparts = UI_Scene.All)]//Auto Maximize Warp Speed -Disabled-Enabled
        public bool maximizeWarpSpeed = false;
        [KSPField(groupName = GROUP, isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "#LOC_KSPIE_AlcubierreDrive_AutoHoldAltitude"), UI_Toggle(disabledText = "#LOC_KSPIE_AlcubierreDrive_Disabled", enabledText = "#LOC_KSPIE_AlcubierreDrive_Enabled", affectSymCounterparts = UI_Scene.All)]//Auto Hold Altitude--Disabled--Enabled
        public bool holdAltitude;
        [KSPField(groupName = GROUP, isPersistant = true, guiActiveEditor = true, guiName = "Main Control"), UI_Toggle(disabledText = "#LOC_KSPIE_AlcubierreDrive_Enabled", enabledText = "#LOC_KSPIE_AlcubierreDrive_Disabled", affectSymCounterparts = UI_Scene.None)]
        public bool IsSlave;
        [KSPField(groupName = GROUP, isPersistant = true, guiActiveEditor = true, guiName = "Trail Effects"), UI_Toggle(disabledText = "#LOC_KSPIE_AlcubierreDrive_Enabled", enabledText = "#LOC_KSPIE_AlcubierreDrive_Disabled", affectSymCounterparts = UI_Scene.All)]
        public bool hideTrail = false;

        // Debugging
        public double receivedExoticMaintenancePower;
        public double exoticMatterMaintenanceRatio;
        public double exoticMatterProduced;
        public double minimumExoticMatterMaintenanceRatio;

        private double responseFactor;
        private double stablePowerSupply;
        private double orbitMultiplier;

        private readonly double[] _engineThrottle = { 0.001, 0.0013, 0.0016, 0.002, 0.0025, 0.0032, 0.004, 0.005, 0.0063, 0.008, 0.01, 0.013, 0.016, 0.02, 0.025, 0.032, 0.04, 0.05, 0.063, 0.08, 0.1, 0.13, 0.16, 0.2, 0.25, 0.32, 0.4, 0.5, 0.63, 0.8, 1, 1.3, 1.6, 2, 2.5, 3.2, 4, 5, 6.3, 8, 10, 13, 16, 20, 25, 32, 40, 50, 63, 80, 100, 130, 160, 200, 250, 320, 400, 500, 630, 800, 1000, 1300, 1600, 2000, 2500, 3200, 4000, 5000, 6300, 8000, 10000, 13000, 16000, 20000, 25000, 32000, 40000, 50000, 63000, 80000, 100000 };

        private GameObject warp_effect;
        private GameObject warp_effect2;
        private Texture[] warp_textures;
        private Texture[] warp_textures2;
        private AudioSource warp_sound;

        double currentExoticMatter;
        double maxExoticMatter;
        double exoticMatterRatio;
        double tex_count;

        private bool activeTrail;
        private bool vesselWasInOuterspace;
        private bool hasrequiredupgrade;
        private bool selectedTargetVesselIsClosest;

        private Rect windowPosition;
        private AnimationState[] animationState;
        private Vector3d heading_act;
        private Vector3d active_part_heading;
        private List<AlcubierreDrive> alcubierreDrives;
        private UI_FloatRange antigravityFloatRange;
        private UI_Toggle holdAltitudeToggle;

        private float windowPositionX = 200;
        private float windowPositionY = 100;
        private float warp_size = 50000;

        private GUIStyle bold_black_style;
        private GUIStyle text_black_style;

        private int _windowID = 252824373;
        private int old_selected_factor = 0;
        private int minimum_selected_factor;
        private int maximumWarpSpeedFactor;
        private int minimumPowerAllowedFactor;
        private int warpTrailTimeout;

        private long insufficientPowerTimeout = 10;
        private long initiateWarpTimeout;
        private long counterCurrent;
        private long counterPreviousChange;

        private Renderer warp_effect1_renderer;
        private Renderer warp_effect2_renderer;

        private Collider warp_effect1_collider;
        private Collider warp_effect2_collider;

        private PartResourceDefinition exoticResourceDefinition;
        private CelestialBody warpInitialMainBody;
        private CelestialBody closestCelestrialBody;
        private Vector3d departureVelocity;
        private ModuleReactionWheel moduleReactionWheel;

        private Texture2D warpWhiteFlash;
        private Texture2D warpRedFlash;

        private BaseField antigravityField;

        // Actions
        [KSPAction("#LOC_KSPIE_AlcubierreDrive_Toggle_WarpWindow")]
        public void ToggleNextPropellantAction(KSPActionParam param)
        {
            showWindow = !showWindow;
        }

        [KSPEvent(groupName = GROUP, guiActive = true, guiName = "#LOC_KSPIE_AlcubierreDrive_startChargingDrive", active = true)]
        public void StartCharging()
        {
            Debug.Log("[KSPI]: Start Charging pressed");

            if (IsEnabled) return;

            if (warpToMassRatio < 1)
            {
                Debug.Log("[KSPI]: warpToMassRatio: " + warpToMassRatio + " is less than 1" );
                var message = Localizer.Format("#LOC_KSPIE_AlcubierreDrive_warpStrenthToLowForVesselMass");
                ScreenMessages.PostScreenMessage(message);
                return;
            }

            insufficientPowerTimeout = maxPowerTimeout;
            IsCharging = true;
            holdAltitude = false;
            antigravityPercentage = 100;

            UpdateAllWarpDriveEffects();
        }

        [KSPEvent(groupName = GROUP, guiActive = true, guiName = "#LOC_KSPIE_AlcubierreDrive_stopChargingDrive", active = false)]
        public void StopCharging()
        {
            Debug.Log("[KSPI]: Stop Charging button pressed");
            IsCharging = false;
            holdAltitude = false;
            antigravityPercentage = 0;

            UpdateAllWarpDriveEffects();
        }

        [KSPAction("#LOC_KSPIE_AlcubierreDrive_activateWarpDrive")]
        public void ActivateWarpDriveAction(KSPActionParam param)
        {
            ActivateWarpDrive();
        }

        [KSPEvent(groupName = GROUP, guiActive = true, guiName = "#LOC_KSPIE_AlcubierreDrive_activateWarpDrive", active = true)]
        public void ActivateWarpDrive()
        {
            Debug.Log("[KSPI]: Activate Warp Drive button pressed");
            if (IsEnabled) return;

            if (warpToMassRatio < 1)
            {
                var message = Localizer.Format("#LOC_KSPIE_AlcubierreDrive_notEnoughWarpPowerToVesselMass");
                Debug.Log("[KSPI]: " + message);
                ScreenMessages.PostScreenMessage(message, 5.0f, ScreenMessageStyle.UPPER_CENTER);
                return;
            }

            if (!CheatOptions.IgnoreMaxTemperature && vessel.atmDensity > 0)
            {
                var message = Localizer.Format("#LOC_KSPIE_AlcubierreDrive_cannotActivateWarpdriveWithinAtmosphere");
                Debug.Log("[KSPI]: " + message);
                ScreenMessages.PostScreenMessage(message, 5.0f, ScreenMessageStyle.UPPER_CENTER);
                return;
            }

            if (antigravityPercentage < 0.5 || (minimumExoticMatterMaintenanceRatio < 0.999 && antigravityPercentage >= 0.5))
            {
                var message = Localizer.Format("#LOC_KSPIE_AlcubierreDrive_warpdriveIsNotFullyChargedForWarp");
                Debug.Log("[KSPI]: " + message);
                ScreenMessages.PostScreenMessage(message, 5.0f, ScreenMessageStyle.UPPER_CENTER);
                return;
            }

            if (maximumWarpSpeedFactor < selected_factor)
                selected_factor = minimumPowerAllowedFactor;

            // TODO fix the stable resource supply thing.

            //if (!CheatOptions.InfiniteElectricity && GetPowerRequirementForWarp(_engineThrottle[selected_factor]) > getStableResourceSupply(KITResourceSettings.ElectricPowerInMegawatt))
            /*
            if (false)
            {
                var message = Localizer.Format("#LOC_KSPIE_AlcubierreDrive_warpPowerReqIsHigherThanMaxPowerSupply");
                Debug.Log("[KSPI]: " + message);
                ScreenMessages.PostScreenMessage(message, 5.0f, ScreenMessageStyle.UPPER_CENTER);
                return;
            }
            */

            IsCharging = false;
            antigravityPercentage = 0;
            initiateWarpTimeout = 10;
        }

        private int GetMaximumFactor(double lightspeed)
        {
            var maxFactor = 0;

            for (var i = 0; i < _engineThrottle.Length; i++)
            {
                if (_engineThrottle[i] > lightspeed)
                    return maxFactor;
                maxFactor = i;
            }
            return maxFactor;
        }

        private void InitiateWarp(IResourceManager resMan)
        {
            Debug.Log("[KSPI]: InitiateWarp started");

            if (maximumWarpSpeedFactor < selected_factor)
                selected_factor = minimumPowerAllowedFactor;

            var selectedWarpSpeed = _engineThrottle[selected_factor];

            // verify if we are not warping into main body
            var cosineAngleToMainBody = Vector3d.Dot(part.transform.up.normalized, (vessel.CoMD - vessel.mainBody.position).normalized);

            var headingModifier = Math.Abs(Math.Min(0, cosineAngleToMainBody));

            allowedWarpDistancePerFrame = PluginSettings.Config.SpeedOfLight * resMan.FixedDeltaTime() * selectedWarpSpeed * headingModifier;
            safetyDistance = spaceSafetyDistance * 1000 * headingModifier;

            if (vessel.altitude < ((vessel.mainBody.atmosphere ? vessel.mainBody.atmosphereDepth : 20000) + allowedWarpDistancePerFrame + safetyDistance))
            {
                var message = Localizer.Format("#LOC_KSPIE_AlcubierreDrive_msg1", vessel.mainBody.name);//"Warp initiation aborted, cannot warp into " +
                Debug.Log("[KSPI]: " + message);
                ScreenMessages.PostScreenMessage(message, 5.0f, ScreenMessageStyle.UPPER_CENTER);
                initiateWarpTimeout = 0;
                return;
            }

            currentPowerRequirementForWarp = GetPowerRequirementForWarp(selectedWarpSpeed);

            var powerReturned = resMan.ConsumeResource(ResourceName.ElectricCharge, currentPowerRequirementForWarp * GameConstants.ecPerMJ);

            if (powerReturned < 0.99 * currentPowerRequirementForWarp)
            {
                initiateWarpTimeout--;

                if (initiateWarpTimeout == 1)
                {
                    while (selected_factor != minimum_selected_factor)
                    {
                        Debug.Log("[KSPI]: call ReduceWarpPower");
                        ReduceWarpPower();
                        selectedWarpSpeed = _engineThrottle[selected_factor];
                        currentPowerRequirementForWarp = GetPowerRequirementForWarp(selectedWarpSpeed);
                        if (powerReturned >= currentPowerRequirementForWarp)
                            return;
                    }
                }
                if (initiateWarpTimeout == 0)
                {
                    var message = Localizer.Format("#LOC_KSPIE_AlcubierreDrive_msg2") + powerReturned + " " + currentPowerRequirementForWarp;//"Not enough power to initiate warp!"
                    Debug.Log("[KSPI]: " + message);
                    ScreenMessages.PostScreenMessage(message, 5.0f, ScreenMessageStyle.UPPER_CENTER);
                    IsCharging = true;
                    return;
                }
            }

            initiateWarpTimeout = 0; // stop initiating to warp

            vesselWasInOuterspace = false;

            // consume all exotic matter to create warp field
            resMan.ConsumeResource(ResourceName.ExoticMatter, exotic_power_required);

            warp_sound.Play();
            warp_sound.loop = true;

            // prevent g-force effects for current and next frame
            PluginHelper.IgnoreGForces(part, 2);

            warpInitialMainBody = vessel.mainBody;
            departureVelocity = vessel.orbit.GetFrameVel();

            active_part_heading = new Vector3d(part.transform.up.x, part.transform.up.z, part.transform.up.y);

            heading_act = active_part_heading * PluginSettings.Config.SpeedOfLight * selectedWarpSpeed;
            serialisedwarpvector = ConfigNode.WriteVector(heading_act);

            if (!vessel.packed)
                vessel.GoOnRails();

            vessel.orbit.UpdateFromStateVectors(vessel.orbit.pos, vessel.orbit.vel + heading_act, vessel.orbit.referenceBody, Planetarium.GetUniversalTime());

            if (!vessel.packed)
                vessel.GoOffRails();

            IsEnabled = true;
            activeTrail = true;

            UpdateAllWarpDriveEffects();

            existing_warp_speed = selectedWarpSpeed;
        }

        [KSPAction("#LOC_KSPIE_AlcubierreDrive_deactivateWarpDrive")]
        public void DeactivateWarpDriveAction(KSPActionParam param)
        {
            DeactivateWarpDrive();
        }

        [KSPEvent(groupName = GROUP, guiActive = true, guiName = "#LOC_KSPIE_AlcubierreDrive_deactivateWarpDrive", active = false)]
        public void DeactivateWarpDrive()
        {
            DeactivateWarpDrive(true);
        }

        public void DeactivateWarpDrive(bool controlled)
        {
            Debug.Log("[KSPI]: Deactivate Warp Drive event called");

            if (!IsEnabled)
            {
                Debug.Log("[KSPI]: canceled, Warp Drive is already inactive");
                return;
            }

            // mark warp to be disabled
            IsEnabled = false;

            // update list of warp drives
            alcubierreDrives = part.vessel.FindPartModulesImplementing<AlcubierreDrive>();

            // update visuals all warp drives
            UpdateAllWarpDriveEffects();

            // Disable sound
            warp_sound.Stop();

            Vector3d reverse_warp_heading =  new Vector3d(-heading_act.x, -heading_act.y, -heading_act.z);

            // prevent g-force effects for current and next frame
            PluginHelper.IgnoreGForces(part, 2);

            // puts the ship back into a simulated orbit and reenables physics
            if (!this.vessel.packed)
                vessel.GoOnRails();

            if (controlled && matchExitToDestinationSpeed && departureVelocity != null)
            {
                Debug.Log("[KSPI]: vessel departure velocity " + departureVelocity.x + " " + departureVelocity.y + " " + departureVelocity.z);
                Vector3d reverse_initial_departure_velocity = new Vector3d(-departureVelocity.x, -departureVelocity.y, -departureVelocity.z);

                // remove vessel departure speed in world
                reverse_warp_heading += reverse_initial_departure_velocity;

                // add celestial body orbit speed to match speed in world
                if (vessel.mainBody.orbit != null)
                    reverse_warp_heading += vessel.mainBody.orbit.GetFrameVel();
            }

            var universalTime = Planetarium.GetUniversalTime();

            vessel.orbit.UpdateFromStateVectors(vessel.orbit.pos, vessel.orbit.vel + reverse_warp_heading, vessel.orbit.referenceBody, universalTime);

            // disables physics and puts the ship into a propagated orbit , is this still needed?
            if (!this.vessel.packed)
                vessel.GoOffRails();

            if (controlled && matchExitToDestinationSpeed && vessel.atmDensity == 0)
            {
                Vector3d circularizationVector;

                var vesselTarget = FlightGlobals.fetch.VesselTarget;
                if (vesselTarget != null)
                {
                    var reverseExitVelocityVector = new Vector3d(-vessel.orbit.vel.x, -vessel.orbit.vel.y, -vessel.orbit.vel.z);
                    vessel.orbit.UpdateFromStateVectors(vessel.orbit.pos, vessel.orbit.vel + reverseExitVelocityVector, vessel.orbit.referenceBody, universalTime);
                    var targetOrbitVector = vesselTarget.GetOrbit().getOrbitalVelocityAtUT(universalTime);
                    circularizationVector = new Vector3d(targetOrbitVector.x, targetOrbitVector.y, targetOrbitVector.z) + reverseExitVelocityVector;
                }
                else
                {
                    var timeAtApoapsis = vessel.orbit.timeToAp < vessel.orbit.period / 2
                        ? vessel.orbit.timeToAp + universalTime
                        : universalTime - (vessel.orbit.period - vessel.orbit.timeToAp);

                    var reverseExitVelocityVector = new Vector3d(-vessel.orbit.vel.x, -vessel.orbit.vel.y, -vessel.orbit.vel.z);
                    var velocityVectorAtApoapsis = vessel.orbit.getOrbitalVelocityAtUT(timeAtApoapsis);
                    var circularOrbitSpeed = CircularOrbitSpeed(vessel.mainBody, vessel.orbit.altitude + vessel.mainBody.Radius);
                    var horizontalVelocityVectorAtApoapsis = new Vector3d(velocityVectorAtApoapsis.x, velocityVectorAtApoapsis.y, 0);
                    circularizationVector = horizontalVelocityVectorAtApoapsis.normalized * (circularOrbitSpeed - horizontalVelocityVectorAtApoapsis.magnitude) + reverseExitVelocityVector;
                }

                vessel.orbit.UpdateFromStateVectors(vessel.orbit.pos, vessel.orbit.vel + circularizationVector, vessel.orbit.referenceBody, universalTime);
            }

            if (warpInitialMainBody == null || vessel.mainBody == warpInitialMainBody) return;

            if (KopernicusHelper.IsStar(part.vessel.mainBody)) return;

            if (!matchExitToDestinationSpeed && vessel.mainBody != warpInitialMainBody)
               Develocitize();
        }

        [KSPAction("#LOC_KSPIE_AlcubierreDrive_increaseWarpSpeed")]
        public void ToggleWarpSpeedUpAction(KSPActionParam param)
        {
            ToggleWarpSpeedUp();
        }

        [KSPEvent(groupName = GROUP, guiActive = true, guiName = "#LOC_KSPIE_AlcubierreDrive_increaseWarpSpeed", active = true)]
        public void ToggleWarpSpeedUp()
        {
            Debug.Log("[KSPI]: Warp Throttle (+) button pressed");
            selected_factor++;
            if (selected_factor >= _engineThrottle.Length)
                selected_factor = _engineThrottle.Length - 1;

            if (!IsEnabled)
                old_selected_factor = selected_factor;
        }

        [KSPAction("#LOC_KSPIE_AlcubierreDrive_increaseWarpSpeed3")]
        public void ToggleWarpSpeedUp3Action(KSPActionParam param)
        {
            ToggleWarpSpeedUp3();
        }

        private void ToggleWarpSpeedUp3()
        {
            Debug.Log("[KSPI]: 3x Speed Up pressed");

            for (var i = 0; i < 3; i++)
            {
                selected_factor++;

                if (selected_factor < _engineThrottle.Length) continue;

                selected_factor = _engineThrottle.Length - 1;
                break;
            }

            if (!IsEnabled)
                old_selected_factor = selected_factor;
        }

        [KSPAction("#LOC_KSPIE_AlcubierreDrive_increaseWarpSpeed10")]
        public void ToggleWarpSpeedUp10Action(KSPActionParam param)
        {
            ToggleWarpSpeedUp10();
        }

        private void ToggleWarpSpeedUp10()
        {
            Debug.Log("[KSPI]: 10x Speed Up pressed");

            for (var i = 0; i < 10; i++)
            {
                selected_factor++;

                if (selected_factor < _engineThrottle.Length) continue;

                selected_factor = _engineThrottle.Length - 1;
                break;
            }

            if (!IsEnabled)
                old_selected_factor = selected_factor;
        }

        [KSPAction("#LOC_KSPIE_AlcubierreDrive_decreaseWarpSpeed")]
        public void ToggleWarpSpeedDownAction(KSPActionParam param)
        {
            ToggleWarpSpeedDown();
        }

        [KSPEvent(groupName = GROUP, guiActive = true, guiActiveUnfocused = true, guiName = "#LOC_KSPIE_AlcubierreDrive_decreaseWarpSpeed", active = true)]
        public void ToggleWarpSpeedDown()
        {
            Debug.Log("[KSPI]: Warp Throttle (-) button pressed");
            selected_factor--;
            if (selected_factor < 0)
                selected_factor = 0;

            if (!IsEnabled)
                old_selected_factor = selected_factor;
        }

        [KSPAction("#LOC_KSPIE_AlcubierreDrive_decreaseWarpSpeed3")]
        public void ToggleWarpSpeedDown3Action(KSPActionParam param)
        {
            ToggleWarpSpeedDown3();
        }

        private void ToggleWarpSpeedDown3()
        {
            Debug.Log("[KSPI]: 3x Speed Down pressed");

            for (var i = 0; i < 3; i++)
            {
                selected_factor--;

                if (selected_factor >= 0) continue;

                selected_factor = 0;
                break;
            }

            if (!IsEnabled)
                old_selected_factor = selected_factor;
        }

        [KSPAction("#LOC_KSPIE_AlcubierreDrive_decreaseWarpSpeed10")]
        public void ToggleWarpSpeedDown10Action(KSPActionParam param)
        {
            ToggleWarpSpeedDown10();
        }

        private void ToggleWarpSpeedDown10()
        {
            Debug.Log("[KSPI]: 10x Speed Down pressed");

            for (var i = 0; i  < 10; i++)
            {
                selected_factor--;

                if (selected_factor >= 0) continue;

                selected_factor = 0;
                break;
            }

            if (!IsEnabled)
                old_selected_factor = selected_factor;
        }

        [KSPAction("#LOC_KSPIE_AlcubierreDrive_ReduceWarpPower")]
        public void ReduceWarpPowerAction(KSPActionParam param)
        {
            ReduceWarpPower();
        }

        [KSPEvent(groupName = GROUP, guiActive = true, guiName = "#LOC_KSPIE_AlcubierreDrive_ReduceWarpPower", active = true)]//Reduce Warp Power
        public void ReduceWarpPower()
        {
            Debug.Log("[KSPI]: Reduce Warp Power button pressed");
            if (selected_factor == minimum_selected_factor) return;

            if (selected_factor < minimum_selected_factor)
                ToggleWarpSpeedUp();
            else if (selected_factor > minimum_selected_factor)
                ToggleWarpSpeedDown();
        }

        [KSPAction("Increase Exotic Matter Percentage")]
        public void IncreaseAntiGravityAction(KSPActionParam param)
        {
            if (antigravityPercentage < 200)
                antigravityPercentage += 5;
        }

        [KSPAction("Decrease Exotic Matter Percentage")]
        public void DecreaseAntiGravityAction(KSPActionParam param)
        {
            if (antigravityPercentage > 0)
                antigravityPercentage -= 5;
        }

        [KSPAction("#LOC_KSPIE_AlcubierreDrive_startChargingDrive")]
        public void StartChargingAction(KSPActionParam param)
        {
            Debug.Log("[KSPI]: Start Charging Action activated");
            StartCharging();
        }

        [KSPAction("#LOC_KSPIE_AlcubierreDrive_stopChargingDrive")]
        public void StopChargingAction(KSPActionParam param)
        {
            Debug.Log("[KSPI]: Stop Charging Action activated");
            StopCharging();
        }

        [KSPAction("#LOC_KSPIE_AlcubierreDrive_toggleChargingDrive")]
        public void ToggleChargingAction(KSPActionParam param)
        {
            Debug.Log("[KSPI]: Toggle Charging Action activated");
            if (IsCharging)
                StopCharging();
            else
                StartCharging();
        }

        [KSPAction("#LOC_KSPIE_AlcubierreDrive_reducePowerConsumption")]
        public void ReduceWarpDriveAction(KSPActionParam param)
        {
            Debug.Log("[KSPI]: ReduceWarpPower action activated");
            ReduceWarpPower();
        }

        [KSPEvent(groupName = GROUP, guiActive = true, guiName = "#LOC_KSPIE_AlcubierreDrive_retrofit", active = true)]
        public void RetrofitDrive()
        {
            Debug.Log("[KSPI]: Retrofit button pressed");
            if (ResearchAndDevelopment.Instance == null) return;

            if (isupgraded || ResearchAndDevelopment.Instance.Science < UpgradeCost()) return;

            isupgraded = true;
            warpdriveType = upgradedName;

            ResearchAndDevelopment.Instance.AddScience(-UpgradeCost(), TransactionReasons.RnDPartPurchase);
        }

        private static float UpgradeCost()
        {
            return 0;
        }

        public override void OnStart(PartModule.StartState state)
        {
            if (state != StartState.Editor)
                vesselTotalMass = vessel.GetTotalMass();

            windowPosition = new Rect(windowPositionX, windowPositionY, 260, 100);
            _windowID = new System.Random(part.GetInstanceID()).Next(int.MaxValue);

            moduleReactionWheel = part.FindModuleImplementing<ModuleReactionWheel>();

            exoticResourceDefinition = PartResourceLibrary.Instance.GetDefinition(KITResourceSettings.ExoticMatter);

            InstanceID = GetInstanceID();

            if (IsSlave)
                Debug.Log("[KSPI] - AlcubierreDrive Slave " + InstanceID + " Started");
            else
                Debug.Log("[KSPI] - AlcubierreDrive Master " + InstanceID + " Started");

            if (!IsNullOrEmpty(AnimationName))
                animationState = PluginHelper.SetUpAnimation(AnimationName, part);

            try
            {
                Events[nameof(StartCharging)].active = !IsSlave;
                Events[nameof(StopCharging)].active = !IsSlave;
                Events[nameof(ActivateWarpDrive)].active = !IsSlave;
                Events[nameof(DeactivateWarpDrive)].active = !IsSlave;
                Events[nameof(ToggleWarpSpeedUp)].active = !IsSlave;
                Events[nameof(ToggleWarpSpeedDown)].active = !IsSlave;
                Events[nameof(ReduceWarpPower)].active = !IsSlave;
                Fields[nameof(showWindow)].guiActive = !IsSlave;
                Fields[nameof(matchExitToDestinationSpeed)].guiActive = !IsSlave;
                Fields[nameof(maximizeWarpSpeed)].guiActive = !IsSlave;
                Fields[nameof(holdAltitude)].guiActive = !IsSlave;
                Fields[nameof(spaceSafetyDistance)].guiActive = !IsSlave;
                Fields[nameof(maximumAllowedWarpThrotle)].guiActive = !IsSlave;
                Fields[nameof(warpToMassRatio)].guiActive = !IsSlave;
                Fields[nameof(minPowerRequirementForLightSpeed)].guiActive = !IsSlave;
                Fields[nameof(currentPowerRequirementForWarp)].guiActive = !IsSlave;
                Fields[nameof(totalWarpPower)].guiActive = !IsSlave;
                Fields[nameof(powerRequirementForMaximumAllowedLightSpeed)].guiActive = !IsSlave;
                Fields[nameof(antigravityAcceleration)].guiActive = !IsSlave;

                var holdAltitudeField = Fields[nameof(holdAltitude)];
                if (holdAltitudeField != null)
                {
                    holdAltitudeToggle = holdAltitudeField.uiControlFlight as UI_Toggle;
                    if (holdAltitudeToggle != null)
                        holdAltitudeToggle.onFieldChanged += HoldAltitudeChanged;
                }

                antigravityField = Fields[nameof(antigravityPercentage)];
                if (antigravityField != null)
                {
                    antigravityField.guiActive = !IsSlave;
                    antigravityFloatRange = antigravityField.uiControlFlight as UI_FloatRange;
                    if (antigravityFloatRange != null)
                    {
                        antigravityFloatRange.controlEnabled = !IsSlave;
                        antigravityFloatRange.onFieldChanged += AntigravityFloatChanged;
                    }
                }

                minimum_selected_factor = _engineThrottle.ToList().IndexOf(_engineThrottle.First(w => Math.Abs(w - 1) < float.Epsilon));
                if (selected_factor == -1)
                    selected_factor = minimum_selected_factor;

                hasrequiredupgrade = PluginHelper.UpgradeAvailable(upgradeTechReq);
                if (hasrequiredupgrade)
                    isupgraded = true;

                warpdriveType = isupgraded ? upgradedName : originalName;

                if (state == StartState.Editor) return;

                // if all drives are slaves, convert the current into a master
                alcubierreDrives = part.vessel.FindPartModulesImplementing<AlcubierreDrive>();
                if (alcubierreDrives.All(m => m.IsSlave))
                {
                    Debug.Log("[KSPI]: All drives were in slave mode, converting drive " + InstanceID + " into master");
                    IsSlave = false;
                }

                // if current is not a slave, turn all other drives into slaves
                if (!IsSlave)
                {
                    foreach (var drive in alcubierreDrives)
                    {
                        var driveId = drive.GetInstanceID();

                        if (driveId == InstanceID) continue;

                        Debug.Log("[KSPI]: Created AlcubierreDrive Slave for drive " + driveId);
                        drive.IsSlave = true;
                    }
                }

                Debug.Log("[KSPI]: AlcubierreDrive on " + part.name + " was Force Activated");
                this.part.force_activate();

                if (serialisedwarpvector != null)
                    heading_act = ConfigNode.ParseVector3D(serialisedwarpvector);

                warp_effect = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                warp_effect2 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

                warp_effect1_renderer = warp_effect.GetComponent<Renderer>();
                warp_effect2_renderer = warp_effect2.GetComponent<Renderer>();

                warp_effect1_collider = warp_effect.GetComponent<Collider>();
                warp_effect2_collider = warp_effect2.GetComponent<Collider>();

                warp_effect1_collider.enabled = false;
                warp_effect2_collider.enabled = false;

                var shipPos = new Vector3(part.transform.position.x, part.transform.position.y, part.transform.position.z);
                var endBeamPos = shipPos + transform.up * warp_size;
                var midPos = (shipPos - endBeamPos) / 2.0f;

                warp_effect.transform.localScale = new Vector3(effectSize1, midPos.magnitude, effectSize1);
                warp_effect.transform.position = new Vector3(midPos.x, shipPos.y + midPos.y, midPos.z);
                warp_effect.transform.rotation = part.transform.rotation;

                warp_effect2.transform.localScale = new Vector3(effectSize2, midPos.magnitude, effectSize2);
                warp_effect2.transform.position = new Vector3(midPos.x, shipPos.y + midPos.y, midPos.z);
                warp_effect2.transform.rotation = part.transform.rotation;

                //warp_effect.layer = LayerMask.NameToLayer("Ignore Raycast");
                //warp_effect.renderer.material = new Material(KSP.IO.File.ReadAllText<AlcubierreDrive>("AlphaSelfIllum.shader"));

                warp_effect1_renderer.material.shader = Shader.Find(warpEffect1ShaderPath);
                warp_effect2_renderer.material.shader = Shader.Find(warpEffect2ShaderPath);

                warpWhiteFlash = GameDatabase.Instance.GetTexture("WarpPlugin/ParticleFX/warp10", false);
                warpRedFlash = GameDatabase.Instance.GetTexture("WarpPlugin/ParticleFX/warpr10", false);

                warp_textures = new Texture[32];

                const string warpTexturePath = "WarpPlugin/ParticleFX/warp";
                for (var i = 0; i < 16; i++)
                {
                    warp_textures[i] = GameDatabase.Instance.GetTexture(i > 0
                        ? warpTexturePath + (i + 1)
                        : warpTexturePath, false);
                }

                //warp_textures[11] = warpWhiteFlash;
                for (var i = 16; i < 32; i++)
                {
                    var j = 31 - i;
                    warp_textures[i] = GameDatabase.Instance.GetTexture(j > 0
                      ? warpTexturePath + (j + 1)
                      : warpTexturePath, false);
                }

                warp_textures2 = new Texture[32];

                const string warprTexturePath = "WarpPlugin/ParticleFX/warpr";
                for (var i = 0; i < 16; i++)
                {
                    warp_textures2[i] = GameDatabase.Instance.GetTexture(i > 0
                        ? warprTexturePath + (i + 1)
                        : warprTexturePath, false);
                }

                //warp_textures2[11] = warpRedFlash;
                for (var i = 16; i < 32; i++)
                {
                    var j = 31 - i;
                    warp_textures2[i] = GameDatabase.Instance.GetTexture(j > 0
                        ? warprTexturePath + (j + 1)
                        : warprTexturePath, false);
                }

                warp_effect1_renderer.material.color = new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, 0.5f);
                warp_effect2_renderer.material.color = new Color(Color.red.r, Color.red.g, Color.red.b, 0.1f);

                warp_effect1_renderer.material.mainTexture = warp_textures[0];
                warp_effect1_renderer.receiveShadows = false;
                //warp_effect.layer = LayerMask.NameToLayer ("Ignore Raycast");
                //warp_effect.collider.isTrigger = true;
                warp_effect2_renderer.material.mainTexture = warp_textures2[0];
                warp_effect2_renderer.receiveShadows = false;
                warp_effect2_renderer.material.mainTextureOffset = new Vector2(-0.2f, -0.2f);
                //warp_effect2.layer = LayerMask.NameToLayer ("Ignore Raycast");
                //warp_effect2.collider.isTrigger = true;
                warp_effect2_renderer.material.renderQueue = 1000;
                warp_effect1_renderer.material.renderQueue = 1001;
                /*gameObject.AddComponent<Light>();
                gameObject.light.color = Color.cyan;
                gameObject.light.intensity = 1f;
                gameObject.light.range = 4000f;
                gameObject.light.type = LightType.Spot;
                gameObject.light.transform.position = end_beam_pos;
                gameObject.light.cullingMask = ~0;*/

                //warp_effect.transform.localScale.y = 2.5f;
                //warp_effect.transform.localScale.z = 200f;

                warp_sound = gameObject.AddComponent<AudioSource>();
                warp_sound.clip = GameDatabase.Instance.GetAudioClip(warpSoundPath);
                warp_sound.volume = GameSettings.SHIP_VOLUME;

                //warp_sound.panLevel = 0;
                warp_sound.panStereo = 0;
                warp_sound.rolloffMode = AudioRolloffMode.Linear;
                warp_sound.Stop();

                if (IsEnabled)
                {
                    warp_sound.Play();
                    warp_sound.loop = true;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[KSPI]: AlcubierreDrive OnStart 1 Exception " + e.Message);
            }

            warpdriveType = originalName;

            // update visuals all warp drives
            UpdateAllWarpDriveEffects();
        }

        private void AntigravityFloatChanged(BaseField field, object oldFieldValueObj)
        {
            holdAltitude = false;
        }

        private void HoldAltitudeChanged(BaseField field, object oldFieldValueObj)
        {
            antigravityPercentage = (float)((decimal)Math.Round(antigravityPercentage / 5) * 5);
        }

        public void VesselChangedSOI()
        {
            if (IsSlave)
                return;

            Debug.Log("[KSPI]: AlcubierreDrive Vessel Changed SOI");
        }

        public void Update()
        {
            if (!HighLogic.LoadedSceneIsEditor)
                return;

            partMass = part.mass;

            var warpDriveList = vessel.FindPartModulesImplementing<AlcubierreDrive>();

            totalWarpPower = warpDriveList.Sum(w => w.warpStrength * (w.isupgraded ? w.warpPowerMultTech1 : w.warpPowerMultTech0));

            warpToMassRatio = vesselTotalMass > 0 ? totalWarpPower / vesselTotalMass : 0;
        }

        public override void OnUpdate()
        {
            Events[nameof(StartCharging)].active = !IsSlave && !IsCharging;
            Events[nameof(StopCharging)].active = !IsSlave && IsCharging;
            Events[nameof(DeactivateWarpDrive)].active = !IsSlave && IsEnabled;
            Fields[nameof(driveStatus)].guiActive = !IsSlave && IsCharging;

            if (moduleReactionWheel != null)
            {
                moduleReactionWheel.Fields[nameof(moduleReactionWheel.authorityLimiter)].guiActive = false;
                moduleReactionWheel.Fields[nameof(moduleReactionWheel.actuatorModeCycle)].guiActive = false;
                moduleReactionWheel.Fields[nameof(moduleReactionWheel.stateString)].guiActive = false;
                moduleReactionWheel.Events[nameof(moduleReactionWheel.OnToggle)].guiActive = false;

                moduleReactionWheel.PitchTorque = activeTrail ? (5 * part.mass * (isupgraded ? warpPowerMultTech1 : warpPowerMultTech0)) : 0;
                moduleReactionWheel.YawTorque = activeTrail ? (5 * part.mass * (isupgraded ? warpPowerMultTech1 : warpPowerMultTech0)) : 0;
                moduleReactionWheel.RollTorque = activeTrail ? (5 * part.mass * (isupgraded ? warpPowerMultTech1 : warpPowerMultTech0)) : 0;
            }

            if (ResearchAndDevelopment.Instance != null)
                Events[nameof(RetrofitDrive)].active = !IsSlave && !isupgraded && ResearchAndDevelopment.Instance.Science >= UpgradeCost() && hasrequiredupgrade;
            else
                Events[nameof(RetrofitDrive)].active = false;

            if (IsSlave) return;

            UpdateAllWarpDriveEffects();
        }

        private void UpdateAllWarpDriveEffects()
        {
            foreach (var drive in alcubierreDrives)
            {
                drive.UpdateEffect(IsEnabled, IsCharging, IsEnabled && warpTrailTimeout == 0, selected_factor);
            }
        }

        private void UpdateEffect(bool isEnabled, bool isCharging, bool activeTrail, int selected_factor)
        {
            this.activeTrail = activeTrail;
            this.selected_factor = selected_factor;

            if (animationState == null) return;

            foreach (var anim in animationState)
            {
                if ((isEnabled || isCharging) && anim.normalizedTime < 1)
                    anim.speed = 1;

                if ((isEnabled || isCharging) && anim.normalizedTime >= 1)
                {
                    anim.speed = 0;
                    anim.normalizedTime = 1;
                }

                if (!isEnabled && !isCharging && anim.normalizedTime > 0)
                    anim.speed = -1;

                if (isEnabled || isCharging || !(anim.normalizedTime <= 0))
                    continue;

                anim.speed = 0;
                anim.normalizedTime = 0;
            }
        }

        public void FixedUpdate() // FixedUpdate is also called when not activated
        {
            if (!IsSlave)
                PluginHelper.UpdateIgnoredGForces();

            if (vessel == null)
                return;

            warpEngineThrottle = _engineThrottle[selected_factor];

            distanceToClosestBody = DistanceToClosestBody(vessel, out closestCelestrialBody, out selectedTargetVesselIsClosest);

            closestCelestrialBodyName = closestCelestrialBody.name;

            gravityPull = FlightGlobals.getGeeForceAtPosition(vessel.GetWorldPos3D()).magnitude;
            gravityAtSeaLevel = closestCelestrialBody.GeeASL * GameConstants.STANDARD_GRAVITY;
            gravityRatio = gravityAtSeaLevel > 0 ? Math.Min(1, gravityPull / gravityAtSeaLevel) : 0;
            gravityDragRatio = Math.Pow(Math.Min(1, 1 - gravityRatio), Math.Max(1, Math.Sqrt(gravityAtSeaLevel)));
            gravityDragPercentage = (1 - gravityDragRatio) * 100;

            maximumWarpForGravityPull = gravityPull > 0 ? 1 / gravityPull : 0;

            cosineAngleToClosestBody = Vector3d.Dot(part.transform.up.normalized, (vessel.CoMD - closestCelestrialBody.position).normalized);

            var cosineAngleModifier = selectedTargetVesselIsClosest ? 0.25 : (1 + 0.5 * cosineAngleToClosestBody);

            maximumWarpForAltitude = 0.1 * cosineAngleModifier * distanceToClosestBody / PluginSettings.Config.SpeedOfLight / TimeWarp.fixedDeltaTime;
            maximumWarpWeighted = (gravityRatio * maximumWarpForGravityPull) + ((1 - gravityRatio) * maximumWarpForAltitude);
            maximumWarpSpeed = Math.Min(maximumWarpWeighted, maximumWarpForAltitude);
            maximumWarpSpeedFactor = GetMaximumFactor(maximumWarpSpeed);
            maximumAllowedWarpThrotle = _engineThrottle[maximumWarpSpeedFactor];
            minimumPowerAllowedFactor = maximumWarpSpeedFactor > minimum_selected_factor ? maximumWarpSpeedFactor : minimum_selected_factor;

            if (alcubierreDrives != null)
                totalWarpPower = alcubierreDrives.Sum(p => p.warpStrength * (p.isupgraded ? warpPowerMultTech1 : warpPowerMultTech0));

            vesselTotalMass = vessel.GetTotalMass();

            if (alcubierreDrives != null && totalWarpPower != 0 && vesselTotalMass != 0)
            {
                warpToMassRatio = totalWarpPower / vesselTotalMass;
                exotic_power_required = (GameConstants.initial_alcubierre_megajoules_required * vesselTotalMass * powerRequirementMultiplier) / warpToMassRatio;

                // spread exotic matter over all coils
                var exoticMatterResource = part.Resources["ExoticMatter"];
                exoticMatterResource.maxAmount = exotic_power_required / alcubierreDrives.Count;
            }

            minPowerRequirementForLightSpeed = GetPowerRequirementForWarp(1);
            powerRequirementForSlowedSubLightSpeed = GetPowerRequirementForWarp(_engineThrottle.First());
            powerRequirementForMaximumAllowedLightSpeed = GetPowerRequirementForWarp(maximumAllowedWarpThrotle);
            currentPowerRequirementForWarp = GetPowerRequirementForWarp(_engineThrottle[selected_factor]);
        }


        private double GetHighestThrottleForAvailablePower()
        {
            foreach (var lightspeedFraction in _engineThrottle.Where(s => s <= maximumAllowedWarpThrotle).Reverse())
            {
                var requiredPower = GetPowerRequirementForWarp(lightspeedFraction);

                if (availablePower > requiredPower)
                    return lightspeedFraction;
            }

            return 1;
        }

        private void MaximizeWarpSpeed()
        {
            var fastestLightspeed = GetHighestThrottleForAvailablePower();

            if (fastestLightspeed > warpEngineThrottle)
            {
                selected_factor = _engineThrottle.IndexOf(fastestLightspeed);
                currentPowerRequirementForWarp = GetPowerRequirementForWarp(fastestLightspeed);
                warpEngineThrottle = fastestLightspeed;
            }
        }

        private double GetLowestThrottleForAvailablePower()
        {
            foreach (var lightspeedFraction in _engineThrottle.Where(s => s < 1))
            {
                var requiredPower = GetPowerRequirementForWarp(lightspeedFraction);

                if (availablePower > requiredPower)
                    return lightspeedFraction;
            }

            return 1;
        }

        private void MinimizeWarpSpeed()
        {
            var slowestSublightSpeed = GetLowestThrottleForAvailablePower();

            if (!(slowestSublightSpeed < warpEngineThrottle))
                return;

            selected_factor = _engineThrottle.IndexOf(slowestSublightSpeed);
            currentPowerRequirementForWarp = GetPowerRequirementForWarp(slowestSublightSpeed);
            warpEngineThrottle = slowestSublightSpeed;
        }

        private static double DeltaVToCircularize(Orbit orbit)
        {
            var rAp = orbit.ApR;
            var rPe = orbit.PeR;
            var mu = orbit.referenceBody.gravParameter;

            return Math.Abs(Sqrt(mu / rAp) - Sqrt((rPe * mu) / (rAp * (rPe + rAp) / 2)));
        }

        private static double Sqrt(double value)
        {
            if (value < 0)
                return -Math.Sqrt(-1 * value);
            else
                return Math.Sqrt(value);
        }

        //Computes the deltaV of the burn needed to circularize an orbit at a given UT.
        public static double DeltaVToCircularize(Orbit o, double UT)
        {
            var desiredVelocity = CircularOrbitSpeed(o.referenceBody, o.radius) ;
            var actualVelocity = o.getOrbitalVelocityAtUT(UT).magnitude;
            return desiredVelocity - actualVelocity;
        }

        //Computes the speed of a circular orbit of a given radius for a given body.
        private static double CircularOrbitSpeed(CelestialBody body, double radius)
        {
            return Math.Sqrt(body.gravParameter / radius);
        }

        private void WarpDriveCharging(IResourceManager resMan)
        {
            part.GetConnectedResourceTotals(exoticResourceDefinition.id, out currentExoticMatter, out maxExoticMatter);

            vesselTotalMass = vessel.GetTotalMass();
            if (totalWarpPower != 0 && vesselTotalMass != 0)
            {
                warpToMassRatio = totalWarpPower / vesselTotalMass;
                exotic_power_required = (GameConstants.initial_alcubierre_megajoules_required * vesselTotalMass * powerRequirementMultiplier) / warpToMassRatio;
                exoticMatterRatio = exotic_power_required > 0 ? Math.Min(1, currentExoticMatter / exotic_power_required) : 0;
            }
            else
            {
                exotic_power_required = 0;
                exoticMatterRatio = 0;
            }

            GenerateAntiGravity(resMan);

            // maintenance power depend on vessel mass and experienced geeforce

            var maximumExoticMaintenancePower = vesselTotalMass * powerRequirementMultiplier * vessel.gravityForPos.magnitude * gravityMaintenancePowerMultiplier;
            var minimumExoticMaintenancePower = 0.5 * 0.5 * maximumExoticMaintenancePower;

            requiredExoticMaintenancePower = exoticMatterRatio * exoticMatterRatio * maximumExoticMaintenancePower;

            var overheatingRatio = resMan.ResourceFillFraction(ResourceName.WasteHeat); 

            var overheatModifier = overheatingRatio < 0.9 ? 1 : (1 - overheatingRatio) * 10;

            receivedExoticMaintenancePower = CheatOptions.InfiniteElectricity
                   ? requiredExoticMaintenancePower
                   : resMan.ConsumeResource(ResourceName.ElectricCharge, requiredExoticMaintenancePower * GameConstants.ecPerMJ) / GameConstants.ecPerMJ;

            minimumExoticMatterMaintenanceRatio = minimumExoticMaintenancePower > 0 ? receivedExoticMaintenancePower / minimumExoticMaintenancePower : 0;

            exoticMatterMaintenanceRatio = requiredExoticMaintenancePower > 0 ? receivedExoticMaintenancePower / requiredExoticMaintenancePower : 0;

            ProduceWasteheat(resMan, receivedExoticMaintenancePower);

            exoticMatterProduced = (1 - exoticMatterMaintenanceRatio) * -maxExoticMatter;

            if ((IsCharging || antigravityPercentage > 0 || exoticMatterRatio > 0 ) && !IsEnabled)
            {
                availablePower = CheatOptions.InfiniteElectricity
                    ? currentPowerRequirementForWarp
                    : stablePowerSupply;

                maxChargePowerRequired = ((antigravityPercentage * 0.005 * exotic_power_required) - currentExoticMatter) * 1000;

                if (maxChargePowerRequired < 0)
                {
                    exoticMatterProduced += maxChargePowerRequired;
                    chargePowerDraw = 0;
                }
                else
                {
                    // TODO fix me 
                    // stablePowerSupply = getAvailableStableSupply(KITResourceSettings.ElectricPowerInMegawatt);
                    stablePowerSupply = 10000;

                    // fix me, should not use fdt here
                    chargePowerDraw = CheatOptions.InfiniteElectricity
                        ? maxChargePowerRequired
                        : Math.Min(maxChargePowerRequired / TimeWarp.fixedDeltaTime, Math.Max(minPowerRequirementForLightSpeed, stablePowerSupply));

                    var resourceBarRatio = resMan.ResourceFillFraction(ResourceName.ElectricCharge);
                    var effectiveResourceThrottling = resourceBarRatio > 0.5 ? 1 : resourceBarRatio * 2;

                    exoticMatterProduced = CheatOptions.InfiniteElectricity
                        ? chargePowerDraw
                        : resMan.ConsumeResource(ResourceName.ElectricCharge, overheatModifier * chargePowerDraw * effectiveResourceThrottling * GameConstants.ecPerMJ);

                    if (!CheatOptions.InfinitePropellant && stablePowerSupply < minPowerRequirementForLightSpeed)
                        insufficientPowerTimeout--;
                    else
                        insufficientPowerTimeout = maxPowerTimeout;

                    if (insufficientPowerTimeout < 0)
                    {
                        insufficientPowerTimeout--;

                        var message = overheatModifier < 0.99 ? Localizer.Format("#LOC_KSPIE_AlcubierreDrive_msg3") :  //"Shutdown Alcubierre Drive due to overheating"
                            Localizer.Format("#LOC_KSPIE_AlcubierreDrive_notEnoughElectricPowerForWarp");

                        Debug.Log("[KSPI]: " + message);
                        ScreenMessages.PostScreenMessage(message, 5.0f, ScreenMessageStyle.UPPER_CENTER);
                        StopCharging();

                        return;
                    }
                }

                ProduceWasteheat(resMan, exoticMatterProduced);
            }

            part.RequestResource(KITResourceSettings.ExoticMatter, -exoticMatterProduced * 0.001 * TimeWarp.fixedDeltaTime / powerRequirementMultiplier);
        }

        private void GenerateAntiGravity(IResourceManager resMan)
        {
            exoticMatterRatio = exotic_power_required > 0 ? Math.Min(1, currentExoticMatter / exotic_power_required) : 0;

            gravityAcceleration = vessel.gravityForPos.magnitude;

            var antigravityForceVector = vessel.gravityForPos * -exoticMatterRatio * antigravityMultiplier;

            antigravityAcceleration = antigravityForceVector.magnitude;

            if (antigravityAcceleration > 0)
                TimeWarp.GThreshold = GThreshold;

            if (!double.IsNaN(antigravityForceVector.x) && !double.IsNaN(antigravityForceVector.y) && !double.IsNaN(antigravityForceVector.z))
            {
                if (vessel.packed)
                    vessel.orbit.Perturb(antigravityForceVector * resMan.FixedDeltaTime(), Planetarium.GetUniversalTime());
                else
                    vessel.ChangeWorldVelocity(antigravityForceVector * resMan.FixedDeltaTime());
            }

            verticalSpeed = vessel.verticalSpeed;

            // TODO fixme
            // stablePowerSupply = getAvailableStableSupply(KITResourceSettings.ElectricPowerInMegawatt);
            stablePowerSupply = 10000; 

            if (holdAltitude)
            {
                orbitMultiplier = vessel.orbit.PeA > vessel.mainBody.atmosphereDepth ? 0 : 1 - Math.Min(1, vessel.horizontalSrfSpeed  /  CircularOrbitSpeed(vessel.mainBody, vessel.mainBody.Radius + vessel.altitude));
                responseFactor = responseMultiplier * stablePowerSupply / exotic_power_required;
                antigravityPercentage = (float)Math.Max(0, Math.Min(100 * orbitMultiplier + (gravityAcceleration != 0 ? responseFactor * -verticalSpeed / gravityAcceleration / resMan.FixedDeltaTime() : 0), 200));
            }
        }

        private void ProduceWasteheat(IResourceManager resMan, double powerReturned)
        {
            resMan.ProduceResource(ResourceName.WasteHeat, powerReturned * (isupgraded ? wasteheatRatioUpgraded : wasteheatRatio));
        }

        private double GetPowerRequirementForWarp(double lightspeedFraction)
        {
            var sqrtSpeed = Math.Sqrt(lightspeedFraction);

            var powerModifier = lightspeedFraction < 1
                ? 1 / sqrtSpeed
                : sqrtSpeed;

            return powerModifier * exotic_power_required * warpPowerReqMult;
        }

        private void UpdateWarpSpeed(IResourceManager resMan)
        {
            if (!IsEnabled || exotic_power_required <= 0) return;

            var selectedLightSpeed = _engineThrottle[selected_factor];

            currentPowerRequirementForWarp = GetPowerRequirementForWarp(selectedLightSpeed);

            /*
             * TODO fixme
            availablePower = CheatOptions.InfiniteElectricity
                ? currentPowerRequirementForWarp
                : getAvailableStableSupply(KITResourceSettings.ElectricPowerInMegawatt);
            */

            availablePower = 10000;

            double powerReturned;

            if (CheatOptions.InfiniteElectricity)
                powerReturned = currentPowerRequirementForWarp;
            else
            {
                powerReturned = resMan.ConsumeResource(ResourceName.ElectricCharge, currentPowerRequirementForWarp * GameConstants.ecPerMJ);
                ProduceWasteheat(resMan, powerReturned);
            }

            var headingModifier = FlightGlobals.fetch.VesselTarget == null ? Math.Abs(Math.Min(0, cosineAngleToClosestBody)) : 1;

            allowedWarpDistancePerFrame = PluginSettings.Config.SpeedOfLight * resMan.FixedDeltaTime() * selectedLightSpeed * headingModifier;

            safetyDistance = FlightGlobals.fetch.VesselTarget == null ? spaceSafetyDistance * 1000 : 0;

            var minimumSpaceAltitude = FlightGlobals.fetch.VesselTarget == null ? (closestCelestrialBody.atmosphere ? closestCelestrialBody.atmosphereDepth : 0) : 0;

            dropoutDistance = minimumSpaceAltitude + allowedWarpDistancePerFrame + safetyDistance;

            if ((!CheatOptions.IgnoreMaxTemperature && vessel.atmDensity > 0) ||   distanceToClosestBody <= dropoutDistance)
            {
                if (vesselWasInOuterspace)
                {
                    var message = FlightGlobals.fetch.VesselTarget == null
                        ? closestCelestrialBody.atmosphere
                            ? "#LOC_KSPIE_AlcubierreDrive_droppedOutOfWarpTooCloseToAtmosphere"
                            : "#LOC_KSPIE_AlcubierreDrive_droppedOutOfWarpTooCloseToSurface"
                        : "#LOC_KSPIE_AlcubierreDrive_msg4";//"Dropped out of warp near target"

                    Debug.Log("[KSPI]: " + Localizer.Format(message));
                    ScreenMessages.PostScreenMessage(Localizer.Format(message), 5);
                    DeactivateWarpDrive(true);
                    vesselWasInOuterspace = false;
                    return;
                }
            }
            else
                vesselWasInOuterspace = true;

            // detect power shortage
            if (currentPowerRequirementForWarp > availablePower)
                insufficientPowerTimeout = -1;
            else if (powerReturned < 0.99 * currentPowerRequirementForWarp)
                insufficientPowerTimeout--;
            else
                insufficientPowerTimeout = 10;

            // retrieve vessel heading
            var newPartHeading = new Vector3d(part.transform.up.x, part.transform.up.z, part.transform.up.y);

            // detect any changes in vessel heading and heading stability
            magnitudeDiff = (active_part_heading - newPartHeading).magnitude;

            // determine if we need to change speed and heading
            var hasPowerShortage = insufficientPowerTimeout < 0;
            var hasHeadingChanged = magnitudeDiff > 0.0001 && counterCurrent > counterPreviousChange + headingChangedTimeout && allowWarpTurning;

            // Speedup to maximum speed when possible and requested
            if (maximizeWarpSpeed && magnitudeDiff <= 0.0001 && maximumWarpSpeedFactor > selected_factor)
            {
                MaximizeWarpSpeed();
            }

            var hasWarpFactorChange = Math.Abs(existing_warp_speed - selectedLightSpeed) > float.Epsilon;
            var hasGravityPullImbalance = maximumWarpSpeedFactor < selected_factor;

            if (hasGravityPullImbalance)
                selected_factor = maximumWarpSpeedFactor;

            if (!CheatOptions.InfiniteElectricity && hasPowerShortage)
            {
                if (availablePower < minPowerRequirementForLightSpeed)
                {
                    var message = Localizer.Format("#LOC_KSPIE_AlcubierreDrive_msg5", availablePower.ToString("0"), minPowerRequirementForLightSpeed.ToString("0"));//"Maximum power supply of " +  + " MW is insufficient power, you need at at least " +  + " MW of Power to maintain Lightspeed with current vessel. Please increase power supply, lower vessel mass or increase Warp Drive mass."
                    Debug.Log("[KSPI]: " + message);
                    ScreenMessages.PostScreenMessage(message, 5);
                    DeactivateWarpDrive(false);
                    return;
                }

                if (selected_factor == minimumPowerAllowedFactor || selected_factor == minimum_selected_factor ||
                    (selectedLightSpeed < 1 && warpEngineThrottle >= maximumAllowedWarpThrotle && powerReturned < 0.99 * currentPowerRequirementForWarp))
                {
                    string message;
                    if (powerReturned < 0.99 * currentPowerRequirementForWarp)
                        message = Localizer.Format("#LOC_KSPIE_AlcubierreDrive_criticalPowerSupplyAt") + " " + (powerReturned / currentPowerRequirementForWarp).ToString("P1") + " " + Localizer.Format("#LOC_KSPIE_AlcubierreDrive_deactivatingWarpDrive");
                    else
                        message = Localizer.Format("#LOC_KSPIE_AlcubierreDrive_criticalPowerShortageWhileAtMinimumSpeed") + " " + Localizer.Format("#LOC_KSPIE_AlcubierreDrive_deactivatingWarpDrive");

                    Debug.Log("[KSPI]: " + message);
                    ScreenMessages.PostScreenMessage(message, 5);
                    DeactivateWarpDrive(true);
                    return;
                }
                var insufficientMessage = Localizer.Format("#LOC_KSPIE_AlcubierreDrive_insufficientPowerPercentageAt") + " " + (powerReturned / currentPowerRequirementForWarp).ToString("P1") + " " + Localizer.Format("#LOC_KSPIE_AlcubierreDrive_reducingElectricPowerDrain");
                Debug.Log("[KSPI]: " + insufficientMessage);
                ScreenMessages.PostScreenMessage(insufficientMessage, 5);
                ReduceWarpPower();
            }

            if (!hasWarpFactorChange && !hasPowerShortage && !hasHeadingChanged && !hasGravityPullImbalance)
                return;

            if (hasHeadingChanged)
                counterPreviousChange = counterCurrent;

            existing_warp_speed = _engineThrottle[selected_factor];

            var reverseHeading = new Vector3d(-heading_act.x, -heading_act.y, -heading_act.z);

            heading_act = newPartHeading * PluginSettings.Config.SpeedOfLight * existing_warp_speed;
            serialisedwarpvector = ConfigNode.WriteVector(heading_act);

            active_part_heading = newPartHeading;

            if (!vessel.packed && useRotateStability)
                OrbitPhysicsManager.HoldVesselUnpack();

            // puts the ship back into a simulated orbit and re-enables physics, is this still needed?
            if (!vessel.packed)
                vessel.GoOnRails();

            vessel.orbit.UpdateFromStateVectors(vessel.orbit.pos, vessel.orbit.vel + reverseHeading + heading_act, vessel.orbit.referenceBody, Planetarium.GetUniversalTime());

            // disables physics and puts the ship into a propagated orbit , is this still needed?
            if (!vessel.packed)
                vessel.GoOffRails();

            // disable warp trail for 2 frames to prevent graphic glitches
            warpTrailTimeout = 2;

            UpdateAllWarpDriveEffects();
        }

        private void Develocitize()
        {
            Debug.Log("[KSPI]: Develocitize");

            // This code is inspired quite heavily by HyperEdit's OrbitEditor.cs
            double universalTime = Planetarium.GetUniversalTime();
            Orbit currentOrbit = vessel.orbitDriver.orbit;
            Vector3d currentOrbitalVelocity = currentOrbit.getOrbitalVelocityAtUT(universalTime);
            Vector3d progradeNormalizedVelocity = currentOrbitalVelocity.normalized;
            Vector3d velocityToCancel = currentOrbitalVelocity;

            // apply gravity drag modifier
            velocityToCancel *= gravityDragRatio;

            // Extremely small velocities cause the game to mess up very badly, so try something small and increase...
            long multiplier = 0;
            Orbit newOrbit;
            do
            {
                Vector3d retrogradeNormalizedVelocity = progradeNormalizedVelocity * -multiplier;

                newOrbit = new Orbit(currentOrbit);
                newOrbit.UpdateFromStateVectors(currentOrbit.pos, retrogradeNormalizedVelocity - velocityToCancel, currentOrbit.referenceBody, universalTime);

                multiplier += 1;
            } while (multiplier < 10000 && double.IsNaN(newOrbit.getOrbitalVelocityAtUT(universalTime).magnitude));

            vessel.Landed = false;
            vessel.Splashed = false;
            vessel.landedAt = Empty;

            // I'm actually not sure what this is for... but HyperEdit does it.
            // I had weird problems when I took it out, anyway.
            try
            {
                OrbitPhysicsManager.HoldVesselUnpack(60);
            }
            catch (NullReferenceException)
            {
                Debug.LogWarning("[KSPI]: NullReferenceException during Develocitize");
            }
            var allVessels = FlightGlobals.fetch == null
                ? (IEnumerable<Vessel>)new[] { vessel }
                : FlightGlobals.Vessels;

            foreach (var currentVessel in allVessels.Where(v => v.packed == false))
            {
                currentVessel.GoOnRails();
            }
            // End HyperEdit code I don't really understand

            currentOrbit.inclination = newOrbit.inclination;
            currentOrbit.eccentricity = newOrbit.eccentricity;
            currentOrbit.semiMajorAxis = newOrbit.semiMajorAxis;
            currentOrbit.LAN = newOrbit.LAN;
            currentOrbit.argumentOfPeriapsis = newOrbit.argumentOfPeriapsis;
            currentOrbit.meanAnomalyAtEpoch = newOrbit.meanAnomalyAtEpoch;
            currentOrbit.epoch = newOrbit.epoch;
            currentOrbit.Init();
            currentOrbit.UpdateFromUT(universalTime);

            vessel.orbitDriver.pos = vessel.orbit.pos.xzy;
            vessel.orbitDriver.vel = vessel.orbit.vel;
        }

        // ReSharper disable once UnusedMember.Global
        public void OnGUI()
        {
            if (vessel == FlightGlobals.ActiveVessel && showWindow)
                windowPosition = GUILayout.Window(_windowID, windowPosition, Window, Localizer.Format("#LOC_KSPIE_AlcubierreDrive_warpControlWindow"));
        }

        private void PrintToGUILayout(string label, string value, GUIStyle boldStyle, GUIStyle textStyle, int widthLabel = 170, int widthValue = 130)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, boldStyle, GUILayout.Width(widthLabel));
            GUILayout.Label(value, textStyle, GUILayout.Width(widthValue));
            GUILayout.EndHorizontal();
        }

        private void Window(int windowId)
        {
            try
            {
                windowPositionX = windowPosition.x;
                windowPositionY = windowPosition.y;

                InitializeStyles();

                if (GUI.Button(new Rect(windowPosition.width - 20, 2, 18, 18), "x"))
                    showWindow = false;

                GUILayout.BeginVertical();

                PrintToGUILayout(Localizer.Format("#LOC_KSPIE_AlcubierreDrive_warpdriveType"), part.partInfo.title, bold_black_style, text_black_style);
                PrintToGUILayout(Localizer.Format("#LOC_KSPIE_AlcubierreDrive_totalWarpPower"), totalWarpPower.ToString("0.0") + " t", bold_black_style, text_black_style);
                PrintToGUILayout(Localizer.Format("#LOC_KSPIE_AlcubierreDrive_engineMass"), vesselTotalMass.ToString("0.000") + " t", bold_black_style, text_black_style);
                PrintToGUILayout(Localizer.Format("#LOC_KSPIE_AlcubierreDrive_warpToMassRatio"), "1:" + warpToMassRatio.ToString("0.000"), bold_black_style, text_black_style);
                PrintToGUILayout(Localizer.Format("#LOC_KSPIE_AlcubierreDrive_gravityAtSeaLevel"), gravityAtSeaLevel.ToString("0.00000") + " m/s\xB2", bold_black_style, text_black_style);
                PrintToGUILayout(Localizer.Format("#LOC_KSPIE_AlcubierreDrive_gravityVesselPull"), gravityPull.ToString("0.00000") + " m/s\xB2", bold_black_style, text_black_style);
                PrintToGUILayout(Localizer.Format("#LOC_KSPIE_AlcubierreDrive_gravityDragPercentage"), gravityDragPercentage.ToString("0.000") + "%", bold_black_style, text_black_style);
                PrintToGUILayout(Localizer.Format("#LOC_KSPIE_AlcubierreDrive_maxAllowedThrotle"), maximumAllowedWarpThrotle.ToString("0.0000") + " c", bold_black_style, text_black_style);
                PrintToGUILayout(Localizer.Format("#LOC_KSPIE_AlcubierreDrive_currentSelectedSpeed"), warpEngineThrottle.ToString("0.0000") + " c", bold_black_style, text_black_style);
                PrintToGUILayout(Localizer.Format("#LOC_KSPIE_AlcubierreDrive_currentPowerReqForWarp"), PluginHelper.getFormattedPowerString(currentPowerRequirementForWarp), bold_black_style, text_black_style);
                PrintToGUILayout(Localizer.Format("#LOC_KSPIE_AlcubierreDrive_currentWarpExitSpeed"), exitSpeed.ToString("0.000") + " m/s", bold_black_style, text_black_style);
                PrintToGUILayout(Localizer.Format("#LOC_KSPIE_AlcubierreDrive_currentWarpExitApoapsis"), exitApoapsis.ToString("0.000") + " km", bold_black_style, text_black_style);
                PrintToGUILayout(Localizer.Format("#LOC_KSPIE_AlcubierreDrive_currentWarpExitPeriapsis"), exitPeriapsis.ToString("0.000") + " km", bold_black_style, text_black_style);
                PrintToGUILayout(Localizer.Format("#LOC_KSPIE_AlcubierreDrive_currentWarpExitEccentricity"), exitEccentricity.ToString("0.000"), bold_black_style, text_black_style);
                PrintToGUILayout(Localizer.Format("#LOC_KSPIE_AlcubierreDrive_currentWarpExitMeanAnomaly"), exitMeanAnomaly.ToString("0.000") + "\xB0", bold_black_style, text_black_style);
                PrintToGUILayout(Localizer.Format("#LOC_KSPIE_AlcubierreDrive_currentWarpExitBurnToCircularize"), exitBurnCircularize.ToString("0.000") + " m/s", bold_black_style, text_black_style);

                PrintToGUILayout(Localizer.Format("#LOC_KSPIE_AlcubierreDrive_MaximumWarpForAltitude"), (maximumWarpForAltitude).ToString("0.000"), bold_black_style, text_black_style);//"Maximum Warp For Altitude"
                PrintToGUILayout(Localizer.Format("#LOC_KSPIE_AlcubierreDrive_Distancetoclosestbody"), (distanceToClosestBody * 0.001).ToString("0.000") + " km" , bold_black_style, text_black_style);//"Distance to closest body"
                PrintToGUILayout(Localizer.Format("#LOC_KSPIE_AlcubierreDrive_Closestbody"), closestCelestrialBodyName, bold_black_style, text_black_style);//"Closest body"

                PrintToGUILayout(Localizer.Format("#LOC_KSPIE_AlcubierreDrive_CosineToClosestBody"), cosineAngleToClosestBody.ToString("0.000"), bold_black_style, text_black_style);//"Cosine To Closest Body"

                PrintToGUILayout(Localizer.Format("#LOC_KSPIE_AlcubierreDrive_status"), driveStatus, bold_black_style, text_black_style);

                var speedText = Localizer.Format("#LOC_KSPIE_AlcubierreDrive_speed");

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("(-) " + speedText, GUILayout.MinWidth(150)))
                    ToggleWarpSpeedDown();
                if (GUILayout.Button("(+) " + speedText, GUILayout.MinWidth(150)))
                    ToggleWarpSpeedUp();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("(-) " + speedText + " x3", GUILayout.MinWidth(150)))
                    ToggleWarpSpeedDown3();
                if (GUILayout.Button("(+) " + speedText + " x3", GUILayout.MinWidth(150)))
                    ToggleWarpSpeedUp3();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("(-) " + speedText + " x10", GUILayout.MinWidth(150)))
                    ToggleWarpSpeedDown10();
                if (GUILayout.Button("(+) " + speedText + " x10", GUILayout.MinWidth(150)))
                    ToggleWarpSpeedUp10();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("(-) " + speedText + " MIN", GUILayout.MinWidth(150)))
                    MinimizeWarpSpeed();
                if (GUILayout.Button("(+) " + speedText + " MAX", GUILayout.MinWidth(150)))
                    MaximizeWarpSpeed ();
                GUILayout.EndHorizontal();

                if (!IsEnabled && GUILayout.Button(Localizer.Format("#LOC_KSPIE_AlcubierreDrive_activateWarpDrive"), GUILayout.ExpandWidth(true)))
                    ActivateWarpDrive();

                if (IsEnabled && GUILayout.Button(Localizer.Format("#LOC_KSPIE_AlcubierreDrive_deactivateWarpDrive"), GUILayout.ExpandWidth(true)))
                    DeactivateWarpDrive(true);

                if (!IsEnabled && !IsCharging && GUILayout.Button(Localizer.Format("#LOC_KSPIE_AlcubierreDrive_startChargingDrive"), GUILayout.ExpandWidth(true)))
                    StartCharging();

                if (!IsEnabled && IsCharging && GUILayout.Button(Localizer.Format("#LOC_KSPIE_AlcubierreDrive_stopChargingDrive"), GUILayout.ExpandWidth(true)))
                    StopCharging();

                GUILayout.EndVertical();
                GUI.DragWindow();
            }
            catch (Exception e)
            {
                Debug.LogError("[KSPI]: AlcubierreDrive Window(" + windowId + "): " + e.Message);
            }
        }

        private void InitializeStyles()
        {
            if (bold_black_style == null)
            {
                bold_black_style = new GUIStyle(GUI.skin.label)
                {
                    fontStyle = FontStyle.Bold,
                    font = PluginHelper.MainFont
                };
            }

            if (text_black_style == null)
            {
                text_black_style = new GUIStyle(GUI.skin.label)
                {
                    fontStyle = FontStyle.Normal,
                    font = PluginHelper.MainFont
                };
            }
        }

        private static double DistanceToClosestBody(Vessel vessel, out CelestialBody closestBody,
            out bool targetVesselIsClosest)
        {
            var minimumDistance = vessel.altitude;
            closestBody = vessel.mainBody;
            targetVesselIsClosest = false;

            var vesselTarget = FlightGlobals.fetch.VesselTarget;
            if (vesselTarget != null)
            {
                var transform = vesselTarget.GetTransform();
                var toTarget = vessel.CoMD - transform.position;
                var distanceToTarget = toTarget.magnitude;

                if (distanceToTarget < minimumDistance)
                {
                    minimumDistance = distanceToTarget;
                    targetVesselIsClosest = true;
                }
            }

            if (vessel.orbit.closestEncounterBody != null)
            {
                var celestialBody = vessel.orbit.closestEncounterBody;
                var toBody = vessel.CoMD - celestialBody.position;
                var distanceToSurfaceBody = toBody.magnitude - celestialBody.Radius;

                if (distanceToSurfaceBody < minimumDistance)
                {
                    minimumDistance = distanceToSurfaceBody;
                    closestBody = celestialBody;
                    targetVesselIsClosest = false;
                }
            }

            if (vessel.mainBody.orbit != null && vessel.mainBody.orbit.referenceBody != null)
            {
                var celestialBody = vessel.mainBody.orbit.referenceBody;
                var toBody = vessel.CoMD - celestialBody.position;
                var distanceToSurfaceBody = toBody.magnitude - celestialBody.Radius;

                if (distanceToSurfaceBody < minimumDistance)
                {
                    minimumDistance = distanceToSurfaceBody;
                    closestBody = celestialBody;
                    targetVesselIsClosest = false;
                }

                foreach (var moon in celestialBody.orbitingBodies)
                {
                    var toMoon = vessel.CoMD - moon.position;
                    var distanceToSurfaceMoon = toMoon.magnitude - moon.Radius;

                    if (!(distanceToSurfaceMoon < minimumDistance)) continue;

                    minimumDistance = distanceToSurfaceMoon;
                    closestBody = moon;
                    targetVesselIsClosest = false;
                }
            }

            foreach (var planet in vessel.mainBody.orbitingBodies)
            {
                var toPlanet = vessel.CoMD - planet.position;
                var distanceToSurfacePlanet = toPlanet.magnitude - planet.Radius;

                if (distanceToSurfacePlanet < minimumDistance)
                {
                    minimumDistance = distanceToSurfacePlanet;
                    closestBody = planet;
                }

                foreach (var moon in planet.orbitingBodies)
                {
                    var toMoon = vessel.CoMD - moon.position;
                    var distanceToSurfaceMoon = toMoon.magnitude - moon.Radius;

                    if (!(distanceToSurfaceMoon < minimumDistance)) continue;

                    minimumDistance = distanceToSurfaceMoon;
                    closestBody = moon;
                    targetVesselIsClosest = false;
                }
            }

            return minimumDistance;
        }

        public ResourcePriorityValue ResourceProcessPriority()
        {
            throw new NotImplementedException();
        }

        public void KITFixedUpdate(IResourceManager resMan)
        {
            distanceToClosestBody = DistanceToClosestBody(vessel, out closestCelestrialBody, out selectedTargetVesselIsClosest);

            if (initiateWarpTimeout > 0)
                InitiateWarp(resMan);

            Orbit currentOrbit = vessel.orbitDriver.orbit;
            double universalTime = Planetarium.GetUniversalTime();

            if (IsEnabled)
            {
                counterCurrent++;

                // disable any geeforce effects during warp
                PluginHelper.IgnoreGForces(part, 2);
                var reverseHeadingWarp = new Vector3d(-heading_act.x, -heading_act.y, -heading_act.z);
                Vector3d currentOrbitalVelocity = vessel.orbitDriver.orbit.getOrbitalVelocityAtUT(universalTime);
                Vector3d newDirection = currentOrbitalVelocity + reverseHeadingWarp;

                Orbit predictedExitOrbit;

                long multiplier = 0;
                do
                {
                    // first predict dropping out of warp
                    predictedExitOrbit = new Orbit(currentOrbit);
                    predictedExitOrbit.UpdateFromStateVectors(currentOrbit.pos, newDirection, vessel.orbit.referenceBody, universalTime);

                    // then calculated predicted gravity breaking
                    if (warpInitialMainBody != null && vessel.mainBody != warpInitialMainBody && !KopernicusHelper.IsStar(part.vessel.mainBody))
                    {
                        Vector3d retrogradeNormalizedVelocity = newDirection.normalized * -multiplier;
                        Vector3d velocityToCancel = predictedExitOrbit.getOrbitalVelocityAtUT(universalTime) * gravityDragRatio;

                        predictedExitOrbit.UpdateFromStateVectors(currentOrbit.pos, retrogradeNormalizedVelocity - velocityToCancel, currentOrbit.referenceBody, universalTime);
                    }

                    multiplier += 1;
                } while (multiplier < 10000 && double.IsNaN(predictedExitOrbit.getOrbitalVelocityAtUT(universalTime).magnitude));

                // update expected exit orbit data
                exitPeriapsis = predictedExitOrbit.PeA * 0.001;
                exitApoapsis = predictedExitOrbit.ApA * 0.001;
                exitSpeed = predictedExitOrbit.getOrbitalVelocityAtUT(universalTime).magnitude;
                exitEccentricity = predictedExitOrbit.eccentricity * 180 / Math.PI;
                exitMeanAnomaly = predictedExitOrbit.meanAnomaly;
                exitBurnCircularize = DeltaVToCircularize(predictedExitOrbit);
            }
            else
            {
                exitPeriapsis = currentOrbit.PeA * 0.001;
                exitApoapsis = currentOrbit.ApA * 0.001;
                exitSpeed = currentOrbit.getOrbitalVelocityAtUT(universalTime).magnitude;
                exitEccentricity = currentOrbit.eccentricity;
                exitMeanAnomaly = currentOrbit.meanAnomaly * 180 / Math.PI;
                exitBurnCircularize = DeltaVToCircularize(currentOrbit);
            }

            warpEngineThrottle = _engineThrottle[selected_factor];

            warp_sound.pitch = (float)Math.Pow(warpEngineThrottle, warpSoundPitchExp);
            warp_sound.volume = GameSettings.SHIP_VOLUME;

            tex_count += warpEngineThrottle;

            if (!IsSlave)
            {
                WarpDriveCharging(resMan);
                UpdateWarpSpeed(resMan);
            }

            // update animation
            if (IsEnabled)
            {
                driveStatus = Localizer.Format("#LOC_KSPIE_AlcubierreDrive_Active");//"Active."
            }
            else
            {
                if (!IsSlave)
                {
                    part.GetConnectedResourceTotals(exoticResourceDefinition.id, out currentExoticMatter, out maxExoticMatter);

                    if (currentExoticMatter < exotic_power_required * 0.999 * 0.5)
                    {
                        var electricalCurrentPct = Math.Min(100, 100 * currentExoticMatter / (exotic_power_required * 0.5));
                        driveStatus = Localizer.Format("#LOC_KSPIE_AlcubierreDrive_Charging") + electricalCurrentPct.ToString("0.00") + "%";//String.Format("Charging: ")
                    }
                    else
                        driveStatus = Localizer.Format("#LOC_KSPIE_AlcubierreDrive_Ready");//"Ready."
                }
            }



            if (activeTrail && !hideTrail && warpTrailTimeout == 0)
            {
                var shipPos = new Vector3(part.transform.position.x, part.transform.position.y, part.transform.position.z);
                var endBeamPos = shipPos + part.transform.up * warp_size;
                var midPos = (shipPos - endBeamPos) / 2f;

                warp_effect.transform.rotation = part.transform.rotation;
                warp_effect.transform.localScale = new Vector3(effectSize1, midPos.magnitude, effectSize1);
                warp_effect.transform.position = new Vector3(shipPos.x + midPos.x, shipPos.y + midPos.y, shipPos.z + midPos.z);
                warp_effect.transform.rotation = part.transform.rotation;

                warp_effect2.transform.rotation = part.transform.rotation;
                warp_effect2.transform.localScale = new Vector3(effectSize2, midPos.magnitude, effectSize2);
                warp_effect2.transform.position = new Vector3(shipPos.x + midPos.x, shipPos.y + midPos.y, shipPos.z + midPos.z);
                warp_effect2.transform.rotation = part.transform.rotation;

                warp_effect1_renderer.material.mainTexture = warp_textures[((int)tex_count) % warp_textures.Length];
                warp_effect2_renderer.material.mainTexture = warp_textures2[((int)tex_count + 8) % warp_textures2.Length];

                warp_effect2_renderer.enabled = true;
                warp_effect1_renderer.enabled = true;
            }
            else
            {
                warp_effect2_renderer.enabled = false;
                warp_effect1_renderer.enabled = false;
            }

            if (warpTrailTimeout > 0)
                warpTrailTimeout--;
        }

        public string KITPartName() => part.partInfo.title;
    }
}