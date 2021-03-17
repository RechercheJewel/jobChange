using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using Modding;
using ModCommon;
using ModCommon.Util;
using UnityEngine;
using System.Security.Cryptography;
using SFCore;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using MonoMod.RuntimeDetour;
using System.Collections;

namespace ClassSwap
{
    public class CHESaveSettings : ModSettings
    {
        // Better charms, insert default values here
        public List<bool> gotCharms = new List<bool>() { true, true, true, true, true, true };
        public List<bool> newCharms = new List<bool>() { false, false, false, false, false, false };
        public List<bool> equippedCharms = new List<bool>() { false, false, false, false, false, false };
        public List<int> charmCosts = new List<int>() { 2, 2, 2, 2, 2, 2 };
    }

    public class CHEGlobalSettings : ModSettings
    {
    }

    public class jobChangeCharms : Mod<CHESaveSettings, CHEGlobalSettings>
    {
        internal static jobChangeCharms Instance;
        //Holds test sprite.
        private Sprite mageSprite;
        private Sprite tankSprite;
        private Sprite jumpSprite;
        private Sprite spiderSprite;
        private Sprite warriorSprite;
        private Sprite priestSprite;
        // Holds the regen time for soul.
        private float soulRegenTime;
        //Checks how many of the charms are equipped.
        private int charmsEquipped;
        private int snailEquipped;
        private int beetleEquipped;
        private int mantisEquipped;
        private int beeEquipped;
        private int mothEquipped;
        private int spiderEquipped;
        private int mantisHits;
        //Counts bee combo.
        private int beeCombo;
        private int beetleCombo;
        //If bee has parried.
        private bool beetleParried;
        //Uses charmhelper to make the charms.
        public CharmHelper knightCharms { get; private set; }

        // Thx to 56
        public override string GetVersion()
        {
            //Assembly asm = Assembly.GetExecutingAssembly();

            //string ver = asm.GetName().Version.ToString();

            //SHA1 sha1 = SHA1.Create();
            //FileStream stream = File.OpenRead(asm.Location);

            //byte[] hashBytes = sha1.ComputeHash(stream);

            //string hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

            //stream.Close();
            //sha1.Clear();

            //string ret = $"{ver}-{hash.Substring(0, 6)}";

            //return ret;
            string ret = "Version 1.0";
            return ret;
        }

        public override void Initialize()
        {
            Log("Initializing");
            Instance = this;
            loadResources();
            string[] resourceNames = GetEmbeddedResourceNames();
            foreach (var s in resourceNames)
                Log(s);
            initGlobalSettings();
            knightCharms = new CharmHelper();
            //How many charms there are.
            knightCharms.customCharms = 6;
            //Charm images.
            knightCharms.customSprites = new Sprite[] { mageSprite, tankSprite, warriorSprite, jumpSprite, priestSprite, spiderSprite };
            initCallbacks();

            Log("Initialized");
        }

        public static string[] GetEmbeddedResourceNames()
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceNames();
        }

