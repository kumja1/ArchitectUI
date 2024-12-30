using System.Drawing;

namespace Architect.UI;

class InputField : Widget
{
    public string Text
    {
        get => field;
        set => SetProperty(ref field, value);
    }

    public InputField()
    {
    
    }


}



