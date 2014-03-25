// I dont take no real credit here, all i did is add the jeep and adminvehicles together and tweek it down to loading a model for basic usage

//    AdminVehicle     //
// for spawn purposes //
///////////////////////
function servercmdPlant(%client,%mName,%mScale)
{
	%raycast = containerRayCast(%client.player.getEyePoint(),vectorAdd(vectorScale(vectorNormalize(%client.player.getEyeVector()),10),%client.player.getEyePoint()),$TypeMasks::TerrainObjectType,%client.player);
	if(!isObject(firstWord(%raycast)))
		return;
	%pos = posFromRaycast(%raycast);
	if(%mScale $= "")
		%mScale = "0.1";
	%DataBlock = "VFM_" @ strlwr(%mName);
	
	%client.ModelPlacement[%client.ModelCount++] = new WheeledVehicle()
	{
		datablock = %DataBlock;
		isAdminVehicle = true;
        	creationTime = getSimTime();
	};
	%client.ModelPlacement[%client.ModelCount].setTransform(%pos);
	%client.ModelPlacement[%client.ModelCount].setScale(%mScale SPC %mScale SPC %mScale);
	%client.GrowCrops("1");
	messageClient(%client,'',"\c6Model Request \c3" @ %client.ModelCount @ "\c6, has been \c2created");
}

function servercmdplantsize(%client,%mNum, %mScale,%speed)
{
	%client.ModelPlacement[%mNum].setscale(%mScale SPC %mScale SPC %mScale);
	messageClient(%client,'',"\c3" @ %client.ModelPlacement[%mNum].getDatablock().UIName @ " \c6scale changed to \c3" @ %mScale);
}

function servercmdGrowSpeed(%client,%speed)
{
	%client.growCrops(%speed);
}
function GameConnection::growCrops(%this,%speed)
{
	if(%speed>10 || %speed $= "")
		%speed = "10";
	if(isObject(%this.stopWatch))
		cancel(%this.stopWatch);
	
	// grow!
	for(%pl=0;%pl<%this.ModelCount;%pl++)
	{
		%XYZ = %this.ModelPlacement[%pl].getDatablock().fullSize;
		if(%this.ModelPlacement[%pl].scale !$= %XYZ SPC %XYZ SPC %XYZ)
		{
			%scale = %this.ModelPlacement[%pl].scale;
			%this.ModelPlacement[%pl].setScale(getWord(%scale,0)+0.1 SPC getWord(%scale,1)+0.1 SPC getWord(%scale,2)+0.1);
		}
	}
	
	if(isObject(%this.ModelPlacement[1]))
		%this.stopWatch = %this.schedule(1000/%speed,"growCrops",%speed);
}

// Vehicle Datablocks //
///////////////////////
datablock WheeledVehicleData(VFM_Wheat)
{
	category = "Vehicles";
	displayName = " ";
	uiName = "Wheat";
	shapeFile = "./shapes/wheat.dts"; // Poof
	emap = true;
	minMountDist = 1;
	fullSize = 1.5;

	numMountPoints = 0;
	
	maxDamage = 30.00;
	destroyedLevel = 30.00;
	// speedDamageScale = 1.04;
	collDamageThresholdVel = 20.0;
	collDamageMultiplier   = 0.02;
	
	massCenter = "0 0 0";
	
	useEyePoint = false;

	numWheels = 0;
	
	// Rigid Body
	mass = 0;
	density = 1;
	drag = 3;
	bodyFriction = 0.6;
	bodyRestitution = 0.6;
	minImpactSpeed = 10;        // Impacts over this invoke the script callback
	softImpactSpeed = 10;       // Play SoftImpact Sound
	hardImpactSpeed = 15;      // Play HardImpact Sound
	groundImpactMinSpeed    = 10.0;

	// Engine
	engineTorque = 12000; //4000;       // Engine power
	engineBrake = 2000;         // Braking when throttle is 0
	brakeTorque = 50000;        // When brakes are applied
	maxWheelSpeed = 30;        // Engine scale by current speed / max speed

	rollForce	= 900;
	yawForce	= 600;
	pitchForce	= 1000;
	rotationalDrag	= 0.2;
	
	// Energy
	maxEnergy = 100;
	jetForce = 3000;
	minJetEnergy = 30;
	jetEnergyDrain = 2;
	
	// Sounds
	//   jetSound = ScoutThrustSound;
	//engineSound = idleSound;
	//squealSound = skidSound;
	softImpactSound = slowImpactSound;
	hardImpactSound = fastImpactSound;
	//wheelImpactSound = slowImpactSound;


	justcollided = 0;
	
	lookUpLimit = 0.65;
	lookDownLimit = 0;

	paintable = true;
};

