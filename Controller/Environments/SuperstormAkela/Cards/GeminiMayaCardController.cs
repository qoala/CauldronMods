﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.SuperstormAkela
{
    public class GeminiMayaCardController : SuperstormAkelaCardController
    {

        public GeminiMayaCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowHighestHP(numberOfTargets: () => 2);
            base.SpecialStringMaker.ShowSpecialString(() => BuildCardsLeftOfThisSpecialString()).Condition = () => Card.IsInPlayAndHasGameText;

        }

        public override void AddTriggers()
        {
            //Increase all project damage dealt by 1.
            AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageType == DamageType.Projectile, 1);

            //At the end of the environment turn, this card deals the 2 targets with the highest HP X+1 lightning damage each, where X is the number of environment cards to the left of this one.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction pca)
        {
            // this card deals the 2 targets with the highest HP X+1 lightning damage each, where X is the number of environment cards to the left of this one.

            Func<Card, int?> amount = (Card c) => GetNumberOfCardsToTheLeftOfThisOne(base.Card) + 1;
            IEnumerator coroutine = DealDamageToHighestHP(base.Card, 1, (Card c) => c.IsTarget, amount, DamageType.Lightning, numberOfTargets: () => 2);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

    }
}