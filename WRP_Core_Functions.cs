// 1. Booting Shit Functions
// =========================
function LoadCalendar()
{
	if(isObject(Tardis)) { Tardis.delete(); }
	new scriptObject(Tardis) { };
	
	// Months
	Tardis.addMonth("January", "31", "The Month Of A New Year.");
	Tardis.addMonth("February", "29", "The Loving/Caring Month");
	Tardis.addMonth("March", "31", "Third Months Into the Year. Keep up the good work!");
	Tardis.addMonth("April", "30", " and all is well.");
	Tardis.addMonth("May", "31", "Have you got an upgrade yet?");
	Tardis.addMonth("June", "30", "Half way through the year.");
	Tardis.addMonth("July", "31", "Start Stocking Up!");
	Tardis.addMonth("August", "31", "Holidays are comming make sure your ready!");
	Tardis.addMonth("September", "30", "Remember to spend wisely.");
	Tardis.addMonth("October", "31", "Get ready for a good harvest!");
	Tardis.addMonth("November", "30", "Get your new upgrades and plan to have a feast!");
	Tardis.addMonth("December", "31", "Spend it with your fellow Blocklanders.");
	if(EywaPopulation.Debug) { warn("WorldRP Debug: Calendar Months Loaded"); }
	
	// Holidays
	Tardis.AddHoliday("1-1", "NEW YEARS", "Keep up the hard work!");
	Tardis.AddHoliday("4-1", "April Fools", "Time to lay out the pranks!");
	Tardis.AddHoliday("7-4", "The Forth of July", "Celebrate Your Freedom!");
	Tardis.AddHoliday("9-11", "911", "Remember those who lost their lifes on the 9-11-01 attack.");
	Tardis.AddHoliday("10-31", "Halloween", "Happy Halloween!");
	Tardis.AddHoliday("11-25", "Thanksgiving", "Eat up and enjoy!");
	Tardis.AddHoliday("12-25", "Christmas", "\c2M\c0e\c2r\c0r\c2y \c2C\c0h\c2r\c0i\c2s\c0t\c2m\c0a\c2s\c0!");
	Tardis.AddHoliday("12-31", "New Years Eve", "PARTY LIKE THERES NO TOMORROW!");
	Tardis.HolidaysLoaded = "1";
	if(EywaPopulation.Debug) { warn("WorldRP Debug: Calendar Holidays Loaded"); }
	
}


// 2. New Functions
//================================
function gameConnection::onSpawn(%client)
{
	// Misc
	%client.SpawnTime = mfloor($Sim::Time);
	EywaPopulation.getData(%client.bl_id).valueName = %client.name;
	EywaPopulation.getData(%client.bl_id).valueOnline = 1;
	%client.showstats();
	%client.CheckUpgrades();
	if(isObject(%client.player))
	{
		%client.player.giveItems();
		// Rank
		%client.player.setScale(EywaPopulation.getData(%client.bl_id).valueScale);
		%client.player.setShapeNameColor();
		%client.player.setShapeNameDistance();
	}
	// Zone
	%client.isOnRadar = false;
	%client.schedule(1000,"getGPS");
	// Hunger
	%client.HungerTick();
	if(EywaPopulation.getData(%client.bl_id).valueHunger <= 0) { EywaPopulation.getData(%client.bl_id).valueHunger = "20"; }
}

function gameConnection::setprevs(%client)
{
	%list = "Spawn Talk Suicide Upgrade";
	%listd = "0 1 1 1";
	for(%l=0;%l<getWordCount(%list);%l++)
	{
		%prev = getWord(%list, %l);
		if(%prev !$= "")
		{
			eval("%client.can" @ %prev @ " = getword(%listd, %l);");
		}
	}
	if(EywaPopulation.Debug) { warn("WorldRP Debug: Set " @ %client.name @ "'s default prevs"); }
}

