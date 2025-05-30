using Rectangle = Raylib_cs.Rectangle;
using H4NationalFocusGUI.components;
using static Raylib_cs.Raylib;
using H4NationalFocusGUI.services;
using System.Numerics;
using Raylib_cs;
using H4NationalFocusGUI.enums;

namespace H4NationalFocusGUI.functional
{
    public class Gui
    {
        Vector2 mouse;
        Vector2 screen;
        private List<Focus> focuses = new();
        private string idInput, iconInput, nameInput, descInput, countryNameInput, createCountryInput;
        private string costInput, xInput, yInput;
        private bool typingId, typingName, typingDesc, typingCreateFocus;
        private bool typingCost, typingX, typingY;
        private string statusMessage;
        private float statusTimer;
        private bool showCreateFocusMenu = false;
        private bool showSetPrerequsites = false;
        private ActiveTextField activeField = ActiveTextField.None;
        private Dictionary<string, Texture2D> loadedIcons;
        private Focus? pendingDeleteFocus = null;
        private GuiLayout layout;
        private GuiService guiService;
        private FocusPrerequisitesService focusPrerequisites;
        private FocusRendererService focusRenderer;
        private LocalisationService localisation;
        private GoalsService goalsService;
        private TexconvWrapper texconvWrapper;
        private FocusSaveService focusSave;

        public Gui(List<Focus>? existingFocuses = null)
        {
            mouse = GetMousePosition();
            screen = new Vector2(GetScreenWidth(), GetScreenHeight());
            focuses = existingFocuses ?? new List<Focus>();

            createCountryInput = "";
            countryNameInput = "";
            idInput = "";
            iconInput = "";
            nameInput = "";
            descInput = "";
            costInput = "10";
            xInput = "0";
            yInput = "0";

            statusMessage = "";
            statusTimer = 2.5f;

            typingCreateFocus = false;
            typingName = false;
            typingDesc = false;
            typingCost = false;
            typingX = false;
            typingY = false;

            loadedIcons = new Dictionary<string, Texture2D>();

            layout = new GuiLayout();
            guiService = new GuiService();
            focusPrerequisites = new FocusPrerequisitesService();
            focusRenderer = new FocusRendererService();
            localisation = new LocalisationService();
            goalsService = new GoalsService();
            texconvWrapper = new TexconvWrapper();
            focusSave = new FocusSaveService();
        }

        public void Update()
        {
            mouse = GetMousePosition();

            if (showCreateFocusMenu)
            {
                if (IsMouseButtonPressed(MouseButton.Left))
                {
                    if (CheckCollisionPointRec(mouse, layout.CreateIdField))
                        activeField = ActiveTextField.Id;
                    else if (CheckCollisionPointRec(mouse, layout.CreateNameField))
                        activeField = ActiveTextField.Name;
                    else if (CheckCollisionPointRec(mouse, layout.CreateDescField))
                        activeField = ActiveTextField.Desc;
                    else if (CheckCollisionPointRec(mouse, layout.CreateCostField))
                        activeField = ActiveTextField.Cost;
                    else if (CheckCollisionPointRec(mouse, layout.CreateXField))
                        activeField = ActiveTextField.X;
                    else if (CheckCollisionPointRec(mouse, layout.CreateYField))
                        activeField = ActiveTextField.Y;
                    else if (CheckCollisionPointRec(mouse, layout.CreateIconField))
                        activeField = ActiveTextField.Icon;
                    else
                        activeField = ActiveTextField.None;
                }

                CaptureTextInput();
            }
        }


        private void CaptureTextInput()
        {
            switch (activeField)
            {
                case ActiveTextField.CreateFocus:
                    focusRenderer.DrawTextBox(ref createCountryInput, ref activeField, layout.CreateFocusField, ActiveTextField.CreateFocus);
                    break;
                case ActiveTextField.Country:
                    focusRenderer.DrawTextBox(ref countryNameInput, ref activeField, layout.CountryNameField, ActiveTextField.Country);
                    break;
                case ActiveTextField.Id:
                    focusRenderer.DrawTextBox(ref idInput, ref activeField, layout.IdField, ActiveTextField.Id);
                    break;
                case ActiveTextField.Name:
                    focusRenderer.DrawTextBox(ref nameInput, ref activeField, layout.NameField, ActiveTextField.Name);
                    break;
                case ActiveTextField.Desc:
                    focusRenderer.DrawTextBox(ref descInput, ref activeField, layout.DescField, ActiveTextField.Desc);
                    break;
                case ActiveTextField.Cost:
                    focusRenderer.DrawTextBox(ref costInput, ref activeField, layout.CostField, ActiveTextField.Cost);
                    break;
                case ActiveTextField.X:
                    focusRenderer.DrawTextBox(ref xInput, ref activeField, layout.XField, ActiveTextField.X);
                    break;
                case ActiveTextField.Y:
                    focusRenderer.DrawTextBox(ref yInput, ref activeField, layout.YField, ActiveTextField.Y);
                    break;
            }
        }

