using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;

namespace Snap
{
    public class SnapManager : ThunderScript
    {
        [ModOption(name: "Left Target Distance", tooltip: "Targets the closest/farthest entity or randomizes the distance", defaultValueIndex = 0, order = 0, category = "Left Hand")]
        public static TargetDistance distanceLeft;
        [ModOption(name: "Left Target Type", tooltip: "Targets enemies/allies/players/all NPCs or Items", defaultValueIndex = 0, order = 1, category = "Left Hand")]
        public static TargetType typeLeft;
        [ModOption(name: "Left Target All of Type", tooltip: "Targets all of the selected type", valueSourceName: nameof(booleanOption), defaultValueIndex = 1, order = 2, category = "Left Hand")]
        public static bool targetAllLeft = false;
        [ModOption(name: "Left NPC Action", tooltip: "The action that's performed on a selected NPC", defaultValueIndex = 0, order = 3, category = "Left Hand")]
        public static NPCAction creatureActionLeft;
        [ModOption(name: "Left Item Action", tooltip: "The action that's performed on a selected Item", defaultValueIndex = 0, order = 4, category = "Left Hand")]
        public static ItemAction itemActionLeft;
        [ModOption(name: "Left Player Action", tooltip: "The action that's performed on the player", defaultValueIndex = 0, order = 5, category = "Left Hand")]
        public static PlayerAction playerActionLeft;
        [ModOption(name: "Left Spawn Item", tooltip: "The Item that's spawned with the SpawnItem action", valueSourceName: nameof(Items), defaultValueIndex = 0, order = 6, category = "Left Hand")]
        public static ItemData itemDataLeft;
        [ModOption(name: "Left Spawn NPC", tooltip: "The NPC that's spawned with the SpawnNPC action", valueSourceName: nameof(Creatures), defaultValueIndex = 0, order = 7, category = "Left Hand")]
        public static ContainerData creatureContainerLeft;
        [ModOption(name: "Left Spawn NPC Faction", tooltip: "The faction of the NPC that's spawned with the SpawnNPC action", valueSourceName: nameof(Factions), defaultValueIndex = 0, order = 8, category = "Left Hand")]
        public static int creatureFactionLeft;
        [ModOptionSlider]
        [ModOption(name: "Left Teleport Distance", tooltip: "How far you teleport, in meters", valueSourceName: nameof(TeleportDistance), defaultValueIndex = 5, order = 9, category = "Left Hand")]
        public static int teleportDistanceLeft;

        [ModOption(name: "Right Target Distance", tooltip: "Targets the closest/farthest entity or randomizes the distance", defaultValueIndex = 0, order = 0, category = "Right Hand")]
        public static TargetDistance distanceRight;
        [ModOption(name: "Right Target Type", tooltip: "Targets enemies/allies/players/all NPCs or Items", defaultValueIndex = 0, order = 1, category = "Right Hand")]
        public static TargetType typeRight;
        [ModOption(name: "Right Target All of Type", tooltip: "Targets all of the selected type", valueSourceName: nameof(booleanOption), defaultValueIndex = 1, order = 2, category = "Right Hand")]
        public static bool targetAllRight = false;
        [ModOption(name: "Right NPC Action", tooltip: "The action that's performed on a selected NPC", defaultValueIndex = 0, order = 3, category = "Right Hand")]
        public static NPCAction creatureActionRight;
        [ModOption(name: "Right Item Action", tooltip: "The action that's performed on a selected Item", defaultValueIndex = 0, order = 4, category = "Right Hand")]
        public static ItemAction itemActionRight;
        [ModOption(name: "Right Player Action", tooltip: "The action that's performed on the player", defaultValueIndex = 0, order = 5, category = "Right Hand")]
        public static PlayerAction playerActionRight;
        [ModOption(name: "Right Spawn Item", tooltip: "The Item that's spawned with the SpawnItem action", valueSourceName: nameof(Items), defaultValueIndex = 0, order = 6, category = "Right Hand")]
        public static ItemData itemDataRight;
        [ModOption(name: "Right Spawn NPC", tooltip: "The NPC that's spawned with the SpawnNPC action", valueSourceName: nameof(Creatures), defaultValueIndex = 0, order = 7, category = "Right Hand")]
        public static ContainerData creatureContainerRight;
        [ModOption(name: "Right Spawn NPC Faction", tooltip: "The faction of the NPC that's spawned with the SpawnNPC action", valueSourceName: nameof(Factions), defaultValueIndex = 0, order = 8, category = "Right Hand")]
        public static int creatureFactionRight;
        [ModOptionSlider]
        [ModOption(name: "Right Teleport Distance", tooltip: "How far you teleport, in meters", valueSourceName: nameof(TeleportDistance), defaultValueIndex = 5, order = 9, category = "Right Hand")]
        public static int teleportDistanceRight;
        public static SnapManager instance;
        public static ModOptionBool[] booleanOption =
        {
            new ModOptionBool("Enabled", true),
            new ModOptionBool("Disabled", false)
        };
        public static ModOptionInt[] TeleportDistance()
        {
            ModOptionInt[] modOptionInts = new ModOptionInt[101];
            int num = 0;
            for (int i = 0; i < modOptionInts.Length; ++i)
            {
                modOptionInts[i] = new ModOptionInt(num.ToString("0"), num);
                num += 1;
            }
            return modOptionInts;
        }
        public static ModOptionCatalogData[] Items()
        {
            List<ModOptionCatalogData> options = new List<ModOptionCatalogData>();
            foreach (CatalogData catalogData in Catalog.GetDataList(Category.Item))
            {
                if (catalogData is ItemData itemData && itemData.purchasable)
                {
                    options.Add(new ModOptionCatalogData(itemData.displayName, itemData));
                }
            }
            return options.ToArray();
        }
        public static ModOptionCatalogData[] Creatures()
        {
            List<ModOptionCatalogData> options = new List<ModOptionCatalogData>();
            foreach (CatalogData catalogData in Catalog.GetDataList(Category.Container))
            {
                if (catalogData is ContainerData containerData)
                {
                    if (containerData.id.Contains("Player") || containerData.id.Contains("Prop") || containerData.id.Contains("Test") || containerData.id.Contains("Torches") || containerData.id.Contains("Weapon")) continue;
                    options.Add(new ModOptionCatalogData(containerData.id, containerData));
                }
            }
            return options.ToArray();
        }
        public static ModOptionInt[] Factions()
        {
            List<ModOptionInt> options = new List<ModOptionInt>();
            foreach (GameData.Faction faction in Catalog.gameData.factions)
            {
                options.Add(new ModOptionInt(faction.name, faction.id));
            }
            return options.ToArray();
        }
        public enum TargetDistance
        {
            Closest,
            Farthest,
            Random
        }
        public enum TargetType
        {
            Enemies,
            Allies,
            AllNPCs,
            Player,
            Items
        }
        public enum NPCAction
        {
            Kill,
            Damage,
            Stagger,
            Destabilize,
            Explode,
            Ignite,
            Freeze,
            Electrocute,
            Smite,
            Slice,
            Decapitate,
            Brainwash,
            Heal,
            Resurrect,
            Clone,
            Polymorph,
            Levitate,
            Crush,
            StopLevitateOrCrush,
            Despawn
        }
        public enum ItemAction
        {
            Duplicate,
            Explode,
            Break,
            Levitate,
            StopLevitate,
            Despawn
        }
        public enum PlayerAction
        {
            Heal,
            Shockwave,
            SpawnItem,
            SpawnNPC,
            AntiMagic,
            ToggleLevitate,
            Invisible,
            Teleport
        }
        public override void ScriptEnable()
        {
            base.ScriptEnable();
            EventManager.onCreatureKill += EventManager_onCreatureKill;
            instance = this;
        }

