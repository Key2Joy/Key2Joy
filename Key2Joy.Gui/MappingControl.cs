using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Key2Joy.Contracts.Mapping;
using Key2Joy.Contracts.Mapping.Actions;
using Key2Joy.Contracts.Mapping.Triggers;
using Key2Joy.Gui.Mapping;
using Key2Joy.Mapping;
using Key2Joy.Plugins;

namespace Key2Joy.Gui;

public partial class MappingControl : UserControl
{
    public event EventHandler OnApplied;

    private MappingProfile _profile;
    private MappedOption _mapping;
    private MappedOption _reverseMapping;
    private bool _dominantReverseCheckedState;

    public MappingControl()
    {
        InitializeComponent();

        this.triggerControl.TriggerChanged += this.TriggerControl_TriggerChanged;
        this.actionControl.ActionChanged += this.ActionControl_ActionChanged;
    }


    public void StartEdit(MappingProfile profile, MappedOption mapping)
    {
        _profile = profile;
        _mapping = mapping;

        this._dominantReverseCheckedState = _mapping == null || _mapping.Children.Any();
        this.triggerControl.SelectTrigger(_mapping.Trigger);
        this.actionControl.SelectAction(_mapping.Action);
        this.RefreshCreateReverseMappingOption();
    }

    public void Apply()
    {
        var trigger = this.triggerControl.Trigger;
        var action = this.actionControl.Action;

        if (trigger == null)
        {
            MessageBox.Show("You must select a trigger!", "No trigger selected!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (action == null)
        {
            MessageBox.Show("You must select an action!", "No action selected!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        this._mapping ??= new MappedOption();
        this._mapping.Trigger = trigger;
        this._mapping.Action = action;

        if (!this.triggerControl.CanMappingSave(this._mapping))
        {
            return;
        }

        if (!this.actionControl.CanMappingSave(this._mapping))
        {
            return;
        }

        if (this.chkCreateReverseMapping.Checked)
        {
            var reverse = MappedOption.GenerateReverseMapping(this._mapping, true);

            if (!this._mapping.Children.Any())
            {
                this._reverseMapping = reverse;
                this._reverseMapping.SetParent(this._mapping);
            }
            else
            {
                // Update the existing reverse mapping
                var existingReverse = this._mapping.Children.First();
                existingReverse.Trigger = reverse.Trigger;
                existingReverse.Action = reverse.Action;

                if (this._mapping.Children.Count > 1)
                {
                    // TODO: What do we do if there's multiple children?
                    MessageBox.Show(
                        $"There are {this._mapping.Children.Count} children of this mapping. Only the first one was updated with the reverse mapping.",
                        "Multiple children",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
        }

        // TODO: PROPER CREATION HANDLING
        //if (existingMapping == null)
        //{
        //    this.Profile.AddMapping(Mapping);
        //}

        if (_reverseMapping != null)
        {
            this._profile.AddMapping(_reverseMapping);
        }


        this.OnApplied?.Invoke(this, EventArgs.Empty);
    }

    private void ActionControl_ActionChanged(object sender, ActionChangedEventArgs e)
       => this.RefreshCreateReverseMappingOption();

    private void TriggerControl_TriggerChanged(object sender, TriggerChangedEventArgs e)
        => this.RefreshCreateReverseMappingOption();

    private void RefreshCreateReverseMappingOption()
    {
        if (this.triggerControl.Trigger is IProvideReverseAspect
            && this.actionControl.Action is IProvideReverseAspect)
        {
            this.chkCreateReverseMapping.Checked = this._dominantReverseCheckedState;
            this.chkCreateReverseMapping.Enabled = true;
        }
        else
        {
            this.chkCreateReverseMapping.Checked = false;
            this.chkCreateReverseMapping.Enabled = false;
        }
    }

    public static Control BuildOptionsForComboBox<TAttribute, TAspect>(ComboBox comboBox, Panel optionsPanel)
        where TAttribute : MappingAttribute
        where TAspect : AbstractMappingAspect
    {
        var optionsUserControl = optionsPanel.Controls.OfType<Control>().FirstOrDefault();

        if (optionsUserControl != null)
        {
            optionsPanel.Controls.Remove(optionsUserControl);
            optionsUserControl.Dispose();
        }

        if (comboBox.SelectedItem == null)
        {
            return null;
        }

        var selected = ((ImageComboBoxItem<KeyValuePair<TAttribute, MappingTypeFactory<TAspect>>>)comboBox.SelectedItem).ItemValue;
        var selectedTypeFactory = selected.Value;

        optionsUserControl = CreateOptionsControl(selectedTypeFactory.FullTypeName);

        if (optionsUserControl == null)
        {
            MessageBox.Show("Could not create options control for " + selectedTypeFactory.FullTypeName + ". \n\nPlease report this issue to the developer.\n\nThe app will now crash.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            throw new NotImplementedException("Could not create options control for " + selectedTypeFactory.FullTypeName);
        }

        if (optionsUserControl is ElementHostProxy pluginUserControl)
        {
            optionsUserControl = new ActionPluginHostControl(pluginUserControl);
        }

        optionsPanel.Controls.Add(optionsUserControl);
        optionsUserControl.Dock = DockStyle.Top;

        return optionsUserControl;
    }

    private static Control CreateOptionsControl(string selectedTypeName)
    {
        var mappingControlFactory = MappingControlRepository.GetMappingControlFactory(selectedTypeName);

        if (mappingControlFactory == null)
        {
            return null;
        }

        return mappingControlFactory.CreateInstance<Control>();
    }

    private void chkCreateReverseMapping_Click(object sender, EventArgs e)
    {
        this._dominantReverseCheckedState = this.chkCreateReverseMapping.Checked;
    }
}
