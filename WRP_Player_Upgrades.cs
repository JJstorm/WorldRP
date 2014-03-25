//================================
// Upgrades
//================================
// package WRPUpgrades { };
// WRPRegPackage(WRPUpgrades);

function gameConnection::CheckUpgrades(%client)
{
	if(!$WRP::Booted)
		return;
	
	%CData = EywaPopulation.getData(%client.bl_id);
	// Rank
	if(%CData.valueRank != EywaRanks.Count && (%CData.valueMoney + %CData.valueBank) >= EywaRanks.RankValue[%CData.valueRank+1])
	{
		%client.QualifiedForUpgrade = 1;
		%what = "rank";
		%client.Upgrade(%what);
	}
	%client.schedule(20000,"CheckUpgrades");
}

// !! Make and upgrade system !!
function gameConnection::Upgrade(%client,%what)
{
	if(!$WRP::Booted || %what $= "" || !%client.canUpgrade || !%client.QualifiedForUpgrade)
		return;
	
	%CData = EywaPopulation.getData(%client.bl_id);
	switch$(strlwr(%what))
	{
		case "class":
			%CData.valueClass++;
			messageClient(%client, '', "\c6You are now in the \c3" @ HandleNum(%CData.valueClass,"class") @ " \c6rank, " @ EywaRanks.ClassDesc[%CData.valueClass] @ "\c6.");
			if(strlwr(EywaRanks.ClassUpgradeMsg[%CData.valueClass]) !$= "blank") { messageclient(%client, '', "\c6" @ EywaRanks.ClassUpgradeMsg[%CData.valueClass] @ "\c6."); }
			if(EywaRanks.ClassPerks[%CData.valueClass] !$= "" && EywaRanks.ClassPerks[%CData.valueClass] !$= "NONE")
			{
				// Word For Word Search
				for(%w=0;%w<getWordCount(EywaRanks.ClassPerks[%CData.valueClass]);%w++)
				{
					// Inform User of New Perks
					%newPerk = EywaRanks.ClassPerks[%CData.valueClass];
					if(!isWord(%newPerk,getWord(%w,EywaRanks.ClassPerks[%CData.valueClass-1]))) { messageClient(%client,'',"\c2+1 Perk\c6: " @ strReplace(%perk,"_"," ")); }
				}
			}
		case "weapon":
			warn("Feature Not Available Yet");
			
		case "building":
			warn("Feature Not Available Yet");
	}
	%client.Unlock(%unlock);
	%client.QualifiedForUpgrade = 0;
}

// Used to Unlock things Restricted My WorldRP
function gameConnection::Unlock(%client,%what)
{
	if(!$WRP::Booted)
		return;

	%CData = EywaPopulation.getData(%client.bl_id);

	// What needs Unlocking?
	if(%what $= "")
	{
		// Add in $WRP::Player Pref support for unlockables
		%Unlockables = "scale datablock clothes ammo datablock";


		%MaxUnlockables = getWordCount(%Unlockables);
		for(%c=1;%c<%MaxUnlockables;%c++)
		{
			%Unlock = getWord(%Unlockables,%c);
			if(%Unlock !$= "")
				%client.Unlock(%Unlock);
		}
		return;
	}


	switch$(strlwr(%what))
	{
		// Top Rank Perk (Unlocks: Player Scale,Player Datablock,Clothes,Ammo)
		case "scale":
			if(%CData.valueClass == EywaRanks.Count) { %client.player.setScale(getWord(%client.player.scale, 0) SPC getWord(%client.player.scale, 1) SPC "2"); }
	}
}