function onWRPBoot()
{
	// Prefs
	$WRP::Booted = 1;
	if(isFile("config/server/WorldRP/Prefs.cs"))
		exec("config/server/WorldRP/Prefs.cs");
	
	// Packages
	WRPActivatePackages();
	
	// Gameplay Structures boot
	ZoneSO.LoadZones();
	
	// Pre-Spawn on boot Adjustments
	for(%c=0;%c<ClientGroup.getCount();%c++)
	{
		%client = ClientGroup.getObject(%c);
		
		// Add Data (Adds data if new user joined while not in DB while WRP isn't booted)
		if(!isObject(EywaPopulation.getData(%client.bl_id))) { EywaPopulation.addData(%client.bl_id);	}
		
		// Spawns Players Where They Needa Be
		if(EywaPopulation.getData(%client.bl_id).XYZPos !$= "")
		{
			
		}
		else
		{
			%client.schedule(100,"onSpawn");
		}
	}
}

function onWRPClose()
{
	// Adjust Players
	for(%c=0;%c<ClientGroup.getCount();%c++)
	{
		%client = ClientGroup.getObject(%c);
		%client.spawnPlayer();
		if(isObject(%client.player)) { %client.player.setScale("1 1 1"); }
	}
	Tardis.SaveClock();
	ZoneSO.SaveZones();
	EywaPopulation.saveData();
	export("$WRP::Server::*", "config/server/WorldRP/Prefs.cs");
	export("$WRP::Extras::*", "config/server/WorldRP/Extras/Prefs.cs");


	// ARCHIVED v21 Gamemode Minigames... No longer need vv
	// LoadMiniGame("close");

	WRPDeactivatePackages();
	$WRP::Booted = 0;
}

function gameconnection::gethunger(%client)
{
	if(!$WRP::Booted || !isObject(%client.player) || %client.player.getState() $= "Dead")
		return;
	
	%lvl = EywaPopulation.getData(%client.bl_id).valueHunger;
	// Level 5
	if(%lvl == 100 || %lvl >= 80)
	{
		%client.player.setScale("1.5 1.5" SPC getWord(EywaPopulation.getData(%client.bl_id).valueScale,2));
		return "<color:" @ $WRP::Hunger::color[5] @ ">" @ $WRP::Hunger::word[5];
	} else
	// Level 4
	if(%lvl < 80 && %lvl >= 60)
	{
		%client.player.setScale("1.125 1.125" SPC getWord(EywaPopulation.getData(%client.bl_id).valueScale,2));
		return "<color:" @ $WRP::Hunger::color[4] @ ">" @ $WRP::Hunger::word[4];
	} else
	// Level 3
	if(%lvl < 60 && %lvl >= 40)
	{
		%client.player.setScale("1 1" SPC getWord(EywaPopulation.getData(%client.bl_id).valueScale,2));
		return "<color:" @ $WRP::Hunger::color[3] @ ">" @ $WRP::Hunger::word[3];
	} else
	// Level 2
	if(%lvl < 40 && %lvl >= 20)
	{
		%client.player.setScale("0.75 0.75" SPC getWord(EywaPopulation.getData(%client.bl_id).valueScale,2));
		return "<color:" @ $WRP::Hunger::color[2] @ ">" @ $WRP::Hunger::word[2];
	} else
	// Level 1
	if(%lvl < 20 && %lvl > 0)
	{
		%client.player.setScale("0.5 0.5" SPC getWord(EywaPopulation.getData(%client.bl_id).valueScale,2));
		return "<color:" @ $WRP::Hunger::color[1] @ ">" @ $WRP::Hunger::word[1];
	}
	else
	{
		EywaPopulation.getData(%client.bl_id).valueHunger = "20";
		%client.player.damage(%client.player, "0 0 0", %client.player.getDatablock().maxDamage, $DamageType::WRPStarvation);
		return "\c0Starved to death";
	}
	
	// Bug Fixes
	if(%lvl > 100)
	{
		EywaPopulation.getData(%client.bl_id).valueHunger = "100";
	} else
	if(%lvl < 0)
	{
		EywaPopulation.getData(%client.bl_id).valueHunger = "0";
	}
}

function gameConnection::HungerTick(%client)
{
	if(!$WRP::Booted || !EywaPopulation.getData(%client.bl_id).valueOnline)
		return;
	
	if(EywaPopulation.getData(%client.bl_id).valueHunger > 0)
	{
		if(mFloor($Sim::Time) > (%client.SpawnTime + 60))
		{
			EywaPopulation.getData(%client.bl_id).valueHunger -= 1;
		}
	}
	else
	{
		if(isObject(%client.player))
		{
			%client.player.kill();
		}
	}
	// Every Hour
	%client.schedule(($WRP::Clock::tickSpeed * 1000) * 60, "HungerTick");
}

