﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.LadyOfTheWood
{
	public class ThundergreyShawlCardController : CardController
	{
		public ThundergreyShawlCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override void AddTriggers()
		{
			//Whenever LadyOfTheWood deals 2 or less damage to a target, that damage is irreducible.
			ITrigger lateIrreducibleTrigger = new Trigger<DealDamageAction>(GameController,
																(DealDamageAction dd) => dd.DamageSource.IsSameCard(base.CharacterCard) && dd.Amount <= 2,
																RetroactiveIrreducibilityResponse,
																new TriggerType[] { TriggerType.WouldBeDealtDamage, TriggerType.MakeDamageIrreducible },
																TriggerTiming.Before,
																GetCardSource(),
																orderMatters: true);
			AddTrigger(lateIrreducibleTrigger);
			//base.AddMakeDamageIrreducibleTrigger((DealDamageAction dd) => dd.DamageSource.IsSameCard(base.CharacterCard) && dd.Amount <= 2);
		}

		public override IEnumerator UsePower(int index = 0)
		{
			//LadyOfTheWood deals up to 2 targets 1 lightning damage each.
			int targets = base.GetPowerNumeral(0, 2);
			int damage = base.GetPowerNumeral(1, 1);
			IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), damage, DamageType.Lightning, new int?(targets), false, new int?(0), cardSource: base.GetCardSource());
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

		private IEnumerator RetroactiveIrreducibilityResponse(DealDamageAction dd)
        {
			IEnumerator coroutine = GameController.MakeDamageIrreducible(dd, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(coroutine);
			}
			else
			{
				GameController.ExhaustCoroutine(coroutine);
			}

			var reduceActions = dd.DamageModifiers.Where((ModifyDealDamageAction mdd) => mdd is ReduceDamageAction).ToList();

			foreach(ReduceDamageAction mod in reduceActions)
            {
				IncreaseDamageAction restoreDamage = new IncreaseDamageAction(mod.CardSource, dd, mod.AmountToReduce, false);

				//we do our best to make it have as little interaction as possible with things that respond to increasing damage
				//since it's supposed to be retroactive undoing of damage decreases
				restoreDamage.AllowTriggersToRespond = false;
				restoreDamage.CanBeCancelled = false;

				var wasUnincreasable = dd.IsUnincreasable;
				dd.IsUnincreasable = false;

				coroutine = GameController.DoAction(restoreDamage);
				if (base.UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(coroutine);
				}
				else
				{
					GameController.ExhaustCoroutine(coroutine);
				}

				dd.IsUnincreasable = wasUnincreasable;
            }
			yield break;
        }

		public override bool CanOrderAffectOutcome(GameAction action)
		{
			return action is DealDamageAction &&  (action as DealDamageAction).DamageSource.Card == base.CharacterCard;
		}
	}
}
