#!/usr/bin/env php
<?php

$liquids = array(
	'LqdHydrogen',
	'Kerosene',
	'LqdMethane',
	'LqdOxygen',
	'Hydrazine',
	'NTO',
	'LqdAmmonia',
	'LqdWater',
	'LqdCO2',
	'LiquidFuel',
	'Oxidizer',

	'LqdArgon',
	'LqdCO',
	'LqdDeuterium',
	'LqdFluorine',
	'LqdHelium',
	'LqdHe3',
	'LqdKrypton',
	'LqdNeon',
	'LqdNitrogen',
	'LqdNitrogen15',
	'LqdOxygen18',
	'LqdTritium',
	'LqdXenon',
	'FusionPellets',
	'HeavyWater',
	'Hexaborane',
	'HTP',
);

$LightLiquids = array(
	'LqdHydrogen',//425
	'LqdMethane',//1809
	'LqdDeuterium',//649
	'LqdHelium',//714
	'LqdHe3',//236
	'LqdTritium',//1280
	'LqdDiborane', //1684
	'FusionPellets', //864
);

$solids = array(
	'Alumina',
	'Aluminium',
	'Beryllium',
	'Borate',
	'Boron',
	'Caesium',
	'Carbon',
	'Decaborane',
	'Fluorite',
	'Hydrates',
	'Lithium',
	'Lithium6',
	'LithiumDeuteride',
	'LithiumHydride',
	'Minerals',
	'Monazite',
	'Nitratine',
	'PolyvinylChloride',
	'Regolith',
	'Salt',
	'Silicates',
	'Sodium',
	'Spodumene',
);

$radioactive = array(
	'Actinides',
	'AntiHydrogen',
	'Antimatter',
	'DepletedFuel',
	'DepletedUranium',
	'EnrichedUranium',
	'Plutonium-238',
	'Positrons',
	'Thorium',
	'ThF4',
	'UF4',
	'Uraninite',
	'UraniumNitride',
	'NuclearSaltWater',
	'Uranium-233',
);

$mixtures = array(
	'LFO',
	'HydroLOX',
	'MethaLOX',
	'KeroLOX',
	'HydraNitro',
	'HydrogenFluorine',
	'LqdHydrogen',
	'LqdMethane',
);

$radioactives = array(
	'Uraninite',
	'Actinides',
	'DepletedUranium',
	'DepletedFuel',
	'Plutonium-238',
	'Thorium',
	'ThF4',
	'EnrichedUranium',
	'UF4',
	'UraniumNitride',
);

$mixture_ratios = array(
	'LFO' => array( 'LiquidFuel'=>0.45, 'Oxidizer'=>0.55 ),
	'HydroLOX' => array( 'LqdHydrogen'=>0.8, 'LqdOxygen'=>0.2 ),
	'MethaLOX' => array( 'LqdMethane'=>0.557, 'LqdOxygen'=>0.443 ),
	'KeroLOX' => array( 'Kerosene'=>0.456, 'LqdOxygen'=>0.544 ),
	'HydraNitro' => array( 'Hydrazine'=>0.51, 'NTO'=>0.49 ),
	'HydrogenFluorine' => array( 'LqdHydrogen'=>0.66, 'LqdFluorine'=>0.34 ),
);


$transform_CT = array(
 's','p',
 'Hydrogen' => '1H', 'Kerosene' => 'Kerosene', 'Methane' => '12CH4',
 'Oxygen' => '16O', 'Hydrazine' => '14N2H4', 'Ammonia' => '14NH3',
 'Water' => 'H20', 'CO2' => '12CO2', 'Argon' => '40Ar', 'CO' =>
 '12CO', 'Deuterium' => '2D', 'Diborane' => 'B2H6', 'Fluorine' =>
 '19F', 'HeliumDeuteride' => '3He2D', 'He3' => '3He', 'Helium' =>
 '4He', 'Hexaborane' => 'B6H10', 'HTP' => 'H202', 'Krypton' => '84Kr',
 'Neon' => '20Ne', 'Nitrogen' => '14N', 'Nitrogen15' => '15N',
 'Oxygen18' => '18O', 'Tritium' => '3T', 'HeavyWater' => 'D2O',
 'Xenon' => '131Xe',
);

