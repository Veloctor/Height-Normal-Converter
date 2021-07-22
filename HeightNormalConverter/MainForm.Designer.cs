namespace HeightNormalConverter
{
	// Token: 0x02000002 RID: 2
	public partial class MainForm : global::System.Windows.Forms.Form
	{
		// Token: 0x0600000B RID: 11 RVA: 0x000025F8 File Offset: 0x000007F8
		protected override void Dispose(bool disposing)
		{
			bool flag = disposing && this.components != null;
			if (flag)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002630 File Offset: 0x00000830
		private void InitializeComponent()
		{
            this.normalMapBox = new System.Windows.Forms.PictureBox();
            this.heightMapBox = new System.Windows.Forms.PictureBox();
            this.convertButton = new System.Windows.Forms.Button();
            this.normalNameLabel = new System.Windows.Forms.Label();
            this.heightNameLabel = new System.Windows.Forms.Label();
            this.normalMapBoxLabel = new System.Windows.Forms.Label();
            this.heightMapBoxLabel = new System.Windows.Forms.Label();
            this.convertModeBox = new System.Windows.Forms.GroupBox();
            this.convertModeH2N = new System.Windows.Forms.RadioButton();
            this.convertModeN2H = new System.Windows.Forms.RadioButton();
            this.NormalGChannelModeBox = new System.Windows.Forms.GroupBox();
            this.NormalGChannelModeDX = new System.Windows.Forms.RadioButton();
            this.NormalGChannelModeGL = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textureHeightMax = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textureHeightMin = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.realHeightMax = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.realHeightMin = new System.Windows.Forms.TextBox();
            this.GradientChannel = new System.Windows.Forms.GroupBox();
            this.ChannelModeGradient = new System.Windows.Forms.RadioButton();
            this.ChannelModeA = new System.Windows.Forms.RadioButton();
            this.ChannelModeB = new System.Windows.Forms.RadioButton();
            this.ChannelModeG = new System.Windows.Forms.RadioButton();
            this.ChannelModeR = new System.Windows.Forms.RadioButton();
            this.InfoBox = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.normalMapBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.heightMapBox)).BeginInit();
            this.convertModeBox.SuspendLayout();
            this.NormalGChannelModeBox.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.GradientChannel.SuspendLayout();
            this.SuspendLayout();
            // 
            // normalMapBox
            // 
            this.normalMapBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.normalMapBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.normalMapBox.Enabled = false;
            this.normalMapBox.ImageLocation = "";
            this.normalMapBox.Location = new System.Drawing.Point(8, 22);
            this.normalMapBox.Margin = new System.Windows.Forms.Padding(2);
            this.normalMapBox.Name = "normalMapBox";
            this.normalMapBox.Size = new System.Drawing.Size(193, 206);
            this.normalMapBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.normalMapBox.TabIndex = 0;
            this.normalMapBox.TabStop = false;
            this.normalMapBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.NormalBox_DragDrop);
            this.normalMapBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.NormalBox_DragEnter);
            // 
            // heightMapBox
            // 
            this.heightMapBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.heightMapBox.Location = new System.Drawing.Point(220, 22);
            this.heightMapBox.Margin = new System.Windows.Forms.Padding(2);
            this.heightMapBox.Name = "heightMapBox";
            this.heightMapBox.Size = new System.Drawing.Size(193, 206);
            this.heightMapBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.heightMapBox.TabIndex = 1;
            this.heightMapBox.TabStop = false;
            this.heightMapBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.HeightBox_DragDrop);
            this.heightMapBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.HeightBox_DragEnter);
            // 
            // convertButton
            // 
            this.convertButton.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.convertButton.Location = new System.Drawing.Point(582, 244);
            this.convertButton.Margin = new System.Windows.Forms.Padding(2);
            this.convertButton.Name = "convertButton";
            this.convertButton.Size = new System.Drawing.Size(139, 50);
            this.convertButton.TabIndex = 2;
            this.convertButton.Text = "转换";
            this.convertButton.UseVisualStyleBackColor = true;
            this.convertButton.Click += new System.EventHandler(this.ConvertButton_Click);
            // 
            // normalNameLabel
            // 
            this.normalNameLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.normalNameLabel.Enabled = false;
            this.normalNameLabel.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.normalNameLabel.Location = new System.Drawing.Point(8, 229);
            this.normalNameLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.normalNameLabel.Name = "normalNameLabel";
            this.normalNameLabel.Size = new System.Drawing.Size(192, 65);
            this.normalNameLabel.TabIndex = 3;
            // 
            // heightNameLabel
            // 
            this.heightNameLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.heightNameLabel.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.heightNameLabel.Location = new System.Drawing.Point(220, 229);
            this.heightNameLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.heightNameLabel.Name = "heightNameLabel";
            this.heightNameLabel.Size = new System.Drawing.Size(192, 65);
            this.heightNameLabel.TabIndex = 4;
            // 
            // normalMapBoxLabel
            // 
            this.normalMapBoxLabel.AutoSize = true;
            this.normalMapBoxLabel.Enabled = false;
            this.normalMapBoxLabel.Font = new System.Drawing.Font("微软雅黑", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.normalMapBoxLabel.Location = new System.Drawing.Point(68, 1);
            this.normalMapBoxLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.normalMapBoxLabel.Name = "normalMapBoxLabel";
            this.normalMapBoxLabel.Size = new System.Drawing.Size(69, 20);
            this.normalMapBoxLabel.TabIndex = 10;
            this.normalMapBoxLabel.Text = "法线贴图";
            // 
            // heightMapBoxLabel
            // 
            this.heightMapBoxLabel.AutoSize = true;
            this.heightMapBoxLabel.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.heightMapBoxLabel.Location = new System.Drawing.Point(294, 0);
            this.heightMapBoxLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.heightMapBoxLabel.Name = "heightMapBoxLabel";
            this.heightMapBoxLabel.Size = new System.Drawing.Size(54, 20);
            this.heightMapBoxLabel.TabIndex = 11;
            this.heightMapBoxLabel.Text = "高程图";
            // 
            // convertModeBox
            // 
            this.convertModeBox.Controls.Add(this.convertModeH2N);
            this.convertModeBox.Controls.Add(this.convertModeN2H);
            this.convertModeBox.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.convertModeBox.Location = new System.Drawing.Point(421, 11);
            this.convertModeBox.Margin = new System.Windows.Forms.Padding(2);
            this.convertModeBox.Name = "convertModeBox";
            this.convertModeBox.Padding = new System.Windows.Forms.Padding(2);
            this.convertModeBox.Size = new System.Drawing.Size(300, 54);
            this.convertModeBox.TabIndex = 12;
            this.convertModeBox.TabStop = false;
            this.convertModeBox.Text = "转换模式";
            // 
            // convertModeH2N
            // 
            this.convertModeH2N.AutoSize = true;
            this.convertModeH2N.Checked = true;
            this.convertModeH2N.Location = new System.Drawing.Point(152, 19);
            this.convertModeH2N.Margin = new System.Windows.Forms.Padding(2);
            this.convertModeH2N.Name = "convertModeH2N";
            this.convertModeH2N.Size = new System.Drawing.Size(147, 24);
            this.convertModeH2N.TabIndex = 1;
            this.convertModeH2N.TabStop = true;
            this.convertModeH2N.Text = "高程图到法线贴图";
            this.convertModeH2N.UseVisualStyleBackColor = true;
            this.convertModeH2N.CheckedChanged += new System.EventHandler(this.ConvertModeH2N_CheckedChanged);
            // 
            // convertModeN2H
            // 
            this.convertModeN2H.AutoSize = true;
            this.convertModeN2H.Location = new System.Drawing.Point(4, 19);
            this.convertModeN2H.Margin = new System.Windows.Forms.Padding(2);
            this.convertModeN2H.Name = "convertModeN2H";
            this.convertModeN2H.Size = new System.Drawing.Size(147, 24);
            this.convertModeN2H.TabIndex = 0;
            this.convertModeN2H.Text = "法线贴图到高程图";
            this.convertModeN2H.UseVisualStyleBackColor = true;
            // 
            // NormalGChannelModeBox
            // 
            this.NormalGChannelModeBox.Controls.Add(this.NormalGChannelModeDX);
            this.NormalGChannelModeBox.Controls.Add(this.NormalGChannelModeGL);
            this.NormalGChannelModeBox.Font = new System.Drawing.Font("微软雅黑", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.NormalGChannelModeBox.Location = new System.Drawing.Point(421, 69);
            this.NormalGChannelModeBox.Margin = new System.Windows.Forms.Padding(2);
            this.NormalGChannelModeBox.Name = "NormalGChannelModeBox";
            this.NormalGChannelModeBox.Padding = new System.Windows.Forms.Padding(2);
            this.NormalGChannelModeBox.Size = new System.Drawing.Size(300, 54);
            this.NormalGChannelModeBox.TabIndex = 14;
            this.NormalGChannelModeBox.TabStop = false;
            this.NormalGChannelModeBox.Text = "法线贴图G通道格式";
            // 
            // NormalGChannelModeDX
            // 
            this.NormalGChannelModeDX.AutoSize = true;
            this.NormalGChannelModeDX.Location = new System.Drawing.Point(155, 19);
            this.NormalGChannelModeDX.Margin = new System.Windows.Forms.Padding(2);
            this.NormalGChannelModeDX.Name = "NormalGChannelModeDX";
            this.NormalGChannelModeDX.Size = new System.Drawing.Size(81, 24);
            this.NormalGChannelModeDX.TabIndex = 1;
            this.NormalGChannelModeDX.Text = "DirectX";
            this.NormalGChannelModeDX.UseVisualStyleBackColor = true;
            // 
            // NormalGChannelModeGL
            // 
            this.NormalGChannelModeGL.AutoSize = true;
            this.NormalGChannelModeGL.Checked = true;
            this.NormalGChannelModeGL.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.NormalGChannelModeGL.Location = new System.Drawing.Point(4, 19);
            this.NormalGChannelModeGL.Margin = new System.Windows.Forms.Padding(2);
            this.NormalGChannelModeGL.Name = "NormalGChannelModeGL";
            this.NormalGChannelModeGL.Size = new System.Drawing.Size(86, 24);
            this.NormalGChannelModeGL.TabIndex = 0;
            this.NormalGChannelModeGL.TabStop = true;
            this.NormalGChannelModeGL.Text = "OpenGL";
            this.NormalGChannelModeGL.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textureHeightMax);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.textureHeightMin);
            this.groupBox3.Font = new System.Drawing.Font("微软雅黑", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox3.Location = new System.Drawing.Point(421, 127);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox3.Size = new System.Drawing.Size(146, 55);
            this.groupBox3.TabIndex = 15;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "高程图储存值范围";
            // 
            // textureHeightMax
            // 
            this.textureHeightMax.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.textureHeightMax.Location = new System.Drawing.Point(91, 22);
            this.textureHeightMax.MaxLength = 4;
            this.textureHeightMax.Name = "textureHeightMax";
            this.textureHeightMax.Size = new System.Drawing.Size(50, 27);
            this.textureHeightMax.TabIndex = 3;
            this.textureHeightMax.Text = "255";
            this.textureHeightMax.TextChanged += new System.EventHandler(this.textureHeightMax_TextChanged);
            this.textureHeightMax.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(61, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(24, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "到";
            // 
            // textureHeightMin
            // 
            this.textureHeightMin.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.textureHeightMin.Location = new System.Drawing.Point(5, 22);
            this.textureHeightMin.MaxLength = 4;
            this.textureHeightMin.Name = "textureHeightMin";
            this.textureHeightMin.Size = new System.Drawing.Size(50, 27);
            this.textureHeightMin.TabIndex = 0;
            this.textureHeightMin.Text = "0";
            this.textureHeightMin.TextChanged += new System.EventHandler(this.textureHeightMin_TextChanged);
            this.textureHeightMin.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox_KeyPress);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.realHeightMax);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.realHeightMin);
            this.groupBox4.Font = new System.Drawing.Font("微软雅黑", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox4.Location = new System.Drawing.Point(575, 127);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox4.Size = new System.Drawing.Size(146, 55);
            this.groupBox4.TabIndex = 17;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "高程图真实值范围";
            // 
            // realHeightMax
            // 
            this.realHeightMax.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.realHeightMax.Location = new System.Drawing.Point(91, 22);
            this.realHeightMax.MaxLength = 4;
            this.realHeightMax.Name = "realHeightMax";
            this.realHeightMax.Size = new System.Drawing.Size(50, 27);
            this.realHeightMax.TabIndex = 3;
            this.realHeightMax.Text = "3";
            this.realHeightMax.TextChanged += new System.EventHandler(this.realHeightMax_TextChanged);
            this.realHeightMax.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(61, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(24, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "到";
            // 
            // realHeightMin
            // 
            this.realHeightMin.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.realHeightMin.Location = new System.Drawing.Point(5, 22);
            this.realHeightMin.MaxLength = 4;
            this.realHeightMin.Name = "realHeightMin";
            this.realHeightMin.Size = new System.Drawing.Size(50, 27);
            this.realHeightMin.TabIndex = 0;
            this.realHeightMin.Text = "0";
            this.realHeightMin.TextChanged += new System.EventHandler(this.realHeightMin_TextChanged);
            this.realHeightMin.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox_KeyPress);
            // 
            // GradientChannel
            // 
            this.GradientChannel.Controls.Add(this.ChannelModeGradient);
            this.GradientChannel.Controls.Add(this.ChannelModeA);
            this.GradientChannel.Controls.Add(this.ChannelModeB);
            this.GradientChannel.Controls.Add(this.ChannelModeG);
            this.GradientChannel.Controls.Add(this.ChannelModeR);
            this.GradientChannel.Font = new System.Drawing.Font("微软雅黑", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.GradientChannel.Location = new System.Drawing.Point(421, 186);
            this.GradientChannel.Margin = new System.Windows.Forms.Padding(2);
            this.GradientChannel.Name = "GradientChannel";
            this.GradientChannel.Padding = new System.Windows.Forms.Padding(2);
            this.GradientChannel.Size = new System.Drawing.Size(300, 54);
            this.GradientChannel.TabIndex = 18;
            this.GradientChannel.TabStop = false;
            this.GradientChannel.Text = "高程图通道";
            // 
            // ChannelModeGradient
            // 
            this.ChannelModeGradient.AutoSize = true;
            this.ChannelModeGradient.Checked = true;
            this.ChannelModeGradient.Location = new System.Drawing.Point(238, 24);
            this.ChannelModeGradient.Margin = new System.Windows.Forms.Padding(2);
            this.ChannelModeGradient.Name = "ChannelModeGradient";
            this.ChannelModeGradient.Size = new System.Drawing.Size(57, 24);
            this.ChannelModeGradient.TabIndex = 0;
            this.ChannelModeGradient.TabStop = true;
            this.ChannelModeGradient.Text = "灰度";
            this.ChannelModeGradient.UseVisualStyleBackColor = true;
            this.ChannelModeGradient.CheckedChanged += new System.EventHandler(this.ChannelModeGradient_CheckedChanged);
            // 
            // ChannelModeA
            // 
            this.ChannelModeA.AutoSize = true;
            this.ChannelModeA.Enabled = false;
            this.ChannelModeA.Location = new System.Drawing.Point(180, 24);
            this.ChannelModeA.Margin = new System.Windows.Forms.Padding(2);
            this.ChannelModeA.Name = "ChannelModeA";
            this.ChannelModeA.Size = new System.Drawing.Size(38, 24);
            this.ChannelModeA.TabIndex = 4;
            this.ChannelModeA.Text = "A";
            this.ChannelModeA.UseVisualStyleBackColor = true;
            this.ChannelModeA.CheckedChanged += new System.EventHandler(this.ChannelModeA_CheckedChanged);
            // 
            // ChannelModeB
            // 
            this.ChannelModeB.AutoSize = true;
            this.ChannelModeB.Location = new System.Drawing.Point(124, 24);
            this.ChannelModeB.Margin = new System.Windows.Forms.Padding(2);
            this.ChannelModeB.Name = "ChannelModeB";
            this.ChannelModeB.Size = new System.Drawing.Size(36, 24);
            this.ChannelModeB.TabIndex = 3;
            this.ChannelModeB.Text = "B";
            this.ChannelModeB.UseVisualStyleBackColor = true;
            this.ChannelModeB.CheckedChanged += new System.EventHandler(this.ChannelModeB_CheckedChanged);
            // 
            // ChannelModeG
            // 
            this.ChannelModeG.AutoSize = true;
            this.ChannelModeG.Location = new System.Drawing.Point(65, 24);
            this.ChannelModeG.Margin = new System.Windows.Forms.Padding(2);
            this.ChannelModeG.Name = "ChannelModeG";
            this.ChannelModeG.Size = new System.Drawing.Size(38, 24);
            this.ChannelModeG.TabIndex = 2;
            this.ChannelModeG.Text = "G";
            this.ChannelModeG.UseVisualStyleBackColor = true;
            this.ChannelModeG.CheckedChanged += new System.EventHandler(this.ChannelModeG_CheckedChanged);
            // 
            // ChannelModeR
            // 
            this.ChannelModeR.AutoSize = true;
            this.ChannelModeR.Location = new System.Drawing.Point(4, 24);
            this.ChannelModeR.Margin = new System.Windows.Forms.Padding(2);
            this.ChannelModeR.Name = "ChannelModeR";
            this.ChannelModeR.Size = new System.Drawing.Size(37, 24);
            this.ChannelModeR.TabIndex = 1;
            this.ChannelModeR.Text = "R";
            this.ChannelModeR.UseVisualStyleBackColor = true;
            this.ChannelModeR.CheckedChanged += new System.EventHandler(this.ChannelModeR_CheckedChanged);
            // 
            // InfoBox
            // 
            this.InfoBox.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.InfoBox.Location = new System.Drawing.Point(421, 244);
            this.InfoBox.Name = "InfoBox";
            this.InfoBox.Size = new System.Drawing.Size(156, 51);
            this.InfoBox.TabIndex = 19;
            this.InfoBox.Text = "提示:不理解的参数可以保持默认,用法可参考作者的B站专栏\r\n";
            this.InfoBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(730, 302);
            this.Controls.Add(this.InfoBox);
            this.Controls.Add(this.GradientChannel);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.NormalGChannelModeBox);
            this.Controls.Add(this.convertModeBox);
            this.Controls.Add(this.heightMapBoxLabel);
            this.Controls.Add(this.normalMapBoxLabel);
            this.Controls.Add(this.heightNameLabel);
            this.Controls.Add(this.normalNameLabel);
            this.Controls.Add(this.convertButton);
            this.Controls.Add(this.heightMapBox);
            this.Controls.Add(this.normalMapBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "法线-高程图转换工具 v0.2 by B站@矢速Velctor";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.normalMapBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.heightMapBox)).EndInit();
            this.convertModeBox.ResumeLayout(false);
            this.convertModeBox.PerformLayout();
            this.NormalGChannelModeBox.ResumeLayout(false);
            this.NormalGChannelModeBox.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.GradientChannel.ResumeLayout(false);
            this.GradientChannel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		// Token: 0x04000006 RID: 6
		private global::System.ComponentModel.IContainer components = null;

		// Token: 0x04000007 RID: 7
		private global::System.Windows.Forms.PictureBox normalMapBox;

		// Token: 0x04000008 RID: 8
		private global::System.Windows.Forms.PictureBox heightMapBox;

		// Token: 0x04000009 RID: 9
		private global::System.Windows.Forms.Button convertButton;

		// Token: 0x0400000A RID: 10
		private global::System.Windows.Forms.Label normalNameLabel;

		// Token: 0x0400000B RID: 11
		private global::System.Windows.Forms.Label heightNameLabel;

		// Token: 0x0400000C RID: 12
		private global::System.Windows.Forms.Label normalMapBoxLabel;

		// Token: 0x0400000D RID: 13
		private global::System.Windows.Forms.Label heightMapBoxLabel;

		// Token: 0x0400000E RID: 14
		private global::System.Windows.Forms.GroupBox convertModeBox;

		// Token: 0x0400000F RID: 15
		private global::System.Windows.Forms.RadioButton convertModeH2N;

		// Token: 0x04000010 RID: 16
		private global::System.Windows.Forms.RadioButton convertModeN2H;
        private System.Windows.Forms.GroupBox NormalGChannelModeBox;
        private System.Windows.Forms.RadioButton NormalGChannelModeDX;
        private System.Windows.Forms.RadioButton NormalGChannelModeGL;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textureHeightMax;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textureHeightMin;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox realHeightMax;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox realHeightMin;
        private System.Windows.Forms.GroupBox GradientChannel;
        private System.Windows.Forms.RadioButton ChannelModeR;
        private System.Windows.Forms.RadioButton ChannelModeG;
        private System.Windows.Forms.RadioButton ChannelModeGradient;
        private System.Windows.Forms.RadioButton ChannelModeA;
        private System.Windows.Forms.RadioButton ChannelModeB;
        private System.Windows.Forms.Label InfoBox;
    }
}