        //Loads the images.
        private void loadResources()
        {
            Assembly _asm = Assembly.GetExecutingAssembly();
            using (Stream s = _asm.GetManifestResourceStream("jobChange.mage.png"))
            {
                if (s != null)
                {
                    byte[] buffer = new byte[s.Length];
                    s.Read(buffer, 0, buffer.Length);
                    s.Dispose();

                    //Create texture from bytes
                    var tex = new Texture2D(2, 2);

                    tex.LoadImage(buffer, true);

                    Log("Something to be logged as long as logging is enabled");
                    LogDebug("Something to be logged as long as debug logs are enabled");
                    LogError("Something to be logged as long as error logs are enabled");
                    LogFine("Something to be logged as long as dev logs are enabled");
                    LogWarn("Something to be logged as long as warning logs are enabled");

                    // Create sprite from texture
                    mageSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                }
            }
            using (Stream s = _asm.GetManifestResourceStream("jobChange.beetle.png"))
            {
                if (s != null)
                {
                    byte[] buffer = new byte[s.Length];
                    s.Read(buffer, 0, buffer.Length);
                    s.Dispose();

                    //Create texture from bytes
                    var tex = new Texture2D(2, 2);

                    tex.LoadImage(buffer, true);

                    Log("Something to be logged as long as logging is enabled");
                    LogDebug("Something to be logged as long as debug logs are enabled");
                    LogError("Something to be logged as long as error logs are enabled");
                    LogFine("Something to be logged as long as dev logs are enabled");
                    LogWarn("Something to be logged as long as warning logs are enabled");

                    // Create sprite from texture
                    tankSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                }
            }
            using (Stream s = _asm.GetManifestResourceStream("jobChange.bee.png"))
            {
                if (s != null)
                {
                    byte[] buffer = new byte[s.Length];
                    s.Read(buffer, 0, buffer.Length);
                    s.Dispose();

                    //Create texture from bytes
                    var tex = new Texture2D(2, 2);

                    tex.LoadImage(buffer, true);

                    Log("Something to be logged as long as logging is enabled");
                    LogDebug("Something to be logged as long as debug logs are enabled");
                    LogError("Something to be logged as long as error logs are enabled");
                    LogFine("Something to be logged as long as dev logs are enabled");
                    LogWarn("Something to be logged as long as warning logs are enabled");

                    // Create sprite from texture
                    jumpSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                }
            }
            using (Stream s = _asm.GetManifestResourceStream("jobChange.mantis.png"))
            {
                if (s != null)
                {
                    byte[] buffer = new byte[s.Length];
                    s.Read(buffer, 0, buffer.Length);
                    s.Dispose();

                    //Create texture from bytes
                    var tex = new Texture2D(2, 2);

                    tex.LoadImage(buffer, true);

                    Log("Something to be logged as long as logging is enabled");
                    LogDebug("Something to be logged as long as debug logs are enabled");
                    LogError("Something to be logged as long as error logs are enabled");
                    LogFine("Something to be logged as long as dev logs are enabled");
                    LogWarn("Something to be logged as long as warning logs are enabled");

                    // Create sprite from texture
                    warriorSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                }
            }
            using (Stream s = _asm.GetManifestResourceStream("jobChange.placeholder.png"))
            {
                if (s != null)
                {
                    byte[] buffer = new byte[s.Length];
                    s.Read(buffer, 0, buffer.Length);
                    s.Dispose();

                    //Create texture from bytes
                    var tex = new Texture2D(2, 2);

                    tex.LoadImage(buffer, true);

                    Log("Something to be logged as long as logging is enabled");
                    LogDebug("Something to be logged as long as debug logs are enabled");
                    LogError("Something to be logged as long as error logs are enabled");
                    LogFine("Something to be logged as long as dev logs are enabled");
                    LogWarn("Something to be logged as long as warning logs are enabled");

                    // Create sprite from texture
                    priestSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                }
            }
            using (Stream s = _asm.GetManifestResourceStream("jobChange.placeholder.png"))
            {
                if (s != null)
                {
                    byte[] buffer = new byte[s.Length];
                    s.Read(buffer, 0, buffer.Length);
                    s.Dispose();

                    //Create texture from bytes
                    var tex = new Texture2D(2, 2);

                    tex.LoadImage(buffer, true);

                    Log("Something to be logged as long as logging is enabled");
                    LogDebug("Something to be logged as long as debug logs are enabled");
                    LogError("Something to be logged as long as error logs are enabled");
                    LogFine("Something to be logged as long as dev logs are enabled");
                    LogWarn("Something to be logged as long as warning logs are enabled");

                    // Create sprite from texture
                    spiderSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                }
            }
        }

        private void initGlobalSettings()
        {
            // Found in a project, might help saving, don't know, but who cares
            // Global Settings
        }

        private void initSaveSettings(SaveGameData data)
        {
            // Charms
            Settings.gotCharms = Settings.gotCharms;
            Settings.newCharms = Settings.newCharms;
            Settings.equippedCharms = Settings.equippedCharms;
            Settings.charmCosts = Settings.charmCosts;
        }

