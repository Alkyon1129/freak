#nullable enable
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;

namespace PierrotTalkFair
{
    internal sealed class ModEntry : Mod
    {
        private const string ModDataKey = "You.PierrotTalkFair/IsPierrot";
        private const string SpriteAssetPath = "assets/pierrot_sprite.png";
        private const string PortraitAssetPath = "assets/pierrot_portrait.png";

        // "그 자리" 타일 좌표
        private const int SpawnTileX = 22;
        private const int SpawnTileY = 77;

        // 말 걸기 허용 범위(타일 사각형)
        private static readonly Rectangle TalkArea = new Rectangle(SpawnTileX - 1, SpawnTileY - 1, 3, 3);

        private NPC? pierrot;
        private Texture2D? portraitSheet;
        private bool didBaseTalk;

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.Player.Warped += OnWarped;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            didBaseTalk = false;
            pierrot = null;
            portraitSheet = Helper.ModContent.Load<Texture2D>(PortraitAssetPath);
        }

        private void OnWarped(object? sender, WarpedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            // 가을 16일 + 축제 중 + Town(축제 임시맵으로 교체됨)
            if (Game1.currentSeason != "fall" || Game1.dayOfMonth != 16)
                return;
            if (!Game1.isFestival())
                return;
            if (e.NewLocation?.NameOrUniqueName != "Town")
                return;

            SpawnOrEnsure(e.NewLocation);
        }

        private void SpawnOrEnsure(GameLocation location)
        {
            // 이미 있으면 패스
            if (pierrot != null && location.characters.Contains(pierrot))
                return;

            // 중복 제거(우리 모드가 만든 Pierrot만)
            location.characters.RemoveAll(c =>
                c is NPC n &&
                n.Name == "Pierrot" &&
                n.modData != null &&
                n.modData.ContainsKey(ModDataKey)
            );

            var sprite = new AnimatedSprite(
                textureName: Helper.ModContent.GetInternalAssetName(SpriteAssetPath).Name,
                currentFrame: 0,
                spriteWidth: 32,
                spriteHeight: 64
            );

            pierrot = new NPC(
                sprite: sprite,
                position: new Vector2(SpawnTileX, SpawnTileY) * Game1.tileSize,
                defaultMap: "Town",
                facingDir: 2,          // 아래를 보게
                name: "Pierrot",        // 표시 이름(대화창/말풍선)
                datable: false,
                schedule: null,
                portrait: portraitSheet
            )
            {
                canSocialize = true,
                ignoreMovementAnimation = true
            };

            // 번역 가능한 표시 이름(대화창 상단 이름)
            pierrot.displayName = Helper.Translation.Get("npc.display_name");

            // 우리 모드 생성 표시
            pierrot.modData[ModDataKey] = "true";

            // 48프레임 반복 애니메이션 (0~47 순차라고 가정)
            StartLoopAnimation(pierrot, startFrame: 0, frameCount: 48, frameDurationMs: 100);

            location.addCharacter(pierrot);
        }

        private static void StartLoopAnimation(NPC npc, int startFrame, int frameCount, int frameDurationMs)
        {
            var frames = new List<FarmerSprite.AnimationFrame>(frameCount);
            for (int i = 0; i < frameCount; i++)
                frames.Add(new FarmerSprite.AnimationFrame(startFrame + i, frameDurationMs));

            npc.Sprite.setCurrentAnimation(frames);
            npc.Sprite.loop = true;
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady || pierrot == null)
                return;
            if (!Game1.isFestival())
                return;
            if (Game1.currentLocation?.NameOrUniqueName != "Town")
                return;
            if (Game1.activeClickableMenu != null)
                return;
            if (!e.Button.IsActionButton())
                return;

            // 근처에서만 말 걸기
            Vector2 t = Game1.player.Tile;
            if (!TalkArea.Contains((int)t.X, (int)t.Y))
                return;

            if (!didBaseTalk)
            {
                didBaseTalk = true;

                // 기본 대사 ($0 = 좌상단 포트레이트)
                pierrot.setNewDialogue(Helper.Translation.Get("dialogue.base"));
                Game1.drawDialogue(pierrot);

                // 대화창 닫힌 뒤 선택지
                Helper.Events.GameLoop.UpdateTicked += WaitThenAsk;
                return;
            }

            AskChoice();
        }

        private void WaitThenAsk(object? sender, UpdateTickedEventArgs e)
        {
            if (Game1.dialogueUp)
                return;

            Helper.Events.GameLoop.UpdateTicked -= WaitThenAsk;
            AskChoice();
        }

        private void AskChoice()
        {
            if (pierrot == null) return;

            var responses = new Response[]
            {
                new Response("cheer", Helper.Translation.Get("choice.cheer")),
                new Response("boo",   Helper.Translation.Get("choice.boo"))
            };

            Game1.currentLocation.createQuestionDialogue(
                question: Helper.Translation.Get("choice.prompt"),
                answerChoices: responses,
                afterQuestion: OnAnswered
            );
        }

        private void OnAnswered(Farmer who, string answer)
        {
            if (pierrot == null) return;

            if (answer == "cheer")
            {
                // $1 = 우상단 포트레이트
                pierrot.setNewDialogue(Helper.Translation.Get("dialogue.cheer"));
                Game1.drawDialogue(pierrot);
            }
            else if (answer == "boo")
            {
                // $2 = 좌하단 포트레이트
                pierrot.setNewDialogue(Helper.Translation.Get("dialogue.boo"));
                Game1.drawDialogue(pierrot);
            }
        }
    }
}
