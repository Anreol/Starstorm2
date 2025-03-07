﻿using RoR2;
using RoR2.Items;
using UnityEngine;
using UnityEngine.Networking;

namespace Moonstorm.Starstorm2.Items
{
    public sealed class WatchMetronome : ItemBase
    {
        private const string token = "SS2_ITEM_WATCHMETRONOME_DESC";
        public override ItemDef ItemDef { get; } = SS2Assets.LoadAsset<ItemDef>("WatchMetronome");

        [ConfigurableField(ConfigDesc = "Amount of charges per stack.")]
        [TokenModifier(token, StatTypes.Default, 0)]
        public static int chargeAmount = 5;

        [ConfigurableField(ConfigDesc = "Maximum movement speed bonus that can be achieved via metronome. (1 = 100%)")]
        [TokenModifier(token, StatTypes.Percentage, 1)]
        public static float maxMovementSpeed = 2;

        public sealed class Behavior : BaseItemBodyBehavior
        {
            [ItemDefAssociation]
            private static ItemDef GetItemDef() => SS2Content.Items.WatchMetronome;
            //this gets halved if they are walking
            private const float buildCoefficient = 0.75f;
            private const float drainCoefficient = -1f;
            private const float notMoveBuffer = 0.6f;

            private float charges;


            public void FixedUpdate()
            {
                //What the fuck? To-do: Full rewrite this.

                //N: full rewrite my ass, this doesnt need a full rewrite you dummy.
                float metronomeCharge = Time.fixedDeltaTime;
                if (!body.isSprinting)
                {
                    metronomeCharge *= buildCoefficient;
                    
                }
                else
                {
                    metronomeCharge *= drainCoefficient;
                }
                charges = Mathf.Clamp(charges + metronomeCharge, 0, stack * chargeAmount);

                if (NetworkServer.active)
                {
                    body.SetBuffCount(SS2Content.Buffs.BuffWatchMetronome.buffIndex, Mathf.RoundToInt(charges));
                }
            }

            private void OnDestroy()
            {
                if (NetworkServer.active)
                {
                    body.SetBuffCount(SS2Content.Buffs.BuffWatchMetronome.buffIndex, 0);
                }
            }
        }
    }
}