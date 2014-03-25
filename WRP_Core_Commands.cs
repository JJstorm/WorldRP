// ===================================
// Table of Contents
// 1. Commands for Everyone to Use
// 2. Admin Only Commands
// ===================================


// ===================================
// 1. Commands for Everyone to Use:
// ===================================
function servercmdStats(%client,%display,%adisplay)
{
	if(!$WRP::Booted)
		return;

	if(%client.isAdmin && %display !$= "")
	{
		if(isObject(findclientbyname(%display)))
		{
			%target = findclientbyname(%display);
			%display = %adisplay;
		}
		else
		{
			%target = %client;
		}
	}
	else
	{
		%target = %client;
		
	}
	
	messageClient(%target, '', "\c3" @ %target.name @ "\c6(BL_ID:\c3" @ %target.bl_id @ "\c6)");
	messageClient(%target, '', "\c6=======================================");

	// Properly Apply Visual Value
	for(%s=1;%s<=EywaPopulation.valueCount;%s++)
	{
		%stat = EywaPopulation.value[%s];
		%value = EywaPopulation.getData(%target.bl_id).value[%stat];
		
		// Show Values?
		if(%display $= "2") { %extra = "\c6(\c3" @ %value @ "\c6)"; } else { %extra = ""; }
		
		// No need for caps here
		switch$(strlwr(%stat))
		{
			// Wallet
			case "money":
				%value = HandleNum(%value,"$");
			case "bank":
				%value = HandleNum(%value,"$");
			case "rank":
				%value = HandleNum(%value,"rank");
			case "online":
				%value = HandleNum(%value,"bool");
			case "hunger":
				%value = %target.gethunger();
			case "ores":
				messageclient(%target, '', "\c6Minerals: \c3" @ getword(%value,0) @ %extra);
				messageclient(%target, '', "\c6Wood: \c3" @ getword(%value,1) @ %extra);
				messageclient(%target, '', "\c6Fish: \c3" @ getword(%value,2) @ %extra);
				messageclient(%target, '', "\c6Plastic: \c3" @ getword(%value,3) @ %extra);
				%NoShow = 1;
			case "scale":
				%NoShow = 1;
			
			default:
				%value = %value @ %extra;
		}
		if(%NoShow) { %NoShow = 0; } else { messageClient(%target, '', "\c6" @ %stat @ ": \c3" @ %value @ %extra); }
	}
}

function servercmdDate(%client)
{
	%month = Tardis.Month;
	%monthname = Tardis.MonthName[%month];
	%day = Tardis.Day;
	%year = Tardis.Year;
	messageClient(%client, '', "\c6 - Today's Date is \c3" @ %MonthName @ ", \c3" @ %day @ "\c6" @ Tardis.getSuffix(%day) @ " year \c3" @ %year);
	messageClient(%client, '', "\c6 - Current Time is \c3" @ Tardis.Display("color"));
	if(Tardis.isHoliday()) { messageClient(%client, '', "\c6- Todays Holiday(s) are \c3" @ Tardis.getHoliday("NamesOnly")); }
}

function servercmdGender(%client, %gen)
{
	if(!$WRP::Booted)
		return;

	%gen = strlwr(%gen);

	if(%gen $= "male" || %gen $= "female" || %gen $= "swap")
	{
		// Restrict Bug Fix
		if(EywaPopulation.getData(%client.bl_id).valueGender !$= "NONE" && %gen !$= "swap")
			return messageClient(%client, '', "\c6Your gender has already been set, type \c3/gender swap\c6 if you want to have a sex change");
		
		switch$(%gen)
		{
			case "male":
				%msg = "\c6You are now know as a \c3male\c6.";
			case "female":
				%msg = "\c6You are now know as a \c3female\c6.";
			case "swap":
				if(EywaPopulation.getData(%client.bl_id).valueGender $= "NONE")
				{
					return messageClient(%client,'',"\c6You can't swap your gender, you haven't selected your gender yet!");
				}
				else if(EywaPopulation.getData(%client.bl_id).valueGender $= "male")
				{
					%gen = "female";
				}
				else if(EywaPopulation.getData(%client.bl_id).valueGender $= "female")
				{
					%gen = "male";
				}
				else
				{
					servercmdgender(%client, %gen);
				}
				%msg = "\c6You have changed your gender to \c3" @ %gen @ "\c6.";
		}
		EywaPopulation.getData(%client.bl_id).valueGender = %gen;
		messageClient(%client, '', %msg);
		%client.canSpawn = "1";
		%client.spawnPlayer();
	}
	else
	{
		return messageClient(%client, '', "\c3" @ %gen @ " \c6is not a valid option.");
	}
}