        //Hooks
        private void initCallbacks()
        {
            // Hooks
            ModHooks.Instance.GetPlayerBoolHook += OnGetPlayerBoolHook;
            ModHooks.Instance.SetPlayerBoolHook += OnSetPlayerBoolHook;
            ModHooks.Instance.GetPlayerIntHook += OnGetPlayerIntHook;
            ModHooks.Instance.SetPlayerIntHook += OnSetPlayerIntHook;
            ModHooks.Instance.AfterSavegameLoadHook += initSaveSettings;
            ModHooks.Instance.ApplicationQuitHook += SaveCHEGlobalSettings;
            ModHooks.Instance.LanguageGetHook += OnLanguageGetHook;
            ModHooks.Instance.SoulGainHook += SoulGain;
            ModHooks.Instance.CharmUpdateHook += OnCharmUpdate;
            ModHooks.Instance.HeroUpdateHook += OnHeroUpdate;
            //ModHooks.Instance.DoAttackHook += OnDoAttack;
            ModHooks.Instance.HitInstanceHook += OnHit;
            ModCommon.ModCommon.OnSpellHook += OnCast;
            //Thanks to 56 for giving me this little snippet.
            //On parrying.
            On.HeroController.NailParry += (orig, self) => 
            {
                //Beetle gains a half second of invulnerability when successfully parrying.
                if (PlayerData.instance.GetBool("equippedCharm_42"))
                {
                    HeroController.instance.StartCoroutine(beetleParry());
                }
                orig(self);
            };
        }


        private IEnumerator beetleParry()
        {
            PlayerData.instance.isInvincible = true;
            beetleParried = true;
            yield return new WaitForSeconds(0.5f);
            PlayerData.instance.isInvincible = false;
            yield return new WaitForSeconds(0.5f);
            beetleParried = false;
        }

        //Sets user invincible for half a second after bodyslamming someone.
        private IEnumerator beetleBodyslam()
        {
            PlayerData.instance.isInvincible = true;
            //PlayerData.instance.blockerHits -= 1;
            yield return new WaitForSeconds(.3f);
            PlayerData.instance.isInvincible = false;
        }



        //On hit effects
        private HitInstance OnHit(Fsm owner, HitInstance hit)
        {
            //Amplifies bee damage by combo.
            if (PlayerData.instance.GetBool("equippedCharm_44") && ((HeroController.instance.GetState("jumping") || HeroController.instance.GetState("falling"))))
            {
                hit.DamageDealt = (8 + beeCombo) * hit.DamageDealt / 10;
            }
            if (PlayerData.instance.GetBool("equippedCharm_42"))
            {
                //Does beetle bodyslam.
                if (PlayerData.instance.GetBool("equippedCharm_5") && (PlayerData.instance.blockerHits > 0) && ((hit.Source.name.Contains("SuperDash") || hit.Source.name.Contains("Trail Effect") || hit.Source.name.Contains("SD Burst") || hit.Source.name.Contains("Crystal Burst"))))
                {
                    hit.DamageDealt = 20 + PlayerData.instance.GetInt("nailSmithUpgrades") * 16;
                    hit.AttackType = AttackTypes.Generic;
                    HeroController.instance.StartCoroutine(beetleBodyslam());
                    //PlayerData.instance.blockerHits -= 1;
                    //For testing.
                    //HeroController.instance.AddHealth(1);
                }
                //Multiplies beetle damage by 3 if they parried.
                else if (beetleParried)
                {
                    hit.DamageDealt = 3 * hit.DamageDealt;
                }
                //Reduces beetle damage by 20%.
                else
                {
                    hit.DamageDealt = 4 * hit.DamageDealt / 5;
                }
            }
            switch (hit.AttackType)
            {
                case AttackTypes.Nail:
                    {
                        //Cuts snail shaman damage in half.
                        if (PlayerData.instance.GetBool("equippedCharm_41"))
                        {
                            hit.DamageDealt = hit.DamageDealt / 2;
                        }
                        //Boosts mantis damage by 1/5th.
                        if (PlayerData.instance.GetBool("equippedCharm_43"))
                        {
                            hit.DamageDealt = 7 * hit.DamageDealt / 6;
                        }
                    }
                    break;
                case AttackTypes.Spell:
                    {
                        //Cuts spell damage in half for mantis.
                        if (PlayerData.instance.GetBool("equippedCharm_43"))
                        {
                            hit.DamageDealt = hit.DamageDealt / 2;
                        }
                    }
                    break;
            }
            return hit;
        }