function gameconnection::ShowStats(%client)
{
	if(!$WRP::Booted)
	{
		if(%WRPStatSchedule)
			cancel(%WRPStatSchedule);
		return;
	}
	
	%font = "<font:Candara:16>";
	// First Line
	// =====================================
	
	// Rank
	%CStat = EywaPopulation.getData(%client.bl_id);
	
	%client.StatPrint = %font @ "\c6Rank: " @ HandleNum(%CStat.valueRank,"Rank");
	
	// Money On Hand
	%client.StatPrint = %client.StatPrint @ " \c7 | \c6Wallet: " @ HandleNum(%CStat.valueMoney,"$");
	
	// Hunger
	%client.StatPrint = %client.StatPrint @ " \c7 | \c6Hunger: " @ %client.gethunger();
	// Minerals
	if(getword(%CStat.valueOre, 0) > 0)
	{
		%client.StatPrint = %client.StatPrint @ " \c7| \c6Minerals: \c3" @ HandleNum(getword(%CStat.valueOre, 0));
	}
	
	// Wood
	if(getword(%CStat.valueOre, 1) > 0)
	{
		%client.StatPrint = %client.StatPrint @ " \c7| \c6Wood: \c3" @ HandleNum(getword(%CStat.valueOre, 1));
	}
	
	// Fish
	if(getword(%CStat.valueOre, 2) > 0)
	{
		%client.StatPrint = %client.StatPrint @ " \c7| \c6Fish: \c3" @ HandleNum(getword(%CStat.valueOre, 2));
	}
	
	// Plastic
	if(getword(%CStat.valueOre, 3) > 0)
	{
		%client.StatPrint = %client.StatPrint @ " \c7| \c6Plastic: \c3" @ HandleNum(getword(%CStat.valueOre, 3));
	}
	
	// Second Line
	// =====================================
	
	// Zone Print
	%client.StatPrint = %client.StatPrint @ "<br>\c6Status: ";
	if(%client.isMoving && !%client.isBuilding)
	{
		%client.StatPrint = %client.StatPrint @ "<color:FFAA00>Traveling";
		%extra = "in";
	}
	else
	{
		if(%client.isBuilding)
		{
			%client.StatPrint = %client.StatPrint @ "\c3Building";
			%extra = "in";
			%client.isBuilding = 0;
		}
		else
		{
			%client.StatPrint = %client.StatPrint @ "<color:00AAFF>Observing";
			%extra = "";
		}
	}
	
	if(%client.InAZone)
	{
		%Znum = ZoneSO.POSIsInAZone(%client.RadarLOC,"num");
		if($WRP::ZoneTypes::DisplayInStats[%Znum])
		{
			%client.StatPrint = %client.StatPrint SPC %extra SPC strReplace($WRP::Zones::Name[%ZNum], "_", " ");
		}
		// Third Line
		// =====================================
		
		// Zone Owner
		%Zinfo = $WRP::Zones::Info[%ZNum];
		if(isWord(%Zinfo,"DisplayFounder")) { %DType = "founder"; } else
		if(isWord(%Zinfo,"DisplayOwner")) { %DType = "owner"; }
		if(%DType !$= "")
			%client.StatPrint = %client.StatPrint @ "<br>\c6" @ $WRP::Zones::Type[%Znum] SPC %DType @ ": \c3" @ strReplace($WRP::Zones::Owner[%Znum], "_", " ");
	}
	
	// When Dead
	if(!isObject(%client.player))
	{
		if(EywaPopulation.getData(%client.bl_id).valueRank <= mFloor(EywaRanks.Count/2)) { %client.StatPrint = "\c6You need to try harder to advance in your rank."; }
		else { %client.StatPrint = "\c6You've put alot of work in to " @ $Pref::Server::Name @ ", please respawn."; }
	}
	commandToClient(%client, 'bottomPrint', %client.StatPrint, 3, false);
	%WRPStatSchedule = %client.schedule(500, "showstats");
}

function gameconnection::ClearCenterPrint(%client)
{
	if(!$WRP::Booted)
		return;
	
	commandtoclient(%client, 'centerprint', " ",10);
}