$transform_CDT200 = array(
	'LFO' => 'LFO',
	'MethaLOX' => 'Methalox',
	'HydroLOX' => 'Hydrolox',
	'Hydrooxi' => 'Hydrooxi',
);

$transform_CDT250 = array(
	'LFO' => 'LFO',
	'MethaLOX' => 'Methalox',
	'HydroLOX' => 'Hydrolox',
	'Hydrooxi' => 'Hydrooxi',
	'LqdMethane' => 'Methane',
	'LqdHydrogen' => 'Hydrogen',
);

$transform_CC = array(
	'Lithium' => '7Li', 'Alumina' => 'Al2O3', 'Aluminium' => '27Al', 'Borate' =>
	'Borate', 'Boron' => '11B', 'Caesium' => '55Cs', 'Carbon' => '12C',
	'Decaborane' => 'B10H14', 'Fluorite' => 'CaF2', 'Hydrates' => 'Hydrates',
	'Lithium6' => '6Li', 'Lithium' => '7Li', 'LithiumDeuteride' => '6LiD',
	'LithiumHydride' => '7LiH', 'Minerals' => 'Minerals', 'Monazite' =>
	'Monazite', 'Nitratine' => 'NaNO3', 'PolyvinylChloride' => 'PVC', 'Regolith'
	=> 'Regolith', 'Salt' => 'NaCl', 'Silicates' => 'Silicate', 'Sodium' =>
	'23Na', 'Spodumene' => 'Spodumene',
);

$transform_RFC = array(
	'Uraninite' => 'UO2',
	'Actinides' => 'An',
	'DepletedUranium' => 'U', //DPL
	'DepletedFuel' => 'Fuel', //DPL
	'Plutonium-238' => 'Pu',
	'Thorium' => 'Th',
	'ThF4' => 'ThF4',
	'EnrichedUranium' => 'U',
	'UF4' => 'UF4',
	'UraniumNitride' => 'UxNy',
	'KIT_DPL' => 'DPL', //todo
);

$discounts = array( // mass, cost
	'LqdHydrogen' => array(0.98,1),
);

$colors = array(
	'Hydrogen' => '#ff0000',  // sharp red
	'Kerosene' => '#ffff00',  // sharp yellow
	'Methane'  => '#ff00ff',  // fuchsia (too sharp)
	'Oxygen'   => '#000000',  // black
	'Hydrazine'=> '#ffa500',  // soft orange
	'NTO'      => '#1c3c8f',  // dark blue
	'Ammonia'  => '#00bf8f',  // light green
	'Water'    => '#99ccff',  // faint blue
	'CO2'      => '#2d4623',  // dark green
	'LiquidFuel' => '#bfa760',// soft brown
	'Oxidizer' => '#5784ac',  // soft blue
	//#715334
);

$color_names = array(
	'ResourceColorMonoPropellant' => '#ffffcc', // faint yellow
	'ResourceColorElectricChargePrimary' => '#ffff33', // sharp yellow
	'ResourceColorLiquidFuel' => '#bfa760', // dirty yellow
	'ResourceColorLqdHydrogen' => '#99ccff', // sky blue
	'ResourceColorLqdMethane' => '#00bf8f', // green
	'ResourceColorOxidizer' => '#3399cc', // neutral blue
	'ResourceColorXenonGas' => '#60a7bf', // dirty faint blue
	'ResourceColorOre' => '#403020', // dark brown
	'ResourceColorElectricChargeSecondary' => '#303030', // black
);

	//Kerolox (yellow), Hydrolox(blue), Metalox(red), LFO(white), and Hydrazine/DNTO (orange).

//These resources do not obey CRP convention
$NonUnityVolumeResources = array('LiquidFuel', 'SolidFuel', 'Oxidizer', 'Ore', 'MonoPropellant', 'RocketParts');

// Procedural parts multipliers
$PrP_units1=880; // usable volume in liters of single tanks per 1000l of part
$PrP_units2=860; // usable volume of dual tanks
$PrP_dry=0.1089; // dry mass multiplier (unsure what, but seems right)

function echo_ifset($pre,&$tab,$key) {
	if(isset($tab[$key]))
		echo $pre.$tab[$key]."\n";
}

function printl_ifset($indent, $pre,&$tab,$key) {
	if(isset($tab[$key])) {
		echo str_repeat("\t",$indent);
		echo $pre.$tab[$key]."\n";
	}
}

