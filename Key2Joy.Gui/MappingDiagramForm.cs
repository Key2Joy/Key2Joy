using System.Data;
using System.Linq;
using System.Windows.Forms;
using Key2Joy.Contracts.Mapping;
using Key2Joy.Gui.Diagram;
using Key2Joy.Mapping;

namespace Key2Joy.Gui;
public partial class MappingDiagramForm : Form
{
    public MappingDiagramForm(MappingProfile profile = null)
    {
        InitializeComponent();

        mappingDiagramControl.Definition = XboxSeriesXGamePadDiagram.Create();

        if (profile != null)
        {
            mappingDiagramControl.Mappings = profile.MappedOptions
                .Cast<AbstractMappedOption>()
                .ToList();
        }
    }
}
