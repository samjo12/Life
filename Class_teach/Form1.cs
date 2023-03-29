﻿using System;
using System.Collections;
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
        Timer timer1 = new Timer();
        public static Random rand1 = new Random(1);
        public static Graphics GR = null;
        public static Bitmap BM = null;
        public static List <Men> people = new List<Men>(); // список всех живых людей
        public static List <Men> deadMens=new List<Men>(); // список покойников, обновляется каждый ход

        public static List<Family> fam_base = new List<Family>(); //список всех семей

        public static List<Cell> FreeCells = new List<Cell>(); // Список пустых клеток
        public static List<Cell> OccupyCells = new List<Cell>();//Список занятых клеток

        //public DataGridView DataGridView1 = new DataGridView();
        public static int panel_width = 100; // ширина информ-панели слева экрана
        public static bool flag_life = false;//Флаг, где true- идет жизнь, false - ждем таймера
        public static int cellSizeX = 100; // размер клетки в пикселях по оси Х
        public static int cellSizeY; // размер клетки в пикселях по оси Y (считаем автомат. как 9:16)       
        public static int numCellX; //Кол-во клеток по горизонтали
        public static int numCellY; //Кол-во клеток по вертикали
        public static int windowTop; //координаты формы Y
        public static int windowLeft;//координаты формы по X

        public static Pole pole; //поле клеток
    // КОНСТАНТЫ
        public const int MAX_AGE = 100; //100 ходов жизни на индивида
        public const int AgeInfant = 10;
        public const int AgeAdult = 18;
        public const int AgeOldman = 45;
        
        public const int MAX_CELL_FOOD = 255; // максимальное значение еды в клетке
        public const int REST_CELL_FOOD = 51; //51 из 255 (20%)возобновляемость ресурсов в пустой клетке 

        public const int FAMILY_MAX_COUNT= 2; //максимальное количество взрослых членов в семье
        public const bool MALE = false;
        public const bool FEMALE = true;

        public const int DELAY_CELL_RESTORE = 3; //задержка восстановления значения food в клетке


        public Form1()
        {
            InitializeComponent();
            GR = Graphics.FromHwnd(Handle);

        }

        public void init_field()
        {
            //передаем размеры формы в пикселях
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
                    newman = new Men(temp); // создаем человека и приписываем его к ячейке
                    temp.Man=newman; //записываем в ячейку ссылку на человека
                    //newman.setCell(temp);
                    people.Add(newman);
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
            // Переберем всех людей и сделать ими ход
            // нужно перебрать все пустые клетки с неполным ресурсом food
            foreach (var person in people)
            {
                pole.moveMen(person);
            }
            //удаляем почивших людей из списка
            foreach(var person in deadMens)
            { 
                pole.deadMan(person);
            }
            deadMens.Clear(); // уже обработали всех жмуров. очищаем список
            foreach (var cell in FreeCells)
            {
                cell.reDrawCell();
            }

        }
        public class Pole
        {
            static int FormWidth;
            static int FormHeight;

            public static Cell[,] CellPole;
            public Pole(int sizeW, int sizeH)
            {// инициализация поля с кормом
                FormWidth = sizeW - panel_width;//получаем размер формы - 100 пикселей на меню
                FormHeight = sizeH;

                //Посчитаем количество клеток на поле

                numCellX = (int)(FormWidth / cellSizeX);
                cellSizeY = cellSizeX;// (int)(9 * cellSizeX / 16);
                numCellY = (int)(FormHeight / cellSizeY);

                CellPole = new Cell [numCellX,numCellY];// Создадим массив для хранения состояния ячеек

                for (int j = 0; j < numCellY; j++)
                {
                    for (int i = 0; i < numCellX; i++)
                    {
                        int X = panel_width + i * cellSizeX; //100+i*cellSizeX
                        int Y = j * cellSizeY;
                        CellPole[i,j] = new Cell(X,Y, cellSizeX, cellSizeY);  //создадим пустые клетки с кормом
                        CellPole[i, j].i = i; CellPole[i, j].j = j;
                        FreeCells.Add(CellPole[i,j]); //записываем в список свободных ячеек
                    }
                }
            }

            public void moveMen(Men person)
            {
                Cell newCell;
                Cell oldCell = person.Cell;

                if (person.age >= MAX_AGE)  // умер по возрасту
                { deadMens.Add(person); setFreeCell(oldCell); return; }


                newCell = lookAroundSingle(oldCell); //осмотрим все соседние клетки и получим выбор
                if (newCell != null)// получили новую клетку для хода
                {
                    setFreeCell(oldCell);
                    setBuzyCell(newCell, person);
                }
                else //умер. нужно убрать его из семьи и из списков всех родственников
                {
                    deadMens.Add(person); setFreeCell(oldCell);
                }

            }

            internal void deadMan(Men men)
            {
                if(men.relatives.Count>0) //удаляем покойного из списков всех родственников
                    foreach(var r in men.relatives)
                        r.relatives.Remove(men);
                if (men.myFamily != null) //удаляем покойного из членов семьи к которой он принадлежал
                    men.myFamily.RemoveFromFamily(men,men.myFamily);
                men.deadTime(); // удаляем все данные по покойному из его экземпляра класса Men
                people.Remove(men);
            }

            public Cell getFreeCellFamily(Family fam)
            {   // возвращаем свободную ячейку расположенную рядом с членами семьи
                // или если все заняты - возвращаем null
                
                return null;
            }

            public void setFreeCell(Cell oldCell)
            {
                OccupyCells.Remove(oldCell);
                FreeCells.Add(oldCell);
                oldCell.Man = null;
            }
            public void setBuzyCell(Cell newCell, Men Man)
            {
                FreeCells.Remove(newCell);
                OccupyCells.Add(newCell);
                Man.Cell = newCell;
                newCell.Man = Man;
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
            public Cell lookAroundSingle(Cell currentCell)
            {   
                Cell newCell=null; 
                int i = currentCell.i;
                int j = currentCell.j;

                // оглядим все клетки вокруг currentCell
                //AC = AroundCells
                List<Cell> AC_moves = new List<Cell>(); // Незанятые клетки
                List<Cell> AC_MaxFood = new List<Cell>(); //Клетки с полным запасом еды
                List<Cell> AC_SomeFood = new List<Cell>(); //Клетки с неполным запасом еды
                List<Men> AC_SinglePartners = new List<Men>(); //List of single partners
                List<Men> AC_NonFamily = new List<Men>(); //Не_члены_семьи поблизости
                List<int> iac = new List<int>();// { i-1, i-1, i, i+1, i+1, i+1, i, i-1 }; //направления взгляда на 
                List<int> jac = new List<int>();// { j, j-1, j-1, j-1, j, j+1, j+1, j+1 }; // все стороны света по осям X & Y
                //bool[] ijacs = new bool[8];
                int ind;
                //AC_NonFamily.ElementAt(ind);
                // не будем смотреть в сторону за края поля, где нет других клеток
                if (i > 0) { iac.Add(i - 1); jac.Add(j); } //Left 0
                if (i > 0 && j > 0) { iac.Add(i - 1); jac.Add(j - 1); } //UP&LEFT 1
                if (j > 0) { iac.Add(i); jac.Add(j - 1); }//UP 2
                if (i < numCellX - 1 && j > 0) { iac.Add(i+1); jac.Add(j - 1); }//UP&Right 3
                if (i < numCellX - 1) { iac.Add(i + 1); jac.Add(j); }//Right 4
                if (i < numCellX - 1 && j < numCellY - 1) { iac.Add(i + 1); jac.Add(j+1); }//Right & DOWN 5
                if (j < numCellY - 1) { iac.Add(i); jac.Add(j + 1); }//DOWN 6
                if (i > 0 && j < numCellY - 1) { iac.Add(i - 1); jac.Add(j + 1); }//Left & DOWN 7


                for (ind=0; ind < iac.Count; ind++)// посмотрим на все соседние ячейки
                {
                    if (CellPole[iac[ind], jac[ind]].Man != null) //соседняя ячейка занята человеком
                    {
                        if (!currentCell.Man.relatives.Contains(CellPole[iac[ind], jac[ind]].Man))
                        { //нашли рядом НЕродственника
                            AC_NonFamily.Add(CellPole[iac[ind], jac[ind]].Man);
                            if (CellPole[iac[ind], jac[ind]].Man.age >= AgeAdult// проверка на возраст
                                || CellPole[iac[ind], jac[ind]].Man.age < AgeOldman) //от 18 до OLDMAN
                            {
                                if (CellPole[iac[ind], jac[ind]].Man.myFamily == null)
                                { // без своей семьи
                                    if (CellPole[iac[ind], jac[ind]].Man.sex != currentCell.Man.sex) //он еще и оказался свободным партнером
                                        AC_SinglePartners.Add(CellPole[iac[ind], jac[ind]].Man); // противоположного пола
                                }
                            }
                        }
                    }
                    else //соседняя ячейка свободна. Оценим ее ресурс
                    {
                        AC_moves.Add(CellPole[iac[ind], jac[ind]]);
                        if(CellPole[iac[ind], jac[ind]].food==MAX_CELL_FOOD)
                            AC_MaxFood.Add(CellPole[iac[ind], jac[ind]]);
                        else AC_SomeFood.Add(CellPole[iac[ind], jac[ind]]);
                    }
                }

                //...
                //Если еды в текущей клетки >50% и у ходящего нет семьи и список AC_SinglePartners не пуст-
                // Сортируем его по параметрам fert и force и делаем лучший выбор. Ход при этом пропускается
                // возвращается текущая клетка и создается новая семья
                //...
                if (AC_SinglePartners.Count > 0) //нужно выбрать достойного партнера
                {
                    if (currentCell.Man.myFamily == null) // создадим семью
                    {
                        Men partner = findBestPartner(AC_SinglePartners, currentCell.Man.sex);
                        Family newFam= new Family(currentCell.Man, partner);
                        fam_base.Add(newFam);
                        currentCell.Man.myFamily = newFam;
                        partner.myFamily = newFam;

                    }
                    else if(currentCell.Man.myFamily.numAdultMembers < FAMILY_MAX_COUNT) //добавим нового члена в семью
                    { 

                    }

                }


                //Оценим количество еды вокруг и 
                // Если клеток с максимальным запасом еды несколько, то выберем направление случайно
                if (AC_MaxFood.Count>0) return AC_MaxFood.ElementAt(rand1.Next(0, AC_MaxFood.Count-1));
                else
                {
                    if (AC_SomeFood.Count == 0) //похоже что еды вокруг совсем нет. 
                    {
                        if (currentCell.food == 0)  //в моей клетке тоже нет еды
                        { 
                            if(AC_NonFamily.Count()>0) // Если вокруг есть неродственники, то нападаем 
                            {                         //на самого слабого, только если наша сила больше чем его
                                //после боя, в случае победы переходим на клетку побежденного
                        // Я голодный вокруг нет переспектив. Нужно напасть на НЕродственника
                        // чтобы восстановить здоровье в случае победы lj 100% и прибавить силу+5% но не более 100%
                        // к первоначальному ее количеству,
                        
                        // запуск боя и переход на клетку оппонента в случае выигрыша.
                        // Battle(currentCell.Man, AC_NonFamily);
                        // return;
                            }
                            else
                            { // если неродственников нет , здоровье уменьшается -40%
                              // идти в случайном направлении.
                                bool dead=currentCell.Man.HealthDown(40); //уменьшаем  здоровье на 40%
                                if (dead == true) //Чел умер от недоедания
                                    return null; 
                                if (AC_moves.Count > 0) return AC_moves.ElementAt(rand1.Next(0, AC_moves.Count - 1));
                            }
                       
                        }
                    }
                    else
                    {// найдем клетку в списке с наибольшим уровнем еды food и сравним с текущей клеткой
                        int max_food = currentCell.food;
                        newCell = currentCell;
                        foreach (var m in AC_SomeFood)
                        {
                            if (m.food > max_food) newCell = m;
                        }
                        
                    }
                  
                }
                
                return newCell;
            }

            private Men findBestPartner(List<Men> SinglePartners, bool sex)
            {
                if (sex == FEMALE)
                {   Men partnerMale=SinglePartners.First();
                    foreach (var p in SinglePartners) 
                        if (p.force > partnerMale.force) partnerMale = p;
                    return partnerMale;
                }
                else //MALE
                {   Men partnerFemale= SinglePartners.First();
                    foreach (var p in SinglePartners)
                        if (p.fert > partnerFemale.fert) partnerFemale = p;
                    return partnerFemale;
                }
            }
            public Cell getCell(int x, int y)
            {
                return CellPole[x,y];
            }

            public Cell setXY(int X, int Y) //возвращаем ячейку по координатам пикселя на экране
            {   //определим в какую ячейку мы попали

                int i = 0, j;
                int x1=-1, y1=-1; //координаты ячейки(по умолчанию -1 не попали ни в какую ячейку)
                for (j = 0; j < numCellY; j++)
                {

                    if (CellPole[i,j].y < Y && (CellPole[i,j].y+cellSizeY) > Y) 
                    {   y1 = j; //нашли в j - номер строки
                        for (i = 0; i < numCellX; i++)
                        {
                            if (CellPole[i,j].x < X && (CellPole[i,j].x + cellSizeX) > X)
                            { //нашли в i - номер столбца
                                x1 = i;
                                break; 
                            }
                        }
                        break; 
                    } 
                }
                if (x1 == -1 || y1 == -1) 
                    return null; //попаданий в ячейки не было
                if (OccupyCells.Contains(CellPole[x1,y1])) return null; //ячейка занята
                else
                {
                    OccupyCells.Add(CellPole[x1,y1]);
                    FreeCells.Remove(CellPole[x1,y1]);
                    return CellPole[x1,y1]; // возвращаем ячейку с полученной координатой
                }
                    
            }

        }
      

        public class Cell
        {
            public int food { get; protected set; } //возобновляемый ресурс при отсутствии человека
            public int i { get; set; }  //номер строки в массиве CellPole
            public int j { get; set; }  //номер столбца в массиве CellPole
            public int x { get; set; } //координата по Х
            public int y { get; set; } //координата по Y
            public int w { get; set; } //ширина в пикселях
            public int h { get; set; } //высота в пикселях
            public Men Man { get { return men; } 
                             set { men = value; delayRestore = DELAY_CELL_RESTORE;  reDrawCell(); } 
                           }
            private Men men;
            int delayRestore; //задержка восстановления значения food

            public Cell(int X, int Y, int W, int H) //создается пустая ячейка с кормом
            {                
                x = X; y = Y; h = H; w = W;
                food = MAX_CELL_FOOD;
                men = null; //ссылка на человека, изначально ни на что не ссылается}
                PaintCell(); //перерисуем клетку
                delayRestore = 0; //задержки восстановления нет
            }
            
            public void reDrawCell()//обновляем цвет ячейки в зависимости от food
            {   // зеленый -белый(много/мало еды)
 
                // перерасчет food
                if (men == null) // клетка пуста
                {
                    if (delayRestore == 0) //задержки нет
                    {
                        if (food < MAX_CELL_FOOD) 
                            food += REST_CELL_FOOD; 
                        if (food > MAX_CELL_FOOD) food = MAX_CELL_FOOD;
                        delayRestore = DELAY_CELL_RESTORE;
                    }
                    else //задержка была задана, уменьшаем на 1
                    { delayRestore--; }
                }
                else // в клетке есть житель
                {
                    if (food > 0)
                    { //еды становиться меньше
                        if (men.age < AgeAdult || men.age > AgeOldman) food -= REST_CELL_FOOD;
                        else food -= REST_CELL_FOOD*2;
                    }
                    if (food < 0) food = 0; //еды становиться меньше
                }
                    PaintCell(); //перерисуем клетку
                    if(men!=null)men.paintMen(); //перерисуем жителя если он находится в клетке
            }
            private void PaintCell()
            {
                SolidBrush myCellColor = new SolidBrush(Color.FromArgb(255 - food, 255, 255 - food)); //255 цвет
                GR.FillRectangle(myCellColor, x, y, w, h);
                Pen fam_pen = new Pen(Color.Black);
                GR.DrawRectangle(fam_pen, x, y, w, h);
            }

        }

        public class Family
        {
            public int R { get; private set; } // цвета семьи
            public int B { get; private set; }
            public int G { get; private set; }

            // семья состоит из взрослых членов разных полов
            // если в семье остается один взрослый - семья распадается,
            // дети являются родственниками всем взрослым членам семьи
            // по умолчанию семья состоит из FAMILY_MAX_COUNT взрослых членов
            public List<Men> members { get; protected set; }  // список членов семьи
            public int numAdultMembers = 0;

            public Family(Men Rod1, Men Rod2) //создаем новую семью, нужно минимум 2 взрослых члена
            {
                if (Rod1.sex == Rod2.sex) return; //однополых семей у нас нет, семью не создаем
                //добавляем в семью пару родителей
                members = new List<Men>();
                if (Rod1 != null) if(!members.Contains(Rod1)) members.Add(Rod1); //проверка при добавлении
                if (Rod2 != null) if (!members.Contains(Rod2)) members.Add(Rod2);// на случай неполной семьи
                numAdultMembers += 2;
            }

            public void PaintFamily(Men member)
            {
                int center5 = (int)(cellSizeX / 20); //10%
                int Radius = (int)cellSizeX / 8;
                int x = member.Cell.x;
                int y = member.Cell.y;

                SolidBrush fam_brush = new SolidBrush(Color.FromArgb(R, B, G));
                GR.FillEllipse(fam_brush, x + center5, y + center5, Radius, Radius);

            }

            public void AddToFamily(Men newMember) // добавляем в семью новорожденного или нового взрослого(если размер семьи
            {                                       // FAMILY_MAX_COUNT не превышен)
                if ((newMember.age>AgeAdult && newMember.age<AgeOldman && numAdultMembers<FAMILY_MAX_COUNT)|| newMember.age==0)
                    if (!members.Contains(newMember)) { members.Add(newMember); } //добавляем
            }
            public void RemoveFromFamily(Men Member, Family fam) //удаляем члена семьи
            {
                int AdultMale = 0;
                int AdultFemale = 0;
                if (members.Contains(Member)) { members.Remove(Member); Member.myFamily = null; }
                foreach(var m in members)// посчитаем количество оставшихся взрослых членов
                    if (m.age >= AgeAdult) if (m.sex == MALE) AdultMale++; else AdultFemale++;
                if (AdultFemale == 0 || AdultMale == 0) { RemoveFamily(); fam_base.Remove(fam); } //ликвидация семьи
            }
            private void RemoveFamily()
            {
                foreach (var m in members)
                { m.myFamily = null; }
                members.Clear();
                members = null;
                
            }

        }
        public class Men
        {
            public bool sex { get; private set; } //True-Female, False-Male
            public Family myFamily { get; set; }
            public List<Men> relatives { get; set; }
            public int age { get; set; } // возраст
            private Cell myCell; //ссылка на клетку своего местонаходжения
            public Cell Cell
            {
                get { return myCell; }
                set { myCell = value; HealthUp(); paintMen(); }// восстановим здоровье если требуется
            } 

            private int R;  // цвета индивида с рождения
            private int B;
            private int G;
            public int force { get; private set; }//текущая сила индивида (можно развить или потерять)
            protected int forceBorn; //сила с рождения 
            public int health { get; private set; }//текущее здоровье индивида
            protected int healthBorn; //здоровье индивила с рождения
            public int fert { get; private set; }//фертильность индивида

            public Men()
            {
                myFamily = null;
                relatives = new List<Men>(); //создаем список родственников
                myCell =pole.getFreeCell();
                randMenParams();
                age = 18;

                paintMen(); 
            }
            public Men(Cell cell)
            {
                myFamily = null;
                relatives = new List<Men>(); //создаем список родственников
                myCell = cell;
                R = rand1.Next(0, 255);
                B = rand1.Next(0, 255);
                G = rand1.Next(0, 255);
                randMenParams();
                age = 18;

                paintMen();
            }
            private void randMenParams()
            {
                this.sex = Convert.ToBoolean(rand1.Next(0, 1));

                forceBorn = rand1.Next(10, 128); //сила от 10 до 128
                healthBorn = rand1.Next(64, 255);  //здоровье задается от макс 255 до четверти 64
                fert = rand1.Next(0,10); //плодовитость. Каждый ход семейные пары при наличии достаточн. кол-ва еды
                //могут произвести ребенка с вероятностью rod1.fert&rod2.fert
                health = healthBorn;
                force = forceBorn;
            }
            public void menBorn(Family family)
            {
                age = 0;
                R = family.R;
                B = family.B;
                G = family.G;
                relatives = new List<Men>(); //создаем список родственников
                //наследуются средние значения параметров взрослых членов семьи +- случайная величина 10%
                int AdultMembers = 0;
                fert = 0; health = 0; force = 1;

                foreach (var m in family.members)
                {
                    if (m.age > AgeAdult || m.age < AgeOldman) 
                    { 
                        AdultMembers++;
                        force += m.force;
                        health += m.health;
                        fert += m.fert;
                    }
                    relatives.Add(m); //создаем список родственников из всех членов семьи
                }
                forceBorn = (int)(force / AdultMembers+(rand1.Next(-1, 1) * rand1.Next(1, 10)));
                healthBorn = (int)(health / AdultMembers + (rand1.Next(-1, 1) * rand1.Next(1, 10)));
                health = healthBorn;
                force = forceBorn;
                fert = (int)(fert / AdultMembers + (rand1.Next(-1, 1) * rand1.Next(1, 10)));
            }

            public void paintMen()
            {
                
                int center5=(int)(cellSizeX/20); //10%
                int Radius=cellSizeX;
                bool DrawOrFill; // Draw - false ; Fill-true
                if (age < AgeInfant) { Radius = (int)cellSizeX / 8; DrawOrFill = false; }
                else if (age < AgeAdult) { Radius = (int)cellSizeX / 2; DrawOrFill = false; }
                else if (age < AgeOldman) { Radius = Radius - center5*2;  DrawOrFill = true; }
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
            public void deadTime()
            {
                myCell = null;
                myFamily = null;
                relatives.Clear();
                relatives = null;

            }
            internal void HealthUp()
            {
                if (health >= healthBorn) return; // со здоровьем все в порядке
                int healthUp=(int)(healthBorn*10/100); //восстанавливается за 10% за ход
                health += healthUp;
            }
            internal bool HealthDown(int percent)
            {
                int healthDown = (int)(healthBorn*percent / 100);
                // индивид ухудшает свое здоровье голодовкой или дракой
                if (health < healthDown) return true; //умер
                else health -= healthDown;
                return false; // живой :)
            }
        } // end for class Men

        private void start_btn_Click(object sender, EventArgs e)
        {
            init_field();

            Men newman = new Men(); // появился какой-то совершеннолетний тип случайного пола без фамилии
            people.Add(newman);
            flag_life = true;
            timer1.Tick += new EventHandler(timer1_Tick); // Every time timer ticks, timer_Tick will be called
            timer1.Interval = 100; // 1 сек
            timer1.Start();
        }
    }
}

