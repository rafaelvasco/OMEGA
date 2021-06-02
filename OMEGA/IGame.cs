using System;

namespace OMEGA
{
    public interface IGame : IDisposable
    {
        double UpdateRate {get;set;}

        bool UnlockFrameRate {get;set;}

        GameInfo GameInfo {get;}

        void Run();

        void Exit();

        void Tick();

        void Load();

        void Update(float dt);

        void FixedUpdate(float dt);

        void OnActivated();

        void OnDeactivated();

        void OnDisplayResize();

        void Draw(Canvas2D canvas, float dt);

    }
}
