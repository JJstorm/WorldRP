function WRPWeatherDrop::onCollision(%this, %obj, %col, %fade, %pos, %normal)
{
	%obj.schedule(33,delete);
	if(%col.getClassName() !$= "fxDTSBrick")
		return;
	
	%cat = strlwr(%col.getDatablock().category);
	if(!%col.isSnow) // %col.getColorID() != $WRP::Colors::Count-1
	{
		%br = new fxDTSbrick()
		{
			datablock = "brick2x2fPrintData";
			position = %pos;
			angleID = 0;
			colorID = 7; // No need for color correction since Gamemode v21 update
			colorFXID = 0;
			isPlanted = 1;
			isSnow = 1;
		};
		// %br.setColliding(0);
		%br.setTrusted(1);
		%br.plant();
	}
	else if(%cat $= "ramps" || %cat $= "special")
	{
		%col.oldColor(%col.getColorID());
		%col.setColor($WRP::Colors::Count-1);
	}
}

datablock ProjectileData(WRPWeatherDrop)
{
   projectileShapeName = "Add-Ons/Projectile_Pinball/pinballProjectile.dts";
   explosion           = "";
   bounceExplosion     = "";
   particleEmitter     = "";
   explodeOnDeath = true;

   brickExplosionRadius = 0;
   brickExplosionImpact = 0;             //destroy a brick if we hit it directly?
   brickExplosionForce  = 0;             
   brickExplosionMaxVolume = 0;          //max volume of bricks that we can destroy
   brickExplosionMaxVolumeFloating = 0;  //max volume of bricks that we can destroy if they aren't connected to the ground (should always be >= brickExplosionMaxVolume)

   sound = "";

   muzzleVelocity      = 50;
   velInheritFactor    = 1.0;

   armingDelay         = 10000;
   lifetime            = 10000;
   fadeDelay           = 9500;
   bounceElasticity    = 0;
   bounceFriction      = 0.9;
   isBallistic         = true;
   gravityMod          = 1.0;

   hasLight    = true;
   lightRadius = "20";
   lightColor  = "1 1 1";
	
   uiName = "WRPWeatherDrop"; 
};

datablock ParticleData(WRPWeatherDropParticle)
{
   dragCoefficient      = 3;
   gravityCoefficient   = -0.0;
   inheritedVelFactor   = 0.15;
   constantAcceleration = 0.0;
   lifetimeMS           = 500;
   lifetimeVarianceMS   = 0;
   textureName          = "base/data/particles/dot";
   spinSpeed		   = 0.0;
   spinRandomMin		= 0.0;
   spinRandomMax		= 0.0;
   colors[0]     = "0.5 0.5 0.9 0.2";
   colors[1]     = "0.5 0.5 0.7 0.1";
   colors[2]     = "0.5 0.5 0.5 0.1";

   sizes[0]      = 0.3;
   sizes[1]      = 0.2;
   sizes[2]      = 0.0;

   times[0] = 0.0;
   times[1] = 0.5;
   times[2] = 1.0;

   useInvAlpha = false;
};

// Nulled Not using
// datablock ParticleEmitterData(WRPWeatherDropEmitter)
// {
//    ejectionPeriodMS = 15;
//    periodVarianceMS = 1;
//    ejectionVelocity = 0.25;
//    velocityVariance = 0.0;
//    ejectionOffset   = 0.0;
//    thetaMin         = 0;
//    thetaMax         = 90;
//    phiReferenceVel  = 0;
//    phiVariance      = 360;
//    overrideAdvance = false;
//    particles = "WRPWeatherDropParticle";
// 
//    uiName = "WRPWeatherDropEmitter";
// };