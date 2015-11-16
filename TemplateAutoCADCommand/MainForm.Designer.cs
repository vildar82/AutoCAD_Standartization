namespace AutoCAD_Standartization
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.groupBoxActive = new System.Windows.Forms.GroupBox();
            this.listBoxLayersDoc = new System.Windows.Forms.ListBox();
            this.labelCountElements = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBoxStd = new System.Windows.Forms.GroupBox();
            this.listBoxLayersStd = new System.Windows.Forms.ListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.listViewTransfer = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonSelectForTransfer = new System.Windows.Forms.Button();
            this.buttonTransfer = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.checkBoxTextStyle = new System.Windows.Forms.CheckBox();
            this.checkBoxSizeStyle = new System.Windows.Forms.CheckBox();
            this.groupBoxActive.SuspendLayout();
            this.groupBoxStd.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxActive
            // 
            this.groupBoxActive.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBoxActive.Controls.Add(this.listBoxLayersDoc);
            this.groupBoxActive.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBoxActive.Location = new System.Drawing.Point(12, 0);
            this.groupBoxActive.Name = "groupBoxActive";
            this.groupBoxActive.Size = new System.Drawing.Size(342, 321);
            this.groupBoxActive.TabIndex = 0;
            this.groupBoxActive.TabStop = false;
            this.groupBoxActive.Text = "Нестандартные слои в чертеже";
            // 
            // listBoxLayersDoc
            // 
            this.listBoxLayersDoc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBoxLayersDoc.FormattingEnabled = true;
            this.listBoxLayersDoc.ItemHeight = 16;
            this.listBoxLayersDoc.Location = new System.Drawing.Point(6, 19);
            this.listBoxLayersDoc.Name = "listBoxLayersDoc";
            this.listBoxLayersDoc.Size = new System.Drawing.Size(330, 292);
            this.listBoxLayersDoc.TabIndex = 0;
            this.listBoxLayersDoc.SelectedIndexChanged += new System.EventHandler(this.listBoxLayersDoc_SelectedIndexChanged);
            // 
            // labelCountElements
            // 
            this.labelCountElements.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelCountElements.AutoSize = true;
            this.labelCountElements.Location = new System.Drawing.Point(115, 324);
            this.labelCountElements.Name = "labelCountElements";
            this.labelCountElements.Size = new System.Drawing.Size(13, 13);
            this.labelCountElements.TabIndex = 2;
            this.labelCountElements.Text = "0";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 324);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Элементов в слое:";
            // 
            // groupBoxStd
            // 
            this.groupBoxStd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBoxStd.Controls.Add(this.listBoxLayersStd);
            this.groupBoxStd.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBoxStd.Location = new System.Drawing.Point(371, 0);
            this.groupBoxStd.Name = "groupBoxStd";
            this.groupBoxStd.Size = new System.Drawing.Size(342, 321);
            this.groupBoxStd.TabIndex = 1;
            this.groupBoxStd.TabStop = false;
            this.groupBoxStd.Text = "Стандартные слои";
            // 
            // listBoxLayersStd
            // 
            this.listBoxLayersStd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBoxLayersStd.FormattingEnabled = true;
            this.listBoxLayersStd.ItemHeight = 16;
            this.listBoxLayersStd.Location = new System.Drawing.Point(6, 19);
            this.listBoxLayersStd.Name = "listBoxLayersStd";
            this.listBoxLayersStd.Size = new System.Drawing.Size(330, 292);
            this.listBoxLayersStd.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.listViewTransfer);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox3.Location = new System.Drawing.Point(12, 351);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(702, 228);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Сопоставленные слои";
            // 
            // listViewTransfer
            // 
            this.listViewTransfer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewTransfer.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listViewTransfer.FullRowSelect = true;
            this.listViewTransfer.Location = new System.Drawing.Point(0, 15);
            this.listViewTransfer.Name = "listViewTransfer";
            this.listViewTransfer.Size = new System.Drawing.Size(696, 207);
            this.listViewTransfer.TabIndex = 0;
            this.listViewTransfer.UseCompatibleStateImageBehavior = false;
            this.listViewTransfer.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Нестандартный слой";
            this.columnHeader1.Width = 300;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Стандартный слой";
            this.columnHeader2.Width = 321;
            // 
            // buttonSelectForTransfer
            // 
            this.buttonSelectForTransfer.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonSelectForTransfer.Location = new System.Drawing.Point(319, 327);
            this.buttonSelectForTransfer.Name = "buttonSelectForTransfer";
            this.buttonSelectForTransfer.Size = new System.Drawing.Size(91, 25);
            this.buttonSelectForTransfer.TabIndex = 3;
            this.buttonSelectForTransfer.Text = "Сопоставить";
            this.buttonSelectForTransfer.UseVisualStyleBackColor = true;
            this.buttonSelectForTransfer.Click += new System.EventHandler(this.buttonSelectForTransfer_Click);
            // 
            // buttonTransfer
            // 
            this.buttonTransfer.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonTransfer.Location = new System.Drawing.Point(486, 585);
            this.buttonTransfer.Name = "buttonTransfer";
            this.buttonTransfer.Size = new System.Drawing.Size(91, 25);
            this.buttonTransfer.TabIndex = 4;
            this.buttonTransfer.Text = "Заменить";
            this.buttonTransfer.UseVisualStyleBackColor = true;
            this.buttonTransfer.Click += new System.EventHandler(this.buttonTransfer_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonDelete.Location = new System.Drawing.Point(12, 585);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(91, 25);
            this.buttonDelete.TabIndex = 5;
            this.buttonDelete.Text = "Удалить";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // checkBoxTextStyle
            // 
            this.checkBoxTextStyle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxTextStyle.AutoSize = true;
            this.checkBoxTextStyle.Location = new System.Drawing.Point(600, 579);
            this.checkBoxTextStyle.Name = "checkBoxTextStyle";
            this.checkBoxTextStyle.Size = new System.Drawing.Size(114, 17);
            this.checkBoxTextStyle.TabIndex = 6;
            this.checkBoxTextStyle.Text = "Текстовые стили";
            this.checkBoxTextStyle.UseVisualStyleBackColor = true;
            // 
            // checkBoxSizeStyle
            // 
            this.checkBoxSizeStyle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxSizeStyle.AutoSize = true;
            this.checkBoxSizeStyle.Location = new System.Drawing.Point(600, 600);
            this.checkBoxSizeStyle.Name = "checkBoxSizeStyle";
            this.checkBoxSizeStyle.Size = new System.Drawing.Size(117, 17);
            this.checkBoxSizeStyle.TabIndex = 7;
            this.checkBoxSizeStyle.Text = "Размерные стили";
            this.checkBoxSizeStyle.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(722, 619);
            this.Controls.Add(this.checkBoxSizeStyle);
            this.Controls.Add(this.checkBoxTextStyle);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonTransfer);
            this.Controls.Add(this.labelCountElements);
            this.Controls.Add(this.buttonSelectForTransfer);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBoxStd);
            this.Controls.Add(this.groupBoxActive);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(738, 906);
            this.MinimumSize = new System.Drawing.Size(738, 510);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Стандартизация чертежа";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBoxActive.ResumeLayout(false);
            this.groupBoxStd.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxActive;
        private System.Windows.Forms.ListBox listBoxLayersDoc;
        private System.Windows.Forms.GroupBox groupBoxStd;
        private System.Windows.Forms.ListBox listBoxLayersStd;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListView listViewTransfer;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button buttonSelectForTransfer;
        private System.Windows.Forms.Label labelCountElements;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonTransfer;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.CheckBox checkBoxTextStyle;
        private System.Windows.Forms.CheckBox checkBoxSizeStyle;
    }
}