﻿using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Barotrauma.Networking
{
    partial class GameServer : NetworkMember
    {
        private NetStats netStats;



        //private GUITextBlock CurrentAction;

        int buttonpage;
        int MaxButtonPages;

        public GUIFrame ClickCommandFrame;
        public GUITextBlock ClickCommandDescription;

        private Boolean IsButtonPageHidden;
        private GUIButton LeftButton;
        private GUIButton HidePageButton;
        private GUIButton RightButton;
        private GUIButton showLogButton;

        private List<List<GUIButton>> PageButtons;

        //private GUIButton Return;

        #region Button Definitions

        //Page 1
        public GUIButton NilModReload;
        public GUIButton settingsButton;
        public GUIButton ShowLagDiagnosticsButton;
        public GUIButton ShowNetStatsButton;
        public GUIButton endRoundButton;

        //Page 2
        public GUIButton ToggleRenderStructureButton;
        public GUIButton ToggleRenderOtherButton;
        public GUIButton ToggleRenderLevelButton;
        public GUIButton ToggleRenderCharacterButton;
        public GUIButton ToggleAITargetsButton;
        public GUIButton ToggleDebugDrawButton;
        public GUIButton ToggleLightsButton;
        public GUIButton ToggleLosButton;
        public GUIButton ToggleFollowSubButton;
        public GUIButton ToggleDeathChat;

        //Page 3
        public GUIButton ForceShuttleButton;
        public GUIButton RecallShuttleButton;
        public GUIButton FreezeButton;
        public GUIButton RelocateButton;
        public GUIButton DetachFromBodyButton;
        public GUIButton ReturnToBodyButton;
        public GUIButton ControlButton;
        public GUIButton ShieldButton;
        public GUIButton KillMonstersButtons;
        public GUIButton KillCreatureButton;
        public GUIButton SpawnCreatureButton;
        public GUIButton ToggleCrewAIButton;
        public GUIButton RemoveCorpseButton;
        public GUIButton ClearCorpseButton;
        public GUIButton ReviveButton;
        public GUIButton HealButton;

        //Page 4
        public GUIButton ToggleCrushButton;
        public GUIButton ToggleGodmodeButton;
        public GUIButton LockSubXButton;
        public GUIButton LockSubYButton;
        public GUIButton TeleportTeam1SubButton;
        public GUIButton TeleportTeam2SubButton;
        public GUIButton RechargePowerTeam1Button;
        public GUIButton RechargePowerTeam2Button;
        public GUIButton FixWallsButton;
        public GUIButton FixItemsButton;
        public GUIButton FiresButton;
        public GUIButton WaterButton;
        public GUIButton OxygenButton;

        #endregion

        private GUIScrollBar clientListScrollBar;

        private GUIScrollBar ProfilingListScrollBar;

        void InitProjSpecific()
        {
            //----------------------------------------
            buttonpage = 1;
            MaxButtonPages = 4;
            PageButtons = new List<List<GUIButton>>();

            ClickCommandFrame = new GUIFrame(
                new Rectangle((int)((GameMain.GraphicsWidth) * 0.3f), (int)(GameMain.GraphicsHeight * 0.00f),
                    (int)(GameMain.GraphicsWidth * 0.40f), (int)(150)),
                "", inGameHUD);

            ClickCommandDescription = new GUITextBlock(new Rectangle(0, -20, (int)(GameMain.GraphicsWidth * 0.35f), 120), "ACTIONAME - STATS OF ACTION - DESCRIPTION OF THE ACTION GOES HERE WHICH LASTS FOR BLOODY AGES ETC ETC ETC ETC ETC XD", "", ClickCommandFrame, true);

            ClickCommandFrame.Visible = false;

            SetupButtons();

            //----------------------------------------
        }

        public void SetupButtons()
        {
            List<GUIButton> tempbuttonpagelist;
            Vector2 ButtonCoord;

            #region MainButtons

            showLogButton = new GUIButton(new Rectangle(GameMain.GraphicsWidth - 70 - 140 - 70, 40, 140 + 130, 20), "Server Log", Alignment.TopLeft, "", inGameHUD);
            showLogButton.ToolTip = "Shows the game log.";
            showLogButton.Visible = true;
            showLogButton.Enabled = true;
            showLogButton.OnClicked = (GUIButton button, object userData) =>
            {
                if (log.LogFrame == null)
                {
                    log.CreateLogFrame();
                }
                else
                {
                    log.LogFrame = null;
                    GUIComponent.KeyboardDispatcher.Subscriber = null;
                }
                return true;
            };

            //LeftButton
            LeftButton = new GUIButton(new Rectangle(GameMain.GraphicsWidth - 70 - 140 - 70, 10, 40, 20), "<-", Alignment.TopLeft, "", inGameHUD);
            LeftButton.ToolTip = "Change button page.";
            LeftButton.Visible = true;
            LeftButton.Enabled = true;
            LeftButton.OnClicked = (GUIButton button, object userData) =>
            {
                SetButtonPage(-1);
                return true;
            };

            //HidePageButton
            HidePageButton = new GUIButton(new Rectangle(GameMain.GraphicsWidth - 147 - 70, 10, 147, 20), "Hide", Alignment.TopLeft, "", inGameHUD);
            HidePageButton.ToolTip = "Hides the buttons.";
            HidePageButton.Visible = true;
            HidePageButton.Enabled = true;
            HidePageButton.OnClicked = (GUIButton button, object userData) =>
            {
                if (IsButtonPageHidden)
                {
                    HidePageButton.ToolTip = "Hides the buttons.";
                    HidePageButton.Text = "Hide Page";
                    IsButtonPageHidden = false;
                }
                else
                {
                    HidePageButton.ToolTip = "Shows the buttons.";
                    HidePageButton.Text = "Show Page";
                    IsButtonPageHidden = true;
                }

                UpdateButtonPage();
                return true;
            };

            //RightButton
            RightButton = new GUIButton(new Rectangle(GameMain.GraphicsWidth - 50, 10, 40, 20), "->", Alignment.TopLeft, "", inGameHUD);
            RightButton.ToolTip = "Change button page.";
            RightButton.Visible = true;
            RightButton.Enabled = true;
            RightButton.OnClicked = (GUIButton button, object userData) =>
            {
                SetButtonPage(1);
                return true;
            };

            #endregion

            #region Button Page 1

            //Y Start      70
            //X Start     140
            //X size      130
            //Y size       20
            //X Spacing   140 (130+10)
            //Y Spacing    30 (20+10)

            tempbuttonpagelist = new List<GUIButton>();

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            //EndRoundButton
            endRoundButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "End round", Alignment.TopLeft, "", inGameHUD);
            endRoundButton.ToolTip = "Immediately ends the game round.";
            endRoundButton.Visible = false;
            endRoundButton.Enabled = true;
            endRoundButton.OnClicked = (btn, userdata) => {
                GameMain.NilMod.RoundEnded = true;
                EndGame(); return true;
            };
            tempbuttonpagelist.Add(endRoundButton);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            //Settingsbutton
            settingsButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "Settings", Alignment.TopLeft, "", inGameHUD);
            settingsButton.ToolTip = "Shows the server settings screen.";
            settingsButton.Visible = false;
            settingsButton.Enabled = true;
            settingsButton.OnClicked = ToggleSettingsFrame;
            settingsButton.UserData = "settingsButton";
            tempbuttonpagelist.Add(settingsButton);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            //LagDiagnosticsButton
            ShowLagDiagnosticsButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "LagProfiler: On", Alignment.TopLeft, "", inGameHUD);
            ShowLagDiagnosticsButton.ToolTip = "Turns on the lag profiling information.";
            ShowLagDiagnosticsButton.Visible = false;
            ShowLagDiagnosticsButton.Enabled = true;
            ShowLagDiagnosticsButton.OnClicked = (GUIButton button, object userData) =>
            {
                DebugConsole.ExecuteCommand("lagprofiler", true);
                return true;
            };
            tempbuttonpagelist.Add(ShowLagDiagnosticsButton);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            //NetStatsButton
            ShowNetStatsButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "NetStats: On", Alignment.TopLeft, "", inGameHUD);
            ShowNetStatsButton.ToolTip = "Turns on the netstats screen which shows the latencies and IP Addresses of connections.";
            ShowNetStatsButton.Visible = false;
            ShowNetStatsButton.Enabled = true;
            ShowNetStatsButton.OnClicked = (GUIButton button, object userData) =>
            {
                DebugConsole.ExecuteCommand("netstats", true);
                return true;
            };
            tempbuttonpagelist.Add(ShowNetStatsButton);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            NilModReload = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "Reload Nilmod Config", Alignment.TopLeft, "", inGameHUD);
            NilModReload.ToolTip = "Reloads all NilMod XMLs during round runtime and retroactively applies them.";
            NilModReload.Visible = false;
            NilModReload.Enabled = true;
            NilModReload.OnClicked = (GUIButton button, object userData) =>
            {
                DebugConsole.ExecuteCommand("nilmodreload", true);
                return true;
            };
            tempbuttonpagelist.Add(NilModReload);

            PageButtons.Add(tempbuttonpagelist);

            #endregion

            #region Button Page 2

            //Y Start      70
            //X Start     140
            //X size      130
            //Y size       20
            //X Spacing   140 (130+10)
            //Y Spacing    30 (20+10)

            tempbuttonpagelist = new List<GUIButton>();

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            ToggleRenderStructureButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "RenderStruct: Off", Alignment.TopLeft, "", inGameHUD);
            ToggleRenderStructureButton.ToolTip = "Turns off rendering of submarine items and background walls, level will still show outer walls, doors, ladders and perhaps stairs.";
            ToggleRenderStructureButton.Visible = false;
            ToggleRenderStructureButton.Enabled = true;
            ToggleRenderStructureButton.OnClicked = (GUIButton button, object userData) =>
            {
                DebugConsole.ExecuteCommand("renderstructure", true);
                return true;
            };
            tempbuttonpagelist.Add(ToggleRenderStructureButton);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            ToggleRenderOtherButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "RenderOther: Off", Alignment.TopLeft, "", inGameHUD);
            ToggleRenderOtherButton.ToolTip = "Turns off particle, lighting, los and other miscellanous rendering.";
            ToggleRenderOtherButton.Visible = false;
            ToggleRenderOtherButton.Enabled = true;
            ToggleRenderOtherButton.OnClicked = (GUIButton button, object userData) =>
            {
                DebugConsole.ExecuteCommand("renderother", true);
                return true;
            };
            tempbuttonpagelist.Add(ToggleRenderOtherButton);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            ToggleRenderLevelButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "RenderLevel: Off", Alignment.TopLeft, "", inGameHUD);
            ToggleRenderLevelButton.ToolTip = "Turns off level rendering, structure rendering will still render the submarine walls unless both are off.";
            ToggleRenderLevelButton.Visible = false;
            ToggleRenderLevelButton.Enabled = true;
            ToggleRenderLevelButton.OnClicked = (GUIButton button, object userData) =>
            {
                DebugConsole.ExecuteCommand("renderlevel", true);
                return true;
            };
            tempbuttonpagelist.Add(ToggleRenderLevelButton);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            ToggleRenderCharacterButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "RenderChar: Off", Alignment.TopLeft, "", inGameHUD);
            ToggleRenderCharacterButton.ToolTip = "Turns off character rendering.";
            ToggleRenderCharacterButton.Visible = false;
            ToggleRenderCharacterButton.Enabled = true;
            ToggleRenderCharacterButton.OnClicked = (GUIButton button, object userData) =>
            {
                DebugConsole.ExecuteCommand("rendercharacter", true);
                return true;
            };
            tempbuttonpagelist.Add(ToggleRenderCharacterButton);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            ToggleAITargetsButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "AITargets: On", Alignment.TopLeft, "", inGameHUD);
            ToggleAITargetsButton.ToolTip = "Turns on AI Targetting range information for Debugdraw mode.";
            ToggleAITargetsButton.Visible = false;
            ToggleAITargetsButton.Enabled = true;
            ToggleAITargetsButton.OnClicked = (GUIButton button, object userData) =>
            {
                DebugConsole.ExecuteCommand("aitargets", true);
                return true;
            };
            tempbuttonpagelist.Add(ToggleAITargetsButton);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            ToggleDebugDrawButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "DebugDraw: On", Alignment.TopLeft, "", inGameHUD);
            ToggleDebugDrawButton.ToolTip = "Turns on debugdraw view information.";
            ToggleDebugDrawButton.Visible = false;
            ToggleDebugDrawButton.Enabled = true;
            ToggleDebugDrawButton.OnClicked = (GUIButton button, object userData) =>
            {
                DebugConsole.ExecuteCommand("debugdraw", true);
                return true;
            };
            tempbuttonpagelist.Add(ToggleDebugDrawButton);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            ToggleLightsButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "Toggle Lights", Alignment.TopLeft, "", inGameHUD);
            ToggleLightsButton.ToolTip = "Toggles game lighting.";
            ToggleLightsButton.Visible = false;
            ToggleLightsButton.Enabled = true;
            ToggleLightsButton.OnClicked = (GUIButton button, object userData) =>
            {
                DebugConsole.ExecuteCommand("lights", true);
                return true;
            };
            tempbuttonpagelist.Add(ToggleLightsButton);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            ToggleLosButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "Toggle Los", Alignment.TopLeft, "", inGameHUD);
            ToggleLosButton.ToolTip = "Toggles line of sight.";
            ToggleLosButton.Visible = false;
            ToggleLosButton.Enabled = true;
            ToggleLosButton.OnClicked = (GUIButton button, object userData) =>
            {
                DebugConsole.ExecuteCommand("los", true);
                return true;
            };
            tempbuttonpagelist.Add(ToggleLosButton);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            ToggleFollowSubButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "FollowSub: Off", Alignment.TopLeft, "", inGameHUD);
            ToggleFollowSubButton.ToolTip = "Stops the camera automatically following submarines.";
            ToggleFollowSubButton.Visible = false;
            ToggleFollowSubButton.Enabled = true;
            ToggleFollowSubButton.OnClicked = (GUIButton button, object userData) =>
            {
                DebugConsole.ExecuteCommand("followsub", true);
                return true;
            };
            tempbuttonpagelist.Add(ToggleFollowSubButton);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            ToggleDeathChat = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "DeathChat: Off", Alignment.TopLeft, "", inGameHUD);
            ToggleDeathChat.ToolTip = "Toggles visibility of the deathchat if you have a living character.";
            ToggleDeathChat.Visible = false;
            ToggleDeathChat.Enabled = true;
            ToggleDeathChat.OnClicked = (GUIButton button, object userData) =>
            {
                DebugConsole.ExecuteCommand("deathchat", true);
                return true;
            };
            tempbuttonpagelist.Add(ToggleDeathChat);

            PageButtons.Add(tempbuttonpagelist);

            #endregion

            #region Button Page 3

            //Y Start      70
            //X Start     140
            //X size      130
            //Y size       20
            //X Spacing   140 (130+10)
            //Y Spacing    30 (20+10)

            tempbuttonpagelist = new List<GUIButton>();

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            RecallShuttleButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "Recall Shuttle", Alignment.TopLeft, "", inGameHUD);
            RecallShuttleButton.ToolTip = "Immediately recalls the shuttle, killing anything on board and resetting the timers.";
            RecallShuttleButton.Visible = false;
            RecallShuttleButton.Enabled = true;
            RecallShuttleButton.OnClicked = (GUIButton button, object userData) =>
            {
                DebugConsole.ExecuteCommand("recallshuttle", true);
                return true;
            };
            tempbuttonpagelist.Add(RecallShuttleButton);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            ForceShuttleButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "Force Shuttle", Alignment.TopLeft, "", inGameHUD);
            ForceShuttleButton.ToolTip = "Immediately recalls and forces out the shuttle, respawning anyone spectating.";
            ForceShuttleButton.Visible = false;
            ForceShuttleButton.Enabled = true;
            ForceShuttleButton.OnClicked = (GUIButton button, object userData) =>
            {
                DebugConsole.ExecuteCommand("forceshuttle", true);
                return true;
            };
            tempbuttonpagelist.Add(ForceShuttleButton);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            FreezeButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "Freeze Characters", Alignment.TopLeft, "", inGameHUD);
            FreezeButton.ToolTip = "Left click a player to freeze their movements - Left click again to unfreeze - hold only shift to repeat - hold ctrl shift and left click to freeze all - hold ctrl and left click to unfreeze everyone - Right click to cancel - Players may still talk if concious.";
            FreezeButton.Visible = false;
            FreezeButton.Enabled = true;
            FreezeButton.OnClicked = (GUIButton button, object userData) =>
            {
                GameMain.NilMod.ActiveClickCommand = true;
                GameMain.NilMod.ClickCommandType = "freeze";
                GameMain.NilMod.ClickCooldown = 0.5f;
                ClickCommandFrame.Visible = true;
                ClickCommandDescription.Text = "FREEZE - Left click a player to freeze their movements - Left click again to unfreeze - hold only shift to repeat - hold ctrl shift and left click to freeze all - hold ctrl and left click to unfreeze everyone - Right click to cancel - Players may still talk if concious.";
                return true;
            };
            tempbuttonpagelist.Add(FreezeButton);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            RelocateButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "Relocate Creature", Alignment.TopLeft, "", inGameHUD);
            RelocateButton.ToolTip = "Left Click to select target to teleport, Left click again to teleport target to new destination, hold shift to repeat (Does not keep last target), Ctrl+Left Click to relocate self, Ctrl+Shift works, Right click to cancel.";
            RelocateButton.Visible = false;
            RelocateButton.Enabled = true;
            RelocateButton.OnClicked = (GUIButton button, object userData) =>
            {
                GameMain.NilMod.ActiveClickCommand = true;
                GameMain.NilMod.ClickCommandType = "relocate";
                GameMain.NilMod.ClickCooldown = 0.5f;
                ClickCommandFrame.Visible = true;
                ClickCommandDescription.Text = "RELOCATE - None Selected - Left Click to select target to teleport, Left click again to teleport target to new destination, hold shift to repeat (Does not keep last target), Ctrl+Left Click to relocate self, Ctrl+Shift works, Right click to cancel.";
                return true;
            };
            tempbuttonpagelist.Add(RelocateButton);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            DetachFromBodyButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "Detach Body", Alignment.TopLeft, "", inGameHUD);
            DetachFromBodyButton.ToolTip = "Freecams you away from your character and drops control.";
            DetachFromBodyButton.Visible = false;
            DetachFromBodyButton.Enabled = true;
            DetachFromBodyButton.OnClicked = (GUIButton button, object userData) =>
            {
                DebugConsole.ExecuteCommand("freecam", true);
                return true;
            };
            tempbuttonpagelist.Add(DetachFromBodyButton);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            ReturnToBodyButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "Return to body.", Alignment.TopLeft, "", inGameHUD);
            ReturnToBodyButton.ToolTip = "Sets control back to the last character you controlled.";
            ReturnToBodyButton.Visible = false;
            ReturnToBodyButton.Enabled = true;
            ReturnToBodyButton.OnClicked = (GUIButton button, object userData) =>
            {
                if (Character.LastControlled != null) Character.Controlled = Character.LastControlled;
                return true;
            };
            tempbuttonpagelist.Add(ReturnToBodyButton);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            ShieldButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "Shield", Alignment.TopLeft, "", inGameHUD);
            ShieldButton.ToolTip = "Grants immunity to pressure, health loss and oxygen loss, it will not revive or heal them - but they may still increase in health/oxygen and huskify.";
            ShieldButton.Visible = false;
            ShieldButton.Enabled = true;
            ShieldButton.OnClicked = (GUIButton button, object userData) =>
            {
                GameMain.NilMod.ActiveClickCommand = true;
                GameMain.NilMod.ClickCommandType = "shield";
                GameMain.NilMod.ClickCooldown = 0.5f;
                ClickCommandFrame.Visible = true;
                ClickCommandDescription.Text = "SHIELD - Left Click close to a creatures center to grant immunity, Hold control to revoke immunity, Hold shift while clicking to repeat, right click to cancel.";
                return true;
            };
            tempbuttonpagelist.Add(ShieldButton);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            ControlButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "Control", Alignment.TopLeft, "", inGameHUD);
            ControlButton.ToolTip = "Turns off rendering of submarine items and background walls, level will still show outer walls, doors, ladders and perhaps stairs.";
            ControlButton.Visible = false;
            ControlButton.Enabled = true;
            ControlButton.OnClicked = (GUIButton button, object userData) =>
            {
                GameMain.NilMod.ActiveClickCommand = true;
                GameMain.NilMod.ClickCommandType = "control";
                GameMain.NilMod.ClickCooldown = 0.5f;
                ClickCommandFrame.Visible = true;
                ClickCommandDescription.Text = "CONTROL - Left Click close to a creatures center to instantaniously kill it, Hold shift while clicking to repeat, right click to cancel.";
                return true;
            };
            tempbuttonpagelist.Add(ControlButton);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            KillMonstersButtons = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "Kill Monsters", Alignment.TopLeft, "", inGameHUD);
            KillMonstersButtons.ToolTip = "Instantly kills every enemy creature on the map.";
            KillMonstersButtons.Visible = false;
            KillMonstersButtons.Enabled = true;
            KillMonstersButtons.OnClicked = (GUIButton button, object userData) =>
            {
                DebugConsole.ExecuteCommand("killmonsters", true);
                return true;
            };
            tempbuttonpagelist.Add(KillMonstersButtons);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            KillCreatureButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "Kill Creature", Alignment.TopLeft, "", inGameHUD);
            KillCreatureButton.ToolTip = "Left Click close to a creatures center to instantaniously kill it, Hold shift while clicking to repeat, right click to cancel.";
            KillCreatureButton.Visible = false;
            KillCreatureButton.Enabled = true;
            KillCreatureButton.OnClicked = (GUIButton button, object userData) =>
            {
                GameMain.NilMod.ActiveClickCommand = true;
                GameMain.NilMod.ClickCommandType = "kill";
                GameMain.NilMod.ClickCooldown = 0.5f;
                ClickCommandFrame.Visible = true;
                ClickCommandDescription.Text = "KILL CREATURE - Left Click close to a creatures center to instantaniously kill it, Hold shift while clicking to repeat, right click to cancel.";
                return true;
            };
            tempbuttonpagelist.Add(KillCreatureButton);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            ToggleCrewAIButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "CrewAI: Off", Alignment.TopLeft, "", inGameHUD);
            ToggleCrewAIButton.ToolTip = "Turns the AI Crews AI Off.";
            ToggleCrewAIButton.Visible = false;
            ToggleCrewAIButton.Enabled = true;
            ToggleCrewAIButton.OnClicked = (GUIButton button, object userData) =>
            {
                if (HumanAIController.DisableCrewAI)
                {
                    DebugConsole.ExecuteCommand("enablecrewai", true);
                }
                else
                {
                    DebugConsole.ExecuteCommand("disablecrewai", true);
                }
                return true;
            };
            tempbuttonpagelist.Add(ToggleCrewAIButton);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            SpawnCreatureButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "Spawn Creature", Alignment.TopLeft, "", inGameHUD);
            SpawnCreatureButton.ToolTip = "Opens the menu to spawn creatures.";
            SpawnCreatureButton.Visible = false;
            SpawnCreatureButton.Enabled = true;
            SpawnCreatureButton.OnClicked = (GUIButton button, object userData) =>
            {
                SpawnCreaturePrompt();
                return true;
            };
            tempbuttonpagelist.Add(SpawnCreatureButton);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            ClearCorpseButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "Clear Corpses", Alignment.TopLeft, "", inGameHUD);
            ClearCorpseButton.ToolTip = "Removes every single none-netplayer corpse from the map instantly, hold shift to remove netplayer corpses as well.";
            ClearCorpseButton.Visible = false;
            ClearCorpseButton.Enabled = true;
            ClearCorpseButton.OnClicked = (GUIButton button, object userData) =>
            {
                if (PlayerInput.KeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                {
                    GameMain.Server.RemoveCorpses(true);
                }
                else
                {
                    GameMain.Server.RemoveCorpses(false);
                }
                return true;
            };
            tempbuttonpagelist.Add(ClearCorpseButton);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            RemoveCorpseButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "Remove Corpse", Alignment.TopLeft, "", inGameHUD);
            RemoveCorpseButton.ToolTip = "Left Click close to a creatures corpse to delete it, Hold shift while clicking to repeat, right click to cancel.";
            RemoveCorpseButton.Visible = false;
            RemoveCorpseButton.Enabled = true;
            RemoveCorpseButton.OnClicked = (GUIButton button, object userData) =>
            {
                GameMain.NilMod.ActiveClickCommand = true;
                GameMain.NilMod.ClickCommandType = "removecorpse";
                GameMain.NilMod.ClickCooldown = 0.5f;
                ClickCommandFrame.Visible = true;
                ClickCommandDescription.Text = "REMOVECORPSE - Left Click close to a creatures corpse to delete it, Hold shift while clicking to repeat, right click to cancel.";
                return true;
            };
            tempbuttonpagelist.Add(RemoveCorpseButton);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            ReviveButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "Revive Character", Alignment.TopLeft, "", inGameHUD);
            ReviveButton.ToolTip = "Left Click close to a creatures center to revive it, Hold shift while clicking to repeat, Hold ctrl when clicking the button to revive self, if detached from body ctrl click corpse to revive+control, right click to cancel.";
            ReviveButton.Visible = false;
            ReviveButton.Enabled = true;
            ReviveButton.OnClicked = (GUIButton button, object userData) =>
            {
                if (PlayerInput.KeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl) && Character.Controlled != null)
                {
                    DebugConsole.ExecuteCommand("revive", true);
                }
                else
                {
                    GameMain.NilMod.ActiveClickCommand = true;
                    GameMain.NilMod.ClickCommandType = "revive";
                    GameMain.NilMod.ClickCooldown = 0.5f;
                    ClickCommandFrame.Visible = true;
                    ClickCommandDescription.Text = "REVIVE - Left Click close to a creatures center to revive it, Hold shift while clicking to repeat, Hold ctrl when clicking the button to revive self, if detached from body ctrl click corpse to revive+control, right click to cancel. - As a note for now IF REVIVING A PLAYER you will wish to open the console (F3) and type setclientcharacter CapitalizedClientName ; clientcharacter to give them the body back.";
                }
                return true;
            };
            tempbuttonpagelist.Add(ReviveButton);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            HealButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "Heal Character", Alignment.TopLeft, "", inGameHUD);
            HealButton.ToolTip = "Left Click close to a creatures center to heal it, Hold shift while clicking to repeat, Hold ctrl when clicking the button to heal self, right click to cancel.";
            HealButton.Visible = false;
            HealButton.Enabled = true;
            HealButton.OnClicked = (GUIButton button, object userData) =>
            {
                if (PlayerInput.KeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl) && Character.Controlled != null)
                {
                    DebugConsole.ExecuteCommand("heal", true);
                }
                else
                {
                    GameMain.NilMod.ActiveClickCommand = true;
                    GameMain.NilMod.ClickCommandType = "heal";
                    GameMain.NilMod.ClickCooldown = 0.5f;
                    ClickCommandFrame.Visible = true;
                    ClickCommandDescription.Text = "HEAL - Left Click close to a creatures center to heal it, Hold shift while clicking to repeat, Hold ctrl when clicking to heal self, right click to cancel.";
                }
                return true;
            };
            tempbuttonpagelist.Add(HealButton);

            PageButtons.Add(tempbuttonpagelist);

            #endregion

            #region Button Page 4

            //Y Start      70
            //X Start     140
            //X size      130
            //Y size       20
            //X Spacing   140 (130+10)
            //Y Spacing    30 (20+10)

            tempbuttonpagelist = new List<GUIButton>();

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            ToggleCrushButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "DepthCrush: Off", Alignment.TopLeft, "", inGameHUD);
            ToggleCrushButton.ToolTip = "Turns off Abyss Crushing damage to submarines.";
            ToggleCrushButton.Visible = false;
            ToggleCrushButton.Enabled = true;
            ToggleCrushButton.OnClicked = (GUIButton button, object userData) =>
            {
                DebugConsole.ExecuteCommand("togglecrush", true);
                return true;
            };
            tempbuttonpagelist.Add(ToggleCrushButton);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            ToggleGodmodeButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "GodMode: On", Alignment.TopLeft, "", inGameHUD);
            ToggleGodmodeButton.ToolTip = "Turns on godmode which stops submarine damage.";
            ToggleGodmodeButton.Visible = false;
            ToggleGodmodeButton.Enabled = true;
            ToggleGodmodeButton.OnClicked = (GUIButton button, object userData) =>
            {
                DebugConsole.ExecuteCommand("godmode", true);
                return true;
            };
            tempbuttonpagelist.Add(ToggleGodmodeButton);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            LockSubXButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "Lock Sub X", Alignment.TopLeft, "", inGameHUD);
            LockSubXButton.ToolTip = "Prevents any submarine/shuttle from moving Left/Right.";
            LockSubXButton.Visible = false;
            LockSubXButton.Enabled = true;
            LockSubXButton.OnClicked = (GUIButton button, object userData) =>
            {
                DebugConsole.ExecuteCommand("lockx", true);
                return true;
            };
            tempbuttonpagelist.Add(LockSubXButton);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            LockSubYButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "Lock Sub Y", Alignment.TopLeft, "", inGameHUD);
            LockSubYButton.ToolTip = "Prevents any submarine/shuttle from moving Up/Down.";
            LockSubYButton.Visible = false;
            LockSubYButton.Enabled = true;
            LockSubYButton.OnClicked = (GUIButton button, object userData) =>
            {
                DebugConsole.ExecuteCommand("locky", true);
                return true;
            };
            tempbuttonpagelist.Add(LockSubYButton);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            TeleportTeam2SubButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "Warp team 2 sub", Alignment.TopLeft, "", inGameHUD);
            TeleportTeam2SubButton.ToolTip = "Teleports the second teams submarine, click then click in-world to teleport. Right click to cancel action.";
            TeleportTeam2SubButton.Visible = false;
            TeleportTeam2SubButton.Enabled = true;
            TeleportTeam2SubButton.OnClicked = (GUIButton button, object userData) =>
            {
                GameMain.NilMod.ActiveClickCommand = true;
                GameMain.NilMod.ClickCommandType = "teleportsub";
                GameMain.NilMod.ClickArgs = new string[] { "1" };
                GameMain.NilMod.ClickCooldown = 0.5f;
                ClickCommandFrame.Visible = true;
                ClickCommandDescription.Text = "TELEPORTSUB - Team 1's submarine - Teleports the chosen teams submarine, left click to teleport. Right click to cancel.";
                return true;
            };
            tempbuttonpagelist.Add(TeleportTeam2SubButton);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            TeleportTeam1SubButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "Warp Team 1 sub", Alignment.TopLeft, "", inGameHUD);
            TeleportTeam1SubButton.ToolTip = "Teleports the first teams submarine, click then click in-world to teleport. Right click to cancel action.";
            TeleportTeam1SubButton.Visible = false;
            TeleportTeam1SubButton.Enabled = true;
            TeleportTeam1SubButton.OnClicked = (GUIButton button, object userData) =>
            {
                GameMain.NilMod.ActiveClickCommand = true;
                GameMain.NilMod.ClickCommandType = "teleportsub";
                GameMain.NilMod.ClickArgs = new string[] { "0" };
                GameMain.NilMod.ClickCooldown = 0.5f;
                ClickCommandFrame.Visible = true;
                ClickCommandDescription.Text = "TELEPORTSUB - Team 0's submarine - Teleports the chosen teams submarine, left click to teleport. Right click to cancel.";
                return true;
            };
            tempbuttonpagelist.Add(TeleportTeam1SubButton);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            RechargePowerTeam2Button = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "Charge team 2 sub", Alignment.TopLeft, "", inGameHUD);
            RechargePowerTeam2Button.ToolTip = "Recharges all power devices for the second teams submarine.";
            RechargePowerTeam2Button.Visible = false;
            RechargePowerTeam2Button.Enabled = true;
            RechargePowerTeam2Button.OnClicked = (GUIButton button, object userData) =>
            {
                DebugConsole.ExecuteCommand("rechargepower 1", true);
                return true;
            };
            tempbuttonpagelist.Add(RechargePowerTeam2Button);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            RechargePowerTeam1Button = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "Charge team 1 sub", Alignment.TopLeft, "", inGameHUD);
            RechargePowerTeam1Button.ToolTip = "Recharges all power devices for the first teams submarine.";
            RechargePowerTeam1Button.Visible = false;
            RechargePowerTeam1Button.Enabled = true;
            RechargePowerTeam1Button.OnClicked = (GUIButton button, object userData) =>
            {
                DebugConsole.ExecuteCommand("rechargepower 0", true);
                return true;
            };
            tempbuttonpagelist.Add(RechargePowerTeam1Button);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            FixWallsButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "Fix All Walls", Alignment.TopLeft, "", inGameHUD);
            FixWallsButton.ToolTip = "Immediately fixes all walls and floors in all submarines, this includes ruin walls.";
            FixWallsButton.Visible = false;
            FixWallsButton.Enabled = true;
            FixWallsButton.OnClicked = (GUIButton button, object userData) =>
            {
                DebugConsole.ExecuteCommand("fixwalls", true);
                return true;
            };
            tempbuttonpagelist.Add(FixWallsButton);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            FixItemsButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "Fix All Items", Alignment.TopLeft, "", inGameHUD);
            FixItemsButton.ToolTip = "Immediately fixes all broken junctions, engines, doors, oxygen tanks, medicals and other items with a condition system.";
            FixItemsButton.Visible = false;
            FixItemsButton.Enabled = true;
            FixItemsButton.OnClicked = (GUIButton button, object userData) =>
            {
                DebugConsole.ExecuteCommand("fixitems", true);
                return true;
            };
            tempbuttonpagelist.Add(FixItemsButton);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            FiresButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "Fire: On", Alignment.TopLeft, "", inGameHUD);
            FiresButton.ToolTip = "Turns on fire control, Left click to add fires.";
            FiresButton.Visible = false;
            FiresButton.Enabled = true;
            FiresButton.OnClicked = (GUIButton button, object userData) =>
            {
                DebugConsole.ExecuteCommand("fire", true);
                return true;
            };
            tempbuttonpagelist.Add(FiresButton);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            WaterButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "Water: On", Alignment.TopLeft, "", inGameHUD);
            WaterButton.ToolTip = "Turns on water control, Left click to add water, Right click to remove.";
            WaterButton.Visible = false;
            WaterButton.Enabled = true;
            WaterButton.OnClicked = (GUIButton button, object userData) =>
            {
                DebugConsole.ExecuteCommand("water", true);
                return true;
            };
            tempbuttonpagelist.Add(WaterButton);

            ButtonCoord = CalculatePageButtonPosition(tempbuttonpagelist.Count());

            OxygenButton = new GUIButton(new Rectangle((int)ButtonCoord.X, (int)ButtonCoord.Y, 130, 20), "Oxygen", Alignment.TopLeft, "", inGameHUD);
            OxygenButton.ToolTip = "Immediately restores all submarines/shuttles oxygen to 100%.";
            OxygenButton.Visible = false;
            OxygenButton.Enabled = true;
            OxygenButton.OnClicked = (GUIButton button, object userData) =>
            {
                DebugConsole.ExecuteCommand("oxygen", true);
                return true;
            };
            tempbuttonpagelist.Add(OxygenButton);

            PageButtons.Add(tempbuttonpagelist);

            #endregion

            UpdateButtonPage();
        }

        Vector2 CalculatePageButtonPosition(int Button)
        {
            int ButtonCoordX = GameMain.GraphicsWidth - 140;
            int ButtonCoordY = 70;

            while(Button > 0)
            {
                if(Button % 2 == 0)
                {
                    ButtonCoordX += 140;
                    ButtonCoordY += 30;
                }
                else
                {
                    ButtonCoordX -= 140;
                }
                Button -= 1;
            }

            return new Vector2(ButtonCoordX, ButtonCoordY);
        }

        public void UpdateButtonPage()
        {
            for (int i = 0; i < PageButtons.Count; i++)
            {
                foreach (GUIButton button in PageButtons[i])
                {
                    if (buttonpage == i + 1 && !IsButtonPageHidden)
                    {
                        button.Visible = true;
                        //button.Enabled = true;
                        button.CanBeFocused = true;
                        button.CanBeSelected = true;
                    }
                    else
                    {
                        button.Visible = false;
                        //button.Enabled = false;
                        button.CanBeFocused = false;
                        button.CanBeSelected = false;
                    }
                }
            }
        }

        public void SetButtonPage(int pageincrement)
        {
            buttonpage = buttonpage + pageincrement;
            //Page Cycling
            if (buttonpage == 0) buttonpage = MaxButtonPages;
            if (buttonpage > MaxButtonPages) buttonpage = 1;

            UpdateButtonPage();
        }

        public override void AddToGUIUpdateList()
        {
            if (started) base.AddToGUIUpdateList();

            if (settingsFrame != null) settingsFrame.AddToGUIUpdateList();
            if (log.LogFrame != null) log.LogFrame.AddToGUIUpdateList();
            //if(started) ingameInfoButton.AddToGUIUpdateList();
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (settingsFrame != null)
            {
                settingsFrame.Draw(spriteBatch);
            }
            else if (log.LogFrame != null)
            {
                log.LogFrame.Draw(spriteBatch);
            }

            if (Screen.Selected == GameMain.GameScreen && !GUI.DisableHUD)
            {
                if (EndVoteCount >= 0)
                {
                    GUI.DrawString(spriteBatch, new Vector2(GameMain.GraphicsWidth - 480.0f, 12),
                        "Votes to end the round (y/n): " + EndVoteCount + "/" + (EndVoteMax - EndVoteCount), Color.White, null, 0, GUI.SmallFont);
                }
            }

            if (ShowNetStats)
            {
                GUI.Font.DrawString(spriteBatch, "Unique Events: " + entityEventManager.UniqueEvents.Count, new Vector2(10, 50), Color.White);

                int width = 200, height = 300;
                int x = GameMain.GraphicsWidth - width, y = (int)(GameMain.GraphicsHeight * 0.3f);


                if (clientListScrollBar == null)
                {
                    clientListScrollBar = new GUIScrollBar(new Rectangle(x + width - 15, y, 15, height), "", 1.0f);
                }


                GUI.DrawRectangle(spriteBatch, new Rectangle(x, y, width, height), Color.Black * 0.7f, true);
                GUI.Font.DrawString(spriteBatch, "Network statistics:", new Vector2(x + 10, y + 10), Color.White);

                GUI.SmallFont.DrawString(spriteBatch, "Connections: " + server.ConnectionsCount, new Vector2(x + 10, y + 30), Color.White);
                GUI.SmallFont.DrawString(spriteBatch, "Received bytes: " + MathUtils.GetBytesReadable(server.Statistics.ReceivedBytes), new Vector2(x + 10, y + 45), Color.White);
                GUI.SmallFont.DrawString(spriteBatch, "Received packets: " + server.Statistics.ReceivedPackets, new Vector2(x + 10, y + 60), Color.White);

                GUI.SmallFont.DrawString(spriteBatch, "Sent bytes: " + MathUtils.GetBytesReadable(server.Statistics.SentBytes), new Vector2(x + 10, y + 75), Color.White);
                GUI.SmallFont.DrawString(spriteBatch, "Sent packets: " + server.Statistics.SentPackets, new Vector2(x + 10, y + 90), Color.White);

                int resentMessages = 0;

                int clientListHeight = connectedClients.Count * 40;
                float scrollBarHeight = (height - 110) / (float)Math.Max(clientListHeight, 110);

                if (clientListScrollBar.BarSize != scrollBarHeight)
                {
                    clientListScrollBar.BarSize = scrollBarHeight;
                }

                int startY = y + 110;
                y = (startY - (int)(clientListScrollBar.BarScroll * (clientListHeight - (height - 110))));
                foreach (Client c in connectedClients)
                {
                    Color clientColor = c.Connection.AverageRoundtripTime > 0.3f ? Color.Red : Color.White;

                    if (y >= startY && y < startY + height - 120)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, c.Name + " (" + c.Connection.RemoteEndPoint.Address.ToString() + ")", new Vector2(x + 10, y), clientColor);
                        GUI.SmallFont.DrawString(spriteBatch, "Ping: " + (int)(c.Connection.AverageRoundtripTime * 1000.0f) + " ms", new Vector2(x + 20, y + 10), clientColor);
                    }
                    if (y + 25 >= startY && y < startY + height - 130) GUI.SmallFont.DrawString(spriteBatch, "Resent messages: " + c.Connection.Statistics.ResentMessages, new Vector2(x + 20, y + 20), clientColor);

                    resentMessages += (int)c.Connection.Statistics.ResentMessages;

                    y += 40;
                }

                clientListScrollBar.Update(1.0f / 60.0f);
                clientListScrollBar.Draw(spriteBatch);

                netStats.AddValue(NetStats.NetStatType.ResentMessages, Math.Max(resentMessages, 0));
                netStats.AddValue(NetStats.NetStatType.SentBytes, server.Statistics.SentBytes);
                netStats.AddValue(NetStats.NetStatType.ReceivedBytes, server.Statistics.ReceivedBytes);

                netStats.Draw(spriteBatch, new Rectangle(200, 0, 800, 200), this);
            }
            if (ShowLagDiagnostics)
            {
                int width = 200, height = 300;
                int x = GameMain.GraphicsWidth - width, y = (int)(GameMain.GraphicsHeight * 0.3f);

                if (ProfilingListScrollBar == null)
                {
                    ProfilingListScrollBar = new GUIScrollBar(new Rectangle(x + width - 15, y, 15, height), "", 1.0f);
                }

                GUI.DrawRectangle(spriteBatch, new Rectangle(x, y, width, height), Color.Black * 0.7f, true);
                GUI.Font.DrawString(spriteBatch, "Profiling statistics:", new Vector2(x + 10, y + 10), Color.White);

                GUI.SmallFont.DrawString(spriteBatch, "Warning - not 100% accurate", new Vector2(x + 10, y + 30), Color.White);
                GUI.SmallFont.DrawString(spriteBatch, "Is a mod addition.", new Vector2(x + 10, y + 45), Color.White);
                GUI.SmallFont.DrawString(spriteBatch, "", new Vector2(x + 10, y + 60), Color.White);

                GUI.SmallFont.DrawString(spriteBatch, "Framerate: " + Math.Round(GameMain.FrameCounter.AverageFramesPerSecond,1), new Vector2(x + 10, y + 75), Color.White);
                GUI.SmallFont.DrawString(spriteBatch, "Bodies: " + GameMain.World.BodyList.Count + " (" + GameMain.World.BodyList.FindAll(b => b.Awake && b.Enabled).Count + "Active", new Vector2(x + 10, y + 90), Color.White);


                //Count of profiling information to scroll through
                int clientListHeight = 19 * 40;
                float scrollBarHeight = (height - 110) / (float)Math.Max(clientListHeight, 110);

                if (ProfilingListScrollBar.BarSize != scrollBarHeight)
                {
                    ProfilingListScrollBar.BarSize = scrollBarHeight;
                }

                int startY = y + 110;
                y = (startY - (int)(ProfilingListScrollBar.BarScroll * (clientListHeight - (height - 110))));
                x -= 5;
                //MainUpdateLoop
                if (y >= startY && y < startY + height - 120)
                {
                    GUI.SmallFont.DrawString(spriteBatch, "MainUpdateLoop:", new Vector2(x + 10, y), Color.White);
                    if (GameMain.NilModProfiler.SampleBufferMainUpdateLoop.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Total Ticks: " + Math.Round(GameMain.NilModProfiler.AverageMainUpdateLoop,0), new Vector2(x + 20, y + 10), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Total Ticks: NA.", new Vector2(x + 20, y + 10), Color.White);
                    }
                }
                y += 40;
                x += 10;

                //GUIUpdate
                if (y >= startY && y < startY + height - 120)
                {
                    GUI.SmallFont.DrawString(spriteBatch, "GUIUpdate:", new Vector2(x + 10, y), Color.White);
                    if (GameMain.NilModProfiler.SampleBufferGUIUpdate.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: " + Math.Round(GameMain.NilModProfiler.AverageGUIUpdate, 0), new Vector2(x + 20, y + 10), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: NA.", new Vector2(x + 20, y + 10), Color.White);
                    }
                    if (GameMain.NilModProfiler.SampleBufferGUIUpdate.Count > 0 && GameMain.NilModProfiler.SampleBufferMainUpdateLoop.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: " + Math.Round((GameMain.NilModProfiler.AverageGUIUpdate / GameMain.NilModProfiler.AverageMainUpdateLoop) * 100, 2) + "%", new Vector2(x + 20, y + 20), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: NA%", new Vector2(x + 20, y + 20), Color.White);
                    }
                }
                y += 40;

                //DebugConsole
                if (y >= startY && y < startY + height - 120)
                {
                    GUI.SmallFont.DrawString(spriteBatch, "DebugConsole:", new Vector2(x + 10, y), Color.White);
                    if (GameMain.NilModProfiler.SampleBufferDebugConsole.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: " + Math.Round(GameMain.NilModProfiler.AverageDebugConsole, 0), new Vector2(x + 20, y + 10), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: NA.", new Vector2(x + 20, y + 10), Color.White);
                    }
                    if (GameMain.NilModProfiler.SampleBufferDebugConsole.Count > 0 && GameMain.NilModProfiler.SampleBufferMainUpdateLoop.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: " + Math.Round((GameMain.NilModProfiler.AverageDebugConsole / GameMain.NilModProfiler.AverageMainUpdateLoop) * 100, 2) + "%", new Vector2(x + 20, y + 20), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: NA%", new Vector2(x + 20, y + 20), Color.White);
                    }
                }
                y += 40;

                //PlayerInput
                if (y >= startY && y < startY + height - 120)
                {
                    GUI.SmallFont.DrawString(spriteBatch, "PlayerInput:", new Vector2(x + 10, y), Color.White);
                    if (GameMain.NilModProfiler.SampleBufferPlayerInput.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: " + Math.Round(GameMain.NilModProfiler.AveragePlayerInput, 0), new Vector2(x + 20, y + 10), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: NA.", new Vector2(x + 20, y + 10), Color.White);
                    }
                    if (GameMain.NilModProfiler.SampleBufferPlayerInput.Count > 0 && GameMain.NilModProfiler.SampleBufferMainUpdateLoop.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: " + Math.Round((GameMain.NilModProfiler.AveragePlayerInput / GameMain.NilModProfiler.AverageMainUpdateLoop) * 100, 2) + "%", new Vector2(x + 20, y + 20), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: NA%", new Vector2(x + 20, y + 20), Color.White);
                    }
                }
                y += 40;

                //SoundPlayer
                if (y >= startY && y < startY + height - 120)
                {
                    GUI.SmallFont.DrawString(spriteBatch, "SoundPlayer:", new Vector2(x + 10, y), Color.White);
                    if (GameMain.NilModProfiler.SampleBufferSoundPlayer.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: " + Math.Round(GameMain.NilModProfiler.AverageSoundPlayer, 0), new Vector2(x + 20, y + 10), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: NA.", new Vector2(x + 20, y + 10), Color.White);
                    }
                    if (GameMain.NilModProfiler.SampleBufferSoundPlayer.Count > 0 && GameMain.NilModProfiler.SampleBufferMainUpdateLoop.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: " + Math.Round((GameMain.NilModProfiler.AverageSoundPlayer / GameMain.NilModProfiler.AverageMainUpdateLoop) * 100, 2) + "%", new Vector2(x + 20, y + 20), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: NA%", new Vector2(x + 20, y + 20), Color.White);
                    }
                }
                y += 40;

                //NetworkMember
                if (y >= startY && y < startY + height - 120)
                {
                    GUI.SmallFont.DrawString(spriteBatch, "NetworkMember:", new Vector2(x + 10, y), Color.White);
                    if (GameMain.NilModProfiler.SampleBufferNetworkMember.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: " + Math.Round(GameMain.NilModProfiler.AverageNetworkMember, 0), new Vector2(x + 20, y + 10), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: NA.", new Vector2(x + 20, y + 10), Color.White);
                    }
                    if (GameMain.NilModProfiler.SampleBufferNetworkMember.Count > 0 && GameMain.NilModProfiler.SampleBufferMainUpdateLoop.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: " + Math.Round((GameMain.NilModProfiler.AverageNetworkMember / GameMain.NilModProfiler.AverageMainUpdateLoop) * 100, 2) + "%", new Vector2(x + 20, y + 20), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: NA%", new Vector2(x + 20, y + 20), Color.White);
                    }
                }
                y += 40;

                //CoroutineManager
                if (y >= startY && y < startY + height - 120)
                {
                    GUI.SmallFont.DrawString(spriteBatch, "CoroutineManager:", new Vector2(x + 10, y), Color.White);
                    if (GameMain.NilModProfiler.SampleBufferCoroutineManager.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: " + Math.Round(GameMain.NilModProfiler.AverageCoroutineManager, 0), new Vector2(x + 20, y + 10), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: NA.", new Vector2(x + 20, y + 10), Color.White);
                    }
                    if (GameMain.NilModProfiler.SampleBufferCoroutineManager.Count > 0 && GameMain.NilModProfiler.SampleBufferMainUpdateLoop.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: " + Math.Round((GameMain.NilModProfiler.AverageCoroutineManager / GameMain.NilModProfiler.AverageMainUpdateLoop) * 100, 2) + "%", new Vector2(x + 20, y + 20), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: NA%", new Vector2(x + 20, y + 20), Color.White);
                    }
                }
                y += 40;

                //GameScreen
                if (y >= startY && y < startY + height - 120)
                {
                    GUI.SmallFont.DrawString(spriteBatch, "GameScreen:", new Vector2(x + 10, y), Color.White);
                    if (GameMain.NilModProfiler.SampleBufferGameScreen.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: " + Math.Round(GameMain.NilModProfiler.AverageGameScreen, 0), new Vector2(x + 20, y + 10), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: NA.", new Vector2(x + 20, y + 10), Color.White);
                    }
                    if (GameMain.NilModProfiler.SampleBufferGameScreen.Count > 0 && GameMain.NilModProfiler.SampleBufferMainUpdateLoop.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: " + Math.Round((GameMain.NilModProfiler.AverageGameScreen / GameMain.NilModProfiler.AverageMainUpdateLoop) * 100, 2) + "%", new Vector2(x + 20, y + 20), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: NA%", new Vector2(x + 20, y + 20), Color.White);
                    }
                }
                y += 40;
                x += 10;

                //GameSessionUpdate
                if (y >= startY && y < startY + height - 120)
                {
                    GUI.SmallFont.DrawString(spriteBatch, "GameSessionUpdate:", new Vector2(x + 10, y), Color.White);
                    if (GameMain.NilModProfiler.SampleBufferGameSessionUpdate.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: " + Math.Round(GameMain.NilModProfiler.AverageGameSessionUpdate, 0), new Vector2(x + 20, y + 10), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: NA.", new Vector2(x + 20, y + 10), Color.White);
                    }
                    if (GameMain.NilModProfiler.SampleBufferGameSessionUpdate.Count > 0 && GameMain.NilModProfiler.SampleBufferMainUpdateLoop.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: " + Math.Round((GameMain.NilModProfiler.AverageGameSessionUpdate / GameMain.NilModProfiler.AverageMainUpdateLoop) * 100, 2) + "%", new Vector2(x + 20, y + 20), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: NA%", new Vector2(x + 20, y + 20), Color.White);
                    }
                    if (GameMain.NilModProfiler.SampleBufferGameSessionUpdate.Count > 0 && GameMain.NilModProfiler.SampleBufferGameScreen.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% GameScreen: " + Math.Round((GameMain.NilModProfiler.AverageGameSessionUpdate / GameMain.NilModProfiler.AverageGameScreen) * 100, 2) + "%", new Vector2(x + 20, y + 30), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% GameScreen: NA%", new Vector2(x + 20, y + 30), Color.White);
                    }
                }
                y += 40;

                //ParticleManager
                if (y >= startY && y < startY + height - 120)
                {
                    GUI.SmallFont.DrawString(spriteBatch, "ParticleManager:", new Vector2(x + 10, y), Color.White);
                    if (GameMain.NilModProfiler.SampleBufferParticleManager.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: " + Math.Round(GameMain.NilModProfiler.AverageParticleManager, 0), new Vector2(x + 20, y + 10), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: NA.", new Vector2(x + 20, y + 10), Color.White);
                    }
                    if (GameMain.NilModProfiler.SampleBufferParticleManager.Count > 0 && GameMain.NilModProfiler.SampleBufferMainUpdateLoop.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: " + Math.Round((GameMain.NilModProfiler.AverageParticleManager / GameMain.NilModProfiler.AverageMainUpdateLoop) * 100, 2) + "%", new Vector2(x + 20, y + 20), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: NA%", new Vector2(x + 20, y + 20), Color.White);
                    }
                    if (GameMain.NilModProfiler.SampleBufferParticleManager.Count > 0 && GameMain.NilModProfiler.SampleBufferGameScreen.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% GameScreen: " + Math.Round((GameMain.NilModProfiler.AverageParticleManager / GameMain.NilModProfiler.AverageGameScreen) * 100, 2) + "%", new Vector2(x + 20, y + 30), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% GameScreen: NA%", new Vector2(x + 20, y + 30), Color.White);
                    }
                }
                y += 40;

                //LightManager
                if (y >= startY && y < startY + height - 120)
                {
                    GUI.SmallFont.DrawString(spriteBatch, "LightManager:", new Vector2(x + 10, y), Color.White);
                    if (GameMain.NilModProfiler.SampleBufferLightManager.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: " + Math.Round(GameMain.NilModProfiler.AverageLightManager, 0), new Vector2(x + 20, y + 10), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: NA.", new Vector2(x + 20, y + 10), Color.White);
                    }
                    if (GameMain.NilModProfiler.SampleBufferLightManager.Count > 0 && GameMain.NilModProfiler.SampleBufferMainUpdateLoop.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: " + Math.Round((GameMain.NilModProfiler.AverageLightManager / GameMain.NilModProfiler.AverageMainUpdateLoop) * 100, 2) + "%", new Vector2(x + 20, y + 20), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: NA%", new Vector2(x + 20, y + 20), Color.White);
                    }
                    if (GameMain.NilModProfiler.SampleBufferLightManager.Count > 0 && GameMain.NilModProfiler.SampleBufferGameScreen.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% GameScreen: " + Math.Round((GameMain.NilModProfiler.AverageLightManager / GameMain.NilModProfiler.AverageGameScreen) * 100, 2) + "%", new Vector2(x + 20, y + 30), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% GameScreen: NA%", new Vector2(x + 20, y + 30), Color.White);
                    }
                }
                y += 40;

                //LevelUpdate
                if (y >= startY && y < startY + height - 120)
                {
                    GUI.SmallFont.DrawString(spriteBatch, "LevelUpdate:", new Vector2(x + 10, y), Color.White);
                    if (GameMain.NilModProfiler.SampleBufferLevelUpdate.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: " + Math.Round(GameMain.NilModProfiler.AverageLevelUpdate, 0), new Vector2(x + 20, y + 10), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: NA.", new Vector2(x + 20, y + 10), Color.White);
                    }
                    if (GameMain.NilModProfiler.SampleBufferLevelUpdate.Count > 0 && GameMain.NilModProfiler.SampleBufferMainUpdateLoop.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: " + Math.Round((GameMain.NilModProfiler.AverageLevelUpdate / GameMain.NilModProfiler.AverageMainUpdateLoop) * 100, 2) + "%", new Vector2(x + 20, y + 20), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: NA%", new Vector2(x + 20, y + 20), Color.White);
                    }
                    if (GameMain.NilModProfiler.SampleBufferLevelUpdate.Count > 0 && GameMain.NilModProfiler.SampleBufferGameScreen.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% GameScreen: " + Math.Round((GameMain.NilModProfiler.AverageLevelUpdate / GameMain.NilModProfiler.AverageGameScreen) * 100, 2) + "%", new Vector2(x + 20, y + 30), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% GameScreen: NA%", new Vector2(x + 20, y + 30), Color.White);
                    }
                }
                y += 40;

                //Character Update
                if (y >= startY && y < startY + height - 120)
                {
                    GUI.SmallFont.DrawString(spriteBatch, "CharacterUpdate:", new Vector2(x + 10, y), Color.White);
                    if (GameMain.NilModProfiler.SampleBufferCharacterUpdate.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: " + Math.Round(GameMain.NilModProfiler.AverageCharacterUpdate,0), new Vector2(x + 20, y + 10), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: NA.", new Vector2(x + 20, y + 10), Color.White);
                    }
                    if (GameMain.NilModProfiler.SampleBufferCharacterUpdate.Count > 0 && GameMain.NilModProfiler.SampleBufferMainUpdateLoop.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: " + Math.Round((GameMain.NilModProfiler.AverageCharacterUpdate / GameMain.NilModProfiler.AverageMainUpdateLoop) * 100, 2) + "%", new Vector2(x + 20, y + 20), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: NA%", new Vector2(x + 20, y + 20), Color.White);
                    }
                    if (GameMain.NilModProfiler.SampleBufferCharacterUpdate.Count > 0 && GameMain.NilModProfiler.SampleBufferGameScreen.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% GameScreen: " + Math.Round((GameMain.NilModProfiler.AverageCharacterUpdate / GameMain.NilModProfiler.AverageGameScreen) * 100, 2) + "%", new Vector2(x + 20, y + 30), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% GameScreen: NA%", new Vector2(x + 20, y + 30), Color.White);
                    }
                }
                y += 40;

                //StatusEffects
                if (y >= startY && y < startY + height - 120)
                {
                    GUI.SmallFont.DrawString(spriteBatch, "StatusEffect:", new Vector2(x + 10, y), Color.White);
                    if (GameMain.NilModProfiler.SampleBufferStatusEffect.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: " + Math.Round(GameMain.NilModProfiler.AverageStatusEffect, 0), new Vector2(x + 20, y + 10), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: NA.", new Vector2(x + 20, y + 10), Color.White);
                    }
                    if (GameMain.NilModProfiler.SampleBufferStatusEffect.Count > 0 && GameMain.NilModProfiler.SampleBufferMainUpdateLoop.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: " + Math.Round((GameMain.NilModProfiler.AverageStatusEffect / GameMain.NilModProfiler.AverageMainUpdateLoop) * 100, 2) + "%", new Vector2(x + 20, y + 20), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: NA%", new Vector2(x + 20, y + 20), Color.White);
                    }
                    if (GameMain.NilModProfiler.SampleBufferStatusEffect.Count > 0 && GameMain.NilModProfiler.SampleBufferGameScreen.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% GameScreen: " + Math.Round((GameMain.NilModProfiler.AverageStatusEffect / GameMain.NilModProfiler.AverageGameScreen) * 100, 2) + "%", new Vector2(x + 20, y + 30), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% GameScreen: NA%", new Vector2(x + 20, y + 30), Color.White);
                    }
                }
                y += 40;

                //SetTransforms
                if (y >= startY && y < startY + height - 120)
                {
                    GUI.SmallFont.DrawString(spriteBatch, "SetTransform:", new Vector2(x + 10, y), Color.White);
                    if (GameMain.NilModProfiler.SampleBufferSetTransforms.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: " + Math.Round(GameMain.NilModProfiler.AverageSetTransforms, 0), new Vector2(x + 20, y + 10), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: NA.", new Vector2(x + 20, y + 10), Color.White);
                    }
                    if (GameMain.NilModProfiler.SampleBufferSetTransforms.Count > 0 && GameMain.NilModProfiler.SampleBufferMainUpdateLoop.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: " + Math.Round((GameMain.NilModProfiler.AverageSetTransforms / GameMain.NilModProfiler.AverageMainUpdateLoop) * 100, 2) + "%", new Vector2(x + 20, y + 20), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: NA%", new Vector2(x + 20, y + 20), Color.White);
                    }
                    if (GameMain.NilModProfiler.SampleBufferSetTransforms.Count > 0 && GameMain.NilModProfiler.SampleBufferGameScreen.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% GameScreen: " + Math.Round((GameMain.NilModProfiler.AverageSetTransforms / GameMain.NilModProfiler.AverageGameScreen) * 100, 2) + "%", new Vector2(x + 20, y + 30), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% GameScreen: NA%", new Vector2(x + 20, y + 30), Color.White);
                    }
                }
                y += 40;

                //MapEntityUpdate
                if (y >= startY && y < startY + height - 120)
                {
                    GUI.SmallFont.DrawString(spriteBatch, "MapEntityUpdate:", new Vector2(x + 10, y), Color.White);
                    if (GameMain.NilModProfiler.SampleBufferMapEntityUpdate.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: " + Math.Round(GameMain.NilModProfiler.AverageMapEntityUpdate, 0), new Vector2(x + 20, y + 10), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: NA.", new Vector2(x + 20, y + 10), Color.White);
                    }
                    if (GameMain.NilModProfiler.SampleBufferMapEntityUpdate.Count > 0 && GameMain.NilModProfiler.SampleBufferMainUpdateLoop.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: " + Math.Round((GameMain.NilModProfiler.AverageMapEntityUpdate / GameMain.NilModProfiler.AverageMainUpdateLoop) * 100, 2) + "%", new Vector2(x + 20, y + 20), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: NA%", new Vector2(x + 20, y + 20), Color.White);
                    }
                    if (GameMain.NilModProfiler.SampleBufferMapEntityUpdate.Count > 0 && GameMain.NilModProfiler.SampleBufferGameScreen.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% GameScreen: " + Math.Round((GameMain.NilModProfiler.AverageMapEntityUpdate / GameMain.NilModProfiler.AverageGameScreen) * 100, 2) + "%", new Vector2(x + 20, y + 30), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% GameScreen: NA%", new Vector2(x + 20, y + 30), Color.White);
                    }
                }
                y += 40;

                //CharacterAnimUpdate
                if (y >= startY && y < startY + height - 120)
                {
                    GUI.SmallFont.DrawString(spriteBatch, "CharacterAnim:", new Vector2(x + 10, y), Color.White);
                    if (GameMain.NilModProfiler.SampleBufferCharacterAnimUpdate.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: " + Math.Round(GameMain.NilModProfiler.AverageCharacterAnimUpdate, 0), new Vector2(x + 20, y + 10), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: NA.", new Vector2(x + 20, y + 10), Color.White);
                    }
                    if (GameMain.NilModProfiler.SampleBufferCharacterAnimUpdate.Count > 0 && GameMain.NilModProfiler.SampleBufferMainUpdateLoop.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: " + Math.Round((GameMain.NilModProfiler.AverageCharacterAnimUpdate / GameMain.NilModProfiler.AverageMainUpdateLoop) * 100, 2) + "%", new Vector2(x + 20, y + 20), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: NA%", new Vector2(x + 20, y + 20), Color.White);
                    }
                    if (GameMain.NilModProfiler.SampleBufferCharacterAnimUpdate.Count > 0 && GameMain.NilModProfiler.SampleBufferGameScreen.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% GameScreen: " + Math.Round((GameMain.NilModProfiler.AverageCharacterAnimUpdate / GameMain.NilModProfiler.AverageGameScreen) * 100, 2) + "%", new Vector2(x + 20, y + 30), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% GameScreen: NA%", new Vector2(x + 20, y + 30), Color.White);
                    }
                }
                y += 40;

                //SubmarineUpdate
                if (y >= startY && y < startY + height - 120)
                {
                    GUI.SmallFont.DrawString(spriteBatch, "SubmarineUpdate:", new Vector2(x + 10, y), Color.White);
                    if (GameMain.NilModProfiler.SampleBufferSubmarineUpdate.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: " + Math.Round(GameMain.NilModProfiler.AverageSubmarineUpdate, 0), new Vector2(x + 20, y + 10), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: NA.", new Vector2(x + 20, y + 10), Color.White);
                    }
                    if (GameMain.NilModProfiler.SampleBufferSubmarineUpdate.Count > 0 && GameMain.NilModProfiler.SampleBufferMainUpdateLoop.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: " + Math.Round((GameMain.NilModProfiler.AverageSubmarineUpdate / GameMain.NilModProfiler.AverageMainUpdateLoop) * 100, 2) + "%", new Vector2(x + 20, y + 20), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: NA%", new Vector2(x + 20, y + 20), Color.White);
                    }
                    if (GameMain.NilModProfiler.SampleBufferSubmarineUpdate.Count > 0 && GameMain.NilModProfiler.SampleBufferGameScreen.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% GameScreen: " + Math.Round((GameMain.NilModProfiler.AverageSubmarineUpdate / GameMain.NilModProfiler.AverageGameScreen) * 100, 2) + "%", new Vector2(x + 20, y + 30), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% GameScreen: NA%", new Vector2(x + 20, y + 30), Color.White);
                    }
                }
                y += 40;

                //RagdollUpdate
                if (y >= startY && y < startY + height - 120)
                {
                    GUI.SmallFont.DrawString(spriteBatch, "RagdollUpdate:", new Vector2(x + 10, y), Color.White);
                    if (GameMain.NilModProfiler.SampleBufferRagdollUpdate.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: " + Math.Round(GameMain.NilModProfiler.AverageRagdollUpdate, 0), new Vector2(x + 20, y + 10), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: NA.", new Vector2(x + 20, y + 10), Color.White);
                    }
                    if (GameMain.NilModProfiler.SampleBufferRagdollUpdate.Count > 0 && GameMain.NilModProfiler.SampleBufferMainUpdateLoop.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: " + Math.Round((GameMain.NilModProfiler.AverageRagdollUpdate / GameMain.NilModProfiler.AverageMainUpdateLoop) * 100, 2) + "%", new Vector2(x + 20, y + 20), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: NA%", new Vector2(x + 20, y + 20), Color.White);
                    }
                    if (GameMain.NilModProfiler.SampleBufferRagdollUpdate.Count > 0 && GameMain.NilModProfiler.SampleBufferGameScreen.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% GameScreen: " + Math.Round((GameMain.NilModProfiler.AverageRagdollUpdate / GameMain.NilModProfiler.AverageGameScreen) * 100, 2) + "%", new Vector2(x + 20, y + 30), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% GameScreen: NA%", new Vector2(x + 20, y + 30), Color.White);
                    }
                }
                y += 40;

                //PhysicsWorldStep
                if (y >= startY && y < startY + height - 120)
                {
                    GUI.SmallFont.DrawString(spriteBatch, "PhysicsWorldStep:", new Vector2(x + 10, y), Color.White);
                    if (GameMain.NilModProfiler.SampleBufferPhysicsWorldStep.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: " + Math.Round(GameMain.NilModProfiler.AveragePhysicsWorldStep, 0), new Vector2(x + 20, y + 10), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "Ticks: NA.", new Vector2(x + 20, y + 10), Color.White);
                    }
                    if (GameMain.NilModProfiler.SampleBufferPhysicsWorldStep.Count > 0 && GameMain.NilModProfiler.SampleBufferMainUpdateLoop.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: " + Math.Round((GameMain.NilModProfiler.AveragePhysicsWorldStep / GameMain.NilModProfiler.AverageMainUpdateLoop) * 100, 2) + "%", new Vector2(x + 20, y + 20), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% Main Loop: NA%", new Vector2(x + 20, y + 20), Color.White);
                    }
                    if (GameMain.NilModProfiler.SampleBufferPhysicsWorldStep.Count > 0 && GameMain.NilModProfiler.SampleBufferGameScreen.Count > 0)
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% GameScreen: " + Math.Round((GameMain.NilModProfiler.AveragePhysicsWorldStep / GameMain.NilModProfiler.AverageGameScreen) * 100, 2) + "%", new Vector2(x + 20, y + 30), Color.White);
                    }
                    else
                    {
                        GUI.SmallFont.DrawString(spriteBatch, "% GameScreen: NA%", new Vector2(x + 20, y + 30), Color.White);
                    }
                }
                //y += 40;

                ProfilingListScrollBar.Update(1.0f / 120.0f);
                ProfilingListScrollBar.Draw(spriteBatch);
            }
        }


        private void UpdateFileTransferIndicator(Client client)
        {
            var transfers = fileSender.ActiveTransfers.FindAll(t => t.Connection == client.Connection);

            var clientNameBox = GameMain.NetLobbyScreen.PlayerList.FindChild(client.Name);

            var clientInfo = clientNameBox.FindChild("filetransfer");
            if (clientInfo == null)
            {
                //clientNameBox.ClearChildren();
                foreach(GUIComponent component in clientNameBox.children)
                {
                    component.Visible = false;
                }
                clientInfo = new GUIFrame(new Rectangle(0, 0, 180, 0), Color.Transparent, Alignment.TopRight, null, clientNameBox);
                clientInfo.UserData = "filetransfer";
            }
            else if (transfers.Count == 0)
            {
                clientInfo.Parent.RemoveChild(clientInfo);
                foreach (GUIComponent component in clientInfo.Parent.children)
                {
                    component.Visible = true;
                }
            }

            clientInfo.ClearChildren();

            var progressBar = new GUIProgressBar(new Rectangle(0, 4, 160, clientInfo.Rect.Height - 8), Color.Green, "", 0.0f, Alignment.Left, clientInfo);
            progressBar.IsHorizontal = true;
            progressBar.ProgressGetter = () => { return transfers.Sum(t => t.Progress) / transfers.Count; };

            var textBlock = new GUITextBlock(new Rectangle(0, 2, 160, 0), "", "", Alignment.TopLeft, Alignment.Left | Alignment.CenterY, clientInfo, true, GUI.SmallFont);
            textBlock.TextGetter = () =>
            { return MathUtils.GetBytesReadable(transfers.Sum(t => t.SentOffset)) + " / " + MathUtils.GetBytesReadable(transfers.Sum(t => t.Data.Length)); };

            var cancelButton = new GUIButton(new Rectangle(-5, 0, 14, 0), "X", Alignment.Right, "", clientInfo);
            cancelButton.OnClicked = (GUIButton button, object userdata) =>
            {
                transfers.ForEach(t => fileSender.CancelTransfer(t));
                return true;
            };
        }

        public override bool SelectCrewCharacter(Character character, GUIComponent characterFrame)
        {
            if (character == null) return false;

            if (character != myCharacter)
            {
                var banButton = new GUIButton(new Rectangle(0, 0, 100, 20), "Ban", Alignment.BottomRight, "", characterFrame);
                banButton.UserData = character.Name;
                banButton.OnClicked += GameMain.NetLobbyScreen.BanPlayer;

                var rangebanButton = new GUIButton(new Rectangle(0, -25, 100, 20), "Ban range", Alignment.BottomRight, "", characterFrame);
                rangebanButton.UserData = character.Name;
                rangebanButton.OnClicked += GameMain.NetLobbyScreen.BanPlayerRange;

                var kickButton = new GUIButton(new Rectangle(0, 0, 100, 20), "Kick", Alignment.BottomLeft, "", characterFrame);
                kickButton.UserData = character.Name;
                kickButton.OnClicked += GameMain.NetLobbyScreen.KickPlayer;
            }

            return true;
        }

        private GUIMessageBox upnpBox;
        void InitUPnP()
        {
            server.UPnP.ForwardPort(config.Port, "barotrauma");

            upnpBox = new GUIMessageBox("Please wait...", "Attempting UPnP port forwarding", new string[] { "Cancel" });
            upnpBox.Buttons[0].OnClicked = upnpBox.Close;
        }

        bool DiscoveringUPnP()
        {
            return server.UPnP.Status == UPnPStatus.Discovering && GUIMessageBox.VisibleBox == upnpBox;
        }

        void FinishUPnP()
        {
            upnpBox.Close(null, null);

            if (server.UPnP.Status == UPnPStatus.NotAvailable)
            {
                new GUIMessageBox("Error", "UPnP not available");
            }
            else if (server.UPnP.Status == UPnPStatus.Discovering)
            {
                new GUIMessageBox("Error", "UPnP discovery timed out");
            }
        }

        public bool StartGameClicked(GUIButton button, object obj)
        {
            return StartGame();
        }

        public void SpawnCreaturePrompt()
        {
            var SpawnCreaturePrompt = new GUIMessageBox("Creature Spawn", "", new string[] { "Left Click to spawn", "Cancel Spawn" }, 410, 430, Alignment.TopCenter);
            var spawnNameFrame = new GUIListBox(new Rectangle(0, 30, 200, 180), Color.White, "", SpawnCreaturePrompt.children[0]);
            var spawnTypeFrame = new GUIListBox(new Rectangle(210, 30, 120, 120), Color.White, "", SpawnCreaturePrompt.children[0]);
            var PromptText = new GUITextBlock(new Rectangle(0, 210, 340, 85), "Use the left list to select the creature to spawn, use the right list to choose where/how to spawn it, finally use the slider for how many to spawn. cancel or right click after using cursor to cancel spawning. left click with cursor mode to spawn in the level.", "", Alignment.TopLeft, Alignment.TopLeft, SpawnCreaturePrompt.children[0],true, GUI.SmallFont);


            string[] Characters;
            Characters = System.IO.Directory.GetDirectories("Content/Characters/");

            List<string> characterFiles = GameMain.Config.SelectedContentPackage.GetFilesOfType(ContentType.Character);

            foreach (string characterfile in characterFiles)
            {
                var charTextBlock = new GUITextBlock(
                new Rectangle(0, 0, 0, 25), ToolBox.LimitString(System.IO.Path.GetFileName(characterfile.Replace(".xml","")), GUI.Font, spawnNameFrame.Rect.Width - 65), "ListBoxElement",
                Alignment.TopLeft, Alignment.CenterLeft, spawnNameFrame)
                {
                    Padding = new Vector4(10.0f, 0.0f, 0.0f, 0.0f),
                    ToolTip = "Spawns the character from " + characterfile.Replace(System.IO.Path.GetFileName(characterfile),""),
                    UserData = System.IO.Path.GetFileNameWithoutExtension(characterfile),
                    TextColor = new Color(Color.White, 1.0f)
                };
            }

            var CursorTextBlock = new GUITextBlock(
                new Rectangle(0, 0, 0, 25), ToolBox.LimitString("Cursor", GUI.Font, spawnTypeFrame.Rect.Width), "ListBoxElement",
                Alignment.TopLeft, Alignment.CenterLeft, spawnTypeFrame)
            {
                Padding = new Vector4(10.0f, 0.0f, 0.0f, 0.0f),
                ToolTip = "Spawns a character where you click, using a position of your mouse cursor, Right clicking cancels this action or when the spawn number runs out.",
                UserData = "Cursor",
                TextColor = new Color(Color.White, 1.0f)
            };

            var InsideTextBlock = new GUITextBlock(
                new Rectangle(0, 0, 0, 25), ToolBox.LimitString("Inside", GUI.Font, spawnTypeFrame.Rect.Width), "ListBoxElement",
                Alignment.TopLeft, Alignment.CenterLeft, spawnTypeFrame)
            {
                Padding = new Vector4(10.0f, 0.0f, 0.0f, 0.0f),
                ToolTip = "Spawns a character inside of the main submarine, using a spawnpoint of type HUMAN",
                UserData = "Inside",
                TextColor = new Color(Color.White, 1.0f)
            };

            var closeTextBlock = new GUITextBlock(
                new Rectangle(0, 0, 0, 25), ToolBox.LimitString("Close", GUI.Font, spawnTypeFrame.Rect.Width), "ListBoxElement",
                Alignment.TopLeft, Alignment.CenterLeft, spawnTypeFrame)
            {
                Padding = new Vector4(10.0f, 0.0f, 0.0f, 0.0f),
                ToolTip = "Spawns a character close to the submarine, using any kind of spawnpoint that is CLOSEST to the submarine (But not inside a hull!)",
                UserData = "Close",
                TextColor = new Color(Color.White, 1.0f)
            };

            var OutsideTextBlock = new GUITextBlock(
                new Rectangle(0, 0, 0, 25), ToolBox.LimitString("Outside", GUI.Font, spawnTypeFrame.Rect.Width), "ListBoxElement",
                Alignment.TopLeft, Alignment.CenterLeft, spawnTypeFrame)
            {
                Padding = new Vector4(10.0f, 0.0f, 0.0f, 0.0f),
                ToolTip = "Spawns a character Outside of the main submarine, using a spawnpoint of type ENEMY",
                UserData = "Outside",
                TextColor = new Color(Color.White, 1.0f)
            };

            spawnNameFrame.Select(0);
            spawnTypeFrame.Select(0);

            var SpawnCountText = new GUITextBlock(new Rectangle(210, 185, 40, 20), "Creatures to spawn: 1", "", SpawnCreaturePrompt.children[0], GUI.SmallFont);
            var SpawnCount = 1f;

            var SpawnCountSlider = new GUIScrollBar(new Rectangle(210, 160, 120, 30), "", 0.1f, SpawnCreaturePrompt.children[0]);
            SpawnCountSlider.UserData = SpawnCountText;
            SpawnCountSlider.Step = 0.1f;
            SpawnCountSlider.BarScroll = (SpawnCount - 1f) * 0.1f;
            SpawnCountSlider.OnMoved = (GUIScrollBar scrollBar, float barScroll) =>
            {
                GUITextBlock voteText = scrollBar.UserData as GUITextBlock;

                SpawnCount = barScroll * 10.0f + 1f;
                SpawnCountText.Text = "Creatures to spawn: " + (int)MathUtils.Round(SpawnCount, 1.0f);
                return true;
            };
            SpawnCountSlider.OnMoved(SpawnCountSlider, SpawnCountSlider.BarScroll);

            SpawnCreaturePrompt.Buttons[0].OnClicked += (btn, userData) =>
            {
                if (spawnTypeFrame.Selected.UserData.ToString().ToLowerInvariant() == "cursor")
                {
                    GameMain.NilMod.ActiveClickCommand = true;
                    GameMain.NilMod.ClickCommandType = "spawncreature";
                    GameMain.NilMod.ClickArgs = new string[]
                    {
                        spawnNameFrame.Selected.UserData.ToString(),
                        spawnTypeFrame.Selected.UserData.ToString(),
                        SpawnCount.ToString()
                    };
                }
                else
                {
                    for (int i = 0; i < SpawnCount; i++)
                    {
                        Character spawnedCharacter = null;
                        Vector2 spawnPosition = Vector2.Zero;
                        WayPoint spawnPoint = null;

                        switch (spawnTypeFrame.Selected.UserData.ToString().ToLowerInvariant())
                        {
                            case "inside":
                                spawnPoint = WayPoint.GetRandom(SpawnType.Human, null, Submarine.MainSub);
                                break;
                            case "outside":
                                spawnPoint = WayPoint.GetRandom(SpawnType.Enemy);
                                break;
                            case "close":
                                float closestDist = -1.0f;
                                foreach (WayPoint wp in WayPoint.WayPointList)
                                {
                                    if (wp.Submarine != null) continue;

                                    //don't spawn inside hulls
                                    if (Hull.FindHull(wp.WorldPosition, null) != null) continue;

                                    float dist = Vector2.Distance(wp.WorldPosition, GameMain.GameScreen.Cam.WorldViewCenter);

                                    if (closestDist < 0.0f || dist < closestDist)
                                    {
                                        spawnPoint = wp;
                                        closestDist = dist;
                                    }
                                }
                                break;
                        }
                        if (spawnPoint != null) spawnPosition = spawnPoint.WorldPosition;

                        if (spawnNameFrame.Selected.UserData.ToString().ToLowerInvariant() == "human")
                        {

                            spawnedCharacter = Character.Create(Character.HumanConfigFile, spawnPosition);
                        }
                        else
                        {
                            spawnedCharacter = Character.Create(
                            "Content/Characters/"
                            + spawnNameFrame.Selected.UserData.ToString().ToUpper().First() + spawnNameFrame.Selected.UserData.ToString().Substring(1)
                            + "/" + spawnNameFrame.Selected.UserData.ToString().ToLower() + ".xml", spawnPosition);
                        }
                    }
                }
                return true;
            };
            SpawnCreaturePrompt.Buttons[0].OnClicked += SpawnCreaturePrompt.Close;
            SpawnCreaturePrompt.Buttons[1].OnClicked += SpawnCreaturePrompt.Close;
        }
    }
}
