using static Raylib_cs.Raylib;
using H4NationalFocusGUI.components;
using H4NationalFocusGUI.services;
using System.Numerics;
using Raylib_cs;
using H4NationalFocusGUI.enums;

namespace H4NationalFocusGUI.functional
{
    public class Gui(List<Focus>? existingFocuses = null)
    {
        private Vector2 mouse = GetMousePosition();
        private readonly Vector2 screen = new(GetScreenWidth(), GetScreenHeight());
        private List<Focus> focuses = existingFocuses ?? [];
        private string idInput = "", iconInput = "", nameInput = "", descInput = "", countryNameInput = "", createCountryInput = "",
            costInput = "10", xInput = "0", yInput = "0";
        
        private string statusMessage = "";
        private float statusTimer = 2.5f;
        private bool showCreateFocusMenu;
        private bool showSetPrerequsites;
        private ActiveTextField activeField = ActiveTextField.None;
        private readonly Dictionary<string, Texture2D> loadedIcons = new();
        private Focus? pendingDeleteFocus;
        private readonly GuiLayout layout = new();
        private readonly GuiService guiService = new();
        private readonly FocusPrerequisitesService focusPrerequisites = new();
        private readonly FocusRendererService focusRenderer = new();
        private readonly LocalisationService localisation = new();
        private readonly FocusSaveService focusSave = new();
        private readonly FocusLoadService focusLoadService = new();

        public void Update()
        {
            mouse = GetMousePosition();

            guiService.Update();
            guiService.UpdateContentSize(focuses);
            
            if (!showCreateFocusMenu) return;

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
            var scrollX = guiService.ScrollX;
            var scrollY = guiService.ScrollY;

            focusRenderer.RenderFocuses(focuses, mouse, loadedIcons, ref pendingDeleteFocus, ref layout.PendingRect, scrollX, scrollY);
            
            DrawRectangle(0, 0, 320, 720, Color.DarkGray);
            
            guiService.DrawScrollbars();
            
            foreach (var focus in focuses.Where(focus => !string.IsNullOrEmpty(focus.IconId) && !loadedIcons.ContainsKey(focus.Id)))
            {
                try
                {
                    loadedIcons[focus.Id] = LoadTexture(focus.IconId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to Load {focus.IconId}: {ex.Message}");
                }
            }
            
            FocusRendererService.DrawField(layout.CreateFocusField, activeField == ActiveTextField.TypingCreateFocus, "Create Focus");

            guiService.ToggleOnClick(mouse, layout.CreateFocusField, ref showCreateFocusMenu);
            if (showCreateFocusMenu)
            {
                DrawCreateFocusMenu();
            }

            FocusRendererService.DrawField(layout.SaveYamlButton, activeField == ActiveTextField.TypingSaveYaml, "Save Yaml", Color.Lime);
            FocusRendererService.DrawField(layout.LoadFocusButton, activeField == ActiveTextField.TypingLoadFocus, "Load Focus", Color.Lime);

            // SAVE YAML
            if (IsMouseButtonPressed(MouseButton.Left) && CheckCollisionPointRec(mouse, layout.SaveYamlButton))
            {
                focusSave.SaveFocus(focuses);
                localisation.CreateLocalisation(focuses, countryNameInput);
                GoalsService.CreateGoalsGfx(focuses, idInput);

                guiService.Draw("Yaml Saved");
            }

            // LOAD Focus
            if (IsMouseButtonPressed(MouseButton.Left) && CheckCollisionPointRec(mouse, layout.LoadFocusButton))
            {
                var loadedFocuses = focusLoadService.ExplorerOpen(ref statusMessage, ref statusTimer);

                if (loadedFocuses != null)
                {
                    focuses.Clear(); // CLEAN THE LAST
                    focuses = loadedFocuses;
                    guiService.Draw("Focus Loaded");
                }
            }

            DrawDeleteFocus();
        }

        private void DrawDeleteFocus()
        {
            if (pendingDeleteFocus == null) return;
            
            Rectangle box = new((screen.X - screen.X * 0.9f) / 2, (screen.Y - screen.Y * 0.9f) / 2, screen.X * 0.9f, screen.Y * 0.9f);

            var result = focusRenderer.Show(box, mouse, "Delete focus ?");
            switch (result)
            {
                case true:
                    focuses.Remove(pendingDeleteFocus);
                    pendingDeleteFocus = null;
                    break;
                case false:
                    pendingDeleteFocus = null;
                    break;
            }
        }

        private void DrawCreatePrerequisitesMenu()
        {
            DrawRectangleRec(layout.CreateMenuPanelPrerequisites, Color.DarkGray);

            Vector2 prerequisitesStart = new(layout.CreateMenuPanelPrerequisites.X + 10, layout.CreateMenuPanelPrerequisites.Y + 40);

            focusPrerequisites.RenderPrerequisites(focuses, mouse, prerequisitesStart);
        }

        private void DrawCreateFocusMenu()
        {
            DrawRectangleRec(layout.CreateMenuPanel, Color.DarkGray);

            FocusRendererService.DrawField(layout.CreateIdField, activeField == ActiveTextField.TypingId, $"ID: {idInput}");
            FocusRendererService.DrawField(layout.CreateNameField, activeField == ActiveTextField.TypingName, $"Name: {nameInput}");
            FocusRendererService.DrawField(layout.CreateDescField, activeField == ActiveTextField.TypingDesc, $"Desc: {descInput}");
            FocusRendererService.DrawField(layout.CreateCostField, activeField == ActiveTextField.TypingCost, $"Cost: {costInput}");
            FocusRendererService.DrawField(layout.CreateXField, activeField == ActiveTextField.TypingX, $"X: {xInput}");
            FocusRendererService.DrawField(layout.CreateYField, activeField == ActiveTextField.TypingY, $"Y: {yInput}");
            FocusRendererService.DrawField(layout.CreateIconField, false, $"Icon: {Path.GetFileName(iconInput)}");

            FocusRendererService.DrawField(layout.CreateSaveButton, activeField == ActiveTextField.TypingSaveFocus, "Save", Color.Lime);

            if (IsMouseButtonPressed(MouseButton.Left) && CheckCollisionPointRec(mouse, layout.CreateIconField))
            {
                focusRenderer.ExplorerOpen(ref iconInput, loadedIcons, ref statusMessage, ref statusTimer);
            }

            if (IsMouseButtonPressed(MouseButton.Left) && CheckCollisionPointRec(mouse, layout.CreateSaveButton))
            {
                var id = idInput.Replace(" ", "_").ToLower();

                var iconFilePath = iconInput;

                var (success, message) = focusSave.TryAddFocus(
                    id,
                    nameInput,
                    descInput,
                    xInput,
                    yInput,
                    costInput,
                    iconFilePath,
                    focusPrerequisites.SelectedPrerequisites,
                    focuses
                );

                guiService.Draw(message);

                if (success)
                {   
                    showCreateFocusMenu = false;
                    iconInput = "";
                    focusPrerequisites.SelectedPrerequisites.Clear();
                }
            }

            // PREREQUISITES
            FocusRendererService.DrawField(layout.CreatePrereqFocusField, activeField == ActiveTextField.TypingCreateFocus, $"Set Prerequisites");

            guiService.ToggleOnClick(mouse, layout.CreatePrereqFocusField, ref showSetPrerequsites);
            if (showSetPrerequsites)
            {
                DrawCreatePrerequisitesMenu();
            }
        }
    }
}