        //On cast.
        public bool OnCast(ModCommon.ModCommon.Spell spell)
        {
            if (PlayerData.instance.GetBool("equippedCharm_44"))
            {
                HeroController.instance.ResetAirMoves();
            }
            return true;
        }

        //Soul regen
        public void OnHeroUpdate()
        {
            float deltaTime = Time.deltaTime;
            // Snail regen.
            if (PlayerData.instance.GetBool("equippedCharm_41"))
            {
                soulRegenTime -= Time.deltaTime;
                if (soulRegenTime <= 0f)
                {
                    soulRegenTime = 1f;
                    HeroController.instance.AddMPCharge(3);
                }
            }
            if (HeroController.instance.GetState("onGround"))
            {
                beeCombo = 0;
            }
        }

        private void SaveCHEGlobalSettings()
        {
            SaveGlobalSettings();
        }

        #region Get/Set Hooks

        // Names and descriptions of charms
        private string OnLanguageGetHook(string key, string sheet)
        {
            //Log($"Sheet: {sheet}; Key: {key}");
            // There probably is a better way to do this, but for now take this
            #region Custom Charms
            if (key.StartsWith("CHARM_NAME_41"))
            {
                int charmNum = int.Parse(key.Split('_')[2]);
                if (knightCharms.charmIDs.Contains(charmNum))
                {
                    return "Snail Shaman";
                }
            }
            if (key.StartsWith("CHARM_DESC_41"))
            {
                int charmNum = int.Parse(key.Split('_')[2]);
                if (knightCharms.charmIDs.Contains(charmNum))
                {
                    return "For those that value their raw magical might over all, the spells of the snail shamans are naturally fitting in their hands. Gives a small passive soul regen and amplifies offensive spell charms, but heavily reduces nail damage.";
                }
            }
            if (key.StartsWith("CHARM_NAME_42"))
            {
                int charmNum = int.Parse(key.Split('_')[2]);
                if (knightCharms.charmIDs.Contains(charmNum))
                {
                    return "Beetle Warrior";
                }
            }
            if (key.StartsWith("CHARM_DESC_42"))
            {
                int charmNum = int.Parse(key.Split('_')[2]);
                if (knightCharms.charmIDs.Contains(charmNum))
                {
                    return "Durable, tough and mighty, the beetle warriors can endure even the fiercest assaults. Gives passive health bonuses and amplifies defensive charms.";
                }
            }
            if (key.StartsWith("CHARM_NAME_43"))
            {
                int charmNum = int.Parse(key.Split('_')[2]);
                if (knightCharms.charmIDs.Contains(charmNum))
                {
                    return "Mantis Lord";
                }
            }
            if (key.StartsWith("CHARM_DESC_43"))
            {
                int charmNum = int.Parse(key.Split('_')[2]);
                if (knightCharms.charmIDs.Contains(charmNum))
                {
                    return "Proud, honorable and fierce, the flurry and might of the mantis lords cut all down before them. Increases damage from nail and amplifies nail focused charms, but reduces soul from strikes.";
                }
            }
            if (key.StartsWith("CHARM_NAME_44"))
            {
                int charmNum = int.Parse(key.Split('_')[2]);
                if (knightCharms.charmIDs.Contains(charmNum))
                {
                    return "Bee Knight";
                }
            }
            if (key.StartsWith("CHARM_DESC_44"))
            {
                int charmNum = int.Parse(key.Split('_')[2]);
                if (knightCharms.charmIDs.Contains(charmNum))
                {
                    return "Fighters of the air, the bee knights are fierce protectors of their queen. While not all bee knights have wings, you won't let that stop you. Amplifies damage of aerial attacks as well as doubles soul gained from hits, as well as amplifying some charms.";
                }
            }
            if (key.StartsWith("CHARM_NAME_45"))
            {
                int charmNum = int.Parse(key.Split('_')[2]);
                if (knightCharms.charmIDs.Contains(charmNum))
                {
                    return "Moth Seer";
                }
            }
            if (key.StartsWith("CHARM_DESC_45"))
            {
                int charmNum = int.Parse(key.Split('_')[2]);
                if (knightCharms.charmIDs.Contains(charmNum))
                {
                    return "TBA";
                }
            }
            if (key.StartsWith("CHARM_NAME_46"))
            {
                int charmNum = int.Parse(key.Split('_')[2]);
                if (knightCharms.charmIDs.Contains(charmNum))
                {
                    return "Spider Weaver";
                }
            }
            if (key.StartsWith("CHARM_DESC_46"))
            {
                int charmNum = int.Parse(key.Split('_')[2]);
                if (knightCharms.charmIDs.Contains(charmNum))
                {
                    return "TBA";
                }
            }
            #endregion
            return Language.Language.GetInternal(key, sheet);
        }

