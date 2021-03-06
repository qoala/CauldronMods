﻿using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Vanish
{
    public class FocusingGauntletCardController : CardController
    {
        public FocusingGauntletCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddIncreaseDamageTrigger(dda => dda.DamageSource.IsSameCard(CharacterCard) && dda.DamageType == DamageType.Energy, 1);
        }
    }
}