using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Burst;

public static class Statics
{
    private static uint ranSeed = 0;

    #region strings

    //Values
    public static string ml = "ml";
    public static string CurrencySymbol = "$";
    public static string LevelPreText = "Lv. ";



    #region Animation

    public static string AnimationAddMoneyTrigger = "AddMoney";
    public static string AnimationSubMoneyTrigger = "SubMoney";
    public static string CupFadeAwayTrigger = "FinishDrink";
    public static string CaveVisualsStatsLightning = "Stats Lightning";
    public static string CustomerDialogMiling = "Milking";
    public static string CustomerDialogFlusterIncrease = "Fluster";

    #endregion


    //Cup Controller

    //public static string CupFullness = "Fullness";

    //public static string CupEspresso = "Espresso";
    //public static string CupCoffee = "_FillerCoffee";
    //public static string CupChocolate = "_FillerChocolate";
    //public static string CupTea = "_FillerTea";
    //public static string CupMilk = "_FillerMilk";
    //public static string CupBreastMilk = "_FillerBreastMilk";
    //public static string CupCream = "_FillerCream";
    //public static string CupSugar = "_FillerSugar";

    //public static string CupIce = "_ExtraIce";
    //public static string CupBoba = "_ExtraBoba";

    //public static string CupWhippedCream = "_ToppingCream";
    //public static string CupCaramelSauce = "_ToppingCaramel";
    //public static string CupChocolateSauce = "_ToppingChocolate";
    //public static string CupSprinkles = "_ToppingSprinkles";


    #region IngreedendNames
    //Fillings
    public static string Espresso = "Espresso";
    public static string Coffee = "Coffee";
    public static string Chocolate = "Chocolate Sauce";
    public static string Tea = "Tea";
    public static string Milk = "Regular Milk";
    public static string BreastMilk = "Breast Milk";
    public static string Cream = "Vanilla Creamer";
    public static string Sugar = "Sugar";
    public static string Ice = "Ice";
    public static string Boba = "Boba";

    //Toppings
    public static string WhippedCream = "Whipped Cream";
    public static string CaramelSauce = "Caramel Sauce";
    public static string ChocolateSauce = "Cocoa Powder";
    public static string Sprinkles = "Candies";
    #endregion

    #region Customer Dialogs

    #region Type
    //[DrinkType] = Based on the main ingredient, check the Drink Types list to the right for what names should apply to what ingredients.
    public static string CustomerDialogDrinkTypeCoffee = "Coffee";
    public static string CustomerDialogDrinkTypeMilk = "Milk";
    public static string CustomerDialogDrinkTypeEspresso = "Latte";
    public static string CustomerDialogDrinkTypeTea = "Tea";
    public static string CustomerDialogDrinkTypeCream = "Shake";
    public static string CustomerDialogDrinkTypeChocolate = "Chocolate Shake";
    #endregion

    #region Modifier
    //[DrinkModifier] = Based on a random secondary ingredient, check the Drink Modifiers list to the right for what words should apply to what ingredients.
    public static string CustomerDialogNoSecondIngreedient = "Regular ";

    //Fillings
    public static string CustomerDialogDrinkModiferEspresso = "Strong ";
    public static string CustomerDialogDrinkModiferCoffee = "Coffee ";
    public static string CustomerDialogDrinkModiferChocolate = "Dark ";
    public static string CustomerDialogDrinkModiferTea = "Tea ";
    public static string CustomerDialogDrinkModiferMilk = "Milk ";
    public static string CustomerDialogDrinkModiferBreastMilk = "Breast Milk ";
    public static string CustomerDialogDrinkModiferCream = "Thick ";
    public static string CustomerDialogDrinkModiferSugar = "Sweet ";
    public static string CustomerDialogDrinkModiferIce = "Iced ";
    public static string CustomerDialogDrinkModiferBoba = "Boba ";

    //Toppings                         
    public static string CustomerDialogDrinkModiferWhippedCream = "Fluffy ";
    public static string CustomerDialogDrinkModiferCaramelSauce = "Fancy ";
    public static string CustomerDialogDrinkModiferChocolateSauce = "Chocolate ";
    public static string CustomerDialogDrinkModiferSprinkles = "Fun ";
    #endregion




    public static string[] CustomerDialogStartGreetings = new string[] { "", "Hi! ", "Hi! ", "Hello. ", "Hello. ", "Hello. ", "Ey, beauty. ", "Hey. ", "Hey. ", "Hey! ", "Hi. ", "Hi. ", "Hi. ", "Hello! ", "Hello! ", "Hi... ", "Hm... ", "Uuuh... ", "Uh... ", "Aha! ", "Yo. ", "Hello... ", "Greetings. ", "Hello young lady. ", "M’lady. ", "Eyo. ", "Sup. ", "Uhuh. ", "Hey there!", "Howdy!", "Hey, what's up? ", "Greetings! ", "Good day! ", "Hola! ", "Hey, stranger. ", "Salutations! ", "Well, hello there! ", "Hi, how are you? ", "Hey, good to see you! ", "Hey, long time no see! ", "Hey, stranger! ", "Hey, what's cooking? ", "Hey, how's it going? ", "Hey, nice to meet you! ", "Hey, good to see you! ", "Hey, what's new? ", "Hey, how's life? ", "Hey, how's everything? ", "Hey, how's your day? ", "Hey, how's your day going? ", "Hey, how's everything going? ", "Hey, how have you been? ", };
    public static string[] CustomerDialogStartPre = new string[] { "", "I want some: ", "Can i get some: ", "Do you have: ", "Could I order: ", "May I have: ", "I'm interested in: ", "Would it be possible to get: ", "Please give me: ", };
    public static string CustomerDialogStartSeperator = ", ";
    public static string CustomerDialogStartEnd = " .";

    public static string[] CustomerDialogSucces = new string[] { "Thank you!" };
    public static string[] CustomerDialogFailed = new string[] { "This was not what I ordererd." };

    public static string TooltipFlusteredNormal = "Feeling Normal";
    public static string TooltipFlusteredLevel1 = "Attention Grabbed";
    public static string TooltipFlusteredLevel2 = "A Bit Flustered";
    public static string TooltipFlusteredLevel3 = "Very Flustered";
    public static string TooltipFlusteredLevel4 = "Super Into It!";

    #endregion

    #region Barista Dialogs

    public static DialogSentence[] BaristaTalk_StartGame_Arcade = {
        new DialogSentence("New job, new life.", "Intro_NewJobNewLife"),
        new DialogSentence("I hope the people here are nice.", "Intro_IHopePeopleAreNice"),
        new DialogSentence("I can’t wait to start!", "Intro_ICantWaitToStart"),
        new DialogSentence("First day on the job.", "Intro_FirstDayOnTheJob") };