        private bool OnGetPlayerBoolHook(string target)
        {
            if (Settings.BoolValues.ContainsKey(target))
            {
                return Settings.BoolValues[target];
            }
            #region Custom Charms
            if (target.StartsWith("gotCharm_"))
            {
                int charmNum = int.Parse(target.Split('_')[1]);
                if (knightCharms.charmIDs.Contains(charmNum))
                {
                    return Settings.gotCharms[knightCharms.charmIDs.IndexOf(charmNum)];
                }
            }
            if (target.StartsWith("newCharm_"))
            {
                int charmNum = int.Parse(target.Split('_')[1]);
                if (knightCharms.charmIDs.Contains(charmNum))
                {
                    return Settings.newCharms[knightCharms.charmIDs.IndexOf(charmNum)];
                }
            }
            if (target.StartsWith("equippedCharm_"))
            {
                int charmNum = int.Parse(target.Split('_')[1]);
                if (knightCharms.charmIDs.Contains(charmNum))
                {
                    return Settings.equippedCharms[knightCharms.charmIDs.IndexOf(charmNum)];
                }
            }
            #endregion
            return PlayerData.instance.GetBoolInternal(target);
        }
        private void OnSetPlayerBoolHook(string target, bool val)
        {
            if (Settings.BoolValues.ContainsKey(target))
            {
                Settings.BoolValues[target] = val;
                return;
            }
            #region Custom Charms
            if (target.StartsWith("gotCharm_"))
            {
                int charmNum = int.Parse(target.Split('_')[1]);
                if (knightCharms.charmIDs.Contains(charmNum))
                {
                    Settings.gotCharms[knightCharms.charmIDs.IndexOf(charmNum)] = val;
                    return;
                }
            }
            if (target.StartsWith("newCharm_"))
            {
                int charmNum = int.Parse(target.Split('_')[1]);
                if (knightCharms.charmIDs.Contains(charmNum))
                {
                    Settings.newCharms[knightCharms.charmIDs.IndexOf(charmNum)] = val;
                    return;
                }
            }
            if (target.StartsWith("equippedCharm_"))
            {
                int charmNum = int.Parse(target.Split('_')[1]);
                if (knightCharms.charmIDs.Contains(charmNum))
                {
                    Settings.equippedCharms[knightCharms.charmIDs.IndexOf(charmNum)] = val;
                    return;
                }
            }
            #endregion
            PlayerData.instance.SetBoolInternal(target, val);
        }

        private int OnGetPlayerIntHook(string target)
        {
            if (Settings.IntValues.ContainsKey(target))
            {
                return Settings.IntValues[target];
            }
            #region Custom Charms
            if (target.StartsWith("charmCost_"))
            {
                int charmNum = int.Parse(target.Split('_')[1]);
                if (knightCharms.charmIDs.Contains(charmNum))
                {
                    return Settings.charmCosts[knightCharms.charmIDs.IndexOf(charmNum)];
                }
            }
            #endregion
            return PlayerData.instance.GetIntInternal(target);
        }
        private void OnSetPlayerIntHook(string target, int val)
        {
            if (Settings.IntValues.ContainsKey(target))
            {
                Settings.IntValues[target] = val;
            }
            else
            {
                PlayerData.instance.SetIntInternal(target, val);
            }
            //Log("Int  set: " + target + "=" + val.ToString());
        }
        #endregion

