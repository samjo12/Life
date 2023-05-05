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


namespace Class_teach
{
    public partial class Form1 : Form
    {
        Timer timer1 = new Timer();
        public static Random rand1 = new Random();
        public static Graphics GR = null, GRW=null;
        public static Bitmap BM = null; //фрейм-буфер
        public static Panel panel;
        public static List<Men> people = new List<Men>(); // список всех живых людей
        public static List<Men> deadMens = new List<Men>(); // список покойников, обновляется каждый ход
        public static List<Men> newBorns = new List<Men>(); // список новорожденных

        public static List<Family> fam_base = new List<Family>(); //список всех семей

        public static List<Cell> FreeCells = new List<Cell>(); // Список пустых клеток
        public static List<Cell> OccupyCells = new List<Cell>();//Список занятых клеток

        public static int panel_width = 100; // ширина информ-панели слева экрана
        public static bool flag_life = false;//Флаг, где true- идет жизнь, false - ждем таймера
        public static int cellSizeX = 10; // размер клетки в пикселях по оси Х
        public static int cellSizeY; // размер клетки в пикселях по оси Y (считаем автомат. как 9:16)       
        public static int numCellX; //Кол-во клеток по горизонтали
        public static int numCellY; //Кол-во клеток по вертикали
        public static int windowTop; //координаты формы Y
        public static int windowLeft;//координаты формы по X

        public static Pole pole = null; //поле клеток
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

        public const int DELAY_CELL_RESTORE = 5; //задержка восстановления значения food в клетке
        public const int createFamilyResource=MAX_CELL_FOOD / 2; //ресурс еды нужный для создания семьи

        public Form1()
        {
            InitializeComponent();
            
            BM = new Bitmap(Size.Width-10, Size.Height-40);  // с размерами
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

            GRW = Graphics.FromHwnd(Handle); // вывод фрейма в окно
        }

