using System;

namespace OMEGA
{
    public abstract class Game : IGame
    {
        public GameInfo GameInfo {get; private set;}

        public double UpdateRate { get; set; } = 60.0;
        public bool UnlockFrameRate { get; set; } = true;

        const int time_history_count = 4;

        private bool running = false;
        private bool resync = true;
        private int update_mult = 1;
        private double fixed_deltatime;
        private double desired_frametime;
        private double vsync_max_error;
        private double[] snap_freqs;
        private double[] time_averager;

        private double prev_frame_time;
        private double frame_accum = 0;


        public Game()
        {
            GameInfo = ResourceLoader.LoadGameInfo();

            Engine.Init(this);

            fixed_deltatime = 1.0 / UpdateRate;
            desired_frametime = Platform.GetPerformanceFrequency() / UpdateRate;
            vsync_max_error = Platform.GetPerformanceFrequency() * 0.0002;

            double time_60hz = Platform.GetPerformanceFrequency() / 60;
            snap_freqs = new double[]{
                time_60hz,      //60fps
                time_60hz*2,    //30fps
                time_60hz*3,    //20fps
                time_60hz*4,    //15fps
                (time_60hz+1)/2 //120fps
            };

            time_averager = new double[time_history_count];

            for (int i = 0; i < time_history_count; i++)
            {
                time_averager[i] = desired_frametime;
            }
        }

        public void Run()
        {
            if (running)
            {
                return;
            }

            running = true;

            Tick();

            Engine.ShowWindow(true);

            prev_frame_time = Platform.GetPerformanceCounter();
            frame_accum = 0;

            while (running)
            {
                Tick();
            }
        }

        public void Exit()
        {
            running = false;
        }

        public void Tick()
        {
            double current_frame_time = Platform.GetPerformanceCounter();

            double delta_time = current_frame_time - prev_frame_time;

            prev_frame_time = current_frame_time;

            // Handle unexpected timer anomalies (overflow, extra slow frames, etc)
            if (delta_time > desired_frametime * 8)
            {
                delta_time = desired_frametime;
            }

            if (delta_time < 0)
            {
                delta_time = 0;
            }

            // VSync Time Snapping
            for (int i = 0; i < snap_freqs.Length; ++i)
            {
                var snap_freq = snap_freqs[i];

                if (Math.Abs(delta_time - snap_freq) < vsync_max_error)
                {
                    delta_time = snap_freq;
                    break;
                }
            }

            // Delta Time Averaging
            for (int i = 0; i < time_history_count - 1; ++i)
            {
                time_averager[i] = time_averager[i + 1];
            }

            time_averager[time_history_count - 1] = delta_time;

            delta_time = 0;

            for (int i = 0; i < time_history_count; ++i)
            {
                delta_time += time_averager[i];
            }

            delta_time /= time_history_count;

            // Add To Accumulator
            frame_accum += delta_time;

            // Spiral of Death Protection
            if (frame_accum > desired_frametime * 8)
            {
                resync = true;
            }

            // Timer Resync Requested
            if (resync)
            {
                frame_accum = 0;
                delta_time = desired_frametime;
                resync = false;
            }

            // Process Events

            Engine.ProcessEvents();

            // Unlocked Frame Rate, Interpolation Enabled
            if (UnlockFrameRate)
            {
                double consumed_delta_time = delta_time;

                while (frame_accum >= desired_frametime)
                {
                    FixedUpdate((float)fixed_deltatime);

                    // Cap Variable Update's dt to not be larger than fixed update, 
                    // and interleave it (so game state can always get animation frame it needs)
                    if (consumed_delta_time > desired_frametime)
                    {
                        Update((float)fixed_deltatime);
                        consumed_delta_time -= desired_frametime;
                    }

                    frame_accum -= desired_frametime;
                }

                Update((float)(consumed_delta_time / Platform.GetPerformanceFrequency()));

                if (Engine.Canvas.NeedsResetDisplay)
                {
                    Engine.Canvas.HandleDisplayChange();
                    OnDisplayResize();
                }

                Draw(Engine.Canvas, (float)(frame_accum / desired_frametime));

                Engine.Canvas.Frame();

            }
            // Locked Frame Rate, No Interpolation
            else
            {
                while (frame_accum >= desired_frametime * update_mult)
                {
                    for (int i = 0; i < update_mult; ++i)
                    {
                        FixedUpdate((float)fixed_deltatime);
                        Update((float)fixed_deltatime);

                        frame_accum -= desired_frametime;
                    }
                }

                if (Engine.Canvas.NeedsResetDisplay)
                {
                    Engine.Canvas.HandleDisplayChange();
                }

                Draw(Engine.Canvas, 1.0f);

                Engine.Canvas.Frame();
            }

        }

        public abstract void Load();

        public virtual void Unload() { }

        public virtual void Update(float dt) { }

        public virtual void FixedUpdate(float dt) { }

        public abstract void Draw(Canvas canvas, float dt);

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
        
        public virtual void OnDisplayResize() {}
    }
}
