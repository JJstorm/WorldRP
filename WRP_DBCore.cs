// Sassy 1.3
// By Jookia under the GPL v3 license.
//
// Update 1.1
//  Cleaned up the code style.
//  Added error stacks.
//  Added a pref array feature.
//  Added a shared isValue function.
//
// Update 1.2
//  Removed getData function.
//  Added dataID variable.
//  Fixed a pref array bug.
//  Removed need for names.
//  Fixed up some more code style stuff.
//
// Update 1.3
//  SassyData added.
//  Optimization. Lots of it.
//  Fixed up even more code style.
//  Made the pref array less hacky.
// 
// Todo
//  Blank line checking.

// SassyData is like a file header.
$EywaData = "1.3"; // Version.

function Eywa::onAdd(%this)
{
	%errorStack = "";
	
	if($server::LAN == true)
		%errorStack = "ERROR: Eywa::onAdd(): Eywa does not support LAN servers!";
	
	if(%this.dataFile $= "")
		%errorStack = %errorStack @ ((%errorStack !$= "") ? "\n" : "") @ "ERROR: Eywa::onAdd(): Eywa needs a dataFile variable!";
	
	if(%errorStack !$= "")
	{
		if(%this.Debug) { warn("WorldRP Debug (Database): " @ %errorStack); }
		
		%this.schedule(0, "delete");
		
		return false;
	}
	
	%this.valueCount = 0;
	%this.dataCount = 0;
	
	if(isFile(%this.dataFile) == true)
	{
		if(%this.Debug) { warn("WorldRP Debug (DB ERROR): Eywa::onAdd(): Previous save file found. Loading.."); }
		
		%this.loadData();
	}
	
	return true;
}

function Eywa::onRemove(%this)
{
	for(%a = 1; %a <= %this.dataCount; %a++)
	{
		if(%this.Debug) { warn("WorldRP Debug (Database): " @ %this.data[%a] @ " Deleted"); }
		
		%this.data[%a].delete();
	}
	
	return true;
}

function Eywa::saveData(%this)
{
	%file = new fileObject();
	%file.openForWrite(%this.dataFile);
	
	%file.writeLine(":" @ $EywaData @ "\n");
	%file.writeLine("VALUES");
	
	for(%a = 1; %a <= %this.valueCount; %a++)
		%file.writeLine(" " @ %this.value[%a] SPC %this.defaultValue[%a]);
	
	if(%this.dataCount > 0)
		%file.writeLine("");
	
	for(%b = 1; %b <= %this.dataCount; %b++)
	{
		if(isObject(%this.data[%b]) == false)
			continue;
		
		%file.writeLine("ID " @ %this.data[%b].ID);
		
		for(%c = 1; %c <= %this.valueCount; %c++)
			%file.writeLine(" " @ %this.value[%c] SPC %this.data[%b].value[%this.value[%c]]);
		
		if(%b < %this.dataCount)
			%file.writeLine("");
	}
	
	%file.close();
	%file.delete();
	
	return true;
}