        public void Render()
        {
            Vector2 mouse = GetMousePosition();

            DrawRectangle(0, 0, 320, 720, Raylib_cs.Color.DarkGray);

            // RENDER FOCUSES FIRST
            focusRenderer.RenderFocuses(focuses, mouse, loadedIcons, ref pendingDeleteFocus, ref layout.PendingRect);

            focusRenderer.DrawField(layout.CreateFocusField, typingCreateFocus, $"Create Focus");

            if (IsMouseButtonPressed(MouseButton.Left) && CheckCollisionPointRec(mouse, layout.CreateFocusField))
            {
                showCreateFocusMenu = !showCreateFocusMenu;
            }

            if (showCreateFocusMenu)
            {
                DrawCreateFocusMenu(mouse);
            }

            DrawRectangleRec(layout.SaveYamlButton, Raylib_cs.Color.DarkGreen);
            DrawText("Save Yaml", 145, 405, 20, Raylib_cs.Color.Black);

            if (IsMouseButtonPressed(MouseButton.Left) && CheckCollisionPointRec(GetMousePosition(), layout.SaveYamlButton))
            {
                focusSave.SaveFocusYaml(focuses);
                localisation.CreateLocalisation(focuses, countryNameInput);
                goalsService.CreateGoalsGfx(focuses, countryNameInput);

                statusMessage = "Yaml saved";
                statusTimer = 2.5f;
            }

            if (!string.IsNullOrEmpty(statusMessage) && statusTimer > 0)
            {
                int fontSize = 20;
                int margem = 10;
                int textWidth = MeasureText(statusMessage, fontSize);
                int textHeight = fontSize;

                int posX = (int)(screen.X - textWidth - margem);
                int posY = (int)(screen.Y - textHeight - margem);

                DrawText(statusMessage, posX, posY, fontSize, Raylib_cs.Color.DarkGreen);
            }

            if (pendingDeleteFocus != null)
            {
                Rectangle box = new((screen.X - screen.X * 0.9f) / 2, (screen.Y - screen.Y * 0.9f) / 2, screen.X * 0.9f, screen.Y * 0.9f);

                var result = focusRenderer.Show(box, mouse, "Delete focus ?");
                if (result == true)
                {
                    focuses.Remove(pendingDeleteFocus);
                    pendingDeleteFocus = null;
                }
                else if (result == false)
                {
                    pendingDeleteFocus = null;
                }
            }
        }

        private void DrawCreatePrerequisitesMenu(Vector2 mouse)
        {
            DrawRectangleRec(layout.CreateMenuPanelPrerequisites, Raylib_cs.Color.DarkGray);

            Vector2 prerequisitesStart = new(layout.CreateMenuPanelPrerequisites.X + 10, layout.CreateMenuPanelPrerequisites.Y + 40);

            focusPrerequisites.RenderPrerequisites(focuses, mouse, prerequisitesStart);
        }

        private void DrawCreateFocusMenu(Vector2 mouse)
        {
            DrawRectangleRec(layout.CreateMenuPanel, Raylib_cs.Color.DarkGray);

            focusRenderer.DrawField(layout.CreateIdField, typingId, $"ID: {idInput}");
            focusRenderer.DrawField(layout.CreateNameField, typingName, $"Name: {nameInput}");
            focusRenderer.DrawField(layout.CreateDescField, typingDesc, $"Desc: {descInput}");
            focusRenderer.DrawField(layout.CreateCostField, typingCost, $"Cost: {costInput}");
            focusRenderer.DrawField(layout.CreateXField, typingX, $"X: {xInput}");
            focusRenderer.DrawField(layout.CreateYField, typingY, $"Y: {yInput}");
            focusRenderer.DrawField(layout.CreateIconField, false, $"Icon: {Path.GetFileName(iconInput)}");

            DrawRectangleRec(layout.CreateSaveButton, Raylib_cs.Color.DarkGreen);
            DrawText("Save", (int)layout.CreateSaveButton.X + 10, (int)layout.CreateSaveButton.Y + 5, 20, Raylib_cs.Color.White);

            if (IsMouseButtonPressed(MouseButton.Left) && CheckCollisionPointRec(mouse, layout.CreateIconField))
            {
                focusRenderer.WindowsExplorerOpen(mouse, layout.CreateIconField, ref iconInput, loadedIcons, ref statusMessage, ref statusTimer);
                TexconvWrapper.ConvertPngToDds("thirdparty/texconv/texconv.exe", iconInput, "mod/gfx/interface/goals", idInput.Replace(" ", "_").ToLower());
            }

            if (loadedIcons.TryGetValue(iconInput, out Texture2D previewIcon))
            {
                DrawTextureEx(previewIcon, new Vector2(layout.CreateIconField.X + 160, layout.CreateIconField.Y + 35), 0, 1.5f, Raylib_cs.Color.White);
            }

            if (IsMouseButtonPressed(MouseButton.Left) && CheckCollisionPointRec(mouse, layout.CreateSaveButton))
            {
                string id = idInput.Replace(" ", "_").ToLower();
                iconInput = $"mod/gfx/interface/goals/{id}.dds";
                string iconName = Path.GetFileNameWithoutExtension(iconInput);
                string iconReference = $"GFX_goal_{iconName}";

                var (success, message) = focusSave.TryAddFocus(id, nameInput, descInput, xInput, yInput, costInput, iconReference,
                 focusPrerequisites.selectedPrerequisites, focuses);

                statusMessage = message;
                statusTimer = 2.5f;

                if (success)
                {
                    showCreateFocusMenu = false;
                }
            }

            // PREREQUISITES
            focusRenderer.DrawField(layout.CreatePrereqFocusField, typingCreateFocus, $"Set Prerequisites");

            if (IsMouseButtonPressed(MouseButton.Left) && CheckCollisionPointRec(mouse, layout.CreatePrereqFocusField))
            {
                showSetPrerequsites = !showSetPrerequsites;
            }

            if (showSetPrerequsites)
            {
                DrawCreatePrerequisitesMenu(mouse);
            }
        }
    }
}