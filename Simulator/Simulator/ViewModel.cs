using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Simulator
{
    public enum ButtonState
    { 
        Off,
        Request,
        Blink,
        Flash,
    }

    public enum ProcessState
    {
        Off,
        OpenValve,//
        SecondSensorON,//b2
        SecondSensorON3Sec,
        FirstSensorON,//b1
        StopValve,//b3
        PumpFaileure,
        BlinkOn,
        BlinkOff,     
        AutoRed,
        AutoYellow,
        AutoGreen,         
    }
    
    class ViewModel : INotifyPropertyChanged
    {

        // foarte important in cadrul aplicatiilor de tip WPF este ca in view model sa fie implementata interfata INotifyPropertyChanged
        // implementarea acestei interfete face ca atunci cand avem in codul xaml controale care fac bind pe anumite proprietati ale viewmodel-ului
        // sa fie actualizate automat atunci cand proprietatiile pe care se face bind sunt actualizate in view model.

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string prop)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        // acest flag este utilizat pentru a trimite date catre alte aplicatii, daca este setat pe true, iar daca este setat pe false face ca simulator sa functioneze ca o aplicatie simpla
        // care nu comunica cu alte aplicatii
        public bool _isSendingData = true;

        // backgroundworker-ul si timer-ul sunt folosite pentru a simula procesul adica pentru a realiza o tranzitie intre anumite stari la anumite perioade de timp
        private BackgroundWorker _worker = new BackgroundWorker();
        private System.Timers.Timer _timer = new System.Timers.Timer();
        
        private Comm.Sender _sender;
        private bool _setNextState = false;
        private ProcessState _nextState;

        public ViewModel() {}

        public void SendData()
        {
            if(_isSendingData)
            {
                _sender.Send(Convert.ToByte(_currentStateOfTheProcess));
            }
        }


        //  de aici in jos este implementata partea de simulare a procesului
        #region Simulator
        public void Init()
        {
            if(_isSendingData)
            {
                _sender = new Comm.Sender("127.0.0.1", 3000);
            }
            
            _timer.Elapsed += _timer_Elapsed;
            _worker.DoWork += _worker_DoWork;
            _worker.RunWorkerAsync();
        }

        private ProcessState _currentStateOfTheProcess = ProcessState.Off;
        public ProcessState CurrentStateOfTheProcess
        {
            get => _currentStateOfTheProcess;
            set
            {
                // urmatoarele doua linii de cod opresc timerul deoarece a avut loc o tranzitie de stare
                _setNextState = false;
                _timer.Stop();

                // dupa care se actualizeaza starea curenta si se trimit actualizarile de stare mai departe
                _currentStateOfTheProcess = value;
                SendData();
            }
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            CurrentStateOfTheProcess = _nextState;
        }

        private void RaiseTimerEvent(ProcessState NextStateOfTheProcess, int TimeInterval)
        {
            if(!_setNextState)
            {
                _setNextState = true;
                _nextState = NextStateOfTheProcess;
                _timer.Interval = TimeInterval;
                _timer.Start();
            }
        }

        private void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // idea de baza a simulatorului este urmatoarea: backgroundworker-ul evalueza tot la 100 de milisecunde starea curenta a procesului 
            // si seteaza in viewmodel variabilele care actualizeaza UI-ul dupa care utilizand RaiseTimerEvent(NextProcessState, 2000) determina o tranzitie de stare peste un
            // interval de timp specificat de al doilea paramteru al acestei metode

            while (true)
            {              
                ProcessNextState(CurrentStateOfTheProcess);
                System.Threading.Thread.Sleep(100);
            }
        }

        // aceasta metoda proceseaza starea curenta, in funtie de valoarea acesteia seteaza anumite variabile care actualizeaza UI-ul 
        // si utilizand metoda RaiseTimerEvent(NextState, 2000) declanseaza o tranzitie viitoare de stare
        public void ProcessNextState(ProcessState CurrentState)
        {
            switch (CurrentState)
            {
                case ProcessState.Off://7
                    IsFirstPumpON = false;
                    IsSecondSensorON = false;
                    IsGreenForCar = false;
                    IsRedForPeople = false;
                    IsGreenForPeople = false;
                    IsSecondPumpON = false;
                    IsFirstSensorON = false;
                    IsSecondLevel = false;
                    IsFirstLevel = false;
                    IsThirdPumpON = false;
                    IsThirdSensorON = false;
                    IsThirdLevel = false;
                    IsValveON = false;


                    //RaiseTimerEvent(ProcessState.Off, 2000);
                   
                    break;
                case ProcessState.BlinkOn://s0
                    IsFirstPumpON = false;
                    IsSecondSensorON = false;
                    IsGreenForCar = false;
                    IsRedForPeople = false;
                    IsGreenForPeople = false;
                    IsSecondPumpON = false;
                    IsFirstSensorON = false;
                    IsSecondLevel = false;
                    IsFirstLevel = false;
                    IsThirdPumpON = false;
                    IsThirdSensorON = false;
                    IsThirdLevel = false;
                    IsValveON = false;




                    RaiseTimerEvent(ProcessState.Off, 2000);

                    break;


                case ProcessState.OpenValve://pune primul nivel apa
                    IsFirstLevel = true;
                    IsFirstPumpON = false;
                    IsSecondSensorON = false;
                    IsGreenForCar = false;
                    IsRedForPeople = false;
                    IsGreenForPeople = false;
                    IsSecondPumpON = false;
                    IsFirstSensorON = false;
                    IsSecondLevel = false;
                    IsThirdPumpON = false;
                    IsThirdSensorON = false;
                    IsThirdLevel = false;
                    IsValveON = true;



                    RaiseTimerEvent(ProcessState.SecondSensorON, 3000);

                    break;
                case ProcessState.SecondSensorON://pune al doilea nivel apa + pompa m1
                    IsFirstLevel = true;
                    IsSecondLevel = true;
                    IsSecondPumpON = false;
                    IsFirstPumpON = true;
                    IsSecondSensorON = true;
                    IsGreenForCar = false;
                    IsRedForPeople = false;
                    IsGreenForPeople = false;
                    IsSecondPumpON = false;
                    IsFirstSensorON = false;
                    IsThirdPumpON = false;
                    IsThirdSensorON = false;
                    IsThirdLevel = false;
                    IsValveON = true;




                    RaiseTimerEvent(ProcessState.SecondSensorON3Sec, 3000);

                    break;
                    
                case ProcessState.SecondSensorON3Sec://porneste pompa m2
                    IsFirstLevel = true;
                    IsSecondLevel = true;
                    IsFirstPumpON = true;
                    IsSecondPumpON = true;
                    IsSecondSensorON = true;
                    IsGreenForCar = false;
                    IsRedForPeople = false;
                    IsGreenForPeople = false;
                    IsFirstSensorON = false;
                    IsThirdPumpON = false;
                    IsThirdSensorON = false;
                    IsThirdLevel = false;
                    IsValveON = true;



                    RaiseTimerEvent(ProcessState.StopValve, 3000);

                    break;

                case ProcessState.FirstSensorON://opreste pompele daca scada subi nivelul senzoruluoi b1
                    IsFirstLevel = false;
                    IsSecondLevel = false;
                    IsFirstPumpON = false;
                    IsSecondPumpON = false;
                    IsSecondSensorON = false;
                    IsGreenForCar = false;
                    IsRedForPeople = false;
                    IsGreenForPeople = false;
                    IsFirstSensorON = true;
                    IsThirdPumpON = false;
                    IsThirdSensorON = false;
                    IsThirdLevel = false;
                    IsValveON = true;


                    RaiseTimerEvent(ProcessState.BlinkOn, 3000);

                    break;

                case ProcessState.PumpFaileure://o poma cedeaza activeaza pompa 3
                    IsFirstLevel = true;
                    IsSecondLevel = true;
                    IsFirstPumpON = true;
                    IsSecondPumpON = false;
                    IsSecondSensorON = true;
                    IsGreenForCar = false;
                    IsRedForPeople = false;
                    IsGreenForPeople = false;
                    IsFirstSensorON = false;
                    IsThirdPumpON = true;
                    IsThirdSensorON = false;
                    IsThirdLevel = false;
                    IsValveON = true;


                    RaiseTimerEvent(ProcessState.Off, 3000);

                    break;

                case ProcessState.StopValve://vas plin inchideaza valva
                    IsFirstLevel = true;
                    IsSecondLevel = true;
                    IsThirdLevel = true;

                    IsFirstPumpON = true;
                    IsSecondPumpON = true;
                    IsSecondSensorON = true;
                    IsGreenForCar = false;
                    IsRedForPeople = false;
                    IsGreenForPeople = false;
                    IsFirstSensorON = false;
                    IsThirdPumpON = false;
                    IsThirdSensorON = true;
                    IsValveON = false;


                    RaiseTimerEvent(ProcessState.PumpFaileure, 3000);

                    break;




                //case ProcessState.BlinkOff:
                //    IsFirstPumpON = false;
                //    IsSecondSensorON = false;
                //    IsGreenForCar = false;
                //    IsRedForPeople = false;
                //    IsGreenForPeople = false;
                    
                //    RaiseTimerEvent(ProcessState.BlinkOn, 2000);
                   
                //    break;               
                //case ProcessState.AutoRed:
                //    IsFirstPumpON = true;
                //    IsSecondSensorON = false;
                //    IsGreenForCar = false;
                //    IsRedForPeople = false;
                //    IsGreenForPeople = true;

                //    RaiseTimerEvent(ProcessState.AutoGreen, 5000);

                //    break;
                //case ProcessState.AutoYellow:
                //    IsFirstPumpON = false;
                //    IsSecondSensorON = true;
                //    IsGreenForCar = false;
                //    IsRedForPeople = true;
                //    IsGreenForPeople = false;
                    
                //    RaiseTimerEvent(ProcessState.AutoRed, 3000);

                //    break;
                //case ProcessState.AutoGreen:
                //    IsFirstPumpON = false;
                //    IsSecondSensorON = false;
                //    IsGreenForCar = true;
                //    IsRedForPeople = true;
                //    IsGreenForPeople = false;

                //    RaiseTimerEvent(ProcessState.AutoYellow, 10000);

                //    break;                              
            }

        }

        // aceasta metoda este apelata direct de pe UI, pentru a forta o tranzitie de stare a procesului
        // tranzitia de stare forteaza oprirea timer-ului care ar trebui sa declanseze urmatoarea tranzitie de stare
        // si seteaza ca si stare curenta starea primita ca si parametru
        // dupa care backgroundworker-ul o sa se ocupe de tot si va declansa urmatoarea tranzitie de stare care urmeaza
        public void ForceNextState(ProcessState NextState)
        {
            CurrentStateOfTheProcess = NextState;
        }
        #endregion

        // de aici in jos sunt proprietatiile pe care le folosim pentru a actualiza UI-ul
        #region UI_updates

        



            private bool _IsFirstSensorON;
        public bool IsFirstSensorON
        {
            get
            {
                return _IsFirstSensorON;
            }
            set
            {
                _IsFirstSensorON = value;
                OnPropertyChanged(nameof(IsFirstSensorVisible));
            }
        }

        public System.Windows.Visibility IsFirstSensorVisible
        {
            get
            {
                if (_IsFirstSensorON)
                {
                    return System.Windows.Visibility.Visible;
                }
                else
                {
                    return System.Windows.Visibility.Hidden;
                }
            }
        }


        
            private bool _IsThirdPumpON;
        public bool IsThirdPumpON
        {
            get
            {
                return _IsThirdPumpON;
            }
            set
            {
                _IsThirdPumpON = value;
                OnPropertyChanged(nameof(IsThirdPumpVisible));
            }
        }

        public System.Windows.Visibility IsThirdPumpVisible
        {
            get
            {
                if (_IsThirdPumpON)
                {
                    return System.Windows.Visibility.Visible;
                }
                else
                {
                    return System.Windows.Visibility.Hidden;
                }
            }
        }

        private bool _IsSecondPumpON;
        public bool IsSecondPumpON
        {
            get
            {
                return _IsSecondPumpON;
            }
            set
            {
                _IsSecondPumpON = value;
                OnPropertyChanged(nameof(IsSecondPumpVisible));
            }
        }

        public System.Windows.Visibility IsSecondPumpVisible
        {
            get
            {
                if (_IsSecondPumpON)
                {
                    return System.Windows.Visibility.Visible;
                }
                else
                {
                    return System.Windows.Visibility.Hidden;
                }
            }
        }


        private bool _IsFirstPumpON;
        public bool IsFirstPumpON 
        {
            get
            {
                return _IsFirstPumpON;
            }
            set
            {
                _IsFirstPumpON = value;
                OnPropertyChanged(nameof(IsFirstPumpVisible));
            }
        }

        public System.Windows.Visibility IsFirstPumpVisible
        {
            get
            {
                if(_IsFirstPumpON)
                {
                    return System.Windows.Visibility.Visible;
                }
                else
                {
                    return System.Windows.Visibility.Hidden;
                }
            }
        }

        private bool _IsSecondSensorON;
        public bool IsSecondSensorON
        {
            get
            {
                return _IsSecondSensorON;
            }
            set
            {
                _IsSecondSensorON = value;
                OnPropertyChanged(nameof(IsSecondSensorVisible));
            }
        }

        public System.Windows.Visibility IsSecondSensorVisible
        {
            get
            {
                if (_IsSecondSensorON)
                {
                    return System.Windows.Visibility.Visible;
                }
                else
                {
                    return System.Windows.Visibility.Hidden;
                }
            }
        }

        private bool _isGreenForCar;
        public bool IsGreenForCar
        {
            get
            {
                return _isGreenForCar;
            }
            set
            {
                _isGreenForCar = value;
                OnPropertyChanged(nameof(IsGreenForCarsVisible));
            }
        }

        public System.Windows.Visibility IsGreenForCarsVisible
        {
            get
            {
                if (_isGreenForCar)
                {
                    return System.Windows.Visibility.Visible;
                }
                else
                {
                    return System.Windows.Visibility.Hidden;
                }
            }
        }

        private bool _isGreenForPeople;
        public bool IsGreenForPeople
        {
            get
            {
                return _isGreenForPeople;
            }
            set
            {
                _isGreenForPeople = value;
                OnPropertyChanged(nameof(IsGreenForPeopleVisible));
            }
        }

        public System.Windows.Visibility IsGreenForPeopleVisible
        {
            get
            {
                if (_isGreenForPeople)
                {
                    return System.Windows.Visibility.Visible;
                }
                else
                {
                    return System.Windows.Visibility.Hidden;
                }
            }
        }

        






        private bool _isRedForPeople;
        public bool IsRedForPeople
        {
            get
            {
                return _isRedForPeople;
            }
            set
            {
                _isRedForPeople = value;
                OnPropertyChanged(nameof(IsRedForPeopleVisible));
            }
        }




        public System.Windows.Visibility IsRedForPeopleVisible
        {
            get
            {
                if (_isRedForPeople)
                {
                    return System.Windows.Visibility.Visible;
                }
                else
                {
                    return System.Windows.Visibility.Hidden;
                }
            }
        }

        private bool _IsFirstLevel;
        public bool IsFirstLevel
        {
            get
            {
                return _IsFirstLevel;
            }
            set
            {
                _IsFirstLevel = value;
                OnPropertyChanged(nameof(IsFirstLevelVisible));
            }
        }
        public System.Windows.Visibility IsFirstLevelVisible
        {
            get
            {
                if (_IsFirstLevel)
                {
                    return System.Windows.Visibility.Visible;
                }
                else
                {
                    return System.Windows.Visibility.Hidden;
                }
            }
        }


        private bool _IsSecondLevel;
        public bool IsSecondLevel
        {
            get
            {
                return _IsSecondLevel;
            }
            set
            {
                _IsSecondLevel = value;
                OnPropertyChanged(nameof(IsSecondLevelVisible));
            }
        }
        public System.Windows.Visibility IsSecondLevelVisible
        {
            get
            {
                if (_IsSecondLevel)
                {
                    return System.Windows.Visibility.Visible;
                }
                else
                {
                    return System.Windows.Visibility.Hidden;
                }
            }
        }
        



                 private bool _IsThirdLevel;
        public bool IsThirdLevel
        {
            get
            {
                return _IsThirdLevel;
            }
            set
            {
                _IsThirdLevel = value;
                OnPropertyChanged(nameof(IsThirdLevelVisible));
            }
        }
        public System.Windows.Visibility IsThirdLevelVisible
        {
            get
            {
                if (_IsThirdLevel)
                {
                    return System.Windows.Visibility.Visible;
                }
                else
                {
                    return System.Windows.Visibility.Hidden;
                }
            }
        }



        private bool _IsThirdLevelON;
        public bool IsThirdSensorON
        {
            get
            {
                return _IsThirdLevelON;
            }
            set
            {
                _IsThirdLevelON = value;
                OnPropertyChanged(nameof(IsThirdSensorVisible));
            }
        }
        public System.Windows.Visibility IsThirdSensorVisible
        {
            get
            {
                if (_IsThirdLevelON)
                {
                    return System.Windows.Visibility.Visible;
                }
                else
                {
                    return System.Windows.Visibility.Hidden;
                }
            }
        }





        private bool _IsValveON;
        public bool IsValveON
        {
            get
            {
                return _IsValveON;
            }
            set
            {
                _IsValveON = value;
                OnPropertyChanged(nameof(IsValveVisible));
            }
        }
        public System.Windows.Visibility IsValveVisible
        {
            get
            {
                if (_IsValveON)
                {
                    return System.Windows.Visibility.Visible;
                }
                else
                {
                    return System.Windows.Visibility.Hidden;
                }
            }
        }



        #endregion
    }
}
