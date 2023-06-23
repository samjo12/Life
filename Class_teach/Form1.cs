using System;
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
using static System.Windows.Forms.AxHost;
using System.Security.Cryptography;


namespace Jiza
{
    public partial class Form1 : Form
    {
        Timer timer1 = new Timer();
        public static Random rand1 = new Random();
        public static Graphics GR = null, GRW = null, GRempty = null;
        public static Bitmap BM = null; //фрейм-буфер
        public static Bitmap BMempty = null; //пустой фрейм закраска
        public static Panel panel;
        public static List<Men> people = new List<Men>(); // список всех живых людей
        public static List<Men> deadMens = new List<Men>(); // список покойников, обновляется каждый ход
        public static List<Men> newBorns = new List<Men>(); // список новорожденных

        public static List<Family> fam_base = new List<Family>(); //список всех семей

        public static List<Cell> FreeCells = new List<Cell>(); // Список пустых клеток
        public static List<Cell> OccupyCells = new List<Cell>();//Список занятых клеток
        public static List<Block> BlockCells = new List<Block>(); //список клеток занятых блоками
        public static int panel_width = 100; // ширина информ-панели слева экрана
        public static bool flag_life = false;//Флаг, где true- идет жизнь, false - ждем таймера
        public static int cellSizeX = 100; // размер клетки в пикселях по оси Х
        public static int cellSizeY; // размер клетки в пикселях по оси Y (считаем автомат. как 9:16)       
        public static int numCellX; //Кол-во клеток по горизонтали
        public static int numCellY; //Кол-во клеток по вертикали
        public static int windowTop; //координаты формы Y
        public static int windowLeft;//координаты формы по X

        public static Pole pole = null; //поле клеток
        public static int notMovable; //количестко клеток стен
        public static long statMoves;
        public static int statPeople = 0;
        public static int statMales = 0;
        public static int statFemales = 0;
        public static string statError = "";
        public static int statAgeInfants = 0;
        public static int statAgeTeens = 0;
        public static int statAgeAdults = 0;
        public static int statAgeOldmans = 0;
        public static int statFams = 0;
        public static int statHalfFams = 0; //неполные семьи
        public static int statBattles = 0;
        public static int statKills = 0;
        public static int statDeath = 0;

        // КОНСТАНТЫ
        public static bool endless_pole = true; //бесконечное поле
        public const int MAX_AGE = 800; //100 ходов жизни на индивида
        public const int MAX_FERT = 20; //максимально возможное число детей

        public const int AgeInfant = MAX_AGE / 10;
        public const int AgeAdult = MAX_AGE * 18 / 100;
        public const int AgeOldman = MAX_AGE * 65 / 100;

        public const int MAX_CELL_FOOD = 255; // максимальное значение еды в клетке
        public const int REST_CELL_FOOD = 32; //51 из 255 (20%)возобновляемость ресурсов в пустой клетке 
        public const int DEC_CELL_FOOD = REST_CELL_FOOD * 1;
        public const int FAMILY_MAX_COUNT = 2; //максимальное количество взрослых членов в семье
        public const bool MALE = false;
        public const bool FEMALE = true;

        public const int DELAY_CELL_RESTORE = 3; //задержка восстановления значения food в клетке
        public const int createFamilyResource = MAX_CELL_FOOD / 2; //ресурс еды нужный для создания семьи

        public Form1()
        {
            InitializeComponent();
            timer1.Interval = 100 - 10 * ((int)nudSpeed.Value - 1);
            timer1.Tick += new EventHandler(timer1_Tick);

            BM = new Bitmap(Size.Width - 10, Size.Height - 40);  // с размерами
            BMempty = new Bitmap(Size.Width - 10, Size.Height - 40);

            //GR = Graphics.FromImage(BM);   // инициализация g
            /*
            pictureb = new Panel
            {
                Name = "panel1",
                Size = new Size(Size.Width - panel_width, Size.Height),
                Location = new Point(0, 0),
                BackgroundImage=BM
            };*/
            //Controls.Add(panel);
            GR = Graphics.FromImage(BM); //Фрейм-буфер для сохранения графики
            GRempty = Graphics.FromImage(BMempty);
            GRW = Graphics.FromHwnd(Handle); // вывод фрейма в окно
            //записываем пустой фрейм экрана
            SolidBrush brush = new SolidBrush(Control.DefaultBackColor);
            GRempty.FillRectangle(brush, 0, 0, BMempty.Width, BMempty.Height);
            Show_instruction(); // выводим инструкцию
        }

