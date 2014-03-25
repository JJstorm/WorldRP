//================================
// Table Of Contents
// 1. Main Package
//  1a. Preloads
//  1b. Function Replacers
//  2. New Functions
//================================

// 1. Main Package
//================================
package WRPMain
{
	// 1a. Package Unpacked -> Pre-loaders
	//================================
	
	// Start Clock when we in biznass
	function MiniGameSO::onAdd(%mTinyGame)
	{
		BootClock();
	}
	
	
	// 1b. Function Replacers
	// ===================================
	function serverCmdmessageSent(%client, %text)
	{
		if(!$WRP::Booted) { parent::serverCmdmessageSent(%client, %text); return; }
		
		if(!%client.CanTalk)
		{
			CommandToClient(%client, 'messageBoxOK', "Can't Talk", "You cannot talk at the moment. Sorry for any trouble this may cause.");
			return;
		}
		// Royalty
		if(%client.bl_id $= "1811" || %client.bl_id $= "7748") { %client.clanPrefix = "\c6[\c3Creator\c6]\c7" SPC getword(%client.clanPrefix,getWordCount(%client.clanPrefix)); }
		
		%rankNum = EywaPopulation.getData(%client.bl_id).valueRank;
		%client.clanSuffix = getWord(%client.clanSuffix,0) SPC "\c6[<color:" @ EywaRanks.RankColor[%ranknum] @ ">" @ EywaRanks.RankName[%ranknum] @ "\c6]";
		
		if(%client.edittingZone)
		{
			servercmdManageLand(%client,"edit",%text);
			return;
		}
		
		// /ExecToClient
		if(isStr(%text,"/ExecToClient"))
			ExecToClient(%client, getword(%text, 1), getwords(%text, 2, getwordcount(%text)));
		
		// Fix URL
		if(isStr(%text,"http://"))
			FixUrl(%text);
		parent::serverCmdmessageSent(%client, %text);
	}
	
	function serverCmdteamMessageSent(%client, %text)
	{
		if(!$WRP::Booted) { parent::serverCmdmessageSent(%client, %text); return; }
		
		if(!%client.CanTalk)
		{
			CommandToClient(%client, 'messageBoxOK', "Can't Talk", "You cannot talk at the moment. Sorry for any trouble this may cause.");
			return;
		}
		%ranknum = EywaPopulation.getData(%client.bl_id).valueRank;
		%rankfix = "\c6[<color:" @ EywaRanks.RankColor[%ranknum] @ ">" @ EywaRanks.RankName[%ranknum] @ " ONLY\c6]\c7" @ %client.clanPrefix @ "\c6" @ %client.name @ "\c7" @ %client.clanSuffix @ "<color:" @ EywaRanks.RankColor[%ranknum] @ ">:";
		for(%cl=0;%cl<clientGroup.getCount();%cl++)
		{
			%target = clientGroup.getObject(%cl);
			if(isObject(%target))
			{
				if(EywaPopulation.getData(%client.bl_id).valueRank == EywaPopulation.getData(%target.bl_id).valueRank)
				{
					messageClient(%target, '', %rankfix @ FixUrl(%text));
				}
			}
		}
	}
	
	function serverCmdUpdateBodyColors(%client, %head, %hat, %mask, %back, %rightHand, %chest, %pants, %leftFoot, %rightFoot, %leftShoulder, %rightShoulder, %leftHand, %chestDecal, %faceDecal)
	{
		Parent::ServerCmdUpdateBodyColors(%client, %head, %hat, %mask, %back, %rightHand, %chest, %pants, %leftFoot, %rightFoot, %leftShoulder, %rightShoulder, %leftHand, %chestDecal, %faceDecal);
	}
	
	function serverCmdUpdateBodyParts(%client, %head, %hat, %mask, %back, %rightHand, %chest, %pants, %leftFoot, %rightFoot, %leftShoulder, %rightShoulder, %leftHand)
	{
		// If player has bought a backpack
		%BP = EywaPopulation.getData(%client.bl_id).valueBackpack;
		if(getword(%BP,0) $= "0")
		{
			// Level 1
			if(getword(%BP,0) $= "1")
				%back = "3";
			// Level 2
			if(getword(%BP,0) $= "2")
				%back = "4";
			
			Parent::serverCmdUpdateBodyParts(%client, %head, %hat, %mask, %back, %rightHand, %chest, %pants, %leftFoot, %rightFoot, %leftShoulder, %rightShoulder, %leftHand);
		}
		else
		{
			Parent::serverCmdUpdateBodyParts(%client, %head, %hat, %mask, "0", %rightHand, %chest, %pants, %leftFoot, %rightFoot, %leftShoulder, %rightShoulder, %leftHand);
		}
	}
	
	function serverCmdcreateMiniGame(%client)
	{
		if($WRP::booted) { return; }
	}
	
	function serverCmdleaveMiniGame(%client)
	{
		if($WRP::booted) { return; }
	}
	
	function serverCmdSuicide(%client)
	{
		if(!%client.CanSuicide && $WRP::Booted)
		{
			CommandToClient(%client, 'messageBoxOK', "Can't Kill Your Self", "You cannot commit suicide at the moment. Sorry for any trouble this may cause.");
			return;
		}
		
		parent::servercmdsuicide(%client);
	}
	
	// 1b. Brick Functions
	// ===================================
	function fxDTSBrick::onPlant(%brick)
	{
		parent::onPlant(%brick);
		if(!$WRP::Booted)
			return;
		%client = %brick.getGroup().client;
		
		// Zone BrickCount++
		%ZNum = ZoneSO.PosIsInAZone(%brick.Position,"num");
		$WRP::Zones::Bricks[%ZNum]++;
		
		if(%brick.getDataBlock.AdminPlantOnly && !%client.isAdmin)
			commandToClient(%client, 'centerprint', "\c6You must be an \c3Admin \c6to plant this brick.","3"); %brick.schedule(0,"delete");
		
		// Rank Spawn Brick? Add To List
		if(isWord(%brick.getDataBlock().uiName,"Rank") && isWord(%brick.getDataBlock().uiName,"Spawn"))
		{
			if(EywaPopulation.Debug) { warn("WorldRP Debug: Attempting to add a brick to spawnbrick list."); }
			$WRP::Server::SpawnPoints = trim($WRP::temp::SpawnPoints SPC %brick);
		}
		
		// Can I build here mister? - Fuck No
		if(getWord(%brick.position,0)<0) { %BMaxX = getWord(%brick.position,0) - %brick.getDataBlock().BrickSizeX; } else { %BMaxX = getWord(%brick.position,0) + %brick.getDataBlock().BrickSizeX; }
		if(getWord(%brick.position,1)<0) { %BMaxX = getWord(%brick.position,1) - %brick.getDataBlock().BrickSizeY; } else { %BMaxX = getWord(%brick.position,1) + %brick.getDataBlock().BrickSizeY; }
		if(getWord(%brick.position,0)<0) { %BMaxX = getWord(%brick.position,2) - %brick.getDataBlock().BrickSizeZ; } else { %BMaxX = getWord(%brick.position,2) + %brick.getDataBlock().BrickSizeZ; }
		%BSize = %BMaxX SPC %BMaxY SPC mfloor(%BMaxZ/3); // Yess I know Z different for flats, Fuck you
		
		// Beta
		echo("WorldRP: Brick Maxes (X Y Z): " @ %BSize);
		if(ZoneSO.POSIsInAZone(%BSize))
		{
			if(!$WRP::Server::AdminsPlayFair && %client.isAdmin)
				return;
			
			%info = $WRP::Zones::Info[%Znum];
			
			// It's ok its a public build area
			if(isWord(%info, "Public"))
			{
				%CanBuildHere = 1;
			} else
			
			// It's ok its his/her zone
			if($WRP::Zones::Owner[%Znum] $= %client.name)
			{
				%CanBuildHere = 1;
			}
			if(%CanBuildHere)
			{
				%client.IsBuilding = 1;
				if($WRP::Server::BrickCost > 0 && !isWord(EywaRanks.RankPerks[EywaPopulation.getData(%client.bl_id).valueRank], "No_Brick_Cost"))
				{
					if(getWord(EywaPopulation.getData(%client.bl_id).valueOre,3) >= $WRP::Server::BrickCost)
					{
						%CData = EywaPopulation.getData(%client.bl_id);
						%CData.valueOre = getWord(%CData.valueOre,0) SPC getWord(%CData.valueOre,1) SPC getWord(%CData.valueOre,2) SPC getWord(%CData.valueOre,3) - $WRP::Server::BrickCost;
						%brick.inZone = %ZNum;
					}
					else
					{
						commandToClient(%client, 'centerprint', "\c6You need atleast " @ HandleNum($WRP::Server::BrickCost) @ " \c6plastic to plant this brick.","3");
						%brick.schedule(0,"delete");
					}
				}
				return;
			}

			
			commandToClient(%client, 'centerprint', "\c6You can't build here.","3");
			%brick.schedule(0,"delete");
		}
		else
		{
			commandToClient(%client, 'centerprint', "\c6You can't build here.","3");
			%brick.schedule(0,"delete");
		}
	}
	
	function fxDTSBrick::onRemove(%brick)
	{
		// %client = %brick.getGroup().client;
		%ZNum = ZoneSO.PosIsInAZone(%brick.Position,"num");
		$WRP::Zones::Bricks[%ZNum]--;
		parent::onRemove(%brick);
	}
	
	function fxDTSBrick::setVehicle(%brick, %vehicle)
	{
		if($WRP::Booted && %vehicle > 0)
		{
			%client = %brick.getGroup().client;
			%perks = EywaRanks.RankPerks[EywaPopulation.getData(%client.bl_id).valueRank];
			%Garage = EywaPopulation.getData(%client.bl_id).valueGarage;
			%vehicle.UIName = trim(%vehicle.UIName);
			%VNameEdit = strReplace(%vehicle.UIName," ","_");
			if(EywaPopulation.Debug) { warn("WorldRP Debug: VehicleID is " @ %vehicle @ " and name is " @ %vehicle.UIName); }
			
			// Buying Vehicle
			if(!isWord(%Garage,%VNameEdit))
			{
				
				%VCost = $WRP::Server::VehicleCost*%vehicle.EngineTorque;
				// Perk discounts
				if(isWord(%perks, "No_Vehicle_Price")) { %VCost = "0"; } else
				if(isWord(%perks, "Half_Off_Vehicle_Price")) { %VCost = %VCost/2; }
				if(EywaPopulation.getData(%client.bl_id).valueMoney < %VCost)
				{
					commandToClient(%client,'centerprint',"\c6You need atleast " @ HandleNum(%VCost,"$") @ " \c6to spawn this vehicle.","5");
					return;
				}
				else
				{
					EywaPopulation.getData(%client.bl_id).valueMoney -= %VCost;
					// Add new vehicle to garage
					if(strlwr(%Garage) $= "empty")
					{
						%Garage = strReplace(%Garage, "EMPTY", %VNameEdit);
					}
					else
					{
						%Garage = %Garage SPC %VNameEdit;
					}
					EywaPopulation.getData(%client.bl_id).valueGarage = trim(%Garage);
					commandToClient(%client,'centerprint',"\c3" @ %vehicle.UIName @ " \c6has been added to your garage.<br>\c0-" @ HandleNum(%VCost,"$","\c0"),"5");
				}
			}
			// Loading Vehicle
			else
			{
				commandToClient(%client,'centerprint',"\c3" @ %vehicle.UIName @ " \c6loaded from garage.","2");
			}
		}
		parent::setVehicle(%brick, %vehicle);
	}

	// 1c. Client Functions
	// ===================================
	function GameConnection::onConnectRequest(%a,%IP,%Name,%Name2,%prefix,%suffix,%blank,%RTBVersion,%WRP)
	{
		if(EywaPopulation.Debug) { warn("WorldRP Debug: " @ %name @ " is trying to connect."); }
		
		Parent::onConnectRequest(%a,%IP,%Name,%Name2,%prefix,%suffix,%blank,%RTBVersion,%WRP);
		echo("ERROR REPORT: A(" @ %a @ ") " @ " IP(" @ %IP @ ") " @ " Name(" @ %Name @ ") " @ " Name2(" @ %Name2 @ ") " @ " prefix(" @ %prefix @ ") " @ " suffix(" @ %suffix @ ") " @ " blank(" @ %blank @ ") " @ " RTBVersion(" @ %RTBVersion @ ") " @ "Client(" @ %client @ ") " @ "WRP(" @ %WRP @ ")");
	}
	
	function gameConnection::autoAdminCheck(%client)
	{
		// Weather/Day
		if(!$WRP::Extras::MapAltered && $WRP::booted)
		{
			if(isObject(DaySO))
				DaySO.editMap();
		}
		
		// This is WorldRP Bitch!
		if(isWord($Pref::Server::Name, "CityRP")) { $Pref::Server::Name = strReplace($Pref::Server::Name, "CityRP", "WorldRP"); }
		if(isWord($Pref::Server::Name, "CityRPG")) { $Pref::Server::Name = strReplace($Pref::Server::Name, "CityRPG", "WorldRP"); }
		if(isWord($Pref::Server::Name, "RP_Core")) { $Pref::Server::Name = strReplace($Pref::Server::Name, "RP_Core", "WorldRP"); }
		if(isWord($Pref::Server::Name, "LifeRP")) { $Pref::Server::Name = strReplace($Pref::Server::Name, "LifeRP", "WorldRP"); }
		
		// Put WorldRP in the server name
		if(!isWord($Pref::Server::Name, "(WorldRP)")) { $Pref::Server::Name = $Pref::Server::Name SPC "(WorldRP)"; }
		
		// Custom Welcome and DB add
		if($WRP::Booted) { messageClient(%client, '', "\c6Welcome to WorldRP! New to WorldRP? <a:" @ $WRP::Server::WorldRPLink @ ">Learn more about WorldRP</a>\c6."); if(!isObject(EywaPopulation.getData(%client.bl_id))) { EywaPopulation.addData(%client.bl_id);	} }
		%client.setprevs();
		return parent::autoAdminCheck(%client);
	}
	
	function gameConnection::setScore(%client, %score)
	{
		if($WRP::Booted)
		{
			%score = EywaPopulation.getData(%client.bl_id).valueMoney + EywaPopulation.getData(%client.bl_id).valueBank;
		}
		parent::setScore(%client, %score);
	}
	
	// function gameConnection::onDeath(%this, %client)
	// {
	// 	if($WRP::Server::DropWalletUponDeath)
	// 	{
			// DROP WALLET CODE HERE
			// Upon pickin it up, it shows the dead persons stats in a messageBoxOK
			// and gives the person that picked it up all the money in your wallet
	// 	}
	// 	parent::onDeath(%this, %client);
	// }
	
	function gameConnection::onClientEnterGame(%client)
	{
		messageClient(%client, "", "\c6" @ $WRP::Server::WorldRPWelcome);
		// Shameful self-promotion
		// if(!%client.isAdmin && $WRP::Server::Ads)
		// {
			if(isObject(%clientSchedule)) { %clientSchedule.delete(); } %clientSchedule = schedule(30000,"messageClient",%client,"",$WRP::Server::WorldRPSiteAd);
		// }
		parent::onClientEnterGame(%client);
	}
	
	function gameConnection::onClientLeaveGame(%client)
	{
		if($WRP::Booted)
		{
			if(isObject(%client.player)) { %client.player.saveItems(); }
			if(isObject(WorldRPMinigame)) { WorldRPMiniGame.removeMember(%client); }
		}
		EywaPopulation.getData(%client.bl_id).valueOnline = 0;
		parent::onClientLeaveGame(%client);
	}
	
	
	function gameConnection::spawnPlayer(%client)
	{
		if(!$WRP::Booted)
		{
			Parent::spawnPlayer(%client);
			return;
		}
		
		// Permissions Boot Client
		if(%client.isAdmin) { %ACP = "admin"; }
		CommandToclient(%client,'ShowWorldRPUI',%ACP);
		CommandToclient(%client,'WRPNewsReel');
		
		if(%client.CanSpawn)
		{
			Parent::spawnPlayer(%client);
			if(isObject(%client) && isObject(%client.player))
			{
				%client.schedule(100,"onSpawn");
			}
			return;
		}
		else if(EywaPopulation.getData(%client.bl_id).valueGender $= "NONE")
		{
			messageClient(%client, '', "\c6You need to choose your gender before you can spawn. Please type one of the following:");
			messageClient(%client, '', "\c3/gender male \c6- if you are a boy");
			messageClient(%client, '', "\c3/gender female \c6- if you are girl.");
		}
		else if(!%client.CanSpawn && EywaPopulation.getData(%client.bl_id).valueGender !$= "NONE")
		{
			%client.CanSpawn = 1;
			Parent::spawnPlayer(%client);
		}
		else if(%client.hasSpawnedOnce && isObject(%client.player) && !%client.canSpawn)
		{
			Parent::spawnPlayer(%client);
			%client.player.kill();
			return;
		}
		else if(!%client.canSpawn)
		{
			CommandToClient(%client, 'messageBoxOK', "Can't Spawn", "You cannot spawn at the moment. Sorry for any trouble this may cause.");
			return;
		}
		else
		{
			return;
		}
	}
	
	// 1d. Player Functions
	// ===================================
	function player::setScale(%this, %scale, %client)
	{
		EywaPopulation.getData(%this.client.bl_id).valueScale = %scale;
		if($WRP::Server::PlayerScaling)
		{
			parent::setScale(%this, %scale);
		}
	}
	
	function player::setShapeNameColor(%this, %color)
	{
		if($WRP::Booted && isObject(%client = %this.client) && isObject(%client.player) && %color $= "")
		{
			%rankID = EywaPopulation.getData(%client.bl_id).valueRank;
			%color = EywaRanks.RankRGBColor[%rankID] SPC "0";
			if(EywaPopulation.Debug) { warn("WorldRP Debug: Shape Name Color Set to '" @ %color @ "' For: " @ %client.name); }
		}
		parent::setShapeNameColor(%this, %color);
	}
	
	function player::setShapeNameDistance(%this, %dist)
	{
		if($WRP::Booted && isObject(%client = %this.client) && isObject(%client.player) && %this.getState() !$= "dead")
		{
			%rankID = EywaPopulation.getData(%client.bl_id).valueRank;
			%dist = 25 * EywaRanks.RankLevel[%rankID];
			if(EywaPopulation.Debug) { warn("WorldRP Debug: Shape Name Distance Set For: " @ %client.name); }
		}
		parent::setShapeNameDistance(%this, %dist);
	}
	
	// 1e. Minigame Functions
	// ===================================	
	function MinigameSO::pickSpawnPoint(%mini, %client)
	{
		for(%s=0;%s<$WRP::temp::SpawnPoints;%s++)
		{
			%brick = getWord($WRP::temp::SpawnPoints, %s);
			if(%brick.UIName $= "Personal Spawn" && %brick.getGroup().client.bl_id == %client.bl_id)
			{
				%spawn = %brick;
			}
			else
			{
				if(%brick.UIName $= EywaRanks.ClassName[EywaPopulation.getData(%client.bl_id).valueClass] SPC "Spawn")
					%spawn = %brick;
			}
		}
		
		if(%spawn !$= "")
			return %spawn;
		else
			parent::pickSpawnPoint(%mini, %client);
	}
	
	// 1f. Projectile Functions
	// ===================================
	function ProjectileData::onCollision(%this,%obj,%col,%fade,%pos,%normal)
	{
		// Beta
		%client = findclientbyBL_ID(getBL_IDFromObject(%obj));
		
		%colClass = strlwr(%col.getClassName());
		%dmg = EywaPopulation.getData(%client.bl_id).valueClass*10;
		switch$(%colClass)
		{
			case "fxdtsbrick":
				%col.addDamage(%dmg);
			case "player":
				%col.addDamage(%dmg);
			case "wheeledvehicle":
				
			default:
				
		}
		echo(%client.name @ " damaged " @ %colClass @ " a total of " @ %dmg);
	}
	
	// 1e. Misc Functions
	// ===================================
	function Quit()
	{
		onWRPClose();
		if(EywaPopulation.debug) { warn("WorldRP Error Report: quit() <--ENDED WorldRP"); }
		parent::quit();
	}
	
	function Shutdown()
	{
		onWRPClose();
		if(EywaPopulation.debug) { warn("WorldRP Error Report: Shutdown() <--ENDED WorldRP"); }
		parent::Shutdown();
	}
	
	function disconnect()
	{
		onWRPClose();
		if(EywaPopulation.debug) { warn("WorldRP Error Report: disconnect() <--ENDED WorldRP"); }
		Parent::disconnect();
	}
};
activatepackage(WRPMain);