function servercmdBuyLand(%client,%sizeX,%sizeY,%sizeZ)
{
	if(!$WRP::Booted || %client.player.getState() $= "Dead")
		return;
	if(%sizeY $= "" || %sizeZ $= "") { %sizeY = %sizeX; %sizeZ = %sizeX; }
	%radius = %sizeX SPC %sizeY SPC %sizeZ;
	
	if(%sizeX < 1 || %sizeY < 1 || %sizeZ < 1)
	{
		messageClient(%client, '', "\c6You need to input a size of the land that is between 1 and " @ $WRP::Zones::RadiusLimit @ "\c6.");
		messageClient(%client, '', "\c6Input the size by typing \c3/BuyLand (Size Here)");
		return;
	}
	else if(%sizeX+(%sizeY+%sizeZ) > $WRP::Zones::RadiusLimit)
	{
		return messageClient(%client, '', "\c6The biggest piece of land you can purchase is \c3" @ $WRP::Zones::RadiusLimit @ " \c6blocks.");
	}
	
	if(EywaPopulation.getData(%client.bl_id).valueMoney >= (%sizeX+(%sizeY+%sizeZ)) * $WRP::Zones::RadiusPrice)
	{
		EywaPopulation.getData(%client.bl_id).valueMoney -= (%sizeX+(%sizeY+%sizeZ)) * $WRP::Zones::RadiusPrice;
		ZoneSO.addZone(%client,"Land",%client.name @"'s_" @ %sizeX @ "x" @ %sizeY @ "x" @ %sizeZ @ "_Land",%radius,"Display_Owner");
		messageClient(%client, '', "\c6You purchased " @ (%sizeX+(%sizeY+%sizeZ)) @ " blocks of land. The max amount of bricks you can build in this zone is " @ (%sizeX+(%sizeY+%sizeZ))*5 @ ".");
		messageClient(%client, '', "\c6To mange your land type \c3/ManageLand");
	}
	else
	{
		return messageClient(%client, '', "\c6You need atleast \c4$" @ (%sizeX+(%sizeY+%sizeZ)) * $WRP::Zones::RadiusPrice @ " \c6to purchase a \c3" @ strReplace(%radius," ","x") @ " \c6 piece of land.");
	}
}

