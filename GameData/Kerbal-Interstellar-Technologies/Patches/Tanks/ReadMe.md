Tank Config ReadMe
==================

Do not rely on other peoples configs and prefix our definitions with `KIT_`
prefix. Single tank resources are specified in the SUBTYPE directly, dual tank
reference B9_TANK_TYPE defined in `MixedLiquidTypes.cfg`.
Flight-switchable may not contains addedMass/addedCost declrarations.
Generate tailored configs for CT, CDT, RFC and generic ones for the rest.

There is also a hard-coded tank type called "Structural" which has zero mass and cost and no resources. It is the default tank type for any subtypes which do not have another one defined.

Docs:
https://github.com/blowfishpro/B9PartSwitch/wiki/ModuleB9PartSwitch
https://github.com/sarbian/ModuleManager/wiki/Module-Manager-Handbook
https://github.com/KSP-RO/ProceduralParts/wiki/Config-Parameter-Documentation

Part definition exports variable when it wants to be patched with fuel configs:

`RESOURCE {
  name = LiterVolume
  maxAmount = 1000
  TankType = ...
}`

TankType = Liquid
TankType = Double
(TankType = Solid)
(TankType = Radioactive)
(TankType = LightCryo (Hydrogen and other light cryogenic fuels))

Todo:

* wetworks - B9 does not support crew...
* antimatter
* verify dry mass and cost of all touched parts
* capacitors
* disable certain configs when mods are present/absent
* light fuel tanks for light resources only
* generic ore and rcs tanks
* K&K radioactive tanks
* flight switchable tanks
* nertea's CryoTanks (they look good!)
* support [SMURFF](https://forum.kerbalspaceprogram.com/index.php?/topic/117992-17-19-smurff-simple-module-adjustments-for-real-ish-fuel-mass-fractions-191-02019-nov-12/)

Flight switchable: CT, CC, wetworks, k&k, mk3

Discounted: (tbd)

Our tanks left to configue:
  RV Matter drop tank
  crashpad: deleted, because kit does not use gases
  RV Antimatter
  ChargedParticleTrap: nice model, transforms
  toroidal: two similar antimatter tanks and a disk
  PositronTanks[12]: two sizes of tank
  AntimatterTrap: tank for antimatter
  AntimatterTanks[12]: two size of tank
  tanks on converters
  UniversalStorage: ??