datablock WheeledVehicleData(VFM_turnip)
{
	category = "Vehicles";
	displayName = " ";
	uiName = "Turnip";
	shapeFile = "./shapes/turnip.dts"; // Poof
	emap = true;
	minMountDist = 1;
	fullSize = 1.1;

	numMountPoints = 0;
	
	maxDamage = 30.00;
	destroyedLevel = 30.00;
	// speedDamageScale = 1.04;
	collDamageThresholdVel = 20.0;
	collDamageMultiplier   = 0.02;
	
	massCenter = "0 0 0";
	
	useEyePoint = false;

	numWheels = 0;
	
	// Rigid Body
	mass = 0;
	density = 1;
	drag = 3;
	bodyFriction = 0.6;
	bodyRestitution = 0.6;
	minImpactSpeed = 10;        // Impacts over this invoke the script callback
	softImpactSpeed = 10;       // Play SoftImpact Sound
	hardImpactSpeed = 15;      // Play HardImpact Sound
	groundImpactMinSpeed    = 10.0;

	// Engine
	engineTorque = 12000; //4000;       // Engine power
	engineBrake = 2000;         // Braking when throttle is 0
	brakeTorque = 50000;        // When brakes are applied
	maxWheelSpeed = 30;        // Engine scale by current speed / max speed

	rollForce	= 900;
	yawForce	= 600;
	pitchForce	= 1000;
	rotationalDrag	= 0.2;
	
	// Energy
	maxEnergy = 100;
	jetForce = 3000;
	minJetEnergy = 30;
	jetEnergyDrain = 2;
	
	// Sounds
	//   jetSound = ScoutThrustSound;
	//engineSound = idleSound;
	//squealSound = skidSound;
	softImpactSound = slowImpactSound;
	hardImpactSound = fastImpactSound;
	//wheelImpactSound = slowImpactSound;


	justcollided = 0;
	
	lookUpLimit = 0.65;
	lookDownLimit = 0;

	paintable = true;
};

datablock WheeledVehicleData(VFM_carrot)
{
	category = "Vehicles";
	displayName = " ";
	uiName = "Carrot";
	shapeFile = "./shapes/carrot.dts"; // Poof
	emap = true;
	minMountDist = 1;
	fullSize = 0.8;

	numMountPoints = 0;
	
	maxDamage = 30.00;
	destroyedLevel = 30.00;
	// speedDamageScale = 1.04;
	collDamageThresholdVel = 20.0;
	collDamageMultiplier   = 0.02;
	
	massCenter = "0 0 0";
	
	useEyePoint = false;

	numWheels = 0;
	
	// Rigid Body
	mass = 0;
	density = 1;
	drag = 3;
	bodyFriction = 0.6;
	bodyRestitution = 0.6;
	minImpactSpeed = 10;        // Impacts over this invoke the script callback
	softImpactSpeed = 10;       // Play SoftImpact Sound
	hardImpactSpeed = 15;      // Play HardImpact Sound
	groundImpactMinSpeed    = 10.0;

	// Engine
	engineTorque = 12000; //4000;       // Engine power
	engineBrake = 2000;         // Braking when throttle is 0
	brakeTorque = 50000;        // When brakes are applied
	maxWheelSpeed = 30;        // Engine scale by current speed / max speed

	rollForce	= 900;
	yawForce	= 600;
	pitchForce	= 1000;
	rotationalDrag	= 0.2;
	
	// Energy
	maxEnergy = 100;
	jetForce = 3000;
	minJetEnergy = 30;
	jetEnergyDrain = 2;
	
	// Sounds
	//   jetSound = ScoutThrustSound;
	//engineSound = idleSound;
	//squealSound = skidSound;
	softImpactSound = slowImpactSound;
	hardImpactSound = fastImpactSound;
	//wheelImpactSound = slowImpactSound;


	justcollided = 0;
	
	lookUpLimit = 0.65;
	lookDownLimit = 0;

	paintable = true;
};