function servercmdManageLand(%client,%arg,%arg2)
{
	if(!$WRP::Booted || %client.player.getState() $= "Dead")
		return;
	
	%arg = strlwr(%arg);
	
	for(%l=0;%l<=$WRP::Zones::Count;%l++)
	{
		if($WRP::Zones::Owner[%l] $= %client.bl_id)
		{
			%Client.LandList = %Client.LandList SPC $WRP::Zones::Name[%l];
			%Client.LandListNums = %Client.LandListNums SPC %l;
		}
	}
	// Show Land
	if(%arg $= "")
	{
		if(getWordCount(%Client.LandList) > 0)
		{
			messageClient(%client, '', "\c6Your Land Names:");
			for(%l=0;%l<=getWordCount(%Client.LandList);%l++)
			{
				%Llist = %Llist @ "\c6" @ getWord(%Client.LandList, %l) @ " \c8| ";
			}
			messageClient(%client, '', %Llist);
			messageClient(%client, '', "-----------------------");
			messageClient(%client, '', "You may edit a piece of land by typing \c3/ManageLand (Land Name Here)");
			return;
		}
		else
		{
			return messageClient(%client, '', "\c6You have no land.");
		}
	}
	// First Stage Of Editting Land
	else if(%arg $= "editname" || %arg $= "editsize")
	{
		if(%Client.ZoneEditNum > 0)
		{
			%client.edittingZone = 1;
			switch$(%arg)
			{
				case "editname":
					messageClient(%client, '', "\c6Type the new name of your land in the chatbox.");
					%client.ZoneEdit = "name";
				case "editsize":
					messageClient(%client, '', "\c6Type the new size of your land in the chatbox.");
					%client.ZoneEdit = "radius";
			}
		}
		else
		{
			return messageClient(%client, '', "\c6You haven't selected a piece of land yet.");
		}
	}
	// Final Stage Of Editting Land
	else if(%client.edittingZone && %client.ZoneEdit !$= "" && %arg $= "edit" && %arg2 !$= "")
	{
		eval("$WRP::Zones::" @ %client.ZoneEdit @ %Client.ZoneEditNum @ " = %arg2;");
		messageClient(%client,'',"\c6Your lands " @ %client.ZoneEdit @ " is now \c3" @ %arg2 @ "\c6.");
		%client.ZoneEdit = "";
		%client.edittingZone = 0;
		%client.ZoneEditNum = "";
		return;
	}
	// Land Picker
	else if(%arg !$= "")
	{
		for(%l=0;%l<=getWordCount(%Client.LandList);%l++)
		{
			if(%arg $= strlwr(getWord(%client.LandList,%l)))
			{
				%Client.ZoneEditNum = getWord(%Client.LandListNums,%l);
			}
		}
		if(%Client.ZoneEditNum !$= "")
		{
			messageClient(%client,'',"\c6You have chosen " @ $WRP::Zones::Name[%Client.ZoneEditNum] @ "\c6.");
			messageClient(%client,'',"\c6You can edit the name of this land by typing \c3/ManageLand editname");
			messageClient(%client,'',"\c6You can edit the size of this land by typing \c3/ManageLand editsize");
		}
		else
		{
			return messageClient(%client,'',"\c6You do not own a piece of land with the name \c3" @ %arg @ "\c6.");
		}
	}
}

function servercmdZoneStats(%client,%ZNum)
{
	if(%client.isAdmin && %ZNum > 0) { } else { %ZNum = ZoneSO.POSIsInAZone(%client.RadarLOC,"num"); }
	
	// Max
	if(%ZNum > $WRP::Zones::Count)
		%ZNum = $WRP::Zones::Count;
	// Min
	if(%ZNum < 1)
		%ZNum = "1";
	CommandToClient(%client, 'messageBoxOK', strReplace($WRP::Zones::Name[%Znum], "_", " ") @ " Stats", "Name: " @ $WRP::Zones::Name[%Znum] @ "<br>Type: " @ $WRP::Zones::Type[%ZNum] @ "<br>Owner: " @ $WRP::Zones::Owner[%Znum] @ "<br>Population: " @ $WRP::Zones::Population[%Znum] @ "<br>Bricks: " @ $WRP::Zones::Bricks @ "<br>Number: " @ $WRP::Zones::Number[%ZNum] @ "<br>Info: " @ $WRP::Zones::Info[%ZNum] @ "<br>Radius: " @ $WRP::Zones::Radius[%ZNum]);
}

// ===================================
// 2. Admin Only Commands:
// ===================================
function servercmdBootWRP(%client)
{
	if($WRP::Booted || !%client.isAdmin)
		return;
	
	messageAll('', "\c3" @ %client.name @ "\c6, activated WorldRP. Loading...");
	echo(%client.name @ ", activated WorldRP.");
	
	// 1st time on WorldRP or a fresh start
	%path = "config/server/WorldRP/";
	if(!isFile(%path @ "prefs.cs") && !isFile(%path @ "Users.dat") && !isFile(%path @ "City_Stats.dat"))
	{
		ZoneSO.addZone(%client,"City","WorldRP 1st City","100 100 100","1stCity DisplayFounder Public");
		schedule(1000,false,"commandToClient",findclientbyname(%client.name),'messageBoxOK',"Welcome To WorldRP!","<color:00AA00>Welcome to WorldRP!<br><br><color:000000>Since this is your 1st time on WorldRP, the 1st city has been made at your location.<br><br>You need to name your city, give it a name by using the following command: <color:ff6600>/nameCity NAMEHERE<br><br>-This will occur anytime a system isnt found or you are starting over");
	}
	onWRPBoot();
}

