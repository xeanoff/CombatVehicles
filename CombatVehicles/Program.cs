namespace WarMachines
{
    public enum VehicleType { ArmoredCar, Tank, AirDefenceVehicle }

    // main abstract class
    abstract class CombatVehicle
    {
        // main fields
        public VehicleType CombatVehicleType { get; protected set; }
        public string Model { get; set; }
        public double Health { get; protected set; }
        public bool IsDestroyed { get => Health <= 0; }

        // attack method, returns veh damage
        public abstract double Attack();

        // defence method, returns taken damage based on protection
        public abstract double Defence(double damageTaken);

        // method for get full veh name
        public string GetVehicleName() => $"[{CombatVehicleType} {Model}]";
    }

    // tank class -> combat veh
    class Tank : CombatVehicle
    {
        private double reloadTime;      // R
        private double accuracy;        // A
        private double armourThickness; // T

        // constructor (init)
        public Tank(string model, double health, double reloadTime, double accuracy, double armourThickness)
        {
            CombatVehicleType = VehicleType.Tank;
            Model = model;
            Health = health;
            this.reloadTime = reloadTime;
            this.accuracy = accuracy;
            this.armourThickness = armourThickness;
        }

        // overrided attack method;
        public override double Attack() => !IsDestroyed ? Math.Round(100 * accuracy / reloadTime, 2) : 0;

        //overrided defence attack method;
        public override double Defence(double damageTaken)
        {
            double totalDamageTaken = damageTaken - armourThickness;
            if (totalDamageTaken > 0)
            {
                if (Health - totalDamageTaken > 0)
                {
                    Health -= totalDamageTaken;
                    return Math.Round(totalDamageTaken, 2);
                }
                Health = 0;
                //IsDestroyed;
                return Math.Round(totalDamageTaken, 2);
            }
            return 0;
        }
    }

    //armored car -> combat vehicle
    class ArmoredCar : CombatVehicle
    {
        private int weaponCount; // C
        private double speed;    // S

        //constructor (init)
        public ArmoredCar(string model, double health, int weaponCount, double speed)
        {
            CombatVehicleType = VehicleType.ArmoredCar;
            Model = model;
            Health = health;
            this.weaponCount = weaponCount;
            this.speed = speed;
        }

        //overrided attack method;
        public override double Attack() => !IsDestroyed ? 50 * weaponCount : 0;

        //overrided defence method;
        public override double Defence(double damageTaken)
        {
            double totalDamageTaken = damageTaken - speed / 2;
            if (totalDamageTaken > 0)
            {
                if (Health - totalDamageTaken > 0)
                {
                    Health -= totalDamageTaken;
                    return Math.Round(totalDamageTaken, 2);
                }
                Health = 0;
                //IsDestroyed = true;
                return Math.Round(totalDamageTaken, 2);
            }
            return 0;
        }
    }

    // air vehicle -> combat vehicle
    class AirDefenceVehicle : CombatVehicle
    {
        private double attackRange; // L
        private double fireRate;    // R
        private int mobility;       // M

        // constructor (init)
        public AirDefenceVehicle(string model, double health, double attackRange, double fireRate, int mobility)
        {
            CombatVehicleType = VehicleType.AirDefenceVehicle;
            Model = model;
            Health = health;
            this.attackRange = attackRange;
            this.fireRate = fireRate;
            this.mobility = mobility;
        }

        // overrided attack method;
        public override double Attack() => !IsDestroyed ? Math.Round(150 + attackRange * (fireRate / 10), 2) : 0;

        // overrided defence method;
        public override double Defence(double damageTaken)
        {
            double totalDamageTaken = damageTaken / mobility;
            if (totalDamageTaken > 0)
            {
                if (Health - totalDamageTaken > 0)
                {
                    Health -= totalDamageTaken;
                    return Math.Round(totalDamageTaken, 2);
                }
                Health = 0;
                //IsDestroyed = true;
                return Math.Round(totalDamageTaken, 2);
            }
            return 0;
        }
    }

    // main battle class
    class Battle
    {
        // team fields
        private List<CombatVehicle> teamRed;
        private List<CombatVehicle> teamBlue;

        // winner
        public string? Winner { get; private set; } = null;

        // constructor (init)
        public Battle(List<CombatVehicle> teamRed, List<CombatVehicle> teamBlue)
        {
            this.teamRed = teamRed;
            this.teamBlue = teamBlue;
        }

        // main battle method
        public void ConstructBattleground()
        {
            // count won rounds
            int redWonRounds = 0;
            int blueWonRounds = 0;
            for (int i = 0; i < teamRed.Count; i++)
            {
                // battle process
                while (!teamRed[i].IsDestroyed && !teamBlue[i].IsDestroyed)
                {
                    // red attacks
                    double redTakeDamage = Math.Round(teamRed[i].Attack(), 2);
                    double blueGiveDamage = teamBlue[i].Defence(redTakeDamage);

                    // blue attacks
                    double blueTakeDamage = Math.Round(teamBlue[i].Attack(), 2);
                    double redGiveDamage = teamRed[i].Defence(blueTakeDamage);

                    Console.WriteLine();

                    //red attack output
                    Console.WriteLine($"{teamRed[i].GetVehicleName()} [RED] attacks {teamBlue[i].GetVehicleName()} [BLUE] with {redTakeDamage} damage!");
                    Console.WriteLine($"{teamBlue[i].GetVehicleName()} [BLUE] defended from {Math.Round(redTakeDamage - blueGiveDamage, 2)} damage ({blueGiveDamage} damage taken)!");

                    //blue attack output
                    Console.WriteLine($"{teamBlue[i].GetVehicleName()} [BLUE] attacks {teamRed[i].GetVehicleName()} [RED] with {blueTakeDamage} damage!");
                    Console.WriteLine($"{teamRed[i].GetVehicleName()} [RED] defended from {Math.Round(blueTakeDamage - redGiveDamage, 2)} damage ({redGiveDamage} damage taken)!");

                    //total HP output
                    Console.WriteLine($"{teamRed[i].GetVehicleName()} [RED] has {Math.Round(teamRed[i].Health, 2)} HP left! {teamBlue[i].GetVehicleName()}[BLUE] has {Math.Round(teamBlue[i].Health, 2)} HP left!");
                }
                // +won round
                if (teamBlue[i].IsDestroyed)
                    redWonRounds += 1;
                else
                    blueWonRounds += 1;
                Console.WriteLine("--------------------------------------------------------------------------------------------------");
            }
            // whoever wins the most rounds wins
            Winner = redWonRounds > blueWonRounds ? "Red" : "Blue";
        }

    }
    internal class Program
    {
        static List<CombatVehicle> FillList(int size)
        {
            // random model names
            string[] randomModels = { "YTO7 Aristodemus",
                                    "L069J Rhino",
                                    "XPH2V Anteater",
                                    "S-80C Lionheart",
                                    "S-V3G Challenger",
                                    "Y-B5I Blizzard",
                                    "D1M7 Diablo",
                                    "YJ48J Comanche",
                                    "I2-3 Thunderclap",
                                    "O-P2 Genesis",
                                    "B7-9R Czar",
                                    "RM-2 Bandit",
                                    "PFP4I Creator",
                                    "P-K7 Curator",
                                    "W1-4E Cyclops",
                                    "A521 Hades",
                                    "O-T4W Thor",
                                    "K364V Julius",
                                    "WGN92 Viper",
                                    "Y-P5 Leopard"};

            // needed classes init
            Random random = new(Guid.NewGuid().GetHashCode());
            List<CombatVehicle> combatVehicleList = new();

            //fill lists with random values
            for (int i = 0; i < size; i++)
            {
                Array values = Enum.GetValues(typeof(VehicleType));
                VehicleType randomVehicle = (VehicleType)values.GetValue(random.Next(values.Length));
                if (randomVehicle == VehicleType.Tank)
                {
                    string modelName = randomModels[random.Next(0, randomModels.Length)];
                    double health = random.NextDouble() * (1300 - 900) + 900;
                    double reloadTime = random.NextDouble() * (10 - 7) + 7;
                    double accuracy = random.NextDouble() * (150 - 50) + 50;
                    double armourThickness = random.NextDouble() * (150 - 50) + 50;
                    Tank tank = new(modelName, health, reloadTime, accuracy, armourThickness);
                    combatVehicleList.Add(tank);
                }
                else if (randomVehicle == VehicleType.ArmoredCar)
                {
                    string modelName = randomModels[random.Next(0, randomModels.Length)];
                    double health = random.NextDouble() * (1300 - 900) + 900;
                    int weaponCount = random.Next(5, 10);
                    double speed = random.NextDouble() * (80 - 40) + 40;
                    ArmoredCar armCar = new(modelName, health, weaponCount, speed);
                    combatVehicleList.Add(armCar);
                }
                else if (randomVehicle == VehicleType.AirDefenceVehicle)
                {
                    string modelName = randomModels[random.Next(0, randomModels.Length)];
                    double health = random.NextDouble() * (1300 - 900) + 900;
                    double attackRange = random.NextDouble() * (50 - 20) + 20;
                    double fireRate = random.NextDouble() * (150 - 50) + 50;
                    int mobility = random.Next(1, 10);
                    AirDefenceVehicle airplane = new(modelName, health, attackRange, fireRate, mobility);
                    combatVehicleList.Add(airplane);
                }
            }
            return combatVehicleList;
        }
        static void Main()
        {
            // enter values
            Console.Write("Enter the number of participants in the battle from 1 side: ");
            int countBattleParticipants = Convert.ToInt16(Console.ReadLine());

            // create lists
            List<CombatVehicle> teamRed = new();
            List<CombatVehicle> teamBlue = new();

            // fill lists
            teamRed = FillList(countBattleParticipants);
            teamBlue = FillList(countBattleParticipants);

            // Battle construct
            Battle battle = new(teamRed, teamBlue);
            battle.ConstructBattleground();
            Console.WriteLine($"{battle.Winner} team are winners!");
        }
    }
}