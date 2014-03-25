exec("./gui/adminGui.gui");
exec("./gui/WRPUI.gui");
exec("./gui/Profiles/WorldRPDefaultWindowProfile.cs");

$WRP::Client::Feed[1] = "<a:www.jjstorm.net>Like WorldRP? Show your Support!</a>";
$WRP::Client::Feed[2] = "Eat shit cuz";
$WRP::Client::Feed[3] = "I like smoking weed";
$WRP::Client::Feed[4] = "This is a message to all noobs, Hello!";
$WRP::Client::Feed[5] = "blah blah News blah!";
$WRP::Client::Feed[6] = "  Vivvys's Profile --";
$WRP::Client::Feed[7] = "  JJstorm's Profile --";
$WRP::Client::Feed[8] = "Enjoy the new GUI! Took long enough to make it.";
$WRP::Client::Feed[9] = "Stay posted, check in on the <a:jjstorm.net>forums</a>.";
$WRP::Client::Feed[10] = "\c3JJstorm \c6is \c4cool";

function clientcmdShowWorldRPUI(%arg)
{
	newChatHUD.add("WRPUI");
	if(strlwr(%arg) $= "admin") {%showA = 1;} else {%showA = 0;}
	WRPUI_AdminCP.setVisible(%showA);
	// Welcome Spawn Image
	clientCmdCenterPrint("<bitmap:Add-Ons/GameMode_WorldRP/Images/HelloWorld.png>",4);
	$CRPClient::Open = 1;
}

function clientcmdWRPNewsReel()
{
	$WRP::Client::JJsCool = 1;
	if($Sim::Time-$WRP::Client::FeedStamp>1*30)
	{
		%NewsFeed = getRandom(10);
		WRPUI_Feed.setText("\c6"@ $WRP::Client::Feed[%NewsFeed],7);
		$WRP::Client::FeedStamp = $Sim::Time;
	}
	if(isEventPending(%loop))
		cancel(%loop);
	%loop = schedule(30000,WRPNewsReel());
}