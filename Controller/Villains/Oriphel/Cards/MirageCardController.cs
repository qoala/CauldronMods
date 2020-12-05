using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public class MirageCardController : OriphelUtilityCardController
    {
        public MirageCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"Reveal the top 2 cards of the villain deck. Put any revealed targets or Transformation cards into play and discard the rest.",
            var fakeStorage = new List<Card> { };
            IEnumerator coroutine = RevealCards_PutSomeIntoPlay_DiscardRemaining(TurnTakerController, TurnTaker.Deck, 2, new LinqCardCriteria((Card c) => c.IsTarget || IsTransformation(c)), true, fakeStorage, fakeStorage, fakeStorage);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //"Each Goon deals the hero target with the highest HP 1 fire damage."
            coroutine = GameController.SelectCardsAndDoAction(DecisionMaker,
                                                    new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && IsGoon(c), "goon"),
                                                    SelectionType.CardToDealDamage,
                                                    HitHighestHP,
                                                    cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        private IEnumerator HitHighestHP(Card c)
        {
            IEnumerator coroutine = DealDamageToHighestHP(c, 1, (Card target) => target.IsHero, (target) => 1, DamageType.Fire);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}