using System;

namespace OMEGA
{
    public abstract class Game : IGame
    {
        public GameInfo GameInfo { get; }

        public double UpdateRate { get; set; } = 60.0;
        public bool UnlockFrameRate { get; set; } = true;

        private const int TimeHistoryCount = 4;
        private const int UpdateMult = 1;
        private bool _running;
        private bool _resync = true;
        private readonly double _fixedDeltatime;
        private readonly double _desiredFrametime;
        private readonly double _vsyncMaxError;
        private readonly double[] _snapFreqs;
        private readonly double[] _timeAverager;

        private double _prevFrameTime;
        private double _frameAccum;


        protected Game()
        {
            GameInfo = ResourceLoader.LoadGameInfo();

            Engine.Init(this);

            _fixedDeltatime = 1.0 / UpdateRate;
            _desiredFrametime = Platform.GetPerformanceFrequency() / UpdateRate;
            _vsyncMaxError = Platform.GetPerformanceFrequency() * 0.0002;

            double time_60Hz = Platform.GetPerformanceFrequency() / 60;
            _snapFreqs = new[]
            {
                time_60Hz, //60fps
                time_60Hz * 2, //30fps
                time_60Hz * 3, //20fps
                time_60Hz * 4, //15fps
                (time_60Hz + 1) / 2 //120fps
            };

            _timeAverager = new double[TimeHistoryCount];

            for (int i = 0; i < TimeHistoryCount; i++)
            {
                _timeAverager[i] = _desiredFrametime;
            }
        }

        public void Run()
        {
            if (_running)
            {
                return;
            }

            _running = true;

            Tick();

            Engine.ShowWindow(true);

            _prevFrameTime = Platform.GetPerformanceCounter();
            _frameAccum = 0;

            while (_running)
            {
                Tick();
            }
        }

        public void Exit()
        {
            _running = false;
        }

        public void Tick()
        {
            double current_frame_time = Platform.GetPerformanceCounter();

            double delta_time = current_frame_time - _prevFrameTime;

            _prevFrameTime = current_frame_time;

            // Handle unexpected timer anomalies (overflow, extra slow frames, etc)
            if (delta_time > _desiredFrametime * 8)
            {
                delta_time = _desiredFrametime;
            }

            if (delta_time < 0)
            {
                delta_time = 0;
            }

            // VSync Time Snapping
            for (int i = 0; i < _snapFreqs.Length; ++i)
            {
                var snap_freq = _snapFreqs[i];

                if (Math.Abs(delta_time - snap_freq) < _vsyncMaxError)
                {
                    delta_time = snap_freq;
                    break;
                }
            }

            // Delta Time Averaging
            for (int i = 0; i < TimeHistoryCount - 1; ++i)
            {
                _timeAverager[i] = _timeAverager[i + 1];
            }

            _timeAverager[TimeHistoryCount - 1] = delta_time;

            delta_time = 0;

            for (int i = 0; i < TimeHistoryCount; ++i)
            {
                delta_time += _timeAverager[i];
            }

            delta_time /= TimeHistoryCount;

            // Add To Accumulator
            _frameAccum += delta_time;

            // Spiral of Death Protection
            if (_frameAccum > _desiredFrametime * 8)
            {
                _resync = true;
            }

            // Timer Resync Requested
            if (_resync)
            {
                _frameAccum = 0;
                delta_time = _desiredFrametime;
                _resync = false;
            }

            // Process Events and Input

            Engine.ProcessEvents();
            Engine.ProcessInput();

            // Unlocked Frame Rate, Interpolation Enabled
            if (UnlockFrameRate)
            {
                double consumed_delta_time = delta_time;

                while (_frameAccum >= _desiredFrametime)
                {
                    FixedUpdate((float) _fixedDeltatime);

                    // Cap Variable Update's dt to not be larger than fixed update, 
                    // and interleave it (so game state can always get animation frame it needs)
                    if (consumed_delta_time > _desiredFrametime)
                    {
                        Update((float) _fixedDeltatime);
                        consumed_delta_time -= _desiredFrametime;
                    }

                    _frameAccum -= _desiredFrametime;
                }

                Update((float) (consumed_delta_time / Platform.GetPerformanceFrequency()));

                if (Engine.Canvas.NeedsResetDisplay)
                {
                    Engine.Canvas.HandleDisplayChange();
                    OnDisplayResize();
                }

                Draw(Engine.Canvas, (float) (_frameAccum / _desiredFrametime));

                GraphicsContext.Frame();
            }
            // Locked Frame Rate, No Interpolation
            else
            {
                while (_frameAccum >= _desiredFrametime * UpdateMult)
                {
                    for (int i = 0; i < UpdateMult; ++i)
                    {
                        FixedUpdate((float) _fixedDeltatime);
                        Update((float) _fixedDeltatime);

                        _frameAccum -= _desiredFrametime;
                    }
                }

                if (Engine.Canvas.NeedsResetDisplay)
                {
                    Engine.Canvas.HandleDisplayChange();
                    OnDisplayResize();
                }

                Draw(Engine.Canvas, 1.0f);

                GraphicsContext.Frame();
            }
        }

        public abstract void Load();

        public virtual void Unload()
        {
        }

        public virtual void Update(float dt)
        {
        }

        public virtual void FixedUpdate(float dt)
        {
        }

        public abstract void Draw(Canvas2D canvas, float dt);

        public void Dispose()
        {
            Unload();

            GC.SuppressFinalize(this);
            Engine.Shutdown();
        }

        public virtual void OnActivated()
        {
        }

        public virtual void OnDeactivated()
        {
        }

        public virtual void OnDisplayResize()
        {
        }
    }
}