function Eywa::loadData(%this)
{
	if(isFile(%this.dataFile) == false)
	{
		if(%this.Debug) { warn("WorldRP Debug (Database): Eywa::loadData(): Data file not found!"); }
		
		return false;
	}
	
	%file = new fileObject();
	%file.openForRead(%this.dataFile);
	
	%currentState = "";
	%fileVersion = 0;
	%EywaVersion = getWord($EywaData, 0);
	%this.valuecount = 0;
	
	while(%file.isEOF() == false)
	{
		%line = %file.readLine();
		
		if(strReplace(%line, " ", "") $= "")
			continue;
		
		if(getSubStr(%line, 0, 1) $= ":")
		{
			%line = getSubStr(%line, 1, strLen(%line) - 1);
			
			%version = getWord(%line, 0);
			
			if(%version > %EywaVersion)
			{
				if(%this.Debug) { warn("WorldRP Debug (Database): Eywa_loadPrefArray([%dataFile: " @ %dataFile @ "]): Attempting to read file from the future!"); }
				return false;
			}
			
			continue;
		}
		
		if(getSubStr(%line, 0, 1) $= " ")
		{
			%line = getSubStr(%line, 1, strLen(%line) - 1);
			
			if(%currentState $= "values")
			{
				%this.valueCount++;
				
				%this.value[%this.valueCount] = getWord(%line, 0);
				%this.defaultValue[%this.valueCount] = getWords(%line, 1, getWordCount(%line) - 1);
			}
			else if(%currentState $= "id" && %this.isValue(getWord(%line, 0)) == true)
				%this.data[%this.dataCount].value[getWord(%line, 0)] = getWords(%line, 1, getWordCount(%line) - 1);
			
			continue;
		}
		
		%currentState = strLwr(getWord(%line, 0));
		
		if(%currentState $= "id" && isObject(%this.dataID[%ID]) == false)
			%this.addData(getWord(%line, 1));
	}
	
	%file.close();
	%file.delete();
	
	return true;
}

function Eywa::addValue(%this, %value, %defaultValue)
{
	%errorStack = "";
	
	if(%value $= "" || %defaultValue $= "")
		%errorStack = "ERROR: Eywa::addValue([value: " @ %value @ "], [defaultValue: " @ %defaultValue @ "]): Incorrect amount of arguments!";
	
	if(%this.isValue(%value) == true)
		%errorStack = %errorStack @ ((%errorStack !$= "") ? "\n" : "") @ "ERROR: Eywa::addValue([value: " @ %value @ "], [defaultValue: " @ %defaultValue @ "]): Value '" @ %value @ "' is already in the database!";
	
	if(getWordCount(%value) > 1)
		%errorStack = %errorStack @ ((%errorStack !$= "") ? "\n" : "") @ "ERROR: Eywa::addValue([value: " @ %value @ "], [defaultValue: " @ %defaultValue @ "]): Values can't be longer then one word!";
	
	if(%errorStack !$= "")
	{
		if(%this.Debug) { warn("WorldRP Debug (Database): " @ %errorStack); }
		
		return false;
	}
	
	%this.valueCount++;
	
	%this.value[%this.valueCount] = %value;
	%this.defaultValue[%this.valueCount] = %defaultValue;
	
	for(%a = 1; %a <= %this.dataCount; %a++)
		%this.data[%a].value[%value] = %defaultValue;
	
	return true;
}

function Eywa::removeValue(%this, %value)
{
	%errorStack = "";
	
	if(%value $= "")
		%errorStack = "ERROR: Eywa::removeValue(): Incorrect amount of arguments!";
	
	if(%this.isValue(%value) == false)
		%errorStack = %errorStack @ ((%errorStack !$= "") ? "\n" : "") @ "ERROR: Eywa::removeValue(): Value '" @ %value @ "' is not found in the database!";
	
	if(%errorStack !$= "")
	{
		if(%this.Debug) { warn("WorldRP Debug (Database): " @ %errorStack); }
		
		return false;
	}
	
	%foundValue = false;
	
	for(%a = 0; %a < %this.valueCount; %a++)
	{
		if(%this.value[%a] $= %value)
		{
			%foundValue = true;
			
			%this.value[%a] = "";
			%this.defaultValue[%a] = "";
			
			continue;
		}
		
		if(%foundValue == true)
		{
			%this.value[%a - 1] = %this.value[%a];
			%this.defaultValue[%a - 1] = %this.defaultValue[%a];
			
			%this.value[%a] = "";
			%this.defaultValue[%a] = "";
		}
	}
	
	%this.valueCount--;
	
	for(%b = 1; %b <= %this.dataCount; %b++)
		%this.data[%b].value[%value] = "";
	
	return true;
}

function Eywa::isValue(%this, %value)
{
	for(%a = 0; %a <= %this.valueCount; %a++)
		if(%this.value[%a] $= %value)
			return true;
	
	return false;
}

