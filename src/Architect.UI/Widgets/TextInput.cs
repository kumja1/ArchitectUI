using System.Drawing;

namespace Architect.UI;

class TextInput : Widget
{
    public string Text
    {
        get => field;
        set => SetProperty(ref field, value);
    }

    public TextInput()
    {
    
    }


}



