﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.Anathema
{
	public class RazorScalesCardController : BodyCardController
    {
		public RazorScalesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			//The first time a Villain target is dealt damage each turn, this card deals the source of that damage 2 melee damage.
			base.AddTrigger<DealDamageAction>((DealDamageAction dd) => !base.IsPropertyTrue("FirstDamageToVillainTargetThisTurn", null) && dd.DidDealDamage && dd.DamageSource.IsTarget && base.IsVillainTarget(dd.Target), new Func<DealDamageAction, IEnumerator>(this.FirstDamageDealtResponse), TriggerType.DealDamage, TriggerTiming.After, ActionDescription.DamageTaken, false, true, null, false, null, null, false, false);
		}

		private IEnumerator FirstDamageDealtResponse(DealDamageAction dd)
		{
			//this card deals the source of that damage 2 melee damage.
			base.SetCardPropertyToTrueIfRealAction(FirstDamageToVillainTargetThisTurn);
			IEnumerator coroutine = base.DealDamage(base.Card, dd.DamageSource.Card, 2, DamageType.Melee, false, false, true, null, null, null, false, null);
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

		private const string FirstDamageToVillainTargetThisTurn = "FirstDamageToVillainTargetThisTurn";

	}
}
