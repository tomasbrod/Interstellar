Tank Config ReadMe
==================

The generate.php scrip generates two kinds of patches, specific, which target
exact parts from this mod, and generic, which target any part having a
LiterVolume definition.

Specific patch files:

* LiquidCT.cfg : CT\* liquid fuel tanks
* SolidCC.cfg : CC\* solid cargo containers
* NuclearRFC.cfg : RFC\* radioactive fuel containers

Generic patch files:

* GenericLiquid
* GenericDual
* GenericSolid
* GenericNuclear
* GenericLight

Generated `ProceduralPartsTanks.cfg` adds a tank using Procedural Parts shape
and fuel switcher module.

GenericDual patch relies on generated file `MixedLiquidTypes.cfg` for the
tankType. Single-resource configurations specify the RESOURCE definitions
directly in the SUBTYPE, because it is shorter.

Static `Transforms.cfg` fixes some inconsistent model transforms.

In static file `Generic.cfg`, stock tanks are patched with LiterVolume
definition, marking them ready for generic patches.

LiterVolume
-----------

Part definition exports variable when it wants to be patched with fuel configs:

```
  RESOURCE {
    name = LiterVolume
    maxAmount = 1000
    TankType = ...
  }
```

* TankType = Liquid
* TankType = Double
* TankType = Solid
* TankType = Nuclear
* TankType = LightCryo (Hydrogen and other light cryogenic fuels)

Dev Notes
------

Do not rely on other peoples configs and prefix our definitions with `KIT_`
prefix. Single tank resources are specified in the SUBTYPE directly, dual tank
reference B9_TANK_TYPE defined in `MixedLiquidTypes.cfg`.
Flight-switchable may not contains addedMass/addedCost declrarations.
Generate tailored configs for CT, CDT, RFC and generic ones for the rest.

There is also a hard-coded tank type called "Structural" which has zero mass
and cost and no resources. It is the default tank type for any subtypes which
do not have another one defined.

Docs:
https://github.com/blowfishpro/B9PartSwitch/wiki/ModuleB9PartSwitch
https://github.com/sarbian/ModuleManager/wiki/Module-Manager-Handbook
https://github.com/KSP-RO/ProceduralParts/wiki/Config-Parameter-Documentation

Resources
---------

* ResourceDistrib: natural distribution of resources
* : resource definition


ToDo
----

* verify dry mass and cost of all touched parts
* light fuel tanks for light resources only
* generic rcs tanks
* K&K radioactive tanks
* nertea's CryoTanks (they look good!)
* procedural parts for solid and nuclear
* support [SMURFF](https://forum.kerbalspaceprogram.com/index.php?/topic/117992-17-19-smurff-simple-module-adjustments-for-real-ish-fuel-mass-fractions-191-02019-nov-12/)
* wetworks - B9 does not support crew...

Flight switchable: CT, CC, wetworks, k&k, mk3

- all radioactive containers
- all cargo containers
- no dual tanks

Discounted: (tbd)

Our tanks left to configue:
  batteries;
  antimatter;
  RV Matter drop tank;
  crashpad: deleted, because kit does not use gases;
  RV Antimatter;
  ChargedParticleTrap: nice model, transforms;
  toroidal: two similar antimatter tanks and a disk;
  PositronTanks[12]: two sizes of tank;
  AntimatterTrap: tank for antimatter;
  AntimatterTanks[12]: two size of tank;
  tanks on converters;
  UniversalStorage: ??;
