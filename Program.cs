using System.ComponentModel;
using System.Runtime.ConstrainedExecution;
using System.Text;
using Laba3;


namespace Laba3
{
    public interface IDateAndCopy
    {
        object DeepCopy();
        DateTime Date { get; set; }
    }


    public class Licenses
    {
        public string OrganisationName { get; set; }
        public string Cvalification { get; set; }
        public DateTime DataSert { get; set; }

        public Licenses(string name, string cvalif, DateTime datasert)
        {
            OrganisationName = name;
            Cvalification = cvalif;
            DataSert = datasert;
        }

        public Licenses()
        {
            OrganisationName = "Default Task";
            Cvalification = "Expert";
            DataSert = new DateTime(2000, 1, 1);
        }

        public override string ToString()
        {
            return $"{OrganisationName} {Cvalification} {DataSert}";
        }
        public object DeepCopy()
        {
            Licenses copy = new Licenses();
            copy.OrganisationName = this.OrganisationName;
            copy.Cvalification = this.Cvalification;
            copy.DataSert = this.DataSert;
            return copy;
        }

    }


    class Program
    {
        static void Main()

        {
            Console.OutputEncoding = Encoding.UTF8;
            
            string KeyMethod(StationWorker worker)
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 23 + (worker.Specialiazacia?.GetHashCode() ?? 0);
                    hash = hash * 23 + worker.Categoria.GetHashCode();
                    hash = hash * 23 + worker.Stage.GetHashCode();
                    return hash.ToString();
                }
            }

            KeySelector<string> keySelector = KeyMethod;

            StationWorkerCollection<string> stationWorkerCollection = new StationWorkerCollection<string>(keySelector);
            StationWorkerCollection<string> stationWorkerCollection2 = new StationWorkerCollection<string>(keySelector);

            stationWorkerCollection.CollectionName = "stationWorkerCollection";
            stationWorkerCollection2.CollectionName = "stationWorkerCollection2";

            Listener listener = new Listener();
            stationWorkerCollection.WorkersChanged += listener.WorkersChanged;
            stationWorkerCollection2.WorkersChanged += listener.WorkersChanged;

            Person person1 = new("Franchesko", "Escorbany", new DateTime(1996, 3, 9));
            Person person2 = new("Alias", "Benisio", new DateTime(1998, 7, 11));
            Person person3 = new("Mary", "Smith", new DateTime(1990, 12, 31));
            Person person4 = new("John", "Doe", new DateTime(1995, 1, 15));
            Person person5 = new("Eliot", "Spenser", new DateTime(1985, 4, 12));

            StationWorker worker = new StationWorker(person1, "Ingener", Rank.First, 2);
            StationWorker stationWorker = new StationWorker(person2, "Tehnic", Rank.High, 14);
            StationWorker stationWorker1 = new StationWorker(person3, "Ingener-Tehnic", Rank.Second, 9);
            StationWorker stationWorker2 = new StationWorker(person4, "Locksmith", Rank.Second, 5);
            StationWorker stationWorker3 = new StationWorker(person5, "Mechanic", Rank.Second, 4);
            StationWorker[] stationWorkers = new StationWorker[] { worker, stationWorker, stationWorker1, stationWorker2 };

           

            Licenses[] licenses = new Licenses[]
           {
                new Licenses("Licens1", "Ingener", new DateTime(1990, 3, 15)),
                new Licenses("Licens2", "Tehnic", new DateTime(1995, 11, 12)),
                new Licenses("Licens3", "Master", new DateTime(1997, 5, 20)),
                new Licenses("Licens4", "Margarita", new DateTime(1999, 8, 18))
           };
            List<Client> clients = new List<Client>()
            {
                new Client(person1, "Break motor", new DateTime(2015, 5, 21)) {},
                new Client(person3, "Break motor", DateTime.Today),
                new Client(person4, "Break motor", new DateTime(2017, 7, 13)),
                new Client(person5, "Break motor", new DateTime(2019, 1, 27))

            };

            stationWorkerCollection.AddWorker(stationWorkers);
            stationWorkerCollection2.AddWorker(stationWorkers);
            foreach (var sad in stationWorkerCollection.WorkerDictionary)
            {
                sad.Value.AddLicense(licenses);
                sad.Value.AddCliens(clients);
            }

            stationWorkerCollection.ChangeRank(KeyMethod(stationWorker1), Rank.First);
            stationWorkerCollection.ChangeSpecialization(KeyMethod(worker), "Ingener-Tehnic");
            stationWorkerCollection.Replace(stationWorker2, stationWorker3);

            stationWorker2.Name = "Change Name";
            stationWorker2.Stage = 95;

            Console.WriteLine("=================== Данные из объекта Listener ====================");
            Console.WriteLine(listener.ToString());

            Console.WriteLine("=================== Данные Max Stage ====================");
            
            int maxStage = stationWorkerCollection.MaxStage;
            Console.WriteLine(maxStage);
            
            Console.WriteLine("=================== Данные сортировки по квалификации(метод) ====================");

            IEnumerable<KeyValuePair<string, StationWorker>> keyValuePairs = stationWorkerCollection.RankForm(Rank.First);
            foreach (var sf in keyValuePairs)
            {
                Console.WriteLine(sf.Value.ToShortString());
            }

          
            Console.WriteLine("=================== Данные сортировки по квалификации(свойство) ====================");

            IEnumerable<IGrouping<Rank, KeyValuePair<string, StationWorker>>> keyValuePairs2 = stationWorkerCollection.GroupedByRank;

            foreach (var group in keyValuePairs2)
            {
                
                foreach (var pair in group)
                {
                    Console.WriteLine(pair.Value.ToShortString());
                }
            }


            Console.WriteLine("=================== Данные сортировки по специализации ====================");

            List<StationWorker> listStat = stationWorkerCollection.GetWorkersBySpecialization("Ingener-Tehnic");

            foreach (var sf in listStat)
            {
                Console.WriteLine(sf.ToShortString());
            }


        }
    }
}