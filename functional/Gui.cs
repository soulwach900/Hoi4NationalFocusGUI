using Rectangle = Raylib_cs.Rectangle;
using H4NationalFocusGUI.components;
using static Raylib_cs.Raylib;
using H4NationalFocusGUI.services;
using System.Numerics;
using Raylib_cs;

namespace H4NationalFocusGUI.functional
{
    public class Gui
    {
        private List<Focus> focuses = new();
        private string idInput, iconInput, nameInput, descInput;
        private string costInput, xInput, yInput;
        private bool typingId, typingName, typingDesc, typingCountry;
        private bool typingCost, typingX, typingY;
        private string countryNameInput;
        private string statusMessage;
        private float statusTimer;
        private Rectangle countryNameField;
        private Rectangle idField;
        private Rectangle nameField;
        private Rectangle descField;
        private Rectangle costField;
        private Rectangle xField;
        private Rectangle yField;
        private Rectangle iconField;
        private Rectangle saveButton;
        private Rectangle saveYamlButton;
        private Dictionary<string, Texture2D> loadedIcons;
        private Focus? pendingDeleteFocus = null;
        private Rectangle pendingRect = new();
        private FocusPrerequisitesService focusPrerequisites;
        private FocusRendererService focusRenderer;
        private LocalisationService localisation;
        private GoalsService goalsService;
        private TexconvWrapper texconvWrapper = new TexconvWrapper();


        public Gui(List<Focus>? existingFocuses = null)
        {
            focuses = existingFocuses ?? new List<Focus>();

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

            typingCountry = false;
            typingId = false;
            typingName = false;
            typingDesc = false;
            typingCost = false;
            typingX = false;
            typingY = false;

            countryNameField = new Rectangle(20, 10, 280, 30);
            idField = new Rectangle(20, 50, 280, 30);
            nameField = new Rectangle(20, 100, 280, 30);
            descField = new Rectangle(20, 150, 280, 30);
            costField = new Rectangle(20, 200, 100, 30);
            xField = new Rectangle(20, 250, 100, 30);
            yField = new Rectangle(20, 300, 100, 30);
            iconField = new Rectangle(20, 350, 280, 30);
            saveButton = new Rectangle(20, 400, 100, 30);
            saveYamlButton = new Rectangle(140, 400, 150, 30);

            loadedIcons = new Dictionary<string, Texture2D>();

            focusPrerequisites = new FocusPrerequisitesService();
            focusRenderer = new FocusRendererService();
            localisation = new LocalisationService();
            goalsService = new GoalsService();
            texconvWrapper = new TexconvWrapper();
        }

        public void Update()
        {
            Vector2 mouse = GetMousePosition();

            if (CheckCollisionPointRec(mouse, countryNameField))
                typingCountry = true;
            else if (CheckCollisionPointRec(mouse, idField))
                typingId = true;
            else if (CheckCollisionPointRec(mouse, nameField))
                typingName = true;
            else if (CheckCollisionPointRec(mouse, descField))
                typingDesc = true;
            else if (CheckCollisionPointRec(mouse, costField))
                typingCost = true;
            else if (CheckCollisionPointRec(mouse, xField))
                typingX = true;
            else if (CheckCollisionPointRec(mouse, yField))
                typingY = true;
            else
            {
                typingCountry = typingId = typingName = typingDesc = typingCost = typingX = typingY = false;
            }

            CaptureTextInput();
        }

        private void CaptureTextInput()
        {
            // CAPTURE INPUT
            if (typingCountry) focusRenderer.DrawTextBox(ref countryNameInput, ref typingCountry, countryNameField);
            if (typingId) focusRenderer.DrawTextBox(ref idInput, ref typingId, idField);
            if (typingName) focusRenderer.DrawTextBox(ref nameInput, ref typingName, nameField);
            if (typingDesc) focusRenderer.DrawTextBox(ref descInput, ref typingDesc, descField);
            if (typingCost) focusRenderer.DrawTextBox(ref costInput, ref typingCost, costField);
            if (typingX) focusRenderer.DrawTextBox(ref xInput, ref typingX, xField);
            if (typingY) focusRenderer.DrawTextBox(ref yInput, ref typingY, yField);
        }

