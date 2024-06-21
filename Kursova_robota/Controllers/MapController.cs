using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kursova_robota.Controllers
{
    public static class MapController
    {
        public const int SizeOfMap = 10;
        public const int SizeOfCell = 50;

        private static int currentPictureToSet = 0;

        public static int[,] map = new int[SizeOfMap, SizeOfMap];

        public static Button[,] buttons = new Button[SizeOfMap, SizeOfMap];

        public static Image SetSprite;

        private static bool isFirstClick;

        private static Point FirstCoordinate;

        public static Form Form;

        private static void ConfigureMapSize(Form current)
        {
            current.Width = SizeOfMap * SizeOfCell + 20;
            current.Height = (SizeOfMap + 1) * SizeOfCell;
        }

        private static void InitializationMap()
        {
            for (int i = 0; i < SizeOfMap; i++)
            {
                for (int j = 0; j < SizeOfMap; j++)
                {
                    map[i, j] = 0;
                }
            }
        }

        public static void Initialization(Form current)
        {
            Form = current;
            currentPictureToSet = 0;
            isFirstClick = true;
            SetSprite = new Bitmap(Path.Combine(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.FullName.ToString(), "Sprites\\tiles.png"));
            ConfigureMapSize(current);
            InitializationMap();
            InitializationButtons(current);
        }

        private static void InitializationButtons(Form current)
        {
            for (int i = 0; i < SizeOfMap; i++)
            {
                for (int j = 0; j < SizeOfMap; j++)
                {
                    Button button = new Button();
                    button.Location = new Point(j * SizeOfCell, i * SizeOfCell);
                    button.Size = new Size(SizeOfCell, SizeOfCell);
                    button.Image = FindImage(0, 0);
                    button.MouseUp += new MouseEventHandler(ButtonPressedMouse);
                    current.Controls.Add(button);
                    buttons[i, j] = button;
                }
            }
        }

        private static void ButtonPressedMouse(object sender, MouseEventArgs e)
        {
            Button pressedButton = sender as Button;
            switch (e.Button.ToString())
            {
                case "Right":
                    RightButtonPressed(pressedButton);
                    break;
                case "Left":
                    LeftButtonPressed(pressedButton);
                    break;
            }
        }

        private static void RightButtonPressed(Button pressedButton)
        {
            currentPictureToSet++;
            currentPictureToSet %= 3;
            int posX = 0;
            int posY = 0;
            switch (currentPictureToSet)
            {
                case 0:
                    posX = 0;
                    posY = 0;
                    break;
                case 1:
                    posX = 0;
                    posY = 2;
                    break;
                case 2:
                    posX = 2;
                    posY = 2;
                    break;
            }
            pressedButton.Image = FindImage(posX, posY);
        }

        private static void LeftButtonPressed(Button pressedButton)
        {
            pressedButton.Enabled = false;
            int iButton = pressedButton.Location.Y / SizeOfCell;
            int jButton = pressedButton.Location.X / SizeOfCell;
            if (isFirstClick)
            {
                FirstCoordinate = new Point(jButton, iButton);
                SeedMap();
                CountCellBomb();
                isFirstClick = false;
            }
            OpenCells(iButton, jButton);

            if (map[iButton, jButton] == -1)
            {
                ShowAllBombs(iButton, jButton);
                MessageBox.Show("Поразка. Спробуйте ще!");
                Form.Controls.Clear();
                Initialization(Form);
            }
        }

        private static void ShowAllBombs(int iBomb, int jBomb)
        {
            for (int i = 0; i < SizeOfMap; i++)
            {
                for (int j = 0; j < SizeOfMap; j++)
                {
                    if (i == iBomb && j == jBomb)
                        continue;
                    if (map[i, j] == -1)
                    {
                        buttons[i, j].Image = FindImage(3, 2);
                    }
                }
            }
        }

        public static Image FindImage(int xPos, int yPos)
        {
            Image image = new Bitmap(SizeOfCell, SizeOfCell);
            Graphics g = Graphics.FromImage(image);
            g.DrawImage(SetSprite, new Rectangle(new Point(0, 0), new Size(SizeOfCell, SizeOfCell)), 0 + 32 * xPos, 0 + 32 * yPos, 33, 33, GraphicsUnit.Pixel);


            return image;
        }

        private static void SeedMap()
        {
            Random r = new Random();
            int number = r.Next(7, 15);

            for (int i = 0; i < number; i++)
            {
                int posI = r.Next(0, SizeOfMap - 1);
                int posJ = r.Next(0, SizeOfMap - 1);

                while (map[posI, posJ] == -1 || (Math.Abs(posI - FirstCoordinate.Y) <= 1 && Math.Abs(posJ - FirstCoordinate.X) <= 1))
                {
                    posI = r.Next(0, SizeOfMap - 1);
                    posJ = r.Next(0, SizeOfMap - 1);
                }
                map[posI, posJ] = -1;
            }
        }

        private static void CountCellBomb()
        {
            for (int i = 0; i < SizeOfMap; i++)
            {
                for (int j = 0; j < SizeOfMap; j++)
                {
                    if (map[i, j] == -1)
                    {
                        for (int k = i - 1; k < i + 2; k++)
                        {
                            for (int l = j - 1; l < j + 2; l++)
                            {
                                if (!IsInBorder(k, l) || map[k, l] == -1)
                                    continue;
                                map[k, l] = map[k, l] + 1;
                            }
                        }
                    }
                }
            }
        }

        private static void OpenCell(int i, int j)
        {
            buttons[i, j].Enabled = false;

            switch (map[i, j])
            {
                case 1:
                    buttons[i, j].Image = FindImage(1, 0);
                    break;
                case 2:
                    buttons[i, j].Image = FindImage(2, 0);
                    break;
                case 3:
                    buttons[i, j].Image = FindImage(3, 0);
                    break;
                case 4:
                    buttons[i, j].Image = FindImage(4, 0);
                    break;
                case 5:
                    buttons[i, j].Image = FindImage(0, 1);
                    break;
                case 6:
                    buttons[i, j].Image = FindImage(1, 1);
                    break;
                case 7:
                    buttons[i, j].Image = FindImage(2, 1);
                    break;
                case 8:
                    buttons[i, j].Image = FindImage(3, 1);
                    break;
                case -1:
                    buttons[i, j].Image = FindImage(1, 2);
                    break;
                case 0:
                    buttons[i, j].Image = FindImage(0, 0);
                    break;
            }
        }

        private static void OpenCells(int i, int j)
        {
            OpenCell(i, j);

            if (map[i, j] > 0)
                return;

            for (int k = i - 1; k < i + 2; k++)
            {
                for (int l = j - 1; l < j + 2; l++)
                {
                    if (!IsInBorder(k, l))
                        continue;
                    if (!buttons[k, l].Enabled)
                        continue;
                    if (map[k, l] == 0)
                        OpenCells(k, l);
                    else if (map[k, l] > 0)
                        OpenCell(k, l);
                }
            }
        }

        private static bool IsInBorder(int i, int j)
        {
            if (i < 0 || j < 0 || j > SizeOfMap - 1 || i > SizeOfMap - 1)
            {
                return false;
            }
            return true;
        }
    }
}