function Eywa::addData(%this, %ID)
{
	%errorStack = "";
	
	if(%ID $= "")
		%errorStack = "ERROR: Eywa::addData([ID: " @ %ID @ "]): Incorrect amount of arguments!";
	
	if(isObject(%this.dataID[%ID]) == true)
		%errorStack = %errorStack @ ((%errorStack !$= "") ? "\n" : "") @ "ERROR: Eywa::addData([ID: " @ %ID @ "]): Data for ID '" @ %ID @ "' is already in the database!";
	
	if(%errorStack !$= "")
	{
		if(%this.Debug) { warn("WorldRP Debug (Database): " @ %errorStack); }
		
		return false;
	}
	
	%data = new scriptObject()
	{
		class = EywaData;
		
		ID = %ID;
		parent = %this;
	};
	
	%this.dataCount++;
	%this.data[%this.dataCount] = %data;
	%this.dataID[%ID] = %data;
	
	return true;
}

function Eywa::removeData(%this, %ID)
{
	%errorStack = "";
	
	if(%ID $= "")
		%errorStack = "ERROR: Eywa::removeData([ID: " @ %ID @ "]): Incorrect amount of arguments!";
	
	if(isObject(%this.dataID[%ID]) == false)
		%errorStack = %errorStack @ ((%errorStack !$= "") ? "\n" : "") @ "ERROR: Eywa::removeData([ID: " @ %ID @ "]): Data for ID '" @ %ID @ "' is not found in the database!";
	
	if(%errorStack !$= "")
	{
		if(%this.Debug) { warn("WorldRP Debug (Database): " @ %errorStack); }
		
		return false;
	}
	
	%foundID = false;
	
	for(%a = 1; %a <= %this.dataCount; %a++)
	{
		if(%this.data[%a].ID == %ID)
		{
			%foundID = true;
			
			%this.data[%a].delete();
			%this.data[%a] = "";
			
			continue;
		}
		
		if(%foundID == true)
		{
			%this.data[%a - 1] = %this.data[%a];
			%this.data[%a] = "";
		}
	}
	
	%this.dataCount--;
	
	return true;
}

function Eywa::getData(%this, %ID)
{
	for(%a = 0; %a <= %this.dataCount; %a++)
	{
		if(%this.data[%a].ID == %ID)
		{
			return %this.data[%a];
		}
	}
	
	return false;
}

function EywaData::onAdd(%this)
{
	%errorStack = "";
	
	if(%this.ID $= "")
		%errorStack = "ERROR: EywaData::onAdd(): ID variable not specified!";
	
	if(!isObject(%this.parent))
		%errorStack = %errorStack @ ((%errorStack !$= "") ? "\n" : "") @ "ERROR: EywaData::onAdd(): Parent object not found!";
	
	if(%errorStack !$= "")
	{
		if(%this.Debug) { warn("WorldRP Debug (Database): " @ %errorStack); }
		
		return false;
	}
	
	for(%a = 1; %a <= %this.parent.valueCount; %a++)
		%this.value[%this.parent.value[%a]] = %this.parent.defaultValue[%a];
	
	return true;
}