        //Gives passive buffs from charms, changes costs.
        private void OnCharmUpdate(PlayerData data, HeroController controller)
        {
            //Adds the HP from defensive charm. TBA
            
            CountCharms();
            //Sets mantis and bee for dashmaster.
            if (PlayerData.instance.GetBool("equippedCharm_44") || PlayerData.instance.GetBool("equippedCharm_43"))
            {
                PlayerData.instance.charmCost_31 = 1;
            }
            else
            {
                PlayerData.instance.charmCost_31 = 2;
            }
            // Makes mantis change sprintmaster.
            if (PlayerData.instance.GetBool("equippedCharm_43"))
            {
                PlayerData.instance.charmCost_37 = 0;
            }
            else
            {
                PlayerData.instance.charmCost_37 = 1;
            }
            //Makes bee gathering swarm free.
            if (PlayerData.instance.GetBool("equippedCharm_44"))
            {
                PlayerData.instance.charmCost_1 = 0;
            }
            else
            {
                PlayerData.instance.charmCost_1 = 1;
            }
            //Sets glowing womb.
            if (PlayerData.instance.GetBool("equippedCharm_44"))
            {
                PlayerData.instance.charmCost_22 = 1;
            }
            else
            {
                PlayerData.instance.charmCost_22 = 2;
            }
            //Sets Flukenest.
            if (PlayerData.instance.GetBool("equippedCharm_44"))
            {
                PlayerData.instance.charmCost_11 = 2;
            }
            else
            {
                PlayerData.instance.charmCost_11 = 3;
            }
            //Sets Hibeblood
            if (PlayerData.instance.GetBool("equippedCharm_44"))
            {
                PlayerData.instance.charmCost_29 = 3;
            }
            else
            {
                PlayerData.instance.charmCost_29 = 4;
            }
            //Sets heavy blow
            if (PlayerData.instance.GetBool("equippedCharm_44") || PlayerData.instance.GetBool("equippedCharm_43"))
            {
                PlayerData.instance.charmCost_15 = 1;
            }
            else
            {
                PlayerData.instance.charmCost_15 = 2;
            }
            //Sets sharp shadow.
            if (PlayerData.instance.GetBool("equippedCharm_44") || PlayerData.instance.GetBool("equippedCharm_43"))
            {
                PlayerData.instance.charmCost_16 = 1;
            }
            else
            {
                PlayerData.instance.charmCost_16 = 2;
            }
            //Sets nailmaster's glory.
            if (PlayerData.instance.GetBool("equippedCharm_43"))
            {
                PlayerData.instance.charmCost_26 = 0;
            }
            else
            {
                PlayerData.instance.charmCost_26 = 1;
            }
            //Sets fury of the fallen.
            if (PlayerData.instance.GetBool("equippedCharm_43"))
            {
                PlayerData.instance.charmCost_6 = 1;
            }
            else
            {
                PlayerData.instance.charmCost_6 = 2;
            }
            //Sets steady body.
            if (PlayerData.instance.GetBool("equippedCharm_43") || PlayerData.instance.GetBool("equippedCharm_42"))
            {
                PlayerData.instance.charmCost_14 = 0;
            }
            else
            {
                PlayerData.instance.charmCost_14 = 1;
            }
            //Sets shaman stone.
            if (PlayerData.instance.GetBool("equippedCharm_41"))
            {
                PlayerData.instance.charmCost_19 = 2;
            }
            else
            {
                PlayerData.instance.charmCost_19 = 3;
            }
            //Sets grubberfly's elegy.
            if (PlayerData.instance.GetBool("equippedCharm_41") || PlayerData.instance.GetBool("equippedCharm_42"))
            {
                PlayerData.instance.charmCost_35 = 2;
            }
            else
            {
                PlayerData.instance.charmCost_35 = 3;
            }
            //Sets shape of unn.
            if (PlayerData.instance.GetBool("equippedCharm_41"))
            {
                PlayerData.instance.charmCost_28 = 2;
            }
            else
            {
                PlayerData.instance.charmCost_28 = 3;
            }
            //Sets stalwart shell.
            if (PlayerData.instance.GetBool("equippedCharm_42"))
            {
                PlayerData.instance.charmCost_4 = 0;
            }
            else
            {
                PlayerData.instance.charmCost_4 = 2;
            }
            //Sets baldur shell.
            if (PlayerData.instance.GetBool("equippedCharm_42"))
            {
                PlayerData.instance.charmCost_5 = 1;
            }
            else
            {
                PlayerData.instance.charmCost_5 = 2;
            }
            //Sets dream shield.
            if (PlayerData.instance.GetBool("equippedCharm_42") || PlayerData.instance.GetBool("equippedCharm_45"))
            {
                PlayerData.instance.charmCost_38 = 1;
            }
            else
            {
                PlayerData.instance.charmCost_38 = 3;
            }
            //Sets defender's crest.
            if (PlayerData.instance.GetBool("equippedCharm_42"))
            {
                PlayerData.instance.charmCost_10 = 0;
            }
            else
            {
                PlayerData.instance.charmCost_10 = 1;
            }
            //Sets thorn's of agony.
            if (PlayerData.instance.GetBool("equippedCharm_42"))
            {
                PlayerData.instance.charmCost_12 = 0;
            }
            else
            {
                PlayerData.instance.charmCost_12 = 1;
            }
            PlayerData.instance.CalculateNotchesUsed();
        }

