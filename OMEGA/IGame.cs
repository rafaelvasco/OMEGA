using System;

namespace OMEGA
{
    public interface IGame : IDisposable
    {
        double UpdateRate {get;set;}

        bool UnlockFrameRate {get;set;}

        void Run();

        void Exit();

        void Tick();

        void Load();

        void VariableUpdate(float dt);

        void FixedUpdate(float dt);

        void OnActivated();

        void OnDeactivated();

        void OnResize();

        void Draw(float dt);

    }
}
