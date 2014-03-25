// Server
// ==============
$WRP::Booted = 1;
// exec("./WRP_Packages.cs"); // Eywa Brings Greeting Gifts!
exec("./WRP_Clock.cs"); // I SUMMON THE TARDIS *Doctor Who 11th Doctor Theme* -- Starts clock at TinyGame start.
exec("./WRP_DBCore.cs"); // Eywa has Landed & Rooting in
exec("./WRP_Datablocks.cs"); // Datablocks Injection
exec("./WRP_Prefs.cs"); // Set Defaultys

// Core
exec("./WRP_Core_Functions.cs"); // Plant Initial setup

exec("./WRP_Core_Commands.cs"); // Load UI/gameplay commands
// exec("./WRP_Core_Zones.cs"); // Setup World Grid


// Player
exec("./WRP_Player_Basics.cs");
// exec("./WRP_Player_Ranks.cs");
// exec("./WRP_Player_Store.cs");
// exec("./WRP_Player_Upgrades.cs");

// New Features
exec("./DRP_content.cs");
exec("./DRP_farming.cs");
exec("./DRP_Functions_misc.cs");
exec("./WRP_ShapeHack.cs");

// vv Client Visor vv
function servercmdTogVisor(%client, %var) { commandToClient(%client,"togvisor",1); }
function servercmdVisor(%client,%var) { commandtoClient(%client,"VisorUpdate",%var); }
function servercmdVisorStats(%client,%var) { commandtoClient(%client,"VisorStatsUpdate",%var); }
// ^^ Client Visor ^^



// Graveyard of Fail aka Archived or Beta BS
// =============================
// exec("./AddonManager.cs");
// exec("./Updater.cs");
// editcolorset(); // Poopsacked due to V21 GameModes with colorset thus no longer neeeded
// 
// === Weather (OLD PRIOR V21) (Put in Add-on Manager Loader)
// deleteVariables("$WRP::Extras::*");
// exec("./Extras/WorldRP_Weather/server.cs");
// == BETA (Realistic Weather aka Collison fix for snow nd shizz)
// exec("./Datablocks.cs"); // Has Weather Drops In It