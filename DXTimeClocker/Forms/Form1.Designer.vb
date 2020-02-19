Imports DevExpress.XtraEditors

Partial Public Class s
    Inherits DevExpress.XtraEditors.XtraForm

    ''' <summary>
    ''' Required designer variable.
    ''' </summary>
    Private components As System.ComponentModel.IContainer = Nothing

    ''' <summary>
    ''' Clean up any resources being used.
    ''' </summary>
    ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso (components IsNot Nothing) Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

#Region "Windows Form Designer generated code"

    ''' <summary>
    ''' Required method for Designer support - do not modify
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(s))
        Me.TxtFilePath = New System.Windows.Forms.TextBox()
        Me.BtnFndFile = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnLdFile = New DevExpress.XtraEditors.SimpleButton()
        Me.PBLoad = New DevExpress.XtraEditors.ProgressBarControl()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TxtRecCnt = New System.Windows.Forms.TextBox()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        CType(Me.PBLoad.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TxtFilePath
        '
        Me.TxtFilePath.Location = New System.Drawing.Point(106, 81)
        Me.TxtFilePath.Name = "TxtFilePath"
        Me.TxtFilePath.Size = New System.Drawing.Size(355, 21)
        Me.TxtFilePath.TabIndex = 0
        '
        'BtnFndFile
        '
        Me.BtnFndFile.Location = New System.Drawing.Point(25, 81)
        Me.BtnFndFile.Name = "BtnFndFile"
        Me.BtnFndFile.Size = New System.Drawing.Size(75, 23)
        Me.BtnFndFile.TabIndex = 1
        Me.BtnFndFile.Text = "File"
        '
        'BtnLdFile
        '
        Me.BtnLdFile.Enabled = False
        Me.BtnLdFile.Location = New System.Drawing.Point(25, 129)
        Me.BtnLdFile.Name = "BtnLdFile"
        Me.BtnLdFile.Size = New System.Drawing.Size(75, 23)
        Me.BtnLdFile.TabIndex = 2
        Me.BtnLdFile.Text = "Load"
        '
        'PBLoad
        '
        Me.PBLoad.Location = New System.Drawing.Point(106, 134)
        Me.PBLoad.Name = "PBLoad"
        Me.PBLoad.Properties.ShowTitle = True
        Me.PBLoad.Properties.Step = 1
        Me.PBLoad.Size = New System.Drawing.Size(277, 18)
        Me.PBLoad.TabIndex = 3
        '
        'Timer1
        '
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold)
        Me.Label1.Location = New System.Drawing.Point(399, 114)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(62, 13)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "#Records"
        '
        'TxtRecCnt
        '
        Me.TxtRecCnt.BackColor = System.Drawing.Color.White
        Me.TxtRecCnt.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold)
        Me.TxtRecCnt.ForeColor = System.Drawing.Color.DarkOliveGreen
        Me.TxtRecCnt.Location = New System.Drawing.Point(389, 131)
        Me.TxtRecCnt.Name = "TxtRecCnt"
        Me.TxtRecCnt.ReadOnly = True
        Me.TxtRecCnt.Size = New System.Drawing.Size(72, 21)
        Me.TxtRecCnt.TabIndex = 5
        Me.TxtRecCnt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = Global.DXTimeClocker.My.Resources.Resources.logo
        Me.PictureBox1.Location = New System.Drawing.Point(25, 13)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(436, 62)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox1.TabIndex = 6
        Me.PictureBox1.TabStop = False
        '
        's
        '
        Me.Appearance.BackColor = System.Drawing.Color.DarkSeaGreen
        Me.Appearance.Options.UseBackColor = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(489, 161)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.TxtRecCnt)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.PBLoad)
        Me.Controls.Add(Me.BtnLdFile)
        Me.Controls.Add(Me.BtnFndFile)
        Me.Controls.Add(Me.TxtFilePath)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Dark Side"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.Name = "s"
        Me.Text = "Time Clock Loader"
        CType(Me.PBLoad.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents TxtFilePath As TextBox
    Friend WithEvents BtnFndFile As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnLdFile As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents PBLoad As ProgressBarControl
    Friend WithEvents Timer1 As Timer
    Friend WithEvents Label1 As Label
    Friend WithEvents TxtRecCnt As TextBox
    Friend WithEvents PictureBox1 As PictureBox

#End Region

End Class