        public void init_field()
        {
            //передаем размеры формы в пикселях
            pole = new Pole(Size.Width-10, Size.Height-40); //создаем поле питательных клеток
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            Men newman;
            Cell newCell;
            if (e.Button == MouseButtons.Right)
            {
                newCell = pole.setXY(e.X, e.Y);
                if (newCell == null) return; //ячейка занята - выходим
                else
                {
                    newman = new Men(newCell); // создаем человека и приписываем его к ячейке
                    newCell.Man = newman; //записываем в ячейку ссылку на человека
                    people.Add(newman);
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            // Начинаем жить если ожидали таймера, если уже живем - то пропускаем тик.
            // if (flag_life is true) return; else flag_life = true;
            if (people.Count == 0) { flag_life = false;  return; }//людей нет - жизнь останавливается
            Do_Life();
            timer1.Start();
        }
        public void Do_Life()
        {
            // Переберем всех людей - каждый делает ход
            
            for (int i = 0; i < people.Count; i++)
            {
                pole.moveMen(people.ElementAt(i));
                people.ElementAt(i).Stat();
            }
            //удаляем почивших людей из списка
            foreach (var person in deadMens)
            {
                pole.deadMan(person);
                if (person.sex == MALE) statMales--; else statFemales--;
                person.Stat(person.age);
                people.Remove(person);
            }
            deadMens.Clear(); // уже обработали всех жмуров. очищаем список
            // добавляем новорожденных
            foreach (var person in newBorns)
                people.Add(person);     // добавляем в список людей

            newBorns.Clear(); // уже обработали всех новорожденных. очищаем список
            // нужно перебрать все пустые клетки с неполным ресурсом food. корм растет
            foreach (var cell in FreeCells)
                cell.foodGrow();

            GRW.DrawImageUnscaled(BM, 0, 0);//обновляем картинку
            statMoves++;
            Do_Stat(); //обновляем статистику на инфопанели слева
        }
        
        public void Do_Stat(bool start = false) //обновляем статистику на textbox
        {
            if(start==true)
            {
                statMoves=0;
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
            }
            statPeople = people.Count;
            tbStatMoves.Text = Convert.ToString(statMoves);
            tbStatPeople.Text = Convert.ToString(statPeople);
            tbStatMales.Text = Convert.ToString(statMales);
            tbStatFemales.Text = Convert.ToString(statFemales);
            tbStatInfants.Text = Convert.ToString(statAgeInfants);
            tbStatTeens.Text = Convert.ToString(statAgeTeens);
            tbStatAdults.Text = Convert.ToString(statAgeAdults);
            tbStatOldMans.Text = Convert.ToString(statAgeOldmans);
            tbStatFam.Text = Convert.ToString(fam_base.Count);
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

                if (person.age >= MAX_AGE)  // умер по возрасту
                { deadMens.Add(person); setFreeCell(oldCell); return; }
                if (person.health <= 0)  // умер по состоянию здоровья
                { deadMens.Add(person); setFreeCell(oldCell); return; }
                if (oldCell.Man == null)
                    oldCell.PaintAlarm();
                newCell = lookAround(oldCell); //осмотрим все соседние клетки и получим выбор
                if (newCell != null)// получили новую клетку для хода
                {   if (newCell != oldCell)
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
                if (person.age >= AgeAdult && person.age < AgeOldman)
                { decFood = DEC_CELL_FOOD * 12 / 10; } //*1.2
                else if (person.age < AgeInfant) { decFood = DEC_CELL_FOOD / 2; }
                else /*all others ages*/{ decFood = DEC_CELL_FOOD; }
                return decFood;
            }
            private void mealMen(Cell newCell, Men person)
            {   // проверим новую ячейку на наличие еды. Если еды достаточно- то восстановим здороvье
                // а если нет, то уменьшим
                int decFood=is_meal(person);

                if (newCell.food >= decFood)
                { //еды в клетке становиться меньше, здоровье восстанавливается
                    newCell.food -= decFood;
                    person.HealthUp();
                }
                else { person.HealthDown(); }
            }

            internal void deadMan(Men men)
            {
                if (men.relatives.Count > 0) //удаляем покойного из списка родни всех его родственников
                    foreach (var r in men.relatives)
                        if (r.relatives != null) r.relatives.Remove(men);
                if (men.myFamily != null) //удаляем покойного из членов семьи к которой он принадлежал
                    men.myFamily.RemoveFromFamily(men);

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
            public Cell lookAround(Cell currentCell)
            {    // возвращаем null если человек умрет
                Cell newCell = null;
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

                int ind;
                //для удобства записи введем переменные
                Men currentMan = currentCell.Man;

                // не будем смотреть в сторону за края поля, где нет других клеток
                //Left 0
                if (i > 0) { iac.Add(i - 1); }
                else if(endless_pole==true){ iac.Add(numCellX - 1);  }
                jac.Add(j);
                //UP&LEFT 1
                if (i > 0) { iac.Add(i - 1); } 
                else if (endless_pole == true){ iac.Add(numCellX - 1); }
                if (j > 0) { jac.Add(j - 1); }
                else if (endless_pole == true){ jac.Add(numCellY - 1); }
                //UP 2
                iac.Add(i);
                if (j > 0) {  jac.Add(j - 1); }
                else if (endless_pole == true) { jac.Add(numCellY - 1); }
                //UP&Right 3
                if (i < numCellX - 1) { iac.Add(i + 1); }
                else if (endless_pole == true) { iac.Add(0); }
                if (j > 0) { jac.Add(j - 1); }
                else if (endless_pole == true) { jac.Add(numCellY - 1); }
                //Right 4
                if (i < numCellX - 1) { iac.Add(i + 1);  }
                else if (endless_pole == true) { iac.Add(0); }
                jac.Add(j);
                //Right & DOWN 5
                if (i < numCellX - 1) { iac.Add(i + 1); }
                else if (endless_pole == true) { iac.Add(0); }
                if (j < numCellY - 1) { jac.Add(j + 1); }
                else if (endless_pole == true) { jac.Add(0); }
                //DOWN 6
                iac.Add(i);
                if (j < numCellY - 1) { jac.Add(j + 1); }
                else if (endless_pole == true) { jac.Add(0); }
                //Left & DOWN 7
                if (i > 0) { iac.Add(i - 1); }
                else if (endless_pole == true) { iac.Add(numCellX - 1); }
                if (j < numCellY - 1) { jac.Add(j + 1); }
                else if (endless_pole == true) { jac.Add(0); }


                    for (ind = 0; ind < iac.Count; ind++)// посмотрим на все соседние ячейки
                {
                    if (CellPole[iac[ind], jac[ind]].Man != null) //соседняя ячейка занята человеком
                    {
                        if (!currentMan.relatives.Contains(CellPole[iac[ind], jac[ind]].Man))
                        { //нашли рядом НЕродственника
                            AC_NonFamily.Add(CellPole[iac[ind], jac[ind]].Man);
                            if (currentMan.age >= AgeAdult && currentMan.age < AgeOldman)//свой возраст>18?
                                if (CellPole[iac[ind], jac[ind]].Man.age >= AgeAdult// проверка на возраст
                                    || CellPole[iac[ind], jac[ind]].Man.age < AgeOldman) //от 18 до OLDMAN
                                {
                                    if (CellPole[iac[ind], jac[ind]].Man.myFamily == null)//встретили несемейного
                                    { // без своей семьи
                                        if (CellPole[iac[ind], jac[ind]].Man.sex != currentMan.sex) //он еще и оказался свободным партнером
                                            AC_SinglePartners.Add(CellPole[iac[ind], jac[ind]].Man); // противоположного пола
                                    }
                                }
                        }
                    }
                    else // ячейка свободна. Оценим ее ресурс, учтем в списках AC_MaxFood и AC_SomeFood
                    {
                        AC_moves.Add(CellPole[iac[ind], jac[ind]]);
                        if (CellPole[iac[ind], jac[ind]].food == MAX_CELL_FOOD)
                            AC_MaxFood.Add(CellPole[iac[ind], jac[ind]]);
                        else AC_SomeFood.Add(CellPole[iac[ind], jac[ind]]);
                    }
                }

            
                //...
                if (currentMan.age >= AgeAdult && currentMan.age < AgeOldman) //взрослый и не старый чел
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
                                return currentCell; //ход окончен, остаемся на месте
                            }
                        }
                        else if (currentMan.myFamily.numAdultMembers < FAMILY_MAX_COUNT) //добавим нового члена в семью
                        {   // этот тот случай когда разрешена семья из 3х и более человек
                            Men partner = findBestPartner(AC_SinglePartners, currentCell);
                            if (partner != null)
                            {
                                currentMan.myFamily.AddToFamily(partner);
                                partner.myFamily = currentMan.myFamily; //вступает в семью
                                return currentCell; //ход окончен, остаемся на месте
                            }
                        }
                    }
                    //Если ход у замужней женщины желающей ребенка, а вокруг есть свободные клетки и ей повезло
                    if (currentMan.sex == FEMALE && currentMan.myFamily != null
                        && AC_moves.Count > 0 && currentMan.fert > 0 && (rand1.Next(0, 2) == 0 ? false : true))
                    {   // проверим достаток еды в семье. Среднее величина всех ресурсов клеток занимаемых семьей в расчете на
                        // одного члена семьи должна быть более 50%
                        int wholeFood = 0;
                        foreach (var member in currentMan.myFamily.members)
                        {
                            wholeFood += member.Cell.food;
                        }
                        if (wholeFood / currentMan.myFamily.members.Count > MAX_CELL_FOOD /2)
                        {   // еды достаточно, в семье рождается ребенок и
                            // помещается на одну из пустых клеток списка AC_moves
                            Cell childCell = AC_moves.ElementAt(rand1.Next(0, AC_moves.Count)); // выберем место для ребенка
                            Family childFam = currentMan.myFamily; //семья родителей
                            Men newman = new Men(childCell, childFam); //родился ребенок
                            childCell.Man = newman;
                            childFam.AddToFamily(newman);// принимаем его в семью
                            people.Add(newman);
                            OccupyCells.Add(childCell);// отмечаем выбранную клетку как занятую
                            FreeCells.Remove(childCell);

                            //newBorns.Add(newman);
                            currentMan.fertDown(); // уменьшаем число ожидаемых детей
                            currentMan.HealthDown(10); //уменьшается здоровье матери на 10%
                            return currentCell; //ход окончен, остаемся на месте
                        }
                    }
                }

                //Оценим количество еды вокруг и 
                // Если клеток с максимальным запасом еды несколько, то выберем направление случайно
                if (AC_MaxFood.Count > 0) return AC_MaxFood.ElementAt(rand1.Next(0, AC_MaxFood.Count));
                else
                {//проверим есть ли неполные клетки в которых достаточно еды
                    int max_food = currentCell.food;
                    int decFood = is_meal(currentMan);
                    newCell = currentCell;
                    if(AC_SomeFood.Count>0)
                    foreach (var m in AC_SomeFood)
                    { // ищем клетку с макс. запасом оставшейся еды
                        if (m.food > max_food) newCell = m;
                    }

                    if (AC_SomeFood.Count == 0 || max_food<decFood) 
                    {//похоже что еды вокруг недостаточно 
                        if (currentCell.food < decFood || AC_moves.Count==0)  
                        {//в моей клетке тоже недостаточно еды или вокруг нет свободных клеток
                            if (AC_NonFamily.Count() > 0) // Если вокруг есть неродственники, то нападаем 
                            { //на самого слабого, и в случае победы забираем ресурсы его клетки
                                // при этом здоровье теряет только проигравший
                                Men opponent=findBestOpp(AC_NonFamily, currentCell);
                                //if (opponent == null) { if(AC_moves.Count>0)return randMove(AC_moves); } //драки не вышло- идем куда идем...
                                bool Wins= currentMan.Battle(opponent);
                                statBattles++;
                                if (Wins == true)return currentCell;//остаемся на месте
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
            private Cell randMove(List<Cell> AC_moves)
            {
                return AC_moves.ElementAt(rand1.Next(0, AC_moves.Count));
            }

            private Men findBestPartner(List<Men> SinglePartners, Cell currentCell)
            { //вернем null если нет подходящего партнера
              //Если сумма еды в клетках чела и его возможного партнера >50% от максимума
              //то подберем ему наилучшего партнера 
              // Сортируем его по параметрам fert-при поиске женщин и force-при поиске мужчин
              // и делаем лучший выбор. 
                bool sex = currentCell.Man.sex;
                //int currentFood = currentCell.food;
                int numPartners=1; //кол-во имеющихся партнеров у чела (при семье от 3х и более партнеров)
                int myFamilyFood = 0;

                Men partner = null;
                if (currentCell.Man.myFamily != null)
                {
                    numPartners = currentCell.Man.myFamily.members.Count; //текущее число пратнеров в семье
                    foreach (var p in currentCell.Man.myFamily.members) myFamilyFood += p.Cell.food; //ресурсы семьи
                }
                else { myFamilyFood= currentCell.food; }
                numPartners++;
                foreach (var p in SinglePartners)
                {
                    if ((myFamilyFood+p.Cell.food)/numPartners < createFamilyResource) continue; //мало еды для создания семьи

                    if (partner == null) { partner = p; continue; }
                    if (sex == MALE) { if (p.force > partner.force) partner = p; }
                    else if (p.fert > partner.fert) partner = p;
                }
                return partner;
                // возвращаем null если ресурсов для создания семьи не хватает createFamilyResource
            }

            private Men findBestOpp(List<Men> NonFamily, Cell currentCell) 
            {   
                Men opp=null; // оппонент выглядящий слабее меня
                Men opp_weakest; // оппонент выглядящий слабейшим, но сильнее меня
                List<Men> Adults=new();
                List<Men> Oldmens=new();
                List<Men> Infants=new();

                // В первую очередь выбираем взрослых оппонентов, потом стариков, потом детей
                // выберем слабейшего противника по наименьшему forceBorn
                opp_weakest = NonFamily.First();
                foreach (var p in NonFamily)
                {
                    if (p.forceBorn < opp_weakest.forceBorn) opp_weakest = p;
                    if (p.age >= AgeAdult && p.age < AgeOldman) { Adults.Add(p); }
                    else if (p.age >= AgeOldman) { Oldmens.Add(p);  }
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
                            if(opp == null) opp = p;
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
            public Cell getCell(int x, int y)
            {
                return CellPole[x, y];
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
                if (OccupyCells.Contains(CellPole[x1, y1])) return null; //ячейка занята
                else
                {
                    OccupyCells.Add(CellPole[x1, y1]);
                    FreeCells.Remove(CellPole[x1, y1]);
                    return CellPole[x1, y1]; // возвращаем ячейку с полученной координатой
                }
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
            public Men Man
            {
                get { return men; }
                set { men = value; delayRestore = DELAY_CELL_RESTORE; reDrawCell(); }
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
            public void foodGrow()// перерасчет food
            {
                if (men != null || food >= MAX_CELL_FOOD) return;// клетка НЕ пуста или имеет макс ресурс еды
                else if (delayRestore > 0){ delayRestore--; return; }
                else//задержки нет
                {
                    food += REST_CELL_FOOD;
                    if (food > MAX_CELL_FOOD) food = MAX_CELL_FOOD;
                    delayRestore = DELAY_CELL_RESTORE;
                    if (food == MAX_CELL_FOOD) reDrawCell();
                }
            }

            public void reDrawCell()//обновляем цвет ячейки в зависимости от food
            {   // зеленый -белый(много/мало еды)
                PaintCell();
                if (men != null) {  men.paintMen(); }//перерисуем жителя если он находится в клетке
                //else  //перерисуем клетку
            }
            private void PaintCell()
            {
                SolidBrush myCellColor = new SolidBrush(Color.FromArgb(255 - food, 255, 255 - food)); //255 цвет
                GR.FillRectangle(myCellColor, x, y, w, h);
                
                Pen fam_pen = new Pen(Color.Gray);
                GR.DrawRectangle(fam_pen, x, y, w, h);
            }
            public void PaintAlarm()
            {
                SolidBrush myCellColor = new SolidBrush(Color.FromArgb(255, 0, 0)); //255 цвет красный
                GR.FillRectangle(myCellColor, x, y, w, h);
                Pen fam_pen = new Pen(Color.Gray);
                GR.DrawRectangle(fam_pen, x, y, w, h);
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
                if (Rod1 != null) if (!members.Contains(Rod1)) members.Add(Rod1); //проверка при добавлении
                if (Rod2 != null) if (!members.Contains(Rod2)) members.Add(Rod2);// на случай неполной семьи
                numAdultMembers += 2;
                //выберем цвет семьи
                R = Rod1.R ^ Rod2.R;
                B = Rod1.B ^ Rod2.B;
                G = Rod1.G ^ Rod2.G;
            }

            public void PaintFamily(int x, int y)
            {
                int Radius = (int)cellSizeX * 10 / 100; //10% от размера клетки
                int center5 = (int)((cellSizeX - Radius) / 2); //10% отступы от краев клетки

                SolidBrush fam_brush = new SolidBrush(Color.FromArgb(R, B, G));
                GR.FillEllipse(fam_brush, x + center5, y + center5, Radius, Radius);
            }

            public void AddToFamily(Men newMember) // добавляем в семью новорожденного или нового взрослого(если размер семьи
            {                                       // FAMILY_MAX_COUNT не превышен)
                if ((newMember.age > AgeAdult && newMember.age < AgeOldman && numAdultMembers < FAMILY_MAX_COUNT) || newMember.age == 0)
                    if (!members.Contains(newMember)) { members.Add(newMember); } //добавляем
            }
            public void RemoveFromFamily(Men Member) //удаляем члена семьи
            {
                int AdultMale = 0;
                int AdultFemale = 0;
                if (members.Contains(Member)) { members.Remove(Member); Member.myFamily = null; }
                foreach (var m in members)// посчитаем количество оставшихся взрослых членов
                    if (m.age >= AgeAdult) if (m.sex == MALE) AdultMale++; else AdultFemale++;
                if (AdultFemale == 0 || AdultMale == 0) { RemoveFamily(); fam_base.Remove(this); } //ликвидация семьи
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
            public int age { get; protected set; } // возраст
            private Cell myCell; //ссылка на клетку своего местонаходжения
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
                        if (m.age > AgeAdult || m.age < AgeOldman)
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
                }
                paintMen();
            }
            private void randMenParams()
            {
                sex = Convert.ToBoolean(rand1.Next(0, 2));
                if (sex == MALE) statMales++;
                else statFemales++; 
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
                if (myFamily != null) myFamily.PaintFamily(myCell.x, myCell.y);

            }
            public void Stat(int ageDeath = 0) //0-человек жив, иначе - возраст смерти
            {   // функция запускается каждый ход. Увеличивает Возраст 
                // дополняются статистические списки
                if (ageDeath != 0)
                {
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
                        if(myFamily!=null)myFamily.RemoveFromFamily(this);
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
            public void deadTime()
            {
                myCell = null;
                myFamily = null;
                relatives.Clear();
                relatives = null;

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
                int modDeltaForce= deltaForce > 0 ? deltaForce : deltaForce * (-1); 
                health += deltaForce;
                opp.health -= modDeltaForce;

                if (deltaForce > 0)
                {
                    myCell.grab(opp.myCell);//ЗАБИРАЮ РЕСУРСЫ ПРОТИВНИКА
                    result = true; // я остался жив
                }
                else //сила противника была выше, если я выживу , то стану сильнее
                {
                    force += (int)(modDeltaForce / 2); //сила выросла
                    opp.myCell.grab(myCell); //противник забрал мои ресурсы

                    result = false;
                }
                return result;
            }
        } // end for class Men

        private void start_btn_Click(object sender, EventArgs e)
        {
            if (pole == null) { init_field(); }
            else if (people.Count > 0 && flag_life==true) { timer1.Stop(); flag_life = false; return; }
            else if(people.Count >0 && flag_life==false){ timer1.Start(); return; }
            Do_Stat(true); //очистка статистики
            for (int i = 0; i < nudPeople.Value; i++)
            {
                Cell myCell = pole.getFreeCell();//ячейки заняты - выходим
                if (myCell == null) break;
                Men newman = new Men(myCell); // появился какой-то совершеннолетний 
                people.Add(newman);     // случайного пола без фамилии
                myCell.Man = newman;
            }
            flag_life = true;
            timer1.Tick += new EventHandler(timer1_Tick); // Every time timer ticks, timer_Tick will be called
             // 0.1 сек
            timer1.Start();
        }

        private void nudCells_ValueChanged(object sender, EventArgs e)
        {
            cellSizeX = (int)nudCells.Value;
        }

        private void nudSpeed_ValueChanged(object sender, EventArgs e)
        {
            timer1.Interval=100 - 10*((int)nudSpeed.Value-1);
        }

        private void nudPeople_ValueChanged(object sender, EventArgs e)
        {
            
        }
    }
}

