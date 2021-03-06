﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Malichae
{
    public class HighBathielCardController : DjinnOngoingController
    {
        public HighBathielCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController, "Bathiel", "Bathiel")
        {
        }

        public override void AddTriggers()
        {
            base.AddIncreaseDamageTrigger(dda => dda.DamageSource.Card == GetCardThisCardIsNextTo(), 1);
            base.AddTriggers();
        }

        public override Power GetGrantedPower(CardController cardController)
        {
            return new Power(cardController.HeroTurnTakerController, this, $"{cardController.Card.Title} deals 1 target 4 energy damage. Destroy {this.Card.Title}.", UseGrantedPower(), 0, null, cardController.GetCardSource());
        }

        private IEnumerator UseGrantedPower()
        {
            int targets = GetPowerNumeral(0, 1);
            int damages = GetPowerNumeral(1, 4);

            var usePowerAction = ActionSources.OfType<UsePowerAction>().First();
            var cs = usePowerAction.CardSource ?? usePowerAction.Power.CardSource;

            var coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, cs.Card), damages, DamageType.Energy, targets, false, targets, cardSource: cs);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //when played via discard, I'll already be in the trash, so skip
            if (!Card.IsInTrash)
            {
                coroutine = DestroySelf();
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }
    }
}
