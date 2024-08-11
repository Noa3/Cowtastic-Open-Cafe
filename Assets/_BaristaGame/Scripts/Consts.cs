public static class Consts
{
    public const string PrefixNewGameVersion = "1.1.0.0"; //This will be used to reset all gamedata, change if new data should be generated

    //Barista Controller
    public const string BustSize = "BustSize";
    public const string BaristaFullness = "Fullness";
    public const string Happiness = "Happiness";
    public const string BeingMilked = "BeingMilked";
    public const string Clothed = "Clothed";
    public const string Talking = "Talking";
    public const string SurpriseGrowth = "SurpriseGrowth";
    public const string MiniSurpriseGrowth = "MiniSurpriseGrowth";
    public const string Random = "Random";
    public const string BaristaFixApron = "FixOutfit";
    public const string BaristaReset = "Reset";
    public const string BaristaGoodEnd = "GoodEnd";
    public const string BaristaBadEnd = "BadEnd";
    public const string BaristaMouseX = "MouseX";
    public const string BaristaMouseY = "MouseY";
    public const string BaristaHeadPat = "HeadPat";
    public const string BaristaAutoFixOutfit = "AutoFixOutfit";

    #region CupShader
    public const string CupShader_Fullness = "_CupFullness";
    public const string CupShader_MilkType = "_Milk_Type";
    public const string CupShader_MilkTypeColor = "_Custom_Milk_Color";

    public const string CupShader_Espresso = "_FillerEspresso";
    public const string CupShader_Coffee = "_FillerCoffee";
    public const string CupShader_Chocolate = "_FillerChocolate";
    public const string CupShader_Tea = "_FillerTea";
    public const string CupShader_Milk = "_FillerMilk";
    public const string CupShader_BreastMilk = "_FillerBreastMilk";
    public const string CupShader_Cream = "_FillerCream";
    public const string CupShader_Sugar = "_FillerSugar";
    public const string CupShader_Ice = "_ExtraIce";
    public const string CupShader_Boba = "_ExtraBoba";

    public const string CupShader_WhippedCream = "_ToppingCream";
    public const string CupShader_CaramelSauce = "_ToppingCaramel";
    public const string CupShader_ChocolateSauce = "_ToppingChocolate";
    public const string CupShader_Sprinkles = "_ToppingSprinkles";
    #endregion

    #region PlayerPrefs

    public const string PlayerPrefPrefix = "PlayerPref";
    public const string PlayerPrefNextIsTutorial = "NextIsTutorial";
    public const string PlayerPrefNextIsMilkyMode = "NextIsMilkyMode";
    public const string PlayerPrefSoundFx = "SoundFx";
    public const string PlayerPrefMusic = "Music";
    public const string PlayerPrefTalk = "Talk";
    public const string PlayerPrefMilkBarista = "MilkBarista";

    public const string PlayerPrefMilkPreset = "MilkPreset";

    public const string PlayerPrefCanMoveCamera = "MoveCamera";
    public const string PlayerPrefShowBestTimes = "ShowBestTimes";

    public const string PlayerPrefShowIntroPopup = "PlayerPrefShowIntroPopup";

    //BestTimes
    public const string PlayerPrefBestTimeNormal = "BestTimeNormal";
    public const string PlayerPrefBestTimeHard = "BestTimeHard";
    public const string PlayerPrefBestTimeMilkymodeSuffix = "_Milkymode";

    public const string PlayerPrefBestTimeCasual = "BestTimeCasual";
    public const string PlayerPrefBestTimeChaos = "BestTimeChaos";
    public const string PlayerPrefBestTimeUltraChaos = "BestTimeUltraChaos";
    public const string PlayerPrefBestTimeNoasMod = "BestTimeNoasMod";

    public const string PlayerPrefMostEarned = "MostEarned";
    public const string PlayerPrefMostServed = "MostServed";
    public const string PlayerPrefMostMilk = "MostMilk";

    public const string PlayerPrefTextureQuality = "TextureQuality";
    public const string PlayerPrefVsync = "MostServed";
    public const string PlayerPrefAntiAlaising = "MostMilk";
    public const string PlayerPrefAutoFixClothes = "AutoFixClothes";

    public const string PlayerPrefSawAvatarAdventurer = "SawAvatarAdventurer";
    public const string PlayerPrefSawAvatarCat = "SawAvatarCat";
    public const string PlayerPrefSawAvatarFairy = "SawAvatarFairy";
    public const string PlayerPrefSawAvatarHero = "SawAvatarAdventurer";
    public const string PlayerPrefSawAvatarKnight = "SawAvatarKnight";

    //Stats
    public const string PlayerPrefMilkProducedOverall = "OverallMilkProduced";
    public const string PlayerPrefTimePlayedOverall = "OverallTimePlayed";
    public const string PlayerPrefCupsSoldOverall = "OverallCupsSold";
    public const string PlayerPrefCustomersOverall = "OverallCustomers";
    public const string PlayerPrefMoneyEarnedOverall = "OverallMoneyEarned";

    //Etc Prefs
    public const string PlayerPrefRandomSeed = "RandomSeed";
    public const string PlayerPrefCurrentSeed = "CurrentSeed";
    public const string PlayerPrefSceneWon = "SceneWon_";

    //Archievement
    public const string ArchievementAvatarAdventurer = "Avatar Adventurer";
    public const string ArchievementAvatarCat = "Avatar Cat";
    public const string ArchievementAvatarFairy = "Avatar Fairy";
    public const string ArchievementAvatarHero = "Avatar Hero";
    public const string ArchievementAvatarKnight = "Avatar Knight";

    #endregion

    public const string AudioVolumeMusic = "Volume_Music";
    public const string AudioVolumeTalk = "Volume_Talking";
    public const string AudioVolumeEffects = "Volume_Effects";

    public const string ARGSandboxUnlocked = "-SandboxUnlocked";

    /// <summary>
    /// Ask if there are commandline arguments
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetArg(string name)
    {
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == name)
            {
                return args[i + 1];
            }
        }
        return null;
    }

}
