//================================
// Table Of Contents
// 1. SO Functions
//================================

// 1. SO Functions
function ZoneSO::addZone(%s,%client,%type,%name,%radius,%info,%owner)
{
	if(!$WRP::Booted || !%s.IsZoneType(%type) || %name $= "" || %radius $= "" || %radius < 1)
		return;
	
	// Bug Fixes
	if(getWordCount(%name) > 1) { %name = strReplace(%name, " ", "_"); }
	if(getWordCount(%owner) > 1) { %owner = strReplace(%input, " ", "_"); }
	if(%owner $= "") { %owner = %client; }
	
	// Being loaded from file
	if(getWordCount(%client) > 1)
	{
		%center = %client;
		%new++;
	}
	// Client is adding zone
	else if(isObject(%client) && getWordCount(%client) == 1)
	{
		serverplay3d(errorSound,%client.player.getHackPosition());
		%center = getWords(%client.RadarLOC,0,2);
		if(strstr(%radius, "_") !$= "-1") { %radius = strReplace(%radius, "_", " "); }
		%owner = %client.name;
		MessageClient(%client, '', "\c6New " @ %type @ ", \c3" @ %name @ " \c6made at \c3" @ %client.RadarLOC @ " \c6with a \c3radius \c6of \c3" @ %radius @ "\c6.");
		$WRP::Zones::Count++;
		%new = $WRP::Zones::Count;
	}
	$WRP::Zones::Center[%new] = %center;
	$WRP::Zones::Radius[%new] = %radius;
	$WRP::Zones::Name[%new] = %name;
	$WRP::Zones::Owner[%new] = %owner;
	$WRP::Zones::Type[%new] = %type;
	$WRP::Zones::Number[%new] = %new;
	$WRP::Zones::Info[%new] = %info;
	$WRP::Zones::Population[%new] = 0;
	$WRP::Zones::Bricks[%new] = 0;
}

function ZoneSO::RemoveZone(%s,%Znum)
{
	if(%Znum<1)
		return;
	
	%s.onRemoveZone(%ZNum);
	%old = $WRP::Zones::Count;
	%new = $WRP::Zones::Count-1;
	if($WRP::Zones::Count > %Znum)
	{
		$WRP::Zones::Center[%new] = $WRP::Zones::Center[%old];
		$WRP::Zones::Radius[%new] = $WRP::Zones::Radius[%old];
		$WRP::Zones::Name[%new] = $WRP::Zones::Name[%old];
		$WRP::Zones::Owner[%new] = $WRP::Zones::Owner[%old];
		$WRP::Zones::Type[%new] = $WRP::Zones::Type[%old];
		$WRP::Zones::Number[%new] = $WRP::Zones::Number[%old];
		$WRP::Zones::Info[%new] = $WRP::Zones::Info[%old];
	}
	else
	{
		$WRP::Zones::Center[%old] = "";
		$WRP::Zones::Radius[%old] = "";
		$WRP::Zones::Name[%old] = "";
		$WRP::Zones::Owner[%old] = "";
		$WRP::Zones::Type[%old] = "";
		$WRP::Zones::Number[%old] = "";
		$WRP::Zones::Info[%old] = "";
	}
	$WRP::Zones::Count--;
}

function ZoneSO::onRemoveZone(%s,%Znum)
{
	if(%ZNum $= "" || %ZNum <= 0)
		return;
	
	// BETA
	// for(%z=1;%z<$WRP::Zones::Count;%z++)
	// {
	// 	if($WRP::Zones::Type[%z] $= "")
	// 	{
	// 			
	// 	}
	// }
}

function ZoneSO::addZoneType(%s,%Name,%DNStats,%DNCenter,%DEMsg,%DLMsg)
{
	if(%name $= "" || strlwr(%name) $= "none" || %DNStats $= "" || %DNCenter $= "")
		return;
	
	if(%s.isZoneType(%Name))
	{
		if(EywaPopulation.Debug) { warn("WorldRP Debug: " @ %Name @ " zone-type was not added because it already exists."); }
		return;
	}
	$WRP::ZoneTypes::Count++;
	%new = $WRP::ZoneTypes::Count;
	$WRP::ZoneTypes::Name[%new] = %name;
	$WRP::ZoneTypes::DisplayInStats[%new] = %DNStats;
	$WRP::ZoneTypes::DisplayInCenter[%new] = %DNCenter;
	$WRP::ZoneTypes::EnterMessage[%new] = %DEMsg;
	$WRP::ZoneTypes::LeaveMessage[%new] = %DLMsg;
}