function servercmdCloseWRP(%client)
{
	if(!$WRP::Booted || !%client.isAdmin)
		return;
	
	messageAll('', "\c3" @ %client.name @ "\c6, deactivated WorldRP. Closing...");
	echo(%client.name @ ", deactivated WorldRP.");
	onWRPClose();
}

// 1st time Name City
function servercmdNameCity(%client,%CName)
{
	if(!%client.isAdmin || %CName $="")
		return;
	
	if($WRP::Zones::Name1 $= %client.name @ "'s 1st City")
	{
		messageClient(%client,'',"\c6You have renamed your city to \c3" @ $WRP::Zones::Name1 @ "\c6 to \c3" @ %CName @ "\c6.");
		$WRP::Zones::Name1 = %CName;
	}
}

function servercmdRestrict(%client, %who, %how)
{
	if(!%client.isAdmin || !$WRP::Booted)
		return;
	
	if(%who $= "" || %how $= "")
		return messageClient(%client, '', "\c6Incorrect amount of arguements. Example: \c3/restrict Player_Name Privilege_To_Restrict");
	
	if(findclientbyname(%who) !$= "")
	{
		%target = findClientByName(%who);
	}
	
	
	switch$(strlwr(%how))
	{
		case "spawn":
			if(!%target.CanSpawn) {	%target.CanSpawn = 1; }	else { %target.CanSpawn = 0; }
			%client.RemoveBody();
		case "talk":
			if(!%target.CanTalk) { %target.CanTalk = 1; } else { %target.CanTalk = 0; }
		case "suicide":
			if(!%target.Cansuicide) { %target.CanSuicide = 1; } else { %target.CanSuicide = 0; }
		case "upgrade":
			if(!%target.CanUpgrade) { %target.CanUpgrade = 1; } else { %target.CanUpgrade = 0; }
	}
	%prev = eval("%target.Can" @ %how);
	if(%prev == 0) { %pword = "restricted"; } else { %pword = "derestricted"; }
	messageClient(%target, '', "\c3" @ %client.name @ "\c6 has " @ %pword @ " your ability to \c3" @ %how @ "\c6.");
	messageClient(%client, '', "\c6You have " @ %pword @ " \c3" @ %target.name @ "'s\c6 ability to \c3" @ %how @ "\c6.");
	return;
}
function servercmdGrant(%client,%who,%what,%amt)
{
	if(!$WRP::Booted)
		return;
	if(!%client.isAdmin || %who $= "" || %what $= "" || %amt $= "")
		return;
	
	if(findclientbyname(%who))
		%target = findclientbyname(%who);
	
	%what = strlwr(%what);
	if(%what $= "money" || %what $= "bank")
	{
		if(EywaPopulation.getData(%target.bl_id).valueOnline)
		{
			EywaPopulation.getData(%target.bl_id).value[%what] += %amt;
		}
		else
		{
			return messageClient(%client, '', "\c3" @ %target.name @ "\c6 isn't online");
		}
		if(%what $= "bank") { %extra = " \c6in you \c3Bank account\c6."; } else { %extra = "\c6."; }
		messageClient(%target, '', "\c3" @ %client.name @ "\c6 has granted you a total of \c3$" @ %amt @ %extra);
		messageClient(%client, '', "\c6You have granted " @ %target.name @ " \c6a total of \c3$" @ %amt @ %extra);
	}
	// else if(RESOURCES AND ALL OTHER SHIT WORTH HAVE THE PLEASURE OF /grant'n)


}

