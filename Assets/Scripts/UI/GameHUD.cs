using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BurgerGame
{
    public class GameHUD : MonoBehaviour
    {
        // 16:9 Reference Dimensions
        private const float REF_WIDTH = 1920f;
        private const float REF_HEIGHT = 1080f;

        private float guiScale = 1f;
        private float guiOffsetX = 0f;
        private float guiOffsetY = 0f;
        private Matrix4x4 originalMatrix;

        // GUI Styles
        private GUIStyle headerStyle;
        private GUIStyle cardStyle;
        private GUIStyle titleStyle;
        private GUIStyle subtitleStyle;
        private GUIStyle textStyle;
        private GUIStyle boldTextStyle;
        private GUIStyle tipStyle;
        private GUIStyle moodStyle;
        private GUIStyle buttonStyle;
        private GUIStyle primaryButtonStyle;
        private GUIStyle modalStyle;

        // Textures
        private Texture2D whiteBoxTexture;
        private Texture2D texHappy;
        private Texture2D texNeutral;
        private Texture2D texAngry;

        // Custom User UI Textures (Loaded automatically if present in Sprites/UI/)
        private Texture2D texMainMenuBg;
        private Texture2D texGameLogo;
        private Texture2D texCardLevel1;
        private Texture2D texCardLevel2;
        private Texture2D texCardLevel3;
        private Texture2D texStarGold;
        private Texture2D texStarEmpty;

        // Main Menu / Lobby Custom UI Kit (ui_main_menu_sheet_blank.png)
        private Texture2D texMainMenuPanelBg;
        private Texture2D texMainMenuInfoBg;
        private Texture2D texMainMenuBtnPlay;
        private Texture2D texMainMenuBtnRecipe;
        private Texture2D texMainMenuBtnExit;
        private Texture2D texMainMenuBadgeStar;
        private Texture2D texMainMenuBtnAudio;
        private Texture2D texMainMenuBtnHelp;
        private bool showHelpModal = false;
        private int helpModalTab = 0; // 0 = Kontrol, 1 = Gameplay & Aturan
        private Vector2 helpScrollPos = Vector2.zero;
        private bool isDraggingHelpScroll = false;
        private float helpDragStartMouseY = 0f;
        private float helpDragStartScrollY = 0f;
        private Texture2D texTabBtnCyan;
        private Texture2D texTabBtnSalmon;
        private Texture2D texBurgerWrapped;

        // In-Game Top Stats HUD Kit
        private Texture2D texHudFrameClean;
        private Texture2D texHudLevelBadge;
        private Texture2D texHudTimerPill;
        private Texture2D texIconStopwatch;
        private Texture2D texHudProgressBg;
        private Texture2D texHudProgressFillCyan;
        public Font customFont;

        [Header("Customer Feedback Toast Textures")]
        public Texture2D customToastPerfectBg;
        public Texture2D customToastFailBg;
        private Texture2D texToastPerfectBg;
        private Texture2D texToastFailBg;

        private GUIStyle hudLevelBadgeStyle;
        private GUIStyle hudTimerStyle;
        private GUIStyle hudIncomeStyle;
        private GUIStyle hudStatsStyle;

        // Level Complete Screen Illustrated UI Kit
        private Texture2D texPanelCompletedBg;
        private Texture2D texStatsBoxBg;
        private Texture2D texTitleLevelSelesai;
        private Texture2D texBtnLevelSelanjutnya;
        private Texture2D texBtnCobaLagi;
        private Texture2D texBtnMenuUtama;
        private Texture2D texBtnGreenBlank;
        private Texture2D texBtnOrangeBlank;
        private Texture2D texBtnBlueBlank;
        private Texture2D texIconCheck;
        private Texture2D texIconCross;
        private Texture2D texIconTrophy;
        private Texture2D texIconPopper;

        // Level Select Custom UI Kit Assets
        private Texture2D texPanelSelectBg;
        private Texture2D texTitlePilihLevel;
        private Texture2D texCardMudah;
        private Texture2D texCardMenengah;
        private Texture2D texCardSulit;
        private Texture2D texBtnPlay1;
        private Texture2D texBtnPlay2;
        private Texture2D texBtnPlay3;
        private Texture2D texBtnKembali;
        private Texture2D texIconClock;
        private Texture2D texIconTarget;
        private Texture2D texLevel1Square;
        private Texture2D texLevel2Square;
        private Texture2D texLevel3Square;
        private GameLevel? selectedLevelDetail = null;
        private float levelDetailOpenTime = 0f;

        // Recipe Book Illustrated UI Kit
        private Texture2D texRecipeBookPanelBg;
        private Texture2D texRecipeTitle;
        private Texture2D texRecipeCardCyan;
        private Texture2D texRecipeCardPeach;
        private Texture2D texRecipeCardSalmon;
        private Texture2D texRecipePricePill;
        private Texture2D texRecipeIngredientSlot;

        // Mobile Controller Textures
        private Texture2D texJoystickBase;
        private Texture2D texJoystickKnob;
        private Texture2D texBtnActionInteract;
        private Texture2D texBtnActionChop;
        private Texture2D texBtnActionDrop;
        private Texture2D texBtnPauseHUD;
        private Texture2D texBtnPause;
        private Texture2D texBtnAudioOn;
        private Texture2D texBtnAudioOff;

        // Custom Order Card UI Textures (User provided in Gemini_Generated_Image_3dh2163dh2163dh2.jpg)
        private Texture2D texOrderCardBg;
        private Texture2D texProgressBarBg;
        private Texture2D texProgressBarFill;
        private Texture2D texIngredientSlot;
        private Texture2D texIngredientSlotChecked;

        // Custom Opening Curtain Texture (Uses scratch_sprites/tirai.jpg or Assets/Resources/Sprites/UI/tirai.jpg)
        public Texture2D customCurtainTex;
        private Texture2D texCurtain;

        // Custom Pause Modal UI Kit Textures (User can provide custom assets or will fallback smoothly)
        public Texture2D customPauseBg;
        public Texture2D customPauseTitle;
        public Texture2D customPauseBtnResume;
        public Texture2D customPauseBtnRestart;
        public Texture2D customPauseBtnMenu;

        private Texture2D texPauseBg;
        private Texture2D texPauseTitle;
        private Texture2D texPauseBtnResume;
        private Texture2D texPauseBtnRestart;
        private Texture2D texPauseBtnMenu;

        // Mobile Controller State
        private bool isDraggingJoystick = false;
        private int joystickPointerId = -1; // -1 = none, -99 = mouse, >=0 = touch fingerId
        private readonly Vector2 joystickCenter = new Vector2(230f, 850f);
        private Vector2 joystickCurrentPos = new Vector2(230f, 850f);
        private const float JOYSTICK_RADIUS = 76f;
        private const float JOYSTICK_BASE_SIZE = 240f;
        private const float JOYSTICK_KNOB_SIZE = 150f;

        // Mobile Action Buttons Rects / Centers
        private readonly Vector2 btnInteractCenter = new Vector2(1730f, 850f);
        private const float BTN_INTERACT_RADIUS = 85f;
        private float btnInteractPressTime = -99f;

        private readonly Vector2 btnChopCenter = new Vector2(1525f, 880f);
        private const float BTN_CHOP_RADIUS = 72f;
        private float btnChopPressTime = -99f;
        private bool isChopButtonHeld = false;
        private float lastChopTriggerTime = -99f;

        private readonly Rect pauseBtnRect = new Rect(1354f, 26f, 62f, 62f);
        private readonly Rect audioHudRect = new Rect(1282f, 26f, 62f, 62f);
        private float lastPauseToggleTime = -99f;
        private float lastAudioToggleTime = -99f;

        // Dark text styles for creamy modal
        private GUIStyle modalDarkTitleStyle;
        private GUIStyle modalDarkTextStyle;
        private GUIStyle modalDarkBoldStyle;

        private PlayerController cachedPlayer;

        // Pop-up Animation & State Tracking
        private readonly Stack<Matrix4x4> matrixStack = new Stack<Matrix4x4>();
        private readonly Stack<Color> colorStack = new Stack<Color>();
        private GameState lastKnownState = (GameState)(-1);
        private float stateTransitionTime = 0f;
        private string lastCustomerSpeech = "";
        private float speechChangeTime = -99f;

        private void Awake()
        {
            whiteBoxTexture = new Texture2D(2, 2);
            whiteBoxTexture.SetPixels(new Color[] { Color.white, Color.white, Color.white, Color.white });
            whiteBoxTexture.Apply();

            LoadAllTextures();
        }

        private void Update()
        {
            OrderManager om = OrderManager.Instance;
            if (om != null)
            {
                if (om.currentState != lastKnownState)
                {
                    lastKnownState = om.currentState;
                    stateTransitionTime = Time.unscaledTime;
                }
            }

            CustomerNPC npc = CustomerNPC.Instance;
            if (npc != null)
            {
                if (npc.currentSpeech != lastCustomerSpeech)
                {
                    lastCustomerSpeech = npc.currentSpeech;
                    speechChangeTime = Time.unscaledTime;
                }
            }

            UpdateMobileInput();
        }

        private float EaseOutBack(float t, float overshoot = 1.70158f)
        {
            t = Mathf.Clamp01(t);
            float t1 = t - 1f;
            return 1f + (overshoot + 1f) * (t1 * t1 * t1) + overshoot * (t1 * t1);
        }

        private void PushPopupTransform(Vector2 pivot, float scale, float alpha = 1f)
        {
            matrixStack.Push(GUI.matrix);
            colorStack.Push(GUI.color);

            scale = Mathf.Max(0.001f, scale);
            Matrix4x4 popupMatrix = Matrix4x4.TRS(
                new Vector3(pivot.x * (1f - scale), pivot.y * (1f - scale), 0f),
                Quaternion.identity,
                new Vector3(scale, scale, 1f)
            );
            GUI.matrix = GUI.matrix * popupMatrix;
            Color c = GUI.color;
            GUI.color = new Color(c.r, c.g, c.b, c.a * Mathf.Clamp01(alpha));
        }

        private void PopPopupTransform()
        {
            if (matrixStack.Count > 0) GUI.matrix = matrixStack.Pop();
            if (colorStack.Count > 0) GUI.color = colorStack.Pop();
        }

        private void DrawOutlinedLabel(Rect rect, string text, GUIStyle style, Color textColor, Color outlineColor, float outlineWidth = 1.2f)
        {
            Color prevNormalColor = style.normal.textColor;
            Color prevGuiColor = GUI.color;

            // Draw outline in 8 directions
            style.normal.textColor = outlineColor;
            GUI.color = outlineColor;

            GUI.Label(new Rect(rect.x - outlineWidth, rect.y, rect.width, rect.height), text, style);
            GUI.Label(new Rect(rect.x + outlineWidth, rect.y, rect.width, rect.height), text, style);
            GUI.Label(new Rect(rect.x, rect.y - outlineWidth, rect.width, rect.height), text, style);
            GUI.Label(new Rect(rect.x, rect.y + outlineWidth, rect.width, rect.height), text, style);

            float diag = outlineWidth * 0.707f;
            GUI.Label(new Rect(rect.x - diag, rect.y - diag, rect.width, rect.height), text, style);
            GUI.Label(new Rect(rect.x + diag, rect.y - diag, rect.width, rect.height), text, style);
            GUI.Label(new Rect(rect.x - diag, rect.y + diag, rect.width, rect.height), text, style);
            GUI.Label(new Rect(rect.x + diag, rect.y + diag, rect.width, rect.height), text, style);

            // Draw main text
            style.normal.textColor = textColor;
            GUI.color = textColor;
            GUI.Label(rect, text, style);

            // Restore colors
            style.normal.textColor = prevNormalColor;
            GUI.color = prevGuiColor;
        }

        private void LoadAllTextures()
        {
            texHappy = LoadTexture("Sprites/customer_happy");
            texNeutral = LoadTexture("Sprites/customer_neutral");
            texAngry = LoadTexture("Sprites/customer_angry");

            // Custom UI Assets
            texMainMenuBg = LoadTexture("Sprites/UI/ui_main_menu_bg") ?? LoadTexture("Sprites/ui_main_menu_bg");
            texGameLogo = LoadTexture("Sprites/UI/ui_game_logo") ?? LoadTexture("Sprites/ui_game_logo");
            texCardLevel1 = LoadTexture("Sprites/UI/ui_card_level1") ?? LoadTexture("Sprites/ui_card_level1");
            texCardLevel2 = LoadTexture("Sprites/UI/ui_card_level2") ?? LoadTexture("Sprites/ui_card_level2");
            texCardLevel3 = LoadTexture("Sprites/UI/ui_card_level3") ?? LoadTexture("Sprites/ui_card_level3");
            texStarGold = LoadTexture("Sprites/UI/ui_star_gold") ?? LoadTexture("Sprites/ui_star_gold");
            texStarEmpty = LoadTexture("Sprites/UI/ui_star_empty") ?? LoadTexture("Sprites/ui_star_empty");

            // Main Menu / Lobby Illustrated UI Kit (ui_main_menu_sheet_blank.png)
            texMainMenuPanelBg = LoadTexture("Sprites/UI/ui_main_panel_bg_tall") ?? LoadTexture("Sprites/UI/ui_main_panel_bg") ?? LoadTexture("Sprites/ui_main_panel_bg");
            texMainMenuInfoBg = LoadTexture("Sprites/UI/ui_info_subpanel_bg") ?? LoadTexture("Sprites/ui_info_subpanel_bg");
            texMainMenuBtnPlay = LoadTexture("Sprites/UI/ui_btn_mulai_main") ?? LoadTexture("Sprites/ui_btn_mulai_main");
            texMainMenuBtnRecipe = LoadTexture("Sprites/UI/ui_btn_buku_resep") ?? LoadTexture("Sprites/ui_btn_buku_resep");
            texMainMenuBtnExit = LoadTexture("Sprites/UI/ui_btn_keluar") ?? LoadTexture("Sprites/ui_btn_keluar");
            texMainMenuBadgeStar = LoadTexture("Sprites/UI/ui_badge_star") ?? LoadTexture("Sprites/ui_badge_star");
            texBtnAudioOn = LoadTexture("Sprites/UI/ui_btn_audio_sq_on") ?? LoadTexture("Sprites/ui_btn_audio_sq_on");
            texBtnAudioOff = LoadTexture("Sprites/UI/ui_btn_audio_sq_off") ?? LoadTexture("Sprites/ui_btn_audio_sq_off");
            texMainMenuBtnAudio = texBtnAudioOn ?? LoadTexture("Sprites/UI/ui_btn_audio") ?? LoadTexture("Sprites/ui_btn_audio");
            texMainMenuBtnHelp = LoadTexture("Sprites/UI/ui_btn_help") ?? LoadTexture("Sprites/ui_btn_help");

            // Level Complete Custom Assets
            texPanelCompletedBg = LoadTexture("Sprites/UI/ui_panel_completed_bg") ?? LoadTexture("Sprites/ui_panel_completed_bg");
            texStatsBoxBg = LoadTexture("Sprites/UI/ui_stats_box_bg") ?? LoadTexture("Sprites/ui_stats_box_bg");
            texTitleLevelSelesai = LoadTexture("Sprites/UI/ui_text_level_selesai") ?? LoadTexture("Sprites/ui_text_level_selesai");
            texBtnLevelSelanjutnya = LoadTexture("Sprites/UI/ui_btn_level_selanjutnya") ?? LoadTexture("Sprites/ui_btn_level_selanjutnya");
            texBtnCobaLagi = LoadTexture("Sprites/UI/ui_btn_coba_lagi") ?? LoadTexture("Sprites/ui_btn_coba_lagi");
            texBtnMenuUtama = LoadTexture("Sprites/UI/ui_btn_menu_utama") ?? LoadTexture("Sprites/ui_btn_menu_utama");
            texBtnGreenBlank = LoadTexture("Sprites/UI/ui_btn_green_blank") ?? LoadTexture("Sprites/ui_btn_green_blank");
            texBtnOrangeBlank = LoadTexture("Sprites/UI/ui_btn_orange_blank") ?? LoadTexture("Sprites/ui_btn_orange_blank");
            texBtnBlueBlank = LoadTexture("Sprites/UI/ui_btn_blue_blank") ?? LoadTexture("Sprites/ui_btn_blue_blank");
            texIconCheck = LoadTexture("Sprites/UI/ui_icon_check") ?? LoadTexture("Sprites/ui_icon_check");
            texIconCross = LoadTexture("Sprites/UI/ui_icon_cross") ?? LoadTexture("Sprites/ui_icon_cross");
            texIconTrophy = LoadTexture("Sprites/UI/ui_icon_trophy") ?? LoadTexture("Sprites/ui_icon_trophy");
            texIconPopper = LoadTexture("Sprites/UI/ui_icon_popper") ?? LoadTexture("Sprites/ui_icon_popper");

            // Top Stats In-Game HUD UI Kit
            texHudFrameClean = LoadTexture("Sprites/UI/ui_hud_frame_clean") ?? LoadTexture("Sprites/ui_hud_frame_clean");
            texHudLevelBadge = LoadTexture("Sprites/UI/ui_hud_level_badge") ?? LoadTexture("Sprites/ui_hud_level_badge");
            texHudTimerPill = LoadTexture("Sprites/UI/ui_hud_timer_pill") ?? LoadTexture("Sprites/ui_hud_timer_pill");
            texIconStopwatch = LoadTexture("Sprites/UI/ui_icon_stopwatch") ?? LoadTexture("Sprites/ui_icon_stopwatch");
            texHudProgressBg = LoadTexture("Sprites/UI/ui_hud_progress_bg") ?? LoadTexture("Sprites/ui_hud_progress_bg");
            texHudProgressFillCyan = LoadTexture("Sprites/UI/ui_hud_progress_fill_cyan") ?? LoadTexture("Sprites/ui_hud_progress_fill_cyan");

            EnsureCustomFont();

            // Level Select Custom UI Kit Assets
            texPanelSelectBg = LoadTexture("Sprites/UI/ui_panel_select_bg") ?? LoadTexture("Sprites/ui_panel_select_bg");
            texTitlePilihLevel = LoadTexture("Sprites/UI/ui_title_pilih_level") ?? LoadTexture("Sprites/ui_title_pilih_level");
            texCardMudah = LoadTexture("Sprites/UI/ui_card_mudah") ?? LoadTexture("Sprites/ui_card_mudah");
            texCardMenengah = LoadTexture("Sprites/UI/ui_card_menengah") ?? LoadTexture("Sprites/ui_card_menengah");
            texCardSulit = LoadTexture("Sprites/UI/ui_card_sulit") ?? LoadTexture("Sprites/ui_card_sulit");
            texBtnPlay1 = LoadTexture("Sprites/UI/ui_btn_play_1") ?? LoadTexture("Sprites/ui_btn_play_1");
            texBtnPlay2 = LoadTexture("Sprites/UI/ui_btn_play_2") ?? LoadTexture("Sprites/ui_btn_play_2");
            texBtnPlay3 = LoadTexture("Sprites/UI/ui_btn_play_3") ?? LoadTexture("Sprites/ui_btn_play_3");
            texBtnKembali = LoadTexture("Sprites/UI/ui_btn_kembali") ?? LoadTexture("Sprites/ui_btn_kembali");
            texIconClock = LoadTexture("Sprites/UI/ui_icon_clock") ?? LoadTexture("Sprites/ui_icon_clock");
            texIconTarget = LoadTexture("Sprites/UI/ui_icon_target") ?? LoadTexture("Sprites/ui_icon_target");
            EnsureLevelSquareTextures();

            // Recipe Book Illustrated UI Kit
            texRecipeBookPanelBg = LoadTexture("Sprites/UI/ui_recipe_book_panel_bg") ?? LoadTexture("Sprites/ui_recipe_book_panel_bg");
            texRecipeTitle = LoadTexture("Sprites/UI/ui_title_buku_resep_resmi") ?? LoadTexture("Sprites/ui_title_buku_resep_resmi");
            texRecipeCardCyan = LoadTexture("Sprites/UI/ui_recipe_card_frame_cyan") ?? LoadTexture("Sprites/ui_recipe_card_frame_cyan");
            texRecipeCardPeach = LoadTexture("Sprites/UI/ui_recipe_card_frame_peach") ?? LoadTexture("Sprites/ui_recipe_card_frame_peach");
            texRecipeCardSalmon = LoadTexture("Sprites/UI/ui_recipe_card_frame_salmon") ?? LoadTexture("Sprites/ui_recipe_card_frame_salmon");
            texRecipePricePill = LoadTexture("Sprites/UI/ui_recipe_price_pill") ?? LoadTexture("Sprites/ui_recipe_price_pill");
            texRecipeIngredientSlot = LoadTexture("Sprites/UI/ui_recipe_ingredient_slot") ?? LoadTexture("Sprites/ui_recipe_ingredient_slot");

            // Mobile Controller UI Kit Textures (control.jpg)
            texJoystickBase = LoadTexture("Sprites/UI/ui_joystick_base") ?? LoadTexture("Sprites/ui_joystick_base")
                           ?? LoadTexture("Sprites/UI/ui_analog_outer") ?? LoadTexture("Sprites/ui_analog_outer");
            texJoystickKnob = LoadTexture("Sprites/UI/ui_joystick_knob") ?? LoadTexture("Sprites/ui_joystick_knob")
                           ?? LoadTexture("Sprites/UI/ui_analog_inner") ?? LoadTexture("Sprites/ui_analog_inner");
            texBtnActionInteract = LoadTexture("Sprites/UI/ui_btn_action_interact") ?? LoadTexture("Sprites/ui_btn_action_interact")
                                ?? LoadTexture("Sprites/UI/ui_button_ambil_taruh") ?? LoadTexture("Sprites/ui_button_ambil_taruh");
            texBtnActionChop = LoadTexture("Sprites/UI/ui_btn_action_chop") ?? LoadTexture("Sprites/ui_btn_action_chop")
                            ?? LoadTexture("Sprites/UI/ui_button_potong") ?? LoadTexture("Sprites/ui_button_potong");
            texBtnActionDrop = LoadTexture("Sprites/UI/ui_btn_action_drop") ?? LoadTexture("Sprites/ui_btn_action_drop");
            texBtnPause = LoadPauseTexture("ui_btn_pause") ?? LoadTexture("Sprites/UI/ui_btn_pause") ?? LoadTexture("Sprites/ui_btn_pause");
            texBtnPauseHUD = (texBtnPause != null) ? texBtnPause : (LoadPauseTexture("ui_btn_pause_hud") ?? LoadTexture("Sprites/UI/ui_btn_pause_hud"));

            // Custom Order Card UI Textures
            texOrderCardBg = LoadTexture("Sprites/UI/ui_order_card_bg") ?? LoadTexture("Sprites/ui_order_card_bg");
            texProgressBarBg = LoadTexture("Sprites/UI/ui_progress_bar_bg") ?? LoadTexture("Sprites/ui_progress_bar_bg");
            texProgressBarFill = LoadTexture("Sprites/UI/ui_progress_bar_fill") ?? LoadTexture("Sprites/ui_progress_bar_fill");
            texIngredientSlot = LoadTexture("Sprites/UI/ui_ingredient_slot_single") ?? LoadTexture("Sprites/ui_ingredient_slot_single");
            texIngredientSlotChecked = LoadTexture("Sprites/UI/ui_ingredient_slot_checked") ?? LoadTexture("Sprites/ui_ingredient_slot_checked");

            // Pause Modal UI Textures (Loaded from ui_pause_spritesheet_all.png)
            EnsurePauseTextures();

            // Customer Feedback Toast Background Textures
            EnsureToastTextures();

            // Opening Curtain Texture (Loads scratch_sprites/tirai.jpg or Assets/Resources/Sprites/UI/tirai.jpg)
            EnsureCurtainTexture();
        }

        private long lastPauseSheetWriteTime = 0;

        private void EnsurePauseTextures()
        {
            string sheetPath = @"C:\Users\HYPE7\Downloads\ui_pause_spritesheet_all.png";
            long sheetTime = System.IO.File.Exists(sheetPath) ? System.IO.File.GetLastWriteTimeUtc(sheetPath).Ticks : 0;

            if (texPauseBg == null || texPauseBtnResume == null || texBtnPause == null || (sheetTime > 0 && sheetTime != lastPauseSheetWriteTime))
            {
                lastPauseSheetWriteTime = sheetTime;
                texPauseBg = (customPauseBg != null) ? customPauseBg : LoadPauseTexture("ui_pause_bg");
                texPauseTitle = (customPauseTitle != null) ? customPauseTitle : LoadPauseTexture("ui_pause_title");
                texPauseBtnResume = (customPauseBtnResume != null) ? customPauseBtnResume : LoadPauseTexture("ui_pause_btn_resume");
                texPauseBtnRestart = (customPauseBtnRestart != null) ? customPauseBtnRestart : LoadPauseTexture("ui_pause_btn_restart");
                texPauseBtnMenu = (customPauseBtnMenu != null) ? customPauseBtnMenu : LoadPauseTexture("ui_pause_btn_menu");
                texBtnPause = LoadPauseTexture("ui_btn_pause");
                texBtnPauseHUD = (texBtnPause != null) ? texBtnPause : LoadPauseTexture("ui_btn_pause_hud");

                Debug.Log($"[GameHUD] EnsurePauseTextures loaded: Bg={(texPauseBg != null)}, Title={(texPauseTitle != null)}, Resume={(texPauseBtnResume != null)}, Restart={(texPauseBtnRestart != null)}, Menu={(texPauseBtnMenu != null)}, BtnPause={(texBtnPause != null)}");
            }
        }

        private void EnsureLevelSquareTextures()
        {
            if (texLevel1Square == null) texLevel1Square = LoadPauseTexture("ui_level_1_square");
            if (texLevel2Square == null) texLevel2Square = LoadPauseTexture("ui_level_2_square");
            if (texLevel3Square == null) texLevel3Square = LoadPauseTexture("ui_level_3_square");
            if (texIconCross == null) texIconCross = LoadTexture("Sprites/UI/ui_icon_cross") ?? LoadTexture("Sprites/ui_icon_cross") ?? LoadPauseTexture("ui_icon_cross");
        }

        private void EnsureHelpTabTextures()
        {
            if (texTabBtnCyan == null) texTabBtnCyan = LoadPauseTexture("ui_tab_btn_active_cyan");
            if (texTabBtnSalmon == null) texTabBtnSalmon = LoadPauseTexture("ui_tab_btn_active_salmon");
            if (texStatsBoxBg == null) texStatsBoxBg = LoadTexture("Sprites/UI/ui_stats_box_bg") ?? LoadTexture("Sprites/ui_stats_box_bg");
            if (texBurgerWrapped == null) texBurgerWrapped = LoadTexture("Sprites/burger_wrapped") ?? LoadTexture("burger_wrapped");
        }

        private Texture2D LoadPauseTexture(string nameWithoutExt)
        {
            string projectRoot = System.IO.Directory.GetParent(Application.dataPath).FullName;
            string userDownloads = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), "Downloads");
            string[] directCandidates = new string[]
            {
                System.IO.Path.Combine(Application.dataPath, "Sprites/UI", nameWithoutExt + ".png"),
                System.IO.Path.Combine(Application.dataPath, "Resources/Sprites/UI", nameWithoutExt + ".png"),
                System.IO.Path.Combine(Application.dataPath, "Sprites", nameWithoutExt + ".png"),
                System.IO.Path.Combine(projectRoot, "scratch_sprites", nameWithoutExt + ".png"),
                System.IO.Path.Combine(userDownloads, nameWithoutExt + ".png")
            };

            foreach (string p in directCandidates)
            {
                if (System.IO.File.Exists(p))
                {
                    try
                    {
                        byte[] data = System.IO.File.ReadAllBytes(p);
                        if (data != null && data.Length > 0)
                        {
                            Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                            if (tex.LoadImage(data))
                            {
                                tex.name = nameWithoutExt;
                                tex.wrapMode = TextureWrapMode.Clamp;
                                tex.filterMode = FilterMode.Bilinear;
                                return tex;
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogWarning($"[GameHUD] Failed loading pause texture from {p}: {ex.Message}");
                    }
                }
            }

            return LoadTexture($"Sprites/UI/{nameWithoutExt}") ?? LoadTexture(nameWithoutExt);
        }

        private long lastToastPerfectWriteTime = 0;
        private long lastToastFailWriteTime = 0;

        private void EnsureToastTextures()
        {
            string pathPerfect = @"C:\Users\HYPE7\Downloads\ui_toast_perfect_bg.png";
            string pathFail = @"C:\Users\HYPE7\Downloads\ui_toast_fail_bg.png";

            long perfectTime = System.IO.File.Exists(pathPerfect) ? System.IO.File.GetLastWriteTimeUtc(pathPerfect).Ticks : 0;
            if (texToastPerfectBg == null || (perfectTime > 0 && perfectTime != lastToastPerfectWriteTime) || (texToastPerfectBg != null && texToastPerfectBg.width != 760))
            {
                lastToastPerfectWriteTime = perfectTime;
                texToastPerfectBg = LoadToastTexture("ui_toast_perfect_bg", pathPerfect);
            }

            long failTime = System.IO.File.Exists(pathFail) ? System.IO.File.GetLastWriteTimeUtc(pathFail).Ticks : 0;
            if (texToastFailBg == null || (failTime > 0 && failTime != lastToastFailWriteTime) || (texToastFailBg != null && texToastFailBg.width != 760))
            {
                lastToastFailWriteTime = failTime;
                texToastFailBg = LoadToastTexture("ui_toast_fail_bg", pathFail);
            }
        }

        private Texture2D LoadToastTexture(string nameWithoutExt, string explicitDownloadPath)
        {
            if (nameWithoutExt.Contains("perfect") && customToastPerfectBg != null) return customToastPerfectBg;
            if (nameWithoutExt.Contains("fail") && customToastFailBg != null) return customToastFailBg;

            if (!string.IsNullOrEmpty(explicitDownloadPath) && System.IO.File.Exists(explicitDownloadPath))
            {
                try
                {
                    byte[] data = System.IO.File.ReadAllBytes(explicitDownloadPath);
                    if (data != null && data.Length > 0)
                    {
                        Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                        if (tex.LoadImage(data))
                        {
                            tex.name = nameWithoutExt;
                            tex.wrapMode = TextureWrapMode.Clamp;
                            tex.filterMode = FilterMode.Bilinear;
                            return tex;
                        }
                    }
                }
                catch {}
            }

            string[] directCandidates = new string[]
            {
                System.IO.Path.Combine(Application.dataPath, "Sprites/UI", nameWithoutExt + ".png"),
                System.IO.Path.Combine(Application.dataPath, "Resources/Sprites/UI", nameWithoutExt + ".png"),
                System.IO.Path.Combine(Application.dataPath, "Sprites", nameWithoutExt + ".png")
            };

            foreach (string p in directCandidates)
            {
                if (System.IO.File.Exists(p))
                {
                    try
                    {
                        byte[] data = System.IO.File.ReadAllBytes(p);
                        if (data != null && data.Length > 0)
                        {
                            Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                            if (tex.LoadImage(data))
                            {
                                tex.name = nameWithoutExt;
                                tex.wrapMode = TextureWrapMode.Clamp;
                                tex.filterMode = FilterMode.Bilinear;
                                return tex;
                            }
                        }
                    }
                    catch {}
                }
            }

            return LoadTexture($"Sprites/UI/{nameWithoutExt}") ?? LoadTexture(nameWithoutExt);
        }

        private Texture2D LoadTexture(string path)
        {
            Texture2D tex = Resources.Load<Texture2D>(path);
            if (tex != null) return tex;

            Sprite sp = Resources.Load<Sprite>(path);
            if (sp != null && sp.texture != null) return sp.texture;

#if UNITY_EDITOR
            tex = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/{path}.png");
            if (tex != null) return tex;
            sp = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/{path}.png");
            if (sp != null && sp.texture != null) return sp.texture;

            tex = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/{path}.jpg");
            if (tex != null) return tex;
            sp = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/{path}.jpg");
            if (sp != null && sp.texture != null) return sp.texture;
#endif

            // Direct file system fallback
            try
            {
                string[] exts = new string[] { ".png", ".jpg" };
                string fileName = System.IO.Path.GetFileName(path);
                string projectRoot = System.IO.Directory.GetParent(Application.dataPath).FullName;

                string[] candidateBases = new string[]
                {
                    System.IO.Path.Combine(Application.dataPath, path),
                    System.IO.Path.Combine(Application.dataPath, "Resources", path),
                    System.IO.Path.Combine(Application.dataPath, "Sprites/UI", fileName),
                    System.IO.Path.Combine(Application.dataPath, "Resources/Sprites/UI", fileName),
                    System.IO.Path.Combine(projectRoot, "scratch_sprites/sliced_menu", fileName),
                    System.IO.Path.Combine(projectRoot, "scratch_sprites", fileName)
                };

                foreach (string candidateBase in candidateBases)
                {
                    foreach (string ext in exts)
                    {
                        string fullPath = candidateBase.EndsWith(ext, System.StringComparison.OrdinalIgnoreCase) 
                            ? candidateBase 
                            : candidateBase + ext;

                        if (System.IO.File.Exists(fullPath))
                        {
                            byte[] data = System.IO.File.ReadAllBytes(fullPath);
                            if (data != null && data.Length > 0)
                            {
                                Texture2D directTex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                                if (directTex.LoadImage(data))
                                {
                                    directTex.wrapMode = TextureWrapMode.Clamp;
                                    directTex.filterMode = FilterMode.Bilinear;
                                    return directTex;
                                }
                            }
                        }
                    }
                }
            }
            catch {}

            return null;
        }

        private Font LoadFontAsset(string name)
        {
            Font f = Resources.Load<Font>($"Fonts/{name}");
            if (f != null) return f;

#if UNITY_EDITOR
            f = UnityEditor.AssetDatabase.LoadAssetAtPath<Font>($"Assets/Fonts/{name}.ttf");
            if (f != null) return f;
            f = UnityEditor.AssetDatabase.LoadAssetAtPath<Font>($"Assets/Resources/Fonts/{name}.ttf");
            if (f != null) return f;
            f = UnityEditor.AssetDatabase.LoadAssetAtPath<Font>($"Assets/Fonts/{name}");
            if (f != null) return f;
#endif
            return null;
        }

        private void EnsureCustomFont()
        {
            if (customFont != null) return;
            customFont = LoadFontAsset("Fredoka") 
                      ?? LoadFontAsset("Fredoka-VariableFont_wdth,wght")
                      ?? LoadFontAsset("Fredoka-VariableFont_wdth,wght.ttf");
            ApplyCustomFontToStyles();
        }

        private void ApplyCustomFontToStyles()
        {
            if (customFont == null) return;
            if (titleStyle != null) titleStyle.font = customFont;
            if (subtitleStyle != null) subtitleStyle.font = customFont;
            if (textStyle != null) textStyle.font = customFont;
            if (boldTextStyle != null) boldTextStyle.font = customFont;
            if (tipStyle != null) tipStyle.font = customFont;
            if (moodStyle != null) moodStyle.font = customFont;
            if (buttonStyle != null) buttonStyle.font = customFont;
            if (primaryButtonStyle != null) primaryButtonStyle.font = customFont;
            if (modalDarkTitleStyle != null) modalDarkTitleStyle.font = customFont;
            if (modalDarkTextStyle != null) modalDarkTextStyle.font = customFont;
            if (modalDarkBoldStyle != null) modalDarkBoldStyle.font = customFont;
            if (hudLevelBadgeStyle != null) hudLevelBadgeStyle.font = customFont;
            if (hudTimerStyle != null) hudTimerStyle.font = customFont;
            if (hudIncomeStyle != null) hudIncomeStyle.font = customFont;
            if (hudStatsStyle != null) hudStatsStyle.font = customFont;
        }

        private void EnsureCurtainTexture()
        {
            if (texCurtain != null) return;

            // 1. Prioritize reading directly from user's explicit path: scratch_sprites/tirai.jpg
            texCurtain = LoadCurtainFromFile();
            if (texCurtain != null) return;

            // 2. Inspector assignment (using Unity overloaded != null check)
            if (customCurtainTex != null)
            {
                texCurtain = customCurtainTex;
                return;
            }

            // 3. Fallbacks from Resources and AssetDatabase
            texCurtain = LoadTexture("Sprites/UI/tirai") ?? LoadTexture("Sprites/tirai") ?? LoadTexture("tirai");
        }

        private Texture2D LoadCurtainFromFile()
        {
            try
            {
                string explicitUserPath = @"D:\eksperimen\Unity\gameNihKyanya\scratch_sprites\tirai.jpg";
                string[] candidates = new string[]
                {
                    System.IO.Path.Combine(Application.dataPath, "Resources/Sprites/UI/tirai.png"),
                    System.IO.Path.Combine(Application.dataPath, "Resources/Sprites/UI/tirai.jpg"),
                    System.IO.Path.Combine(Application.dataPath, "Sprites/UI/tirai.png"),
                    System.IO.Path.Combine(Application.dataPath, "Sprites/UI/tirai.jpg"),
                    explicitUserPath
                };

                foreach (string path in candidates)
                {
                    if (System.IO.File.Exists(path))
                    {
                        byte[] fileData = System.IO.File.ReadAllBytes(path);
                        if (fileData != null && fileData.Length > 0)
                        {
                            Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                            if (tex.LoadImage(fileData))
                            {
                                // If user provided the multi-curtain sheet (1376x768), crop the closed level curtain
                                if (tex.width == 1376 && tex.height == 768)
                                {
                                    int cropX = 713;
                                    int cropY = 768 - 376;
                                    int cropW = 604;
                                    int cropH = 344;
                                    Color[] pixels = tex.GetPixels(cropX, cropY, cropW, cropH);
                                    Texture2D croppedTex = new Texture2D(cropW, cropH, TextureFormat.RGBA32, false);
                                    croppedTex.name = "tirai_cropped";
                                    croppedTex.SetPixels(pixels);
                                    croppedTex.Apply();
                                    croppedTex.wrapMode = TextureWrapMode.Clamp;
                                    croppedTex.filterMode = FilterMode.Bilinear;
                                    Debug.Log($"<color=#2ECC71><b>[GameHUD]</b> Berhasil memuat dan meng-crop tirai dari {path} ({cropW}x{cropH})</color>");
                                    return croppedTex;
                                }

                                tex.name = "tirai_texture";
                                tex.wrapMode = TextureWrapMode.Clamp;
                                tex.filterMode = FilterMode.Bilinear;
                                Debug.Log($"<color=#2ECC71><b>[GameHUD]</b> Berhasil memuat gambar tirai dari {path} ({tex.width}x{tex.height})</color>");
                                return tex;
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("[GameHUD] LoadCurtainFromFile error: " + ex.Message);
            }
            return null;
        }

        private void ComputeGUIScaleParams()
        {
            float screenW = Screen.width;
            float screenH = Screen.height;

            float targetAspect = 16f / 9f;
            float currentAspect = screenW / (screenH > 0 ? screenH : 1f);

            if (currentAspect > targetAspect)
            {
                // Screen is wider than 16:9 -> fit to height, center horizontally
                guiScale = screenH / REF_HEIGHT;
                guiOffsetX = (screenW - REF_WIDTH * guiScale) * 0.5f;
                guiOffsetY = 0f;
            }
            else
            {
                // Screen is narrower than 16:9 (or exact 16:9) -> fit to width, center vertically
                guiScale = screenW / REF_WIDTH;
                guiOffsetX = 0f;
                guiOffsetY = (screenH - REF_HEIGHT * guiScale) * 0.5f;
            }
        }

        public Vector2 ScreenToGUI(Vector2 screenPos)
        {
            ComputeGUIScaleParams();
            float screenY_GUI = Screen.height - screenPos.y;
            float s = guiScale > 0.0001f ? guiScale : 1f;
            float x = (screenPos.x - guiOffsetX) / s;
            float y = (screenY_GUI - guiOffsetY) / s;
            return new Vector2(x, y);
        }

        private void Begin16x9GUI()
        {
            originalMatrix = GUI.matrix;
            ComputeGUIScaleParams();

            GUI.matrix = Matrix4x4.TRS(
                new Vector3(guiOffsetX, guiOffsetY, 0f), 
                Quaternion.identity, 
                new Vector3(guiScale, guiScale, 1f)
            );
        }

        private void End16x9GUI()
        {
            matrixStack.Clear();
            colorStack.Clear();
            GUI.matrix = originalMatrix;
        }

        private void InitStyles()
        {
            if (headerStyle != null) return;

            EnsureCustomFont();

            headerStyle = new GUIStyle(GUI.skin.box);
            headerStyle.normal.background = whiteBoxTexture;

            cardStyle = new GUIStyle(GUI.skin.box);
            cardStyle.normal.background = whiteBoxTexture;

            modalStyle = new GUIStyle(GUI.skin.box);
            modalStyle.normal.background = whiteBoxTexture;

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                font = customFont,
                fontSize = 24,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft
            };
            titleStyle.normal.textColor = Color.white;

            subtitleStyle = new GUIStyle(GUI.skin.label)
            {
                font = customFont,
                fontSize = 15,
                fontStyle = FontStyle.Italic,
                alignment = TextAnchor.MiddleLeft
            };
            subtitleStyle.normal.textColor = new Color(0.85f, 0.88f, 0.92f);

            textStyle = new GUIStyle(GUI.skin.label)
            {
                font = customFont,
                fontSize = 15,
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.MiddleLeft
            };
            textStyle.normal.textColor = Color.white;

            boldTextStyle = new GUIStyle(GUI.skin.label)
            {
                font = customFont,
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft
            };
            boldTextStyle.normal.textColor = Color.white;

            tipStyle = new GUIStyle(GUI.skin.label)
            {
                font = customFont,
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            tipStyle.normal.textColor = new Color(0.1f, 0.1f, 0.1f);

            moodStyle = new GUIStyle(GUI.skin.label)
            {
                font = customFont,
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleRight
            };

            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                font = customFont,
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };

            primaryButtonStyle = new GUIStyle(GUI.skin.button)
            {
                font = customFont,
                fontSize = 19,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };

            modalDarkTitleStyle = new GUIStyle(GUI.skin.label)
            {
                font = customFont,
                fontSize = 22,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            modalDarkTitleStyle.normal.textColor = new Color(0.25f, 0.20f, 0.18f);

            modalDarkTextStyle = new GUIStyle(GUI.skin.label)
            {
                font = customFont,
                fontSize = 16,
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.MiddleLeft
            };
            modalDarkTextStyle.normal.textColor = new Color(0.22f, 0.26f, 0.30f);

            modalDarkBoldStyle = new GUIStyle(GUI.skin.label)
            {
                font = customFont,
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft
            };
            modalDarkBoldStyle.normal.textColor = new Color(0.18f, 0.22f, 0.25f);

            // In-Game Top Stats HUD Dedicated Styles
            hudLevelBadgeStyle = new GUIStyle(GUI.skin.label)
            {
                font = customFont,
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            hudLevelBadgeStyle.normal.textColor = new Color(0.10f, 0.22f, 0.32f);

            hudTimerStyle = new GUIStyle(GUI.skin.label)
            {
                font = customFont,
                fontSize = 17,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            hudTimerStyle.normal.textColor = new Color(0.12f, 0.28f, 0.22f);

            hudIncomeStyle = new GUIStyle(GUI.skin.label)
            {
                font = customFont,
                fontSize = 19,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft
            };
            hudIncomeStyle.normal.textColor = new Color(0.12f, 0.12f, 0.12f);

            hudStatsStyle = new GUIStyle(GUI.skin.label)
            {
                font = customFont,
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft
            };
            hudStatsStyle.normal.textColor = new Color(0.20f, 0.24f, 0.28f);

            ApplyCustomFontToStyles();
        }

        private void OnGUI()
        {
            if (whiteBoxTexture == null)
            {
                whiteBoxTexture = new Texture2D(2, 2);
                whiteBoxTexture.SetPixels(new Color[] { Color.white, Color.white, Color.white, Color.white });
                whiteBoxTexture.Apply();
            }

            InitStyles();
            EnsureCustomFont();
            EnsureToastTextures();
            EnsurePauseTextures();
            if (customFont != null)
            {
                GUI.skin.font = customFont;
            }

            OrderManager om = OrderManager.Instance;
            if (om == null) return;

            if (om.currentState != lastKnownState)
            {
                lastKnownState = om.currentState;
                stateTransitionTime = Time.unscaledTime;
            }

            if (cachedPlayer == null)
            {
                cachedPlayer = Object.FindAnyObjectByType<PlayerController>();
            }

            // Apply 16:9 Matrix Scaling
            Begin16x9GUI();

            switch (om.currentState)
            {
                case GameState.MainMenu:
                    DrawMainMenu(om);
                    break;
                case GameState.LevelSelect:
                    DrawLevelSelect(om);
                    break;
                case GameState.RecipeBook:
                    DrawRecipeBook(om);
                    break;
                case GameState.Playing:
                    DrawInGameHUD(om);
                    break;
                case GameState.Paused:
                    DrawInGameHUD(om);
                    DrawPauseModal(om);
                    break;
                case GameState.LevelComplete:
                    DrawInGameHUD(om);
                    DrawLevelCompleteModal(om, true);
                    break;
                case GameState.GameOver:
                    DrawInGameHUD(om);
                    DrawLevelCompleteModal(om, false);
                    break;
            }

            End16x9GUI();
        }

        // =========================================================================
        // 1. IN-GAME HUD (16:9 SCALED)
        // =========================================================================
        private void DrawInGameHUD(OrderManager om)
        {
            // 1. Top Stats Bar & HUD Pause Button
            DrawTopStats(om);

            // 2. Active Customer Order Tickets
            DrawActiveOrders(om);

            // 3. Floating In-World Speech Bubble above Kasir Window
            DrawCustomerWorldSpeech(om);

            // 4. Floating Feedback Toasts
            DrawFeedbackToasts(om);

            // 5. Mobile Controls (Virtual Analog Joystick & Action Buttons)
            DrawMobileControls(om);

            // 6. Opening Screen Transition ("Layar Terbuka" / Roller Shutter)
            if (om.isOpeningTransition)
            {
                DrawOpeningCurtainTransition(om);
            }
        }

        private void DrawTopStats(OrderManager om)
        {
            int score = om.score;
            int delivered = om.deliveredCount;
            int failed = om.failedCount;
            float time = om.roundTimer;
            int target = om.activeConfig != null ? om.activeConfig.targetScore : 100;

            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);

            // Panel Top Right - 460 x 138 (aspect ratio matching 640x190 asset)
            float panelW = 460f;
            float panelH = 138f;
            Rect statsRect = new Rect(REF_WIDTH - panelW - 30f, 16f, panelW, panelH);

            // 1. HUD Audio On/Off Button (Immediately left of Pause button)
            bool isGameMuted = AudioManager.Instance != null && AudioManager.Instance.isMuted;
            Texture2D inGameAudioTex = isGameMuted ? (texBtnAudioOff ?? texMainMenuBtnAudio) : (texBtnAudioOn ?? texMainMenuBtnAudio);
            float audioPressScale = (Time.unscaledTime - lastAudioToggleTime < 0.15f) ? 0.90f : 1.0f;
            Rect drawAudioRect = new Rect(
                audioHudRect.x + audioHudRect.width * (1f - audioPressScale) * 0.5f,
                audioHudRect.y + audioHudRect.height * (1f - audioPressScale) * 0.5f,
                audioHudRect.width * audioPressScale,
                audioHudRect.height * audioPressScale
            );

            if (inGameAudioTex != null)
            {
                GUI.color = Color.white;
                GUI.DrawTexture(drawAudioRect, inGameAudioTex, ScaleMode.ScaleToFit);
            }
            else
            {
                GUI.color = isGameMuted ? new Color(0.95f, 0.45f, 0.45f, 0.95f) : new Color(0.35f, 0.82f, 0.95f, 0.95f);
                GUI.Box(drawAudioRect, GUIContent.none, cardStyle);
                GUI.color = Color.white;
                titleStyle.alignment = TextAnchor.MiddleCenter;
                titleStyle.fontSize = Mathf.RoundToInt(13 * audioPressScale);
                GUI.Label(drawAudioRect, isGameMuted ? "OFF" : "ON", titleStyle);
                titleStyle.alignment = TextAnchor.MiddleLeft;
            }

            if (GUI.Button(drawAudioRect, GUIContent.none, GUIStyle.none))
            {
                if (Time.unscaledTime - lastAudioToggleTime >= 0.30f)
                {
                    lastAudioToggleTime = Time.unscaledTime;
                    PlayButtonClick();
                    if (AudioManager.Instance != null) AudioManager.Instance.ToggleMute();
                    if (Event.current != null) Event.current.Use();
                }
            }

            // 2. HUD Pause Button (ui_btn_pause, single tap/click with tactile press feedback)
            Texture2D pauseTexToUse = (texBtnPause != null) ? texBtnPause : texBtnPauseHUD;
            float pausePressScale = (Time.unscaledTime - lastPauseToggleTime < 0.15f) ? 0.90f : 1.0f;
            Rect drawPauseRect = new Rect(
                pauseBtnRect.x + pauseBtnRect.width * (1f - pausePressScale) * 0.5f,
                pauseBtnRect.y + pauseBtnRect.height * (1f - pausePressScale) * 0.5f,
                pauseBtnRect.width * pausePressScale,
                pauseBtnRect.height * pausePressScale
            );

            if (pauseTexToUse != null)
            {
                GUI.color = Color.white;
                GUI.DrawTexture(drawPauseRect, pauseTexToUse, ScaleMode.ScaleToFit);
            }
            else
            {
                GUI.color = new Color(0.95f, 0.94f, 0.91f, 0.95f);
                GUI.Box(drawPauseRect, GUIContent.none, cardStyle);
                GUI.color = new Color(0.12f, 0.12f, 0.12f, 1f);
                titleStyle.alignment = TextAnchor.MiddleCenter;
                titleStyle.fontSize = Mathf.RoundToInt(26 * pausePressScale);
                GUI.Label(drawPauseRect, "⏸", titleStyle);
                titleStyle.alignment = TextAnchor.MiddleLeft;
            }

            // Only trigger pause when currently Playing (prevents double-triggering or unpausing from HUD)
            if (om.currentState == GameState.Playing)
            {
                if (GUI.Button(drawPauseRect, GUIContent.none, GUIStyle.none))
                {
                    if (Time.unscaledTime - lastPauseToggleTime >= 0.35f)
                    {
                        lastPauseToggleTime = Time.unscaledTime;
                        PlayButtonClick();
                        om.TogglePause();
                        if (Event.current != null) Event.current.Use();
                    }
                }
            }

            // 3. Stats Panel Background Frame (ui_hud_frame_clean)
            if (texHudFrameClean != null)
            {
                GUI.color = Color.white;
                GUI.DrawTexture(statsRect, texHudFrameClean, ScaleMode.StretchToFill);
            }
            else
            {
                GUI.color = new Color(0.96f, 0.95f, 0.93f, 0.96f);
                GUI.Box(statsRect, GUIContent.none, headerStyle);

                GUI.color = new Color(0.65f, 0.85f, 0.95f, 1f);
                GUI.Box(new Rect(statsRect.x, statsRect.y, 6, panelH), GUIContent.none, headerStyle);
            }

            // Content bounds inside frame (clear of the cyan stripe on left)
            float contentLeft = statsRect.x + 36f;
            float contentRight = statsRect.x + panelW - 18f;
            float contentWidth = contentRight - contentLeft;

            // Row 1: Level Badge (left) & Timer Pill (right)
            string levelTitle = (!string.IsNullOrEmpty(om.activeConfig?.levelTitle)) 
                ? om.activeConfig.levelTitle 
                : $"Level {(int)om.currentLevel}";
            string levelTag = levelTitle.ToUpper();

            float badgeW = 110f;
            float badgeH = 32f;
            Rect badgeRect = new Rect(contentLeft, statsRect.y + 12f, badgeW, badgeH);
            if (texHudLevelBadge != null)
            {
                GUI.color = Color.white;
                GUI.DrawTexture(badgeRect, texHudLevelBadge, ScaleMode.StretchToFill);
            }
            else
            {
                GUI.color = new Color(0.40f, 0.85f, 0.95f, 1f);
                GUI.Box(badgeRect, GUIContent.none, cardStyle);
            }
            GUI.color = Color.white;
            GUI.Label(badgeRect, $"<b>{levelTag}</b>", hudLevelBadgeStyle);

            // Timer Pill with Stopwatch Icon
            float timerPillW = 126f;
            float timerPillH = 38f;
            float timerPillX = contentRight - timerPillW;
            float timerPillY = statsRect.y + 10f;
            Rect timerPillRect = new Rect(timerPillX, timerPillY, timerPillW, timerPillH);

            if (texHudTimerPill != null)
            {
                GUI.color = Color.white;
                GUI.DrawTexture(timerPillRect, texHudTimerPill, ScaleMode.StretchToFill);
            }
            else
            {
                GUI.color = new Color(0.85f, 0.97f, 0.93f, 1f);
                GUI.Box(timerPillRect, GUIContent.none, cardStyle);
            }

            // Stopwatch Icon inside Pill
            float iconSwSize = 22f;
            Rect swRect = new Rect(timerPillRect.x + 8f, timerPillRect.y + (timerPillH - iconSwSize) * 0.5f, iconSwSize, iconSwSize);
            if (texIconStopwatch != null)
            {
                GUI.color = Color.white;
                GUI.DrawTexture(swRect, texIconStopwatch, ScaleMode.ScaleToFit);
            }
            else
            {
                GUI.color = Color.black;
                GUI.Label(swRect, "⏱", hudTimerStyle);
            }

            // Time digits inside Pill
            string timerHex = time < 20f ? "#C0392B" : (time < 45f ? "#B7950B" : "#1B5E20");
            Rect timerTextRect = new Rect(timerPillRect.x + 30f, timerPillRect.y, timerPillW - 34f, timerPillH);
            GUI.color = Color.white;
            GUI.Label(timerTextRect, $"<color={timerHex}><b>{minutes:00}:{seconds:00}</b></color>", hudTimerStyle);

            // Row 2: Pendapatan & Target
            float incY = statsRect.y + 48f;
            string targetColor = score >= target ? "#1E8449" : "#C06A0B";
            GUI.color = Color.white;
            GUI.Label(new Rect(contentLeft, incY, contentWidth, 26f), 
                $"<b>PENDAPATAN:</b> <color={targetColor}><b>${score}</b></color> <size=13>/ <b>${target}</b></size>", hudIncomeStyle);

            // Row 3: Served & Failed with Icons
            float statRowY = statsRect.y + 77f;
            float statIconSize = 22f;

            // Served / Check
            Rect chkRect = new Rect(contentLeft, statRowY, statIconSize, statIconSize);
            if (texIconCheck != null)
            {
                GUI.color = Color.white;
                GUI.DrawTexture(chkRect, texIconCheck, ScaleMode.ScaleToFit);
            }
            GUI.color = Color.white;
            GUI.Label(new Rect(chkRect.xMax + 6f, statRowY - 1f, 130f, 24f), 
                $"<b>Terlayani:</b> {delivered}", hudStatsStyle);

            // Failed / Cross
            Rect crsRect = new Rect(contentLeft + 160f, statRowY, statIconSize, statIconSize);
            if (texIconCross != null)
            {
                GUI.color = Color.white;
                GUI.DrawTexture(crsRect, texIconCross, ScaleMode.ScaleToFit);
            }
            GUI.color = Color.white;
            GUI.Label(new Rect(crsRect.xMax + 6f, statRowY - 1f, 130f, 24f), 
                $"<b>Gagal:</b> {failed}", hudStatsStyle);

            // Row 4: Progress Bar (ui_hud_progress_bg & ui_hud_progress_fill_cyan)
            float barW = contentWidth;
            float barH = 15f;
            float barX = contentLeft;
            float barY = statsRect.y + panelH - barH - 12f;

            if (texHudProgressBg != null)
            {
                GUI.color = Color.white;
                GUI.DrawTexture(new Rect(barX, barY, barW, barH), texHudProgressBg, ScaleMode.StretchToFill);
            }
            else
            {
                GUI.color = new Color(0.86f, 0.84f, 0.80f, 0.95f);
                GUI.Box(new Rect(barX, barY, barW, barH), GUIContent.none, headerStyle);
            }

            float progressRatio = Mathf.Clamp01((float)score / target);
            float fillW = barW * progressRatio;
            if (fillW > 2f)
            {
                if (texHudProgressFillCyan != null)
                {
                    GUI.BeginGroup(new Rect(barX, barY, fillW, barH));
                    GUI.color = Color.white;
                    GUI.DrawTexture(new Rect(0, 0, barW, barH), texHudProgressFillCyan, ScaleMode.StretchToFill);
                    GUI.EndGroup();
                }
                else
                {
                    GUI.color = score >= target ? new Color(0.65f, 0.90f, 0.75f) : new Color(1.0f, 0.82f, 0.60f);
                    GUI.Box(new Rect(barX, barY, fillW, barH), GUIContent.none, headerStyle);
                }
            }
        }

        private void DrawActiveOrders(OrderManager om)
        {
            if (om.activeOrders == null || om.activeOrders.Count == 0) return;

            float startX = 30f;
            float startY = 16f;
            float gap = 16f;

            float statsWidth = 460f;
            float availableWidth = REF_WIDTH - statsWidth - startX - 25f;
            int orderCount = Mathf.Max(1, om.activeOrders.Count);
            float cardWidth = Mathf.Clamp((availableWidth - (orderCount - 1) * gap) / orderCount, 230f, 290f);
            float cardHeight = cardWidth * (569f / 793f); // Keeps exact aspect ratio of the user's order card asset!

            // Plate ingredients held by player
            List<ItemType> plateIngredients = null;
            if (cachedPlayer != null && cachedPlayer.heldItem != null && cachedPlayer.heldItem.IsPlate())
            {
                plateIngredients = cachedPlayer.heldItem.stackedIngredients;
            }

            for (int i = 0; i < om.activeOrders.Count; i++)
            {
                ActiveOrder order = om.activeOrders[i];
                float timeRatio = Mathf.Clamp01(order.remainingTime / order.totalTime);

                // Shake animation if angry
                float shakeX = 0f;
                if (timeRatio < 0.25f)
                {
                    shakeX = Mathf.Sin((Time.time + i * 2f) * 28f) * 4f;
                }

                Rect cardRect = new Rect(startX + i * (cardWidth + gap) + shakeX, startY, cardWidth, cardHeight);

                // Pop-in bounce when order first arrives
                float orderAge = order.totalTime - order.remainingTime;
                bool isPopping = orderAge < 0.35f;
                if (isPopping)
                {
                    float ot = orderAge / 0.35f;
                    float cardScale = Mathf.Max(0.001f, Mathf.LerpUnclamped(0.25f, 1f, EaseOutBack(ot, 2.2f)));
                    Vector2 cardCenter = new Vector2(cardRect.x + cardWidth * 0.5f, cardRect.y + cardHeight * 0.5f);
                    PushPopupTransform(cardCenter, cardScale, Mathf.Clamp01(ot * 3f));
                }

                // Mood status
                Texture2D currentFace;
                string moodText;
                Color moodColor;

                if (timeRatio > 0.50f)
                {
                    currentFace = texHappy;
                    moodText = "Santai";
                    moodColor = new Color(0.65f, 0.90f, 0.75f); // Pastel mint (#A8E6CF)
                }
                else if (timeRatio > 0.25f)
                {
                    currentFace = texNeutral;
                    moodText = "Buruan";
                    moodColor = new Color(1.0f, 0.82f, 0.60f); // Pastel peach (#FFD3B6)
                }
                else
                {
                    currentFace = texAngry;
                    moodText = "MARAH!";
                    moodColor = new Color(1.0f, 0.65f, 0.65f); // Pastel blush rose (#FFAAA6)
                }

                // 1. Draw Card Background
                if (texOrderCardBg != null)
                {
                    GUI.color = Color.white;
                    GUI.DrawTexture(cardRect, texOrderCardBg, ScaleMode.StretchToFill);

                    // If angry, pulse soft pastel red overlay
                    if (timeRatio < 0.25f)
                    {
                        float pulse = (Mathf.Sin(Time.time * 12f) + 1f) * 0.5f;
                        GUI.color = new Color(1f, 0.65f, 0.65f, 0.15f + pulse * 0.18f);
                        GUI.DrawTexture(cardRect, texOrderCardBg, ScaleMode.StretchToFill);
                    }
                }
                else
                {
                    Color cardBgColor = timeRatio < 0.25f 
                        ? new Color(0.98f, 0.92f, 0.92f, 0.97f) 
                        : new Color(0.97f, 0.96f, 0.94f, 0.97f);

                    GUI.color = cardBgColor;
                    GUI.Box(cardRect, GUIContent.none, cardStyle);

                    Rect accentRect = new Rect(cardRect.x, cardRect.y, cardWidth, 5);
                    GUI.color = moodColor;
                    GUI.Box(accentRect, GUIContent.none, headerStyle);
                }

                // 2. Top Coral Header Section
                // Customer Mood Face Icon (Right side)
                float faceSize = cardHeight * 0.28f;
                Rect faceRect = new Rect(cardRect.x + cardWidth - cardWidth * 0.08f - faceSize, cardRect.y + cardHeight * 0.08f, faceSize, faceSize);
                if (currentFace != null)
                {
                    GUI.color = Color.white;
                    GUI.DrawTexture(faceRect, currentFace, ScaleMode.ScaleToFit);
                }

                // Order Number & Customer Name - Hitam Bold!
                float leftPad = cardWidth * 0.08f;
                float textWidth = cardWidth - leftPad - faceSize - cardWidth * 0.04f;

                modalDarkTextStyle.fontSize = Mathf.Clamp(Mathf.RoundToInt(cardHeight * 0.075f), 11, 14);
                modalDarkTextStyle.alignment = TextAnchor.MiddleLeft;
                GUI.color = new Color(0.12f, 0.12f, 0.12f);
                GUI.Label(new Rect(cardRect.x + leftPad, cardRect.y + cardHeight * 0.07f, textWidth, cardHeight * 0.11f), 
                    $"<b>#{order.orderNumber}</b> • <b>{order.customerName}</b>", modalDarkTextStyle);

                // Recipe Name - Hitam Bold!
                modalDarkBoldStyle.fontSize = Mathf.Clamp(Mathf.RoundToInt(cardHeight * 0.095f), 13, 17);
                modalDarkBoldStyle.alignment = TextAnchor.MiddleLeft;
                GUI.color = new Color(0.10f, 0.10f, 0.10f);
                GUI.Label(new Rect(cardRect.x + leftPad, cardRect.y + cardHeight * 0.18f, textWidth, cardHeight * 0.13f), 
                    $"<b>{order.recipe.recipeName}</b>", modalDarkBoldStyle);

                // Reward & Mood Text (Bold, enlarged font with crisp outline)
                modalDarkBoldStyle.fontSize = Mathf.Clamp(Mathf.RoundToInt(cardHeight * 0.096f), 14, 18);
                modalDarkBoldStyle.alignment = TextAnchor.MiddleLeft;
                Rect rewardRect = new Rect(cardRect.x + leftPad, cardRect.y + cardHeight * 0.305f, 74, cardHeight * 0.12f);
                DrawOutlinedLabel(rewardRect, $"<b>+${order.recipe.reward}</b>", modalDarkBoldStyle, new Color(0.12f, 0.50f, 0.22f), Color.white, 1.2f);

                moodStyle.fontSize = Mathf.Clamp(Mathf.RoundToInt(cardHeight * 0.092f), 13, 17);
                moodStyle.fontStyle = FontStyle.Bold;
                moodStyle.alignment = TextAnchor.MiddleLeft;
                Rect moodRect = new Rect(cardRect.x + leftPad + 76, cardRect.y + cardHeight * 0.305f, textWidth - 76, cardHeight * 0.12f);
                DrawOutlinedLabel(moodRect, $"<b>{moodText}</b>", moodStyle, new Color(0.12f, 0.12f, 0.12f), Color.white, 1.2f);

                // 3. Middle Groove: Timer Progress Bar (Fits neatly in the asset groove! - Pastel Colors)
                float barX = cardRect.x + cardWidth * 0.075f;
                float barY = cardRect.y + cardHeight * 0.448f;
                float barW = cardWidth * 0.85f;
                float barH = cardHeight * 0.085f;
                Rect barRect = new Rect(barX, barY, barW, barH);

                if (texProgressBarBg != null)
                {
                    GUI.color = Color.white;
                    GUI.DrawTexture(barRect, texProgressBarBg, ScaleMode.StretchToFill);
                }
                else
                {
                    GUI.color = new Color(0.92f, 0.90f, 0.88f, 0.95f); // pastel warm track
                    GUI.Box(barRect, GUIContent.none, headerStyle);
                }

                if (texProgressBarFill != null)
                {
                    if (timeRatio < 0.25f)
                    {
                        GUI.color = new Color(1.0f, 0.65f, 0.65f); // pastel blush rose
                    }
                    else if (timeRatio < 0.50f)
                    {
                        GUI.color = new Color(1.0f, 0.82f, 0.60f); // pastel peach
                    }
                    else
                    {
                        GUI.color = new Color(0.65f, 0.90f, 0.75f); // pastel mint
                    }

                    if (timeRatio > 0.005f)
                    {
                        float fillW = barRect.width * timeRatio;
                        GUI.BeginGroup(new Rect(barRect.x, barRect.y, fillW, barRect.height));
                        GUI.DrawTexture(new Rect(0, 0, barRect.width, barRect.height), texProgressBarFill, ScaleMode.StretchToFill);
                        GUI.EndGroup();
                    }
                }
                else
                {
                    Rect barFill = new Rect(barRect.x, barRect.y, barRect.width * timeRatio, barRect.height);
                    GUI.color = moodColor;
                    GUI.Box(barFill, GUIContent.none, headerStyle);
                }

                // 4. Bottom Cream Section: Ingredients Slots & Outlines
                List<ItemType> remainingPlate = plateIngredients != null ? new List<ItemType>(plateIngredients) : null;
                int ingCount = order.recipe.requiredIngredients.Count;

                float maxSlotH = cardHeight * 0.26f;
                float maxSlotW = (cardWidth * 0.85f - (ingCount - 1) * 6f) / Mathf.Max(1, ingCount);
                float slotSize = Mathf.Clamp(Mathf.Min(maxSlotH, maxSlotW), 26f, 44f);
                float slotGap = 6f;
                float totalRowW = ingCount * slotSize + (ingCount - 1) * slotGap;
                float startSlotX = cardRect.x + (cardWidth - totalRowW) * 0.5f;
                float slotY = cardRect.y + cardHeight * 0.565f;

                for (int j = 0; j < ingCount; j++)
                {
                    ItemType ing = order.recipe.requiredIngredients[j];
                    Rect slotRect = new Rect(startSlotX + j * (slotSize + slotGap), slotY, slotSize, slotSize);

                    bool isIngredientOnPlate = false;
                    if (remainingPlate != null && remainingPlate.Remove(ing))
                    {
                        isIngredientOnPlate = true;
                    }

                    // Draw the custom slot outline (checked green variant if on plate, single cyan/coral variant otherwise)
                    if (isIngredientOnPlate && texIngredientSlotChecked != null)
                    {
                        GUI.color = Color.white;
                        GUI.DrawTexture(slotRect, texIngredientSlotChecked, ScaleMode.StretchToFill);
                    }
                    else if (texIngredientSlot != null)
                    {
                        GUI.color = Color.white;
                        GUI.DrawTexture(slotRect, texIngredientSlot, ScaleMode.StretchToFill);
                    }
                    else
                    {
                        // Fallback
                        GUI.color = isIngredientOnPlate ? new Color(0.12f, 0.50f, 0.24f, 0.95f) : new Color(0.18f, 0.22f, 0.28f, 0.85f);
                        GUI.Box(slotRect, GUIContent.none, cardStyle);
                    }

                    // Ingredient item icon inside slot
                    float iconPad = slotSize * 0.12f;
                    Rect iconRect = new Rect(slotRect.x + iconPad, slotRect.y + iconPad, slotSize - iconPad * 2f, slotSize - iconPad * 2f);

                    Texture2D ingTex = ItemDatabase.GetTexture(ing);
                    if (ingTex != null)
                    {
                        GUI.color = Color.white;
                        GUI.DrawTexture(iconRect, ingTex, ScaleMode.ScaleToFit);
                    }
                    else
                    {
                        GUI.color = ItemDatabase.GetColor(ing);
                        GUI.Box(iconRect, GUIContent.none, cardStyle);
                    }

                    // Checkmark indicator if ready on plate
                    if (isIngredientOnPlate)
                    {
                        GUI.color = new Color(0.1f, 0.85f, 0.25f);
                        tipStyle.fontSize = Mathf.Clamp(Mathf.RoundToInt(slotSize * 0.38f), 10, 16);
                        float badgeSize = slotSize * 0.42f;
                        GUI.Label(new Rect(slotRect.x + slotRect.width - badgeSize * 0.75f, slotRect.y - badgeSize * 0.25f, badgeSize, badgeSize), "✔", tipStyle);
                    }
                }

                // 5. Remaining Time Countdown at Card Bottom (Shifted upwards, bold, enlarged font with outline)
                float timeY = cardRect.y + cardHeight * 0.825f; // Raised slightly upwards
                float timeH = cardHeight * 0.12f;
                Rect timeRect = new Rect(cardRect.x + cardWidth * 0.06f, timeY, cardWidth * 0.88f, timeH);

                modalDarkBoldStyle.fontSize = Mathf.Clamp(Mathf.RoundToInt(cardHeight * 0.092f), 14, 18);
                modalDarkBoldStyle.alignment = TextAnchor.MiddleCenter;

                bool isUrgent = timeRatio < 0.25f;
                string timeStr = isUrgent
                    ? $"⏱ Sisa waktu: {order.remainingTime:0.0}s!"
                    : $"⏱ Sisa waktu: {order.remainingTime:0.0}s";

                Color timeTextColor = isUrgent ? new Color(0.88f, 0.12f, 0.12f) : new Color(0.20f, 0.18f, 0.18f);
                Color timeOutlineColor = Color.white;

                DrawOutlinedLabel(timeRect, timeStr, modalDarkBoldStyle, timeTextColor, timeOutlineColor, 1.2f);
                modalDarkBoldStyle.alignment = TextAnchor.MiddleLeft;

                if (isPopping)
                {
                    PopPopupTransform();
                }
            }
        }

        private void DrawCustomerWorldSpeech(OrderManager om)
        {
            CustomerNPC npc = CustomerNPC.Instance;
            if (npc == null || Camera.main == null) return;
            if (!npc.IsPresent || string.IsNullOrEmpty(npc.currentSpeech)) return;

            // Offset bubble to the right of the bottom customer so it never blocks the delivery counter
            Vector3 worldPos = npc.transform.position + new Vector3(2.2f, 0.6f, 0);
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

            if (screenPos.z < 0) return;

            // Unproject from Screen to 16:9 Reference Space
            float guiX = (screenPos.x - guiOffsetX) / guiScale;
            float guiY = (Screen.height - screenPos.y - guiOffsetY) / guiScale;

            float bubbleW = 380f;
            float bubbleH = 64f;
            Rect bubbleRect = new Rect(guiX - bubbleW / 2f, guiY - bubbleH, bubbleW, bubbleH);

            // Pop-in bounce when speech changes
            float bubbleAge = Time.unscaledTime - speechChangeTime;
            bool isSpeechPopping = bubbleAge < 0.35f;
            if (isSpeechPopping)
            {
                float st = bubbleAge / 0.35f;
                float bScale = Mathf.Max(0.001f, Mathf.LerpUnclamped(0.3f, 1f, EaseOutBack(st, 2.2f)));
                Vector2 bubbleCenter = new Vector2(bubbleRect.x + bubbleW * 0.5f, bubbleRect.y + bubbleH * 0.5f);
                PushPopupTransform(bubbleCenter, bScale, Mathf.Clamp01(st * 3f));
            }

            EnsureToastTextures();

            Texture2D speechBg = null;
            if (npc.celebrationTimer > 0f)
            {
                speechBg = npc.isHappyCelebration ? texToastPerfectBg : texToastFailBg;
            }

            if (speechBg != null)
            {
                GUI.color = Color.white;
                GUI.DrawTexture(bubbleRect, speechBg);

                // Text inside custom feedback background
                GUI.color = new Color(0.12f, 0.12f, 0.14f, 1f);
                modalDarkBoldStyle.fontSize = 14;
                GUI.Label(new Rect(bubbleRect.x + 38f, bubbleRect.y + 4f, bubbleW - 56f, bubbleH - 8f), 
                    $"<b>{npc.currentSpeech}</b>", modalDarkBoldStyle);
            }
            else
            {
                // Balloon Frame - Soft Pastel Cream
                GUI.color = new Color(0.97f, 0.96f, 0.94f, 0.97f);
                GUI.Box(bubbleRect, GUIContent.none, cardStyle);

                // Balloon Left Accent - Soft Pastel Sky Mint
                GUI.color = new Color(0.65f, 0.85f, 0.95f, 1f);
                GUI.Box(new Rect(bubbleRect.x, bubbleRect.y, 6, bubbleH), GUIContent.none, headerStyle);

                // Text - Hitam Bold!
                GUI.color = new Color(0.12f, 0.12f, 0.12f, 1f);
                modalDarkBoldStyle.fontSize = 14;
                GUI.Label(new Rect(bubbleRect.x + 14, bubbleRect.y + 6, bubbleW - 24, bubbleH - 12), 
                    $"<b>{npc.currentSpeech}</b>", modalDarkBoldStyle);
            }

            if (isSpeechPopping)
            {
                PopPopupTransform();
            }
        }

        private void DrawFeedbackToasts(OrderManager om)
        {
            if (om.activeToasts == null || om.activeToasts.Count == 0) return;

            EnsureToastTextures();

            float startY = 95f;
            float w = 760f;
            float h = 88f;
            float x = (REF_WIDTH - w) / 2f;

            for (int i = 0; i < om.activeToasts.Count; i++)
            {
                FeedbackToast toast = om.activeToasts[i];
                float alpha = Mathf.Clamp01(toast.timer / 0.4f);

                Rect r = new Rect(x, startY + i * (h + 10f), w, h);

                float toastAge = toast.maxTimer - toast.timer;
                bool isToastPopping = toastAge < 0.25f;
                if (isToastPopping)
                {
                    float tt = toastAge / 0.25f;
                    float tScale = Mathf.Max(0.001f, Mathf.LerpUnclamped(0.5f, 1f, EaseOutBack(tt, 1.8f)));
                    Vector2 tCenter = new Vector2(r.x + w * 0.5f, r.y + h * 0.5f);
                    PushPopupTransform(tCenter, tScale, 1f);
                }

                bool isFail = toast.type == ToastType.Fail 
                    || toast.message.StartsWith("❌") 
                    || toast.message.StartsWith("X ")
                    || (toast.color.r > 0.8f && toast.color.g < 0.5f);

                Texture2D toastBg = isFail ? texToastFailBg : texToastPerfectBg;

                if (toastBg != null)
                {
                    GUI.color = new Color(1f, 1f, 1f, alpha);
                    GUI.DrawTexture(r, toastBg);
                }
                else
                {
                    // Pastel Cream Card Background fallback only if missing
                    GUI.color = new Color(0.97f, 0.96f, 0.94f, 0.96f * alpha);
                    GUI.Box(r, GUIContent.none, cardStyle);

                    Color pastelAccent = Color.Lerp(toast.color, Color.white, 0.35f);
                    GUI.color = new Color(pastelAccent.r, pastelAccent.g, pastelAccent.b, alpha);
                    GUI.Box(new Rect(r.x, r.y, 8, h), GUIContent.none, headerStyle);
                }

                // Toast message text with customFont (Fredoka)
                boldTextStyle.fontSize = 18;
                boldTextStyle.alignment = TextAnchor.MiddleLeft;
                GUI.color = new Color(0.12f, 0.12f, 0.14f, alpha);
                GUI.Label(new Rect(r.x + 44f, r.y + 4f, w - 74f, h - 8f), $"<b>{toast.message}</b>", boldTextStyle);

                if (isToastPopping)
                {
                    PopPopupTransform();
                }
            }
        }

        private void DrawBottomControls(OrderManager om)
        {
            float w = 820f;
            float h = 48f;
            Rect rect = new Rect((REF_WIDTH - w) / 2f, REF_HEIGHT - h - 14f, w, h);

            GUI.color = new Color(0.96f, 0.95f, 0.93f, 0.94f);
            GUI.Box(rect, GUIContent.none, headerStyle);

            boldTextStyle.fontSize = 14;
            GUI.color = new Color(0.12f, 0.12f, 0.12f, 1f);
            GUI.Label(new Rect(rect.x + 16, rect.y + 4, rect.width - 32, 20), 
                "<b>KONTROL:</b>  [Analog / WASD] Gerak   |   [Cyan / E] Ambil/Taruh   |   [Oranye / F] Potong   |   Buang bahan di Tong Sampah", boldTextStyle);
            GUI.Label(new Rect(rect.x + 16, rect.y + 24, rect.width - 32, 20), 
                "<color=#555555><b>⏸ Tombol Jeda di kanan atas / [ESC]   |   [R] Restart Ronde</b></color>", boldTextStyle);
        }

        // =========================================================================
        // MOBILE ONSCREEN CONTROLS (VIRTUAL JOYSTICK & ACTION BUTTONS)
        // =========================================================================
        private void DrawMobileControls(OrderManager om)
        {
            // 1. Virtual Joystick (Bottom-Left)
            Rect baseRect = new Rect(joystickCenter.x - JOYSTICK_BASE_SIZE * 0.5f, joystickCenter.y - JOYSTICK_BASE_SIZE * 0.5f, JOYSTICK_BASE_SIZE, JOYSTICK_BASE_SIZE);
            if (texJoystickBase != null)
            {
                GUI.color = new Color(1f, 1f, 1f, 0.92f);
                GUI.DrawTexture(baseRect, texJoystickBase, ScaleMode.ScaleToFit);
            }
            else
            {
                GUI.color = new Color(0.12f, 0.15f, 0.20f, 0.70f);
                GUI.Box(baseRect, GUIContent.none, cardStyle);
            }

            // Joystick Knob (Synchronized position)
            bool isKnobDeflected = Vector2.Distance(joystickCurrentPos, joystickCenter) > 4f;
            float knobScale = (isDraggingJoystick || isKnobDeflected) ? 1.04f : 1.0f;
            float knobDisplaySize = JOYSTICK_KNOB_SIZE * knobScale;
            Rect knobRect = new Rect(joystickCurrentPos.x - knobDisplaySize * 0.5f, joystickCurrentPos.y - knobDisplaySize * 0.5f, knobDisplaySize, knobDisplaySize);
            if (texJoystickKnob != null)
            {
                GUI.color = (isDraggingJoystick || isKnobDeflected) ? Color.white : new Color(0.92f, 0.96f, 1f, 0.90f);
                GUI.DrawTexture(knobRect, texJoystickKnob, ScaleMode.ScaleToFit);
            }
            else
            {
                GUI.color = (isDraggingJoystick || isKnobDeflected) ? new Color(0.2f, 0.85f, 0.95f, 0.95f) : new Color(0.3f, 0.5f, 0.7f, 0.85f);
                GUI.Box(knobRect, GUIContent.none, modalStyle);
            }

            // Joystick Subtitle
            tipStyle.alignment = TextAnchor.MiddleCenter;
            tipStyle.fontSize = 13;
            Color gerakColor = isKnobDeflected ? new Color(0.35f, 0.95f, 1f, 0.95f) : new Color(0.85f, 0.9f, 0.95f, 0.75f);
            DrawOutlinedLabel(new Rect(joystickCenter.x - 70f, joystickCenter.y + JOYSTICK_BASE_SIZE * 0.5f + 2f, 140f, 20f), 
                "<b>GERAK</b>", tipStyle, gerakColor, new Color(0f, 0f, 0f, 0.8f), 1.0f);
            tipStyle.fontSize = 11;
            DrawOutlinedLabel(new Rect(joystickCenter.x - 70f, joystickCenter.y + JOYSTICK_BASE_SIZE * 0.5f + 20f, 140f, 16f), 
                "[WASD / Analog]", tipStyle, new Color(0.8f, 0.88f, 0.95f, 0.75f), new Color(0f, 0f, 0f, 0.8f), 1.0f);

            // 2. Action Buttons (Bottom-Right)
            // (a) AMBIL / TARUH (Primary Action) - Cyan Button
            bool isInteractPressed = (Time.unscaledTime - btnInteractPressTime < 0.16f) || Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Space);
            float interactScale = isInteractPressed ? 0.88f : 1f;
            DrawCircularActionButton(btnInteractCenter, BTN_INTERACT_RADIUS * interactScale, texBtnActionInteract, 
                new Color(0.15f, 0.75f, 0.85f), "AMBIL / TARUH", "E / Spasi", isInteractPressed);

            // (b) POTONG (Chop Action) - Coral Orange Button
            bool isChopPressed = (Time.unscaledTime - btnChopPressTime < 0.16f) || isChopButtonHeld || Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.C);
            float chopScale = isChopPressed ? 0.88f : 1f;
            DrawCircularActionButton(btnChopCenter, BTN_CHOP_RADIUS * chopScale, texBtnActionChop, 
                new Color(0.95f, 0.45f, 0.35f), "POTONG", "F", isChopPressed);
        }

        private void DrawCircularActionButton(Vector2 center, float radius, Texture2D texture, Color fallbackColor, string label, string keyHint, bool isPressed)
        {
            float diameter = radius * 2f;
            Rect rect = new Rect(center.x - radius, center.y - radius, diameter, diameter);

            if (texture != null)
            {
                GUI.color = isPressed ? new Color(1.15f, 1.15f, 1.15f, 1.0f) : Color.white;
                GUI.DrawTexture(rect, texture, ScaleMode.ScaleToFit);
            }
            else
            {
                GUI.color = isPressed ? new Color(fallbackColor.r * 1.2f, fallbackColor.g * 1.2f, fallbackColor.b * 1.2f, 0.95f) 
                                      : new Color(fallbackColor.r, fallbackColor.g, fallbackColor.b, 0.88f);
                GUI.Box(rect, GUIContent.none, cardStyle);
            }

            // Button label text below button with outline
            boldTextStyle.alignment = TextAnchor.MiddleCenter;
            boldTextStyle.fontSize = radius >= 70f ? 15 : 13;
            Color labelColor = isPressed ? new Color(1f, 0.95f, 0.4f, 1f) : Color.white;
            DrawOutlinedLabel(new Rect(center.x - 90f, center.y + (isPressed ? radius + 1f : radius + 2f), 180f, 20f), 
                $"<b>{label}</b>", boldTextStyle, labelColor, new Color(0f, 0f, 0f, 0.85f), 1.2f);

            tipStyle.alignment = TextAnchor.MiddleCenter;
            tipStyle.fontSize = 11;
            Color hintColor = isPressed ? new Color(1f, 0.85f, 0.3f, 0.95f) : new Color(0.8f, 0.88f, 0.95f, 0.75f);
            DrawOutlinedLabel(new Rect(center.x - 60f, center.y + (isPressed ? radius + 19f : radius + 20f), 120f, 16f), 
                $"[{keyHint}]", tipStyle, hintColor, new Color(0f, 0f, 0f, 0.8f), 1.0f);
            boldTextStyle.alignment = TextAnchor.MiddleLeft;
        }

        private void UpdateMobileInput()
        {
            OrderManager om = OrderManager.Instance;
            if (om == null || om.currentState != GameState.Playing || om.isOpeningTransition)
            {
                if (isDraggingJoystick) ReleaseJoystick();
                isChopButtonHeld = false;
                return;
            }

            if (cachedPlayer == null)
            {
                cachedPlayer = PlayerController.Instance ?? Object.FindAnyObjectByType<PlayerController>();
            }

            // Synchronize PC keyboard inputs for buttons (E / Space, F / C)
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))
            {
                btnInteractPressTime = Time.unscaledTime;
            }
            if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.C))
            {
                btnChopPressTime = Time.unscaledTime;
            }

            // Continuous Chop handling if chop button held
            if (isChopButtonHeld && Time.unscaledTime - lastChopTriggerTime >= 0.26f)
            {
                lastChopTriggerTime = Time.unscaledTime;
                if (cachedPlayer != null) cachedPlayer.TriggerAlternateAction();
            }

            // 1. Mobile Multi-Touch Handling (Android device / Remote touch)
            if (Input.touchCount > 0)
            {
                bool joystickActive = false;
                bool chopActive = false;

                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch touch = Input.GetTouch(i);
                    Vector2 guiPos = ScreenToGUI(touch.position);

                    // A. Check Joystick Touch
                    if (touch.fingerId == joystickPointerId)
                    {
                        if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                        {
                            joystickActive = true;
                            UpdateJoystickDrag(guiPos);
                        }
                        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                        {
                            ReleaseJoystick();
                        }
                    }
                    else if (!isDraggingJoystick && touch.phase == TouchPhase.Began)
                    {
                        if (Vector2.Distance(guiPos, joystickCenter) <= JOYSTICK_RADIUS * 1.5f)
                        {
                            isDraggingJoystick = true;
                            joystickPointerId = touch.fingerId;
                            joystickActive = true;
                            UpdateJoystickDrag(guiPos);
                            continue;
                        }
                    }

                    // B. Check Action Buttons
                    if (Vector2.Distance(guiPos, btnChopCenter) <= BTN_CHOP_RADIUS)
                    {
                        if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
                        {
                            chopActive = true;
                        }
                    }

                    if (touch.phase == TouchPhase.Began)
                    {
                        ProcessButtonTap(guiPos, om);
                    }
                }

                isChopButtonHeld = chopActive;
                if (isDraggingJoystick && !joystickActive)
                {
                    ReleaseJoystick();
                }
            }
            // 2. Mouse Handling (Unity Editor testing / PC)
            else
            {
                Vector2 mouseGui = ScreenToGUI(Input.mousePosition);

                if (Input.GetMouseButtonDown(0))
                {
                    if (Vector2.Distance(mouseGui, joystickCenter) <= JOYSTICK_RADIUS * 1.5f)
                    {
                        isDraggingJoystick = true;
                        joystickPointerId = -99;
                        UpdateJoystickDrag(mouseGui);
                    }
                    else
                    {
                        ProcessButtonTap(mouseGui, om);
                    }
                }
                else if (Input.GetMouseButton(0))
                {
                    if (isDraggingJoystick && joystickPointerId == -99)
                    {
                        UpdateJoystickDrag(mouseGui);
                    }

                    if (Vector2.Distance(mouseGui, btnChopCenter) <= BTN_CHOP_RADIUS)
                    {
                        isChopButtonHeld = true;
                    }
                    else
                    {
                        isChopButtonHeld = false;
                    }
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    if (isDraggingJoystick && joystickPointerId == -99)
                    {
                        ReleaseJoystick();
                    }
                    isChopButtonHeld = false;
                }
            }

            // 3. Synchronize Analog Joystick position with PC Keyboard (WASD & Arrow Keys)
            if (!isDraggingJoystick)
            {
                float keyX = 0f;
                float keyY = 0f;
                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) keyX += 1f;
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) keyX -= 1f;
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) keyY += 1f;
                if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) keyY -= 1f;

                Vector2 keyDir = new Vector2(keyX, keyY);
                if (keyDir.sqrMagnitude > 1f) keyDir.Normalize();

                if (keyDir.sqrMagnitude > 0.01f)
                {
                    // GUI coordinates: +X is right, +Y is DOWN (so upward movement in world is -Y in GUI)
                    Vector2 targetKnobPos = joystickCenter + new Vector2(keyDir.x * JOYSTICK_RADIUS * 0.85f, -keyDir.y * JOYSTICK_RADIUS * 0.85f);
                    joystickCurrentPos = Vector2.Lerp(joystickCurrentPos, targetKnobPos, Time.unscaledDeltaTime * 24f);
                }
                else
                {
                    // Smoothly slide back to center when released
                    joystickCurrentPos = Vector2.Lerp(joystickCurrentPos, joystickCenter, Time.unscaledDeltaTime * 18f);
                    if (Vector2.Distance(joystickCurrentPos, joystickCenter) < 0.25f)
                    {
                        joystickCurrentPos = joystickCenter;
                    }
                }
            }
        }

        private void UpdateJoystickDrag(Vector2 guiPos)
        {
            Vector2 offset = guiPos - joystickCenter;
            float dist = offset.magnitude;
            if (dist > JOYSTICK_RADIUS)
            {
                offset = offset.normalized * JOYSTICK_RADIUS;
            }
            joystickCurrentPos = joystickCenter + offset;

            // In GUI, +Y is downward, so upward movement is negative in GUI Y
            Vector2 inputDir = new Vector2(offset.x / JOYSTICK_RADIUS, -offset.y / JOYSTICK_RADIUS);
            if (cachedPlayer != null)
            {
                cachedPlayer.mobileMovementInput = inputDir;
            }
        }

        private void ReleaseJoystick()
        {
            isDraggingJoystick = false;
            joystickPointerId = -1;
            if (cachedPlayer != null)
            {
                cachedPlayer.mobileMovementInput = Vector2.zero;
            }
        }

        private void ProcessButtonTap(Vector2 guiPos, OrderManager om)
        {
            // Top HUD Audio & Pause buttons are exclusively handled by GUI.Button in OnGUI (prevents double triggers on MouseDown + MouseUp)
            Rect audioHitRect = new Rect(audioHudRect.x - 12f, audioHudRect.y - 12f, audioHudRect.width + 24f, audioHudRect.height + 24f);
            Rect pauseHitRect = new Rect(pauseBtnRect.x - 12f, pauseBtnRect.y - 12f, pauseBtnRect.width + 24f, pauseBtnRect.height + 24f);
            if (audioHitRect.Contains(guiPos) || pauseHitRect.Contains(guiPos))
            {
                return;
            }

            if (cachedPlayer == null) return;

            // 2. Ambil / Letak (Primary Action)
            if (Vector2.Distance(guiPos, btnInteractCenter) <= BTN_INTERACT_RADIUS)
            {
                PlayButtonClick();
                cachedPlayer.TriggerPrimaryAction();
                btnInteractPressTime = Time.unscaledTime;
                return;
            }

            // 3. Potong (Chop Action)
            if (Vector2.Distance(guiPos, btnChopCenter) <= BTN_CHOP_RADIUS)
            {
                cachedPlayer.TriggerAlternateAction();
                btnChopPressTime = Time.unscaledTime;
                lastChopTriggerTime = Time.unscaledTime;
                return;
            }
        }

        // =========================================================================
        // OPENING SCREEN TRANSITION ("TIRAI TERBUKA" / CURTAIN LIFTS UP)
        // =========================================================================
        private void DrawOpeningCurtainTransition(OrderManager om)
        {
            EnsureCurtainTexture();

            float t = om.openingTransitionTime;
            const float HOLD_TIME = 0.35f;
            const float SLIDE_TIME = 1.05f;

            // Phase 1: Wait / Closed (t < HOLD_TIME) -> Curtain completely covers screen
            // Phase 2: Slide UP (t: HOLD_TIME -> HOLD_TIME + SLIDE_TIME) -> Curtain smoothly slides up
            // Phase 3: Fully open (t >= HOLD_TIME + SLIDE_TIME) -> Curtain off-screen
            float slideProgress = Mathf.Clamp01((t - HOLD_TIME) / SLIDE_TIME);
            float ease = Mathf.SmoothStep(0f, 1f, slideProgress);

            float curtainY = -REF_HEIGHT * ease;

            // Draw curtain if still visible on screen
            if (curtainY > -REF_HEIGHT)
            {
                float s = guiScale > 0.0001f ? guiScale : 1f;
                float extraLeft = guiOffsetX / s;
                float totalW = Screen.width > 0 ? (Screen.width / s) : REF_WIDTH;
                Rect curtainRect = new Rect(-extraLeft, curtainY, totalW, REF_HEIGHT);

                if (texCurtain != null)
                {
                    GUI.color = Color.white;
                    GUI.DrawTexture(curtainRect, texCurtain, ScaleMode.StretchToFill);
                }
                else
                {
                    // Fallback rich curtain slate
                    GUI.color = new Color(0.12f, 0.10f, 0.14f, 0.99f);
                    GUI.Box(curtainRect, GUIContent.none, headerStyle);
                }

                // Bottom Hem Trim & Shadow
                float hemY = curtainY + REF_HEIGHT;
                if (hemY > 0f && hemY < REF_HEIGHT + 30f)
                {
                    // Sleek golden trim at the bottom edge of the curtain
                    GUI.color = new Color(0.95f, 0.72f, 0.20f, 0.95f);
                    GUI.Box(new Rect(-extraLeft, hemY - 4f, totalW, 4f), GUIContent.none, headerStyle);

                    // Soft ambient drop shadow below the rising curtain hem
                    GUI.color = new Color(0f, 0f, 0f, 0.40f * (1f - slideProgress));
                    GUI.Box(new Rect(-extraLeft, hemY, totalW, 18f), GUIContent.none, headerStyle);
                }
            }
        }

        // =========================================================================
        // 2. MAIN MENU SCREEN (16:9 PROPORTIONED)
        // =========================================================================
        private void DrawMainMenu(OrderManager om)
        {
            float elapsed = Time.unscaledTime - stateTransitionTime;

            // Background Texture & subtle dark vignette
            if (texMainMenuBg != null)
            {
                GUI.color = Color.white;
                GUI.DrawTexture(new Rect(0, 0, REF_WIDTH, REF_HEIGHT), texMainMenuBg, ScaleMode.StretchToFill);

                // Subtle dark vignette gradient overlay to make foreground menu pop
                GUI.color = new Color(0.04f, 0.05f, 0.07f, 0.40f);
                GUI.Box(new Rect(0, 0, REF_WIDTH, REF_HEIGHT), GUIContent.none, headerStyle);
            }
            else
            {
                GUI.color = new Color(0.08f, 0.10f, 0.14f, 0.98f);
                GUI.Box(new Rect(0, 0, REF_WIDTH, REF_HEIGHT), GUIContent.none, headerStyle);
            }

            // Exactly matching the centered layout from the screenshot (736px asset width - NO STRETCH)
            float centerW = 736f;
            float centerH = 670f;
            Rect menuRect = new Rect((REF_WIDTH - centerW) / 2f, (REF_HEIGHT - centerH) / 2f, centerW, centerH);

            // Pop-in bounce for menu card
            float t = Mathf.Clamp01(elapsed / 0.35f);
            float menuScale = Mathf.Max(0.001f, Mathf.LerpUnclamped(0.92f, 1f, EaseOutBack(t, 1.2f)));
            Vector2 menuCenter = new Vector2(REF_WIDTH * 0.5f, REF_HEIGHT * 0.5f);
            PushPopupTransform(menuCenter, menuScale, Mathf.Clamp01(t / 0.15f));

            // Central Card: Use ui_main_panel_bg asset (or fallback to dark box)
            if (texMainMenuPanelBg != null)
            {
                GUI.color = Color.white;
                GUI.DrawTexture(menuRect, texMainMenuPanelBg, ScaleMode.StretchToFill);
            }
            else
            {
                GUI.color = new Color(0.10f, 0.13f, 0.18f, 0.92f);
                GUI.Box(menuRect, GUIContent.none, modalStyle);

                GUI.color = new Color(0.95f, 0.65f, 0.15f);
                GUI.Box(new Rect(menuRect.x, menuRect.y, centerW, 6), GUIContent.none, headerStyle);
            }

            // 1. Logo Image (BORGOR / Game Logo) - Large & Centered in the expanded space
            if (texGameLogo != null)
            {
                float logoW = 540f;
                float logoH = 260f;
                float logoX = menuRect.x + (centerW - logoW) / 2f;
                float logoY = menuRect.y + 36f;
                GUI.color = Color.white;
                GUI.DrawTexture(new Rect(logoX, logoY, logoW, logoH), texGameLogo, ScaleMode.ScaleToFit);
            }
            else
            {
                GUI.color = texMainMenuPanelBg != null ? new Color(0.22f, 0.15f, 0.10f) : Color.white;
                titleStyle.fontSize = 38;
                titleStyle.fontStyle = FontStyle.Bold;
                titleStyle.alignment = TextAnchor.MiddleCenter;
                GUI.Label(new Rect(menuRect.x, menuRect.y + 70f, centerW, 50f), "BURGER MASTER EXPRESS", titleStyle);

                subtitleStyle.fontSize = 16;
                subtitleStyle.alignment = TextAnchor.MiddleCenter;
                GUI.color = texMainMenuPanelBg != null ? new Color(0.38f, 0.28f, 0.20f) : new Color(0.85f, 0.88f, 0.92f);
                GUI.Label(new Rect(menuRect.x, menuRect.y + 130f, centerW, 26f), "Simulasi Dapur & Manajemen Restoran Cepat Saji", subtitleStyle);
            }

            // Top-left Star Badge (ui_badge_star: natural 246x88 ratio)
            if (texMainMenuBadgeStar != null)
            {
                int totalStars = om.GetBestStars(GameLevel.Level1) + om.GetBestStars(GameLevel.Level2) + om.GetBestStars(GameLevel.Level3);
                float starBadgeW = 150f;
                float starBadgeH = starBadgeW * (88f / 246f);
                Rect starBadgeRect = new Rect(menuRect.x + 28f, menuRect.y + 20f, starBadgeW, starBadgeH);
                GUI.color = Color.white;
                GUI.DrawTexture(starBadgeRect, texMainMenuBadgeStar, ScaleMode.StretchToFill);

                GUIStyle starBadgeStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 15,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };
                starBadgeStyle.normal.textColor = new Color(0.22f, 0.16f, 0.10f);
                GUI.Label(new Rect(starBadgeRect.x + 38f, starBadgeRect.y + (starBadgeH - 24f) * 0.5f, starBadgeRect.width - 44f, 24f), 
                    $"{totalStars} / 9", starBadgeStyle);
            }

            // Top-right Icons: Help (?) and Audio buttons (natural 88x88 square ratio)
            float iconBtnSize = 48f;
            float helpBtnX = menuRect.x + centerW - 28f - iconBtnSize;
            float helpBtnY = menuRect.y + 20f;
            Rect helpBtnRect = new Rect(helpBtnX, helpBtnY, iconBtnSize, iconBtnSize);

            // Help Question Mark Button (ui_btn_help)
            DrawIconButton(helpBtnRect, texMainMenuBtnHelp, "?", new Color(0.98f, 0.55f, 0.55f), () =>
            {
                showHelpModal = true;
                helpScrollPos = Vector2.zero;
            }, !showHelpModal);

            // Audio Mute/Unmute Button (ui_btn_audio_sq_on / ui_btn_audio_sq_off) beside Help (?) button
            bool isMenuMuted = AudioManager.Instance != null && AudioManager.Instance.isMuted;
            Texture2D menuAudioTex = isMenuMuted ? (texBtnAudioOff ?? texMainMenuBtnAudio) : (texBtnAudioOn ?? texMainMenuBtnAudio);
            float audioBtnX = helpBtnX - 10f - iconBtnSize;
            float audioBtnY = helpBtnY;
            Rect audioBtnRect = new Rect(audioBtnX, audioBtnY, iconBtnSize, iconBtnSize);
            DrawIconButton(audioBtnRect, menuAudioTex, isMenuMuted ? "MUTE" : "AUDIO", isMenuMuted ? new Color(0.95f, 0.45f, 0.45f) : new Color(0.35f, 0.78f, 0.95f), () =>
            {
                if (AudioManager.Instance != null) AudioManager.Instance.ToggleMute();
            }, !showHelpModal);

            // 2. Buttons: Centered & natural 518px width matching asset (NO STRETCH)
            float btnW = 518f;
            float btnX = menuRect.x + (centerW - btnW) / 2f;
            float btnY = menuRect.y + 315f;

            // Button 1: Green Play Button (ui_btn_mulai_main - 518x88 asset)
            DrawCenteredMenuButton(new Rect(btnX, btnY, btnW, 84f), texMainMenuBtnPlay, 
                "PILIH MISI & MULAI MAIN", new Color(0.18f, 0.82f, 0.44f), 19, () =>
            {
                om.GoToLevelSelect();
            }, !showHelpModal);

            // Button 2: Blue Recipe Button (ui_btn_buku_resep - 518x78 asset)
            DrawCenteredMenuButton(new Rect(btnX, btnY + 84f + 12f, btnW, 76f), texMainMenuBtnRecipe, 
                "BUKU RESEP BURGER RESMI", new Color(0.20f, 0.62f, 0.88f), 18, () =>
            {
                om.GoToRecipeBook();
            }, !showHelpModal);

            // Button 3: Red/Coral Exit Button (ui_btn_keluar - 518x70 asset)
            DrawCenteredMenuButton(new Rect(btnX, btnY + 84f + 12f + 76f + 12f, btnW, 68f), texMainMenuBtnExit, 
                "KELUAR DARI PERMAINAN", new Color(0.55f, 0.60f, 0.68f), 17, () =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }, !showHelpModal);

            // Bottom shortcut hint inside panel
            tipStyle.fontSize = 12;
            tipStyle.alignment = TextAnchor.MiddleCenter;
            Color hintCol = texMainMenuPanelBg != null ? new Color(0.40f, 0.30f, 0.22f, 0.90f) : new Color(0.70f, 0.75f, 0.80f, 0.85f);
            GUI.color = hintCol;
            GUI.Label(new Rect(menuRect.x, menuRect.y + 596f, centerW, 22f), 
                "Shortcut: [M] Mute Musik   •   [?] Panduan   •   [ESC] Jeda Permainan", tipStyle);

            PopPopupTransform();

            // Watermark / Credit Author di sebelah kiri bawah lobby (Main Menu)
            string authorText = "Created by Charisma Tegar Davian Putra (062401030 - ComputerScience)";
            GUIStyle authorStyle = new GUIStyle(GUI.skin.label)
            {
                font = customFont,
                fontSize = 15,
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.MiddleLeft
            };
            Vector2 authorTextSize = authorStyle.CalcSize(new GUIContent(authorText));
            float authorPadX = 14f;
            float authorBoxW = authorTextSize.x + authorPadX * 2f;
            float authorBoxH = 34f;
            float authorBoxX = 24f;
            float authorBoxY = REF_HEIGHT - authorBoxH - 18f;
            Rect authorBoxRect = new Rect(authorBoxX, authorBoxY, authorBoxW, authorBoxH);
            Rect authorLabelRect = new Rect(authorBoxX + authorPadX, authorBoxY + (authorBoxH - authorTextSize.y) * 0.5f, authorTextSize.x + 10f, authorTextSize.y);

            // Subtle dark background pill box for clarity against any wallpaper
            GUI.color = new Color(0.05f, 0.07f, 0.10f, 0.65f);
            GUI.Box(authorBoxRect, GUIContent.none, cardStyle);

            // Text with soft outline using custom font
            DrawOutlinedLabel(authorLabelRect, authorText, authorStyle, 
                new Color(0.94f, 0.95f, 0.97f, 0.95f), 
                new Color(0.04f, 0.05f, 0.08f, 0.85f), 1.2f);

            // Quick Help Dialog if ? button clicked
            if (showHelpModal)
            {
                DrawQuickHelpModal(om);
            }
        }

        // =========================================================================
        // 3. LEVEL SELECT SCREEN (3 SQUARE LEVEL BADGES + CENTER DETAIL MODAL)
        // =========================================================================
        private void DrawLevelSelect(OrderManager om)
        {
            EnsureLevelSquareTextures();

            float elapsed = Time.unscaledTime - stateTransitionTime;
            float bgAlpha = Mathf.Clamp01(elapsed / 0.20f);

            // 1. Restaurant background with dark vignette
            if (texMainMenuBg != null)
            {
                GUI.color = Color.white;
                GUI.DrawTexture(new Rect(0, 0, REF_WIDTH, REF_HEIGHT), texMainMenuBg, ScaleMode.StretchToFill);

                GUI.color = new Color(0.04f, 0.05f, 0.07f, 0.60f * bgAlpha);
                GUI.Box(new Rect(0, 0, REF_WIDTH, REF_HEIGHT), GUIContent.none, headerStyle);
            }
            else
            {
                GUI.color = new Color(0.08f, 0.10f, 0.14f, 0.98f * bgAlpha);
                GUI.Box(new Rect(0, 0, REF_WIDTH, REF_HEIGHT), GUIContent.none, headerStyle);
            }

            // 2. Entrance pop-up spring physics for the dialog container
            float panelT = Mathf.Clamp01(elapsed / 0.35f);
            float panelScale = Mathf.Max(0.001f, Mathf.LerpUnclamped(0.92f, 1f, EaseOutBack(panelT, 1.2f)));
            PushPopupTransform(new Vector2(REF_WIDTH * 0.5f, REF_HEIGHT * 0.5f), panelScale, Mathf.Clamp01(panelT / 0.18f));

            // Outer Illustrated Board
            float panelW = 1660f;
            float panelH = 970f;
            float panelX = (REF_WIDTH - panelW) / 2f;
            float panelY = (REF_HEIGHT - panelH) / 2f - 10f;
            Rect panelRect = new Rect(panelX, panelY, panelW, panelH);

            if (texPanelSelectBg != null)
            {
                GUI.color = Color.white;
                GUI.DrawTexture(panelRect, texPanelSelectBg, ScaleMode.StretchToFill);
            }
            else
            {
                GUI.color = new Color(0.14f, 0.18f, 0.24f, 0.96f);
                GUI.Box(panelRect, GUIContent.none, modalStyle);
            }

            // 3. Header Title Banner & Subtitle
            if (texTitlePilihLevel != null)
            {
                float titleW = 730f;
                float titleH = 185f;
                float titleX = (REF_WIDTH - titleW) / 2f;
                float titleY = panelY + 12f;
                GUI.color = Color.white;
                GUI.DrawTexture(new Rect(titleX, titleY, titleW, titleH), texTitlePilihLevel, ScaleMode.ScaleToFit);
            }
            else
            {
                titleStyle.fontSize = 32;
                titleStyle.alignment = TextAnchor.MiddleCenter;
                GUI.Label(new Rect(panelX, panelY + 24f, panelW, 44f), "PILIH TINGKAT LEVEL PERMAINAN", titleStyle);
            }

            // Header Subtitle Text
            modalDarkTextStyle.fontSize = 17;
            modalDarkTextStyle.alignment = TextAnchor.MiddleCenter;
            GUI.color = Color.white;
            GUI.Label(new Rect(panelX, panelY + 184f, panelW, 30f), 
                "<b>Klik salah satu nomor level untuk melihat detail misi & mulai bermain!</b>", modalDarkTextStyle);

            // Data arrays for 3 levels
            GameLevel[] levels = new GameLevel[] { GameLevel.Level1, GameLevel.Level2, GameLevel.Level3 };
            Texture2D[] squareIcons = new Texture2D[] { texLevel1Square, texLevel2Square, texLevel3Square };
            Texture2D[] cardFrames = new Texture2D[] { texCardMudah, texCardMenengah, texCardSulit };
            Texture2D[] playButtons = new Texture2D[] { texBtnPlay1, texBtnPlay2, texBtnPlay3 };
            string[] difficultyTitles = new string[] { "LEVEL 1 • MUDAH", "LEVEL 2 • MENENGAH", "LEVEL 3 • SULIT" };
            Color[] badgeColors = new Color[] {
                new Color(0.16f, 0.68f, 0.36f),
                new Color(0.92f, 0.58f, 0.15f),
                new Color(0.88f, 0.22f, 0.22f)
            };

            // 4. 3 Square Number Badges (NO frames, NO cards, ONLY the 3 number squares)
            float sqSize = 280f;
            float sqGap = 120f;
            float totalSquaresW = 3 * sqSize + 2 * sqGap;
            float startX = (REF_WIDTH - totalSquaresW) * 0.5f;
            float startY = panelY + 310f;

            for (int i = 0; i < levels.Length; i++)
            {
                GameLevel lvl = levels[i];
                Rect sqRect = new Rect(startX + i * (sqSize + sqGap), startY, sqSize, sqSize);

                // Staggered pop-in bounce animation
                float boxDelay = 0.06f * i;
                float boxElapsed = Mathf.Max(0f, elapsed - boxDelay);
                float bt = Mathf.Clamp01(boxElapsed / 0.35f);
                float boxScale = Mathf.Max(0.001f, Mathf.LerpUnclamped(0.50f, 1f, EaseOutBack(bt, 1.8f)));

                bool isHovered = sqRect.Contains(Event.current.mousePosition) && !selectedLevelDetail.HasValue;
                float hoverBoost = isHovered ? 1.08f : 1.0f;
                Vector2 boxCenter = new Vector2(sqRect.x + sqSize * 0.5f, sqRect.y + sqSize * 0.5f);

                PushPopupTransform(boxCenter, boxScale * hoverBoost, Mathf.Clamp01(bt * 2.5f));

                // Draw ONLY the square number texture (NO frames, NO background box)
                Texture2D currentSq = squareIcons[i];
                if (currentSq != null)
                {
                    GUI.color = isHovered ? new Color(1.08f, 1.08f, 1.08f, 1f) : Color.white;
                    GUI.DrawTexture(sqRect, currentSq, ScaleMode.ScaleToFit);
                }
                else
                {
                    GUI.color = badgeColors[i];
                    GUI.Box(sqRect, GUIContent.none, cardStyle);
                    titleStyle.fontSize = 54;
                    titleStyle.alignment = TextAnchor.MiddleCenter;
                    GUI.color = Color.white;
                    GUI.Label(sqRect, $"{i + 1}", titleStyle);
                }

                // Clicking anywhere on the square opens the detail modal
                if (!selectedLevelDetail.HasValue && GUI.Button(sqRect, GUIContent.none, GUIStyle.none))
                {
                    PlayButtonClick();
                    selectedLevelDetail = lvl;
                    levelDetailOpenTime = Time.unscaledTime;
                }

                PopPopupTransform();
            }

            // 5. Bottom Navigation Button (Illustrated "◀ KEMBALI KE MENU" centered at bottom)
            float backBtnW = 320f;
            float backBtnH = 58f;
            float backBtnX = (REF_WIDTH - backBtnW) * 0.5f;
            float backBtnY = panelY + panelH - 110f;
            Rect backRect = new Rect(backBtnX, backBtnY, backBtnW, backBtnH);

            if (!selectedLevelDetail.HasValue)
            {
                DrawModalSubButton(backRect, texBtnKembali, "◀  KEMBALI KE MENU", new Color(0.6f, 0.65f, 0.72f), () =>
                {
                    selectedLevelDetail = null;
                    om.GoToMainMenu();
                });
            }
            else
            {
                if (texBtnKembali != null)
                {
                    GUI.color = new Color(1f, 1f, 1f, 0.5f);
                    GUI.DrawTexture(backRect, texBtnKembali, ScaleMode.ScaleToFit);
                }
            }

            PopPopupTransform();

            // =====================================================================
            // 6. DETAIL MODAL POPUP (CENTER OF SCREEN)
            // =====================================================================
            if (selectedLevelDetail.HasValue)
            {
                DrawLevelDetailModal(om, selectedLevelDetail.Value);
            }
        }

        private void DrawLevelDetailModal(OrderManager om, GameLevel lvl)
        {
            float elapsed = Time.unscaledTime - levelDetailOpenTime;

            float cardW = 500f;
            float cardH = 610f;
            float cardX = (REF_WIDTH - cardW) / 2f;
            float cardY = (REF_HEIGHT - cardH) / 2f;
            Rect cRect = new Rect(cardX, cardY, cardW, cardH);

            // 1. Dim Backdrop Overlay (Visual only, NOT a full-screen button)
            float dimAlpha = Mathf.Clamp01(elapsed / 0.20f) * 0.74f;
            GUI.color = new Color(0f, 0f, 0f, dimAlpha);
            GUI.Box(new Rect(0, 0, REF_WIDTH, REF_HEIGHT), GUIContent.none, headerStyle);

            // Clicking outside the modal card closes the modal
            if (Event.current != null && Event.current.type == EventType.MouseDown)
            {
                if (!cRect.Contains(Event.current.mousePosition))
                {
                    selectedLevelDetail = null;
                    Event.current.Use();
                    return;
                }
            }

            // Escape key closes detail modal
            if (Event.current != null && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
            {
                selectedLevelDetail = null;
                Event.current.Use();
                return;
            }

            // 2. Entrance spring bounce
            float t = Mathf.Clamp01(elapsed / 0.32f);
            float scale = Mathf.Max(0.001f, Mathf.LerpUnclamped(0.40f, 1f, EaseOutBack(t, 1.6f)));
            Vector2 center = new Vector2(REF_WIDTH * 0.5f, REF_HEIGHT * 0.5f);

            PushPopupTransform(center, scale, Mathf.Clamp01(t / 0.15f));

            int lvlIdx = Mathf.Clamp((int)lvl - 1, 0, 2);
            LevelConfig cfg = om.GetLevelConfig(lvl);
            Texture2D[] cardFrames = new Texture2D[] { texCardMudah, texCardMenengah, texCardSulit };
            Texture2D[] playButtons = new Texture2D[] { texBtnPlay1, texBtnPlay2, texBtnPlay3 };
            Texture2D[] squareIcons = new Texture2D[] { texLevel1Square, texLevel2Square, texLevel3Square };
            Color[] badgeColors = new Color[] {
                new Color(0.16f, 0.68f, 0.36f),
                new Color(0.92f, 0.58f, 0.15f),
                new Color(0.88f, 0.22f, 0.22f)
            };

            // A. Card Background Frame
            if (cardFrames[lvlIdx] != null)
            {
                GUI.color = Color.white;
                GUI.DrawTexture(cRect, cardFrames[lvlIdx], ScaleMode.StretchToFill);
            }
            else
            {
                GUI.color = new Color(0.98f, 0.95f, 0.90f, 0.98f);
                GUI.Box(cRect, GUIContent.none, cardStyle);
            }

            // B. Close Button using ui_icon_cross asset
            float closeSz = 48f;
            Rect closeRect = new Rect(cRect.x + cardW - closeSz - 18f, cRect.y + 16f, closeSz, closeSz);
            bool isCloseHovered = closeRect.Contains(Event.current.mousePosition);
            if (texIconCross != null)
            {
                GUI.color = isCloseHovered ? new Color(1.15f, 1.15f, 1.15f, 1f) : Color.white;
                GUI.DrawTexture(closeRect, texIconCross, ScaleMode.ScaleToFit);
            }
            else
            {
                GUI.color = isCloseHovered ? new Color(0.95f, 0.35f, 0.35f) : new Color(0.85f, 0.30f, 0.30f, 0.95f);
                GUI.Box(closeRect, GUIContent.none, cardStyle);
                titleStyle.fontSize = 20;
                titleStyle.alignment = TextAnchor.MiddleCenter;
                GUI.color = Color.white;
                GUI.Label(closeRect, "✕", titleStyle);
            }

            if (GUI.Button(closeRect, GUIContent.none, GUIStyle.none))
            {
                PlayButtonClick();
                selectedLevelDetail = null;
            }

            // C. Big Square Level Asset (ui_level_X_square.png) positioned cleanly at the top
            float sqSz = 220f;
            Rect sqRect = new Rect(cRect.x + (cardW - sqSz) / 2f, cRect.y + 44f, sqSz, sqSz);
            Texture2D currentSq = squareIcons[lvlIdx];
            if (currentSq != null)
            {
                GUI.color = Color.white;
                GUI.DrawTexture(sqRect, currentSq, ScaleMode.ScaleToFit);
            }

            // D. Level Stats Container (Waktu, Target, Kesabaran, Maks. Order)
            float statsBoxY = sqRect.yMax + 14f;
            float statsBoxH = 175f;
            Rect statsBoxRect = new Rect(cRect.x + 24f, statsBoxY, cardW - 48f, statsBoxH);
            if (texStatsBoxBg != null)
            {
                GUI.color = Color.white;
                GUI.DrawTexture(statsBoxRect, texStatsBoxBg, ScaleMode.StretchToFill);
            }
            else
            {
                GUI.color = new Color(0.88f, 0.92f, 0.96f, 0.95f);
                GUI.Box(statsBoxRect, GUIContent.none, cardStyle);
            }

            float colW = (statsBoxRect.width - 24f) * 0.5f;
            float col1X = statsBoxRect.x + 12f;
            float col2X = statsBoxRect.x + 12f + colW;

            // --- ROW 1: Waktu & Target ---
            float row1Y = statsBoxRect.y + 14f;

            // 1. WAKTU
            if (texIconClock != null)
            {
                GUI.color = Color.white;
                GUI.DrawTexture(new Rect(col1X + 4f, row1Y + 3f, 26f, 26f), texIconClock, ScaleMode.ScaleToFit);
            }
            modalDarkTextStyle.fontSize = 14;
            modalDarkTextStyle.alignment = TextAnchor.MiddleLeft;
            GUI.color = Color.white;
            GUI.Label(new Rect(col1X + 34f, row1Y, colW - 36f, 18f), "Waktu Ronde:", modalDarkTextStyle);

            modalDarkBoldStyle.fontSize = 18;
            modalDarkBoldStyle.alignment = TextAnchor.MiddleLeft;
            GUI.Label(new Rect(col1X + 34f, row1Y + 18f, colW - 36f, 24f), $"<b>{cfg.duration}s</b>", modalDarkBoldStyle);

            // 2. TARGET UANG / SKOR
            if (texIconTarget != null)
            {
                GUI.color = Color.white;
                GUI.DrawTexture(new Rect(col2X + 4f, row1Y + 3f, 26f, 26f), texIconTarget, ScaleMode.ScaleToFit);
            }
            GUI.Label(new Rect(col2X + 34f, row1Y, colW - 36f, 18f), "Target Omset:", modalDarkTextStyle);
            GUI.Label(new Rect(col2X + 34f, row1Y + 18f, colW - 36f, 24f), $"<color=#1B5E20><b>${cfg.targetScore}</b></color>", modalDarkBoldStyle);

            // Divider line 1
            float div1Y = statsBoxRect.y + 80f;
            GUI.color = new Color(0f, 0f, 0f, 0.12f);
            GUI.Box(new Rect(statsBoxRect.x + 16f, div1Y, statsBoxRect.width - 32f, 1.5f), GUIContent.none, headerStyle);

            // --- ROW 2: Kesabaran Pelanggan & Rekor ---
            float row2Y = statsBoxRect.y + 90f;

            // 3. KESABARAN PELANGGAN
            if (texHappy != null)
            {
                GUI.color = Color.white;
                GUI.DrawTexture(new Rect(col1X + 5f, row2Y + 3f, 24f, 24f), texHappy, ScaleMode.ScaleToFit);
            }
            GUI.color = Color.white;
            GUI.Label(new Rect(col1X + 34f, row2Y, colW - 36f, 18f), "Kesabaran:", modalDarkTextStyle);
            GUI.Label(new Rect(col1X + 34f, row2Y + 18f, colW - 36f, 24f), $"<b>{cfg.patienceTime:0}s</b>", modalDarkBoldStyle);

            // 4. MAKSIMAL ORDER
            GUI.Label(new Rect(col2X + 34f, row2Y, colW - 36f, 18f), "Maks. Pelanggan:", modalDarkTextStyle);
            GUI.Label(new Rect(col2X + 34f, row2Y + 18f, colW - 36f, 24f), $"<b>{cfg.maxOrders} Order</b>", modalDarkBoldStyle);

            // Divider line 2
            float div2Y = statsBoxRect.y + 138f;
            GUI.color = new Color(0f, 0f, 0f, 0.12f);
            GUI.Box(new Rect(statsBoxRect.x + 16f, div2Y, statsBoxRect.width - 32f, 1.5f), GUIContent.none, headerStyle);

            // Row 3: Rekor Terbaik
            int bestStars = om.GetBestStars(lvl);
            int bestScore = om.GetBestScore(lvl);
            modalDarkBoldStyle.fontSize = 14;
            modalDarkBoldStyle.alignment = TextAnchor.MiddleCenter;
            string starDisplay = $"{bestStars}/3 Bintang";
            GUI.color = Color.white;
            GUI.Label(new Rect(statsBoxRect.x + 10f, statsBoxRect.y + 145f, statsBoxRect.width - 20f, 24f),
                $"Rekor Anda: <color=#B78103><b>{starDisplay} (${bestScore})</b></color>", modalDarkBoldStyle);

            // E. Big PLAY Button (Illustrated 3D Button)
            float playBtnW = cardW - 48f;
            float playBtnH = 70f;
            Rect playBtnRect = new Rect(cRect.x + 24f, statsBoxRect.yMax + 18f, playBtnW, playBtnH);

            DrawModalSubButton(playBtnRect, playButtons[lvlIdx], $"MAINKAN LEVEL {lvlIdx + 1}", badgeColors[lvlIdx], () =>
            {
                selectedLevelDetail = null;
                om.StartLevel(lvl);
            });

            PopPopupTransform();
        }

        // =========================================================================
        // 4. RECIPE BOOK SCREEN (16:9 PROPORTIONED)
        // =========================================================================
        private void DrawRecipeBook(OrderManager om)
        {
            float elapsed = Time.unscaledTime - stateTransitionTime;

            float bgAlpha = Mathf.Clamp01(elapsed / 0.20f);
            GUI.color = new Color(0.06f, 0.08f, 0.12f, 0.95f * bgAlpha);
            GUI.Box(new Rect(0, 0, REF_WIDTH, REF_HEIGHT), GUIContent.none, headerStyle);

            float totalW = 1280f;
            float totalH = 800f;
            Rect container = new Rect((REF_WIDTH - totalW) / 2f, (REF_HEIGHT - totalH) / 2f, totalW, totalH);

            // Pop-in bounce for container
            float t = Mathf.Clamp01(elapsed / 0.32f);
            float scale = Mathf.Max(0.001f, Mathf.LerpUnclamped(0.88f, 1f, EaseOutBack(t, 1.2f)));
            Vector2 center = new Vector2(REF_WIDTH * 0.5f, REF_HEIGHT * 0.5f);

            PushPopupTransform(center, scale, Mathf.Clamp01(t / 0.15f));

            // 1. Panel Background (1280x800)
            if (texRecipeBookPanelBg != null)
            {
                GUI.color = Color.white;
                GUI.DrawTexture(container, texRecipeBookPanelBg, ScaleMode.StretchToFill);
            }
            else
            {
                GUI.color = new Color(0.12f, 0.15f, 0.20f, 0.96f);
                GUI.Box(container, GUIContent.none, modalStyle);
            }

            // 2. Title Banner (ui_title_buku_resep_resmi.png is 960x200, aspect 4.8:1)
            float titleW = 580f;
            float titleH = 120f;
            Rect titleRect = new Rect(container.x + (totalW - titleW) / 2f, container.y - 20f, titleW, titleH);
            if (texRecipeTitle != null)
            {
                GUI.color = Color.white;
                GUI.DrawTexture(titleRect, texRecipeTitle, ScaleMode.ScaleToFit);
            }
            else
            {
                GUI.Label(new Rect(container.x, container.y + 24, totalW, 42), "BUKU RESEP BURGER RESMI", titleStyle);
            }

            // 3. Subtitle description
            subtitleStyle.fontSize = 15;
            subtitleStyle.fontStyle = FontStyle.Bold;
            subtitleStyle.alignment = TextAnchor.MiddleCenter;
            subtitleStyle.normal.textColor = texRecipeBookPanelBg != null ? new Color(0.42f, 0.32f, 0.22f) : new Color(0.85f, 0.88f, 0.92f);
            GUI.Label(new Rect(container.x, container.y + 78f, totalW, 26f), 
                "Susun bahan di atas Piring sesuai urutan resep untuk memuaskan pelanggan", subtitleStyle);
            subtitleStyle.fontStyle = FontStyle.Italic;
            subtitleStyle.normal.textColor = new Color(0.85f, 0.88f, 0.92f);

            // 4. Recipes Grid (2 Columns x 3 Rows = 6 Cards)
            LevelConfig maxCfg = om.GetLevelConfig(GameLevel.Level3);
            float cardW = 560f;
            float cardH = 176f;
            float colGap = 36f;
            float rowGap = 12f;
            float startX = container.x + (totalW - (cardW * 2f + colGap)) / 2f;
            float startY = container.y + 112f;

            for (int i = 0; i < maxCfg.recipes.Count; i++)
            {
                BurgerRecipe rec = maxCfg.recipes[i];
                float colX = startX + (i % 2) * (cardW + colGap);
                float rowY = startY + (i / 2) * (cardH + rowGap);
                Rect rRect = new Rect(colX, rowY, cardW, cardH);

                // Alternating card colors (Cyan, Peach, Salmon)
                Texture2D cardFrame = null;
                int cycle = i % 3;
                if (cycle == 0) cardFrame = texRecipeCardCyan;
                else if (cycle == 1) cardFrame = texRecipeCardPeach;
                else cardFrame = texRecipeCardSalmon;

                if (cardFrame != null)
                {
                    GUI.color = Color.white;
                    GUI.DrawTexture(rRect, cardFrame, ScaleMode.StretchToFill);
                }
                else
                {
                    GUI.color = new Color(0.16f, 0.20f, 0.26f, 0.96f);
                    GUI.Box(rRect, GUIContent.none, cardStyle);
                }

                // Recipe Name & Price (Sejajar secara horizontal, tanpa badge tag)
                float headerY = rRect.y + 34f;
                float headerH = 32f;

                // Recipe Name (Digeser sedikit ke kanan & bold agar jelas terbaca)
                boldTextStyle.fontSize = 21;
                boldTextStyle.alignment = TextAnchor.MiddleLeft;
                boldTextStyle.normal.textColor = new Color(0.10f, 0.14f, 0.20f);
                GUI.Label(new Rect(rRect.x + 38f, headerY, 375f, headerH), $"<b>{rec.recipeName}</b>", boldTextStyle);

                // Price (Sejajar dengan nama resep)
                Rect priceRect = new Rect(rRect.x + 424f, headerY, 102f, headerH);
                boldTextStyle.fontSize = 17;
                boldTextStyle.alignment = TextAnchor.MiddleCenter;
                boldTextStyle.normal.textColor = new Color(0.06f, 0.36f, 0.15f);
                GUI.Label(priceRect, $"<b>+${rec.reward}</b>", boldTextStyle);

                // Recipe Subtitle / Description (sedikit turun & diberi outline agar kontras dan mudah terbaca)
                boldTextStyle.fontSize = 14;
                boldTextStyle.alignment = TextAnchor.MiddleLeft;
                Rect subRect = new Rect(rRect.x + 24f, rRect.y + 88f, rRect.width - 48f, 22f);
                DrawOutlinedLabel(subRect, $"<b>{rec.subtitle}</b>", boldTextStyle, 
                    new Color(0.22f, 0.16f, 0.10f), 
                    new Color(1f, 1f, 1f, 0.95f), 
                    1f);
                boldTextStyle.normal.textColor = Color.white;

                // Ingredients visual strip with slots (ui_recipe_ingredient_slot.png)
                float slotSz = 44f;
                float slotGap = 8f;
                float chipX = rRect.x + 24f;
                float chipY = rRect.y + 114f;

                for (int k = 0; k < rec.requiredIngredients.Count; k++)
                {
                    ItemType ing = rec.requiredIngredients[k];
                    Rect slotRect = new Rect(chipX, chipY, slotSz, slotSz);

                    if (texRecipeIngredientSlot != null)
                    {
                        GUI.color = Color.white;
                        GUI.DrawTexture(slotRect, texRecipeIngredientSlot, ScaleMode.ScaleToFit);
                    }
                    else
                    {
                        GUI.color = new Color(0.20f, 0.24f, 0.30f, 0.92f);
                        GUI.Box(slotRect, GUIContent.none, cardStyle);
                    }

                    Texture2D ingTex = ItemDatabase.GetTexture(ing);
                    if (ingTex != null)
                    {
                        GUI.color = Color.white;
                        GUI.DrawTexture(new Rect(slotRect.x + 4f, slotRect.y + 4f, slotSz - 8f, slotSz - 8f), ingTex, ScaleMode.ScaleToFit);
                    }
                    chipX += slotSz + slotGap;
                }

                // Ingredient count tag
                boldTextStyle.fontSize = 12;
                boldTextStyle.alignment = TextAnchor.MiddleRight;
                boldTextStyle.normal.textColor = new Color(0.55f, 0.45f, 0.35f);
                GUI.Label(new Rect(rRect.x + rRect.width - 130f, chipY + 12f, 108f, 20f), $"{rec.requiredIngredients.Count} Bahan", boldTextStyle);

                // Reset styles
                boldTextStyle.alignment = TextAnchor.MiddleLeft;
                boldTextStyle.normal.textColor = Color.white;
                textStyle.normal.textColor = Color.white;
            }

            // 5. Centered Back Navigation Button
            float backBtnW = 340f;
            float backBtnH = 58f;
            Rect backRect = new Rect(container.x + (totalW - backBtnW) / 2f, container.y + totalH - 74f, backBtnW, backBtnH);
            DrawModalSubButton(backRect, texBtnKembali ?? texBtnMenuUtama, "◀  KEMBALI KE MENU", new Color(0.6f, 0.65f, 0.72f), () => om.GoToMainMenu());

            // Support ESC key
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
            {
                om.GoToMainMenu();
                Event.current.Use();
            }

            PopPopupTransform();
        }

        // =========================================================================
        // 5. PAUSE MODAL (16:9 PROPORTIONED)
        // =========================================================================
        private void DrawPauseModal(OrderManager om)
        {
            EnsurePauseTextures();

            float elapsed = Time.unscaledTime - stateTransitionTime;

            // Smooth backdrop dim
            float bgAlpha = Mathf.Clamp01(elapsed / 0.20f) * 0.72f;
            GUI.color = new Color(0f, 0f, 0f, bgAlpha);
            GUI.Box(new Rect(0, 0, REF_WIDTH, REF_HEIGHT), GUIContent.none, headerStyle);

            Texture2D bgTex = (customPauseBg != null) ? customPauseBg : texPauseBg;
            Texture2D titleTex = (customPauseTitle != null) ? customPauseTitle : texPauseTitle;
            Texture2D resumeTex = (customPauseBtnResume != null) ? customPauseBtnResume : texPauseBtnResume;
            Texture2D restartTex = (customPauseBtnRestart != null) ? customPauseBtnRestart : texPauseBtnRestart;
            Texture2D menuTex = (customPauseBtnMenu != null) ? customPauseBtnMenu : texPauseBtnMenu;

            // Matches 632x514 aspect ratio (1.23:1)
            float w = 620f;
            float h = 504f;
            Rect modal = new Rect((REF_WIDTH - w) / 2f, (REF_HEIGHT - h) / 2f, w, h);

            // Pop-up bounce animation
            float t = Mathf.Clamp01(elapsed / 0.32f);
            float scale = Mathf.Max(0.001f, Mathf.LerpUnclamped(0.35f, 1f, EaseOutBack(t, 1.8f)));
            Vector2 center = new Vector2(REF_WIDTH * 0.5f, REF_HEIGHT * 0.5f);

            PushPopupTransform(center, scale, Mathf.Clamp01(t / 0.15f));

            if (bgTex != null)
            {
                GUI.color = Color.white;
                GUI.DrawTexture(modal, bgTex, ScaleMode.StretchToFill);
            }
            else
            {
                GUI.color = new Color(0.11f, 0.14f, 0.19f, 0.98f);
                GUI.Box(modal, GUIContent.none, modalStyle);

                // Gold top accent
                GUI.color = new Color(0.95f, 0.65f, 0.15f);
                GUI.Box(new Rect(modal.x, modal.y, w, 6), GUIContent.none, headerStyle);
            }

            // Title Banner in Peach Header
            if (titleTex != null)
            {
                float tw = 440f;
                float th = 74f;
                GUI.color = Color.white;
                GUI.DrawTexture(new Rect(modal.x + (w - tw) / 2f, modal.y + 40f, tw, th), titleTex, ScaleMode.ScaleToFit);
            }
            else
            {
                GUI.color = Color.white;
                titleStyle.fontSize = 30;
                titleStyle.alignment = TextAnchor.MiddleCenter;
                GUI.Label(new Rect(modal.x, modal.y + 45f, w, 40), "⏸  PERMAINAN DIJEDA", titleStyle);
                titleStyle.alignment = TextAnchor.MiddleLeft;
            }

            float btnW = 440f;
            float btnH = 64f;
            float btnX = modal.x + (w - btnW) / 2f;
            float btnY = modal.y + 134f;
            float spacing = 74f;

            // 1. LANJUTKAN
            Rect resumeRect = new Rect(btnX, btnY, btnW, btnH);
            DrawModalSubButton(resumeRect, resumeTex, "▶  LANJUTKAN [ESC]", new Color(0.18f, 0.82f, 0.44f), () =>
            {
                if (Time.unscaledTime - lastPauseToggleTime >= 0.35f)
                {
                    lastPauseToggleTime = Time.unscaledTime;
                    om.TogglePause();
                    if (Event.current != null) Event.current.Use();
                }
            });

            // 2. RESTART RONDE
            Rect restartRect = new Rect(btnX, btnY + spacing, btnW, btnH);
            DrawModalSubButton(restartRect, restartTex, "ULANGI RONDE [R]", new Color(0.95f, 0.65f, 0.15f), () =>
            {
                om.RestartCurrentLevel();
            });

            // 3. MENU UTAMA
            Rect menuRect = new Rect(btnX, btnY + spacing * 2, btnW, btnH);
            DrawModalSubButton(menuRect, menuTex, "KEMBALI KE MENU", new Color(0.6f, 0.65f, 0.72f), () =>
            {
                om.GoToMainMenu();
            });

            // 4. BGM VOLUME SLIDER STRIP
            float sliderY = modal.y + 364f;
            float sliderH = 96f;
            Rect boxRect = new Rect(btnX, sliderY, btnW, sliderH);

            // Container background card
            GUI.color = new Color(0.93f, 0.89f, 0.83f, 0.95f);
            GUI.Box(boxRect, GUIContent.none, cardStyle);

            bool isMuted = AudioManager.Instance != null && AudioManager.Instance.isMuted;
            float currentVol = (AudioManager.Instance != null) ? AudioManager.Instance.bgmVolume : 0.5f;
            float displayVol = isMuted ? 0f : currentVol;

            // Top Row: Speaker Icon Button
            Rect iconRect = new Rect(boxRect.x + 14f, boxRect.y + 12f, 30f, 30f);
            Texture2D audioIcon = (isMuted || displayVol <= 0.001f) ? texBtnAudioOff : texBtnAudioOn;
            if (audioIcon != null)
            {
                GUI.color = Color.white;
                GUI.DrawTexture(iconRect, audioIcon, ScaleMode.ScaleToFit);
            }
            else
            {
                titleStyle.fontSize = 13;
                titleStyle.alignment = TextAnchor.MiddleCenter;
                GUI.color = Color.white;
                GUI.Label(iconRect, (isMuted || displayVol <= 0.001f) ? "OFF" : "ON", titleStyle);
            }

            if (GUI.Button(iconRect, GUIContent.none, GUIStyle.none))
            {
                PlayButtonClick();
                if (AudioManager.Instance != null) AudioManager.Instance.ToggleMute();
            }

            // Top Row: Title Label
            Rect titleLblRect = new Rect(boxRect.x + 50f, boxRect.y + 12f, 180f, 30f);
            boldTextStyle.fontSize = 15;
            boldTextStyle.alignment = TextAnchor.MiddleLeft;
            boldTextStyle.normal.textColor = new Color(0.25f, 0.18f, 0.12f);
            GUI.Label(titleLblRect, "<b>VOLUME BGM</b>", boldTextStyle);

            // Top Row: Status / Percentage tag
            Rect tagRect = new Rect(boxRect.x + boxRect.width - 180f, boxRect.y + 12f, 164f, 30f);
            boldTextStyle.alignment = TextAnchor.MiddleRight;
            string volTag = (isMuted || displayVol <= 0.001f)
                ? "<color=#C0392B><b>NONAKTIF (0%)</b></color>"
                : $"<color=#27AE60><b>{Mathf.RoundToInt(displayVol * 100f)}%</b></color>";
            GUI.Label(tagRect, volTag, boldTextStyle);
            boldTextStyle.normal.textColor = Color.white;

            // Bottom Row: Slider Track & Knob
            float trackX = boxRect.x + 18f;
            float trackY = boxRect.y + 54f;
            float trackW = boxRect.width - 36f;
            float trackH = 22f;
            Rect trackRect = new Rect(trackX, trackY, trackW, trackH);

            // Track Background
            if (texHudProgressBg != null)
            {
                GUI.color = Color.white;
                GUI.DrawTexture(trackRect, texHudProgressBg, ScaleMode.StretchToFill);
            }
            else
            {
                GUI.color = new Color(0.20f, 0.18f, 0.16f, 0.90f);
                GUI.Box(trackRect, GUIContent.none, cardStyle);
            }

            // Fill Bar
            float fillW = trackW * displayVol;
            if (displayVol > 0.001f)
            {
                Rect fillRect = new Rect(trackX + 2f, trackY + 2f, Mathf.Max(0f, fillW - 4f), trackH - 4f);
                if (texHudProgressFillCyan != null)
                {
                    GUI.color = isMuted ? new Color(0.6f, 0.6f, 0.6f, 0.8f) : Color.white;
                    GUI.DrawTexture(fillRect, texHudProgressFillCyan, ScaleMode.StretchToFill);
                }
                else
                {
                    GUI.color = isMuted ? new Color(0.6f, 0.6f, 0.6f) : new Color(0.18f, 0.80f, 0.84f);
                    GUI.Box(fillRect, GUIContent.none, headerStyle);
                }
            }

            // Knob / Thumb
            float knobSz = 30f;
            float knobCenterX = trackX + fillW;
            Rect knobRect = new Rect(knobCenterX - knobSz * 0.5f, trackY + (trackH - knobSz) * 0.5f, knobSz, knobSz);

            // Knob drop shadow
            GUI.color = new Color(0f, 0f, 0f, 0.35f);
            GUI.Box(new Rect(knobRect.x + 1f, knobRect.y + 2f, knobRect.width, knobRect.height), GUIContent.none, cardStyle);

            // Knob Body
            GUI.color = isMuted ? new Color(0.85f, 0.85f, 0.85f) : Color.white;
            GUI.Box(knobRect, GUIContent.none, cardStyle);

            // Knob Inner Indicator
            float dotSz = 12f;
            Rect dotRect = new Rect(knobCenterX - dotSz * 0.5f, trackY + (trackH - dotSz) * 0.5f, dotSz, dotSz);
            GUI.color = isMuted ? new Color(0.75f, 0.25f, 0.25f) : new Color(0.16f, 0.72f, 0.78f);
            GUI.Box(dotRect, GUIContent.none, headerStyle);

            // Mouse Click & Drag Event Handling
            Rect hitRect = new Rect(trackX - 15f, trackY - 14f, trackW + 30f, trackH + 28f);
            int sliderId = GUIUtility.GetControlID(FocusType.Passive);
            Event evt = Event.current;
            if (evt != null)
            {
                if (evt.type == EventType.MouseDown && hitRect.Contains(evt.mousePosition))
                {
                    GUIUtility.hotControl = sliderId;
                    float rel = Mathf.Clamp01((evt.mousePosition.x - trackX) / trackW);
                    float newVol = (rel < 0.03f) ? 0f : rel;
                    if (AudioManager.Instance != null) AudioManager.Instance.SetVolume(newVol);
                    evt.Use();
                }
                else if (evt.type == EventType.MouseDrag && GUIUtility.hotControl == sliderId)
                {
                    float rel = Mathf.Clamp01((evt.mousePosition.x - trackX) / trackW);
                    float newVol = (rel < 0.03f) ? 0f : rel;
                    if (AudioManager.Instance != null) AudioManager.Instance.SetVolume(newVol);
                    evt.Use();
                }
                else if (evt.type == EventType.MouseUp && GUIUtility.hotControl == sliderId)
                {
                    GUIUtility.hotControl = 0;
                    evt.Use();
                }
            }

            PopPopupTransform();
        }

        // =========================================================================
        // 6. LEVEL COMPLETE / GAME OVER MODAL (16:9 PROPORTIONED)
        // =========================================================================
        private void DrawLevelCompleteModal(OrderManager om, bool isVictory)
        {
            float elapsed = Time.unscaledTime - stateTransitionTime;

            // 1. Smooth dim background overlay fade
            float bgAlpha = Mathf.Clamp01(elapsed / 0.25f) * 0.78f;
            GUI.color = new Color(0f, 0f, 0f, bgAlpha);
            GUI.Box(new Rect(0, 0, REF_WIDTH, REF_HEIGHT), GUIContent.none, headerStyle);

            float w = 780f;
            float h = 650f;
            Rect modal = new Rect((REF_WIDTH - w) / 2f, (REF_HEIGHT - h) / 2f + 15f, w, h);

            // Pop-up elastic bounce transformation for the modal card
            float t = Mathf.Clamp01(elapsed / 0.38f);
            float modalScale = Mathf.Max(0.001f, Mathf.LerpUnclamped(0.35f, 1f, EaseOutBack(t, 2.0f)));
            float modalAlpha = Mathf.Clamp01(elapsed / 0.18f);
            Vector2 modalCenter = new Vector2(REF_WIDTH * 0.5f, REF_HEIGHT * 0.5f);

            PushPopupTransform(modalCenter, modalScale, modalAlpha);

            // Illustrated Modal Background Card
            if (texPanelCompletedBg != null)
            {
                GUI.color = Color.white;
                GUI.DrawTexture(modal, texPanelCompletedBg, ScaleMode.StretchToFill);
            }
            else
            {
                GUI.color = new Color(0.12f, 0.15f, 0.20f, 0.98f);
                GUI.Box(modal, GUIContent.none, modalStyle);

                Color headerCol = isVictory ? new Color(0.18f, 0.85f, 0.44f) : new Color(0.95f, 0.25f, 0.25f);
                GUI.color = headerCol;
                GUI.Box(new Rect(modal.x, modal.y, w, 6), GUIContent.none, headerStyle);
            }

            // 2. Title Banner (3D "LEVEL SELESAI!" with party poppers on victory, or Animated Red Pill on failure)
            if (isVictory && texTitleLevelSelesai != null)
            {
                float titleW = 540f;
                float titleH = titleW * (295f / 742f); // ~214f
                Rect titleRect = new Rect(modal.x + (w - titleW) / 2f, modal.y - 70f, titleW, titleH);

                float bannerT = Mathf.Clamp01((elapsed - 0.12f) / 0.28f);
                float bannerScale = Mathf.Max(0.001f, Mathf.LerpUnclamped(0.5f, 1f, EaseOutBack(bannerT, 2.2f)));
                Vector2 bannerCenter = new Vector2(titleRect.x + titleW * 0.5f, titleRect.y + titleH * 0.5f);

                PushPopupTransform(bannerCenter, bannerScale, Mathf.Clamp01(bannerT * 3f));
                GUI.color = Color.white;
                GUI.DrawTexture(titleRect, texTitleLevelSelesai, ScaleMode.ScaleToFit);
                PopPopupTransform();
            }
            else if (!isVictory)
            {
                // Animated Failure Header Pill (Soft Pastel Coral / Tomato)
                float bannerW = 440f;
                float bannerH = 54f;
                Rect bannerRect = new Rect(modal.x + (w - bannerW) / 2f, modal.y + 20f, bannerW, bannerH);

                float bannerT = Mathf.Clamp01((elapsed - 0.10f) / 0.26f);
                float bannerScale = Mathf.Max(0.001f, Mathf.LerpUnclamped(0.5f, 1f, EaseOutBack(bannerT, 2.0f)));
                Vector2 bannerCenter = new Vector2(bannerRect.x + bannerW * 0.5f, bannerRect.y + bannerH * 0.5f);

                PushPopupTransform(bannerCenter, bannerScale, Mathf.Clamp01(bannerT * 3f));

                // Soft pastel red background card
                GUI.color = new Color(1.0f, 0.90f, 0.90f, 0.98f);
                GUI.Box(bannerRect, GUIContent.none, cardStyle);

                // Top & Bottom accent border
                GUI.color = new Color(0.92f, 0.40f, 0.40f, 1f);
                GUI.Box(new Rect(bannerRect.x, bannerRect.y, bannerW, 3f), GUIContent.none, headerStyle);
                GUI.Box(new Rect(bannerRect.x, bannerRect.y + bannerH - 3f, bannerW, 3f), GUIContent.none, headerStyle);

                // Title Label
                GUI.color = Color.white;
                modalDarkTitleStyle.fontSize = 26;
                modalDarkTitleStyle.alignment = TextAnchor.MiddleCenter;
                GUI.Label(bannerRect, "<color=#B71C1C><b>LEVEL GAGAL!</b></color>", modalDarkTitleStyle);

                PopPopupTransform();
            }
            else
            {
                string titleText = "LEVEL SELESAI!";
                titleStyle.fontSize = 32;
                titleStyle.alignment = TextAnchor.MiddleCenter;
                GUI.color = Color.white;
                GUI.Label(new Rect(modal.x, modal.y + 24, w, 44), titleText, texPanelCompletedBg != null ? modalDarkTitleStyle : titleStyle);
            }

            // 3. 3 Star Rating (Cute Smiling Stars with Sequential Pop-in)
            float starsY = isVictory ? (modal.y + 115f) : (modal.y + 90f);
            float sSize = 58f;
            if (texStarGold != null && texStarEmpty != null)
            {
                float gap = 16f;
                float startStarsX = modal.x + (w - (3 * sSize + 2 * gap)) / 2f;

                // Base empty stars background
                GUI.color = new Color(1f, 1f, 1f, 0.45f);
                for (int s = 0; s < 3; s++)
                {
                    Rect slotRect = new Rect(startStarsX + s * (sSize + gap), starsY, sSize, sSize);
                    GUI.DrawTexture(slotRect, texStarEmpty, ScaleMode.ScaleToFit);
                }

                // Sequential punchy pop-up for each earned gold star!
                for (int s = 0; s < om.starsEarned; s++)
                {
                    float sStart = 0.32f + s * 0.14f;
                    if (elapsed >= sStart)
                    {
                        float st = Mathf.Clamp01((elapsed - sStart) / 0.22f);
                        float starScale = Mathf.Max(0.001f, Mathf.LerpUnclamped(0f, 1f, EaseOutBack(st, 2.6f)));
                        Rect starRect = new Rect(startStarsX + s * (sSize + gap), starsY, sSize, sSize);
                        Vector2 starCenter = new Vector2(starRect.x + sSize * 0.5f, starRect.y + sSize * 0.5f);

                        PushPopupTransform(starCenter, starScale, Mathf.Clamp01(st * 4f));
                        GUI.color = Color.white;
                        GUI.DrawTexture(starRect, texStarGold, ScaleMode.ScaleToFit);
                        PopPopupTransform();
                    }
                }
                starsY += sSize + 6f;
            }

            // 4. Star Rating Subtitle
            string starVisual;
            if (om.starsEarned >= 3) starVisual = "BINTANG 3: LUAR BIASA!";
            else if (om.starsEarned == 2) starVisual = "BINTANG 2: SANGAT BAGUS!";
            else if (om.starsEarned == 1) starVisual = "BINTANG 1: LULUS TARGET";
            else starVisual = "<color=#C0392B><b>TARGET BELUM TERCAPAI (ULANGI)</b></color>";

            float subAlpha = Mathf.Clamp01((elapsed - 0.25f) / 0.25f);
            modalDarkTitleStyle.fontSize = 20;
            modalDarkTitleStyle.alignment = TextAnchor.MiddleCenter;
            GUI.color = new Color(1f, 1f, 1f, subAlpha);
            GUI.Label(new Rect(modal.x, starsY, w, 28), starVisual, texPanelCompletedBg != null ? modalDarkTitleStyle : subtitleStyle);
            starsY += 34f;

            // 5. Stats Container Card (Illustrated Soft Blue Box)
            float statsW = 600f;
            float statsH = 180f;
            Rect statsRect = new Rect(modal.x + (w - statsW) / 2f, starsY, statsW, statsH);

            if (texStatsBoxBg != null)
            {
                GUI.color = Color.white;
                GUI.DrawTexture(statsRect, texStatsBoxBg, ScaleMode.StretchToFill);
            }
            else
            {
                GUI.color = new Color(0.16f, 0.20f, 0.26f, 0.92f);
                GUI.Box(statsRect, GUIContent.none, cardStyle);
            }

            int target = om.activeConfig != null ? om.activeConfig.targetScore : 100;
            string targetColor = om.score >= target ? "#1B5E20" : "#B71C1C";
            GUIStyle textUseStyle = texStatsBoxBg != null ? modalDarkTextStyle : textStyle;
            GUIStyle boldUseStyle = texStatsBoxBg != null ? modalDarkBoldStyle : boldTextStyle;

            GUI.color = Color.white;
            GUI.Label(new Rect(statsRect.x + 28, statsRect.y + 16, statsRect.width - 56, 30), 
                $"Total Pendapatan: <color={targetColor}><b>${om.score}</b></color>  (Target: ${target})", boldUseStyle);

            // Delivered line with Checkmark icon
            float iconSize = 24f;
            float line2Y = statsRect.y + 54;
            if (texIconCheck != null)
            {
                GUI.DrawTexture(new Rect(statsRect.x + 28, line2Y + 1, iconSize, iconSize), texIconCheck, ScaleMode.ScaleToFit);
                GUI.Label(new Rect(statsRect.x + 60, line2Y, statsRect.width - 70, 26), 
                    $"Pesanan Berhasil Disajikan: <b>{om.deliveredCount} Pelanggan</b>", textUseStyle);
            }
            else
            {
                GUI.Label(new Rect(statsRect.x + 28, line2Y, statsRect.width - 56, 26), 
                    $"Pesanan Berhasil Disajikan: <b>{om.deliveredCount} Pelanggan</b>", textUseStyle);
            }

            // Failed line with Cross icon
            float line3Y = statsRect.y + 90;
            if (texIconCross != null)
            {
                GUI.DrawTexture(new Rect(statsRect.x + 28, line3Y + 1, iconSize, iconSize), texIconCross, ScaleMode.ScaleToFit);
                GUI.Label(new Rect(statsRect.x + 60, line3Y, statsRect.width - 70, 26), 
                    $"Pesanan Hangus / Kecewa: <b>{om.failedCount} Pelanggan</b>", textUseStyle);
            }
            else
            {
                GUI.Label(new Rect(statsRect.x + 28, line3Y, statsRect.width - 56, 26), 
                    $"Pesanan Hangus / Kecewa: <b>{om.failedCount} Pelanggan</b>", textUseStyle);
            }

            // Highscore line with Trophy icon
            float line4Y = statsRect.y + 126;
            int best = om.GetBestScore(om.currentLevel);
            if (texIconTrophy != null)
            {
                GUI.DrawTexture(new Rect(statsRect.x + 28, line4Y + 1, iconSize, iconSize), texIconTrophy, ScaleMode.ScaleToFit);
                GUI.Label(new Rect(statsRect.x + 60, line4Y, statsRect.width - 70, 26), 
                    $"Skor Tertinggi Level Ini: <color=#B78103><b>${best}</b></color>", textUseStyle);
            }
            else
            {
                GUI.Label(new Rect(statsRect.x + 28, line4Y, statsRect.width - 56, 26), 
                    $"Skor Tertinggi Level Ini: <color=#F1C40F><b>${best}</b></color>", textUseStyle);
            }

            // 6. Action Buttons
            float btnsY = statsRect.y + statsH + 18f;

            if (isVictory && om.currentLevel != GameLevel.Level3)
            {
                // Green Button: "LEVEL SELANJUTNYA"
                float btnNextW = 480f;
                float btnNextH = 54f;
                Rect nextRect = new Rect(modal.x + (w - btnNextW) / 2f, btnsY, btnNextW, btnNextH);

                DrawModalSubButton(nextRect, texBtnLevelSelanjutnya, "LEVEL SELANJUTNYA", new Color(0.18f, 0.85f, 0.44f), () => om.NextLevel());

                btnsY += 62f;

                // Two buttons below: Coba Lagi & Menu Utama
                float subBtnW = 230f;
                float subBtnH = 50f;
                float gap = 20f;
                float startSubX = modal.x + (w - (2 * subBtnW + gap)) / 2f;
                Rect retryRect = new Rect(startSubX, btnsY, subBtnW, subBtnH);
                Rect homeRect = new Rect(startSubX + subBtnW + gap, btnsY, subBtnW, subBtnH);

                DrawModalSubButton(retryRect, texBtnCobaLagi, "COBA LAGI", new Color(0.95f, 0.65f, 0.15f), () => om.RestartCurrentLevel());
                DrawModalSubButton(homeRect, texBtnMenuUtama, "MENU UTAMA", new Color(0.6f, 0.65f, 0.72f), () => om.GoToMainMenu());
            }
            else
            {
                // Game Over (Failed Target) or Final Level 3 Victory -> Only Coba Lagi & Menu Utama (NO Next Level Button!)
                if (!isVictory)
                {
                    // Helpful Retry Hint Box
                    Rect tipBox = new Rect(modal.x + (w - 560f) / 2f, btnsY, 560f, 38f);
                    GUI.color = new Color(1f, 0.95f, 0.88f, 0.95f);
                    GUI.Box(tipBox, GUIContent.none, cardStyle);
                    GUI.color = Color.white;
                    tipStyle.fontSize = 14;
                    GUI.Label(tipBox, "<b>Tips:</b> Layani pelanggan secepat mungkin untuk bonus tip & raih target!", tipStyle);
                    btnsY += 46f;
                }

                float subBtnW = 260f;
                float subBtnH = 54f;
                float gap = 30f;
                float startSubX = modal.x + (w - (2 * subBtnW + gap)) / 2f;
                Rect retryRect = new Rect(startSubX, btnsY, subBtnW, subBtnH);
                Rect homeRect = new Rect(startSubX + subBtnW + gap, btnsY, subBtnW, subBtnH);

                DrawModalSubButton(retryRect, texBtnCobaLagi, "COBA LAGI", new Color(0.95f, 0.65f, 0.15f), () => om.RestartCurrentLevel());
                DrawModalSubButton(homeRect, texBtnMenuUtama, "MENU UTAMA", new Color(0.6f, 0.65f, 0.72f), () => om.GoToMainMenu());
            }

            PopPopupTransform();
        }

        private void PlayButtonClick()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayButtonClickSFX();
            }
        }

        private void DrawModalSubButton(Rect r, Texture2D tex, string fallbackLabel, Color fallbackCol, System.Action onClick)
        {
            bool hover = r.Contains(Event.current.mousePosition);
            float btnScale = hover ? 1.04f : 1.0f;
            Vector2 center = new Vector2(r.x + r.width * 0.5f, r.y + r.height * 0.5f);

            PushPopupTransform(center, btnScale, 1f);

            if (tex != null)
            {
                GUI.color = hover ? new Color(1.08f, 1.08f, 1.08f, 1f) : Color.white;
                GUI.DrawTexture(r, tex, ScaleMode.ScaleToFit);
                if (GUI.Button(r, GUIContent.none, GUIStyle.none))
                {
                    PlayButtonClick();
                    onClick?.Invoke();
                }
            }
            else
            {
                GUI.color = fallbackCol;
                buttonStyle.fontSize = 16;
                if (GUI.Button(r, fallbackLabel, buttonStyle))
                {
                    PlayButtonClick();
                    onClick?.Invoke();
                }
            }

            PopPopupTransform();
        }

        private void DrawPillButton(Rect r, Texture2D tex, string label, Color fallbackCol, int fontSize, System.Action onClick)
        {
            bool hover = r.Contains(Event.current.mousePosition);
            float btnScale = hover ? 1.04f : 1.0f;
            Vector2 center = new Vector2(r.x + r.width * 0.5f, r.y + r.height * 0.5f);

            PushPopupTransform(center, btnScale, 1f);

            if (tex != null)
            {
                GUI.color = hover ? new Color(1.10f, 1.10f, 1.10f, 1f) : Color.white;
                GUI.DrawTexture(r, tex, ScaleMode.StretchToFill);

                GUIStyle btnTextStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = fontSize,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };

                // Drop shadow
                btnTextStyle.normal.textColor = new Color(0.08f, 0.20f, 0.10f, 0.85f);
                GUI.Label(new Rect(r.x, r.y + 2, r.width, r.height), label, btnTextStyle);

                // Foreground
                btnTextStyle.normal.textColor = Color.white;
                GUI.Label(r, label, btnTextStyle);

                if (GUI.Button(r, GUIContent.none, GUIStyle.none))
                {
                    PlayButtonClick();
                    onClick?.Invoke();
                }
            }
            else
            {
                GUI.color = fallbackCol;
                primaryButtonStyle.fontSize = fontSize;
                if (GUI.Button(r, label, primaryButtonStyle))
                {
                    PlayButtonClick();
                    onClick?.Invoke();
                }
            }

            PopPopupTransform();
        }

        private void DrawCenteredMenuButton(Rect r, Texture2D tex, string text, Color fallbackCol, int fontSize, System.Action onClick, bool isClickable = true)
        {
            bool hover = isClickable && r.Contains(Event.current.mousePosition);
            float btnScale = hover ? 1.025f : 1.0f;
            Vector2 center = new Vector2(r.x + r.width * 0.5f, r.y + r.height * 0.5f);

            PushPopupTransform(center, btnScale, 1f);

            if (tex != null)
            {
                GUI.color = hover ? new Color(1.10f, 1.10f, 1.10f, 1f) : Color.white;
                GUI.DrawTexture(r, tex, ScaleMode.StretchToFill);

                // Offset text slightly to the right if button has icon on the left (play/recipe icons)
                float leftOffset = (tex == texMainMenuBtnPlay || tex == texMainMenuBtnRecipe) ? 26f : 0f;
                Rect labelRect = new Rect(r.x + leftOffset, r.y, r.width - leftOffset, r.height);

                GUIStyle btnStyle = new GUIStyle(GUI.skin.label)
                {
                    font = customFont,
                    fontSize = fontSize,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };
                btnStyle.normal.textColor = new Color(0.16f, 0.12f, 0.08f);

                GUI.color = Color.white;
                GUI.Label(labelRect, text, btnStyle);

                if (isClickable && GUI.Button(r, GUIContent.none, GUIStyle.none))
                {
                    PlayButtonClick();
                    onClick?.Invoke();
                }
            }
            else
            {
                GUI.color = fallbackCol;
                buttonStyle.fontSize = fontSize;
                if (isClickable)
                {
                    if (GUI.Button(r, text, buttonStyle))
                    {
                        PlayButtonClick();
                        onClick?.Invoke();
                    }
                }
                else
                {
                    GUI.Box(r, text, buttonStyle);
                }
            }

            PopPopupTransform();
        }

        private void DrawMenuPillButton(Rect r, Texture2D tex, string title, string subtitle, Color fallbackCol, int fontSize, System.Action onClick)
        {
            bool hover = r.Contains(Event.current.mousePosition);
            float btnScale = hover ? 1.03f : 1.0f;
            Vector2 center = new Vector2(r.x + r.width * 0.5f, r.y + r.height * 0.5f);

            PushPopupTransform(center, btnScale, 1f);

            if (tex != null)
            {
                GUI.color = hover ? new Color(1.10f, 1.10f, 1.10f, 1f) : Color.white;
                GUI.DrawTexture(r, tex, ScaleMode.StretchToFill);

                // Offset text to the right because play/recipe icons are on the left
                float textLeftPad = r.height * 0.90f;
                Rect titleRect = new Rect(r.x + textLeftPad, r.y + (string.IsNullOrEmpty(subtitle) ? (r.height - 24f) * 0.5f : 14f), r.width - textLeftPad - 16f, 26f);
                Rect subRect = new Rect(r.x + textLeftPad, r.y + 40f, r.width - textLeftPad - 16f, 20f);

                GUIStyle titleStyleBtn = new GUIStyle(GUI.skin.label)
                {
                    font = customFont,
                    fontSize = fontSize,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleLeft
                };
                titleStyleBtn.normal.textColor = new Color(0.18f, 0.12f, 0.08f); // Bold dark brown/black

                GUI.color = Color.white;
                GUI.Label(titleRect, title, titleStyleBtn);

                if (!string.IsNullOrEmpty(subtitle))
                {
                    GUIStyle subStyleBtn = new GUIStyle(GUI.skin.label)
                    {
                        font = customFont,
                        fontSize = Mathf.Max(11, fontSize - 6),
                        fontStyle = FontStyle.Normal,
                        alignment = TextAnchor.MiddleLeft
                    };
                    subStyleBtn.normal.textColor = new Color(0.35f, 0.28f, 0.22f);
                    GUI.Label(subRect, subtitle, subStyleBtn);
                }

                if (GUI.Button(r, GUIContent.none, GUIStyle.none))
                {
                    PlayButtonClick();
                    onClick?.Invoke();
                }
            }
            else
            {
                GUI.color = fallbackCol;
                primaryButtonStyle.fontSize = fontSize;
                if (GUI.Button(r, title, primaryButtonStyle))
                {
                    PlayButtonClick();
                    onClick?.Invoke();
                }
            }

            PopPopupTransform();
        }

        private void DrawIconButton(Rect r, Texture2D tex, string fallbackLabel, Color fallbackCol, System.Action onClick, bool isClickable = true)
        {
            bool hover = isClickable && r.Contains(Event.current.mousePosition);
            float btnScale = hover ? 1.08f : 1.0f;
            Vector2 center = new Vector2(r.x + r.width * 0.5f, r.y + r.height * 0.5f);

            PushPopupTransform(center, btnScale, 1f);

            if (tex != null)
            {
                GUI.color = hover ? new Color(1.15f, 1.15f, 1.15f, 1f) : Color.white;
                GUI.DrawTexture(r, tex, ScaleMode.StretchToFill);

                if (isClickable && GUI.Button(r, GUIContent.none, GUIStyle.none))
                {
                    PlayButtonClick();
                    onClick?.Invoke();
                }
            }
            else
            {
                GUI.color = fallbackCol;
                buttonStyle.fontSize = 20;
                if (isClickable)
                {
                    if (GUI.Button(r, fallbackLabel, buttonStyle))
                    {
                        PlayButtonClick();
                        onClick?.Invoke();
                    }
                }
                else
                {
                    GUI.Box(r, fallbackLabel, buttonStyle);
                }
            }

            PopPopupTransform();
        }

        private void DrawQuickHelpModal(OrderManager om)
        {
            EnsureHelpTabTextures();

            // Dimmed overlay
            GUI.color = new Color(0.02f, 0.03f, 0.05f, 0.78f);
            GUI.Box(new Rect(0, 0, REF_WIDTH, REF_HEIGHT), GUIContent.none, headerStyle);

            float w = 1040f;
            float h = 790f;
            Rect modalRect = new Rect((REF_WIDTH - w) * 0.5f, (REF_HEIGHT - h) * 0.5f, w, h);

            // Click outside modal bounds closes it
            if (Event.current.type == EventType.MouseDown && !modalRect.Contains(Event.current.mousePosition))
            {
                showHelpModal = false;
                Event.current.Use();
                return;
            }

            // Modal Background
            if (texMainMenuPanelBg != null)
            {
                GUI.color = Color.white;
                GUI.DrawTexture(modalRect, texMainMenuPanelBg, ScaleMode.StretchToFill);
            }
            else
            {
                GUI.color = new Color(0.96f, 0.93f, 0.88f, 0.98f);
                GUI.Box(modalRect, GUIContent.none, modalStyle);
            }

            // Title Header at the top (Clean text, no emoji to avoid missing glyphs on Android)
            modalDarkTitleStyle.fontSize = 24;
            modalDarkTitleStyle.alignment = TextAnchor.MiddleCenter;
            modalDarkTitleStyle.normal.textColor = new Color(0.24f, 0.18f, 0.14f);
            GUI.color = Color.white;
            GUI.Label(new Rect(modalRect.x, modalRect.y + 14f, w, 32f), "PANDUAN LENGKAP & ATURAN RESTORAN", modalDarkTitleStyle);

            // Tab Selector (Two distinct tabs with custom textures)
            float tabY = modalRect.y + 52f;
            float tabH = 58f;
            float tabGap = 16f;
            float tabMargin = 28f;
            float tabW = (w - tabMargin * 2f - tabGap) * 0.5f;

            Rect tab0Rect = new Rect(modalRect.x + tabMargin, tabY, tabW, tabH);
            Rect tab1Rect = new Rect(tab0Rect.xMax + tabGap, tabY, tabW, tabH);

            DrawHelpTabButton(tab0Rect, texTabBtnCyan, "PANDUAN KONTROL & ALUR", helpModalTab == 0, new Color(0.18f, 0.72f, 0.45f), () =>
            {
                if (helpModalTab != 0)
                {
                    helpModalTab = 0;
                    helpScrollPos = Vector2.zero;
                }
            });

            DrawHelpTabButton(tab1Rect, texTabBtnSalmon, "ATURAN SKOR & PENALTI", helpModalTab == 1, new Color(0.92f, 0.55f, 0.15f), () =>
            {
                if (helpModalTab != 1)
                {
                    helpModalTab = 1;
                    helpScrollPos = Vector2.zero;
                }
            });

            // Scroll View Area
            float scrollY = tabY + tabH + 10f;
            float bottomBarH = 62f;
            float scrollH = modalRect.height - (scrollY - modalRect.y) - bottomBarH - 8f;
            Rect scrollRect = new Rect(modalRect.x + 24f, scrollY, w - 48f, scrollH);

            float contentInnerW = scrollRect.width - 24f;
            float innerContentH = (helpModalTab == 0) ? 680f : 1650f;
            float maxScroll = Mathf.Max(0f, innerContentH - scrollH);

            // 1. Android Touchscreen Drag-to-Scroll
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                Vector2 guiTouch = ScreenToGUI(touch.position);
                if (scrollRect.Contains(guiTouch))
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        isDraggingHelpScroll = true;
                        helpDragStartScrollY = helpScrollPos.y;
                        helpDragStartMouseY = guiTouch.y;
                    }
                    else if (touch.phase == TouchPhase.Moved && isDraggingHelpScroll)
                    {
                        float s = guiScale > 0.0001f ? guiScale : 1f;
                        float deltaGUI = touch.deltaPosition.y / s;
                        helpScrollPos.y = Mathf.Clamp(helpScrollPos.y + deltaGUI, 0f, maxScroll);
                    }
                    else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        isDraggingHelpScroll = false;
                    }
                }
            }
            else
            {
                // 2. PC / Editor Mouse Drag-to-Scroll via IMGUI Events
                Event e = Event.current;
                if (e != null)
                {
                    if (e.type == EventType.MouseDown && scrollRect.Contains(e.mousePosition))
                    {
                        isDraggingHelpScroll = true;
                        helpDragStartMouseY = e.mousePosition.y;
                        helpDragStartScrollY = helpScrollPos.y;
                    }
                    else if (e.type == EventType.MouseDrag && isDraggingHelpScroll)
                    {
                        float deltaY = e.mousePosition.y - helpDragStartMouseY;
                        helpScrollPos.y = Mathf.Clamp(helpDragStartScrollY - deltaY, 0f, maxScroll);
                    }
                    else if (e.type == EventType.MouseUp)
                    {
                        isDraggingHelpScroll = false;
                    }
                }
            }

            helpScrollPos = GUI.BeginScrollView(scrollRect, helpScrollPos, new Rect(0, 0, contentInnerW, innerContentH), false, true);

            if (helpModalTab == 0)
            {
                DrawHelpControlsTab(contentInnerW);
            }
            else
            {
                DrawHelpGameplayTab(om, contentInnerW);
            }

            GUI.EndScrollView();

            // Bottom Exit Button
            float btnExitW = 420f;
            float btnExitH = 52f;
            Rect btnExitRect = new Rect(modalRect.x + (w - btnExitW) * 0.5f, modalRect.y + h - 60f, btnExitW, btnExitH);
            DrawCenteredMenuButton(btnExitRect, texMainMenuBtnExit, "TUTUP PANDUAN [ESC]", new Color(0.95f, 0.45f, 0.45f), 17, () =>
            {
                showHelpModal = false;
            });

            // Close on ESC
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
            {
                showHelpModal = false;
                Event.current.Use();
            }
        }

        private void DrawHelpTabButton(Rect r, Texture2D tex, string fallbackTitle, bool isActive, Color activeColor, System.Action onClick)
        {
            bool hover = r.Contains(Event.current.mousePosition);
            float scale = hover ? 1.02f : 1.0f;
            Vector2 center = new Vector2(r.x + r.width * 0.5f, r.y + r.height * 0.5f);

            PushPopupTransform(center, scale, 1f);

            if (tex != null)
            {
                if (isActive)
                {
                    // Aktif: Tampilan penuh cerah
                    GUI.color = hover ? new Color(1.10f, 1.10f, 1.10f, 1f) : Color.white;
                }
                else
                {
                    // Nonaktif: Dibuat lebih gelap dari yang aktif
                    GUI.color = hover ? new Color(0.68f, 0.68f, 0.68f, 0.95f) : new Color(0.48f, 0.48f, 0.48f, 0.88f);
                }

                GUI.DrawTexture(r, tex, ScaleMode.StretchToFill);
            }
            else
            {
                if (isActive)
                {
                    GUI.color = activeColor;
                    GUI.Box(r, GUIContent.none, primaryButtonStyle ?? buttonStyle);

                    // Bright highlight bar at bottom
                    GUI.color = Color.white;
                    GUI.DrawTexture(new Rect(r.x + 16f, r.yMax - 4f, r.width - 32f, 4f), whiteBoxTexture);

                    modalDarkBoldStyle.fontSize = 15;
                    modalDarkBoldStyle.alignment = TextAnchor.MiddleCenter;
                    modalDarkBoldStyle.normal.textColor = Color.white;
                    GUI.Label(r, fallbackTitle, modalDarkBoldStyle);
                }
                else
                {
                    // Nonaktif fallback: Lebih gelap
                    GUI.color = hover ? new Color(0.65f, 0.62f, 0.58f, 0.95f) : new Color(0.45f, 0.42f, 0.38f, 0.88f);
                    GUI.Box(r, GUIContent.none, cardStyle);

                    modalDarkBoldStyle.fontSize = 15;
                    modalDarkBoldStyle.alignment = TextAnchor.MiddleCenter;
                    modalDarkBoldStyle.normal.textColor = hover ? new Color(0.18f, 0.15f, 0.12f) : new Color(0.42f, 0.36f, 0.30f);
                    GUI.Label(r, fallbackTitle, modalDarkBoldStyle);
                }

                // Restore style color
                modalDarkBoldStyle.normal.textColor = new Color(0.18f, 0.22f, 0.25f);
            }

            if (GUI.Button(r, GUIContent.none, GUIStyle.none))
            {
                PlayButtonClick();
                onClick?.Invoke();
            }

            PopPopupTransform();
        }

        private void DrawHelpControlsTab(float cw)
        {
            float padding = 16f;
            float innerW = cw - padding * 2f;

            // CARD 1: KONTROL TOMBOL
            float c1H = 340f;
            Rect c1 = new Rect(0, 0, cw, c1H);
            GUI.color = new Color(0.96f, 0.93f, 0.88f, 0.96f);
            GUI.Box(c1, GUIContent.none, cardStyle);

            GUI.color = Color.white;
            modalDarkBoldStyle.fontSize = 17;
            modalDarkBoldStyle.alignment = TextAnchor.MiddleLeft;
            modalDarkBoldStyle.normal.textColor = new Color(0.18f, 0.22f, 0.25f);
            GUI.Label(new Rect(padding, 12f, innerW, 26f), "<b>PANDUAN KONTROL LENGKAP (PC & MOBILE):</b>", modalDarkBoldStyle);

            float colW = (innerW - 20f) * 0.5f;

            // Sub-box 1: PC Keyboard
            Rect pcBox = new Rect(padding, 44f, colW, 240f);
            GUI.color = new Color(0.91f, 0.93f, 0.95f, 0.92f);
            GUI.Box(pcBox, GUIContent.none, cardStyle);

            GUI.color = Color.white;
            modalDarkBoldStyle.fontSize = 14;
            modalDarkBoldStyle.normal.textColor = new Color(0.12f, 0.25f, 0.40f);
            GUI.Label(new Rect(pcBox.x + 12f, pcBox.y + 10f, pcBox.width - 24f, 22f), "<b>[PC] KEYBOARD KOMPUTER</b>", modalDarkBoldStyle);

            modalDarkTextStyle.fontSize = 13;
            modalDarkTextStyle.normal.textColor = new Color(0.20f, 0.24f, 0.28f);
            float y = pcBox.y + 36f;
            GUI.Label(new Rect(pcBox.x + 12f, y, pcBox.width - 24f, 20f), "• <b>[W] [A] [S] [D] / Panah:</b> Gerak Koki", modalDarkTextStyle); y += 22f;
            GUI.Label(new Rect(pcBox.x + 12f, y, pcBox.width - 24f, 20f), "• <b>[E]:</b> Ambil / Taruh Bahan & Piring", modalDarkTextStyle); y += 22f;
            GUI.Label(new Rect(pcBox.x + 12f, y, pcBox.width - 24f, 20f), "• <b>[Spasi] / [F]:</b> Memotong di Meja Iris", modalDarkTextStyle); y += 22f;
            GUI.Label(new Rect(pcBox.x + 12f, y, pcBox.width - 24f, 20f), "• <b>Tong Sampah:</b> Dekati & tekan <b>[E]</b> untuk buang", modalDarkTextStyle); y += 22f;
            GUI.Label(new Rect(pcBox.x + 12f, y, pcBox.width - 24f, 20f), "• <b>[M]:</b> Mute / Unmute Audio Musik", modalDarkTextStyle); y += 22f;
            GUI.Label(new Rect(pcBox.x + 12f, y, pcBox.width - 24f, 20f), "• <b>[ESC]:</b> Buka Menu Jeda (Pause)", modalDarkTextStyle);

            // Sub-box 2: Mobile Touchscreen
            Rect mobBox = new Rect(pcBox.xMax + 20f, 44f, colW, 240f);
            GUI.color = new Color(0.92f, 0.95f, 0.91f, 0.92f);
            GUI.Box(mobBox, GUIContent.none, cardStyle);

            GUI.color = Color.white;
            modalDarkBoldStyle.fontSize = 14;
            modalDarkBoldStyle.normal.textColor = new Color(0.12f, 0.38f, 0.22f);
            GUI.Label(new Rect(mobBox.x + 12f, mobBox.y + 10f, mobBox.width - 24f, 22f), "<b>[MOBILE] LAYAR SENTUH HP</b>", modalDarkBoldStyle);

            y = mobBox.y + 36f;
            GUI.Label(new Rect(mobBox.x + 12f, y, mobBox.width - 24f, 20f), "• <b>Virtual Joystick:</b> Geser analog kiri bawah untuk gerak", modalDarkTextStyle); y += 22f;
            GUI.Label(new Rect(mobBox.x + 12f, y, mobBox.width - 24f, 20f), "• <b>Tombol Cyan (Tangan):</b> Ambil / Taruh barang", modalDarkTextStyle); y += 22f;
            GUI.Label(new Rect(mobBox.x + 12f, y, mobBox.width - 24f, 20f), "• <b>Tombol Oranye (Pisau):</b> Memotong sayuran", modalDarkTextStyle); y += 22f;
            GUI.Label(new Rect(mobBox.x + 12f, y, mobBox.width - 24f, 20f), "• <b>Tombol Pause (HUD):</b> Jeda permainan di kanan atas", modalDarkTextStyle); y += 22f;
            GUI.Label(new Rect(mobBox.x + 12f, y, mobBox.width - 24f, 20f), "• <b>Tombol Audio (HUD):</b> Mute/Unmute musik di kanan atas", modalDarkTextStyle);

            tipStyle.fontSize = 12;
            tipStyle.alignment = TextAnchor.MiddleCenter;
            tipStyle.normal.textColor = new Color(0.40f, 0.34f, 0.28f);
            GUI.color = Color.white;
            GUI.Label(new Rect(padding, 296f, innerW, 24f), "<i>[INFO] Kontrol PC & Mobile dapat digunakan secara bersamaan tanpa perlu mengubah pengaturan!</i>", tipStyle);

            // CARD 2: 5 LANGKAH ALUR KERJA DAPUR
            float c2Y = c1.yMax + 14f;
            float c2H = 310f;
            Rect c2 = new Rect(0, c2Y, cw, c2H);
            GUI.color = new Color(0.93f, 0.95f, 0.92f, 0.96f);
            GUI.Box(c2, GUIContent.none, cardStyle);

            GUI.color = Color.white;
            modalDarkBoldStyle.fontSize = 17;
            modalDarkBoldStyle.normal.textColor = new Color(0.18f, 0.22f, 0.25f);
            GUI.Label(new Rect(padding, c2Y + 12f, innerW, 26f), "<b>5 LANGKAH UTAMA ALUR KERJA DAPUR (WORKFLOW):</b>", modalDarkBoldStyle);

            y = c2Y + 44f;
            DrawWorkflowStepRow(padding, y, innerW, "1", "Ambil Piring Bersih", "Ambil piring dari Rak Piring (Plate Counter). Piring adalah wadah mutlak untuk merakit tumpukan burger."); y += 46f;
            DrawWorkflowStepRow(padding, y, innerW, "2", "Panggang Patty Daging", "Letakkan patty mentah di atas Panggangan/Grill. Tunggu bar hijau penuh! Hati-hati jangan sampai gosong!"); y += 46f;
            DrawWorkflowStepRow(padding, y, innerW, "3", "Potong Sayuran Segar", "Bawa selada/tomat ke Meja Potong. Tekan tombol potong berkali-kali hingga sayuran menjadi irisan siap saji."); y += 46f;
            DrawWorkflowStepRow(padding, y, innerW, "4", "Rakit Bahan ke Piring", "Dekati piring lalu tekan tombol Ambil/Taruh untuk menyusun Roti, Patty matang, Keju, dan Sayuran sesuai tiket."); y += 46f;
            DrawWorkflowStepRow(padding, y, innerW, "5", "Antar ke Meja Saji", "Angkat piring burger lengkap dan bawa ke Meja Saji (Delivery Counter) sebelum batas waktu sabar pelanggan habis!"); y += 46f;

            GUI.Label(new Rect(padding, y + 2f, innerW, 22f), "<i>[TIPS] Semakin cepat pesanan diantar saat meteran kesabaran masih hijau penuh, semakin besar bonus uang tip yang didapat!</i>", tipStyle);
        }

        private void DrawWorkflowStepRow(float x, float y, float w, string num, string stepTitle, string stepDesc)
        {
            // Number badge box
            Rect numRect = new Rect(x, y + 2f, 26f, 26f);
            GUI.color = new Color(0.18f, 0.72f, 0.45f);
            GUI.Box(numRect, GUIContent.none, primaryButtonStyle ?? buttonStyle);

            modalDarkBoldStyle.fontSize = 14;
            modalDarkBoldStyle.alignment = TextAnchor.MiddleCenter;
            modalDarkBoldStyle.normal.textColor = Color.white;
            GUI.Label(numRect, num, modalDarkBoldStyle);

            // Step Title & Description
            modalDarkBoldStyle.fontSize = 13;
            modalDarkBoldStyle.alignment = TextAnchor.MiddleLeft;
            modalDarkBoldStyle.normal.textColor = new Color(0.15f, 0.18f, 0.22f);
            GUI.Label(new Rect(x + 34f, y, 210f, 24f), stepTitle, modalDarkBoldStyle);

            modalDarkTextStyle.fontSize = 12;
            modalDarkTextStyle.alignment = TextAnchor.MiddleLeft;
            modalDarkTextStyle.normal.textColor = new Color(0.30f, 0.35f, 0.40f);
            GUI.Label(new Rect(x + 248f, y, w - 248f, 38f), stepDesc, modalDarkTextStyle);

            modalDarkBoldStyle.normal.textColor = new Color(0.18f, 0.22f, 0.25f);
        }

        private void DrawHelpGameplayTab(OrderManager om, float cw)
        {
            float padding = 16f;
            float innerW = cw - padding * 2f;
            float curY = 0f;

            // CARD 1: ATURAN OMSET & TIP PELANGGAN
            float c1H = 210f;
            Rect c1 = new Rect(0, curY, cw, c1H);
            GUI.color = new Color(0.95f, 0.93f, 0.88f, 0.96f);
            GUI.Box(c1, GUIContent.none, cardStyle);

            GUI.color = Color.white;
            modalDarkBoldStyle.fontSize = 17;
            modalDarkBoldStyle.alignment = TextAnchor.MiddleLeft;
            modalDarkBoldStyle.normal.textColor = new Color(0.18f, 0.22f, 0.25f);
            GUI.Label(new Rect(padding, curY + 12f, innerW, 26f), "<b>ATURAN OMSET PENDAPATAN & TIP PELANGGAN:</b>", modalDarkBoldStyle);

            float y = curY + 42f;
            modalDarkTextStyle.fontSize = 13;
            modalDarkTextStyle.normal.textColor = new Color(0.20f, 0.24f, 0.28f);

            GUI.Label(new Rect(padding, y, innerW, 22f), 
                "• <b>Pesanan Berhasil Disajikan:</b> Mendapatkan omset senilai harga resep burger (<b>+$35 s/d +$95</b>).", modalDarkTextStyle);
            y += 26f;

            GUI.Label(new Rect(padding, y, innerW, 22f), 
                "• <b>Bonus Tip Pelanggan Cepat:</b> Bonus ekstra acak (<b>+$1 s/d +$10</b>) tergantung seberapa cepat pesanan disajikan.", modalDarkTextStyle);
            y += 24f;
            GUI.Label(new Rect(padding + 18f, y, innerW - 18f, 20f), 
                "<i>(Semakin tebal bar hijau waktu sabar pelanggan saat disajikan, semakin besar bonus tip yang Anda peroleh!)</i>", tipStyle);
            y += 28f;

            GUI.Label(new Rect(padding, y, innerW, 22f), 
                "• <b>Penalti Pesanan Hangus / Terlambat:</b> Jika waktu sabar pelanggan habis, pelanggan pergi dengan kecewa.", modalDarkTextStyle);
            y += 24f;
            modalDarkBoldStyle.fontSize = 13;
            modalDarkBoldStyle.normal.textColor = new Color(0.75f, 0.18f, 0.18f);
            GUI.Label(new Rect(padding + 18f, y, innerW - 18f, 20f), 
                "Toko dikenakan sanksi denda pemotongan omset sebesar -$15 per pesanan yang hangus!", modalDarkBoldStyle);
            modalDarkBoldStyle.normal.textColor = new Color(0.18f, 0.22f, 0.25f);

            curY += c1H + 14f;

            // CARD 2: ATURAN PENGURANGAN SKOR TEMPAT SAMPAH (TRASHBIN PENALTY)
            float c2H = 410f;
            Rect c2 = new Rect(0, curY, cw, c2H);
            GUI.color = new Color(0.98f, 0.92f, 0.90f, 0.96f);
            GUI.Box(c2, GUIContent.none, cardStyle);

            GUI.color = Color.white;
            modalDarkBoldStyle.fontSize = 17;
            modalDarkBoldStyle.normal.textColor = new Color(0.75f, 0.18f, 0.18f);
            GUI.Label(new Rect(padding, curY + 12f, innerW, 26f), "<b>ATURAN DENDA TEMPAT SAMPAH (PEMBOROSAN BAHAN BAKU):</b>", modalDarkBoldStyle);

            y = curY + 40f;
            modalDarkTextStyle.fontSize = 13;
            modalDarkTextStyle.normal.textColor = new Color(0.25f, 0.20f, 0.20f);
            GUI.Label(new Rect(padding, y, innerW, 36f), 
                "Membuang bahan makanan yang masih layak pakai atau piring ke tong sampah dianggap pemborosan modal restoran. " +
                "Omset toko langsung dipotong sesuai nilai denda bahan (Omset tidak akan minus / minimum $0):", modalDarkTextStyle);
            y += 42f;

            // Denda Table (Clean Grid Box with In-Game Textures and ui_stats_box_bg Badges)
            Rect tableRect = new Rect(padding, y, innerW, 236f);
            GUI.color = new Color(1.0f, 0.97f, 0.95f, 0.95f);
            GUI.Box(tableRect, GUIContent.none, cardStyle);

            float colW = (innerW - 24f) / 3f;
            float rowH = 48f;
            float ty = tableRect.y + 10f;

            // Baris 1: Roti, Selada Segar, Irisan Selada
            DrawPenaltyItemBox(new Rect(tableRect.x + 8f, ty, colW, rowH), ItemDatabase.GetTexture(ItemType.BunBottom), "Roti Burger", "-$3", "Bahan dasar roti");
            DrawPenaltyItemBox(new Rect(tableRect.x + 8f + colW + 4f, ty, colW, rowH), ItemDatabase.GetTexture(ItemType.Lettuce), "Selada Segar (Utuh)", "-$4", "Sayuran mentah");
            DrawPenaltyItemBox(new Rect(tableRect.x + 8f + (colW + 4f) * 2f, ty, colW, rowH), ItemDatabase.GetTexture(ItemType.LettuceChopped), "Irisan Selada", "-$6", "Sayur siap saji");
            ty += rowH + 6f;

            // Baris 2: Tomat Segar, Irisan Tomat, Keju Slice
            DrawPenaltyItemBox(new Rect(tableRect.x + 8f, ty, colW, rowH), ItemDatabase.GetTexture(ItemType.Tomato), "Tomat Segar (Utuh)", "-$4", "Tomat mentah");
            DrawPenaltyItemBox(new Rect(tableRect.x + 8f + colW + 4f, ty, colW, rowH), ItemDatabase.GetTexture(ItemType.TomatoSliced), "Irisan Tomat", "-$6", "Tomat potong siap saji");
            DrawPenaltyItemBox(new Rect(tableRect.x + 8f + (colW + 4f) * 2f, ty, colW, rowH), ItemDatabase.GetTexture(ItemType.Cheese), "Keju Slice", "-$5", "Keju cheddar premium");
            ty += rowH + 6f;

            // Baris 3: Patty Mentah, Patty Matang, Patty Gosong
            DrawPenaltyItemBox(new Rect(tableRect.x + 8f, ty, colW, rowH), ItemDatabase.GetTexture(ItemType.PattyRaw), "Patty Mentah", "-$6", "Daging sapi mentah");
            DrawPenaltyItemBox(new Rect(tableRect.x + 8f + colW + 4f, ty, colW, rowH), ItemDatabase.GetTexture(ItemType.PattyCooked), "Patty Matang", "-$10", "Daging matang sempurna");
            DrawPenaltyItemBox(new Rect(tableRect.x + 8f + (colW + 4f) * 2f, ty, colW, rowH), ItemDatabase.GetTexture(ItemType.PattyBurnt), "Patty Gosong", "-$4", "Daging rusak/hangus");
            ty += rowH + 6f;

            // Baris 4: Piring Bersih & Akumulasi Burger di Piring
            float colW2 = (innerW - 20f) * 0.5f;
            DrawPenaltyItemBox(new Rect(tableRect.x + 8f, ty, colW2, rowH), ItemDatabase.GetTexture(ItemType.Plate), "Piring Bersih", "-$5", "Piring pecah/terbuang");
            DrawPenaltyItemBox(new Rect(tableRect.x + 12f + colW2, ty, colW2, rowH), texBurgerWrapped ?? ItemDatabase.GetTexture(ItemType.BunBottom), "Burger di Piring", "-$26 s/d -$35", "Akumulasi seluruh bahan di piring");

            y = tableRect.yMax + 10f;
            tipStyle.fontSize = 12;
            tipStyle.alignment = TextAnchor.MiddleLeft;
            tipStyle.normal.textColor = new Color(0.65f, 0.20f, 0.15f);
            GUI.color = Color.white;
            GUI.Label(new Rect(padding, y, innerW, 22f), 
                "<i>[PERHATIAN]: Membuang burger yang sudah berisi tumpukan bahan akan menjumlahkan denda setiap bahan di atasnya!</i>", tipStyle);

            curY += c2H + 14f;

            // CARD 3: SISTEM BINTANG & SYARAT KELULUSAN
            float c3H = 190f;
            Rect c3 = new Rect(0, curY, cw, c3H);
            GUI.color = new Color(0.93f, 0.95f, 0.98f, 0.96f);
            GUI.Box(c3, GUIContent.none, cardStyle);

            GUI.color = Color.white;
            modalDarkBoldStyle.fontSize = 17;
            modalDarkBoldStyle.normal.textColor = new Color(0.15f, 0.25f, 0.45f);
            GUI.Label(new Rect(padding, curY + 12f, innerW, 26f), "<b>SISTEM BINTANG & SYARAT KELULUSAN MISI:</b>", modalDarkBoldStyle);

            y = curY + 42f;
            modalDarkTextStyle.fontSize = 13;
            modalDarkTextStyle.normal.textColor = new Color(0.20f, 0.24f, 0.28f);
            GUI.Label(new Rect(padding, y, innerW, 22f), 
                "Pemain <b>WAJIB</b> mencapai minimal 100% Target Omset sebelum timer level habis untuk lulus level:", modalDarkTextStyle);
            y += 26f;

            GUI.Label(new Rect(padding, y, innerW, 22f), 
                "• <b>1 Bintang (100% Target Omset):</b> Standar kelulusan minimum restoran tercapai.", modalDarkTextStyle); y += 24f;
            GUI.Label(new Rect(padding, y, innerW, 22f), 
                "• <b>2 Bintang (125% Target Omset):</b> Performa sangat baik, pesanan cepat dan minim pemborosan.", modalDarkTextStyle); y += 24f;
            GUI.Label(new Rect(padding, y, innerW, 22f), 
                "• <b>3 Bintang (150% Target Omset):</b> Gelar Koki Legendaris Bintang Lima!", modalDarkTextStyle); y += 24f;
            GUI.Label(new Rect(padding, y, innerW, 22f), 
                "• <b>Level Gagal:</b> Jika waktu ronde habis tetapi omset masih di bawah Target Omset level.", modalDarkTextStyle);

            curY += c3H + 14f;

            // CARD 4: RINCIAN 3 LEVEL & RESEP LENGKAP
            float c4H = 740f;
            Rect c4 = new Rect(0, curY, cw, c4H);
            GUI.color = new Color(0.96f, 0.94f, 0.90f, 0.96f);
            GUI.Box(c4, GUIContent.none, cardStyle);

            GUI.color = Color.white;
            modalDarkBoldStyle.fontSize = 17;
            modalDarkBoldStyle.normal.textColor = new Color(0.35f, 0.22f, 0.10f);
            GUI.Label(new Rect(padding, curY + 12f, innerW, 26f), "<b>RINCIAN 3 TINGKAT LEVEL & RESEP LENGKAP:</b>", modalDarkBoldStyle);

            y = curY + 44f;

            // SUB-CARD LEVEL 1
            float subBoxH1 = 200f;
            Rect sub1 = new Rect(padding, y, innerW, subBoxH1);
            GUI.color = new Color(0.91f, 0.96f, 0.91f, 0.95f);
            GUI.Box(sub1, GUIContent.none, cardStyle);

            GUI.color = Color.white;
            modalDarkBoldStyle.fontSize = 15;
            modalDarkBoldStyle.normal.textColor = new Color(0.12f, 0.42f, 0.22f);
            GUI.Label(new Rect(sub1.x + 12f, sub1.y + 10f, sub1.width - 24f, 22f), "<b>LEVEL 1: MULA-MULA (MUDAH)</b>", modalDarkBoldStyle);

            modalDarkTextStyle.fontSize = 12;
            modalDarkTextStyle.normal.textColor = new Color(0.20f, 0.28f, 0.22f);
            GUI.Label(new Rect(sub1.x + 12f, sub1.y + 32f, sub1.width - 24f, 20f), 
                "Durasi: <b>120 Detik</b>   |   Target Omset: <b>$250</b>   |   Sabar Pelanggan: <b>40 Detik</b>   |   Maks Antrean: <b>2 Order</b>", modalDarkTextStyle);

            float ry = sub1.y + 56f;
            DrawRecipeDetailRow(sub1.x + 12f, ry, sub1.width - 24f, "Burger Klasik", "$30", "POPULER", "Roti Bawah + Daging Gurih Matang + Roti Atas", new Color(0.20f, 0.65f, 0.35f));
            ry += 66f;
            DrawRecipeDetailRow(sub1.x + 12f, ry, sub1.width - 24f, "Cheeseburger", "$40", "FAVORIT", "Roti Bawah + Daging Gurih Matang + Keju Cheddar + Roti Atas", new Color(0.90f, 0.60f, 0.15f));

            y = sub1.yMax + 14f;

            // SUB-CARD LEVEL 2
            float subBoxH2 = 210f;
            Rect sub2 = new Rect(padding, y, innerW, subBoxH2);
            GUI.color = new Color(0.98f, 0.94f, 0.88f, 0.95f);
            GUI.Box(sub2, GUIContent.none, cardStyle);

            GUI.color = Color.white;
            modalDarkBoldStyle.fontSize = 15;
            modalDarkBoldStyle.normal.textColor = new Color(0.68f, 0.38f, 0.10f);
            GUI.Label(new Rect(sub2.x + 12f, sub2.y + 10f, sub2.width - 24f, 22f), "<b>LEVEL 2: JAM SIBUK (MENENGAH)</b>", modalDarkBoldStyle);

            modalDarkTextStyle.fontSize = 12;
            modalDarkTextStyle.normal.textColor = new Color(0.35f, 0.26f, 0.15f);
            GUI.Label(new Rect(sub2.x + 12f, sub2.y + 32f, sub2.width - 24f, 20f), 
                "Durasi: <b>180 Detik</b>   |   Target Omset: <b>$400</b>   |   Sabar Pelanggan: <b>60 Detik</b>   |   Maks Antrean: <b>3 Order</b>", modalDarkTextStyle);

            ry = sub2.y + 56f;
            DrawRecipeDetailRow(sub2.x + 12f, ry, sub2.width - 24f, "Burger Sayur Segar", "$45", "VEGETARIAN", "Roti Bawah + Irisan Selada + Irisan Tomat + Keju + Roti Atas (Gunakan Meja Potong)", new Color(0.18f, 0.65f, 0.40f));
            ry += 66f;
            DrawRecipeDetailRow(sub2.x + 12f, ry, sub2.width - 24f, "Dobel Cheeseburger", "$65", "SUPER GURIH", "Roti Bawah + 2x Daging Matang + 2x Keju Cheddar + Roti Atas", new Color(0.88f, 0.45f, 0.15f));

            y = sub2.yMax + 14f;

            // SUB-CARD LEVEL 3
            float subBoxH3 = 210f;
            Rect sub3 = new Rect(padding, y, innerW, subBoxH3);
            GUI.color = new Color(0.98f, 0.90f, 0.90f, 0.95f);
            GUI.Box(sub3, GUIContent.none, cardStyle);

            GUI.color = Color.white;
            modalDarkBoldStyle.fontSize = 15;
            modalDarkBoldStyle.normal.textColor = new Color(0.75f, 0.20f, 0.20f);
            GUI.Label(new Rect(sub3.x + 12f, sub3.y + 10f, sub3.width - 24f, 22f), "<b>LEVEL 3: RESTORAN BINTANG LIMA (SULIT)</b>", modalDarkBoldStyle);

            modalDarkTextStyle.fontSize = 12;
            modalDarkTextStyle.normal.textColor = new Color(0.38f, 0.18f, 0.18f);
            GUI.Label(new Rect(sub3.x + 12f, sub3.y + 32f, sub3.width - 24f, 20f), 
                "Durasi: <b>210 Detik</b>   |   Target Omset: <b>$900</b>   |   Sabar Pelanggan: <b>75 Detik</b>   |   Maks Antrean: <b>4 Order</b>", modalDarkTextStyle);

            ry = sub3.y + 56f;
            DrawRecipeDetailRow(sub3.x + 12f, ry, sub3.width - 24f, "Burger Spesial Komplit", "$80", "DELUXE SUPREME", "Roti Bawah + Daging Matang + Keju + Irisan Selada + Irisan Tomat + Roti Atas", new Color(0.75f, 0.25f, 0.50f));
            ry += 66f;
            DrawRecipeDetailRow(sub3.x + 12f, ry, sub3.width - 24f, "Monster Meat Burger", "$95", "PORSI MONSTER", "Roti Bawah + 2x Daging Matang + Keju + Irisan Selada + Roti Atas", new Color(0.85f, 0.20f, 0.20f));

            modalDarkBoldStyle.normal.textColor = new Color(0.18f, 0.22f, 0.25f);
        }

        private void DrawPenaltyItemBox(Rect r, Texture2D icon, string name, string penalty, string note)
        {
            GUI.color = new Color(1f, 1f, 1f, 0.95f);
            GUI.Box(r, GUIContent.none, cardStyle);

            // 1. Food / Item Icon on the left
            float iconSz = 34f;
            Rect iconR = new Rect(r.x + 8f, r.y + (r.height - iconSz) * 0.5f, iconSz, iconSz);
            if (icon != null)
            {
                GUI.color = Color.white;
                GUI.DrawTexture(iconR, icon, ScaleMode.ScaleToFit);
            }

            // 2. Penalty Tag on the right with ui_stats_box_bg
            float badgeW = penalty.Length > 6 ? 116f : 76f;
            float badgeH = 32f;
            Rect badgeR = new Rect(r.xMax - badgeW - 8f, r.y + (r.height - badgeH) * 0.5f, badgeW, badgeH);

            if (texStatsBoxBg != null)
            {
                GUI.color = Color.white;
                GUI.DrawTexture(badgeR, texStatsBoxBg, ScaleMode.StretchToFill);
            }
            else
            {
                GUI.color = new Color(0.88f, 0.22f, 0.22f, 0.92f);
                GUI.Box(badgeR, GUIContent.none, cardStyle);
            }

            // Badge text: Clean bold dark red for penalty amount
            modalDarkBoldStyle.fontSize = penalty.Length > 6 ? 11 : 14;
            modalDarkBoldStyle.alignment = TextAnchor.MiddleCenter;
            modalDarkBoldStyle.normal.textColor = new Color(0.82f, 0.16f, 0.16f);
            GUI.color = Color.white;
            GUI.Label(badgeR, penalty, modalDarkBoldStyle);

            // 3. Middle: Name & Note (Between icon and badge)
            float textX = r.x + iconSz + 14f;
            float textW = badgeR.x - textX - 4f;

            modalDarkBoldStyle.fontSize = 12;
            modalDarkBoldStyle.alignment = TextAnchor.MiddleLeft;
            modalDarkBoldStyle.normal.textColor = new Color(0.18f, 0.20f, 0.22f);
            GUI.Label(new Rect(textX, r.y + 5f, textW, 18f), name, modalDarkBoldStyle);

            modalDarkTextStyle.fontSize = 10;
            modalDarkTextStyle.alignment = TextAnchor.MiddleLeft;
            modalDarkTextStyle.normal.textColor = new Color(0.48f, 0.52f, 0.56f);
            GUI.Label(new Rect(textX, r.y + 24f, textW, 16f), note, modalDarkTextStyle);

            modalDarkBoldStyle.normal.textColor = new Color(0.18f, 0.22f, 0.25f);
        }

        private void DrawRecipeDetailRow(float x, float y, float w, string name, string price, string tag, string ingredients, Color tagColor)
        {
            Rect r = new Rect(x, y, w, 58f);
            GUI.color = Color.white;
            GUI.Box(r, GUIContent.none, cardStyle);

            // Tag badge
            Rect tagR = new Rect(r.x + 10f, r.y + 8f, 110f, 20f);
            GUI.color = tagColor;
            GUI.Box(tagR, GUIContent.none, primaryButtonStyle ?? buttonStyle);
            modalDarkBoldStyle.fontSize = 10;
            modalDarkBoldStyle.alignment = TextAnchor.MiddleCenter;
            modalDarkBoldStyle.normal.textColor = Color.white;
            GUI.Label(tagR, tag, modalDarkBoldStyle);

            // Recipe Name
            modalDarkBoldStyle.fontSize = 14;
            modalDarkBoldStyle.alignment = TextAnchor.MiddleLeft;
            modalDarkBoldStyle.normal.textColor = new Color(0.15f, 0.18f, 0.22f);
            GUI.Label(new Rect(r.x + 130f, r.y + 6f, w - 210f, 22f), name, modalDarkBoldStyle);

            // Price pill
            Rect priceR = new Rect(r.xMax - 72f, r.y + 6f, 62f, 24f);
            GUI.color = new Color(0.18f, 0.72f, 0.45f);
            GUI.Box(priceR, GUIContent.none, primaryButtonStyle ?? buttonStyle);
            modalDarkBoldStyle.fontSize = 12;
            modalDarkBoldStyle.alignment = TextAnchor.MiddleCenter;
            modalDarkBoldStyle.normal.textColor = Color.white;
            GUI.Label(priceR, price, modalDarkBoldStyle);

            // Ingredients text
            modalDarkTextStyle.fontSize = 11;
            modalDarkTextStyle.alignment = TextAnchor.MiddleLeft;
            modalDarkTextStyle.normal.textColor = new Color(0.35f, 0.38f, 0.42f);
            GUI.Label(new Rect(r.x + 10f, r.y + 30f, w - 20f, 22f), $"<b>Bahan:</b> {ingredients}", modalDarkTextStyle);

            modalDarkBoldStyle.normal.textColor = new Color(0.18f, 0.22f, 0.25f);
        }
    }
}
