using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public abstract class OriphelShardkeyCardController : OriphelUtilityCardController
    {
        protected OriphelShardkeyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"At the start of the villain turn, reveal the top card of the villain deck. If a Transformation card is revealed, play it, 
            //otherwise put it on the bottom of the villain deck.",
            AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker,
                                        RevealTopCardResponse,
                                        TriggerType.RevealCard);
        }

        private IEnumerator RevealTopCardResponse(PhaseChangeAction pc)
        {
            //At the start of the villain turn, reveal the top card of the villain deck.
            var revealStorage = new List<Card> { };
            IEnumerator coroutine = GameController.RevealCards(TurnTakerController, TurnTaker.Deck, 1, revealStorage, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            Card revealedCard = revealStorage.FirstOrDefault();
            if (revealedCard != null)
            {
                //If a Transformation card is revealed, play it.
                if (IsTransformation(revealedCard))
                {
                    coroutine = GameController.PlayCard(DecisionMaker, revealedCard, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }
                }
                else
                {
                    coroutine = GameController.SendMessageAction($"{Card.Title} puts {revealedCard.Title} on the bottom of the villain deck.", Priority.Medium, GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }
                    //otherwise put it on the bottom of the villain deck.",
                    coroutine = GameController.MoveCard(DecisionMaker, revealedCard, TurnTaker.Deck, toBottom: true, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }
                }
            }
            // TODO - CleanupRevealedCards can interfere with other in progress actions that reveal cards.
            //        Should be replaced with the guys of CleanupRevealedCards - BulkMoveaction + Inhibitor
            coroutine = CleanupRevealedCards(TurnTaker.Revealed, TurnTaker.Deck);
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