// Main Stat editor aka Master of Disaster Stat Blaster
function servercmdeditstat(%client,%who,%stat,%amt1,%amt2,%amt3,%amt4,%amt5)
{
	if(!$WRP::Booted)
		return;
	if(!%client.isAdmin || %who $= "" || !isObject(%client) || %stat $= "" || %amt1 $= "")
		return;
	
	%target = findclientbyname(%who);
	
	if(!EywaPopulation.getData(%target.bl_id).valueOnline)
		return messageclient(%client, '', "\c3" @ %target.name @ " \c6is not online.");

	for(%s=0;%s<EywaPopulation.valueCount;%s++)
	{
		%value = EywaPopulation.value[%s];
		%sstat = EywaPopulation.getData(%target.bl_id).value[%value];
		if(strlwr(%stat) $= strlwr(%value))
		{
			%foundstat = 1;
		}
	}

	if(%foundstat)
	{
		if(getWordCount(EywaPopulation.getData(%target.bl_id).value[%stat]) > 1)
		{
			if(%amt1 !$= "")
				%amt = %amt1;
			if(%amt2 !$= "")
				%amt = %amt SPC %amt2;
			if(%amt3 !$= "")
				%amt = %amt SPC %amt3;
			if(%amt4 !$= "")
				%amt = %amt SPC %amt4;
			if(%amt5 !$= "")
				%amt = %amt SPC %amt5;
		}
		else
		{
			%amt = %amt1;
		}
		EywaPopulation.getData(%target.bl_id).value[%stat] = %amt;
		messageClient(%target, '', "\c3" @ %client.name @ "\c6 has altered your \c3" @ %stat @ " \c6stat to \c3" @ EywaPopulation.getData(%target.bl_id).value[%stat] @  "\c6.");
		messageClient(%client, '', "\c6You altered " @ %target.name @ "\c6's \c3" @ %stat @ " \c6stat to \c3" @ EywaPopulation.getData(%target.bl_id).value[%stat] @  "\c6.");
		%foundstat = 0;
		return;
	}
	else
	{
		return messageClient(%target, '', "\c3" @ %stat @ " \c6isn't a valid stat.");
	}
}

// BETA FIX
// Set $ENVDayOffset
function servercmdForceTick(%client, %type, %howmany)
{
	if(!%client.isAdmin || !$WRP::Booted || %type $= "")
		return;
	if(strlwr($WRP::Clock::Type) !$= "real") { return messageClient(%client, '', "\c6You aren't a time lord, you can't force time to go by on a real clock! --\c3Doctor Who Joke"); }
	
	%type = strlwr(%type);
	%howmany = mFloor(%howmany);
	if(%type $= "" && %howmany $= "")
	{
		servercmdForceTick(%client,"hour","1");
	}
	else if(%type $= "min" || %type $= "hour" || %type $= "day" || %type $= "month" || %type $= "year")
	{
		messageAll('', "\c3" @ %client.name @ " \c6forced 1 " @ %type @ " to pass.");
		eval("Tardis." @ %type @ "++;");
		eval("Tardis." @ %type @ "tick();");
		Tardis.CheckClock();
	}
	else
	{
		return messageClient(%client, '', "\c3" @ %type @ " \c6is not a valid option.");
	}
}

function servercmdNewZone(%client,%type,%name,%sizex,%sizeY,%sizeZ,%info)
{
	if(!$WRP::Booted || %client.player.getState() $= "Dead" || !%client.isAdmin || %type $= "" || %name $= "" || %sizeX < 1 || %sizeY < 1 || %sizeZ < 1)
		return;
	
	if(ZoneSO.isZoneType(%type))
	{
		ZoneSO.addZone(%client,%type,%name,%sizeX @ "_" @ %sizeY @ "_" @ %sizeZ,%info);
	}
	else
	{
		messageClient(%client, "", "\c3" @ %type @ " \c6isn't a valid zone type");
	}
	return;
}

