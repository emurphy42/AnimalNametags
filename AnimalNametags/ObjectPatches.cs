using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Characters;

namespace AnimalNametags
{
    internal class ObjectPatches
    {
        // initialized by ModEntry.cs
        public static ModEntry ModInstance;
        public static ModConfig Config;
        public static bool IsActive = true;

        private static Color ConvertFromHex(string s)
        {
            if (s.Length != 7)
            {
                return Color.Gray;
            }

            var r = Convert.ToInt32(s.Substring(1, 2), 16);
            var g = Convert.ToInt32(s.Substring(3, 2), 16);
            var b = Convert.ToInt32(s.Substring(5, 2), 16);
            return new Color(r, g, b);
        }

        public static void Animal_draw_Postfix(SpriteBatch b, NPC __instance)
        {
            if (!IsActive)
            {
                return;
            }

            var heightOffset = (__instance.GetType() == typeof(FarmAnimal)) ? 32 : -16;

            var font = Game1.content.Load<SpriteFont>("Fonts\\SmallFont", LocalizedContentManager.CurrentLanguageCode);
            var width = (int)font.MeasureString(__instance.Name).X;
            var height = (int)font.MeasureString(__instance.Name).Y;

            var x = (int)(__instance.StandingPixel.X - width / 2 - Game1.viewport.X);
            var y = (int)(__instance.StandingPixel.Y - (__instance.GetBoundingBox().Height * 2 + height * 2) + heightOffset - Game1.viewport.Y);

            var drawLayer = (float)__instance.StandingPixel.Y / 10000f;

            var textColor = ConvertFromHex(Config.TextColor);
            if (Config.AlsoApplyOpacityToText)
            {
                textColor *= Config.BackgroundOpacity;
            }

            var bgColor = ConvertFromHex(Config.BackgroundColor) * Config.BackgroundOpacity;

            b.Draw(
                texture: Game1.staminaRect,
                destinationRectangle: new Rectangle(x, y, width, height),
                sourceRectangle: null,
                color: bgColor,
                rotation: 0f,
                origin: Vector2.Zero,
                effects: SpriteEffects.None,
                layerDepth: drawLayer
            );
            b.DrawString(
                spriteFont: font,
                text: __instance.Name,
                position: new Vector2(x, y),
                color: textColor,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: 1f,
                effects: SpriteEffects.None,
                layerDepth: drawLayer + 0.001f
            );
        }
    }
}