function Eywa_loadPrefArray(%dataFile)
{
	if(isFile(%dataFile) == false)
	{
		echo("ERROR: Eywa_loadPrefArray::loadData(): File not found!");
		
		return false;
	}
	
	%file = new fileObject();
	%file.openForRead(%dataFile);
	
	%currentState = "";
	
	%prefCount = 0;
	%defaultPref = "";
	%countPref = "";
	%ID = 0;
	%fileVersion = 0;
	%EywaVersion = getWord($EywaData, 0);
	
	while(%file.isEOF() == false)
	{
		%line = %file.readLine();
		
		if(strReplace(%line, " ", "") $= "")
			continue;
		
		if(getSubStr(%line, 0, 1) $= ":")
		{
			%line = getSubStr(%line, 1, strLen(%line) - 1);
			
			%version = getWord(%line, 0);
			
			if(%version > %EywaVersion)
			{
				echo("ERROR: Eywa_loadPrefArray([%dataFile: " @ %dataFile @ "]): Attempting to read file from the future!");
				return false;
			}
			
			continue;
		}
		
		if(getSubStr(%line, 0, 1) $= " ")
		{
			%line = getSubStr(%line, 1, strLen(%line) - 1);
			
			if(%version == 0) // Before 1.3 or no header.
			{
				if(%currentState $= "prefs")
				{
					%prefTag[getWord(%line, 0)] = %prefCount++;
					%pref[%prefCount] = getWord(%line, 1);
					%defaultPref[%prefCount] = getWords(%line, 2, getWordCount(%line) - 1);
				}
				else if(%currentState $= "id")
				{
					%pref = %pref[%prefTag[getWord(%line, 0)]];
					%value = getWords(%line, 1, getWordCount(%line) - 1);
					
					eval(strReplace(%pref, "%ID", %ID) @ " = \"" @ %value @ "\";");
				}
			}
			else
			{
				if(%currentState $= "prefs")
				{
					%tag = getWord(%line, 0);
					
					if(%tag $= "Eywa_PREFCOUNT")
						%countPref = getWord(%line, 1);
					else if(%tag $= "Eywa_STARTCOUNT")
						%ID = getWord(%line, 1) - 1;
					else
					{
						%prefTag[%tag] = %prefCount++;
						%pref[%prefCount] = getWord(%line, 1);
						%defaultPref[%prefCount] = getWords(%line, 2, getWordCount(%line) - 1);
					}
				}
				else if(%currentState $= "pref")
				{
					%pref = %pref[%prefTag[getWord(%line, 0)]];
					%value = getWords(%line, 1, getWordCount(%line) - 1);
					
					eval(%pref @ "[" @ %ID @ "] = \"" @ %value @ "\";");
				}
			}
			
			continue;
		}
		
		%currentState = strLwr(getWord(%line, 0));
		
		if(%version == 0 && %currentState $= "id") // Before 1.3.
		{
			%ID = getWord(%line, 1);
			
			for(%a = 1; %a <= %prefCount; %a++)
				eval(strReplace(%pref[%a], "%ID", %ID) @ " = \"" @ (%value = %defaultPref[%a]) @ "\";");
		}
		else if(%currentState $= "pref")
			eval(%countPref @ " = " @ %ID++ @ ";");
	}
	
	%file.close();
	%file.delete();
	
	return true;
}


function SeedOfEywa(%req,%genName,%file)
{
	if(isObject(eval(%gen)))
		eval(%gen @ ".delete()");
		warn(%gen @ ".delete() Executed via SeedOfEywa eval");
	
	// Default DB(s)
	%gen = new scriptObject(%genName)
	{
		class = "Eywa";
		dataFile = "config/server/WorldRP/" @ %file @ ".dat";
		Debug = false;
	};
	if(fileexists(%gen.DataFile))
		
	
	switch$(strlwr(%req))
	{
		case "population":
			%gen.addValue("Name", "NONE");
			%gen.addValue("Online", "0");
			%gen.addValue("Gender", "NONE");
			%gen.addValue("Rank", "1");
			%gen.addValue("Money", "0");
			%gen.addValue("Bank", "0");
			%gen.addValue("Exp", "0");
			// %gen.addValue("Scale", "1 1 1");
			%gen.addValue("Hunger", "100");
			%gen.addValue("Ore", "0" SPC "0" SPC "0" SPC "0"); // Minerals, Wood, Fish, Plastic
			%gen.addValue("Items", "NONE");
			%gen.addValue("Garage", "EMPTY");
		case "rank":
			return;
		case "crop":
			return;
		case "drugs":
			%gen.AddValue("Weed","BUZZCODEEVALHERE");
			
	}
	if(EywaPopulation.Debug) { warn("WorldRP Debug: Population Database Booted"); }
}
SeedOfEywa("population","EywaPopulation","Users");