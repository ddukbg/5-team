namespace 네이버_서이추_안부글_쪽지
{
    partial class LoginForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtID = new System.Windows.Forms.TextBox();
            this.txtPW = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnLogin = new System.Windows.Forms.Button();
            this.btnClearNID = new System.Windows.Forms.Button();
            this.btnClearNPW = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "아이디 :";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtID
            // 
            this.txtID.Location = new System.Drawing.Point(97, 33);
            this.txtID.Name = "txtID";
            this.txtID.Size = new System.Drawing.Size(198, 21);
            this.txtID.TabIndex = 1;
            this.txtID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtPW
            // 
            this.txtPW.Location = new System.Drawing.Point(97, 60);
            this.txtPW.Name = "txtPW";
            this.txtPW.PasswordChar = '*';
            this.txtPW.Size = new System.Drawing.Size(198, 21);
            this.txtPW.TabIndex = 3;
            this.txtPW.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtPW.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPW_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "비밀번호 :";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(97, 87);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(198, 28);
            this.btnLogin.TabIndex = 4;
            this.btnLogin.Text = "로그인";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // btnClearNID
            // 
            this.btnClearNID.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnClearNID.Location = new System.Drawing.Point(278, 33);
            this.btnClearNID.Name = "btnClearNID";
            this.btnClearNID.Size = new System.Drawing.Size(17, 21);
            this.btnClearNID.TabIndex = 5;
            this.btnClearNID.Text = "X";
            this.btnClearNID.UseVisualStyleBackColor = true;
            this.btnClearNID.Click += new System.EventHandler(this.btnClearNID_Click);
            // 
            // btnClearNPW
            // 
            this.btnClearNPW.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnClearNPW.Location = new System.Drawing.Point(278, 60);
            this.btnClearNPW.Name = "btnClearNPW";
            this.btnClearNPW.Size = new System.Drawing.Size(17, 21);
            this.btnClearNPW.TabIndex = 6;
            this.btnClearNPW.Text = "X";
            this.btnClearNPW.UseVisualStyleBackColor = true;
            this.btnClearNPW.Click += new System.EventHandler(this.btnClearNPW_Click);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(358, 148);
            this.Controls.Add(this.btnClearNPW);
            this.Controls.Add(this.btnClearNID);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.txtPW);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtID);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "네이버 로그인";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtID;
        private System.Windows.Forms.TextBox txtPW;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnClearNID;
        private System.Windows.Forms.Button btnClearNPW;
    }
}