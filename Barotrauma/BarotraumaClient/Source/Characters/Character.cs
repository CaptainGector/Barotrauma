using Barotrauma.Networking;
using Barotrauma.Particles;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Barotrauma
{
    partial class Character : Entity, IDamageable, ISerializableEntity, IClientSerializable, IServerSerializable
    {
        protected float soundTimer;
        protected float soundInterval;

        protected float hudInfoTimer;
        protected bool hudInfoVisible;
        protected Vector2 LastHealthStatusVector;

        float hudInfoHeight;
        private Boolean showinghealth;

        private List<CharacterSound> sounds;

        //the Character that the player is currently controlling
        private static Character controlled;

        private static Character spied;

        public static Character Spied
        {
            get { return spied; }
            set
            {
                if (spied == value) return;
                spied = value;
                CharacterHUD.Reset();

                if (controlled != null)
                {
                    controlled.Enabled = true;
                }
            }
        }

        public static Character Controlled
        {
            get { return controlled; }
            set
            {
                if (controlled == value) return;

                if(controlled != null && value == null)
                {
                    LastControlled = controlled;
                }

                controlled = value;
                CharacterHUD.Reset();

                if (controlled != null)
                {
                    controlled.Enabled = true;
                }
            }
        }

        public static Character LastControlled;
        public static Character SpawnCharacter;

        private Dictionary<object, HUDProgressBar> hudProgressBars;

        public Dictionary<object, HUDProgressBar> HUDProgressBars
        {
            get { return hudProgressBars; }
        }

        partial void InitProjSpecific(XDocument doc)
        {
            soundInterval = doc.Root.GetAttributeFloat("soundinterval", 10.0f);

            keys = new Key[Enum.GetNames(typeof(InputType)).Length];

            for (int i = 0; i < Enum.GetNames(typeof(InputType)).Length; i++)
            {
                keys[i] = new Key(GameMain.Config.KeyBind((InputType)i));
            }

            var soundElements = doc.Root.Elements("sound").ToList();

            sounds = new List<CharacterSound>();
            foreach (XElement soundElement in soundElements)
            {
                sounds.Add(new CharacterSound(soundElement));
            }

            hudProgressBars = new Dictionary<object, HUDProgressBar>();
        }


        public static void ViewSpied(float deltaTime, Camera cam, bool moveCam = true)
        {
            /*
            if (!DisableControls)
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    keys[i].SetState();
                }
            }
            else
            {
                foreach (Key key in keys)
                {
                    if (key == null) continue;
                    key.Reset();
                }
            }
            */

            if (moveCam)
            {
                if (Spied.needsAir && !Spied.Shielded &&
                    Spied.pressureProtection < 80.0f &&
                    (Spied.AnimController.CurrentHull == null || Spied.AnimController.CurrentHull.LethalPressure > 50.0f))
                {
                    float pressure = Spied.AnimController.CurrentHull == null ? 100.0f : Spied.AnimController.CurrentHull.LethalPressure;

                    cam.Zoom = MathHelper.Lerp(cam.Zoom,
                        (pressure / 50.0f) * Rand.Range(1.0f, 1.05f),
                        (pressure - 50.0f) / 50.0f);
                }

                if (Spied.IsHumanoid)
                {
                    if (!(Spied.SpeciesName.ToLowerInvariant() == "human") && GameMain.NilMod.UseCreatureZoomBoost)
                    {
                        cam.ZoomModifier = -0.10f;
                        cam.OffsetAmount = MathHelper.Lerp(cam.OffsetAmount, MathHelper.Clamp((300.0f * GameMain.NilMod.CreatureZoomMultiplier), 300f, 500f), deltaTime);
                    }
                    else
                    {
                        cam.OffsetAmount = MathHelper.Lerp(cam.OffsetAmount, 250.0f, deltaTime);
                    }
                }
                else
                {
                    float tempmass = Spied.Mass;
                    if (GameMain.NilMod.UseCreatureZoomBoost)
                    {
                        //increased visibility range when controlling large a non-humanoid
                        if ((tempmass) >= 1000)
                        {
                            tempmass = 1200f;
                            cam.ZoomModifier = -1f;
                        }
                        else if ((tempmass) >= 800)
                        {
                            tempmass = 1000f;
                            cam.ZoomModifier = -1f;
                        }
                        else if ((tempmass) >= 600)
                        {
                            tempmass = 800f;
                            cam.ZoomModifier = -1f;
                        }
                        else if ((tempmass) >= 400)
                        {
                            tempmass = 600f;
                            cam.ZoomModifier = -0.5f;
                        }
                        else if ((tempmass) >= 300)
                        {
                            tempmass = 500f;
                            cam.ZoomModifier = -0.4f;
                        }
                        else if ((tempmass) >= 200)
                        {
                            tempmass = 450f;
                            cam.ZoomModifier = -0.3f;
                        }
                        else if ((tempmass) >= 150)
                        {
                            tempmass = 400f;
                            cam.ZoomModifier = -0.3f;
                        }
                        else if ((tempmass) >= 100)
                        {
                            tempmass = 350f;
                            cam.ZoomModifier = -0.3f;
                        }
                        else if ((tempmass) >= 0)
                        {
                            tempmass = 300f;
                            cam.ZoomModifier = -0.3f;
                        }
                    }
                    cam.OffsetAmount = MathHelper.Lerp(cam.OffsetAmount, MathHelper.Clamp((tempmass * GameMain.NilMod.CreatureZoomMultiplier), 250.0f, 1600.0f), deltaTime);
                }
            }

            /*
            Spied.cursorPosition = cam.ScreenToWorld(PlayerInput.MousePosition);
            if (Spied.AnimController.CurrentHull != null && Spied.AnimController.CurrentHull.Submarine != null)
            {
                Spied.cursorPosition -= Spied.AnimController.CurrentHull.Submarine.Position;
            }

            Vector2 mouseSimPos = ConvertUnits.ToSimUnits(Spied.cursorPosition);
            

            if (moveCam)
            {
                if (DebugConsole.IsOpen || GUI.PauseMenuOpen || IsUnconscious ||
                    (GameMain.GameSession?.CrewManager?.CrewCommander != null && GameMain.GameSession.CrewManager.CrewCommander.IsOpen))
                {
                    if (deltaTime > 0.0f) cam.OffsetAmount = 0.0f;
                }
                else if (Lights.LightManager.ViewTarget == Spied && Vector2.DistanceSquared(Spied.AnimController.Limbs[0].SimPosition, mouseSimPos) > 1.0f)
                {
                    Body body = Submarine.CheckVisibility(Spied.AnimController.Limbs[0].SimPosition, mouseSimPos);
                    Structure structure = body == null ? null : body.UserData as Structure;

                    float sightDist = Submarine.LastPickedFraction;
                    if (body?.UserData is Structure && !((Structure)body.UserData).CastShadow)
                    {
                        sightDist = 1.0f;
                    }
                    cam.OffsetAmount = MathHelper.Lerp(cam.OffsetAmount, Math.Max(250.0f, sightDist * 500.0f), 0.05f);
                }
            }
            */
        }

        /// <summary>
        /// Control the Character according to player input
        /// </summary>
        public void ControlLocalPlayer(float deltaTime, Camera cam, bool moveCam = true)
        {
            if (!DisableControls)
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    keys[i].SetState();
                }
            }
            else
            {
                foreach (Key key in keys)
                {
                    if (key == null) continue;
                    key.Reset();
                }
            }

            if (moveCam)
            {
                if (needsAir && !Shielded &&
                    pressureProtection < 80.0f &&
                    (AnimController.CurrentHull == null || AnimController.CurrentHull.LethalPressure > 50.0f))
                {
                    float pressure = AnimController.CurrentHull == null ? 100.0f : AnimController.CurrentHull.LethalPressure;

                    cam.Zoom = MathHelper.Lerp(cam.Zoom,
                        (pressure / 50.0f) * Rand.Range(1.0f, 1.05f),
                        (pressure - 50.0f) / 50.0f);
                }

                if (IsHumanoid)
                {
                    if(!(SpeciesName.ToLowerInvariant() == "human") && GameMain.NilMod.UseCreatureZoomBoost)
                    {
                        cam.ZoomModifier = -0.10f;
                        cam.OffsetAmount = MathHelper.Lerp(cam.OffsetAmount, MathHelper.Clamp((300.0f * GameMain.NilMod.CreatureZoomMultiplier),300f,500f), deltaTime);
                    }
                    else
                    {
                        cam.OffsetAmount = MathHelper.Lerp(cam.OffsetAmount, 250.0f, deltaTime);
                    }
                }
                else
                {
                    float tempmass = Mass;
                    if (GameMain.NilMod.UseCreatureZoomBoost)
                    {
                        //increased visibility range when controlling large a non-humanoid
                        if ((tempmass) >= 1000)
                        {
                            tempmass = 1200f;
                            cam.ZoomModifier = -1f;
                        }
                        else if ((tempmass) >= 800)
                        {
                            tempmass = 1000f;
                            cam.ZoomModifier = -1f;
                        }
                        else if ((tempmass) >= 600)
                        {
                            tempmass = 800f;
                            cam.ZoomModifier = -1f;
                        }
                        else if ((tempmass) >= 400)
                        {
                            tempmass = 600f;
                            cam.ZoomModifier = -0.5f;
                        }
                        else if ((tempmass) >= 300)
                        {
                            tempmass = 500f;
                            cam.ZoomModifier = -0.4f;
                        }
                        else if ((tempmass) >= 200)
                        {
                            tempmass = 450f;
                            cam.ZoomModifier = -0.3f;
                        }
                        else if ((tempmass) >= 150)
                        {
                            tempmass = 400f;
                            cam.ZoomModifier = -0.3f;
                        }
                        else if ((tempmass) >= 100)
                        {
                            tempmass = 350f;
                            cam.ZoomModifier = -0.3f;
                        }
                        else if ((tempmass) >= 0)
                        {
                            tempmass = 300f;
                            cam.ZoomModifier = -0.3f;
                        }
                    }
                    cam.OffsetAmount = MathHelper.Lerp(cam.OffsetAmount, MathHelper.Clamp((tempmass * GameMain.NilMod.CreatureZoomMultiplier), 250.0f, 1600.0f), deltaTime);
                }
            }

            cursorPosition = cam.ScreenToWorld(PlayerInput.MousePosition);
            if (AnimController.CurrentHull != null && AnimController.CurrentHull.Submarine != null)
            {
                cursorPosition -= AnimController.CurrentHull.Submarine.Position;
            }

            Vector2 mouseSimPos = ConvertUnits.ToSimUnits(cursorPosition);
            if (moveCam)
            {
                if (GUI.PauseMenuOpen || IsUnconscious ||
                    (GameMain.GameSession?.CrewManager?.CrewCommander != null && GameMain.GameSession.CrewManager.CrewCommander.IsOpen))
                {
                    if (deltaTime > 0.0f) cam.OffsetAmount = 0.0f;
                }
                else if (DebugConsole.IsOpen && (Lights.LightManager.ViewTarget == this && Vector2.DistanceSquared(AnimController.Limbs[0].SimPosition, mouseSimPos) > 1.0f))
                {
                    Body body = Submarine.CheckVisibility(AnimController.Limbs[0].SimPosition, mouseSimPos);
                    Structure structure = body == null ? null : body.UserData as Structure;

                    float sightDist = Submarine.LastPickedFraction;
                    if (body?.UserData is Structure && !((Structure)body.UserData).CastShadow)
                    {
                        sightDist = 1.0f;
                    }
                    cam.ZoomModifier = -0.4f;
                    //cam.OffsetAmount = MathHelper.Lerp(cam.OffsetAmount, Math.Max(250.0f, sightDist * 500.0f), 0.05f);
                    if (deltaTime > 0.0f) cam.OffsetAmount = 300.0f;
                }
                else if (Lights.LightManager.ViewTarget == this && Vector2.DistanceSquared(AnimController.Limbs[0].SimPosition, mouseSimPos) > 1.0f)
                {
                    Body body = Submarine.CheckVisibility(AnimController.Limbs[0].SimPosition, mouseSimPos);
                    Structure structure = body == null ? null : body.UserData as Structure;

                    float sightDist = Submarine.LastPickedFraction;
                    if (body?.UserData is Structure && !((Structure)body.UserData).CastShadow)
                    {
                        sightDist = 1.0f;
                    }
                    cam.OffsetAmount = MathHelper.Lerp(cam.OffsetAmount, Math.Max(250.0f, sightDist * 500.0f), 0.05f);
                }
            }

            DoInteractionUpdate(deltaTime, mouseSimPos);

            DisableControls = false;
        }

        partial void UpdateControlled(float deltaTime,Camera cam)
        {
            if (controlled != this || spied != null) return;

            ControlLocalPlayer(deltaTime, cam);

            Lights.LightManager.ViewTarget = this;
            CharacterHUD.Update(deltaTime, this);

            foreach (HUDProgressBar progressBar in hudProgressBars.Values)
            {
                progressBar.Update(deltaTime);
            }

            foreach (var pb in hudProgressBars.Where(pb => pb.Value.FadeTimer <= 0.0f).ToList())
            {
                hudProgressBars.Remove(pb.Key);
            }
        }

        partial void DamageHUD(float amount)
        {
            if(spied == this) CharacterHUD.TakeDamage(amount);
            else if (controlled == this) CharacterHUD.TakeDamage(amount);
        }

        partial void UpdateOxygenProjSpecific(float prevOxygen)
        {
            if (prevOxygen > 0.0f && Oxygen <= 0.0f && controlled == this)
            {
                SoundPlayer.PlaySound("drown");
            }
        }

        partial void KillProjSpecific()
        {
            if (GameMain.NetworkMember != null && controlled == this)
            {
                string chatMessage = TextManager.Get("Self_CauseOfDeathDescription." + causeOfDeath.ToString());
                if (GameMain.Client != null) chatMessage += " " + TextManager.Get("DeathChatNotification");

                GameMain.NetworkMember.AddChatMessage(chatMessage, ChatMessageType.Dead);
                GameMain.LightManager.LosEnabled = false;
                controlled = null;
            }

            PlaySound(CharacterSound.SoundType.Die);
        }

        partial void DisposeProjSpecific()
        {
            if (controlled == this) controlled = null;
            if (Spied == this) Spied = null;

            if (GameMain.GameSession?.CrewManager != null &&
                GameMain.GameSession.CrewManager.GetCharacters().Contains(this))
            {
                GameMain.GameSession.CrewManager.RemoveCharacter(this);
            }

            if (GameMain.NetworkMember?.Character == this) GameMain.NetworkMember.Character = null;

            if (Lights.LightManager.ViewTarget == this) Lights.LightManager.ViewTarget = null;
        }

        partial void UpdateProjSpecific(float deltaTime, Camera cam)
        {
            if (info != null || health < maxHealth * 0.98f)
            {
                hudInfoTimer -= deltaTime;
                if (hudInfoTimer <= 0.0f)
                {
                    if (controlled == null)
                    {
                        hudInfoVisible = true;
                    }

                    //if the character is not in the camera view, the name can't be visible and we can avoid the expensive visibility checks
                    else if (WorldPosition.X < cam.WorldView.X || WorldPosition.X > cam.WorldView.Right || 
                            WorldPosition.Y > cam.WorldView.Y || WorldPosition.Y < cam.WorldView.Y - cam.WorldView.Height)
                    {
                        hudInfoVisible = false;
                    }
                    else
                    {
                        //Ideally it shouldn't send the character entirely if we can't see them but /shrug, this isn't the most hacker-proof game atm
                        hudInfoVisible = controlled.CanSeeCharacter(this);                    
                    }
                    hudInfoTimer = Rand.Range(0.5f, 1.0f);
                }
            }
        }

        public static void AddAllToGUIUpdateList()
        {
            for (int i = 0; i < CharacterList.Count; i++)
            {
                CharacterList[i].AddToGUIUpdateList();
            }
        }

        public virtual void AddToGUIUpdateList()
        {
            if(spied == this)
            {
                CharacterHUD.AddToGUIUpdateList(this);
            }
            else if (spied == null && controlled == this)
            {
                CharacterHUD.AddToGUIUpdateList(this);
            }
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!Enabled) return;

            AnimController.Draw(spriteBatch);
        }

        public void DrawHUD(SpriteBatch spriteBatch, Camera cam)
        {
            CharacterHUD.Draw(spriteBatch, this, cam);
        }

        public virtual void DrawFront(SpriteBatch spriteBatch, Camera cam)
        {
            if (!Enabled) return;

            if (GameMain.DebugDraw)
            {
                AnimController.DebugDraw(spriteBatch);

                if (aiTarget != null) aiTarget.Draw(spriteBatch);
            }
            
            if (GUI.DisableHUD) return;

            Vector2 pos = DrawPosition;
            pos.Y += hudInfoHeight;
            if(!showinghealth) pos.Y -= 30f;

            if (CurrentHull != null && DrawPosition.Y > CurrentHull.WorldRect.Y - 120.0f)
            {
                float lowerAmount = DrawPosition.Y - (CurrentHull.WorldRect.Y - 120.0f);
                hudInfoHeight = MathHelper.Lerp(hudInfoHeight, 100.0f - lowerAmount, 0.65f);
                hudInfoHeight = Math.Max(hudInfoHeight, 20.0f);
            }
            else
            {
                hudInfoHeight = MathHelper.Lerp(hudInfoHeight, 100.0f, 0.65f);
            }
            pos.Y = -pos.Y;

            if (speechBubbleTimer > 0.0f)
            {
                GUI.SpeechBubbleIcon.Draw(spriteBatch, pos - Vector2.UnitY * 100.0f,
                    speechBubbleColor * Math.Min(speechBubbleTimer, 1.0f), 0.0f,
                    Math.Min(speechBubbleTimer, 1.0f));
            }

            //if (this == controlled) return;

            float hoverRange = 500.0f;
            float fadeOutRange = 300.0f;
            float cursorDist = Vector2.Distance(WorldPosition, cam.ScreenToWorld(PlayerInput.MousePosition));
            float hudInfoAlpha = MathHelper.Clamp(1.0f - (cursorDist - (hoverRange - fadeOutRange)) / fadeOutRange, 0.25f, 1.0f);

            //Disable the brightness changes of players names for characterless server hosts
            if (GameMain.Server != null && Character.Controlled == null) hudInfoAlpha = 1.0f;

            if (hudInfoVisible && info != null)
            {
                string name = Info.DisplayName;

                if (name != Info.Name)
                {
                    if (GameMain.Client != null
                        && (GameMain.Client.HasPermission(ClientPermissions.Kick) 
                       || GameMain.Client.HasPermission(ClientPermissions.Ban)))
                    {
                        name += " (" + Info.Name + ")";
                    }
                    else if (GameMain.Server != null)
                    {
                        name += " (" + Info.Name + ")";
                    }
                    else if (controlled == null)
                    {
                        name += " " + TextManager.Get("Disguised");
                    }
                }

                Vector2 namePos = new Vector2(pos.X, pos.Y + 10.0f - (5.0f / cam.Zoom)) - GUI.Font.MeasureString(Info.Name) * 0.5f / cam.Zoom;

                Vector2 screenSize = new Vector2(GameMain.GraphicsWidth, GameMain.GraphicsHeight);
            	Vector2 viewportSize = new Vector2(cam.WorldView.Width, cam.WorldView.Height);
            	namePos.X -= cam.WorldView.X; namePos.Y += cam.WorldView.Y;
            	namePos *= screenSize / viewportSize;
            	namePos.X = (float)Math.Floor(namePos.X); namePos.Y = (float)Math.Floor(namePos.Y);
            	namePos *= viewportSize / screenSize;
            	namePos.X += cam.WorldView.X; namePos.Y -= cam.WorldView.Y;

                Color nameColor = Color.White;

                if (GameMain.NilMod.UseRecolouredNameInfo)
                {
                    if (Character.Controlled == null)
                    {
                        if (TeamID == 0)
                        {
                            if (!isDead)
                            {
                                nameColor = Color.White;
                            }
                            else
                            {
                                nameColor = Color.DarkGray;
                            }
                        }
                        else if (TeamID == 1)
                        {
                            if (!isDead)
                            {
                                nameColor = Color.LightBlue;
                            }
                            else
                            {
                                nameColor = Color.DarkBlue;
                            }
                        }
                        else if (TeamID == 2)
                        {
                            if (!isDead)
                            {
                                nameColor = Color.Red;
                            }
                            else
                            {
                                nameColor = Color.DarkRed;
                            }
                        }

                        //Im not really sure where to put this relative to teams and such so it can simply override all of them.
                        if (HuskInfectionState >= 0.5f || this.SpeciesName.ToLowerInvariant() == "humanhusk")
                        {
                            nameColor = new Color(255, 100, 255, 255);
                        }
                    }
                    else
                    {
                        if (TeamID == Character.Controlled.TeamID)
                        {
                            nameColor = Color.LightBlue;
                            if (IsDead)
                            {
                                nameColor = Color.DarkBlue;
                            }
                        }
                        if (TeamID != Character.Controlled.TeamID)
                        {
                            nameColor = Color.Red;
                            if (IsDead)
                            {
                                nameColor = Color.DarkRed;
                            }
                        }
                    }


                    if (Character.Controlled != null && TeamID != Character.Controlled.TeamID)
                    {
                        nameColor = Color.Red;
                        if (IsDead)
                        {
                            nameColor = Color.DarkRed;
                        }
                    }
                }
                else
                {
                    if (Character.Controlled != null)
                    {
                        if (TeamID != Character.Controlled.TeamID) nameColor = Color.Red;
                    }
                }

                GUI.Font.DrawString(spriteBatch, name, namePos + new Vector2(1.0f / cam.Zoom, 1.0f / cam.Zoom), Color.Black, 0.0f, Vector2.Zero, 1.0f / cam.Zoom, SpriteEffects.None, 0.001f);
                GUI.Font.DrawString(spriteBatch, name, namePos, nameColor * hudInfoAlpha, 0.0f, Vector2.Zero, 1.0f / cam.Zoom, SpriteEffects.None, 0.0f);

                if (GameMain.DebugDraw)
                {
                    GUI.Font.DrawString(spriteBatch, ID.ToString(), namePos - new Vector2(0.0f, 20.0f), Color.White);
                }
            }

            showinghealth = false;

            if (isDead) return;
            
            if (GameMain.NilMod.UseUpdatedCharHUD)
            {
                if (((health < maxHealth * 0.98f) || oxygen < 95f || bleeding >= 0.05f || (((AnimController.CurrentHull == null) ?
                    100.0f : Math.Min(AnimController.CurrentHull.LethalPressure, 100.0f)) > 10f && NeedsAir && !Shielded && PressureProtection == 0f) || HuskInfectionState > 0f || Stun > 0f) && hudInfoVisible)
                {
                    showinghealth = true;

                    //Basic Colour
                    Color baseoutlinecolour;
                    //Basic Flash Colour if fine
                    Color FlashColour;
                    //Final Calculated outline colour
                    Color outLineColour;

                    //Negative Colours
                    Color NegativeLow = new Color(145, 145, 145, 160);
                    Color NegativeHigh = new Color(25, 25, 25, 220);

                    //Health Colours
                    Color HealthPositiveHigh = new Color(0, 255, 0, 15);
                    Color HealthPositiveLow = new Color(255, 0, 0, 60);
                    //Oxygen Colours
                    Color OxygenPositiveHigh = new Color(0, 255, 255, 15);
                    Color OxygenPositiveLow = new Color(0, 0, 200, 60);
                    //Stun Colours
                    Color StunPositiveHigh = new Color(235, 135, 45, 100);
                    Color StunPositiveLow = new Color(204, 119, 34, 30);
                    //Bleeding Colours
                    Color BleedPositiveHigh = new Color(255, 50, 50, 100);
                    Color BleedPositiveLow = new Color(150, 50, 50, 15);
                    //Pressure Colours
                    Color PressurePositiveHigh = new Color(255, 255, 0, 100);
                    Color PressurePositiveLow = new Color(125, 125, 0, 15);

                    //Husk Colours
                    Color HuskPositiveHigh = new Color(255, 100, 255, 150);
                    Color HuskPositiveLow = new Color(125, 30, 125, 15);

                    float pressureFactor = (AnimController.CurrentHull == null) ?
                    100.0f : Math.Min(AnimController.CurrentHull.LethalPressure, 100.0f);
                    if ((PressureProtection > 0.0f && (WorldPosition.Y > GameMain.NilMod.PlayerCrushDepthOutsideHull || (WorldPosition.Y > GameMain.NilMod.PlayerCrushDepthInHull && CurrentHull != null))) || Shielded) pressureFactor = 0.0f;

                    if (IsRemotePlayer || this == Character.Controlled || AIController is HumanAIController)
                    {
                        //A players basic flash if ok
                        baseoutlinecolour = new Color(255, 255, 255, 15);
                        FlashColour = new Color(220, 220, 220, 15);

                        if (HuskInfectionState >= 0.98f) FlashColour = new Color(200, 0, 200, 255);
                        else if (HuskInfectionState > 0.5f) FlashColour = new Color(85, 0, 85, 150);
                        else if (HuskInfectionState > 0f) FlashColour = new Color(50, 0, 120, 50);
                        else if (pressureFactor > 80f) FlashColour = new Color(200, 200, 0, 100);
                        else if (bleeding > 0.45f) FlashColour = new Color(80, 30, 20, 100);
                        else if (pressureFactor > 45f) FlashColour = new Color(200, 200, 0, 100);
                        else if (health < 0f) FlashColour = new Color(25, 25, 25, 40);
                        else if (oxygen < 0f) FlashColour = new Color(40, 40, 255, 40);
                        else if (pressureFactor > 5f) FlashColour = new Color(200, 200, 0, 100);
                        else if (oxygen < 35f) FlashColour = new Color(40, 40, 255, 40);
                        else if (health < 25f) FlashColour = new Color(25, 25, 25, 40);
                        else if (oxygen < 70f) FlashColour = new Color(40, 40, 255, 40);
                        else if (health < 50f) FlashColour = new Color(25, 25, 25, 40);
                        else if (Stun >= 1f) FlashColour = new Color(5, 5, 5, 80);

                        if (IsUnconscious || Stun >= 5f) baseoutlinecolour = new Color(40, 40, 40, 35);
                    }
                    //Is an AI or well, not controlled by anybody, make their border different
                    else
                    {
                        baseoutlinecolour = new Color(40, 40, 40, 15);
                        FlashColour = new Color(5, 5, 5, 15);

                        //if (HuskInfectionState >= 2f) FlashColour = new Color(255, 0, 255, 255);
                        //else if (HuskInfectionState > 1f) FlashColour = new Color(200, 0, 200, 150);
                        //else if (HuskInfectionState > 0f) FlashColour = new Color(120, 0, 120, 100);
                        if (pressureFactor > 80f && NeedsAir) FlashColour = new Color(200, 200, 0, 100);
                        else if (bleeding > 1f) FlashColour = new Color(255, 10, 10, 100);
                        else if (pressureFactor > 45f && NeedsAir) FlashColour = new Color(200, 200, 0, 100);
                        else if (Stun >= 1f) FlashColour = new Color(10, 10, 10, 100);
                    }

                    if (GameMain.NilMod.CharFlashColourTime >= (NilMod.CharFlashColourRate / 2))
                    {
                        outLineColour = Color.Lerp(baseoutlinecolour, FlashColour, (GameMain.NilMod.CharFlashColourTime - (NilMod.CharFlashColourRate / 2)) / (NilMod.CharFlashColourRate / 2)) * hudInfoAlpha;
                    }
                    else
                    {
                        outLineColour = Color.Lerp(FlashColour, baseoutlinecolour, GameMain.NilMod.CharFlashColourTime / (NilMod.CharFlashColourRate / 2)) * hudInfoAlpha;
                    }



                    //Smooth out the Health bar movement a little c:
                    //if (LastHealthStatusVector == null || LastHealthStatusVector == Vector2.Zero) LastHealthStatusVector = new Vector2(pos.X - 20f, -pos.Y);
                    //if ((LastHealthStatusVector.X + 40f) - DrawPosition.X > 2.0f || (LastHealthStatusVector.X + 40f) - DrawPosition.X < -2.0f || (LastHealthStatusVector.Y - 70f) - pos.Y > 2.0f || (LastHealthStatusVector.Y - 70f) - pos.Y < -2.0f) LastHealthStatusVector = new Vector2(pos.X - 40f, -pos.Y);
                    //Vector2 healthBarPos = LastHealthStatusVector;

                    //Smooth out the Health bar movement a little c:

                    Vector2 namePos = Vector2.Zero;

                    if (info != null)
                    {
                        namePos = new Vector2(pos.X, pos.Y + 10.0f - (5.0f / cam.Zoom)) - GUI.Font.MeasureString(Info.Name) * 0.5f / cam.Zoom;
                    }
                    

                    Vector2 screenSize = new Vector2(GameMain.GraphicsWidth, GameMain.GraphicsHeight);
                    Vector2 viewportSize = new Vector2(cam.WorldView.Width, cam.WorldView.Height);
                    namePos.X -= cam.WorldView.X; namePos.Y += cam.WorldView.Y;
                    namePos *= screenSize / viewportSize;
                    namePos.X = (float)Math.Floor(namePos.X); namePos.Y = (float)Math.Floor(namePos.Y);
                    namePos *= viewportSize / screenSize;
                    namePos.X += cam.WorldView.X; namePos.Y -= cam.WorldView.Y;

                    if(info == null)
                    {
                        if (LastHealthStatusVector == null || LastHealthStatusVector == Vector2.Zero) LastHealthStatusVector = new Vector2(pos.X - 20f, -pos.Y);
                        if ((LastHealthStatusVector.X + 40f) - DrawPosition.X > 2.0f || (LastHealthStatusVector.X + 40f) - DrawPosition.X < -2.0f || (LastHealthStatusVector.Y - 70f) - pos.Y > 2.0f || (LastHealthStatusVector.Y - 70f) - pos.Y < -2.0f) LastHealthStatusVector = new Vector2(pos.X - 40f, -pos.Y);
                    }
                    else
                    {
                        if (LastHealthStatusVector == null || LastHealthStatusVector == Vector2.Zero) LastHealthStatusVector = new Vector2(namePos.X, -namePos.Y);
                        if ((LastHealthStatusVector.X + 20f) - namePos.X > 2.0f || (LastHealthStatusVector.X + 20f) - namePos.X < -2.0f || (LastHealthStatusVector.Y - 30f) - namePos.Y > 2.0f || (LastHealthStatusVector.Y - 30f) - namePos.Y < -2.0f) LastHealthStatusVector = new Vector2(namePos.X + 20f, -namePos.Y - 30f);
                    }

                    Vector2 healthBarPos = LastHealthStatusVector;

                    //GUI.DrawProgressBar(spriteBatch, healthBarPos, new Vector2(100.0f, 10.0f), health / maxHealth, Color.Lerp(Color.Red, Color.Green, health / maxHealth) * 0.8f);

                    //Health Bar (Keep visible)
                    if (Health >= 0f)
                    {
                        if ((NeedsAir && oxygen > 85f) || !NeedsAir)
                        {
                            GUI.DrawProgressBar(spriteBatch, healthBarPos, new Vector2(80.0f, 20.0f), health / maxHealth, Color.Lerp(HealthPositiveLow, HealthPositiveHigh, health / maxHealth)  * hudInfoAlpha, outLineColour, 2f, 0, "Left");
                        }
                        else
                        {
                            GUI.DrawProgressBar(spriteBatch, healthBarPos, new Vector2(80.0f, 10.0f), health / maxHealth, Color.Lerp(HealthPositiveLow, HealthPositiveHigh, health / maxHealth)  * hudInfoAlpha, outLineColour, 2f, 0, "Left");
                        }
                    }
                    //Health has gone below 0
                    else
                    {
                        if ((NeedsAir && oxygen > 85f) || !NeedsAir)
                        {
                            GUI.DrawProgressBar(spriteBatch, healthBarPos, new Vector2(80.0f, 20.0f), -(health / maxHealth), Color.Lerp(NegativeLow, NegativeHigh, -(health / maxHealth))  * hudInfoAlpha, outLineColour, 2f, 0, "Right");
                        }
                        else
                        {
                            GUI.DrawProgressBar(spriteBatch, healthBarPos, new Vector2(80.0f, 10.0f), -(health / maxHealth), Color.Lerp(NegativeLow, NegativeHigh, -(health / maxHealth))  * hudInfoAlpha, outLineColour, 2f, 0, "Right");
                        }
                    }

                    //Oxygen Bar
                    if (NeedsAir && (oxygen <= 85f && oxygen >= 0f))
                    {
                        GUI.DrawProgressBar(spriteBatch, new Vector2(healthBarPos.X, healthBarPos.Y - 10f), new Vector2(80.0f, 10.0f), oxygen / 100f, Color.Lerp(OxygenPositiveLow, OxygenPositiveHigh, oxygen / 100f)  * hudInfoAlpha, outLineColour, 2f, 0f, "Left");
                    }
                    //Oxygen has gone below 0
                    else if (NeedsAir && oxygen < 0f)
                    {
                        GUI.DrawProgressBar(spriteBatch, new Vector2(healthBarPos.X, healthBarPos.Y - 10f), new Vector2(80.0f, 10.0f), -(oxygen / 100f), Color.Lerp(NegativeLow, NegativeHigh, -(oxygen / 100f))  * hudInfoAlpha, outLineColour, 2f, 0f, "Right");
                    }

                    //Stun Bar
                    if (Stun > 1.0f && !IsUnconscious)
                    {
                        GUI.DrawProgressBar(spriteBatch, new Vector2(healthBarPos.X, healthBarPos.Y - 20f), new Vector2(80.0f, 10.0f), Stun / 60f, Color.Lerp(StunPositiveLow, StunPositiveHigh, Stun / 60f)  * hudInfoAlpha, outLineColour, 2f, 0f, "Left");
                    }

                    //Bleed Bar
                    if (bleeding > 0.0f)
                    {
                        GUI.DrawProgressBar(spriteBatch, new Vector2(healthBarPos.X, healthBarPos.Y + 10f), new Vector2(40.0f, 10.0f), bleeding / 5f, Color.Lerp(BleedPositiveLow, BleedPositiveHigh, bleeding / 5f)  * hudInfoAlpha, outLineColour, 2f, 0f, "Left");
                    }
                    //Pressure Bar
                    if (pressureFactor > 0.0f)
                    {
                        GUI.DrawProgressBar(spriteBatch, new Vector2(healthBarPos.X + 40f, healthBarPos.Y + 10f), new Vector2(40.0f, 10.0f), pressureFactor / 100f, Color.Lerp(PressurePositiveLow, PressurePositiveHigh, pressureFactor / 100f)  * hudInfoAlpha, outLineColour, 2f, 0f, "Right");
                    }
                    //Husk Bar
                    if (HuskInfectionState > 0.0f)
                    {
                        GUI.DrawProgressBar(spriteBatch, new Vector2(healthBarPos.X + 80f, healthBarPos.Y + ((pressureFactor > 0.0f) ? 10.0f : 0.0f)), new Vector2(20.0f, (pressureFactor > 0.0f) ? 30.0f : 20.0f), HuskInfectionState, Color.Lerp(HuskPositiveLow, HuskPositiveHigh, HuskInfectionState) * hudInfoAlpha, outLineColour, 2f, 0f, "Bottom");
                    }
                }
            }
            else
            {
                Vector2 healthBarPos = new Vector2(pos.X - 50, DrawPosition.Y + 80.0f);

                GUI.DrawProgressBar(spriteBatch, healthBarPos, new Vector2(100.0f, 15.0f), 
                    health / maxHealth, 
                    Color.Lerp(Color.Red, Color.Green, health / maxHealth) * 0.8f * hudInfoAlpha,
                    new Color(0.5f, 0.57f, 0.6f, 1.0f) * hudInfoAlpha,2f,0f,"Left");
            }
        }

        /// <summary>
        /// Creates a progress bar that's "linked" to the specified object (or updates an existing one if there's one already linked to the object)
        /// The progress bar will automatically fade out after 1 sec if the method hasn't been called during that time
        /// </summary>
        public HUDProgressBar UpdateHUDProgressBar(object linkedObject, Vector2 worldPosition, float progress, Color emptyColor, Color fullColor)
        {
            if (controlled != this) return null;

            HUDProgressBar progressBar = null;
            if (!hudProgressBars.TryGetValue(linkedObject, out progressBar))
            {
                progressBar = new HUDProgressBar(worldPosition, Submarine, emptyColor, fullColor);
                hudProgressBars.Add(linkedObject, progressBar);
            }

            progressBar.WorldPosition = worldPosition;
            progressBar.FadeTimer = Math.Max(progressBar.FadeTimer, 1.0f);
            progressBar.Progress = progress;

            return progressBar;
        }

        public void PlaySound(CharacterSound.SoundType soundType)
        {
            if (sounds == null || sounds.Count == 0) return;

            var matchingSounds = sounds.FindAll(s => s.Type == soundType);
            if (matchingSounds.Count == 0) return;

            var selectedSound = matchingSounds[Rand.Int(matchingSounds.Count)];
            selectedSound.Sound.Play(1.0f, selectedSound.Range, AnimController.WorldPosition);
        }

        partial void ImplodeFX()
        {
            Vector2 centerOfMass = AnimController.GetCenterOfMass();

            SoundPlayer.PlaySound("implode", 1.0f, 150.0f, WorldPosition);

            for (int i = 0; i < 10; i++)
            {
                Particle p = GameMain.ParticleManager.CreateParticle("waterblood",
                    ConvertUnits.ToDisplayUnits(centerOfMass) + Rand.Vector(5.0f),
                    Rand.Vector(10.0f));
                if (p != null) p.Size *= 2.0f;

                GameMain.ParticleManager.CreateParticle("bubbles",
                    ConvertUnits.ToDisplayUnits(centerOfMass) + Rand.Vector(5.0f),
                    new Vector2(Rand.Range(-50f, 50f), Rand.Range(-100f, 50f)));

                GameMain.ParticleManager.CreateParticle("gib",
                    WorldPosition + Rand.Vector(Rand.Range(0.0f, 50.0f)),
                    Rand.Range(0.0f, MathHelper.TwoPi),
                    Rand.Range(200.0f, 700.0f), null);
            }

            for (int i = 0; i < 30; i++)
            {
                GameMain.ParticleManager.CreateParticle("heavygib",
                    WorldPosition + Rand.Vector(Rand.Range(0.0f, 50.0f)),
                    Rand.Range(0.0f, MathHelper.TwoPi),
                    Rand.Range(50.0f, 500.0f), null);
            }
        }
    }
}