        public void Show_instruction()
        {
            MessageBox.Show("Питательные клетки\n" +
                "Игровое поле состоит из питательных клеток зеленого цвета. Чем зеленее клетка - " +
                "тем больше в ней еды( в белой клетке еды нет совсем). " +
                "Размерность поля имеет два вида: ограниченный рамками экрана и бесконечный. " +
                "Изначально поле заселяется заданным количеством разнополых взрослых особей " +
                "Мелюков и Фелюков, далее называемых народом МЕФОВ. Данные организмы свободно" +
                "перемещаются по полю в поисках изобилия пищи." +
                "Перемещаясь по клеткам, Фелюки и Мелюки расходуют их ресурс на свое питание. " +
                "Будучи оставленной в покое, питательная клетка способна к постепенному восстановлению " +
                "своих ресурсов за несколько циклов.\n\n" +
                "Народ Мефов\n" +
                "Мефы не являются бессмертными существами и живут определенное количество циклов. " +
                "В их жизни имеется несколько возрастных этапов: " +
                "детство (способны лишь немного кушать и двигаться), взросление(кушают, двигаются, " +
                "принимают участие в драках), врослая жизнь (много кушают, заводят семью, размножаются, " +
                "сражаются и строят дома) и старость (скромное питание и силы уже не те). " +
                "Организмы мефов обладают параметрами здоровья и силы, которые очень важны для " +
                "их борьбы за свое существование. В тяжелых жизненных ситуациях, от недостатка пищи " +
                "здоровье мефов может ухудшаться, вплоть до смертельного исхода. Поэтому от голода " +
                "и от тесноты, они способны проявить агрессию по отношению к соседям (но не родственникам). " +
                "Мефы выбирают себе наименее сильного соперника и проводят поединок. Проигравший по силе и " +
                "здоровью отступает неся урон или погибает, а победитель получает необходимую ему пищу " +
                "из клетки оппонента и место для перемещения. Каждый меф обозначен кругом своего цвета.\n\n" +
                "Семьи у Мефов\n" +
                "Данные разнополые существа способны к созданию семьи из заданного количества взрослых " +
                "членов и половому размножению. Пары из одной семьи не создаются." +
                "Семья создается навсегда, но может распасться в результате гибели ее членов. " +
                "Если в семье не останется двух взрослых разнополых членов - семья распадается. " +
                "Дети рожденные в семье помнят о своей принадлежности по рождению к той семье, в " +
                "которой они появились на свет и знают своих родственников. " +
                "Женские особи в плане способности к размножению имеют параметр Фертильности, " +
                "который позволяет им иметь определенное количество детей за время своей жизни, " +
                "при условии нахождения в семье. " +
                "Новорожденный наследует средний набор параметров всех взрослых членов семьи с " +
                "отклонением до 10%. Члены семьи имею один общий цвет семьи. Семейное древо " +
                "прирастает по линии Мелюков (по мужской). " +
                "Семейные мефы склонны держаться друг-друга, а не теряться на просторах поля.\n\n" +
                "Постройки у Мефов\n" +
                "Скопление семейных мефов в одном месте позволяет им возводить дома для семьи, " +
                "Если врослый член семьи был окружен другими членами семьи или постройками семьи, " +
                "то на этом месте появляется Дом семьи. В дом перемещаются все взрослые члены семьи. " +
                "С этого момента, каждые несколько циклов, дом будет взращивать взрослого члена семьи " +
                "При наличии дома, он становиться местом привлечения всех членов семьи на поле. " +
                "Обитатели дома  получают питание из клеток вокруг дома и защищяют его." +
                "Сруктура из 3х3 домов образует хутор, 5х5 - поселок, 7х7-город, 9х9- полис, 11х11-сити",
                "Симуляция Жиза");
        }
        public void init_field()
        {
            //ощищаем поле
            //передаем размеры формы в пикселях
            pole = new Pole(Size.Width - 10, Size.Height - 40); //создаем поле питательных клеток
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            Cell newCell;

            if (pole == null) return; //перед игрой, pole может быть еще не создано
            newCell = pole.setXY(e.X, e.Y);
            if (newCell == null) return;

            if (e.Button == MouseButtons.Right)
            {
                /*   if (newCell.Man != null && newCell.Man.myFamily!=null) //клетка c семейным челом
                   {
                       if (newCell.Man.notMovable) // Ячейка была стеной - ломаем строение :)
                       {
                           newCell.Man.notMovable = false;
                           OccupyCells.Remove(newCell);
                           FreeCells.Add(newCell);
                       }
                       else
                       {
                           newCell.Man.notMovable = true; //ячейка теперь занята - стеной :)
                           OccupyCells.Add(newCell);
                           FreeCells.Remove(newCell);
                       }
                       newCell.reDrawCell();
                   }*/
            }
            else if (e.Button == MouseButtons.Left) //ставим/убираем человека
            {
                /* if (newCell.Man.notMovable) return; //на стену не ставим. Сорри :)
                 if (newCell.Man == null) //создадим нового человека
                 {
                     newCell.Man = new Men(newCell); // появился какой-то совершеннолетний 
                     newBorns.Add(newCell.Man);     // случайного пола без фамилии
                     OccupyCells.Add(newCell);
                     FreeCells.Remove(newCell);
                 }
                 else // удалим чела из клетки
                 {
                     deadMens.Add(newCell.Man); //ставим в очередь на удаление
                     OccupyCells.Remove(newCell);
                     FreeCells.Add(newCell);
                 }
                 newCell.reDrawCell();*/
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            // Начинаем жить если ожидали таймера, если уже живем - то пропускаем тик.
            // if (flag_life is true) return; else flag_life = true;
            if (people.Count == 0)
            { flag_life = false; btnReset_Click(null, null); return; }//людей нет - жизнь останавливается
            Do_Life();
            if (flag_life == false) timer1.Stop();
            else timer1.Start();
        }
        public void Do_Life()
        {
            foreach (var item in BlockCells)
            {

                //поедим
                // размножимся

            }

            deadMens.Clear(); // уже обработали всех жмуров. очищаем список

            // Переберем всех людей - каждый делает ход
            for (int i = 0; i < people.Count; i++)
            {
                pole.moveMen(people.ElementAt(i));
                people.ElementAt(i).Stat();
            }

            //удаляем почивших людей из списка
            foreach (var person in deadMens.ToList())
            {
                pole.deadMan(person);
                person.Stat(person.age);
                people.Remove(person);
            }
            // добавляем новорожденных
            foreach (var person in newBorns)
                people.Add(person);     // добавляем в список людей

            newBorns.Clear(); // уже обработали всех новорожденных. очищаем список
            // нужно перебрать все пустые клетки с неполным ресурсом food. корм растет
            foreach (var cell in FreeCells.ToList())
                cell.foodGrow();

            GRW.DrawImageUnscaled(BM, 0, 0);//обновляем картинку
            statMoves++;
            Do_Stat(); //обновляем статистику на инфопанели слева

        }

        public void Do_Stat(bool start = false) //обновляем статистику на textbox
        {
            if (start == true)
            {
                statPeople = 0;
                statMoves = 0;
                statPeople = 0;
                statMales = 0;
                statFemales = 0;
                statAgeInfants = 0;
                statAgeTeens = 0;
                statAgeAdults = 0;
                statAgeOldmans = 0;
                statFams = 0;
                statBattles = 0;
                statKills = 0;
                statDeath = 0;
                GRW.DrawImageUnscaled(BMempty, 0, 0);//обновляем картинку
            }
            tbStatMoves.Text = Convert.ToString(statMoves);
            tbStatPeople.Text = Convert.ToString(statPeople);
            tbStatMales.Text = Convert.ToString(statMales);
            tbStatFemales.Text = Convert.ToString(statFemales);
            tbStatInfants.Text = Convert.ToString(statAgeInfants);
            tbStatTeens.Text = Convert.ToString(statAgeTeens);
            tbStatAdults.Text = Convert.ToString(statAgeAdults);
            tbStatOldMans.Text = Convert.ToString(statAgeOldmans);
            tbStatFam.Text = Convert.ToString(statFams);
            tbStatBattles.Text = Convert.ToString(statBattles);
            tbStatDeath.Text = Convert.ToString(statDeath);
            tbStatKills.Text = Convert.ToString(statKills);

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
                //GRW.DrawImageUnscaled(BMempty, 0, 0);//обновляем картинку

                //Посчитаем количество клеток на поле
                numCellX = (int)(FormWidth / cellSizeX);
                cellSizeY = cellSizeX;// (int)(9 * cellSizeX / 16);
                numCellY = (int)(FormHeight / cellSizeY);

                CellPole = new Cell[numCellX, numCellY];// Создадим массив для хранения состояния ячеек

                for (int j = 0; j < numCellY; j++)
                {
                    for (int i = 0; i < numCellX; i++)
                    {
                        int X = panel_width + i * cellSizeX; //100+i*cellSizeX
                        int Y = j * cellSizeY;
                        CellPole[i, j] = new Cell(X, Y, cellSizeX, cellSizeY);  //создадим пустые клетки с кормом
                        CellPole[i, j].i = i; CellPole[i, j].j = j;
                        FreeCells.Add(CellPole[i, j]); //записываем в список свободных ячеек
                    }
                }
            }

            public void moveMen(Men person)
            {
                Cell newCell;
                Cell oldCell = person.Cell;
                if (person.resident_flag) return; //чел проживает в блоке 
                if (person.age >= MAX_AGE)  // умер по возрасту
                { deadMens.Add(person); setFreeCell(oldCell); return; }
                if (person.health <= 0)  // умер по состоянию здоровья
                { deadMens.Add(person); setFreeCell(oldCell); return; }

                newCell = lookAround(oldCell); //осмотрим все соседние клетки и получим выбор
                if (newCell != null)// получили новую клетку для хода
                {
                    if (newCell != oldCell)
                    {
                        setFreeCell(oldCell);
                        setBuzyCell(newCell, person);
                    }
                    mealMen(newCell, person); //пора поесть+, или поголодать -
                }
                else //умер. нужно убрать его из семьи и из списков всех родственников
                {
                    deadMens.Add(person); //добавляем в список на удаление
                    setFreeCell(oldCell); //освобождаем клетку
                }
            }
            public int is_meal(Men person)
            {
                // вернем требуемое кол-во еды
                int decFood;
                if (person.isAdult())
                { decFood = DEC_CELL_FOOD * 12 / 10; } //*1.2
                else if (person.age < AgeInfant) { decFood = DEC_CELL_FOOD / 2; }
                else /*all others ages*/{ decFood = DEC_CELL_FOOD; }
                return decFood;
            }
            private void mealMen(Cell newCell, Men person)
            {   // проверим новую ячейку на наличие еды. Если еды достаточно- то восстановим здороvье
                // а если нет, то уменьшим
                int decFood = is_meal(person);

                if (newCell.food >= decFood)
                { //еды в клетке становиться меньше, здоровье восстанавливается
                    newCell.food -= decFood;
                    person.HealthUp();
                }
                else { person.HealthDown(); }
            }

            internal void deadMan(Men men)
            {
                if (men.relatives != null && men.relatives.Count > 0) //удаляем покойного из списка родни всех его родственников
                    foreach (var r in men.relatives)
                        if (r.relatives != null) r.relatives.Remove(men);
                if (men.myFamily != null) //удаляем покойного из членов семьи к которой он принадлежал
                    men.myFamily.RemoveFromFamily(men);

                men.deadTime(); // удаляем все данные по покойному из его экземпляра класса Men
                people.Remove(men);
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
            {  // отбираем все свободные ячейки и случайно возвращаем любую из них,
                // или если все заняты - возвращаем null
                int c = rand1.Next(0, FreeCells.Count);
                Cell freecell = FreeCells.ElementAt(c);
                // сразу заносим ячейку в список занятыми ячейками
                // и исключаем из списка со свободными ячейками
                OccupyCells.Add(freecell);
                FreeCells.RemoveAt(c);
                return freecell;
            }
            private int plusCell(int ac, int limit)
            {
                if (ac < limit - 1) { return (ac + 1); }
                else if (endless_pole == true) { return 0; }
                return -1; //запрещенное состояние
            }
            private int minusCell(int ac, int limit)
            {
                if (ac > 0) { return (ac - 1); }
                else if (endless_pole == true) { return (limit - 1); }
                return -1; //запрещенное состояние
            }
            protected List<Cell> getCellsAroundMe(Cell currentCell) //Проверено 100%
            {    // вернем список клеток окружающих ходящего персонажа    
                 // не будем смотреть в сторону за края поля, где нет других клеток
                 // или будем, если включен режим бесконечного поля endless_pole

                int i = currentCell.i;
                int j = currentCell.j;
                int iac, jac;
                List<Cell> cells = new List<Cell>();

                //UP 0 (0,-)
                if (j > 0) { cells.Add(CellPole[i, j - 1]); }
                else if (endless_pole == true) { cells.Add(CellPole[i, numCellY - 1]); }

                //UP&Right 1 (+,-)
                iac = plusCell(i, numCellX);
                jac = minusCell(j, numCellY);
                if ((iac + jac) >= 0) cells.Add(CellPole[iac, jac]);

                //Right 2 (+,0)
                if (i < numCellX - 1) { cells.Add(CellPole[i + 1, j]); }
                else if (endless_pole == true) { cells.Add(CellPole[0, j]); }

                //Right & DOWN 3 (+,+)
                iac = plusCell(i, numCellX);
                jac = plusCell(j, numCellY);
                if ((iac + jac) >= 0) cells.Add(CellPole[iac, jac]);

                //DOWN 4 (0,+)
                if (j < numCellY - 1) { cells.Add(CellPole[i, j + 1]); }
                else if (endless_pole == true) { cells.Add(CellPole[i, 0]); }

                //Left & DOWN 5 (-,+)
                iac = minusCell(i, numCellX);
                jac = plusCell(j, numCellY);
                if ((iac + jac) >= 0) cells.Add(CellPole[iac, jac]);

                //Left 6 (-,0)
                if (i > 0) { cells.Add(CellPole[i - 1, j]); }
                else if (endless_pole == true) { cells.Add(CellPole[numCellX - 1, j]); }

                //UP&LEFT 7 (-,-)
                iac = minusCell(i, numCellX);
                jac = minusCell(j, numCellY);
                if ((iac + jac) >= 0) cells.Add(CellPole[iac, jac]);

                return cells;
            }

            public Cell goToMyFamily(Cell currentCell, List<Cell> MaxFood, List<Cell> SomeFood, List<Cell> Moves, List<Men> NonFamily)
            {   //for closer position to family member
                //нужно сдвинуться в направлении члена семьи с флагом withFamily так, чтобы у новой ячейки,
                //индексы по X и/или Y сократилось
                //Если наш чел уже с семьей
                Cell newCell = currentCell;
                Men currentMen = currentCell.Man;
                int currX = currentCell.i;// текущие координаты ходящего
                int currY = currentCell.j;
                bool flag_gotoBlock = false;
                List<Cell> Food = new List<Cell>();
                foreach (var cell in MaxFood) Food.Add(cell);
                foreach (var cell in SomeFood) if (cell.food >= is_meal(currentMen)) Food.Add(cell);

                List<int[]> path = new List<int[]>();

                // мужик тянет семью к блоку(если такой есть) своей семьи по рождению
                // женщина тянется к супругу

                // ? для женщин ? найдем все возможные пути до взрослых членов нашей семьи
                // сначала надо проверить:
                // - есть ли в нашей семье блок
                // - есть ли блок в семье по рождению у взрослого мужчины
                // если такие блоки есть - прокладываем путь к ним,
                //а если нет, то если мы взрослый мужчина - идем за едой
                // если нет то идем за взрослым мужчиной


                foreach (var man in currentMen.myFamily.members) //перебираем членов семьи 
                {                        // рассчитаем до них путь
                    int[] coordMember = new int[2];//0-x,2-y
                    int xwayTo1, xwayTo2, ywayTo1, ywayTo2; // расстояния по x и y +с  учетом опции бесконечное поле
                    int x;// координаты x ,y клетки члена семьи
                    int y;

                    if (man == currentMen) continue; //самого себя пропустим
                    if (man.isAdult() && man.sex == MALE) //взрослый мужчина
                    {
                        if (currentMen.myFamily.blocks != null)
                        {
                            x = currentMen.myFamily.blocks.ElementAt(0).Cell.x; // координаты первого семейного блока
                            y = currentMen.myFamily.blocks.ElementAt(0).Cell.y;
                            flag_gotoBlock = true;
                        }
                        else if (currentMen.myFamilyByBorn!=null && currentMen.myFamilyByBorn.blocks != null)
                        {
                            x = currentMen.myFamilyByBorn.blocks.ElementAt(0).Cell.x;
                            y = currentMen.myFamilyByBorn.blocks.ElementAt(0).Cell.y;
                            flag_gotoBlock = true;
                        }
                        else
                        {
                            x = man.Cell.i; //координаты взрослого члена семьи
                            y = man.Cell.j;
                        }
                    }
                    else { //правило для детей и женщин - держаться за взрослыми :)
                        x = man.Cell.i; //координаты взрослого члена семьи
                        y = man.Cell.j;
                    }
                    // поле Не бесконечно
                    if (currX > x) xwayTo1 = -(currX - x);
                    else xwayTo1 = x - currX;
                    coordMember[0] = xwayTo1;
                    if (currY > y) ywayTo1 = -(currY - y);
                    else ywayTo1 = y - currY;
                    coordMember[1] = ywayTo1;

                    if (endless_pole) //если поле бесконечно проверим альтернативный вариант через край
                    {
                        if (x > currX)
                        {
                            xwayTo2 = -currX - (numCellX - 1 - x);
                            if (Math.Abs(xwayTo2) < Math.Abs(xwayTo1)) { coordMember[0] = xwayTo2; }
                        }
                        else
                        {
                            xwayTo2 = numCellX - 1 - currX + x;
                            if (Math.Abs(xwayTo2) < Math.Abs(xwayTo1)) { coordMember[0] = xwayTo2; }
                        }
                        if (y > currY)
                        {
                            ywayTo2 = -currY - (numCellY - 1 - y);
                            if (Math.Abs(ywayTo2) < Math.Abs(ywayTo1)) { coordMember[1] = ywayTo2; }
                        }
                        else
                        {
                            ywayTo2 = numCellY - 1 - currY + y;
                            if (Math.Abs(ywayTo2) < Math.Abs(ywayTo1)) { coordMember[1] = ywayTo2; }
                        }
                    }
                    path.Add(coordMember); //добавляем путь к взрослому члену семьи в список  
                    if (flag_gotoBlock==true) break;
                }
                // нет пути домой или заблокирован путь
                if (path.Count < 1) return findBestFood(currentCell, MaxFood, SomeFood, Moves, NonFamily);


                // найдем самый короткий маршрут к одному из членов семьи
                int bestScore = 0;
                for (int k = 0; k < path.Count; k++)
                {
                    int minLength = numCellX + numCellY; //минимальное кол-во клеток до ближайшего члена семьи
                    int[] coordMember = path.ElementAt(0); // здесь будет самый короткий путь

                    foreach (var direction in path)
                    { //переберем маршруты до найденных членов семьи и выберем самый короткий
                        int currLength = Math.Abs(direction[0]) > Math.Abs(direction[1]) ? direction[0] : direction[1];
                        currLength = Math.Abs(currLength); //убираем знак если нужно

                        if (currLength < minLength)
                        {
                            minLength = currLength;
                            coordMember[0] = direction[0];
                            coordMember[1] = direction[1];
                        }
                    }

                    // проверим есть ли свободные клетки в выбранном направлении
                    foreach (var cell in Food)
                    {
                        //if (cell.food < is_meal(currentMen)) //мало еды - клетка не подходит
                        //{ Food.Remove(cell); continue; }

                        int wayScore = 0;

                        if (currX + (coordMember[0] > 0 ? 1 : (-1)) == cell.i)
                            wayScore++; //Направление по Х подходит

                        if (currY + (coordMember[1] > 0 ? 1 : (-1)) == cell.j)
                            wayScore++;
                        if (wayScore == 0) { continue; } // направление не подходит
                        if (wayScore == 2) { if (cell != null) return cell; }//нам точно туда
                        else if (wayScore >= 0 && wayScore > bestScore) { bestScore = wayScore; newCell = cell; }
                    }
                    path.Remove(coordMember);

                }
                if (newCell == null) return findBestFood(currentCell, MaxFood, SomeFood, Moves, NonFamily);
                else return newCell;
                //return findBestFood(currentCell, MaxFood, SomeFood, Moves, NonFamily);
            }
            public Cell lookAround(Cell currentCell)
            {
                Cell newCell; // возвращаем новую клетку по окончанию хода
                // возвращаем null если человек умрет, или текущую клетку, если некуда пойти

                // оглядим все клетки вокруг currentCell
                //AC = AroundCells
                List<Cell> cellsAround; // все клетки вокруг ходящего
                List<Cell> AC_moves = new List<Cell>(); // Незанятые клетки
                List<Cell> AC_MaxFood = new List<Cell>(); //Клетки с полным запасом еды
                List<Cell> AC_SomeFood = new List<Cell>(); //Клетки с неполным запасом еды
                List<Men> AC_SinglePartners = new List<Men>(); //List of single partners
                List<Men> AC_NonFamily = new List<Men>(); //Не_члены_семьи поблизости
                List<Men> AC_Family = new List<Men>(); //члены семьи поблизости
                List<Block> AC_Family_Blocks = new List<Block>(); // блоки семьи поблизости
                List<Block> AC_NonFamily_Blocks = new List<Block>(); // блоки НЕсемьи поблизости

                //для удобства записи введем переменные
                Men currentMan = currentCell.Man;
                if (currentMan == null) return null;
                currentMan.withFamily = false;//пока неизвестно с семьей ли мы
                //if (cell.Man.notMovable) continue; //стена - ее обходим точно
                cellsAround = getCellsAroundMe(currentCell); //получаем список клеток вокруг
                currentMan.withFamily = false; //пока неясно есть ли рядом взрослые
                foreach (var cell in cellsAround)
                {
                    if (cell.Man != null) //соседняя ячейка занята человеком
                    {
                        if (!currentMan.relatives.Contains(cell.Man))
                        { //нашли рядом НЕродственника
                            AC_NonFamily.Add(cell.Man);
                            if (currentMan.isAdult())//свой возраст adult?
                                if (cell.Man.isAdult()) //проверка возраста 
                                {                          //соседа если от Adult до OLDMAN
                                    if (cell.Man.myFamily == null)//встретили несемейного
                                    { // без своей семьи
                                        if (cell.Man.sex != currentMan.sex) //он еще и оказался свободным партнером
                                            AC_SinglePartners.Add(cell.Man); // противоположного пола
                                    }
                                }
                        }

                        //если текущий чел из нашей семьи, 
                        if (currentMan.myFamily != null && currentMan.myFamily == cell.Man.myFamily)
                        {
                            AC_Family.Add(cell.Man);
                            if (currentMan.isAdult()) // наш чел взрослый?
                            {
                                if (cell.Man.isAdult())
                                {
                                    currentMan.withFamily = true;
                                    cell.Man.withFamily = true;
                                }
                                else cell.Man.withFamily = true;
                            }
                            else // наш чел салага
                            {
                                if (cell.Man.isAdult()) currentMan.withFamily = true;
                            }

                        }

                    }
                    else // в клетке не человек
                    {
                        if (cell.block == null) // и не блок
                        {// ячейка свободна. Оценим ее ресурс, учтем в списках AC_MaxFood и AC_SomeFood
                            AC_moves.Add(cell);
                            if (cell.food == MAX_CELL_FOOD) AC_MaxFood.Add(cell);
                            else AC_SomeFood.Add(cell);
                        }
                        else
                        {
                            if (cell.block.myFamily == currentMan.myFamily) AC_Family_Blocks.Add(cell.block);
                            else AC_NonFamily_Blocks.Add(cell.block);
                        }
                    }

                }

                //...
                if (currentMan.isAdult()) //взрослый и не старый чел
                {
                    if (AC_SinglePartners.Count > 0) // вокруг есть одинокие возможные партнеры
                    {
                        if (currentMan.myFamily == null) // до сих пор нет семьи? 
                        {   //можно выбрать самого достойного партнера
                            Men partner = findBestPartner(AC_SinglePartners, currentCell);
                            if (partner != null)
                            {   // есть подходящий - создадим семью
                                Family newFam = new Family(currentMan, partner);
                                fam_base.Add(newFam);
                                currentMan.myFamily = newFam;
                                partner.myFamily = newFam;

                                AC_NonFamily.Remove(partner);
                                AC_Family.Add(partner);
                                AC_SinglePartners.Remove(partner);
                                currentMan.withFamily = true;
                                partner.withFamily = true;
                                //return currentCell; 
                            }
                        }
                        else if (currentMan.myFamily.numAdultMembers < FAMILY_MAX_COUNT) //добавим нового члена в семью
                        {   // этот тот случай когда разрешена семья из 3х и более человек
                            Men partner = findBestPartner(AC_SinglePartners, currentCell);
                            if (partner != null)
                            {
                                currentMan.myFamily.AddToFamily(partner);
                                partner.myFamily = currentMan.myFamily; //вступает в семью
                                AC_Family.Add(partner);
                                AC_NonFamily.Remove(partner);
                                AC_SinglePartners.Remove(partner);
                                currentMan.withFamily = true;
                                partner.withFamily = true;
                                //return currentCell; //ход окончен, остаемся на месте
                            }
                        }
                    }

                }

                //если наш чел семейный, то смотрим, есть ли вокруг члены семьи 
                // если есть, то получаем 
                if (currentMan.myFamily != null) //если человек семейный
                {
                    if (AC_Family.Count + AC_Family_Blocks.Count == 8) //чел окружен членами семьи и их домами
                    {
                        Block newBlock = new Block(currentCell, currentMan.myFamily);
                        BlockCells.Add(newBlock);
                        currentCell.Man = null;
                        currentCell.block = newBlock;
                        return currentCell;
                    }
                    if (currentMan.withFamily == false)
                    { //пытаемся вернуться к семье
                        //получаем направление движения списком, 
                        //и проверяем куда сытнее будет сходить
                        //если в направлении семьи нельзя пойти без ущерба для здоровья,
                        //то идем туда где лучше кормят
                        return goToMyFamily(currentCell, AC_MaxFood, AC_SomeFood, AC_moves, AC_NonFamily);
                    }
                    else
                    { //человек рядом с семьей
                        //Если ход у замужней женщины(рядом с семьей) желающей ребенка, а вокруг есть свободные клетки
                        if (currentMan.sex == FEMALE && AC_moves.Count > 0 && currentMan.fert > 0)
                        {   // проверим достаток еды в семье. Среднее величина всех ресурсов клеток занимаемых семьей в расчете на
                            // одного члена семьи должна быть более 50%
                            Cell babyCell = isBabyBorn(currentCell, AC_moves); //если ребенок родилcя- получаем его клетку, иначе null
                            if (babyCell != null) AC_moves.Remove(babyCell); //клетка занята ребенком
                        }
                        // идем туда где больше еды, при условии что рядом будет член семьи
                        //return goToMyFamily(currentCell, AC_MaxFood, AC_SomeFood, AC_moves, AC_NonFamily);
                    }

                }
                //Чтобы сделать ход - выберем стратегию 
                // стратегия "где сытнее"

                newCell = findBestFood(currentCell, AC_MaxFood, AC_SomeFood, AC_moves, AC_NonFamily);
                return newCell;
            }

            private Cell isBabyBorn(Cell currentCell, List<Cell> Moves)
            {
                Men currentMan = currentCell.Man;
                int wholeFood = 0;
                foreach (var member in currentMan.myFamily.members)
                {
                    wholeFood += member.Cell.food;
                }
                if (wholeFood / currentMan.myFamily.members.Count > MAX_CELL_FOOD / 2)
                {   // еды достаточно, в семье рождается ребенок и
                    // помещается на одну из пустых клеток списка AC_moves
                    int myMove = RandomNumberGenerator.GetInt32(0, Moves.Count);
                    //Cell childCell = AC_moves.ElementAt(rand1.Next(0, AC_moves.Count)); // выберем место для ребенка
                    Cell childCell = Moves.ElementAt(myMove);
                    Family childFam = currentMan.myFamily; //семья родителей
                    Men newman = new Men(childCell, childFam); //родился ребенок
                    childCell.Man = newman;
                    childFam.AddToFamily(newman);// принимаем его в семью

                    people.Add(newman);
                    OccupyCells.Add(childCell);// отмечаем выбранную клетку как занятую
                    FreeCells.Remove(childCell);

                    //newBorns.Add(newman);
                    currentMan.fertDown(); // уменьшаем число ожидаемых детей
                                           //currentMan.HealthDown(10); //уменьшается здоровье матери на 10%
                    return childCell; //вернем клетку занятую ребенком
                }
                return null; //ребенок не родился
            }
            private Cell findBestFood(Cell currentCell, List<Cell> MaxFood, List<Cell> SomeFood, List<Cell> Moves, List<Men> NonFamily)
            {
                //Оценим количество еды вокруг и 
                // Если клеток с максимальным запасом еды несколько, то выберем направление случайно
                //if(currentCell.Man.withFamily==true)return currentCell;
                //return randMove(Moves);
                Cell newCell;
                int myMove;
                if (MaxFood.Count > 0)
                {
                    myMove = RandomNumberGenerator.GetInt32(0, MaxFood.Count);
                    newCell = MaxFood.ElementAt(myMove);
                }
                else
                {//проверим есть ли неполные клетки в которых достаточно еды
                    int max_food = currentCell.food;
                    int decFood = is_meal(currentCell.Man);
                    newCell = currentCell;
                    if (SomeFood.Count > 0)
                        foreach (var m in SomeFood)
                        { // ищем клетку с макс. запасом оставшейся еды
                            if (m.food > max_food) { max_food = m.food; newCell = m; }
                        }

                    if (SomeFood.Count == 0 || max_food < decFood)
                    {//похоже что еды вокруг недостаточно 
                        if (currentCell.food < decFood || Moves.Count == 0)
                        {//в моей клетке тоже недостаточно еды или вокруг нет свободных клеток
                            if (NonFamily.Count() > 0) // Если вокруг есть неродственники, то нападаем 
                            { //на самого слабого, и в случае победы забираем ресурсы его клетки
                                // при этом здоровье теряет только проигравший
                                Men opponent = findBestOpp(NonFamily, currentCell);
                                //if (opponent == null) { if(AC_moves.Count>0)return randMove(AC_moves); } //драки не вышло- идем куда идем...
                                bool Wins = currentCell.Man.Battle(opponent);
                                statBattles++;
                                if (Wins == true) return currentCell;//остаемся на месте
                                else // currentMen - погиб в бою
                                {
                                    statKills++;
                                    return null; // умер
                                }
                            }
                        }
                    }
                }
                return newCell;
            }

            private Men findBestPartner(List<Men> SinglePartners, Cell currentCell)
            { //вернем null если нет подходящего партнера
              //Если сумма еды в клетках чела и его возможного партнера >50% от максимума
              //то подберем ему наилучшего партнера 
              // Сортируем его по параметрам fert-при поиске женщин и force-при поиске мужчин
              // и делаем лучший выбор. 
                bool sex = currentCell.Man.sex;
                //int currentFood = currentCell.food;
                int numPartners = 1; //кол-во имеющихся партнеров у чела (при семье от 3х и более партнеров)
                int myFamilyFood = 0;

                Men partner = null;
                if (currentCell.Man.myFamily != null)
                {
                    numPartners = currentCell.Man.myFamily.members.Count; //текущее число пратнеров в семье
                    foreach (var p in currentCell.Man.myFamily.members) myFamilyFood += p.Cell.food; //ресурсы семьи
                }
                else { myFamilyFood = currentCell.food; }
                numPartners++;
                foreach (var p in SinglePartners)
                {
                    if (p == null || p.Cell == null) continue;
                    if ((myFamilyFood + p.Cell.food) / numPartners < createFamilyResource) continue; //мало еды для создания семьи

                    if (partner == null) { partner = p; continue; }
                    if (sex == MALE) { if (p.force > partner.force) partner = p; }
                    else if (p.fert > partner.fert) partner = p;
                }
                return partner;
                // возвращаем null если ресурсов для создания семьи не хватает createFamilyResource
            }

            private Men findBestOpp(List<Men> NonFamily, Cell currentCell)
            {
                Men opp = null; // оппонент выглядящий слабее меня
                Men opp_weakest; // оппонент выглядящий слабейшим, но сильнее меня
                List<Men> Adults = new();
                List<Men> Oldmens = new();
                List<Men> Infants = new();

                // В первую очередь выбираем взрослых оппонентов, потом стариков, потом детей
                // выберем слабейшего противника по наименьшему forceBorn
                opp_weakest = NonFamily.First();
                foreach (var p in NonFamily)
                {
                    if (p.forceBorn < opp_weakest.forceBorn) opp_weakest = p;
                    if (p.isAdult()) { Adults.Add(p); }
                    else if (p.age >= AgeOldman) { Oldmens.Add(p); }
                    else { Infants.Add(p); }
                }
                if (Adults.Count() > 0)
                    foreach (var p in Adults)
                    {
                        if (p.forceBorn < currentCell.Man.force)
                        {
                            if (opp == null) opp = p;
                            else if (p.forceBorn > opp.forceBorn) opp = p;
                        }
                    }
                else if (Oldmens.Count() > 0)
                    foreach (var p in Oldmens)
                    {
                        if (p.forceBorn < currentCell.Man.force)
                        {
                            if (opp == null) opp = p;
                            else if (p.forceBorn > opp.forceBorn) opp = p;
                        }

                    }
                else if (Infants.Count() > 0)
                    foreach (var p in Infants)
                    {
                        if (p.forceBorn < currentCell.Man.force)
                        {
                            if (opp == null) opp = p;
                            else if (p.forceBorn > opp.forceBorn) opp = p;
                        }
                    }
                if (opp == null) //все оппоненты выглядят сильнее
                {
                    return opp_weakest; //голод! нападем на наименее сильного

                }
                else return opp;
            }

            public Cell setXY(int X, int Y) //возвращаем ячейку по координатам пикселя на экране
            {   //определим в какую ячейку мы попали

                int i = 0, j;
                int x1 = -1, y1 = -1; //координаты ячейки(по умолчанию -1 не попали ни в какую ячейку)
                for (j = 0; j < numCellY; j++)
                {
                    if (CellPole[i, j].y < Y && (CellPole[i, j].y + cellSizeY) > Y)
                    {
                        y1 = j; //нашли в j - номер строки
                        for (i = 0; i < numCellX; i++)
                        {
                            if (CellPole[i, j].x < X && (CellPole[i, j].x + cellSizeX) > X)
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
                return CellPole[x1, y1]; // возвращаем ячейку с полученной координатой
            }
        }

        public class Cell
        {
            public int food { get; set; } //возобновляемый ресурс при отсутствии человека
            public int i { get; set; }  //номер строки в массиве CellPole
            public int j { get; set; }  //номер столбца в массиве CellPole
            public int x { get; set; } //координата по Х
            public int y { get; set; } //координата по Y
            public int w { get; set; } //ширина в пикселях
            public int h { get; set; } //высота в пикселях
            public Block block = null; //строение
            private Men men;
            public Men Man
            {
                get { return men; }
                set { men = value; 
                      if(value==null)delayRestore = DELAY_CELL_RESTORE; 
                      reDrawCell(); 
                    }
            }

            int delayRestore; //задержка восстановления значения food

            public Cell(int X, int Y, int W, int H) //создается пустая ячейка с кормом
            {
                x = X; y = Y; h = H; w = W;
                food = MAX_CELL_FOOD;
                men = null; //ссылка на человека, изначально ни на что не ссылается}
                PaintCell(); //перерисуем клетку
                delayRestore = 0; //задержки восстановления нет
            }

            public void foodGrow()// перерасчет food
            {
                if (men != null || food >= MAX_CELL_FOOD || block!=null) return;// клетка НЕ пуста или имеет макс ресурс еды
                else if (delayRestore > 0) { delayRestore--; return; }
                    else//задержки нет
                    {
                        food += REST_CELL_FOOD;
                        if (food > MAX_CELL_FOOD) food = MAX_CELL_FOOD;
                        delayRestore = DELAY_CELL_RESTORE;
                        /*if (food == MAX_CELL_FOOD) */
                        reDrawCell();
                    }
            }

            public void reDrawCell()//обновляем цвет ячейки в зависимости от food
            {   // зеленый -белый(много/мало еды)

                PaintCell();
                if (block != null) { PaintBlock(); return; }
                if (men != null) { men.paintMen(); }//перерисуем жителя если он находится в клетке

                //else  //перерисуем клетку
            }
            private void PaintCell()
            {
                SolidBrush myCellColor;

                myCellColor = new SolidBrush(Color.FromArgb(255 - food, 255, 255 - food));
                //255 цвет

                GR.FillRectangle(myCellColor, x, y, w, h);
                Pen fam_pen = new Pen(Color.Gray);
                GR.DrawRectangle(fam_pen, x, y, w, h);
            }
            public void PaintBlock()
            {
                SolidBrush myCellColor = new SolidBrush(Color.FromArgb(block.myFamily.R, block.myFamily.G, block.myFamily.B)); // цвет семьи
                GR.FillRectangle(myCellColor, x+1, y+1, w-1, h-1);
                //   Pen fam_pen = new Pen(Color.Gray);
                //  GR.DrawRectangle(fam_pen, x, y, w, h);
            }
            public void grab(Cell source) //забираем все ресурсы клетки source
            {
                int need = MAX_CELL_FOOD - food;
                if (food + source.food > MAX_CELL_FOOD) { food = MAX_CELL_FOOD; source.food -= need; }
                else { food += source.food; source.food = 0; }
            }
        }

        public class Family
        {
            public int R { get; private set; } // цвета семьи
            public int B { get; private set; }
            public int G { get; private set; }
            //фамилия передается по мужской линии

            // семья состоит из взрослых членов разных полов
            // если в семье остается один взрослый - семья распадается,
            // дети являются родственниками всем взрослым членам семьи
            // по умолчанию семья состоит из FAMILY_MAX_COUNT взрослых членов
            public List<Men> members { get; protected set; }  // список членов семьи
            public List<Men> membersByBorn { get; protected set; } // список членов семьи по рождению
            public List<Block> blocks { get; set; }

            public int numAdultMembers = 0;

            public Family(Men Rod1, Men Rod2) //создаем новую семью, нужно минимум 2 взрослых члена
            { // Если мужчина имеет семью, то новая семья не создается, а женщина берет его фамилию
                if (Rod1.sex == Rod2.sex) return; //однополых семей у нас нет, семью не создаем
/*
                Men men, women;
                men = Rod1.sex == MALE ? Rod1 : Rod2;
                if (men.myFamilyByBorn != null) //если  мен родился в семье
                {   //определим кто мужчина, а кто женщина

                    women = men == Rod1 ? Rod2 : Rod1;
                    //присоединяюсь со своей парой к своей семье 
                    men.myFamilyByBorn.AddToFamily(men);
                    men.myFamily.AddToFamily(women);
                }*/

                    //добавляем в семью пару родителей 
                    members = new List<Men>();
                    membersByBorn = new List<Men>();
                    if (Rod1 != null) if (!members.Contains(Rod1)) members.Add(Rod1); //проверка при добавлении
                    if (Rod2 != null) if (!members.Contains(Rod2)) members.Add(Rod2);// на случай неполной семьи
                    numAdultMembers += 2;
                    //выберем цвет семьи
                    R = Rod1.R ^ Rod2.R;
                    B = Rod1.B ^ Rod2.B;
                    G = Rod1.G ^ Rod2.G;
                    statFams++;


            }

            public void AddToFamily(Men newMember) // добавляем в семью новорожденного или нового взрослого(если размер семьи
            {                                       // FAMILY_MAX_COUNT не превышен)
                if ((newMember.isAdult() &&
                     numAdultMembers < FAMILY_MAX_COUNT) || newMember.age == 0)
                    if (!members.Contains(newMember))
                    {
                        members.Add(newMember);//добавляем нового члена семьи
                        if (newMember.age == 0) membersByBorn.Add(newMember);
                    }
            }
            public void RemoveFromFamily(Men Member) //посмертно убираем чела из семьи
            {
                int AdultMale = 0;
                int AdultFemale = 0;

                if (members.Contains(Member)) { members.Remove(Member); Member.myFamily = null; }
                if (membersByBorn.Contains(Member)) { membersByBorn.Remove(Member); Member.myFamilyByBorn = null; }

                foreach (var m in members)// посчитаем количество оставшихся взрослых членов
                    if (m.age >= AgeAdult) if (m.sex == MALE) AdultMale++; else AdultFemale++;
                if (AdultFemale == 0 || AdultMale == 0) { statHalfFams++; }
                if (members.Count == 0 && membersByBorn.Count == 0) { RemoveFamily(); fam_base.Remove(this); } //ликвидация семьи
            }
            private void RemoveFamily()
            {
                foreach (var m in members)
                {
                    m.myFamily = null;
                    m.withFamily = false;
                }
                members.Clear();
                members = null;
                statFams--;
            }

        }

        public class Men
        {
            public bool sex { get; private set; } //True-Female, False-Male
            public Family myFamily { get; set; }
            public Family myFamilyByBorn { get; set; }
            public List<Men> relatives { get; set; }
            public int age { get; protected set; } // возраст
            private Cell myCell; //ссылка на клетку своего местонаходжения
            public bool resident_flag = false; //true если проживает в блоке
            public Cell Cell
            {
                get { return myCell; }
                set { myCell = value; paintMen(); }
            }

            public int R { get; private set; }  // цвета индивида с рождения
            public int B { get; private set; }
            public int G { get; private set; }
            public int force { get; private set; }//текущая сила индивида (можно развить или потерять)
            public int forceBorn { get; private set; } //сила от рождения (типа физических данных)
            public int health { get; private set; }//текущее здоровье индивида
            protected int healthBorn; //здоровье индивила с рождения
            public int fert { get; private set; }//фертильность индивида кол-во детей которое он может произвести
            protected int fertBorn { get; private set; }//наследственная плодовитость
            public bool withFamily = false; //индикатор того, что человек находиться в контакте с семьей(true)
            public Men(Cell cell, Family family = null)
            {
                myFamily = family; //служит для принятия в семью 3-го и далее взрослого партнера

                relatives = new List<Men>(); //создаем список родственников
                myCell = cell;
                if (family == null) // появился взрослый чел со старта или кликом
                {
                    R = rand1.Next(0, 255);
                    B = rand1.Next(0, 255);
                    G = rand1.Next(0, 255);
                    randMenParams();
                    age = AgeAdult;
                    statAgeAdults++;
                }
                else //ребенок родился в семье
                {
                    myFamilyByBorn = family; //семья по рождению
                    age = 0;
                    sex = Convert.ToBoolean(rand1.Next(0, 2));
                    if (sex == MALE) statMales++;
                    else if (sex == FEMALE) statFemales++;
                    R = family.R;
                    B = family.B;
                    G = family.G;
                    //наследуются средние значения параметров взрослых членов семьи +- случайная величина 10%
                    int AdultMembers = 0;
                    fert = 0; health = 0; force = 1;

                    foreach (var m in family.members)
                    {
                        if (m.isAdult())
                        {
                            AdultMembers++;
                            force += m.force;
                            health += m.health;
                            fert += m.fert;
                        }
                        relatives.Add(m); //создаем список родственников из всех членов семьи
                    }
                    forceBorn = (int)((force / AdultMembers) * (1 + rand1.Next(-1, 2) / 10)); //+- 10% от среднего 
                    forceBorn /= 8; // у ребенка еще мало сил
                    healthBorn = (int)((health / AdultMembers) * (1 + rand1.Next(-1, 2) / 10));//значения в семье
                    healthBorn /= 8;// у ребенка еще не много здоровья
                    fertBorn = (int)((fert / AdultMembers) * (1 + rand1.Next(-1, 2) / 10));

                    force = forceBorn;
                    health = healthBorn;
                    fert = fertBorn;

                    statAgeInfants++;
                    statPeople++;
                }
                paintMen();
            }
            private void randMenParams()
            {
                sex = Convert.ToBoolean(rand1.Next(0, 2));
                if (sex == MALE) statMales++;
                else statFemales++;
                statPeople++;
                forceBorn = rand1.Next(10, 128); //сила от 10 до 128
                healthBorn = rand1.Next(64, 255);  //здоровье задается от макс 255 до четверти 64
                fertBorn = rand1.Next(0, MAX_FERT); //плодовитость. Каждый ход семейные пары при наличии достаточн. кол-ва еды
                //могут произвести ребенка с вероятностью rod1.fert&rod2.fert
                health = healthBorn;
                force = forceBorn;
                fert = fertBorn; //кол-во детей которое осталось/возможно произвести
            }

            public void paintMen()
            {
                int Radius = (int)cellSizeX * 80 / 100; //80% от размера клетки
                bool DrawOrFill; // Draw - false ; Fill-true

                if (age < AgeInfant) { Radius = (int)cellSizeX / 4; DrawOrFill = false; }
                else if (age < AgeAdult) { Radius = (int)cellSizeX / 2; DrawOrFill = false; }
                else if (age < AgeOldman) { DrawOrFill = true; }
                else { DrawOrFill = false; }
                int center5 = (int)((cellSizeX - Radius) / 2); //10% отступы от краев клетки
                if (myCell == null) return; //чел уже умер или его нет в этой клетке ... или Глюк. Выходим
                if (DrawOrFill == true)
                {
                    SolidBrush fam_brush = new SolidBrush(Color.FromArgb(R, B, G));
                    GR.FillEllipse(fam_brush, myCell.x + center5, myCell.y + center5, Radius, Radius);
                    Pen fam_pen = new Pen(Color.Black);
                    GR.DrawEllipse(fam_pen, myCell.x + center5, myCell.y + center5, Radius, Radius);
                }
                else
                {
                    Pen fam_pen = new Pen(Color.FromArgb(R, B, G));
                    GR.DrawEllipse(fam_pen, myCell.x + center5, myCell.y + center5, Radius, Radius);
                }
                if (myFamily != null)
                {
                    SolidBrush fam_brush = new SolidBrush(Color.FromArgb(myFamily.R, myFamily.B, myFamily.G));
                    GR.FillEllipse(fam_brush, myCell.x + center5, myCell.y + center5, Radius, Radius);
                }
                // нарисуем вертикальную шкалу здоровья по левому краю клетки
                //Pen health_pen = new Pen(Color.Black);
                //int ttt =  cellSizeY * ((health <= healthBorn ? health : 1 )/ (healthBorn>0?healthBorn:1));
                //GR.DrawLine(health_pen, myCell.x+1, myCell.y+ttt, 
                //    myCell.x+1, myCell.y+cellSizeY);
            }

            public void Stat(int ageDeath = 0) //0-человек жив, иначе - возраст смерти
            {   // функция запускается каждый ход. Увеличивает Возраст 
                // дополняются статистические списки
                if (ageDeath != 0)
                {
                    if (sex == MALE) statMales--; else statFemales--;
                    if (ageDeath < AgeInfant) statAgeInfants--;
                    else if (ageDeath < AgeAdult) statAgeTeens--;
                    else if (ageDeath < AgeOldman) statAgeAdults--;
                    else if (ageDeath < MAX_AGE) statAgeOldmans--;
                    statDeath++;
                    return;
                }

                age++;
                switch (age)
                {
                    case AgeInfant:
                        ModifyMan(age); statAgeInfants--; statAgeTeens++;
                        break;
                    case AgeAdult:
                        ModifyMan(age); statAgeTeens--; statAgeAdults++;
                        if (myFamily != null) { myFamily.RemoveFromFamily(this); }
                        myFamily = null; //ребенок стал взрослым и теперь будет искать себе новую семью
                        break;
                    case AgeOldman:
                        ModifyMan(age); statAgeAdults--; statAgeOldmans++;
                        break;
                    case MAX_AGE:
                        statAgeOldmans--; //при смерти
                        break;
                }
            }
            private void ModifyMan(int statAge)
            {
                switch (statAge)
                {
                    case AgeInfant:
                        healthBorn *= 4; forceBorn *= 4;//у подростка силы и здоровья в 4раза больше чем у ребенка
                        break;
                    case AgeAdult:
                        healthBorn *= 2; forceBorn *= 2;//у взрослого силы и здоровья вдвое больше чем у подростка
                        break;
                    case AgeOldman:
                        healthBorn /= 4; if (health > healthBorn) health = healthBorn; // в старости здоровье хуже 25%
                        forceBorn /= 4; if (force > forceBorn) force = forceBorn; // в старости силы осталось 25%
                        break;
                }

            }
            public bool isAdult()
            {
                if (age >= AgeAdult && age < AgeOldman) return true;
                else return false;
            }

            public void deadTime()
            {
                myCell = null;
                myFamily = null;
                if (relatives != null)
                {
                    relatives.Clear();
                    relatives = null;
                }
                statPeople--;

            }
            internal void HealthUp(int newValue = 0) //новое значение растущего здоровья
            {   // или здоровье прогрессивно восстанавливается от сытой жизни
                if (health >= healthBorn) return; // со здоровьем все в порядке, повышать некуда
                //Увеличиваем здоровье на заданную величину, если это требует newValue
                if (newValue != 0) { health += newValue; return; }
                //просто восстанавливаем здоровье за счет еды 
                int healthUp;
                int currentPercent = 100 * health / healthBorn; //текущий процент силы
                // введем прогрессивную шкалу восстановления силы
                if (currentPercent > 50) healthUp = healthBorn / 100; // +1%
                else if (currentPercent > 25) healthUp = healthBorn * 10 / 100;//+10%
                else if (currentPercent > 10) healthUp = healthBorn * 15 / 100; //+15%
                else healthUp = healthBorn * 20 / 100; //+20% //меньше минимума не падает сила
                health += healthUp;
                if (health > healthBorn) health = healthBorn;
            }
            internal bool HealthDown(int newValue = 0) //уменьшаем здоровье на значение
            {
                int healthDown;
                int currentPercent = 100 * health / healthBorn; //текущий процент здоровья
                if (newValue != 0)
                {
                    currentPercent -= newValue; if (currentPercent <= 0) { health = 0; return true; }//умер
                    health = currentPercent * healthBorn / 100; return false;
                }
                // введем прогрессивную шкалу снижения здоровья от недоедания
                if (currentPercent > 50) healthDown = healthBorn * 20 / 100; // -20%
                else if (currentPercent > 25) healthDown = healthBorn * 10 / 100;//-10%
                else if (currentPercent > 10) healthDown = healthBorn / 100; //-1%
                else healthDown = 0; //меньше минимума не падает сила
                force -= healthDown;
                //int healthDown = (int)(healthBorn*percent / 100);
                // индивид ухудшает свое здоровье голодовкой или дракой
                if (health < healthDown) { health -= healthDown; return true; } //умер
                else health -= healthDown;
                return false; // живой :)
            }
            internal void ForceUp(int newValue = 0) //новое значение
            {  //сила растет после успешной драки, или если newvalue=0 (еды много) , то прогрессивно растет до forceBorn
                if (newValue != 0) { force = newValue; return; }
                //просто восстанавливаем силу за счет еды 
                int forceUp;
                int currentPercent = 100 * force / forceBorn; //текущий процент силы
                // введем прогрессивную шкалу восстановления силы
                if (currentPercent > 50) forceUp = forceBorn / 100; // +1%
                else if (currentPercent > 25) forceUp = forceBorn * 10 / 100;//+10%
                else if (currentPercent > 10) forceUp = forceBorn * 15 / 100; //+15%
                else forceUp = forceBorn * 20 / 100; //+20% //меньше минимума не падает сила
                force += forceUp;
                if (force > forceBorn) force = forceBorn;
            }
            internal void ForceDown(int value = 0)
            {   //сила снижается от голода
                if (value != 0) // это снижение на определенную величину по требованию
                {
                    if (force > value) force -= value;
                    else force = forceBorn * 10 / 100;
                    return;
                }
                int forceDown;
                int currentPercent = 100 * force / forceBorn; //текущий процент силы
                // введем прогрессивную шкалу падения силы
                if (currentPercent > 50) forceDown = forceBorn * 20 / 100; // -20%
                else if (currentPercent > 25) forceDown = forceBorn * 10 / 100;//-10%
                else if (currentPercent > 10) forceDown = forceBorn / 100; //-1%
                else forceDown = 0; //меньше минимума не падает сила
                force -= forceDown;
            }
            public void fertDown()
            {
                if (fert > 0) fert--;
            }
            public bool Battle(Men opp)
            {
                bool result;
                int deltaForce = force - opp.force;
                int modDeltaForce = deltaForce > 0 ? deltaForce : deltaForce * (-1);
                health += deltaForce;
                opp.health -= modDeltaForce;

                if (deltaForce > 0)
                {
                    myCell.grab(opp.myCell);//ПЕРЕНОШУ ЕДУ ПРОТИВНИКА В свою КЛЕТКУ
                    //opp.myCell.grab(myCell);//ПЕРЕНОШУ СВОЮ ЕДУ В КЛЕТКУ ПРОТИВНИКА
                    result = true; // я остался жив
                }
                else //сила противника была выше, если я выживу , то стану сильнее
                {
                    force += (int)(modDeltaForce / 2); //сила выросла
                    opp.myCell.grab(myCell); //противник забрал мои ресурсы
                    //myCell.grab(opp.myCell); // противник перенес свои ресурсы на мою клетку 
                    result = false;
                }
                return result;
            }
        } // end for class Men
        public class Block //клетка-блок
        { // дом Семьи, появляется на том месте, где взрослый член семьи окружен
          // другими членами семьи и/или блоками своей семьи(семьями родственников).
          // После создания, в такой блок перемещаются 
          // все взрослые члены семьи
            public Family myFamily;
            public List<Men> residents;
            public Cell Cell; //ссылка на клетку местонаходжения блока
            //питание блока семьи происходит отбором средней величины от 
            //граничных питательных клеток. 
            //При нападении на блок, его здоровью складывается из всех взрослых членов семьи
            //если человек в блоке умер, его место может занять любой из подошедших взрослых
            //членов семьи
            // каждые AgeAdult циклов блок семьи производит врослое потомство 
            // равное числу членов семьи в блоке, которое помещается за пределы блока 
            public int health { get; private set; }
            public Block(Cell currentCell, Family family)
            {
                int man = 0, woman = 0;
                myFamily = family;
                Cell = currentCell;
                residents = new List<Men>();
                foreach (var member in family.members)
                {
                    if (man > 0 && woman > 0) break; //парочка уже заселена в блок
                    if (member.isAdult())
                    {
                        if (member.sex == MALE)
                        {
                            if (man == 0) man++;
                            else continue; // мужчина уже поселен в блок

                        }
                        else if (member.sex == FEMALE && woman == 0)
                        {
                            if (woman == 0) woman++;
                            else continue; // женщина уже поселена в блоке
                        }
                        residents.Add(member);
                        member.resident_flag = true;
                        OccupyCells.Remove(member.Cell);//освобождаем клетку чела
                        FreeCells.Add(member.Cell);
                        member.Cell = currentCell; // переселяем его в блок
                        currentCell.reDrawCell();

                    }
                }



            }

            // во время хода, происходит расчет кол-ва питательных клеток вокруг блока
            // и с помощью 
        }
        public class Hall //структура состоящая из нескольких блоков.
        {
            public static int count { get; set; }

            public static List<Hall> blocks { get; set; }
            //текущая сила ,блока (равна среднему всех челов из которых образован блок)
            public static int force { get; private set; }
            //текущее здоровье блока (равна среднему всех челов из которых образован блок)
            public static int health { get; private set; }


            public Hall(Cell currentCell, Family family)
            {
                paintHall();
            }
            private void paintHall()
            {

            }
        }

        private void start_btn_Click(object sender, EventArgs e)
        {
            if (pole == null) { init_field(); }
            else if (people.Count > 0 && flag_life == true) {/* timer1.Stop(); */flag_life = false; return; }
            else if (people.Count > 0 && flag_life == false) { timer1.Start(); flag_life = true; return; }
            Do_Stat(true); //очистка статистики

            //  добавим первых людей
            for (int i = 0; i < nudPeople.Value; i++)
            {
                Cell myCell = pole.getFreeCell();//ячейки заняты - выходим
                if (myCell == null) break;
                Men newman = new Men(myCell); // появился какой-то совершеннолетний 
                people.Add(newman);     // случайного пола без фамилии
                myCell.Man = newman;
            }

            flag_life = true;

            // 0.1 сек
            timer1.Start();
        }

        private void nudCells_ValueChanged(object sender, EventArgs e)
        {
            cellSizeX = (int)nudCells.Value;
        }

        private void nudSpeed_ValueChanged(object sender, EventArgs e)
        {
            timer1.Interval = 100 - 10 * ((int)nudSpeed.Value - 1);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {

            timer1.Stop();
            flag_life = false;
            //очистка экрана и вывод статистики ?
            //&message.Show



            Do_Stat(true); //очистка статистики и списков данных
            //очистка списков
            people.Clear();
            deadMens.Clear();
            newBorns.Clear();
            fam_base.Clear();
            FreeCells.Clear();
            OccupyCells.Clear();

            pole = null;
            //очистка экрана и вывод статистики ?

        }

        private void nudWalls_ValueChanged(object sender, EventArgs e)
        {

        }

        private void nudPeople_ValueChanged(object sender, EventArgs e)
        {

        }

    }

}

