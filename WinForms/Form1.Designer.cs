namespace WinForms
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            points = new Button();
            parameters = new Button();
            movement = new Button();
            clean = new Button();
            curve = new Button();
            polygone = new Button();
            beziers = new Button();
            fillCurve = new Button();
            track = new Button();
            SuspendLayout();
            // 
            // points
            // 
            points.Location = new Point(12, 12);
            points.Name = "points";
            points.Size = new Size(112, 30);
            points.TabIndex = 0;
            points.Text = "Точки";
            points.UseVisualStyleBackColor = true;
            // 
            // parameters
            // 
            parameters.AutoSize = true;
            parameters.Location = new Point(12, 48);
            parameters.Name = "parameters";
            parameters.Size = new Size(112, 30);
            parameters.TabIndex = 1;
            parameters.Text = "Параметры";
            parameters.UseVisualStyleBackColor = true;
            // 
            // movement
            // 
            movement.Location = new Point(12, 228);
            movement.Name = "movement";
            movement.Size = new Size(112, 30);
            movement.TabIndex = 2;
            movement.Text = "Движение";
            movement.UseVisualStyleBackColor = true;
            // 
            // clean
            // 
            clean.Location = new Point(12, 300);
            clean.Name = "clean";
            clean.Size = new Size(112, 30);
            clean.TabIndex = 3;
            clean.Text = "Очистить";
            clean.UseVisualStyleBackColor = true;
            // 
            // curve
            // 
            curve.Location = new Point(12, 84);
            curve.Name = "curve";
            curve.Size = new Size(112, 30);
            curve.TabIndex = 4;
            curve.Text = "Кривая";
            curve.UseVisualStyleBackColor = true;
            // 
            // polygone
            // 
            polygone.Location = new Point(12, 120);
            polygone.Name = "polygone";
            polygone.Size = new Size(112, 30);
            polygone.TabIndex = 5;
            polygone.Text = "Ломаная";
            polygone.UseVisualStyleBackColor = true;
            // 
            // beziers
            // 
            beziers.Location = new Point(12, 156);
            beziers.Name = "beziers";
            beziers.Size = new Size(112, 30);
            beziers.TabIndex = 6;
            beziers.Text = "Безье";
            beziers.UseVisualStyleBackColor = true;
            // 
            // fillCurve
            // 
            fillCurve.AutoSize = true;
            fillCurve.Location = new Point(12, 192);
            fillCurve.Name = "fillCurve";
            fillCurve.Size = new Size(112, 30);
            fillCurve.TabIndex = 7;
            fillCurve.Text = "Заполненная";
            fillCurve.UseVisualStyleBackColor = true;
            // 
            // track
            // 
            track.Location = new Point(12, 264);
            track.Name = "track";
            track.Size = new Size(112, 30);
            track.TabIndex = 8;
            track.Text = "След";
            track.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(track);
            Controls.Add(fillCurve);
            Controls.Add(beziers);
            Controls.Add(polygone);
            Controls.Add(curve);
            Controls.Add(clean);
            Controls.Add(movement);
            Controls.Add(parameters);
            Controls.Add(points);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button points;
        private Button parameters;
        private Button movement;
        private Button clean;
        private Button curve;
        private Button polygone;
        private Button beziers;
        private Button fillCurve;
        private Button track;
    }
}