function GameConnection::StopPlayer(%client,%POS)
{
	if(%POS $= "" || getWordCount(%POS) < 3)
		%POS = %client.player.getTransform();
	%client.player.setVelocity("0 0 0");
	%client.player.setTransform(%POS);
}

function FixUrl(%NonLinkedText)
{
	if(getWordCount(%NonLinkedText) <= 0)
		return;
	
	%domain = getSubStr(%NonLinkedText,7,strlen(%word));
	for(%u=0;%u<getWordCount(%NonLinkedText);%u++) { %text = StrReplace(%NonLinkedText, "http://", "<a:" @ %domain @ ">" @ %domain @ "</a>"); }
	return %text;
}

function IsWord(%str,%word)
{
	for(%w=0;%w<=getWordCount(%str);%w++)
	{
		%foundword = strlwr(getWord(%str,%w));
		if(%foundword $= strlwr(%word))
			%isWord = 1;
			return true;
	}
	if(!%isWord) { return false; }
}

function IsStr(%str,%find)
{
	%len = strlen(%str)-1;
	for(%a=0;%a<(%len)-strlen(%find);%a++)
	{
		%fstr = getsubstr(%str,%a,%a+strlen(%find));
		if(strlwr(%fstr) $= strlwr(%find))
		{
			%foundit = 1;
			return %foundit;
		}
	}
	if(!%foundit) { return false; }
}

function HandleNum(%amt,%type,%detail)
{
	if(%type $= "$") { if(%detail $= "") { %detail = "\c2"; } %amt = %detail @ "$" @ abvNum(%amt); } else
	if(%type $= "bool") { if(%amt == 1) { %amt = "Yes"; } else { %amt = "No"; } } else
	if(%type $= "rank") { %detail = "<color:" @ EywaRanks.RankColor[%amt] @ ">"; %amt = %detail @ EywaRanks.RankName[%amt]; }
		
	return %amt;
}

function player::giveItems(%this)
{
	if(getWordCount(EywaPopulation.getData(%this.client.bl_id).valueItems) > 0 && EywaPopulation.getData(%this.client.bl_id).valueItems !$= "NONE")
	{
		%tools = EywaPopulation.getData(%this.client.bl_id).valueItems;
	}
	else
	{
		%tools = $WRP::Server::DefaultItems;
	}
	
	for(%i=0;%i<%this.getDatablock().maxTools;%i++)
	{
		if(!isObject(getWord(%tools, %i)))
		{
			%this.tool[%i] = "";
			messageClient(%this.client, 'MsgItemPickup', "", %i, 0);
		}
		else
		{	
			%this.tool[%i] = nameToID(getWord(%tools, %i));
			messageClient(%this.client, 'MsgItemPickup', "", %i, nameToID(getWord(%tools, %i)));
		}
		
	}
}

function player::saveItems(%this)
{
	for(%i=0;%i<getWordCount(EywaPopulation.getData(%this.client.bl_id).valueItems);%i++)
	{
		%item = getWord(EywaPopulation.getData(%this.client.bl_id).valueItems, %i);
		if(isObject(%tool))
		{
			%Sitems = %Sitems SPC %item.getName();
			EywaPopulation.getData(%this.client.bl_id).valueItems = %Sitems;
		}
		
	}
}
// RP_Core Functions
function abvNum(%number)
{
	// Billion
	if (%number >= 1000000000)
	{
		%text = mFloor(%number / 1000000000) @ "G";
	}
	// Million
	else if (%number >= 1000000)
	{
		%text = mFloor(%number / 1000000) @ "M";
	}
	// Thousand
	else if (%number >= 1000)
	{
		%text = mFloor(%number / 1000) @ "K";
	}
	// Nothing happens
	else
	{
		%text = %number;
	}
	return %text;
}

function WRPCopyFile(%from, %to)
{
	%from = findFirstFile(%from);
	if(!isFile(%from))
		return;
	
	// Open the lid
	%fileFrom = new fileObject();
	%fileTo = new fileObject();
	%fileFrom.openForRead(%from);
	%fileTo.openForWrite(%to);
	
	// Transfering
	while (!%fileFrom.isEOF())
		%fileTo.writeLine(%fileFrom.readLine());
	
	// Shut the can
	%fileTo.close();
	%fileFrom.close();
	%fileTo.delete();
	%fileFrom.delete();
}