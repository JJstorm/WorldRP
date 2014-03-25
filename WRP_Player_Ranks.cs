//================================
// Classes aka Ranks
//================================
new scriptObject(EywaRanks)
{
	class = "Eywa";
	dataFile = "config/server/WorldRP/Ranks.dat";
	Debug = false;
};

function EywaRanks::AddRank(%s,%id,%name,%level,%value,%color,%desc,%upgrademsg,%perks)
{
	// ================= Rank Debug ================ \\
	%msg = "WorldRP: Failed adding Rank because";
	if(%s.RankName[%id] $= %name) { %msg = %msg @ ", Rank Already Exists"; }
	if(%id $= "") { %msg = %msg @ ", ID is missing"; }
	if(%name $= "") { %msg = %msg @ ", No name"; }
	if(%level $= "") { %msg = %msg @ ", Level is blank"; }
	if(%value $= "") { %msg = %msg @ ", Value is blank"; }
	if(%color $= "") { %msg = %msg @ ", Color is blank"; }
	if(%desc $= "") { %msg = %msg @ ", No description"; }
	// Show Report
	if(EywaPopulation.Debug && %msg !$= "WorldRP: Failed adding Rank because")
	{ warn(%msg); return; }
	// =============================================== \\
	
	
	
	// Guiness World Records
	%level = strReplace(%level,"_"," ");
	%money = getWord(%level,0);
	%exp = getWord(%level,1);
	if(%money > %s.HighestMoneyValue)
		%s.HighestMoneyValue = %money;
	if(%exp > %s.HighestExp)
		%s.HighestExpValue = %exp;
	
	
	%s.Count++;
	if(%id $= "" || %id <= 0) { %id = %s.Count; }
	%s.RankName[%id] = %name;
	%s.RankLevel[%id] = %level;
	%s.RankValue[%id] = %value;
	%s.RankColor[%id] = getWord(%color,0);
	%s.RankRGBColor[%id] = getWords(%color,1,getWordCount(%color));
	%s.RankDesc[%id] = %desc;
	%s.RankUpgradeMsg[%id] = %upgrademsg;
	%s.RankPerks[%id] = %perks;
	
	// Create Spawn Rank Brick
	%SpawnBrickName = "WRPRank" @ %id @ "SpawnBrickData";
	if(!isObject(%SpawnBrickName))
	{
		datablock fxDtsBrickData(WRPSpawnBrickData : brickSpawnPointData)
		{
			category = "WorldRP Bricks";
			subCategory = "WorldRP Rank Spawns";
			
			uiName = %s.RankName[%id] @ " Rank Spawn";
			
			specialBrickType = "";
			AdminPlantOnly = true;
			
			spawnData = "Rank" @ %id @ "Spawn";
		};
		WRPSpawnBrickData.setName(%SpawnBrickName);
	}
}

function EywaRanks::LoadRanks(%s)
{
	%s.Count = 0;
	
	%dat = "config/server/WorldRP/Ranks.dat";
	
	if(!isFile(%dat))
		WRPCopyFile("Add-Ons/Gamemode_WorldRP/Default Files/Ranks.dat", %dat);
	
	%stream = new fileObject();
	%stream.openForRead(%dat);
	
	while(!%stream.isEOF())
	{
		%line = trim(%stream.readLine());
		
		// Blanks to pass
		if(getSubStr(%line, 0, 2) $= "//" || %line $= "")
			continue;
		
		%valueData = strlwr(getWord(%line, 0));
		%start = strlen(%ValueData) + 1;
		%str = getSubStr(%line,%start,strlen(%line));
		// Stack up temp data for new Rank addition
		switch$(%valueData)
		{
			case "id":
				$WRP::temp::ID = %str;
			case "name":
				$WRP::temp::Name = %str;
			case "level":
				$WRP::temp::Level = %str;
			case "value":
				$WRP::temp::Value = %str;
			case "color":
				$WRP::temp::Color = %str;
			case "desc":
				$WRP::temp::Desc = %str;
			case "upgrademsg":
				$WRP::temp::UpgradeMsg = %str;
			case "perks":
				$WRP::temp::Perks = %str;
		}
		// Make Sure we got all the info we need
		if($WRP::temp::ID !$= "" && $WRP::temp::Name !$= "" && $WRP::temp::Level !$= "" && $WRP::temp::Value !$= "" && $WRP::temp::Color !$= "" && $WRP::temp::Desc !$= "" && $WRP::temp::UpgradeMsg !$= "" && $WRP::temp::Perks !$= "")
		{
			%s.AddRank($WRP::temp::ID,$WRP::temp::Name,$WRP::temp::Level,$WRP::temp::Value,$WRP::temp::Color,$WRP::temp::Desc,$WRP::temp::UpgradeMsg,$WRP::temp::Perks);
			deleteVariables("$WRP::temp::*");
		}
	}
	deleteVariables("$WRP::temp::*");
	%stream.close();
	%stream.delete();
}
if(isObject(EywaRanks)) { EywaRanks.LoadRanks(); }

function EywaRanks::isRank(%s,%f)
{
	if(%f $= "")
		return;
	
	for(%c=0;%c<%s.Count;%c++)
	{
		if(strlwr(%f) $= strlwr(%s.Rankname))
		{
			return true;
		}
	}
}

// BETA    vv--(Flagged for removal)--vv
//=======================================
	function EywaRanks::getRankColor(%s,%RankID,%type)
	{
		if(%RankID < 1 || %RankID $= "")
			return;
		
		%Ccolor = EywaRanks.RankColor[%RankID];
		if(%type $= "hex")
		{
			%num1 = getsubstr(%Ccolor,0,2);
			%num2 = getsubstr(%Ccolor,3,2);
			%num3 = getsubstr(%Ccolor,6,2);
			for(%a=0;%a<3;%a++)
			{
				%anum = (%a * 2) - 1;
				
				%num = getsubstr(%num[%a], %anum, 1);
				switch$(strlwr(%num))
				{
					case "f":
						%Nnum = "1";
					case "e":
						%Nnum = "0.95";
					case "d":
						%Nnum = "0.9";
					case "c":
						%Nnum = "0.85";
					case "b":
						%Nnum = "0.8";
					case "9":
						%Nnum = "0.75";
					case "8":
						%Nnum = "0.7";
					case "7":
						%Nnum = "0.65";
					case "6":
						%Nnum = "0.6";
					case "5":
						%Nnum = "0.55";
					case "4":
						%Nnum = "0.5";
					case "3":
						%Nnum = "0.4";
					case "2":
						%Nnum = "0.2";
					case "1":
						%Nnum = "0.1";
					case "0":
						%Nnum = "0";
				}
				if(%Nnum !$= "")
				{
					%Nnum = %Nnum @ " ";
					%Nnumlist = %Nnumlist @ %Nnum;
				}
			}
			if(getWordCount(%Nnumlist) == 4)
			{
				return %Nnumlist @ "1";
			}
			
		}
		else
		{
			return %Ccolor;
		}
	}