function ZoneSO::RemoveZoneType(%s,%Ztype)
{
	%Znum = %s.getZoneTypeNum(%Ztype);
	if(%Znum<1 || %Ztype $= "" || !%s.isZoneType(%Ztype))
		return;
	
	%s.onRemoveZoneType(%Ztype);
	%new = $WRP::ZoneTypes::Count-1;
	%old = $WRP::ZoneTypes::Count;
	if($WRP::ZoneTypes::Count > %Znum)
	{
		$WRP::ZoneTypes::Name[%new] = $WRP::ZoneTypes::Name[%old];
		$WRP::ZoneTypes::DisplayInStats[%new] = $WRP::ZoneTypes::DisplayInStats[%old];
		$WRP::ZoneTypes::DisplayInCenter[%new] = $WRP::ZoneTypes::DisplayInCenter[%old];
		$WRP::ZoneTypes::EnterMessage[%new] = $WRP::ZoneTypes::EnterMessage[%old];
		$WRP::ZoneTypes::LeaveMessage[%new] = $WRP::ZoneTypes::LeaveMessage[%old];
	}
	else
	{
		$WRP::ZoneTypes::Name[%old] = "";
		$WRP::ZoneTypes::DisplayInStats[%old] = "";
		$WRP::ZoneTypes::DisplayInCenter[%old] = "";
		$WRP::ZoneTypes::EnterMessage[%old] = "";
		$WRP::ZoneTypes::LeaveMessage[%old] = "";
	}
	$WRP::ZoneTypes::Count--;
}

function ZoneSO::onRemoveZoneType(%s,%Ztype)
{
	// Fix the zones
	for(%z=1;%z<$WRP::Zones::Count;%z++)
	{
		if(strlwr($WRP::Zones::Type[%z]) $= strlwr(%Ztype))
		{
			$WRP::Zones::Type[%z] = "Zone";
		}
	}
}

function ZoneSO::getZoneType(%s,%client)
{
	if(!isObject(%client) || !$WRP::Booted)
		return;
	
	for(%t=1;%t<=$WRP::ZoneTypes::Count;%t++)
	{
		%type = $WRP::ZoneTypes::Name[%t];
		if(getWordCount(%client.In[%type]) > 1)
		{
			%foundtype = %type;
		}
	}
	if(%foundtype !$= "")
		return %type;
}

function ZoneSO::getZoneTypeNum(%s,%type)
{
	if(%type $= "" || !$WRP::Booted)
		return;
	
	for(%t=1;%t<=$WRP::ZoneTypes::Count;%t++)
	{
		if(strlwr(%type) $= strlwr($WRP::ZoneTypes::Name[%t]))
		{
			%FTN = %t;
		}
	}
	if(%FTN > 0)
	{
		return %FTN;
	}
	else
	{
		return 0;
	}
}

function ZoneSO::isZoneType(%s,%type)
{
	for(%z=1;%z<=$WRP::ZoneTypes::Count;%z++)
	{
		if(strlwr(%type) $= strlwr($WRP::ZoneTypes::Name[%z]))
		{
			%isZone = 1;
		}
	}
	if(%isZone) { return 1; } else { return 0; }
}

function ZoneSO::IsInAZone(%s,%client)
{
	if(%s.POSIsInAZone(%client.RadarLOC))
	{
		%client.InAZone = 1;
	}
	else
	{
		%client.InAZone = 0;
	}
}

function ZoneSO::POSIsInAZone(%s,%pos,%request)
{
	if(getWordCount(%pos) < 3)
		return 0;
	
	%posX = getWord(%pos,0);
	%posY = getWord(%pos,1);
	%posZ = getWord(%pos,2);
	
	for(%z=1;%z<=$WRP::Zones::Count;%z++)
	{
		%radius = $WRP::Zones::Radius[%z];
		if(getWordCount(%radius) == 3)
		{
			%radiusX = mAbs(getWord(%radius,0))/2;
			%radiusY = mAbs(getWord(%radius,1))/2;
			%radiusZ = mAbs((getWord(%radius,2)/2)*2.5);
		}
		else
		{
			%radius = mAbs(getWord(%radius,0))/2;
			%radiusX = %radius;
			%radiusY = %radius;
			%radiusZ = %radius*2.5;
		}
		
		%center = $WRP::Zones::Center[%z];
		%type = $WRP::Zones::Type[%z];
		
		// How Far From Center
		%ZoneX = mAbs(getWord(%center,0) - %posX);
		%ZoneY = mAbs(getWord(%center,1) - %posY);
		%ZoneZ = mAbs(getWord(%center,2) - %posZ);
		
		// In Zone
		if(%ZoneX <= %radiusX && %ZoneY <= %radiusY && %ZoneZ <= %radiusZ)
		{
			%InZone = 1;
			%ZoneNum = %z;
		}
		else
		{
			%InZone = 0;
		}
	}
	
	if(%InZone == 1)
	{
		if(strlwr(%request) $= "num")
		{
			return %ZoneNum;
		}
		else
		{
			return 1;
		}
	}
	else
	{
		return 0;
	}
}

