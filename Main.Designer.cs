namespace lpr381Project
{
    partial class Main
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
            mainTextDisplay = new RichTextBox();
            mainInputBox = new RichTextBox();
            enterInput = new Button();
            primalSimplexBtn = new Button();
            revisedPrimalBtn = new Button();
            branchAndBoundBtn = new Button();
            button1 = new Button();
            cuttingPlaneBtn = new Button();
            sensitivityAnalysisBtn = new Button();
            JesusLovesYou = new Label();
            SuspendLayout();
            // 
            // mainTextDisplay
            // 
            mainTextDisplay.Location = new Point(28, 297);
            mainTextDisplay.Name = "mainTextDisplay";
            mainTextDisplay.ReadOnly = true;
            mainTextDisplay.Size = new Size(1102, 260);
            mainTextDisplay.TabIndex = 0;
            mainTextDisplay.Text = "";
            mainTextDisplay.WordWrap = false;
            // 
            // mainInputBox
            // 
            mainInputBox.Location = new Point(33, 12);
            mainInputBox.Name = "mainInputBox";
            mainInputBox.Size = new Size(619, 131);
            mainInputBox.TabIndex = 1;
            mainInputBox.Text = "";
            // 
            // enterInput
            // 
            enterInput.Location = new Point(38, 153);
            enterInput.Name = "enterInput";
            enterInput.Size = new Size(125, 33);
            enterInput.TabIndex = 2;
            enterInput.Text = "Enter";
            enterInput.UseVisualStyleBackColor = true;
            enterInput.Click += enterInput_Click;
            // 
            // primalSimplexBtn
            // 
            primalSimplexBtn.Location = new Point(38, 225);
            primalSimplexBtn.Name = "primalSimplexBtn";
            primalSimplexBtn.Size = new Size(165, 33);
            primalSimplexBtn.TabIndex = 3;
            primalSimplexBtn.Text = "Primal Simplex";
            primalSimplexBtn.UseVisualStyleBackColor = true;
            primalSimplexBtn.Click += primalSimplexBtn_Click;
            // 
            // revisedPrimalBtn
            // 
            revisedPrimalBtn.Location = new Point(209, 225);
            revisedPrimalBtn.Name = "revisedPrimalBtn";
            revisedPrimalBtn.Size = new Size(152, 33);
            revisedPrimalBtn.TabIndex = 4;
            revisedPrimalBtn.Text = "Revised Primal Simplex";
            revisedPrimalBtn.UseVisualStyleBackColor = true;
            revisedPrimalBtn.Click += revisedPrimal_Click;
            // 
            // branchAndBoundBtn
            // 
            branchAndBoundBtn.Location = new Point(367, 225);
            branchAndBoundBtn.Name = "branchAndBoundBtn";
            branchAndBoundBtn.Size = new Size(170, 33);
            branchAndBoundBtn.TabIndex = 5;
            branchAndBoundBtn.Text = "Branch And Bound Simplex";
            branchAndBoundBtn.UseVisualStyleBackColor = true;
            branchAndBoundBtn.Click += branchAndBoundBtn_Click;
            // 
            // button1
            // 
            button1.Location = new Point(543, 225);
            button1.Name = "button1";
            button1.Size = new Size(170, 33);
            button1.TabIndex = 6;
            button1.Text = "Branch And Bound KnapSack";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // cuttingPlaneBtn
            // 
            cuttingPlaneBtn.Location = new Point(719, 225);
            cuttingPlaneBtn.Name = "cuttingPlaneBtn";
            cuttingPlaneBtn.Size = new Size(170, 33);
            cuttingPlaneBtn.TabIndex = 7;
            cuttingPlaneBtn.Text = "Cutting Plane";
            cuttingPlaneBtn.UseVisualStyleBackColor = true;
            cuttingPlaneBtn.Click += cuttingPlaneBtn_Click;
            // 
            // sensitivityAnalysisBtn
            // 
            sensitivityAnalysisBtn.Location = new Point(895, 225);
            sensitivityAnalysisBtn.Name = "sensitivityAnalysisBtn";
            sensitivityAnalysisBtn.Size = new Size(170, 33);
            sensitivityAnalysisBtn.TabIndex = 8;
            sensitivityAnalysisBtn.Text = "Sensitivity Analysis";
            sensitivityAnalysisBtn.UseVisualStyleBackColor = true;
            sensitivityAnalysisBtn.Click += sensitivityAnalysisBtn_Click;
            // 
            // JesusLovesYou
            // 
            JesusLovesYou.AutoSize = true;
            JesusLovesYou.Font = new Font("Segoe UI", 14F);
            JesusLovesYou.Location = new Point(658, 12);
            JesusLovesYou.Name = "JesusLovesYou";
            JesusLovesYou.Size = new Size(237, 125);
            JesusLovesYou.TabIndex = 9;
            JesusLovesYou.Text = "LPR 381 Project By: \r\nJanita de Vries: 577698 \r\nChristian Olivier: 576753\r\nTiaan Wessels: 600164\r\nBrett David Wilson: 601081";
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1157, 583);
            Controls.Add(JesusLovesYou);
            Controls.Add(sensitivityAnalysisBtn);
            Controls.Add(cuttingPlaneBtn);
            Controls.Add(button1);
            Controls.Add(branchAndBoundBtn);
            Controls.Add(revisedPrimalBtn);
            Controls.Add(primalSimplexBtn);
            Controls.Add(enterInput);
            Controls.Add(mainInputBox);
            Controls.Add(mainTextDisplay);
            Name = "Main";
            Text = "Main";
            Load += Main_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox mainTextDisplay;
        private RichTextBox mainInputBox;
        private Button enterInput;
        private Button primalSimplexBtn;
        private Button revisedPrimalBtn;
        private Button branchAndBoundBtn;
        private Button button1;
        private Button cuttingPlaneBtn;
        private Button sensitivityAnalysisBtn;
        private Label JesusLovesYou;
    }
}