function servercmdEditZone(%client,%num,%what,%arg)
{
	if(!$WRP::Booted || !isObject(%client.player) || %client.player.getState() $= "Dead" || !%client.isAdmin)
		return;
	
	if(strlwr(%num) $= "this")
	{
		if(!%client.InAZone)
			return messageClient(%client,'',"\c6You are not in a zone.");
		for(%z=1;%z<3;%z++)
		{
			switch(%z)
			{
				case 1:
					%type = "City";
				case 2:
					%type = "Land";
				case 3:
					%type = "Zone";
			}
			if(%client.In[%type] !$= "false")
			{
				%num = getWord(%client.In[%type], 1);
			}
		}
	}
	
	if(%num > 0 && %what !$= "" && %arg !$= "")
	{
		switch$(strlwr(%what))
		{
			case "center":
				$WRP::Zones::Center[%num] = %arg;
			case "radius":
				$WRP::Zones::Radius[%num] = %arg;
			case "name":
				$WRP::Zones::Name[%num] = %arg;
			case "owner":
				$WRP::Zones::Owner[%num] = %arg;
			case "type":
				$WRP::Zones::Type[%num] = %arg;
			case "number":
				$WRP::Zones::Number[%num] = %arg;
			case "info":
				%arg = strReplace(%arg, "_", " ");
				$WRP::Zones::Info[%num] = %arg;
				if(getWordCount(%arg) < 2) { messageClient(%client,'',"\c6You can enter multiple words for info, by putting a \c3_ \c6 between each word."); }
			default:
				return messageClient(%client,'',"\c6You need to input what you want edit.");
		}
		// Update Clients
		for(%c=0;%c<ClientGroup.getCount();%c++)
		{
			%client = ClientGroup.getObject(%c);
			if(isObject(%client))
			{
				if(getWord(%client.In[%type], 1) == %num)
				{
					%client.In[%type] = "false";
					%client.Schedule(100,"ClearCenterPrint");
				}
			}
		}
		ZoneSO.SaveZones();
		return messageAll("","\c6" @ $WRP::Zones::Name[%num] @ "'s \c6" @ %what @ " has been changed to \c3" @ %arg @ "\c6.");
	}
	else
	{
		return messageClient(%client,'',"\c6The correct way to use this function is \c3/EditZone (Number) (Zone option to edit) (New zone option string)");
	}
}

function servercmdGotoZone(%client,%id)
{
	if(!$WRP::Booted || !isObject(%client.player) || %client.player.getState() $= "Dead" || !%client.isAdmin)
		return;
	
	if(%id > 0)
	{
		%tele = $WRP::Zones::Center[%id];
	}
	else if(%id !$= "")
	{
		for(%z=1;%z<$WRP::Zones::Count;%z++) { if($WRP::Zones::Name[%z] $= %id) { %tele = $WRP::Zones::Center[%z]; } }
	}
	else
	{
		return;
	}
	if(%tele !$= "") { %client.player.setTransform(%tele); }
}

function ExecToClient(%client, %target, %cmd)
{
	if(!$WRP::Booted || !%client.isAdmin || !isObject(findclientbyname(%target)))
		return;
	
	commandtoclient(%target, "WRPExecCmd", %cmd);
	messageClient(%client, '', "\c2 ++ You Executed " @ %cmd @ " to " @ findclientbyname(%target).name);
}


// BETA ZONE
function servercmdRealWeather(%client,%howmany,%velocity,%sofar)
{
	if(%sofar $= "") { %sofar = "0"; }
	
	if(%sofar<=%howmany)
	{
		%dist = "100";
		
		%randomPosition = setWord(%client.player.getTransform(), 2, getWord(%client.player.getTransform(), 2) + 100);
		%randomPosition = setWord(%randomPosition, 0, getRandom(getWord(%randomPosition, 0) - %dist, getWord(%randomPosition, 0) + %dist));
		%randomPosition = setWord(%randomPosition, 1, getRandom(getWord(%randomPosition, 1) - %dist, getWord(%randomPosition, 1) + %dist));
		%p = new Projectile()
		{
			dataBlock = "WRPWeatherDrop";
			initialPosition = %randomPosition;
			initialVelocity = strReplace(%velocity,"_"," ");
			// sourceObject = %this;
			// client = %client;
			// sourceClient = %client;
			// sourceSlot = 0;
			// originPoint = %start;
		};
		messageAll("","\c6Pinball " @ %soFar @ "/" @ %howmany @ "  Dropped at " @ %p.initialPosition);
	}
	else
	{
		%sofar = 0;
		return;
	}
	schedule(33,false,"servercmdBETA",%client,%howmany,%velocity,%sofar++);
}