        //From API
        internal void SetIntSwappedArgs(int value, string name)
        {
            PlayerData.instance.SetInt(name, value);
        }


        //Replaces soul hits
        private int SoulGain(int amount)
        {
            //Mantis gets no soul
            if (PlayerData.instance.GetBool("equippedCharm_43"))
            {
                if (PlayerData.instance.GetBool("equippedCharm_6"))
                {
                    mantisHits++;
                    mantisHits++;
                }
                else
                {
                    mantisHits++;
                }
                if (mantisHits >= 10)
                {
                    HeroController.instance.AddHealth(1);
                    mantisHits = 0;
                }
                return 0;
            }
            //Bee soul gain, 50% extra if in air.
            else if (PlayerData.instance.GetBool("equippedCharm_44") && ((HeroController.instance.GetState("jumping") || HeroController.instance.GetState("falling"))))
            {
                if (beeCombo < 12)
                {
                    beeCombo++;
                }
                return amount;
            }
            //Refreshes baldur shell ever 3 hits, if under 4.
            if (PlayerData.instance.GetBool("equippedCharm_42") && PlayerData.instance.blockerHits < 4)
            {
                beetleCombo++;
                if (beetleCombo >= 3)
                {
                    PlayerData.instance.blockerHits++;
                }
                return amount;
            }          
            
            return amount;
            
        }
        //Counts how many of the charms are equipped.
        public void CountCharms()
        {
            if (PlayerData.instance.GetBool("equippedCharm_41"))
            {
                snailEquipped = 1;
            }
            else
            {
                snailEquipped = 0;
            }
            if (PlayerData.instance.GetBool("equippedCharm_42"))
            {
                beetleEquipped = 1;
            }
            else
            {
                beetleEquipped = 0;
            }
            if (PlayerData.instance.GetBool("equippedCharm_43"))
            {
                mantisEquipped = 1;
            }
            else
            {
                mantisEquipped = 0;
            }
            if (PlayerData.instance.GetBool("equippedCharm_44"))
            {
                beeEquipped = 1;
            }
            else
            {
                beeEquipped = 0;
            }
            if (PlayerData.instance.GetBool("equippedCharm_45"))
            {
                mothEquipped = 1;
            }
            else
            {
                mothEquipped = 0;
            }
            if (PlayerData.instance.GetBool("equippedCharm_46"))
            {
                spiderEquipped = 1;
            }
            else
            {
                spiderEquipped = 0;
            }
            charmsEquipped = snailEquipped + beetleEquipped + mantisEquipped + beeEquipped + mothEquipped + spiderEquipped;
            if (charmsEquipped > 1)
            {
                PlayerData.instance.overcharmed = true;
            }
        }

    }
}