        public void Render()
        {
            Vector2 mouse = GetMousePosition();

            FocusSaveService focusSave = new FocusSaveService();

            DrawRectangle(0, 0, 320, 720, Raylib_cs.Color.DarkGray);

            // RENDER FIELDS
            focusRenderer.DrawField(countryNameField, typingCountry, $"Country: {countryNameInput}");
            focusRenderer.DrawField(idField, typingId, $"ID: {idInput}");
            focusRenderer.DrawField(nameField, typingName, $"Name: {nameInput}");
            focusRenderer.DrawField(descField, typingDesc, $"Desc: {descInput}");
            focusRenderer.DrawField(costField, typingCost, $"Cost: {costInput}");
            focusRenderer.DrawField(xField, typingX, $"X: {xInput}");
            focusRenderer.DrawField(yField, typingY, $"Y: {yInput}");
            focusRenderer.DrawField(iconField, false, $"Icon: {Path.GetFileName(iconInput)}");

            if (IsMouseButtonPressed(MouseButton.Left) && CheckCollisionPointRec(GetMousePosition(), iconField))
            {
                focusRenderer.WindowsExplorerOpen(mouse, iconField, ref iconInput, loadedIcons, ref statusMessage, ref statusTimer);
                TexconvWrapper.ConvertPngToDds("thirdparty/texconv/texconv.exe", iconInput, "mod/gfx/interface/goals", idInput.Replace(" ", "_").ToLower());
            }

            DrawRectangleRec(saveButton, Raylib_cs.Color.Green);
            DrawText("Save", 30, 405, 20, Raylib_cs.Color.Black);

            if (IsMouseButtonPressed(MouseButton.Left) && CheckCollisionPointRec(mouse, saveButton))
            {
                string id = idInput.Replace(" ", "_").ToLower();

                iconInput = $"mod/gfx/interface/goals/{id}.dds";
                string iconName = Path.GetFileNameWithoutExtension(iconInput);
                string iconReference = $"GFX_goal_{iconName}";

                focusSave.TryAddFocus(id, nameInput, descInput, xInput, yInput, costInput, iconReference, focusPrerequisites.selectedPrerequisites, focuses);

                statusMessage = "Focus saved";
                statusTimer = 2.5f;
            }



            DrawRectangleRec(saveYamlButton, Raylib_cs.Color.DarkGreen);
            DrawText("Save Yaml", 145, 405, 20, Raylib_cs.Color.Black);

            if (IsMouseButtonPressed(MouseButton.Left) && CheckCollisionPointRec(GetMousePosition(), saveYamlButton))
            {
                focusSave.SaveFocusYaml(focuses);
                localisation.CreateLocalisation(focuses, countryNameInput);
                goalsService.CreateGoalsGfx(focuses, countryNameInput);

                statusMessage = "Yaml saved";
            }


            // PREREQUISITES
            focusPrerequisites.RenderPrerequisites(focuses, mouse);


            if (!string.IsNullOrEmpty(statusMessage) && statusTimer > 0)
                DrawText(statusMessage, 320 + 20, 20, 20, Raylib_cs.Color.DarkGreen);

            focusRenderer.RenderFocuses(focuses, mouse, loadedIcons, ref pendingDeleteFocus, ref pendingRect);

            if (pendingDeleteFocus != null)
            {
                var screen = new Vector2(GetScreenWidth(), GetScreenHeight());
                Rectangle box = new((screen.X - screen.X * 0.9f) / 2, (screen.Y - screen.Y * 0.9f) / 2, screen.X * 0.9f, screen.Y * 0.9f);

                var result = focusRenderer.Show(box, mouse, "Delete focus?");
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
    }
}