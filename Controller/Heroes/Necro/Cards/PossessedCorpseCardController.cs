﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

namespace Cauldron.Necro
{
    public class PossessedCorpseCardController : UndeadCardController
    {
        public PossessedCorpseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, 2)
        {
        }

        public override void AddTriggers()
        {
            //At the end of your turn, this card deals the non-undead target with the second lowest HP 2 toxic damage.
            base.AddEndOfTurnTrigger(tt => tt == TurnTaker, p => base.DealDamageToLowestHP(Card, 1, c => !this.IsUndead(c) && IsHeroConsidering1929(c), c => 2, DamageType.Toxic), TriggerType.DealDamage);
        }
    }
}
