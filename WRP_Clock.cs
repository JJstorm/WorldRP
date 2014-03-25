//================================
// Clock
//================================

package WRPClock
{
	function BootClock()
	{
		if(!$WRP::Booted)
			return;
		
		Tardis.LoadClock();
		Tardis.Tick();
	}
	
	function Tardis::LoadClock(%s)
	{
		deleteVariables("$WRP::Clock::*");
		if(isFile("config/server/WorldRP/Clock.cs"))
		{
			exec("config/server/WorldRP/Clock.cs");
			%s.Min = $WRP::Clock::Min;
			%s.Hour = $WRP::Clock::Hour;
			%s.Day = $WRP::Clock::Day;
			%s.Month = $WRP::Clock::Month;
			%s.Year = $WRP::Clock::Year;
			if(EywaPopulation.Debug) { warn("WorldRP: Clock Loaded From File"); }
			
			return;
		}
		// Default
		%s.Min = "0";
		%s.Hour = "1";
		%s.Day = "1";
		%s.Month = "1";
		%s.Year = "1";
		%s.AMPM = "AM";
		$WRP::Clock::Type = "env";
		if(EywaPopulation.Debug) { warn("WorldRP: Clock has started ticking"); }
	}
	
	// The GOLDEN GEAR of The Clock
	function Tardis::tick(%s)
	{
		if(!$WRP::Booted)
			return;
		
		if(%s.Tick)
			cancel(%s.Tick);
		
		// Adjusts the timer accordingly
		if($WRP::Clock::Type $= "env")
		{
			%tickSpeed = $EnvGuiServer::DayLength;
		}
		else if($WRP::Clock::Type $= "CustomTicker")
		{
			%tickSpeed = $WRP::Clock::TickSpeed * 1000;
		}
		%s.Tick = %s.schedule(%tickSpeed, "tick");
		
		%s.Min++;
		%s.MinTick();
	}
	
	function Tardis::addMonth(%s, %name, %days, %desc)
	{
		if(!$WRP::Booted)
			return;
		if(%name $= "" || %days $= "")
			if(EywaPopulation.Debug) { warn("WorldRP Debug: Month " @ %name SPC "was not added, incorrect arguements."); }
		
		%s.TotalMonths++;
		%NewMonth = %s.TotalMonths;
		%s.MonthName[%NewMonth] = %name;
		%s.MonthDays[%NewMonth] = %days;
		%s.MonthDesc[%NewMonth] = %desc;
	}
	
	function Tardis::addHoliday(%s, %HDate, %HName, %HDesc)
	{
		if(!$WRP::Booted)
			return;
		
		if(%HDate $= "" || strstr(%date, "-") !$= "-1" || %HName $= "" || %HDesc $= "")
			if(EywaPopulation.Debug) { warn("WorldRP Debug: Holiday " @ %HName SPC "was not added, incorrect arguements."); }
		
		%s.HolidayCount++;
		%NewHoliday = %s.HolidayCount;
		%s.HolidayDate[%NewHoliday] = %HDate;
		%s.HolidayName[%NewHoliday] = %HName;
		%s.HolidayDesc[%NewHoliday] = %HDesc;
	}
	
	function Tardis::SaveClock(%s)
	{
		$WRP::Clock::Min = %s.Min;
		$WRP::Clock::Hour = %s.Min;
		$WRP::Clock::Day = %s.Day;
		$WRP::Clock::Month = %s.Month;
		$WRP::Clock::Year = %s.Year;
		export("$WRP::Clock::*", "config/server/WorldRP/Clock.cs");
		deleteVariables("$WRP::Clock::*");
	}
	
	function Tardis::CheckClock(%s)
	{
		if(!$WRP::Booted)
			return;
		
		if($WRP::Clock::Type $= "env" || $WRP::Clock::Type $= "CustomTicker")
		{
			// Hours
			if(%s.Min >= 60)
			{
				%s.Min = "0";
				%s.Hour++;
				%s.HourTick();
				%s.getAMPM();
			}
			// Days
			if(%s.Hour >= 24)
			{
				%s.Hour = "1";
				%s.Day++;
				%s.DayTick();
			}
			// Months
			if(%s.Day >= %s.MonthDays[%s.Day])
			{
				%s.Day = "1";
				%s.Month++;
				%s.MonthTick();
			}
			// Years
			if(%s.Month >= %s.TotalMonths)
			{
				%s.month = "1";
				%s.year++;
				%s.YearTick();
			}
		}
		// else if($WRP::Clock::Type $= "CustomTicker") { }
	}
	
	function Tardis::Display(%s,%type)
	{
		// 24-Hour Clock
		if($WRP::Clock::ArmyTime $= "1")
		{
			if(%s.hour < 10)
			{
				%hour = "0" @ %s.hour;
			}
			else
			{
				%hour = %s.hour;
			}
		}
		// 12-Hour Clock
		else if(%s.hour > 12)
		{
			%hour = (%s.hour - 12) @ "\c6:\c3";
		}
		else
		{
			%hour = %s.hour @ "\c6:\c3";
		}
		// Minute Fix-Up
		if(%s.Min < 10 && $WRP::Clock::Type $= "game") { %min = "0" @ %s.Min; } else { %min = %s.Min; }
		
		// Display
		if(%type $= "tick")
		{
			return messageAll('',"\c3" @ %hour @ %min @ " \c6" @ %s.getAMPM());
		}
		else if(%type $= "color")
		{
			return "\c3" @ %hour @ %min @ " \c6" @ %s.getAMPM();
		}
		else
		{
			return %hour @ %min SPC %s.getAMPM();
		}
	}
	
	function Tardis::MinTick(%s)
	{
		if(!$WRP::Booted)
			return;
		
		%s.CheckClock();
	}
	
	function Tardis::HourTick(%s)
	{
		if(!$WRP::Booted)
			return;
		
		// Save DB
		EywaPopulation.savedata();
		
		if(!$WRP::Clock::HourDisplay)
			return;
		%s.Display("tick");
		
		// Weather/Day
		DaySO.getDay();
		if(%s.Hour == 2)
		{
			WeatherSO.getWeather();
		}

	}
	
	function Tardis::DayTick(%s)
	{
		// Day Tick
		if(%s.isHoliday())
			messageAll('',"\c6Today is " @ %s.getHoliday());
	}
	function Tardis::MonthTick(%s)
	{
		if(!$WRP::Booted)
			return;
		
		// Month Tick
		%MonthNum = %s.Month;
		messageAll('',"\c6The month is now \c3" @ %s.MonthName[%MonthNum] @ "\c6, " @ %s.MonthDesc[%MonthNum]);
	}
	
	function Tardis::YearTick(%s)
	{
		if(!$WRP::Booted)
			return;
		
		// Year Tick
		messageAll('',"\c6WOOHOO! Another year gone by! Welcome the new year \c3" @ %s.year @ "\c6!");
	}

	function Tardis::getSuffix(%s, %num)
	{
		// Double digit and if the tens place is 1
		if(strlen(%num) > 1 && getSubStr(%num, (strlen(%num) - 2), 1) $= "1")
		{
			return "th";
		}
		else
		{
			switch(getSubStr(%num, (strlen(%num) - 1), 1))
			{
				case 1: return "st";
				case 2: return "nd";
				case 3: return "rd";
				default: return "th";
			}
		}
	}
	
	function Tardis::getAMPM(%s)
	{
		// 24-Hour Clock
		if($WRP::Clock::ArmyTime)
		{
			return;
		}
		else
		// AM
		if(%s.Hour < 12 && %s.Hour <= 24)
		{
			%s.AMPM = "AM";
		}
		// PM
		else if(%s.Hour > 12 && %s.Hour <= 24)
		{
			%s.AMPM = "PM";
		}
		return %s.AMPM;
	}
	
	function Tardis::isHoliday(%s)
	{
		for(%a=0;%s.HolidayDate[%a] !$= "";%a++)
		{
			if(%s.HolidayDate[%a] $= %s.Month @ "-" @ %s.Day)
				return 1;
		}
	}
	
	function Tardis::getHoliday(%s,%view)
	{
		for(%a=0;%s.HolidayDate[%a] !$= "";%a++)
		{
			if(%s.HolidayDate[%a] $= %s.Month @ "-" @ %s.Day)
			{
				%HCount++;
				if(%HCount > 1) { %divider = "|"; } else { %divider = ""; }
				if(%view $= "NamesOnly")
				{
					%Holidays = "\c3" @ %s.HolidayName[%a] @ %divider @ %Holidays;
				}
				else
				{
					%Holidays = "\c3" @ %s.HolidayName[%a] @ "\c6, " @ %s.HolidayDesc[%a] @ %divider @ %Holidays;
				}
			}
		}
		return %Holidays;
	}
};
WRPRegPackage(WRPClock);