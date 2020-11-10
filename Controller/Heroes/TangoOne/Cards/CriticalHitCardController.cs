﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace CauldronMods.TangoOne
{
    public class CriticalHitCardController : CardController

    {
        public static string Identifier = "CriticalHit";

        public CriticalHitCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }
    }
}
