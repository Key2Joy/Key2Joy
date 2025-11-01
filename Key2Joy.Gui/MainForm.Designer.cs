namespace Key2Joy.Gui
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.olvMappings = new BrightIdeasSoftware.ObjectListView();
            this.olvColumnAction = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnTrigger = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.pnlActionManagement = new System.Windows.Forms.Panel();
            this.btnCreateMapping = new System.Windows.Forms.Button();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.txtFilterLabel = new System.Windows.Forms.Label();
            this.pnlProfileManagement = new System.Windows.Forms.Panel();
            this.txtProfileName = new System.Windows.Forms.TextBox();
            this.lblProfileName = new System.Windows.Forms.Label();
            this.chkArmed = new System.Windows.Forms.CheckBox();
            this.menMainMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newProfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadProfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.openProfileFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupMappingsByToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewScriptOutputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewLogFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewEventViewerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createNewMappingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.fillProfileWithToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allGamePadJoystickActionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.releaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pressAndReleaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allKeyboardActionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pressToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.releaseToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.pressAndReleaseToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.testMappingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testGamePadJoystickToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.devicetestscomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gamepadtestercomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testKeyboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testMouseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.withSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateOppositePressStateMappingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reportAProblemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewSourceCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ntfIndicator = new System.Windows.Forms.NotifyIcon(this.components);
            this.pnlMainMenu = new System.Windows.Forms.Panel();
            this.pnlNotificationsParent = new System.Windows.Forms.Panel();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.exitProgramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pluginsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.managePluginsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openPluginsFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.userConfigurationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deviceListControl = new Key2Joy.Gui.DeviceListControl();
            ((System.ComponentModel.ISupportInitialize)(this.olvMappings)).BeginInit();
            this.pnlActionManagement.SuspendLayout();
            this.pnlProfileManagement.SuspendLayout();
            this.menMainMenu.SuspendLayout();
            this.pnlMainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // olvMappings
            // 
            this.olvMappings.AllColumns.Add(this.olvColumnAction);
            this.olvMappings.AllColumns.Add(this.olvColumnTrigger);
            this.olvMappings.CellEditUseWholeCell = false;
            this.olvMappings.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnAction,
            this.olvColumnTrigger});
            this.olvMappings.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvMappings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.olvMappings.EmptyListMsg = "There are no mappings, or a search filter is applied that matched no mappings.";
            this.olvMappings.EmptyListMsgFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.olvMappings.FullRowSelect = true;
            this.olvMappings.HideSelection = false;
            this.olvMappings.LabelWrap = false;
            this.olvMappings.Location = new System.Drawing.Point(0, 69);
            this.olvMappings.Margin = new System.Windows.Forms.Padding(4);
            this.olvMappings.Name = "olvMappings";
            this.olvMappings.RowHeight = 25;
            this.olvMappings.Size = new System.Drawing.Size(810, 611);
            this.olvMappings.TabIndex = 84;
            this.olvMappings.UseCellFormatEvents = true;
            this.olvMappings.UseCompatibleStateImageBehavior = false;
            this.olvMappings.UseFiltering = true;
            this.olvMappings.UseHotItem = true;
            this.olvMappings.UseTranslucentHotItem = true;
            this.olvMappings.UseTranslucentSelection = true;
            this.olvMappings.View = System.Windows.Forms.View.Details;
            // 
            // olvColumnAction
            // 
            this.olvColumnAction.AspectName = "Action";
            this.olvColumnAction.DisplayIndex = 1;
            this.olvColumnAction.Text = "Action";
            this.olvColumnAction.UseInitialLetterForGroup = true;
            // 
            // olvColumnTrigger
            // 
            this.olvColumnTrigger.AspectName = "Trigger";
            this.olvColumnTrigger.DisplayIndex = 0;
            this.olvColumnTrigger.Groupable = false;
            this.olvColumnTrigger.Text = "Trigger";
            // 
            // pnlActionManagement
            // 
            this.pnlActionManagement.Controls.Add(this.btnCreateMapping);
            this.pnlActionManagement.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlActionManagement.Location = new System.Drawing.Point(0, 680);
            this.pnlActionManagement.Margin = new System.Windows.Forms.Padding(4);
            this.pnlActionManagement.Name = "pnlActionManagement";
            this.pnlActionManagement.Padding = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.pnlActionManagement.Size = new System.Drawing.Size(810, 54);
            this.pnlActionManagement.TabIndex = 0;
            // 
            // btnCreateMapping
            // 
            this.btnCreateMapping.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCreateMapping.Location = new System.Drawing.Point(630, 7);
            this.btnCreateMapping.Margin = new System.Windows.Forms.Padding(4);
            this.btnCreateMapping.Name = "btnCreateMapping";
            this.btnCreateMapping.Size = new System.Drawing.Size(174, 40);
            this.btnCreateMapping.TabIndex = 0;
            this.btnCreateMapping.Text = "Create New Mapping";
            this.btnCreateMapping.UseVisualStyleBackColor = true;
            this.btnCreateMapping.Click += new System.EventHandler(this.BtnCreateMapping_Click);
            // 
            // txtFilter
            // 
            this.txtFilter.Location = new System.Drawing.Point(97, 6);
            this.txtFilter.Margin = new System.Windows.Forms.Padding(4);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(245, 25);
            this.txtFilter.TabIndex = 1;
            this.txtFilter.TextChanged += new System.EventHandler(this.TxtFilter_TextChanged);
            // 
            // txtFilterLabel
            // 
            this.txtFilterLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFilterLabel.Image = global::Key2Joy.Gui.Properties.Resources.magnifier;
            this.txtFilterLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtFilterLabel.Location = new System.Drawing.Point(17, 0);
            this.txtFilterLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txtFilterLabel.Name = "txtFilterLabel";
            this.txtFilterLabel.Size = new System.Drawing.Size(71, 39);
            this.txtFilterLabel.TabIndex = 89;
            this.txtFilterLabel.Text = "Search";
            this.txtFilterLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pnlProfileManagement
            // 
            this.pnlProfileManagement.BackColor = System.Drawing.SystemColors.ControlLight;
            this.pnlProfileManagement.Controls.Add(this.txtFilter);
            this.pnlProfileManagement.Controls.Add(this.txtFilterLabel);
            this.pnlProfileManagement.Controls.Add(this.txtProfileName);
            this.pnlProfileManagement.Controls.Add(this.lblProfileName);
            this.pnlProfileManagement.Controls.Add(this.chkArmed);
            this.pnlProfileManagement.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlProfileManagement.Location = new System.Drawing.Point(0, 30);
            this.pnlProfileManagement.Margin = new System.Windows.Forms.Padding(4);
            this.pnlProfileManagement.Name = "pnlProfileManagement";
            this.pnlProfileManagement.Padding = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.pnlProfileManagement.Size = new System.Drawing.Size(810, 39);
            this.pnlProfileManagement.TabIndex = 82;
            // 
            // txtProfileName
            // 
            this.txtProfileName.Location = new System.Drawing.Point(420, 6);
            this.txtProfileName.Margin = new System.Windows.Forms.Padding(4);
            this.txtProfileName.Name = "txtProfileName";
            this.txtProfileName.Size = new System.Drawing.Size(183, 25);
            this.txtProfileName.TabIndex = 85;
            this.txtProfileName.TextChanged += new System.EventHandler(this.TxtProfileName_TextChanged);
            // 
            // lblProfileName
            // 
            this.lblProfileName.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProfileName.Location = new System.Drawing.Point(361, 0);
            this.lblProfileName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblProfileName.Name = "lblProfileName";
            this.lblProfileName.Size = new System.Drawing.Size(51, 39);
            this.lblProfileName.TabIndex = 88;
            this.lblProfileName.Text = "Profile";
            this.lblProfileName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkArmed
            // 
            this.chkArmed.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkArmed.Dock = System.Windows.Forms.DockStyle.Right;
            this.chkArmed.Location = new System.Drawing.Point(696, 7);
            this.chkArmed.Margin = new System.Windows.Forms.Padding(4);
            this.chkArmed.Name = "chkArmed";
            this.chkArmed.Size = new System.Drawing.Size(108, 25);
            this.chkArmed.TabIndex = 81;
            this.chkArmed.Text = "Connect";
            this.chkArmed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkArmed.UseVisualStyleBackColor = true;
            this.chkArmed.CheckedChanged += new System.EventHandler(this.ChkEnabled_CheckedChanged);
            // 
            // menMainMenu
            // 
            this.menMainMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menMainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menMainMenu.Location = new System.Drawing.Point(0, 0);
            this.menMainMenu.Name = "menMainMenu";
            this.menMainMenu.Padding = new System.Windows.Forms.Padding(7, 3, 0, 3);
            this.menMainMenu.Size = new System.Drawing.Size(810, 30);
            this.menMainMenu.TabIndex = 81;
            this.menMainMenu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProfileToolStripMenuItem,
            this.loadProfileToolStripMenuItem,
            this.toolStripSeparator3,
            this.openProfileFolderToolStripMenuItem,
            this.toolStripSeparator2,
            this.closeToolStripMenuItem,
            this.exitProgramToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(46, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newProfileToolStripMenuItem
            // 
            this.newProfileToolStripMenuItem.Name = "newProfileToolStripMenuItem";
            this.newProfileToolStripMenuItem.Size = new System.Drawing.Size(255, 26);
            this.newProfileToolStripMenuItem.Text = "New Profile";
            this.newProfileToolStripMenuItem.Click += new System.EventHandler(this.NewProfileToolStripMenuItem_Click);
            // 
            // loadProfileToolStripMenuItem
            // 
            this.loadProfileToolStripMenuItem.Name = "loadProfileToolStripMenuItem";
            this.loadProfileToolStripMenuItem.Size = new System.Drawing.Size(255, 26);
            this.loadProfileToolStripMenuItem.Text = "Load Profile";
            this.loadProfileToolStripMenuItem.Click += new System.EventHandler(this.LoadProfileToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(252, 6);
            // 
            // openProfileFolderToolStripMenuItem
            // 
            this.openProfileFolderToolStripMenuItem.Name = "openProfileFolderToolStripMenuItem";
            this.openProfileFolderToolStripMenuItem.Size = new System.Drawing.Size(255, 26);
            this.openProfileFolderToolStripMenuItem.Text = "Open Profile Folder";
            this.openProfileFolderToolStripMenuItem.Click += new System.EventHandler(this.OpenProfileFolderToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(252, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(255, 26);
            this.closeToolStripMenuItem.Text = "Close to notification tray";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.CloseToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.groupMappingsByToolStripMenuItem,
            this.viewScriptOutputToolStripMenuItem,
            this.toolStripSeparator5,
            this.pluginsToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(55, 24);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // groupMappingsByToolStripMenuItem
            // 
            this.groupMappingsByToolStripMenuItem.Name = "groupMappingsByToolStripMenuItem";
            this.groupMappingsByToolStripMenuItem.Size = new System.Drawing.Size(232, 26);
            this.groupMappingsByToolStripMenuItem.Text = "Group Mappings By...";
            // 
            // viewScriptOutputToolStripMenuItem
            // 
            this.viewScriptOutputToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewLogFileToolStripMenuItem,
            this.viewEventViewerToolStripMenuItem});
            this.viewScriptOutputToolStripMenuItem.Name = "viewScriptOutputToolStripMenuItem";
            this.viewScriptOutputToolStripMenuItem.Size = new System.Drawing.Size(232, 26);
            this.viewScriptOutputToolStripMenuItem.Text = "View Script Output";
            // 
            // viewLogFileToolStripMenuItem
            // 
            this.viewLogFileToolStripMenuItem.Name = "viewLogFileToolStripMenuItem";
            this.viewLogFileToolStripMenuItem.Size = new System.Drawing.Size(213, 26);
            this.viewLogFileToolStripMenuItem.Text = "View Log File";
            this.viewLogFileToolStripMenuItem.Click += new System.EventHandler(this.ViewLogFileToolStripMenuItem_Click);
            // 
            // viewEventViewerToolStripMenuItem
            // 
            this.viewEventViewerToolStripMenuItem.Name = "viewEventViewerToolStripMenuItem";
            this.viewEventViewerToolStripMenuItem.Size = new System.Drawing.Size(213, 26);
            this.viewEventViewerToolStripMenuItem.Text = "View Event Viewer";
            this.viewEventViewerToolStripMenuItem.Click += new System.EventHandler(this.ViewEventViewerToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(229, 6);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createNewMappingToolStripMenuItem,
            this.toolStripSeparator6,
            this.fillProfileWithToolStripMenuItem,
            this.testMappingsToolStripMenuItem,
            this.withSelectedToolStripMenuItem,
            this.toolStripSeparator4,
            this.userConfigurationsToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(58, 24);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // createNewMappingToolStripMenuItem
            // 
            this.createNewMappingToolStripMenuItem.Name = "createNewMappingToolStripMenuItem";
            this.createNewMappingToolStripMenuItem.Size = new System.Drawing.Size(307, 26);
            this.createNewMappingToolStripMenuItem.Text = "Create New Mapping";
            this.createNewMappingToolStripMenuItem.Click += new System.EventHandler(this.CreateNewMappingToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(304, 6);
            // 
            // fillProfileWithToolStripMenuItem
            // 
            this.fillProfileWithToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allGamePadJoystickActionsToolStripMenuItem,
            this.allKeyboardActionsToolStripMenuItem});
            this.fillProfileWithToolStripMenuItem.Name = "fillProfileWithToolStripMenuItem";
            this.fillProfileWithToolStripMenuItem.Size = new System.Drawing.Size(307, 26);
            this.fillProfileWithToolStripMenuItem.Text = "Fill Profile With...";
            // 
            // allGamePadJoystickActionsToolStripMenuItem
            // 
            this.allGamePadJoystickActionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pressToolStripMenuItem,
            this.releaseToolStripMenuItem,
            this.pressAndReleaseToolStripMenuItem});
            this.allGamePadJoystickActionsToolStripMenuItem.Name = "allGamePadJoystickActionsToolStripMenuItem";
            this.allGamePadJoystickActionsToolStripMenuItem.Size = new System.Drawing.Size(286, 26);
            this.allGamePadJoystickActionsToolStripMenuItem.Text = "All GamePad/Joystick Actions";
            // 
            // pressToolStripMenuItem
            // 
            this.pressToolStripMenuItem.Name = "pressToolStripMenuItem";
            this.pressToolStripMenuItem.Size = new System.Drawing.Size(244, 26);
            this.pressToolStripMenuItem.Text = "Press";
            this.pressToolStripMenuItem.Click += new System.EventHandler(this.GamePadPressToolStripMenuItem_Click);
            // 
            // releaseToolStripMenuItem
            // 
            this.releaseToolStripMenuItem.Name = "releaseToolStripMenuItem";
            this.releaseToolStripMenuItem.Size = new System.Drawing.Size(244, 26);
            this.releaseToolStripMenuItem.Text = "Release";
            this.releaseToolStripMenuItem.Click += new System.EventHandler(this.GamePadReleaseToolStripMenuItem_Click);
            // 
            // pressAndReleaseToolStripMenuItem
            // 
            this.pressAndReleaseToolStripMenuItem.Name = "pressAndReleaseToolStripMenuItem";
            this.pressAndReleaseToolStripMenuItem.Size = new System.Drawing.Size(244, 26);
            this.pressAndReleaseToolStripMenuItem.Text = "Both Press and Release";
            this.pressAndReleaseToolStripMenuItem.Click += new System.EventHandler(this.GamePadPressAndReleaseToolStripMenuItem_Click);
            // 
            // allKeyboardActionsToolStripMenuItem
            // 
            this.allKeyboardActionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pressToolStripMenuItem1,
            this.releaseToolStripMenuItem1,
            this.pressAndReleaseToolStripMenuItem1});
            this.allKeyboardActionsToolStripMenuItem.Name = "allKeyboardActionsToolStripMenuItem";
            this.allKeyboardActionsToolStripMenuItem.Size = new System.Drawing.Size(286, 26);
            this.allKeyboardActionsToolStripMenuItem.Text = "All Keyboard Actions";
            // 
            // pressToolStripMenuItem1
            // 
            this.pressToolStripMenuItem1.Name = "pressToolStripMenuItem1";
            this.pressToolStripMenuItem1.Size = new System.Drawing.Size(244, 26);
            this.pressToolStripMenuItem1.Text = "Press";
            this.pressToolStripMenuItem1.Click += new System.EventHandler(this.KeyboardPressToolStripMenuItem_Click);
            // 
            // releaseToolStripMenuItem1
            // 
            this.releaseToolStripMenuItem1.Name = "releaseToolStripMenuItem1";
            this.releaseToolStripMenuItem1.Size = new System.Drawing.Size(244, 26);
            this.releaseToolStripMenuItem1.Text = "Release";
            this.releaseToolStripMenuItem1.Click += new System.EventHandler(this.KeyboardReleaseToolStripMenuItem_Click);
            // 
            // pressAndReleaseToolStripMenuItem1
            // 
            this.pressAndReleaseToolStripMenuItem1.Name = "pressAndReleaseToolStripMenuItem1";
            this.pressAndReleaseToolStripMenuItem1.Size = new System.Drawing.Size(244, 26);
            this.pressAndReleaseToolStripMenuItem1.Text = "Both Press and Release";
            this.pressAndReleaseToolStripMenuItem1.Click += new System.EventHandler(this.KeyboardPressAndReleaseToolStripMenuItem_Click);
            // 
            // testMappingsToolStripMenuItem
            // 
            this.testMappingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testGamePadJoystickToolStripMenuItem,
            this.testKeyboardToolStripMenuItem,
            this.testMouseToolStripMenuItem});
            this.testMappingsToolStripMenuItem.Name = "testMappingsToolStripMenuItem";
            this.testMappingsToolStripMenuItem.Size = new System.Drawing.Size(307, 26);
            this.testMappingsToolStripMenuItem.Text = "Test Mappings";
            // 
            // testGamePadJoystickToolStripMenuItem
            // 
            this.testGamePadJoystickToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.devicetestscomToolStripMenuItem,
            this.gamepadtestercomToolStripMenuItem});
            this.testGamePadJoystickToolStripMenuItem.Name = "testGamePadJoystickToolStripMenuItem";
            this.testGamePadJoystickToolStripMenuItem.Size = new System.Drawing.Size(249, 26);
            this.testGamePadJoystickToolStripMenuItem.Text = "Test GamePad / Joystick";
            // 
            // devicetestscomToolStripMenuItem
            // 
            this.devicetestscomToolStripMenuItem.Name = "devicetestscomToolStripMenuItem";
            this.devicetestscomToolStripMenuItem.Size = new System.Drawing.Size(231, 26);
            this.devicetestscomToolStripMenuItem.Text = "devicetests.com";
            this.devicetestscomToolStripMenuItem.Click += new System.EventHandler(this.DevicetestscomToolStripMenuItem_Click);
            // 
            // gamepadtestercomToolStripMenuItem
            // 
            this.gamepadtestercomToolStripMenuItem.Name = "gamepadtestercomToolStripMenuItem";
            this.gamepadtestercomToolStripMenuItem.Size = new System.Drawing.Size(231, 26);
            this.gamepadtestercomToolStripMenuItem.Text = "gamepad-tester.com";
            this.gamepadtestercomToolStripMenuItem.Click += new System.EventHandler(this.GamepadtestercomToolStripMenuItem_Click);
            // 
            // testKeyboardToolStripMenuItem
            // 
            this.testKeyboardToolStripMenuItem.Name = "testKeyboardToolStripMenuItem";
            this.testKeyboardToolStripMenuItem.Size = new System.Drawing.Size(249, 26);
            this.testKeyboardToolStripMenuItem.Text = "Test Keyboard";
            this.testKeyboardToolStripMenuItem.Click += new System.EventHandler(this.TestKeyboardToolStripMenuItem_Click);
            // 
            // testMouseToolStripMenuItem
            // 
            this.testMouseToolStripMenuItem.Name = "testMouseToolStripMenuItem";
            this.testMouseToolStripMenuItem.Size = new System.Drawing.Size(249, 26);
            this.testMouseToolStripMenuItem.Text = "Test Mouse";
            this.testMouseToolStripMenuItem.Click += new System.EventHandler(this.TestMouseToolStripMenuItem_Click);
            // 
            // withSelectedToolStripMenuItem
            // 
            this.withSelectedToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generateOppositePressStateMappingsToolStripMenuItem});
            this.withSelectedToolStripMenuItem.Name = "withSelectedToolStripMenuItem";
            this.withSelectedToolStripMenuItem.Size = new System.Drawing.Size(307, 26);
            this.withSelectedToolStripMenuItem.Text = "With Selected...";
            // 
            // generateOppositePressStateMappingsToolStripMenuItem
            // 
            this.generateOppositePressStateMappingsToolStripMenuItem.Name = "generateOppositePressStateMappingsToolStripMenuItem";
            this.generateOppositePressStateMappingsToolStripMenuItem.Size = new System.Drawing.Size(277, 26);
            this.generateOppositePressStateMappingsToolStripMenuItem.Text = "Generate Reverse Mappings";
            this.generateOppositePressStateMappingsToolStripMenuItem.Click += new System.EventHandler(this.GenerateReverseMappingsToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(304, 6);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reportAProblemToolStripMenuItem,
            this.viewSourceCodeToolStripMenuItem,
            this.toolStripSeparator1,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(55, 24);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // reportAProblemToolStripMenuItem
            // 
            this.reportAProblemToolStripMenuItem.Name = "reportAProblemToolStripMenuItem";
            this.reportAProblemToolStripMenuItem.Size = new System.Drawing.Size(212, 26);
            this.reportAProblemToolStripMenuItem.Text = "Report a Problem";
            this.reportAProblemToolStripMenuItem.Click += new System.EventHandler(this.ReportAProblemToolStripMenuItem_Click);
            // 
            // viewSourceCodeToolStripMenuItem
            // 
            this.viewSourceCodeToolStripMenuItem.Name = "viewSourceCodeToolStripMenuItem";
            this.viewSourceCodeToolStripMenuItem.Size = new System.Drawing.Size(212, 26);
            this.viewSourceCodeToolStripMenuItem.Text = "View Source Code";
            this.viewSourceCodeToolStripMenuItem.Click += new System.EventHandler(this.ViewSourceCodeToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(209, 6);
            // 
            // ntfIndicator
            // 
            this.ntfIndicator.Icon = ((System.Drawing.Icon)(resources.GetObject("ntfIndicator.Icon")));
            this.ntfIndicator.Text = "Key2Joy";
            this.ntfIndicator.Visible = true;
            this.ntfIndicator.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NtfIndicator_MouseDoubleClick);
            // 
            // pnlMainMenu
            // 
            this.pnlMainMenu.Controls.Add(this.menMainMenu);
            this.pnlMainMenu.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlMainMenu.Location = new System.Drawing.Point(0, 0);
            this.pnlMainMenu.Margin = new System.Windows.Forms.Padding(4);
            this.pnlMainMenu.Name = "pnlMainMenu";
            this.pnlMainMenu.Size = new System.Drawing.Size(810, 30);
            this.pnlMainMenu.TabIndex = 85;
            // 
            // pnlNotificationsParent
            // 
            this.pnlNotificationsParent.AutoSize = true;
            this.pnlNotificationsParent.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlNotificationsParent.Location = new System.Drawing.Point(0, 69);
            this.pnlNotificationsParent.Margin = new System.Windows.Forms.Padding(4);
            this.pnlNotificationsParent.Name = "pnlNotificationsParent";
            this.pnlNotificationsParent.Size = new System.Drawing.Size(810, 0);
            this.pnlNotificationsParent.TabIndex = 89;
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.olvMappings);
            this.splitContainer.Panel1.Controls.Add(this.pnlActionManagement);
            this.splitContainer.Panel1.Controls.Add(this.pnlNotificationsParent);
            this.splitContainer.Panel1.Controls.Add(this.pnlProfileManagement);
            this.splitContainer.Panel1.Controls.Add(this.pnlMainMenu);
            this.splitContainer.Panel1MinSize = 650;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.deviceListControl);
            this.splitContainer.Panel2MinSize = 80;
            this.splitContainer.Size = new System.Drawing.Size(915, 734);
            this.splitContainer.SplitterDistance = 810;
            this.splitContainer.SplitterWidth = 5;
            this.splitContainer.TabIndex = 90;
            // 
            // exitProgramToolStripMenuItem
            // 
            this.exitProgramToolStripMenuItem.Image = global::Key2Joy.Gui.Properties.Resources.door_out;
            this.exitProgramToolStripMenuItem.Name = "exitProgramToolStripMenuItem";
            this.exitProgramToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitProgramToolStripMenuItem.Size = new System.Drawing.Size(255, 26);
            this.exitProgramToolStripMenuItem.Text = "Exit";
            this.exitProgramToolStripMenuItem.Click += new System.EventHandler(this.ExitProgramToolStripMenuItem_Click);
            // 
            // pluginsToolStripMenuItem
            // 
            this.pluginsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.managePluginsToolStripMenuItem,
            this.openPluginsFolderToolStripMenuItem});
            this.pluginsToolStripMenuItem.Image = global::Key2Joy.Gui.Properties.Resources.plugin;
            this.pluginsToolStripMenuItem.Name = "pluginsToolStripMenuItem";
            this.pluginsToolStripMenuItem.Size = new System.Drawing.Size(232, 26);
            this.pluginsToolStripMenuItem.Text = "Plugins";
            // 
            // managePluginsToolStripMenuItem
            // 
            this.managePluginsToolStripMenuItem.Name = "managePluginsToolStripMenuItem";
            this.managePluginsToolStripMenuItem.Size = new System.Drawing.Size(225, 26);
            this.managePluginsToolStripMenuItem.Text = "Manage Plugins";
            this.managePluginsToolStripMenuItem.Click += new System.EventHandler(this.ManagePluginsToolStripMenuItem_Click);
            // 
            // openPluginsFolderToolStripMenuItem
            // 
            this.openPluginsFolderToolStripMenuItem.Name = "openPluginsFolderToolStripMenuItem";
            this.openPluginsFolderToolStripMenuItem.Size = new System.Drawing.Size(225, 26);
            this.openPluginsFolderToolStripMenuItem.Text = "Open Plugins Folder";
            this.openPluginsFolderToolStripMenuItem.Click += new System.EventHandler(this.OpenPluginsFolderToolStripMenuItem_Click);
            // 
            // userConfigurationsToolStripMenuItem
            // 
            this.userConfigurationsToolStripMenuItem.Image = global::Key2Joy.Gui.Properties.Resources.cog;
            this.userConfigurationsToolStripMenuItem.Name = "userConfigurationsToolStripMenuItem";
            this.userConfigurationsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Oemcomma)));
            this.userConfigurationsToolStripMenuItem.Size = new System.Drawing.Size(307, 26);
            this.userConfigurationsToolStripMenuItem.Text = "Configuration";
            this.userConfigurationsToolStripMenuItem.Click += new System.EventHandler(this.UserConfigurationsToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Image = global::Key2Joy.Gui.Properties.Resources.information;
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(212, 26);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItem_Click);
            // 
            // deviceListControl
            // 
            this.deviceListControl.AutoScroll = true;
            this.deviceListControl.BackColor = System.Drawing.Color.Transparent;
            this.deviceListControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.deviceListControl.Location = new System.Drawing.Point(0, 0);
            this.deviceListControl.Margin = new System.Windows.Forms.Padding(5);
            this.deviceListControl.Name = "deviceListControl";
            this.deviceListControl.Size = new System.Drawing.Size(100, 734);
            this.deviceListControl.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(915, 734);
            this.Controls.Add(this.splitContainer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(930, 639);
            this.Name = "MainForm";
            this.Text = "Key2Joy - Alpha Version";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.SizeChanged += new System.EventHandler(this.MainForm_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.olvMappings)).EndInit();
            this.pnlActionManagement.ResumeLayout(false);
            this.pnlProfileManagement.ResumeLayout(false);
            this.pnlProfileManagement.PerformLayout();
            this.menMainMenu.ResumeLayout(false);
            this.menMainMenu.PerformLayout();
            this.pnlMainMenu.ResumeLayout(false);
            this.pnlMainMenu.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.CheckBox chkArmed;
        private System.Windows.Forms.Panel pnlActionManagement;
        private System.Windows.Forms.Panel pnlProfileManagement;
        private System.Windows.Forms.TextBox txtProfileName;
        private System.Windows.Forms.Label lblProfileName;
        private BrightIdeasSoftware.OLVColumn olvColumnTrigger;
        private BrightIdeasSoftware.OLVColumn olvColumnAction;
        private System.Windows.Forms.MenuStrip menMainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadProfileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fillProfileWithToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reportAProblemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewSourceCodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Button btnCreateMapping;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openProfileFolderToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon ntfIndicator;
        private System.Windows.Forms.ToolStripMenuItem newProfileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitProgramToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testMappingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.Panel pnlMainMenu;
        private System.Windows.Forms.ToolStripMenuItem testGamePadJoystickToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testKeyboardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testMouseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allGamePadJoystickActionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pressAndReleaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pressToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem releaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allKeyboardActionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pressAndReleaseToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem pressToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem releaseToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem userConfigurationsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewScriptOutputToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewLogFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewEventViewerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem devicetestscomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gamepadtestercomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pluginsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem managePluginsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openPluginsFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem withSelectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateOppositePressStateMappingsToolStripMenuItem;
        private BrightIdeasSoftware.ObjectListView olvMappings;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.Label txtFilterLabel;
        private System.Windows.Forms.Panel pnlNotificationsParent;
        private System.Windows.Forms.SplitContainer splitContainer;
        private DeviceListControl deviceListControl;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem createNewMappingToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem groupMappingsByToolStripMenuItem;
    }
}

