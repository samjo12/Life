using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
//using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Class_teach
{
    public partial class Form1 : Form
    {
        int count = 0;
        Timer timer1 = new Timer();
        public static Random rand1 = new Random(1);
        // bool flag_life=false; //Флаг, где true- идет жизнь, false - ждем таймера
        public static Graphics GR = null;
        public static Bitmap BM = null;
        //public static List <Men> pop_base = new List<Men>();
        public static List<Family> fam_base = new List<Family>();
        public DataGridView DataGridView1 = new DataGridView();
        public static int panel_width = 100;
        public static bool flag_life = false;
        public static int cellSizeX = 100; // размер клетки в пикселях по оси Х
        public static int cellSizeY; // размер клетки в пикселях по оси Y (считаем автомат. как 9:16)       
        public static int numCellX; //Кол-во клеток по горизонтали
        public static int numCellY; //Кол-во клеток по вертикали
        public static int windowTop; //координаты формы Y
        public static int windowLeft;//координаты формы по X
        public static int REST_FOOD = 51; //51 из 255 (20%)возобновляемость ресурсов в отсутствии человека
        public static Pole pole; //поле клеток

        public Form1()
        {
            InitializeComponent();
            GR = Graphics.FromHwnd(Handle);

        }

        public void init_field()
        {
            //передаем размеры формы в пикселях и размер одной клетки по оси X
            windowTop = this.Top;
            windowLeft = this.Left;
            pole = new Pole(Size.Width, Size.Height); //создаем поле питательных клеток



        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        { Men newman;
            Cell temp;
            if (e.Button == MouseButtons.Right)
            {
                
                temp = pole.setXY(e.X, e.Y);
                if (temp == null) return; //ячейка занята - выходим
                else
                {
                    newman = new Men(); // создаем человека и приписываем его к ячейке
                    newman.setCell(temp);
                    //pop_base.Add(newman);
                }
            }
        }


        private void Timer1_Start(object sender, EventArgs e)
        {


        }

        private void Timer1_Stop(object sender, EventArgs e)
        {
            timer1.Stop();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            // Начинаем жить если ожидали таймера, если уже живем - то пропускаем тик.
            // if (flag_life is true) return; else flag_life = true;
            Do_Life();
            timer1.Start();
        }
        public void Do_Life()
        {
            // нужно перебрать все клетки с ресурсами и всех людей и сделать ими ход

            //обновим клетки




        }
        public class Pole
        {
            static int FormWidth;
            static int FormHeight;

            public static List<Cell> FreeCells=new List<Cell>();
            public static List<Cell> OccupyCells= new List<Cell>();
            public static Cell[] CellPole;
            public Pole(int sizeW, int sizeH)
            {// инициализация поля с кормом
                FormWidth = sizeW - panel_width;//получаем размер формы - 100 пикселей на меню
                FormHeight = sizeH;

                //cellSizeX = csizeX;

                //Посчитаем количество клеток на поле

                numCellX = (int)(FormWidth / cellSizeX);
                cellSizeY = cellSizeX;// (int)(9 * cellSizeX / 16);
                numCellY = (int)(FormHeight / cellSizeY);

                CellPole = new Cell[numCellY * numCellX];// Создадим массив для хранения состояния ячеек

                for (int j = 0; j < numCellY; j++)
                {
                    for (int i = 0; i < numCellX; i++)
                    {
                        int X = panel_width + i * cellSizeX; //100+i*cellSizeX
                        int Y = j * cellSizeY;
                        CellPole[j * numCellX + i] = new Cell(X,Y, cellSizeX, cellSizeY);  //создадим пустые клетки с кормом
                        FreeCells.Add(CellPole[j * numCellX + i]); //записываем в список свободных ячеек
                    }
                }
            }

            public void HerZnaet()
            {
                for (int j = 0; j < numCellY * numCellX; j++)
                { return; }
            }

            public Cell getFreeCellFamily(Family fam)
            {   // возвращаем свободную ячейку расположенную рядом с членами семьи
                // или если все заняты - возвращаем null
                
                return null;
            }

            public void setFreeCell(Cell beFree)
            {
                OccupyCells.Remove(beFree);
                FreeCells.Add(beFree);
            }

            public Cell getFreeCell()
            {  // отбираем все свобдные ячейки и случайно возвращаем любую из них,
                // или если все заняты - возвращаем null
                int c = rand1.Next(0, FreeCells.Count);
                Cell freecell = FreeCells.ElementAt(c);
                // сразу заносим ячейку в список занятыми ячейками
                // и исключаем из списка со свободными ячейками
                OccupyCells.Add(freecell);
                FreeCells.RemoveAt(c);
                return freecell;
            }
            public Cell getCell(int number)
            {
                return CellPole[number];
            }
            public int getCellFood(int number)
            {
                return CellPole[number].food;
            }
            public void setCellFood(int number, int Food)
            {
                CellPole[number].food = Food;
            }

            public Cell setXY(int X, int Y) //получаем координату точки на экране
            {   //определим в какую ячейку мы попали

                int i, j,stl=-1,str=-1;
                for (j = 0; j < numCellY; j++)
                {

                    if (CellPole[j * numCellX].y < Y && (CellPole[j * numCellX].y+cellSizeY) > Y) 
                    {   str = j; //нашли в j - номер строки
                        for (i = 0; i < numCellX; i++)
                        {
                            if (CellPole[j * numCellX + i].x < X && (CellPole[j * numCellX + i].x + cellSizeX) > X)
                            { //нашли в i - номер столбца
                                stl = i;
                                break; 
                            }
                        }
                        break; 
                    } 
                }
                if (str == -1 || stl == -1) 
                    return null;
                if (OccupyCells.Contains(CellPole[str * numCellX + stl])) return null; //ячейка занята
                else
                {
                    OccupyCells.Add(CellPole[str * numCellX + stl]);
                    FreeCells.Remove(CellPole[str * numCellX + stl]);
                    return CellPole[str * numCellX + stl]; // возвращаем ячейку с полученной координатой
                }
                    
            }

        }
        

        public class Cell
        {

            public int food { get; set; } //возобновляемый ресурс при отсутствии человека
            public int x { get; set; }
            public int y { get; set; }
            public int w { get; set; } //ширина
            public int h { get; set; } //высота
            public Men men { get; set; }

            public void placeMen(Men men1) //заселяем в клетку жителя
            { men = men1;  }
            public Cell(int X, int Y, int W, int H) //создается пустая ячейка с кормом
            {                
                x = X; y = Y; h = H; w = W;
                food = 255;
                men = null; //ссылка на человека, изначально ни на что не ссылается}
                PaintCell(); //перерисуем клетку

            }
            private void reDrawCell()//обновляем цвет ячейки в зависимости от food
            {   // зеленый -белый(много/мало еды)
                if (men == null)
                {
                    if (food >= 255) return;
                    else
                    {
                        food += REST_FOOD; if (food > 255) food = 255;
                        PaintCell();
                    }

                }
                else // в клетке есть житель
                {
                    food -= REST_FOOD; if (food < 0) food = 0; //еды становиться меньше
                    PaintCell(); //перерисуем клетку
                    men.paintMen(); //перерисуем жителя
                }

            }
            private void PaintCell()
            {
                SolidBrush myCellColor = new SolidBrush(Color.FromArgb(255 - food, 255, 255 - food));
                GR.FillRectangle(myCellColor, x, y, w, h);
                Pen fam_pen = new Pen(Color.Black);
                GR.DrawRectangle(fam_pen, x, y, w, h);
            }

        }

            public class Family
            {
                public int R; // цвета семьи
                public int B;
                public int G;
                List<Men> members = new List<Men>(); // список членов семьи

                public Family(Men Rod1, Men Rod2) //создаем новую семью
                {
                    //добавляем в семью пару
                    if (Rod1 != null) if (members.Contains(Rod1) == false) members.Add(Rod1);
                    if (Rod2 != null) if (members.Contains(Rod2) == false) members.Add(Rod2);
                    using (GR = Graphics.FromImage(BM))
                    {

                        Pen fam_pen = new Pen(Color.FromArgb(0, 0, 0), 1);
                        SolidBrush fam_brush = new SolidBrush(Color.Blue);
                        GR.FillEllipse(fam_brush, 300, 240, 10, 10);
                    }
                }

                public Family(Men Child) // добавляем в семью новорожденного
                {
                    members.Add(Child);
                }

                public void Dispose() // распад семьи
                {

                }

            }
            public class Men
            {
                bool sex;
                Family myFamily;
                int age; // возраст
                private Cell myCell; //ссылка на клетку своего местонаходжения
                private int R;  // цвета индивида с рождения
                private int B;
                private int G;
                private int force; //сила индивида
                private int wel; //здоровье индивида
                private int fert; //фертильность индивида


                public Cell getCell() { return myCell; }
                public void setCell(Cell mycell) { myCell = mycell; }
                public Family getFamily() { return myFamily; }
                public void setFamily(Family family) { myFamily = family; }
                public Men()
                {
                    this.sex = Convert.ToBoolean(rand1.Next(0, 1));
                    this.myFamily = null;
                    myCell=pole.getFreeCell();


                    age = 18;
                    R = rand1.Next(0, 255);
                    B = rand1.Next(0, 255);
                    G = rand1.Next(0, 255);
                    paintMen(); 
                }
                public void menBorn(Family family)
                {
                   age = 0;
                   R = family.R;
                   B = family.B;
                   G = family.G;
                 
                    
                }

                public void paintMen()
                {
                    
                    int center5=(int)(cellSizeX/20); //10%
                    int Radius=cellSizeX;
                    bool DrawOrFill; // Draw - false ; Fill-true
                    if (age < 10) { Radius = (int)cellSizeX / 8; DrawOrFill = false; }
                    else if (age < 18) { Radius = (int)cellSizeX / 2; DrawOrFill = false; }
                    else if (age < 45) { Radius = Radius - center5*2;  DrawOrFill = true; }
                    else { DrawOrFill = false; }


                    if (DrawOrFill == true)
                    {
                        SolidBrush fam_brush = new SolidBrush(Color.FromArgb(R, B, G));
                        GR.FillEllipse(fam_brush, myCell.x+center5, myCell.y+center5, Radius, Radius);
                        Pen fam_pen = new Pen(Color.Black);
                        GR.DrawEllipse(fam_pen, myCell.x + center5, myCell.y + center5, Radius, Radius);
                }
                    else {
                        Pen fam_pen = new Pen(Color.FromArgb(R, B, G));
                        GR.DrawEllipse(fam_pen, myCell.x+center5, myCell.y+center5, Radius, Radius);

                    }

                }

            } // end for class Men

            private void start_btn_Click(object sender, EventArgs e)
            {
                init_field();
                flag_life = true;
                timer1.Tick += new EventHandler(timer1_Tick); // Every time timer ticks, timer_Tick will be called
                timer1.Interval = 1000; // 1 сек
                timer1.Start();

                Men newman = new Men(); // появился какой-то совершеннолетний тип случайного пола без фамилии
                //pop_base.Add(newman);
            }
        }
    }

// Закрашиваем фигуры
/*
SolidBrush myCorp = new SolidBrush(Color.DarkMagenta);
SolidBrush myTrum = new SolidBrush(Color.DarkOrchid);
SolidBrush myTrub = new SolidBrush(Color.DeepPink);
SolidBrush mySeа = new SolidBrush(Color.Blue);
 g.FillRectangle(myTrub, 300, 125, 75, 75); // 1 труба (прямоугольник)
 g.FillRectangle(myTrub, 480, 125, 75, 75); // 2 труба (прямоугольник)
 g.FillPolygon(myCorp, new Point[]      // корпус (трапеция)
   {
     new Point(100,300),new Point(700,300),
     new Point(700,300),new Point(600,400),
     new Point(600,400),new Point(200,400),
     new Point(200,400),new Point(100,300)
   }
 );
 g.FillRectangle(myTrum, 250, 200, 350, 100); // палуба (прямоугольник)
                                              // Море - 12 секторов-полуокружностей

 int x = 50;
 int Radius = 50;
 while (x <= pictureBox1.Width - Radius)
 {
     g.FillPie(mySeа, 0 + x, 375, 50, 50, 0, -180);
     x += 50;
 }*/
// Иллюминаторы 