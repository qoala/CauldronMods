﻿using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Cauldron.MagnificentMara;

namespace CauldronTests
{
    [TestFixture()]
    public class MagnificentMaraTests : BaseTest
    {
        #region MaraHelperFunctions
        protected HeroTurnTakerController mara { get { return FindHero("MagnificentMara"); } }
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(mara.CharacterCard, 1);
            DealDamage(villain, mara, 2, DamageType.Melee);
        }

        protected DamageType DTM => DamageType.Melee;

        protected Card MDP  { get { return FindCardInPlay("MobileDefensePlatform"); } }
        #endregion maraHelperFunctions

        [Test]
        public void TestMaraLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(mara);
            Assert.IsInstanceOf(typeof(MagnificentMaraCharacterCardController), mara.CharacterCardController);

            Assert.AreEqual(27, mara.CharacterCard.HitPoints);
        }
        [Test]
        public void TestMaraPower()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            StartGame();

            PlayCard("SurgeOfStrength"); //for distinguishing damage source
            PlayCard("HeavyPlating"); //for testing it doesn't need to do the first damage

            Card mdp = MDP;

            DecisionSelectCards = new Card[] { legacy.CharacterCard, mdp, bunker.CharacterCard, mdp };
            QuickHPStorage(legacy.CharacterCard, mdp, bunker.CharacterCard);

            UsePower(mara);
            QuickHPCheck(-1, -2, 0);

            UsePower(mara);
            QuickHPCheck(0, -1, 0);
        }
        [Test]
        public void TestMaraPowerFirstDamageKills()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            StartGame();

            SetHitPoints(legacy, 1);

            QuickHPStorage(MDP);
            DecisionSelectCards = new Card[] { legacy.CharacterCard, MDP };

            UsePower(mara);

            AssertIncapacitated(legacy);
            QuickHPCheck(0);
        }
        [Test]
        public void TestAbracadabraBasic()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            StartGame();

            Card abra = PlayCard("Abracadabra");
            Card surge = PlayCard("SurgeOfStrength");
            Card plating = PutInHand("HeavyPlating");

            DecisionYesNo = true;
            DecisionSelectTurnTaker = bunker.TurnTaker;
            DecisionSelectCard = plating;

            DestroyCard(surge);

            AssertInHand(surge);
            AssertIsInPlay(plating);
        }
        [Test]
        public void TestAbracadabraCanReplaySameCard()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            StartGame();

            Card abra = PlayCard("Abracadabra");
            Card surge = PlayCard("SurgeOfStrength");
            Card plating = PlayCard("HeavyPlating");

            DecisionYesNo = true;
            DecisionSelectTurnTaker = bunker.TurnTaker;
            DecisionSelectCard = plating;

            DestroyCard(plating);

            AssertIsInPlay(plating);
        }
        [Test]
        public void TestAbracadabraOptional()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            StartGame();

            Card abra = PlayCard("Abracadabra");
            Card surge = PlayCard("SurgeOfStrength");
            Card plating = PlayCard("HeavyPlating");

            DecisionYesNo = false;
            DecisionSelectTurnTaker = bunker.TurnTaker;
            DecisionSelectCard = plating;

            DestroyCard(plating);

            AssertInTrash(plating);
            AssertIsInPlay(abra);
        }
        [Test]
        public void TestAbracadabraNoTriggerOnSelf()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            StartGame();

            Card abra = PlayCard("Abracadabra");
            Card kalpak = PlayCard("KalpakOfMysteries");

            AssertNoDecision();
            DestroyCard(kalpak);
            AssertIsInPlay(abra);
        }
        [Test]
        public void TestConvincingDoubleBasic()
        {
            //There are a million things that could go wrong with Convincing Double. 
            //For now I'm just going to make a very basic test to show that the fundamental
            //functionality is there.

            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            StartGame();
            Card thokk = PutInHand("Thokk");
            Card external = PutInHand("ExternalCombustion");
            Card transmutive = PutInHand("TransmutiveRecovery");

            //needs extra one-shots in hand so we actually make decisions
            PutInHand("BolsterAllies");
            PutInHand("KnowWhenToTurnLoose");
            PutInHand("AdhesiveFoamGrenade");

            DecisionSelectTurnTakers = new TurnTaker[] { legacy.TurnTaker, bunker.TurnTaker, scholar.TurnTaker, legacy.TurnTaker };
            DecisionSelectCards = new Card[] { thokk, GetCardInPlay("MobileDefensePlatform"), external, transmutive };
            DecisionAutoDecideIfAble = true;
            SetHitPoints(new TurnTakerController[] { legacy, bunker, scholar }, 15);
            
            QuickHandStorage(legacy, bunker, scholar);
            QuickHPStorage(legacy, bunker, scholar);
            AssertNotDamageSource(legacy.CharacterCard);

            for(int i = 0; i < 3; i++)
            {
                DecisionSelectTurnTakersIndex = i;
                PlayCard("ConvincingDouble");
                //first, Legacy hands Thokk to Bunker
                //then Bunker hands External Combustion to Scholar
                //finally Scholar gives Transmutive Recovery to Legacy
            }

            //each one lost a card, Legacy drew 2 from Recovery and Bunker drew 1 from Thokk
            QuickHandCheck(1, 0, -1);

            //Legacy gained two, Bunker gained nothing, Scholar hit himself for two
            QuickHPCheck(2, 0, -2);
        }
        [Test]
        public void TestConvincingDoubleWithSentinels()
        {
            //just to make sure it doesn't break *too* badly.

            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "TheSentinels", "TheScholar", "Megalopolis");

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card thokk = PutInHand("Thokk");
            Card transmutive = PutInHand("TransmutiveRecovery");
            Card dichotomy = PutInHand("HorrifyingDichotomy");

            //needs extra one-shots in hand so we actually make decisions
            PutInHand("BolsterAllies");
            PutInHand("KnowWhenToTurnLoose");
            PutInHand("SecondChance");

            //make sure we can distinguish who's playing what
            PlayCard("SurgeOfStrength");

            SetHitPoints(new Card[] { mainstay, writhe, medico, idealist, legacy.CharacterCard, scholar.CharacterCard }, 8);

            DecisionSelectCards = new Card[] { thokk, idealist, mdp };
            DecisionSelectTurnTakers = new TurnTaker[] { legacy.TurnTaker, sentinels.TurnTaker, scholar.TurnTaker, sentinels.TurnTaker, sentinels.TurnTaker, legacy.TurnTaker };

            QuickHPStorage(writhe, mdp, scholar.CharacterCard);
            QuickHandStorage(legacy, sentinels, scholar);

            PlayCard("ConvincingDouble");

            //Legacy hands Sentinels Thokk, they pick Idealist to take it
            QuickHPCheck(0, -3, 0);

            QuickHandCheck(-1, 1, 0);

            DecisionSelectCards = new Card[] { transmutive, writhe };
            DecisionSelectCardsIndex = 0;

            PlayCard("ConvincingDouble");

            //Scholar hands Sentinels Transmutive Recovery, they pick Writhe to take it
            QuickHandCheck(0, 2, -1);
            QuickHPCheck(2, 0, 0); 
            //this ABSOLUTELY DOES WORK GOT DANG

            DecisionSelectCards = new Card[] { dichotomy, writhe, mdp };
            DecisionSelectCardsIndex = 0;
            AssertNotDamageSource(writhe);
            AssertNotDamageSource(medico);

            PlayCard("ConvincingDouble");

            //Sentinels hand Legacy Horrifying Dichotomy, who ought to do both of the damages.
            QuickHandCheck(0, -1, 0);
            QuickHPCheck(-4, -4, 0);

            //Assert.Ignore("Pass-to-Sentinels doesn't work, not sure it's fixable. Pass-from-sentinels doesn't, and I'm not sure why.");
        }
        [Test]
        public void TestDowsingCrystalSimple()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "TheSentinels", "TheScholar", "Megalopolis");

            StartGame();

            DestroyCard("MobileDefensePlatform");
            PlayCard("DowsingCrystal");
            UsePower("DowsingCrystal");

            AssertNumberOfStatusEffectsInPlay(1);
            UsePower("DowsingCrystal");

            AssertNumberOfStatusEffectsInPlay(2);
            DecisionYesNo = true;

            QuickHPStorage(baron);
            PlayCard("BladeBattalion");

            AssertNumberOfStatusEffectsInPlay(0);
            QuickHPCheck(-4);
        }
        [Test]
        public void TestDowsingCrystalOptional()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "TheSentinels", "TheScholar", "Megalopolis");

            StartGame();

            DestroyCard("MobileDefensePlatform");
            PlayCard("DowsingCrystal");
            UsePower("DowsingCrystal");

            AssertNumberOfStatusEffectsInPlay(1);
            UsePower("DowsingCrystal");

            AssertNumberOfStatusEffectsInPlay(2);
            DecisionYesNo = false;

            QuickHPStorage(baron);

            PlayCard("BladeBattalion");

            AssertNumberOfStatusEffectsInPlay(2);
            QuickHPCheck(0);
        }
        [Test]
        public void TestGlimpse()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "TheSentinels", "TheScholar", "Megalopolis");

            StartGame();

            Card glimpse = PutInHand("GlimpseOfThingsToCome");
            Card crystal = PutInHand("DowsingCrystal");

            DecisionSelectCard = crystal;

            QuickHandStorage(mara);

            PlayCard(glimpse);
            AssertInTrash(glimpse);
            AssertIsInPlay(crystal);
            QuickHandCheck(-1);
        }
    }
}
