using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Starlight
{
    public class StarlightOfAsheronCharacterCardController : StarlightSubCharacterCardController
    {
        public StarlightOfAsheronCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {

            int targets = GetPowerNumeral(0, 1);
            int amount = GetPowerNumeral(1, 1);
            List<Card> storedResults = new List<Card> { };
            IEnumerator pickStarlight = SelectActiveCharacterCardToDealDamage(storedResults, amount, DamageType.Radiant);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(pickStarlight);
            }
            else
            {
                GameController.ExhaustCoroutine(pickStarlight);
            }
            Card chosenStarlight = storedResults.FirstOrDefault();

            //Play a constellation.
            IEnumerator playRoutine = SelectAndPlayCardFromHand(HeroTurnTakerController, false, null, new LinqCardCriteria((Card c) => IsConstellation(c)));
            //One Starlight deals 1 target 1 radiant damage.
            IEnumerator damageRoutine = GameController.SelectTargetsAndDealDamage(HeroTurnTakerController,
                                                                        new DamageSource(GameController, chosenStarlight),
                                                                        amount,
                                                                        DamageType.Radiant,
                                                                        targets,
                                                                        false,
                                                                        targets,
                                                                        cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(playRoutine);
                yield return GameController.StartCoroutine(damageRoutine);
            }
            else
            {
                GameController.ExhaustCoroutine(playRoutine);
                GameController.ExhaustCoroutine(damageRoutine);
            }
            yield break;
        }

    }
}