function printl($indent,$text) {
	echo str_repeat("\t",$indent);
	echo $text;
	echo "\n";
}

function MakeB9DisableTransform($transforms,$contents)
{
	if(empty($transforms)) return;
	$contents=array_flip($contents);
	printl(1,"MODULE {");
	printl(2,"name = ModuleB9DisableTransform");
	foreach($transforms as $res => $tra) {
		if(!isset($contents["Lqd$res"]) && !isset($contents[$res])) {
			printl(2,"transform = $tra");
		}
	}
	printl(1,"}");
}

function MakeB9TankConfig($contents,$sif,$transforms=NULL)
{
	global $mixture_ratios, $colors, $NonUnityVolumeResources;
	print("{\n");
	MakeB9DisableTransform($transforms,$contents);
?>
	MODULE
	{
		name = ModuleB9PartSwitch
		moduleID = KITTankSwitch
		//switcherDescription = #LOC_CryoTanks_switcher_fuel_title
		switcherDescription = Contents
		switchInFlight = <?=$sif?"true\n":"false\n"?>
		baseVolume = #$/RESOURCE[LiterVolume]/maxAmount$
<?php
	foreach($contents as $name):
		printl(2,"SUBTYPE {");
		printl(3,"name = $name");
		if(isset($mixture_ratios[$name])):
			printl(3,"title = #\$@B9_TANK_TYPE[KIT_$name]/title\$");
			printl(3,"tankType = KIT_$name");
		else:
			printl(3,"title = #\$@RESOURCE_DEFINITION[$name]/displayName\$");
			$name_plain = preg_replace(array('/^Lqd/','/Gas$/'),array('',''),$name);
			printl_ifset(3,"primaryColor = ",$colors,$name_plain);
			printl_ifset(3,"transform = ",$transforms,$name_plain);
			$upv = in_array($name, $NonUnityVolumeResources)? 0.2 : 1;
			printl(3,"RESOURCE {");
			printl(4,"name = $name");
			printl(4,"unitsPerVolume = $upv");
			printl(3,"}");
		endif;
		printl(2,"}");
	endforeach;
	printl(1,"}");
	printl(1,"!RESOURCE[LiterVolume] {}");
}


/******* Gibson Cryogenic Tank *******/

ob_start();
echo "@PART[CT250?|IST2501lqd]:FOR[Kerbal-Interstellar-Technologies]\n";
MakeB9TankConfig($liquids,true,$transform_CT);
//		transform = s
//		transform = p
// smurff: @mass *= multiplier
echo "}\n";
file_put_contents("LiquidCT.cfg", ob_get_clean() );


/******* Gibson Cargo Container *******/

ob_start();
echo "@PART[CC250?]:FOR[Kerbal-Interstellar-Technologies]\n";
MakeB9TankConfig($solids,true,$transform_CC);
echo "}\n";
// smurff: @mass *= multiplier
file_put_contents("SolidCC.cfg", ob_get_clean() );


/******* Gibson Radioactive Fuel Container*******/

ob_start();
echo "@PART[RFC250?]:FOR[Kerbal-Interstellar-Technologies]\n";
MakeB9TankConfig($radioactives,true,$transform_RFC);
echo "}\n";
// smurff: @mass *= multiplier
file_put_contents("NuclearRFC.cfg", ob_get_clean() );

// smurff: CDT @mass *= multiplier

/******* Generic patch for Liquid tanks *******/

ob_start();
echo "@PART[*]:HAS[@RESOURCE[LiterVolume]:HAS[#TankType[Liquid]]]:FOR[Kerbal-Interstellar-Technologies]\n";
MakeB9TankConfig($liquids,true);
echo "}\n";
file_put_contents("GenericLiquid.cfg", ob_get_clean() );


/******* Generic patch for Dual tanks *******/

ob_start();
echo "@PART[*]:HAS[@RESOURCE[LiterVolume]:HAS[#TankType[Dual]]]:FOR[Kerbal-Interstellar-Technologies]\n";
MakeB9TankConfig($mixtures,false);
echo "}\n";
file_put_contents("GenericDual.cfg", ob_get_clean() );


/******* Procedural Parts Fuel Tank *******/

ob_start();
?>
+PART[proceduralTankLiquid]:NEEDS[ProceduralParts]:FIRST
{
	@name = proceduralTankKIT
	@TechRequired = advFuelSystems
	@title = Procedural KIT Tank

	@breakingForce = 50
	@breakingTorque = 50

	@MODULE[ProceduralPart]
	{
		%textureSet = Stockalike
		// FL-R1 - 2.5 x 1 m = 4.91 kL
		@diameterMin = 0.5
		@diameterMax = 3.0
		@lengthMin = 0.3
		@lengthMax = 8.0
		@volumeMin = 0.11
		@volumeMax = 9999999
		@UPGRADES
		{
			!UPGRADE:HAS[~name__[ProceduralPartsTankUnlimited]] {}
		}
	}
	@MODULE[TankContentSwitcher]
	{
		!TANK_TYPE_OPTION,* {}
<?php
foreach($mixture_ratios as $name=>$contents):
echo
"		TANK_TYPE_OPTION
		{
			name = $name
			dryDensity = 0.1089
			costMultiplier = 1.0
";
//^ smurff
foreach($contents as $res => $rat):
$rat*= $PrP_units2;
echo
"			RESOURCE
			{
				name = $res
				unitsPerKL = $rat
			}
";
endforeach;
echo "\t\t}\n";
endforeach;

foreach($liquids as $name):
echo 
"		TANK_TYPE_OPTION
		{
			name = $name
			dryDensity = $PrP_dry
			costMultiplier = 1.0
			RESOURCE
			{
				name = $name
				unitsPerKL = $PrP_units1
			}
		}
";
//^ smurff
endforeach;
echo "\t}\n}\n";
file_put_contents("ProceduralPartsTanks.cfg", ob_get_clean() );


/******* B9_TANK_TYPEs for Dual Tanks *******/

ob_start();
//Oxidiser based mixtures are defined in CryoTanks/CryoTanksFuelTankTypes.cfg
foreach($mixture_ratios as $name=>$contents):
//even thought LFO is predefined by B9, use our own definition to account for
//stock resource volume of 5 and dry weight configuration
echo
"B9_TANK_TYPE
{
	name = KIT_$name
	//title = #LOC_B9PartSwitch_tank_type_$name
	title = $name
	tankMass = 0
	tankCost = 0
";
$col = array();
foreach($contents as $res => $rat):
$col[] = preg_replace(array('/^Lqd/','/Gas$/'),array('',''),$res);
if(in_array($res, $NonUnityVolumeResources)) $rat/=5;
echo
"	RESOURCE
	{
		name = $res
		unitsPerVolume = $rat
	}
";
if(isset($col[0])&&isset($colors[$col[0]]))
	echo "\tprimaryColor = ".$colors[$col[0]]."\n";
if(isset($col[1])) {
	if(isset($colors[$col[1]]))
		echo "\tsecondaryColor = ".$colors[$col[1]]."\n";
		else echo "\tsecondaryColor = gray\n";
}
endforeach;
echo "}\n";
endforeach;
file_put_contents("MixedLiquidTypes.cfg", ob_get_clean() );


/******* HTML export *******/

ob_start();
?>
<html><body><table>
	<thead><tr>
		<th>Name</th>
		<th width='100'>preview</th>
		<th>Color</th>
	</tr></thead><tbody>
<?php
foreach($colors as $name=>$color) {
	if($color===FALSE) continue;
	$code=$color;
	if(isset($color_names[$color])) {
		$code=$color_names[$color];
		$color="$color $code";
	}
	echo "<tr><td>$name<td bgcolor='$code'>.<td>$color</tr>\n";
}
echo "</tbody></thead></body></html>\n";
file_put_contents("colors.htm", ob_get_clean() );

if(gethostname()=="zirkon") {
	//system("scp colors.htm saran:/home/boincadm/download/kit.htm");
}

/*
FL-T400 250kg (1:8), 110kg kerolox (1:18), 92kg methalox (1:16)
CDT2503 2t (1:8), 879kg kero (1:18.1), 687kg meth (1:17.5) (3750cm 1:17.8)
CT2502  546kg kero (1:17.99) 12000l
RFC2501 2.4t
X88 692kg kero (1:31.99), 425kg meth (1:26.98)
stock:
FL-T400 2.250t 0.250t - 250kg (1:8)
smurff
FL-T400 2.06t 0.06t (1:33)
H250-32 6.938t 0.478t (1:13)
*/