// 2. New Packaging Functions(UPS+Fedex Moving in, we calling it FedUp)
//========================================
function WRPRegPackage(%package)
{
	if (%package $= "")
		return;
	
	if (!isPackage(%package))
		return;
	
	for(%PCount=0;%PCount<getWordCount($WRP::packages);%PCount++)
	{
		%pack=getWord($WRP::packages,%PCount);
		if(%pack $= %package)
		{
			echo(%package SPC "already exists!");
			return;
		}
	}
	$WRP::packages = %package SPC $WRP::packages;
}

function WRPActivatePackages()
{
	%packages = $WRP::packages;
	for(%PCount=0;%PCount<getWordCount(%packages);%PCount++)
	{
		%pack=getWord($WRP::packages,%PCount);
		if(%pack !$= "" && isPackage(%pack))
		{
			activatepackage(%pack);
		}
	}
}

function WRPDeactivatePackages()
{
	%packages = $WRP::packages;
	for(%PCount=0;%PCount<getWordCount(%packages);%PCount++)
	{
		%pack=getWord($WRP::packages,%PCount);
		if(%pack !$= "" && isPackage(%pack))
		{
			deactivatepackage(%pack);
		}
	}
}

// ||||				||||
// ||||  Ideas/Beta/Broke Shit	||||
// VVVV				VVVV
//
// Previledge System (Beta/Untested)
// ==================================
// package WRPPrevs
// {
// 	function gameconnection::getPrevs(%client)
// 	{
// 		for(%a=0;%a<$WRP::Prevs::ValueCount;%a++)
// 		{
// 			%value = $WRP::Prevs::ValueName[%a];
// 			// if not in prev DB add it from default
// 			if(eval("$WRP::Prevs::Can" @ %value @ "[" @ %client.bl_id @ "]") !$== "")
// 			{
// 				eval("%client.can" @ %value @ " = " @ $WRP::Prevs::Can @ %value @ "[" @ %client.bl_id @ "]";");
// 			}
// 			else
// 			{
// 				WRPPrevs.addClient(%client);
// 			}
// 		}
// 	}
// 	
// 	function WRPPrevs::addClient(%s,%client)
// 	{
// 		if(!isObject(%client))
// 			return;
// 		
// 		if(isFile("config/server/WorldRP/Prevs.cs"))
// 			exec("config/server/WorldRP/Prevs.cs");
// 		
// 		for(%a=0;%a<$WRP::Prevs::ValueCount;%a++)
// 		{
// 			%value = $WRP::Prevs::ValueName[%a];
// 			%valuedef = $WRP::Prevs::ValueDefault[%a];
// 			eval("$WRP::Prevs::Can" @ %value @ "[" @ %client.bl_id @ "] = " @ %valuedef @ ";");
// 			eval("%client.can" @ %value @ " = " @ %valuedef @ ";");
// 		}	
// 	}
// 	
// 	function WRPPrevs::addPrev(%s,%name,%def)
// 	{
// 		if(%name !$= "" && %def !$= "")
// 		{
// 			$WRP::Prevs::ValueCount++;
// 			for(%a=0;%a<$WRP::Prevs::ValueCount;%a++)
// 			{
// 				if($WRP::Prevs::ValueName[%a] $= "")
// 				{
// 					%new = $WRP::Prevs::ValueCount;
// 					$WRP::Prevs::ValueName[%new] = %name;
// 					$WRP::Prevs::ValueDefault[%new] = %def;
//					export("$WRP::Prevs::*", "config/server/WorldRP/Prevs.cs");
// 					deleteVariables("$WRP::Class::*");
// 				}
// 			}
// 		}
// 	}
// };
// WRPRegPackage(WRPPrevs);