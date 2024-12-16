using System;
using System.Threading;
using System.Threading.Tasks;

// Name : Abebe Megibar
// Id : UGR/3819/13
// Assignment 1 : (5%)
// Deadline : 12/12/2024

namespace ConsoleStopwatch
{
    // Define delegate for stopwatch events
    public delegate void StopwatchEventHandler(string message);

    // Stopwatch class
    public class Stopwatch
    {
        // Fields
        private TimeSpan _timeElapsed;
        private bool _isRunning;
        private CancellationTokenSource _cancellationTokenSource;

        // Properties
        public TimeSpan TimeElapsed => _timeElapsed;
        public bool IsRunning => _isRunning;

        // Events
        public event StopwatchEventHandler OnStarted;
        public event StopwatchEventHandler OnStopped;
        public event StopwatchEventHandler OnReset;

        // Constructor
        public Stopwatch()
        {
            _timeElapsed = TimeSpan.Zero;
            _isRunning = false;
        }

        // Start method
        public void Start()
        {
            if (_isRunning)
            {
                OnStarted?.Invoke("Stopwatch is already running.");
                return;
            }

            _isRunning = true;
            _cancellationTokenSource = new CancellationTokenSource();

            OnStarted?.Invoke("Stopwatch Started!");

            // Start the ticking task
            Task.Run(() => Tick(_cancellationTokenSource.Token));
        }

        // Stop method
        public void Stop()
        {
            if (!_isRunning)
            {
                OnStopped?.Invoke("Stopwatch is not running.");
                return;
            }

            _isRunning = false;
            _cancellationTokenSource.Cancel();

            OnStopped?.Invoke($"Stopwatch Stopped! Total Time Elapsed: {_timeElapsed}");
        }

        // Reset method
        public void Reset()
        {
            if (_isRunning)
            {
                Stop();
            }

            _timeElapsed = TimeSpan.Zero;

            OnReset?.Invoke("Stopwatch Reset!");
        }

        // Tick method to increment time
        private void Tick(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    Thread.Sleep(1000);
                    _timeElapsed = _timeElapsed.Add(TimeSpan.FromSeconds(1));
                }
            }
            catch (TaskCanceledException)
            {
                // Task was cancelled, gracefully exit
            }
        }
    }

    // Main program
    class Program
    {
        static void Main(string[] args)
        {
            var stopwatch = new Stopwatch();

            // Subscribe to events
            stopwatch.OnStarted += DisplayMessage;
            stopwatch.OnStopped += DisplayMessage;
            stopwatch.OnReset += DisplayMessage;

            Console.WriteLine("====================================================");
            Console.WriteLine("                Console Stopwatch                   ");
            Console.WriteLine("====================================================");
            Console.WriteLine("Commands:");
            Console.WriteLine(" S - Start");
            Console.WriteLine(" T - Stop");
            Console.WriteLine(" R - Reset");
            Console.WriteLine(" Q - Quit");
            Console.WriteLine("====================================================");

            Task.Run(() => DisplayRealTimeTime(stopwatch));

            while (true)
            {
                Console.Write("Enter command (S/T/R/Q): ");
                var input = Console.ReadKey(true).Key;

                switch (input)
                {
                    case ConsoleKey.S:
                        stopwatch.Start();
                        break;

                    case ConsoleKey.T:
                        stopwatch.Stop();
                        break;

                    case ConsoleKey.R:
                        stopwatch.Reset();
                        break;

                    case ConsoleKey.Q:
                        stopwatch.Stop();
                        Console.WriteLine("Thank you for using the Stopwatch. Goodbye!");
                        Console.WriteLine("====================================================");
                        return;

                    default:
                        Console.WriteLine("Invalid command. Please try again.");
                        break;
                }
            }
        }

        // Event handler to display messages
        static void DisplayMessage(string message)
        {
            Console.WriteLine($"[INFO]: {message}");
        }

        // Display real-time stopwatch time in a non-blocking way
        static void DisplayRealTimeTime(Stopwatch stopwatch)
        {
            while (true)
            {
                if (stopwatch.IsRunning)
                {
                    Console.Clear();
                    Console.WriteLine("====================================================");
                    Console.WriteLine("                Console Stopwatch                   ");
                    Console.WriteLine("====================================================");
                    Console.WriteLine($"Current Time Elapsed: {stopwatch.TimeElapsed}");
                    Console.WriteLine("====================================================");
                    Console.WriteLine("Commands:");
                    Console.WriteLine(" S - Start | T - Stop | R - Reset | Q - Quit");
                    Console.WriteLine("====================================================");
                }
                Thread.Sleep(500); // Refresh every 500ms
            }
        }
    }
}




