
using Architect.Enums;

namespace Architect.Widgets;
class DockPanel : Widget
{
    public DockPanel()
    {
        Content = new Container
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Content = new Row
            {

                Content = {
                }
            }
        };
    }
}


