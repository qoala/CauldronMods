﻿using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Vector
{
    public class VectorBaseCharacterCardController : VillainCharacterCardController
    {
        protected VectorBaseCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        protected bool IsVirus(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "virus");
        }

        protected bool IsVehicle(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "vehicle");
        }

        protected bool IsPawn(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "pawn");
        }

        protected bool IsSuperVirusInPlay()
        {
            return base.GameController.IsCardInPlayAndNotUnderCard(SupervirusCardController.Identifier);
        }

        protected Card GetSuperVirusCard()
        {
            return FindCardsWhere(card => card.Identifier == SupervirusCardController.Identifier
                                          && card.IsInPlayAndHasGameText).FirstOrDefault();
        }

        protected Card GetSuperVirusCardNotInPlay()
        {
            return FindCardsWhere(card => card.Identifier == SupervirusCardController.Identifier
                                          && !card.IsInPlayAndHasGameText).FirstOrDefault();
        }
    }
}
