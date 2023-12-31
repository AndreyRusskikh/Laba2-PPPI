﻿using System.Collections;
using System.ComponentModel;

namespace Laba3
{
    public delegate void WorkersChangedHandler<TKey>(object source, WorkersChangedEventArgs<TKey> args);


    public enum Update
    {
        Add,
        Replace,
        Property
    }
    public class StationWorker : Person, INotifyPropertyChanged, IDateAndCopy 
    {
        //public delegate INotifyPropertyChanged PropChange(string name);

        private string specialiazacia;
        private Rank categoria;
        private int stage;
        internal List<Licenses> licensesSert;
        internal List<Client> clients;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public Person PersonInfo
        {
            get
            {
                return new Person(name, LastName, DateOfBirth);
            }
            set
            {
                name = value.Name;
                LastName = value.LastName;
                DateOfBirth = value.DateOfBirth;
            }
        }

        
        public string Specialiazacia
        {
            get { return specialiazacia; }
            set { specialiazacia = value; OnPropertyChanged(nameof(Specialiazacia)); }
        }

        public Rank Categoria
        {
            get { return categoria; }
            set { categoria = value; OnPropertyChanged(nameof(Categoria));}
        }

        public int Stage
        {
            get { return stage; }
            set
            {
                if (value < 0 || value > 100)
                    throw new ArgumentOutOfRangeException("Stage", $"Значення властивості Stage повинно бути в межах від 0 до 100. Передано значення: {value}");

                stage = value;
            }
        }


        public List<Client> Clients
        {
            get { return clients; }
            set { clients = value; }
        }

        public StationWorker(Person person, string specializacia, Rank categ, int stages)
            : base(person.Name, person.LastName, person.DateOfBirth)
        {
            this.specialiazacia = specializacia;
            this.categoria = categ;
            this.stage = stages;
        }



        //public DevTeam() : this("Default Project", "Default Organization", 1, DateTime.Now)
        public StationWorker()
         : base("Pierto", "Feraro", new DateTime(1990, 12, 31))
        {
            specialiazacia = "Mehanick";
            categoria = Rank.Second;
            stage = 10;
        }

        
        public Client this[int cl]
        {
            get
            {
                return clients[cl];
            }
            set
            {
                clients[cl] = value;
            }
        }
        public Client this[string mist]
        {
            get
            {
                if (!string.IsNullOrEmpty(mist))
                {
                    for (int i = 0; i < clients.Count; i++)
                    {
                        if (clients[i].typemist == mist)
                            return clients[i];
                    }
                    throw new KeyNotFoundException("Car does not exist");
                }
                else
                {
                    Console.WriteLine("Mist is null or empty");
                    throw new ArgumentNullException(nameof(mist));
                }
            }
        }

        public Licenses? LicenseNewData
        {
            get
            {
                if (licensesSert.Count == 0)
                {
                    return null;
                }
                Licenses highestPriorityTask = licensesSert[0];

                foreach (Licenses task in licensesSert)
                {
                    if (task.DataSert < highestPriorityTask.DataSert)
                    {
                        highestPriorityTask = task;
                    }
                }
                return highestPriorityTask;
            }
        }

        public DateTime Date { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        // Метод добавления лицензий

        public void AddLicense(params Licenses[] newLicenses)
        {
            if (licensesSert == null)
                licensesSert = new List<Licenses>();
            licensesSert.AddRange(newLicenses);
        }
        public void AddCliens(List<Client> client)
        {
            if (clients == null)
                clients = new List<Client>();
            clients.AddRange(client);
        }
        // Метод вывода задач
        public override string ToString()
        {
            string tasksString = "";
            if(licensesSert == null)
                licensesSert = new List<Licenses>();
            foreach (Licenses task in licensesSert)
            {
                tasksString += $"\n\t{task}";
            }
            string clientString = "";

            if (clients == null)
                clients = new List<Client>();
            
            foreach (Client cl1 in clients)
            {
                clientString += $"\n\t{cl1}";
            }
            return $"Name: {this.Name}\nLast Name: {this.LastName}\nDate of birth: {this.dateOfBirth}\nSpecialiazacia: {specialiazacia}\nCategoria: {categoria}\nStage: {stage}\nLicense:{tasksString}\nClient:{clientString} \n";


        }

        //Метод вывода проектов
        public virtual string ToShortString()
        {
           

            if (clients == null)
                clients = new List<Client>();
            
            return $"WorkerName: {this.Name}\nSpecialiazacia: {specialiazacia}\nCategoria {categoria}\nStage: {stage}\n All client: {clients.Count}\n";
        }

        public override object DeepCopy()
        {
            StationWorker copy = new StationWorker();
            copy.specialiazacia = this.specialiazacia;
            copy.LastName = this.LastName;
            copy.categoria = this.categoria;
            copy.stage = this.stage;

            if (this.licensesSert != null)
            {
                copy.licensesSert = new List<Licenses>();
                foreach (Licenses lic in this.licensesSert)
                {
                    copy.licensesSert.Add((Licenses)lic.DeepCopy());
                }
            }

            if (this.clients != null)
            {
                copy.clients = new List<Client>();
                foreach (Client cl in this.clients)
                {
                    copy.clients.Add((Client)cl.DeepCopy());
                }
            }



            return copy;
        }


        public IEnumerator GetEnumerator()
        {
            return new StationWorkerEnumerator(this);
        }

        public IEnumerable<Client> GetOldRepairClients()
        {
            DateTime oneMonthAgo = DateTime.Now.AddMonths(-1);
            foreach (Client client in clients)
            {
                if (client.dateSto < oneMonthAgo)
                {
                    yield return client;
                }
            }
        }
        public IEnumerable<Client> GetRepairClientsToday()
        {
            DateTime today = DateTime.Today;
            foreach (Client client in clients)
            {
                if (client.dateSto == today)
                {
                    yield return client;
                }
            }
        }
        public IEnumerable<Client> GetClientsByMist(string mist)
        {
            foreach (Client client in clients)
            {
                if (client.typemist == mist)
                {
                    yield return client;
                }
            }
        }


    }
    public class StationWorkerEnumerator : IEnumerator
    {
        private StationWorker stationWorker;
        private int currentIndex;

        public StationWorkerEnumerator(StationWorker sw)
        {
            stationWorker = sw;
            currentIndex = -1;
        }

        public bool MoveNext()
        {
            while (++currentIndex < stationWorker.licensesSert.Count)
            {
                if (stationWorker.licensesSert[currentIndex].DataSert.Year <= 2010)
                {
                    return true;
                }
            }
            return false;
        }

        public void Reset()
        {
            currentIndex = -1;
        }

        public object Current
        {
            get { return stationWorker.licensesSert[currentIndex]; }
        }
    }
}