function ZoneSO::SaveZones(%s)
{
	if(!$WRP::Booted)
		return;
	if(!isObject(MissionInfo) || getWordCount(MissionInfo.Name) < 1)
	{
		if(EywaPopulation.Debug) { warn("WorldRP Debug: Could not save zones. The map MissionInfo is corrupted."); }
		return;
	}
	export("$WRP::Zones::*","config/server/WorldRP/Zones/" @ strReplace(MissionInfo.Name, " ", "_") @ ".cs");
	export("$WRP::ZoneTypes::*","config/server/WorldRP/Zones/Zone_Types.cs");
	if(EywaPopulation.Debug) { warn("WorldRP Debug: Zones saved for " @ MissionInfo.Name @ "."); }
	return;
}

function ZoneSO::LoadZones(%s)
{
	if(!$WRP::Booted)
		return;
	
	%ZoneFile = "config/server/WorldRP/Zones/" @ strReplace(MissionInfo.Name, " ", "_") @ ".cs";
	if(isFile(%ZoneFile))
	{
		deleteVariables("$WRP::Zones::*");
		$WRP::Zones::Count = "0";
		exec(%ZoneFile);
		for(%z=1;%z<=$WRP::Zones::Count;%z++)
		{
			%s.addZone($WRP::Zones::Center[%z],$WRP::Zones::Type[%z],$WRP::Zones::Name[%z],$WRP::Zones::Radius[%z],$WRP::Zones::Info[%z],$WRP::Zones::Owner[%z]);
		}
		if(EywaPopulation.Debug) { echo("WorldRP Debug: Loaded " @ %z @ " zone(s) for " @ MissionInfo.Name @ "."); }
	}
}

function ZoneSO::onEnterZone(%s,%client,%num)
{
	if(!$WRP::Booted || %client.player.getState() $= "Dead" || !isObject(%client) || %num < 1 || %num > $WRP::Zones::Count)
		return;
	
	%type = $WRP::Zones::Type[%num];
	%client.In[%type] = $WRP::Zones::Name[%num] SPC %num;
	$WRP::Zones::Clients[%num] = $WRP::Zones::Clients[%num] SPC %client;
	%s.setZoneStats(%client,%num);
	%Tnum = %s.getZoneTypeNum(%type);
	%msg = $WRP::ZoneTypes::EnterMessage[%Tnum];
	%msg = strReplace(%msg,"^name",$WRP::Zones::Name[%num]);
	%msg = strReplace(%msg,"^owner",$WRP::Zones::Owner[%num]);
	%msg = strReplace(%msg,"^population",$WRP::Zones::Population[%num]);
	%msg = strReplace(%msg,"^bricks",$WRP::Zones::Bricks[%num]);
	%msg = strReplace(%msg,"_"," ");
	
	// Center Print
	if($WRP::ZoneTypes::DisplayInCenter[%TNum] && $WRP::ZoneTypes::EnterMessage[%Tnum] !$= "")
	{
		commandtoclient(%client, 'centerprint', %msg, "3");
	}
	%s.IsInAZone(%client);
}

function ZoneSO::onLeaveZone(%s,%client,%num)
{
	if(!$WRP::Booted || !isObject(%client) || %num < 1 || %num > $WRP::Zones::Count)
		return;
		
	%type = $WRP::Zones::Type[%num];
	%client.In[%type] = "false";
	$WRP::Zones::Clients[%num] = strReplace($WRP::Zones::Clients[%num],%client,"");
	%s.setZoneStats(%client,%num);
	%Tnum = %s.getZoneTypeNum(%type);
	%msg = $WRP::ZoneTypes::LeaveMessage[%Tnum];
	%msg = strReplace(%msg,"^name",$WRP::Zones::Name[%num]);
	%msg = strReplace(%msg,"^owner",$WRP::Zones::Owner[%num]);
	%msg = strReplace(%msg,"^population",$WRP::Zones::Population[%num]);
	%msg = strReplace(%msg,"^bricks",$WRP::Zones::Bricks[%num]);
	%msg = strReplace(%msg,"_"," ");
	
	// Center Print
	if($WRP::ZoneTypes::DisplayInCenter[%TNum] && $WRP::ZoneTypes::LeaveMessage[%Tnum] !$= "")
	{
		commandtoclient(%client, 'centerprint', %msg, "3");
	}
	%s.IsInAZone(%client);
}

function ZoneSO::setZoneStats(%s,%client,%zoneNum)
{
	$WRP::Zones::Clients[%zoneNum] = trim($WRP::Zones::Clients[%zoneNum]);
	$WRP::Zones::Population[%zoneNum] = getWordCount($WRP::Zones::Clients[%zoneNum]);
}

