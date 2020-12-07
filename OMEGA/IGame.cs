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

        void VariableUpdate(float dt);

        void FixedUpdate(float dt);

        void OnActivated();

        void OnDeactivated();

        void Draw(Canvas canvas, float dt);

    }
}
