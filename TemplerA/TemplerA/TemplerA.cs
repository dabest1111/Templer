﻿

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;

    using SharpDX;

namespace TemplerA
{

    internal class Program
   { 


        private static Ability Refraction, Meld, Trap, ptrap;
        private static Item blink, bkb, phase, hex;
        private static readonly Menu Menu = new Menu("TemplerA", "templera", true, "npc_dota_hero_Templar_Assassin", true);
        private static Hero me, target;
        private static bool combo, psi;
        private static AbilityToggler menuValue;

        private static bool menuvalueSet;


        static void Main(string[] args)
        {
            Game.OnUpdate += Game_OnUpdate;
            Game.OnWndProc += Game_OnWndProc;
            var menu_zuena = new Menu("Options", "opsi");            
            menu_zuena.AddItem(new MenuItem("enable", "enable").SetValue(true));                       
            Menu.AddSubMenu(menu_zuena);
            menu_zuena.AddItem(new MenuItem("comboKey", "Combo Key").SetValue(new KeyBind(32, KeyBindType.Press)));
            Menu.AddToMainMenu();
            var dict = new Dictionary<string, bool>
            {
               {"item_black_king_bar", true }, { "item_sheepstick", true }, {"item_phase_boots", true }, {"item_blink",true}
            };
            Menu.AddItem(
                new MenuItem("Items", "Items:").SetValue(new AbilityToggler(dict)));
        }


        public static void Game_OnUpdate(EventArgs args)
        {
            me = ObjectMgr.LocalHero;

            if (!Game.IsInGame || Game.IsPaused || Game.IsWatchingGame)
                return;

            if (me.ClassID != ClassID.CDOTA_Unit_Hero_TemplarAssassin)
                return;

            if (me == null)
                return;

            if (Refraction == null)
                Refraction = me.Spellbook.Spell1;

            if (Meld == null)
                Meld = me.Spellbook.Spell2;

            if (Trap == null)
                Trap = me.Spellbook.SpellD;

            if (ptrap == null)
                ptrap = me.Spellbook.SpellR;

            if (blink == null)
                blink = me.FindItem("item_blink");

            if (bkb == null)
                bkb = me.FindItem("item_black_king_bar");

            if (phase == null)
                phase = me.FindItem("item_phase_boots");

            if (hex == null)
                hex = me.FindItem("item_sheepstick");

            if (!menuvalueSet)
            {
                menuValue = Menu.Item("Items").GetValue<AbilityToggler>();
                menuvalueSet = true;
            }


           



            if (combo && Menu.Item("enable").GetValue<bool>())
            {
                target = me.ClosestToMouseTarget(1000);
                
                if (target != null && target.IsAlive && !target.IsInvul() && !target.IsIllusion)
                {

                    var attackrange = 190 + (60 * me.Spellbook.Spell3.Level);                    
                    if (me.CanAttack() && me.CanCast())
                    {

                    

                        var traps = ObjectMgr.GetEntities<Unit>().Where(Unit => Unit.Name == "npc_dota_templar_assassin_psionic_trap").ToList();
                        foreach (var q in traps)
                        {
                            if (target.Position.Distance2D(q.Position) < 370 && q.Spellbook.SpellQ.CanBeCasted() && Utils.SleepCheck("traps") && !target.Modifiers.ToList().Exists(x => x.Name == "modifier_templar_assassin_trap_slow"))
                            {
                                q.Spellbook.SpellQ.UseAbility();
                                Utils.Sleep(150 + Game.Ping, "traps");
                            }
                        }
                        
                        if (ptrap.CanBeCasted() && Utils.SleepCheck ("ptrap") && Utils.SleepCheck("traps") && !target.Modifiers.ToList().Exists(x => x.Name == "modifier_templar_assassin_trap_slow"))
                        {
                            ptrap.UseAbility(target.Position);
                            Utils.Sleep(150 + Game.Ping, "ptrap");
                        }

                                                                                               
                        if (Refraction.CanBeCasted() && Utils.SleepCheck("Refraction"))
                        {
                            Refraction.UseAbility();
                            Utils.Sleep(150 + Game.Ping, "Refraction");
                        }

                        if (bkb != null && bkb.CanBeCasted() && Utils.SleepCheck("bkb") &&  menuValue.IsEnabled(bkb.Name) && me.Distance2D(target) <= 620)
                        {
                            bkb.UseAbility();
                            Utils.Sleep(150 + Game.Ping, "bkb");
                        }

                        if (phase != null && phase.CanBeCasted() && Utils.SleepCheck("phase") && menuValue.IsEnabled(phase.Name) && !blink.CanBeCasted() && me.Distance2D(target) >= attackrange)
                        {
                            phase.UseAbility();
                            Utils.Sleep(150 + Game.Ping, "phase");
                        }


                        if (blink != null && blink.CanBeCasted() && menuValue.IsEnabled(blink.Name) &&  me.Distance2D(target) > 500 && me.Distance2D(target) <= 1170 && Utils.SleepCheck("blink"))
                        {
                            blink.UseAbility(target.Position);
                            Utils.Sleep(250 + Game.Ping, "blink");
                        }

                        if (hex != null && hex.CanBeCasted() && menuValue.IsEnabled(hex.Name) && Utils.SleepCheck("hex"))
                        {
                            hex.UseAbility(target);
                            Utils.Sleep(150 + Game.Ping, "hex");
                        }


                        if (me.Distance2D(target) <= attackrange && Meld.CanBeCasted() && Utils.SleepCheck("Meld"))
                        {
                            Meld.UseAbility();
                            Utils.Sleep(250 + Game.Ping, "Meld");
                        }

                                
                        if (!Meld.CanBeCasted() && Utils.SleepCheck("attack2") && me.Distance2D(target) <= attackrange)
                        {
                            me.Attack(target);
                            Utils.Sleep(Game.Ping + 150, "attack2");
                        }

                        if (!me.IsAttacking() && me.Distance2D(target) >= attackrange && Utils.SleepCheck("follow"))
                        {
                            me.Move(Game.MousePosition);
                            Utils.Sleep(150 + Game.Ping, "follow");
                        }
                    }

                    else if (Utils.SleepCheck("attack1"))
                    {
                        me.Attack(target);
                        Utils.Sleep(150, "attack1");
                    }

                }
                else
                {
                    me.Move(Game.MousePosition);
                }
            }
        }

        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (!Game.IsChatOpen)
            {
                if (Menu.Item("comboKey").GetValue<KeyBind>().Active)
                {                 
                    combo = true;
                }
                else
                {
                    combo = false;
                }

                

            }

        }
    }
}