        private void EventManager_onCreatureKill(Creature creature, Player player, CollisionInstance collisionInstance, EventTime eventTime)
        {
            creature?.ragdoll?.RemovePhysicModifier(instance);
        }
    }
    public class SnapSpell : SpellCastCharge
    {
        bool active = false;
        static bool levitating = false;
        EffectData explosionData;
        EffectData burnData;
        EffectData snapData;
        EffectData electrocuteData;
        EffectData shockwaveData;
        EffectData largeBoltData;
        List<ItemData> itemData = new List<ItemData>();
        public override void Load(SpellCaster spellCaster, Level level)
        {
            base.Load(spellCaster, level);
            explosionData = (EffectData)Catalog.GetData<EffectData>("MeteorExplosion").Clone();
            burnData = (EffectData)Catalog.GetData<EffectData>("Ignite").Clone();
            snapData = (EffectData)Catalog.GetData<EffectData>("SnapEffect").Clone();
            electrocuteData = (EffectData)Catalog.GetData<EffectData>("ImbueLightningRagdoll").Clone();
            shockwaveData = (EffectData)Catalog.GetData<EffectData>("SpellGravityShockwave").Clone();
            largeBoltData = Catalog.GetData<EffectData>("SmiteBolt");
            foreach (ItemData data in Catalog.GetDataList<ItemData>())
            {
                if (data.category == "Breakable" && !itemData.Contains(data)) itemData.Add(data);
            }
        }
        public override void FireAxis(float value)
        {
            base.FireAxis(value);
            if (value >= 1f && !active)
            {
                active = true;
                currentCharge = 1;
                Snap();
            }
            else if (value < 0.8f)
            {
                active = false;
                currentCharge = 0;
            }
        }
        public void Snap()
        {
            EffectInstance instance = snapData.Spawn(spellCaster.magic);
            instance.SetIntensity(1f);
            instance.Play();
            if (spellCaster.ragdollHand.side == Side.Left)
            {
                switch (SnapManager.typeLeft)
                {
                    case SnapManager.TargetType.Player:
                        PerformPlayerActionLeft();
                        break;
                    case SnapManager.TargetType.Items:
                        PerformItemActionLeft();
                        break;
                    default:
                        PerformNPCActionLeft();
                        break;
                }
            }
            else
            {
                switch (SnapManager.typeRight)
                {
                    case SnapManager.TargetType.Player:
                        PerformPlayerActionRight();
                        break;
                    case SnapManager.TargetType.Items:
                        PerformItemActionRight();
                        break;
                    default:
                        PerformNPCActionRight();
                        break;
                }
            }
        }
        public void PerformNPCActionLeft()
        {
            if (SnapManager.targetAllLeft)
            {
                if (SnapManager.creatureActionLeft != SnapManager.NPCAction.Clone)
                {
                    List<Creature> creatures = new List<Creature>();
                    foreach (Creature creature in Creature.allActive)
                    {
                        if (creature.loaded && !creature.isPlayer && !creatures.Contains(creature) && 
                            ((creature.IsEnemy(spellCaster.mana.creature) && SnapManager.typeLeft == SnapManager.TargetType.Enemies) ||
                            (!creature.IsEnemy(spellCaster.mana.creature) && SnapManager.typeLeft == SnapManager.TargetType.Allies) ||
                            SnapManager.typeLeft == SnapManager.TargetType.AllNPCs) &&
                            ((!creature.isKilled && SnapManager.creatureActionLeft != SnapManager.NPCAction.Resurrect) || (creature.isKilled && SnapManager.creatureActionLeft == SnapManager.NPCAction.Resurrect)))
                            creatures.Add(creature);
                    }
                    for (int i = creatures.Count -1; i >= 0; --i)
                    {
                        GameManager.local.StartCoroutine(ContinueNPCActionLeft(creatures[i]));
                    }
                }
            }
            else
            {
                switch (SnapManager.distanceLeft)
                {
                    case SnapManager.TargetDistance.Random:
                        List<Creature> creatures = new List<Creature>();
                        foreach (Creature creature in Creature.allActive)
                        {
                            if (creature.loaded && !creature.isPlayer && !creatures.Contains(creature) &&
                                ((creature.IsEnemy(spellCaster.mana.creature) && SnapManager.typeLeft == SnapManager.TargetType.Enemies) ||
                                (!creature.IsEnemy(spellCaster.mana.creature) && SnapManager.typeLeft == SnapManager.TargetType.Allies) ||
                                SnapManager.typeLeft == SnapManager.TargetType.AllNPCs) &&
                                ((!creature.isKilled && SnapManager.creatureActionLeft != SnapManager.NPCAction.Resurrect) || (creature.isKilled && SnapManager.creatureActionLeft == SnapManager.NPCAction.Resurrect)))
                                creatures.Add(creature);
                        }
                        int index = UnityEngine.Random.Range(0, creatures.Count);
                        if (creatures.Count > 0)
                        {
                            GameManager.local.StartCoroutine(ContinueNPCActionLeft(creatures[index]));
                        }
                        break;
                    default:
                        GameManager.local.StartCoroutine(ContinueNPCActionLeft(CalculateNPCDistanceLeft()));
                        break;
                }
            }
        }
        public void PerformNPCActionRight()
        {
            if (SnapManager.targetAllRight)
            {
                if (SnapManager.creatureActionRight != SnapManager.NPCAction.Clone)
                {
                    List<Creature> creatures = new List<Creature>();
                    foreach (Creature creature in Creature.allActive)
                    {
                        if (creature.loaded && !creature.isPlayer && !creatures.Contains(creature) &&
                            ((creature.IsEnemy(spellCaster.mana.creature) && SnapManager.typeRight == SnapManager.TargetType.Enemies) ||
                            (!creature.IsEnemy(spellCaster.mana.creature) && SnapManager.typeRight == SnapManager.TargetType.Allies) ||
                            SnapManager.typeRight == SnapManager.TargetType.AllNPCs) &&
                            ((!creature.isKilled && SnapManager.creatureActionRight != SnapManager.NPCAction.Resurrect) || (creature.isKilled && SnapManager.creatureActionRight == SnapManager.NPCAction.Resurrect)))
                            creatures.Add(creature);
                    }
                    for (int i = creatures.Count - 1; i >= 0; --i)
                    {
                        GameManager.local.StartCoroutine(ContinueNPCActionRight(creatures[i]));
                    }
                }
            }
            else
            {
                switch (SnapManager.distanceRight)
                {
                    case SnapManager.TargetDistance.Random:
                        List<Creature> creatures = new List<Creature>();
                        foreach (Creature creature in Creature.allActive)
                        {
                            if (creature.loaded && !creature.isPlayer && !creatures.Contains(creature) &&
                                ((creature.IsEnemy(spellCaster.mana.creature) && SnapManager.typeRight == SnapManager.TargetType.Enemies) ||
                                (!creature.IsEnemy(spellCaster.mana.creature) && SnapManager.typeRight == SnapManager.TargetType.Allies) ||
                                SnapManager.typeRight == SnapManager.TargetType.AllNPCs) &&
                                ((!creature.isKilled && SnapManager.creatureActionRight != SnapManager.NPCAction.Resurrect) || (creature.isKilled && SnapManager.creatureActionRight == SnapManager.NPCAction.Resurrect)))
                                creatures.Add(creature);
                        }
                        int index = UnityEngine.Random.Range(0, creatures.Count);
                        if (creatures.Count > 0)
                        {
                            GameManager.local.StartCoroutine(ContinueNPCActionRight(creatures[index]));
                        }
                        break;
                    default:
                        GameManager.local.StartCoroutine(ContinueNPCActionRight(CalculateNPCDistanceRight()));
                        break;
                }
            }
        }
        public Creature CalculateNPCDistanceLeft()
        {
            Creature creature = null;
            foreach (Creature npc in Creature.allActive)
            {
                if (npc.loaded && !npc.isPlayer && 
                    ((npc.IsEnemy(spellCaster.mana.creature) && SnapManager.typeLeft == SnapManager.TargetType.Enemies) ||
                    (!npc.IsEnemy(spellCaster.mana.creature) && SnapManager.typeLeft == SnapManager.TargetType.Allies) ||
                    SnapManager.typeLeft == SnapManager.TargetType.AllNPCs) &&
                    ((!npc.isKilled && SnapManager.creatureActionLeft != SnapManager.NPCAction.Resurrect) || (npc.isKilled && SnapManager.creatureActionLeft == SnapManager.NPCAction.Resurrect)) &&
                    ((SnapManager.distanceLeft == SnapManager.TargetDistance.Closest && (creature == null || Vector3.Distance(npc.transform.position, spellCaster.magic.position) < Vector3.Distance(creature.transform.position, spellCaster.magic.position))) ||
                    (SnapManager.distanceLeft == SnapManager.TargetDistance.Farthest && (creature == null || Vector3.Distance(npc.transform.position, spellCaster.magic.position) > Vector3.Distance(creature.transform.position, spellCaster.magic.position)))))
                    creature = npc;
            }
            return creature;
        }
        public Creature CalculateNPCDistanceRight()
        {
            Creature creature = null;
            foreach (Creature npc in Creature.allActive)
            {
                if (npc.loaded && !npc.isPlayer &&
                    ((npc.IsEnemy(spellCaster.mana.creature) && SnapManager.typeRight == SnapManager.TargetType.Enemies) ||
                    (!npc.IsEnemy(spellCaster.mana.creature) && SnapManager.typeRight == SnapManager.TargetType.Allies) ||
                    SnapManager.typeRight == SnapManager.TargetType.AllNPCs) &&
                    ((!npc.isKilled && SnapManager.creatureActionRight != SnapManager.NPCAction.Resurrect) || (npc.isKilled && SnapManager.creatureActionRight == SnapManager.NPCAction.Resurrect)) &&
                    ((SnapManager.distanceRight == SnapManager.TargetDistance.Closest && (creature == null || Vector3.Distance(npc.transform.position, spellCaster.magic.position) < Vector3.Distance(creature.transform.position, spellCaster.magic.position))) ||
                    (SnapManager.distanceRight == SnapManager.TargetDistance.Farthest && (creature == null || Vector3.Distance(npc.transform.position, spellCaster.magic.position) > Vector3.Distance(creature.transform.position, spellCaster.magic.position)))))
                    creature = npc;
            }
            return creature;
        }
        public IEnumerator ContinueNPCActionLeft(Creature creature)
        {
            if (creature != null)
                switch (SnapManager.creatureActionLeft)
                {
                    case SnapManager.NPCAction.Kill:
                        creature.Kill();
                        break;
                    case SnapManager.NPCAction.Damage:
                        creature.Damage(new CollisionInstance(new DamageStruct(DamageType.Unknown, 10)));
                        break;
                    case SnapManager.NPCAction.Stagger:
                        creature.TryPush(Creature.PushType.Grab, creature.transform.position - spellCaster.magic.position, 0);
                        break;
                    case SnapManager.NPCAction.Destabilize:
                        creature.ragdoll.SetState(Ragdoll.State.Destabilized);
                        break;
                    case SnapManager.NPCAction.Explode:
                        Explode(creature.ragdoll.targetPart.transform.position);
                        break;
                    case SnapManager.NPCAction.Ignite:
                        GameManager.local.StartCoroutine(Ignite(creature));
                        break;
                    case SnapManager.NPCAction.Freeze:
                        creature.ragdoll.SetState(Ragdoll.State.Frozen);
                        break;
                    case SnapManager.NPCAction.Electrocute:
                        creature.TryElectrocute(10, 5, true, false, electrocuteData);
                        break;
                    case SnapManager.NPCAction.Smite:
                        Smite(creature);
                        break;
                    case SnapManager.NPCAction.Slice:
                        foreach (RagdollPart part in creature.ragdoll.parts)
                        {
                            if (part.sliceAllowed) creature.ragdoll.TrySlice(part);
                            if (part.data.sliceForceKill) creature.Kill();
                            yield return null;
                        }
                        break;
                    case SnapManager.NPCAction.Decapitate:
                        creature.ragdoll.TrySlice(creature.ragdoll.GetPart(RagdollPart.Type.Neck));
                        if (creature.ragdoll.GetPart(RagdollPart.Type.Neck).data.sliceForceKill) creature.Kill();
                        break;
                    case SnapManager.NPCAction.Brainwash:
                        creature.SetFaction(spellCaster.mana.creature.factionId);
                        break;
                    case SnapManager.NPCAction.Heal:
                        creature.Heal(creature.maxHealth - creature.currentHealth, spellCaster.mana.creature);
                        break;
                    case SnapManager.NPCAction.Resurrect:
                        creature.brain.Load(creature.data.brainId);
                        creature.Resurrect(creature.maxHealth, spellCaster.mana.creature);
                        creature.ragdoll.SetState(Ragdoll.State.Destabilized);
                        break;
                    case SnapManager.NPCAction.Clone:
                        creature.data.SpawnAsync(spellCaster.mana.creature.transform.position + new Vector3(spellCaster.mana.creature.ragdoll.headPart.transform.forward.x, 0, spellCaster.mana.creature.ragdoll.headPart.transform.forward.z) * 2, spellCaster.mana.creature.ragdoll.headPart.transform.rotation.eulerAngles.y);
                        break;
                    case SnapManager.NPCAction.Polymorph:
                        if (!itemData.IsNullOrEmpty())
                            itemData[UnityEngine.Random.Range(0, itemData.Count)].SpawnAsync(item =>
                            {
                                creature.Kill();
                                creature.Despawn();
                            }, creature.ragdoll.targetPart.transform.position);
                        break;
                    case SnapManager.NPCAction.Levitate:
                        creature.ragdoll.SetState(Ragdoll.State.Destabilized);
                        creature.ragdoll.SetPhysicModifier(SnapManager.instance, 0);
                        creature.brain.AddNoStandUpModifier(SnapManager.instance);
                        break;
                    case SnapManager.NPCAction.Crush:
                        creature.ragdoll.SetState(Ragdoll.State.Destabilized);
                        creature.ragdoll.SetPhysicModifier(SnapManager.instance, 10);
                        creature.brain.AddNoStandUpModifier(SnapManager.instance);
                        break;
                    case SnapManager.NPCAction.StopLevitateOrCrush:
                        creature.ragdoll.RemovePhysicModifier(SnapManager.instance);
                        creature.brain.RemoveNoStandUpModifier(SnapManager.instance);
                        break;
                    case SnapManager.NPCAction.Despawn:
                        creature.Despawn();
                        break;
                }
            yield break;
        }
        public IEnumerator ContinueNPCActionRight(Creature creature)
        {
            if (creature != null)
                switch (SnapManager.creatureActionRight)
                {
                    case SnapManager.NPCAction.Kill:
                        creature.Kill();
                        break;
                    case SnapManager.NPCAction.Damage:
                        creature.Damage(new CollisionInstance(new DamageStruct(DamageType.Unknown, 10)));
                        break;
                    case SnapManager.NPCAction.Stagger:
                        creature.TryPush(Creature.PushType.Grab, creature.transform.position - spellCaster.magic.position, 0);
                        break;
                    case SnapManager.NPCAction.Destabilize:
                        creature.ragdoll.SetState(Ragdoll.State.Destabilized);
                        break;
                    case SnapManager.NPCAction.Explode:
                        Explode(creature.ragdoll.targetPart.transform.position);
                        break;
                    case SnapManager.NPCAction.Ignite:
                        GameManager.local.StartCoroutine(Ignite(creature));
                        break;
                    case SnapManager.NPCAction.Freeze:
                        creature.ragdoll.SetState(Ragdoll.State.Frozen);
                        break;
                    case SnapManager.NPCAction.Electrocute:
                        creature.TryElectrocute(10, 5, true, false, electrocuteData);
                        break;
                    case SnapManager.NPCAction.Smite:
                        Smite(creature);
                        break;
                    case SnapManager.NPCAction.Slice:
                        foreach (RagdollPart part in creature.ragdoll.parts)
                        {
                            if (part.sliceAllowed) creature.ragdoll.TrySlice(part);
                            if (part.data.sliceForceKill) creature.Kill();
                            yield return null;
                        }
                        break;
                    case SnapManager.NPCAction.Decapitate:
                        creature.ragdoll.TrySlice(creature.ragdoll.GetPart(RagdollPart.Type.Neck));
                        if (creature.ragdoll.GetPart(RagdollPart.Type.Neck).data.sliceForceKill) creature.Kill();
                        break;
                    case SnapManager.NPCAction.Brainwash:
                        creature.SetFaction(spellCaster.mana.creature.factionId);
                        break;
                    case SnapManager.NPCAction.Heal:
                        creature.Heal(creature.maxHealth - creature.currentHealth, spellCaster.mana.creature);
                        break;
                    case SnapManager.NPCAction.Resurrect:
                        creature.brain.Load(creature.data.brainId);
                        creature.Resurrect(creature.maxHealth, spellCaster.mana.creature);
                        creature.ragdoll.SetState(Ragdoll.State.Destabilized);
                        break;
                    case SnapManager.NPCAction.Clone:
                        creature.data.SpawnAsync(spellCaster.mana.creature.transform.position + new Vector3(spellCaster.mana.creature.ragdoll.headPart.transform.forward.x, 0, spellCaster.mana.creature.ragdoll.headPart.transform.forward.z) * 2, spellCaster.mana.creature.ragdoll.headPart.transform.rotation.eulerAngles.y);
                        break;
                    case SnapManager.NPCAction.Polymorph:
                        if (!itemData.IsNullOrEmpty())
                            itemData[UnityEngine.Random.Range(0, itemData.Count)].SpawnAsync(item =>
                            {
                                creature.Kill();
                                creature.Despawn();
                            }, creature.ragdoll.targetPart.transform.position);
                        break;
                    case SnapManager.NPCAction.Levitate:
                        creature.ragdoll.SetState(Ragdoll.State.Destabilized);
                        creature.ragdoll.SetPhysicModifier(SnapManager.instance, 0);
                        creature.brain.AddNoStandUpModifier(SnapManager.instance);
                        break;
                    case SnapManager.NPCAction.Crush:
                        creature.ragdoll.SetState(Ragdoll.State.Destabilized);
                        creature.ragdoll.SetPhysicModifier(SnapManager.instance, 10);
                        creature.brain.AddNoStandUpModifier(SnapManager.instance);
                        break;
                    case SnapManager.NPCAction.StopLevitateOrCrush:
                        creature.ragdoll.RemovePhysicModifier(SnapManager.instance);
                        creature.brain.RemoveNoStandUpModifier(SnapManager.instance);
                        break;
                    case SnapManager.NPCAction.Despawn:
                        creature.Despawn();
                        break;
                }
            yield break;
        }

