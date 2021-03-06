﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.TheMistressOfFate
{
    public class TheMistressOfFateTurnTakerController : TurnTakerController
    {
        public TheMistressOfFateTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {

        }

        public override IEnumerator StartGame()
        {
            yield break;
        }
    }
}