    public static DialogSentence[] BaristaTalk_Cookie = {
        new DialogSentence("I want that cookie!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("That cookie looks tasty.", "Spoken_Giggles") ,    //No Voice Line
        new DialogSentence("I wonder how that cookie tastes...?", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("I wonder if the whole cookie will fit in my mouth?", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("When do I get the magic cookie?", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("This job promised certain benefits, right?", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("I want to grow...", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Cookie craving incoming!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Cookie alert! I repeat, cookie alert!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("I've got my eyes on that cookie!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Time to satisfy my cookie cravings!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Cookie mission: Activate!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Cookie radar engaged!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("I'll trade a coffee for a cookie!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("To cookie or not to cookie? That is the question.", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Resistance is futile... when it comes to cookies!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Cookie cravings: 100% satisfaction guaranteed!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Cookie time is the best time!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Cookie dreams are made of this!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Cookie anticipation level: Maximum!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("My sweet tooth is calling for that cookie!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("That cookie is calling my name!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("I've got a one-track mind... and it's on that cookie!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Cookie obsession: Activated!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("All I want for now is that cookie!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("One small step for a barista, one giant leap for cookie-kind!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Cookie cravings: Achieving peak levels!", "Spoken_Giggles") ,   //No Voice Line
        new DialogSentence("I know you have magic for me~", "Spoken_Giggles") };    //No Voice Line


    public static DialogSentence[] BaristaTalk_CookieBuyed = {
        new DialogSentence("Mmm! It tastes good!~", "Reactions_mmmm"),    //No Voice Line
        new DialogSentence("Something feels different...?", "Reactions_mmmm"),    //No Voice Line
        new DialogSentence("I want more!", "Reactions_mmmm") ,    //No Voice Line
        new DialogSentence("It tastes like milk!", "Reactions_mmmm") ,    //No Voice Line
        new DialogSentence("Now we’re getting somewhere!", "Reactions_mmmm") ,    //No Voice Line
        new DialogSentence("Whoa, that feels great!", "Reactions_mmmm") ,    //No Voice Line
        new DialogSentence("I wish we had more of those.", "Reactions_mmmm"),    //No Voice Line
        new DialogSentence("The cookie quest was worth it! Delicious!", "Reactions_mmmm"),    //No Voice Line
        new DialogSentence("That cookie hit the spot! More, please!", "Reactions_mmmm"),    //No Voice Line
        new DialogSentence("Cookie satisfaction achieved!", "Reactions_mmmm"),    //No Voice Line
        new DialogSentence("I'm in cookie heaven right now!", "Reactions_mmmm"),    //No Voice Line
        new DialogSentence("Mmm! Best decision ever!", "Reactions_mmmm"),    //No Voice Line
        new DialogSentence("Cookie bliss! Can't get enough!", "Reactions_mmmm"),    //No Voice Line
        new DialogSentence("This cookie is a game-changer!", "Reactions_mmmm"),    //No Voice Line
        new DialogSentence("Cookie magic at work! I'm loving it!", "Reactions_mmmm"),    //No Voice Line
        new DialogSentence("Who knew one cookie could bring so much joy?", "Reactions_mmmm"),    //No Voice Line
        new DialogSentence("Now this is what I call a cookie masterpiece!", "Reactions_mmmm"),    //No Voice Line
        new DialogSentence("Cookie paradise found!", "Reactions_mmmm"),    //No Voice Line
        new DialogSentence("The taste of victory... and cookies!", "Reactions_mmmm"),    //No Voice Line
        new DialogSentence("That cookie deserves a gold medal!", "Reactions_mmmm"),    //No Voice Line
        new DialogSentence("I'm on cloud nine with this cookie!", "Reactions_mmmm"),    //No Voice Line
        new DialogSentence("Milk and cookies, the perfect combo!", "Reactions_mmmm"),    //No Voice Line
        new DialogSentence("One bite, and I'm hooked! More, please!", "Reactions_mmmm"),    //No Voice Line
        new DialogSentence("Cookie perfection achieved!", "Reactions_mmmm"),    //No Voice Line
        new DialogSentence("Cookie magic activated! Can't stop eating!", "Reactions_mmmm"),    //No Voice Line
        new DialogSentence("This cookie is a taste sensation!", "Reactions_mmmm"),    //No Voice Line
        new DialogSentence("I need a lifetime supply of these cookies!", "Reactions_mmmm"),    //No Voice Line
        new DialogSentence("*Monch Munch*", "Reactions_mmmm") };

    //Right before the apron snaps.  The apron snaps at 0.0175, so activate this at 0.165?  Or 16% depending on which value format is being used.
    public static DialogSentence[] BaristaTalk_ApronLimit = {
        new DialogSentence("The apron is starting to feel a little tight...", "Apron_FeelingTight"),
        new DialogSentence("Do we have a larger apron at the back?", "Apron_LargerApron"),
        new DialogSentence("I hope I can get these stains out again...", "Apron_Stains"),
        new DialogSentence("Uh oh… it’s about to blow!", "Apron_AboutToBlow"),
        new DialogSentence("My apron can’t take any more of this!", "Apron_CantTakeAnyMore"),
        new DialogSentence("Finally!~  Just snap already~", "Apron_JustSnapAlready"),
        new DialogSentence("I think we have a bigger apron in the back, but...", "Apron_InTheBack"),
        new DialogSentence("It’s time.", "Apron_ItsTime") };

    //Barista Milk over 80%  Are these being used?
    public static DialogSentence[] BaristaTalk_TooFull = {
        new DialogSentence("I feel so Full!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Please dont burst on me.", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Do they slosh around?", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("They have grown so big already!", "Spoken_Ah"),     //No Voice Line
        new DialogSentence("Uuuuhm, can you do something about this?", "Spoken_Ah") ,    //No Voice Line
        new DialogSentence("I’ve got a lot more milk than you'd think.  That’s just what I can get in the cup.", "Spoken_Ah") ,    //No Voice Line
        new DialogSentence("Uh, boss... Can you help me with this?", "Spoken_Ah") ,    //No Voice Line
        new DialogSentence("Don’t any customers want some of this?  I’m so full...", "Spoken_Ah") ,    //No Voice Line
        new DialogSentence("I can’t get much fuller than this...", "Spoken_Ah") ,    //No Voice Line
        new DialogSentence("Help?  Please?", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("I feel like I’m going to burst...", "Spoken_Ah") ,    //No Voice Line
        new DialogSentence("I want to get big, but this is a bit uncomfortable...", "Spoken_Ah") ,    //No Voice Line
        new DialogSentence("I need a higher capacity before I can stay this full for long...", "Spoken_Ah") ,    //No Voice Line
        new DialogSentence("Gah!  Just milk me already!", "Moo_Happy") ,    //No Voice Line
        new DialogSentence("Milk me!  Milk me!  Please!", "Moo_Happy"),    //No Voice Line
        new DialogSentence("If this wasn’t so uncomfortable I’d love this...", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("I don’t mind if they watch, just empty me please...", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("I'm feeling so swollen!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("I hope I don't overflow!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Feeling like a milk-filled balloon!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("I'm afraid to move too much...", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Overflow imminent! SOS!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("My cups runneth over, quite literally...", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Do you think anyone would notice if I leaked?", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("I need to relieve some pressure, fast!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Feeling like a milk factory on overdrive!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Can't focus with all this milk sloshing around!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Feels like they're about to burst out of my chest!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Help me out here, they're reaching critical mass!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Is this what they mean by 'milked to perfection'?", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Feels like I'm carrying a dairy farm!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Overflow warning! Danger, danger!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("I'm a milk volcano ready to erupt!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Can't focus with this milky distraction!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Just need to find a way to release all this milk!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("These milk jugs are ready to explode!", "Spoken_Ah"),   //No Voice Line
        new DialogSentence("I’m going to explode.", "Spoken_Ah") };    //No Voice Line

    //This seems unused and redundant, currently the money dialogues serve this purpose.
    public static DialogSentence[] BaristaTalk_BuyUpgrade = {
        new DialogSentence("Time for an upgrade, perhaps?", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Investing in upgrades sounds like a plan!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Let's see what upgrades are available.", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("It might be time to level up our operations.", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Enhancing our business could be beneficial.", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("I wonder if there are any worthwhile upgrades.", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Considering some upgrades for our cafe.", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Exploring upgrade options seems like a good idea.", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Let's boost our efficiency with some upgrades!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Upgrades could give us an edge in the market.", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Should we buy something?", "Spoken_Ah") };    //No Voice Line

    //Not Used Currently. We probably don't need these.
    public static DialogSentence[] BaristaTalk_UpgradeBuyed = {
        new DialogSentence("Oh!  Thank you!~", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("I love this, thank you!~", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Just what I needed!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Wonderful!~", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Keep it coming~", "Spoken_Ah") ,    //No Voice Line
        new DialogSentence("More please~", "Spoken_Ah") ,    //No Voice Line
        new DialogSentence("What a great addition! Thank you!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("This upgrade is a game-changer! Thanks!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("I can already feel the benefits of this upgrade!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Our cafe just got even better with this upgrade!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("I appreciate the support! More upgrades, please!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Thanks for investing in our success!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("One step closer to cafe perfection! Thank you!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("The future looks brighter with this upgrade!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Fantastic choice! Let's keep improving!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("I'm thrilled with this upgrade! More, please!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("The cafe just leveled up! Thanks for the upgrade!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("I'm loving the upgrades! Keep 'em coming!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Upgrade acquired! Let's make our cafe even better!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Thanks for making our cafe even more awesome!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("This upgrade is like a breath of fresh air! Thank you!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("I'm feeling inspired by this upgrade! More, please!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Kudos on the upgrade choice! Let's make magic happen!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Our cafe just got a boost! Thanks for the upgrade!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("This upgrade is a real game-changer! Thank you!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("The upgrade train has left the station! Choo choo!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Upgrade achieved! The cafe's future is looking bright!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("With this upgrade, the sky's the limit! Thank you!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("This upgrade is like music to my ears! Thanks a bunch!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("You've got a knack for picking upgrades! Thanks!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("I'm over the moon about this upgrade! Let's keep 'em coming!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Upgrade unlocked! Our cafe's potential just went up!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("I'm feeling energized by this upgrade! Let's do this!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Upgrade approved! Thanks for investing in our success!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("This upgrade is exactly what we needed! Thank you!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("I've got a good feeling about this upgrade! Thanks!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Thanks~", "Spoken_Ah") };    //No Voice Line

    public static DialogSentence[] BaristaTalk_WelcomingNewCustomer = {
        new DialogSentence("Hello!", "Greetings_Hello"),
        new DialogSentence("Hi there!", "Greetings_HiThere"),
        new DialogSentence("Welcome!", "Greetings_Welcome"),
        new DialogSentence("Can I get you anything?", "Greetings_CanIGetYouAnything"),
        new DialogSentence("The regular?", "Greetings_TheRegular"),
        new DialogSentence("How may I help you?", "Greetings_HowMayIHelpYou"),
        new DialogSentence("What can I get yah?", "Greetings_WhatCanIGetYa"),
        new DialogSentence("Welcome! What can I get you today?", "Greetings_WelcomeWhatCanIGetYou"),
        new DialogSentence("You look good today! What can I bring you?", "Greetings_YouLookGoodToday"),
        new DialogSentence("Today's special: Fresh milk!", "Greetings_TodaysSpecial"),
        new DialogSentence("Have you tried boba milk yet?", "Greetings_BobaMilk") };

    public static DialogSentence[] BaristaTalk_BadEnd = {
        new DialogSentence("That’s it, I quit!", "Quitting_IQuit"),
        new DialogSentence("Worst boss ever... I’m out of here.", "Quitting_IQuit") ,    //No Voice Line
        new DialogSentence("I’m way too sore to work, bye...", "Quitting_IQuit"),    //No Voice Line
        new DialogSentence("I'm done dealing with this chaos! I quit!", "Quitting_IQuit"),    //No Voice Line
        new DialogSentence("I can't take it anymore! I'm leaving!", "Quitting_IQuit"),    //No Voice Line
        new DialogSentence("I deserve better than this! I'm out!", "Quitting_IQuit"),    //No Voice Line
        new DialogSentence("This job is not worth the stress. I'm done!", "Quitting_IQuit"),    //No Voice Line
        new DialogSentence("No amount of coffee is worth this hassle. I'm gone!", "Quitting_IQuit"),    //No Voice Line
        new DialogSentence("I've had enough! Time to find a better opportunity.", "Quitting_IQuit"),    //No Voice Line
        new DialogSentence("I refuse to work under these conditions. I quit!", "Quitting_IQuit"),    //No Voice Line
        new DialogSentence("Life's too short for this nonsense. I'm leaving.", "Quitting_IQuit"),    //No Voice Line
        new DialogSentence("Sorry, but I'm done playing barista. I'm outta here!", "Quitting_IQuit"),    //No Voice Line
        new DialogSentence("I didn't sign up for this! I quit!", "Quitting_IQuit"),    //No Voice Line
        new DialogSentence("Forget this, I’m going home.", "Quitting_IQuit") };    //No Voice Line


    #region Idle Talk

    /*
    * All of these have been redistributed into the Mood and Bust categories, this can be removed.
    * public static string[] BaristaTalk_Idle = { "Praise the Milk!", "I used to be a Cow like you, until I took a Bottle to the knee.", "The right Coffee in the wrong Milk can make all the difference in the Cup.", "Barbara used Milking...it's super effective!", "A man attacked me with cream, butter and milk. How dairy.", "My friend thinks she produces almond milk. She must be nuts." , "My friend is always trying to make her other firnds nervous. She's a fan of milkshakes." , "The milk didn't like my last joke. He wasn't a-moo-sed.",  "There's a lot going on today.", "Cow to the milk: 'I am your father'." , "I wasn't able to milk yesterday. It was an udder failure." , " I love milk when it's churned. It's butter that way.",  "I hope it doesn't rain this afternoon.", "A little wipe over it and everything sparkling clean again!", "The spoiled milk always got what it wanted." , "Spoiler alert! The milk's gone off.",  "I hope we have a few more cups in the back.", "I'd tell you a joke about milk but it's whey too cheesy.", " I introduced chocolate to milk. They did a chocolate milk shake.", "My dad landed a new job at the dairy. He's the cow-ordinator.", "Angry cows are usually to blame for the sour milk.", "Do you get a Milkshake if i jump on a pogostoick?", "Somehow the customers are looking at me strangely today... do I have something on my face?", "I wonder what Lacto is doing right now?", "I wonder what the candy in management is all about?", "I have to give Meowth some milk later..." };
    */

    public static DialogSentence[] BaristaTalk_Idle_Mood_20 = {
        new DialogSentence("Growing bigger isn’t worth it if the job’s this bad...", "Upset_GrowingBiggerIsntWorthIt"),
        new DialogSentence("Angry cows are usually to blame for the sour milk.", "Jokes_AngryCowsSourMilk"),
        new DialogSentence("I’ve had just enough of working here!", "Spoken_Grunt"),    //No Voice Line
        new DialogSentence("Ugh!  I’m about to quit!", "Reactions_Bleh"),    //No Voice Line
        new DialogSentence("This job is terrible!", "Spoken_Grunt") ,    //No Voice Line
        new DialogSentence("I want to go home...", "Spoken_Grunt") ,    //No Voice Line
        new DialogSentence("I don’t get paid enough for this...", "Spoken_Grunt") ,    //No Voice Line
        new DialogSentence("Uuugh...", "Spoken_Grunt"),
        new DialogSentence("This job better improve soon...", "Reactions_Bleh") ,    //No Voice Line
        new DialogSentence("You’d better let me take a break.", "Spoken_Grunt") ,    //No Voice Line
        new DialogSentence("I’m having a bad day...", "Reactions_Bleh") ,    //No Voice Line
        new DialogSentence("Please squeeze me or something...", "Reactions_Bleh") ,    //No Voice Line
        new DialogSentence("I just wanted to be a dairy cow...", "Spoken_Grunt") ,    //No Voice Line
        new DialogSentence("Moo...", "Moo_Tired"),
        new DialogSentence("I can't keep up with this workload...", "Spoken_Grunt"),    //No Voice Line
        new DialogSentence("I feel like I'm at my breaking point...", "Reactions_Bleh"),    //No Voice Line
        new DialogSentence("Is it time to clock out yet?", "Spoken_Grunt"),    //No Voice Line
        new DialogSentence("Every day feels like a struggle...", "Spoken_Grunt"),    //No Voice Line
        new DialogSentence("I'm running on fumes here...", "Reactions_Bleh"),    //No Voice Line
        new DialogSentence("I'm so tired of this routine...", "Spoken_Grunt"),    //No Voice Line
        new DialogSentence("Feels like I'm stuck in a rut...", "Reactions_Bleh"),    //No Voice Line
        new DialogSentence("I can barely keep my eyes open...", "Spoken_Grunt"),    //No Voice Line
        new DialogSentence("Will this shift ever end?", "Reactions_Bleh"),    //No Voice Line
        new DialogSentence("Just gotta push through another day...", "Spoken_Grunt"),    //No Voice Line
        new DialogSentence("I need a vacation... or a miracle.", "Reactions_Bleh"),    //No Voice Line
        new DialogSentence("Wishing for a ray of sunshine in this gloom...", "Spoken_Grunt"),    //No Voice Line
        new DialogSentence("Can't shake off this feeling of exhaustion...", "Reactions_Bleh"),    //No Voice Line
        new DialogSentence("I'm starting to lose my motivation...", "Spoken_Grunt"),    //No Voice Line
        new DialogSentence("Everything just seems so overwhelming...", "Reactions_Bleh"),    //No Voice Line
        new DialogSentence("I'm not sure how much longer I can keep this up...", "Spoken_Grunt"),    //No Voice Line
        new DialogSentence("Just trying to survive the day...", "Reactions_Bleh"),    //No Voice Line
        new DialogSentence("This job is sucking the life out of me...", "Spoken_Grunt"),    //No Voice Line
        new DialogSentence("Feeling like I'm stuck in a never-ending cycle...", "Reactions_Bleh"),    //No Voice Line
        new DialogSentence("I'm so drained... emotionally and physically.", "Spoken_Grunt"),    //No Voice Line
        new DialogSentence("Hoping for a change in luck... and mood.", "Reactions_Bleh"),    //No Voice Line
        new DialogSentence("Just counting down the minutes until I can leave...", "Spoken_Grunt"),    //No Voice Line
        new DialogSentence("I'm so close to calling it quits...", "Reactions_Bleh"),    //No Voice Line
        new DialogSentence("I feel like I'm hitting rock bottom...", "Spoken_Grunt"),    //No Voice Line
        new DialogSentence("I just want to crawl into bed and never come out...", "Reactions_Bleh"),    //No Voice Line
        new DialogSentence("Feels like I'm running on empty...", "Spoken_Grunt"),    //No Voice Line
        new DialogSentence("I'm in desperate need of a pick-me-up...", "Reactions_Bleh"),    //No Voice Line
        new DialogSentence("Just gotta keep putting one foot in front of the other...", "Spoken_Grunt"),    //No Voice Line
        new DialogSentence("Wishing for a spark of energy to get me through...", "Reactions_Bleh"),    //No Voice Line
        new DialogSentence("Is it too much to ask for a little break?", "Spoken_Grunt"),    //No Voice Line
        new DialogSentence("Maybe I'd tell some jokes if I were in a better moo'd.", "Moo_Tired") };    //No Voice Line

    public static DialogSentence[] BaristaTalk_Idle_Mood_40 = {
        new DialogSentence("Is this job always this hard?", "Spoken_Sigh") ,    //No Voice Line
        new DialogSentence("Spoiler alert! The milk's gone off.", "Spoken_Sigh"),    //No Voice Line
        new DialogSentence("I hope it doesn't rain this afternoon.", "Casual_RainThisAfterNoon"),
        new DialogSentence("I need a break...", "Reactions_Bleh") ,    //No Voice Line
        new DialogSentence("The right coffee in the wrong milk can make all the difference in the cup.", "Spoken_Sigh"),    //No Voice Line
        new DialogSentence("Keep it together Barbra, the benefits are worth it...", "Spoken_Sigh") ,    //No Voice Line
        new DialogSentence("Bleh...", "Reactions_Bleh") ,
        new DialogSentence("This is tiring.", "Spoken_Sigh") ,    //No Voice Line
        new DialogSentence("I wish this job was a little nicer.", "Spoken_Sigh") ,    //No Voice Line
        new DialogSentence("This place is really busy, isn’t it?", "Spoken_Sigh") ,    //No Voice Line
        new DialogSentence("I need to restock the supplies.", "Spoken_Sigh") ,    //No Voice Line
        new DialogSentence("Can I take a break?", "Spoken_Sigh") ,    //No Voice Line
        new DialogSentence("There’s so much work to do!", "Spoken_Sigh") ,    //No Voice Line
        new DialogSentence("Can I have a snack?", "Spoken_Sigh") ,    //No Voice Line
        new DialogSentence("Breath in… Breath out...", "Reactions_Bleh") ,    //No Voice Line
        new DialogSentence("This isn’t too different from my last job.", "Spoken_Sigh") ,    //No Voice Line
        new DialogSentence("I could use a little more attention.", "Spoken_Sigh"),    //No Voice Line
        new DialogSentence("Moo...?", "Moo_Curious"),
        new DialogSentence("I'm feeling a bit overwhelmed today.", "Spoken_Sigh"),    //No Voice Line
        new DialogSentence("I hope things get easier soon...", "Spoken_Sigh"),    //No Voice Line
        new DialogSentence("Just trying to keep my head above water...", "Reactions_Bleh"),    //No Voice Line
        new DialogSentence("Is it just me or is today dragging on forever?", "Spoken_Sigh"),    //No Voice Line
        new DialogSentence("Feeling a little burnt out...", "Reactions_Bleh"),    //No Voice Line
        new DialogSentence("Trying to stay positive, but it's tough...", "Spoken_Sigh"),    //No Voice Line
        new DialogSentence("I need a breather... this job can be exhausting.", "Spoken_Sigh"),    //No Voice Line
        new DialogSentence("I wish I could catch a break...", "Reactions_Bleh"),    //No Voice Line
        new DialogSentence("This workload is really testing my limits...", "Spoken_Sigh"),    //No Voice Line
        new DialogSentence("Feeling like I'm in need of a pick-me-up...", "Reactions_Bleh"),    //No Voice Line
        new DialogSentence("Just gotta keep pushing through, I guess...", "Spoken_Sigh"),    //No Voice Line
        new DialogSentence("I wish I had a bit more energy for this...", "Spoken_Sigh"),    //No Voice Line
        new DialogSentence("Is it time to clock out yet?", "Spoken_Sigh"),    //No Voice Line
        new DialogSentence("Could really use a moment to recharge...", "Reactions_Bleh"),    //No Voice Line
        new DialogSentence("Feeling like I'm stuck in a bit of a rut...", "Spoken_Sigh"),    //No Voice Line
        new DialogSentence("I hope tomorrow is a better day...", "Spoken_Sigh"),    //No Voice Line
        new DialogSentence("I'm feeling a bit drained...", "Reactions_Bleh"),    //No Voice Line
        new DialogSentence("Trying to keep my spirits up, but it's tough...", "Spoken_Sigh"),    //No Voice Line
        new DialogSentence("Can't shake off this feeling of exhaustion...", "Reactions_Bleh"),    //No Voice Line
        new DialogSentence("Wishing for a little bit of luck to come my way...", "Spoken_Sigh"),    //No Voice Line
        new DialogSentence("I never knew smiling could be so tiring.", "Spoken_Sigh") };    //No Voice Line

    public static DialogSentence[] BaristaTalk_Idle_Mood_60 = {
        new DialogSentence("I wonder what Lacti is up to?", "Spoken_Hm") ,    //No Voice Line
        new DialogSentence("Do we have a few more cups in the back?", "Casual_CupsInTheBack"),
        new DialogSentence("The spoiled milk always got what it wanted.", "Jokes_SpoiledMilkAlwaysGotWhatItWanted"),
        new DialogSentence("There's a lot going on today.", "Casual_LotGoingOn"),
        new DialogSentence("A man attacked me with cream, butter and milk. How dairy.", "Jokes_HowDairy"),
        new DialogSentence("I used to be a cow like you, until I took a bottle to the breast.", "Jokes_ACowLikeYou"),
        new DialogSentence("I’m feeling productive.", "Casual_FeelingProductive"),
        new DialogSentence("Can you grab me some more coffee from the back?", "Casual_CoffeeFromTheBack"),
        new DialogSentence("The coffee smells so nice.", "Casual_CoffeeSmellsNice"),
        new DialogSentence("The customers keep rolling in.", "Casual_CustomersRollingIn"),
        new DialogSentence("Do you think they like me?", "Casual_DoTheyLikeMe"),
        new DialogSentence("Normal day at the job.", "Casual_NormalDayAtTheJob"),
        new DialogSentence("There we go, counter’s all clean again.", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("I can get kinda moody when I’m tired.", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("I could always use more attention, if you’d like.", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("How do I use the coffee machine again?", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("Do we even need to sell regular milk?", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("Feeling pretty good about today!", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("Looks like it's shaping up to be a great day!", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("I'm on top of things today!", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("Feeling energized and ready to tackle anything!", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("I'm loving the vibe in the cafe today!", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("It's a pleasure to work in such a lovely environment!", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("Happiness is contagious, isn't it?", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("Ready to make some delicious drinks!", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("I'm feeling pretty chipper today!", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("Let's keep the good vibes flowing!", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("Feeling like I can handle anything that comes my way!", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("Everything seems brighter when I'm in a good mood!", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("I've got a smile on my face and coffee in my hand!", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("The coffee tastes even better when I'm happy!", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("I'm like a ray of sunshine today!", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("Today's looking like a great day to make some coffee!", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("I'm in my element when I'm in a good mood!", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("Ready to spread some happiness, one cup at a time!", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("Feeling cheerful and ready to take on the day!", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("Happiness is the secret ingredient in every drink!", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("Moo...?", "Moo_Curious"),
        new DialogSentence("Wait, its not normal for coffee and sugar to make a cowgirl grow?", "Spoken_Hm") };    //No Voice Line

    public static DialogSentence[] BaristaTalk_Idle_Mood_80 = {
        new DialogSentence("I hope Lacti escapes work to see me here today.", "Spoken_Sigh_Happy") ,    //No Voice Line
        new DialogSentence("My friend is always trying to scare her other friends. She's a fan of milkshakes.", "Jokes_FriendScaringOtherFriends"),
        new DialogSentence("My friend thinks she produces almond milk. She must be nuts.", "Jokes_AlmondMilk"),
        new DialogSentence("How are you feeling, boss?", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("Want to go somewhere after work today?", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("This job is surprisingly fun!", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("Anything I can do for you?", "Spoken_Sigh_Happy") ,    //No Voice Line
        new DialogSentence("The customers seem to like me.", "Spoken_Sigh_Happy") ,    //No Voice Line
        new DialogSentence("Hmm hm hmm hhm hmmm!~", "Spoken_Humming"),
        new DialogSentence("What a nice day!", "Spoken_Sigh_Happy") ,    //No Voice Line
        new DialogSentence("Watch this trick I can do with the cups.", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("You’ve been a great boss.", "Spoken_Sigh_Happy") ,    //No Voice Line
        new DialogSentence("Mood swings?  Not if you keep treating me this well!", "Spoken_Sigh_Happy") ,    //No Voice Line
        new DialogSentence("Cows can get kinda MOOdy if they aren't treated as well as you treat me~.", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("I feel like I'm on top of the world today!", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("Isn't it nice to have a job that makes you smile?", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("Feeling so content and happy right now!", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("Every day feels like a good day when you're in a good mood!", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("I could dance with joy right now!", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("My heart feels light and happy today!", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("I've got a skip in my step and a smile on my face!", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("Feels like the stars have aligned today!", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("Isn't life wonderful when you're feeling this good?", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("I feel like I could take on the world right now!", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("I've got a happy heart and a caffeine buzz!", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("Today's shaping up to be a real gem!", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("I'm feeling so grateful for everything right now!", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("Everything just feels so right today!", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("I feel like I'm glowing with happiness!", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("I'm in such a good mood, I could burst!", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("Happiness is contagious, isn't it?", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("Isn't life grand when you're feeling this good?", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("Feeling like I'm walking on air today!", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("I'm feeling so blessed and happy right now!", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("Moo!", "Moo_Happy"),
        new DialogSentence("I’m feeling stretchy.", "Spoken_Sigh_Happy") };    //No Voice Line

    public static DialogSentence[] BaristaTalk_Idle_Mood_100 = {
        new DialogSentence("I’m feeling great!", "Spoken_Giggles") ,    //No Voice Line
        new DialogSentence("Cow to the milk: 'I am your mother'.", "Jokes_CowToTheMilk"),
        new DialogSentence("My dad landed a new job at the dairy. He's the cow-ordinator.", "Jokes_DadAtTheDairy"),
        new DialogSentence("I introduced chocolate to milk. They did a chocolate milkshake.", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("The milk didn't like my last joke. He wasn't a-moo-sed.", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("I love this job!", "Spoken_Giggles") ,    //No Voice Line
        new DialogSentence("Can my day even get any better?", "Spoken_Giggles") ,    //No Voice Line
        new DialogSentence("The customers are being very nice to me today.", "Spoken_Giggles") ,    //No Voice Line
        new DialogSentence("Want to go with me for drinks later?", "Spoken_Giggles") ,    //No Voice Line
        new DialogSentence("We’re doing great!~", "Spoken_Giggles") ,    //No Voice Line
        new DialogSentence("Eeee!~", "Reactions_Eeeee"),
        new DialogSentence("Ahaha!~", "Spoken_Giggles"),
        new DialogSentence("Mooo!~", "Moo_Happiest"),
        new DialogSentence("Wanna give me a squeeze?", "Spoken_Giggles") ,    //No Voice Line
        new DialogSentence("I’ll make sure to save extra milk for you later!~", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("I'm practically floating on air!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("I've got a smile that won't quit!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Today's so good, it feels like a dream!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Is this what pure happiness feels like? It's amazing!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("I'm radiating positivity and joy!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Feeling like I'm on cloud nine today!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("My heart is bursting with happiness!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("I'm so happy, I could burst into song!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Grinning from ear to ear today!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Today feels like a perfect day!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("I'm filled to the brim with joy and laughter!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("I'm living my best life right now!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Feeling like the luckiest barista in the world!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Today's so wonderful, I want to remember it forever!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Can you feel the happiness radiating from me?", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Feeling like I've won the happiness lottery today!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("I'm overflowing with happiness!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Every moment today feels like a gift!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("I'm so grateful for all the happiness in my life!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("I'm bursting with sunshine and rainbows today!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("I feel like I could conquer the world with this happiness!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Life is a beautiful melody and today I'm dancing to the happiest tune!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("I'm positively beaming with joy!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Today feels like a fairytale come true!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("I'm spreading happiness like confetti today!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("I feel like I'm wrapped in a warm, fuzzy blanket of happiness!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("I'm so happy, I could hug the entire world!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Today's happiness level: off the charts!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("I'm radiating so much happiness, I might just cause a joy overload!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("I've got enough happiness to light up the whole city!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("I'm so thrilled, I could do a happy dance right here, right now!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Today feels like a dream I never want to wake up from!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("I'm so filled with joy, it's like every day is my birthday!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("I'm positively buzzing with happiness today!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Moo!~", "Moo_Happier") };

    public static DialogSentence[] BaristaTalk_Idle_Bust_20 = {
        new DialogSentence("I feel so small.", "Spoken_Ah") ,    //No Voice Line
        new DialogSentence("You promised to help me grow, right?", "Spoken_Ah") ,    //No Voice Line
        new DialogSentence("Still waiting for those special benefits to kick in...", "Spoken_Ah") ,    //No Voice Line
        new DialogSentence("Can you buy me something?", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("We can always do better.", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("I can barely feel their weight yet.", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("I wonder if I’ll be as big as the other cow girls.", "Spoken_Ah") };    //No Voice Line

    public static DialogSentence[] BaristaTalk_Idle_Bust_50 = {
        new DialogSentence("Now we’re starting to make good progress.", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("This is only the beginning.", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("Now I’m starting to feel some nice weight on me.", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("Let’s keep the milk flowing!", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("This job has some great perks.", "Spoken_Hm") ,    //No Voice Line
        new DialogSentence("You said you own a farm too, right?", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("Do you have any lotion?", "Spoken_Hm") };    //No Voice Line

    public static DialogSentence[] BaristaTalk_Idle_Bust_80 = {
        new DialogSentence("This is getting out of hand, and I’m all for it!", "Spoken_Sigh_Happy") ,    //No Voice Line
        new DialogSentence("I wasn't able to milk yesterday. It was an udder failure.", "Jokes_UdderFailure"),
        new DialogSentence("I love milk when it's churned. It's butter that way.", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("More!", "Reactions_More"),
        new DialogSentence("Bigger!", "Reactions_Bigger"),
        new DialogSentence("We can always go bigger!", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("My back is extremely strong, I can carry these no problem!", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("If only I could get as meaty as I can get milky.", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("The customers really seem to like me.", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("These wonders can do more than make milk.", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("I’m one of your best girls yet?  You flatter me!~", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("I can’t wait to meet the other cow girls.", "Casual_MeetTheOtherCowGirls") };

    public static DialogSentence[] BaristaTalk_Idle_Bust_100 = {
        new DialogSentence("I get promoted when I’m too big to serve drinks, right?", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Do you get a milkshake if I jump on a pogostick?", "Jokes_MilkshakePogoStick"),
        new DialogSentence("I have to give Mewth some milk later...", "Casual_MewthMilk"),
        new DialogSentence("The customers are looking at me strangely today... do I have something on my face?", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("I'd tell you a joke about milk but it's whey too cheesy.", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("So close to greatness!~", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("I’m overflowing with power!  And milk.", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("More!  More!", "Reactions_MoreMore"),
        new DialogSentence("Please!  Just a bit bigger!~", "Reactions_Bigger"),    //No Voice Line
        new DialogSentence("Even I can’t handle much bigger.", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("I can’t wait to sell my own brand of milk~", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Bigger! Bigger!~", "Reactions_Bigger"),
        new DialogSentence("This milk flow will never end!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("The end is near.", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("First the cafe, then the world!", "Spoken_Giggles") ,    //No Voice Line
        new DialogSentence("I hope I get city size in the sequel.", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Lacti will be so jealous.", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Praise the Milk!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("I get to be milked this much every day?  Life is bliss.", "Spoken_Giggles") };    //No Voice Line

    public static DialogSentence[] BaristaTalk_Idle_Money_Above_25 = {
        new DialogSentence("We should buy something.", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("Could you buy me something?", "Interaction_CouldYouBuyMeSomething"),    //No Voice Line
        new DialogSentence("I’d really like something from the back.", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("Is there anything we can add to the shop?", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("We’re making plenty of extra cash.", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("I wonder what we should buy?", "Spoken_Hm") };    //No Voice Line

    public static DialogSentence[] BaristaTalk_Idle_Money_Above_100 = {
        new DialogSentence("Are you saving up for something?", "Money_SavingUpForSomething"),
        new DialogSentence("Is there something big you want to buy?", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("We’ve made a lot today.", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("Look at all that money.", "Spoken_Hm") ,    //No Voice Line
        new DialogSentence("We're rolling in cash today!", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("I never knew we could have this much money!", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("Our profits are through the roof!", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("I feel like we're swimming in a pool of money!", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("Looks like we hit the jackpot today!", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("Who knew serving coffee could be this lucrative?", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("We've got more money than we know what to do with!", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("I never thought we'd have this much money in our coffers!", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("I'm starting to think we're in the wrong business, with all this money!", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("Our profits are sky-high today!", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("It's like we're running a money-printing operation instead of a cafe!", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("We've got more money than we know what to do with!", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("I've never seen so much money in one place before!", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("We're practically drowning in money today!", "Spoken_Hm"),    //No Voice Line
        new DialogSentence("With all this money, we could buy the whole town!", "Spoken_Hm") ,   //No Voice Line
        new DialogSentence("Um boss, you remember you can spend money right?", "Spoken_Hm") };    //No Voice Line

    #endregion

    public static DialogSentence[] BaristaTalk_ResetCup = {
        new DialogSentence("That wasn't right...", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("I better try again!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("The next one will be better!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Oops!  Let me try again...", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("That’s not quite right...", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Is that coming out of my pay?", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Let’s try that again.", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Looks like I need more practice.", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Oops! That didn't turn out as planned.", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Practice makes perfect, right?", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("I'll get it right this time, just watch!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Let's not dwell on the past, onto the next one!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("I’ll make it up on the next cup!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("A small setback, but I'll bounce back!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Well, that didn't quite hit the mark.", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Back to the drawing board, I suppose.", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Hmm, let me recalibrate.", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Just a minor hiccup in my masterpiece-making process.", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Not quite what I had in mind, but I'll get there.", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Whoops, let's try that again, shall we?", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Well, that was unexpected. Let's try another go.", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Not every cup can be a winner, right?", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Time to channel my inner perfectionist!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("The journey to the perfect cup continues!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Let's turn this mishap into a masterpiece!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Looks like I need to fine-tune my skills.", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("A little setback won't stop me!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Onward and upward! Next cup, here I come.", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Patience and practice, that's the key!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Not quite there yet, but I'm getting closer.", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Time to redeem myself with the next cup!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("I'll make up for it with an extra shot of effort!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("The pursuit of the perfect brew continues!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("Failure is just another step towards success!", "Spoken_Ah"),    //No Voice Line
        new DialogSentence("One more time!", "Spoken_Ah") };    //No Voice Line


    public static DialogSentence[] BaristaTalk_FinishCup = {
        new DialogSentence("Be careful, it's still a bit hot!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Here! It's a little special, just for you!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Is this correct?", "Spoken_Giggles") ,    //No Voice Line
        new DialogSentence("Order up!~", "Spoken_Giggles") ,    //No Voice Line
        new DialogSentence("Enjoy your drink!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("I hope you like it.", "Spoken_Giggles") ,    //No Voice Line
        new DialogSentence("Here you go, have a great day!", "Spoken_Giggles") ,    //No Voice Line
        new DialogSentence("There you go, come back soon!~", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Voilà! Your customized creation, just for you!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("I put a little extra love into this one, enjoy!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Here's your beverage, made with care!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Your order, served with a smile!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("May this drink bring a little joy to your day!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Perfection in a cup, just for you!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Enjoy your sip of happiness!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Your drink awaits! Sip and savor!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Crafted with care, just for you!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Your order is ready, enjoy the flavor!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Here's your beverage, made to perfection!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("I hope this drink makes your day a little brighter!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Indulge in this little delight, it's all yours!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Savor the flavor! It's brewed just for you!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Here you go! Drink up and enjoy!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Your perfect cup, served with a smile!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Take a sip and let the world melt away!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("I hope this drink exceeds your expectations!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("May your day be as delightful as this drink!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Sip, savor, and enjoy! It's made just for you!", "Spoken_Giggles"),    //No Voice Line
        new DialogSentence("Here it is, should be just the way you like it.", "Spoken_Giggles") };    //No Voice Line

    public static DialogSentence[] BaristaTalk_AddMilk = {
        new DialogSentence("Aaghh! A lot of it missed the cup!", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("Hmmnnn~ ... Oh was somebody watching?", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("Oh no, my milk slipped into the drink!~", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("Barbra used Milking... it's super effective!", "Reactions_Ooooh"),    //No Voice Line
        new DialogSentence("Let’s try to get this in the cup this time.", "Spoken_Sigh_Happy") ,    //No Voice Line
        new DialogSentence("Ahh~ That feels nice~", "Reactions_mmmm") ,    //No Voice Line
        new DialogSentence("Sweet relief.", "Reactions_mmmm") ,    //No Voice Line
        new DialogSentence("Hhngg!~", "Reactions_mmmm"),
        new DialogSentence("Ooooh!~", "Reactions_Ooooh"),
        new DialogSentence("Moooo!~", "Moo_Happier"),
        new DialogSentence("M-mooo~", "Moo_Surprised"),
        new DialogSentence("Best part of the job~", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("Ah, a splash of milk just for good measure!", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("My milk magic adds an extra touch of flavor!", "Reactions_Ooooh"),    //No Voice Line
        new DialogSentence("Adding my special touch to elevate this drink!", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("A little milk, a lot of love!", "Reactions_mmmm"),    //No Voice Line
        new DialogSentence("Milk, the secret ingredient for perfection!", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("There we go, a dairy delight!", "Reactions_mmmm"),    //No Voice Line
        new DialogSentence("My milk-fu is strong today!", "Reactions_Ooooh"),    //No Voice Line
        new DialogSentence("A touch of creaminess, just how you like it!", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("Milk it is, and milk it shall be!", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("Precision milking at its finest!", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("Moo-velous! Another drop in the cup.", "Moo_Happier"),    //No Voice Line
        new DialogSentence("Watch out for the milk! It's a slippery one!", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("Just a touch of bovine bliss!", "Reactions_mmmm"),    //No Voice Line
        new DialogSentence("Udderly delightful, wouldn't you say?", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("One moo-splash of milk coming right up!", "Moo_Surprised"),    //No Voice Line
        new DialogSentence("Milk, the unsung hero of every cup!", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("Adding a creamy touch to your beverage experience!", "Reactions_Ooooh"),    //No Voice Line
        new DialogSentence("Milk makes everything better, doesn't it?", "Spoken_Sigh_Happy"),    //No Voice Line
        new DialogSentence("Squeeze ‘em dry~", "Spoken_Sigh_Happy") };    //No Voice Line

    public static DialogSentence[] BaristaTalk_PatHead = {
        new DialogSentence("What do you want?", "Interaction_WhatDoYouWant"),
        new DialogSentence("Yes silly?", "Interaction_YesSilly"),
        new DialogSentence("*giggles*", "Spoken_Giggles") ,
        new DialogSentence("Moo...?", "Moo_Curious"),
        new DialogSentence("Hm?  Do you need something?", "Interaction_DoYouNeedSomething"),
        new DialogSentence("Are you free after work?", "Interaction_FreeAfterWork"),

        new DialogSentence("Thanks for offering me this job.", "Interaction_ThanksForTheJob"),
        new DialogSentence("Are you saving up for something?", "Money_SavingUpForSomething"),
        new DialogSentence("Could you buy me something?", "Interaction_CouldYouBuyMeSomething"),
        new DialogSentence("I'll have to give Mewth some milk later...", "Casual_MewthMilk"),
        new DialogSentence("I wasn't able to milk yesterday. It was an udder failure.", "Jokes_UdderFailure"),
        new DialogSentence("I can’t wait to meet the other cow girls.", "Casual_MeetTheOtherCowGirls"),
        new DialogSentence("Do you get a milkshake if I jump on a pogostick?", "Jokes_MilkshakePogoStick"),
        new DialogSentence("I hope it doesn't rain this afternoon.", "Casual_RainThisAfterNoon"),
        new DialogSentence("Cow to the milk: 'I am your mother'.", "Jokes_CowToTheMilk"),
        new DialogSentence("This is only the beginning.", "OnlyTheBeginning"),
        new DialogSentence("My dad landed a new job at the dairy. He's the cow-ordinator.", "Jokes_DadAtTheDairy"),
        new DialogSentence("My friend is always trying to scare her other friends. She's a fan of milkshakes.", "Jokes_FriendScaringOtherFriends"),
        new DialogSentence("My friend thinks she produces almond milk. She must be nuts.", "Jokes_AlmondMilk"),
        new DialogSentence("Do we have a few more cups in the back?", "Casual_CupsInTheBack"),
        new DialogSentence("The spoiled milk always got what it wanted.", "Jokes_SpoiledMilkAlwaysGotWhatItWanted"),
        new DialogSentence("There's a lot going on today.", "Casual_LotGoingOn"),
        new DialogSentence("A man attacked me with cream, butter and milk. How dairy.", "Jokes_HowDairy"),
        new DialogSentence("I used to be a cow like you, until I took a bottle to the breast.", "Jokes_ACowLikeYou"),
        new DialogSentence("I’m feeling productive.", "Casual_FeelingProductive"),
        new DialogSentence("Can you grab me some more coffee from the back?", "Casual_CoffeeFromTheBack"),
        new DialogSentence("The coffee smells so nice.", "Casual_CoffeeSmellsNice"),
        new DialogSentence("The customers keep rolling in.", "Casual_CustomersRollingIn"),
        new DialogSentence("Do you think they like me?", "Casual_DoTheyLikeMe"),
        new DialogSentence("Just a normal day at the job.", "Casual_NormalDayAtTheJob"),

        new DialogSentence("Hey! Careful!", "Reactions_HeyCareful") };

    #endregion



    //Etc.

    public static string ButtonMaxUpgrades = "Max";

    public static string TextBestTime = "Best Time";
    public static string TextNoRecord = "No Record";
    public static string TextTime = "Time";
    public static string TextNewRecord = "New Record";

    public static string TextActualTime = "Your Time";

    public static string TextMoneyEarned = "Money Earned";
    public static string TextMostEarned = "Most Earned";

    public static string TextCustomerServed = "Customers Served";
    public static string TextMostServed = "Most Served";

    public static string TextMilkCreated = "Milk Created";
    public static string TextMostMilk = "Most Milk";

    public static string TextOverallMilkProduced = "Milk Produced: ";
    public static string TextCupsSold = "Cups Sold: ";
    public static string TextTimePlayed = "Time Played: ";
    public static string TextEarnedMoney = "Money Earned: ";


    #endregion

    public static Color MilkColor_Thick = new Color(0.9433962f, 0.8751631f, 0.8410466f);
    public static Color MilkColor_Creamy = new Color(0.9716981f, 0.884612f, 0.9268206f);
    public static Color MilkColor_Chocolate = new Color(0.4716981f, 0.1941721f, 0.06452475f);
    public static Color MilkColor_Blue = new Color(0.2971698f, 0.8429658f, 1f);
    public static Color MilkColor_Green = new Color(0.5585459f, 1f, 0.3915094f);
    public static Color MilkColor_Raspberry = new Color(1.0f, 0.375f, 0.7f);
    public static Color MilkColor_Void = Color.black;

    #region Logic

    [BurstCompile]
    public static bool RandomBool(float chanceOfSuccess = 0.5f)
    {
        return GetRandom().NextBool();
        //return GetRandom().NextFloat() < chanceOfSuccess;
    }

    [BurstCompile]
    public static T GetRandomFromArray<T>(T[] fromArray)
    {
        if (fromArray == null || fromArray.Length == 0)
        {
            Debug.LogError("Array is empty! check what you do! ");
            return default(T);
        }
        if (fromArray.Length == 1)
        {
            return fromArray[0];
        }
        int rnd = Statics.GetRandomRange(0, fromArray.Length - 1);
        return fromArray[rnd];
    }

    [BurstCompile]
    public static void Shuffle<T>(this IList<T> array)
    {
        if (array == null || array.Count <= 1)
        {
            return;
        }

        for (int i = array.Count - 1; i > 0; i--)
        {
            int j = GetRandom().NextInt(i + 1);

            T temp = array[j];
            array[j] = array[i];
            array[i] = temp;
        }
    }

    [BurstCompile]
    public static Unity.Mathematics.Random GetRandom()
    {
        if (ranSeed == 0)
        {
            ranSeed = getRandomSeed();
        }
        else if (ranSeed < 1 || ranSeed == uint.MaxValue)
        {
            ranSeed = 1;
        }
        ranSeed++;
        ranSeed = ranSeed / 7 * 11;

        return new Unity.Mathematics.Random(ranSeed);
    }

    [BurstCompile]
    private static uint getRandomSeed()
    {
        if (PlayerPrefs.HasKey(Consts.PlayerPrefRandomSeed))
        {
            return (uint)PlayerPrefs.GetInt(Consts.PlayerPrefRandomSeed);
        }
        else
        {
            return (uint)Time.realtimeSinceStartup * 7711;
        }
    }

    [BurstCompile]
    public static int GetRandomRange(int minValue, int maxValue)
    {
        if (minValue == maxValue)
        {
            return minValue;
        }
        else
        if (maxValue > minValue) //Security check that min is always smaler than max
        {
            return (GetRandom().NextInt(minValue - 1, maxValue) + 1);
        }
        else
        {
            return (GetRandom().NextInt(maxValue - 1, minValue) + 1);
        }

#pragma warning disable CS0162
        Debug.LogWarning("Code shouldnt be executed here!");
#pragma warning restore CS0162
        return 0;
    }

    [BurstCompile]
    public static uint GetRandomRange(uint minValue, uint maxValue)
    {
        if (minValue == maxValue)
        {
            return minValue;
        }

        if (maxValue >= minValue)
        {
            return GetRandom().NextUInt(minValue, maxValue);
        }
        else
        {
            return GetRandom().NextUInt(maxValue, minValue);
        }

    }

    [BurstCompile]
    public static float GetRandomRange(float minValue, float maxValue)
    {
        if (maxValue >= minValue)
        {
            return GetRandom().NextFloat(minValue, maxValue);
        }
        else
        {
            return GetRandom().NextFloat(maxValue, minValue);
        }

    }

    [BurstCompile]
    public static byte GetRandomRange(byte minValue, byte maxValue)
    {
        if (maxValue >= minValue)
        {
            return (byte)(GetRandom().NextInt(minValue - 1, maxValue) + 1);
        }
        else
        {
            return (byte)(GetRandom().NextInt(maxValue - 1, minValue) + 1);
        }
    }

    [BurstCompile]
    public static float DoubleToFloat(double value)
    {
        if (value > float.MaxValue)
        {
            return float.MaxValue;
        }
        else if (value < float.MinValue)
        {
            return float.MinValue;
        }
        else { return (float)value; }
    }


    public static void UnlockItem(string UnlockId)
    {
        PermanentUnlock[] Unlockables = GameObject.FindObjectsOfType<PermanentUnlock>();
        if (Unlockables != null && Unlockables.Count() > 0)
        {
            for (int i = 0; i < Unlockables.Count(); i++)
            {
                if (Unlockables[i].UnlockId == UnlockId)
                {
                    Unlockables[i].Unlock(true);
                    break;
                }
            }
        }

        PlayerPrefs.SetInt("unlocked_" + UnlockId, 1);
    }

    #endregion
}