        public void PerformPlayerActionLeft()
        {
            switch (SnapManager.playerActionLeft)
            {
                case SnapManager.PlayerAction.Heal:
                    spellCaster.mana.creature.Heal(spellCaster.mana.creature.maxHealth - spellCaster.mana.creature.currentHealth, spellCaster.mana.creature);
                    break;
                case SnapManager.PlayerAction.Shockwave:
                    Shockwave(spellCaster.mana.creature.transform.position);
                    break;
                case SnapManager.PlayerAction.SpawnItem:
                    SnapManager.itemDataLeft.SpawnAsync(item =>
                    {
                        if (spellCaster.ragdollHand.grabbedHandle == null && item?.GetMainHandle(spellCaster.ragdollHand.side) != null)
                            spellCaster.ragdollHand.Grab(item.GetMainHandle(spellCaster.ragdollHand.side));
                    }, spellCaster.magic.position, spellCaster.magic.rotation);
                    break;
                case SnapManager.PlayerAction.SpawnNPC:
                    CreatureTable creatureTableData = Catalog.GetData<CreatureTable>("Humans");
                    if (creatureTableData == null || !creatureTableData.TryPick(out CreatureData creatureData))
                        return;
                    CreatureData data = (CreatureData)creatureData.Clone();
                    data.containerID = SnapManager.creatureContainerLeft.id;
                    data.factionId = SnapManager.creatureFactionLeft;
                    data.SpawnAsync(spellCaster.mana.creature.transform.position + new Vector3(spellCaster.mana.creature.ragdoll.headPart.transform.forward.x, 0, spellCaster.mana.creature.ragdoll.headPart.transform.forward.z) * 2, spellCaster.mana.creature.ragdoll.headPart.transform.rotation.eulerAngles.y);
                    break;
                case SnapManager.PlayerAction.AntiMagic:
                    for (int index = Item.allActive.Count - 1; index >= 0; --index)
                    {
                        Item obj = Item.allActive[index];
                        ItemData itemData;
                        if (!obj.holder && !obj.disallowDespawn && obj.TryGetData(out itemData) && (itemData.HasModule<ItemModuleMagicProjectile>() || itemData.HasModule<ItemModuleAreaProjectile>()) && !obj.worldAttached)
                            obj.Despawn();
                    }
                    foreach (Creature creature in Creature.allActive)
                    {
                        if (!creature.isPlayer) creature.mana.currentMana = 0;
                    }
                    break;
                case SnapManager.PlayerAction.ToggleLevitate:
                    if (levitating)
                    {
                        spellCaster.mana.creature.ragdoll.RemovePhysicModifier(SnapManager.instance);
                        spellCaster.mana.creature.currentLocomotion.RemovePhysicModifier(SnapManager.instance);
                        levitating = false;
                    }
                    else
                    {
                        spellCaster.mana.creature.ragdoll.SetPhysicModifier(SnapManager.instance, 0.1f);
                        spellCaster.mana.creature.currentLocomotion.SetPhysicModifier(SnapManager.instance, 0.1f);
                        levitating = true;
                    }
                    break;
                case SnapManager.PlayerAction.Invisible:
                    spellCaster.mana.creature.hidden = !spellCaster.mana.creature.hidden;
                    foreach (Creature.RendererData renderer in spellCaster.mana.creature.renderers)
                    {
                        if (renderer.splitRenderer != null) renderer.splitRenderer.enabled = !spellCaster.mana.creature.hidden;
                        else if (renderer.renderer != null) renderer.renderer.enabled = !spellCaster.mana.creature.hidden;
                    }
                    spellCaster.mana.creature.HideItemsInHolders(spellCaster.mana.creature.hidden);
                    if (spellCaster.mana.creature.brain.instance.GetModule<BrainModuleSightable>(false) is BrainModuleSightable sight)
                    {
                        sight.sightDetectionMaxDistance = spellCaster.mana.creature.hidden ? 0 : 20;
                        sight.sightMaxDistance = spellCaster.mana.creature.hidden ? 5 : 30;
                    }
                    foreach (Creature creature in Creature.allActive)
                    {
                        if (creature.brain.currentTarget == spellCaster.mana.creature && spellCaster.mana.creature.hidden)
                        {
                            creature.brain.currentTarget = null;
                            creature.brain.SetState(Brain.State.Investigate);
                        }
                    }
                    break;
                case SnapManager.PlayerAction.Teleport:
                    Player.local.Teleport(spellCaster.mana.creature.transform.position + spellCaster.mana.creature.ragdoll.headPart.transform.forward * SnapManager.teleportDistanceLeft, spellCaster.mana.creature.transform.rotation, false, false);
                    break;
            }
        }
        public void PerformPlayerActionRight()
        {
            switch (SnapManager.playerActionRight)
            {
                case SnapManager.PlayerAction.Heal:
                    spellCaster.mana.creature.Heal(spellCaster.mana.creature.maxHealth - spellCaster.mana.creature.currentHealth, spellCaster.mana.creature);
                    break;
                case SnapManager.PlayerAction.Shockwave:
                    Shockwave(spellCaster.mana.creature.transform.position);
                    break;
                case SnapManager.PlayerAction.SpawnItem:
                    SnapManager.itemDataRight.SpawnAsync(item =>
                    {
                        if (spellCaster.ragdollHand.grabbedHandle == null && item?.GetMainHandle(spellCaster.ragdollHand.side) != null)
                            spellCaster.ragdollHand.Grab(item.GetMainHandle(spellCaster.ragdollHand.side));
                    }, spellCaster.magic.position, spellCaster.magic.rotation);
                    break;
                case SnapManager.PlayerAction.SpawnNPC:
                    CreatureTable creatureTableData = Catalog.GetData<CreatureTable>("Humans");
                    if (creatureTableData == null || !creatureTableData.TryPick(out CreatureData creatureData))
                        return;
                    CreatureData data = (CreatureData)creatureData.Clone();
                    data.containerID = SnapManager.creatureContainerRight.id;
                    data.factionId = SnapManager.creatureFactionRight;
                    data.SpawnAsync(spellCaster.mana.creature.transform.position + new Vector3(spellCaster.mana.creature.ragdoll.headPart.transform.forward.x, 0, spellCaster.mana.creature.ragdoll.headPart.transform.forward.z) * 2, spellCaster.mana.creature.ragdoll.headPart.transform.rotation.eulerAngles.y);
                    break;
                case SnapManager.PlayerAction.AntiMagic:
                    for (int index = Item.allActive.Count - 1; index >= 0; --index)
                    {
                        Item obj = Item.allActive[index];
                        ItemData itemData;
                        if (!obj.holder && !obj.disallowDespawn && obj.TryGetData(out itemData) && (itemData.HasModule<ItemModuleMagicProjectile>() || itemData.HasModule<ItemModuleAreaProjectile>()) && !obj.worldAttached)
                            obj.Despawn();
                    }
                    foreach (Creature creature in Creature.allActive)
                    {
                        if (!creature.isPlayer) creature.mana.currentMana = 0;
                    }
                    break;
                case SnapManager.PlayerAction.ToggleLevitate:
                    if (levitating)
                    {
                        spellCaster.mana.creature.ragdoll.RemovePhysicModifier(SnapManager.instance);
                        spellCaster.mana.creature.currentLocomotion.RemovePhysicModifier(SnapManager.instance);
                        levitating = false;
                    }
                    else
                    {
                        spellCaster.mana.creature.ragdoll.SetPhysicModifier(SnapManager.instance, 0.1f);
                        spellCaster.mana.creature.currentLocomotion.SetPhysicModifier(SnapManager.instance, 0.1f);
                        levitating = true;
                    }
                    break;
                case SnapManager.PlayerAction.Invisible:
                    spellCaster.mana.creature.hidden = !spellCaster.mana.creature.hidden;
                    foreach (Creature.RendererData renderer in spellCaster.mana.creature.renderers)
                    {
                        if (renderer.splitRenderer != null) renderer.splitRenderer.enabled = !spellCaster.mana.creature.hidden;
                        else if (renderer.renderer != null) renderer.renderer.enabled = !spellCaster.mana.creature.hidden;
                    }
                    spellCaster.mana.creature.HideItemsInHolders(spellCaster.mana.creature.hidden);
                    if (spellCaster.mana.creature.brain.instance.GetModule<BrainModuleSightable>(false) is BrainModuleSightable sight)
                    {
                        sight.sightDetectionMaxDistance = spellCaster.mana.creature.hidden ? 0 : 20;
                        sight.sightMaxDistance = spellCaster.mana.creature.hidden ? 5 : 30;
                    }
                    foreach (Creature creature in Creature.allActive)
                    {
                        if (creature.brain.currentTarget == spellCaster.mana.creature && spellCaster.mana.creature.hidden)
                        {
                            creature.brain.currentTarget = null;
                            creature.brain.SetState(Brain.State.Investigate);
                        }
                    }
                    break;
                case SnapManager.PlayerAction.Teleport:
                    Player.local.Teleport(spellCaster.mana.creature.transform.position + spellCaster.mana.creature.ragdoll.headPart.transform.forward * SnapManager.teleportDistanceRight, spellCaster.mana.creature.transform.rotation, false, false);
                    break;
            }
        }
        public void PerformItemActionLeft()
        {
            if (SnapManager.targetAllLeft)
            {
                if (SnapManager.itemActionLeft != SnapManager.ItemAction.Duplicate && SnapManager.itemActionLeft != SnapManager.ItemAction.Despawn)
                {
                    List<Item> items = new List<Item>();
                    foreach (Item item in Item.allActive)
                    {
                        if (item.spawnTime != 0 && item.data.GetModule<ItemModuleSpell>() == null && !item.physicBody.isKinematic && !items.Contains(item) && item.data.prefabAddress != "" && item.data.prefabAddress != null) items.Add(item);
                    }
                    if (items.Count > 0)
                    {
                        for (int i = items.Count - 1; i >= 0; --i)
                        {
                            ContinueItemActionLeft(items[i]);
                        }
                    }
                }
                if (SnapManager.itemActionLeft == SnapManager.ItemAction.Despawn)
                {
                    for (int index = Item.allActive.Count - 1; index >= 0; --index)
                    {
                        Item obj = Item.allActive[index];
                        ItemData data;
                        if (!obj.holder && !obj.disallowDespawn && (!obj.TryGetData(out data) || !data.HasModule<ItemModuleReturnInInventory>()) && !obj.worldAttached)
                            obj.Despawn();
                    }
                }
            }
            else
            {
                switch (SnapManager.distanceLeft)
                {
                    case SnapManager.TargetDistance.Random:
                        List<Item> items = new List<Item>();
                        foreach(Item item in Item.allActive)
                        {
                            if (item.data.GetModule<ItemModuleSpell>() == null && !item.physicBody.isKinematic && !items.Contains(item) && item.data.prefabAddress != "" && item.data.prefabAddress != null && ((!item.disallowDespawn && SnapManager.itemActionLeft == SnapManager.ItemAction.Despawn) || SnapManager.itemActionLeft != SnapManager.ItemAction.Despawn)) items.Add(item);
                        }
                        int index = UnityEngine.Random.Range(0, items.Count);
                        if (items.Count > 0)
                        {
                            ContinueItemActionLeft(items[index]);
                        }
                        break;
                    default:
                        ContinueItemActionLeft(CalculateItemDistanceLeft());
                        break;
                }
            }
        }
        public void PerformItemActionRight()
        {
            if (SnapManager.targetAllRight)
            {
                if (SnapManager.itemActionRight != SnapManager.ItemAction.Duplicate && SnapManager.itemActionRight != SnapManager.ItemAction.Despawn)
                {
                    List<Item> items = new List<Item>();
                    foreach (Item item in Item.allActive)
                    {
                        if (item.spawnTime != 0 && item.data.GetModule<ItemModuleSpell>() == null && !item.physicBody.isKinematic && !items.Contains(item) && item.data.prefabAddress != "" && item.data.prefabAddress != null) items.Add(item);
                    }
                    if (items.Count > 0)
                    {
                        for (int i = items.Count - 1; i >= 0; --i)
                        {
                            ContinueItemActionRight(items[i]);
                        }
                    }
                }
                if (SnapManager.itemActionRight == SnapManager.ItemAction.Despawn)
                {
                    for (int index = Item.allActive.Count - 1; index >= 0; --index)
                    {
                        Item obj = Item.allActive[index];
                        ItemData data;
                        if (!obj.holder && !obj.disallowDespawn && (!obj.TryGetData(out data) || !data.HasModule<ItemModuleReturnInInventory>()) && !obj.worldAttached)
                            obj.Despawn();
                    }
                }
            }
            else
            {
                switch (SnapManager.distanceRight)
                {
                    case SnapManager.TargetDistance.Random:
                        List<Item> items = new List<Item>();
                        foreach (Item item in Item.allActive)
                        {
                            if (item.data.GetModule<ItemModuleSpell>() == null && !item.physicBody.isKinematic && !items.Contains(item) && item.data.prefabAddress != "" && item.data.prefabAddress != null && ((!item.disallowDespawn && SnapManager.itemActionRight == SnapManager.ItemAction.Despawn) || SnapManager.itemActionRight != SnapManager.ItemAction.Despawn)) items.Add(item);
                        }
                        int index = UnityEngine.Random.Range(0, items.Count);
                        if (items.Count > 0)
                        {
                            ContinueItemActionRight(items[index]);
                        }
                        break;
                    default:
                        ContinueItemActionRight(CalculateItemDistanceRight());
                        break;
                }
            }
        }
        public void ContinueItemActionLeft(Item item)
        {
            if (item != null)
                switch (SnapManager.itemActionLeft)
                {
                    case SnapManager.ItemAction.Duplicate:
                        item.data.SpawnAsync(spawnedItem =>
                        {
                            if (spellCaster.ragdollHand.grabbedHandle == null && spawnedItem?.GetMainHandle(spellCaster.ragdollHand.side) != null)
                                spellCaster.ragdollHand.Grab(spawnedItem.GetMainHandle(spellCaster.ragdollHand.side));
                        }, spellCaster.magic.position, spellCaster.magic.rotation);
                        break;
                    case SnapManager.ItemAction.Explode:
                        if (!item.IsHanded() && !item.isBrokenPiece)
                            Explode(item.transform.position);
                        break;
                    case SnapManager.ItemAction.Break:
                        if (item.breakable != null && !item.breakable.IsBroken)
                            item.breakable.Break();
                        break;
                    case SnapManager.ItemAction.Levitate:
                        item.SetPhysicModifier(SnapManager.instance, 0);
                        break;
                    case SnapManager.ItemAction.StopLevitate:
                        item.RemovePhysicModifier(SnapManager.instance);
                        break;
                    case SnapManager.ItemAction.Despawn:
                        item.Despawn();
                        break;
                }
        }
        public void ContinueItemActionRight(Item item)
        {
            if (item != null)
                switch (SnapManager.itemActionRight)
                {
                    case SnapManager.ItemAction.Duplicate:
                        item.data.SpawnAsync(spawnedItem =>
                        {
                            if (spellCaster.ragdollHand.grabbedHandle == null && spawnedItem?.GetMainHandle(spellCaster.ragdollHand.side) != null)
                                spellCaster.ragdollHand.Grab(spawnedItem.GetMainHandle(spellCaster.ragdollHand.side));
                        }, spellCaster.magic.position, spellCaster.magic.rotation);
                        break;
                    case SnapManager.ItemAction.Explode:
                        if (!item.IsHanded() && !item.isBrokenPiece)
                            Explode(item.transform.position);
                        break;
                    case SnapManager.ItemAction.Break:
                        if (item.breakable != null && !item.breakable.IsBroken)
                            item.breakable.Break();
                        break;
                    case SnapManager.ItemAction.Levitate:
                        item.SetPhysicModifier(SnapManager.instance, 0);
                        break;
                    case SnapManager.ItemAction.StopLevitate:
                        item.RemovePhysicModifier(SnapManager.instance);
                        break;
                    case SnapManager.ItemAction.Despawn:
                        item.Despawn();
                        break;
                }
        }
        public Item CalculateItemDistanceLeft()
        {
            Item item1 = null;
            foreach (Item item in Item.allActive)
            {
                if (item.loaded && item.data.GetModule<ItemModuleSpell>() == null && !item.physicBody.isKinematic && item.data.prefabAddress != "" && item.data.prefabAddress != null && 
                    ((SnapManager.distanceLeft == SnapManager.TargetDistance.Closest && (item1 == null || Vector3.Distance(item.transform.position, spellCaster.magic.position) < Vector3.Distance(item1.transform.position, spellCaster.magic.position))) ||
                    (SnapManager.distanceLeft == SnapManager.TargetDistance.Farthest && (item1 == null || Vector3.Distance(item.transform.position, spellCaster.magic.position) > Vector3.Distance(item1.transform.position, spellCaster.magic.position))))) item1 = item;
            }
            return item1;
        }
        public Item CalculateItemDistanceRight()
        {
            Item item1 = null;
            foreach (Item item in Item.allActive)
            {
                if (item.loaded && item.data.GetModule<ItemModuleSpell>() == null && !item.physicBody.isKinematic && item.data.prefabAddress != "" && item.data.prefabAddress != null &&
                    ((SnapManager.distanceRight == SnapManager.TargetDistance.Closest && (item1 == null || Vector3.Distance(item.transform.position, spellCaster.magic.position) < Vector3.Distance(item1.transform.position, spellCaster.magic.position))) ||
                    (SnapManager.distanceRight == SnapManager.TargetDistance.Farthest && (item1 == null || Vector3.Distance(item.transform.position, spellCaster.magic.position) > Vector3.Distance(item1.transform.position, spellCaster.magic.position))))) item1 = item;
            }
            return item1;
        }
        public IEnumerator Ignite(Creature creature)
        {
            EffectInstance burnInstance = burnData.Spawn(creature.transform);
            burnInstance.SetRenderer(creature.GetRendererForVFX(), false);
            burnInstance.SetIntensity(1f);
            burnInstance.Play();
            float time = Time.time;
            while(Time.time - time < 10)
            {
                CollisionInstance instance = new CollisionInstance(new DamageStruct(DamageType.Energy, 5));
                creature.Damage(instance);
                yield return new WaitForSeconds(1);
            }
            burnInstance.Stop();
            yield break;
        }
        public void Smite(Creature creature)
        {
            GameObject source = new GameObject("Source");
            GameObject impact = new GameObject("Impact");
            EffectInstance boltInstance;
            boltInstance = largeBoltData.Spawn(creature.transform.position, Quaternion.LookRotation(Vector3.up));
            source.transform.position = creature.transform.position + (Vector3.up * 100);
            impact.transform.position = creature.transform.position;
            CollisionInstance collision = new CollisionInstance(new DamageStruct(DamageType.Energy, 50));
            collision.contactPoint = creature.transform.position;
            collision.contactNormal = Vector3.up;
            collision.targetCollider = creature.ragdoll.rootPart.colliderGroup.colliders[0];
            collision.targetColliderGroup = creature.ragdoll.rootPart.colliderGroup;
            collision.damageStruct.hitRagdollPart = creature.ragdoll.rootPart;
            creature.Damage(collision);
            creature.TryPush(Creature.PushType.Magic, Vector3.down, 3);
            creature.TryElectrocute(10, 5, true, false, electrocuteData);
            creature.lastInteractionTime = Time.time;
            creature.lastInteractionCreature = imbue?.colliderGroup?.collisionHandler?.item?.mainHandler?.ragdoll?.creature;
            boltInstance.SetSource(source.transform);
            boltInstance.SetTarget(impact.transform);
            boltInstance.SetIntensity(1f);
            boltInstance.Play();
            GameObject.Destroy(source, 5);
            GameObject.Destroy(impact, 5);
        }
        public void Explode(Vector3 origin)
        {
            EffectInstance effectInstance = explosionData.Spawn(origin, Quaternion.LookRotation(Vector3.forward, Vector3.up));
            effectInstance.SetIntensity(1f);
            effectInstance.Play();
            Collider[] sphereContacts = Physics.OverlapSphere(origin, 5, 232799233);
            List<Creature> creaturesPushed = new List<Creature>();
            List<Rigidbody> rigidbodiesPushed = new List<Rigidbody>();
            List<Creature> forcedPhysicCreatures = new List<Creature>();
            creaturesPushed.Add(spellCaster.mana.creature);
            foreach (Creature creature in Creature.allActive)
            {
                if (!creature.isPlayer && !creature.isKilled && Vector3.Distance(origin, creature.transform.position) < 5 && !creaturesPushed.Contains(creature))
                {
                    forcedPhysicCreatures.Add(creature);
                    creature.ragdoll.AddPhysicToggleModifier(this);
                    CollisionInstance collision = new CollisionInstance(new DamageStruct(DamageType.Energy, 25));
                    collision.damageStruct.hitRagdollPart = creature.ragdoll.rootPart;
                    collision.casterHand = spellCaster;
                    creature.Damage(collision);
                    creature.ragdoll.SetState(Ragdoll.State.Destabilized);
                    creature.lastInteractionTime = Time.time;
                    creature.lastInteractionCreature = spellCaster.mana.creature;
                    creaturesPushed.Add(creature);
                }
            }
            foreach (Collider collider in sphereContacts)
            {
                Breakable breakable = collider.attachedRigidbody?.GetComponentInParent<Breakable>();
                if (breakable != null)
                {
                    if (!breakable.IsBroken && breakable.canInstantaneouslyBreak)
                        breakable.Break();
                    for (int index = 0; index < breakable.subBrokenItems.Count; ++index)
                    {
                        Rigidbody rigidBody = breakable.subBrokenItems[index].physicBody.rigidBody;
                        if (rigidBody && !rigidbodiesPushed.Contains(rigidBody))
                        {
                            rigidBody.AddExplosionForce(10, origin, 5, 0.5f, ForceMode.VelocityChange);
                            rigidbodiesPushed.Add(rigidBody);
                        }
                    }
                    for (int index = 0; index < breakable.subBrokenBodies.Count; ++index)
                    {
                        PhysicBody subBrokenBody = breakable.subBrokenBodies[index];
                        if (subBrokenBody && !rigidbodiesPushed.Contains(subBrokenBody.rigidBody))
                        {
                            subBrokenBody.rigidBody.AddExplosionForce(10, origin, 5, 0.5f, ForceMode.VelocityChange);
                            rigidbodiesPushed.Add(subBrokenBody.rigidBody);
                        }
                    }
                }
                if (collider.attachedRigidbody != null && !collider.attachedRigidbody.isKinematic && Vector3.Distance(origin, collider.transform.position) < 5)
                {
                    if (collider.attachedRigidbody.gameObject.layer != GameManager.GetLayer(LayerName.NPC) && !rigidbodiesPushed.Contains(collider.attachedRigidbody))
                    {
                        collider.attachedRigidbody.AddExplosionForce(10, origin, 5, 0.5f, ForceMode.VelocityChange);
                        rigidbodiesPushed.Add(collider.attachedRigidbody);
                    }
                }
            }
            foreach (Creature creature in forcedPhysicCreatures)
                creature?.ragdoll.RemovePhysicToggleModifier(this);
        }
        public void Shockwave(Vector3 origin)
        {
            EffectInstance effectInstance = shockwaveData.Spawn(origin, Quaternion.LookRotation(Vector3.up));
            effectInstance.SetIntensity(1f);
            effectInstance.Play();
            Collider[] sphereContacts = Physics.OverlapSphere(origin, 10, 232799233);
            List<Creature> creaturesPushed = new List<Creature>();
            List<Rigidbody> rigidbodiesPushed = new List<Rigidbody>();
            List<Creature> forcedPhysicCreatures = new List<Creature>();
            creaturesPushed.Add(spellCaster.mana.creature);
            rigidbodiesPushed.Add(spellCaster.mana.creature.locomotion.rb);
            foreach (Creature creature in Creature.allActive)
            {
                if (!creature.isPlayer && !creature.isKilled && Vector3.Distance(origin, creature.transform.position) < 10 && !creaturesPushed.Contains(creature))
                {
                    forcedPhysicCreatures.Add(creature);
                    creature.ragdoll.AddPhysicToggleModifier(this);
                    creature.ragdoll.SetState(Ragdoll.State.Destabilized);
                    creature.lastInteractionTime = Time.time;
                    creature.lastInteractionCreature = spellCaster.mana.creature;
                    creaturesPushed.Add(creature);
                }
            }
            foreach (Collider collider in sphereContacts)
            {
                if (collider.attachedRigidbody != null && !collider.attachedRigidbody.isKinematic && Vector3.Distance(origin, collider.transform.position) < 10)
                {
                    if (collider.attachedRigidbody.gameObject.layer != GameManager.GetLayer(LayerName.NPC) && !rigidbodiesPushed.Contains(collider.attachedRigidbody) && collider.attachedRigidbody.GetComponentInParent<Player>() == null)
                    {
                        collider.attachedRigidbody.AddExplosionForce(20, origin, 10, 0.5f, ForceMode.VelocityChange);
                        rigidbodiesPushed.Add(collider.attachedRigidbody);
                    }
                }
            }
            foreach (Creature creature in forcedPhysicCreatures)
                creature?.ragdoll.RemovePhysicToggleModifier(this);
        }
    }
}
