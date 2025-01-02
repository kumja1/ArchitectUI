
namespace Architect.Common.Interfaces;

    public interface IRenderManager
    {
        void Tick();
        void AddDirtyWidget(IWidget widget);
        void RemoveDirtyWidget(IWidget widget);
    }
