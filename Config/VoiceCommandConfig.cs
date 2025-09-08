using BepInEx.Configuration;

namespace MageArenaRussianVoice.Config
{
    public static class VoiceCommandConfig
    {
        public static ConfigEntry<string> FireballCommand;
        public static ConfigEntry<string> FrostBoltCommand;
        public static ConfigEntry<string> WormCommand;
        public static ConfigEntry<string> HoleCommand;
        public static ConfigEntry<string> MagicMissileCommand;
        public static ConfigEntry<string> MirrorCommand;
        
        public static ConfigEntry<string> RockCommand;
        public static ConfigEntry<string> WispCommand;
        public static ConfigEntry<string> BlastCommand;
        public static ConfigEntry<string> DivineCommand;
        public static ConfigEntry<string> BlinkCommand;
        public static ConfigEntry<string> ThunderboltCommand;
        public static ConfigEntry<string> BubbleCommand;
        public static ConfigEntry<string> DoomCommand;

        public static void Init(ConfigFile config)
        {
            FireballCommand = config.Bind("Commands", "Fireball", "огненный шар",
                "Russian command for Fireball spell (все варианты слова могут быть разделены через пробел)");

            FrostBoltCommand = config.Bind("Commands", "FrostBolt", "сосулька",
                "Russian command for Frost Bolt spell (все варианты слова могут быть разделены через пробел)");

            WormCommand = config.Bind("Commands", "Worm", "вход",
                "Russian command for Worm spell (все варианты слова могут быть разделены через пробел)");

            HoleCommand = config.Bind("Commands", "Hole", "выход",
                "Russian command for Hole spell (все варианты слова могут быть разделены через пробел)");

            MagicMissileCommand = config.Bind("Commands", "MagicMissile", "магический снаряд",
                "Russian command for Magic Missile spell (все варианты слова могут быть разделены через пробел)");

            MirrorCommand = config.Bind("Commands", "Mirror", "зеркало",
                "Russian command for Mirror spell (все варианты слова могут быть разделены через пробел)");

            RockCommand = config.Bind("AdditionalCommands", "Rock", "валун",
                "Russian command for Rock spell (все варианты слова могут быть разделены через пробел)");

            WispCommand = config.Bind("AdditionalCommands", "Wisp", "дух",
                "Russian command for Wisp spell (все варианты слова могут быть разделены через пробел)");

            BlastCommand = config.Bind("AdditionalCommands", "Blast", "тёмный луч",
                "Russian command for Blast spell (все варианты слова могут быть разделены через пробел)");

            DivineCommand = config.Bind("AdditionalCommands", "Divine", "божий свет",
                "Russian command for Divine spell (все варианты слова могут быть разделены через пробел)");

            BlinkCommand = config.Bind("AdditionalCommands", "Blink", "прыжок",
                "Russian command for Blink spell (все варианты слова могут быть разделены через пробел)");

            ThunderboltCommand = config.Bind("AdditionalCommands", "Thunderbolt", "гром",
                "Russian command for Thunderbolt spell (все варианты слова могут быть разделены через пробел)");

            BubbleCommand = config.Bind("AdditionalCommands", "Bubble", "пузырь",
                "Russian command for Bubble spell (все варианты слова могут быть разделены через пробел)");

            DoomCommand = config.Bind("AdditionalCommands", "Doom", "смерть",
                "Russian command for Doom spell (все варианты слова могут быть разделены через пробел)");
        }
    }
}