using GenericModConfigMenu;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace AnimalNametags
{
    public class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration from the player.</summary>
        private ModConfig Config;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();

            Helper.Events.GameLoop.GameLaunched += (e, a) => OnGameLaunched(e, a);

            ObjectPatches.ModInstance = this;
            ObjectPatches.Config = this.Config;

            var harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.Patch(
                original: AccessTools.Method(type: typeof(StardewValley.FarmAnimal), nameof(StardewValley.FarmAnimal.draw), parameters: new[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.Animal_draw_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(type: typeof(StardewValley.Characters.Pet), nameof(StardewValley.Characters.Pet.draw), parameters: new[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.Animal_draw_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(type: typeof(StardewValley.Characters.Horse), nameof(StardewValley.Characters.Horse.draw), parameters: new[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.Animal_draw_Postfix))
            );
        }

        /// <summary>Add to Generic Mod Config Menu</summary>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );

            // add config options
            configMenu.AddKeybind(
                mod: this.ModManifest,
                name: () => Helper.Translation.Get("Options_ToggleKey_Name"),
                tooltip: () => Helper.Translation.Get("Options_ToggleKey_Tooltip"),
                getValue: () => this.Config.ToggleKey,
                setValue: value => this.Config.ToggleKey = value
            );
            // future improvement: if GMCM Options then use it instead
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("Options_TextColor_Name"),
                tooltip: () => Helper.Translation.Get("Options_TextColor_Tooltip"),
                getValue: () => Config.TextColor.ToUpper(),
                setValue: value => Config.TextColor = value.ToUpper()
            );
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("Options_BackgroundColor_Name"),
                tooltip: () => Helper.Translation.Get("Options_BackgroundColor_Tooltip"),
                getValue: () => Config.BackgroundColor.ToUpper(),
                setValue: value => Config.BackgroundColor = value.ToUpper()
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => Helper.Translation.Get("Options_BackgroundOpacity_Name"),
                tooltip: () => Helper.Translation.Get("Options_BackgroundOpacity_Tooltip"),
                getValue: () => this.Config.BackgroundOpacity,
                setValue: value => this.Config.BackgroundOpacity = (float)value,
                min: 0f,
                max: 1f
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("Options_AlsoApplyOpacityToText_Name"),
                tooltip: () => Helper.Translation.Get("Options_AlsoApplyOpacityToText_Tooltip"),
                getValue: () => Config.AlsoApplyOpacityToText,
                setValue: value => Config.AlsoApplyOpacityToText = value
            );
        }
    }
}