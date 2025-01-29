namespace Architect.Common.Interfaces;
    public interface IFocusable
    {
        bool IsFocused { get; set; }

        void SetFocus(bool focus);
        
    }
