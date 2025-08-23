namespace lpr381Project
{
    partial class Analysis
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
            HandleNonBasicVariableRangeBtn = new Button();
            HandleNonBasicVariableChangeBtn = new Button();
            HandleBasicVariableRangeBtn = new Button();
            HandleBasicVariableChangeBtn = new Button();
            HandleRHSRangeBtn = new Button();
            HandleRHSChangeBtn = new Button();
            HandleNonBasicColumnRangeBtn = new Button();
            HandleNonBasicColumnChangeBtn = new Button();
            inputBox = new TextBox();
            outputBox = new RichTextBox();
            sendInput = new Button();
            valueInput = new TextBox();
            inputChoicelbl = new Label();
            inputValuelbl = new Label();
            inputRow = new TextBox();
            InputRowLbl = new Label();
            addActivityBtn = new Button();
            addConstraintBtn = new Button();
            actsBtn = new Button();
            consBtn = new Button();
            inputConsLbl = new Label();
            dataGridViewCons = new DataGridView();
            dataGridViewActs = new DataGridView();
            doDualityBtn = new Button();
            shadowPriceBtn = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridViewCons).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewActs).BeginInit();
            SuspendLayout();
            // 
            // HandleNonBasicVariableRangeBtn
            // 
            HandleNonBasicVariableRangeBtn.Location = new Point(29, 12);
            HandleNonBasicVariableRangeBtn.Name = "HandleNonBasicVariableRangeBtn";
            HandleNonBasicVariableRangeBtn.Size = new Size(179, 36);
            HandleNonBasicVariableRangeBtn.TabIndex = 0;
            HandleNonBasicVariableRangeBtn.Text = "Non Basic Variable Range";
            HandleNonBasicVariableRangeBtn.UseVisualStyleBackColor = true;
            HandleNonBasicVariableRangeBtn.Click += HandleNonBasicVariableRangeBtn_Click;
            // 
            // HandleNonBasicVariableChangeBtn
            // 
            HandleNonBasicVariableChangeBtn.Location = new Point(29, 58);
            HandleNonBasicVariableChangeBtn.Name = "HandleNonBasicVariableChangeBtn";
            HandleNonBasicVariableChangeBtn.Size = new Size(179, 36);
            HandleNonBasicVariableChangeBtn.TabIndex = 1;
            HandleNonBasicVariableChangeBtn.Text = "Non Basic Variable Change";
            HandleNonBasicVariableChangeBtn.UseVisualStyleBackColor = true;
            HandleNonBasicVariableChangeBtn.Click += HandleNonBasicVariableChangeBtn_Click;
            // 
            // HandleBasicVariableRangeBtn
            // 
            HandleBasicVariableRangeBtn.Location = new Point(29, 102);
            HandleBasicVariableRangeBtn.Name = "HandleBasicVariableRangeBtn";
            HandleBasicVariableRangeBtn.Size = new Size(179, 36);
            HandleBasicVariableRangeBtn.TabIndex = 2;
            HandleBasicVariableRangeBtn.Text = "Basic Variable Range";
            HandleBasicVariableRangeBtn.UseVisualStyleBackColor = true;
            HandleBasicVariableRangeBtn.Click += HandleBasicVariableRangeBtn_Click;
            // 
            // HandleBasicVariableChangeBtn
            // 
            HandleBasicVariableChangeBtn.Location = new Point(29, 146);
            HandleBasicVariableChangeBtn.Name = "HandleBasicVariableChangeBtn";
            HandleBasicVariableChangeBtn.Size = new Size(179, 36);
            HandleBasicVariableChangeBtn.TabIndex = 3;
            HandleBasicVariableChangeBtn.Text = "Basic Variable Change";
            HandleBasicVariableChangeBtn.UseVisualStyleBackColor = true;
            HandleBasicVariableChangeBtn.Click += HandleBasicVariableChangeBtn_Click;
            // 
            // HandleRHSRangeBtn
            // 
            HandleRHSRangeBtn.Location = new Point(29, 188);
            HandleRHSRangeBtn.Name = "HandleRHSRangeBtn";
            HandleRHSRangeBtn.Size = new Size(179, 36);
            HandleRHSRangeBtn.TabIndex = 4;
            HandleRHSRangeBtn.Text = "RHS Range";
            HandleRHSRangeBtn.UseVisualStyleBackColor = true;
            HandleRHSRangeBtn.Click += HandleRHSRangeBtn_Click;
            // 
            // HandleRHSChangeBtn
            // 
            HandleRHSChangeBtn.Location = new Point(29, 230);
            HandleRHSChangeBtn.Name = "HandleRHSChangeBtn";
            HandleRHSChangeBtn.Size = new Size(179, 36);
            HandleRHSChangeBtn.TabIndex = 5;
            HandleRHSChangeBtn.Text = "RHS Change";
            HandleRHSChangeBtn.UseVisualStyleBackColor = true;
            HandleRHSChangeBtn.Click += HandleRHSChangeBtn_Click;
            // 
            // HandleNonBasicColumnRangeBtn
            // 
            HandleNonBasicColumnRangeBtn.Location = new Point(29, 272);
            HandleNonBasicColumnRangeBtn.Name = "HandleNonBasicColumnRangeBtn";
            HandleNonBasicColumnRangeBtn.Size = new Size(179, 36);
            HandleNonBasicColumnRangeBtn.TabIndex = 6;
            HandleNonBasicColumnRangeBtn.Text = "Non Basic Column Range";
            HandleNonBasicColumnRangeBtn.UseVisualStyleBackColor = true;
            HandleNonBasicColumnRangeBtn.Click += HandleNonBasicColumnRangeBtn_Click;
            // 
            // HandleNonBasicColumnChangeBtn
            // 
            HandleNonBasicColumnChangeBtn.Location = new Point(29, 314);
            HandleNonBasicColumnChangeBtn.Name = "HandleNonBasicColumnChangeBtn";
            HandleNonBasicColumnChangeBtn.Size = new Size(179, 36);
            HandleNonBasicColumnChangeBtn.TabIndex = 7;
            HandleNonBasicColumnChangeBtn.Text = "Non Basic Column Change";
            HandleNonBasicColumnChangeBtn.UseVisualStyleBackColor = true;
            HandleNonBasicColumnChangeBtn.Click += HandleNonBasicColumnChangeBtn_Click;
            // 
            // inputBox
            // 
            inputBox.Location = new Point(262, 110);
            inputBox.Name = "inputBox";
            inputBox.Size = new Size(133, 23);
            inputBox.TabIndex = 8;
            // 
            // outputBox
            // 
            outputBox.Location = new Point(262, 172);
            outputBox.Name = "outputBox";
            outputBox.ReadOnly = true;
            outputBox.Size = new Size(627, 311);
            outputBox.TabIndex = 9;
            outputBox.Text = "";
            // 
            // sendInput
            // 
            sendInput.Location = new Point(262, 139);
            sendInput.Name = "sendInput";
            sendInput.Size = new Size(74, 22);
            sendInput.TabIndex = 10;
            sendInput.Text = "Enter";
            sendInput.UseVisualStyleBackColor = true;
            sendInput.Click += sendInput_Click;
            // 
            // valueInput
            // 
            valueInput.Location = new Point(262, 81);
            valueInput.Name = "valueInput";
            valueInput.Size = new Size(133, 23);
            valueInput.TabIndex = 11;
            // 
            // inputChoicelbl
            // 
            inputChoicelbl.AutoSize = true;
            inputChoicelbl.Location = new Point(401, 113);
            inputChoicelbl.Name = "inputChoicelbl";
            inputChoicelbl.Size = new Size(75, 15);
            inputChoicelbl.TabIndex = 12;
            inputChoicelbl.Text = "Input Option";
            // 
            // inputValuelbl
            // 
            inputValuelbl.AutoSize = true;
            inputValuelbl.Location = new Point(401, 86);
            inputValuelbl.Name = "inputValuelbl";
            inputValuelbl.Size = new Size(66, 15);
            inputValuelbl.TabIndex = 13;
            inputValuelbl.Text = "Input Value";
            // 
            // inputRow
            // 
            inputRow.Location = new Point(262, 52);
            inputRow.Name = "inputRow";
            inputRow.Size = new Size(133, 23);
            inputRow.TabIndex = 14;
            // 
            // InputRowLbl
            // 
            InputRowLbl.AutoSize = true;
            InputRowLbl.Location = new Point(401, 55);
            InputRowLbl.Name = "InputRowLbl";
            InputRowLbl.Size = new Size(93, 15);
            InputRowLbl.TabIndex = 15;
            InputRowLbl.Text = "Input Row Index";
            // 
            // addActivityBtn
            // 
            addActivityBtn.Location = new Point(710, 12);
            addActivityBtn.Name = "addActivityBtn";
            addActivityBtn.Size = new Size(179, 36);
            addActivityBtn.TabIndex = 16;
            addActivityBtn.Text = "Add Activity";
            addActivityBtn.UseVisualStyleBackColor = true;
            addActivityBtn.Click += addActivityBtn_Click;
            // 
            // addConstraintBtn
            // 
            addConstraintBtn.Location = new Point(710, 97);
            addConstraintBtn.Name = "addConstraintBtn";
            addConstraintBtn.Size = new Size(179, 36);
            addConstraintBtn.TabIndex = 17;
            addConstraintBtn.Text = "Add Constraint";
            addConstraintBtn.UseVisualStyleBackColor = true;
            addConstraintBtn.Click += addConstraintBtn_Click;
            // 
            // actsBtn
            // 
            actsBtn.Location = new Point(710, 62);
            actsBtn.Name = "actsBtn";
            actsBtn.Size = new Size(74, 22);
            actsBtn.TabIndex = 20;
            actsBtn.Text = "Enter";
            actsBtn.UseVisualStyleBackColor = true;
            actsBtn.Click += actsConsBtn_Click;
            // 
            // consBtn
            // 
            consBtn.Location = new Point(710, 146);
            consBtn.Name = "consBtn";
            consBtn.Size = new Size(74, 22);
            consBtn.TabIndex = 22;
            consBtn.Text = "Enter";
            consBtn.UseVisualStyleBackColor = true;
            consBtn.Click += consBtn_Click;
            // 
            // inputConsLbl
            // 
            inputConsLbl.AutoSize = true;
            inputConsLbl.Location = new Point(710, 69);
            inputConsLbl.Name = "inputConsLbl";
            inputConsLbl.Size = new Size(155, 15);
            inputConsLbl.TabIndex = 23;
            inputConsLbl.Text = "please use <= >= before rhs";
            // 
            // dataGridViewCons
            // 
            dataGridViewCons.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCons.Location = new Point(262, 6);
            dataGridViewCons.Name = "dataGridViewCons";
            dataGridViewCons.Size = new Size(431, 155);
            dataGridViewCons.TabIndex = 24;
            // 
            // dataGridViewActs
            // 
            dataGridViewActs.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewActs.Location = new Point(262, 6);
            dataGridViewActs.Name = "dataGridViewActs";
            dataGridViewActs.Size = new Size(431, 155);
            dataGridViewActs.TabIndex = 25;
            // 
            // doDualityBtn
            // 
            doDualityBtn.Location = new Point(29, 356);
            doDualityBtn.Name = "doDualityBtn";
            doDualityBtn.Size = new Size(179, 36);
            doDualityBtn.TabIndex = 26;
            doDualityBtn.Text = "Do Duality";
            doDualityBtn.UseVisualStyleBackColor = true;
            doDualityBtn.Click += doDuality_Click;
            // 
            // shadowPriceBtn
            // 
            shadowPriceBtn.Location = new Point(29, 398);
            shadowPriceBtn.Name = "shadowPriceBtn";
            shadowPriceBtn.Size = new Size(179, 36);
            shadowPriceBtn.TabIndex = 27;
            shadowPriceBtn.Text = "Show Shadow Price";
            shadowPriceBtn.UseVisualStyleBackColor = true;
            shadowPriceBtn.Click += shadowPriceBtn_Click;
            // 
            // Analysis
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(925, 522);
            Controls.Add(shadowPriceBtn);
            Controls.Add(doDualityBtn);
            Controls.Add(dataGridViewActs);
            Controls.Add(dataGridViewCons);
            Controls.Add(inputConsLbl);
            Controls.Add(consBtn);
            Controls.Add(actsBtn);
            Controls.Add(addConstraintBtn);
            Controls.Add(addActivityBtn);
            Controls.Add(InputRowLbl);
            Controls.Add(inputRow);
            Controls.Add(inputValuelbl);
            Controls.Add(inputChoicelbl);
            Controls.Add(valueInput);
            Controls.Add(sendInput);
            Controls.Add(outputBox);
            Controls.Add(inputBox);
            Controls.Add(HandleNonBasicColumnChangeBtn);
            Controls.Add(HandleNonBasicColumnRangeBtn);
            Controls.Add(HandleRHSChangeBtn);
            Controls.Add(HandleRHSRangeBtn);
            Controls.Add(HandleBasicVariableChangeBtn);
            Controls.Add(HandleBasicVariableRangeBtn);
            Controls.Add(HandleNonBasicVariableChangeBtn);
            Controls.Add(HandleNonBasicVariableRangeBtn);
            Name = "Analysis";
            Text = "Analysis";
            Load += Analysis_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridViewCons).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewActs).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button HandleNonBasicVariableRangeBtn;
        private Button HandleNonBasicVariableChangeBtn;
        private Button HandleBasicVariableRangeBtn;
        private Button HandleBasicVariableChangeBtn;
        private Button HandleRHSRangeBtn;
        private Button HandleRHSChangeBtn;
        private Button HandleNonBasicColumnRangeBtn;
        private Button HandleNonBasicColumnChangeBtn;
        private TextBox inputBox;
        private RichTextBox outputBox;
        private Button sendInput;
        private TextBox valueInput;
        private Label inputChoicelbl;
        private Label inputValuelbl;
        private TextBox inputRow;
        private Label InputRowLbl;
        private Button addActivityBtn;
        private Button addConstraintBtn;
        private Button actsBtn;
        private Button consBtn;
        private Label inputConsLbl;
        private DataGridView dataGridViewCons;
        private DataGridView dataGridViewActs;
        private Button doDualityBtn;
        private Button shadowPriceBtn;
    }
}