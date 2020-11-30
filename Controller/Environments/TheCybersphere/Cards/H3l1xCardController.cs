﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.TheCybersphere
{
    public class H3l1xCardController : TheCybersphereCardController
    {

        public H3l1xCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //At the start of the environment turn, this card deals the hero target with the lowest HP 1 melee damage, the hero target with the second highest HP 3 melee damage, and the hero target with the highest HP 5 melee damage.
            AddDealDamageAtStartOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => c.IsHero && c.IsTarget, TargetType.LowestHP, 1, DamageType.Melee);
            AddDealDamageAtStartOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => c.IsHero && c.IsTarget, TargetType.HighestHP, 3, DamageType.Melee, highestLowestRanking: 2);
            AddDealDamageAtStartOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => c.IsHero && c.IsTarget, TargetType.HighestHP, 5, DamageType.Melee);

        }


    }
}