function ZoneSO::DeleteBrickNotInAZone(%s)
{
	if(!$WRP::Server::AllowPlayersToBuildOutOfZones)
		return;
	
	for(%c=1;%c<=ClientGroup.getCount();%c++)
	{
		%client = ClientGroup.getObject(%c);
		%BrickGroup = getBrickGroupFromObject(%client);
		if(%BrickGroup.getCount>0)
		{
			for(%b=1;%b<=%BrickGroup.getCount();%b++)
			{
				%brick = %BrickGroup.getObject(%b);
				if(%s.PosIsInAZone(%brick))
				{
					if(%brick.InZone $= "" || %brick.inZone <= 0)
					{
						%brick.InZone = %s.PosIsInAZone(%brick,"num");
					}
				}
				else
				{
					%brick.delete();
				}
			}
		}
	}
}

function GameConnection::getGPS(%client)
{
	if(!$WRP::Booted || !isObject(%client.player))
	{
		cancel(%GPSLoop);
		return;
	}
	if(%client.schedule) { cancel(%client.schedule); }
	if(%client.RadarLOC == %client.player.getTransform()) { %client.isMoving = false; } else { %client.isMoving = true; }
	%client.RadarLOC = %client.player.getTransform();
	%client.RadarPOS = mFloor(getWord(%client.RadarLOC,0)) SPC mFloor(getWord(%client.RadarLOC,1)) SPC mFloor(getWord(%client.RadarLOC,2));
	
	if(!%client.isOnRadar)
	{
		%client.SpawnRadarPOS = %client.RadarLOC;
		%client.InAZone = false;
		for(%z=1;%z<$WRP::ZoneTypes::Count;%z++)
		{
			%type = $WRP::ZoneTypes::Name[%z];
			if(ZoneSO.isZoneType(%type))
				%client.In[%type] = "false";
		}
		%client.isOnRadar = true;
	}
	
	// Entering Zone
	if(ZoneSO.PosIsInAZone(%client.RadarLOC))
	{
		%client.ZoneNum = ZoneSO.PosIsInAZone(%client.RadarLOC,"num");
		%client.ZoneType = $WRP::Zones::Type[%client.ZoneNum];
		%client.LastZonePOS = %client.player.getTransform();
		if(%client.In[%client.ZoneType] $= "false")
		{
			ZoneSO.setZoneStats();
			ZoneSO.onEnterZone(%client,%client.ZoneNum);
		}
	}
	else
	// Leaving Zone
	{
		if(%client.In[%client.ZoneType] !$= "false")
		{
			if(isWord($WRP::Zones::Info[%client.ZoneNum], "noleave"))
			{
				%client.StopPlayer(%client.LastZonePOS);
				commandtoclient(%client, 'centerprint', "\c6You cannot leave this zone at this time.", "3");
			}
			else if(ZoneSO.getLeaveLevel(%client.ZoneNum) > 0)
			{
				%LLevel = ZoneSO.getLeaveLevel(%client.ZoneNum);
				if(EywaPopulation.getData(%client.bl_id).valueClass < %LLevel)
				{
					%client.StopPlayer(%client.LastZonePOS);
					commandtoclient(%client, 'centerprint', "\c6You must be atleast a <color:" @ ClassSO.ClassColor[%LLevel] @ ">" @ ClassSO.ClassName[%LLevel] @ " \c6to leave.", "3");
				}
			}
			else
			{
				ZoneSO.setZoneStats();
				ZoneSO.onLeaveZone(%client,%client.ZoneNum);
			}
		}
	}
	%client.schedule = %client.schedule(100,getGPS);
}

function ZoneSO::getLeaveLevel(%s,%ZNum)
{
	if(isStr($WRP::Zones::Info1,"LeaveAtLevel"))
	{
		for(%a=0;%a<getWordCount($WRP::Zones::Info[%ZNUM]);%a++)
		{
			%word = getWord($WRP::Zones::Info[%ZNUM],%a);
			%startPOS = strPos(%word,"_")+1;
			%Level = getSubStr(%word,%startPOS,strlen(%word));
		}
		if(%Level > 0) { return %level; }
	}
}

if(isObject(ZoneSO))
{
	ZoneSO.delete();
}
new ScriptObject(ZoneSO) { };
function LoadZoneTypes()
{
	ZoneSO.addZoneType("City",1,1,"<font:Monotype Corsiva:30>\c6Welcome to \c3^name\n<font:Impact:20>\c6Founder:\c3^owner\n\c6Population: \c3^population\n\c6Bricks: \c3^bricks","<font:Impact:20>\c6Leaving \c3^name \c6District");
	ZoneSO.addZoneType("Land",1,0,"","");
	ZoneSO.addZoneType("Zone",0,1,"\c6^name.","");
	ZoneSO.addZoneType("Hidden",0,0,"","");
}
LoadZoneTypes();