datablock WheeledVehicleData(VFM_herb)
{
	category = "Vehicles";
	displayName = " ";
	uiName = "Herb";
	shapeFile = "./shapes/herb.dts"; // Poof
	emap = true;
	minMountDist = 1;
	fullSize = 1;

	numMountPoints = 0;
	
	maxDamage = 30.00;
	destroyedLevel = 30.00;
	// speedDamageScale = 1.04;
	collDamageThresholdVel = 20.0;
	collDamageMultiplier   = 0.02;
	
	massCenter = "0 0 0";
	
	useEyePoint = false;

	numWheels = 0;
	
	// Rigid Body
	mass = 0;
	density = 1;
	drag = 3;
	bodyFriction = 0.6;
	bodyRestitution = 0.6;
	minImpactSpeed = 10;        // Impacts over this invoke the script callback
	softImpactSpeed = 10;       // Play SoftImpact Sound
	hardImpactSpeed = 15;      // Play HardImpact Sound
	groundImpactMinSpeed    = 10.0;

	// Engine
	engineTorque = 12000; //4000;       // Engine power
	engineBrake = 2000;         // Braking when throttle is 0
	brakeTorque = 50000;        // When brakes are applied
	maxWheelSpeed = 30;        // Engine scale by current speed / max speed

	rollForce	= 900;
	yawForce	= 600;
	pitchForce	= 1000;
	rotationalDrag	= 0.2;
	
	// Energy
	maxEnergy = 100;
	jetForce = 3000;
	minJetEnergy = 30;
	jetEnergyDrain = 2;
	
	// Sounds
	//   jetSound = ScoutThrustSound;
	//engineSound = idleSound;
	//squealSound = skidSound;
	softImpactSound = slowImpactSound;
	hardImpactSound = fastImpactSound;
	//wheelImpactSound = slowImpactSound;


	justcollided = 0;
	
	lookUpLimit = 0.65;
	lookDownLimit = 0;

	paintable = true;
};

datablock WheeledVehicleData(VFM_tomato)
{
	category = "Vehicles";
	displayName = " ";
	uiName = "Tomato";
	shapeFile = "./shapes/tomato.dts"; // Poof
	emap = true;
	minMountDist = 1;
	fullSize = 2;

	numMountPoints = 0;
	
	maxDamage = 30.00;
	destroyedLevel = 30.00;
	// speedDamageScale = 1.04;
	collDamageThresholdVel = 20.0;
	collDamageMultiplier   = 0.02;
	
	massCenter = "0 0 0";
	
	useEyePoint = false;

	numWheels = 0;
	
	// Rigid Body
	mass = 0;
	density = 1;
	drag = 3;
	bodyFriction = 0.6;
	bodyRestitution = 0.6;
	minImpactSpeed = 10;        // Impacts over this invoke the script callback
	softImpactSpeed = 10;       // Play SoftImpact Sound
	hardImpactSpeed = 15;      // Play HardImpact Sound
	groundImpactMinSpeed    = 10.0;

	// Engine
	engineTorque = 12000; //4000;       // Engine power
	engineBrake = 2000;         // Braking when throttle is 0
	brakeTorque = 50000;        // When brakes are applied
	maxWheelSpeed = 30;        // Engine scale by current speed / max speed

	rollForce	= 900;
	yawForce	= 600;
	pitchForce	= 1000;
	rotationalDrag	= 0.2;
	
	// Energy
	maxEnergy = 100;
	jetForce = 3000;
	minJetEnergy = 30;
	jetEnergyDrain = 2;
	
	// Sounds
	//   jetSound = ScoutThrustSound;
	//engineSound = idleSound;
	//squealSound = skidSound;
	softImpactSound = slowImpactSound;
	hardImpactSound = fastImpactSound;
	//wheelImpactSound = slowImpactSound;


	justcollided = 0;
	
	lookUpLimit = 0.65;
	lookDownLimit = 0;

	paintable = true;
};
