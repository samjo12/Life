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
        int count=0;            
        Timer timer1 = new Timer();
        public static Random rand1 = new Random(1);
       // bool flag_life=false; //Флаг, где true- идет жизнь, false - ждем таймера
        public static Graphics GR=null;
        public static Bitmap BM=null;
        List<Men> pop_base= new List<Men>();
        List<Family> fam_base = new List <Family>();
        public DataGridView DataGridView1 = new DataGridView();
        public float FormWidth; //размер формы
        public float FormHeight;
        bool flag_life = false;
        public int cellSizeX = 10; // размер клетки в пикселях по оси Х
        public int cellSizeY; // размер клетки в пикселях по оси Y (считаем автомат. как 9:16)       
        public int numCellX; //Кол-во клеток по горизонтали
        public int numCellY; //Кол-во клеток по вертикали
        public static int restFood=25; //% возобновляемость ресурсов в отсутствии человека
        public Cell[] CellPole;

        public Form1()
        {
            InitializeComponent();
            GR = Graphics.FromHwnd(Handle);

        }

        public void init_field()
        {
            FormWidth = Size.Width-100;//получаем размер формы - 100 пикселей на меню
            FormHeight = Size.Height;
            //Посчитаем количество клеток на поле

            numCellX = (int)(FormWidth / cellSizeX);
            cellSizeY= (int)(9 * cellSizeX / 16);
            numCellY = (int)(FormHeight / cellSizeY);

            CellPole = new Cell[numCellY * numCellX];
            // Создадим массив для хранения состояния ячеек
           
            for(int j=0; j < numCellY; j++)
            {
                for(int i=0;i<numCellX; i++)
                {
                    CellPole[j * numCellX + i] = new Cell(null); // инициализация поля с кормом
                    CellPole[j * numCellX + i].x = 100 + i*cellSizeX;
                    CellPole[j * numCellX + i].y = j * cellSizeY;
                    CellPole[j * numCellX + i].h = cellSizeY;
                    CellPole[j * numCellX + i].w = cellSizeX;
                }
            }
            //GR.Clear(Color.FromArgb(0,255,0)); 
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Men newman =new Men(null);
                newman.setXY(this.Location.X, this.Location.Y);
                pop_base.Add(newman);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {

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

                for (int j = 0; j < numCellY*numCellX; j++)
                {

                        CellPole[j].ReDraw(); // Обновим клетку с кормом

                }

            
        }

        public class Cell
        {
            public int food; //возобновляемый ресурс при отсутствии человека
            public int x;
            public int y;
            public int w;
            public int h;
            public Men men=null; //ссылка на человека, изначально ни на что не ссылается
            public Cell(Men men1)
            {
                men = men1;
                food = 255; //изначальна клетка создается с кормом
            }
            public void ReDraw() //обновляем цвет ячейки в зависимости от food
            {   // зеленый -белый(много/мало еды)
                if (men == null && food < 255) // нет человека - запас еды восстанавливается
                {
                    food += restFood; if (food > 255) food = 255;
                }
                else 
                {
                    food -= restFood; if (food < 0) food = 0;
                }

                SolidBrush myCellColor = new SolidBrush(Color.FromArgb(255 - food, 255, 255 - food));
                GR.FillRectangle(myCellColor, x, y, w, h);

            }
        }

        public class Family
        {
            public int R; // цвета семьи
            public int B;
            public int G;
            List<Men> members = new List<Men>(); // список членов семьи

            public Family(Men Rod1,Men Rod2) //создаем новую семью
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
            Family my_family;
            int age; // возраст
            private int x=300;   // координата X
            private int y=240;  // координата Y
            int R;  // цвета индивида с рождения
            int B;
            int G;
            int force; //сила индивида
            int wel; //здоровье индивида
            int fert; //фертильность индивида
            int Radius; //радиус

            public void setXY(int xx, int yy)
            {
                x = xx; y = yy;
            }
            public int getX(){ return x; }
            public int getY() { return y; }
            public Men(Family new_fam)
            {
                this.sex =  Convert.ToBoolean(rand1.Next(0, 1));
                this.my_family = new_fam;
                if (new_fam is null) { 
                 this.age = 18; 
                    R = rand1.Next(0, 255);
                    B = rand1.Next(0, 255);
                    G = rand1.Next(0, 255);
                } else { 
                    this.age = 0;
                    R = new_fam.R;
                    B = new_fam.B;
                    G = new_fam.G;
                }
                if (age >= 10) Radius = 10; else Radius = 5;

                    //Graphics g = pictureBox1.CreateGraphics();
                    // Создаем объекты-кисти для закрашивания фигур
                    //Выбираем перо myPen желтого цвета толщиной в 1 пикселя:
                    //myGrid.Background = brush;
                    //Pen fam_pen = new Pen(Color.FromArgb(0, 0, 0), 1);
                    // g.DrawEllipse(fam_pen, 300, 240, 10, 10); // окружностей
                    SolidBrush fam_brush = new SolidBrush(Color.Blue);
                    GR.FillEllipse(fam_brush, x, y, Radius, Radius);
               
            }

        }

        private void start_btn_Click(object sender, EventArgs e)
        {
            init_field();
            flag_life = true;
            timer1.Tick += new EventHandler(timer1_Tick); // Every time timer ticks, timer_Tick will be called
            timer1.Interval = 1000; // 1 сек
            timer1.Start();

            Men newman = new Men(null); // появился какой-то совершеннолетний тип случайного пола без фамилии
            pop_base.Add(newman);
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